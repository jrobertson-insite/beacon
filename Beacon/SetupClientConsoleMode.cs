using System;

namespace Beacon
{
    public abstract class SetupClientConsoleMode : ConsoleMode
    {
        protected ClientProject ClientProject => ApplicationState.SetupClientState.ClientProject;

        protected void PrintOnEntered(string title, string message)
        {
            Console.Clear();
            Print(ApplicationState.SetupClientState.GetHeader() + (title != null ? $" - {title}" : null));
            if (message != null)
            {
                Print(message);   
            }
        }
    }
}