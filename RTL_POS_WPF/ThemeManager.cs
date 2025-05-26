using System;
using System.Linq;
using System.Windows;

namespace RTL_POS_WPF
{
    public static class ThemeManager
    {
        public static void ApplyThemeToWindow(Window window, string themePath)
        {
            var dictionaries = window.Resources.MergedDictionaries;
            var existingTheme = dictionaries
                .FirstOrDefault(d => d.Source != null && d.Source.OriginalString.StartsWith("Themes/Styles-"));
            if (existingTheme != null)
                dictionaries.Remove(existingTheme);

            var newTheme = new ResourceDictionary { Source = new Uri(themePath, UriKind.Relative) };
            dictionaries.Add(newTheme);
        }
    }
}
