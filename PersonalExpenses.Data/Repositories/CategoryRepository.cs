using PersonalExpenses.Data.Contexts;
using PersonalExpenses.Entities;

namespace PersonalExpenses.Data.Repositories;

public class CategoryRepository : BaseRepository<Category>
{
    public CategoryRepository(IFileContext context) : base(context, "categories.json")
    {
        
    } 
}