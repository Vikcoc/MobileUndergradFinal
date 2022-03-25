using Android.Views;
using AndroidX.RecyclerView.Widget;
using MobileUndergradFinal.Helper;
using System;
using System.Collections.Generic;
using Android.Widget;
using Communication.SourceVariantDto;
using MobileUndergradFinal.AdapterDto;

namespace MobileUndergradFinal.Adapters
{
    public class FountainsTypeAdapter : RecyclerView.Adapter
    {
        private readonly Action<WaterSourceVariant> _selectedItem;
        private readonly List<WaterSourceVariant> _water;

        public FountainsTypeAdapter(Action<WaterSourceVariant> selectedItem)
        {
            _selectedItem = selectedItem;
            _water = new List<WaterSourceVariant>();
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if(holder.ItemViewType != 0)
                return;
            var topText = holder.ItemView.FindViewById<TextView>(Resource.Id.textView4);
            topText.Text = _water[position].Name;
            var bottomText = holder.ItemView.FindViewById<TextView>(Resource.Id.textView5);
            bottomText.Text = _water[position].Description;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            switch (viewType)
            {
                case 0:
                {
                    var view = LayoutInflater.From(parent.Context)
                        .Inflate(Resource.Layout.fountain_type_layout, parent, false);
                    var holder = new CustomViewHolder(view);
                    view.Click += (sender, args) => { _selectedItem?.Invoke(_water[holder.AbsoluteAdapterPosition]); };
                    return holder;
                }
                default:
                {
                    var view = LayoutInflater.From(parent.Context)
                        .Inflate(Resource.Layout.contribution_short_layout, parent, false);
                    var holder = new CustomViewHolder(view);
                    return holder;
                }
            }
        }

        public override int GetItemViewType(int position)
        {
            return position >= _water.Count ? 1 : 0;
        }

        public override int ItemCount => _water.Count == 0 ? 1 : _water.Count;

        public void AddItems(List<WaterSourceVariant> items)
        {
            var current = ItemCount;
            _water.AddRange(items);
            if(current > 0)
                NotifyItemRangeInserted(current, items.Count);
            else
                NotifyDataSetChanged();
        }
    }
}