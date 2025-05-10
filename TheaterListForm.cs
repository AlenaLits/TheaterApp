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
    public partial class TheaterListForm : Form
    {
        private string connString = "Host=localhost;Username=postgres;Password=0813;Database=\"Theatres\"";

        public TheaterListForm()
        {
            InitializeComponent();
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
            // Кнопка Залы
            dataGridViewTheaters.Columns.Add(new DataGridViewButtonColumn
            {
                HeaderText = "Залы",
                Text = "Залы",
                UseColumnTextForButtonValue = true,
                Width = 60
            });
            // Кнопка Редактировать
            dataGridViewTheaters.Columns.Add(new DataGridViewButtonColumn
            {
                HeaderText = "Редактировать",
                Text = "✏️",
                UseColumnTextForButtonValue = true,
                Width = 60
            });
            // Кнопка Удалить
            dataGridViewTheaters.Columns.Add(new DataGridViewButtonColumn
            {
                HeaderText = "Удалить",
                Text = "❌",
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
            if (e.RowIndex < 0) return;

            var row = dataGridViewTheaters.Rows[e.RowIndex];
            int theaterId = Convert.ToInt32(((DataRowView)row.DataBoundItem)["idTheaters"]);

            switch (e.ColumnIndex)
            {
                case 3: // Залы
                    var hallsForm = new HallListForm(theaterId);
                    hallsForm.ShowDialog();
                    break;

                case 4: // Редактировать
                    string name = row.Cells["NameTheaters"].Value.ToString();
                    string address = row.Cells["Address"].Value.ToString();
                    string phone = row.Cells["ContactPhone"].Value.ToString();
                    using (var editForm = new AddTheatreForm(theaterId, name, address, phone))
                    {
                        if (editForm.ShowDialog() == DialogResult.OK)
                            LoadTheaters();
                    }
                    break;

                case 5: // Удалить
                    if (MessageBox.Show("Удалить выбранный театр?", "Подтверждение", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        DeleteTheater(theaterId);
                        LoadTheaters();
                    }
                    break;
            }
        }
        private void DeleteTheater(int id)
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("DELETE FROM public.\"Theaters\" WHERE \"idTheaters\" = @id", conn))
                {
                    cmd.Parameters.AddWithValue("id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            using (var form = new AddTheatreForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                    LoadTheaters();
            }
        }
    }
}
