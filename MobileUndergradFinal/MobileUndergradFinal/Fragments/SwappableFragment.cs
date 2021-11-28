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
using Fragment = AndroidX.Fragment.App.Fragment;

namespace MobileUndergradFinal.Fragments
{
    public abstract class SwappableFragment : Fragment
    {
        protected SwappableFragment(int contentLayoutId) : base(contentLayoutId)
        {
        }

        public abstract void OnBackPressed();
    }
}