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


            int dimension = model.Dimension.Value;


            // Controlamos los coeficientes
            if (model.Coeficientes == null ||
                model.Coeficientes.Length != dimension * dimension)
            {
                model.MensajeError =
                    "Debe completar todos los coeficientes de la matriz.";

                return View(model);
            }


            // Controlamos los términos independientes
            if (model.TerminosIndependientes == null ||
                model.TerminosIndependientes.Length != dimension)
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
            int dimension = model.Dimension.Value;

            // Matriz aumentada
            double[,] matriz =
                new double[dimension, dimension + 1];


            int posicion = 0;


            // Cargamos la matriz
            for (int fila = 0;
                 fila < dimension;
                 fila++)
            {
                for (int columna = 0;
                     columna < dimension;
                     columna++)
                {
                    matriz[fila, columna] =
                        model.Coeficientes[posicion];

                    posicion++;
                }


                matriz[fila, dimension] =
                    model.TerminosIndependientes[fila];
            }


            // GAUSS-JORDAN
            for (int filaDiagonal = 0;
                 filaDiagonal < dimension;
                 filaDiagonal++)
            {
                // PIVOTEO PARCIAL
                int filaMayor = filaDiagonal;

                double mayorValor =
                    Math.Abs(
                        matriz[filaDiagonal, filaDiagonal]
                    );


                for (int fila = filaDiagonal + 1;
                     fila < dimension;
                     fila++)
                {
                    double valorActual =
                        Math.Abs(
                            matriz[fila, filaDiagonal]
                        );


                    if (valorActual > mayorValor)
                    {
                        mayorValor = valorActual;

                        filaMayor = fila;
                    }
                }


                // Si toda la columna tiene cero
                if (mayorValor < 0.0000000001)
                {
                    model.MensajeError =
                        "No se puede continuar porque el sistema tiene un pivote igual a cero.";

                    model.Converge = false;

                    model.Solucion = null;

                    return;
                }


                // Cambiamos las filas
                if (filaMayor != filaDiagonal)
                {
                    for (int columna = 0;
                         columna <= dimension;
                         columna++)
                    {
                        double auxiliar =
                            matriz[filaDiagonal, columna];

                        matriz[filaDiagonal, columna] =
                            matriz[filaMayor, columna];

                        matriz[filaMayor, columna] =
                            auxiliar;
                    }
                }


                // Obtenemos el pivote
                double coeficienteDiagonal =
                    matriz[filaDiagonal, filaDiagonal];


                // Normalizamos la fila
                for (int columna = 0;
                     columna <= dimension;
                     columna++)
                {
                    matriz[filaDiagonal, columna] =
                        matriz[filaDiagonal, columna]
                        / coeficienteDiagonal;
                }


                // Hacemos cero todos los demás
                // elementos de la columna
                for (int fila = 0;
                     fila < dimension;
                     fila++)
                {
                    if (fila == filaDiagonal)
                    {
                        continue;
                    }


                    double coeficienteCero =
                        matriz[fila, filaDiagonal];


                    for (int columna = 0;
                         columna <= dimension;
                         columna++)
                    {
                        matriz[fila, columna] =
                            matriz[fila, columna]
                            -
                            coeficienteCero
                            *
                            matriz[filaDiagonal, columna];
                    }
                }
            }


            // Guardamos la solución
            model.Solucion =
                new double[dimension];


            for (int fila = 0;
                 fila < dimension;
                 fila++)
            {
                model.Solucion[fila] =
                    matriz[fila, dimension];
            }


            model.Converge = true;

            model.IteracionesRealizadas = null;

            model.Error = null;

            model.MensajeError = null;
        }


        private void ResolverGaussSeidel(
            Unidad2ViewModel model)
        {
            int dimension =
                model.Dimension.Value;


            double[,] matriz =
                new double[dimension, dimension];


            int posicion = 0;


            // Cargamos la matriz
            for (int fila = 0;
                 fila < dimension;
                 fila++)
            {
                for (int columna = 0;
                     columna < dimension;
                     columna++)
                {
                    matriz[fila, columna] =
                        model.Coeficientes[posicion];

                    posicion++;
                }
            }


            // La diagonal principal no puede tener ceros
            for (int fila = 0;
                 fila < dimension;
                 fila++)
            {
                if (Math.Abs(
                        matriz[fila, fila]
                    ) < 0.0000000001)
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


            // Solución inicial
            // El apunte utiliza valores iniciales en cero
            double[] vectorResultado =
                new double[dimension];


            double[] vectorAnterior =
                new double[dimension];


            bool esSolucion = false;


            int contador = 0;


            double errorMaximo = 0;


            while (contador < iteracionesMaximas &&
                   !esSolucion)
            {
                contador++;


                // Guardamos la solución anterior
                for (int i = 0;
                     i < dimension;
                     i++)
                {
                    vectorAnterior[i] =
                        vectorResultado[i];
                }


                // Calculamos cada incógnita
                for (int fila = 0;
                     fila < dimension;
                     fila++)
                {
                    double resultado =
                        model.TerminosIndependientes[fila];


                    double coeficienteIncognita =
                        matriz[fila, fila];


                    for (int columna = 0;
                         columna < dimension;
                         columna++)
                    {
                        if (fila != columna)
                        {
                            // Gauss-Seidel utiliza los
                            // valores encontrados en
                            // la misma iteración
                            resultado =
                                resultado
                                -
                                matriz[fila, columna]
                                *
                                vectorResultado[columna];
                        }
                    }


                    vectorResultado[fila] =
                        resultado
                        /
                        coeficienteIncognita;
                }


                // Comparamos dos iteraciones sucesivas
                int variablesDentroTolerancia = 0;


                errorMaximo = 0;


                for (int i = 0;
                     i < dimension;
                     i++)
                {
                    double errorRelativo;


                    if (Math.Abs(
                            vectorResultado[i]
                        ) < 0.0000000001)
                    {
                        // Evitamos dividir por cero
                        errorRelativo =
                            Math.Abs(
                                vectorResultado[i]
                                -
                                vectorAnterior[i]
                            );
                    }
                    else
                    {
                        errorRelativo =
                            Math.Abs(
                                (
                                    vectorResultado[i]
                                    -
                                    vectorAnterior[i]
                                )
                                /
                                vectorResultado[i]
                            );
                    }


                    if (errorRelativo > errorMaximo)
                    {
                        errorMaximo =
                            errorRelativo;
                    }


                    if (errorRelativo < tolerancia)
                    {
                        variablesDentroTolerancia++;
                    }
                }


                // TODAS las incógnitas
                // deben cumplir la tolerancia
                if (variablesDentroTolerancia ==
                    dimension)
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
                    new double[dimension];


                for (int i = 0;
                     i < dimension;
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