using System.Collections.Generic;
using Android.Graphics;
using Android.Net;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using MobileUndergradFinal.Helper;

namespace MobileUndergradFinal.Adapters
{
    public class PlacePictureAdapter : RecyclerView.Adapter
    {
        private readonly List<(Uri, Bitmap)> _pictures;

        public PlacePictureAdapter()
        {
            _pictures = new List<(Uri, Bitmap)>();
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var img = holder.ItemView.FindViewById<ImageView>(Resource.Id.imageView1);
            img.SetImageBitmap(_pictures[position].Item2);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var view = LayoutInflater.From(parent.Context)
                .Inflate(Resource.Layout.add_new_place_picture, parent, false);
            var holder = new CustomViewHolder(view);
            view.Click += (sender, args) => { 
                _pictures.RemoveAt(holder.AbsoluteAdapterPosition);
                NotifyItemRemoved(holder.AbsoluteAdapterPosition);
            };
            return holder;
        }

        public override int ItemCount => _pictures.Count;

        public void AddImages(List<(Uri, Bitmap)> pictures)
        {
            _pictures.AddRange(pictures);
            NotifyItemRangeInserted(ItemCount - pictures.Count, pictures.Count);
        }
    }
}