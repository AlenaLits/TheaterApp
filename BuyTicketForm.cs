using Npgsql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace TheaterApp
{
    public partial class BuyTicketForm : Form
    {
        private readonly int clientId;
        private readonly int scheduleId;
        private readonly string performanceName;
        private byte[] svgData;
        private int selectedSeatId = -1; // Для хранения выбранного места

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
                        string htmlContent = $@"
<html>
    <head>
        <meta http-equiv='X-UA-Compatible' content='IE=edge'>
        <style>
            path {{
                fill: blue;
                stroke: black;
                stroke-width: 2;
                cursor: pointer;
            }}
            rect {{
                fill: blue;
                stroke: black;
                stroke-width: 2;
                cursor: pointer;
            }}
        </style>
        <script>
    function elementClick(id) {{
        // Вызов метода из C#
        window.external.ElementClick(id);
    }}
</script>
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

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            var document = webBrowser1.Document;

            string[] clickableTags = { "path", "circle", "rect", "ellipse", "polygon" };

            foreach (string tag in clickableTags)
            {
                foreach (HtmlElement element in webBrowser1.Document.GetElementsByTagName(tag))
                {
                    element.AttachEventHandler("onclick", delegate
                    {
                        Element_Click(element); // Передаем сам HtmlElement в метод
                    });
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
            //string id = element.GetAttribute("id");
            //MessageBox.Show("Вы кликнули по месту с ID: " + id);

            string seatId = element.GetAttribute("id");
            if (!string.IsNullOrEmpty(seatId))
            {
                selectedSeatId = int.Parse(seatId);
                MessageBox.Show($"Вы выбрали место с ID: {selectedSeatId}");
                lblSelectedSeat.Text = $"Выбранное место: {seatId}";
                LoadSeatPrice(selectedSeatId);
            }
        }

        private void LoadSeatPrice(int seatId)
        {
            using (var conn = new NpgsqlConnection("Host=localhost;Username=postgres;Password=0813;Database=\"Theatres\""))
            {
                conn.Open();

                string query = "SELECT \"Price\" FROM public.\"Seats\" WHERE \"idSeat\" = @seatId";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("seatId", seatId);
                    var price = cmd.ExecuteScalar();
                    lblPrice.Text = $"Цена: {price}";
                    lblFinalPrice.Text = price.ToString(); // Итоговая цена без скидки
                }
            }
        }

        private void cmbDiscounts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbDiscounts.SelectedItem is KeyValuePair<string, Tuple<int, decimal>> selectedDiscount)
            {
                decimal basePrice = decimal.Parse(lblFinalPrice.Text); // исходная цена
                decimal discountValue = selectedDiscount.Value.Item2; // значение скидки
                int discountType = selectedDiscount.Value.Item1; // тип скидки (1 - процентная, 2 - фиксированная)

                decimal finalPrice = 0;

                if (discountType == 1) // Процентная скидка
                {
                    finalPrice = basePrice - (basePrice * discountValue / 100);
                }
                else if (discountType == 2) // Фиксированная скидка
                {
                    finalPrice = basePrice - discountValue;
                }

                lblFinalPrice.Text = finalPrice.ToString("F2"); // обновление итоговой цены
            }
        }

        private void btnBuy_Click(object sender, EventArgs e)
        {
            if (selectedSeatId == -1)
            {
                MessageBox.Show("Выберите место для покупки.");
                return;
            }

            // Логика покупки билета
            using (var conn = new NpgsqlConnection("Host=localhost;Username=postgres;Password=0813;Database=\"Theatres\""))
            {
                conn.Open();

                decimal finalPrice = decimal.Parse(lblFinalPrice.Text);
                var cmd = new NpgsqlCommand("INSERT INTO public.\"Tickets\" (\"ClientId\", \"ScheduleId\", \"SeatId\", \"Price\", \"PurchaseDate\") VALUES (@clientId, @scheduleId, @seatId, @price, @purchaseDate)", conn);
                cmd.Parameters.AddWithValue("clientId", clientId);
                cmd.Parameters.AddWithValue("scheduleId", scheduleId);
                cmd.Parameters.AddWithValue("seatId", selectedSeatId);
                cmd.Parameters.AddWithValue("price", finalPrice);
                cmd.Parameters.AddWithValue("purchaseDate", DateTime.Now);

                cmd.ExecuteNonQuery();
                MessageBox.Show("Билет успешно куплен!");
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
