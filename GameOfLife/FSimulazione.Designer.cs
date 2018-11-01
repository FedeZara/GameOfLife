namespace GameOfLife
{
    partial class FSimulazione
    {
        /// <summary>
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Pulire le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione Windows Form

        /// <summary>
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timerSimulazione = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // timerSimulazione
            // 
            this.timerSimulazione.Interval = 1000;
            this.timerSimulazione.Tick += new System.EventHandler(this.TimerStep);
            // 
            // FSimulazione
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(784, 749);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MinimumSize = new System.Drawing.Size(1, 1);
            this.Name = "FSimulazione";
            this.Text = "Game of life";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timerSimulazione;
    }
}

