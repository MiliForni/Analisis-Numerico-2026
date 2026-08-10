using System;
using System.Web.Mvc;
using Calculus;
using AnalisisNumerico2026.Models;

namespace AnalisisNumerico2026.Controllers
{
    public class Unidad1Controller : Controller
    {
        // Muestra la pantalla de Unidad 1
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.FuncionValida = false;
            return View(new Unidad1ViewModel());
        }


        // Se ejecuta cuando el usuario presiona Calcular
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(Unidad1ViewModel model)
        {
            // Controlar campos vacíos y validaciones del modelo
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Normalizar la función por si se ingresó coma decimal
            model.Funcion = model.Funcion.Replace(',', '.');

            // Crear el objeto de Calculus.dll
            Calculo calculo = new Calculo();

            // Evaluar sintaxis de la función
            if (!calculo.Sintaxis(model.Funcion, 'x'))
            {
                ViewBag.FuncionValida = false;

                model.MensajeError =
                    "La función ingresada tiene una sintaxis inválida.";

                return View(model);
            }

            ViewBag.FuncionValida = true;

            double xi = model.Xi.Value;
            double xd = model.Xd.Value;
            double tolerancia = model.Tolerancia.Value;
            int iteraciones = model.Iteraciones.Value;

            // Evaluar función en Xi y Xd
            double fXi = calculo.EvaluaFx(xi);
            double fXd = calculo.EvaluaFx(xd);

            // Controlar resultados inválidos
            if (double.IsNaN(fXi) ||
                double.IsInfinity(fXi) ||
                double.IsNaN(fXd) ||
                double.IsInfinity(fXd))
            {
                model.MensajeError =
                    "La función no está definida en alguno de los extremos del intervalo.";

                return View(model);
            }

            // Si f(xi) * f(xd) > 0, el intervalo no sirve
            if (fXi * fXd > 0)
            {
                model.MensajeError =
                    "El intervalo ingresado no es válido. Debe volver a ingresar Xi y Xd.";

                model.Converge = false;

                return View(model);
            }

            // Si Xi es raíz
            if (fXi == 0)
            {
                model.Raiz = xi;
                model.Error = 0;
                model.IteracionesRealizadas = 0;
                model.Converge = true;

                return View(model);
            }

            // Si Xd es raíz
            if (fXd == 0)
            {
                model.Raiz = xd;
                model.Error = 0;
                model.IteracionesRealizadas = 0;
                model.Converge = true;

                return View(model);
            }


            double xrAnterior = 0;
            double xr = 0;
            double error = 0;

            for (int i = 1; i <= iteraciones; i++)
            {
                // Calcular Xr según el método elegido
                xr = CalcularXr(
                    model.Metodo,
                    calculo,
                    xi,
                    xd
                );

                // Calcular error relativo
                if (xr != 0)
                {
                    error = Math.Abs(
                        (xr - xrAnterior) / xr
                    );
                }
                else
                {
                    error = 0;
                }

                // Evaluar f(xr)
                double fXr = calculo.EvaluaFx(xr);

                if (double.IsNaN(fXr) ||
                    double.IsInfinity(fXr))
                {
                    model.MensajeError =
                        "La función no puede evaluarse en el valor calculado.";

                    model.Converge = false;

                    return View(model);
                }

                // Criterio de corte
                if (Math.Abs(fXr) < tolerancia ||
                    error < tolerancia)
                {
                    model.Raiz = xr;
                    model.Error = error;
                    model.IteracionesRealizadas = i;
                    model.Converge = true;

                    return View(model);
                }

                // Actualizar el intervalo
                if (calculo.EvaluaFx(xi) * fXr > 0)
                {
                    xi = xr;
                }
                else
                {
                    xd = xr;
                }

                xrAnterior = xr;
            }

            // Si supera la cantidad máxima de iteraciones
            model.Raiz = xr;
            model.Error = error;
            model.IteracionesRealizadas = iteraciones;
            model.Converge = false;

            return View(model);
        }


        // Calcula Xr según Bisección o Regla Falsa
        private double CalcularXr(
            string metodo,
            Calculo calculo,
            double xi,
            double xd)
        {
            if (metodo == "Biseccion")
            {
                // Método de Bisección
                return (xi + xd) / 2;
            }

            if (metodo == "ReglaFalsa")
            {
                // Método de Regla Falsa
                double fXi = calculo.EvaluaFx(xi);
                double fXd = calculo.EvaluaFx(xd);

                return (fXd * xi - fXi * xd)
                       / (fXd - fXi);
            }

            return 0;
        }
    }
}