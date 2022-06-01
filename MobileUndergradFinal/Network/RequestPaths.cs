using System;
using System.Collections.Generic;
using System.Text;

namespace Network
{
    public static class RequestPaths
    {
        public const string SignIn = "Account/sign_in";
        public const string SignUp = "Account/sign_up";
        public const string WaterSourceVariant = "WaterSourceVariant";
        public const string UserName = "Account/name";
        public const string Picture = "Media/";
        public const string AddPlace = "WaterSourcePlace";
        public const string MyContributions = "WaterSourceContribution/mine/";
        public const string AroundMeWithState = "WaterSourcePlace/get_in_rectangle_with_state/";
        public const string AroundMe = "WaterSourcePlace/get_in_rectangle/";
        public const string Place = "WaterSourcePlace/";
        public const string ContributionsOfPlace = "WaterSourceContribution/of_place/";
        public const string Contribution = "WaterSourceContribution";
    }
}
