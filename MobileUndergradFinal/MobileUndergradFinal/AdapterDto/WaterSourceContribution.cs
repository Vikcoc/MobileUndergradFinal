using Communication.SourceContributionDto;
using System;

namespace MobileUndergradFinal.AdapterDto
{
    public class WaterSourceContribution
    {
        public Guid Id { get; set; }
        public Guid WaterUserId { get; set; }
        public Guid WaterSourcePlaceId { get; set; }
        public ContributionTypeDto ContributionType { get; set; }
        public string Details { get; set; }
        public Guid? RelatedContributionId { get; set; }
    }
}