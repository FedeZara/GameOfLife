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
using System.Threading;

namespace GameOfLife
{ 

    public partial class FSimulazione : Form
    {
        public CElemento[,] Elementi { get; set; }
        
        public static int DimCasella = 30;
        public static Random rnd { get; set; }
        private int TimerCarota;
        private int IntervalloCarote;
        private Task CalcoloPosizioniTask;


        public FSimulazione(int wGriglia, int hGriglia, int numConigli, int numLupi, int numCarote, int intervalloCarote)
        {
            InitializeComponent();

            Elementi = new CElemento[wGriglia,hGriglia];

            Inizializza(wGriglia, hGriglia, numConigli, numLupi, numCarote, intervalloCarote);
        }

        public void Inizializza(int wGriglia, int hGriglia, int numConigli, int numLupi, int numCarote, int intervalloCarote)
        {
            TimerCarota = 0;
            IntervalloCarote = intervalloCarote;
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            int[,] Combinazioni = new int[wGriglia * hGriglia, 2];
            int[] indiciCombinazioni = new int[wGriglia * hGriglia];
            for(int x=0; x<wGriglia; x++)
            {
                for(int y=0; y<hGriglia; y++)
                {
                    Combinazioni[y + x * hGriglia, 0] = x;
                    Combinazioni[y + x * hGriglia, 1] = y;
                    indiciCombinazioni[y + x * hGriglia] = y + x * hGriglia;
                }
            }
            rnd = new Random();
            indiciCombinazioni = indiciCombinazioni.OrderBy(x => rnd.Next()).ToArray();
            int i = 0;
            for(int j=0; j<numConigli; j++)
            {
                Elementi[Combinazioni[indiciCombinazioni[i], 0], Combinazioni[indiciCombinazioni[i], 1]] = new CConiglio(Combinazioni[indiciCombinazioni[i], 0], Combinazioni[indiciCombinazioni[i], 1]);
                i++;
            }
            for (int j = 0; j < numLupi; j++)
            {
                Elementi[Combinazioni[indiciCombinazioni[i], 0], Combinazioni[indiciCombinazioni[i], 1]] = new CLupo(Combinazioni[indiciCombinazioni[i], 0], Combinazioni[indiciCombinazioni[i], 1]);
                i++;
            }
            for (int j = 0; j < numCarote; j++)
            {
                Elementi[Combinazioni[indiciCombinazioni[i], 0], Combinazioni[indiciCombinazioni[i], 1]] = new CCarota(Combinazioni[indiciCombinazioni[i], 0], Combinazioni[indiciCombinazioni[i], 1]);
                i++;
            }

            for (int x = 0; x < Elementi.GetLength(0); x++)
            {
                for (int y = 0; y < Elementi.GetLength(1); y++)
                {
                    if (!(Elementi[x, y] is null))
                    {
                        this.Controls.Add(Elementi[x, y].PB);
                    }
                }
            }

            Panel Pannello = new Panel();
            Pannello.Location = new Point(0, 0);
            Pannello.Size = new Size(wGriglia * DimCasella, 60);
            Pannello.BackColor = Color.White;
            UCImpostazioni Impostazioni = new UCImpostazioni();
            Impostazioni.Location = new Point(0, 0);
            Pannello.Controls.Add(Impostazioni);
            this.Controls.Add(Pannello);

            CalcoloPosizioniTask = Task.Run(() => CalcoloPosizioni());
        }


        public void Risultati(int ConigliRimasti, int LupiRimasti)
        {
            FRisultati Risultati = new FRisultati(ConigliRimasti, LupiRimasti);
            this.Hide();
            Risultati.ShowDialog();
            this.Close();
        }

        public void CalcoloPosizioni()
        {
            //Calcola una nuova posizione per ogni animale presente nella griglia
            bool[,] Occupato = new bool[Elementi.GetLength(0), Elementi.GetLength(1)];
            for (int x = 0; x<Elementi.GetLength(0); x++)
            {
                for (int y = 0; y<Elementi.GetLength(1); y++)
                {
                    if (Elementi[x, y] is CAnimale)
                    {
                        (Elementi[x, y] as CAnimale).Muovi(Elementi, Occupato);                
                    }
                }
            }
        }

        public void AggiornaPosizioni()
        {
            CalcoloPosizioniTask.Wait();
            CElemento[,] NuoviElementi = new CElemento[Elementi.GetLength(0), Elementi.GetLength(1)];
            for (int x = 0; x < Elementi.GetLength(0); x++)
            {
                for (int y = 0; y < Elementi.GetLength(1); y++)
                {
                    if (!(Elementi[x, y] is null))
                    {
                        if (Elementi[x, y].DaEliminare)
                        {
                            Controls.Remove(Elementi[x, y].PB);
                            Elementi[x, y].PB.Dispose();
                            Elementi[x, y] = null;
                            continue;
                        }
                        int newX = Elementi[x, y].X;
                        int newY = Elementi[x, y].Y;
                        NuoviElementi[newX, newY] = Elementi[x, y];

                        (Elementi[x, y] as CAnimale)?.MostraPosizioneCambiata();

                    }
                }
            }
            Elementi = NuoviElementi;
        }

        public void CalcolaNumeroAnimali(out int LupiTrovati, out int ConigliTrovati)
        {
            ConigliTrovati = 0;
            LupiTrovati = 0;
            for (int x = 0; x < Elementi.GetLength(0); x++)
            {
                for (int y = 0; y < Elementi.GetLength(1); y++)
                {
                    if (!(Elementi[x, y] is null))
                    {
                        //Verifica se sono rimasti ancora lupi o conigli
                        if (Elementi[x, y] is CLupo)
                            LupiTrovati++;
                        else if (Elementi[x, y] is CConiglio)
                            ConigliTrovati++;
                    }
                }
            }
        }
        public void VerificaFineSimulazione()
        {
            int LupiTrovati, ConigliTrovati;
            CalcolaNumeroAnimali(out LupiTrovati, out ConigliTrovati);
            if (ConigliTrovati == 0 || LupiTrovati == 0)
            {
                timerSimulazione.Stop();
                Risultati(ConigliTrovati, LupiTrovati);
            }
        }

        public void AggiungiNuovaCarota()
        {
            //Inserisce una nuova carota ogni tot (IntervalloCarote) secondi
            TimerCarota++;
            if (TimerCarota == IntervalloCarote)
            {
                TimerCarota = 0;
                int x, y;

                do
                {
                    x = rnd.Next(0, Elementi.GetLength(0));
                    y = rnd.Next(0, Elementi.GetLength(1));
                }
                while (!(Elementi[x, y] is null));
                Elementi[x, y] = new CCarota(x, y);
                this.Controls.Add(Elementi[x, y].PB);
            }
        }

        public void TimerStep(object sender, EventArgs e)
        {          
            AggiornaPosizioni();
            VerificaFineSimulazione();  
            AggiungiNuovaCarota();
            CalcoloPosizioniTask = Task.Run(() => CalcoloPosizioni());
        }
        public void Pause()
        {
            timerSimulazione.Stop();
        }

        public void Play()
        {
            timerSimulazione.Start();
        }

        public void Stop()
        {
            timerSimulazione.Stop();

            int LupiTrovati, ConigliTrovati;
            CalcolaNumeroAnimali(out LupiTrovati, out ConigliTrovati);

            Risultati(ConigliTrovati, LupiTrovati);
        }
    }
}
