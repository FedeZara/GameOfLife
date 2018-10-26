using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameOfLife
{
    public partial class FSimulazione : Form
    {
        public CCarota[] Carote { get; set; }

        public CLupo[] Lupi { get; set; }
        public CConiglio[] Conigli { get; set; }

        public CElemento[] Elementi { get; set; }

        public PictureBox[,] pbGriglia { get; set; }
        public int[,] Griglia { get; set; }
   

        public FSimulazione(int wGriglia, int hGriglia, int numConigli, int numLupi, int numCarote, int intervalloCarote)
        {
            InitializeComponent();

            pbGriglia = new PictureBox[wGriglia, hGriglia];
            Elementi = new CElemento[numConigli + numLupi + numCarote];
            Carote = new CCarota[numCarote];
            Lupi = new CLupo[numLupi];
            Conigli = new CConiglio[numConigli];
            Griglia = new int[wGriglia, hGriglia];

            Inizializza(wGriglia, hGriglia, numConigli, numLupi, numCarote, intervalloCarote);
            Avvia();
            Risultati();
        }
        public void Inizializza(int wGriglia, int hGriglia, int numConigli, int numLupi, int numCarote, int intervalloCarote)
        {
            this.Size = new Size(wGriglia*20, hGriglia*20);
            int[,] Combinazioni = new int[wGriglia * hGriglia, 2];
            int[] indiciCombinazioni = new int[wGriglia * hGriglia];
            for(int x=0; x<wGriglia; x++)
            {
                for(int y=0; y<hGriglia; y++)
                {
                    Combinazioni[y + x * hGriglia, 0] = x;
                    Combinazioni[y + x * hGriglia, 1] = y;
                    indiciCombinazioni[y + x * hGriglia] = y + x * hGriglia;
                    pbGriglia[x, y] = new PictureBox();
                    pbGriglia[x, y].Size = new Size(20, 20);
                    pbGriglia[x, y].Location = new Point(x * 20, y * 20);
                    pbGriglia[x, y].SizeMode = PictureBoxSizeMode.StretchImage;
                    this.Controls.Add(pbGriglia[x, y]);
                }
            }
            Random rnd = new Random();
            indiciCombinazioni = indiciCombinazioni.OrderBy(x => rnd.Next()).ToArray();
            int i = 0;
            for(int j=0; j<numConigli; j++)
            {
                Elementi[i] = Conigli[j] = new CConiglio(Combinazioni[indiciCombinazioni[i], 0], Combinazioni[indiciCombinazioni[i], 1]);
                i++;
            }
            for (int j = 0; j < numLupi; j++)
            {
                Elementi[i] = Lupi[j] = new CLupo(Combinazioni[indiciCombinazioni[i], 0], Combinazioni[indiciCombinazioni[i], 1]);
                i++;
            }
            for (int j = 0; j < numCarote; j++)
            {
                Elementi[i] = Carote[j] = new CCarota(Combinazioni[indiciCombinazioni[i], 0], Combinazioni[indiciCombinazioni[i], 1]);
                i++;
            }
           
        }
        public void MostraGriglia()
        {
            foreach(CElemento e in Elementi)
            {
                e.Mostra();
            }
        }
        public void Avvia()
        {
            MostraGriglia();
        }
        public void Risultati()
        {

        }
    }
}
