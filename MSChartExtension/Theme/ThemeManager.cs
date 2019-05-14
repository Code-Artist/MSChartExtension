using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace System.Windows.Forms.DataVisualization.Charting
{
    internal static class ThemeManager
    {
        public static Dictionary<string, ThemeBase> GetThemes()
        {
            Dictionary<string, ThemeBase> themes = new Dictionary<string, ThemeBase>();

            foreach (Type item in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (!item.IsClass) continue;
                if (item.IsAbstract) continue;
                if (item.IsSubclassOf(typeof(ThemeBase)))
                {
                    ThemeBase t = Activator.CreateInstance(item) as ThemeBase;
                    themes.Add(t.Name, t);
                }
            }
            return themes;
        }
    }
}
