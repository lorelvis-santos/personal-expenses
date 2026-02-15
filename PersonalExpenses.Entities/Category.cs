namespace PersonalExpenses.Entities;

public class Category : BaseModel
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal Budget { get; set; }
}