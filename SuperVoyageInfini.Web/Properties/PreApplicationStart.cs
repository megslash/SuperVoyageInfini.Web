using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SuperVoyageInfini.Web.Properties
{
    public class PreApplicationStart
    {
        public static void InitializeApplication()
        {
            System.Web.WebPages.Razor.WebPageRazorHost.AddGlobalImport("SuperVoyageInfini.Resources");
        }

    }
}