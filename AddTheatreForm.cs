using Npgsql;
using System;
using System.Windows.Forms;

namespace TheaterApp
{
    public partial class AddTheatreForm : Form
    {
        private int? theaterId = null;
        public int InsertedTheatreId { get; private set; }

        public AddTheatreForm()
        {
            InitializeComponent();
        }

        // Конструктор для редактирования
        public AddTheatreForm(int theaterId, string name, string address, string phone)
        {
            InitializeComponent();
            this.theaterId = theaterId;
            textBoxName.Text = name;
            textBoxAddress.Text = address;
            textBoxPhone.Text = phone;
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            var name = textBoxName.Text.Trim();
            var address = textBoxAddress.Text.Trim();
            var phone = textBoxPhone.Text.Trim();

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(address) || string.IsNullOrEmpty(phone))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }

            using (var conn = new NpgsqlConnection("Host=localhost;Username=postgres;Password=0813;Database=\"Theatres\""))
            {
                conn.Open();

                if (theaterId == null)
                {
                    // Проверка на уникальность имени
                    var checkCmd = new NpgsqlCommand("SELECT COUNT(*) FROM public.\"Theaters\" WHERE \"NameTheaters\" = @name", conn);
                    checkCmd.Parameters.AddWithValue("name", name);
                    var count = (long)checkCmd.ExecuteScalar();

                    if (count > 0)
                    {
                        MessageBox.Show("Театр с таким названием уже существует.");
                        return;
                    }

                    // Вставка нового театра
                    var cmd = new NpgsqlCommand("INSERT INTO public.\"Theaters\" (\"NameTheaters\", \"Address\", \"ContactPhone\") VALUES (@name, @address, @phone) RETURNING \"idTheaters\";", conn);
                    cmd.Parameters.AddWithValue("name", name);
                    cmd.Parameters.AddWithValue("address", address);
                    cmd.Parameters.AddWithValue("phone", phone);
                    InsertedTheatreId = (int)cmd.ExecuteScalar();

                    MessageBox.Show("Театр добавлен!");

                    // Предложение добавить залы
                    if (MessageBox.Show("Хотите добавить зал?", "Добавление зала", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        var addHallForm = new AddHallForm(InsertedTheatreId);
                        addHallForm.ShowDialog();
                    }
                }
                else
                {
                    // Обновление существующего театра
                    var cmd = new NpgsqlCommand("UPDATE public.\"Theaters\" SET \"NameTheaters\" = @name, \"Address\" = @address, \"ContactPhone\" = @phone WHERE \"idTheaters\" = @id", conn);
                    cmd.Parameters.AddWithValue("name", name);
                    cmd.Parameters.AddWithValue("address", address);
                    cmd.Parameters.AddWithValue("phone", phone);
                    cmd.Parameters.AddWithValue("id", theaterId.Value);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Театр обновлён!");
                }
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите отменить?", "Отмена", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                this.Close();
            }
        }
    }
}
