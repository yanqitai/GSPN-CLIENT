using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Windows.Forms;
using WindowsFormsApp1.Views;

namespace WindowsFormsApp1
{
    static class version
    {
        public static string versionCode = "0.9.4 - Release Github ";
    }

    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            new Main();
            //Application.Run(new Login());
        }
    }
}
