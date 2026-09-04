using System;
using System.Web.Mvc;
using Calculus;
using AnalisisNumerico2026.Models;

namespace AnalisisNumerico2026.Controllers
{
    public class Unidad1Controller : Controller
    {
        // GET
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.FuncionValida = false;

            return View(new Unidad1ViewModel
            {
                XMin = -5,
                XMax = 5
            });
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(Unidad1ViewModel model)
        {
            ViewBag.FuncionValida = false;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Validar Bisección y Regla Falsa
            if (model.Metodo == "Biseccion" ||
                model.Metodo == "ReglaFalsa")
            {
                if (!model.Xi.HasValue ||
                    !model.Xd.HasValue)
                {
                    model.MensajeError =
                        "Debe ingresar Xi y Xd para utilizar este método.";

                    return View(model);
                }
            }

            // Validar Newton-Raphson
            if (model.Metodo == "Newton")
            {
                if (!model.X0.HasValue)
                {
                    model.MensajeError =
                        "Debe ingresar X0 para utilizar Newton-Raphson.";

                    return View(model);
                }
            }

            // Validar Secante
            if (model.Metodo == "Secante")
            {
                if (!model.X0.HasValue ||
                    !model.X1.HasValue)
                {
                    model.MensajeError =
                        "Debe ingresar X0 y X1 para utilizar el método de la Secante.";

                    return View(model);
                }
            }

            // Validar escala
            if (model.XMin.HasValue &&
                model.XMax.HasValue &&
                model.XMin.Value >= model.XMax.Value)
            {
                model.MensajeError =
                    "X mínimo debe ser menor que X máximo.";

                return View(model);
            }

            model.Funcion =
                model.Funcion.Replace(',', '.');

            Calculo calculo =
                new Calculo();

            // Validar sintaxis
            if (!calculo.Sintaxis(model.Funcion, 'x'))
            {
                model.MensajeError =
                    "La función ingresada tiene una sintaxis inválida.";

                ViewBag.FuncionValida = false;

                return View(model);
            }

            ViewBag.FuncionValida = true;

            double tolerancia =
                model.Tolerancia.Value;

            int iteraciones =
                model.Iteraciones.Value;

            // Bisección
            if (model.Metodo == "Biseccion")
            {
                return ResolverMetodoCerrado(
                    model,
                    calculo,
                    tolerancia,
                    iteraciones,
                    false
                );
            }

            // Regla Falsa
            if (model.Metodo == "ReglaFalsa")
            {
                return ResolverMetodoCerrado(
                    model,
                    calculo,
                    tolerancia,
                    iteraciones,
                    true
                );
            }

            // Newton-Raphson
            if (model.Metodo == "Newton")
            {
                return ResolverNewton(
                    model,
                    calculo,
                    tolerancia,
                    iteraciones
                );
            }

            // Secante
            if (model.Metodo == "Secante")
            {
                return ResolverSecante(
                    model,
                    calculo,
                    tolerancia,
                    iteraciones
                );
            }

            model.MensajeError =
                "El método seleccionado no es válido.";

            return View(model);
        }

