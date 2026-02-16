using PersonalExpenses.Entities;
using PersonalExpenses.Data.Repositories;

namespace PersonalExpenses.Business;

public class ExpenseService
{
    private ExpenseRepository _repo;
    private CategoryRepository _categoryRepo;

    public ExpenseService(ExpenseRepository repo, CategoryRepository categoryRepo)
    {
        _repo = repo;
        _categoryRepo = categoryRepo;
    }

    public List<Expense> GetAll() => _repo.GetAll();
    public List<Expense> GetAllReversed()
    {
        List<Expense> expenses = _repo.GetAll();
        expenses.Reverse();
        return expenses;
    }
    public Expense? GetById(string id) => _repo.GetById(id);

    public (bool Success, string Message) Create(decimal amount, string description, string categoryId)
    {
        var validation = Validate(amount, description, categoryId);

        if (!validation.Success)
        {
            return (false, validation.Message);
        }

        Expense expense = new()
        {
            Amount = amount,
            Description = description,
            CategoryId = categoryId,
            Date = DateTime.Now
        };

        bool created = _repo.Add(expense);

        return created ? 
            (true, "Gasto agregado correctamente.") :
            (false, "Error al guardar en base de datos.");
    }

    public (bool Success, string Message) Update(string id, decimal amount, string description, string categoryId)
    {
        Expense? expense = _repo.GetById(id);

        if (expense == null)
        {
            return (false, "El gasto no existe.");
        }

        var validation = Validate(amount, description, categoryId);

        if (!validation.Success)
        {
            return (false, validation.Message);
        }

        expense.Amount = amount;
        expense.Description = description;
        expense.CategoryId = categoryId;

        bool updated = _repo.Update(expense);

        return updated ? 
            (true, "Gasto actualizado correctamente.") :
            (false, "Error al actualizar en base de datos.");
    }

    public (bool Success, string Message) Validate(decimal amount, string description, string categoryId)
    {
        if (amount <= 0)
        {
            return (false, "Monto inválido.");
        }

        Category? category = _categoryRepo.GetById(categoryId);

        if (category == null)
        {
            return (false, "La categoría proporcionada no existe.");
        }
            
        if (!string.IsNullOrEmpty(description) && description.Length < 3)
        {
            return (false, "La descripción debe tener al menos 3 caracteres.");
        }

        return (true, "Operación realizada correctamente.");
    }

    public (bool Success, string Message) Delete(string id)
    {
        throw new NotImplementedException();
    }
}