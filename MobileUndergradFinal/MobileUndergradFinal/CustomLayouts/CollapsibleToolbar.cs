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
using Android.Util;
using AndroidX.ConstraintLayout.Motion.Widget;
using Google.Android.Material.AppBar;

namespace MobileUndergradFinal.CustomLayouts
{
    public class CollapsibleToolbar : MotionLayout, AppBarLayout.IOnOffsetChangedListener
    {
        public CollapsibleToolbar(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public CollapsibleToolbar(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
        }

        public void OnOffsetChanged(AppBarLayout appBarLayout, int verticalOffset)
        {
            this.Progress = (float) -verticalOffset / appBarLayout.TotalScrollRange;
        }

        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();
            (Parent as AppBarLayout)?.AddOnOffsetChangedListener(this);
        }
    }
}