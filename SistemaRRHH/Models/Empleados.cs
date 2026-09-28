using System;
using System.ComponentModel.DataAnnotations;

namespace SistemaRRHH.Models
{
    public class Empleados
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string ContrasenaHash { get; set; } = null!;
        public DateTime FechaIngreso { get; set; }
        public int? DiasVacacionesDisponibles { get; set; }
        public int? DepartamentoId { get; set; }
        public string? Rol { get; set; }
    }
}