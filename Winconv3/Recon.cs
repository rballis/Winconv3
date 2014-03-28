using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Winconv3
{
    class Recon
    {
        private Exception m_ex;

        // Klasse Recnen
        public Recon()
        {
        }

        // Rechnen
        public double recon(string sValueFrom, string sEquation)
        {
            // Klasse Equation Instanz erzeugen
            Equation cEquation = new Equation();

            // Token aus Klasse Equation, auflösen und ergebniss zurück
            return disolvToken(cEquation.disolv(sValueFrom, sEquation));
        }

        // Exception zurückliefern
        public Exception getException()
        {
            return m_ex;
        }

        // Token auflösen
        private double disolvToken(List<object> lToken)
        {
            // Var. Wert
            double dValueTo = 0;
            // Var. der Math. Funktion
            string sFunction = "";

            try
            {
                // Iteration durch Token
                foreach (object token in lToken)
                {
                    // Prüfen, welche Element ist Token...
                    if (token is double)
                    {
                        //... ist Wert double, Berechnen mit Wert, dem vorherigen Var. Wert, der Funktion
                        // und Ergebniss an Var. Wert zuweisen
                        dValueTo = calculatToken(sFunction, dValueTo, (double)token);
                    }
                    else if (token is string)
                    {
                        //... ist Funktion, an Var. Math. Funktion zuweisen
                        sFunction = (string)token;
                    }
                    else if (token is object)
                    {
                        //... ist neues Token, Token auflösen, Ergebniss an Var. Wert zuweisen
                        dValueTo = calculatToken(sFunction, dValueTo, disolvToken((List<object>)token));
                    }
                }
            }
            catch (Exception ex)
            {
                m_ex = ex;
                return 0;
            }

            // Rückgabe Var. Wert
            return dValueTo;
        }

        // Token berechnen
        private double calculatToken(string sFunction, double dValueA, double dValueB)
        {
            // Var. Wert
            double dValueTo = 0;

            // Schalter Funktion, entsprechend der Werte Rechnen
            switch (sFunction)
            {
                case "+":
                    dValueTo = dValueA + dValueB;
                    break;

                case "-":
                    dValueTo = dValueA - dValueB;
                    break;

                case "*":
                    dValueTo = dValueA * dValueB;
                    break;

                case "/":
                    dValueTo = dValueA / dValueB;
                    break;

                case "^":
                    dValueTo = Math.Pow(dValueA, dValueB);
                    break;

                case "acos":
                    dValueTo = Math.Acos(dValueB);
                    break;

                case "asin":
                    dValueTo = Math.Asin(dValueB);
                    break;

                case "atan":
                    dValueTo = Math.Atan(dValueB);
                    break;

                case "atan2":
                    dValueTo = Math.Atan2(dValueA, dValueB);
                    break;

                case "cos":
                    dValueTo = Math.Cos(dValueB);
                    break;

                case "cosh":
                    dValueTo = Math.Cosh(dValueB);
                    break;

                case "exp":
                    dValueTo = Math.Exp(dValueB);
                    break;

                case "floor":
                    dValueTo = Math.Floor(dValueB);
                    break;

                case "log":
                    dValueTo = Math.Log(dValueB);
                    break;

                case "log10":
                    dValueTo = Math.Log10(dValueB);
                    break;

                case "sin":
                    dValueTo = Math.Sin(dValueB);
                    break;

                case "sinh":
                    dValueTo = Math.Sinh(dValueB);
                    break;

                case "sqrt":
                    dValueTo = Math.Sqrt(dValueB);
                    break;

                case "tan":
                    dValueTo = Math.Tan(dValueB);
                    break;

                case "tanh":
                    dValueTo = Math.Tanh(dValueB);
                    break;

                default:
                    dValueTo = dValueB;
                    break;
            }

            // Rückgabe Var. Wert
            return dValueTo;
        }
    }
}
