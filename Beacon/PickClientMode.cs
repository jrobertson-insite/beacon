using System;
using System.IO;
using System.Threading;

namespace Beacon
{
    public class PickClientMode : ClientsViewMode
    {
        protected override void PrintHeader()
        {
            Print(ApplicationState.SetupClientState.GetHeader() + " - Select Client");
            base.PrintHeader();
        }

        protected override int GetRows()
        {
            return base.GetRows() - 1;
        }

        protected override ConsoleMode OnEnterKey(ClientProject clientProject)
        {
            Console.Clear();
            ApplicationState.PickClient(clientProject);
            Print(ApplicationState.SetupClientState.GetHeader());
            Console.WriteLine();
            Print("Cloning repo if needed");
            while (!ApplicationState.SetupClientState.RepositoryReady && !ApplicationState.SetupClientState.CloneRepositoryFailed)
            {
                Console.Write(".");
                Thread.Sleep(100);
            }

            if (ApplicationState.SetupClientState.RepositoryReady)
            {
                return new PickEnvironmentMode();   
            }
            else
            {
                Print("Press any key to return");
                var key = Console.ReadKey(true);
                return new PickClientMode();
            }
        }

        public override void OnEntered()
        {
            ApplicationState.SetupClientState = new SetupClientState();
            base.OnEntered();
        }

        public override ConsoleMode ReplaceStateIfNeeded()
        {
            if (!Directory.Exists(ApplicationState.CommerceRepo))
            {
                Console.Clear();
                Console.WriteLine("Your commerce repo needs to be at " + ApplicationState.CommerceRepo);
                Console.WriteLine("Press any key to continue.");
                Console.ReadKey(true);
                return new MainMenuMode();
            }

            if (!GitHelper.IsClean(ApplicationState.CommerceRepo))
            {
                Console.Clear();
                Console.WriteLine("Your commerce repo is not clean. Please stash/commit any changes or use the main menu option to clean it.");
                Console.WriteLine("Press any key to continue.");
                Console.ReadKey(true);
                return new MainMenuMode();
            }

            return this;
        }
    }
}