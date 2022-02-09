using System;

namespace BusinessLogic.Startup
{
    public interface ISplashScreen
    {
        string AccessToken { get; }
        void MoveToDashboard();
        void MoveToLogin();

        Action AfterInitialization { get; set; }
    }
}
