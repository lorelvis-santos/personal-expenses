using PersonalExpenses.Business;
using PersonalExpenses.Presentation.Views.Reports;

namespace PersonalExpenses.Presentation.Controllers;

public class ReportController : BaseController
{
    private readonly ReportMenu _menu;
    private readonly ExpenseService _service;

    public ReportController(ReportMenu menu, ExpenseService service) : base(menu)
    {
        _menu = menu;
        _service = service;
    }

    protected override bool HandleChoice(int choice)
    {
        if (choice == -1)
        {
            return false;
        }

        switch (choice)
        {
            case 0:
                // Ver resumen detallado
                ShowSummary();
                break;
            case 1:
                // Exportar resumen de un mes en especifico
                ExportMonth();
                break;
        }

        return true;
    }

    private void ShowSummary()
    {
        Console.Clear();
        Console.WriteLine();
        Console.WriteLine("\tGestor de Gastos Personales");
        Console.WriteLine();
        Console.WriteLine("\t=== RESUMEN FINANCIERO ===");
        
        var stats = _service.GetGeneralSummary();

        // 1. Mostrar Totales
        Console.WriteLine($"\n\tTOTAL GASTADO (Histórico): {stats.TotalGeneral:C2}");
        
        // 2. Mostrar por Categoría
        Console.WriteLine("\n\t--- Desglose por Categoría ---");
        foreach (var item in stats.ByCategory)
        {
            Console.WriteLine($"\t{item.Key}: {item.Value.Total:C2} ({item.Value.Percentage:F2}%)");
        }

        // 3. Mostrar por Mes
        Console.WriteLine("\n\t--- Histórico Mensual ---");
        foreach (var item in stats.ByMonth)
        {
            Console.WriteLine($"\t{item.Key}: {item.Value:C2}");
        }

        // 4. Mostrar Alertas
        if (stats.BudgetAlerts.Any())
        {
            Console.WriteLine("\n\t[!] ALERTAS DE PRESUPUESTO DEL MES CORRIENTE [!]");
            foreach (var alert in stats.BudgetAlerts)
            {
                Console.WriteLine($"\t{alert}");
            }
        }

        Console.WriteLine("\n\t--------------------------------");
        Console.Write("\tPresiona cualquier tecla para volver.");
        Console.ReadKey();
    }

    private bool ExportMonth()
    {
        Console.Clear();
        Console.WriteLine();
        Console.WriteLine("\tGestor de Gastos Personales");
        Console.WriteLine();
        _ = int.TryParse(PromptInput("Año (ej. 2026)"), out int year);
        _ = int.TryParse(PromptInput("Mes (1-12)"), out int month);

        var result = _service.ExportMonthlyReport(year, month);
        
        if (!result.Success)
        {
            Console.WriteLine($"\n\tERROR: {result.Message}");
        } else
        {
            Console.WriteLine($"\n\t{result.Message}");
        }

        Console.ReadKey();

        return result.Success;
    }

    // TODO: Tengo que mover esto a una clase helper
    public static string PromptInput(string message, bool optional = false)
    {
        string? input;

        do
        {
            Console.Write($"\t{message} >> ");
            input = Console.ReadLine();
        } while (!optional && string.IsNullOrWhiteSpace(input));

        return input?.Trim() ?? "";
    }
    // ---
}