using PersonalExpenses.Entities;
using PersonalExpenses.Business;
using PersonalExpenses.Presentation.Views.Categories;
using PersonalExpenses.Presentation.Extensions;
using PersonalExpenses.Presentation.Enums;

namespace PersonalExpenses.Presentation.Controllers;

public class CategoryController : BaseController
{
    private readonly CategoryMenu _menu;
    private readonly CategorySubMenu _subMenu;
    private readonly CategoryService _service;
    private List<Category> _categories;

    public CategoryController(CategoryMenu menu, CategorySubMenu subMenu, CategoryService service) : base(menu)
    {
        _menu = menu;
        _subMenu = subMenu;
        _service = service;
        _categories = [];
    }

    public new bool Execute()
    {
        _categories = _service.GetAll();

        List<string> data = [.. _categories.Select(c => {
            decimal? remaining = _service.GetReimainingBudget(c.Id);
            string remainingText;

            if (remaining > 0)
            {
                remainingText = $"Restante: RD{remaining:C2}";
            } else if (remaining == 0) 
            {
                remainingText = "ALERTA: El presupuesto llegó a su límite";
            } else
            {
                remainingText = $"ALERTA: Sobregirado por RD{remaining * -1:C2}";
            }

            return $"Nombre: {c.Name} | Presupuesto: RD{c.Budget:C2} | {remainingText}";
        })];

        int rowsPerPage = 10;
        _menu.Pages = data.ToPages(rowsPerPage);
        _menu.RowsPerPage = rowsPerPage;
        _menu.Tips = ["Pulsa [I] para agregar una nueva categoría"];

        return base.Execute();
    }

    protected override bool HandleChoice(int choice)
    {
        if (choice == -1)
        {
            return false;
        }

        var specialKey = (SpecialKeys)choice;

        // Caso especial: Insercion
        if (specialKey == SpecialKeys.Insert)
        {
            CreateCategory();
            return true;
        }

        Category? selected = _categories[choice];

        if (selected != null)
        {
            ExecuteSubMenu(selected);
        }

        return true;
    }

    private void ExecuteSubMenu(Category category)
    {
        _subMenu.Subtitle = $"Selecciona una opción para la categoría '{category.Name}'";
        int choice = _subMenu.Show();

        switch (choice)
        {
            case 0:
                EditCategory(category);
                break;
            case 1:
                DeleteCategory(category.Id);
                break;
            default:
                break;
        }
    }

    private bool CreateCategory()
    {
        Console.Clear();
        Console.WriteLine();
        Console.WriteLine("\tGestor de Gastos Personales");
        Console.WriteLine();
        Console.WriteLine("\tNueva categoría");
        Console.WriteLine();

        string name = PromptInput("Nombre");
        string description = PromptInput("Descripción");
        _ = decimal.TryParse(PromptInput("Presupuesto"), out decimal budget);

        var result = _service.Create(name, description, budget);

        if (!result.Success)
        {
            Console.WriteLine($"\n\tERROR: {result.Message}");
        } else
        {
            Console.WriteLine($"\n\t{result.Message}");
        }

        Console.ReadKey();

        return result.Success;
    }

    private bool EditCategory(Category category)
    {
        Console.Clear();
        Console.WriteLine();
        Console.WriteLine("\tPanel de administración");
        Console.WriteLine();
        Console.WriteLine($"\tEditando categoría '{category.Name}'");
        Console.WriteLine();
        Console.WriteLine("\tDatos actuales");
        Console.WriteLine();
        Console.WriteLine($"\tNombre: {category.Name}");
        Console.WriteLine($"\tDescripción: {category.Description}");
        Console.WriteLine($"\tPresupuesto: {category.Budget}");
        Console.WriteLine();
        Console.WriteLine("\tDeja vacío el campo que no quieras modificar");
        Console.WriteLine();
        string name = PromptInput("Nombre", true);
        string description = PromptInput("Descripción", true);
        _ = decimal.TryParse(PromptInput("Presupuesto", true), out decimal budget);

        var result = _service.Update(
            category.Id,
            name != "" ? name : category.Name,
            description != "" ? description : category.Description,
            budget != 0 ? budget : category.Budget
        );

        if (!result.Success)
        {
            Console.WriteLine($"\n\tERROR: {result.Message}");
        } else
        {
            Console.WriteLine($"\n\t{result.Message}");
        }

        Console.ReadKey();

        return result.Success;
    }

    private bool DeleteCategory(string id)
    {
        var result = _service.Delete(id);

        if (!result.Success)
        {
            Console.WriteLine($"\n\tERROR: {result.Message}");
        } else
        {
            Console.WriteLine($"\n\t{result.Message}");
        }

        Console.ReadKey();

        return result.Success;
    }

    // TODO: Tengo que mover esto a una clase helper
    public static string PromptInput(string message, bool optional = false)
    {
        string? input;

        do
        {
            Console.Write($"\t{message} >> ");
            input = Console.ReadLine();
        } while (!optional && string.IsNullOrWhiteSpace(input));

        return input?.Trim() ?? "";
    }
    // ---
}