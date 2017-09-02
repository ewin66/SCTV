using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SCTVFTP
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            SCTVFTPForm sctvFtp = new SCTVFTPForm();
            
            //make form visible
            sctvFtp.Opacity = 100;
            //sctvFtp.FTPServerIP = "electrodata/electrodata";
            sctvFtp.FTPServerIP = "electrodatallc.com/electrodata/NhAIk3PEgjcdEeIn2ZsvMrZ0FZuzb9TR0RBnjyQ4x68=/";
            sctvFtp.FTPUserID = "bob";
            sctvFtp.FTPPassword = "bob";

            Application.Run(sctvFtp);
        }
    }
}