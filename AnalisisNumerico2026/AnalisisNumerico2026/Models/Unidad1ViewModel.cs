using System.ComponentModel.DataAnnotations;

namespace AnalisisNumerico2026.Models
{
    public class Unidad1ViewModel
    {
        [Required(ErrorMessage = "Debe ingresar una función.")]
        [Display(Name = "Función")]
        public string Funcion { get; set; }


        [Required(ErrorMessage = "Debe seleccionar un método.")]
        [Display(Name = "Método")]
        public string Metodo { get; set; }


        [Required(ErrorMessage = "Debe ingresar la cantidad de iteraciones.")]
        [Range(1, int.MaxValue,
            ErrorMessage = "Las iteraciones deben ser mayores a 0.")]
        [Display(Name = "Iteraciones")]
        public int? Iteraciones { get; set; }


        [Required(ErrorMessage = "Debe ingresar una tolerancia.")]
        [Range(0.0000000001, double.MaxValue,
            ErrorMessage = "La tolerancia debe ser mayor a 0.")]
        [Display(Name = "Tolerancia")]
        public double? Tolerancia { get; set; }


        // MÉTODOS CERRADOS
    

        [Display(Name = "Xi")]
        public double? Xi { get; set; }

        [Display(Name = "Xd")]
        public double? Xd { get; set; }


        
        // MÉTODOS ABIERTOS
        

        [Display(Name = "X0")]
        public double? X0 { get; set; }

        [Display(Name = "X1")]
        public double? X1 { get; set; }

        // ESCALA DEL GRÁFICO

        [Display(Name = "X mínimo")]
        public double? XMin { get; set; }

        [Display(Name = "X máximo")]
        public double? XMax { get; set; }

        // RESULTADOS
        public double? Raiz { get; set; }

        public double? Error { get; set; }

        public int? IteracionesRealizadas { get; set; }

        public bool? Converge { get; set; }

        public string MensajeError { get; set; }
    }
}