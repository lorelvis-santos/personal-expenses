namespace PersonalExpenses.Entities;

public class Expense : BaseModel
{
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public DateTime Date { get; set; }
    public required string CategoryId { get; set; }
}