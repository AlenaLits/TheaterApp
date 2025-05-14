using Npgsql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace TheaterApp
{
    public partial class AddHallForm : Form
    {
        private readonly int theatreId;
        private readonly int? hallIdToEdit = null;
        private byte[] svgData;
        private readonly HashSet<string> selectedElements = new HashSet<string>();

        public AddHallForm(int theatreId)
        {
            InitializeComponent();
            this.theatreId = theatreId;
            this.webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(this.webBrowser1_DocumentCompleted);
            LoadCategories();
        }

        // Конструктор для редактирования зала
        public AddHallForm(int theatreId, int hallId, string name, int capacity)
        {
            InitializeComponent();
            this.theatreId = theatreId;
            this.hallIdToEdit = hallId;

            textBoxHallName.Text = name;
            numericUpDownSeats.Value = capacity;

            this.webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(this.webBrowser1_DocumentCompleted);

            LoadSvgFromDatabase(hallId);
            LoadCategories();
        }

        private void LoadSvgFromDatabase(int hallId)
        {
            using (var conn = new NpgsqlConnection("Host=localhost;Username=postgres;Password=0813;Database=\"Theatres\""))
            {
                conn.Open();
                var cmd = new NpgsqlCommand("SELECT \"SchemeHalls\" FROM public.\"Halls\" WHERE \"idHalls\" = @id", conn);
                cmd.Parameters.AddWithValue("id", hallId);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read() && !reader.IsDBNull(0))
                    {
                        svgData = (byte[])reader["SchemeHalls"];
                        string svgContent = Encoding.UTF8.GetString(svgData);

                        // Получаем цвет для каждого места
                        var seatColors = GetSeatColorsByCategory();

                        // Генерируем JS для окрашивания
                        StringBuilder coloringScript = new StringBuilder();
                        coloringScript.AppendLine("<script>");
                        coloringScript.AppendLine("window.onload = function() {");

                        foreach (var pair in seatColors)
                        {
                            // ID в SVG предполагается вида seat23
                            string elementId = $"\"seat{pair.Key}\"";
                            coloringScript.AppendLine($@"
                                var el = document.getElementById({elementId});
                                if (el) {{
                                    el.setAttribute('fill', '{pair.Value}');
                                }}
                            ");
                        }

                        coloringScript.AppendLine("};");
                        coloringScript.AppendLine("</script>");

                        string htmlContent = $@"
                        <html>
                            <head>
                                <meta http-equiv='X-UA-Compatible' content='IE=edge'>
                                {coloringScript}
                            </head>
                            <body>
                                {svgContent}
                            </body>
                        </html>";

                        webBrowser1.DocumentText = htmlContent;
                    }
                }
            }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "SVG Files (*.svg)|*.svg";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    svgData = File.ReadAllBytes(ofd.FileName);
                    MessageBox.Show("Схема загружена!");

                    string svgContent = File.ReadAllText(ofd.FileName, Encoding.UTF8);
                    string htmlContent = $@"
<html>
    <head>
        <meta http-equiv='X-UA-Compatible' content='IE=edge'>
        //<style>
        //    path {{
        //        fill: blue;
        //        stroke: black;
        //        stroke-width: 2;
        //    }}
        //    rect {{
        //        fill: blue;
        //        stroke: black;
        //        stroke-width: 2;
        //    }}
        //</style>
        
    </head>
    <body>
        {svgContent}
    </body>
</html>";

                    webBrowser1.DocumentText = htmlContent;
                }
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            var hallName = textBoxHallName.Text.Trim();
            var seatsCount = (int)numericUpDownSeats.Value;

            if (string.IsNullOrEmpty(hallName) || svgData == null)
            {
                MessageBox.Show("Введите название зала и загрузите схему.");
                return;
            }

            using (var conn = new NpgsqlConnection("Host=localhost;Username=postgres;Password=0813;Database=\"Theatres\""))
            {
                conn.Open();

                if (hallIdToEdit.HasValue)
                {
                    // Режим редактирования
                    var cmd = new NpgsqlCommand("UPDATE public.\"Halls\" SET \"NameHalls\" = @name, \"CountSeats\" = @count, \"SchemeHalls\" = @scheme WHERE \"idHalls\" = @id", conn);
                    cmd.Parameters.AddWithValue("name", hallName);
                    cmd.Parameters.AddWithValue("count", seatsCount);
                    cmd.Parameters.AddWithValue("scheme", svgData);
                    cmd.Parameters.AddWithValue("id", hallIdToEdit.Value);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Зал успешно обновлён!");
                }
                else
                {
                    // Режим добавления
                    var cmd = new NpgsqlCommand("INSERT INTO public.\"Halls\" (\"Theaters\", \"NameHalls\", \"CountSeats\", \"SchemeHalls\") VALUES (@theatre, @name, @count, @scheme)", conn);
                    cmd.Parameters.AddWithValue("theatre", theatreId);
                    cmd.Parameters.AddWithValue("name", hallName);
                    cmd.Parameters.AddWithValue("count", seatsCount);
                    cmd.Parameters.AddWithValue("scheme", svgData);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Зал добавлен!");
                }
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        private void LoadCategories()
        {
            using (var conn = new NpgsqlConnection("Host=localhost;Username=postgres;Password=0813;Database=\"Theatres\""))
            {
                conn.Open();
                var cmd = new NpgsqlCommand("SELECT \"idCategorySeats\", \"NameCategorySeats\" FROM public.\"CategorySeats\"", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        comboBoxCategories.Items.Add(new
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        });
                    }
                }
            }

            comboBoxCategories.DisplayMember = "Name";
            comboBoxCategories.ValueMember = "Id";
        }
        private void ButtonAssignCategory_Click(object sender, EventArgs e)
        {
            if (selectedElements.Count == 0)
            {
                MessageBox.Show("Выберите хотя бы одно место.");
                return;
            }

            if (comboBoxCategories.SelectedItem == null)
            {
                MessageBox.Show("Выберите категорию.");
                return;
            }

            dynamic selectedCategory = comboBoxCategories.SelectedItem;
            int categoryId = selectedCategory.Id;

            AssignCategoryToSelectedSeats(categoryId); // Назначаем категорию выбранным местам
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
        private Dictionary<string, bool> elementClickHandlers = new Dictionary<string, bool>();

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            var document = webBrowser1.Document;

            string[] clickableTags = { "path", "circle", "rect", "ellipse", "polygon" };

            foreach (string tag in clickableTags)
            {
                foreach (HtmlElement element in document.GetElementsByTagName(tag))
                {
                    // Get the ID of the element
                    string id = element.GetAttribute("id");

                    // Skip this element if it already has an event handler attached
                    if (!string.IsNullOrEmpty(id) && !elementClickHandlers.ContainsKey(id))
                    {
                        // Attach the click event handler if not already done
                        element.AttachEventHandler("onclick", delegate
                        {
                            Element_Click(element);
                        });

                        // Mark that this element has a click handler
                        elementClickHandlers[id] = true;
                    }
                }
            }
        }
        private void AssignCategoryToSelectedSeats(int categoryId)
        {
            using (var conn = new NpgsqlConnection("Host=localhost;Username=postgres;Password=0813;Database=\"Theatres\""))
            {
                conn.Open();

                foreach (string seatId in selectedElements)
                {
                    // Используем регулярное выражение для извлечения чисел из строки ID
                    var match = System.Text.RegularExpressions.Regex.Match(seatId, @"\d+");

                    if (match.Success)
                    {
                        // Извлекаем только числовую часть из строки
                        int seatIdInt = int.Parse(match.Value);

                        // Выполняем добавление новой строки в таблицу Seats
                        var insertCmd = new NpgsqlCommand("INSERT INTO public.\"Seats\" (\"Category\", \"Halls\", \"NumberSeats\") VALUES (@category, @hallId, @seatId)", conn);
                        insertCmd.Parameters.AddWithValue("category", categoryId);
                        insertCmd.Parameters.AddWithValue("hallId", hallIdToEdit.Value); // hallIdToEdit — это id текущего зала
                        insertCmd.Parameters.AddWithValue("seatId", seatIdInt); // Используем извлечённое число
                        insertCmd.ExecuteNonQuery();
                    }
                    else
                    {
                        // Если не удалось извлечь числа из ID, показываем сообщение
                        MessageBox.Show($"Невозможно извлечь номер места из ID: {seatId}");
                    }
                }

                MessageBox.Show("Категория назначена выбранным местам.");
                selectedElements.Clear(); // Очистка выделения после назначения категории
                LoadSvgFromDatabase(hallIdToEdit.Value);
            }
        }

        private void Element_Click(HtmlElement element)
        {
            string id = element.GetAttribute("id");

            if (string.IsNullOrEmpty(id))
            {
                MessageBox.Show("У элемента нет ID!");
                return;
            }

            //MessageBox.Show($"Вы кликнули по элементу с ID: {id}");

            // Логика изменения стиля и работы с selectedElements
            if (selectedElements.Contains(id))
            {
                selectedElements.Remove(id);
                element.Style = "fill: blue; stroke: black; stroke-width: 2;"; // Сброс цвета
            }
            else
            {
                selectedElements.Add(id);
                element.Style = "fill: orange; stroke: black; stroke-width: 2;"; // Выделяем цветом
            }

            Console.WriteLine($"Текущие выбранные элементы: {string.Join(", ", selectedElements)}");
        }
        private Dictionary<int, string> GetSeatColorsByCategory()
        {
            var result = new Dictionary<int, string>();

            if (!hallIdToEdit.HasValue)
                return result;

            using (var conn = new NpgsqlConnection("Host=localhost;Username=postgres;Password=0813;Database=\"Theatres\""))
            {
                conn.Open();
                var cmd = new NpgsqlCommand(@"
                    SELECT s.""NumberSeats"", c.""Color""
        
                            FROM public.""Seats"" s
                    JOIN public.""CategorySeats"" c ON s.""Category"" = c.""idCategorySeats""
                    WHERE s.""Halls"" = @hallId", conn);
                cmd.Parameters.AddWithValue("hallId", hallIdToEdit.Value);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int number = reader.GetInt32(0);
                string color = reader.GetString(1);
                result[number] = color;
                    }
                }
            }

            return result;
        }
    }
}
