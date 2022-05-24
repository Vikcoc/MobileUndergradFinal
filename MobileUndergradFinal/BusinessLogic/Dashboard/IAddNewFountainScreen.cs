using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Communication.SourceVariantDto;

namespace BusinessLogic.Dashboard
{
    public interface IAddNewFountainScreen : IAuthenticatedScreen
    {
        Func<Task> OnScreenVisible { get; set; }
        void SetWaterSourceVariants(List<WaterSourceVariantDto> waterSources);
        Guid? SelectedVariant { get; }
    }
}
