using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Collections.Concurrent;

namespace GameOfLife
{
    public class SemaphoreQueue
    {
        private SemaphoreSlim semaphore;
        private ConcurrentQueue<TaskCompletionSource<bool>> queue =
            new ConcurrentQueue<TaskCompletionSource<bool>>();
        public SemaphoreQueue(int initialCount)
        {
            semaphore = new SemaphoreSlim(initialCount);
        }
        public SemaphoreQueue(int initialCount, int maxCount)
        {
            semaphore = new SemaphoreSlim(initialCount, maxCount);
        }
        public void Wait()
        {
            WaitAsync().Wait();
        }
        public Task WaitAsync()
        {
            var tcs = new TaskCompletionSource<bool>();
            queue.Enqueue(tcs);
            semaphore.WaitAsync().ContinueWith(t =>
            {
                TaskCompletionSource<bool> popped;
                if (queue.TryDequeue(out popped))
                    popped.SetResult(true);
            });
            return tcs.Task;
        }
        public void Release()
        {
            semaphore.Release();
        }
    }

    public class FineSimulazioneEventArgs : EventArgs
    {
        public int ConigliRimasti { get; set; }
        public int LupiRimasti { get; set; }

        public FineSimulazioneEventArgs(int conigliRimasti, int lupiRimasti)
        {
            ConigliRimasti = conigliRimasti;
            LupiRimasti = lupiRimasti;
        }
    }

    public class CellaAggiornataEventArgs
    {
        public int X { get; set; }
        public int Y { get; set; }
        public CElemento Elemento { get; set; }

        public CellaAggiornataEventArgs(int X, int Y, CElemento Elemento)
        {
            this.X = X; this.Y = Y; this.Elemento = Elemento;
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
        private SemaphoreQueue numConigliMutex = new SemaphoreQueue(1, 1);
        private SemaphoreQueue numLupiMutex = new SemaphoreQueue(1, 1);
        public int NumConigli { get; set; }
        public int NumLupi { get; set; }
        public Griglia(int larghezza, int altezza)
        {
            Celle = new CCella[larghezza, altezza];
            Larghezza = larghezza;
            Altezza = altezza;
            
            for(int i=0; i<larghezza; i++)
            {
                for (int j = 0; j < altezza; j++)
                    Celle[i, j] = new CCella(i, j);
            }
        }

        public void Inizializza(int numConigli, int numLupi, int numCarote, int intervalloCarote)
        {
            NumConigli = numConigli;
            NumLupi = numLupi;
            SimulazioneFinita = false;
            this.intervalloCarote = intervalloCarote*1000;
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
                //(c.Elemento as CAnimale)?.DiminuisciVita();
                (c.Elemento as CAnimale)?.Muovi();
            }

            //InserisciCarote();
        }

        public async Task DiminuisciNumConigli()
        {
            await numConigliMutex.WaitAsync();

            NumConigli--;

            if(NumConigli == 0)
            {
                SimulazioneFinita = true;
                OnFineSimulazione(new FineSimulazioneEventArgs(NumConigli, NumLupi));
            }

            numConigliMutex.Release();
        }

        public async Task DiminuisciNumLupi()
        {
            await numLupiMutex.WaitAsync();

            NumLupi--;

            if (NumLupi == 0)
            {
                SimulazioneFinita = true;
                OnFineSimulazione(new FineSimulazioneEventArgs(NumConigli, NumLupi));
            }

            numLupiMutex.Release();
        }

        public async void InserisciCarote()
        {
            while (true)
            {
                if (SimulazioneFinita)
                {
                    return;
                }

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
                        Celle[x, y].AggiornaCella();
                    }
                    Celle[x, y].RilasciaAccesso();
                }


