using System;
using System.ComponentModel.DataAnnotations;

namespace Communication.SourceContributionDto
{
    public class WaterSourceContributionCreateDto
    {
        public Guid WaterSourcePlaceId { get; set; }
        public ContributionTypeDto ContributionType { get; set; }
        [MaxLength(200)]
        public string Details { get; set; }
        public Guid? RelatedContributionId { get; set; }
    }
}
