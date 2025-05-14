namespace TheaterApp
{
    partial class ClientForm
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
            this.btnTheaters = new System.Windows.Forms.Button();
            this.btnPerformances = new System.Windows.Forms.Button();
            this.btnMyTickets = new System.Windows.Forms.Button();
            this.buttonLogout = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnTheaters
            // 
            this.btnTheaters.Location = new System.Drawing.Point(30, 146);
            this.btnTheaters.Name = "btnTheaters";
            this.btnTheaters.Size = new System.Drawing.Size(200, 40);
            this.btnTheaters.TabIndex = 0;
            this.btnTheaters.Text = "Театры";
            this.btnTheaters.UseVisualStyleBackColor = true;
            this.btnTheaters.Click += new System.EventHandler(this.btnTheaters_Click);
            // 
            // btnPerformances
            // 
            this.btnPerformances.Location = new System.Drawing.Point(263, 146);
            this.btnPerformances.Name = "btnPerformances";
            this.btnPerformances.Size = new System.Drawing.Size(200, 40);
            this.btnPerformances.TabIndex = 1;
            this.btnPerformances.Text = "Спектакли";
            this.btnPerformances.UseVisualStyleBackColor = true;
            this.btnPerformances.Click += new System.EventHandler(this.btnPerformances_Click);
            // 
            // btnMyTickets
            // 
            this.btnMyTickets.Location = new System.Drawing.Point(499, 146);
            this.btnMyTickets.Name = "btnMyTickets";
            this.btnMyTickets.Size = new System.Drawing.Size(200, 40);
            this.btnMyTickets.TabIndex = 2;
            this.btnMyTickets.Text = "Мои билеты";
            this.btnMyTickets.UseVisualStyleBackColor = true;
            this.btnMyTickets.Click += new System.EventHandler(this.btnMyTickets_Click);
            // 
            // buttonLogout
            // 
            this.buttonLogout.Location = new System.Drawing.Point(263, 380);
            this.buttonLogout.Name = "buttonLogout";
            this.buttonLogout.Size = new System.Drawing.Size(200, 40);
            this.buttonLogout.TabIndex = 3;
            this.buttonLogout.Text = "Выйти";
            this.buttonLogout.UseVisualStyleBackColor = true;
            this.buttonLogout.Click += new System.EventHandler(this.buttonLogout_Click);
            // 
            // ClientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.buttonLogout);
            this.Controls.Add(this.btnMyTickets);
            this.Controls.Add(this.btnPerformances);
            this.Controls.Add(this.btnTheaters);
            this.Name = "ClientForm";
            this.Text = "Форма для покупателя";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnTheaters;
        private System.Windows.Forms.Button btnPerformances;
        private System.Windows.Forms.Button btnMyTickets;
        private System.Windows.Forms.Button buttonLogout;
    }
}