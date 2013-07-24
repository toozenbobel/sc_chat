using Android.App;
using Android.OS;
using Cirrious.MvvmCross.Droid.Views;

namespace StreamChat.Droid.Views
{
    [Activity(Label = "Чаты стримов")]
    public class MainPageView : MvxActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.MainPageView);
        }
    }
}