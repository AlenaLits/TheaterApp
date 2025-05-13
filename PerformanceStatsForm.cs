using ClosedXML.Excel;
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
    public partial class PerformanceStatsForm : Form
    {
        public PerformanceStatsForm()
        {
            InitializeComponent();
            SetupGrid();
            LoadTheaters();
        }
        private void SetupGrid()
        {
            dataGridViewStats.Columns.Clear();
            dataGridViewStats.AutoGenerateColumns = false;
            dataGridViewStats.AllowUserToAddRows = false;
            dataGridViewStats.ReadOnly = true;
            dataGridViewStats.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dataGridViewStats.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "PerformanceName",
                HeaderText = "Спектакль",
                DataPropertyName = "PerformanceName"
            });

            dataGridViewStats.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TicketsSold",
                HeaderText = "Продано билетов",
                DataPropertyName = "TicketsSold"
            });

            dataGridViewStats.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TotalSeats",
                HeaderText = "Всего мест",
                DataPropertyName = "TotalSeats"
            });

            dataGridViewStats.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Popularity",
                HeaderText = "Популярность (%)",
                DataPropertyName = "Popularity"
            });
        }
        private void LoadTheaters()
        {
            using (var conn = new NpgsqlConnection("Host=localhost;Username=postgres;Password=0813;Database=\"Theatres\""))
            {
                conn.Open();
                string query = "SELECT \"idTheaters\", \"NameTheaters\" FROM \"Theaters\" ORDER BY \"NameTheaters\"";

                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    DataTable dt = new DataTable();
                    dt.Load(reader);

                    comboBoxTheaters.DisplayMember = "NameTheaters";
                    comboBoxTheaters.ValueMember = "idTheaters";
                    comboBoxTheaters.DataSource = dt;

                    // Добавим пункт "Все театры"
                    DataRow allRow = dt.NewRow();
                    allRow["idTheaters"] = DBNull.Value;
                    allRow["NameTheaters"] = "Все театры";
                    dt.Rows.InsertAt(allRow, 0);
                    comboBoxTheaters.SelectedIndex = 0;
                }
            }
        }
        private void btnGenerateReport_Click(object sender, EventArgs e)
        {
            DateTime from = dateTimePickerFrom.Value.Date;
            DateTime to = dateTimePickerTo.Value.Date.AddDays(1).AddTicks(-1);
            object selectedTheaterId = comboBoxTheaters.SelectedValue ?? DBNull.Value;

            using (var conn = new NpgsqlConnection("Host=localhost;Username=postgres;Password=0813;Database=\"Theatres\""))
            {
                conn.Open();
                string query = @"
            WITH TotalSeats AS (
                SELECT s.""idSchedule"", COUNT(*) AS Total
                FROM ""Seats"" se
                JOIN ""PerformanceSchedule"" s ON se.""Halls"" = s.""Hall""
                JOIN ""Halls"" h ON s.""Hall"" = h.""idHalls""
                WHERE (@theaterId IS NULL OR h.""Theaters"" = @theaterId)
                GROUP BY s.""idSchedule""
            ),
            SoldTickets AS (
                SELECT s.""idSchedule"", COUNT(t.""idTickets"") AS Sold
                FROM ""Tickets"" t
                JOIN ""PerformanceSchedule"" s ON t.""Schedule"" = s.""idSchedule""
                JOIN ""Halls"" h ON s.""Hall"" = h.""idHalls""
                WHERE t.""Status"" = 1
                  AND s.""DateTime"" BETWEEN @from AND @to
                  AND (@theaterId IS NULL OR h.""Theaters"" = @theaterId)
                GROUP BY s.""idSchedule""
            )
            SELECT 
                p.""NamePerformances"" AS PerformanceName,
                COALESCE(SUM(st.Sold), 0) AS TicketsSold,
                COALESCE(SUM(ts.Total), 0) AS TotalSeats,
                ROUND(
                    CASE 
                        WHEN SUM(ts.Total) > 0 
                        THEN (SUM(st.Sold)::decimal / SUM(ts.Total)) * 100
                        ELSE 0 
                    END, 2
                ) AS Popularity
            FROM ""PerformanceSchedule"" s
            JOIN ""Performances"" p ON s.""Performance"" = p.""idPerformances""
            JOIN ""Halls"" h ON s.""Hall"" = h.""idHalls""
            LEFT JOIN SoldTickets st ON s.""idSchedule"" = st.""idSchedule""
            LEFT JOIN TotalSeats ts ON s.""idSchedule"" = ts.""idSchedule""
            WHERE s.""DateTime"" BETWEEN @from AND @to
              AND (@theaterId IS NULL OR h.""Theaters"" = @theaterId)
            GROUP BY p.""NamePerformances""
            ORDER BY Popularity DESC;
        ";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("from", from);
                    cmd.Parameters.AddWithValue("to", to);
                    cmd.Parameters.AddWithValue("theaterId", selectedTheaterId ?? DBNull.Value);

                    using (var adapter = new NpgsqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dataGridViewStats.DataSource = dt;
                    }
                }
            }
        }

        private void btnExportToExcel_Click(object sender, EventArgs e)
        {
            if (dataGridViewStats.DataSource is DataTable dt && dt.Rows.Count > 0)
            {
                using (SaveFileDialog saveDialog = new SaveFileDialog())
                {
                    saveDialog.Filter = "Excel Files|*.xlsx";
                    saveDialog.Title = "Сохранить как Excel";
                    saveDialog.FileName = "Статистика по спектаклям.xlsx";

                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        using (var workbook = new XLWorkbook())
                        {
                            var worksheet = workbook.Worksheets.Add("Статистика");
                            for (int i = 0; i < dt.Columns.Count; i++)
                            {
                                worksheet.Cell(1, i + 1).Value = dt.Columns[i].ColumnName;
                            }

                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                for (int j = 0; j < dt.Columns.Count; j++)
                                {
                                    worksheet.Cell(i + 2, j + 1).Value = dt.Rows[i][j]?.ToString();
                                }
                            }

                            workbook.SaveAs(saveDialog.FileName);
                            MessageBox.Show("Экспорт завершён!", "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Нет данных для экспорта.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        
        }
    }
}
