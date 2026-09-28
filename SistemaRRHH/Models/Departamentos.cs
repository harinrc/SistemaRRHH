using System.ComponentModel.DataAnnotations;

namespace SistemaRRHH.Models
{
    public class Departamentos
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }
    }
}
