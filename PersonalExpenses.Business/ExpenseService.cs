using PersonalExpenses.Entities;
using PersonalExpenses.Data.Repositories;

namespace PersonalExpenses.Business;

public class ExpenseService
{
    private ExpenseRepository _repo;
    private CategoryRepository _categoryRepo;

    public ExpenseService(ExpenseRepository repo, CategoryRepository categoryRepo)
    {
        _repo = repo;
        _categoryRepo = categoryRepo;
    }

    public List<Expense> GetAll() => _repo.GetAll();
    public List<Expense> GetAllReversed()
    {
        List<Expense> expenses = _repo.GetAll();
        expenses.Reverse();
        return expenses;
    }
    public List<Expense> FilterByCategories(List<string> categoriesId)
    {
        return GetAllReversed().FindAll(e => categoriesId.Contains(e.CategoryId));
    }

    public List<Expense> ApplyFilters(List<string> categoryIds, DateTime? from, DateTime? to)
    {
        List<Expense> query = GetAllReversed();

        if (categoryIds.Count > 0)
        {
            query = [.. query.Where(e => categoryIds.Contains(e.CategoryId))];
        }

        if (from.HasValue)
        {
            query = [.. query.Where(e => e.Date.Date >= from.Value.Date)];
        }

        if (to.HasValue)
        {
            query = [.. query.Where(e => e.Date.Date <= to.Value.Date)];
        }

        return query;
    }

    // 1. Para la VISTA GENERAL (Consola)
    // Retorna: (Total, Porcentajes por Categoría, Totales por Mes, Alertas)
    public (
        decimal TotalGeneral, 
        Dictionary<string, (decimal Total, double Percentage)> ByCategory, 
        Dictionary<string, decimal> ByMonth, List<string> BudgetAlerts
    ) GetGeneralSummary()
    {
        var expenses = GetAll();
        var categories = _categoryRepo.GetAll();

        decimal totalGeneral = expenses.Sum(e => e.Amount);
        
        // Agrupación por Categoría
        var byCategory = expenses
            .GroupBy(e => e.CategoryId)
            .ToDictionary(
                g => categories.FirstOrDefault(c => c.Id == g.Key)?.Name ?? "Desconocido",
                g => {
                    decimal catTotal = g.Sum(e => e.Amount);
                    // Evitar división por cero
                    double percentage = totalGeneral > 0 ? (double)(catTotal / totalGeneral * 100) : 0;
                    return (catTotal, percentage);
                }
            );

        // Agrupación por Mes/Año
        var byMonth = expenses
            .GroupBy(e => e.Date.ToString("yyyy-MM")) // Clave ej: "2026-02"
            .OrderByDescending(g => g.Key)
            .ToDictionary(g => g.Key, g => g.Sum(e => e.Amount));

        // Alertas de Presupuesto (Req. 6)
        var alerts = new List<string>();
        foreach (var cat in categories)
        {
            // Sumamos gastos de esta categoría
            decimal spent = expenses.Where(e => e.CategoryId == cat.Id && e.Date.Month == DateTime.Now.Month).Sum(e => e.Amount);
            if (spent > cat.Budget)
            {
                alerts.Add($"ALERTA: '{cat.Name}' excedió su presupuesto ({spent:C2} / {cat.Budget:C2})");
            }
        }

        return (totalGeneral, byCategory, byMonth, alerts);
    }

