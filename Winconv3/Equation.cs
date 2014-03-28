using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Linq;
using System.Text;

namespace Winconv3
{
    class Equation
    {
        // Memb. Var.
        private string[] m_sEquationPart;
        private int m_iEquation = 0;
        private Exception m_ex;

        // Klasse Equation
        public Equation()
        {
        }

        // Auflösung der Formel
        public List<object> disolv(string sValueFrom, string sEquation)
        {
            // String Equation in Kleinbuschstaben
            sEquation = sEquation.ToLower();
            // Platzhalter auflösen
            sEquation = sEquation.Replace("[i]", sValueFrom);
            sEquation = sEquation.Replace("[pi]", Math.PI.ToString());
            // Dezimalzeichen setzen
            sEquation = sEquation.Replace(".", Regex.Escape(Thread.CurrentThread.CurrentUICulture.NumberFormat.NumberDecimalSeparator));

            // Formelelemente in Array aufteilen
            m_sEquationPart = sEquation.Split('\x20');

            // Formelelemente auflösen und als Liste zurückgeben 
            return disolvEquation();
        }

        // Exception zurückliefern
        public Exception getException()
        {
            return m_ex;
        }

        // Formelenente auflösen
        private List<object> disolvEquation()
        {
            // List Objekt Token
            List<object> lToken = null;

            try
            {
                // Neues Token erzeugen
                lToken = new List<object>();

                // Iteration durch Formelelemente im Array
                while (m_iEquation < m_sEquationPart.Length)
                {
                    // Formelement prüfen...
                    if (isNumber(m_sEquationPart[m_iEquation]))
                    {
                        //... wenn Element eine Zahl, als double an Token einhängen
                        lToken.Add((double)Convert.ToDecimal(m_sEquationPart[m_iEquation]));
                    }
                    else if (m_sEquationPart[m_iEquation] == "(")
                    {
                        //... Wenn Element Klammerung öffnen, Elementenzähler addieren...
                        m_iEquation++;
                        //... weitere Formelement auflösen und mit neuen Token an Token anhängen
                        lToken.Add(disolvEquation());
                    }
                    else if (m_sEquationPart[m_iEquation] == ")")
                    {
                        //... Wenn Element Klammerung zu, zurück mit Token
                        return lToken;
                    }
                    else
                    {
                        //... Wenn Element Math. Funktion, Funktion als string an Token anhängen
                        lToken.Add((string)m_sEquationPart[m_iEquation]);
                    }

                    // Elementenzähler addieren
                    m_iEquation++;
                }
            }
            catch (Exception ex)
            {
                // Bei Exception, Var. in Memb. Var. scherieben
                m_ex = ex;
                // Token NULL zurück
                return null;
            }

            // Token zurück
            return lToken;
        }

        // Zahl prüfen
        private static bool isNumber(string source)
        {
            // Aus dem System das Dezimal Zeichen ausgeben
            string decimalSeperator = Regex.Escape(Thread.CurrentThread.CurrentUICulture.NumberFormat.NumberDecimalSeparator);

            // Pattern zusammensetzen, mit Dezimal Zeichen
            string pattern = @"^\d{1,}" + decimalSeperator + @"{0,1}\d{0,}";

            // Match prüfen und ergebniss zurück
            return Regex.Match(source, pattern).Success;
        }
    }
}
