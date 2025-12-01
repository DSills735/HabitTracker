using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
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
        private static void ViewRecords()
        {
            string date = GetDateInput();
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
        
    }
    
}