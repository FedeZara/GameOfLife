namespace GameOfLife
{
    partial class FRisultati
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblRisultato = new System.Windows.Forms.Label();
            this.lblNumConigli = new System.Windows.Forms.Label();
            this.lblNumLupi = new System.Windows.Forms.Label();
            this.btnNuovaSimulazione = new System.Windows.Forms.Button();
            this.btnChiudi = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblRisultato
            // 
            this.lblRisultato.Font = new System.Drawing.Font("Arial", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRisultato.Location = new System.Drawing.Point(0, 22);
            this.lblRisultato.Name = "lblRisultato";
            this.lblRisultato.Size = new System.Drawing.Size(338, 37);
            this.lblRisultato.TabIndex = 0;
            this.lblRisultato.Text = "I conigli hanno vinto!";
            this.lblRisultato.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNumConigli
            // 
            this.lblNumConigli.AutoSize = true;
            this.lblNumConigli.Location = new System.Drawing.Point(39, 70);
            this.lblNumConigli.Name = "lblNumConigli";
            this.lblNumConigli.Size = new System.Drawing.Size(125, 20);
            this.lblNumConigli.TabIndex = 1;
            this.lblNumConigli.Text = "10 conigli rimasti";
            // 
            // lblNumLupi
            // 
            this.lblNumLupi.AutoSize = true;
            this.lblNumLupi.Location = new System.Drawing.Point(205, 70);
            this.lblNumLupi.Name = "lblNumLupi";
            this.lblNumLupi.Size = new System.Drawing.Size(96, 20);
            this.lblNumLupi.TabIndex = 1;
            this.lblNumLupi.Text = "0 lupi rimasti";
            // 
            // btnNuovaSimulazione
            // 
            this.btnNuovaSimulazione.Location = new System.Drawing.Point(12, 112);
            this.btnNuovaSimulazione.Name = "btnNuovaSimulazione";
            this.btnNuovaSimulazione.Size = new System.Drawing.Size(168, 34);
            this.btnNuovaSimulazione.TabIndex = 2;
            this.btnNuovaSimulazione.Text = "Nuova simulazione";
            this.btnNuovaSimulazione.UseVisualStyleBackColor = true;
            this.btnNuovaSimulazione.Click += new System.EventHandler(this.btnNuovaSimulazione_Click);
            // 
            // btnChiudi
            // 
            this.btnChiudi.Location = new System.Drawing.Point(209, 112);
            this.btnChiudi.Name = "btnChiudi";
            this.btnChiudi.Size = new System.Drawing.Size(117, 34);
            this.btnChiudi.TabIndex = 2;
            this.btnChiudi.Text = "Chiudi";
            this.btnChiudi.UseVisualStyleBackColor = true;
            this.btnChiudi.Click += new System.EventHandler(this.btnChiudi_Click);
            // 
            // FRisultati
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(338, 158);
            this.Controls.Add(this.btnChiudi);
            this.Controls.Add(this.btnNuovaSimulazione);
            this.Controls.Add(this.lblNumLupi);
            this.Controls.Add(this.lblNumConigli);
            this.Controls.Add(this.lblRisultato);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "FRisultati";
            this.Text = "Game of life - Risultati";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblRisultato;
        private System.Windows.Forms.Label lblNumConigli;
        private System.Windows.Forms.Label lblNumLupi;
        private System.Windows.Forms.Button btnNuovaSimulazione;
        private System.Windows.Forms.Button btnChiudi;
    }
}