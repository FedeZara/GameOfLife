using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;

namespace GameOfLife
{
    public class FineSimulazioneEventArgs : EventArgs
    {
        public int ConigliTrovati { get; set; }
        public int LupiTrovati { get; set; }

        public FineSimulazioneEventArgs(int conigliTrovati, int lupiTrovati)
        {
            ConigliTrovati = conigliTrovati;
            LupiTrovati = lupiTrovati;
        }
    }
    public class Griglia
    {
        private int intervalloCarote;
        public Random rnd { get; set; }
        public CCella[,] Celle { get; set; }
        public int Larghezza { get; set; }
        public int Altezza { get; set; }
        public bool SimulazioneFinita { get; set; }
        private SemaphoreSlim simulazioneFinitaMutex = new SemaphoreSlim(1, 1);
        public Griglia(int larghezza, int altezza, Control.ControlCollection PBCollection)
        {
            Celle = new CCella[larghezza, altezza];
            Larghezza = larghezza;
            Altezza = altezza;

            for(int i=0; i<larghezza; i++)
            {
                for (int j = 0; j < altezza; j++)
                    Celle[i, j] = new CCella(i, j, PBCollection);
            }
        }

        public void Inizializza(int numConigli, int numLupi, int numCarote, int intervalloCarote)
        {
            this.intervalloCarote = intervalloCarote;
            int[,] Combinazioni = new int[Larghezza * Altezza, 2];
            int[] indiciCombinazioni = new int[Larghezza * Altezza];
            for (int x = 0; x < Larghezza; x++)
            {
                for (int y = 0; y < Altezza; y++)
                {
                    Combinazioni[y + x * Altezza, 0] = x;
                    Combinazioni[y + x * Altezza, 1] = y;
                    indiciCombinazioni[y + x * Altezza] = y + x * Altezza;
                }
            }
            rnd = new Random();
            indiciCombinazioni = indiciCombinazioni.OrderBy(x => rnd.Next()).ToArray();
            int i = 0;
            for (int j = 0; j < numConigli; j++)
            {
                Celle[Combinazioni[indiciCombinazioni[i], 0], Combinazioni[indiciCombinazioni[i], 1]].Elemento = new CConiglio(Combinazioni[indiciCombinazioni[i], 0], Combinazioni[indiciCombinazioni[i], 1], this);
                i++;
            }
            for (int j = 0; j < numLupi; j++)
            {
                Celle[Combinazioni[indiciCombinazioni[i], 0], Combinazioni[indiciCombinazioni[i], 1]].Elemento = new CLupo(Combinazioni[indiciCombinazioni[i], 0], Combinazioni[indiciCombinazioni[i], 1], this);
                i++;
            }
            for (int j = 0; j < numCarote; j++)
            {
                Celle[Combinazioni[indiciCombinazioni[i], 0], Combinazioni[indiciCombinazioni[i], 1]].Elemento = new CCarota(Combinazioni[indiciCombinazioni[i], 0], Combinazioni[indiciCombinazioni[i], 1], this);
                i++;
            }

            foreach(CCella c in Celle)
            {
               (c.Elemento as CAnimale)?.Muovi();
            }

            InserisciCarote();
            VerificaFineSimulazione();
        }

        
        public async void InserisciCarote()
        {
            while (true)
            {
                await simulazioneFinitaMutex.WaitAsync();
                if (SimulazioneFinita)
                {
                    simulazioneFinitaMutex.Release();
                    return;
                }
                simulazioneFinitaMutex.Release();

                int x, y;

                bool cellaVuotaTrovata = false;
                while(!cellaVuotaTrovata)
                {
                    x = rnd.Next(0, Celle.GetLength(0));
                    y = rnd.Next(0, Celle.GetLength(1));

                    await Celle[x, y].OttieniAccessoAsync();
                    if(Celle[x,y].Elemento == null)
                    {
                        cellaVuotaTrovata = true;
                        Celle[x, y].Elemento = new CCarota(x, y, this);
                    }
                    Celle[x, y].RilasciaAccesso();
                }


                await Task.Delay(intervalloCarote);
            }
        }


