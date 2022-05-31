using Network;
using Network.Response;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Communication.SourceContributionDto;
using Communication.SourcePlaceDto;

namespace BusinessLogic.Dashboard
{
    public class DashboardLogic
    {
        private readonly IDashboardScreen _dashboardScreen;
        private readonly NetworkService _networkService;
        public DashboardLogic(IDashboardScreen dashboardScreen)
        {
            _dashboardScreen = dashboardScreen;
            _networkService = new NetworkService();


            _dashboardScreen.OnAddNewFountainPress = _dashboardScreen.MoveToAddNewFountain;
            _dashboardScreen.OnSignOutPress = _dashboardScreen.SignOutAndMoveToLogin;
            _dashboardScreen.OnScreenVisible = LoadScreenAsync;
            _dashboardScreen.OnMapPress = () => _dashboardScreen.MoveToMap();
            _dashboardScreen.OnSeeAllPlacesPress = () => _dashboardScreen.MoveToMap();
            _dashboardScreen.OnPlaceSelected = x => _dashboardScreen.MoveToMap(x);
        }

        public async Task LoadScreenAsync()
        {
            _networkService.BearerToken = _dashboardScreen.AccessToken;
            var res = await _networkService.GetAsync<string>(RequestPaths.UserName);

            switch (res.ErrorType)
            {
                case ErrorType.None:
                {
                    _dashboardScreen.Welcome = _dashboardScreen.WelcomeText + " " + res.Data;
                    break;
                }
                case ErrorType.Actionable:
                    if(res.Error == ErrorStrings.Unauthorized)
                    {
                        _dashboardScreen.SignOutAndMoveToLogin();
                        return;
                    }

                    _dashboardScreen.DisplayError(res.Error);
                    break;
                case ErrorType.NonActionable:
                    _dashboardScreen.DisplayError(res.Error);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var res3 = await _networkService.GetAsync<List<WaterSourceContributionWithPlaceDto>>(RequestPaths.MyContributions + "0/5");
            switch (res3.ErrorType)
            {
                case ErrorType.None:
                {
                    _dashboardScreen.WaterContributions = res3.Data;
                    foreach (var dto in res3.Data)
                        await GetImageForContributionAsync(dto.Id, dto.WaterSourcePlace.Picture);
                    break;
                }
                case ErrorType.Actionable:
                    if (res3.Error == ErrorStrings.Unauthorized)
                    {
                        _dashboardScreen.SignOutAndMoveToLogin();
                        return;
                    }

                    _dashboardScreen.DisplayError(res3.Error);
                    break;
                case ErrorType.NonActionable:
                    _dashboardScreen.DisplayError(res3.Error);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public async Task GetPlacesAroundMap()
        {
            var res2 = await _networkService.GetAsync<List<WaterSourcePlaceListingWithContributionDto>>
                (RequestPaths.AroundMeWithState + _dashboardScreen.MapLeft + "/" + _dashboardScreen.MapBot + "/" + _dashboardScreen.MapRight + "/" + _dashboardScreen.MapTop);
            switch (res2.ErrorType)
            {
                case ErrorType.None:
                {
                    _dashboardScreen.WaterPlaces = res2.Data;
                    foreach (var dto in res2.Data)
                        await GetImageForPlaceAsync(dto.Id, dto.Picture);
                    break;
                }
                case ErrorType.Actionable:
                    if (res2.Error == ErrorStrings.Unauthorized)
                    {
                        _dashboardScreen.SignOutAndMoveToLogin();
                        return;
                    }

                    _dashboardScreen.DisplayError(res2.Error);
                    break;
                case ErrorType.NonActionable:
                    _dashboardScreen.DisplayError(res2.Error);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async Task GetImageForPlaceAsync(Guid placeId, Guid imageId)
        {
            var res = await _networkService.GetAsync<Stream>(RequestPaths.Picture + imageId);
            switch (res.ErrorType)
            {
                case ErrorType.None:
                {
                    _dashboardScreen.AddPlacePicture(placeId, res.Data);
                    break;
                }
                case ErrorType.Actionable:
                    if (res.Error == ErrorStrings.Unauthorized)
                    {
                        _dashboardScreen.SignOutAndMoveToLogin();
                        return;
                    }
                    _dashboardScreen.DisplayError(res.Error);
                    break;
                case ErrorType.NonActionable:
                    _dashboardScreen.DisplayError(res.Error);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async Task GetImageForContributionAsync(Guid contributionId, Guid imageId)
        {
            var res = await _networkService.GetAsync<Stream>(RequestPaths.Picture + imageId);
            switch (res.ErrorType)
            {
                case ErrorType.None:
                {
                    _dashboardScreen.AddContributionPicture(contributionId, res.Data);
                    break;
                }
                case ErrorType.Actionable:
                    if (res.Error == ErrorStrings.Unauthorized)
                    {
                        _dashboardScreen.SignOutAndMoveToLogin();
                        return;
                    }
                    _dashboardScreen.DisplayError(res.Error);
                    break;
                case ErrorType.NonActionable:
                    _dashboardScreen.DisplayError(res.Error);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}