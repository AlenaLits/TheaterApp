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
    public partial class TheaterPerformancesForm : Form
    {
        private int theaterId;
        private int clientId;
        private string theaterName;
        private string connString = "Host=localhost;Username=postgres;Password=0813;Database=\"Theatres\"";
        public TheaterPerformancesForm(int theaterId, int clientId, string theaterName)
        {
            InitializeComponent();
            this.theaterId = theaterId;
            this.clientId = clientId;
            this.theaterName = theaterName;
            this.Text = $"Спектакли в театре \"{theaterName}\"";

            SetupGrid();
            LoadSchedule();
        }
        private void SetupGrid()
        {
            dataGridViewPerformances.Columns.Clear();
            dataGridViewPerformances.AutoGenerateColumns = false;
            dataGridViewPerformances.AllowUserToAddRows = false;
            dataGridViewPerformances.ReadOnly = true;
            dataGridViewPerformances.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dataGridViewPerformances.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "idSchedule",
                DataPropertyName = "idSchedule",
                Visible = false
            });

            dataGridViewPerformances.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "PerformanceName",
                HeaderText = "Название спектакля",
                DataPropertyName = "PerformanceName",
                Width = 200
            });

            dataGridViewPerformances.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DateTime",
                HeaderText = "Дата и время",
                DataPropertyName = "DateTime",
                Width = 150
            });
            dataGridViewPerformances.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "MinPrice",
                HeaderText = "Цена от",
                DataPropertyName = "MinPrice",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "C0" } // формат рубли
            });
            dataGridViewPerformances.Columns.Add(new DataGridViewButtonColumn
            {
                HeaderText = "Подробнее",
                Text = "Описание",
                UseColumnTextForButtonValue = true,
                Width = 120
            });
            dataGridViewPerformances.Columns.Add(new DataGridViewButtonColumn
            {
                HeaderText = "Действие",
                Text = "Купить билет",
                UseColumnTextForButtonValue = true,
                Width = 120
            });

            dataGridViewPerformances.CellContentClick += dataGridViewPerformances_CellContentClick;
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
                    ps.""DateTime"",
                    MIN(sp.""Price"") AS ""MinPrice""
                FROM public.""PerformanceSchedule"" ps
                JOIN public.""Performances"" p ON ps.""Performance"" = p.""idPerformances""
                JOIN public.""Halls"" h ON ps.""Hall"" = h.""idHalls""
                JOIN public.""Seats"" s ON s.""Halls"" = h.""idHalls""
                JOIN public.""SeatPrices"" sp ON sp.""CategoryId"" = s.""Category"" AND sp.""ScheduleId"" = ps.""idSchedule""
                WHERE h.""Theaters"" = @theaterId
                GROUP BY ps.""idSchedule"", p.""NamePerformances"", ps.""DateTime""
                ORDER BY ps.""DateTime"";
                ";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("theaterId", theaterId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        var dt = new DataTable();
                        dt.Load(reader);
                        dataGridViewPerformances.DataSource = dt;
                    }
                }
            }
        }

        private void dataGridViewPerformances_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 5 && e.RowIndex >= 0)
            {
                int scheduleId = (int)dataGridViewPerformances.Rows[e.RowIndex].Cells["idSchedule"].Value;
                string performanceName = dataGridViewPerformances.Rows[e.RowIndex].Cells["PerformanceName"].Value.ToString();

                // Открытие формы покупки билета

                var form = new BuyTicketForm(clientId, scheduleId, performanceName);
                form.ShowDialog();
            }
            if (e.ColumnIndex == 4 && e.RowIndex >= 0)
            {
                // Предположим, в DataGridView есть колонка с ID спектакля
                int scheduleId = Convert.ToInt32(dataGridViewPerformances.Rows[e.RowIndex].Cells["idSchedule"].Value);

                var detailsForm = new PerformanceDetailsForm(scheduleId);
                detailsForm.ShowDialog();
            }

        }
    }
}
