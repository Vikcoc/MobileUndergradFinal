using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Graphics;
using AndroidX.RecyclerView.Widget;

namespace MobileUndergradFinal.ItemDecorators
{
    public class FountainTypesDecorator : RecyclerView.ItemDecoration
    {
        private readonly int _topSpace;

        public FountainTypesDecorator(int topSpace)
        {
            _topSpace = topSpace;
        }

        public override void GetItemOffsets(Rect outRect, View view, RecyclerView parent, RecyclerView.State state)
        {
            outRect.Top = _topSpace;
            outRect.Bottom = _topSpace;
            outRect.Left = _topSpace;
            outRect.Right = _topSpace;
        }
    }
}