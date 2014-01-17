//using KaraokeOnline.Data;
using System.IO.IsolatedStorage;
using System.Windows.Media;

namespace KaraokeOnline.Settings
{
    public static class ApplicationSettings
    {
        public static void SetSetting<T>(string key, T value, bool save = false)
        {
            IsolatedStorageSettings.ApplicationSettings[key] = value;
            if (save)
                IsolatedStorageSettings.ApplicationSettings.Save();
        }

        public static T GetSetting<T>(string key)
        {
            return GetSetting(key, default(T));
        }

        public static T GetSetting<T>(string key, T defaultValue)
        {
            return IsolatedStorageSettings.ApplicationSettings.Contains(key) &&
                    IsolatedStorageSettings.ApplicationSettings[key] is T
                ? (T)IsolatedStorageSettings.ApplicationSettings[key]
                : defaultValue;
        }

        public static bool HasSetting<T>(string key)
        {
            return IsolatedStorageSettings.ApplicationSettings.Contains(key) &&
                IsolatedStorageSettings.ApplicationSettings[key] is T;
        }

        public static bool RemoveSetting(string key)
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains(key))
            {
                var result = IsolatedStorageSettings.ApplicationSettings.Remove(key);
                if (result)
                    IsolatedStorageSettings.ApplicationSettings.Save();

                return result;
            }

