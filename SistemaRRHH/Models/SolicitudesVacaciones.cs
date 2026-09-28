using System;
using System.ComponentModel.DataAnnotations;

namespace SistemaRRHH.Models
{
    public class SolicitudesVacaciones
    {
        [Key]
        public int Id { get; set; }

        public int EmpleadoId { get; set; }

        [Required]
        public DateTime FechaInicio { get; set; }

        [Required]
        public DateTime FechaFin { get; set; }

        public int DiasSolicitados { get; set; }

        public string Estado { get; set; } = "Pendiente";

        public DateTime? FechaSolicitud { get; set; } = DateTime.Now;

        public string? Comentarios { get; set; }
    }
}