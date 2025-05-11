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
    public partial class ClientsTheaterForm : Form
    {
        private int clientId;
        private string connString = "Host=localhost;Username=postgres;Password=0813;Database=\"Theatres\"";
        public ClientsTheaterForm(int clientId)
        {
            InitializeComponent();
            this.clientId = clientId;
            SetupGrid();
            LoadTheaters();
        }
        private void SetupGrid()
        {
            dataGridViewTheaters.Columns.Clear();
            dataGridViewTheaters.AutoGenerateColumns = false;
            dataGridViewTheaters.AllowUserToAddRows = false;
            dataGridViewTheaters.ReadOnly = true;
            dataGridViewTheaters.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Скрытый ID
            dataGridViewTheaters.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "idTheaters",
                DataPropertyName = "idTheaters",
                Visible = false
            });
            // Название
            dataGridViewTheaters.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "NameTheaters",
                HeaderText = "Название",
                DataPropertyName = "NameTheaters",
                Width = 150
            });
            // Адрес
            dataGridViewTheaters.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Address",
                HeaderText = "Адрес",
                DataPropertyName = "Address",
                Width = 200
            });
            // Телефон
            dataGridViewTheaters.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ContactPhone",
                HeaderText = "Телефон",
                DataPropertyName = "ContactPhone",
                Width = 100
            });
            // Кнопка Спектакли
            dataGridViewTheaters.Columns.Add(new DataGridViewButtonColumn
            {
                HeaderText = "Спектакли",
                Text = "Открыть",
                UseColumnTextForButtonValue = true,
                Width = 60
            });

            dataGridViewTheaters.CellContentClick += dataGridViewTheaters_CellContentClick;
        }
        private void LoadTheaters()
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                using (var da = new NpgsqlDataAdapter("SELECT \"idTheaters\", \"NameTheaters\", \"Address\", \"ContactPhone\" FROM public.\"Theaters\" ORDER BY \"NameTheaters\"", conn))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    dataGridViewTheaters.DataSource = dt;
                }
            }
        }
        private void dataGridViewTheaters_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 4 && e.RowIndex >= 0)
            {
                int theaterId = (int)dataGridViewTheaters.Rows[e.RowIndex].Cells[0].Value;
                string theaterName = dataGridViewTheaters.Rows[e.RowIndex].Cells[1].Value.ToString();
                var form = new TheaterPerformancesForm(theaterId, clientId, theaterName);
                form.ShowDialog();
            }
        }
    }
}