        // Métodos cerrados
        private ActionResult ResolverMetodoCerrado(
            Unidad1ViewModel model,
            Calculo calculo,
            double tolerancia,
            int iteraciones,
            bool reglaFalsa)
        {
            double xi =
                model.Xi.Value;

            double xd =
                model.Xd.Value;

            double fXi =
                calculo.EvaluaFx(xi);

            double fXd =
                calculo.EvaluaFx(xd);

            // Validar valores
            if (!EsNumeroValido(fXi) ||
                !EsNumeroValido(fXd))
            {
                model.MensajeError =
                    "La función no está definida en alguno de los extremos del intervalo.";

                model.Converge = false;

                return View("Index", model);
            }

            // Validar intervalo
            if (fXi * fXd > 0)
            {
                model.MensajeError =
                    "El intervalo ingresado no es válido. Debe volver a ingresar Xi y Xd.";

                model.Converge = false;

                return View("Index", model);
            }

            // Xi es raíz
            if (Math.Abs(fXi) < tolerancia)
            {
                model.Raiz = xi;
                model.Error = 0;
                model.IteracionesRealizadas = 0;
                model.Converge = true;

                return View("Index", model);
            }

            // Xd es raíz
            if (Math.Abs(fXd) < tolerancia)
            {
                model.Raiz = xd;
                model.Error = 0;
                model.IteracionesRealizadas = 0;
                model.Converge = true;

                return View("Index", model);
            }

            double xrAnterior = 0;
            double xr = 0;
            double error = 0;

            for (int i = 1;
                 i <= iteraciones;
                 i++)
            {
                // Bisección
                if (!reglaFalsa)
                {
                    xr =
                        (xi + xd) / 2;
                }

                // Regla Falsa
                else
                {
                    fXi =
                        calculo.EvaluaFx(xi);

                    fXd =
                        calculo.EvaluaFx(xd);

                    double denominador =
                        fXd - fXi;

                    if (Math.Abs(denominador) < 0.000000000001)
                    {
                        model.MensajeError =
                            "No se puede continuar porque se produjo una división por cero.";

                        model.Converge = false;

                        return View("Index", model);
                    }

                    xr =
                        (fXd * xi - fXi * xd)
                        / denominador;
                }

                // Calcular error
                if (i == 1)
                {
                    error =
                        double.MaxValue;
                }
                else if (xr != 0)
                {
                    error =
                        Math.Abs(
                            (xr - xrAnterior) / xr
                        );
                }
                else
                {
                    error =
                        Math.Abs(
                            xr - xrAnterior
                        );
                }

                double fXr =
                    calculo.EvaluaFx(xr);

                if (!EsNumeroValido(fXr))
                {
                    model.MensajeError =
                        "La función no puede evaluarse en el valor calculado.";

                    model.Converge = false;

                    return View("Index", model);
                }

                // Criterio de paro
                if (Math.Abs(fXr) < tolerancia ||
                    error < tolerancia)
                {
                    model.Raiz = xr;

                    model.Error =
                        error == double.MaxValue
                        ? 0
                        : error;

                    model.IteracionesRealizadas = i;

                    model.Converge = true;

                    return View("Index", model);
                }

                fXi =
                    calculo.EvaluaFx(xi);

                // Mover extremos
                if (fXi * fXr > 0)
                {
                    xi = xr;
                }
                else
                {
                    xd = xr;
                }

                xrAnterior = xr;
            }

            model.Raiz = xr;

            model.Error =
                error == double.MaxValue
                ? null
                : (double?)error;

            model.IteracionesRealizadas =
                iteraciones;

            model.Converge = false;

            return View("Index", model);
        }

        // Newton-Raphson
        private ActionResult ResolverNewton(
            Unidad1ViewModel model,
            Calculo calculo,
            double tolerancia,
            int iteraciones)
        {
            double xAnterior =
                model.X0.Value;

            double xActual =
                xAnterior;

            double error = 0;

            for (int i = 1;
                 i <= iteraciones;
                 i++)
            {
                double fx =
                    calculo.EvaluaFx(xAnterior);

                // Si el punto inicial ya es raíz
                if (Math.Abs(fx) < tolerancia)
                {
                    model.Raiz =
                        xAnterior;

                    model.Error =
                        0;

                    model.IteracionesRealizadas =
                        i - 1;

                    model.Converge =
                        true;

                    return View("Index", model);
                }

                // Calcular derivada numérica
                double derivada =
                    DerivadaNumerica(
                        calculo,
                        xAnterior
                    );

                // Validar función y derivada
                if (!EsNumeroValido(fx) ||
                    !EsNumeroValido(derivada))
                {
                    model.MensajeError =
                        "Newton-Raphson no puede continuar porque la función o la derivada no están definidas en el punto actual.";

                    model.Raiz = null;
                    model.Error = null;
                    model.IteracionesRealizadas = i - 1;
                    model.Converge = false;

                    return View("Index", model);
                }

                /*
                 * Si la derivada numérica es extremadamente grande,
                 * puede indicar que la derivada no está definida.
                 *
                 * Esto ocurre en el ejercicio con:
                 * cbrt(x-2)-0.7*x+2
                 * X0 = 2
                 */
                if (Math.Abs(derivada) > 1000)
                {
                    model.MensajeError =
                        "Newton-Raphson no puede continuar porque la derivada no está definida en el punto actual.";

                    model.Raiz = null;
                    model.Error = null;
                    model.IteracionesRealizadas = i - 1;
                    model.Converge = false;

                    return View("Index", model);
                }

                // Derivada cero o muy cercana a cero
                if (Math.Abs(derivada) < 0.000000000001)
                {
                    model.MensajeError =
                        "Newton-Raphson no puede continuar porque la derivada es cero o muy cercana a cero.";

                    model.Raiz = null;
                    model.Error = null;
                    model.IteracionesRealizadas = i - 1;
                    model.Converge = false;

                    return View("Index", model);
                }

                // Fórmula Newton-Raphson
                xActual =
                    xAnterior -
                    (fx / derivada);

                // Validar resultado
                if (!EsNumeroValido(xActual))
                {
                    model.MensajeError =
                        "Newton-Raphson produjo un resultado numérico inválido.";

                    model.Raiz = null;
                    model.Error = null;
                    model.IteracionesRealizadas = i;
                    model.Converge = false;

                    return View("Index", model);
                }

                // Calcular error
                if (xActual != 0)
                {
                    error =
                        Math.Abs(
                            (xActual - xAnterior)
                            / xActual
                        );
                }
                else
                {
                    error =
                        Math.Abs(
                            xActual - xAnterior
                        );
                }

                double fActual =
                    calculo.EvaluaFx(xActual);

                if (!EsNumeroValido(fActual))
                {
                    model.MensajeError =
                        "La función no puede evaluarse en el valor obtenido.";

                    model.Raiz = null;
                    model.Error = null;
                    model.IteracionesRealizadas = i;
                    model.Converge = false;

                    return View("Index", model);
                }

                /*
                 * IMPORTANTE:
                 *
                 * No aceptamos solamente:
                 * error < tolerancia
                 *
                 * porque X puede cambiar muy poco
                 * y aun así NO ser una raíz.
                 *
                 * Para aceptar la raíz exigimos
                 * que f(x) sea cercana a cero.
                 */
                if (Math.Abs(fActual) < tolerancia)
                {
                    model.Raiz =
                        xActual;

                    model.Error =
                        error;

                    model.IteracionesRealizadas =
                        i;

                    model.Converge =
                        true;

                    return View("Index", model);
                }

                xAnterior =
                    xActual;
            }

            // No convergió
            model.Raiz = null;

            model.Error =
                error;

            model.IteracionesRealizadas =
                iteraciones;

            model.Converge =
                false;

            model.MensajeError =
                "Newton-Raphson alcanzó el máximo de iteraciones sin encontrar una raíz.";

            return View("Index", model);
        }

