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
    public partial class FRisultati : Form
    {
        public FRisultati(int ConigliRimasti, int LupiRimasti)
        {
            InitializeComponent();

            if(ConigliRimasti == 0 && LupiRimasti == 0)
            {
                lblRisultato.Text = "Pareggio!";
            }
            else if(ConigliRimasti == 0)
            {
                lblRisultato.Text = "I lupi hanno vinto!";
            }
            else if (LupiRimasti == 0)
            {
                lblRisultato.Text = "I conigli hanno vinto!";
            }
            else
            {
                lblRisultato.Text = "Simulazione interrotta!";
            }

            lblNumConigli.Text = ConigliRimasti + " conigli rimasti";
            lblNumLupi.Text = LupiRimasti + " lupi rimasti";
        }

        private void btnChiudi_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnNuovaSimulazione_Click(object sender, EventArgs e)
        {
            FAvvio Avvio = new FAvvio();
            this.Hide();
            Avvio.ShowDialog();
            this.Close();
        }
    }
}
