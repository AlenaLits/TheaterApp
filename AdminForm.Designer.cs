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
            this.buttonRegistr = new System.Windows.Forms.Button();
            this.buttonSchedule = new System.Windows.Forms.Button();
            this.buttonLogout = new System.Windows.Forms.Button();
            this.buttonSales = new System.Windows.Forms.Button();
            this.buttonStats = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonAddPerformance
            // 
            this.buttonAddPerformance.Location = new System.Drawing.Point(12, 12);
            this.buttonAddPerformance.Name = "buttonAddPerformance";
            this.buttonAddPerformance.Size = new System.Drawing.Size(160, 50);
            this.buttonAddPerformance.TabIndex = 0;
            this.buttonAddPerformance.Text = "Спектакли";
            this.buttonAddPerformance.UseVisualStyleBackColor = true;
            this.buttonAddPerformance.Click += new System.EventHandler(this.buttonAddPerformance_Click);
            // 
            // buttonAddTheater
            // 
            this.buttonAddTheater.Location = new System.Drawing.Point(315, 12);
            this.buttonAddTheater.Name = "buttonAddTheater";
            this.buttonAddTheater.Size = new System.Drawing.Size(160, 50);
            this.buttonAddTheater.TabIndex = 1;
            this.buttonAddTheater.Text = "Театры";
            this.buttonAddTheater.UseVisualStyleBackColor = true;
            this.buttonAddTheater.Click += new System.EventHandler(this.buttonAddTheater_Click);
            // 
            // buttonGenre
            // 
            this.buttonGenre.Location = new System.Drawing.Point(12, 100);
            this.buttonGenre.Name = "buttonGenre";
            this.buttonGenre.Size = new System.Drawing.Size(160, 50);
            this.buttonGenre.TabIndex = 2;
            this.buttonGenre.Text = "Жанры спектакля";
            this.buttonGenre.UseVisualStyleBackColor = true;
            this.buttonGenre.Click += new System.EventHandler(this.buttonGenre_Click);
            // 
            // buttonAddSeatCategory
            // 
            this.buttonAddSeatCategory.Location = new System.Drawing.Point(628, 100);
            this.buttonAddSeatCategory.Name = "buttonAddSeatCategory";
            this.buttonAddSeatCategory.Size = new System.Drawing.Size(160, 50);
            this.buttonAddSeatCategory.TabIndex = 3;
            this.buttonAddSeatCategory.Text = "Добавить категорию места";
            this.buttonAddSeatCategory.UseVisualStyleBackColor = true;
            this.buttonAddSeatCategory.Click += new System.EventHandler(this.buttonAddSeatCategory_Click);
            // 
            // buttonAddDiscount
            // 
            this.buttonAddDiscount.Location = new System.Drawing.Point(315, 100);
            this.buttonAddDiscount.Name = "buttonAddDiscount";
            this.buttonAddDiscount.Size = new System.Drawing.Size(160, 50);
            this.buttonAddDiscount.TabIndex = 4;
            this.buttonAddDiscount.Text = "Скидки";
            this.buttonAddDiscount.UseVisualStyleBackColor = true;
            this.buttonAddDiscount.Click += new System.EventHandler(this.buttonAddDiscount_Click);
            // 
            // buttonRegistr
            // 
            this.buttonRegistr.Location = new System.Drawing.Point(315, 198);
            this.buttonRegistr.Name = "buttonRegistr";
            this.buttonRegistr.Size = new System.Drawing.Size(160, 50);
            this.buttonRegistr.TabIndex = 6;
            this.buttonRegistr.Text = "Добавить нового администратора";
            this.buttonRegistr.UseVisualStyleBackColor = true;
            this.buttonRegistr.Click += new System.EventHandler(this.buttonRegistr_Click);
            // 
            // buttonSchedule
            // 
            this.buttonSchedule.Location = new System.Drawing.Point(628, 12);
            this.buttonSchedule.Name = "buttonSchedule";
            this.buttonSchedule.Size = new System.Drawing.Size(160, 50);
            this.buttonSchedule.TabIndex = 7;
            this.buttonSchedule.Text = "Расписание спектаклей";
            this.buttonSchedule.UseVisualStyleBackColor = true;
            this.buttonSchedule.Click += new System.EventHandler(this.button1_Click);
            // 
            // buttonLogout
            // 
            this.buttonLogout.Location = new System.Drawing.Point(628, 198);
            this.buttonLogout.Name = "buttonLogout";
            this.buttonLogout.Size = new System.Drawing.Size(160, 50);
            this.buttonLogout.TabIndex = 8;
            this.buttonLogout.Text = "Выйти";
            this.buttonLogout.UseVisualStyleBackColor = true;
            this.buttonLogout.Click += new System.EventHandler(this.buttonLogout_Click);
            // 
            // buttonSales
            // 
            this.buttonSales.Location = new System.Drawing.Point(12, 198);
            this.buttonSales.Name = "buttonSales";
            this.buttonSales.Size = new System.Drawing.Size(159, 50);
            this.buttonSales.TabIndex = 9;
            this.buttonSales.Text = "Отчет по продажам";
            this.buttonSales.UseVisualStyleBackColor = true;
            this.buttonSales.Click += new System.EventHandler(this.buttonSales_Click);
            // 
            // buttonStats
            // 
            this.buttonStats.Location = new System.Drawing.Point(12, 283);
            this.buttonStats.Name = "buttonStats";
            this.buttonStats.Size = new System.Drawing.Size(159, 50);
            this.buttonStats.TabIndex = 10;
            this.buttonStats.Text = "Статистика по спектаклям";
            this.buttonStats.UseVisualStyleBackColor = true;
            this.buttonStats.Click += new System.EventHandler(this.buttonStats_Click);
            // 
            // AdminForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.buttonStats);
            this.Controls.Add(this.buttonSales);
            this.Controls.Add(this.buttonLogout);
            this.Controls.Add(this.buttonSchedule);
            this.Controls.Add(this.buttonRegistr);
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
        private System.Windows.Forms.Button buttonRegistr;
        private System.Windows.Forms.Button buttonSchedule;
        private System.Windows.Forms.Button buttonLogout;
        private System.Windows.Forms.Button buttonSales;
        private System.Windows.Forms.Button buttonStats;
    }
}