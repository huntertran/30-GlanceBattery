#define DEBUG_AGENT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Windows.Phone.Devices.Power;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using GlanceBattery.Resources;

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
    }
}