                await Task.Delay(intervalloCarote);
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
        private SemaphoreQueue semaforo = new SemaphoreQueue(1, 1);
        public CElemento Elemento { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public CCella(int X, int Y)
        {
            this.X = X; this.Y = Y;
        }

        public event EventHandler<CellaAggiornataEventArgs> CellaAggiornata;

        protected void OnCellaAggiornata(CellaAggiornataEventArgs e)
        {
            CellaAggiornata?.Invoke(this, e);
        }
        public void AggiornaCella()
        {
            CellaAggiornataEventArgs e = new CellaAggiornataEventArgs(X, Y, Elemento);
            OnCellaAggiornata(e);
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

    public class CInformazioni
    {
        public bool AccessoOttenuto { get; set; }
        public bool NuovaPosizioneTrovata { get; set; }
        public bool CiboTrovato { get; set; }
        public int newX { get; set; }
        public int newY { get; set; }
        public bool Eliminato { get; set; }

        public CInformazioni(bool accessoOttenuto, bool nuovaPosizioneTrovata, bool ciboTrovato,
            int x, int y, bool eliminato)
        {
            AccessoOttenuto = accessoOttenuto;
            NuovaPosizioneTrovata = nuovaPosizioneTrovata;
            CiboTrovato = ciboTrovato;
            newX = x;
            newY = y;
            Eliminato = eliminato;
        }
    }
    public abstract class CAnimale : CElemento
    {
        public int Vita { get; set; }
        public CAnimale(int X, int Y, Griglia griglia) : base(X, Y, griglia) { Vita = 10; }

        public async Task AnalizzaCella(int xCella, int yCella, CInformazioni informazioni)
        {
            // rilascia l'accesso dell'elemento per permettere ad altri Task di ottenerlo
            if (informazioni.AccessoOttenuto)
            {
                griglia.Celle[X, Y].RilasciaAccesso();
                informazioni.AccessoOttenuto = false;
            }

            bool asseY = (yCella - Y) != 0;

            // se la riga/colonna è pari richiede l'accesso prima di sè stessa e poi della cella che si sta analizzando
            // se la riga/colonna è dispari viceversa
            if ((X % 2 == 0 && !asseY) || (Y % 2 == 1 && asseY))
            {
                await griglia.Celle[X, Y].OttieniAccessoAsync();

                // controlla se l'elemento è stato mangiato o se è morto di fame
                if (griglia.Celle[X, Y].Elemento != this)
                {
                    griglia.Celle[X, Y].RilasciaAccesso();
                    if (informazioni.NuovaPosizioneTrovata)
                        griglia.Celle[informazioni.newX, informazioni.newY].RilasciaAccesso();
                    informazioni.Eliminato = true;
                    return;
                }

                await griglia.Celle[xCella, yCella].OttieniAccessoAsync();
            }
            else
            {
                await griglia.Celle[xCella, yCella].OttieniAccessoAsync();
                await griglia.Celle[X, Y].OttieniAccessoAsync();
                
                // controlla se l'elemento è stato mangiato o se è morto di fame
                if (griglia.Celle[X, Y].Elemento != this)
                {
                    griglia.Celle[xCella, yCella].RilasciaAccesso();
                    griglia.Celle[X, Y].RilasciaAccesso();
                    if (informazioni.NuovaPosizioneTrovata)
                        griglia.Celle[informazioni.newX, informazioni.newY].RilasciaAccesso();
                    informazioni.Eliminato = true;
                    return;
                }
            }

            informazioni.AccessoOttenuto = true;

            // controlla se nella cella che si sta analizzando c'è del cibo
            bool ciboPresenteNellaCella = CiboPresenteNellaCella(griglia.Celle[xCella, yCella].Elemento);

            // se è già stata trovata una cella vicina con del cibo e la cella in analisi contiene del cibo
            // ha una possibilità del 50% di rendere la cella in analisi la nuova cella verso cui spostarsi
            if (informazioni.CiboTrovato && ciboPresenteNellaCella && griglia.rnd.Next(0, 100) < 50)
            {
                // non occorre più avere l'accesso della cella precedente
                griglia.Celle[informazioni.newX, informazioni.newY].RilasciaAccesso();

                informazioni.newX = xCella;
                informazioni.newY = yCella;
            }

            // se non è ancora stata trovata una cella vicina con del cibo e la cella in analisi contiene del cibo
            // rendi la cella in analisi la nuova cella verso cui spostarsi
            else if (!informazioni.CiboTrovato && ciboPresenteNellaCella)
            {
                // se era già stata selezionata un'altra cella come prossima cella, rilascia l'accesso
                if (informazioni.NuovaPosizioneTrovata)
                    griglia.Celle[informazioni.newX, informazioni.newY].RilasciaAccesso();

                informazioni.newX = xCella;
                informazioni.newY = yCella;
                informazioni.CiboTrovato = true;
                informazioni.NuovaPosizioneTrovata = true;
            }

            // se non è stato trovato ancora cibo, la cella in analisi non ne contiene ed è vuota, 
            // segna la cella in analisi come prossima cella se non è stata trovata nessuna nuova posizione
            // oppure segnala con il 50% di probabilità se è già stata trovata un'altra cella verso cui muoversi
            else if (!informazioni.CiboTrovato && (!informazioni.NuovaPosizioneTrovata || griglia.rnd.Next(0, 100) < 50) && griglia.Celle[xCella, yCella].Elemento == null)
            {
                // se era già stata selezionata un'altra cella come prossima cella, rilascia l'accesso
                if (informazioni.NuovaPosizioneTrovata)
                    griglia.Celle[informazioni.newX, informazioni.newY].RilasciaAccesso();

                informazioni.newX = xCella;
                informazioni.newY = yCella;
                informazioni.NuovaPosizioneTrovata = true;
            }

            // nel caso la cella in analisi non sia stata segnata come prossima cella, il suo accesso si può rilasciare
            if (informazioni.newX != xCella || informazioni.newY != yCella)
                griglia.Celle[xCella, yCella].RilasciaAccesso();
        }
        public async void Muovi()
        {
            while (true)
            {
                if (griglia.SimulazioneFinita)
                    return;

                CInformazioni informazioni = new CInformazioni(false, false, false, -1, -1, false);
                if (X > 0)
                {
                    await AnalizzaCella(X - 1, Y, informazioni);
                    if (informazioni.Eliminato || griglia.SimulazioneFinita)
                    {
                        return;
                    }
                }


                if (X < griglia.Celle.GetLength(0) - 1)
                {
                    await AnalizzaCella(X + 1, Y, informazioni);
                    if (informazioni.Eliminato || griglia.SimulazioneFinita)
                        return;
                }

                if (Y > 0)
                {
                    await AnalizzaCella(X, Y - 1, informazioni);
                    if (informazioni.Eliminato || griglia.SimulazioneFinita)
                        return;
                }

                if (Y < griglia.Celle.GetLength(1) - 1)
                {
                    await AnalizzaCella(X, Y + 1, informazioni);
                    if (informazioni.Eliminato || griglia.SimulazioneFinita)
                        return;
                }

                if (informazioni.CiboTrovato)
                {
                    Vita = 10;
                    griglia.Celle[informazioni.newX, informazioni.newY].Elimina();
                    await DiminuisciContatoreCibo();
                }

                if (informazioni.NuovaPosizioneTrovata)
                {
                    int oldX, oldY;
                    griglia.Celle[informazioni.newX, informazioni.newY].Elemento = this;
                    griglia.Celle[informazioni.newX, informazioni.newY].AggiornaCella();
                    griglia.Celle[X, Y].Elimina();
                    oldX = X;
                    oldY = Y;
                    X = informazioni.newX;
                    Y = informazioni.newY;
                    griglia.Celle[oldX, oldY].RilasciaAccesso();
                    griglia.Celle[X, Y].RilasciaAccesso();
                }
                else
                {
                    griglia.Celle[X, Y].RilasciaAccesso();
                }

                await Task.Delay(griglia.rnd.Next(500, 1000));
            }
        }

        public abstract Task DiminuisciContatoreCibo();

        public async void DiminuisciVita()
        {
            while (true)
            {
                if (griglia.SimulazioneFinita)
                    return;
                await griglia.Celle[X, Y].OttieniAccessoAsync();
                Vita--;
                griglia.Celle[X, Y].AggiornaCella();
                if (Vita == 0)
                {
                    griglia.Celle[X, Y].Elimina();
                    griglia.Celle[X, Y].RilasciaAccesso();
                    await DiminuisciContatore();
                    return;
                }
                griglia.Celle[X, Y].RilasciaAccesso();
                await Task.Delay(2000);
            }
        }

        public abstract Task DiminuisciContatore();

        public abstract bool CiboPresenteNellaCella(CElemento elemento);
    }
    public class CConiglio : CAnimale
    {
        public CConiglio(int X, int Y, Griglia griglia) : base(X, Y, griglia) { }

        public override bool CiboPresenteNellaCella(CElemento elemento)
        {
            return elemento is CCarota;
        }

        public override async Task DiminuisciContatore()
        {
            await griglia.DiminuisciNumConigli();
        }

        public override async Task DiminuisciContatoreCibo()
        {
            await Task.Delay(0);
        }

    }

    public class CLupo : CAnimale
    {
        public CLupo(int X, int Y, Griglia griglia) : base(X, Y, griglia) { }

        public override bool CiboPresenteNellaCella(CElemento elemento)
        {
            return elemento is CConiglio;
        }

        public override async Task DiminuisciContatore()
        {
            await griglia.DiminuisciNumLupi();
        }

        public override async Task DiminuisciContatoreCibo()
        {
            await griglia.DiminuisciNumConigli();
        }
    }

    public class CCarota : CElemento
    {
        public CCarota(int X, int Y, Griglia griglia) : base(X, Y, griglia) { }
    }
}
