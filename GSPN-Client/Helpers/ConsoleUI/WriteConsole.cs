using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WindowsFormsApp1.ConsoleUI
{
    public class WriteConsole
    {
        //Escreve no Console especificando o local da tela
        public static void WriteInLine(String text, int line = 0, int left = 0, bool returnToCurrentLine = false)
        {
            var topOffset = line == 0 ? Console.CursorTop : line;
            var bottomOffset = left == 0 ? Console.CursorLeft : left;

            clearLine(bottomOffset, topOffset);

            Console.SetCursorPosition(bottomOffset, topOffset);

            Console.Write(text);

            //retorna para a linha primaria
            if (returnToCurrentLine)
            {
                Console.SetCursorPosition(bottomOffset, topOffset);
            }
        }

        //replica um caracter de um local da tela
        public static void Separator(char text, int top = 0, int bottom = 0)
        {
            if (top != 0)
            {
                Console.CursorTop += top;
                Console.CursorLeft = 0;
            }

            Console.CursorTop += 1;
            Console.CursorLeft = 0;

            Console.Write(new String(text, Console.BufferWidth - Console.CursorLeft));

            //Console.CursorTop += -1;

            if (bottom != 0)
            {
                Console.CursorTop += bottom;
                Console.CursorLeft = 0;
            }
        }

        //limpa uma linha especifica do console
        public static void clearLine(int left, int top)
        {
            int pLeft = Console.CursorLeft;
            int pTop = Console.CursorTop;

            Console.SetCursorPosition(left, top);
            Console.Write(new string(' ', Console.BufferWidth - Console.CursorLeft));

            Console.SetCursorPosition(pLeft, pTop);
        }
    }
}
