using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using ApisWebConsult.Data;

namespace ApisWebConsult.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // 🔒 protegido con JWT
    public class ClientesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClientesController(AppDbContext context)
        {
            _context = context;
        }

        // Endpoint: obtener todos los clientes
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var clientes = await _context.Cliente
                .FromSqlRaw("EXEC sp_ConsultarClientes") // tu SP en SQL Server
                .ToListAsync();

            if (clientes == null || !clientes.Any())
                return NotFound("No se encontraron clientes.");

            return Ok(clientes);
        }

        // Endpoint: obtener cliente por cédula
        [HttpGet("{cedula}")]
        public async Task<IActionResult> GetByCedula(string cedula)
        {
            var param = new SqlParameter("@Cedula", cedula);

            var clientes = await _context.Cliente
                .FromSqlRaw("EXEC sp_ConsultarClientePorCedula @Cedula", param)
                .ToListAsync();

            if (clientes == null || !clientes.Any())
                return NotFound($"No se encontró cliente con cédula {cedula}.");

            return Ok(clientes);
        }
    }
}