namespace PersonalExpenses.Entities;

public class Expense : BaseModel
{
    public decimal Amount { get; set; }
    public required string Description { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
    public required string CategoryId { get; set; }
}