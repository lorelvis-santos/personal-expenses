using PersonalExpenses.Business;
using PersonalExpenses.Data.Contexts;
using PersonalExpenses.Data.Repositories;
using PersonalExpenses.Presentation.Controllers;
using PersonalExpenses.Presentation.Views;
using PersonalExpenses.Presentation.Views.Categories;
using PersonalExpenses.Presentation.Views.Expenses;

IFileContext context = new JsonFileContext();

CategoryRepository categoryRepository = new(context);
ExpenseRepository expenseRepository = new(context);

CategoryMenu categoryMenu = new();
CategorySubMenu categorySubMenu = new();
CategoryService categoryService = new(categoryRepository, expenseRepository);
CategoryController categoryController = new(categoryMenu, categorySubMenu, categoryService);

ExpenseMenu expenseMenu = new();
ExpenseSubMenu expenseSubMenu = new();
ExpenseService expenseService = new(expenseRepository, categoryRepository);
ExpenseController expenseController = new(expenseMenu, expenseSubMenu, categoryMenu, expenseService, categoryService);

HomeMenu homeMenu = new();
HomeController homeController = new(homeMenu, categoryController, expenseController);

bool homeLoop = true;

while (homeLoop)
{
    homeLoop = homeController.Execute();
}