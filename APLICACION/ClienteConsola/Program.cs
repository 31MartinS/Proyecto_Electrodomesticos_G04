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
            ApiService.BaseUrl = "http://10.40.13.67:5001/api";
            // BancoSoapService.ServiceUrl = "http://10.40.13.67:5000/BancoService.asmx";

            bool continuar = true;

            while (continuar)
            {
                Console.Clear();
                Console.WriteLine("=== SISTEMA COMERCIALIZADORA (CONSOLA) ===");
                Console.WriteLine("1. Ver Productos");
                Console.WriteLine("2. Nueva Venta (Facturación)");
                Console.WriteLine("3. Ver Facturas por Cédula");
                Console.WriteLine("4. Salir");
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
                        await VerFacturasPorCedula();
                        break;
                    case "4":
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
                Console.WriteLine("\n{0,-5} {1,-15} {2,-30} {3,10}", "Nº", "Código", "Nombre", "Precio");
                Console.WriteLine(new string('-', 65));
                
                for (int i = 0; i < productos.Count; i++)
                {
                    var p = productos[i];
                    Console.WriteLine("{0,-5} {1,-15} {2,-30} ${3,9:N2}", 
                        (i + 1), p.Codigo, p.Nombre.Length > 30 ? p.Nombre.Substring(0, 27) + "..." : p.Nombre, p.PrecioVenta);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener productos: {ex.Message}");
            }
        }

        // --- MÓDULO DE CLIENTES ---
        static async Task VerFacturasPorCedula()
        {
            Console.Clear();
            Console.WriteLine("=== VER FACTURAS POR CÉDULA ===");
            Console.Write("\nIngrese la cédula del cliente: ");
            string cedula = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(cedula))
            {
                Console.WriteLine("\nCédula no válida.");
                return;
            }

            if (cedula.Length != 10)
            {
                Console.WriteLine("\nLa cédula debe tener 10 dígitos.");
                return;
            }

            try
            {
                Console.WriteLine("\nBuscando facturas...");
                var facturas = await ApiService.GetFacturasByCedulaAsync(cedula);

                if (facturas == null || facturas.Count == 0)
                {
                    Console.WriteLine("\n✗ No se encontraron facturas para esta cédula.");
                    return;
                }

                Console.WriteLine($"\n✓ Se encontraron {facturas.Count} factura(s):");
                Console.WriteLine("\n{0,-5} {1,-20} {2,-12} {3,-15} {4,12}", "Nº", "Número Factura", "Fecha", "Forma Pago", "Total");
                Console.WriteLine(new string('-', 70));

                for (int i = 0; i < facturas.Count; i++)
                {
                    var f = facturas[i];
                    Console.WriteLine("{0,-5} {1,-20} {2,-12:yyyy-MM-dd} {3,-15} ${4,11:N2}",
                        (i + 1),
                        f.NumeroFactura,
                        f.Fecha,
                        f.FormaPago,
                        f.Total);
                }

                Console.WriteLine(new string('-', 70));
                Console.Write("\n¿Desea ver detalles de alguna factura? (S/N): ");
                if (Console.ReadLine()?.ToUpper() == "S")
                {
                    Console.Write("Ingrese el número de la factura a ver: ");
                    if (int.TryParse(Console.ReadLine(), out int numFactura) && numFactura > 0 && numFactura <= facturas.Count)
                    {
                        await MostrarDetalleFactura(facturas[numFactura - 1]);
                    }
                    else
                    {
                        Console.WriteLine("✗ Número de factura inválido.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n✗ Error al buscar facturas: {ex.Message}");
            }
        }

        static async Task MostrarDetalleFactura(FacturaListDto factura)
        {
            try
            {
                Console.Clear();
                Console.WriteLine("=== DETALLE DE FACTURA ===");
                
                Console.WriteLine($"\nNúmero: {factura.NumeroFactura}");
                Console.WriteLine($"Fecha: {factura.Fecha:dd/MM/yyyy HH:mm}");
                Console.WriteLine($"Cliente: {factura.Cliente.Nombres} {factura.Cliente.Apellidos}");
                Console.WriteLine($"Cédula: {factura.Cliente.Cedula}");
                Console.WriteLine($"Forma de Pago: {factura.FormaPago}");
                
                Console.WriteLine("\n--- PRODUCTOS ---");
                Console.WriteLine("{0,-5} {1,-35} {2,10} {3,12} {4,12}", "Cant", "Producto", "Precio", "Subtotal", "");
                Console.WriteLine(new string('-', 75));
                
                foreach (var detalle in factura.Detalles)
                {
                    string nombreCorto = detalle.NombreProducto.Length > 35 
                        ? detalle.NombreProducto.Substring(0, 32) + "..." 
                        : detalle.NombreProducto;
                    Console.WriteLine("{0,-5} {1,-35} ${2,9:N2} ${3,11:N2}",
                        detalle.Cantidad,
                        nombreCorto,
                        detalle.PrecioUnitario,
                        detalle.Subtotal);
                }
                
                Console.WriteLine(new string('-', 75));
                Console.WriteLine($"Subtotal: ${factura.Subtotal,12:N2}");
                if (factura.Descuento > 0)
                {
                    Console.WriteLine($"Descuento: -${factura.Descuento,11:N2}");
                }
                Console.WriteLine($"TOTAL: ${factura.Total,15:N2}");
                Console.WriteLine(new string('=', 75));
                
                // Si es crédito, mostrar tabla de amortización
                if (factura.IdCreditoBanco.HasValue)
                {
                    Console.Write("\n¿Desea ver la tabla de amortización? (S/N): ");
                    if (Console.ReadLine()?.ToUpper() == "S")
                    {
                        await MostrarTablaAmortizacion(factura.IdCreditoBanco.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n✗ Error al mostrar detalles: {ex.Message}");
            }
        }

        static async Task MostrarTablaAmortizacion(int idCredito)
        {
            try
            {
                Console.WriteLine("\n=== TABLA DE AMORTIZACIÓN ===");
                var tabla = await BancoSoapService.GetAmortizationScheduleAsync(idCredito);
                
                if (tabla.Cuotas != null && tabla.Cuotas.Count > 0)
                {
                    Console.WriteLine("\n{0,-8} {1,-12} {2,12} {3,12} {4,12} {5,12}", 
                        "Cuota", "Fecha", "Valor", "Interés", "Capital", "Saldo");
                    Console.WriteLine(new string('-', 75));
                    
                    foreach (var cuota in tabla.Cuotas)
                    {
                        Console.WriteLine("{0,-8} {1,-12:yyyy-MM-dd} ${2,11:N2} ${3,11:N2} ${4,11:N2} ${5,11:N2}",
                            cuota.NumeroCuota,
                            cuota.FechaVencimiento,
                            cuota.ValorCuota,
                            cuota.InteresPagado,
                            cuota.CapitalPagado,
                            cuota.SaldoRestante);
                    }
                    Console.WriteLine(new string('-', 75));
                }
                else
                {
                    Console.WriteLine("\n✗ No se pudo obtener la tabla de amortización.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n✗ Error al obtener tabla de amortización: {ex.Message}");
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

            // Cargar productos disponibles
            List<ProductoDto> productos;
            try
            {
                productos = await ApiService.GetProductosAsync();
                if (productos.Count == 0)
                {
                    Console.WriteLine("\nNo hay productos disponibles.");
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError al cargar productos: {ex.Message}");
                return;
            }

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
                Console.WriteLine("\n--- PRODUCTOS DISPONIBLES ---");
                Console.WriteLine("{0,-5} {1,-15} {2,-35} {3,10}", "Nº", "Código", "Nombre", "Precio");
                Console.WriteLine(new string('-', 70));
                
                for (int i = 0; i < productos.Count; i++)
                {
                    var p = productos[i];
                    string nombreCorto = p.Nombre.Length > 35 ? p.Nombre.Substring(0, 32) + "..." : p.Nombre;
                    Console.WriteLine("{0,-5} {1,-15} {2,-35} ${3,9:N2}", 
                        (i + 1), p.Codigo, nombreCorto, p.PrecioVenta);
                }
                
                Console.Write("\nSeleccione el número del producto (0 para terminar): ");
                if (int.TryParse(Console.ReadLine(), out int opcion))
                {
                    if (opcion == 0)
                    {
                        agregando = false;
                        continue;
                    }
                    
                    if (opcion < 1 || opcion > productos.Count)
                    {
                        Console.WriteLine("✗ Número de producto inválido.");
                        continue;
                    }
                    
                    var productoSeleccionado = productos[opcion - 1];
                    
                    Console.Write($"Cantidad de '{productoSeleccionado.Nombre}': ");
                    if (int.TryParse(Console.ReadLine(), out int cant) && cant > 0)
                    {
                        facturaDto.Detalles.Add(new DetalleFacturaDto 
                        { 
                            IdProducto = productoSeleccionado.IdProducto, 
                            Cantidad = cant 
                        });
                        
                        decimal subtotal = productoSeleccionado.PrecioVenta * cant;
                        subtotalEstimado += subtotal;
                        
                        Console.WriteLine($"✓ Agregado: {productoSeleccionado.Nombre} x {cant} = ${subtotal:N2}");
                    }
                    else
                    {
                        Console.WriteLine("✗ Cantidad inválida.");
                    }
                }
                else
                {
                    Console.WriteLine("✗ Opción inválida.");
                }
            }

            if (facturaDto.Detalles.Count == 0) 
            {
                Console.WriteLine("\nNo se agregó ningún producto. Venta cancelada.");
                return;
            }

            Console.WriteLine($"\n" + new string('=', 50));
            Console.WriteLine($"Total Estimado: ${subtotalEstimado:N2}");
            Console.WriteLine(new string('=', 50));

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