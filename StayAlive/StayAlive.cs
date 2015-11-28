using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Timers;
using System.Threading;
using StayAlive.Properties;

namespace StayAlive
{
    public partial class StayAlive : Form
    {
        private static System.Timers.Timer aTimer;
        private IntPtr thisHandler;
        private IntPtr foregroundHandler;
        private int times = 0;
        delegate void SetTextCallback(string text);

        public StayAlive()
        {
            InitializeComponent();
            thisHandler = FindWindow(null, Resources.MainWindowTitle);
            this.NLO.MouseDoubleClick += new MouseEventHandler(this.notifyIcon1_MouseDoubleClick);

            this.Controls.Add(logBox);
            aTimer = new System.Timers.Timer(2000);
            aTimer.Elapsed += OnTimedEvent;
            aTimer.Enabled = true;
        }
        // Get a handle to an application window.
        [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        // Activate an application window.
        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("USER32.DLL")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.DLL", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern int GetWindowText(IntPtr hWnd, [Out] StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("User32.DLL", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern void LockWorkStation();
        [DllImport("User32.DLL", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.DLL", EntryPoint = "GetWindowLong")]
        public static extern int GetWindowLongA(int hwnd, int nIndex);
        [DllImport("user32.DLL", EntryPoint = "SetWindowLong")]
        public static extern int SetWindowLongA(int hwnd, int nIndex, int dwNewLong);

        [DllImport("user32.DLL", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool GetWindowInfo(IntPtr hWnd, out WINDOWINFO pwi);


        private void Form1_Load(object sender, EventArgs e)
        {
            Hide();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)//this code gets fired on every resize  
            {//so we check if the form was minimized  
                Hide();//hides the program on the taskbar  
                NLO.Visible = true;//shows our tray icon  
            }

        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            new Thread(new ThreadStart(this.ThreadProcSafe)).Start();
        }

        private void ThreadProcSafe()
        {
            Console.WriteLine("tick");
            IntPtr logOffHandle = FindWindow(null, Resources.TargetWindowTitle);
            if (logOffHandle == IntPtr.Zero)
            {
                /*StringBuilder sb = new StringBuilder(256);
                foregroundHandler = GetForegroundWindow();
                GetWindowText(foregroundHandler, sb, sb.Capacity);

                String title = sb.ToString();
                sb.Clear();
                GetClassName(foregroundHandler, sb, sb.Capacity);
                String className = sb.ToString();

                if (className.Equals("Shell_TrayWnd"))
                {
                    //LockWorkStation();
                    //ShowWindow(foregroundHandler, 0);
                    //Developex.ScreenLocker.ShowSystemModalDialog("Never Log off", this);
                    //this.AppendText(String.Format("\"log off\" window is not found {0} <------------>   HWND:{1}, title: {2}, className: {3}", DateTime.Now, foregroundHandler, title, className));
                }
                //if (className.Equals("DV2ControlHost"))
                //{
                //    ShowWindow(foregroundHandler, 0);
                //}
                this.AppendText(String.Format("\"log off\" window is not found {0} <------------>   HWND:{1}, title: {2}, className: {3}", DateTime.Now, foregroundHandler, title, className));
                //ShowWindow(FindWindow("Shell_TrayWnd", null), 5);
                //this.ShowDialog();
                 */
            }
            else
            {
                WINDOWINFO winInfo = new WINDOWINFO();
                GetWindowInfo(logOffHandle, out winInfo);

                this.AppendText(String.Format("Prevented from logging off {0} times at {1}", (++times), DateTime.Now));
                this.AppendText(String.Format("dwExStyle: {0}, dwStyle: {1}, atomWindowType: {2}", winInfo.dwExStyle, winInfo.dwStyle, winInfo.atomWindowType));
                SendKeys.SendWait("{ENTER}");
            }
        }

        private void AppendText(string text)
        {
            if (this.logBox.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(AppendText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.logBox.AppendText(text);
                this.logBox.AppendText(Environment.NewLine);

            }
        }



        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ShowDialog();//shows the program on taskbar  
            this.WindowState = FormWindowState.Normal;//undoes the minimized state of the form  
            NLO.Visible = false;//hides tray icon again  

        }

        private void logBox_TextChanged(object sender, EventArgs e)
        {

        }



    }
}
