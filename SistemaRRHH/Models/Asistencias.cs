using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaRRHH.Models
{
    public class Asistencias
    {
        [Key]
        public int Id { get; set; }

        public int EmpleadoId { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime Fecha { get; set; }

        public DateTime HoraEntrada { get; set; }

        public DateTime? HoraSalida { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal? HorasTrabajadas { get; set; }
    }
}
