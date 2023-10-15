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
        private Button readButton;
        private Button clearButton;
        private DataGridView dataGridView;

        public MainForm()
        {
            InitializeComponent();
            InitializeDatabase();
        }

     private void InitializeComponent()
{
    // Создание текстовых полей для ввода даты, города и суммы
    dateTextBox = new TextBox();
    cityTextBox = new TextBox();
    amountTextBox = new TextBox();

    // Создание кнопок для выполнения операций добавления, чтения и очистки данных
    addButton = new Button();
    readButton = new Button();
    clearButton = new Button();

    // Создание таблицы для отображения данных
    dataGridView = new DataGridView();

    // Установка положения и размеров элементов управления на форме
    dateTextBox.Location = new System.Drawing.Point(50, 50);
    dateTextBox.Name = "dateTextBox";
    dateTextBox.Size = new System.Drawing.Size(100, 20);

    cityTextBox.Location = new System.Drawing.Point(200, 50);
    cityTextBox.Name = "cityTextBox";
    cityTextBox.Size = new System.Drawing.Size(100, 20);

    amountTextBox.Location = new System.Drawing.Point(350, 50);
    amountTextBox.Name = "amountTextBox";
    amountTextBox.Size = new System.Drawing.Size(100, 20);

    // Установка положения и размеров кнопок
    addButton.Location = new System.Drawing.Point(500, 50);
    addButton.Name = "addButton";
    addButton.Size = new System.Drawing.Size(75, 23);
    addButton.Text = "Add";
    addButton.UseVisualStyleBackColor = true;
    addButton.Click += AddButton_Click;

    readButton.Location = new System.Drawing.Point(600, 50);
    readButton.Name = "readButton";
    readButton.Size = new System.Drawing.Size(75, 23);
    readButton.Text = "Read";
    readButton.UseVisualStyleBackColor = true;
    readButton.Click += ReadButton_Click;

    clearButton.Location = new System.Drawing.Point(700, 50);
    clearButton.Name = "clearButton";
    clearButton.Size = new System.Drawing.Size(75, 23);
    clearButton.Text = "Clear";
    clearButton.UseVisualStyleBackColor = true;
    clearButton.Click += ClearButton_Click;

    // Установка положения и размеров таблицы
    dataGridView.Location = new System.Drawing.Point(50, 100);
    dataGridView.Name = "dataGridView";
    dataGridView.Size = new System.Drawing.Size(500, 300);
    dataGridView.ColumnCount = 3;
    dataGridView.Columns[0].Name = "Date";
    dataGridView.Columns[1].Name = "City";
    dataGridView.Columns[2].Name = "Amount";

    // Добавление элементов на форму
    Controls.Add(dateTextBox);
    Controls.Add(cityTextBox);
    Controls.Add(amountTextBox);
    Controls.Add(addButton);
    Controls.Add(readButton);
    Controls.Add(clearButton);
    Controls.Add(dataGridView);

    // Создание и добавление меток для указания предназначения каждого текстового поля
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
                    dataGridView.Rows.Add(new string[] { date, city, amount.ToString() });
                    MessageBox.Show("Данные успешно добавлены в базу данных.");
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

    private void ReadButton_Click(object sender, EventArgs e)
        {
            string query = "SELECT * FROM Transactions";
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(query, connection);
            var dataSet = new System.Data.DataSet();
            dataAdapter.Fill(dataSet, "Transactions");

            dataGridView.Rows.Clear();
            foreach (System.Data.DataRow row in dataSet.Tables["Transactions"].Rows)
            {
                string date = row["Date"].ToString();
                string city = row["City"].ToString();
                string amount = row["Amount"].ToString();
                dataGridView.Rows.Add(new string[] { date, city, amount });
            }
        }

    private void ClearButton_Click(object sender, EventArgs e)
        {
           string clearQuery = "DELETE FROM Transactions";
        command = new SQLiteCommand(clearQuery, connection);

        if (command.ExecuteNonQuery() > 0)
        {
        dataGridView.Rows.Clear();
        MessageBox.Show("Все записи успешно удалены из базы данных.");
        }
        else
        {
        MessageBox.Show("Произошла ошибка при очистке базы данных.");
     }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            connection.Close();
        }
    }
}
