namespace PersonalExpenses.Data.Interfaces;

public interface IRepository<T>
{
    bool Add(T entity);
    List<T> GetAll();
    T? GetById(string id);
    bool Update(T entity);
    bool Delete(T entity);
}