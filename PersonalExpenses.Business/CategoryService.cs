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
}