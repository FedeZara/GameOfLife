using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameOfLife
{ 

    public partial class FSimulazione : Form
    {         
        public Griglia Griglia { get; set; }
        public const int DimCasella = 30;
        private int IntervalloCarote;


        public FSimulazione(int wGriglia, int hGriglia, int numConigli, int numLupi, int numCarote, int intervalloCarote)
        {
            InitializeComponent();

            Panel Pannello = new Panel();
            Pannello.Location = new Point(0, 0);
            Pannello.Size = new Size(wGriglia * DimCasella, 60);
            Pannello.BackColor = Color.White;
            UCImpostazioni Impostazioni = new UCImpostazioni();
            Impostazioni.Location = new Point(0, 0);
            Pannello.Controls.Add(Impostazioni);
            this.Controls.Add(Pannello);

            Griglia = new Griglia(wGriglia, hGriglia, Pannello.Controls);

            Griglia.FineSimulazione += Risultati;

            Griglia.Inizializza(numConigli, numLupi, numCarote, intervalloCarote);

            IntervalloCarote = intervalloCarote;
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;


            for (int x = 0; x < Griglia.Celle.GetLength(0); x++)
            {
                for (int y = 0; y < Griglia.Celle.GetLength(1); y++)
                {
                    if (!(Griglia.Celle[x, y] is null))
                    {
                        this.Controls.Add(Griglia.Celle[x, y].PB);
                    }
                }
            }

     

        }

        public void Risultati(object sender, FineSimulazioneEventArgs e)
        {
            FRisultati Risultati = new FRisultati(e.ConigliTrovati, e.LupiTrovati);
            this.Hide();
            Risultati.ShowDialog();
            this.Close();
        }     
    }
}
