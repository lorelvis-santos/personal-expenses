using PersonalExpenses.Presentation.Views;

namespace PersonalExpenses.Presentation.Controllers;

public class HomeController : BaseController
{
    private readonly CategoryController _categoryController;
    private readonly ExpenseController _expenseController;
    private readonly ReportController _reportController;

    public HomeController(HomeMenu homeMenu, CategoryController categoryController, ExpenseController expenseController, ReportController reportController) : base(homeMenu)
    {
        _categoryController = categoryController;
        _expenseController = expenseController;
        _reportController = reportController;
    }

    protected override bool HandleChoice(int choice)
    {
        switch (choice)
        {
            case -1:
                Console.Clear();
                Console.WriteLine();
                Console.WriteLine("\tGestor de Gastos Personales");

                Console.WriteLine();
                Console.WriteLine("\t¿Estás seguro que deseas salir de la aplicación? (y/n)");

                Console.WriteLine();
                Console.Write("\tRespuesta >> ");

                string answer = Console.ReadLine()?.ToLower() ?? "n";

                if (answer == "y" || answer == "yes")
                    return false;

                break;

            case 0:
                bool categoryLoop = true;

                while (categoryLoop)
                {
                    categoryLoop = _categoryController.Execute();
                }

                break;
                
            case 1:
                bool expenseLoop = true;

                while (expenseLoop)
                {
                    expenseLoop = _expenseController.Execute();
                }

                break;
            case 2:
                bool reportLoop = true;

                while (reportLoop)
                {
                    reportLoop = _reportController.Execute();
                }

                break;

            default:
                Console.WriteLine();
                Console.WriteLine("\tError: has ingresado un número inválido.");
                Console.WriteLine();
                Console.Write("\tPresiona una tecla para reintentarlo...");
                Console.ReadKey();

                break;
        }

        return true;
    }
}