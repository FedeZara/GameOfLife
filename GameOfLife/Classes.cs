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
        private SemaphoreQueue simulazioneFinitaMutex = new SemaphoreQueue(1, 1);
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

            SimulazioneInPausa = true;
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
                (c.Elemento as CAnimale)?.DiminuisciVita();
                (c.Elemento as CAnimale)?.Muovi();
            }

            InserisciCarote();
        }

        public async Task DiminuisciNumConigli()
        {
            await numConigliMutex.WaitAsync();

            NumConigli--;

            if(NumConigli == 0)
            {
                await simulazioneFinitaMutex.WaitAsync();
                SimulazioneFinita = true;
                OnFineSimulazione(new FineSimulazioneEventArgs(NumConigli, NumLupi));
                simulazioneFinitaMutex.Release();
            }

            numConigliMutex.Release();
        }

        public async Task DiminuisciNumLupi()
        {
            await numLupiMutex.WaitAsync();

            NumLupi--;

            if (NumLupi == 0)
            {
                await simulazioneFinitaMutex.WaitAsync();
                SimulazioneFinita = true;
                OnFineSimulazione(new FineSimulazioneEventArgs(NumConigli, NumLupi));
                simulazioneFinitaMutex.Release();
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

                if (!SimulazioneInPausa)
                {
                    int x, y;

                    bool cellaVuotaTrovata = false;
                    while (!cellaVuotaTrovata)
                    {
                        x = rnd.Next(0, Celle.GetLength(0));
                        y = rnd.Next(0, Celle.GetLength(1));

                        await Celle[x, y].OttieniAccessoAsync();
                        if (Celle[x, y].Elemento == null)
                        {
                            cellaVuotaTrovata = true;
                            Celle[x, y].Elemento = new CCarota(x, y, this);
                            Celle[x, y].AggiornaCella();
                        }
                        try
                        {
                            Celle[x, y].RilasciaAccesso();
                        }
                        catch(Exception e)
                        {

                        }
                    }
                }

                await Task.Delay(intervalloCarote);
            }
        }

        public bool SimulazioneInPausa { get; set; }
        public void Avvia()
        {
            SimulazioneInPausa = false;
        }

        public void Pausa()
        {
            SimulazioneInPausa = true;
        }

        public void Stoppa()
        {
            SimulazioneFinita = true;
            OnFineSimulazione(new FineSimulazioneEventArgs(NumConigli, NumLupi));
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
        public SemaphoreQueue vitaMutex = new SemaphoreQueue(1, 1);
        public int Vita { get; set; }
        public CAnimale(int X, int Y, Griglia griglia) : base(X, Y, griglia) { Vita = 10; }

        public void AnalizzaCella(int xCella, int yCella, ref int newX, ref int newY, ref bool NuovaPosizioneTrovata, ref bool CiboTrovato)
        {
            // controlla se nella cella che si sta analizzando c'è del cibo
            bool ciboPresenteNellaCella = CiboPresenteNellaCella(griglia.Celle[xCella, yCella].Elemento);

            // se è già stata trovata una cella vicina con del cibo e la cella in analisi contiene del cibo
            // ha una possibilità del 50% di rendere la cella in analisi la nuova cella verso cui spostarsi
            if (CiboTrovato && ciboPresenteNellaCella && griglia.rnd.Next(0, 100) < 50)
            {
                // non occorre più avere l'accesso della cella precedente
                griglia.Celle[newX, newY].RilasciaAccesso();

                newX = xCella;
                newY = yCella;
            }

            // se non è ancora stata trovata una cella vicina con del cibo e la cella in analisi contiene del cibo
            // rendi la cella in analisi la nuova cella verso cui spostarsi
            else if (!CiboTrovato && ciboPresenteNellaCella)
            {
                // se era già stata selezionata un'altra cella come prossima cella, rilascia l'accesso
                if (NuovaPosizioneTrovata)
                    griglia.Celle[newX, newY].RilasciaAccesso();

                newX = xCella;
                newY = yCella;
                CiboTrovato = true;
                NuovaPosizioneTrovata = true;
            }

            // se non è stato trovato ancora cibo, la cella in analisi non ne contiene ed è vuota, 
            // segna la cella in analisi come prossima cella se non è stata trovata nessuna nuova posizione
            // oppure segnala con il 50% di probabilità se è già stata trovata un'altra cella verso cui muoversi
            else if (!CiboTrovato && (!NuovaPosizioneTrovata || griglia.rnd.Next(0, 100) < 50) && griglia.Celle[xCella, yCella].Elemento == null)
            {
                // se era già stata selezionata un'altra cella come prossima cella, rilascia l'accesso
                if (NuovaPosizioneTrovata)
                    griglia.Celle[newX, newY].RilasciaAccesso();

                newX = xCella;
                newY = yCella;
                NuovaPosizioneTrovata = true;
            }

            // nel caso la cella in analisi non sia stata segnata come prossima cella, il suo accesso si può rilasciare
            if (newX != xCella || newY != yCella)
                griglia.Celle[xCella, yCella].RilasciaAccesso();
        }
        public async void Muovi()
        {
            while (true)
            {
                if (griglia.SimulazioneFinita)
                    return;

                if (!griglia.SimulazioneInPausa)
                {
                    if (Y > 0)
                        await griglia.Celle[X, Y - 1].OttieniAccessoAsync();
                    if (X > 0)
                        await griglia.Celle[X - 1, Y].OttieniAccessoAsync();


                    await griglia.Celle[X, Y].OttieniAccessoAsync();

                    var animale = griglia.Celle[X, Y].Elemento as CAnimale;
                    if (animale != null)
                    {
                        if(animale.Vita == 0)
                        {
                            griglia.Celle[X, Y].Elimina();
                            await DiminuisciContatore();
                        }
                    }

                    if (griglia.Celle[X, Y].Elemento != this)
                    {
                        griglia.Celle[X, Y].RilasciaAccesso();
                        
                        if (Y > 0)
                            griglia.Celle[X, Y - 1].RilasciaAccesso();
                        if (X > 0)
                            griglia.Celle[X - 1, Y].RilasciaAccesso();

                        return;
                    }

                    if (X < griglia.Celle.GetLength(0) - 1)
                        await griglia.Celle[X + 1, Y].OttieniAccessoAsync();
                    if (Y < griglia.Celle.GetLength(1) - 1)
                        await griglia.Celle[X, Y + 1].OttieniAccessoAsync();


                    int newX = -1, newY = -1;
                    bool CiboTrovato = false, NuovaPosizioneTrovata = false;


                    if (Y > 0)
                    {
                        AnalizzaCella(X, Y - 1, ref newX, ref newY, ref NuovaPosizioneTrovata, ref CiboTrovato);
                    }

                    if (X > 0)
                    {
                        AnalizzaCella(X - 1, Y, ref newX, ref newY, ref NuovaPosizioneTrovata, ref CiboTrovato);
                    }

                    if (X < griglia.Celle.GetLength(0) - 1)
                    {
                        AnalizzaCella(X + 1, Y, ref newX, ref newY, ref NuovaPosizioneTrovata, ref CiboTrovato);
                    }

                    if (Y < griglia.Celle.GetLength(1) - 1)
                    {
                        AnalizzaCella(X, Y + 1, ref newX, ref newY, ref NuovaPosizioneTrovata, ref CiboTrovato);
                    }

                    

                    if (NuovaPosizioneTrovata)
                    {
                        int oldX, oldY;
                        oldX = X;
                        oldY = Y;
                        X = newX;
                        Y = newY;
                        if (CiboTrovato)
                        {
                            await vitaMutex.WaitAsync();
                            Vita = 10;
                            vitaMutex.Release();
                            await DiminuisciContatoreCibo();
                        }
                        griglia.Celle[X, Y].Elemento = this;
                        griglia.Celle[X, Y].AggiornaCella();
                        griglia.Celle[oldX, oldY].Elimina();
                        griglia.Celle[oldX, oldY].RilasciaAccesso();
                        griglia.Celle[X, Y].RilasciaAccesso();
                    }
                    else
                    {
                        griglia.Celle[X, Y].RilasciaAccesso();
                    }
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

                if (!griglia.SimulazioneInPausa)
                {
                    await vitaMutex.WaitAsync();
                    Vita--;
                    vitaMutex.Release();
                    int a = X, b = Y;
                    griglia.Celle[X, Y].AggiornaCella();

                    if (Vita == 0)
                    {
                        return;
                    }
                }
                
                await Task.Delay(1000);
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
