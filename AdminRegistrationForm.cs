using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TheaterApp
{
    public partial class AdminRegistrationForm : Form
    {
        public AdminRegistrationForm()
        {
            InitializeComponent();
        }

        private void buttonRegister_Click(object sender, EventArgs e)
        {
            string username = textBoxUsername.Text.Trim();
            string password = textBoxPassword.Text;
            string confirmPassword = textBoxConfirmPassword.Text;

            if (password != confirmPassword)
            {
                MessageBox.Show("Пароли не совпадают!");
                return;
            }

            var salt = GenerateSalt();
            var passwordHash = HashPassword(password, salt);

            using (var conn = new NpgsqlConnection("Host=localhost;Username=postgres;Password=0813;Database=\"Theatres\""))
            {
                conn.Open();

                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Проверка уникальности username
                        var checkCmd = new NpgsqlCommand("SELECT COUNT(*) FROM \"Administrators\" WHERE \"Username\" = @username", conn);
                        checkCmd.Parameters.AddWithValue("username", username);
                        var exists = (long)checkCmd.ExecuteScalar();
                        if (exists > 0)
                        {
                            MessageBox.Show("Пользователь с таким именем уже существует!");
                            return;
                        }

                        var cmdClient = new NpgsqlCommand(@"
                    INSERT INTO ""Administrators"" 
                        (""Username"", ""DateRegistration"", ""PasswordHash"", ""Salt"")
                    VALUES 
                        (@username, @date, @hash, @salt)", conn);
                        cmdClient.Parameters.AddWithValue("date", DateTime.Now.Date);
                        cmdClient.Parameters.AddWithValue("hash", passwordHash);
                        cmdClient.Parameters.AddWithValue("salt", salt);
                        cmdClient.Parameters.AddWithValue("username", username);
                        cmdClient.ExecuteNonQuery();

                        transaction.Commit();
                        MessageBox.Show("Регистрация прошла успешно!");
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Ошибка регистрации: " + ex.Message);
                    }
                }
            }
        }
        private string GenerateSalt()
        {
            var rng = new RNGCryptoServiceProvider();
            byte[] saltBytes = new byte[16];
            rng.GetBytes(saltBytes);
            return Convert.ToBase64String(saltBytes);
        }

        private string HashPassword(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var combined = Encoding.UTF8.GetBytes(password + salt);
                return Convert.ToBase64String(sha256.ComputeHash(combined));
            }
        }
    }
}
