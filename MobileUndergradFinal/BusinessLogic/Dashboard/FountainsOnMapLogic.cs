using Communication.SourceContributionDto;
using Communication.SourcePlaceDto;
using Network;
using Network.Response;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BusinessLogic.Dashboard
{
    public class FountainsOnMapLogic
    {
        private readonly IFountainsOnMapScreen _screen;
        private readonly NetworkService _networkService;

        public FountainsOnMapLogic(IFountainsOnMapScreen screen)
        {
            _screen = screen;
            _networkService = new NetworkService();
        }

        public async Task GetPlacesAroundMap()
        {
            _networkService.BearerToken = _screen.AccessToken;
            decimal mapLeft, mapBot, mapRight, mapTop;
            if (_screen.MapLeft == 0 && _screen.MapBot == 0 && _screen.MapRight == 0 && _screen.MapTop == 0)
            {
                mapLeft = 25.940406036769225M;
                mapBot = 44.353416170044575M;
                mapRight = 26.22968228764095M;
                mapTop = 44.54220245174628M;
            }
            else
            {
                mapLeft = _screen.MapLeft;
                mapBot = _screen.MapBot;
                mapRight = _screen.MapRight;
                mapTop = _screen.MapTop;
            }
            var res2 = await _networkService.GetAsync<List<WaterSourcePlaceListingDto>>
                (RequestPaths.AroundMe + mapLeft + "/" + mapBot + "/" + mapRight + "/" + mapTop);
            switch (res2.ErrorType)
            {
                case ErrorType.None:
                    _screen.WaterPlaces = res2.Data;
                    return;
                case ErrorType.Actionable:
                    if (res2.Error == ErrorStrings.Unauthorized)
                    {
                        _screen.SignOutAndMoveToLogin();
                        return;
                    }

                    _screen.DisplayError(res2.Error);
                    return;
                case ErrorType.NonActionable:
                    _screen.DisplayError(res2.Error);
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public async Task GetPlace(Guid id)
        {
            _networkService.BearerToken = _screen.AccessToken;
            var res2 = await _networkService.GetAsync<WaterSourcePlaceListingDto>
                (RequestPaths.Place + id);
            switch (res2.ErrorType)
            {
                case ErrorType.None:
                    _screen.SelectedPlace = res2.Data;
                    await GetImageForSelectedPlaceAsync(res2.Data.Picture);
                    await GetContributionsForSelectedPlaceAsync(res2.Data.Id);
                    return;
                case ErrorType.Actionable:
                    if (res2.Error == ErrorStrings.Unauthorized)
                    {
                        _screen.SignOutAndMoveToLogin();
                        return;
                    }

                    _screen.DisplayError(res2.Error);
                    return;
                case ErrorType.NonActionable:
                    _screen.DisplayError(res2.Error);
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async Task GetContributionsForSelectedPlaceAsync(Guid placeId)
        {
            _networkService.BearerToken = _screen.AccessToken;
            var res2 = await _networkService.GetAsync< List<WaterSourceContributionDto>>
                (RequestPaths.ContributionsOfPlace + placeId + "/0/30");
            switch (res2.ErrorType)
            {
                case ErrorType.None:
                    _screen.SelectedContributions = res2.Data;
                    return;
                case ErrorType.Actionable:
                    if (res2.Error == ErrorStrings.Unauthorized)
                    {
                        _screen.SignOutAndMoveToLogin();
                        return;
                    }

                    _screen.DisplayError(res2.Error);
                    return;
                case ErrorType.NonActionable:
                    _screen.DisplayError(res2.Error);
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async Task GetImageForSelectedPlaceAsync(Guid imageId)
        {
            var res = await _networkService.GetAsync<Stream>(RequestPaths.Picture + imageId);
            switch (res.ErrorType)
            {
                case ErrorType.None:
                {
                    _screen.AddSelectedPlacePicture(res.Data);
                    break;
                }
                case ErrorType.Actionable:
                    if (res.Error == ErrorStrings.Unauthorized)
                    {
                        _screen.SignOutAndMoveToLogin();
                        return;
                    }
                    _screen.DisplayError(res.Error);
                    break;
                case ErrorType.NonActionable:
                    _screen.DisplayError(res.Error);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public async Task AddRightContributionAsync(ContributionTypeDto current, string comment, Guid placeId)
        {
            var res = await _networkService.PostAsync<WaterSourceContributionDto>(RequestPaths.Contribution,
                new WaterSourceContributionCreateDto
                {
                    ContributionType = current switch
                    {
                        ContributionTypeDto.Creation => ContributionTypeDto.CreateIncident,
                        ContributionTypeDto.CreateIncident => ContributionTypeDto.ConfirmIncident,
                        ContributionTypeDto.ConfirmIncident => ContributionTypeDto.InfirmIncident,
                        ContributionTypeDto.InfirmIncident => ContributionTypeDto.RemoveIncident,
                        ContributionTypeDto.RemoveIncident => ContributionTypeDto.CreateIncident,
                    },
                    Details = comment,
                    WaterSourcePlaceId = placeId,
                });
        }

        public async Task AddLeftContributionAsync(string comment, Guid placeId)
        {
            var res = await _networkService.PostAsync<WaterSourceContributionDto>(RequestPaths.Contribution,
                new WaterSourceContributionCreateDto
                {
                    ContributionType = ContributionTypeDto.InfirmIncident,
                    Details = comment,
                    WaterSourcePlaceId = placeId,
                });
        }
    }
}
