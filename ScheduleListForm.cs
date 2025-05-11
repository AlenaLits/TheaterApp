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
    public partial class ScheduleListForm : Form
    {
        private readonly string connString = "Host=localhost;Username=postgres;Password=0813;Database=\"Theatres\"";
        public ScheduleListForm()
        {
            InitializeComponent();
            SetupGrid();
            LoadSchedule();
        }
        private void SetupGrid()
        {
            dataGridViewSchedule.Columns.Clear();
            dataGridViewSchedule.AutoGenerateColumns = false;
            dataGridViewSchedule.AllowUserToAddRows = false;
            dataGridViewSchedule.ReadOnly = true;
            dataGridViewSchedule.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dataGridViewSchedule.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "idSchedule",
                HeaderText = "ID",
                DataPropertyName = "idSchedule"
            });

            dataGridViewSchedule.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "PerformanceName",
                HeaderText = "Спектакль",
                DataPropertyName = "PerformanceName"
            });

            dataGridViewSchedule.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TheaterName",
                HeaderText = "Театр",
                DataPropertyName = "TheaterName"
            });

            dataGridViewSchedule.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "HallName",
                HeaderText = "Зал",
                DataPropertyName = "HallName"
            });

            dataGridViewSchedule.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DateTime",
                HeaderText = "Время",
                DataPropertyName = "DateTime"
            });

            dataGridViewSchedule.Columns.Add(new DataGridViewButtonColumn
            {
                HeaderText = "Редактировать",
                Text = "✏️",
                UseColumnTextForButtonValue = true
            });

            dataGridViewSchedule.Columns.Add(new DataGridViewButtonColumn
            {
                HeaderText = "Удалить",
                Text = "❌",
                UseColumnTextForButtonValue = true
            });

            dataGridViewSchedule.CellContentClick += dataGridViewSchedule_CellContentClick;
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
    FROM public.""PerformanceSchedule"" ps
    JOIN public.""Performances"" p ON ps.""Performance"" = p.""idPerformances""
    JOIN public.""Halls"" h ON ps.""Hall"" = h.""idHalls""
    JOIN public.""Theaters"" t ON h.""Theaters"" = t.""idTheaters""
    ORDER BY ps.""DateTime"";
";
                using (var da = new NpgsqlDataAdapter(query, conn))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    dataGridViewSchedule.DataSource = dt;
                }
            }
        }

        private void dataGridViewSchedule_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            int scheduleId = Convert.ToInt32(dataGridViewSchedule.Rows[e.RowIndex].Cells["idSchedule"].Value);

            if (e.ColumnIndex == 5) // Редактировать
            {
                using (var form = new EditScheduleForm(scheduleId)) // создадим форму ниже
                {
                    if (form.ShowDialog() == DialogResult.OK)
                        LoadSchedule();
                }
            }
            else if (e.ColumnIndex == 6) // Удалить
            {
                if (MessageBox.Show("Удалить расписание?", "Подтверждение", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    DeleteSchedule(scheduleId);
                    LoadSchedule();
                }
            }
        }

        private void DeleteSchedule(int id)
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("DELETE FROM public.\"PerformanceSchedule\" WHERE \"idSchedule\" = @id", conn))
                {
                    cmd.Parameters.AddWithValue("id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            using (var form = new EditScheduleForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                    LoadSchedule();
            }
        }
    }
}
