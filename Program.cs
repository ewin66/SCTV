using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SCTV
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                for (int x = 0; x < args.Length; x++)
                {
                    System.Diagnostics.EventLog.WriteEntry("SCTVargs", args[x].ToString());
                }

                Application.Run(new SCTV());
            }
            catch(Exception ex)
            {
                System.Diagnostics.EventLog.WriteEntry("SCTV", ex.Message);
            }
        }
    }
}