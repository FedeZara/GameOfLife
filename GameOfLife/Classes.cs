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
    public class Griglia
    {
        public Random rnd { get; set; }
        public CElemento[,] Elementi { get; set; }
        public int Larghezza { get; set; }
        public int Altezza { get; set; }
        public Griglia(int larghezza, int altezza)
        {
            Elementi = new CElemento[larghezza, altezza];
            Larghezza = larghezza;
            Altezza = altezza;
        }

        public void Inizializza(int numConigli, int numLupi, int numCarote, int intervalloCarote)
        {
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
        }
    }

    public abstract class CElemento
    {
        protected Griglia griglia;
        private SemaphoreSlim mutex = new SemaphoreSlim(1, 1);
        protected PictureBox _PB;
        public bool DaEliminare { set; get; }
        public CElemento(Griglia griglia) { this.griglia = griglia; }
        virtual public PictureBox PB
        {
            get
            {
                if(_PB == null)
                {
                    _PB = new PictureBox();
                    _PB.Size = new Size(FSimulazione.DimCasella, FSimulazione.DimCasella);
                    _PB.Location = new Point(X * FSimulazione.DimCasella, 60 + Y * FSimulazione.DimCasella);
                    _PB.SizeMode = PictureBoxSizeMode.StretchImage;
                    _PB.Image = Immagine;
                }
                return _PB;
            }
        }
        abstract public Image Immagine { get; }
        public int X { get; set; }
        public int Y { get; set; }
        public CElemento(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }
        public async Task OttieniAccesso()
        {
            await mutex.WaitAsync();
        }
        public void RilasciaAccesso()
        {
            mutex.Release();
        }
    }
    public abstract class CAnimale : CElemento
    {
        public override PictureBox PB
        {
            get
            {
                base.PB.BackColor = Color.FromArgb(100, 0, 255, 0);
                return _PB;
            }
        }
        public int Vita { get; set; }
        public CAnimale(Griglia griglia) : base(griglia) { }
        
        public abstract void Muovi();
        public void MostraPosizioneCambiata()
        {           
            PB.Location = new Point(X * FSimulazione.DimCasella, 60 + Y * FSimulazione.DimCasella);
            PB.BackColor = Color.FromArgb(100, (10 - Vita) * 25, Vita * 25, 0);          
        }
        public CAnimale(int X, int Y) : base(X, Y)
        {
            Vita = 10;
        }
    }
    public class CConiglio : CAnimale
    {
        public override Image Immagine
        {
            get
            {
                return GameOfLife.Properties.Resources.coniglio;
            }
        }

        public CConiglio(Griglia griglia) : base(griglia) { }

        public override async void Muovi()
        {
            while (true)
            {
                if (Vita == 0 || DaEliminare)
                {
                    DaEliminare = true;
                    return;
                }
                bool NuovaPosizioneTrovata = false;
                bool CarotaTrovata = false;
                int newX = -1, newY = -1;

                await griglia.Elementi[X, Y].OttieniAccesso();

                if (X > 0)
                {
                    await griglia.Elementi[X - 1, Y].OttieniAccesso();

                    if (griglia.Elementi[X - 1, Y] == null)
                    {
                        newX = X - 1;
                        newY = Y;
                        NuovaPosizioneTrovata = true;
                    }
                    else if (griglia.Elementi[X - 1, Y] is CCarota){
                        newX = X - 1;
                        newY = Y;
                        NuovaPosizioneTrovata = true;
                        if (griglia.Elementi[X - 1, Y] is CCarota)
                        {
                            CarotaTrovata = true;
                        }
                    }

                    if (!NuovaPosizioneTrovata)
                        griglia.Elementi[X - 1, Y].RilasciaAccesso();
                }

                if (X < griglia.Elementi.GetLength(0) - 1)
                {
                    await griglia.Elementi[X + 1, Y].OttieniAccesso();

                    if (CarotaTrovata && (griglia.Elementi[X + 1, Y] is CCarota) && griglia.rnd.Next(0, 100) < 50)
                    {
                        griglia.Elementi[newX, newY].RilasciaAccesso();
                        newX = X + 1;
                        newY = Y;
                    }
                    else if (!CarotaTrovata && (griglia.Elementi[X + 1, Y] is CCarota))
                    {
                        griglia.Elementi[newX, newY].RilasciaAccesso();
                        newX = X + 1;
                        newY = Y;
                        CarotaTrovata = true;
                        NuovaPosizioneTrovata = true;
                    }
                    else if (!CarotaTrovata && (!NuovaPosizioneTrovata || griglia.rnd.Next(0, 100) < 50) && griglia.Elementi[X + 1, Y] == null)
                    {
                        griglia.Elementi[newX, newY].RilasciaAccesso();
                        newX = X + 1;
                        newY = Y;
                        NuovaPosizioneTrovata = true;
                    }

                    if(newX != X + 1 || newY != Y)
                        griglia.Elementi[X + 1, Y].RilasciaAccesso();

                }
                if (Y > 0)
                {
                    await griglia.Elementi[X, Y - 1].OttieniAccesso();

                    if (CarotaTrovata && (griglia.Elementi[X, Y - 1] is CCarota) && griglia.rnd.Next(0, 100) < 50)
                    {
                        griglia.Elementi[newX, newY].RilasciaAccesso();
                        newX = X;
                        newY = Y - 1;
                    }
                    else if (!CarotaTrovata && (griglia.Elementi[X, Y - 1] is CCarota))
                    {
                        griglia.Elementi[newX, newY].RilasciaAccesso();
                        newX = X;
                        newY = Y - 1;
                        CarotaTrovata = true;
                        NuovaPosizioneTrovata = true;
                    }
                    else if (!CarotaTrovata && (!NuovaPosizioneTrovata || griglia.rnd.Next(0, 100) < 50) && griglia.Elementi[X, Y - 1] == null)
                    {
                        griglia.Elementi[newX, newY].RilasciaAccesso();
                        newX = X;
                        newY = Y - 1;
                        NuovaPosizioneTrovata = true;
                    }

                    if (newX != X || newY != Y - 1)
                        griglia.Elementi[X, Y - 1].RilasciaAccesso();
                }
                if (Y < griglia.Elementi.GetLength(1) - 1)
                {
                    await griglia.Elementi[X, Y - 1].OttieniAccesso();
                    if (CarotaTrovata && (griglia.Elementi[X, Y + 1] is CCarota) && griglia.rnd.Next(0, 100) < 50)
                    {
                        griglia.Elementi[newX, newY].RilasciaAccesso();
                        newX = X;
                        newY = Y + 1;
                    }
                    else if (!CarotaTrovata && (griglia.Elementi[X, Y + 1] is CCarota))
                    {
                        griglia.Elementi[newX, newY].RilasciaAccesso();
                        newX = X;
                        newY = Y + 1;
                        CarotaTrovata = true;
                        NuovaPosizioneTrovata = true;
                    }
                    else if (!CarotaTrovata && (!NuovaPosizioneTrovata || griglia.rnd.Next(0, 100) < 50) && griglia.Elementi[X, Y - 1] == null)
                    {
                        griglia.Elementi[newX, newY].RilasciaAccesso();
                        newX = X;
                        newY = Y + 1;
                        NuovaPosizioneTrovata = true;
                    }

                    if (newX != X || newY != Y + 1)
                        griglia.Elementi[X, Y + 1].RilasciaAccesso();
                }
                if (CarotaTrovata)
                {
                    Vita = 11;
                    griglia.Elementi[newX, newY].DaEliminare = true;
                }
                Vita--;
                if (newX == -1)
                {
                    newX = X;
                    newY = Y;
                }

                X = newX;
                Y = newY;

                griglia.Elementi[X, Y].RilasciaAccesso();

                await Task.Delay(griglia.rnd.Next(500, 3000));
            }            
        }
        public CConiglio(int X, int Y) : base(X, Y)
        {

        }
    }

    public class CLupo : CAnimale
    {
        public override Image Immagine
        {
            get
            {
                return GameOfLife.Properties.Resources.lupo;
            }
        }
        public CLupo(Griglia griglia) : base(griglia) { }
        public override void Muovi(CElemento[,] griglia.Elementi, bool[,] Occupato)
        {
            if (Vita == 0 || DaEliminare)
            {
                DaEliminare = true;
                return;
            }
            bool NuovaPosizioneTrovata = false;
            bool ConiglioTrovato = false;
            int newX = -1, newY = -1;
            if(X > 0)
            {
                if(!Occupato[X-1, Y])
                {
                    if (!(griglia.Elementi[X - 1, Y] is CCarota))
                    {
                        NuovaPosizioneTrovata = true;
                        newX = X - 1;
                        newY = Y;
                        if (griglia.Elementi[X - 1, Y] is CConiglio)
                        {
                            ConiglioTrovato = true;
                        }
                    }
                }
            }
            if(X < griglia.Elementi.GetLength(0) - 1)
            {
                if (!Occupato[X + 1, Y])
                {
                    if(ConiglioTrovato && (griglia.Elementi[X + 1, Y] is CConiglio) && griglia.rnd.Next(0, 100) < 50)
                    {
                        newX = X + 1;
                        newY = Y;
                    }
                    else if(!ConiglioTrovato && (griglia.Elementi[X + 1, Y] is CConiglio))
                    {
                        newX = X + 1;
                        newY = Y;
                        ConiglioTrovato = true;
                        NuovaPosizioneTrovata = true;
                    }
                    else if (!ConiglioTrovato && !(griglia.Elementi[X + 1, Y] is CCarota) && (!NuovaPosizioneTrovata || griglia.rnd.Next(0, 100) < 50))
                    {
                        newX = X + 1;
                        newY = Y;
                        NuovaPosizioneTrovata = true;
                    }
                }
            }
            if (Y > 0)
            {
                if (!Occupato[X, Y-1])
                {
                    if (ConiglioTrovato && (griglia.Elementi[X, Y - 1] is CConiglio) && griglia.rnd.Next(0, 100) < 50)
                    {
                        newX = X;
                        newY = Y-1;
                    }
                    else if (!ConiglioTrovato && (griglia.Elementi[X, Y - 1] is CConiglio))
                    {
                        newX = X;
                        newY = Y - 1;
                        ConiglioTrovato = true;
                        NuovaPosizioneTrovata = true;
                    }
                    else if (!ConiglioTrovato && !(griglia.Elementi[X, Y-1] is CCarota) && (!NuovaPosizioneTrovata || griglia.rnd.Next(0, 100) < 50))
                    {
                        newX = X;
                        newY = Y - 1;
                        NuovaPosizioneTrovata = true;
                    }
                }
            }
            if (Y < griglia.Elementi.GetLength(1) - 1)
            {
                if (!Occupato[X, Y + 1])
                {
                    if (ConiglioTrovato && (griglia.Elementi[X, Y + 1] is CConiglio) && griglia.rnd.Next(0, 100) < 50)
                    {
                        newX = X;
                        newY = Y + 1;
                    }
                    else if (!ConiglioTrovato && (griglia.Elementi[X, Y + 1] is CConiglio))
                    {
                        newX = X;
                        newY = Y + 1;
                        ConiglioTrovato = true;
                        NuovaPosizioneTrovata = true;
                    }
                    else if (!ConiglioTrovato && !(griglia.Elementi[X, Y+1] is CCarota) && (!NuovaPosizioneTrovata || griglia.rnd.Next(0, 100) < 50))
                    {
                        newX = X;
                        newY = Y + 1;
                        NuovaPosizioneTrovata = true;
                    }
                }
            }
            if (ConiglioTrovato)
            {
                Vita = 11;

                griglia.Elementi[newX, newY].DaEliminare = true;
            }
            Vita--;
            if (newX == -1)
            {
                newX = X;
                newY = Y;
            }
            Occupato[newX, newY] = true;
            X = newX;
            Y = newY;
        }
        public CLupo(int X, int Y) : base(X, Y)
        {

        }
    }

    public class CCarota : CElemento
    {
        public CCarota(Griglia griglia) : base(griglia) { }
        public override Image Immagine
        {
            get
            {
                return GameOfLife.Properties.Resources.carota;
            }
        }
        public override PictureBox PB
        {
            get
            {
                base.PB.BackColor = Color.FromArgb(255, 255, 255);
                return _PB;
            }
        }
        public CCarota(int X, int Y) : base(X, Y)
        {

        }
    }
}
