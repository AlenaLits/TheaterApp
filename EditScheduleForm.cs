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
            LoadPerformances();
            LoadHalls();

            if (scheduleId.HasValue)
                LoadScheduleData();
        }
        private void EditScheduleForm_Load(object sender, EventArgs e)
        {
            LoadPerformances();
            LoadHalls();

            if (scheduleId.HasValue)
                LoadScheduleData();
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
            }
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
            }

            DialogResult = DialogResult.OK;
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
