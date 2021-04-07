using System;
using Alba.CsConsoleFormat;

namespace Beacon.Screens
{
    public abstract class ConsoleMode
    {
        public abstract ConsoleMode DoWork();
        public abstract void OnEntered();

        protected static void ClearAndPrint(string text, ConsoleColor color = ConsoleColor.White)
        {
            Console.Clear();
            Console.WriteLine(@"                                __   ___       ___  ___                              
                               |  \ |    |\   |    |   | |\  |                       
                               |  / |    | \  |    |   | | \ |                       
                               |--  |--- |--\ |    |   | |  \|                       
                               |  \ |    |  | |    |   | |   |                       
                               |__/ |___ |  | |___ |___| |   |                       ");
            Print(text, color);
        }

        protected static void Print(string text, ConsoleColor color = ConsoleColor.White, ConsoleColor? bgColor = null)
        {
            var span = new Span(text) { Color = color };
            if (bgColor.HasValue)
            {
                span.Background = bgColor.Value;
            }
            ConsoleRenderer.RenderDocument(new Document(span));
        }
        
        protected static string Pad(string value, int size)
        {
            while (value.Length < size)
            {
                value += " ";
            }

            return value;
        }

        public virtual ConsoleMode ReplaceStateIfNeeded()
        {
            return this;
        }
    }
}