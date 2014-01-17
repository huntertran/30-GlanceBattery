#define DEBUG_AGENT

using Windows.System;
using GlanceBattery.Data;
using GlanceBattery.Resources;
using KaraokeOnline.Settings;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Navigation;
using Windows.Phone.Devices.Power;
using Microsoft.Phone.Tasks;

namespace GlanceBattery
{
    public partial class MainPage : PhoneApplicationPage
    {

        PeriodicTask periodicTask { get; set; }

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();

            this.Loaded += OnLoaded;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //Cout how many time this app was openned
            CountOpen();
            //Common task: Ask if user want to download newer version
            SetupUI();
            //Update tiles
            StartPeriodicAgent();

            base.OnNavigatedTo(e);
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            BatteryPercentTextBlock.Text = Battery.GetDefault().RemainingChargePercent.ToString();
        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
        private void AboutMenuBarItem_OnClick(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/PageGroups/AboutGroup/about.xaml", UriKind.Relative));
        }

        private void RateAndReviewMenuBarItem_OnClick(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/PageGroups/RateAndReviewGroup/RateAndReviewPage.xaml", UriKind.Relative));
        }

        private static readonly string IconicTileQuery = "tile=iconic";
        public string Name = "Periodic Agent";

        #region Live Tiles
        private void ApplicationBarIconButton_OnClick(object sender, EventArgs e)
        {
            Uri tileUri = new Uri(string.Concat("/MainPage.xaml?", IconicTileQuery), UriKind.Relative);
            ShellTileData tileData = CreateIconicTileData();
            ShellTile iconicTile = this.FindTile(IconicTileQuery);
            if (iconicTile != null)
            {
                iconicTile.Delete();
                ShellTile.Create(tileUri, tileData, true);
                StartPeriodicAgent();
            }
            else
            {
                ShellTile.Create(tileUri, tileData, true);
                StartPeriodicAgent();
            }

            ShellTile primaryTile = ShellTile.ActiveTiles.FirstOrDefault();
            primaryTile.Update(tileData);
        }

        private ShellTileData CreateIconicTileData()
        {
            IconicTileData iconicTileData = new IconicTileData();
            iconicTileData.Count = 99;
            iconicTileData.IconImage = new Uri("/Assets/Icon/battery-medium.png", UriKind.Relative);
            iconicTileData.SmallIconImage = new Uri("/Assets/Icon/battery-medium.png", UriKind.Relative);
            iconicTileData.WideContent1 = "Your battery is at " + Battery.GetDefault().RemainingChargePercent +" %";
            iconicTileData.WideContent2 = "Time remaining: " + ToReadableString(Battery.GetDefault().RemainingDischargeTime);

            return iconicTileData;
        }

        private ShellTile FindTile(string partOfUri)
        {
            ShellTile shellTile = ShellTile.ActiveTiles.FirstOrDefault(
            tile => tile.NavigationUri.ToString().Contains(partOfUri));

            return shellTile;
        }

        public void StartPeriodicAgent()
        {
            periodicTask = ScheduledActionService.Find(Name) as PeriodicTask;

            if (periodicTask != null)
            {
                RemoveAgent();
            }


            periodicTask = new PeriodicTask(Name);
            periodicTask.Description = "this describes the periodic task";

            try
            {
                ScheduledActionService.Add(periodicTask);
                //LoadPeriodicTaskData();

#if(DEBUG_AGENT)
                ScheduledActionService.LaunchForTest(Name, TimeSpan.FromSeconds(30));
#endif
            }
            catch (Exception e) { }
        }

        public void RemoveAgent()
        {
            try
            {
                ScheduledActionService.Remove(Name);
                //EmptyPeriodicData();
            }
            catch (Exception e)
            {
            }
        }

        public static string ToReadableString(TimeSpan span)
        {
            string formatted = string.Format("{0}{1}{2}{3}",
                span.Duration().Days > 0
                    ? string.Format("{0:0} day{1}, ", span.Days, span.Days == 1 ? String.Empty : "s")
                    : string.Empty,
                span.Duration().Hours > 0
                    ? string.Format("{0:0} hour{1}, ", span.Hours, span.Hours == 1 ? String.Empty : "s")
                    : string.Empty,
                span.Duration().Minutes > 0
                    ? string.Format("{0:0} minute{1}, ", span.Minutes, span.Minutes == 1 ? String.Empty : "s")
                    : string.Empty,
                span.Duration().Seconds > 0
                    ? string.Format("{0:0} second{1}", span.Seconds, span.Seconds == 1 ? String.Empty : "s")
                    : string.Empty);

            if (formatted.EndsWith(", ")) formatted = formatted.Substring(0, formatted.Length - 2);

            if (string.IsNullOrEmpty(formatted)) formatted = "0 seconds";

            return formatted;
        }

        private async void LockscreenApplicationBarIconButton_OnClick(object sender, EventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings-lock:"));
        }

        #endregion

        #region Rate

        private void CountOpen()
        {
            StaticData.openCount = ApplicationSettings.GetSetting<int>("openCount", 0);
            StaticData.openCount++;
            ApplicationSettings.SetSetting<int>("openCount", StaticData.openCount, true);
        }

        private void SetupUI()
        {
            if (StaticData.EnableAppLink)
            {
                if (MessageBox.Show(AppResources.SplashScreen_SetupUI_New_version + StaticData.checkParameterData.latestVersion, AppResources.SplashScreen_SetupUI_Great_news, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    WebBrowserTask webBrowserTask = new WebBrowserTask();
                    webBrowserTask.Uri = new Uri(StaticData.checkParameterData.wpapplink);
                    webBrowserTask.Show();
                }
            }

            if (StaticData.openCount % 5 == 0 && ApplicationSettings.GetSetting<bool>("hasReview", false) == false)
            {
                if (MessageBox.Show(AppResources.SplashScreen_SetupUI_RateDetails, AppResources.SplashScreen_SetupUI_Rate,
                    MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    MarketplaceReviewTask marketPlace = new MarketplaceReviewTask();
                    marketPlace.Show();
                    ApplicationSettings.SetSetting<bool>("hasReview", true, true);
                    GoogleAnalytics.EasyTracker.GetTracker().SendSocial("MarketPlace", "rate", "MarketPlace");
                }
                else
                {
                    if (MessageBox.Show(AppResources.SplashScreen_SetupUI_FeedbackDetails, AppResources.SplashScreen_SetupUI_Feedback,
                    MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        EmailComposeTask email = new EmailComposeTask();
                        email.To = "cuoilennaocacban2@hotmail.com";
                        email.Subject = AppResources.SplashScreen_SetupUI__EmailHeader;
                        email.Body = AppResources.SplashScreen_SetupUI_EmailBody;
                        email.Show();

                        //ApplicationSettings.SetSetting<bool>("hasReview", true, true);
                    }
                    else
                    {
                        MessageBox.Show(AppResources.SplashScreen_SetupUI_FeedbackRemind);

                        //No, they didn't
                        //ApplicationSettings.SetSetting<bool>("hasReview", true, true);
                    }
                }
            }
        }

        #endregion

    }
}