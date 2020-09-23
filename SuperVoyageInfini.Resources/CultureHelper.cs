using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperVoyageInfini.Resources
{
    public static class CultureHelper
    {
        public static bool Is(string twoLetterLang)
        {
            return System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == twoLetterLang;
        }

        public static string GetTwoLetterLang()
        {
            return System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        }

    }

}
