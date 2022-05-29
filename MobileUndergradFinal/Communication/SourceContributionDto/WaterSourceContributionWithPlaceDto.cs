using Communication.SourcePlaceDto;
using System;
using System.ComponentModel.DataAnnotations;

namespace Communication.SourceContributionDto
{
    public class WaterSourceContributionWithPlaceDto
    {
        public Guid Id { get; set; }
        public Guid WaterUserId { get; set; }
        public Guid WaterSourcePlaceId { get; set; }
        public ContributionTypeDto ContributionType { get; set; }
        [MaxLength(200)]
        public string Details { get; set; }
        public Guid? RelatedContributionId { get; set; }
        public WaterSourcePlaceListingDto WaterSourcePlace { get; set; }
    }
}
