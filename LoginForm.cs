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
    public partial class LoginForm : Form
    {
        public string Role { get; private set; }
        public LoginForm()
        {
            InitializeComponent();
        }
        private static string ComputeHash(string password, string salt)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var combined = Encoding.UTF8.GetBytes(password + salt);
                var hash = sha256.ComputeHash(combined);
                return Convert.ToBase64String(hash);
            }
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            string username = textBoxUsername.Text;
            string password = textBoxPassword.Text;

            using (var conn = new NpgsqlConnection("Host=localhost;Username=postgres;Password=0813;Database=\"Theatres\""))
            {
                conn.Open();

                // 1. Проверка среди администраторов
                using (var adminCmd = new NpgsqlCommand("SELECT \"PasswordHash\", \"Salt\" FROM \"Administrators\" WHERE \"Username\" = @username", conn))
                {
                    adminCmd.Parameters.AddWithValue("username", username);
                    using (var reader = adminCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string hash = reader.GetString(0);
                            string salt = reader.GetString(1);
                            string computedHash = ComputeHash(password, salt);
                            if (computedHash == hash)
                            {
                                this.Role = "admin";
                                this.DialogResult = DialogResult.OK;
                                return;
                            }
                        }
                    }
                }

                // 2. Проверка среди клиентов (отдельно, после закрытия reader)
                using (var clientCmd = new NpgsqlCommand("SELECT \"PasswordHash\", \"Salt\" FROM \"Clients\" WHERE \"Username\" = @username", conn))
                {
                    clientCmd.Parameters.AddWithValue("username", username);
                    using (var reader = clientCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string hash = reader.GetString(0);
                            string salt = reader.GetString(1);
                            string computedHash = ComputeHash(password, salt);
                            if (computedHash == hash)
                            {
                                this.Role = "client";
                                this.DialogResult = DialogResult.OK;
                                return;
                            }
                        }
                    }
                }

                // Если не найдено ни в одной таблице
                MessageBox.Show("Неверное имя пользователя или пароль");
            }
        }

        private void buttonRegistr_Click(object sender, EventArgs e)
        {
            var regForm = new ClientRegistrationForm();
            regForm.ShowDialog();
        }
    }
}
