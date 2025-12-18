using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using BancoServiceRef;
using ClienteWeb.Models;
using ClienteWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

// Alias para evitar choques de nombres
using AmortizationCuotaModel = ClienteWeb.Models.AmortizationCuota;
using EvalResponseModel = ClienteWeb.Models.EvaluateCreditResponse;

namespace ClienteWeb.Controllers
{
    public class VentasController : Controller
    {
        private readonly BancoSoapService _banco;
        private const string CARRITO_SESSION_KEY = "Carrito";

        public VentasController(BancoSoapService banco)
        {
            _banco = banco;
        }

        // ====== 1. PANTALLA CÉDULA ======

        [HttpGet]
        public IActionResult Cedula()
        {
            return View(new VentaCedulaViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Cedula(VentaCedulaViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Cedula))
            {
                model.Error = "Por favor ingrese una cédula.";
                return View(model);
            }

            if (model.Cedula.Length != 10)
            {
                model.Error = "La cédula debe tener 10 dígitos.";
                return View(model);
            }

            var cliente = await ApiService.GetClienteByCedulaAsync(model.Cedula);
            if (cliente == null)
            {
                model.Error = "Cliente no encontrado. Registre el cliente primero.";
                return View(model);
            }

            return RedirectToAction("Productos", new { cedula = model.Cedula });
        }

        // ====== 2. PANTALLA PRODUCTOS ======

        [HttpGet]
        public async Task<IActionResult> Productos(string cedula)
        {
            if (string.IsNullOrWhiteSpace(cedula))
                return RedirectToAction("Cedula");

            var productos = await ApiService.GetProductosAsync();

            var vm = new VentaProductosViewModel
            {
                Cedula = cedula,
                Productos = productos
            };

            return View(vm);
        }

        // ====== 3A. Venta en EFECTIVO ======

