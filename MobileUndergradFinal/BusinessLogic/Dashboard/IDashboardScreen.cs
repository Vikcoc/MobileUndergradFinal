using System;

namespace BusinessLogic.Dashboard
{
    public interface IDashboardScreen
    {
        void SignOutAndMoveToLogin();
        void MoveToAddNewFountain();

        Action OnSignOutPress { get; set; }
        Action OnAddNewFountainPress { get; set; }
    }
}