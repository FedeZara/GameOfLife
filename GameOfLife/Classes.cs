using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace GameOfLife
{
    public abstract class CElemento
    {
        protected PictureBox _PB;
        public bool DaEliminare { set; get; }
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
        public abstract void Muovi(CElemento[,] Elementi, bool[,] Occupato);
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

        public override void Muovi(CElemento[,] Elementi, bool[,] Occupato)
        {
            if(Vita == 0 || DaEliminare)
            {
                DaEliminare = true;
                return;
            }
            bool NuovaPosizioneTrovata = false;
            bool CarotaTrovata = false;
            int newX = -1, newY = -1;
            if (X > 0)
            {
                if (!Occupato[X - 1, Y])
                {
                    newX = X - 1;
                    newY = Y;
                    NuovaPosizioneTrovata = true;
                    if (Elementi[X - 1, Y] is CCarota)
                    {
                        CarotaTrovata = true;
                    }
                }
            }
            if (X < Elementi.GetLength(0) - 1)
            {
                if (!Occupato[X + 1, Y])
                {
                    if (CarotaTrovata && (Elementi[X + 1, Y] is CCarota) && FSimulazione.rnd.Next(0, 100) < 50)
                    {
                        newX = X + 1;
                        newY = Y;
                    }
                    else if (!CarotaTrovata && (Elementi[X + 1, Y] is CCarota))
                    {
                        newX = X + 1;
                        newY = Y;
                        CarotaTrovata = true;
                        NuovaPosizioneTrovata = true;
                    }
                    else if (!CarotaTrovata && (!NuovaPosizioneTrovata || FSimulazione.rnd.Next(0, 100) < 50))
                    {
                        newX = X + 1;
                        newY = Y;
                        NuovaPosizioneTrovata = true;
                    }
                }
            }
            if (Y > 0)
            {
                if (!Occupato[X, Y - 1])
                {
                    if (CarotaTrovata && (Elementi[X, Y - 1] is CCarota) && FSimulazione.rnd.Next(0, 100) < 50)
                    {
                        newX = X;
                        newY = Y - 1;
                    }
                    else if (!CarotaTrovata && (Elementi[X, Y - 1] is CCarota))
                    {
                        newX = X;
                        newY = Y - 1;
                        CarotaTrovata = true;
                        NuovaPosizioneTrovata = true;
                    }
                    else if (!CarotaTrovata && (!NuovaPosizioneTrovata || FSimulazione.rnd.Next(0, 100) < 50))
                    {
                        newX = X;
                        newY = Y - 1;
                        NuovaPosizioneTrovata = true;
                    }
                }
            }
            if (Y < Elementi.GetLength(1) - 1)
            {
                if (!Occupato[X, Y + 1])
                {
                    if (CarotaTrovata && (Elementi[X, Y + 1] is CCarota) && FSimulazione.rnd.Next(0, 100) < 50)
                    {
                        newX = X;
                        newY = Y + 1;
                    }
                    else if (!CarotaTrovata && (Elementi[X, Y + 1] is CCarota))
                    {
                        newX = X;
                        newY = Y + 1;
                        CarotaTrovata = true;
                        NuovaPosizioneTrovata = true;
                    }
                    else if (!CarotaTrovata && (!NuovaPosizioneTrovata || FSimulazione.rnd.Next(0, 100) < 50))
                    {
                        newX = X;
                        newY = Y + 1;
                        NuovaPosizioneTrovata = true;
                    }
                }
            }
            if (CarotaTrovata)
            {
                Vita = 11;
                Elementi[newX, newY].DaEliminare = true;
            }
            Vita--;
            if(newX == -1) {
                newX = X;
                newY = Y;
            }
            Occupato[newX, newY] = true;
            X = newX;
            Y = newY;
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
        public override void Muovi(CElemento[,] Elementi, bool[,] Occupato)
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
                    if (!(Elementi[X - 1, Y] is CCarota))
                    {
                        NuovaPosizioneTrovata = true;
                        newX = X - 1;
                        newY = Y;
                        if (Elementi[X - 1, Y] is CConiglio)
                        {
                            ConiglioTrovato = true;
                        }
                    }
                }
            }
            if(X < Elementi.GetLength(0) - 1)
            {
                if (!Occupato[X + 1, Y])
                {
                    if(ConiglioTrovato && (Elementi[X + 1, Y] is CConiglio) && FSimulazione.rnd.Next(0, 100) < 50)
                    {
                        newX = X + 1;
                        newY = Y;
                    }
                    else if(!ConiglioTrovato && (Elementi[X + 1, Y] is CConiglio))
                    {
                        newX = X + 1;
                        newY = Y;
                        ConiglioTrovato = true;
                        NuovaPosizioneTrovata = true;
                    }
                    else if (!ConiglioTrovato && !(Elementi[X + 1, Y] is CCarota) && (!NuovaPosizioneTrovata || FSimulazione.rnd.Next(0, 100) < 50))
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
                    if (ConiglioTrovato && (Elementi[X, Y - 1] is CConiglio) && FSimulazione.rnd.Next(0, 100) < 50)
                    {
                        newX = X;
                        newY = Y-1;
                    }
                    else if (!ConiglioTrovato && (Elementi[X, Y - 1] is CConiglio))
                    {
                        newX = X;
                        newY = Y - 1;
                        ConiglioTrovato = true;
                        NuovaPosizioneTrovata = true;
                    }
                    else if (!ConiglioTrovato && !(Elementi[X, Y-1] is CCarota) && (!NuovaPosizioneTrovata || FSimulazione.rnd.Next(0, 100) < 50))
                    {
                        newX = X;
                        newY = Y - 1;
                        NuovaPosizioneTrovata = true;
                    }
                }
            }
            if (Y < Elementi.GetLength(1) - 1)
            {
                if (!Occupato[X, Y + 1])
                {
                    if (ConiglioTrovato && (Elementi[X, Y + 1] is CConiglio) && FSimulazione.rnd.Next(0, 100) < 50)
                    {
                        newX = X;
                        newY = Y + 1;
                    }
                    else if (!ConiglioTrovato && (Elementi[X, Y + 1] is CConiglio))
                    {
                        newX = X;
                        newY = Y + 1;
                        ConiglioTrovato = true;
                        NuovaPosizioneTrovata = true;
                    }
                    else if (!ConiglioTrovato && !(Elementi[X, Y+1] is CCarota) && (!NuovaPosizioneTrovata || FSimulazione.rnd.Next(0, 100) < 50))
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

                Elementi[newX, newY].DaEliminare = true;
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
