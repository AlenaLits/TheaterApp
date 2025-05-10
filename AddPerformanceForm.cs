using Npgsql;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml.Linq;

namespace TheaterApp
{
    public partial class AddPerformanceForm : Form
    {
        private int? performanceId = null;

        // Конструктор для редактирования
        public AddPerformanceForm(int performanceId, string name, string description, string genre, string ageRestrictions, int duration)
        {
            InitializeComponent();
            this.Load += new EventHandler(this.AddPerformanceForm_Load);
            this.performanceId = performanceId;
            textBoxName.Text = name;
            textBoxDescription.Text = description;
            comboBoxGenre.SelectedItem = genre;
            comboBoxAgeRating.SelectedItem = ageRestrictions;
            int hours = duration / 60;
            int minutes = duration - hours * 60;
            numericUpDownHours.Value = hours;
            numericUpDownMinutes.Value = minutes;
        }
        public AddPerformanceForm()
        {
            InitializeComponent();
            this.Load += new EventHandler(this.AddPerformanceForm_Load);
        }

        private void AddPerformanceForm_Load(object sender, EventArgs e)
        {
            LoadGenres();
            LoadAgeRatings();
        }

        // Класс для возрастных ограничений
        private class AgeLimitItem
        {
            public int Id { get; set; }
            public int Age { get; set; }

            public override string ToString()
            {
                return Age.ToString(); // Здесь мы возвращаем значение типа int как строку
            }
        }

        // Метод загрузки жанров
        private void LoadGenres()
        {
            string connString = "Host=localhost;Username=postgres;Password=0813;Database=\"Theatres\"";
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT \"idGenre\", \"NameGenre\" FROM public.\"Genre\"", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    var genreList = new List<KeyValuePair<int, string>>();

                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0); // idGenre
                        string name = reader.GetString(1); // NameGenre

                        genreList.Add(new KeyValuePair<int, string>(id, name));
                    }

                    comboBoxGenre.DataSource = genreList;
                    comboBoxGenre.DisplayMember = "Value"; // Показываем название
                    comboBoxGenre.ValueMember = "Key";     // Получаем id при необходимости
                }
            }
        }

        // Метод загрузки возрастных ограничений
        private void LoadAgeRatings()
        {
            string connString = "Host=localhost;Username=postgres;Password=0813;Database=\"Theatres\"";
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT \"idAgeRestrictions\", \"ValueAgeRestrictions\" FROM public.\"AgeRestrictions\"", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    var ageLimitItems = new List<AgeLimitItem>();

                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);          // idAgeLimit
                        int age = reader.GetInt32(1);         // ValueAgeLimit — числовое значение

                        ageLimitItems.Add(new AgeLimitItem { Id = id, Age = age });
                    }

                    comboBoxAgeRating.DataSource = ageLimitItems;
                    comboBoxAgeRating.DisplayMember = "Age";  // Display the age value
                    comboBoxAgeRating.ValueMember = "Id";    // Use the ID as the value
                }
            }
        }


        // Сохранение добавленного спектакля
        private void buttonSave_Click(object sender, EventArgs e)
        {
            int genreId = 0;
            int ageRatingId = 0;

            // Проверка для жанра
            if (comboBoxGenre.SelectedItem is KeyValuePair<int, string> selectedGenre)
            {
                genreId = selectedGenre.Key;
            }
            else
            {
                MessageBox.Show("Ошибка при выборе жанра");
                return;
            }

            // Проверка для возрастного ограничения
            if (comboBoxAgeRating.SelectedItem is AgeLimitItem selectedAgeRating) // Cast to AgeLimitItem
            {
                ageRatingId = selectedAgeRating.Id; // Use the Id from AgeLimitItem
            }
            else
            {
                MessageBox.Show("Ошибка при выборе возрастного ограничения");
                return;
            }

            var name = textBoxName.Text.Trim();
            var description = textBoxDescription.Text.Trim();
            int hours = (int)numericUpDownHours.Value;
            int minutes = (int)numericUpDownMinutes.Value;
            int totalMinutes = hours * 60 + minutes;

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Введите название спектакля.");
                return;
            }

            if (totalMinutes == 0)
            {
                MessageBox.Show("Длительность не может быть 0.");
                return;
            }

            // Основной запрос на вставку данных
            using (var conn = new NpgsqlConnection("Host=localhost;Username=postgres;Password=0813;Database=\"Theatres\""))
            {
                
                conn.Open();
                if (performanceId == null)
                {
                    // Проверка на уникальность имени
                    var checkCmd = new NpgsqlCommand("SELECT COUNT(*) FROM public.\"Performances\" WHERE \"NamePerformances\" = @name", conn);
                    checkCmd.Parameters.AddWithValue("name", name);
                    var count = (long)checkCmd.ExecuteScalar();

                    if (count > 0)
                    {
                        MessageBox.Show("Спектакль с таким названием уже существует.");
                        return;
                    }
                    var cmd = new NpgsqlCommand("INSERT INTO public.\"Performances\" (\"NamePerformances\", \"Description\", \"Duration\", \"Genre\", \"AgeRestrictions\") VALUES (@name, @desc, @duration, @genre, @age)", conn);
                    cmd.Parameters.AddWithValue("name", name);
                    cmd.Parameters.AddWithValue("desc", description);
                    cmd.Parameters.AddWithValue("duration", totalMinutes);
                    cmd.Parameters.AddWithValue("genre", genreId);
                    cmd.Parameters.AddWithValue("age", ageRatingId);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Спекталь добавлен!");
                }
                else
                {
                    // Обновление существующего спектакля
                    var cmd = new NpgsqlCommand("UPDATE public.\"Performances\" SET \"NamePerformances\" = @name, \"Description\" = @description, \"Duration\" = @duration, \"Genre\" = @genre, \"AgeRestrictions\" = @ageRestrictions WHERE \"idPerformances\" = @id", conn);
                    cmd.Parameters.AddWithValue("name", name);
                    cmd.Parameters.AddWithValue("description", description);
                    cmd.Parameters.AddWithValue("duration", totalMinutes);
                    cmd.Parameters.AddWithValue("genre", genreId);
                    cmd.Parameters.AddWithValue("ageRestrictions", ageRatingId);
                    cmd.Parameters.AddWithValue("id", performanceId.Value);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Спектакль обновлён!");
                }
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        // Отмена
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
