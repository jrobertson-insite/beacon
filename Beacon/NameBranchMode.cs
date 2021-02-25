using System;
using System.Threading;

namespace Beacon
{
    public class NameBranchMode : ConsoleMode
    {
        public override ConsoleMode DoWork()
        {
            var line = Console.ReadLine();
            if (line.IsBlank())
            {
                line = ApplicationState.SetupClientState.ClientProject.Name + "-" + ApplicationState.SetupClientState.Version;
            }
            ApplicationState.SetupClientState.BranchName = line;
            Console.Clear();
            Print(ApplicationState.SetupClientState.GetHeader());
            Console.WriteLine();
            Print("Checking out version");
            var message = GitHelper.CheckoutBranch(ApplicationState.CommerceRepo, $"{ApplicationState.SetupClientState.Version} -b {line}");
            if (message != null)
            {
                Console.WriteLine(message);
                Console.WriteLine("Press any key");
                Console.ReadKey(true);
                ApplicationState.SetupClientState.BranchName = null;
                return new NameBranchMode();
            }

            return new PickBlueprintNode();
        }

        public override void OnEntered()
        {
            Console.Clear();
            var clientProject = ApplicationState.SetupClientState.ClientProject;
            var version = ApplicationState.SetupClientState.Version;
            Print(ApplicationState.SetupClientState.GetHeader() + " - Name Branch");
            Print($"What should we name the branch? (Press enter for '{clientProject.Name}-{version}')");
        }
    }
}