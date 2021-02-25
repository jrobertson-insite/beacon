using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TeamCitySharp;

namespace Beacon
{
    public class ClientProjectLoader
    {
        private const string fileName = "clientProjects-1.0.json";

        public static IDictionary<string, ClientProject> GetClientProjects()
        {
            if (!File.Exists(fileName))
            {
                LoadAndSaveClientProjects();
            }
            else
            {
                // TODO I could pull the spire clients out of https://github.com/InsiteSoftware/insite-commerce-config/blob/master/kubernetes/clients.yaml
                if (File.GetLastWriteTime(fileName) < DateTime.Now.AddHours(1))
                {
                    Task.Factory.StartNew(LoadAndSaveClientProjects);   
                }
            }
            
            var list = JsonConvert.DeserializeObject<List<ClientProject>>(File.ReadAllText(fileName));
            return list.ToDictionary(o => o.Name, o => o);
        }

        private static void LoadAndSaveClientProjects()
        {
            var clientProjects = GetProjectsFromTeamcity();
            File.WriteAllText(fileName, JsonConvert.SerializeObject(clientProjects.Values.Select(o => o).ToList()));
        }
        
        // TODO error if not on VPN
        private static IDictionary<string, ClientProject> GetProjectsFromTeamcity()
        {
            var client = new TeamCityClient("ISHQ-BUILDSERVER.insitesofthosting.com:8111");
            // TODO have user enter their info? grab from stonemason instead through an API? 

            var clientProjects = new Dictionary<string, ClientProject>();

            void AddClientProject(string projectName, string gitUrl, bool hasSandbox, bool hasProduction, bool hasSpire)
            {
                if (projectName == null)
                {
                    return;
                }

                if (!clientProjects.TryGetValue(projectName, out var clientProject))
                {
                    clientProject = new ClientProject
                    {
                        Name = projectName,
                        GitUrl = gitUrl.Replace("git@github.com", "git@github-work-clients")
                    };
                }

                if (hasSandbox)
                {
                    clientProject.HasSandbox = true;
                }
                if (hasProduction)
                {
                    clientProject.HasProduction = true;
                }

                if (hasSpire)
                {
                    clientProject.HasSpire = true;
                }

                clientProjects[clientProject.Name] = clientProject;
            }
            
            foreach (var buildConfig in client.BuildConfigs.ByProjectId("DevOps").Where(o => o.Name.StartsWithIgnoreCase("InspectExtensions")))
            {
                var fullBuildConfig = client.BuildConfigs.ByConfigurationId(buildConfig.Id);
                var projectName = fullBuildConfig.Parameters.Property.FirstOrDefault(o => o.Name.EqualsIgnoreCase("projectName"))?.Value;
                var gitUrl = fullBuildConfig.Parameters.Property.FirstOrDefault(o => o.Name.EqualsIgnoreCase("giturl"))?.Value;
                
                AddClientProject(projectName, gitUrl, buildConfig.Name.ContainsIgnoreCase("sandbox"), buildConfig.Name.ContainsIgnoreCase("production"), false);
            }

            foreach (var project in client.Projects.All())
            {
                if (project.Name.EqualsIgnoreCase("sandbox branch")
                    || project.Name.EqualsIgnoreCase("production branch"))
                {
                    foreach (var buildConfig in client.BuildConfigs.ByProjectId(project.Id))
                    {
                        var fullBuildConfig = client.BuildConfigs.ByConfigurationId(buildConfig.Id);
                        var projectName = fullBuildConfig.Parameters.Property.FirstOrDefault(o => o.Name.EqualsIgnoreCase("projectName"))?.Value;
                        var vcsRoot = client.VcsRoots.ById(fullBuildConfig.VcsRootEntries.VcsRootEntry.First().VcsRoot.Id);
                        var gitUrl = vcsRoot.Properties.Property.FirstOrDefault(o => o.Name.EqualsIgnoreCase("url"))?.Value;

                        AddClientProject(projectName, gitUrl, project.Name.EqualsIgnoreCase("sandbox branch"), project.Name.EqualsIgnoreCase("production branch"), true);
                    }
                }
            }

            return clientProjects;
        }
    }
}