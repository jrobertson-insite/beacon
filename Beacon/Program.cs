using System;
using System.IO;
using System.Threading;
using Beacon.Screens;
using Beacon.Services;

namespace Beacon
{
    class Program
    {
        static void Main()
        {
            VersionChecker.CheckForNewVersion();

            WriteBeaconImage();
            var thread = new Thread(LoadClientProjects);
            thread.Start();
            while (ApplicationState.ClientProjects == null)
            {
                Thread.Sleep(100);
                WriteInitializing();
            }

            new App().Run();
        }

        private static void LoadClientProjects()
        {
            ApplicationState.ClientProjects = ClientProjectLoader.GetClientProjects();
        }

        private static int count = 0;

        private static void WriteInitializing()
        {
            Console.SetCursorPosition(0, 25);
            var result = "Initializing";
            var x = 0;
            while (x < count % 10)
            {
                result += ".";
                x++;
            }

            while (x < 10)
            {
                result += "  ";
                x++;
            }

            count++;
            Console.WriteLine(result);
        }

        private static void WriteBeaconImage()
        {
            Console.Clear();
            Console.WriteLine(
                @"                                                                                                      
                                                                                                      
                                                                                                      
          _______                                                                                     
         /*******\                                                                                    
         |*******|<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
         |*******|<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
         |*******|                               __   ___       ___  ___                              
          |||||||                               |  \ |    |\   |    |   | |\  |                       
          |||||||                               |  / |    | \  |    |   | | \ |                       
          |||||||                               |--  |--- |--\ |    |   | |  \|                       
          |||||||                               |  \ |    |  | |    |   | |   |                       
          |||||||                               |__/ |___ |  | |___ |___| |   |                       
          |||||||                                                                                     
          |||||||                                                                                     
         /|||||||\                                                                                    
        /|||||||||\                                                                                   
       /|||||||||||\                                                                                  
#############################                                                                         
##############################))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))
##################################))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))
#########################################)))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))
##########################################################))))))))))))))))))))))))))))))))))))))))))))
#############################################################################################)))))))))"
            );
        }
    }

    public class ClientProject
    {
        public string Name { get; set; }
        public string GitUrl { get; set; }
        public bool HasSandbox { get; set; }
        public bool HasProduction { get; set; }
        public bool HasSpire { get; set; }

        public string GetLocalPath()
        {
            return Path.Combine(@"c:\Projects\Clients", this.Name);
        }
    }
}
