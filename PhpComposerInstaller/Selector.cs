using System;

namespace PhpComposerInstaller {
    internal class Selector {
        public static string ShowSelectorMenu(string[] options) {
            int optionsCount = options.Length;
            int selected = 0;
            bool done = false;
            while (!done) {
                for (int i = 0; i < optionsCount; i++) {
                    if (selected == i) {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write("    > ");
                    } else {
                        Console.Write("      ");
                    }
                    Console.WriteLine(options[i]);
                    Console.ResetColor();
                }
                switch (Console.ReadKey(true).Key) {
                    case ConsoleKey.UpArrow:
                        selected = Math.Max(0, selected - 1);
                        break;
                    case ConsoleKey.DownArrow:
                        selected = Math.Min(optionsCount - 1, selected + 1);
                        break;
                    case ConsoleKey.Enter:
                        done = true;
                        break;
                }
                if (!done) Console.CursorTop = Console.CursorTop - optionsCount;
            }
            return options[selected];
        }
    }
}
