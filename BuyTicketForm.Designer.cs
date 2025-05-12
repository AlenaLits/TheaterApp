namespace TheaterApp
{
    partial class BuyTicketForm
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
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.lblSelectedSeat = new System.Windows.Forms.Label();
            this.lblPrice = new System.Windows.Forms.Label();
            this.lblFinalPrice = new System.Windows.Forms.Label();
            this.cmbDiscounts = new System.Windows.Forms.ComboBox();
            this.btnBuy = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxBasePrice = new System.Windows.Forms.TextBox();
            this.textBoxFinalPrice = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // webBrowser1
            // 
            this.webBrowser1.Location = new System.Drawing.Point(13, 13);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(392, 425);
            this.webBrowser1.TabIndex = 0;
            // 
            // lblSelectedSeat
            // 
            this.lblSelectedSeat.AutoSize = true;
            this.lblSelectedSeat.Location = new System.Drawing.Point(425, 13);
            this.lblSelectedSeat.Name = "lblSelectedSeat";
            this.lblSelectedSeat.Size = new System.Drawing.Size(175, 16);
            this.lblSelectedSeat.TabIndex = 1;
            this.lblSelectedSeat.Text = "Выберите место из схемы";
            // 
            // lblPrice
            // 
            this.lblPrice.AutoSize = true;
            this.lblPrice.Location = new System.Drawing.Point(425, 55);
            this.lblPrice.Name = "lblPrice";
            this.lblPrice.Size = new System.Drawing.Size(43, 16);
            this.lblPrice.TabIndex = 2;
            this.lblPrice.Text = "Цена:";
            // 
            // lblFinalPrice
            // 
            this.lblFinalPrice.AutoSize = true;
            this.lblFinalPrice.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblFinalPrice.Location = new System.Drawing.Point(425, 197);
            this.lblFinalPrice.Name = "lblFinalPrice";
            this.lblFinalPrice.Size = new System.Drawing.Size(121, 16);
            this.lblFinalPrice.TabIndex = 3;
            this.lblFinalPrice.Text = "Итоговая цена:";
            // 
            // cmbDiscounts
            // 
            this.cmbDiscounts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDiscounts.FormattingEnabled = true;
            this.cmbDiscounts.Location = new System.Drawing.Point(565, 88);
            this.cmbDiscounts.Name = "cmbDiscounts";
            this.cmbDiscounts.Size = new System.Drawing.Size(223, 24);
            this.cmbDiscounts.TabIndex = 4;
            this.cmbDiscounts.SelectedIndexChanged += new System.EventHandler(this.cmbDiscounts_SelectedIndexChanged);
            // 
            // btnBuy
            // 
            this.btnBuy.Location = new System.Drawing.Point(428, 241);
            this.btnBuy.Name = "btnBuy";
            this.btnBuy.Size = new System.Drawing.Size(140, 34);
            this.btnBuy.TabIndex = 5;
            this.btnBuy.Text = "Купить билет";
            this.btnBuy.UseVisualStyleBackColor = true;
            this.btnBuy.Click += new System.EventHandler(this.btnBuy_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(595, 241);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(140, 34);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Отменить";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(428, 91);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(131, 16);
            this.label1.TabIndex = 7;
            this.label1.Text = "Применить скидку:";
            // 
            // textBoxBasePrice
            // 
            this.textBoxBasePrice.Location = new System.Drawing.Point(474, 52);
            this.textBoxBasePrice.Name = "textBoxBasePrice";
            this.textBoxBasePrice.Size = new System.Drawing.Size(100, 22);
            this.textBoxBasePrice.TabIndex = 8;
            // 
            // textBoxFinalPrice
            // 
            this.textBoxFinalPrice.Location = new System.Drawing.Point(553, 197);
            this.textBoxFinalPrice.Name = "textBoxFinalPrice";
            this.textBoxFinalPrice.Size = new System.Drawing.Size(100, 22);
            this.textBoxFinalPrice.TabIndex = 9;
            // 
            // BuyTicketForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.textBoxFinalPrice);
            this.Controls.Add(this.textBoxBasePrice);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnBuy);
            this.Controls.Add(this.cmbDiscounts);
            this.Controls.Add(this.lblFinalPrice);
            this.Controls.Add(this.lblPrice);
            this.Controls.Add(this.lblSelectedSeat);
            this.Controls.Add(this.webBrowser1);
            this.Name = "BuyTicketForm";
            this.Text = "BuyTicketForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.Label lblSelectedSeat;
        private System.Windows.Forms.Label lblPrice;
        private System.Windows.Forms.Label lblFinalPrice;
        private System.Windows.Forms.ComboBox cmbDiscounts;
        private System.Windows.Forms.Button btnBuy;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxBasePrice;
        private System.Windows.Forms.TextBox textBoxFinalPrice;
    }
}