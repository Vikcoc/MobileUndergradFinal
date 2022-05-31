using System;
using System.Collections.Generic;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Communication.SourceContributionDto;
using MobileUndergradFinal.AdapterDto;
using MobileUndergradFinal.Helper;

namespace MobileUndergradFinal.Adapters
{
    public class FountainsAdapter : RecyclerView.Adapter
    {
        private readonly List<WaterSourcePlaceListingWithContribution> _places;
        private readonly Action<Guid> _selectedPlace;

        public FountainsAdapter(Action<Guid> selectedPlace)
        {
            _selectedPlace = selectedPlace;
            _places = new List<WaterSourcePlaceListingWithContribution>();
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (holder.ItemViewType != 0)
                return;
            holder.ItemView.FindViewById<TextView>(Resource.Id.topText).Text = _places[position].Nickname;
            holder.ItemView.FindViewById<TextView>(Resource.Id.bottomText).Text = _places[position].Address;
            holder.ItemView.FindViewById<TextView>(Resource.Id.contributionType).Text = _places[position].ToString();
            switch (_places[position].Contribution.ContributionType)
            {
                case ContributionTypeDto.Creation:
                    holder.ItemView.FindViewById<ImageView>(Resource.Id.contributionPicture).SetImageResource(Resource.Drawable.water_fountain_on_surface);
                    break;
                case ContributionTypeDto.CreateIncident:
                    holder.ItemView.FindViewById<ImageView>(Resource.Id.contributionPicture).SetImageResource(Resource.Drawable.water_fountain_warn);
                    break;
                case ContributionTypeDto.ConfirmIncident:
                    holder.ItemView.FindViewById<ImageView>(Resource.Id.contributionPicture).SetImageResource(Resource.Drawable.water_fountain_break);
                    break;
                case ContributionTypeDto.InfirmIncident:
                    holder.ItemView.FindViewById<ImageView>(Resource.Id.contributionPicture).SetImageResource(Resource.Drawable.water_fountain_break);
                    break;
                case ContributionTypeDto.RemoveIncident:
                    holder.ItemView.FindViewById<ImageView>(Resource.Id.contributionPicture).SetImageResource(Resource.Drawable.water_fountain_valid);
                    break;
                case ContributionTypeDto.Update:
                    holder.ItemView.FindViewById<ImageView>(Resource.Id.contributionPicture).SetImageResource(Resource.Drawable.water_fountain);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (_places[position].Picture != null)
                holder.ItemView.FindViewById<ImageView>(Resource.Id.placePicture).SetImageBitmap(_places[position].Picture);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            switch (viewType)
            {
                case 0:
                    {
                        var view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.contribution_short_layout, parent, false);
                        var holder = new CustomViewHolder(view);
                        view.Click += (sender, args) => _selectedPlace.Invoke(_places[holder.AbsoluteAdapterPosition].Id);
                        return holder;
                    }
                default:
                    {
                        var view = LayoutInflater.From(parent.Context)
                            .Inflate(Resource.Layout.fountain_type_layout, parent, false);
                        var holder = new CustomViewHolder(view);
                        return holder;
                    }
            }
        }

        public override int GetItemViewType(int position)
        {
            return position >= _places.Count ? 1 : 0;
        }

        public override int ItemCount => _places.Count == 0 ? 1 : _places.Count;

        public void AddItems(List<WaterSourcePlaceListingWithContribution> items)
        {
            var current = ItemCount;
            _places.AddRange(items);
            if (current > 0)
                NotifyItemRangeInserted(current, items.Count);
            else
                NotifyDataSetChanged();
        }

        public void AddPicture(Guid variantId, Bitmap picture)
        {
            var index = _places.FindIndex(x => x.Id == variantId);
            if (index == -1)
                return;
            _places[index].Picture = picture;
            NotifyItemChanged(index);
        }
    }
}