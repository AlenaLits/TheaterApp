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
    public partial class MyTicketsForm : Form
    {
        private int clientId;
        private string connString = "Host=localhost;Username=postgres;Password=0813;Database=\"Theatres\"";

        public MyTicketsForm(int clientId)
        {
            InitializeComponent();
            this.clientId = clientId;
            LoadTickets();
            SetupGrid();
        }
        private void SetupGrid()
        {
            dataGridViewTickets.Columns.Clear();
            dataGridViewTickets.AutoGenerateColumns = false;
            dataGridViewTickets.AllowUserToAddRows = false;
            dataGridViewTickets.ReadOnly = true;
            dataGridViewTickets.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dataGridViewTickets.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "PerformanceName",
                HeaderText = "Название",
                DataPropertyName = "PerformanceName",
                Width = 150
            });

            dataGridViewTickets.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TheaterName",
                HeaderText = "Театр",
                DataPropertyName = "TheaterName",
                Width = 150
            });

            dataGridViewTickets.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "HallName",
                HeaderText = "Зал",
                DataPropertyName = "HallName",
                Width = 100
            });

            dataGridViewTickets.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DateTime",
                HeaderText = "Дата и время",
                DataPropertyName = "DateTime",
                Width = 160
            });

            dataGridViewTickets.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SeatNumber",
                HeaderText = "Место",
                DataPropertyName = "SeatNumber",
                Width = 60
            });

            dataGridViewTickets.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TicketPrice",
                HeaderText = "Цена",
                DataPropertyName = "TicketPrice",
                Width = 80
            });
        }
        private void LoadTickets()
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                string query = @"
                    SELECT 
                        p.""NamePerformances"" AS ""PerformanceName"",
                        t.""NameTheaters"" AS ""TheaterName"",
                        h.""NameHalls"" AS ""HallName"",
                        s.""DateTime"" AS ""DateTime"",
                        se.""NumberSeats"" AS ""SeatNumber"",
                        b.""Price"" AS ""TicketPrice""
                    FROM ""Tickets"" b
                    JOIN ""PerformanceSchedule"" s ON b.""Schedule"" = s.""idSchedule""
                    JOIN ""Performances"" p ON s.""Performance"" = p.""idPerformances""
                    JOIN ""Halls"" h ON s.""Hall"" = h.""idHalls""
                    JOIN ""Theaters"" t ON h.""Theaters"" = t.""idTheaters""
                    JOIN ""Seats"" se ON b.""Seats"" = se.""idSeats""
                    WHERE b.""Clients"" = @clientId
                    ORDER BY s.""DateTime"" DESC;
                ";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("clientId", clientId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        DataTable dt = new DataTable();
                        dt.Load(reader);
                        dataGridViewTickets.DataSource = dt;
                    }
                }
            }
        }
    }
}
