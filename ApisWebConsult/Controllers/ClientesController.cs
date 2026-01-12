using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using ApisWebConsult.Data;

namespace ApisWebConsult.Controllers
{
    [ApiController]
    [Route("api/clientes")]
    [Authorize] // 🔒 protegido con JWT
    public class ClientesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClientesController(AppDbContext context)
        {
            _context = context;
        }

        // Endpoint: obtener todos los clientes
        [HttpGet("ObtenerClientes")]
        public async Task<IActionResult> Clientes()
        {
            try
            {
                var clientes = await _context.Cliente
                    .FromSqlRaw("EXEC sp_ConsultarClientes")
                    .ToListAsync();

                if (clientes == null || !clientes.Any())
                    return NotFound("No se encontraron clientes.");

                return Ok(clientes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, " ERROR SQL: " + ex.Message);
            }
        }


        [HttpGet("ConsultarCliente")]
        public async Task<IActionResult> ConsultarCliente(string cedula)
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