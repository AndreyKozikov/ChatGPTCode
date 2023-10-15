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
    {  // Переменные для работы с базой данных и элементами управления на форме
        private SQLiteConnection connection; // Подключение к базе данных SQLite
        private SQLiteCommand command; // Команда базы данных

        private TextBox dateTextBox; // Поле для ввода даты
        private TextBox cityTextBox; // Поле для ввода города
        private TextBox amountTextBox; // Поле для ввода суммы
        private Button addButton; // Кнопка для добавления записи
        private Button readButton; // Кнопка для чтения данных из базы
        private Button clearButton; // Кнопка для очистки записей в базе и на форме
        private DataGridView dataGridView; // Таблица для отображения данных
        private Panel panel; // Объявление переменной panel в классе

        public MainForm()
        {
            InitializeComponent();
            InitializeDatabase();
            this.AutoSize = true; // Установка автоматического размера формы

        }

        private void InitializeComponent()
        {
            // Создание панели для размещения элементов
            panel = new Panel();
            panel.Location = new System.Drawing.Point(0, 0);
            panel.Size = new System.Drawing.Size(800, 500); // Задайте размер, который подходит для вашего приложения
            panel.AutoScroll = true; // Включите автоматическую прокрутку

            // Создание текстовых меток
            var dateLabel = new Label();
            dateLabel.Text = "Введите дату:";
            dateLabel.Location = new System.Drawing.Point(50, 30);

            var cityLabel = new Label();
            cityLabel.Text = "Введите город:";
            cityLabel.Location = new System.Drawing.Point(200, 30);

            var amountLabel = new Label();
            amountLabel.Text = "Введите сумму:";
            amountLabel.Location = new System.Drawing.Point(350, 30);

            // Создание текстовых полей для ввода даты, города и суммы
            dateTextBox = new TextBox();
            cityTextBox = new TextBox();
            amountTextBox = new TextBox();

            // Создание кнопок для выполнения операций добавления, чтения и очистки данных
            addButton = new Button();
            readButton = new Button();
            clearButton = new Button();
            // Создание кнопки для сохранения данных в базе данных
            var saveToDatabaseButton = new Button();
            saveToDatabaseButton.Location = new System.Drawing.Point(500, 400);
            saveToDatabaseButton.Name = "saveToDatabaseButton";
            saveToDatabaseButton.Size = new System.Drawing.Size(150, 30);
            saveToDatabaseButton.Text = "Сохранить в базу данных";
            saveToDatabaseButton.UseVisualStyleBackColor = true;
            saveToDatabaseButton.Click += SaveToDatabaseButton_Click;

            // Создание таблицы для отображения данных
            dataGridView = new DataGridView();

            // Установка положения и размеров элементов управления на панели
            dateLabel.Location = new System.Drawing.Point(50, 30);
            dateLabel.Name = "dateLabel";
            dateLabel.Size = new System.Drawing.Size(150, 20);

            cityLabel.Location = new System.Drawing.Point(200, 30);
            cityLabel.Name = "cityLabel";
            cityLabel.Size = new System.Drawing.Size(150, 20);

            amountLabel.Location = new System.Drawing.Point(350, 30);
            amountLabel.Name = "amountLabel";
            amountLabel.Size = new System.Drawing.Size(150, 20);

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

            dataGridView.Location = new System.Drawing.Point(50, 100);
            dataGridView.Name = "dataGridView";
            dataGridView.Size = new System.Drawing.Size(500, 300);
            dataGridView.ColumnCount = 3;
            dataGridView.Columns[0].Name = "Date";
            dataGridView.Columns[1].Name = "City";
            dataGridView.Columns[2].Name = "Amount";

            // Добавление элементов на панель
            panel.Controls.Add(dateLabel);
            panel.Controls.Add(cityLabel);
            panel.Controls.Add(amountLabel);
            panel.Controls.Add(dateTextBox);
            panel.Controls.Add(cityTextBox);
            panel.Controls.Add(amountTextBox);
            panel.Controls.Add(addButton);
            panel.Controls.Add(readButton);
            panel.Controls.Add(clearButton);
            panel.Controls.Add(dataGridView);
            panel.Controls.Add(saveToDatabaseButton);

            // Добавление панели на форму
            Controls.Add(panel);

            // Инициализация connection и command
            connection = new SQLiteConnection("Data Source=database.db;Version=3;");
            connection.Open();
            command = connection.CreateCommand();
        }


        private void SaveToDatabaseButton_Click(object sender, EventArgs e)
        {
            bool hasNullValue = false;

            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.Index == dataGridView.Rows.Count - 1) continue; // Пропустить последнюю строку

                if (row.Cells[0].Value != null && row.Cells[1].Value != null && row.Cells[2].Value != null)
                {
                    string date = row.Cells[0].Value.ToString();
                    string city = row.Cells[1].Value.ToString();
                    string amount = row.Cells[2].Value.ToString();

                    string insertQuery = "INSERT INTO Transactions (Date, City, Amount) VALUES (@date, @city, @amount)";
                    command = new SQLiteCommand(insertQuery, connection);
                    command.Parameters.AddWithValue("@date", date);
                    command.Parameters.AddWithValue("@city", city);
                    command.Parameters.AddWithValue("@amount", amount);

                    if (command.ExecuteNonQuery() <= 0)
                    {
                        hasNullValue = true;
                        break;
                    }
                }
                else
                {
                    hasNullValue = true;
                    break;
                }
            }

            if (!hasNullValue)
            {
                MessageBox.Show("Данные успешно добавлены в базу данных.");
            }
            else
            {
                MessageBox.Show("Одно или несколько значений в таблице пустые. Невозможно добавить в базу данных.");
            }
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
