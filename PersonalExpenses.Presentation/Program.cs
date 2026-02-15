using PersonalExpenses.Business;
using PersonalExpenses.Data.Contexts;
using PersonalExpenses.Data.Repositories;
using PersonalExpenses.Presentation.Controllers;
using PersonalExpenses.Presentation.Views;

IFileContext context = new JsonFileContext();

CategoryRepository categoryRepository = new(context);
ExpenseRepository expenseRepository = new(context);

CategoryMenu categoryMenu = new();
CategorySubMenu categorySubMenu = new();
CategoryService categoryService = new(categoryRepository, expenseRepository);
CategoryController categoryController = new(categoryMenu, categorySubMenu, categoryService);

HomeMenu homeMenu = new();
HomeController homeController = new(homeMenu, categoryController);

bool homeLoop = true;

while (homeLoop)
{
    homeLoop = homeController.Execute();
}