using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TeamCitySharp;
using YamlDotNet.RepresentationModel;

namespace Beacon.Services
{
    public class ClientProjectLoader
    {
        private static string fileName =>
            Path.Combine(
                Environment.GetEnvironmentVariable(
                    RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "LocalAppData" : "Home"
                ),
                "Beacon",
                "clientProjects-1.0.json"
            );

        public static IDictionary<string, ClientProject> GetClientProjects()
        {
            new FileInfo(fileName).Directory.EnsureExists();
            if (!File.Exists(fileName))
            {
                LoadAndSaveClientProjects();
            }
            else
            {
                Task.Factory.StartNew(LoadAndSaveClientProjects);
            }

            var list = JsonConvert.DeserializeObject<List<ClientProject>>(
                File.ReadAllText(fileName)
            );
            return list.ToDictionary(o => o.Name, o => o);
        }

        private static void LoadAndSaveClientProjects()
        {
            var clientProjects = new Dictionary<string, ClientProject>();
            GetProjectsFromLegacyYaml(clientProjects);
            GetProjectsFromYaml(clientProjects);
            File.WriteAllText(
                fileName,
                JsonConvert.SerializeObject(
                    clientProjects.Values.OrderBy(o => o.Name).Select(o => o).ToList()
                )
            );
        }

        private static void GetProjectsFromYaml(Dictionary<string, ClientProject> clientProjects)
        {
            var projectPath = Path.Combine(
                ClientProject.LocalProjectPath(),
                "insite-commerce-config"
            );
            GitHelper.CloneOrPull(
                projectPath,
                "https://github.com/InsiteSoftware/insite-commerce-config.git",
                true
            );

            GitHelper.CheckoutBranch(projectPath, "master", true);

            var content = File.ReadAllText(Path.Combine(projectPath, "kubernetes", "clients.yaml"));

            var stream = new YamlStream();
            stream.Load(new StringReader(content));

            var rootNode = stream.Documents[0].RootNode;
            var hosts = rootNode["all"]["hosts"] as YamlMappingNode;

            //x.RootNode["all"]["hosts"]["bunzl"]["git_uri"].ToString();
            foreach (var client in hosts.Children)
            {
                var projectName = (client.Key as YamlScalarNode).Value;
                var properties = client.Value as YamlMappingNode;
                var gitUrl = "";
                var hasSandbox = false;
                var hasProduction = false;
                var hasSpire = false;

                foreach (var property in properties)
                {
                    var propertyName = (property.Key as YamlScalarNode).Value;
                    if (propertyName == "git_uri")
                    {
                        gitUrl = property.Value.ToString();
                    }
                    else if (propertyName == "environments")
                    {
                        var environments = property.Value as YamlMappingNode;
                        foreach (var environment in environments.Children)
                        {
                            var environmentName = (environment.Key as YamlScalarNode).Value;
                            if (environmentName.Contains("sandbox"))
                            {
                                hasSandbox = true;
                            }
                            else if (environmentName.Contains("production"))
                            {
                                hasProduction = true;
                            }
                        }

                        hasSpire = property.Value.ToString().Contains("cms, Spire");
                    }
                }

                if (!gitUrl.IsBlank())
                {
                    AddClientProject(
                        clientProjects,
                        projectName,
                        gitUrl,
                        hasSandbox,
                        hasProduction,
                        hasSpire
                    );
                }
            }
        }

        private static void GetProjectsFromLegacyYaml(
            Dictionary<string, ClientProject> clientProjects
        )
        {
            var projectPath = Path.Combine(
                ClientProject.LocalProjectPath(),
                "teamcity-partner-builds"
            );
            GitHelper.CloneOrPull(
                projectPath,
                "https://github.com/InsiteSoftware/teamcity-partner-builds.git",
                true
            );
            GitHelper.CheckoutBranch(projectPath, "legacy", true);

            var content = File.ReadAllText(Path.Combine(projectPath, ".teamcity", "clients.yaml"));

            var stream = new YamlStream();
            stream.Load(new StringReader(content));

            var rootNode = stream.Documents[0].RootNode;
            var hosts = rootNode["clients"] as YamlMappingNode;

            foreach (var client in hosts.Children)
            {
                var projectName = (client.Key as YamlScalarNode).Value;
                var gitUrl = "";
                var hasSandbox = false;
                var hasProduction = false;

                var environments = (client.Value["environments"] as YamlMappingNode);
                if (environments == null)
                {
                    continue;
                }

                foreach (var environment in environments)
                {
                    var propertyName = (environment.Key as YamlScalarNode).Value;
                    if (propertyName == "Sandbox")
                    {
                        hasSandbox = true;
                    }
                    else if (propertyName == "Production")
                    {
                        hasProduction = true;
                    }

                    foreach (var property in environment.Value["parameters"] as YamlMappingNode)
                    {
                        if (property.Key.ToString() == "gitUrl")
                        {
                            gitUrl = property.Value.ToString();
                        }
                    }
                }

                if (!gitUrl.IsBlank())
                {
                    AddClientProject(
                        clientProjects,
                        projectName,
                        gitUrl,
                        hasSandbox,
                        hasProduction,
                        false
                    );
                }
            }
        }

        private static void AddClientProject(
            IDictionary<string, ClientProject> clientProjects,
            string projectName,
            string gitUrl,
            bool hasSandbox,
            bool hasProduction,
            bool hasSpire
        )
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
    }
}
