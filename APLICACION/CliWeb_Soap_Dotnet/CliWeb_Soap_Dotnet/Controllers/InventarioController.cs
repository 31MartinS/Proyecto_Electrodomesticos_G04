using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ClienteWeb.Models;
using ClienteWeb.Services;

namespace ClienteWeb.Controllers
{
    public class InventarioController : Controller
    {
        // LISTAR PRODUCTOS
        public async Task<IActionResult> Index()
        {
            var productos = await ApiService.GetProductosAsync();
            return View(productos); // Vista: Views/Inventario/Index.cshtml
        }

        // CREAR - GET
        [HttpGet]
        public IActionResult Crear()
        {
            return View(new CreateProductoDto()); // Vista: Views/Inventario/Crear.cshtml
        }

        // CREAR - POST
        [HttpPost]
        public async Task<IActionResult> Crear(CreateProductoDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await ApiService.CreateProductoAsync(model);
                TempData["Mensaje"] = "Producto creado correctamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        // EDITAR - GET
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var producto = await ApiService.GetProductoAsync(id);
            if (producto == null)
                return NotFound();

            var model = new CreateProductoDto
            {
                Codigo = producto.Codigo,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                PrecioVenta = producto.PrecioVenta
            };

            ViewBag.IdProducto = producto.IdProducto;
            return View(model); // Vista: Views/Inventario/Editar.cshtml
        }

        // EDITAR - POST
        [HttpPost]
        public async Task<IActionResult> Editar(int id, CreateProductoDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await ApiService.UpdateProductoAsync(id, model);
                TempData["Mensaje"] = "Producto actualizado correctamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        // ELIMINAR
        [HttpPost]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                await ApiService.DeleteProductoAsync(id);
                TempData["Mensaje"] = "Producto eliminado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Index");
        }
    }
}
