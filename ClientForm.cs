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
    public partial class ClientForm : Form
    {
        private int clientId;
        public ClientForm(int clientId)
        {
            InitializeComponent();
            this.clientId = clientId;
        }

        private void ClientForm_Load(object sender, EventArgs e)
        {
            // Можно стилизовать или добавить приветствие
        }

        private void btnTheaters_Click(object sender, EventArgs e)
        {
            var form = new ClientsTheaterForm(clientId);
            form.ShowDialog();
        }

        private void btnPerformances_Click(object sender, EventArgs e)
        {
            var form = new ClientsPerformanceForm(clientId);
            form.ShowDialog();
        }

        private void btnMyTickets_Click(object sender, EventArgs e)
        {
            var form = new MyTicketsForm(clientId);
            form.ShowDialog();
        }

        private void buttonLogout_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
