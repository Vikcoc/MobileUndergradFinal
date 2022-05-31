using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Communication.SourceContributionDto;
using Communication.SourcePlaceDto;

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
        decimal MapLeft { get; }
        decimal MapBot { get; }
        decimal MapRight { get; }
        decimal MapTop { get; }

        List<WaterSourcePlaceListingWithContributionDto> WaterPlaces { set; }
        void AddPlacePicture(Guid placeId, Stream image);
        void MoveToMap(Guid? placeId = null);
        Action OnMapPress { get; set; }
        Action OnSeeAllPlacesPress { get; set; }
        Action<Guid> OnPlaceSelected { get; set; }
    }
}