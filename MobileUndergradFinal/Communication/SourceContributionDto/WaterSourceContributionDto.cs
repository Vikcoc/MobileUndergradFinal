using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Communication.SourceContributionDto
{
    public class WaterSourceContributionDto
    {
        public Guid Id { get; set; }
        public Guid WaterUserId { get; set; }
        public Guid WaterSourcePlaceId { get; set; }
        public ContributionTypeDto ContributionType { get; set; }
        public string Details { get; set; }
        public Guid? RelatedContributionId { get; set; }
    }
}
