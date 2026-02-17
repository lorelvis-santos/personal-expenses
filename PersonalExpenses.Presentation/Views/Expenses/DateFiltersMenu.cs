using PersonalExpenses.Presentation.Core;

namespace PersonalExpenses.Presentation.Views.Expenses;

public class DateFiltersMenu : IView
{
    public string Subtitle { get; set; } = "Selecciona el rango de fechas";
    public int Show()
    {
        return Menu.Show(
            "Gestor de Gastos Personales",
            [
                "Este mes",
                "Mes pasado",
                "Rango personalizado"
            ],
            Subtitle
        );
    }
}