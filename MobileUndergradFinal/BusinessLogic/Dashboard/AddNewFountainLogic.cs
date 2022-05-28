using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Communication.SourceContributionDto;
using Communication.SourcePlaceDto;
using Communication.SourceVariantDto;
using Network;
using Network.Response;

namespace BusinessLogic.Dashboard
{
    public class AddNewFountainLogic
    {
        private readonly IAddNewFountainScreen _addNewFountainScreen;
        private readonly NetworkService _networkService;

        public AddNewFountainLogic(IAddNewFountainScreen addNewFountainScreen)
        {
            _addNewFountainScreen = addNewFountainScreen;
            _networkService = new NetworkService();

            addNewFountainScreen.OnSubmitButtonPress += OnSubmitButtonPress;
        }

        public async Task RequestVariants()
        {
            _networkService.BearerToken = _addNewFountainScreen.AccessToken;
            var res = await _networkService.GetAsync<List<WaterSourceVariantDto>>(RequestPaths.WaterSourceVariant);

            switch (res.ErrorType)
            {
                case ErrorType.None:
                {
                    _addNewFountainScreen.SetWaterSourceVariants(res.Data);
                    foreach (var variantDto in res.Data)
                        await GetPictureForVariant(variantDto.Id);
                    break;
                }
                case ErrorType.Actionable:
                    if (res.Error == ErrorStrings.Unauthorized)
                    {
                        _addNewFountainScreen.SignOutAndMoveToLogin();
                        return;
                    }
                    _addNewFountainScreen.DisplayError(res.Error);
                    break;
                case ErrorType.NonActionable:
                    _addNewFountainScreen.DisplayError(res.Error);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async Task GetPictureForVariant(Guid variantId)
        {
            var res = await _networkService.GetAsync<Stream>(RequestPaths.Picture + variantId);
            switch (res.ErrorType)
            {
                case ErrorType.None:
                {
                    _addNewFountainScreen.AddPicture(variantId, res.Data);
                    break;
                }
                case ErrorType.Actionable:
                    if (res.Error == ErrorStrings.Unauthorized)
                    {
                        _addNewFountainScreen.SignOutAndMoveToLogin();
                        return;
                    }
                    _addNewFountainScreen.DisplayError(res.Error);
                    break;
                case ErrorType.NonActionable:
                    _addNewFountainScreen.DisplayError(res.Error);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async Task OnSubmitButtonPress()
        {
            var makeRequest = true;
            if (string.IsNullOrWhiteSpace(_addNewFountainScreen.Nickname))
            {
                makeRequest = false;
                _addNewFountainScreen.NicknameError = _addNewFountainScreen.NicknameErrorText;
            }
            if (!_addNewFountainScreen.Latitude.HasValue || !_addNewFountainScreen.Longitude.HasValue)
            {
                makeRequest = false;
                _addNewFountainScreen.MapError = _addNewFountainScreen.MapErrorText;
            }
            if (!_addNewFountainScreen.VariantId.HasValue)
            {
                makeRequest = false;
                _addNewFountainScreen.VariantError = _addNewFountainScreen.VariantErrorText;
            }

            var pictures = _addNewFountainScreen.Pictures;
            if (!pictures.Any())
            {
                makeRequest = false;
                _addNewFountainScreen.PicturesError = _addNewFountainScreen.PicturesErrorText;
            }

            if (makeRequest)
            {
                var pictureIds = new List<Guid>(pictures.Count);
                foreach (var picture in pictures)
                {
                    var pictureId = await _networkService.PostAsync<Guid>(RequestPaths.Picture, picture);
                    switch (pictureId.ErrorType)
                    {
                        case ErrorType.None:
                        {
                            pictureIds.Add(pictureId.Data);
                            break;
                        }
                        case ErrorType.Actionable:
                            if (pictureId.Error == ErrorStrings.Unauthorized)
                            {
                                _addNewFountainScreen.SignOutAndMoveToLogin();
                                return;
                            }

                            _addNewFountainScreen.DisplayError(pictureId.Error);
                            break;
                        case ErrorType.NonActionable:
                            _addNewFountainScreen.DisplayError(pictureId.Error);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                var res = await _networkService.PostAsync<WaterSourceContributionDto>(RequestPaths.AddPlace, new WaterSourcePlaceCreateDto
                {
                    Address = _addNewFountainScreen.Address,
                    Latitude = _addNewFountainScreen.Latitude.Value,
                    Longitude = _addNewFountainScreen.Longitude.Value,
                    Nickname = _addNewFountainScreen.Nickname,
                    Pictures = pictureIds,
                    WaterSourceVariantId = _addNewFountainScreen.VariantId.Value
                });

                switch (res.ErrorType)
                {
                    case ErrorType.None:
                    {
                        _addNewFountainScreen.DisplayError("Done");
                            break;
                    }
                    case ErrorType.Actionable:
                        if (res.Error == ErrorStrings.Unauthorized)
                        {
                            _addNewFountainScreen.SignOutAndMoveToLogin();
                            return;
                        }

                        _addNewFountainScreen.DisplayError(res.Error);
                        break;
                    case ErrorType.NonActionable:
                        _addNewFountainScreen.DisplayError(res.Error);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
