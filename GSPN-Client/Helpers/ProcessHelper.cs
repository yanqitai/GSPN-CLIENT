using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1.Helpers
{
    public class ProcessHelper
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool MoveWindow(IntPtr hWnd, int x, int y, int width, int height, bool repaint);

        public static void StartProcess(String process, String name, int x = 0, int y = 0, int width = 0, int height = 0)
        {
            if (width == 0 && height == 0)
            {
                width = 700;
                height = 550;
            }

            var prc = Process.Start(process, name);

            prc.WaitForInputIdle();

            bool ok = MoveWindow(prc.MainWindowHandle, x, y, width, height, true);
        }

        public static void StartProcess(String process)
        {
            var prc = Process.Start(process);
            prc.WaitForExit();
        }

        public static async Task<bool> CloseProcessByTitle(String name, String title)
        {

            Process[] pname = Process.GetProcessesByName(name);
            var a = pname.ToList();
            await a.ForEachAsync(async x =>
           {
               if (x.MainWindowTitle.Contains(title)) x.CloseMainWindow();
           });

            return true;
        }
    }
}
