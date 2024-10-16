using System.Text.Json;

public static class HistoryLogger
{
    private static readonly string filePath = "DeletedRecords.json";

    public static void LogDeletedRecord(string record)
    {
        try
        {
            List<string> deletedRecords = new List<string>();

            if (File.Exists(filePath))
            {
                string existingJson = File.ReadAllText(filePath);
                if (!string.IsNullOrWhiteSpace(existingJson))
                {
                    deletedRecords = JsonSerializer.Deserialize<List<string>>(existingJson) ?? new List<string>();
                }
            }

            deletedRecords.Add(record);

            string jsonString = JsonSerializer.Serialize(deletedRecords, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, jsonString);

            Console.WriteLine("Record successfully logged in history.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error logging record to history: {ex.Message}");
        }
    }

    public static void ViewHistory<T>()
    {
        try
        {
            if (File.Exists(filePath))
            {
                string jsonString = File.ReadAllText(filePath);
                List<T> records = JsonSerializer.Deserialize<List<T>>(jsonString);

                if (records != null && records.Count > 0)
                {
                    Console.WriteLine("Deleted Records History:");
                    foreach (var record in records)
                    {
                        Console.WriteLine(JsonSerializer.Serialize(record, new JsonSerializerOptions { WriteIndented = true }));
                    }
                }
                else
                {
                    Console.WriteLine("No records found.");
                }
            }
            else
            {
                Console.WriteLine("No history records found.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading history: {ex.Message}");
        }
    }
}

