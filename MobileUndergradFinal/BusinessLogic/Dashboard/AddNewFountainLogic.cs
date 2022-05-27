using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
