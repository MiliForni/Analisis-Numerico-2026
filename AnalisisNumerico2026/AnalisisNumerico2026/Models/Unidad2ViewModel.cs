using System.ComponentModel.DataAnnotations;

namespace AnalisisNumerico2026.Models
{
    public class Unidad2ViewModel
    {
        [Required(ErrorMessage = "Debe ingresar la dimensión.")]
        [Range(2, 10, ErrorMessage = "La dimensión debe estar entre 2 y 10.")]
        [Display(Name = "Dimensión")]
        public int? Dimension { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un método.")]
        [Display(Name = "Método")]
        public string Metodo { get; set; }

        [Display(Name = "Iteraciones")]
        [Range(1, 10000, ErrorMessage = "Las iteraciones deben ser mayores a 0.")]
        public int? Iteraciones { get; set; }

        [Display(Name = "Tolerancia")]
        [Range(0.0000000001, double.MaxValue,
            ErrorMessage = "La tolerancia debe ser mayor a 0.")]
        public double? Tolerancia { get; set; }

        // Coeficientes de la matriz A
        public double[] Coeficientes { get; set; }

        // Términos independientes
        public double[] TerminosIndependientes { get; set; }

        // Resultado de cada incógnita
        public double[] Solucion { get; set; }

        // Indica si el método logró encontrar una solución
        public bool? Converge { get; set; }

        // Se utiliza principalmente para Gauss-Seidel
        public int? IteracionesRealizadas { get; set; }

        // Error obtenido en Gauss-Seidel
        public double? Error { get; set; }

        // Para mostrar errores o avisos
        public string MensajeError { get; set; }
    }
}