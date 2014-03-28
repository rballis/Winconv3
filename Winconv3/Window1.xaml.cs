using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Xml.Linq;

namespace Winconv3
{
    /// <summary>
    /// Interaktionslogik für Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private ReadXML m_cReadXml = null;
        private XElement m_cXElement = null;
        private string m_sEquation = "";
        private string m_sXmlFile = "";

        public Window1()
        {
            // Initialisierung
            InitializeComponent();
            // XML Datei lesen
            m_cReadXml = new ReadXML();

            // Komponenten ausschalten
            converting.IsEnabled = false;
            valueFrom.IsEnabled = false;
            cleareFrom.IsEnabled = false;
            unitFrom.IsEnabled = false;
            interChange.IsEnabled = false;
            calculate.IsEnabled = false;
            valueTo.IsEnabled = false;
            cleareTo.IsEnabled = false;
            unitTo.IsEnabled = false;
            decA.IsEnabled = false;
            dec0.IsEnabled = false;
            dec3.IsEnabled = false;
            dec6.IsEnabled = false;
            dec9.IsEnabled = false;

            // Init
            init();
        }

        // Init...
        private void init()
        {
            bool bEquationFile = false;
            bool bEquationFileParam = false;
            // Aufrufparameter einholen
            string[] args = Environment.GetCommandLineArgs();

            // Prüfen, ob zusätzliche Aufrufparameter angegeben wurden...
            if (args.Length > 1)
            {
                //... wenn ja, Parameter einzeln auslesen...
                for (int i = 1; i < args.Length; i++)
                {
                    //... wenn ext. Eq. Datei...
                    if (args[i].ToLower().Equals("-equationfile"))
                    {
                        //... Flaggen setzen
                        bEquationFileParam = true;
                        bEquationFile = true;
                        // weiter
                        continue;
                    }

                    // wenn Flagge gesetzt, wird jetzt Eq. Datei Name als Parameter erwartet...
                    if (bEquationFileParam)
                    {
                        //... in Memb. Var. schreiben
                        m_sXmlFile = args[i];
                        // Flagge zurücksetzen
                        bEquationFileParam = false;
                    }
                }
            }

            // wenn ext. Eq. Datei Flagge nicht gesetzt...
            if (!bEquationFile)
            {
                //... Datei wird im Arbeitsverzeichniss erwartet
                m_sXmlFile = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Winconv3.xml");
            }

            // XElemente einholen
            if ((m_cXElement = m_cReadXml.getXElement(m_sXmlFile)) == null)
            {
                return;
            }

            try
            {
                // aus XElemente das Tag "name" auslesen
                var rXML = from xml in m_cXElement.Descendants()
                           where (string)xml.Attribute("name") != null
                           select new
                           {
                               converting = (string)xml.Attribute("name")
                           };

                // Erstes Element in ComboBox Converting schreiben
                converting.Items.Add("<Chose>");

                // Iteration durch alle Elemente...
                foreach (var xml in rXML)
                {
                    if (xml.converting != null)
                    {
                        //... an ComboBox converting anhängen
                        converting.Items.Add(xml.converting);
                    }
                }
            }
            catch (Exception ex)
            {
                return;
            }

            // wenn Elemente in ComboBox converting...
            if (converting.Items.Count > 1)
            {
                //... ComboBox aktivieren
                setConvertingOn(true);
            }
        }

        // Selektion ComboBox converting
        private void converting_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Flagge für gefundenes Element
            bool bMatchConverting = false;

            // ComboBox unitFrom Elemente löschen
            unitFrom.Items.Clear();

            // wenn keine Elemente in ComboBox converting gewählt...
            if (converting.SelectedIndex == 0)
            {
                //... Elemente wieder ausschalten
                setFromOn(false);
                setToOn(false);
                return;
            }

