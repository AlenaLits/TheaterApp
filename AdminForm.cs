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
            var form = new AddPerformanceForm();
            form.ShowDialog();
        }

        private void buttonAddTheater_Click(object sender, EventArgs e)
        {
            var form = new AddTheatreForm();
            form.ShowDialog();
        }

        private void buttonGenre_Click(object sender, EventArgs e)
        {
            var form = new AddGenreForm();
            form.ShowDialog();
        }

        private void buttonAddDiscount_Click(object sender, EventArgs e)
        {
            var form = new AddDiscountForm();
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
    }
}