        public async void VerificaFineSimulazione()
        {
            while (true)
            {
                int ConigliTrovati = 0;
                int LupiTrovati = 0;
                for (int x = 0; x < Celle.GetLength(0); x++)
                {
                    for (int y = 0; y < Celle.GetLength(1); y++)
                    {
                        await Celle[x, y].OttieniAccessoAsync();
                        if (!(Celle[x, y].Elemento is null))
                        {
                            //Verifica se sono rimasti ancora lupi o conigli
                            if (Celle[x, y].Elemento is CLupo)
                                LupiTrovati++;
                            else if (Celle[x, y].Elemento is CConiglio)
                                ConigliTrovati++;
                        }
                        Celle[x, y].RilasciaAccesso();
                    }
                }

                if (ConigliTrovati == 0 || LupiTrovati == 0)
                {
                    await simulazioneFinitaMutex.WaitAsync();
                    SimulazioneFinita = true;
                    simulazioneFinitaMutex.Release();
                    OnFineSimulazione(new FineSimulazioneEventArgs(ConigliTrovati, LupiTrovati));

                    return;
                }
                await Task.Delay(1000);
            }
        }

        private void OnFineSimulazione(FineSimulazioneEventArgs e)
        {
            FineSimulazione?.Invoke(this, e);
        }

        public event EventHandler<FineSimulazioneEventArgs> FineSimulazione;

    }

    public class CCella
    { 
        private SemaphoreSlim semaforo = new SemaphoreSlim(1, 1);
        private PictureBox _PB;
        public CElemento Elemento { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        private Control.ControlCollection PBCollection;
        public PictureBox PB
        {
            get => _PB;
        }

        public CCella(int X, int Y, Control.ControlCollection PBCollection)
        {
            this.X = X; this.Y = Y;
            this.PBCollection = PBCollection;
            _PB = new PictureBox();
            _PB.Size = new Size(FSimulazione.DimCasella, FSimulazione.DimCasella);
            _PB.Location = new Point(X * FSimulazione.DimCasella, 60 + Y * FSimulazione.DimCasella);
            _PB.SizeMode = PictureBoxSizeMode.StretchImage;


            PBCollection.Add(PB);

        }

        public void AggiornaCella()
        {
            PBCollection.Owner.BeginInvoke((Action)(() =>
            {
                if (Elemento is CCarota)
                    _PB.Image = GameOfLife.Properties.Resources.coniglio;
                else if (Elemento is CLupo)
                    _PB.Image = GameOfLife.Properties.Resources.lupo;
                else if (Elemento is CConiglio)
                    _PB.Image = GameOfLife.Properties.Resources.carota;
                else
                    _PB.Image = null;
            }));
            
        }

        public async Task OttieniAccessoAsync()
        {
            await semaforo.WaitAsync();
        }

        public void RilasciaAccesso()
        {
            semaforo.Release();
        }


        public void Elimina()
        {
            Elemento = null;
            AggiornaCella();
        }
    }

    public abstract class CElemento
    {
        protected Griglia griglia;
        public CElemento(int X, int Y, Griglia griglia)
        {
            this.X = X;
            this.Y = Y;
            this.griglia = griglia;
        }
        public int X { get; set; }
        public int Y { get; set; }
    }
    public abstract class CAnimale : CElemento
    {
        public int Vita { get; set; }
        public CAnimale(int X, int Y, Griglia griglia) : base(X, Y, griglia) { Vita = 10; }
        
        public abstract void Muovi();

        public async void DiminuisciVita()
        {
            while (true)
            {
                await griglia.Celle[X, Y].OttieniAccessoAsync();
                Vita--;
                if(Vita == 0)
                {
                    griglia.Celle[X, Y].Elimina();
                    griglia.Celle[X, Y].RilasciaAccesso();
                    return;
                }
                griglia.Celle[X, Y].RilasciaAccesso();
                await Task.Delay(2000);
            }
        }
    }
    public class CConiglio : CAnimale
    {
        public CConiglio(int X, int Y, Griglia griglia) : base(X, Y, griglia) { }

