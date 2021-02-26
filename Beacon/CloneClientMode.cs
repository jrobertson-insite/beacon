using System;
using System.Threading;
using System.Threading.Tasks;

namespace Beacon
{
    public class CloneClientMode : PickClientMode
    {
        protected override string GetHeader()
        {
            return "Clone Client Repository";
        }

        protected override int GetRows()
        {
            return base.GetRows() - 1;
        }
        
        protected override ConsoleMode OnEnterKey(ClientProject clientProject)
        {
            Console.Clear();
            var done = false;
            Task.Factory.StartNew(() =>
            {
                GitHelper.CloneOrPull(clientProject.GetLocalPath(), clientProject.GitUrl);
                done = true;
            });
            Console.WriteLine();
            Print("Cloning repo if needed");
            while (!done)
            {
                Console.Write(".");
                Thread.Sleep(100);
            }
            
            Print("Cloned to " + clientProject.GetLocalPath());
            Print("Press any key to continue.");
            Console.ReadKey(true);
            return new MainMenuMode();
        }

        public override ConsoleMode ReplaceStateIfNeeded()
        {
            return this;
        }
    }
}