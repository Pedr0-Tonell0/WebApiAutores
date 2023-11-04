using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validaciones;

namespace WebApiAutores.DTO
{
    public class LibroPatchDTO
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 120, ErrorMessage = "El campo {0} es no debe tener más de {1} caracteres")]
        [PrimeraLetraMayuscula]
        public string Titulo { get; set; }
        public DateTime? FechaPublicacion { get; set; }
    }
}
