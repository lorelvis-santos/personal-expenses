using PersonalExpenses.Entities;
using PersonalExpenses.Data.Repositories;
using PersonalExpenses.Data;

namespace PersonalExpenses.Business;

public class CategoryService
{
    private CategoryRepository _repo;

    public CategoryService(CategoryRepository repo)
    {
        _repo = repo;
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

    public Category? FindById(string id) => _repo.FindById(id);
    public bool ExistsByName(string name) => _repo.FindByName(name) != null;

    public (bool Success, string Message) Create(string name, decimal budget, string? description = null)
    {
        // Pasamos null en currentId porque es una creación
        var (Success, Message) = Validate(name, budget, description, currentId: null);
        
        if (!Success)
        {
            return (false, Message);
        }

        Category category = new()
        {
            Name = name.Trim(),
            Budget = budget,
            Description = description?.Trim()
        };

        bool created = _repo.Add(category);

        return created ? 
            (true, "Categoría creada correctamente.") :
            (false, "Error al guardar en base de datos.");
    }

    public (bool Success, string Message) Update(string id, string name, decimal budget, string? description = null)
    {
        Category? category = _repo.FindById(id);

        if (category == null)
        {
            return (false, "La categoría no existe.");
        }

        var (Success, Message) = Validate(name, budget, description, currentId: id);

        if (!Success)
        {
            return (false, Message);
        }

        // Actualizamos campos
        category.Name = name.Trim();
        category.Budget = budget;
        category.Description = description?.Trim();

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
        Category? categoryWithSameName = _repo.FindByName(name);
        
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

    
}