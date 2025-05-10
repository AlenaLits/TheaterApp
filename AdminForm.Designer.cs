namespace TheaterApp
{
    partial class AdminForm
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
            this.buttonAddPerformance = new System.Windows.Forms.Button();
            this.buttonAddTheater = new System.Windows.Forms.Button();
            this.buttonGenre = new System.Windows.Forms.Button();
            this.buttonAddSeatCategory = new System.Windows.Forms.Button();
            this.buttonAddDiscount = new System.Windows.Forms.Button();
            this.buttonReports = new System.Windows.Forms.Button();
            this.buttonRegistr = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonAddPerformance
            // 
            this.buttonAddPerformance.Location = new System.Drawing.Point(114, 64);
            this.buttonAddPerformance.Name = "buttonAddPerformance";
            this.buttonAddPerformance.Size = new System.Drawing.Size(160, 50);
            this.buttonAddPerformance.TabIndex = 0;
            this.buttonAddPerformance.Text = "Добавить спектакль";
            this.buttonAddPerformance.UseVisualStyleBackColor = true;
            this.buttonAddPerformance.Click += new System.EventHandler(this.buttonAddPerformance_Click);
            // 
            // buttonAddTheater
            // 
            this.buttonAddTheater.Location = new System.Drawing.Point(114, 195);
            this.buttonAddTheater.Name = "buttonAddTheater";
            this.buttonAddTheater.Size = new System.Drawing.Size(160, 50);
            this.buttonAddTheater.TabIndex = 1;
            this.buttonAddTheater.Text = "Добавить театр";
            this.buttonAddTheater.UseVisualStyleBackColor = true;
            this.buttonAddTheater.Click += new System.EventHandler(this.buttonAddTheater_Click);
            // 
            // buttonGenre
            // 
            this.buttonGenre.Location = new System.Drawing.Point(114, 343);
            this.buttonGenre.Name = "buttonGenre";
            this.buttonGenre.Size = new System.Drawing.Size(160, 50);
            this.buttonGenre.TabIndex = 2;
            this.buttonGenre.Text = "Добавить жанр спектакля";
            this.buttonGenre.UseVisualStyleBackColor = true;
            this.buttonGenre.Click += new System.EventHandler(this.buttonGenre_Click);
            // 
            // buttonAddSeatCategory
            // 
            this.buttonAddSeatCategory.Location = new System.Drawing.Point(496, 195);
            this.buttonAddSeatCategory.Name = "buttonAddSeatCategory";
            this.buttonAddSeatCategory.Size = new System.Drawing.Size(160, 50);
            this.buttonAddSeatCategory.TabIndex = 3;
            this.buttonAddSeatCategory.Text = "Добавить категорию места";
            this.buttonAddSeatCategory.UseVisualStyleBackColor = true;
            this.buttonAddSeatCategory.Click += new System.EventHandler(this.buttonAddSeatCategory_Click);
            // 
            // buttonAddDiscount
            // 
            this.buttonAddDiscount.Location = new System.Drawing.Point(496, 64);
            this.buttonAddDiscount.Name = "buttonAddDiscount";
            this.buttonAddDiscount.Size = new System.Drawing.Size(160, 50);
            this.buttonAddDiscount.TabIndex = 4;
            this.buttonAddDiscount.Text = "Добавить скидки";
            this.buttonAddDiscount.UseVisualStyleBackColor = true;
            this.buttonAddDiscount.Click += new System.EventHandler(this.buttonAddDiscount_Click);
            // 
            // buttonReports
            // 
            this.buttonReports.Location = new System.Drawing.Point(496, 273);
            this.buttonReports.Name = "buttonReports";
            this.buttonReports.Size = new System.Drawing.Size(160, 50);
            this.buttonReports.TabIndex = 5;
            this.buttonReports.Text = "Отчеты";
            this.buttonReports.UseVisualStyleBackColor = true;
            // 
            // buttonRegistr
            // 
            this.buttonRegistr.Location = new System.Drawing.Point(496, 343);
            this.buttonRegistr.Name = "buttonRegistr";
            this.buttonRegistr.Size = new System.Drawing.Size(160, 50);
            this.buttonRegistr.TabIndex = 6;
            this.buttonRegistr.Text = "Добавить нового администратора";
            this.buttonRegistr.UseVisualStyleBackColor = true;
            this.buttonRegistr.Click += new System.EventHandler(this.buttonRegistr_Click);
            // 
            // AdminForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.buttonRegistr);
            this.Controls.Add(this.buttonReports);
            this.Controls.Add(this.buttonAddDiscount);
            this.Controls.Add(this.buttonAddSeatCategory);
            this.Controls.Add(this.buttonGenre);
            this.Controls.Add(this.buttonAddTheater);
            this.Controls.Add(this.buttonAddPerformance);
            this.Name = "AdminForm";
            this.Text = "AdminForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonAddPerformance;
        private System.Windows.Forms.Button buttonAddTheater;
        private System.Windows.Forms.Button buttonGenre;
        private System.Windows.Forms.Button buttonAddSeatCategory;
        private System.Windows.Forms.Button buttonAddDiscount;
        private System.Windows.Forms.Button buttonReports;
        private System.Windows.Forms.Button buttonRegistr;
    }
}