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
using System.IO;

namespace TheaterApp
{
    public partial class AddHallForm : Form
    {
        private readonly int theatreId;
        private byte[] svgData;
        public AddHallForm(int theatreId)
        {
            InitializeComponent();
            this.theatreId = theatreId;
            this.webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(this.webBrowser1_DocumentCompleted);

        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "SVG Files (*.svg)|*.svg";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    svgData = File.ReadAllBytes(ofd.FileName);
                    MessageBox.Show("Схема загружена!");

                    string svgContent = File.ReadAllText(ofd.FileName, Encoding.UTF8);
                    string htmlContent = $@"
    <html>
        <head>
            <meta http-equiv='X-UA-Compatible' content='IE=edge'>
            <style>
                path {{
                    fill: blue;  /* Цвет для всех мест */
                    stroke: black;
                    stroke-width: 2;
                }}
                rect {{
                    fill: blue;  /* Цвет для прямоугольников */
                    stroke: black;
                    stroke-width: 2;
                }}
                /* Дополнительные стили для других элементов SVG */
            </style>
            <script>
                function elementClick(id) {{
                    alert('Вы кликнули по месту с ID: ' + id);
                }}
            </script>
        </head>
        <body>
            {svgContent}
        </body>
    </html>";

                    webBrowser1.DocumentText = htmlContent;
                }
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            var hallName = textBoxHallName.Text.Trim();
            var seatsCount = (int)numericUpDownSeats.Value;

            if (string.IsNullOrEmpty(hallName) || svgData == null)
            {
                MessageBox.Show("Введите название зала и загрузите схему.");
                return;
            }

            using (var conn = new NpgsqlConnection("Host=localhost;Username=postgres;Password=0813;Database=\"Theatres\""))
            {
                conn.Open();
                var cmd = new NpgsqlCommand("INSERT INTO public.\"Halls\" (\"Theaters\", \"NameHalls\", \"CountSeats\", \"SchemeHalls\") VALUES (@theatre, @name, @count, @scheme)", conn);
                cmd.Parameters.AddWithValue("theatre", theatreId);
                cmd.Parameters.AddWithValue("name", hallName);
                cmd.Parameters.AddWithValue("count", seatsCount);
                cmd.Parameters.AddWithValue("scheme", svgData);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Зал добавлен!");
            this.Close();
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

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            var document = webBrowser1.Document;

            string[] clickableTags = { "path", "circle", "rect", "ellipse", "polygon" };

            foreach (string tag in clickableTags)
            {
                foreach (HtmlElement element in webBrowser1.Document.GetElementsByTagName(tag))
                {
                    element.AttachEventHandler("onclick", delegate {
                        Element_Click(element);
                    });
                }
            }

        }
        private void Element_Click(HtmlElement element)
        {
            string id = element.GetAttribute("id");
            MessageBox.Show("Вы кликнули по месту с ID: " + id);
        }
    }
}
