using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Beacon
{
    public class PickBlueprintNode : ConsoleMode
    {
        private IList<string> blueprints = new List<string>();
        
        public override ConsoleMode DoWork()
        {
            if (!ApplicationState.SetupClientState.ClientProject.HasSpire)
            {
                return new EndNode();
            }

            var key = Console.ReadKey(true).KeyChar;
            for (var x = 0; x < blueprints.Count; x++)
            {
                if ((x + 1).ToString() == key.ToString())
                {
                    ApplicationState.SetupClientState.BlueprintName = blueprints[x];
                    return new EndNode();
                }   
            }

            return this;
        }

        public override void OnEntered()
        {
            if (ApplicationState.SetupClientState.ClientProject.HasSpire)
            {
                Console.Clear();
                Print(ApplicationState.SetupClientState.GetHeader() + " - Select Blueprint");
                var blueprintsPath = Path.Combine(ApplicationState.SetupClientState.ClientProject.GetLocalPath(), @"src\FrontEnd\modules\blueprints");
                foreach (var directory in Directory.GetDirectories(blueprintsPath))
                {
                    var directoryInfo = new DirectoryInfo(directory);
                    blueprints.Add(directoryInfo.Name);
                }
                for (var x = 0; x < blueprints.Count; x++)
                {
                    Console.WriteLine($"  {(x + 1)}. {blueprints[x]}");
                }
            }
            
            var thread = new Thread(ProjectHelper.DoStuff);
            thread.Start();
        }
    }
}