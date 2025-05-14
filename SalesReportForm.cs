using ClosedXML.Excel;
using Npgsql;
using System;
using System.Data;
using System.Windows.Forms;

namespace TheaterApp
{
    public partial class SalesReportForm : Form
    {
        private string connString = "Host=localhost;Username=postgres;Password=0813;Database=\"Theatres\"";

        public SalesReportForm()
        {
            InitializeComponent();
            LoadTheaters();
            SetupGrid();
        }

        private void LoadTheaters()
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                string query = @"SELECT ""idTheaters"", ""NameTheaters"" FROM ""Theaters"" ORDER BY ""NameTheaters""";
                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    var dt = new DataTable();
                    dt.Load(reader);

                    // Добавим "Все театры"
                    DataRow allRow = dt.NewRow();
                    allRow["idTheaters"] = DBNull.Value;
                    allRow["NameTheaters"] = "Все театры";
                    dt.Rows.InsertAt(allRow, 0);

                    comboBoxTheaters.DataSource = dt;
                    comboBoxTheaters.DisplayMember = "NameTheaters";
                    comboBoxTheaters.ValueMember = "idTheaters";
                }
            }
        }

        private void SetupGrid()
        {
            dataGridViewSales.Columns.Clear();
            dataGridViewSales.AutoGenerateColumns = false;
            dataGridViewSales.AllowUserToAddRows = false;
            dataGridViewSales.ReadOnly = true;
            dataGridViewSales.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dataGridViewSales.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TheaterName",
                HeaderText = "Театр",
                DataPropertyName = "NameTheaters"
            });

            dataGridViewSales.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "PerformanceName",
                HeaderText = "Спектакль",
                DataPropertyName = "NamePerformances"
            });

            dataGridViewSales.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TicketsSold",
                HeaderText = "Продано билетов",
                DataPropertyName = "TicketsSold"
            });

            dataGridViewSales.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Revenue",
                HeaderText = "Выручка (₽)",
                DataPropertyName = "Revenue",
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" }
            });
        }

        private void btnGenerateReport_Click(object sender, EventArgs e)
        {
            int? theaterId = comboBoxTheaters.SelectedValue is DBNull ? null : (int?)comboBoxTheaters.SelectedValue;
            DateTime fromDate = dateTimePickerFrom.Value.Date;
            DateTime toDate = dateTimePickerTo.Value.Date.AddDays(1).AddTicks(-1); // до конца дня

            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                string query = @"
                SELECT
                    t.""NameTheaters"",
                    p.""NamePerformances"",
                    COUNT(b.""idTickets"") AS TicketsSold,
                    COALESCE(SUM(b.""PriceAfterDiscount""), 0) AS Revenue
                FROM ""Tickets"" b
                JOIN ""PerformanceSchedule"" s ON b.""Schedule"" = s.""idSchedule""
                JOIN ""Performances"" p ON s.""Performance"" = p.""idPerformances""
                JOIN ""Halls"" h ON s.""Hall"" = h.""idHalls""
                JOIN ""Theaters"" t ON h.""Theaters"" = t.""idTheaters""
                  AND (@theaterId IS NULL OR t.""idTheaters"" = @theaterId)
                  AND s.""DateTime"" BETWEEN @from AND @to
                GROUP BY t.""NameTheaters"", p.""NamePerformances""
                ORDER BY Revenue DESC;
                ";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.Add("theaterId", NpgsqlTypes.NpgsqlDbType.Integer).Value =
    theaterId.HasValue ? (object)theaterId.Value : DBNull.Value;
                    cmd.Parameters.AddWithValue("from", fromDate);
                    cmd.Parameters.AddWithValue("to", toDate);

                    using (var reader = cmd.ExecuteReader())
                    {
                        var dt = new DataTable();
                        dt.Load(reader);
                        dataGridViewSales.DataSource = dt;

                        int totalTickets = 0;
                        decimal totalRevenue = 0;

                        foreach (DataRow row in dt.Rows)
                        {
                            totalTickets += Convert.ToInt32(row["TicketsSold"]);
                            totalRevenue += Convert.ToDecimal(row["Revenue"]);
                        }

                        labelTotalTickets.Text = $"Всего продано билетов: {totalTickets}";
                        labelTotalRevenue.Text = $"Общая выручка: {totalRevenue:N2} ₽";
                    }
                }
            }
        }

        private void ExportToExcel_Click(object sender, EventArgs e)
        {
            ExportToExcel();
        }
        private void ExportToExcel()
        {
            if (dataGridViewSales.Rows.Count == 0)
            {
                MessageBox.Show("Нет данных для экспорта.", "Выгрузка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog()
            {
                Filter = "Excel файл|*.xlsx",
                Title = "Сохранить как Excel файл",
                FileName = "Отёт по продажам.xlsx"
            })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("Отчёт по продажам");

                        // Заголовки
                        for (int i = 0; i < dataGridViewSales.Columns.Count; i++)
                        {
                            worksheet.Cell(1, i + 1).Value = dataGridViewSales.Columns[i].HeaderText;
                            worksheet.Cell(1, i + 1).Style.Font.Bold = true;
                        }

                        // Данные
                        for (int i = 0; i < dataGridViewSales.Rows.Count; i++)
                        {
                            for (int j = 0; j < dataGridViewSales.Columns.Count; j++)
                            {
                                worksheet.Cell(i + 2, j + 1).Value = dataGridViewSales.Rows[i].Cells[j].Value?.ToString();
                            }
                        }

                        worksheet.Columns().AdjustToContents();

                        // Сохраняем
                        workbook.SaveAs(sfd.FileName);
                        MessageBox.Show("Файл успешно сохранён!", "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }
    }
}
