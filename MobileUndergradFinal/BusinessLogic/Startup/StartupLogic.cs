using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogic.Startup
{
    public class StartupLogic
    {
        private readonly ISplashScreen _splash;

        public StartupLogic(ISplashScreen splash)
        {
            _splash = splash;

            splash.AfterInitialization = AfterInitialization;
        }

        private void AfterInitialization()
        {
            if(string.IsNullOrWhiteSpace(_splash.AccessToken))
                _splash.MoveToLogin();
            else
                _splash.MoveToDashboard();
        }
    }
}
