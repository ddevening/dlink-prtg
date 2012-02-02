using System;

//http://code.google.com/p/prtg-addons/wiki/PTF_Custom_Sensors
namespace dlink_prtg
{
    class Program
    ﻿{﻿
        [STAThread]
        public static void Main(string[] args)
        {
            ScrapeDLinkPage scrape = new ScrapeDLinkPage(args);
        }
    }
}