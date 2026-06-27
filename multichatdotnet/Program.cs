/*
 * @brief Entry Point
 * 
 * @author Prahlad Yeri <prahladyeri@yahoo.com>
 * @license MIT
 * @date 2026-06-03
 */

using multichatdotnet.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;


namespace multichatdotnet
{
    internal static class Program
    {
        private static Mutex mutex = null;
        public static AppSettings Settings;
        public static readonly string LocalAppData = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    Application.ProductName
                );

        public static List<ProviderInfo> DefaultProviders;

        [STAThread]
        static void Main()
        {
            const string mutexName = "Global\\multiChatBot";
            bool createdNew;
            mutex = new Mutex(true, mutexName, out createdNew);
            if (!createdNew)
            {
                MessageBox.Show("Another instance of the application is already running.", "Instance Already Running",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            Application.ThreadException += (sender, args) =>
            {
                MessageBox.Show(args.Exception.ToString(), "Unhandled UI Exception");
            };
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                Exception ex = args.ExceptionObject as Exception;
                MessageBox.Show(ex?.ToString() ?? "Unknown error", "Fatal Exception");
            };

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            //TODO: Future Proofing
            //System.Net.ServicePointManager.SecurityProtocol =
            //    (System.Net.SecurityProtocolType)3072 | // TLS 1.2
            //    (System.Net.SecurityProtocolType)12288; // TLS 1.3

            string thePath = Path.Combine(Application.StartupPath , Application.ProductName + ".db");
            DBAL.SetConnectionString(thePath);
            Settings = AppSettings.Load();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
