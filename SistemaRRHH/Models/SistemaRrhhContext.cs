using Microsoft.EntityFrameworkCore;

namespace SistemaRRHH.Models
{
    public class SistemaRrhhContext : DbContext
    {
        public SistemaRrhhContext(DbContextOptions<SistemaRrhhContext> options)
            : base(options)
        {
        }

        public DbSet<Empleados> Empleados { get; set; }
        public DbSet<Asistencias> Asistencias { get; set; }
        public DbSet<SolicitudesVacaciones> SolicitudesVacaciones { get; set; }
        public DbSet<Departamentos> Departamentos { get; set; } 


    }
}