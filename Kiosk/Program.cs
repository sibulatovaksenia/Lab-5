using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kiosk
{
    class Program
    {
        static string connectionString = @"Server=localhost\SQLEXPRESS;Database=master;Trusted_Connection=True;";


        static void Main(string[] args)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Створення таблиці Kiosk
                string createTableQuery = @"
                    CREATE TABLE KioskSQL (
                        Id INT PRIMARY KEY IDENTITY(1,1),
                        Name NVARCHAR(100),
                        Type NVARCHAR(20),
                        Quantity INT,
                        PricePerUnit DECIMAL(10, 2)
                    );";

                using (SqlCommand command = new SqlCommand(createTableQuery, connection))
                {
                    try
                    {
                        command.ExecuteNonQuery();
                        Console.WriteLine("Таблиця 'KioskSQL' успішно створена.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Помилка створення таблиці: " + ex.Message);
                    }
                }

                // Додавання прикладів даних до таблиці
                string insertDataQuery = @"
                    INSERT INTO KioskSQL (Name, Type, Quantity, PricePerUnit) VALUES
                    ('Газета A', 'газета', 50, 5.00),
                    ('Журнал B', 'журнал', 20, 15.50),
                    ('Газета C', 'газета', 30, 3.75),
                    ('Журнал D', 'журнал', 15, 20.00);";

                using (SqlCommand command = new SqlCommand(insertDataQuery, connection))
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine("Дані додані до таблиці 'KioskSQL'.");
                }

                // Перегляд даних з таблиці
                string selectAllQuery = "SELECT * FROM KioskSQL";
                using (SqlCommand command = new SqlCommand(selectAllQuery, connection))
                {
                    SqlDataReader reader = command.ExecuteReader();
                    Console.WriteLine("\nДані в таблиці 'KioskSQL':");
                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["Id"]} {reader["Name"]} {reader["Type"]} {reader["Quantity"]} {reader["PricePerUnit"]}");
                    }
                    reader.Close();
                }

                // Визначення загальної вартості усіх газет
                string totalCostQuery = "SELECT SUM(Quantity * PricePerUnit) AS TotalCost FROM KioskSQL WHERE Type = 'газета'";
                using (SqlCommand command = new SqlCommand(totalCostQuery, connection))
                {
                    var totalCost = command.ExecuteScalar();
                    Console.WriteLine($"\nЗагальна вартість усіх газет: {totalCost} грн");
                }

                // Підрахунок кількості журналів у вказаному діапазоні цін
                decimal minPrice = 10.00m; // Замість цього значення можна підставити Х
                decimal maxPrice = 20.00m; // Замість цього значення можна підставити Y
                string countJournalsQuery = @"
                    SELECT COUNT(*) AS JournalCount FROM Kiosk 
                    WHERE Type = 'журнал' AND PricePerUnit BETWEEN @MinPrice AND @MaxPrice";

                using (SqlCommand command = new SqlCommand(countJournalsQuery, connection))
                {
                    command.Parameters.AddWithValue("@MinPrice", minPrice);
                    command.Parameters.AddWithValue("@MaxPrice", maxPrice);
                    var journalCount = command.ExecuteScalar();
                    Console.WriteLine($"\nКількість журналів з ціною від {minPrice} до {maxPrice} грн: {journalCount}");
                }
            }

        }
    }
}

