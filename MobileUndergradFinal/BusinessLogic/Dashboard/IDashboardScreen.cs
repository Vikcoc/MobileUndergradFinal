using System;

namespace BusinessLogic.Dashboard
{
    public interface IDashboardScreen : IAuthenticatedScreen
    {
        void MoveToAddNewFountain();

        Action OnSignOutPress { get; set; }
        Action OnAddNewFountainPress { get; set; }
    }
}