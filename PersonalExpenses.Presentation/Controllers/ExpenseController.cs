using PersonalExpenses.Entities;
using PersonalExpenses.Business;
using PersonalExpenses.Presentation.Views.Categories;
using PersonalExpenses.Presentation.Views.Expenses;
using PersonalExpenses.Presentation.Extensions;
using PersonalExpenses.Presentation.Enums;
using PersonalExpenses.Presentation.Models;

namespace PersonalExpenses.Presentation.Controllers;

public class ExpenseController : BaseController
{
    private readonly ExpenseMenu _menu;
    private readonly ExpenseSubMenu _subMenu;
    private readonly ExpenseFiltersMenu _expenseFiltersMenu;
    private readonly DateFiltersMenu _dateFiltersMenu;
    private readonly CategoryMenu _categoryMenu;
    private readonly ExpenseService _service;
    private readonly CategoryService _categoryService;
    private ExpenseFilterState _filterState = new();
    private List<Expense> _expenses;

    public ExpenseController(ExpenseMenu menu, ExpenseSubMenu subMenu, ExpenseFiltersMenu expenseFiltersMenu, DateFiltersMenu dateFiltersMenu, CategoryMenu categoryMenu, ExpenseService service, CategoryService categoryService) : base(menu)
    {
        _menu = menu;
        _categoryMenu = categoryMenu;
        _expenseFiltersMenu = expenseFiltersMenu;
        _dateFiltersMenu = dateFiltersMenu;
        _subMenu = subMenu;
        _service = service;
        _categoryService = categoryService;
        _expenses = [];
    }

    public new bool Execute()
    {
        List<Category> _categories = _categoryService.GetAll();
        Category? category;

        _expenses = _service.ApplyFilters(
            [.. _filterState.Categories.Select(c => c.Id)],
            _filterState.From,
            _filterState.To
        );

        List<string> data = [.. _expenses.Select(p => {
            category = _categories.FirstOrDefault(c => c.Id == p.CategoryId);
            return $"Categoría: {category?.Name ?? "N/A"} | Monto: RD{p.Amount:C2} | Fecha: {p.Date:d}" ?? "N/A";
        })];

        int rowsPerPage = 10;
        _menu.Pages = data.ToPages(rowsPerPage);
        _menu.RowsPerPage = rowsPerPage;

        _menu.Tips = [
            "Pulsa [I] para agregar un nuevo gasto",
            "Pulsa [F] para filtrar por categorías",
            "Pulsa [L] para limpiar los filtros"
        ];

        List<string> activeFiltersInfo = ["Filtros actuales"];

        if (_filterState.Categories.Count > 0)
        {
            // activeFiltersInfo.Add($"- Categorías: {_filterState.Categories.Count} seleccionadas");
            activeFiltersInfo.Add($"- Categorías: {string.Join(", ", _filterState.Categories.Select(c => c .Name))}");
        }

        if (_filterState.From.HasValue || _filterState.To.HasValue)
        {
            activeFiltersInfo.Add($"- Fechas: {_filterState.From?.ToShortDateString() ?? "*"} hasta {_filterState.To?.ToShortDateString() ?? "*"}");
        }

        if (_filterState.IsActive)
        {
            _menu.Tips = [.. activeFiltersInfo, "", .. _menu.Tips];
        }

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
            CreateExpense();
            return true;
        }

        if (specialKey == SpecialKeys.SetFilters)
        {
            SetFilters();
            return true;
        }

        if (specialKey == SpecialKeys.ClearFilters)
        {
            _filterState.Clear();
            return true;
        }

        Expense? selected = _expenses[choice];

        if (selected != null)
        {
            ExecuteSubMenu(selected);
        }

