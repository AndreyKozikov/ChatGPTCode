using System;
using System.Data.SQLite;
using System.Windows.Forms;

namespace DatabaseApp
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }

    public class MainForm : Form
    {
        private SQLiteConnection connection;
        private SQLiteCommand command;

        private TextBox dateTextBox;
        private TextBox cityTextBox;
        private TextBox amountTextBox;
        private Button addButton;
        private DataGridView dataGridView;

        public MainForm()
        {
            InitializeComponent();
            InitializeDatabase();
        }

        private void InitializeComponent()
        {
            dateTextBox = new TextBox();
            cityTextBox = new TextBox();
            amountTextBox = new TextBox();
            addButton = new Button();
            dataGridView = new DataGridView();

            dateTextBox.Location = new System.Drawing.Point(50, 50);
            dateTextBox.Name = "dateTextBox";
            dateTextBox.Size = new System.Drawing.Size(100, 20);

            cityTextBox.Location = new System.Drawing.Point(200, 50);
            cityTextBox.Name = "cityTextBox";
            cityTextBox.Size = new System.Drawing.Size(100, 20);

            amountTextBox.Location = new System.Drawing.Point(350, 50);
            amountTextBox.Name = "amountTextBox";
            amountTextBox.Size = new System.Drawing.Size(100, 20);

            addButton.Location = new System.Drawing.Point(500, 50);
            addButton.Name = "addButton";
            addButton.Size = new System.Drawing.Size(75, 23);
            addButton.Text = "Add";
            addButton.UseVisualStyleBackColor = true;
            addButton.Click += AddButton_Click;

            dataGridView.Location = new System.Drawing.Point(50, 100);
            dataGridView.Size = new System.Drawing.Size(525, 200);
            dataGridView.Columns.Add("Date", "Дата");
            dataGridView.Columns.Add("City", "Город");
            dataGridView.Columns.Add("Amount", "Сумма");

            Controls.Add(dateTextBox);
            Controls.Add(cityTextBox);
            Controls.Add(amountTextBox);
            Controls.Add(addButton);
            Controls.Add(dataGridView);

            var dateLabel = new Label();
            dateLabel.Text = "Дата:";
            dateLabel.Location = new System.Drawing.Point(50, 30);

            var cityLabel = new Label();
            cityLabel.Text = "Город:";
            cityLabel.Location = new System.Drawing.Point(200, 30);

            var amountLabel = new Label();
            amountLabel.Text = "Сумма:";
            amountLabel.Location = new System.Drawing.Point(350, 30);

            Controls.Add(dateLabel);
            Controls.Add(cityLabel);
            Controls.Add(amountLabel);
        }

        private void InitializeDatabase()
        {
            connection = new SQLiteConnection("Data Source=database.db;Version=3;");
            connection.Open();

            string createTableQuery = "CREATE TABLE IF NOT EXISTS Transactions (Id INTEGER PRIMARY KEY AUTOINCREMENT, Date TEXT, City TEXT, Amount REAL)";
            command = new SQLiteCommand(createTableQuery, connection);
            command.ExecuteNonQuery();
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            string date = dateTextBox.Text;
            string city = cityTextBox.Text;
            double amount;

            if (double.TryParse(amountTextBox.Text, out amount))
            {
                string insertQuery = "INSERT INTO Transactions (Date, City, Amount) VALUES (@date, @city, @amount)";
                command = new SQLiteCommand(insertQuery, connection);
                command.Parameters.AddWithValue("@date", date);
                command.Parameters.AddWithValue("@city", city);
                command.Parameters.AddWithValue("@amount", amount);

                if (command.ExecuteNonQuery() > 0)
                {
                    MessageBox.Show("Данные успешно добавлены в базу данных.");
                    dataGridView.Rows.Add(new string[] { date, city, amount.ToString() });
                    dateTextBox.Clear();
                    cityTextBox.Clear();
                    amountTextBox.Clear();
                }
                else
                {
                    MessageBox.Show("Произошла ошибка при добавлении данных в базу данных.");
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, введите корректное значение для Суммы.");
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            connection.Close();
        }
    }
}
