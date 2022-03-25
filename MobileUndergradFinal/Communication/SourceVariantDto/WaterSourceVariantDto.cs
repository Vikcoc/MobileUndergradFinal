using System;

namespace Communication.SourceVariantDto
{
    public class WaterSourceVariantDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid? Picture { get; set; }
    }
}
