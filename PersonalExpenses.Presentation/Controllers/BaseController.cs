using PersonalExpenses.Presentation.Core;

namespace PersonalExpenses.Presentation.Controllers;

public abstract class BaseController
{
    private readonly IView view;

    public BaseController(IView view)
    {
        this.view = view;
    }

    public bool Execute()
    {
        return HandleChoice(view.Show());
    }

    protected abstract bool HandleChoice(int choice);
}