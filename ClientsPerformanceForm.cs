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
    public partial class ClientsPerformanceForm : Form
    {
        private int clientId;
        private string connString = "Host=localhost;Username=postgres;Password=0813;Database=\"Theatres\"";
        public ClientsPerformanceForm(int clientId)
        {
            InitializeComponent();
            this.clientId = clientId;
            SetupGrid();
            LoadPerformances();
        }
        private void SetupGrid()
        {
            dataGridViewPerformances.Columns.Clear();
            dataGridViewPerformances.AutoGenerateColumns = false;
            dataGridViewPerformances.AllowUserToAddRows = false;
            dataGridViewPerformances.ReadOnly = true;
            dataGridViewPerformances.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Скрытый ID
            dataGridViewPerformances.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "idPerformances",
                DataPropertyName = "idPerformances",
                Visible = false
            });
            // Название
            dataGridViewPerformances.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "NamePerformances",
                HeaderText = "Название",
                DataPropertyName = "NamePerformances",
                Width = 150
            });
            // Описание
            dataGridViewPerformances.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Description",
                HeaderText = "Описание",
                DataPropertyName = "Description",
                Width = 200
            });
            // Жанр
            dataGridViewPerformances.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Genre",
                HeaderText = "Жанр",
                DataPropertyName = "Genre",
                Width = 100
            });
            // Возрастное ограничение
            dataGridViewPerformances.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "AgeRestrictions",
                HeaderText = "Возрастное ограничение",
                DataPropertyName = "AgeRestrictions",
                Width = 100
            });
            // Длительность
            dataGridViewPerformances.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Duration",
                HeaderText = "Длительность",
                DataPropertyName = "Duration",
                Width = 100
            });
            dataGridViewPerformances.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "MinPrice",
                HeaderText = "Цена от",
                DataPropertyName = "MinPrice",
                Width = 90,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "C0" } // форматирование в рублях
            });
            // Кнопка Театры
            dataGridViewPerformances.Columns.Add(new DataGridViewButtonColumn
            {
                HeaderText = "Театры",
                Text = "Открыть",
                UseColumnTextForButtonValue = true,
                Width = 60
            });

            dataGridViewPerformances.CellContentClick += dataGridViewTheaters_CellContentClick;
        }
        private void LoadPerformances()
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                using (var da = new NpgsqlDataAdapter(@"
                SELECT 
                    p.""idPerformances"", 
                    p.""NamePerformances"", 
                    p.""Description"", 
                    g.""NameGenre"" AS ""Genre"", 
                    a.""ValueAgeRestrictions"" AS ""AgeRestrictions"", 
                    p.""Duration"",
                    (
                        SELECT MIN(sp.""Price"") 
                        FROM ""PerformanceSchedule"" ps
                        JOIN ""SeatPrices"" sp ON sp.""ScheduleId"" = ps.""idSchedule""
                        JOIN ""Halls"" h ON ps.""Hall"" = h.""idHalls""
                        JOIN ""Seats"" s ON s.""Halls"" = h.""idHalls""
                        WHERE ps.""Performance"" = p.""idPerformances""
                          AND sp.""CategoryId"" = s.""Category""
                    ) AS ""MinPrice""
                FROM public.""Performances"" p
                JOIN public.""Genre"" g ON p.""Genre"" = g.""idGenre""
                JOIN public.""AgeRestrictions"" a ON p.""AgeRestrictions"" = a.""idAgeRestrictions""
                ORDER BY p.""NamePerformances"";
                ", conn))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    dataGridViewPerformances.DataSource = dt;
                }
            }
        }
        private void dataGridViewTheaters_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 7 && e.RowIndex >= 0)
            {
                int performanceId = (int)dataGridViewPerformances.Rows[e.RowIndex].Cells[0].Value;
                string performanceName = dataGridViewPerformances.Rows[e.RowIndex].Cells[1].Value.ToString();
                var form = new PerformancesTheaterForm(performanceId, clientId, performanceName);
                form.ShowDialog();
            }
        }
    }
}
