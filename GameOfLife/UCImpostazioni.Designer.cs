namespace GameOfLife
{
    partial class UCImpostazioni
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

        #region Codice generato da Progettazione componenti

        /// <summary> 
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare 
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnPlayPause = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnPlayPause
            // 
            this.btnPlayPause.BackColor = System.Drawing.Color.White;
            this.btnPlayPause.Image = global::GameOfLife.Properties.Resources.play_button;
            this.btnPlayPause.Location = new System.Drawing.Point(21, 14);
            this.btnPlayPause.Name = "btnPlayPause";
            this.btnPlayPause.Size = new System.Drawing.Size(32, 32);
            this.btnPlayPause.TabIndex = 0;
            this.btnPlayPause.UseVisualStyleBackColor = false;
            this.btnPlayPause.Click += new System.EventHandler(this.btnPlayPause_Click);
            // 
            // btnStop
            // 
            this.btnStop.BackColor = System.Drawing.Color.White;
            this.btnStop.Image = global::GameOfLife.Properties.Resources.stop;
            this.btnStop.Location = new System.Drawing.Point(59, 14);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(32, 32);
            this.btnStop.TabIndex = 0;
            this.btnStop.UseVisualStyleBackColor = false;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // UCImpostazioni
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnPlayPause);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "UCImpostazioni";
            this.Size = new System.Drawing.Size(110, 60);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnPlayPause;
        private System.Windows.Forms.Button btnStop;
    }
}
