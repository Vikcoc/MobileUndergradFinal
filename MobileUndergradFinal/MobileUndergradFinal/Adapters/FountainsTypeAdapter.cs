using Android.Views;
using AndroidX.RecyclerView.Widget;
using MobileUndergradFinal.Helper;
using System;

namespace MobileUndergradFinal.Adapters
{
    public class FountainsTypeAdapter : RecyclerView.Adapter
    {
        private Action _selectedItem;
        public FountainsTypeAdapter(Action selectedItem)
        {
            _selectedItem = selectedItem;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.contribution_short_layout, parent, false);
            view.Click += (sender, args) =>
            {
                _selectedItem?.Invoke();
            };
            return new CustomViewHolder(view);
        }

        public override int ItemCount => 30;
    }
}