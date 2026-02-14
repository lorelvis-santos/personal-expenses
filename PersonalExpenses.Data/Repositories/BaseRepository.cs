using PersonalExpenses.Data.Interfaces;
using PersonalExpenses.Data.Contexts;
using PersonalExpenses.Entities;

namespace PersonalExpenses.Data.Repositories;

public abstract class BaseRepository<T> : IRepository<T> where T : BaseModel
{
    protected readonly IFileContext _context;
    protected readonly string _fileName;

    protected BaseRepository(IFileContext context, string fileName)
    {
        _context = context;
        _fileName = fileName;
    }

    public List<T> GetAll()
    {
        return _context.LoadData<T>(_fileName);
    }

    public T? GetById(string id)
    {
        List<T> items = GetAll();
        return items.FirstOrDefault(item => item.Id == id);
    }

    public bool Add(T item)
    {
        List<T> items = GetAll();
        items.Add(item);
        return _context.SaveData(_fileName, items);
    }

    public bool Update(T item)
    {
        List<T> items = GetAll();
        int index = items.FindIndex(i => i.Id == item.Id);

        if (index == -1)
        {
            return false;
        }

        items[index] = item;
        return _context.SaveData(_fileName, items);
    }

    public bool Delete(string id)
    {
        List<T> items = GetAll();
        T? itemToRemove = GetById(id);

        if (itemToRemove == null)
        {
            return false;
        }

        items.Remove(itemToRemove);
        return _context.SaveData(_fileName, items);
    }
}

