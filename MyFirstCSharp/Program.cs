using System;
using Microsoft.Data.Sqlite;

class Program
{
    static void Main(string[] args)
    {
        using (var connection = new SqliteConnection("Data Source=mydatabase.db"))
        {
            connection.Open();

            // Create a table
            using (var createTableCmd = connection.CreateCommand())
            {
                createTableCmd.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Expense (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        Price REAL
                    );";
                createTableCmd.ExecuteNonQuery();
            }

            // Main loop
            while (true)
            {
                Console.WriteLine("\nPlease select mode:");
                Console.WriteLine("1. Show item list");
                Console.WriteLine("2. Add an item");
                Console.WriteLine("3. Edit an item");
                Console.WriteLine("4. Delete an item");
                Console.WriteLine("0. Exit");
                string mode = Console.ReadLine();

                if (mode == "0")
                {
                    Console.WriteLine("Exiting program.");
                    break;
                }

                else if (mode == "1")
                {
                    var selectCmd = connection.CreateCommand();
                    selectCmd.CommandText = "SELECT Id, Name, Price FROM Expense";

                    using (var reader = selectCmd.ExecuteReader())
                    {
                        Console.WriteLine("Item List:");
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string name = reader.GetString(1);
                            double price = reader.GetDouble(2);
                            Console.WriteLine($"ID: {id} | Name: {name} | Price: {price}");
                        }
                    }
                }

                else if (mode == "2")
                {
                    Console.Write("Enter item name: ");
                    string itemName = Console.ReadLine();

                    Console.Write("Enter price: ");
                    string priceInput = Console.ReadLine();

                    if (double.TryParse(priceInput, out double price) && price > 0)
                    {
                        var insertCmd = connection.CreateCommand();
                        insertCmd.CommandText = "INSERT INTO Expense (Name, Price) VALUES ($name, $price);";
                        insertCmd.Parameters.AddWithValue("$name", itemName);
                        insertCmd.Parameters.AddWithValue("$price", price);
                        insertCmd.ExecuteNonQuery();
                        Console.WriteLine("Item added successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Invalid price.");
                    }
                }

                else if (mode == "3")
                {
                    Console.Write("Enter the ID of the item to edit: ");
                    string idInput = Console.ReadLine();
                    Console.Write("Enter new name: ");
                    string newName = Console.ReadLine();
                    Console.Write("Enter new price: ");
                    string newPriceInput = Console.ReadLine();

                    if (int.TryParse(idInput, out int id) && double.TryParse(newPriceInput, out double newPrice))
                    {
                        var updateCmd = connection.CreateCommand();
                        updateCmd.CommandText = @"
                            UPDATE Expense
                            SET Name = $name, Price = $price
                            WHERE Id = $id;";
                        updateCmd.Parameters.AddWithValue("$name", newName);
                        updateCmd.Parameters.AddWithValue("$price", newPrice);
                        updateCmd.Parameters.AddWithValue("$id", id);

                        int rowsAffected = updateCmd.ExecuteNonQuery();
                        Console.WriteLine(rowsAffected > 0 ? "Item updated." : "Item not found.");
                    }
                    else
                    {
                        Console.WriteLine("Invalid ID or price.");
                    }
                }

                else if (mode == "4")
                {
                    Console.Write("Enter the ID of the item to delete: ");
                    string idInput = Console.ReadLine();
                    if (int.TryParse(idInput, out int id))
                    {
                        var deleteCmd = connection.CreateCommand();
                        deleteCmd.CommandText = "DELETE FROM Expense WHERE Id = $id;";
                        deleteCmd.Parameters.AddWithValue("$id", id);
                        int rowsDeleted = deleteCmd.ExecuteNonQuery();
                        Console.WriteLine(rowsDeleted > 0 ? "Item deleted." : "Item not found.");
                    }
                    else
                    {
                        Console.WriteLine("Invalid ID.");
                    }
                }

                else
                {
                    Console.WriteLine("Invalid option.");
                }
            }
        }
    }
}
