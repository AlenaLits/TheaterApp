using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TheaterApp
{
    public partial class PerformanceDetailsForm : Form
    {
        private int scheduleId;
        public PerformanceDetailsForm(int scheduleId)
        {
            InitializeComponent();
            this.scheduleId = scheduleId;
            LoadPerformanceDetails();
        }
        private void LoadPerformanceDetails()
        {
            using (var conn = new NpgsqlConnection("Host=localhost;Username=postgres;Password=0813;Database=\"Theatres\""))
            {
                conn.Open();

                string query = @"
                    SELECT 
                        p.""NamePerformances"" AS PerformanceName,
                        p.""Description"" AS Description,
                        g.""NameGenre"" AS GenreName,
                        a.""ValueAgeRestrictions"" AS AgeRestrictionName,
                        p.""Duration"" AS Duration,
                        t.""NameTheaters"" AS TheaterName,
                        h.""NameHalls"" AS HallName,
                        s.""DateTime"" AS DateTime,
                        t.""Address"" AS TheaterAddress,
                        t.""ContactPhone"" AS TheaterPhone
                    FROM ""PerformanceSchedule"" s
                    JOIN ""Performances"" p ON s.""Performance"" = p.""idPerformances""
                    JOIN ""Halls"" h ON s.""Hall"" = h.""idHalls""
                    JOIN ""Theaters"" t ON h.""Theaters"" = t.""idTheaters""
                    JOIN ""Genre"" g ON p.""Genre"" = g.""idGenre""
                    JOIN ""AgeRestrictions"" a ON p.""AgeRestrictions"" = a.""idAgeRestrictions""
                    WHERE s.""idSchedule"" = @scheduleId;
                ";
    
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("scheduleId", scheduleId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            textBoxNamePerformance.Text = reader["PerformanceName"].ToString();
                            textBoxDescription.Text = reader["Description"].ToString();
                            textBoxGenre.Text = reader["GenreName"].ToString();
                            textBoxAge.Text = reader["AgeRestrictionName"].ToString();
                            textBoxDuration.Text = reader["Duration"].ToString();
                            textBoxNameTheater.Text = reader["TheaterName"].ToString();
                            textBoxHall.Text = reader["HallName"].ToString();
                            textBoxDate.Text = Convert.ToDateTime(reader["DateTime"]).ToString("f");
                            textBoxAddressTheater.Text = reader["TheaterAddress"].ToString();
                            textBoxPhoneTheater.Text = reader["TheaterPhone"].ToString();
                        }
                    }
                }
            }
        }
    }
}
