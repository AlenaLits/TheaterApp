using DocumentFormat.OpenXml.Office.Word;
using Npgsql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace TheaterApp
{
    public partial class BuyTicketForm : Form
    {
        private readonly int clientId;
        private readonly int scheduleId;
        private readonly string performanceName;
        private byte[] svgData;
        private int selectedSeat = -1; // Для хранения выбранного места
        private Dictionary<string, bool> elementClickHandlers = new Dictionary<string, bool>();
        private readonly HashSet<string> selectedElements = new HashSet<string>();

        public BuyTicketForm(int clientId, int scheduleId, string performanceName)
        {
            InitializeComponent();
            this.clientId = clientId;
            this.scheduleId = scheduleId;
            this.performanceName = performanceName;
            this.Text = $"Покупка билета: {performanceName}";

            webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(this.webBrowser1_DocumentCompleted);
            LoadSeatScheme();
            LoadDiscounts();
        }

        private void LoadSeatScheme()
        {
            using (var conn = new NpgsqlConnection("Host=localhost;Username=postgres;Password=0813;Database=\"Theatres\""))
            {
                conn.Open();

                var cmd = new NpgsqlCommand(@"
                    SELECT h.""SchemeHalls""
                    FROM public.""PerformanceSchedule"" ps
                    JOIN public.""Halls"" h ON ps.""Hall"" = h.""idHalls""
                    WHERE ps.""idSchedule"" = @scheduleId", conn);
                cmd.Parameters.AddWithValue("scheduleId", scheduleId);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read() && !reader.IsDBNull(0))
                    {
                        svgData = (byte[])reader["SchemeHalls"];
                        string svgContent = Encoding.UTF8.GetString(svgData);
                        var occupiedSeats = GetOccupiedSeatIds();

                        foreach (var seatId in occupiedSeats)
                        {
                            // Пример: меняем fill у места с id=seatId на серый
                            svgContent = svgContent.Replace($"id=\"{seatId}\"", $"id=\"{seatId}\" style=\"fill:green;pointer-events:none;\"");
                        }
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
        }
        private HashSet<int> GetOccupiedSeatIds()
        {
            var occupiedSeats = new HashSet<int>();

            using (var conn = new NpgsqlConnection("Host=localhost;Username=postgres;Password=0813;Database=\"Theatres\""))
            {
                conn.Open();
                var cmd = new NpgsqlCommand(@"SELECT ""Seats"" FROM public.""Tickets"" WHERE ""Schedule"" = @scheduleId", conn);
                cmd.Parameters.AddWithValue("scheduleId", scheduleId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        occupiedSeats.Add(reader.GetInt32(0));
                    }
                }
            }

            return occupiedSeats;
        }
        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            var document = webBrowser1.Document;

            string[] clickableTags = { "path", "circle", "rect", "ellipse", "polygon" };

            foreach (string tag in clickableTags)
            {
                foreach (HtmlElement element in document.GetElementsByTagName(tag))
                {
                    string id = element.GetAttribute("id");

                    if (string.IsNullOrEmpty(id))
                    {
                        Console.WriteLine("Элемент не имеет ID, пропускаем.");
                        continue;  // Пропускаем элементы без ID
                    }

                    if (!elementClickHandlers.ContainsKey(id))
                    {
                        element.AttachEventHandler("onclick", delegate
                        {
                            Element_Click(element);
                        });

                        elementClickHandlers[id] = true;
                    }
                }
            }
        }
        private void LoadDiscounts()
        {
            using (var conn = new NpgsqlConnection("Host=localhost;Username=postgres;Password=0813;Database=\"Theatres\""))
            {
                conn.Open();

                var cmd = new NpgsqlCommand(@"
            SELECT d.""NameDiscounts"", d.""Value"", d.""TypeDiscount""
            FROM public.""Discounts"" d
            WHERE d.""ValidityPeriod"" > NOW()", conn); // только актуальные скидки
                using (var reader = cmd.ExecuteReader())
                {
                    var discounts = new List<KeyValuePair<string, Tuple<int, decimal>>>();
                    while (reader.Read())
                    {
                        string discountName = reader.GetString(0);
                        decimal discountValue = reader.GetDecimal(1);
                        int discountType = reader.GetInt32(2);
                        discounts.Add(new KeyValuePair<string, Tuple<int, decimal>>(discountName, Tuple.Create(discountType, discountValue)));
                    }

                    cmbDiscounts.DataSource = discounts;
                    cmbDiscounts.DisplayMember = "Key";
                    cmbDiscounts.ValueMember = "Value"; // Значение будет хранить Tuple (TypeDiscount, Value)
                }
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
            // Извлекаем числовую часть из ID (например, "86" из "seat86")
            var match = System.Text.RegularExpressions.Regex.Match(id, @"\d+");

            if (match.Success)
            {
                // Извлекаем число и преобразуем в int
                selectedSeat = int.Parse(match.Value);
                MessageBox.Show($"Вы выбрали место с ID: {selectedSeat}");
                lblSelectedSeat.Text = $"Выбранное место: {selectedSeat}";
                LoadSeatPrice(selectedSeat);
                element.Style = "fill: orange; stroke: black; stroke-width: 2;"; // Выделяем цветом
            }
            else
            {
                MessageBox.Show("Не удалось извлечь ID места.");
            }
        }

        private void LoadSeatPrice(int seatId)
        {
            using (var conn = new NpgsqlConnection("Host=localhost;Username=postgres;Password=0813;Database=\"Theatres\""))
            {
                conn.Open();

                // Получаем информацию о категории места
                //string categoryQuery = "SELECT \"Category\" FROM public.\"Seats\" WHERE \"idSeats\" = @seatId";
                //int categoryId = 0;
                //using (var cmd = new NpgsqlCommand(categoryQuery, conn))
                //{
                //    cmd.Parameters.AddWithValue("seatId", seatId);
                //    var result = cmd.ExecuteScalar();
                //    if (result != null)
                //    {
                //        categoryId = Convert.ToInt32(result);
                //    }
                //}
                string query = @"
                    SELECT s.""Category""
                    FROM public.""Seats"" s
                    JOIN public.""PerformanceSchedule"" ps ON s.""Halls"" = ps.""Hall""
                    WHERE ps.""idSchedule"" = @scheduleId
                      AND s.""NumberSeats"" = @seatNumber";

                int categoryId = 0;

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("scheduleId", scheduleId);
                    cmd.Parameters.AddWithValue("seatNumber", seatId);

                    var result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        categoryId = Convert.ToInt32(result);
                    }
                    else
                    {
                        MessageBox.Show("Категория не найдена.");
                    }
                }
                // Получаем информацию о расписании
                //string scheduleQuery = "SELECT \"idSchedule\" FROM public.\"PerformanceSchedule\" WHERE \"idSchedule\" = @scheduleId";
                //using (var cmd = new NpgsqlCommand(scheduleQuery, conn))
                //{
                //    cmd.Parameters.AddWithValue("scheduleId", scheduleId);
                //    using (var reader = cmd.ExecuteReader())
                //    {
                //        if (reader.Read())
                //        {
                //            // Запись найдена
                //            scheduleId = reader.GetInt32(0);
                //        }
                //        else
                //        {
                //            MessageBox.Show("Расписание с таким id не найдено.");
                //        }
                //    }
                //}

                // Получаем цену для места на основе категории и расписания
                string priceQuery = "SELECT \"Price\" FROM public.\"SeatPrices\" WHERE \"ScheduleId\" = @scheduleId AND \"CategoryId\" = @categoryId";
                using (var cmd = new NpgsqlCommand(priceQuery, conn))
                {
                    cmd.Parameters.AddWithValue("scheduleId", scheduleId);
                    cmd.Parameters.AddWithValue("categoryId", categoryId);
                    var price = cmd.ExecuteScalar();
                    if (price != null)
                    {
                        textBoxBasePrice.Text = price.ToString();
                        textBoxFinalPrice.Text = price.ToString(); // Итоговая цена без скидки
                    }
                    else
                    {
                        MessageBox.Show("Цена для выбранного места не найдена.");
                    }
                }
            }
        }

        private void cmbDiscounts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbDiscounts.SelectedItem is KeyValuePair<string, Tuple<int, decimal>> selectedDiscount)
            {
                decimal basePrice = decimal.Parse(textBoxBasePrice.Text); // исходная цена
                decimal discountValue = selectedDiscount.Value.Item2; // значение скидки
                int discountType = selectedDiscount.Value.Item1; // тип скидки (1 - процентная, 2 - фиксированная)

                decimal finalPrice = 0;

                if (discountType == 2) // Процентная скидка
                {
                    finalPrice = basePrice - (basePrice * discountValue / 100);
                }
                else if (discountType == 1) // Фиксированная скидка
                {
                    finalPrice = basePrice - discountValue;
                }

                textBoxFinalPrice.Text = finalPrice.ToString("F2"); // обновление итоговой цены
            }
        }

        private void btnBuy_Click(object sender, EventArgs e)
        {
            if (selectedSeat == -1)
            {
                MessageBox.Show("Выберите место для покупки.");
                return;
            }
            using (var conn = new NpgsqlConnection("Host=localhost;Username=postgres;Password=0813;Database=\"Theatres\""))
            {
                conn.Open();

                // Получаем ID места
                var getSeatIdCmd = new NpgsqlCommand(@"SELECT ""idSeats"" 
                                               FROM public.""Seats"" s
                                               JOIN public.""PerformanceSchedule"" ps ON s.""Halls"" = ps.""Hall""
                                               WHERE ps.""idSchedule"" = @scheduleId
                                               AND s.""NumberSeats"" = @seatNumber", conn);
                getSeatIdCmd.Parameters.AddWithValue("scheduleId", scheduleId);
                getSeatIdCmd.Parameters.AddWithValue("seatNumber", selectedSeat);

                var result = getSeatIdCmd.ExecuteScalar();
                if (result == null)
                {
                    MessageBox.Show("Место не найдено в базе данных.");
                    return;
                }

                int selectedSeatId = Convert.ToInt32(result);
                // Логика покупки билета
                

                decimal finalPrice = decimal.Parse(textBoxFinalPrice.Text);
                var checkCmd = new NpgsqlCommand(@"SELECT COUNT(*) FROM public.""Tickets"" WHERE ""Schedule"" = @scheduleId AND ""Seats"" = @seatId", conn);
                checkCmd.Parameters.AddWithValue("scheduleId", scheduleId);
                checkCmd.Parameters.AddWithValue("seatId", selectedSeatId);

                int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (count > 0)
                {
                    MessageBox.Show("Это место уже куплено другим пользователем.");
                    return;
                }

                var cmd = new NpgsqlCommand("INSERT INTO public.\"Tickets\" (\"Clients\", \"Schedule\", \"Seats\", \"Price\", \"DatePurchase\") VALUES (@clientId, @scheduleId, @seatId, @price, @purchaseDate)", conn);
                cmd.Parameters.AddWithValue("clientId", clientId);
                cmd.Parameters.AddWithValue("scheduleId", scheduleId);
                cmd.Parameters.AddWithValue("seatId", selectedSeatId);
                cmd.Parameters.AddWithValue("price", finalPrice);
                cmd.Parameters.AddWithValue("purchaseDate", DateTime.Now);

                cmd.ExecuteNonQuery();
                MessageBox.Show("Билет успешно куплен!");
                this.Close();
                
            }
        }
        private void btnCancel_Click(object sender, EventArgs e)
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