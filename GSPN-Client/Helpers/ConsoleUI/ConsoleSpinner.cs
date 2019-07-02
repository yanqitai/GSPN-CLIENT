using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WindowsFormsApp1.ConsoleUI
{
    public class Spinner : IDisposable
    {
        private const string Sequence = @"/-\|";
        private int counter = 0;
        private int left;
        private int top;
        private bool dynamic;
        private readonly int delay;
        private bool active;
        private readonly Thread thread;

        public Spinner(bool dynamic = true, int left = 0, int top = 0, int delay = 100)
        {
            this.dynamic = dynamic;
            this.left = left;
            this.top = top;
            this.delay = delay;
            thread = new Thread(Spin);
        }

        public void Start()
        {
            if (dynamic)
            {
                this.left = Console.CursorLeft;
                this.top = Console.CursorTop;
                if (left == 0) left = 1;
            }

            active = true;
            if (!thread.IsAlive)
                thread.Start();
        }

        public void Stop()
        {
            active = false;
            Draw(' ');
            Console.ResetColor();
        }

        private void Spin()
        {
            while (active)
            {
                Turn();
                Thread.Sleep(delay);
            }
        }

        private void Draw(char c)
        {

            Console.SetCursorPosition(left, top);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(c);
        }

        private void Turn()
        {
            Draw(Sequence[++counter % Sequence.Length]);
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
