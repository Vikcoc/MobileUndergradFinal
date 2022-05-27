using Communication.SourceVariantDto;
using System;
using System.Collections.Generic;
using System.IO;

namespace BusinessLogic.Dashboard
{
    public interface IAddNewFountainScreen : IAuthenticatedScreen
    {
        void SetWaterSourceVariants(List<WaterSourceVariantDto> waterSources);
        Guid? SelectedVariant { get; }

        void AddPicture(Guid variantId, Stream picture);
    }
}
