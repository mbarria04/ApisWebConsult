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
    public class ReportesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReportesController(AppDbContext context)
        {
            _context = context;
        }

        // Obtener todos los menús
        [HttpGet("menus")]
        public async Task<IActionResult> GetMenus()
        {
            try
            {
                var menus = await _context.Menus
                    .FromSqlRaw("EXEC sp_ConsultarMenus") // tu SP en SQL Server
                    .ToListAsync();

                if (menus == null || !menus.Any())
                {
                    return NotFound("No se encontraron menús.");
                }

                return Ok(menus);
            }
            catch (SqlException sqlEx)
            {
                // Errores específicos de SQL Server
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Error en la base de datos: {sqlEx.Message}");
            }
            catch (DbUpdateException dbEx)
            {
                // Errores de EF Core
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Error al actualizar datos: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                // Cualquier otro error
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Error inesperado: {ex.Message}");
            }
        }

        // Obtener menús filtrados por IdPadre
        [HttpGet("menus/{idPadre}")]
        public async Task<IActionResult> GetMenusByPadre(int idPadre)
        {
            var param = new SqlParameter("@IdPadre", idPadre);

            var menus = await _context.Menus
                .FromSqlRaw("EXEC sp_ConsultarMenus @IdPadre", param)
                .ToListAsync();

            return Ok(menus);
        }
    }

}
