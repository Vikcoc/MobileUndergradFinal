using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Communication.SourceContributionDto;
using Communication.SourcePlaceDto;

namespace BusinessLogic.Dashboard
{
    public interface IFountainsOnMapScreen  : IAuthenticatedScreen
    {
        public List<WaterSourcePlaceListingDto> WaterPlaces { set; }

        decimal MapLeft { get; }
        decimal MapBot { get; }
        decimal MapRight { get; }
        decimal MapTop { get; }

        WaterSourcePlaceListingDto SelectedPlace { set; }
        List<WaterSourceContributionDto> SelectedContributions { set; }
        void AddSelectedPlacePicture(Stream image);
    }
}