        public override async void Muovi()
        {
            while (true)
            {
                bool NuovaPosizioneTrovata = false;
                bool CarotaTrovata = false;
                int newX = -1, newY = -1;

                if (X > 0)
                {
                    if (X % 2 == 0)
                    {
                        await griglia.Celle[X, Y].OttieniAccessoAsync();
                        if (griglia.Celle[X, Y].Elemento != this)
                        {
                            griglia.Celle[X, Y].RilasciaAccesso();
                            return;
                        }

                        await griglia.Celle[X - 1, Y].OttieniAccessoAsync();
                    }
                    else
                    {
                        await griglia.Celle[X - 1, Y].OttieniAccessoAsync();
                        await griglia.Celle[X, Y].OttieniAccessoAsync();
                        if (griglia.Celle[X, Y].Elemento != this)
                        {
                            griglia.Celle[X, Y].RilasciaAccesso();
                            return;
                        }
                    }

                    if (griglia.Celle[X - 1, Y].Elemento == null)
                    {
                        newX = X - 1;
                        newY = Y;
                        NuovaPosizioneTrovata = true;
                    }
                    else if (griglia.Celle[X - 1, Y].Elemento is CCarota){
                        newX = X - 1;
                        newY = Y;
                        NuovaPosizioneTrovata = true;
                        if (griglia.Celle[X - 1, Y].Elemento is CCarota)
                        {
                            CarotaTrovata = true;
                        }
                    }

                    if (!NuovaPosizioneTrovata)
                        griglia.Celle[X - 1, Y].RilasciaAccesso();
                }

                griglia.Celle[X, Y].RilasciaAccesso();

                if (X < griglia.Celle.GetLength(0) - 1)
                {
                    if (X % 2 == 0)
                    {
                        await griglia.Celle[X, Y].OttieniAccessoAsync();
                        if (griglia.Celle[X, Y].Elemento != this)
                        {
                            griglia.Celle[X, Y].RilasciaAccesso();
                            return;
                        }
                        await griglia.Celle[X + 1, Y].OttieniAccessoAsync();
                    }
                    else
                    {
                        await griglia.Celle[X + 1, Y].OttieniAccessoAsync();
                        await griglia.Celle[X, Y].OttieniAccessoAsync();
                        if (griglia.Celle[X, Y].Elemento != this)
                        {
                            griglia.Celle[X, Y].RilasciaAccesso();
                            return;
                        }
                    }

                    if (CarotaTrovata && (griglia.Celle[X + 1, Y].Elemento is CCarota) && griglia.rnd.Next(0, 100) < 50)
                    {
                        griglia.Celle[newX, newY].RilasciaAccesso();
                        newX = X + 1;
                        newY = Y;
                    }
                    else if (!CarotaTrovata && (griglia.Celle[X + 1, Y].Elemento is CCarota))
                    {
                        griglia.Celle[newX, newY].RilasciaAccesso();
                        newX = X + 1;
                        newY = Y;
                        CarotaTrovata = true;
                        NuovaPosizioneTrovata = true;
                    }
                    else if (!CarotaTrovata && (!NuovaPosizioneTrovata || griglia.rnd.Next(0, 100) < 50) && griglia.Celle[X + 1, Y].Elemento == null)
                    {
                        griglia.Celle[newX, newY].RilasciaAccesso();
                        newX = X + 1;
                        newY = Y;
                        NuovaPosizioneTrovata = true;
                    }

                    if(newX != X + 1 || newY != Y)
                        griglia.Celle[X + 1, Y].RilasciaAccesso();

                }

                griglia.Celle[X, Y].RilasciaAccesso();

                if (Y > 0)
                {
                    if(Y % 2 == 0)
                    {
                        await griglia.Celle[X, Y].OttieniAccessoAsync();
                        if (griglia.Celle[X, Y].Elemento != this)
                        {
                            griglia.Celle[X, Y].RilasciaAccesso();
                            return;
                        }
                        await griglia.Celle[X, Y - 1].OttieniAccessoAsync();
                    }
                    else
                    {
                        await griglia.Celle[X, Y - 1].OttieniAccessoAsync();
                        await griglia.Celle[X, Y].OttieniAccessoAsync();
                        if (griglia.Celle[X, Y].Elemento != this)
                        {
                            griglia.Celle[X, Y].RilasciaAccesso();
                            return;
                        }
                    }


                    if (CarotaTrovata && (griglia.Celle[X, Y - 1].Elemento is CCarota) && griglia.rnd.Next(0, 100) < 50)
                    {
                        griglia.Celle[newX, newY].RilasciaAccesso();
                        newX = X;
                        newY = Y - 1;
                    }
                    else if (!CarotaTrovata && (griglia.Celle[X, Y - 1].Elemento is CCarota))
                    {
                        griglia.Celle[newX, newY].RilasciaAccesso();
                        newX = X;
                        newY = Y - 1;
                        CarotaTrovata = true;
                        NuovaPosizioneTrovata = true;
                    }
                    else if (!CarotaTrovata && (!NuovaPosizioneTrovata || griglia.rnd.Next(0, 100) < 50) && griglia.Celle[X, Y - 1].Elemento == null)
                    {
                        griglia.Celle[newX, newY].RilasciaAccesso();
                        newX = X;
                        newY = Y - 1;
                        NuovaPosizioneTrovata = true;
                    }

                    if (newX != X || newY != Y - 1)
                        griglia.Celle[X, Y - 1].RilasciaAccesso();
                }

                griglia.Celle[X, Y].RilasciaAccesso();

                if (Y < griglia.Celle.GetLength(1) - 1)
                {
                    if (Y % 2 == 0)
                    {
                        await griglia.Celle[X, Y].OttieniAccessoAsync();
                        if (griglia.Celle[X, Y].Elemento != this)
                        {
                            griglia.Celle[X, Y].RilasciaAccesso();
                            return;
                        }
                        await griglia.Celle[X, Y + 1].OttieniAccessoAsync();
                    }
                    else
                    {
                        await griglia.Celle[X, Y + 1].OttieniAccessoAsync();
                        await griglia.Celle[X, Y].OttieniAccessoAsync();
                        if (griglia.Celle[X, Y].Elemento != this)
                        {
                            griglia.Celle[X, Y].RilasciaAccesso();
                            return;
                        }
                    }
                    if (CarotaTrovata && (griglia.Celle[X, Y + 1].Elemento is CCarota) && griglia.rnd.Next(0, 100) < 50)
                    {
                        griglia.Celle[newX, newY].RilasciaAccesso();
                        newX = X;
                        newY = Y + 1;
                    }
                    else if (!CarotaTrovata && (griglia.Celle[X, Y + 1].Elemento is CCarota))
                    {
                        griglia.Celle[newX, newY].RilasciaAccesso();
                        newX = X;
                        newY = Y + 1;
                        CarotaTrovata = true;
                        NuovaPosizioneTrovata = true;
                    }
                    else if (!CarotaTrovata && (!NuovaPosizioneTrovata || griglia.rnd.Next(0, 100) < 50) && griglia.Celle[X, Y - 1].Elemento == null)
                    {
                        griglia.Celle[newX, newY].RilasciaAccesso();
                        newX = X;
                        newY = Y + 1;
                        NuovaPosizioneTrovata = true;
                    }

                    if (newX != X || newY != Y + 1)
                        griglia.Celle[X, Y + 1].RilasciaAccesso();
                }

                if (CarotaTrovata)
                {
                    Vita = 11;
                    griglia.Celle[newX, newY].Elimina();
                }

                if (newX != -1)
                {
                    int oldX, oldY;
                    griglia.Celle[newX, newY].Elemento = this;
                    griglia.Celle[newX, newY].AggiornaCella();
                    griglia.Celle[X, Y].Elimina();
                    oldX = X;
                    oldY = Y;
                    newX = X;
                    newY = Y;
                    griglia.Celle[oldX, oldY].RilasciaAccesso();
                    griglia.Celle[X, Y].RilasciaAccesso();
                }

                await Task.Delay(griglia.rnd.Next(500, 3000));
            }            
        }
    }

