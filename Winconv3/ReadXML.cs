using System;
using System.Xml.Linq;

namespace Winconv3
{
    class ReadXML
    {
        // Memb. Var.
        private Exception m_ex;

        public ReadXML()
        {
        }

        // XElemente einlesen
        public XElement getXElement(string sPath)
        {
            try
            {
                // XElemente einlesen, Dateiname aus Pfad erzeugen, laden und zurückgeben
                return XElement.Load(sPath);
            }
            catch (Exception ex)
            {
                // Exception Var. in Memb. Var. schreiben
                m_ex = ex;
                return null;
            }
        }

        // Rückgabe der Exception
        public Exception getException()
        {
            return m_ex;
        }
    }
}
