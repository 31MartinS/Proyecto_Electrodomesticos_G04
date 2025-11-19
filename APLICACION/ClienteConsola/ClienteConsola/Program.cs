using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClienteConsola.Models; // Asegúrate que este using coincida con el namespace de tus modelos
using ClienteConsola.Services; // Asegúrate que este using coincida con el namespace de tus servicios

namespace ClienteConsola
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Sistema de Login
            if (!AutenticarUsuario())
            {
                Console.WriteLine("Acceso denegado. El programa se cerrará.");
                return;
            }

            // 1. Configuración Inicial de URLs (Igual que en tu App.xaml.cs)
            ApiService.BaseUrl = "http://10.40.20.89:5001/api";
            // BancoSoapService.ServiceUrl = "http://10.40.20.89:5000/BancoService.asmx";

            bool continuar = true;

            while (continuar)
            {
                Console.Clear();
                Console.WriteLine("=== SISTEMA COMERCIALIZADORA (CONSOLA) ===");
                Console.WriteLine("1. Ver Productos");
                Console.WriteLine("2. Nueva Venta (Facturación)");
                Console.WriteLine("3.  Salir");
                Console.Write("Seleccione una opción: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        await ListarProductos();
                        break;
                    case "2":
                        await ProcesoVenta();
                        break;
                    case "3":
                        continuar = false;
                        break;
                    default:
                        Console.WriteLine("Opción no válida.");
                        break;
                }

                if (continuar)
                {
                    Console.WriteLine("\nPresione cualquier tecla para volver al menú...");
                    Console.ReadKey();
                }
            }
        }

        // --- MÓDULO DE PRODUCTOS ---
        static async Task ListarProductos()
        {
            Console.WriteLine("\n--- LISTA DE PRODUCTOS ---");
            try
            {
                var productos = await ApiService.GetProductosAsync();
                foreach (var p in productos)
                {
                    Console.WriteLine($"ID: {p.IdProducto} | Código: {p.Codigo} | {p.Nombre} | ${p.PrecioVenta}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener productos: {ex.Message}");
            }
        }

        // --- MÓDULO DE CLIENTES ---
        static async Task MenuClientes()
        {
            Console.WriteLine("\n--- GESTIÓN DE CLIENTES ---");
            Console.WriteLine("1. Buscar por Cédula");
            Console.WriteLine("2. Crear Nuevo Cliente");
            Console.Write("Opción: ");
            var op = Console.ReadLine();

            if (op == "1")
            {
                Console.Write("Ingrese Cédula: ");
                var cedula = Console.ReadLine();
                var cliente = await ApiService.GetClienteByCedulaAsync(cedula);
                if (cliente != null)
                    Console.WriteLine($"Encontrado: {cliente.Nombres} {cliente.Apellidos} - {cliente.Email}");
                else
                    Console.WriteLine("Cliente no encontrado.");
            }
            else if (op == "2")
            {
                var nuevoCliente = new ClienteComercializadora();
                Console.Write("Cédula: "); nuevoCliente.Cedula = Console.ReadLine();
                Console.Write("Nombres: "); nuevoCliente.Nombres = Console.ReadLine();
                Console.Write("Apellidos: "); nuevoCliente.Apellidos = Console.ReadLine();
                Console.Write("Dirección: "); nuevoCliente.Direccion = Console.ReadLine();
                Console.Write("Teléfono: "); nuevoCliente.Telefono = Console.ReadLine();
                Console.Write("Email: "); nuevoCliente.Email = Console.ReadLine();

                try
                {
                    await ApiService.CreateClienteAsync(nuevoCliente);
                    Console.WriteLine("¡Cliente creado exitosamente!");
                }
                catch (Exception ex) { Console.WriteLine($"Error: {ex.Message}"); }
            }
        }

        // --- MÓDULO DE VENTA (FACTURACIÓN & SOAP) ---
        static async Task ProcesoVenta()
        {
            Console.Clear();
            Console.WriteLine("=== NUEVA VENTA ===");

            // 1. Identificar Cliente
            Console.Write("Ingrese Cédula del Cliente: ");
            string cedula = Console.ReadLine();
            var cliente = await ApiService.GetClienteByCedulaAsync(cedula);

            if (cliente == null)
            {
                Console.WriteLine("Cliente no existe.");
                return;
            }
            Console.WriteLine($"Cliente: {cliente.Nombres} {cliente.Apellidos}");

            // 2. Selección de Productos
            var facturaDto = new CreateFacturaDto
            {
                Cedula = cedula,
                Detalles = new List<DetalleFacturaDto>()
            };

            bool agregando = true;
            decimal subtotalEstimado = 0;

            while (agregando)
            {
                Console.Write("\nIngrese ID del Producto a vender: ");
                if (int.TryParse(Console.ReadLine(), out int idProd))
                {
                    Console.Write("Cantidad: ");
                    int cant = int.Parse(Console.ReadLine());

                    // Consultar producto para saber precio (opcional, para mostrar subtotal)
                    try {
                        var prod = await ApiService.GetProductoAsync(idProd);
                        facturaDto.Detalles.Add(new DetalleFacturaDto { IdProducto = idProd, Cantidad = cant });
                        subtotalEstimado += prod.PrecioVenta * cant;
                        Console.WriteLine($"Agregado: {prod.Nombre} x {cant}");
                    }
                    catch { Console.WriteLine("Producto no válido."); }
                }

                Console.Write("¿Agregar otro producto? (S/N): ");
                if (Console.ReadLine().ToUpper() != "S") agregando = false;
            }

            if (facturaDto.Detalles.Count == 0) return;

            Console.WriteLine($"\nTotal Estimado: ${subtotalEstimado}");

            // 3. Forma de Pago
            Console.WriteLine("Seleccione Forma de Pago: (1) Efectivo, (2) Crédito Directo");
            var pago = Console.ReadLine();

            if (pago == "1")
            {
                facturaDto.FormaPago = "Efectivo";
                facturaDto.PlazoMeses = 0;
                await FinalizarFactura(facturaDto);
            }
            else if (pago == "2")
            {
                facturaDto.FormaPago = "Credito";
                Console.Write("Ingrese Plazo (meses): ");
                facturaDto.PlazoMeses = int.Parse(Console.ReadLine());

                Console.WriteLine("Consultando Central de Riesgos (SOAP)... espere.");

                // --- LÓGICA SOAP ---
                var requestCredito = new EvaluateCreditRequest
                {
                    Cedula = cedula,
                    PrecioElectrodomestico = subtotalEstimado,
                    PlazoMeses = facturaDto.PlazoMeses
                };

                try
                {
                    var respuestaBanco = await BancoSoapService.EvaluateCreditAsync(requestCredito);

                    if (respuestaBanco.Aprobado)
                    {
                        Console.WriteLine($"[BANCO] Crédito APROBADO. ID Crédito: {respuestaBanco.IdCredito}");
                        facturaDto.IdCreditoBanco = respuestaBanco.IdCredito;
                        
                        // Opcional: Mostrar tabla de amortización
                        Console.WriteLine("¿Ver tabla de amortización? (S/N)");
                        if(Console.ReadLine().ToUpper() == "S")
                        {
                            var tabla = await BancoSoapService.GetAmortizationScheduleAsync(respuestaBanco.IdCredito);
                            Console.WriteLine("Cuota | Fecha | Valor");
                            foreach(var c in tabla.Cuotas)
                            {
                                Console.WriteLine($"{c.NumeroCuota} | {c.FechaVencimiento:d} | ${c.ValorCuota}");
                            }
                        }

                        await FinalizarFactura(facturaDto);
                    }
                    else
                    {
                        Console.WriteLine($"[BANCO] Crédito RECHAZADO. Motivo: {respuestaBanco.Mensaje}");
                        Console.WriteLine($"Monto máximo sugerido: ${respuestaBanco.MontoMaximo}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error de comunicación con el Banco: {ex.Message}");
                }
            }
        }

        static async Task FinalizarFactura(CreateFacturaDto dto)
        {
            try
            {
                Console.WriteLine("Generando Factura en el sistema...");
                var resultado = await ApiService.CreateFacturaAsync(dto);
                
                Console.WriteLine("\n========================================");
                Console.WriteLine($"FACTURA GENERADA: {resultado.NumeroFactura}");
                Console.WriteLine($"Total a Pagar: ${resultado.Total}");
                Console.WriteLine($"Estado: {(resultado.Aprobado ? "APROBADA" : "PENDIENTE")}");
                Console.WriteLine("========================================");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al facturar: {ex.Message}");
            }
        }

        // Sistema de Login
        static bool AutenticarUsuario()
        {
            const string USUARIO_VALIDO = "MONSTER";
            const string CONTRASEÑA_VALIDA = "monster9";
            const int INTENTOS_MAXIMOS = 3;

            int intentos = 0;

            while (intentos < INTENTOS_MAXIMOS)
            {
                Console.Clear();
                Console.WriteLine("=== LOGIN DEL SISTEMA ===");
                Console.WriteLine($"Intentos disponibles: {INTENTOS_MAXIMOS - intentos}\n");

                Console.Write("Usuario: ");
                string? usuario = Console.ReadLine();

                Console.Write("Contraseña: ");
                string? contraseña = Console.ReadLine();

                if (usuario == USUARIO_VALIDO && contraseña == CONTRASEÑA_VALIDA)
                {
                    Console.WriteLine("\n✓ Autenticación exitosa. Bienvenido!");
                    System.Threading.Thread.Sleep(1500);
                    return true;
                }

                intentos++;

                if (intentos < INTENTOS_MAXIMOS)
                {
                    Console.WriteLine($"\n✗ Usuario o contraseña incorrectos.");
                    Console.WriteLine($"Intentos restantes: {INTENTOS_MAXIMOS - intentos}");
                    Console.WriteLine("Presione cualquier tecla para intentar de nuevo...");
                    Console.ReadKey();
                }
                else
                {
                    Console.WriteLine($"\n✗ Ha agotado los {INTENTOS_MAXIMOS} intentos permitidos.");
                    Console.WriteLine("Acceso denegado. El programa se cerrará.");
                    System.Threading.Thread.Sleep(2000);
                }
            }

            return false;
        }
    }
}