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
                Console.WriteLine("4. Updte an existing record");
                Console.WriteLine();
                string userInput = Console.ReadLine()!;

                switch (userInput)
                {
                    case "0":
                        close = true;
                        break;

                    case "1":
                        ViewRecords();
                        break;
                    case "2":
                        AddRecord();
                        break;
                    case "3":
                        //DeleteRecord();
                        break;
                    case "4":
                        //UpdateRecord();
                        break;
                    default:
                        close = true;
                        break;
                }
            }
        }
        private static void ViewRecords()
        {
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
            string date = GetDateInput();
            int quanitity = GetNumberInput();

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCommand = connection.CreateCommand();
                tableCommand.CommandText =
                    $"INSERT INTO Coding(date, quantity) VALUES('{date}', '{quanitity}')";

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
            return dateInput;
        }
        internal static int GetNumberInput()
        {
            Console.WriteLine("How many hours did you work on coding today?");
            int hours = Convert.ToInt32(Console.ReadLine());

            return hours;
        }

        private static void DeleteRecord()
        {
            Console.Clear();
            ViewRecords();
            var recordID = GetNumberInput(Console.WriteLine("Please type the ID of the record you would like to delete, or 0 (zero) to return to the main menu."));

        }
        
    }
    
    public class CodingHours
    {
        public int id { get; set;}

        public DateTime date{ get; set;}

        public int Quantity {get; set;}
    }
}