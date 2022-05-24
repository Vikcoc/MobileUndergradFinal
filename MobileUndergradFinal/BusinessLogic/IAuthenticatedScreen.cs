using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogic
{
    public interface IAuthenticatedScreen : IErrorScreen
    {
        void SignOutAndMoveToLogin();
        string AccessToken { get;}
    }
}
