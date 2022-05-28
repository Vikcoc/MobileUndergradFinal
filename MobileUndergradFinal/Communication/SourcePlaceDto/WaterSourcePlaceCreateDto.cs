using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Communication.SourcePlaceDto
{
    public class WaterSourcePlaceCreateDto
    {
        [MaxLength(50)]
        [Required]
        public string Nickname { get; set; }
        [MaxLength(200)]
        [Required]
        public string Address { get; set; }
        [Required]
        public decimal Latitude { get; set; }
        [Required]
        public decimal Longitude { get; set; }
        [Required]
        public Guid WaterSourceVariantId { get; set; }
        [Required]
        public List<Guid> Pictures { get; set; }
    }
}
