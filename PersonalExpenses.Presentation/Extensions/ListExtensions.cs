namespace PersonalExpenses.Presentation.Extensions;

public static class ListExtensions
{
    // Usando 'this' se puede llamar directamente desde cualquier List<T>
    public static List<T> TakeFirstN<T>(this List<T> data, int limit)
    {
        if (data == null || data.Count == 0 || limit < 1)
            return [];

        List<T> items = [];
        int actualLimit = (limit == -1 || limit > data.Count) ? data.Count : limit;

        for (int i = 0; i < actualLimit; i++)
        {
            items.Add(data[i]);
        }

        return items;
    }

    public static List<T> TakeFirstN<T>(this List<T> data, int start, int limit = -1)
    {
        if (data == null || data.Count == 0 || start >= data.Count || (limit < 1 && limit != -1))
            return [];

        List<T> items = [];
        int actualLimit = (limit == -1 || limit > data.Count) ? data.Count : limit;
            
        for (int i = start; i < actualLimit; i++)
        {
            items.Add(data[i]);
        }

        return items;
    }
}