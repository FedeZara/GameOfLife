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
        abstract public Image Immagine { get; }
        public int X { get; set; }
        public int Y { get; set; }
        public CElemento(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }
        public void Mostra(PictureBox[,] Griglia)
        {
            Griglia[X, Y].Image = Immagine;
        }
    }
    public abstract class CAnimale : CElemento
    {
        public abstract void Muovi();
        public CAnimale(int X, int Y) : base(X, Y)
        {

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
        public override void Muovi()
        {
            throw new NotImplementedException();
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
        public override void Muovi()
        {
            throw new NotImplementedException();
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
        public CCarota(int X, int Y) : base(X, Y)
        {

        }
    }
}
