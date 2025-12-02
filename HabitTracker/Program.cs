using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Data.Sqlite;

namespace habitTracker
{
    class Program
    {
        
        static string connectionString = "Data Source = habitTracker.db";
        static void Main(string[] args)
        {
            

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCommand = connection.CreateCommand();
                tableCommand.CommandText = @"CREATE TABLE IF NOT EXISTS Coding(
                                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                            Date TEXT,
                                            Quantity INTEGER
                                            )";

                tableCommand.ExecuteNonQuery();
                connection.Close();
            }
            GetUserInput();
        }
        static void GetUserInput()
        {
            Console.Clear();
            bool close = false;
            while (!close)
            {
                Console.WriteLine("Welcome to the main menu");
                Console.WriteLine("What do you want to do?");
                Console.WriteLine("0. Exit");
                Console.WriteLine("1. View records");
                Console.WriteLine("2. Add new record");
                Console.WriteLine("3. Delete an existing record");
                Console.WriteLine("4. Update an existing record");
                Console.WriteLine();
                string userInput = Console.ReadLine()!;

                switch (userInput)
                {
                    case "0":
                        Environment.Exit(0);
                        break;

                    case "1":
                        ViewRecords();
                        break;
                    case "2":
                        AddRecord();
                        break;
                    case "3":
                        DeleteRecord();
                        break;
                    case "4":
                        UpdateRecord();
                        break;
                    default:
                        Console.WriteLine("Selection not recognized. Please insert a number between 0 and 4.");
                        break;
                }
            }
        }
        private static void ViewRecords()
        {
            Console.Clear();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCommand = connection.CreateCommand();
                tableCommand.CommandText = 
                    "SELECT * FROM Coding";
                    List<CodingHours> tableData = new();

                    SqliteDataReader reader = tableCommand.ExecuteReader();

                    if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        tableData.Add(
                        new CodingHours
                        {
                            id=reader.GetInt32(0),
                            date = DateTime.ParseExact(reader.GetString(1), "mm-dd-yy", new CultureInfo("en-US")),
                            Quantity = reader.GetInt32(2)
                        }); ;
                    }
                }
                else
                {
                    Console.WriteLine("No rows are found");
                }
                connection.Close();

                Console.WriteLine("---------------------------------------------------------\n");

                foreach (var ch in tableData)
                {
                    Console.WriteLine($"{ch.id} - {ch.date.ToString("mm-dd-yy")} - Hours: {ch.Quantity}");
                }

                Console.WriteLine("---------------------------------------------------------\n");
            }
        }
        private static void AddRecord()
        {
            Console.Clear();
            string date = GetDateInput();
            int quantity = GetNumberInput("Please insert the number of hours you have coded today. Enter 0 (zero) to return to the main menu.");
            if(quantity <= 0)
            {
                GetUserInput();
            }
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCommand = connection.CreateCommand();
                tableCommand.CommandText =
                    $"INSERT INTO Coding(date, quantity) VALUES('{date}', '{quantity}')";

                tableCommand.ExecuteNonQuery();

                connection.Close();
            }

        }
        
        internal static string GetDateInput()
        {
            Console.WriteLine("Enter the date (MM-DD-YY), or press 0 (zero) to return to the main menu: ");
            string dateInput = Console.ReadLine()!;

            if (dateInput == "0")
            {
                GetUserInput();
            }

            while(!DateTime.TryParseExact(dateInput, "mm-dd-yy", new CultureInfo("en-US"), DateTimeStyles.None, out _))
            {
                Console.WriteLine("Invalid input. Please make sure the format is correct (mm-dd-yy). Press 0 (zero) to return to the main menu. ");
                dateInput = Console.ReadLine()!;
            }
            return dateInput;
        }
        internal static int GetNumberInput(string message)
        {
            Console.WriteLine(message);
            string hours = Console.ReadLine()!;
            if(hours == "0")
            {
                GetUserInput();
            }
            while(!Int32.TryParse(hours, out _ ) || Convert.ToInt32(hours) < 0)
            {
                Console.WriteLine("Invalid. Try again.");
                hours = Console.ReadLine()!;
            }

            int finalHour = Convert.ToInt32(hours);
            return finalHour;
        }

        private static void DeleteRecord()
        {
            Console.Clear();
            ViewRecords();
            var recordID = GetNumberInput("Please type the ID of the record you would like to delete, or 0 (zero) to return to the main menu.");
            if(recordID <= 0)
            {
                GetUserInput();
            }
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCommand = connection.CreateCommand();
                tableCommand.CommandText = $"DELETE from Coding WHERE ID = '{recordID}'";

                int rowCount = tableCommand.ExecuteNonQuery();

                if(rowCount == 0)
                {
                    Console.WriteLine($"Record with id: {recordID} does not exist.");
                    DeleteRecord();
                }
                Console.WriteLine($"Record {recordID} was deleted.");

                connection.Close();

                GetUserInput();
            }
        }

        internal static void UpdateRecord()
        {
            Console.Clear();
            ViewRecords();

            var recordID = GetNumberInput("Please type the ID of the record to update. Press 0 (zero) to return to the main menu.");

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = $"SELECT EXISTS(SELECT 1 FROM Coding WHERE Id = '{recordID}')";

                int query = Convert.ToInt32(tableCmd.ExecuteScalar());

                if(query == 0)
                {
                    Console.WriteLine($"Record ID {recordID} does not exist.");
                    connection.Close();
                    UpdateRecord();
                }

                string date = GetDateInput();
                int quantity = GetNumberInput("Please type how many hours you have coded today.");

                var tableCommand = connection.CreateCommand();
                tableCommand.CommandText = $"UPDATE Coding SET date = '{date}', Quantity = '{quantity}' WHERE Id = '{recordID}'";
                tableCommand.ExecuteNonQuery();
                connection.Close();

            }
        }
        
    }
    
    public class CodingHours
    {
        public int id { get; set;}

        public DateTime date{ get; set;}

        public int Quantity {get; set;}
    }
}