    public class CLupo : CAnimale
    {
        public CLupo(int X, int Y, Griglia griglia) : base(X, Y, griglia) { }
        public override async void Muovi()
        {
            while (true)
            {
                bool NuovaPosizioneTrovata = false;
                bool ConiglioTrovato = false;
                int newX = -1, newY = -1;

                if (X > 0)
                {
                    if (X % 2 == 0)
                    {
                        await griglia.Celle[X, Y].OttieniAccessoAsync();
                        if (griglia.Celle[X, Y].Elemento != this)
                        {
                            griglia.Celle[X, Y].RilasciaAccesso();
                            return;
                        }

                        await griglia.Celle[X - 1, Y].OttieniAccessoAsync();
                    }
                    else
                    {
                        await griglia.Celle[X - 1, Y].OttieniAccessoAsync();
                        await griglia.Celle[X, Y].OttieniAccessoAsync();
                        if (griglia.Celle[X, Y].Elemento != this)
                        {
                            griglia.Celle[X, Y].RilasciaAccesso();
                            return;
                        }
                    }

                    if (griglia.Celle[X - 1, Y].Elemento == null)
                    {
                        newX = X - 1;
                        newY = Y;
                        NuovaPosizioneTrovata = true;
                    }
                    else if (griglia.Celle[X - 1, Y].Elemento is CConiglio)
                    {
                        newX = X - 1;
                        newY = Y;
                        NuovaPosizioneTrovata = true;
                        if (griglia.Celle[X - 1, Y].Elemento is CConiglio)
                        {
                            ConiglioTrovato = true;
                        }
                    }

                    if (!NuovaPosizioneTrovata)
                        griglia.Celle[X - 1, Y].RilasciaAccesso();
                }

                griglia.Celle[X, Y].RilasciaAccesso();

                if (X < griglia.Celle.GetLength(0) - 1)
                {
                    if (X % 2 == 0)
                    {
                        await griglia.Celle[X, Y].OttieniAccessoAsync();
                        if (griglia.Celle[X, Y].Elemento != this)
                        {
                            griglia.Celle[X, Y].RilasciaAccesso();
                            return;
                        }
                        await griglia.Celle[X + 1, Y].OttieniAccessoAsync();
                    }
                    else
                    {
                        await griglia.Celle[X + 1, Y].OttieniAccessoAsync();
                        await griglia.Celle[X, Y].OttieniAccessoAsync();
                        if (griglia.Celle[X, Y].Elemento != this)
                        {
                            griglia.Celle[X, Y].RilasciaAccesso();
                            return;
                        }
                    }

                    if (ConiglioTrovato && (griglia.Celle[X + 1, Y].Elemento is CConiglio) && griglia.rnd.Next(0, 100) < 50)
                    {
                        griglia.Celle[newX, newY].RilasciaAccesso();
                        newX = X + 1;
                        newY = Y;
                    }
                    else if (!ConiglioTrovato && (griglia.Celle[X + 1, Y].Elemento is CConiglio))
                    {
                        griglia.Celle[newX, newY].RilasciaAccesso();
                        newX = X + 1;
                        newY = Y;
                        ConiglioTrovato = true;
                        NuovaPosizioneTrovata = true;
                    }
                    else if (!ConiglioTrovato && (!NuovaPosizioneTrovata || griglia.rnd.Next(0, 100) < 50) && griglia.Celle[X + 1, Y].Elemento == null)
                    {
                        griglia.Celle[newX, newY].RilasciaAccesso();
                        newX = X + 1;
                        newY = Y;
                        NuovaPosizioneTrovata = true;
                    }

                    if (newX != X + 1 || newY != Y)
                        griglia.Celle[X + 1, Y].RilasciaAccesso();

                }

                griglia.Celle[X, Y].RilasciaAccesso();

                if (Y > 0)
                {
                    if (Y % 2 == 0)
                    {
                        await griglia.Celle[X, Y].OttieniAccessoAsync();
                        if (griglia.Celle[X, Y].Elemento != this)
                        {
                            griglia.Celle[X, Y].RilasciaAccesso();
                            return;
                        }
                        await griglia.Celle[X, Y - 1].OttieniAccessoAsync();
                    }
                    else
                    {
                        await griglia.Celle[X, Y - 1].OttieniAccessoAsync();
                        await griglia.Celle[X, Y].OttieniAccessoAsync();
                        if (griglia.Celle[X, Y].Elemento != this)
                        {
                            griglia.Celle[X, Y].RilasciaAccesso();
                            return;
                        }
                    }


                    if (ConiglioTrovato && (griglia.Celle[X, Y - 1].Elemento is CConiglio) && griglia.rnd.Next(0, 100) < 50)
                    {
                        griglia.Celle[newX, newY].RilasciaAccesso();
                        newX = X;
                        newY = Y - 1;
                    }
                    else if (!ConiglioTrovato && (griglia.Celle[X, Y - 1].Elemento is CConiglio))
                    {
                        griglia.Celle[newX, newY].RilasciaAccesso();
                        newX = X;
                        newY = Y - 1;
                        ConiglioTrovato = true;
                        NuovaPosizioneTrovata = true;
                    }
                    else if (!ConiglioTrovato && (!NuovaPosizioneTrovata || griglia.rnd.Next(0, 100) < 50) && griglia.Celle[X, Y - 1].Elemento == null)
                    {
                        griglia.Celle[newX, newY].RilasciaAccesso();
                        newX = X;
                        newY = Y - 1;
                        NuovaPosizioneTrovata = true;
                    }

                    if (newX != X || newY != Y - 1)
                        griglia.Celle[X, Y - 1].RilasciaAccesso();
                }

                griglia.Celle[X, Y].RilasciaAccesso();

                if (Y < griglia.Celle.GetLength(1) - 1)
                {
                    if (Y % 2 == 0)
                    {
                        await griglia.Celle[X, Y].OttieniAccessoAsync();
                        if (griglia.Celle[X, Y].Elemento != this)
                        {
                            griglia.Celle[X, Y].RilasciaAccesso();
                            return;
                        }
                        await griglia.Celle[X, Y + 1].OttieniAccessoAsync();
                    }
                    else
                    {
                        await griglia.Celle[X, Y + 1].OttieniAccessoAsync();
                        await griglia.Celle[X, Y].OttieniAccessoAsync();
                        if (griglia.Celle[X, Y].Elemento != this)
                        {
                            griglia.Celle[X, Y].RilasciaAccesso();
                            return;
                        }
                    }
                    if (ConiglioTrovato && (griglia.Celle[X, Y + 1].Elemento is CConiglio) && griglia.rnd.Next(0, 100) < 50)
                    {
                        griglia.Celle[newX, newY].RilasciaAccesso();
                        newX = X;
                        newY = Y + 1;
                    }
                    else if (!ConiglioTrovato && (griglia.Celle[X, Y + 1].Elemento is CConiglio))
                    {
                        griglia.Celle[newX, newY].RilasciaAccesso();
                        newX = X;
                        newY = Y + 1;
                        ConiglioTrovato = true;
                        NuovaPosizioneTrovata = true;
                    }
                    else if (!ConiglioTrovato && (!NuovaPosizioneTrovata || griglia.rnd.Next(0, 100) < 50) && griglia.Celle[X, Y - 1].Elemento == null)
                    {
                        griglia.Celle[newX, newY].RilasciaAccesso();
                        newX = X;
                        newY = Y + 1;
                        NuovaPosizioneTrovata = true;
                    }

                    if (newX != X || newY != Y + 1)
                        griglia.Celle[X, Y + 1].RilasciaAccesso();
                }

                if (ConiglioTrovato)
                {
                    Vita = 11;
                    griglia.Celle[newX, newY].Elimina();
                }

                Vita--;

                if (newX != -1)
                {
                    int oldX, oldY;
                    griglia.Celle[newX, newY].Elemento = this;
                    griglia.Celle[newX, newY].AggiornaCella();
                    griglia.Celle[X, Y].Elimina();
                    oldX = X;
                    oldY = Y;
                    newX = X;
                    newY = Y;
                    griglia.Celle[oldX, oldY].RilasciaAccesso();
                    griglia.Celle[X, Y].RilasciaAccesso();
                }

                await Task.Delay(griglia.rnd.Next(500, 3000));
            }     
        }

    }

    public class CCarota : CElemento
    {
        public CCarota(int X, int Y, Griglia griglia) : base(X, Y, griglia) { }
    }
}
