using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace GameOfLife
{
    public abstract class CElemento
    {
        abstract public Image Immagine { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public CElemento(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }
        public void Mostra()
        {
            
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
        public CCarota(int X, int Y) : base(X, Y)
        {

        }
    }
}
