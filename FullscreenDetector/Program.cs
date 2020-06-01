using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Automation;
using System.Runtime.CompilerServices;

namespace FullscreenDetector
{
    class Program
    {
        private static string processName;
        private static bool killedProcess;
        private static string processPath;
        private static Object syncObj = new Object();

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(HandleRef hWnd, [In, Out] ref RECT rect);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern void GetWindowThreadProcessId(IntPtr hWnd, out uint procId);

        public static bool IsForegroundFullScreen(string excludedProcess)
        {
            return IsForegroundFullScreen(null, excludedProcess);
        }

        public static bool IsForegroundFullScreen(System.Windows.Forms.Screen screen, String excludedProcess)
        {

            if (screen == null)
            {
                screen = System.Windows.Forms.Screen.PrimaryScreen;
            }
            RECT rect = new RECT();
            IntPtr hWnd = (IntPtr)GetForegroundWindow();


            GetWindowRect(new HandleRef(null, hWnd), ref rect);

            /* in case you want the process name:*/
            uint procId = 0;
            GetWindowThreadProcessId(hWnd, out procId);
            var proc = System.Diagnostics.Process.GetProcessById((int)procId);
            // check excluded processes
            if(proc.ProcessName.Equals(excludedProcess) || proc.ProcessName.Equals("explorer"))
            {
                return false;
            }

            if (screen.Bounds.Width == (rect.right - rect.left) && screen.Bounds.Height == (rect.bottom - rect.top))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static Process getForegroundProcess(Screen screen)
        {
            if (screen == null)
            {
                screen = System.Windows.Forms.Screen.PrimaryScreen;
            }

            RECT rect = new RECT();
            IntPtr hWnd = (IntPtr)GetForegroundWindow();


            GetWindowRect(new HandleRef(null, hWnd), ref rect);

            /* in case you want the process name:*/
            uint procId = 0;
            GetWindowThreadProcessId(hWnd, out procId);
            return Process.GetProcessById((int)procId);
        }

        private static void OnFocusChangedHandler(object src, AutomationFocusChangedEventArgs args)
        {
            Console.WriteLine("Focus changed!");
            /*AutomationElement element = src as AutomationElement;
            if (element != null)
            {
                string name = element.Current.Name;
                string id = element.Current.AutomationId;
                int processId = element.Current.ProcessId;
                using (Process process = Process.GetProcessById(processId))
                {
                    Console.WriteLine("  Name: {0}, Id: {1}, Process: {2}", name, id, process.ProcessName);
                }
            }*/

            CheckFullscreenApp();
        }

        private static void CheckFullscreenApp()
        {
            lock (syncObj)
            {
                if (IsForegroundFullScreen(processName))
                {
                    var processes = Process.GetProcesses();
                    for (int i = 0; i < processes.Length && !killedProcess; i++)
                    {
                        if (processes[i].ProcessName.Equals(processName))
                        {
                            Console.WriteLine("Process \"" + processes[i].ProcessName + "\" went into fullscreen.");
                            Console.WriteLine("Found process \"" + processName + "\". Killing it.");
                            processPath = processes[i].MainModule.FileName;
                            processes[i].Kill();
                            killedProcess = true;
                            break;
                        }
                    }
                    if (!killedProcess)
                    {
                        Console.WriteLine("Process could not be found!");
                    }
                }
                else
                {
                    if (killedProcess)
                    {
                        killedProcess = false;
                        Console.WriteLine("Starting \"" + processPath + "\" again");
                        Process.Start(processPath);
                    }
                }
            }
        }

        static int Main(string[] args)
        {
            if(args.Length < 1)
            {
                Console.WriteLine("Not enough arguments");
                return -1;
            }

            var proc = Process.GetProcessesByName("FullscreenDetector");
            if(proc.Length > 1)
            {
                Console.WriteLine("Process already running");
                return -1;
            }

            processName = args[0];
            processPath = "";
            killedProcess = false;

            Console.WriteLine("Process to kill and restart: " + processName);
            Console.WriteLine("Looking for fullsceen app.");

            Automation.AddAutomationFocusChangedEventHandler(OnFocusChangedHandler);

            while (true)
            {
                CheckFullscreenApp();
                Thread.Sleep(1000);
            }

        }
    }
}
