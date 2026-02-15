// using PersonalExpenses.Data.Contexts;
using PersonalExpenses.Presentation.Controllers;
using PersonalExpenses.Presentation.Views;

// IFileContext context = new JsonFileContext();

HomeMenu homeMenu = new();
HomeController homeController = new(homeMenu);

bool homeLoop = true;

while (homeLoop)
{
    homeLoop = homeController.Execute();
}