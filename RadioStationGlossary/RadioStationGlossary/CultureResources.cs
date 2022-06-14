using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;         //
using System.Windows.Data;          //
using System.Windows;               //

namespace RadioStationGlossary
{
    class CultureResources
    {
        static readonly List<CultureInfo> cultures = new List<CultureInfo>();

        static CultureResources()
        {
            cultures.Add(CultureInfo.GetCultureInfo("en-US"));
            cultures.Add(CultureInfo.GetCultureInfo("ja-JP"));
        }

        public static IList<CultureInfo> Cultures
        {
            get { return cultures; }
        }

        public Properties.Resources GetResourceInstance()
        {
            return new Properties.Resources();
        }

        private static ObjectDataProvider provider;
        public static ObjectDataProvider ResourceProvider
        {
            get
            {
                if (provider == null && Application.Current != null)
                    provider = (ObjectDataProvider)Application.Current.FindResource("Resources");
                return provider;
            }
        }

        public static void ChangeCulture(CultureInfo culture)
        {
            Properties.Resources.Culture = culture;
            ResourceProvider.Refresh();
        }

        public static void ChangeCulture(string name)
        {
            Properties.Resources.Culture = CultureInfo.GetCultureInfo(name);
            ResourceProvider.Refresh();
        }

    }
}
