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
    public partial class EditScheduleForm : Form
    {
        private int? scheduleId; // null — если добавление
        private string connString = "Host=localhost;Username=postgres;Password=0813;Database=\"Theatres\"";
        public EditScheduleForm(int? scheduleId = null)
        {
            InitializeComponent();
            this.scheduleId = scheduleId;

            InitializePriceGrid();
            LoadPerformances();
            LoadHalls();

            if (scheduleId.HasValue)
                LoadScheduleData();
        }
        private void EditScheduleForm_Load(object sender, EventArgs e)
        {
            
        }
        private void LoadPerformances()
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT \"idPerformances\", \"NamePerformances\" FROM public.\"Performances\" ORDER BY \"NamePerformances\"", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    DataTable dt = new DataTable();
                    dt.Load(reader);
                    comboBoxPerformance.DisplayMember = "NamePerformances";
                    comboBoxPerformance.ValueMember = "idPerformances";
                    comboBoxPerformance.DataSource = dt;
                }
            }
        }

        private void LoadHalls()
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(@"
                    SELECT h.""idHalls"", 
                           t.""NameTheaters"" || ' - ' || h.""NameHalls"" AS FullName
                    FROM public.""Halls"" h
                    JOIN public.""Theaters"" t ON h.""Theaters"" = t.""idTheaters""
                    ORDER BY FullName", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    DataTable dt = new DataTable();
                    dt.Load(reader);
                    comboBoxHall.DisplayMember = "FullName";
                    comboBoxHall.ValueMember = "idHalls";
                    comboBoxHall.DataSource = dt;
                }
            }
            comboBoxHall.SelectedIndexChanged += ComboBoxHall_SelectedIndexChanged;
        }
        private void ComboBoxHall_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxHall.SelectedValue is int hallId)
            {
                LoadCategoriesForHall(hallId);
            }
        }
        private void LoadCategoriesForHall(int hallId)
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                string sql = @"
                    SELECT DISTINCT cs.""idCategorySeats"", cs.""NameCategorySeats""
                    FROM public.""Seats"" s
                    JOIN public.""CategorySeats"" cs ON s.""Category"" = cs.""idCategorySeats""
                    WHERE s.""Halls"" = @hallId";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("hallId", hallId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        DataTable dt = new DataTable();
                        dt.Load(reader);

                        dataGridViewPrices.Rows.Clear();
                        foreach (DataRow row in dt.Rows)
                        {
                            dataGridViewPrices.Rows.Add(row["idCategorySeats"], row["NameCategorySeats"], 0);
                        }
                    }
                }
            }
        }
        private void LoadScheduleData()
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(@"SELECT * FROM public.""PerformanceSchedule"" WHERE ""idSchedule"" = @id", conn))
                {
                    cmd.Parameters.AddWithValue("id", scheduleId.Value);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            comboBoxPerformance.SelectedValue = reader.GetInt32(reader.GetOrdinal("Performance"));
                            comboBoxHall.SelectedValue = reader.GetInt32(reader.GetOrdinal("Hall"));
                            dateTimePickerTime.Value = reader.GetDateTime(reader.GetOrdinal("DateTime"));
                        }
                    }
                }

                if (comboBoxHall.SelectedValue is int hallId)
                {
                    LoadCategoriesForHall(hallId);

                    foreach (DataGridViewRow row in dataGridViewPrices.Rows)
                    {
                        int categoryId = Convert.ToInt32(row.Cells["CategoryId"].Value);

                        var priceCmd = new NpgsqlCommand(@"
                            SELECT ""Price"" FROM public.""SeatPrices""
                            WHERE ""ScheduleId"" = @scheduleId AND ""CategoryId"" = @categoryId", conn);
                        priceCmd.Parameters.AddWithValue("scheduleId", scheduleId.Value);
                        priceCmd.Parameters.AddWithValue("categoryId", categoryId);

                        var result = priceCmd.ExecuteScalar();
                        if (result != null)
                        {
                            row.Cells["Price"].Value = Convert.ToDecimal(result);
                        }
                    }
                }
            }
        }
        private void InitializePriceGrid()
        {
            dataGridViewPrices.Columns.Clear();
            dataGridViewPrices.AllowUserToAddRows = false;
            dataGridViewPrices.RowHeadersVisible = false;

            var idColumn = new DataGridViewTextBoxColumn
            {
                Name = "CategoryId",
                Visible = false
            };
            dataGridViewPrices.Columns.Add(idColumn);

            var nameColumn = new DataGridViewTextBoxColumn
            {
                Name = "NameCategorySeats",
                HeaderText = "Категория",
                ReadOnly = true,
                Width = 200
            };
            dataGridViewPrices.Columns.Add(nameColumn);

            var priceColumn = new DataGridViewTextBoxColumn
            {
                Name = "Price",
                HeaderText = "Цена",
                Width = 100
            };
            dataGridViewPrices.Columns.Add(priceColumn);
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            int performanceId = (int)comboBoxPerformance.SelectedValue;
            int hallId = (int)comboBoxHall.SelectedValue;
            DateTime selectedDateTime = dateTimePickerTime.Value;

            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                string sql;
                if (scheduleId.HasValue)
                {
                    sql = @"UPDATE public.""PerformanceSchedule""
                            SET ""Performance"" = @performance, ""Hall"" = @hall, ""DateTime"" = @datetime 
                            WHERE ""idSchedule"" = @id";
                }
                else
                {
                    sql = @"INSERT INTO public.""PerformanceSchedule"" (""Performance"", ""Hall"", ""DateTime"") 
                            VALUES(@performance, @hall, @datetime)";
                }

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("performance", performanceId);
                    cmd.Parameters.AddWithValue("hall", hallId);
                    cmd.Parameters.AddWithValue("datetime", selectedDateTime);
                    if (scheduleId.HasValue)
                        cmd.Parameters.AddWithValue("id", scheduleId.Value);
                    cmd.ExecuteNonQuery();
                }

                int actualScheduleId = scheduleId ?? GetLastInsertedScheduleId(conn);

                foreach (DataGridViewRow row in dataGridViewPrices.Rows)
                {
                    if (row.Cells["CategoryId"].Value == null || row.Cells["Price"].Value == null)
                        continue;

                    int categoryId = Convert.ToInt32(row.Cells["CategoryId"].Value);
                    decimal price = Convert.ToDecimal(row.Cells["Price"].Value);

                    var cmdPrice = new NpgsqlCommand(@"
                        INSERT INTO public.""SeatPrices"" (""ScheduleId"", ""CategoryId"", ""Price"")
                        VALUES (@scheduleId, @categoryId, @price)
                        ON CONFLICT (""ScheduleId"", ""CategoryId"")
                        DO UPDATE SET ""Price"" = EXCLUDED.""Price""
                    ", conn);
                    cmdPrice.Parameters.AddWithValue("scheduleId", actualScheduleId);
                    cmdPrice.Parameters.AddWithValue("categoryId", categoryId);
                    cmdPrice.Parameters.AddWithValue("price", price);
                    cmdPrice.ExecuteNonQuery();
                }
            }

            DialogResult = DialogResult.OK;
        }
        private int GetLastInsertedScheduleId(NpgsqlConnection conn)
        {
            using (var cmd = new NpgsqlCommand(@"SELECT currval(pg_get_serial_sequence('""PerformanceSchedule""','idSchedule'))", conn))
            {
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Вы уверены, что хотите отменить и закрыть форму?",
                "Отменить подтверждение",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                this.Close();
            }
        }
    }
    
}
