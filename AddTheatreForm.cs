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
    public partial class AddTheatreForm : Form
    {
        public int InsertedTheatreId { get; private set; }
        public AddTheatreForm()
        {
            InitializeComponent();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            var name = textBoxName.Text.Trim();
            var address = textBoxAddress.Text.Trim();
            var phone = textBoxPhone.Text.Trim();

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Введите название театра.");
                return;
            }
            if (string.IsNullOrEmpty(address))
            {
                MessageBox.Show("Введите адрес театра.");
                return;
            }
            if (string.IsNullOrEmpty(phone))
            {
                MessageBox.Show("Введите контактный телефон театра.");
                return;
            }

            using (var conn = new NpgsqlConnection("Host=localhost;Username=postgres;Password=0813;Database=\"Theatres\""))
            {
                conn.Open();

                // Проверка существования театра с таким же именем
                var checkCmd = new NpgsqlCommand("SELECT COUNT(*) FROM public.\"Theaters\" WHERE \"NameTheaters\" = @name", conn);
                checkCmd.Parameters.AddWithValue("name", name);
                var count = (long)checkCmd.ExecuteScalar();

                if (count > 0)
                {
                    MessageBox.Show("Театр с таким названием уже существует.");
                    return;
                }

                // Основной запрос на вставку театра с возвратом ID
                var cmd = new NpgsqlCommand("INSERT INTO public.\"Theaters\" (\"NameTheaters\", \"Address\", \"ContactPhone\") VALUES (@name, @address, @phone) RETURNING \"idTheaters\";", conn);
                cmd.Parameters.AddWithValue("name", name);
                cmd.Parameters.AddWithValue("address", address);
                cmd.Parameters.AddWithValue("phone", phone);

                // Получение ID вставленного театра
                InsertedTheatreId = (int)cmd.ExecuteScalar();
            }

            MessageBox.Show("Театр добавлен!.");

            // Открытие формы для добавления зала
            var addHallForm = new AddHallForm(InsertedTheatreId);
            addHallForm.ShowDialog();

            // Закрытие текущей формы
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
