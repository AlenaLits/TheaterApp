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
    public partial class DiscountListForm : Form
    {
        private string connString = "Host=localhost;Username=postgres;Password=0813;Database=\"Theatres\"";
        public DiscountListForm()
        {
            InitializeComponent();
            SetupGrid();
            LoadDiscounts();
        }
        private void SetupGrid()
        {
            dataGridViewDiscount.Columns.Clear();
            dataGridViewDiscount.AutoGenerateColumns = false;
            dataGridViewDiscount.AllowUserToAddRows = false;
            dataGridViewDiscount.ReadOnly = true;
            dataGridViewDiscount.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Название
            dataGridViewDiscount.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "NameDiscounts",
                HeaderText = "Название",
                DataPropertyName = "NameDiscounts",
                Width = 150
            });
            // Описание
            dataGridViewDiscount.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Description",
                HeaderText = "Описание",
                DataPropertyName = "Description",
                Width = 200
            });
            // Тип скидки
            dataGridViewDiscount.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TypeDiscount",
                HeaderText = "Тип скидки",
                DataPropertyName = "TypeDiscount",
                Width = 100
            });
            // Период действия
            dataGridViewDiscount.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ValidityPeriod",
                HeaderText = "Период действия",
                DataPropertyName = "ValidityPeriod",
                Width = 100
            });
            // Размер скидки
            dataGridViewDiscount.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Value",
                HeaderText = "Размер скидки",
                DataPropertyName = "Value",
                Width = 100
            });
            // Кнопка Редактировать
            dataGridViewDiscount.Columns.Add(new DataGridViewButtonColumn
            {
                HeaderText = "Редактировать",
                Text = "✏️",
                UseColumnTextForButtonValue = true,
                Width = 60
            });
            // Кнопка Удалить
            dataGridViewDiscount.Columns.Add(new DataGridViewButtonColumn
            {
                HeaderText = "Удалить",
                Text = "❌",
                UseColumnTextForButtonValue = true,
                Width = 60
            });

            dataGridViewDiscount.CellContentClick += dataGridViewDiscount_CellContentClick;
        }
        private void LoadDiscounts()
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                using (var da = new NpgsqlDataAdapter(@"
                    SELECT 
                        d.""idDiscounts"", 
                        d.""NameDiscounts"", 
                        d.""Description"", 
                        t.""NameTypeDiscount"" AS ""TypeDiscount"", 
                        d.""ValidityPeriod"", 
                        d.""Value""
                    FROM public.""Discounts"" d
                    JOIN public.""TypeDiscount"" t ON d.""TypeDiscount"" = t.""idTypeDiscount""
                    ORDER BY d.""NameDiscounts""
                ", conn))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    dataGridViewDiscount.DataSource = dt;
                }
            }
        }
        private void dataGridViewDiscount_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dataGridViewDiscount.Rows[e.RowIndex];
            int discountsId = Convert.ToInt32(((DataRowView)row.DataBoundItem)["idDiscounts"]);

            switch (e.ColumnIndex)
            {
                case 5: // Редактировать
                    string name = row.Cells["NameDiscounts"].Value.ToString();
                    string description = row.Cells["Description"].Value.ToString();
                    string type = row.Cells["TypeDiscount"].Value.ToString();
                    DateTime period = (DateTime)row.Cells["ValidityPeriod"].Value;
                    double value = Convert.ToDouble(row.Cells["Value"].Value);
                    using (var editForm = new AddDiscountForm(discountsId, name, description, type, period, value))
                    {
                        if (editForm.ShowDialog() == DialogResult.OK)
                            LoadDiscounts();
                    }
                    break;

                case 6: // Удалить
                    if (MessageBox.Show("Удалить выбранную скидку?", "Подтверждение", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        DeleteDiscounts(discountsId);
                        LoadDiscounts();
                    }
                    break;
            }

        }
        private void DeleteDiscounts(int id)
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("DELETE FROM public.\"Discounts\" WHERE \"idDiscounts\" = @id", conn))
                {
                    cmd.Parameters.AddWithValue("id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            using (var form = new AddDiscountForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                    LoadDiscounts();
            }
        }
    }
}
