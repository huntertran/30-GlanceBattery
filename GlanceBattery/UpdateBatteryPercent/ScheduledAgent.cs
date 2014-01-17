#define DEBUG_AGENT
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using Windows.Phone.Devices.Power;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;

namespace UpdateBatteryPercent
{
    public class ScheduledAgent : ScheduledTaskAgent
    {

        private static int count = Battery.GetDefault().RemainingChargePercent;
        private static readonly string IconicTileQuery = "tile=iconic";

        /// <remarks>
        /// ScheduledAgent constructor, initializes the UnhandledException handler
        /// </remarks>
        static ScheduledAgent()
        {
            // Subscribe to the managed exception handler
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                Application.Current.UnhandledException += UnhandledException;
            });
        }

        /// Code to execute on Unhandled Exceptions
        private static void UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                Debugger.Break();
            }
        }

        /// <summary>
        /// Agent that runs a scheduled task
        /// </summary>
        /// <param name="task">
        /// The invoked task
        /// </param>
        /// <remarks>
        /// This method is called when a periodic or resource intensive task is invoked
        /// </remarks>
        protected override void OnInvoke(ScheduledTask task)
        {
            //TODO: Add code to perform your task in background
            UpdatePrimaryTile(count, "%");
            UpdateSecondaryTile(count);
            count++;

#if DEBUG_AGENT
            ScheduledActionService.LaunchForTest(task.Name, TimeSpan.FromSeconds(30));
#endif
            NotifyComplete();
        }

        public static void UpdateSecondaryTile(int count)
        {
            Uri tileUri = new Uri(string.Concat("/MainPage.xaml?", IconicTileQuery), UriKind.Relative);
            ShellTileData tileData = CreateIconicTileData();
            ShellTile iconicTile = FindTile(IconicTileQuery);
            if (iconicTile != null)
            {
                iconicTile.Update(tileData);
            }
        }

        public static void UpdatePrimaryTile(int count, string content)
        {
            //FlipTileData primaryTileData = new FlipTileData();
            //primaryTileData.Count = count;
            //primaryTileData.BackContent = content;


            ShellTileData tileData = CreateIconicTileData();
            ShellTile primaryTile = ShellTile.ActiveTiles.First();
            primaryTile.Update(tileData);
        }

        private static ShellTileData CreateIconicTileData()
        {
            IconicTileData iconicTileData = new IconicTileData();
            iconicTileData.Count = count;
            iconicTileData.IconImage = new Uri("/Assets/Icon/battery-medium.png", UriKind.Relative);
            iconicTileData.SmallIconImage = new Uri("/Assets/Icon/battery-medium.png", UriKind.Relative);
            iconicTileData.WideContent1 = "Your battery is at " + count + " %";
            iconicTileData.WideContent2 = "Time remaining: " + ToReadableString(Battery.GetDefault().RemainingDischargeTime);

            return iconicTileData;
        }

        private static ShellTile FindTile(string partOfUri)
        {
            ShellTile shellTile = ShellTile.ActiveTiles.FirstOrDefault(
            tile => tile.NavigationUri.ToString().Contains(partOfUri));

            return shellTile;
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