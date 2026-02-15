using PersonalExpenses.Presentation.Views;

namespace PersonalExpenses.Presentation.Controllers;

public class HomeController : BaseController
{
    // private readonly ProductController _productController;
    // private readonly InvoiceController _invoiceController;

    public HomeController(HomeMenu homeMenu) : base(homeMenu)
    {
        
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
                Console.WriteLine("Categorías...");
                Console.ReadKey();


                break;
                
            case 1:
                Console.WriteLine("Gastos...");
                Console.ReadKey();

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