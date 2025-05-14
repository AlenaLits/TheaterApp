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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewHalls)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridViewHalls
            // 
            this.dataGridViewHalls.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewHalls.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewHalls.Location = new System.Drawing.Point(3, 3);
            this.dataGridViewHalls.Name = "dataGridViewHalls";
            this.dataGridViewHalls.RowHeadersWidth = 51;
            this.dataGridViewHalls.RowTemplate.Height = 24;
            this.dataGridViewHalls.Size = new System.Drawing.Size(794, 389);
            this.dataGridViewHalls.TabIndex = 0;
            // 
            // buttonAddHall
            // 
            this.buttonAddHall.Dock = System.Windows.Forms.DockStyle.Left;
            this.buttonAddHall.Location = new System.Drawing.Point(3, 398);
            this.buttonAddHall.Name = "buttonAddHall";
            this.buttonAddHall.Size = new System.Drawing.Size(134, 49);
            this.buttonAddHall.TabIndex = 1;
            this.buttonAddHall.Text = "Добавить зал";
            this.buttonAddHall.UseVisualStyleBackColor = true;
            this.buttonAddHall.Click += new System.EventHandler(this.buttonAddHall_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.dataGridViewHalls, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.buttonAddHall, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 87.77778F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.22222F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(800, 450);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // HallListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "HallListForm";
            this.Text = "Список залов";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewHalls)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewHalls;
        private System.Windows.Forms.Button buttonAddHall;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}