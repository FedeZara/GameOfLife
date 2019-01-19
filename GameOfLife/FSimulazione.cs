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
        public const int DimCasella = 40;
        private int IntervalloCarote;

        public PictureBox[,] CellePB;

        private Panel Pannello;

        public FSimulazione(int wGriglia, int hGriglia, int numConigli, int numLupi, int numCarote, int intervalloCarote)
        {
            InitializeComponent();

            Pannello = new Panel();
            Pannello.Location = new Point(0, 0);
            Pannello.Size = new Size(wGriglia * DimCasella, 60);
            Pannello.BackColor = Color.White;
            UCImpostazioni Impostazioni = new UCImpostazioni();
            Impostazioni.Location = new Point(0, 0);
            Impostazioni.Pausa += Pausa;
            Impostazioni.Avvia += Avvia;
            Impostazioni.Stoppa += Stoppa;
            Pannello.Controls.Add(Impostazioni);
            this.Controls.Add(Pannello);


            Griglia = new Griglia(wGriglia, hGriglia);

            Griglia.FineSimulazione += Risultati;

            Griglia.Inizializza(numConigli, numLupi, numCarote, intervalloCarote);

            IntervalloCarote = intervalloCarote;
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        
            CellePB = new PictureBox[wGriglia, hGriglia];
            for (int x = 0; x < wGriglia; x++)
            {
                for (int y = 0; y < hGriglia; y++)
                {
                    Griglia.Celle[x, y].CellaAggiornata += AggiornaCella;
                    
                    CellePB[x,y] = new PictureBox();
                    CellePB[x, y].Size = new Size(DimCasella, DimCasella);
                    CellePB[x, y].Location = new Point(x * DimCasella, 60 + y * DimCasella);
                    CellePB[x, y].SizeMode = PictureBoxSizeMode.StretchImage;
                    if (Griglia.Celle[x, y].Elemento is CConiglio || Griglia.Celle[x, y].Elemento is CLupo)
                    {
                        CellePB[x, y].BackColor = ColoreVita(10);
                    }
                    CellePB[x, y].Image = ImmagineElemento(Griglia.Celle[x, y].Elemento);
           
                    this.Controls.Add(CellePB[x, y]);
                }
            }
        }

        private Color ColoreVita(int vita)
        {
            return Color.FromArgb(100, (10 - vita) * 255 / 10, vita * 255 / 10, 0);
        }

        private Image ImmagineElemento(CElemento elemento)
        {
            if (elemento is CCarota)
                return GameOfLife.Properties.Resources.carota;
            else if (elemento is CLupo)
                return GameOfLife.Properties.Resources.lupo;
            else if (elemento is CConiglio)
                return GameOfLife.Properties.Resources.coniglio;
            return null;
        }


        public void AggiornaCella(object sender, CellaAggiornataEventArgs e)
        {
            BeginInvoke((Action)(() => CellePB[e.X, e.Y].Image = ImmagineElemento(e.Elemento)));

            var elemento = e.Elemento as CAnimale;
            if(elemento == null)
            {
                BeginInvoke((Action)(() => CellePB[e.X, e.Y].BackColor = Color.Transparent));
            }
            else
            {
                BeginInvoke((Action)(() => CellePB[e.X, e.Y].BackColor = ColoreVita(elemento.Vita)));
            }
        }

        public async void Risultati(object sender, FineSimulazioneEventArgs e)
        {
            await Task.Delay(1000);
            FRisultati Risultati = new FRisultati(e.ConigliRimasti, e.LupiRimasti);
            this.Hide();
            Risultati.ShowDialog();
            this.Close();
        }     

        public void Pausa(object sender, EventArgs e)
        {
            Griglia.Pausa();
        }

        public void Avvia(object sender, EventArgs e)
        {
            Griglia.Avvia();
        }

        public void Stoppa(object sender, EventArgs e)
        {
            Griglia.Stoppa();
        }

        private void FSimulazione_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(!Griglia.SimulazioneFinita)
                Griglia.Stoppa();
        }
    }
}
