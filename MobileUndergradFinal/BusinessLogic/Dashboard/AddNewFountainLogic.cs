using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            _addNewFountainScreen.OnScreenVisible = OnScreenVisible;
        }

        private async Task OnScreenVisible()
        {
            _networkService.BearerToken = _addNewFountainScreen.AccessToken;
            var res = await _networkService.GetAsync<List<WaterSourceVariantDto>>(RequestPaths.WaterSourceVariant);

            switch (res.ErrorType)
            {
                case ErrorType.None:
                {
                    _addNewFountainScreen.SetWaterSourceVariants(res.Data);
                    break;
                }
                case ErrorType.Actionable:
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
