using Android.Graphics;
using System;

namespace MobileUndergradFinal.AdapterDto
{
    public class WaterSourcePlaceListing
    {
        public Guid Id { get; set; }
        public string Nickname { get; set; }
        public string Address { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public Guid WaterSourceVariantId { get; set; }
        public Bitmap Picture { get; set; }
    }
}