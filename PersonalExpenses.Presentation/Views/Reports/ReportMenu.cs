using PersonalExpenses.Presentation.Core;

namespace PersonalExpenses.Presentation.Views.Reports;

public class ReportMenu : IView
{
    public int Show()
    {
        return Menu.Show(
            "Gestor de Gastos Personales",
            [
                "Ver resumen de todos los gastos",
                "Exportar resumen mensual"
            ],
            "Consulta reportes sobre tus gastos registrados"
        );
    }
}