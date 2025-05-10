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
    public partial class AddDiscountForm : Form
    {
        private int? discountsId = null;
        public AddDiscountForm()
        {
            InitializeComponent();
            this.Load += new EventHandler(this.AddDiscountForm_Load);
        }

        private void AddDiscountForm_Load(object sender, EventArgs e)
        {
            LoadDiscount();
        }
        // Конструктор для редактирования
        public AddDiscountForm(int discountsId, string name, string description, string type, DateTime period, double value)
        {
            InitializeComponent();
            this.Load += new EventHandler(this.AddDiscountForm_Load);
            this.discountsId = discountsId;
            textBoxName.Text = name;
            textBoxDescription.Text = description;
            comboBoxType.SelectedItem = type;
            dateTimePickerDate.Value = period;
            textBoxValue.Text = value.ToString();
        }
        private void LoadDiscount()
        {
            string connString = "Host=localhost;Username=postgres;Password=0813;Database=\"Theatres\"";
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT \"idTypeDiscount\", \"NameTypeDiscount\" FROM public.\"TypeDiscount\"", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    var typeList = new List<KeyValuePair<int, string>>();

                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0); // idGenre
                        string name = reader.GetString(1); // NameGenre

                        typeList.Add(new KeyValuePair<int, string>(id, name));
                    }

                    comboBoxType.DataSource = typeList;
                    comboBoxType.DisplayMember = "Value"; // Показываем название
                    comboBoxType.ValueMember = "Key";     // Получаем id при необходимости
                }
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            int typeId = 0;

            // Проверка для жанра
            if (comboBoxType.SelectedItem is KeyValuePair<int, string> selectedGenre)
            {
                typeId = selectedGenre.Key;
            }
            else
            {
                MessageBox.Show("Ошибка при выборе типа скидки");
                return;
            }
            var name = textBoxName.Text.Trim();
            var description = textBoxDescription.Text.Trim();
            if (!double.TryParse(textBoxValue.Text.Trim(), out double value))
            {
                MessageBox.Show("Введите корректное числовое значение скидки.");
                return;
            }
            DateTime date = dateTimePickerDate.Value;

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Введите название скидки.");
                return;
            }
            // Основной запрос на вставку данных
            using (var conn = new NpgsqlConnection("Host=localhost;Username=postgres;Password=0813;Database=\"Theatres\""))
            {
                conn.Open();
                if (discountsId == null)
                {
                    // Проверка на уникальность имени
                    var checkCmd = new NpgsqlCommand("SELECT COUNT(*) FROM public.\"Discounts\" WHERE \"NameDiscounts\" = @name", conn);
                    checkCmd.Parameters.AddWithValue("name", name);
                    var count = (long)checkCmd.ExecuteScalar();
                    if (count > 0)
                    {
                        MessageBox.Show("Скидка с таким названием уже существует.");
                        return;
                    }
                    var cmd = new NpgsqlCommand("INSERT INTO public.\"Discounts\" (\"NameDiscounts\", \"Description\", \"TypeDiscount\", \"ValidityPeriod\", \"Value\") VALUES (@name, @desc, @type, @date, @value)", conn);
                    cmd.Parameters.AddWithValue("name", name);
                    cmd.Parameters.AddWithValue("desc", description);
                    cmd.Parameters.AddWithValue("type", typeId);
                    cmd.Parameters.AddWithValue("date", date);
                    cmd.Parameters.AddWithValue("value", value);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Скидка добавлена!");
                }
                else
                {
                    // Обновление существующего спектакля
                    var cmd = new NpgsqlCommand("UPDATE public.\"Discounts\" SET \"NameDiscounts\" = @name, \"Description\" = @description, \"TypeDiscount\" = @type, \"ValidityPeriod\" = @period, \"Value\" = @value WHERE \"idDiscounts\" = @id", conn);
                    cmd.Parameters.AddWithValue("name", name);
                    cmd.Parameters.AddWithValue("description", description);
                    cmd.Parameters.AddWithValue("type", typeId);
                    cmd.Parameters.AddWithValue("period", date);
                    cmd.Parameters.AddWithValue("value", value);
                    cmd.Parameters.AddWithValue("id", discountsId.Value);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Скидка обновлена!");
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
