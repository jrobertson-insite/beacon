using System;
using System.Threading;

namespace Beacon.Screens
{
    public class EndNodeMode : SetupClientConsoleMode
    {
        public override ConsoleMode DoWork()
        {
            Console.ReadKey(true);
            ApplicationState.SetupClientState = new SetupClientState();
            return null;
        }

        public override void OnEntered()
        {
            PrintOnEntered(null, null);
            Console.Write("Working");
            while (!ApplicationState.SetupClientState.SetupComplete)
            {
                Console.Write(".");
                Thread.Sleep(200);
            }
            Console.WriteLine();
            if (ApplicationState.SetupClientState.ClientProject.HasSpire)
            {
                Print("Press any key to stop node and exit.");
            }
            else
            {
                Print("Press any key to return to exit.");
            }
        }
    }
}