        // Secante
        private ActionResult ResolverSecante(
            Unidad1ViewModel model,
            Calculo calculo,
            double tolerancia,
            int iteraciones)
        {
            double xAnterior =
                model.X0.Value;

            double xActual =
                model.X1.Value;

            double xNuevo =
                xActual;

            double error = 0;

            for (int i = 1;
                 i <= iteraciones;
                 i++)
            {
                double fAnterior =
                    calculo.EvaluaFx(xAnterior);

                double fActual =
                    calculo.EvaluaFx(xActual);

                if (!EsNumeroValido(fAnterior) ||
                    !EsNumeroValido(fActual))
                {
                    model.MensajeError =
                        "La función no puede evaluarse en uno de los valores utilizados.";

                    model.Converge = false;

                    return View("Index", model);
                }

                double denominador =
                    fActual - fAnterior;

                // División por cero
                if (Math.Abs(denominador) <
                    0.000000000001)
                {
                    model.MensajeError =
                        "El método de la Secante no puede continuar porque f(X1) - f(X0) es cero o muy cercano a cero.";

                    model.Raiz = null;
                    model.Error = null;
                    model.IteracionesRealizadas = i - 1;
                    model.Converge = false;

                    return View("Index", model);
                }

                // Fórmula Secante
                xNuevo =
                    xActual -
                    (
                        fActual *
                        (xActual - xAnterior)
                    )
                    / denominador;

                if (!EsNumeroValido(xNuevo))
                {
                    model.MensajeError =
                        "El método produjo un resultado numérico inválido.";

                    model.Raiz = null;
                    model.Converge = false;

                    return View("Index", model);
                }

                // Calcular error
                if (xNuevo != 0)
                {
                    error =
                        Math.Abs(
                            (xNuevo - xActual)
                            / xNuevo
                        );
                }
                else
                {
                    error =
                        Math.Abs(
                            xNuevo - xActual
                        );
                }

                double fNuevo =
                    calculo.EvaluaFx(xNuevo);

                if (!EsNumeroValido(fNuevo))
                {
                    model.MensajeError =
                        "La función no puede evaluarse en el nuevo valor obtenido.";

                    model.Converge = false;

                    return View("Index", model);
                }

                // Criterio de paro
                if (Math.Abs(fNuevo) < tolerancia ||
                    error < tolerancia)
                {
                    model.Raiz =
                        xNuevo;

                    model.Error =
                        error;

                    model.IteracionesRealizadas =
                        i;

                    model.Converge =
                        true;

                    return View("Index", model);
                }

                xAnterior =
                    xActual;

                xActual =
                    xNuevo;
            }

            model.Raiz =
                xNuevo;

            model.Error =
                error;

            model.IteracionesRealizadas =
                iteraciones;

            model.Converge =
                false;

            return View("Index", model);
        }

        // Derivada numérica
        private double DerivadaNumerica(
            Calculo calculo,
            double x)
        {
            const double h =
                0.000001;

            double fDerecha =
                calculo.EvaluaFx(x + h);

            double fIzquierda =
                calculo.EvaluaFx(x - h);

            return
                (fDerecha - fIzquierda)
                / (2 * h);
        }

        // Validar número
        private bool EsNumeroValido(
            double numero)
        {
            return
                !double.IsNaN(numero) &&
                !double.IsInfinity(numero);
        }
    }
}