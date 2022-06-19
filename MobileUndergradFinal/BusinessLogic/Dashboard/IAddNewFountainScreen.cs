using Communication.SourceVariantDto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BusinessLogic.Dashboard
{
    public interface IAddNewFountainScreen : IAuthenticatedScreen
    {
        void SetWaterSourceVariants(List<WaterSourceVariantDto> waterSources);
        void AddPicture(Guid variantId, Stream picture);

        string Nickname { get; }
        string Address { get; }
        decimal? Latitude { get; }
        decimal? Longitude { get; }
        Guid? VariantId { get; }
        List<Stream> Pictures { get; }

        string NicknameError { set; }
        string MapError { set; }
        string VariantError { set; }
        string PicturesError { set; }
        Func<Task> OnSubmitButtonPress { get; set; }

        string NicknameErrorText { get; }
        string MapErrorText { get; }
        string VariantErrorText { get; }
        string PicturesErrorText { get; }
        void GoBack();
    }
}
