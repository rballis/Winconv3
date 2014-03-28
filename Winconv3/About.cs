using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Winconv3
{
    public partial class About : Form
    {
        private string m_sPath = "";

        // Klasse About
        public About()
        {
            // Init Comp.
            InitializeComponent();
        }

        public void setXmlFile(string sPath)
        {
            // Dateiname an TextFeld übergeben
            xmlFile.Text = sPath;
        }

        // Aufruf Dialog schliessen
        private void cloes_Click(object sender, EventArgs e)
        {
            // Dialog schliessen
            this.Close();
        }
    }
}
