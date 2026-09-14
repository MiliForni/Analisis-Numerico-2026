using System;
using System.Web.Mvc;
using AnalisisNumerico2026.Models;

namespace AnalisisNumerico2026.Controllers
{
    public class Unidad2Controller : Controller
    {
        // GET
        public ActionResult Index()
        {
            var model = new Unidad2ViewModel
            {
                Dimension = 3,
                Metodo = "Gauss-Jordan",
                Iteraciones = 100,
                Tolerancia = 0.0001
            };

            return View(model);
        }

        // POST
        [HttpPost]
        public ActionResult Index(Unidad2ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!model.Dimension.HasValue)
            {
                model.MensajeError =
                    "Debe ingresar la dimensión del sistema.";

                return View(model);
            }

            int n = model.Dimension.Value;

            // Controlamos los coeficientes
            if (model.Coeficientes == null ||
                model.Coeficientes.Length != n * n)
            {
                model.MensajeError =
                    "Debe completar todos los coeficientes de la matriz.";

                return View(model);
            }

            // Controlamos los términos independientes
            if (model.TerminosIndependientes == null ||
                model.TerminosIndependientes.Length != n)
            {
                model.MensajeError =
                    "Debe completar todos los términos independientes.";

                return View(model);
            }

            // GAUSS-JORDAN
            if (model.Metodo == "Gauss-Jordan")
            {
                ResolverGaussJordan(model);
            }

            // GAUSS-SEIDEL
            else if (model.Metodo == "Gauss-Seidel")
            {
                if (!model.Iteraciones.HasValue ||
                    model.Iteraciones.Value <= 0)
                {
                    model.MensajeError =
                        "Debe ingresar una cantidad de iteraciones mayor a 0.";

                    return View(model);
                }

                if (!model.Tolerancia.HasValue ||
                    model.Tolerancia.Value <= 0)
                {
                    model.MensajeError =
                        "Debe ingresar una tolerancia mayor a 0.";

                    return View(model);
                }

                ResolverGaussSeidel(model);
            }

            else
            {
                model.MensajeError =
                    "Debe seleccionar un método.";
            }

            return View(model);
        }

        private void ResolverGaussJordan(Unidad2ViewModel model)
        {
            int n = model.Dimension.Value;

            // Matriz aumentada
            double[,] matriz = new double[n, n + 1];

            int posicion = 0;

            // Cargamos la matriz
            for (int fila = 0; fila < n; fila++)
            {
                for (int columna = 0; columna < n; columna++)
                {
                    matriz[fila, columna] =
                        model.Coeficientes[posicion];

                    posicion++;
                }

                matriz[fila, n] =
                    model.TerminosIndependientes[fila];
            }

            // Gauss-Jordan
            for (int filaDiagonal = 0;
                 filaDiagonal < n;
                 filaDiagonal++)
            {
                double coeficienteDiagonal =
                    matriz[filaDiagonal, filaDiagonal];

                // No podemos dividir por cero
                if (Math.Abs(coeficienteDiagonal) < 0.0000000001)
                {
                    model.MensajeError =
                        "No se puede continuar porque se encontró un pivote igual a cero.";

                    model.Converge = false;
                    model.Solucion = null;

                    return;
                }

                // Dividimos toda la fila por el pivote
                for (int columna = 0;
                     columna <= n;
                     columna++)
                {
                    matriz[filaDiagonal, columna] =
                        matriz[filaDiagonal, columna]
                        / coeficienteDiagonal;
                }

                // Hacemos cero el resto de la columna
                for (int fila = 0;
                     fila < n;
                     fila++)
                {
                    if (fila == filaDiagonal)
                    {
                        continue;
                    }

                    double coeficienteCero =
                        matriz[fila, filaDiagonal];

                    for (int columna = 0;
                         columna <= n;
                         columna++)
                    {
                        matriz[fila, columna] =
                            matriz[fila, columna]
                            - coeficienteCero
                            * matriz[filaDiagonal, columna];
                    }
                }
            }

            // Guardamos la solución
            model.Solucion = new double[n];

            for (int fila = 0;
                 fila < n;
                 fila++)
            {
                model.Solucion[fila] =
                    matriz[fila, n];
            }

            model.Converge = true;
            model.IteracionesRealizadas = null;
            model.Error = null;
            model.MensajeError = null;
        }


