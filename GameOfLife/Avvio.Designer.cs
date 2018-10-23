namespace GameOfLife
{
    partial class FAvvio
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
            this.lblTitolo = new System.Windows.Forms.Label();
            this.nudConigli = new System.Windows.Forms.NumericUpDown();
            this.lblNumConigli = new System.Windows.Forms.Label();
            this.lblNumLupi = new System.Windows.Forms.Label();
            this.nudLupi = new System.Windows.Forms.NumericUpDown();
            this.lblNumCarote = new System.Windows.Forms.Label();
            this.nudCarote = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.nudIntervalloCarote = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.btnInizia = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nudConigli)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLupi)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCarote)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudIntervalloCarote)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTitolo
            // 
            this.lblTitolo.AutoSize = true;
            this.lblTitolo.Font = new System.Drawing.Font("Arial", 72F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitolo.Location = new System.Drawing.Point(22, 15);
            this.lblTitolo.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblTitolo.Name = "lblTitolo";
            this.lblTitolo.Size = new System.Drawing.Size(597, 111);
            this.lblTitolo.TabIndex = 0;
            this.lblTitolo.Text = "Game of life";
            // 
            // nudConigli
            // 
            this.nudConigli.Location = new System.Drawing.Point(156, 166);
            this.nudConigli.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.nudConigli.Name = "nudConigli";
            this.nudConigli.Size = new System.Drawing.Size(145, 29);
            this.nudConigli.TabIndex = 1;
            // 
            // lblNumConigli
            // 
            this.lblNumConigli.AutoSize = true;
            this.lblNumConigli.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNumConigli.Location = new System.Drawing.Point(59, 168);
            this.lblNumConigli.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblNumConigli.Name = "lblNumConigli";
            this.lblNumConigli.Size = new System.Drawing.Size(68, 22);
            this.lblNumConigli.TabIndex = 2;
            this.lblNumConigli.Text = "Conigli";
            // 
            // lblNumLupi
            // 
            this.lblNumLupi.AutoSize = true;
            this.lblNumLupi.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNumLupi.Location = new System.Drawing.Point(59, 207);
            this.lblNumLupi.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblNumLupi.Name = "lblNumLupi";
            this.lblNumLupi.Size = new System.Drawing.Size(46, 22);
            this.lblNumLupi.TabIndex = 4;
            this.lblNumLupi.Text = "Lupi";
            // 
            // nudLupi
            // 
            this.nudLupi.Location = new System.Drawing.Point(156, 205);
            this.nudLupi.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.nudLupi.Name = "nudLupi";
            this.nudLupi.Size = new System.Drawing.Size(145, 29);
            this.nudLupi.TabIndex = 3;
            // 
            // lblNumCarote
            // 
            this.lblNumCarote.AutoSize = true;
            this.lblNumCarote.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNumCarote.Location = new System.Drawing.Point(59, 246);
            this.lblNumCarote.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblNumCarote.Name = "lblNumCarote";
            this.lblNumCarote.Size = new System.Drawing.Size(67, 22);
            this.lblNumCarote.TabIndex = 6;
            this.lblNumCarote.Text = "Carote";
            // 
            // nudCarote
            // 
            this.nudCarote.Location = new System.Drawing.Point(156, 244);
            this.nudCarote.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.nudCarote.Name = "nudCarote";
            this.nudCarote.Size = new System.Drawing.Size(145, 29);
            this.nudCarote.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(333, 251);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(166, 22);
            this.label1.TabIndex = 8;
            this.label1.Text = "Nuova carota ogni ";
            // 
            // nudIntervalloCarote
            // 
            this.nudIntervalloCarote.Location = new System.Drawing.Point(496, 249);
            this.nudIntervalloCarote.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.nudIntervalloCarote.Name = "nudIntervalloCarote";
            this.nudIntervalloCarote.Size = new System.Drawing.Size(68, 29);
            this.nudIntervalloCarote.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(567, 251);
            this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 22);
            this.label2.TabIndex = 9;
            this.label2.Text = "secondi";
            // 
            // btnInizia
            // 
            this.btnInizia.Location = new System.Drawing.Point(554, 319);
            this.btnInizia.Name = "btnInizia";
            this.btnInizia.Size = new System.Drawing.Size(191, 32);
            this.btnInizia.TabIndex = 10;
            this.btnInizia.Text = "Avvia simulazione";
            this.btnInizia.UseVisualStyleBackColor = true;
            // 
            // FAvvio
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 22F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(803, 372);
            this.Controls.Add(this.btnInizia);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.nudIntervalloCarote);
            this.Controls.Add(this.lblNumCarote);
            this.Controls.Add(this.nudCarote);
            this.Controls.Add(this.lblNumLupi);
            this.Controls.Add(this.nudLupi);
            this.Controls.Add(this.lblNumConigli);
            this.Controls.Add(this.nudConigli);
            this.Controls.Add(this.lblTitolo);
            this.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.Name = "FAvvio";
            this.Text = "Game of Life - Avvio";
            ((System.ComponentModel.ISupportInitialize)(this.nudConigli)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLupi)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCarote)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudIntervalloCarote)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitolo;
        private System.Windows.Forms.NumericUpDown nudConigli;
        private System.Windows.Forms.Label lblNumConigli;
        private System.Windows.Forms.Label lblNumLupi;
        private System.Windows.Forms.NumericUpDown nudLupi;
        private System.Windows.Forms.Label lblNumCarote;
        private System.Windows.Forms.NumericUpDown nudCarote;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nudIntervalloCarote;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnInizia;
    }
}