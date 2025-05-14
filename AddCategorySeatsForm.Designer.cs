namespace TheaterApp
{
    partial class AddCategorySeatsForm
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
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.labelName = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonColor = new System.Windows.Forms.Button();
            this.panelColorPreview = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(418, 279);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(100, 30);
            this.buttonCancel.TabIndex = 17;
            this.buttonCancel.Text = "Отменить";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(233, 279);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(100, 30);
            this.buttonSave.TabIndex = 16;
            this.buttonSave.Text = "Сохранить";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // textBoxName
            // 
            this.textBoxName.Location = new System.Drawing.Point(388, 95);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(200, 22);
            this.textBoxName.TabIndex = 15;
            // 
            // labelName
            // 
            this.labelName.AutoSize = true;
            this.labelName.Location = new System.Drawing.Point(213, 95);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(120, 16);
            this.labelName.TabIndex = 14;
            this.labelName.Text = "Категория места:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(216, 144);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 16);
            this.label1.TabIndex = 18;
            this.label1.Text = "Цвет:";
            // 
            // buttonColor
            // 
            this.buttonColor.Location = new System.Drawing.Point(290, 140);
            this.buttonColor.Name = "buttonColor";
            this.buttonColor.Size = new System.Drawing.Size(130, 24);
            this.buttonColor.TabIndex = 19;
            this.buttonColor.Text = "Выбрать цвет";
            this.buttonColor.UseVisualStyleBackColor = true;
            this.buttonColor.Click += new System.EventHandler(this.buttonColor_Click);
            // 
            // panelColorPreview
            // 
            this.panelColorPreview.Location = new System.Drawing.Point(446, 140);
            this.panelColorPreview.Name = "panelColorPreview";
            this.panelColorPreview.Size = new System.Drawing.Size(200, 100);
            this.panelColorPreview.TabIndex = 20;
            // 
            // AddCategorySeatsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.panelColorPreview);
            this.Controls.Add(this.buttonColor);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.textBoxName);
            this.Controls.Add(this.labelName);
            this.Name = "AddCategorySeatsForm";
            this.Text = "AddCategorySeatsForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonColor;
        private System.Windows.Forms.Panel panelColorPreview;
    }
}