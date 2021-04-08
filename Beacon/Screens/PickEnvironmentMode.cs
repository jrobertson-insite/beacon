using System;

namespace Beacon.Screens
{
    public class PickEnvironmentMode : ConsoleMode
    {
        private ClientProject clientProject =>
            ApplicationState.SetupClientState.ClientProject;

        public override ConsoleMode DoWork()
        {
            var key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Backspace)
            {
                return new PickClientMode();
            }
            else if (key.KeyChar == '1' && clientProject.HasSandbox)
            {
                return Checkout("sandbox");
            }
            else if (key.KeyChar == '2' && clientProject.HasProduction)
            {
                return Checkout("production");
            }

            return this;
        }

        private ConsoleMode Checkout(string branch)
        {
            Console.Clear();
            Print(ApplicationState.SetupClientState.GetHeader());
            Console.WriteLine();
            Print("Cloning or pulling repo");
            Console.WriteLine("Pulling any changes");
            ApplicationState.SetupClientState.ClientBranch = branch;
            var message = GitHelper.CheckoutBranch(
                clientProject.GetLocalPath(),
                branch
            );
            if (message == null)
            {
                message = GitHelper.Pull(clientProject.GetLocalPath());
                if (message == null)
                {
                    return new PickVersionMode();
                }
            }
            Console.WriteLine(message);
            Console.WriteLine("Press any key");
            var key = Console.ReadKey(true);
            return new MainMenuMode();
        }

        public override void OnEntered()
        {
            Console.Clear();
            Print(
                ApplicationState.SetupClientState.GetHeader() + " - Select Environment"
            );
            if (clientProject.HasSandbox)
            {
                Print("  1. Sandbox");
            }
            else
            {
                Print("  1. Sandbox - Not Found");
            }

            if (clientProject.HasProduction)
            {
                Print("  2. Production");
            }
            else
            {
                Print("  2. Production - Not Found");
            }
        }
    }
}
