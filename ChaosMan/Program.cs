using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Globalization;
using System.Text;

namespace ChaosMan
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args">command line arguments</param>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new WindowedForm());

            //int iHandle = 0;

            //if (args.Length > 0)
            //{
            //    string arg = args[0].ToLower(CultureInfo.InvariantCulture).Trim().Substring(0, 2);
            //    switch (arg)
            //    {
            //        case "/p":  // preview
            //            if (args.Length == 2)
            //            {
            //                int.TryParse(args[1], out iHandle);
            //            }
            //            break;
            //        case "/s": // show
            //            break;
            //        case "/c":  // config
            //        default:
            //            Application.Run(new OptionsForm());
            //            return;
            //    }

            //    Application.Run(new ChaosManForm(iHandle));
            //}
            //else
            //{
            //    Application.Run(new OptionsForm());
            //}
        }
    }
}