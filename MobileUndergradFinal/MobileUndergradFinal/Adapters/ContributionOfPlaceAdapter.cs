using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Communication.SourceContributionDto;
using MobileUndergradFinal.Helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MobileUndergradFinal.Adapters
{
    public class ContributionOfPlacesAdapter : RecyclerView.Adapter
    {
        private readonly List<WaterSourceContributionDto> _places;

        public ContributionOfPlacesAdapter()
        {
            _places = new List<WaterSourceContributionDto>();
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (holder.ItemViewType != 0)
                return;
            holder.ItemView.FindViewById<TextView>(Resource.Id.textView4).Text = _places[position].ContributionType.ToString();
            holder.ItemView.FindViewById<TextView>(Resource.Id.textView5).Text = _places[position].Details;
            switch (_places[position].ContributionType)
            {
                case ContributionTypeDto.Creation:
                    holder.ItemView.FindViewById<ImageView>(Resource.Id.imageView2).SetImageResource(Resource.Drawable.water_fountain_on_surface);
                    break;
                case ContributionTypeDto.CreateIncident:
                    holder.ItemView.FindViewById<ImageView>(Resource.Id.imageView2).SetImageResource(Resource.Drawable.water_fountain_warn);
                    break;
                case ContributionTypeDto.ConfirmIncident:
                    holder.ItemView.FindViewById<ImageView>(Resource.Id.imageView2).SetImageResource(Resource.Drawable.water_fountain_break);
                    break;
                case ContributionTypeDto.InfirmIncident:
                    holder.ItemView.FindViewById<ImageView>(Resource.Id.imageView2).SetImageResource(Resource.Drawable.water_fountain_break);
                    break;
                case ContributionTypeDto.RemoveIncident:
                    holder.ItemView.FindViewById<ImageView>(Resource.Id.imageView2).SetImageResource(Resource.Drawable.water_fountain_valid);
                    break;
                case ContributionTypeDto.Update:
                    holder.ItemView.FindViewById<ImageView>(Resource.Id.imageView2).SetImageResource(Resource.Drawable.water_fountain);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
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
                        return holder;
                    }
                default:
                    {
                        var view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.no_items, parent, false);
                        return new CustomViewHolder(view);
                    }
            }
        }

        public override int GetItemViewType(int position)
        {
            return position >= _places.Count ? 1 : 0;
        }

        public override int ItemCount => _places.Count == 0 ? 1 : _places.Count;

        public void SetItems(List<WaterSourceContributionDto> items)
        {
            _places.Clear();
            _places.AddRange(items);
            NotifyDataSetChanged();
        }

        public ContributionTypeDto GetLastType()
        {
            return _places.First(x => x.ContributionType != ContributionTypeDto.Update).ContributionType;
        }
    }
}