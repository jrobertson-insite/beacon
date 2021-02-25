using System;
using System.IO;

namespace Beacon
{
    public class CleanCommerceMode : ConsoleMode
    {
        public override ConsoleMode DoWork()
        {
            var key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Backspace)
            {
                return new MainMenuMode();
            }

            if (key.Key == ConsoleKey.Enter)
            {
                SystemHelper.ExecuteApplication("git", $"-C {ApplicationState.CommerceRepo} reset --hard");
                Console.WriteLine("Cleaning up any leftover empty directories.");
                foreach (var directory in Directory.GetDirectories(Path.Combine(ApplicationState.CommerceRepo, @"FrontEnd\Modules\blueprints")))
                {
                    var directoryInfo = new DirectoryInfo(directory);
                    if (directoryInfo.Name.EqualsIgnoreCase("example") || directoryInfo.Name.EqualsIgnoreCase("buildBreaker"))
                    {
                        continue;
                    }

                    directoryInfo.Delete(true);
                }

                foreach (var directory in Directory.GetDirectories(Path.Combine(ApplicationState.CommerceRepo, @"FrontEnd\Modules\blueprints-shell")))
                {
                    var directoryInfo = new DirectoryInfo(directory);
                    if (directoryInfo.Name.EqualsIgnoreCase("example") || directoryInfo.Name.EqualsIgnoreCase("buildBreaker"))
                    {
                        continue;
                    }

                    directoryInfo.Delete(true);
                }
                
                Console.WriteLine("Done. Press any key.");
                Console.ReadKey(true);
                return new MainMenuMode();
            }

            return this;
        }

        public override void OnEntered()
        {
            ClearAndPrint("Clean Commerce:");
            Print("  This will git reset --hard and delete any remaining empty theme/blueprint folders.");
            Print("  Press enter to continue or backspace to change your mind.");
        }
    }
}