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
    public partial class PerformanceListForm : Form
    {
        private string connString = "Host=localhost;Username=postgres;Password=0813;Database=\"Theatres\"";
        public PerformanceListForm()
        {
            InitializeComponent();
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
            // Кнопка Редактировать
            dataGridViewPerformances.Columns.Add(new DataGridViewButtonColumn
            {
                HeaderText = "Редактировать",
                Text = "✏️",
                UseColumnTextForButtonValue = true,
                Width = 60
            });
            // Кнопка Удалить
            dataGridViewPerformances.Columns.Add(new DataGridViewButtonColumn
            {
                HeaderText = "Удалить",
                Text = "❌",
                UseColumnTextForButtonValue = true,
                Width = 60
            });

            dataGridViewPerformances.CellContentClick += dataGridViewPerformances_CellContentClick;
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
                        p.""Duration""
                    FROM public.""Performances"" p
                    JOIN public.""Genre"" g ON p.""Genre"" = g.""idGenre""
                    JOIN public.""AgeRestrictions"" a ON p.""AgeRestrictions"" = a.""idAgeRestrictions""
                    ORDER BY p.""NamePerformances""
                ", conn))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    dataGridViewPerformances.DataSource = dt;
                }
            }
        }
        private void dataGridViewPerformances_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dataGridViewPerformances.Rows[e.RowIndex];
            int performanceId = Convert.ToInt32(((DataRowView)row.DataBoundItem)["idPerformances"]);

            switch (e.ColumnIndex)
            {
                case 5: // Редактировать
                    string name = row.Cells["NamePerformances"].Value.ToString();
                    string description = row.Cells["Description"].Value.ToString();
                    string genre = row.Cells["Genre"].Value.ToString();
                    string ageRestrictions = row.Cells["AgeRestrictions"].Value.ToString();
                    int duration = (int)row.Cells["Duration"].Value;
                    using (var editForm = new AddPerformanceForm(performanceId, name, description, genre, ageRestrictions, duration))
                    {
                        if (editForm.ShowDialog() == DialogResult.OK)
                            LoadPerformances();
                    }
                    break;

                case 6: // Удалить
                    if (MessageBox.Show("Удалить выбранный спектакль?", "Подтверждение", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        DeletePerformance(performanceId);
                        LoadPerformances();
                    }
                    break;
            }

        }
        private void DeletePerformance(int id)
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("DELETE FROM public.\"Performances\" WHERE \"idPerformances\" = @id", conn))
                {
                    cmd.Parameters.AddWithValue("id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            using (var form = new AddPerformanceForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                    LoadPerformances();
            }
        }
    }
}
