using Microsoft.AspNetCore.Mvc;
using ComercializadoraAPI.Data;
using ComercializadoraAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ComercializadoraAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly ComercializadoraDbContext _context;
        private readonly ILogger<ClientesController> _logger;

        public ClientesController(ComercializadoraDbContext context, ILogger<ClientesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClienteComercializadora>>> GetClientes()
        {
            try
            {
                var clientes = await _context.Clientes.ToListAsync();
                return Ok(clientes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener clientes");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("{cedula}")]
        public async Task<ActionResult<ClienteComercializadora>> GetCliente(string cedula)
        {
            try
            {
                var cliente = await _context.Clientes
                    .FirstOrDefaultAsync(c => c.Cedula == cedula);

                if (cliente == null)
                {
                    return NotFound("Cliente no encontrado");
                }

                return Ok(cliente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener cliente");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPost]
        public async Task<ActionResult<ClienteComercializadora>> CreateCliente(ClienteComercializadora cliente)
        {
            try
            {
                // Validar que la cédula no exista
                var existe = await _context.Clientes.AnyAsync(c => c.Cedula == cliente.Cedula);
                if (existe)
                {
                    return BadRequest("Ya existe un cliente con esa cédula");
                }

                _context.Clientes.Add(cliente);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetCliente), new { cedula = cliente.Cedula }, cliente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear cliente");
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}
