using System;
using System.Threading.Tasks;

namespace BusinessLogic.Dashboard
{
    public interface IDashboardScreen : IAuthenticatedScreen
    {
        void MoveToAddNewFountain();

        Action OnSignOutPress { get; set; }
        Action OnAddNewFountainPress { get; set; }
        Func<Task> OnScreenVisible { get; set; }

        string Welcome { set; }
        string WelcomeText { get; }
    }
}