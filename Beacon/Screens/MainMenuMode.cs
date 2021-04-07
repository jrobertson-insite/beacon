using System;

namespace Beacon.Screens
{
    public class MainMenuMode : ConsoleMode
    {
        public override ConsoleMode DoWork()
        {
            var key = Console.ReadKey(true).KeyChar;

            switch (key)
            {
                case '1':
                    return new PickClientMode();
                case '2':
                    return new CloneClientMode();
                case '3':
                    return new CleanCommerceMode();
                case '4':
                    Print("Goodbye!");
                    return null;
                default:
                    Print("Sorry I don't understand. Type a number.");
                    break;
            }

            return this;
        }

        public override void OnEntered()
        {
            ClearAndPrint("Main Menu:");
            Print("  1. setup client - Get a client repo, branch your local repo to a specific version, load their blueprint/themes/extensions");
            Print("  2. clone client - Get a client repo");
            Print("  3. clean commerce - clean up after #1");
            Print("  4. exit");
        }
    }
}