    // 2. Para EXPORTAR (JSON)
    public (bool Success, string Message) ExportMonthlyReport(int year, int month)
    {
        if (year < 2000 || year > DateTime.Now.Year)
        {
            return (false, "Año inválido, debe de ser entre el 2000 y el actual.");
        }

        if (month < 1 || month > 12)
        {
            return (false, "Mes inválido.");
        }

        var expenses = GetAll()
            .Where(e => e.Date.Year == year && e.Date.Month == month)
            .ToList();

        if (!expenses.Any())
        {
            return (false, "No hay gastos en el mes seleccionado.");
        }

        // Construimos el objeto del reporte de forma anónima
        var report = new
        {
            GeneratedDate = DateTime.Now,           // FechaGeneracion
            Period = $"{month}/{year}",             // Periodo
            MonthlyTotal = expenses.Sum(e => e.Amount), // TotalMes
            TransactionCount = expenses.Count,      // CantidadTransacciones
            CategoryBreakdown = expenses            // DetallePorCategoria
                .GroupBy(e => e.CategoryId)
                .Select(g => new {
                    Category = _categoryRepo.GetById(g.Key)?.Name ?? "Desconocida", // Categoria
                    Total = g.Sum(e => e.Amount)
                })
                .ToList()
        };

        string fileName = $"summary_{year}_{month:00}.json";
        
        // Usamos el método puente del repositorio
        if (_repo.SaveReport(report, fileName))
        {
            return (true, $"Reporte exportado exitosamente: {fileName}");
        }
        
        return (false, "Error al escribir el archivo.");
    }

    public (
        decimal Total,
        Dictionary<string, decimal> ByCategory,
        Dictionary<string, decimal> ByMonth
    ) GetStatistics(List<Expense> expenses, List<Category> categories)
    {
        decimal total = expenses.Sum(e => e.Amount);

        var byCategory = expenses
            .GroupBy(e => e.CategoryId)
            .ToDictionary(
                g => categories.FirstOrDefault(c => c.Id == g.Key)?.Name ?? "N/A", 
                g => g.Sum(e => e.Amount)
            );

        var byMonth = expenses
            .GroupBy(e => e.Date.ToString("MMMM yyyy")) // Ej: "Febrero 2026"
            .ToDictionary(g => g.Key, g => g.Sum(e => e.Amount));

        return (total, byCategory, byMonth);
    }

    public Expense? GetById(string id) => _repo.GetById(id);

    public (bool Success, string Message) Create(decimal amount, string description, string categoryId)
    {
        var validation = Validate(amount, description, categoryId);

        if (!validation.Success)
        {
            return (false, validation.Message);
        }

        Expense expense = new()
        {
            Amount = amount,
            Description = description,
            CategoryId = categoryId,
            Date = DateTime.Now
        };

        bool created = _repo.Add(expense);

        return created ? 
            (true, "Gasto agregado correctamente.") :
            (false, "Error al guardar en base de datos.");
    }

    public (bool Success, string Message) Update(string id, decimal amount, string description, string categoryId)
    {
        Expense? expense = _repo.GetById(id);

        if (expense == null)
        {
            return (false, "El gasto no existe.");
        }

        var validation = Validate(amount, description, categoryId);

        if (!validation.Success)
        {
            return (false, validation.Message);
        }

        expense.Amount = amount;
        expense.Description = description;
        expense.CategoryId = categoryId;

        bool updated = _repo.Update(expense);

        return updated ? 
            (true, "Gasto actualizado correctamente.") :
            (false, "Error al actualizar en base de datos.");
    }

    public (bool Success, string Message) Validate(decimal amount, string description, string categoryId)
    {
        if (amount <= 0)
        {
            return (false, "Monto inválido.");
        }

        Category? category = _categoryRepo.GetById(categoryId);

        if (category == null)
        {
            return (false, "La categoría proporcionada no existe.");
        }
            
        if (!string.IsNullOrEmpty(description) && description.Length < 3)
        {
            return (false, "La descripción debe tener al menos 3 caracteres.");
        }

        return (true, "Operación realizada correctamente.");
    }

    public (bool Success, string Message) Delete(string id)
    {
        Expense? expense = _repo.GetById(id);

        if (expense == null)
        {
            return (false, "La categoría no existe.");
        }

        bool deleted = _repo.Delete(id);

        return deleted ? 
            (true, $"Gasto eliminado correctamente.") :
            (false, $"El gasto no pudo ser eliminado.");
    }
}