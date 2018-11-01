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
                (Parent.Parent as FSimulazione).Pause();
                btnNext.Enabled = true;
            }
            else
            {
                btnPlayPause.Image = GameOfLife.Properties.Resources.pause;
                (Parent.Parent as FSimulazione).Play();
                btnNext.Enabled = false;
            }
            InEsecuzione = !InEsecuzione;
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            (Parent.Parent as FSimulazione).TimerStep(sender, e);
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            (Parent.Parent as FSimulazione).Stop();
        }
    }
}