        private void ResolverGaussSeidel(Unidad2ViewModel model)
        {
            int n = model.Dimension.Value;

            double[,] matriz = new double[n, n];

            int posicion = 0;

            // Pasamos los coeficientes al arreglo bidimensional
            for (int fila = 0;
                 fila < n;
                 fila++)
            {
                for (int columna = 0;
                     columna < n;
                     columna++)
                {
                    matriz[fila, columna] =
                        model.Coeficientes[posicion];

                    posicion++;
                }
            }

            // Controlamos que ningún elemento
            // de la diagonal principal sea cero
            for (int fila = 0;
                 fila < n;
                 fila++)
            {
                if (Math.Abs(matriz[fila, fila]) <
                    0.0000000001)
                {
                    model.MensajeError =
                        "Gauss-Seidel no puede continuar porque hay un valor igual a cero en la diagonal principal.";

                    model.Converge = false;
                    model.Solucion = null;
                    model.Error = null;
                    model.IteracionesRealizadas = 0;

                    return;
                }
            }

            int iteracionesMaximas =
                model.Iteraciones.Value;

            double tolerancia =
                model.Tolerancia.Value;

            // Solución inicial: todos los valores en cero
            double[] vectorResultado =
                new double[n];

            double[] vectorAnterior =
                new double[n];

            bool esSolucion = false;

            double errorMaximo = 0;

            int contador = 0;


            while (contador < iteracionesMaximas &&
                   !esSolucion)
            {
                contador++;

                // Guardamos la iteración anterior
                for (int i = 0; i < n; i++)
                {
                    vectorAnterior[i] =
                        vectorResultado[i];
                }


                // Calculamos cada incógnita
                for (int fila = 0;
                     fila < n;
                     fila++)
                {
                    double resultado =
                        model.TerminosIndependientes[fila];

                    double coeficienteIncognita =
                        matriz[fila, fila];


                    for (int columna = 0;
                         columna < n;
                         columna++)
                    {
                        if (fila != columna)
                        {
                            resultado =
                                resultado
                                - matriz[fila, columna]
                                * vectorResultado[columna];
                        }
                    }


                    vectorResultado[fila] =
                        resultado
                        / coeficienteIncognita;
                }


                // Comparamos con la iteración anterior
                int contadorDentroTolerancia = 0;

                errorMaximo = 0;


                for (int i = 0;
                     i < n;
                     i++)
                {
                    double errorRelativo;


                    // Evitamos dividir por cero
                    if (Math.Abs(vectorResultado[i]) <
                        0.0000000001)
                    {
                        errorRelativo =
                            Math.Abs(
                                vectorResultado[i]
                                - vectorAnterior[i]
                            );
                    }
                    else
                    {
                        errorRelativo =
                            Math.Abs(
                                (vectorResultado[i]
                                - vectorAnterior[i])
                                / vectorResultado[i]
                            );
                    }


                    if (errorRelativo > errorMaximo)
                    {
                        errorMaximo =
                            errorRelativo;
                    }


                    if (errorRelativo < tolerancia)
                    {
                        contadorDentroTolerancia++;
                    }
                }


                // Todas las variables deben cumplir
                // con la tolerancia
                if (contadorDentroTolerancia == n)
                {
                    esSolucion = true;
                }
            }


            model.IteracionesRealizadas =
                contador;

            model.Error =
                errorMaximo;


            if (esSolucion)
            {
                model.Solucion =
                    new double[n];

                for (int i = 0;
                     i < n;
                     i++)
                {
                    model.Solucion[i] =
                        vectorResultado[i];
                }

                model.Converge = true;

                model.MensajeError = null;
            }
            else
            {
                model.Solucion = null;

                model.Converge = false;

                model.MensajeError =
                    "Gauss-Seidel alcanzó el máximo de iteraciones sin encontrar una solución.";
            }
        }
    }
}