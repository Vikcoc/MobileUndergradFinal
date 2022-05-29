using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Communication.SourceContributionDto;

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

        List<WaterSourceContributionWithPlaceDto> WaterContributions { set; }
        void AddContributionPicture(Guid contributionId, Stream image);
    }
}