        return true;
    }

    private void ExecuteSubMenu(Expense expense)
    {
        int choice = _subMenu.Show();

        switch (choice)
        {
            case 0:
                EditExpense(expense);
                break;
            case 1:
                DeleteExpense(expense.Id);
                break;
            default:
                break;
        }
    }

    private bool CreateExpense()
    {
        Console.Clear();
        Console.WriteLine();
        Console.WriteLine("\tGestor de Gastos Personales");
        Console.WriteLine();
        Console.WriteLine("\tNuevo gasto");

        Category? category = SelectCategory();

        if (category == null)
        {
            return false;
        }
        
        Console.WriteLine();
        Console.WriteLine($"\tCategoría seleccionada >> {category.Name}");
        _ = decimal.TryParse(PromptInput("Monto"), out decimal amount);
        string description = PromptInput("Descripción");

        var result = _service.Create(amount, description, category.Id);

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

    private bool EditExpense(Expense expense)
    {
        Category? category = _categoryService.GetById(expense.CategoryId);

        if (category == null)
        {
            return false;
        }

        Console.Clear();
        Console.WriteLine();
        Console.WriteLine("\tPanel de administración");
        Console.WriteLine();
        Console.WriteLine($"\tEditando gasto");
        Console.WriteLine();
        Console.WriteLine("\tDatos actuales");
        Console.WriteLine();
        Console.WriteLine($"\tCategoría: {category.Name ?? "N/A"}");
        Console.WriteLine($"\tPresupuesto: RD{expense.Amount:C2}");
        Console.WriteLine($"\tDescripción: {expense.Description}");
        Console.WriteLine();
        Console.WriteLine("\tDeja vacío el campo que no quieras modificar");
        Console.WriteLine();

        Category? selectedCategory = SelectCategory(["Presiona [ESC] para mantener la categoría actual"]);

        if (selectedCategory != null)
        {
            category = selectedCategory;
        }
        
        Console.WriteLine();
        Console.WriteLine($"\tCategoría seleccionada >> {category.Name}");
        _ = decimal.TryParse(PromptInput("Monto", true), out decimal amount);
        string description = PromptInput("Descripción", true);

        var result = _service.Update(
            expense.Id,
            amount != 0 ? amount : expense.Amount,
            description != "" ? description : expense.Description,
            category.Id
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

    private bool DeleteExpense(string id)
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

    private bool SetFilters()
    {
        // Mostrar filtros actuales ; hay que colocarle Tips
        int choice = _expenseFiltersMenu.Show();

        switch (choice)
        {
            case -1:
                return false;
            case 0:
                SetCategoriesFilters();
                break;
            case 1:
                SetDateFilters();
                break;
        }

        return true;
    }

    private bool SetCategoriesFilters()
    {
        bool filtersLoop = true;
        List<string>? tips = null;

        while (filtersLoop)
        {
            if (_filterState.Categories.Count > 0)
            {
                tips = new(
                    [
                        "Filtros actuales",
                        .. _filterState.Categories.Select(c => $"- Nombre: {c.Name}"), 
                        "", 
                        "Presiona [ESC] para regresar"
                    ]
                );
            } else
            {
                tips = [];
            }

            Category? category = SelectCategory(tips?.ToArray() ?? null);

            if (category == null)
            {
                filtersLoop = false;
                break;
            }

            if (_filterState.Categories.FirstOrDefault(c => c.Id == category.Id) != null)
            {
                _filterState.Categories = [.. _filterState.Categories.Where(c => c.Id != category.Id)];
                continue;
            }

            _filterState.Categories.Add(category);
        }

        return true;
    }

    private bool SetDateFilters()
    {
        int choice = _dateFiltersMenu.Show();

        DateTime now = DateTime.Now;

        switch (choice)
        {
            case -1:
                return false;
            case 0:
                // Este mes
                _filterState.From = new DateTime(now.Year, now.Month, 1);
                _filterState.To = _filterState.From.Value.AddMonths(1).AddDays(-1);
                break;
            case 1: 
                // Mes pasado 
                var lastMonth = now.AddMonths(-1);
                _filterState.From = new DateTime(lastMonth.Year, lastMonth.Month, 1);
                _filterState.To = _filterState.From.Value.AddMonths(1).AddDays(-1);
                break;
            case 2:
                // Fecha especifica
                Console.WriteLine();
                _filterState.From = PromptDate("Fecha Inicio (dd/MM/yyyy)", true);
                _filterState.To = PromptDate("Fecha Fin (dd/MM/yyyy)", true);
                break;
        }

        return true;
    }

    private Category? SelectCategory(string[]? tips = null)
    {
        // Obtenemos las categorias
        List<Category> categories = _categoryService.GetAll();
        if (categories.Count == 0)
        {
            return null;
        }

        // Mostramos las categorias con paginacion
        List<string> data = [.. categories.Select(c => {
            decimal? remaining = _categoryService.GetReimainingBudget(c.Id);
            string remainingText = remaining > 0 ? $"Restante: RD{remaining:C2}" : $"ALERTA: Sobregirado por RD{remaining * -1:C2}";
            return $"Nombre: {c.Name} | Presupuesto: RD{c.Budget:C2} | {remainingText}";
        })];

        int rowsPerPage = 10;
        _categoryMenu.Pages = data.ToPages(rowsPerPage);
        _categoryMenu.RowsPerPage = rowsPerPage;
        _categoryMenu.Tips = tips != null && tips.Length > 0 ? tips : ["Presiona [ESC] para cancelar"];
        _categoryMenu.SpecialKeys = false;

        string originalSubtitle = _categoryMenu.Subtitle;
        _categoryMenu.Subtitle = "Selecciona una categoría";

        // Mostramos la vista
        int choice = _categoryMenu.Show();

        // Colocamos los valores originales del menu
        _categoryMenu.Subtitle = originalSubtitle;
        _categoryMenu.SpecialKeys = true;

        // Retornamos el objeto si la elección es válida
        if (choice == -1)
        {
            return null; // El usuario canceló
        }
        
        return categories[choice];
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

    // Helper para pedir fechas sin que explote la app
    private DateTime? PromptDate(string label, bool allowEmpty)
    {
        while (true)
        {
            string input = PromptInput(label, allowEmpty);

            if (string.IsNullOrWhiteSpace(input) && allowEmpty)
            {
                return null;
            }
            
            if (DateTime.TryParseExact(input, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime date))
            {
                return date;
            }

            Console.WriteLine("\t>> Formato inválido. Use dd/MM/yyyy");
        }
    }
    // ---
}