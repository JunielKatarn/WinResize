using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinResize
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Rect
    {
        public int Left;        // x position of upper-left corner
        public int Top;         // y position of upper-left corner
        public int Right;       // x position of lower-right corner
        public int Bottom;      // y position of lower-right corner
    }

    class Program
    {
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int X);

        [DllImport("user32.dll")]
        public static extern bool SetFocus(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);

        [DllImport("user32.dll")]
        public static extern bool GetClientRect(IntPtr hwnd, ref Rect rectangle);

        static void Main(string[] args)
        {
            Console.Title = "WinResize";
            //Read parameters from command line
            if (args.Count() == 0)
            {
                Console.WriteLine("Usage: WinResize <process ID> <width> <length>");
                return;
            }

            int processId = int.Parse(args[0]);
            int width = int.Parse(args[1]);
            int height = int.Parse(args[2]);
            using (Process process = Process.GetProcessById(processId))
            {
                var handle = process.MainWindowHandle;
                Rect winRect = new Rect();
                Rect cliRect = new Rect();
                GetWindowRect(handle, ref winRect);
                GetClientRect(handle, ref cliRect);
                int deltaX = (winRect.Right - winRect.Left) - (cliRect.Right - cliRect.Left);
                int deltaY = (winRect.Bottom - winRect.Top) - (cliRect.Bottom - cliRect.Top);
                Console.WriteLine($"Window  : ({winRect.Right - winRect.Left}, {winRect.Bottom - winRect.Top})");
                Console.WriteLine($"Delta   : ({deltaX}, {deltaY})");
                Console.WriteLine($"Client  : ({cliRect.Right - cliRect.Left}, {cliRect.Bottom - cliRect.Top})");
                Console.WriteLine("\nAdjusting...");

                MoveWindow(handle, winRect.Left, winRect.Top, width + deltaX, height + deltaY, true);
                SetFocus(handle);
            }

            Console.WriteLine("Window resized, have fun!");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
