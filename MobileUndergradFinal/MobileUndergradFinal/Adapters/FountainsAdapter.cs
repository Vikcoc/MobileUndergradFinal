using Android.Views;
using AndroidX.RecyclerView.Widget;
using MobileUndergradFinal.Helper;

namespace MobileUndergradFinal.Adapters
{
    public class FountainsAdapter : RecyclerView.Adapter
    {
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.contribution_short_layout, parent, false);
            return new CustomViewHolder(view);
        }

        public override int ItemCount => 5;
    }
}