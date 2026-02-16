using PersonalExpenses.Entities;
using PersonalExpenses.Data.Repositories;

namespace PersonalExpenses.Business;

public class CategoryService
{
    private readonly CategoryRepository _repo;
    private readonly ExpenseRepository _expenseRepo;

    public CategoryService(CategoryRepository repo, ExpenseRepository expenseRepo)
    {
        _repo = repo;
        _expenseRepo = expenseRepo;
    }

    /*
        To do:
        1. Agregar categoria
            - Validar que no existan nombres duplicados
        2. Editar categoria
            - Validar que no existan nombres duplicados
        3. Eliminar categoria
            - No permitir eliminar una categoria que tenga gastos asociados
    */

    public List<Category> GetAll() => _repo.GetAll();
    public Category? GetById(string id) => _repo.GetById(id);
    public bool ExistsByName(string name) => _repo.GetByName(name) != null;

    public decimal? GetReimainingBudget(string id)
    {
        Category? category = _repo.GetById(id);

        if (category == null)
        {
            return null;
        }

        decimal totalExpenses = _expenseRepo.GetAll().Where(c => c.CategoryId == id).Sum(c => c.Amount);

        return category.Budget - totalExpenses;
    }

    public (bool Success, string Message) Create(string name,  string description, decimal budget)
    {
        // Pasamos null en currentId porque es una creación
        var validation = Validate(name, budget, description, currentId: null);
        
        if (!validation.Success)
        {
            return (false, validation.Message);
        }

        Category category = new()
        {
            Name = name.Trim(),
            Budget = budget,
            Description = description.Trim()
        };

        bool created = _repo.Add(category);

        return created ? 
            (true, "Categoría creada correctamente.") :
            (false, "Error al guardar en base de datos.");
    }

    public (bool Success, string Message) Update(string id, string name, string description, decimal budget)
    {
        Category? category = _repo.GetById(id);

        if (category == null)
        {
            return (false, "La categoría no existe.");
        }

        var validation = Validate(name, budget, description, currentId: id);

        if (!validation.Success)
        {
            return (false, validation.Message);
        }

        // Actualizamos campos
        category.Name = name.Trim();
        category.Budget = budget;
        category.Description = description.Trim();

        bool updated = _repo.Update(category);
        
        return updated ? 
            (true, "Categoría actualizada correctamente.") :
            (false, "Error al guardar cambios.");
    }

    public (bool Success, string Message) Validate(string name, decimal budget, string? description, string? currentId)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return (false, "El nombre de la categoría es requerido.");
        }

        name = name.Trim();

        if (name.Length < 3)
        {
            return (false, "El nombre debe tener al menos 3 caracteres.");
        }

        // Lógica de duplicados
        Category? categoryWithSameName = _repo.GetByName(name);
        
        if (categoryWithSameName != null && (currentId == null || categoryWithSameName.Id != currentId))
        {
            // CurrentId == null -> Creando, error de inmediato.
            // CurrentId != null -> Actualizando, la categoria encontrada debe de tener un id diferente para considerarse duplicada.
            return (false, "Ya existe otra categoría con ese nombre.");
        }

        if (budget < 100)
        {
            return (false, "El presupuesto debe ser de mínimo RD$100.00.");
        }
            
        if (!string.IsNullOrEmpty(description) && description.Length < 20)
        {
            return (false, "La descripción debe tener al menos 20 caracteres.");
        }

        return (true, "Operación realizada correctamente.");
    }

    public (bool Success, string Message) Delete(string id)
    {
        Category? category = _repo.GetById(id);

        if (category == null)
        {
            return (false, "La categoría no existe.");
        }

        // Validar que no tenga gastos asociados
        if (_expenseRepo.HasExpensesForCategory(id))
        {
            return (false, $"No se puede eliminar la categoría '{category.Name}', tiene gastos registrados.");
        }

        bool deleted = _repo.Delete(id);

        return deleted ? 
            (true, $"Categoría '{category.Name}' eliminada correctamente.") :
            (false, $"La categoría '{category.Name}' no pudo ser eliminada.");
    }
}