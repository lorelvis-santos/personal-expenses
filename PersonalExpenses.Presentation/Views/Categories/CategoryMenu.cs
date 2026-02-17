using PersonalExpenses.Presentation.Core;

namespace PersonalExpenses.Presentation.Views.Categories;

public class CategoryMenu : IView
{
    // Propiedades para guardar la configuración antes de llamar a Show()
    public string[][] Pages { get; set; } = [];
    public int RowsPerPage { get; set; } = 10;
    public string[]? Tips { get; set; } = null;
    public bool SpecialKeys { get; set; } = true;
    public string Subtitle { get; set; } = "¡Estás administrando las categorías!";

    public int Show()
    {
        return Menu.Show(
            "Gestor de Gastos Personales",
            Pages,
            1,
            RowsPerPage,
            SpecialKeys,
            Subtitle,
            Tips
        );
    }
}