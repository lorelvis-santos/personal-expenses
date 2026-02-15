using PersonalExpenses.Data.Contexts;
using PersonalExpenses.Entities;

namespace PersonalExpenses.Data.Repositories;

public class ExpenseRepository : BaseRepository<Expense>
{
    public ExpenseRepository(IFileContext context) : base(context, "expenses.json")
    {
        
    }

    public bool HasExpensesForCategory(string categoryId)
    {
        return GetAll().Any(expense => expense.CategoryId == categoryId);
    }
}