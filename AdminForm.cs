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
    public partial class AdminForm : Form
    {
        public AdminForm()
        {
            InitializeComponent();
        }

        private void buttonAddPerformance_Click(object sender, EventArgs e)
        {
            var form = new PerformanceListForm();
            form.ShowDialog();
        }

        private void buttonAddTheater_Click(object sender, EventArgs e)
        {
            var form = new TheaterListForm();
            form.ShowDialog();
        }

        private void buttonGenre_Click(object sender, EventArgs e)
        {
            var form = new GenreListForm();
            form.ShowDialog();
        }

        private void buttonAddDiscount_Click(object sender, EventArgs e)
        {
            var form = new DiscountListForm();
            form.ShowDialog();
        }

        private void buttonAddSeatCategory_Click(object sender, EventArgs e)
        {
            var form = new AddCategorySeatsForm();
            form.ShowDialog();
        }

        private void buttonRegistr_Click(object sender, EventArgs e)
        {
            var form = new AdminRegistrationForm();
            form.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var form = new ScheduleListForm();
            form.ShowDialog();

        }

        private void buttonLogout_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
