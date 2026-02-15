using PersonalExpenses.Presentation.Core;

namespace PersonalExpenses.Presentation.Views;

public class HomeMenu : IView
{
    public int Show()
    {
        return Menu.Show(
            "Gestor de Gastos Personales",
            [
                "Administrar categorías",
                "Administrar gastos"
            ]
        );
    }
}