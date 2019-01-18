using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameOfLife
{
    public partial class UCImpostazioni : UserControl
    {
        private bool InEsecuzione;
        public UCImpostazioni()
        {
            InitializeComponent();
            InEsecuzione = false;
        }

        private void btnPlayPause_Click(object sender, EventArgs e)
        {
            if (InEsecuzione)
            {
                btnPlayPause.Image = GameOfLife.Properties.Resources.play_button;
                
               
            }
            else
            {
                btnPlayPause.Image = GameOfLife.Properties.Resources.pause;
                
                
            }
            InEsecuzione = !InEsecuzione;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            
        }
    }
}
