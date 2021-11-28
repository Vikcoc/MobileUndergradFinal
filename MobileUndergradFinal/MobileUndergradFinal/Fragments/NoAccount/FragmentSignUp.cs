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
using BusinessLogic;

namespace MobileUndergradFinal.Fragments.NoAccount
{
    public class FragmentSignUp : SwappableFragment, ISignUpScreen
    {
        public FragmentSignUp() : base(Resource.Layout.sign_up)
        {
        }

        public override void OnBackPressed()
        {
            ((MainActivity)Activity).GoToSignIn();
        }
    }
}