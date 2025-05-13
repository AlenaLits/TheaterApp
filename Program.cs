using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TheaterApp
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            while (true)
            {
                using (var loginForm = new LoginForm())
                {
                    var result = loginForm.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        if (loginForm.Role == "admin")
                        {
                            var adminForm = new AdminForm();
                            adminForm.ShowDialog(); // ждём закрытия
                        }
                        else if (loginForm.Role == "client")
                        {
                            var clientForm = new ClientForm(loginForm.UserId);
                            clientForm.ShowDialog();
                        }
                    }
                    else
                    {
                        break; // пользователь закрыл окно входа — завершаем приложение
                    }
                }
            }
        }
    }
}
