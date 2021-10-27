using System;
using System.IO;
using NUnit.Framework;
using YamlDotNet.RepresentationModel;

namespace Beacon.Tests
{
    [TestFixture]
    public class Tester
    {
        [Test]
        public void Test()
        {
            var projectPath = Path.Combine(
                ClientProject.LocalProjectPath(),
                "teamcity-partner-builds"
            );
            GitHelper.CloneOrPull(
                projectPath,
                "https://github.com/InsiteSoftware/teamcity-partner-builds.git"
            );
            GitHelper.CheckoutBranch(projectPath, "legacy");

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
                var hasSpire = false;
                Console.WriteLine(projectName);

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
                Console.WriteLine(gitUrl);
            }
        }
    }
}

