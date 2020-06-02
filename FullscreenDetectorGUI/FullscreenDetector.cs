using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Automation;
using System.Windows.Forms;

namespace FullscreenDetector
{
    public class FullscreenDetector
    {
        public static string processName = "";
        private static bool killedProcess = false;
        private static string processPath = "";
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
            IntPtr hWnd = (IntPtr) GetForegroundWindow();


            GetWindowRect(new HandleRef(null, hWnd), ref rect);

            /* in case you want the process name:*/
            uint procId = 0;
            GetWindowThreadProcessId(hWnd, out procId);
            var proc = System.Diagnostics.Process.GetProcessById((int) procId);
            // check excluded processes
            if (proc.ProcessName.Equals(excludedProcess) || proc.ProcessName.Equals("explorer"))
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
            IntPtr hWnd = (IntPtr) GetForegroundWindow();


            GetWindowRect(new HandleRef(null, hWnd), ref rect);

            /* in case you want the process name:*/
            uint procId = 0;
            GetWindowThreadProcessId(hWnd, out procId);
            return Process.GetProcessById((int) procId);
        }

        public static void OnFocusChangedHandler(object src, AutomationFocusChangedEventArgs args)
        {
            Console.WriteLine("Focus changed!");

            CheckFullscreenApp();
        }

        public static void CheckFullscreenApp()
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
    }
}