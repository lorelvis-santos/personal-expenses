namespace PersonalExpenses.Entities;

public class Category : BaseModel
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public decimal Budget { get; set; }
}