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
        public AddCategorySeatsForm()
        {
            InitializeComponent();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            var name = textBoxName.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Введите категорию места.");
                return;
            }
            // Основной запрос на вставку данных
            using (var conn = new NpgsqlConnection("Host=localhost;Username=postgres;Password=0813;Database=\"Theatres\""))
            {
                conn.Open();
                var cmd = new NpgsqlCommand("INSERT INTO public.\"CategorySeats\" (\"NameCategorySeats\") VALUES (@name)", conn);
                cmd.Parameters.AddWithValue("name", name);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Категория добавлена!.");
            this.Close();
        }
    }
}
