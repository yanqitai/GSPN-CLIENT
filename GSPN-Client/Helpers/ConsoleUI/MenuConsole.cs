using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WindowsFormsApp1.ConsoleHelpers
{
    public class MenuConsole
    {
        public ConsoleMenuItem[] MenuItems { get; set; }
        private string Description;
        private int selectedItemIndex = 0;
        private bool loopComplete = false;

        public MenuConsole(string description, IEnumerable<ConsoleMenuItem> menuItems)
        {
            MenuItems = menuItems.ToArray();
            Description = description;
        }

        public async void RunConsoleMenu()
        {
            //this will resise the console if the amount of elements in the list are too big
            if ((MenuItems.Count()) > Console.WindowHeight)
            {
                //TODO: Deal with console pagging...
            }

            int topOffset = Console.CursorTop;
            int bottomOffset = Console.CursorLeft;

            ConsoleKeyInfo kb;

            Boolean loop = false;

            while (!loop)
            {

                if (!string.IsNullOrEmpty(Description))
                {
                    Console.WriteLine($"{Description}: {Environment.NewLine}");
                }

                Console.CursorVisible = false;

                while (!loopComplete)
                {
                    topOffset = Console.CursorTop;
                    bottomOffset = Console.CursorLeft;

                    for (int i = 0; i < MenuItems.Length; i++)
                    {
                        WriteConsoleItem(i, selectedItemIndex);
                    }

                    kb = Console.ReadKey(true);
                    HandleKeyPress(kb.Key);

                    //Reset the cursor to the top of the screen
                    Console.SetCursorPosition(bottomOffset, topOffset);
                }

                Console.CursorVisible = true;
                loopComplete = false;

                //set the cursor just after the menu so that the program can continue after the menu
                //Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop);

                topOffset = Console.CursorTop;
                bottomOffset = Console.CursorLeft;

                await MenuItems[selectedItemIndex].CallBack();
                
            }
        }

        private void HandleKeyPress(ConsoleKey pressedKey)
        {
            switch (pressedKey)
            {
                case ConsoleKey.UpArrow:
                    selectedItemIndex = (selectedItemIndex == 0) ? MenuItems.Length - 1 : selectedItemIndex - 1;
                    break;

                case ConsoleKey.DownArrow:
                    selectedItemIndex = (selectedItemIndex == MenuItems.Length - 1) ? 0 : selectedItemIndex + 1;
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

            Console.WriteLine(" {0,-20}", this.MenuItems[itemIndex].Name);
            Console.ResetColor();
        }
    }


    public class ConsoleMenuItem
    {
        public string Name { get; set; }
        public Func<Task> CallBack { get; set; }

        public ConsoleMenuItem(string label, Func<Task> callback)
        {
            Name = label;
            CallBack = callback;
        }
    }

}