using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace StayAlive
{
    static class Program
    {
        static Guid g;
        static Mutex mutex = new Mutex(true, "{3F2504E0-4F89-41D3-9A0C-0305E82C3301}");
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new StayAlive());
                mutex.ReleaseMutex();
            }
        }


    }
}
