using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WindowsFormsApp1.ConsoleUI;

namespace WindowsFormsApp1.ConsoleHelpers
{
    public class DateSelectionConsole
    {
        public List<int> MenuItems { get; set; }
        private int selectedItemIndex = 0;
        private bool loopComplete = false;

        public DateSelectionConsole()
        {
            MenuItems = new List<int>() { DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year };
        }

        public async Task<DateTime> RunConsoleDateSelection()
        {
            ConsoleKeyInfo kb;

            Console.WriteLine($"{Environment.NewLine} Selecine a data para pesquisa: {Environment.NewLine}");

            int top = Console.CursorTop;
            int left = Console.CursorLeft;

            int topOffset;
            int bottomOffset;


            Console.CursorVisible = false;

            while (!loopComplete)
            {
                topOffset = Console.CursorTop;
                bottomOffset = Console.CursorLeft;

                for (int i = 0; i < MenuItems.Count; i++)
                {
                    WriteConsoleItem(i, selectedItemIndex);
                }

                kb = Console.ReadKey(true);
                HandleKeyPress(kb.Key);

                //Reset the cursor to the top of the screen
                Console.SetCursorPosition(bottomOffset, topOffset);
            }

            Console.CursorVisible = true;

            //coloca o cursor no final da pagina
            Console.SetCursorPosition(left, top + MenuItems.Count);

            //return result
            return new DateTime(MenuItems[2], MenuItems[1], MenuItems[0]);
        }

        private void HandleKeyPress(ConsoleKey pressedKey)
        {
            switch (pressedKey)
            {
                case ConsoleKey.UpArrow:
                    selectedItemIndex = (selectedItemIndex == 0) ? MenuItems.Count - 1 : selectedItemIndex - 1;
                    break;

                case ConsoleKey.DownArrow:
                    selectedItemIndex = (selectedItemIndex == MenuItems.Count - 1) ? 0 : selectedItemIndex + 1;
                    break;

                case ConsoleKey.LeftArrow:
                    //0 is Day
                    //1 is Month
                    //2 is Year

                    if (selectedItemIndex == 0)
                    {
                        MenuItems[0] = MenuItems[0] <= 1 ? MenuItems[0] = 31 /*DateTime.DaysInMonth(MenuItems[2], MenuItems[1])*/ : MenuItems[0] - 1;
                        break;
                    }

                    if (selectedItemIndex == 1)
                    {
                        MenuItems[1] = MenuItems[1] <= 1 ? MenuItems[1] = 12 : MenuItems[1] - 1;
                        break;
                    }

                    MenuItems[2] -= 1;

                    break;

                case ConsoleKey.RightArrow:
                    //0 is Day
                    //1 is Month
                    //2 is Year

                    if (selectedItemIndex == 0)
                    {
                        MenuItems[0] = MenuItems[0] >= 31 ? MenuItems[0] = 1 : MenuItems[0] + 1;
                        break;
                    }

                    if (selectedItemIndex == 1)
                    {
                        MenuItems[1] = MenuItems[1] >= 12 ? MenuItems[1] = 1 : MenuItems[1] + 1;
                        break;
                    }

                    MenuItems[2] += 1;
                    break;

                case ConsoleKey.Enter:
                    loopComplete = true;
                    break;
            }
        }

        private void WriteConsoleItem(int itemIndex, int selectedItemIndex)
        {
            if (itemIndex == selectedItemIndex)
            {
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.ForegroundColor = ConsoleColor.Black;
            }

            Console.WriteLine(" {0,-20}", this.MenuItems[itemIndex]);
            Console.ResetColor();
        }
    }
}