using System;
using Android.Graphics;

namespace MobileUndergradFinal.AdapterDto
{
    public class WaterSourcePlaceListingWithContribution
    {
        public Guid Id { get; set; }
        public string Nickname { get; set; }
        public string Address { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public Bitmap Picture { get; set; }
        public WaterSourceContribution Contribution { get; set; }
    }
}