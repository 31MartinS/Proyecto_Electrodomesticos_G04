using Microsoft.AspNetCore.Mvc;
using ComercializadoraAPI.Data;
using ComercializadoraAPI.DTOs;
using ComercializadoraAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace ComercializadoraAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        private readonly ComercializadoraDbContext _context;
        private readonly ILogger<ProductosController> _logger;

        public ProductosController(ComercializadoraDbContext context, ILogger<ProductosController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductoDto>>> GetProductos()
        {
            try
            {
                var productosEnt = await _context.Productos.ToListAsync();

                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var productos = productosEnt.Select(p => new ProductoDto
                {
                    IdProducto = p.IdProducto,
                    Codigo = p.Codigo,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    PrecioVenta = p.PrecioVenta,
                    ImageUrl = string.IsNullOrEmpty(p.ImageFileName) ? null : $"{baseUrl}/images/products/{p.ImageFileName}"
                }).ToList();

                return Ok(productos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener productos");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductoDto>> GetProducto(int id)
        {
            try
            {
                var p = await _context.Productos.FirstOrDefaultAsync(x => x.IdProducto == id);
                if (p == null)
                {
                    return NotFound("Producto no encontrado");
                }

                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var producto = new ProductoDto
                {
                    IdProducto = p.IdProducto,
                    Codigo = p.Codigo,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    PrecioVenta = p.PrecioVenta,
                    ImageUrl = string.IsNullOrEmpty(p.ImageFileName) ? null : $"{baseUrl}/images/products/{p.ImageFileName}"
                };

                return Ok(producto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener producto");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPost]
        public async Task<ActionResult<ProductoDto>> CreateProducto(CreateProductoDto createDto)
        {
            try
            {
                // Validar que el código no exista
                var existe = await _context.Productos.AnyAsync(p => p.Codigo == createDto.Codigo);
                if (existe)
                {
                    return BadRequest("Ya existe un producto con ese código");
                }

                var producto = new Producto
                {
                    Codigo = createDto.Codigo,
                    Nombre = createDto.Nombre,
                    Descripcion = createDto.Descripcion,
                    PrecioVenta = createDto.PrecioVenta
                };

                _context.Productos.Add(producto);
                await _context.SaveChangesAsync();

                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var productoDto = new ProductoDto
                {
                    IdProducto = producto.IdProducto,
                    Codigo = producto.Codigo,
                    Nombre = producto.Nombre,
                    Descripcion = producto.Descripcion,
                    PrecioVenta = producto.PrecioVenta,
                    ImageUrl = null
                };

                return CreatedAtAction(nameof(GetProducto), new { id = producto.IdProducto }, productoDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear producto");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ProductoDto>> UpdateProducto(int id, CreateProductoDto updateDto)
        {
            try
            {
                var producto = await _context.Productos.FindAsync(id);
                if (producto == null)
                {
                    return NotFound("Producto no encontrado");
                }

                // Verificar que el código no esté en uso por otro producto
                var existeOtroCodigo = await _context.Productos
                    .AnyAsync(p => p.Codigo == updateDto.Codigo && p.IdProducto != id);
                if (existeOtroCodigo)
                {
                    return BadRequest("Ya existe otro producto con ese código");
                }

                producto.Codigo = updateDto.Codigo;
                producto.Nombre = updateDto.Nombre;
                producto.Descripcion = updateDto.Descripcion;
                producto.PrecioVenta = updateDto.PrecioVenta;

                _context.Productos.Update(producto);
                await _context.SaveChangesAsync();

                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var productoDto = new ProductoDto
                {
                    IdProducto = producto.IdProducto,
                    Codigo = producto.Codigo,
                    Nombre = producto.Nombre,
                    Descripcion = producto.Descripcion,
                    PrecioVenta = producto.PrecioVenta,
                    ImageUrl = string.IsNullOrEmpty(producto.ImageFileName) ? null : $"{baseUrl}/images/products/{producto.ImageFileName}"
                };

                return Ok(productoDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar producto");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProducto(int id)
        {
            try
            {
                var producto = await _context.Productos.FindAsync(id);
                if (producto == null)
                {
                    return NotFound("Producto no encontrado");
                }

                // Verificar si tiene facturas asociadas
                var tieneFacturas = await _context.DetallesFactura
                    .AnyAsync(df => df.IdProducto == id);
                if (tieneFacturas)
                {
                    return BadRequest("No se puede eliminar el producto porque tiene facturas asociadas");
                }

                // Eliminar imagen física si existe
                if (!string.IsNullOrEmpty(producto.ImageFileName))
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "products");
                    var filePath = Path.Combine(uploadsFolder, producto.ImageFileName);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _context.Productos.Remove(producto);
                await _context.SaveChangesAsync();

                return Ok(new { Mensaje = "Producto eliminado exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar producto");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPost("{id}/image")]
        public async Task<ActionResult> UploadImage(int id, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Archivo inválido");
            }

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound("Producto no encontrado");
            }

            var allowed = new[] { "image/png", "image/jpeg", "image/jpg", "image/gif" };
            if (!allowed.Contains(file.ContentType))
            {
                return BadRequest("Tipo de archivo no permitido. Use PNG/JPEG/GIF.");
            }

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "products");
            Directory.CreateDirectory(uploadsFolder);

            // Eliminar imagen anterior si existe
            if (!string.IsNullOrEmpty(producto.ImageFileName))
            {
                var oldFilePath = Path.Combine(uploadsFolder, producto.ImageFileName);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }

            var fileName = $"prod_{producto.IdProducto}_{DateTime.UtcNow:yyyyMMddHHmmss}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            producto.ImageFileName = fileName;
            _context.Productos.Update(producto);
            await _context.SaveChangesAsync();

            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var imageUrl = $"{baseUrl}/images/products/{fileName}";

            return Ok(new { ImageUrl = imageUrl });
        }
    }
}
