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
    public class ContributionListingAdapter : RecyclerView.Adapter
    {
        private readonly List<WaterSourceContributionWithPlace> _contributions;

        public ContributionListingAdapter()
        {
            _contributions = new List<WaterSourceContributionWithPlace>();
        }


        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (holder.ItemViewType != 0)
                return;
            holder.ItemView.FindViewById<TextView>(Resource.Id.topText).Text = _contributions[position].WaterSourcePlace.Nickname;
            holder.ItemView.FindViewById<TextView>(Resource.Id.bottomText).Text = _contributions[position].WaterSourcePlace.Address;
            holder.ItemView.FindViewById<TextView>(Resource.Id.contributionType).Text = _contributions[position].ContributionType.ToString();
            switch (_contributions[position].ContributionType)
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

            if (_contributions[position].WaterSourcePlace.Picture != null)
                holder.ItemView.FindViewById<ImageView>(Resource.Id.placePicture).SetImageBitmap(_contributions[position].WaterSourcePlace.Picture);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            switch (viewType)
            {
                case 0:
                {
                    var view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.contribution_short_layout, parent, false);
                    return new CustomViewHolder(view);
                }
                default:
                {
                    var view = LayoutInflater.From(parent.Context)
                        .Inflate(Resource.Layout.no_items, parent, false);
                    var holder = new CustomViewHolder(view);
                    return holder;
                }
            }
        }

        public override int GetItemViewType(int position)
        {
            return position >= _contributions.Count ? 1 : 0;
        }

        public override int ItemCount => _contributions.Count == 0 ? 1 : _contributions.Count;

        public void AddItems(List<WaterSourceContributionWithPlace> items)
        {
            var current = ItemCount;
            _contributions.AddRange(items);
            if (current > 0)
                NotifyItemRangeInserted(current, items.Count);
            else
                NotifyDataSetChanged();
        }

        public void AddPicture(Guid variantId, Bitmap picture)
        {
            var index = _contributions.FindIndex(x => x.Id == variantId);
            if (index == -1)
                return;
            _contributions[index].WaterSourcePlace.Picture = picture;
            NotifyItemChanged(index);
        }
    }
}