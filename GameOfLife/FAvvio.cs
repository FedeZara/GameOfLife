using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameOfLife
{
    public partial class FAvvio : Form
    {
        private int numConigli;
        private int numLupi;
        private int numCarote;
        private int wGriglia;
        private int hGriglia;
        public FAvvio()
        {
            InitializeComponent();
            numCarote = (int)nudCarote.Value;
            numLupi = (int)nudLupi.Value;
            numConigli = (int)nudConigli.Value;
            wGriglia = (int)nudWGriglia.Value;
            hGriglia = (int)nudHGriglia.Value;
        }

        private void btnInizia_Click(object sender, EventArgs e)
        {
            numCarote = (int)nudCarote.Value;
            numLupi = (int)nudLupi.Value;
            numConigli = (int)nudConigli.Value;
            wGriglia = (int)nudWGriglia.Value;
            hGriglia = (int)nudHGriglia.Value;
            FSimulazione Gioco = new FSimulazione(wGriglia, hGriglia, numConigli, numLupi, numCarote, (int)nudIntervalloCarote.Value);
            this.Hide();
            Gioco.ShowDialog();
            this.Close();
        }

        private void nud_ValueChanged(object sender, EventArgs e)
        {
            int numCaselle = (int)(nudWGriglia.Value * nudHGriglia.Value);
            if((int)(nudCarote.Value + nudLupi.Value + nudConigli.Value) > numCaselle / 2){
                nudCarote.Value = numCarote;
                nudConigli.Value = numConigli;
                nudLupi.Value = numLupi;
                nudWGriglia.Value = wGriglia;
                nudHGriglia.Value = hGriglia;
            }
            else
            {
                numCarote = (int)nudCarote.Value;
                numLupi = (int)nudLupi.Value;
                numConigli = (int)nudConigli.Value;
                wGriglia = (int)nudWGriglia.Value;
                hGriglia = (int)nudHGriglia.Value;
            }
        }
    }
}
