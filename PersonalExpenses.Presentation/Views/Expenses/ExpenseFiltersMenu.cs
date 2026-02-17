using PersonalExpenses.Presentation.Core;

namespace PersonalExpenses.Presentation.Views.Expenses;

public class ExpenseFiltersMenu : IView
{
    public string Subtitle { get; set; } = "Selecciona el tipo de filtrado";
    public string[]? Tips { get; set; } = null;
    public int Show()
    {
        return Menu.Show(
            "Gestor de Gastos Personales",
            [
                "Por categoría",
                "Por rango de fechas"
            ],
            Subtitle,
            false,
            Tips
        );
    }
}