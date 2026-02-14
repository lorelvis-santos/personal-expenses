namespace PersonalExpenses.Data.Contexts;

public interface IFileContext<T>
{
    List<T> LoadData(string fileName);
    bool SaveData<T>(string fileName, List<T> data);
}