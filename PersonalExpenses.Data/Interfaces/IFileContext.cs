namespace PersonalExpenses.Data.Contexts;

public interface IFileContext
{
    List<T> LoadData<T>(string fileName);
    bool SaveData<T>(string fileName, List<T> data);
}