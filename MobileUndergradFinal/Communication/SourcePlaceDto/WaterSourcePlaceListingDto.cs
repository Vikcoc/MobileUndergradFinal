using System;
using System.ComponentModel.DataAnnotations;

namespace Communication.SourcePlaceDto
{
    public class WaterSourcePlaceListingDto
    {
        public Guid Id { get; set; }
        public string Nickname { get; set; }
        public string Address { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public Guid WaterSourceVariantId { get; set; }
        public Guid Picture { get; set; }
    }
}
