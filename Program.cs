using System;
using System.Drawing;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Edit
{

    public static class Program
    {
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int MessageBox(IntPtr hWnd, String text, String caption, uint type);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]        
        public static extern int SetParent(
            IntPtr hWndChild,
            IntPtr hWndNewParent
        );

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]        
        public static extern bool SetWindowPos(
            IntPtr hWnd,
            IntPtr hWndInsertAfter,
            int  X,
            int  Y,
            int  cx,
            int  cy,
            uint uFlags
        );

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]        
        public static extern long SetWindowTheme(
            IntPtr    hwnd,
            string    pszSubAppName,
            string    pszSubIdList
        );

        private const int SW_MAXIMIZE = 3;
        private const int SW_MINIMIZE = 6;
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern long SetWindowLongA(
            IntPtr hWnd,
            int    nIndex,
            long   dwNewLong
        );

        [DllImport("user32.dll")]
        public static extern long GetWindowLongA(
            IntPtr hWnd,
            int  nIndex
        );

        public static IntPtr RemoveBorders(IntPtr WindowHandle)
        {
            long WindowStyle = GetWindowLongA(WindowHandle, -16);

            //Redraw
            SetWindowLongA(WindowHandle, -16, (WindowStyle & ~0x00080000));
            SetWindowLongA(WindowHandle, -16, (WindowStyle & ~0x00800000 | 0x00400000));
            return WindowHandle;
        }

        private static Process process = null;

        [STAThread]
        static void Main(string[] args)
        {
            //MessageBox(new IntPtr(0), "Hello World!", "Hello Dialog", 0);            
            var form = new Form();
            var button = new Button();
            form.Controls.Add(button);
            Panel panel = new Panel();
            // panel.BackColor = Color.Red;
            panel.Dock = DockStyle.Fill;
            button.Click += (s, e) => {
                button.Visible = false;
                process = Process.Start("cmd.exe");
                Thread.Sleep(500);
                SetParent(process.MainWindowHandle, panel.Handle);
                form.Controls.Add(panel);
                SetWindowPos(process.MainWindowHandle, (IntPtr)0, 0, 0, panel.Width, panel.Height, 0x0040);
                SetWindowTheme(process.MainWindowHandle, "cmd", null);
                ShowWindow(process.MainWindowHandle, SW_MAXIMIZE);
                RemoveBorders(process.MainWindowHandle);
            };
            form.Resize += (s, e) => { 
                ThreadPool.QueueUserWorkItem(delegate { 
                    SetWindowPos(process.MainWindowHandle, (IntPtr)0, 0, 0, panel.Width, panel.Height, 0x0040);
                });
            };
            //button.PerformClick();
            Application.EnableVisualStyles();
            Application.Run(form);
        }
    }
}


// namespace Edit;

// using System;
// using Edit.Structs;

// public static class Program
// {
//     private static bool IsTrapped = false;

//     private const int CURSOR_BLINK_DELAY = 10; 
//     public static void Main(string[] args)
//     {
//         Console.Clear();
//         Console.SetCursorPosition(0, 0);

//         var w = Console.WindowWidth;
//         var h = Console.WindowHeight;
//         //System.Console.WriteLine("w={0},h={1}",w,h);
//         var buffer = ConsoleBuffer.Default(w, h);
//         buffer.Grid = [.. Enumerable.Range(0,3).Select(_=>new char[w+1])];
//         buffer.Grid[0][0] = 'T';
//         buffer.Grid[0][1] = 'e';
//         buffer.Grid[0][2] = 's';
//         buffer.Grid[0][3] = 't';
//         Print(buffer);
//         Console.CancelKeyPress += delegate(object? sender, ConsoleCancelEventArgs e) {
//             e.Cancel = true;
//             IsTrapped = true;
//         };
//         do{
//             Task.Delay(CURSOR_BLINK_DELAY).Wait();
//             ConsoleKeyInfo key = Console.ReadKey();
//             switch(key.Key)
//             {
//                 case ConsoleKey.Escape:{ 
//                     buffer.IsInEditMode = !buffer.IsInEditMode; 
//                 } break;
//                 case ConsoleKey.UpArrow:{
//                     if (Console.CursorTop <= 0) continue;
//                     Console.SetCursorPosition(Console.CursorLeft,Console.CursorTop-1);
//                     buffer.Cursor = new(Console.CursorLeft, Console.CursorTop);
//                 } break;
//                 case ConsoleKey.DownArrow:{  
//                     if (Console.CursorTop >= Console.WindowHeight-1) continue;
//                     Console.SetCursorPosition(Console.CursorLeft,Console.CursorTop+1);
//                     buffer.Cursor = new(Console.CursorLeft, Console.CursorTop);
//                 } break;
//                 case ConsoleKey.LeftArrow:{  
//                     if (Console.CursorLeft <= 0) continue;
//                     Console.SetCursorPosition(Console.CursorLeft-1,Console.CursorTop);
//                     buffer.Cursor = new(Console.CursorLeft, Console.CursorTop);
//                 } break;
//                 case ConsoleKey.RightArrow:{  
//                     if (Console.CursorLeft >= Console.WindowWidth-1) continue;
//                     Console.SetCursorPosition(Console.CursorLeft+1,Console.CursorTop);
//                     buffer.Cursor = new(Console.CursorLeft, Console.CursorTop);
//                 } break;
//                 case ConsoleKey.Delete:{
//                     Console.Write(' ');

//                 }break;
//                 case ConsoleKey.Backspace:{
//                     Console.Write(' ');
//                     Console.SetCursorPosition(Console.CursorLeft-1,Console.CursorTop);
//                 }break;
//                 case ConsoleKey.Enter:{
//                     Console.SetCursorPosition(0,Console.CursorTop+1);
//                     buffer.Cursor.Y++;
//                 } break;  
//                 default:{
//                     if (buffer.IsInEditMode) continue;
//                     buffer.Grid[Console.CursorTop][Console.CursorLeft-1] = key.KeyChar;
//                     // var line = buffer.Lines[buffer.Cursor.Y];
//                     // var p = buffer.Cursor.X;
//                     // buffer.Lines[buffer.Cursor.Y] = $"{line[0..p]}{key.KeyChar}{line[p..]}";
//                 }break;
//             }
//         }
//         while(!IsTrapped);

//         Console.WriteLine("\n---------------------------------------------\n");
//         Print(buffer, Console.CursorTop);
//     }

//     public static void Print(ConsoleBuffer buffer, int line = 0)
//     {
//         Console.SetCursorPosition(0, line);
//         for(int y=0;y<buffer.Grid.Length;y++){
//             for(int x=0;x<buffer.Grid[y].Length;x++){
//                 System.Console.Write(buffer.Grid[y][x]);
//             }
//             System.Console.WriteLine();
//         }
//     }
// }