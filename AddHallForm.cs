using Npgsql;
using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace TheaterApp
{
    public partial class AddHallForm : Form
    {
        private readonly int theatreId;
        private readonly int? hallIdToEdit = null;
        private byte[] svgData;

        public AddHallForm(int theatreId)
        {
            InitializeComponent();
            this.theatreId = theatreId;
            this.webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(this.webBrowser1_DocumentCompleted);
        }

        // Конструктор для редактирования зала
        public AddHallForm(int theatreId, int hallId, string name, int capacity)
        {
            InitializeComponent();
            this.theatreId = theatreId;
            this.hallIdToEdit = hallId;

            textBoxHallName.Text = name;
            numericUpDownSeats.Value = capacity;

            this.webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(this.webBrowser1_DocumentCompleted);

            LoadSvgFromDatabase(hallId);
        }

        private void LoadSvgFromDatabase(int hallId)
        {
            using (var conn = new NpgsqlConnection("Host=localhost;Username=postgres;Password=0813;Database=\"Theatres\""))
            {
                conn.Open();
                var cmd = new NpgsqlCommand("SELECT \"SchemeHalls\" FROM public.\"Halls\" WHERE \"idHalls\" = @id", conn);
                cmd.Parameters.AddWithValue("id", hallId);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read() && !reader.IsDBNull(0))
                    {
                        svgData = (byte[])reader["SchemeHalls"];
                        string svgContent = Encoding.UTF8.GetString(svgData);
                        string htmlContent = $@"
<html>
    <head>
        <meta http-equiv='X-UA-Compatible' content='IE=edge'>
        //<style>
        //    path {{
        //        fill: blue;
        //        stroke: black;
        //        stroke-width: 2;
        //    }}
        //    rect {{
        //        fill: blue;
        //        stroke: black;
        //        stroke-width: 2;
        //    }}
        //</style>
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
        //<style>
        //    path {{
        //        fill: blue;
        //        stroke: black;
        //        stroke-width: 2;
        //    }}
        //    rect {{
        //        fill: blue;
        //        stroke: black;
        //        stroke-width: 2;
        //    }}
        //</style>
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

                if (hallIdToEdit.HasValue)
                {
                    // Режим редактирования
                    var cmd = new NpgsqlCommand("UPDATE public.\"Halls\" SET \"NameHalls\" = @name, \"CountSeats\" = @count, \"SchemeHalls\" = @scheme WHERE \"idHalls\" = @id", conn);
                    cmd.Parameters.AddWithValue("name", hallName);
                    cmd.Parameters.AddWithValue("count", seatsCount);
                    cmd.Parameters.AddWithValue("scheme", svgData);
                    cmd.Parameters.AddWithValue("id", hallIdToEdit.Value);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Зал успешно обновлён!");
                }
                else
                {
                    // Режим добавления
                    var cmd = new NpgsqlCommand("INSERT INTO public.\"Halls\" (\"Theaters\", \"NameHalls\", \"CountSeats\", \"SchemeHalls\") VALUES (@theatre, @name, @count, @scheme)", conn);
                    cmd.Parameters.AddWithValue("theatre", theatreId);
                    cmd.Parameters.AddWithValue("name", hallName);
                    cmd.Parameters.AddWithValue("count", seatsCount);
                    cmd.Parameters.AddWithValue("scheme", svgData);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Зал добавлен!");
                }
            }
            this.DialogResult = DialogResult.OK;
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
                    element.AttachEventHandler("onclick", delegate
                    {
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
