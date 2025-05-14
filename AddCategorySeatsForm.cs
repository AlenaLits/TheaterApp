using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TheaterApp
{
    public partial class AddCategorySeatsForm : Form
    {
        private int? categoryId = null;
        private string connString = "Host=localhost;Username=postgres;Password=0813;Database=\"Theatres\"";

        private string selectedColorHex = "#FFFFFF"; // Цвет по умолчанию
        public AddCategorySeatsForm()
        {
            InitializeComponent();
        }
        public AddCategorySeatsForm(int id, string name, string colorHex) : this()
        {
            categoryId = id;
            textBoxName.Text = name;
            selectedColorHex = colorHex;
            panelColorPreview.BackColor = ColorTranslator.FromHtml(colorHex);
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            string name = textBoxName.Text.Trim();

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Введите название категории.");
                return;
            }

            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                if (categoryId == null)
                {
                    // Добавление
                    var cmd = new NpgsqlCommand("INSERT INTO public.\"CategorySeats\" (\"NameCategorySeats\", \"Color\") VALUES (@name, @color)", conn);
                    cmd.Parameters.AddWithValue("name", name);
                    cmd.Parameters.AddWithValue("color", selectedColorHex);
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    // Обновление
                    var cmd = new NpgsqlCommand("UPDATE public.\"CategorySeats\" SET \"NameCategorySeats\" = @name, \"Color\" = @color WHERE \"idCategorySeats\" = @id", conn);
                    cmd.Parameters.AddWithValue("id", categoryId.Value);
                    cmd.Parameters.AddWithValue("name", name);
                    cmd.Parameters.AddWithValue("color", selectedColorHex);
                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Категория сохранена.");
            DialogResult = DialogResult.OK;
            Close();
        
        }

        private void buttonColor_Click(object sender, EventArgs e)
        {
            using (ColorDialog dlg = new ColorDialog())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    selectedColorHex = ColorTranslator.ToHtml(dlg.Color);
                    panelColorPreview.BackColor = dlg.Color;
                }
            }
        }
    }
}
