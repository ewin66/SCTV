using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;

namespace SCTV
{
    static class Program
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                bool createdNew = true;
                using (Mutex mutex = new Mutex(true, "CashRegister", out createdNew))
                {
                    if (createdNew)
                    {
                        AppDomain currentDomain = AppDomain.CurrentDomain;
                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);
                        
                        SCTVObjects.SplashScreenNew.ShowSplashScreen("start");

                        for (int x = 0; x < args.Length; x++)
                        {
                            System.Diagnostics.EventLog.WriteEntry("SCTVargs", args[x].ToString());
                        }

                        Application.Run(new SCTV());
                    }
                    else
                    {
                        Process current = Process.GetCurrentProcess();
                        foreach (Process process in Process.GetProcessesByName(current.ProcessName))
                        {
                            if (process.Id != current.Id)
                            {
                                SetForegroundWindow(process.MainWindowHandle);
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.EventLog.WriteEntry("SCTV", ex.Message);
            }

            
        }
    }
}