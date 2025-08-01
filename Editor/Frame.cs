using System;
using System.Threading;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Editor
{
    public class Frame : Form
    {
        

        private readonly MyRichTextBox rtb;
        public Frame(string[] args)
        {
            Text = "Editor";
            rtb = new MyRichTextBox() { 
                Font = new Font("Agave Nerd Font",12,FontStyle.Regular)
            };
            rtb.Dock = DockStyle.Fill;
            rtb.BorderStyle = BorderStyle.None;
            Controls.Add(rtb);
            rtb.ForeColor = Color.White;
            rtb.BackColor = Color.FromArgb(255,40, 40, 40);
            this.BackColor = Color.FromArgb(255,50, 50, 50);
            this.Padding = new Padding(5, 5, 5, 5);
            if(args.Length>0){
                rtb.Text = File.ReadAllText(args[0]);
                Text = args[0];
                Shown += (s,e) => {
                    ThreadPool.QueueUserWorkItem(delegate
                    {
                        rtb.BeginUpdate();
                        foreach (var keyword in keywords)
                        {
                            HighlightText(rtb, keyword, Color.Pink);
                        }
                        foreach (var keyword in PUNCTS)
                        {
                            HighlightText(rtb, keyword + "", Color.Yellow, true);
                        }
                        rtb.EndUpdate();
                        rtb.Invalidate();
                    });
                };
            }
        }

        private static readonly string[] keywords = {
            "new",
            "namespace",
            "using",
            "while",
            "do",
            "extern",
            "bool",
            "long",
            "uint",
            "ulong",
            "IntPtr",
            "public",
            "class",
            "void",
            "int",
            "double",
            "protected",
            "private",
            "static",
            "virtual",
            "overriden",
            "string",
            "char",
            "internal",
            "return",
            "switch",
            "if",
            "else",
            "??",
            "<<",
            ">>",
            "delegate",
            "=>",
            "object",
            "DateTime",
            "null",
            "foreach",
            "in",
            "case",
            "default",
            "break",
            "throw",
            "fixed",
            "get",
            "set",
            "readonly",
            "[]",
            ""
        };
        private static readonly char[] PUNCTS = { 
            '{', 
            '}',
            ':',
            '?',
            '!', 
            '(', 
            ')', 
            ';', 
            ',',
            '[',
            '.',
            ']', 
            '"', 
            '\'', 
            ' ', 
            '\n', 
            '\r', 
            '\t', 
            '\0' 
        };

        public void HighlightText(MyRichTextBox myRtb, string word, Color color, bool force = false)
        {

            if (word == string.Empty)
                return;

            int s_start = myRtb.SelectionStart, startIndex = 0, index;
            var scolor = myRtb.SelectionColor;

            while ((index = myRtb.Text.IndexOf(word, startIndex)) != -1)
            {
                var p = index == 0 || PUNCTS.Contains(myRtb.Text[index - 1]);
                var n = index+word.Length < myRtb.Text.Length && PUNCTS.Contains(myRtb.Text[index+word.Length]);
                if (force || (p && n))
                {
                    myRtb.Select(index, word.Length);
                    myRtb.SelectionColor = color;
                }

                startIndex = index + word.Length;
            }

            myRtb.SelectionStart = s_start;
            myRtb.SelectionLength = 0;
            myRtb.SelectionColor = scolor;

        }
    }

    public class MyRichTextBox : RichTextBox
    {
        public void BeginUpdate() {
        SendMessage(this.Handle, WM_SETREDRAW, (IntPtr)0, IntPtr.Zero);
        }
        public void EndUpdate() {
        SendMessage(this.Handle, WM_SETREDRAW, (IntPtr)1, IntPtr.Zero);
        }
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);
        private const int WM_SETREDRAW = 0x0b;        
    }
}