        [HttpGet]
        public async Task<IActionResult> Efectivo(string cedula, int idProducto)
        {
            if (string.IsNullOrWhiteSpace(cedula))
                return RedirectToAction("Cedula");

            var producto = await ApiService.GetProductoAsync(idProducto);

            var subtotal = producto.PrecioVenta;
            var descuento = subtotal * 0.33m;
            var total = subtotal - descuento;

            var vm = new VentaEfectivoViewModel
            {
                Cedula = cedula,
                IdProducto = producto.IdProducto,
                NombreProducto = producto.Nombre,
                PrecioProducto = producto.PrecioVenta,
                Subtotal = subtotal,
                Descuento = descuento,
                Total = total
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Efectivo(VentaEfectivoViewModel model)
        {
            var factura = new CreateFacturaDto
            {
                Cedula = model.Cedula,
                FormaPago = "EFECTIVO",
                PlazoMeses = 0,
                IdCreditoBanco = null,
                Detalles = new List<DetalleFacturaDto>
                {
                    new DetalleFacturaDto
                    {
                        IdProducto = model.IdProducto,
                        Cantidad = 1
                    }
                }
            };

            var respuesta = await ApiService.CreateFacturaAsync(factura);

            var vm = new VentaEfectivoExitoViewModel
            {
                Factura = respuesta
            };

            return View("EfectivoExito", vm);
        }

        // ====== 3B. GET: Crédito ======

        [HttpGet]
        public async Task<IActionResult> Credito(string cedula, int idProducto)
        {
            if (string.IsNullOrWhiteSpace(cedula))
                return RedirectToAction("Cedula");

            var producto = await ApiService.GetProductoAsync(idProducto);

            // ESTA respuesta es la del banco (BancoServiceRef.EvaluateCreditResponse)
            var evaluacionBanco = await _banco.EvaluateCreditAsync(
                cedula,
                producto.PrecioVenta,
                12
            );

            // AQUÍ la copio a tu DTO (ClienteWeb.Models.EvaluateCreditResponse)
            var evaluacion = new EvalResponseModel
            {
                SujetoCredito = evaluacionBanco.SujetoCredito,
                MontoMaximo = evaluacionBanco.MontoMaximo,
                Aprobado = evaluacionBanco.Aprobado,
                IdCredito = evaluacionBanco.IdCredito,
                Mensaje = evaluacionBanco.Mensaje
            };

            var vm = new VentaCreditoViewModel
            {
                Cedula = cedula,
                IdProducto = producto.IdProducto,
                NombreProducto = producto.Nombre,
                PrecioProducto = producto.PrecioVenta,
                Evaluacion = evaluacion,
                IdCreditoBanco = evaluacion.IdCredito,
                PlazoMesesSeleccionado = 12
            };

            return View(vm);
        }

        // ====== 4. POST: Confirmar Crédito ======

        [HttpPost]
        public async Task<IActionResult> Credito(VentaCreditoViewModel model)
        {
            if (model.Evaluacion == null || !model.Evaluacion.Aprobado)
            {
                ModelState.AddModelError(string.Empty, "El crédito no está aprobado.");
                return View(model);
            }

            if (model.PlazoMesesSeleccionado <= 0)
            {
                ModelState.AddModelError(nameof(model.PlazoMesesSeleccionado), "Seleccione el número de cuotas.");
                return View(model);
            }

            var factura = new CreateFacturaDto
            {
                Cedula = model.Cedula,
                FormaPago = "CREDITO",
                PlazoMeses = model.PlazoMesesSeleccionado,
                IdCreditoBanco = model.IdCreditoBanco,
                Detalles = new List<DetalleFacturaDto>
                {
                    new DetalleFacturaDto
                    {
                        IdProducto = model.IdProducto,
                        Cantidad = 1
                    }
                }
            };

            var respuesta = await ApiService.CreateFacturaAsync(factura);

            // Tabla de amortización
            var cuotas = new List<AmortizationCuotaModel>();
            if (respuesta.IdCreditoBanco.HasValue)
            {
                var amortizacion = await _banco.GetAmortizationScheduleAsync(respuesta.IdCreditoBanco.Value);
                if (amortizacion.Cuotas != null)
                {
                    cuotas = amortizacion.Cuotas
                        .Select(c => new AmortizationCuotaModel
                        {
                            NumeroCuota = c.NumeroCuota,
                            FechaVencimiento = c.FechaVencimiento,
                            ValorCuota = c.ValorCuota,
                            InteresPagado = c.InteresPagado,
                            CapitalPagado = c.CapitalPagado,
                            SaldoRestante = c.SaldoRestante
                        })
                        .ToList();
                }
            }

            var vm = new VentaCreditoExitoViewModel
            {
                Factura = respuesta,
                Cuotas = cuotas
            };

            return View("CreditoExito", vm);
        }

        // ====== CARRITO DE COMPRAS ======

        [HttpGet]
        public async Task<IActionResult> Carrito(string cedula)
        {
            if (string.IsNullOrWhiteSpace(cedula))
                return RedirectToAction("Cedula");

            var productos = await ApiService.GetProductosAsync();
            var carrito = GetCarrito();

            var vm = new VentaCarritoViewModel
            {
                Cedula = cedula,
                Productos = productos,
                CarritoItems = carrito
            };

            return View(vm);
        }

        [HttpPost]
        public IActionResult AgregarAlCarrito(int idProducto, string nombre, decimal precio, int cantidad, string? imageUrl)
        {
            var carrito = GetCarrito();
            var itemExistente = carrito.FirstOrDefault(i => i.IdProducto == idProducto);

            if (itemExistente != null)
            {
                itemExistente.Cantidad += cantidad;
            }
            else
            {
                carrito.Add(new CarritoItem
                {
                    IdProducto = idProducto,
                    Nombre = nombre,
                    Precio = precio,
                    Cantidad = cantidad,
                    ImageUrl = imageUrl
                });
            }

            SaveCarrito(carrito);
            return Ok(new { success = true, itemsCount = carrito.Count, total = carrito.Sum(i => i.Subtotal) });
        }

        [HttpPost]
        public IActionResult EliminarDelCarrito(int idProducto)
        {
            var carrito = GetCarrito();
            var item = carrito.FirstOrDefault(i => i.IdProducto == idProducto);
            if (item != null)
            {
                carrito.Remove(item);
                SaveCarrito(carrito);
            }
            return Ok(new { success = true, itemsCount = carrito.Count, total = carrito.Sum(i => i.Subtotal) });
        }

        [HttpPost]
        public IActionResult ActualizarCantidad(int idProducto, int cantidad)
        {
            var carrito = GetCarrito();
            var item = carrito.FirstOrDefault(i => i.IdProducto == idProducto);
            if (item != null && cantidad > 0)
            {
                item.Cantidad = cantidad;
                SaveCarrito(carrito);
            }
            return Ok(new { success = true, subtotal = item?.Subtotal ?? 0, total = carrito.Sum(i => i.Subtotal) });
        }

        [HttpPost]
        public IActionResult LimpiarCarrito()
        {
            HttpContext.Session.Remove(CARRITO_SESSION_KEY);
            return Ok(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> PagarCarritoEfectivo(string cedula)
        {
            var carrito = GetCarrito();
            if (carrito.Count == 0)
            {
                return BadRequest("El carrito está vacío");
            }

            var factura = new CreateFacturaDto
            {
                Cedula = cedula,
                FormaPago = "EFECTIVO",
                PlazoMeses = 0,
                Detalles = carrito.Select(item => new DetalleFacturaDto
                {
                    IdProducto = item.IdProducto,
                    Cantidad = item.Cantidad
                }).ToList()
            };

            var respuesta = await ApiService.CreateFacturaAsync(factura);
            LimpiarCarrito();

            var vm = new VentaEfectivoExitoViewModel
            {
                Factura = respuesta
            };

            return View("EfectivoExito", vm);
        }

        [HttpPost]
        public async Task<IActionResult> PagarCarritoCredito(string cedula, int plazoMeses)
        {
            var carrito = GetCarrito();
            if (carrito.Count == 0)
            {
                return BadRequest("El carrito está vacío");
            }

            var total = carrito.Sum(i => i.Subtotal);

            // Evaluar crédito
            var evaluacionBanco = await _banco.EvaluateCreditAsync(cedula, total, plazoMeses);

            if (!evaluacionBanco.Aprobado)
            {
                return Json(new { success = false, mensaje = evaluacionBanco.Mensaje });
            }

            var factura = new CreateFacturaDto
            {
                Cedula = cedula,
                FormaPago = "CREDITO",
                PlazoMeses = plazoMeses,
                IdCreditoBanco = evaluacionBanco.IdCredito,
                Detalles = carrito.Select(item => new DetalleFacturaDto
                {
                    IdProducto = item.IdProducto,
                    Cantidad = item.Cantidad
                }).ToList()
            };

            var respuesta = await ApiService.CreateFacturaAsync(factura);
            LimpiarCarrito();

            // Tabla de amortización
            var cuotas = new List<AmortizationCuotaModel>();
            if (respuesta.IdCreditoBanco.HasValue)
            {
                var amortizacion = await _banco.GetAmortizationScheduleAsync(respuesta.IdCreditoBanco.Value);
                if (amortizacion.Cuotas != null)
                {
                    cuotas = amortizacion.Cuotas
                        .Select(c => new AmortizationCuotaModel
                        {
                            NumeroCuota = c.NumeroCuota,
                            FechaVencimiento = c.FechaVencimiento,
                            ValorCuota = c.ValorCuota,
                            InteresPagado = c.InteresPagado,
                            CapitalPagado = c.CapitalPagado,
                            SaldoRestante = c.SaldoRestante
                        })
                        .ToList();
                }
            }

            var vm = new VentaCreditoExitoViewModel
            {
                Factura = respuesta,
                Cuotas = cuotas
            };

            return View("CreditoExito", vm);
        }

        private List<CarritoItem> GetCarrito()
        {
            var carritoJson = HttpContext.Session.GetString(CARRITO_SESSION_KEY);
            if (string.IsNullOrEmpty(carritoJson))
            {
                return new List<CarritoItem>();
            }
            return JsonConvert.DeserializeObject<List<CarritoItem>>(carritoJson) ?? new List<CarritoItem>();
        }

        private void SaveCarrito(List<CarritoItem> carrito)
        {
            var carritoJson = JsonConvert.SerializeObject(carrito);
            HttpContext.Session.SetString(CARRITO_SESSION_KEY, carritoJson);
        }
    }
}
