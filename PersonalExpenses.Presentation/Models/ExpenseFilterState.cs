using PersonalExpenses.Entities;

namespace PersonalExpenses.Presentation.Models;

public class ExpenseFilterState
{
    public List<Category> Categories { get; set; } = [];
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }

    public bool IsActive => Categories.Count > 0 || From.HasValue || To.HasValue;
    
    public void Clear()
    {
        Categories.Clear();
        From = null;
        To = null;
    }
}