using PersonalExpenses.Presentation.Core;

namespace PersonalExpenses.Presentation.Views;

public class CategorySubMenu : IView
{
    public string Subtitle { get; set; } = "Selecciona una opción para la categoría";
    public int Show()
    {
        return Menu.Show(
            "Gestor de Gastos Personales",
            [
                "Editar",
                "Eliminar"
            ],
            Subtitle
        );
    }
}