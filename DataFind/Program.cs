using System.Data.SqlClient;

class Program
{
    static void Main()
    {
        string connectionString = "";//Add Your connection string here
        Console.Write("Enter text to search: ");
        string searchText = Console.ReadLine();
        string filePath = @"D:\SearchResults.txt"; // Output file on D drive
        SearchTextInDatabase(connectionString, searchText, filePath);
    }

    static void SearchTextInDatabase(string connectionString, string searchText, string filePath)
    {
        using (var writer = new StreamWriter(filePath, append: false)) // overwrite existing file
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();

            // Get all varchar/nvarchar columns
            string columnQuery = @"
                SELECT TABLE_SCHEMA, TABLE_NAME, COLUMN_NAME 
                FROM INFORMATION_SCHEMA.COLUMNS
                WHERE DATA_TYPE IN ('varchar', 'nvarchar')";

            var columns = new List<(string Schema, string Table, string Column)>();

            using (var cmd = new SqlCommand(columnQuery, connection))
            {
                cmd.CommandTimeout = 0;
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        columns.Add((reader.GetString(0), reader.GetString(1), reader.GetString(2)));
                    }
                }
            }

            int total = columns.Count;
            int count = 0;

            Console.WriteLine($"\nSearching {total} columns...");

            foreach (var col in columns)
            {
                count++;
                Console.Write($"\rProcessing {count}/{total}: {col.Schema}.{col.Table}.{col.Column}...");

                string sql = $"SELECT TOP 1 1 FROM [{col.Schema}].[{col.Table}] WHERE [{col.Column}] LIKE @search";
                using (var searchCmd = new SqlCommand(sql, connection))
                {
                    searchCmd.CommandTimeout = 0;
                    searchCmd.Parameters.AddWithValue("@search", "%" + searchText + "%");
                    var result = searchCmd.ExecuteScalar();
                    if (result != null)
                    {
                        string foundText = $"{col.Schema}.{col.Table}.{col.Column}";
                        Console.WriteLine($"\nFound in --> {foundText}");
                        writer.WriteLine(foundText); // Write to file immediately
                        writer.Flush(); // Ensure it writes immediately
                    }
                }
            }

            Console.WriteLine($"\nSearch completed. Results saved in {filePath}");
            Console.ReadLine();
        }
    }
}
