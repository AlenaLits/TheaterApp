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
    public partial class PerformancesTheaterForm : Form
    {
        private int performanceId;
        private int clientId;
        private string performanceName;
        private string connString = "Host=localhost;Username=postgres;Password=0813;Database=\"Theatres\"";
        public PerformancesTheaterForm(int performanceId, int clientId, string performanceName)
        {
            InitializeComponent();
            this.performanceId = performanceId;
            this.clientId = clientId;
            this.performanceName = performanceName;
            this.Text = $"Театры, которые показывают \"{performanceName}\"";

            SetupGrid();
            LoadSchedule();
        }
        private void SetupGrid()
        {
            dataGridViewTheaters.Columns.Clear();
            dataGridViewTheaters.AutoGenerateColumns = false;
            dataGridViewTheaters.AllowUserToAddRows = false;
            dataGridViewTheaters.ReadOnly = true;
            dataGridViewTheaters.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dataGridViewTheaters.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "idSchedule",
                DataPropertyName = "idSchedule",
                Visible = false
            });

            dataGridViewTheaters.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TheaterName",
                HeaderText = "Театр",
                DataPropertyName = "TheaterName",
                Width = 200
            });

            dataGridViewTheaters.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "HallName",
                HeaderText = "Зал",
                DataPropertyName = "HallName",
                Width = 150
            });

            dataGridViewTheaters.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DateTime",
                HeaderText = "Дата и время",
                DataPropertyName = "DateTime",
                Width = 150
            });
            dataGridViewTheaters.Columns.Add(new DataGridViewButtonColumn
            {
                HeaderText = "Подробнее",
                Text = "Описание",
                UseColumnTextForButtonValue = true,
                Width = 120
            });
            dataGridViewTheaters.Columns.Add(new DataGridViewButtonColumn
            {
                HeaderText = "Действие",
                Text = "Купить билет",
                UseColumnTextForButtonValue = true,
                Width = 120
            });

            dataGridViewTheaters.CellContentClick += dataGridViewPerformances_CellContentClick;
        }
        private void LoadSchedule()
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                string query = @"
                SELECT 
                    ps.""idSchedule"", 
                    p.""NamePerformances"" AS ""PerformanceName"", 
                    t.""NameTheaters"" AS ""TheaterName"",
                    h.""NameHalls"" AS ""HallName"",
                    ps.""DateTime""
                FROM ""PerformanceSchedule"" ps
                JOIN ""Performances"" p ON ps.""Performance"" = p.""idPerformances""
                JOIN ""Halls"" h ON ps.""Hall"" = h.""idHalls""
                JOIN ""Theaters"" t ON h.""Theaters"" = t.""idTheaters""
                WHERE p.""idPerformances"" = @performanceId
                ORDER BY ps.""DateTime"";
                ";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("performanceId", performanceId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        var dt = new DataTable();
                        dt.Load(reader);
                        dataGridViewTheaters.DataSource = dt;
                    }
                }
            }
        }
        private void dataGridViewPerformances_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 5 && e.RowIndex >= 0)
            {
                int scheduleId = (int)dataGridViewTheaters.Rows[e.RowIndex].Cells["idSchedule"].Value;
                string performance = performanceName;

                // Открытие формы покупки билета
                var form = new BuyTicketForm(clientId, scheduleId, performance);
                form.ShowDialog();
            }
            if (e.ColumnIndex == 4 && e.RowIndex >= 0)
            {
                // Предположим, в DataGridView есть колонка с ID спектакля
                int scheduleId = Convert.ToInt32(dataGridViewTheaters.Rows[e.RowIndex].Cells["idSchedule"].Value);

                var detailsForm = new PerformanceDetailsForm(scheduleId);
                detailsForm.ShowDialog();
            }

        }
    }
}
