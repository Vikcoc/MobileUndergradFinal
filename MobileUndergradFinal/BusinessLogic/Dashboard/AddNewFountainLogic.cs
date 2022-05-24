using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Communication.SourceVariantDto;
using Network;

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
            await _networkService.GetAsync<List<WaterSourceVariantDto>>(RequestPaths.WaterSourceVariant,
                x => 
                    _addNewFountainScreen.SetWaterSourceVariants(x),
                x => 
                    Debug.WriteLine(x.Aggregate((a, b) => a + b)));
        }
    }
}
