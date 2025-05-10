using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TheaterApp
{
    public partial class AddGenreForm : Form
    {
        private int? genreId = null;
        public AddGenreForm()
        {
            InitializeComponent();
        }
        // Конструктор для редактирования
        public AddGenreForm(int genreId, string name)
        {
            InitializeComponent();
            this.genreId = genreId;
            textBoxName.Text = name;
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            var name = textBoxName.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Введите название жанра.");
                return;
            }
            // Основной запрос на вставку данных
            using (var conn = new NpgsqlConnection("Host=localhost;Username=postgres;Password=0813;Database=\"Theatres\""))
            {
                conn.Open();
                if (genreId == null)
                {
                    // Проверка на уникальность имени
                    var checkCmd = new NpgsqlCommand("SELECT COUNT(*) FROM public.\"Genre\" WHERE \"NameGenre\" = @name", conn);
                    checkCmd.Parameters.AddWithValue("name", name);
                    var count = (long)checkCmd.ExecuteScalar();

                    if (count > 0)
                    {
                        MessageBox.Show("Жанр с таким названием уже существует.");
                        return;
                    }

                    // Вставка нового театра
                    var cmd = new NpgsqlCommand("INSERT INTO public.\"Genre\" (\"NameGenre\") VALUES (@name)", conn);
                    cmd.Parameters.AddWithValue("name", name);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Жанр добавлен!");
                }
                else
                {
                    // Обновление существующего театра
                    var cmd = new NpgsqlCommand("UPDATE public.\"Genre\" SET \"NameGenre\" = @name WHERE \"idGenre\" = @id", conn);
                    cmd.Parameters.AddWithValue("name", name);
                    cmd.Parameters.AddWithValue("id", genreId.Value);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Жанр обновлён!");
                }

            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
            "Вы уверены, что хотите отменить и закрыть форму?",
            "Отменить подтверждение",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                this.Close();
            }
        }
    }
}
