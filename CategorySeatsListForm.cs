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
    public partial class CategorySeatsListForm : Form
    {
        private string connString = "Host=localhost;Username=postgres;Password=0813;Database=\"Theatres\"";

        public CategorySeatsListForm()
        {
            InitializeComponent();
            SetupGrid();
            LoadCategorySeats();
        }

        private void SetupGrid()
        {
            dataGridViewCategorySeats.Columns.Clear();
            dataGridViewCategorySeats.AutoGenerateColumns = false;
            dataGridViewCategorySeats.AllowUserToAddRows = false;
            dataGridViewCategorySeats.ReadOnly = true;
            dataGridViewCategorySeats.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCategorySeats.CellPainting += dataGridViewCategorySeats_CellPainting;

            dataGridViewCategorySeats.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "NameCategorySeats",
                HeaderText = "Категория",
                DataPropertyName = "NameCategorySeats",
                Width = 150
            });

            dataGridViewCategorySeats.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Color",
                HeaderText = "Цвет",
                DataPropertyName = "Color",
                Width = 100
            });

            dataGridViewCategorySeats.Columns.Add(new DataGridViewButtonColumn
            {
                HeaderText = "Редактировать",
                Text = "✏️",
                UseColumnTextForButtonValue = true,
                Width = 60
            });

            dataGridViewCategorySeats.Columns.Add(new DataGridViewButtonColumn
            {
                HeaderText = "Удалить",
                Text = "❌",
                UseColumnTextForButtonValue = true,
                Width = 60
            });

            dataGridViewCategorySeats.CellContentClick += dataGridViewCategorySeats_CellContentClick;
        }
        private void dataGridViewCategorySeats_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 &&
                dataGridViewCategorySeats.Columns[e.ColumnIndex].Name == "Color")
            {
                e.Handled = true;
                e.PaintBackground(e.CellBounds, true);

                try
                {
                    string colorHex = e.FormattedValue?.ToString();
                    using (Brush brush = new SolidBrush(ColorTranslator.FromHtml(colorHex)))
                    {
                        var rect = new Rectangle(e.CellBounds.X + 10, e.CellBounds.Y + 5, e.CellBounds.Width - 20, e.CellBounds.Height - 10);
                        e.Graphics.FillRectangle(brush, rect);
                        e.Graphics.DrawRectangle(Pens.Black, rect);
                    }
                }
                catch
                {
                    // если цвет невалиден — ничего не рисуем
                }
            }
        }
        private void LoadCategorySeats()
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                using (var da = new NpgsqlDataAdapter("SELECT \"idCategorySeats\", \"NameCategorySeats\", \"Color\" FROM public.\"CategorySeats\" ORDER BY \"NameCategorySeats\"", conn))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    dataGridViewCategorySeats.DataSource = dt;
                }
            }
        }

        private void dataGridViewCategorySeats_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dataGridViewCategorySeats.Rows[e.RowIndex];
            int id = Convert.ToInt32(((DataRowView)row.DataBoundItem)["idCategorySeats"]);

            switch (e.ColumnIndex)
            {
                case 2: // Редактировать
                    string name = row.Cells["NameCategorySeats"].Value.ToString();
                    string color = row.Cells["Color"].Value.ToString();
                    using (var editForm = new AddCategorySeatsForm(id, name, color))
                    {
                        if (editForm.ShowDialog() == DialogResult.OK)
                            LoadCategorySeats();
                    }
                    break;

                case 3: // Удалить
                    if (MessageBox.Show("Удалить выбранную категорию?", "Подтверждение", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        DeleteCategory(id);
                        LoadCategorySeats();
                    }
                    break;
            }
        }

        private void DeleteCategory(int id)
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("DELETE FROM public.\"CategorySeats\" WHERE \"idCategorySeats\" = @id", conn))
                {
                    cmd.Parameters.AddWithValue("id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            using (var form = new AddCategorySeatsForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                    LoadCategorySeats();
            }
        }
    }
}
