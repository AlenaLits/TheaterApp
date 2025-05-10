namespace TheaterApp
{
    partial class HallListForm
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
            this.dataGridViewHalls = new System.Windows.Forms.DataGridView();
            this.buttonAddHall = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewHalls)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewHalls
            // 
            this.dataGridViewHalls.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewHalls.Location = new System.Drawing.Point(12, 12);
            this.dataGridViewHalls.Name = "dataGridViewHalls";
            this.dataGridViewHalls.RowHeadersWidth = 51;
            this.dataGridViewHalls.RowTemplate.Height = 24;
            this.dataGridViewHalls.Size = new System.Drawing.Size(776, 374);
            this.dataGridViewHalls.TabIndex = 0;
            // 
            // buttonAddHall
            // 
            this.buttonAddHall.Location = new System.Drawing.Point(333, 408);
            this.buttonAddHall.Name = "buttonAddHall";
            this.buttonAddHall.Size = new System.Drawing.Size(134, 30);
            this.buttonAddHall.TabIndex = 1;
            this.buttonAddHall.Text = "Добавить зал";
            this.buttonAddHall.UseVisualStyleBackColor = true;
            this.buttonAddHall.Click += new System.EventHandler(this.buttonAddHall_Click);
            // 
            // HallListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.buttonAddHall);
            this.Controls.Add(this.dataGridViewHalls);
            this.Name = "HallListForm";
            this.Text = "HallListForm";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewHalls)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewHalls;
        private System.Windows.Forms.Button buttonAddHall;
    }
}