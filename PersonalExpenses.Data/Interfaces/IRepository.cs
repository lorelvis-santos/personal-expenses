namespace PersonalExpenses.Data.Interfaces;

public interface IRepository<T>
{
    bool Add(T entity);
    List<T> GetAll();
    T? FindById(string id);
    bool Update(T entity);
    bool Delete(string id);
}