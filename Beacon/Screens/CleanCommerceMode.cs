using System;
using System.IO;

namespace Beacon.Screens
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
                SystemHelper.ExecuteApplication(
                    "git",
                    $"-C {ApplicationState.CommerceRepo} reset --hard"
                );

                void CleanStuff(string path)
                {
                    foreach (var directory in Directory.GetDirectories(
                        Path.Combine(ApplicationState.CommerceRepo, path)
                    ))
                    {
                        var directoryInfo = new DirectoryInfo(directory);
                        if (
                            directoryInfo.Name.EqualsIgnoreCase("example")
                            || directoryInfo.Name.EqualsIgnoreCase(
                                "buildBreaker"
                            )
                            || directoryInfo.Name.EqualsIgnoreCase("gsd")
                        )
                        {
                            continue;
                        }

                        directoryInfo.Delete(true);
                    }
                }

                // TODO show git status before doing this?
                Console.WriteLine(
                    "Cleaning up any leftover empty directories."
                );
                CleanStuff(@"FrontEnd\Modules\blueprints");
                CleanStuff(@"FrontEnd\Modules\blueprints-shell");
                Console.WriteLine("Done. Press any key.");
                Console.ReadKey(true);
                return new MainMenuMode();
            }

            return this;
        }

        public override void OnEntered()
        {
            ClearAndPrint("Clean Commerce:");
            Print(
                "  This will git reset --hard and delete any remaining empty theme/blueprint folders."
            );
            Print(
                "  Press enter to continue or backspace to change your mind."
            );
        }
    }
}
