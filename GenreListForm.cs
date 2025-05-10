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
    public partial class GenreListForm : Form
    {
        private string connString = "Host=localhost;Username=postgres;Password=0813;Database=\"Theatres\"";
        public GenreListForm()
        {
            InitializeComponent();
            SetupGrid();
            LoadGenre();
        }
        private void SetupGrid()
        {
            dataGridViewGenre.Columns.Clear();
            dataGridViewGenre.AutoGenerateColumns = false;
            dataGridViewGenre.AllowUserToAddRows = false;
            dataGridViewGenre.ReadOnly = true;
            dataGridViewGenre.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Название
            dataGridViewGenre.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "NameGenre",
                HeaderText = "Название",
                DataPropertyName = "NameGenre",
                Width = 150
            });
            // Кнопка Редактировать
            dataGridViewGenre.Columns.Add(new DataGridViewButtonColumn
            {
                HeaderText = "Редактировать",
                Text = "✏️",
                UseColumnTextForButtonValue = true,
                Width = 60
            });
            // Кнопка Удалить
            dataGridViewGenre.Columns.Add(new DataGridViewButtonColumn
            {
                HeaderText = "Удалить",
                Text = "❌",
                UseColumnTextForButtonValue = true,
                Width = 60
            });

            dataGridViewGenre.CellContentClick += dataGridViewGenre_CellContentClick;
        }
        private void LoadGenre()
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                using (var da = new NpgsqlDataAdapter("SELECT \"idGenre\", \"NameGenre\" FROM public.\"Genre\" ORDER BY \"NameGenre\"", conn))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    dataGridViewGenre.DataSource = dt;
                }
            }
        }
        private void dataGridViewGenre_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dataGridViewGenre.Rows[e.RowIndex];
            int genreId = Convert.ToInt32(((DataRowView)row.DataBoundItem)["idGenre"]);

            switch (e.ColumnIndex)
            {
                case 1: // Редактировать
                    string name = row.Cells["NameGenre"].Value.ToString();
                    using (var editForm = new AddGenreForm(genreId, name))
                    {
                        if (editForm.ShowDialog() == DialogResult.OK)
                            LoadGenre();
                    }
                    break;

                case 2: // Удалить
                    if (MessageBox.Show("Удалить выбранный жанр?", "Подтверждение", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        DeleteGenre(genreId);
                        LoadGenre();
                    }
                    break;
            }
        }
        private void DeleteGenre(int id)
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("DELETE FROM public.\"Genre\" WHERE \"idGenre\" = @id", conn))
                {
                    cmd.Parameters.AddWithValue("id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            using (var form = new AddGenreForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                    LoadGenre();
            }
        }
    }
}