            return false;
        }

        public static void SetSettingSafe<T>(string key, T value, bool save = false)
        {
            if (HasSetting<T>(key))
            {
                RemoveSetting(key);
            }
            ;
            SetSetting<T>(key, value, true);
        }
    }

    public class ColorSettingsHelper
    {
        public static void LyricColor(Color colorValue)
        {
            if (ApplicationSettings.HasSetting<string>("WordColor"))
            {
                ApplicationSettings.RemoveSetting("WordColor");
            }

            string colorString = colorValue.ToString();

            ApplicationSettings.SetSetting<string>("WordColor", colorString, true);

            //StaticData.wordColor = GetLyricColor();
        }

        public static Color GetLyricColor()
        {
            string colorString = "";

            if (ApplicationSettings.HasSetting<string>("WordColor"))
            {
                colorString = ApplicationSettings.GetSetting<string>("WordColor");
                Color newColor = Convert(colorString);
                return newColor;
            }
            else
            {
                //Default Value
                Color newColor = Colors.White;
                SetColor(newColor, "WordColor");
                return newColor;
            }
        }

        /// <summary>
        /// Set Color for approriate word color
        /// </summary>
        /// <param name="colorValue">Color to set</param>
        /// <param name="colorType">WordColor, MaleColor, FemaleColor or BothColor</param>
        public static void SetColor(Color colorValue, string colorType)
        {
            if (ApplicationSettings.HasSetting<string>(colorType))
            {
                ApplicationSettings.RemoveSetting(colorType);
            }

            string colorString = colorValue.ToString();

            ApplicationSettings.SetSetting<string>(colorType, colorString, true);
        }

        public static Color GetMaleColor()
        {
            string colorString = "";

            if (ApplicationSettings.HasSetting<string>("MaleColor"))
            {
                colorString = ApplicationSettings.GetSetting<string>("MaleColor");
                Color newColor = Convert(colorString);
                return newColor;
            }
            else
            {
                //Default Value
                Color newColor = Color.FromArgb(255, 0, 206, 209);
                SetColor(newColor, "MaleColor");
                return newColor;
            }
        }

        public static Color GetFemaleColor()
        {
            string colorString = "";

            if (ApplicationSettings.HasSetting<string>("FemaleColor"))
            {
                colorString = ApplicationSettings.GetSetting<string>("FemaleColor");
                Color newColor = Convert(colorString);
                return newColor;
            }
            else
            {
                //Default Value
                Color newColor = Color.FromArgb(255, 152, 251, 152);
                SetColor(newColor, "FemaleColor");
                return newColor;
            }
        }

        public static Color GetBothColor()
        {
            string colorString = "";

            if (ApplicationSettings.HasSetting<string>("BothColor"))
            {
                colorString = ApplicationSettings.GetSetting<string>("BothColor");
                Color newColor = Convert(colorString);
                return newColor;
            }
            else
            {
                //Default Value
                Color newColor = Color.FromArgb(255, 255, 192, 203);
                SetColor(newColor, "BothColor");
                return newColor;
            }
        }

        public static Color Convert(string hex)
        {
            byte alpha;
            byte pos = 0;

            hex = hex.TrimStart('#');

            if (hex.Length == 8)
            {
                alpha = System.Convert.ToByte(hex.Substring(pos, 2), 16);
                pos = 2;
            }
            else
            {
                alpha = System.Convert.ToByte("ff", 16);
            }

            byte red = System.Convert.ToByte(hex.Substring(pos, 2), 16);

            pos += 2;
            byte green = System.Convert.ToByte(hex.Substring(pos, 2), 16);

            pos += 2;
            byte blue = System.Convert.ToByte(hex.Substring(pos, 2), 16);

            return Color.FromArgb(alpha, red, green, blue);
        }
    }

    public enum LyricFontSize
    {
        Small,
        Medium,
        Large
    }

    public class FontSizeSettingHelper
    {
        public static double GetFontSize()
        {
            double result = 0;

            if (ApplicationSettings.HasSetting<double>("FontSize"))
            {
                result = ApplicationSettings.GetSetting<double>("FontSize");
                return result;
            }

            return 30;
        }

        public static void SetFontSize(LyricFontSize fontSize)
        {
            if (fontSize == LyricFontSize.Small)
            {
                if (ApplicationSettings.HasSetting<string>("FontSize"))
                {
                    ApplicationSettings.RemoveSetting("FontSize");
                }

                ApplicationSettings.SetSetting<double>("FontSize", 25, true);
            }

            if (fontSize == LyricFontSize.Medium)
            {
                if (ApplicationSettings.HasSetting<string>("FontSize"))
                {
                    ApplicationSettings.RemoveSetting("FontSize");
                }

                ApplicationSettings.SetSetting<double>("FontSize", 30, true);
            }

            if (fontSize == LyricFontSize.Large)
            {
                if (ApplicationSettings.HasSetting<string>("FontSize"))
                {
                    ApplicationSettings.RemoveSetting("FontSize");
                }

                ApplicationSettings.SetSetting<double>("FontSize", 35, true);
            }
        }
    }

    public class LanguageSettingHelper
    {
        public static string GetLanguage()
        {
            string result = "vi";

            if (ApplicationSettings.HasSetting<string>("Language"))
            {
                result = ApplicationSettings.GetSetting<string>("Language");
                return result;
            }
            else
            {
                ApplicationSettings.SetSettingSafe("Language", "vi", true);
            }

            return result;
        }

        public static void SetLanguage(string language)
        {
            ApplicationSettings.SetSettingSafe("Language", language, true);
        }
    }

    public class ShowVideoSettingHelper
    {
        public static bool IsShowVideo()
        {
            bool result = false;

            if (ApplicationSettings.HasSetting<bool>("ShowVideo"))
            {
                result = ApplicationSettings.GetSetting<bool>("ShowVideo");
                return result;
            }
            else
            {
                ApplicationSettings.SetSettingSafe("ShowVideo", false, true);
            }

            return result;
        }

        public static void SetShowVideo(bool value)
        {
            ApplicationSettings.SetSettingSafe("ShowVideo", value, true);
        }
    }

    public class AnimationSettingHelper
    {
        public static string GetLanguage()
        {
            string settingKey = "OrientaionAnimationMode";
            string result = "2";

            if (ApplicationSettings.HasSetting<string>(settingKey))
            {
                result = ApplicationSettings.GetSetting<string>(settingKey);
                return result;
            }
            else
            {
                ApplicationSettings.SetSettingSafe(settingKey, result, true);
            }

            return result;
        }

        public static void SetLanguage(string language)
        {
            string settingKey = "OrientaionAnimationMode";
            ApplicationSettings.SetSettingSafe(settingKey, language, true);
        }
    }
}
