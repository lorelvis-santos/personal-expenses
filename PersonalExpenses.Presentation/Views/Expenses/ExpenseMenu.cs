using PersonalExpenses.Presentation.Core;

namespace PersonalExpenses.Presentation.Views.Expenses;

public class ExpenseMenu : IView
{
    // Propiedades para guardar la configuración antes de llamar a Show()
    public string[][] Pages { get; set; } = [];
    public int RowsPerPage { get; set; } = 10;
    public string[]? Tips { get; set; } = null;
    public string Subtitle { get; set; } = "¡Estás administrando tus gastos!";

    public int Show()
    {
        return Menu.Show(
            "Gestor de Gastos Personales",
            Pages,
            1,
            RowsPerPage,
            true,
            Subtitle,
            Tips
        );
    }
}