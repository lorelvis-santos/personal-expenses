using PersonalExpenses.Presentation.Core;

namespace PersonalExpenses.Presentation.Views.Expenses;

public class ExpenseSubMenu : IView
{
    public string Subtitle { get; set; } = "Selecciona una opción para el gasto";
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