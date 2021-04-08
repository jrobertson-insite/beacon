using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Beacon
{
    public static class ApplicationState
    {
        public static IDictionary<string, ClientProject> ClientProjects = null;
        public static string CommerceRepo = @"C:\Projects\insite-commerce";

        public static IEnumerable<ClientProject> GetSortedClientProjects()
        {
            return ClientProjects.Values.OrderBy(o => o.Name);
        }

        public static SetupClientState SetupClientState = new SetupClientState();

        public static void PickClient(ClientProject clientProject)
        {
            SetupClientState.ClientProject = clientProject;
            var thread = new Thread(
                () =>
                {
                    var message = GitHelper.CloneOrPull(
                        clientProject.GetLocalPath(),
                        clientProject.GitUrl
                    );
                    if (message == null)
                    {
                        SetupClientState.RepositoryReady = true;
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine(message);
                        SetupClientState.CloneRepositoryFailed = true;
                    }
                }
            );
            thread.Start();
        }
    }

    public class SetupClientState
    {
        public ClientProject ClientProject { get; set; }
        public bool RepositoryReady { get; set; }
        public bool CloneRepositoryFailed { get; set; }
        public string ClientBranch { get; set; }
        public string Version { get; set; }
        public string BranchName { get; set; }
        public string BlueprintName { get; set; }
        public bool SetupComplete { get; set; }

        public string GetHeader()
        {
            var result = "Setup Client";
            if (this.ClientProject == null)
            {
                return result;
            }
            result += " - " + this.ClientProject.Name;

            if (this.ClientBranch == null)
            {
                return result;
            }
            result += " - " + this.ClientBranch;

            if (this.Version == null)
            {
                return result;
            }
            result += " - " + this.Version;

            if (this.BranchName == null)
            {
                return result;
            }
            result += " - " + BranchName;

            if (this.BlueprintName == null)
            {
                return result;
            }

            result += " - " + BlueprintName;

            return result;
        }
    }
}
