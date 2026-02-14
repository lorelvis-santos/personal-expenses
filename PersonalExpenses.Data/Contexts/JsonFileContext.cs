using System.Text.Json;

namespace PersonalExpenses.Data.Contexts;

public class JsonFileContext : IFileContext
{
    private static readonly JsonSerializerOptions _options = new () { WriteIndented = true };

    public List<T> LoadData<T>(string fileName)
    {
        if (!File.Exists(fileName))
        {
            return [];
        }

        try
        {
            string json = File.ReadAllText(fileName);
            return JsonSerializer.Deserialize<List<T>>(json) ?? [];
        }
        catch
        {
            return [];
        }
    }

    public bool SaveData<T>(string fileName, List<T> data)
    {
        string json = JsonSerializer.Serialize(data, _options);

        string? directory = Path.GetDirectoryName(fileName);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        try
        {
            File.WriteAllText(fileName, json);
            return true;
        } 
        catch(Exception ex)
        {
            Console.WriteLine($"JSONFileContext@SaveData: {ex.Message}");
            return false;
        }
    }
}