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
    public partial class HallListForm : Form
    {
        private readonly int theaterId;
        private string connString = "Host=localhost;Username=postgres;Password=0813;Database=\"Theatres\"";
        public HallListForm(int theaterId)
        {
            InitializeComponent();
            this.theaterId = theaterId;
            SetupGrid();
            LoadHalls();
        }
        private void SetupGrid()
        {
            dataGridViewHalls.Columns.Clear();
            dataGridViewHalls.AutoGenerateColumns = false;
            dataGridViewHalls.AllowUserToAddRows = false;
            dataGridViewHalls.ReadOnly = true;

            // Название зала
            dataGridViewHalls.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "NameHalls",
                HeaderText = "Название зала",
                DataPropertyName = "NameHalls",
                Width = 200
            });

            // Вместимость
            dataGridViewHalls.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CountSeats",
                HeaderText = "Вместимость",
                DataPropertyName = "CountSeats",
                Width = 100
            });

            // Редактировать
            dataGridViewHalls.Columns.Add(new DataGridViewButtonColumn
            {
                HeaderText = "Ред.",
                Text = "✏️",
                UseColumnTextForButtonValue = true,
                Width = 60
            });

            // Удалить
            dataGridViewHalls.Columns.Add(new DataGridViewButtonColumn
            {
                HeaderText = "Удалить",
                Text = "❌",
                UseColumnTextForButtonValue = true,
                Width = 60
            });

            dataGridViewHalls.CellContentClick += dataGridViewHalls_CellContentClick;
        }
        private void LoadHalls()
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                var query = "SELECT \"idHalls\", \"NameHalls\", \"CountSeats\" FROM public.\"Halls\" WHERE \"Theaters\" = @id ORDER BY \"NameHalls\"";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("id", theaterId);
                    using (var adapter = new NpgsqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        adapter.Fill(dt);
                        dataGridViewHalls.DataSource = dt;
                    }
                }
            }
        }

        private void dataGridViewHalls_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dataGridViewHalls.Rows[e.RowIndex];
            int hallId = Convert.ToInt32(((DataRowView)row.DataBoundItem)["idHalls"]);

            if (e.ColumnIndex == 2) // редактировать
            {
                string name = row.Cells["NameHalls"].Value.ToString();
                int capacity = Convert.ToInt32(row.Cells["CountSeats"].Value);
                using (var form = new AddHallForm(theaterId, hallId, name, capacity))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                        LoadHalls();
                }
            }
            else if (e.ColumnIndex == 3) // удалить
            {
                if (MessageBox.Show("Удалить зал?", "Подтверждение", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    using (var conn = new NpgsqlConnection(connString))
                    {
                        conn.Open();
                        var cmd = new NpgsqlCommand("DELETE FROM public.\"Halls\" WHERE \"idHalls\" = @id", conn);
                        cmd.Parameters.AddWithValue("id", hallId);
                        cmd.ExecuteNonQuery();
                    }
                    LoadHalls();
                }
            }
        }

        private void buttonAddHall_Click(object sender, EventArgs e)
        {
            using (var form = new AddHallForm(theaterId))
            {
                if (form.ShowDialog() == DialogResult.OK)
                    LoadHalls();
            }
        }
    }
}
