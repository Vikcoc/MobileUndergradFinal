using Communication.SourceContributionDto;
using System;

namespace MobileUndergradFinal.AdapterDto
{
    public class WaterSourceContributionWithPlace
    {
        public Guid Id { get; set; }
        public Guid WaterUserId { get; set; }
        public Guid WaterSourcePlaceId { get; set; }
        public ContributionTypeDto ContributionType { get; set; }
        public string Details { get; set; }
        public Guid? RelatedContributionId { get; set; }
        public WaterSourcePlaceListing WaterSourcePlace { get; set; }
    }
}