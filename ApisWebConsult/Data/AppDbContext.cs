using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using ApisWebConsult.Modelo;

namespace ApisWebConsult.Data
{
    

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Reporte> Reportes { get; set; }
        public DbSet<Menu> Menus { get; set; }
        
        public DbSet<Cliente> Cliente { get; set; }

    }

}
