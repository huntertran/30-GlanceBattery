using KaraokeOnline.Settings;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;

namespace KaraokeOnline.PageGroups.RateAndReview
{
    public partial class RateAndReviewPage : PhoneApplicationPage
    {
        public RateAndReviewPage()
        {
            InitializeComponent();
        }

        private void rateAndReviewGrid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            MarketplaceReviewTask marketPlace = new MarketplaceReviewTask();
            marketPlace.Show();
            ApplicationSettings.SetSetting<bool>("hasReview", true, true);
            GoogleAnalytics.EasyTracker.GetTracker().SendSocial("MarketPlace", "rate", "MarketPlace");
        }

        private void feedBackGrid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            EmailComposeTask email = new EmailComposeTask();
            email.To = "cuoilennaocacban2@hotmail.com";
            email.Subject = "[Karaoke Online for Windows Phone] - User feedback";
            email.Body = "Hello.\r\nI want to tell you about ";
            email.Show();

            ApplicationSettings.SetSetting<bool>("hasReview", true, true);
        }
    }
}