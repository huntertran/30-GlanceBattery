using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlanceBattery.Data
{
    public class StaticData
    {
        public static int openCount = 0;
        public static CheckParameterData checkParameterData = new CheckParameterData();
        public static string appVersion;
        public static bool EnableAppLink = false;
    }

    public class CheckParameterData
    {
        public string enableSync { get; set; }
        public string wpapplink { get; set; }
        public string androidapplink { get; set; }
        public string iosapplink { get; set; }
        public string notetouser { get; set; }
        public string bloglink { get; set; }
        public string w8applink { get; set; }
        public string latestVersion { get; set; }
        public string adMode { get; set; }
    }
}