            try
            {
                // aus XElemente "name" und "nameFrom" auslesen
                var rXML = from xml in m_cXElement.Descendants()
                           select new
                           {
                               name = (string)xml.Attribute("name"),
                               nameFrom = (string)xml.Attribute("nameFrom")
                           };

                //Erstes Element in ComboBox unitFrom eintragen
                unitFrom.Items.Add("<Chose>");

                // Iteration durch alle Elemente
                foreach (var xml in rXML)
                {
                    if (xml.name != null)
                    {
                        // wenn Element mit selktierten converting übereinstimmt...
                        if (xml.name == (string)converting.SelectedValue)
                        {
                            //... Flagge setzen
                            bMatchConverting = true;
                        }
                        else
                        {
                            //... Flagge einholen
                            bMatchConverting = false;
                        }
                    }

                    // wenn Flagge gesetzt
                    if (bMatchConverting)
                    {
                        if (xml.nameFrom != null)
                        {
                            // Element in ComboBox unitFrom eintragen
                            unitFrom.Items.Add(xml.nameFrom);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return;
            }

            // Wenn keine Elemente in ComboBox unitFrom eingetragen...
            if (unitFrom.Items.Count > 1)
            {
                // Elemente wieder ausschalten
                setFromOn(true);
            }
        }

        // Lösschalter für valueFrom
        private void cleareFrom_Click(object sender, RoutedEventArgs e)
        {
            // Eintrag löschen
            valueFrom.Clear();
            // Focus für neuen Wert setzen
            valueFrom.Focus();
        }

        // Lösschalter für valueTo
        private void cleareTo_Click(object sender, RoutedEventArgs e)
        {
            // Eintrag löschen
            valueTo.Clear();
        }

        // Selektion unitFrom
        private void unitFrom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Aufruf Methode unitFromChange
            unitFromChange();
        }

        // Methode unitFromChange
        private void unitFromChange()
        {
            // Match- Flaggen
            bool bMatchConverting = false;
            bool bMatchFrom = false;

            // Einträge aus ComboBox unitFrom löschen
            unitTo.Items.Clear();

            // wenn keine Elemente aus ComboBox unitFrom gewählt...
            if (unitFrom.SelectedIndex == 0)
            {
                // Elemente wieder ausschalten
                setToOn(false);
                return;
            }

            try
            {
                // aus XElemente "name" und "nameFrom" und "nameTo" auslesen
                var rXML = from xml in m_cXElement.Descendants()
                           select new
                           {
                               name = (string)xml.Attribute("name"),
                               nameFrom = (string)xml.Attribute("nameFrom"),
                               nameTo = (string)xml.Attribute("nameTo")
                           };

                //Erstes Element in ComboBox unitTo eintragen
                unitTo.Items.Add("<Chose>");

                // Iteartion durch alle Elemente
                foreach (var xml in rXML)
                {
                    if (xml.name != null)
                    {
                        // wenn Element mit selktierten converting übereinstimmt...
                        if (xml.name == (string)converting.SelectedValue)
                        {
                            //... Flagge setzen
                            bMatchConverting = true;
                        }
                        else
                        {
                            //... Flagge einholen
                            bMatchConverting = false;
                        }
                    }

                    if (xml.nameFrom != null)
                    {
                        // wenn Element mit selktierten unitFrom übereinstimmt...
                        if (xml.nameFrom == (string)unitFrom.SelectedValue)
                        {
                            //... Flagge setzen
                            bMatchFrom = true;
                        }
                        else
                        {
                            //... Flagge einholen
                            bMatchFrom = false;
                        }
                    }

                    // wenn beide Flaggen gesetzt...
                    if (bMatchConverting && bMatchFrom)
                    {
                        if (xml.nameTo != null)
                        {
                            //... Element in ComboBox unitTo eintragen
                            unitTo.Items.Add(xml.nameTo);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return;
            }

            // Wenn keine Elemente in ComboBox unitFrom eingetragen...
            if (unitTo.Items.Count > 1)
            {
                // Elemente wieder ausschalten
                setToOn(true);
            }
        }

        // Selektion unitTo
        private void unitTo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Match- Flaggen
            bool bMatchConverting = false;
            bool bMatchFrom = false;

            // wenn keine Elemente aus ComboBox unitFrom gewählt...
            if (unitTo.SelectedIndex == 0)
            {
                //... Memb. Var. für Formel löschen
                m_sEquation = "";
                return;
            }

            try
            {
                // aus XElemente "name" und "nameFrom" und "nameTo" und "equation" auslesen
                var rXML = from xml in m_cXElement.Descendants()
                           select new
                           {
                               name = (string)xml.Attribute("name"),
                               nameFrom = (string)xml.Attribute("nameFrom"),
                               nameTo = (string)xml.Attribute("nameTo"),
                               equation = (string)xml.Value
                           };

                // Iteration durch alle Elemente
                foreach (var xml in rXML)
                {
                    // wenn Element ist converting...
                    if (xml.name != null)
                    {
                        //... wenn Element mit selktierten converting übereinstimmt...
                        if ((!bMatchConverting) && (xml.name == (string)converting.SelectedValue))
                        {
                            // Flagge setzen
                            bMatchConverting = true;
                        }
                        // nächstes Element
                        continue;
                    }

                    // wenn Element ist unitFrom...
                    if (xml.nameFrom != null)
                    {
                        //... wenn Element mit selktierten unitFrom übereinstimmt...
                        if ((!bMatchFrom) && (xml.nameFrom == (string)unitFrom.SelectedValue))
                        {
                            // Flagge setzen
                            bMatchFrom = true;
                        }
                        // nächstes Element
                        continue;
                    }

                    // wenn Element ist unitTo...
                    if (xml.nameTo != null)
                    {
                        //... wenn Element mit selktierten unitTo übereinstimmt...
                        if (xml.nameTo == (string)unitTo.SelectedValue)
                        {
                            //... wenn beide Flaggen gesetzt...
                            if ((bMatchConverting) && (bMatchFrom))
                            {
                                if (xml.equation != null)
                                {
                                    //... Memb. Var. Formel einholen
                                    m_sEquation = xml.equation;
                                    // Rechnen und Ergebnis ausgeben
                                    valueTo.Text = recon(valueFrom.Text.ToString(), m_sEquation);
                                }
                                // Ende der Iteration
                                break;
                            }
                        }
                        // nächstes Element
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }

        // Austausch der Value von valueTo nach valueFrom
        private void interChange_Click(object sender, RoutedEventArgs e)
        {
            // Wert austauschen
            valueFrom.Text = valueTo.Text;
            // Feldinhalt löschen
            valueTo.Clear();
            // ComboBox Inhalt austauschen
            unitFrom.SelectedValue = unitTo.SelectedValue;

            // Prüfen, ComboBox Inhalt austauschbar...
            if (unitFrom.SelectedIndex == -1)
            {
                //... wenn nicht, CombOBox auf ersten Wert setzen
                unitFrom.SelectedIndex = 0;
                // und zurück
                return;
            }

            // ernwuter Aufruf der Methode
            unitFromChange();
        }

        // Rechnen
        private void calculate_Click(object sender, RoutedEventArgs e)
        {
            // Rechnen, Ergebniss einholen und setzen
            valueTo.Text = recon(valueFrom.Text.ToString(), m_sEquation);
        }

        // Aktiv- Deaktivierung der Felder
        private void setConvertingOn(bool bOn)
        {
            // wenn ein...
            if (bOn)
            {
                //... erster Eintrag der ComboBox
                converting.SelectedIndex = 0;
            }

            converting.IsEnabled = bOn;
        }

        // Aktiv- Deaktivierung der Felder
        private void setFromOn(bool bOn)
        {
            // wenn ein...
            if (!bOn)
            {
                //... nicht ein, ComboBox Inhalt löschen
                valueFrom.Clear();
            }
            else
            {
                //... erster Eintrag der ComboBox
                unitFrom.SelectedIndex = 0;
            }

            // weitere Felder nach parameter bOn schalten
            valueFrom.IsEnabled = bOn;
            cleareFrom.IsEnabled = bOn;
            unitFrom.IsEnabled = bOn;
        }

        // Aktiv- Deaktivierung der Felder
        private void setToOn(bool bOn)
        {
            // weinn ein...
            if (!bOn)
            {
                //... nicht ein, ComboBox Inhalt löschen
                valueTo.Clear();
            }
            else
            {
                //... erster Eintrag der ComboBox
                unitTo.SelectedIndex = 0;
                //... decimal auf Floatingpoint setzen
                decA.IsChecked = true;
            }

            // weitere Felder nach parameter bOn schalten
            interChange.IsEnabled = bOn;
            calculate.IsEnabled = bOn;
            valueTo.IsEnabled = bOn;
            cleareTo.IsEnabled = bOn;
            unitTo.IsEnabled = bOn;
            decA.IsEnabled = bOn;
            dec0.IsEnabled = bOn;
            dec3.IsEnabled = bOn;
            dec6.IsEnabled = bOn;
            dec9.IsEnabled = bOn;
        }

        // Rechnen
        private string recon(string sValueFrom, string sEquation)
        {
            // Ext. Klasse Recon
            Recon cRecon = null;
            // String Umrechnungszahl
            string sValueTo = "";

            // Prüfen ob Umrechnungszahl und Formel einen Inhalt haben...
            if ((sValueFrom.Length == 0) || (sEquation.Length == 0))
            {
                //... wenn nicht, zurück
                return "";
            }

            // Prüfen ob Umrechnungszahl gültig...
            if (!checkValue(sValueFrom))
            {
                //... wenn nicht, Fehler in Ergebniss schreiben und zurück
                return "[ERROR]";
            }

            // Klasse Recon Instanz erzeugen
            if ((cRecon = new Recon()) != null)
            {
                // Dezimalzahl gewünscht...
                if (dec0.IsChecked.Value)
                {
                    //... rechnen, keine Dezimalzahl ausgeben
                    sValueTo = cRecon.recon(sValueFrom, sEquation).ToString("0");
                }
                else if (dec3.IsChecked.Value)
                {
                    //... rechnen, drei Stellen Dezimalzahl ausgeben
                    sValueTo = cRecon.recon(sValueFrom, sEquation).ToString("0.000");
                }
                else if (dec6.IsChecked.Value)
                {
                    //... rechnen, sechs Stellen Dezimalzahl ausgeben
                    sValueTo = cRecon.recon(sValueFrom, sEquation).ToString("0.000000");
                }
                else if (dec9.IsChecked.Value)
                {
                    //... rechnen, neun Stellen Dezimalzahl ausgeben
                    sValueTo = cRecon.recon(sValueFrom, sEquation).ToString("0.000000000");
                }
                else
                {
                    //... rechnen, Fliesskomma ausgeben
                    sValueTo = cRecon.recon(sValueFrom, sEquation).ToString();
                }
            }

            // Ergebniss zurück
            return sValueTo;
        }

        // Zahl prüfen
        private bool checkValue(string sValue)
        {
            // Dezimal aus System abrufen
            string decimalSeperator = Regex.Escape(Thread.CurrentThread.CurrentUICulture.NumberFormat.NumberDecimalSeparator);

            // Pattern zusammenbauen inkl. Dezimal aus System
            string pattern = @"^\d{1,}" + decimalSeperator + @"{0,1}\d{0,}";

            // Regulären Ausdruck der Dezimal zeichen
            Regex rgx = new Regex(decimalSeperator, RegexOptions.IgnoreCase);
            // Zählen der Vorkommen
            MatchCollection matches = rgx.Matches(sValue);

            // wenn mehr als ain Dezimal Zeichen...
            if (matches.Count > 1)
            {
                //... mit false zurück
                return false;
            }

            // Ausdruck numerisch mit pattern prüfen und Ergebniss zurück
            return Regex.Match(sValue, pattern).Success;
        }

        // Auswahl About
        private void about_Click(object sender, RoutedEventArgs e)
        {
            // Klasse About Dialog
            About cAbout = new About();

            // XML Datei an Klasse übergeben
            cAbout.setXmlFile(m_sXmlFile);
            // Dialog modal zeigen
            cAbout.ShowDialog();
        }

        // Auswahl Fliesskomma
        private void decA_Checked(object sender, RoutedEventArgs e)
        {
            // Dezimal schalten
            decChanged();
        }

        // Auswahl keine Dezimal Stelle
        private void dec0_Checked(object sender, RoutedEventArgs e)
        {
            // Dezimal schalten
            decChanged();
        }

        // Auswahl drei Dezimal Stellen
        private void dec3_Checked(object sender, RoutedEventArgs e)
        {
            // Dezimal schalten
            decChanged();
        }

        // Auswahl sechs Dezimal Stellen
        private void dec6_Checked(object sender, RoutedEventArgs e)
        {
            // Dezimal schalten
            decChanged();
        }

        // Auswahl neun Dezimal Stellen
        private void dec9_Checked(object sender, RoutedEventArgs e)
        {
            // Dezimal schalten
            decChanged();
        }

        // Dezimal schalten
        private void decChanged()
        {
            // Prüfen ob Attribute aus den Feldern einen Inhalt habne...
            if ((valueFrom.Text.Length == 0) || (valueTo.Text.Length == 0) ||
                (unitFrom.SelectedIndex == 0) || (unitTo.SelectedIndex == 0) || (m_sEquation.Length == 0))
            {
                // wenn nicht, zurück
                return;
            }

            // Rechnen
            valueTo.Text = recon(valueFrom.Text.ToString(), m_sEquation);
        }
    }
}
