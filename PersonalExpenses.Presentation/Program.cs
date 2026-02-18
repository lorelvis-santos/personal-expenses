using PersonalExpenses.Business;
using PersonalExpenses.Data.Contexts;
using PersonalExpenses.Data.Repositories;
using PersonalExpenses.Presentation.Controllers;
using PersonalExpenses.Presentation.Views;
using PersonalExpenses.Presentation.Views.Categories;
using PersonalExpenses.Presentation.Views.Expenses;
using PersonalExpenses.Presentation.Views.Reports;

IFileContext context = new JsonFileContext();

CategoryRepository categoryRepository = new(context);
ExpenseRepository expenseRepository = new(context);

CategoryMenu categoryMenu = new();
CategorySubMenu categorySubMenu = new();
CategoryService categoryService = new(categoryRepository, expenseRepository);
CategoryController categoryController = new(categoryMenu, categorySubMenu, categoryService);

ExpenseMenu expenseMenu = new();
ExpenseSubMenu expenseSubMenu = new();
ExpenseFiltersMenu expenseFiltersMenu = new();
DateFiltersMenu dateFiltersMenu = new();
ExpenseService expenseService = new(expenseRepository, categoryRepository);
ExpenseController expenseController = new(expenseMenu, expenseSubMenu, expenseFiltersMenu, dateFiltersMenu, categoryMenu, expenseService, categoryService);

ReportMenu reportMenu = new();
ReportController reportController = new(reportMenu, expenseService);

HomeMenu homeMenu = new();
HomeController homeController = new(homeMenu, categoryController, expenseController, reportController);

// DESCOMENTAR ESTO UNA VEZ, EJECUTAR Y LUEGO COMENTAR/BORRAR
// DataSeeder.SeedExpenses("expenses.json");

bool homeLoop = true;

while (homeLoop)
{
    homeLoop = homeController.Execute();
}