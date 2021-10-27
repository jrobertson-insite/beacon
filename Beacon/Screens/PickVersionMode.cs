using System;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Beacon.Screens
{
    public class PickVersionMode : SetupClientConsoleMode
    {
        private string defaultVersion;

        public override ConsoleMode DoWork()
        {
            var version = Console.ReadLine();
            if (version.IsBlank())
            {
                version = this.defaultVersion;
            }
            if (!GitHelper.TagExists(ApplicationState.CommerceRepo, version))
            {
                Console.WriteLine($"The version {version} does not have a tag.");
            }
            else
            {
                ApplicationState.SetupClientState.Version = version;
                return new NameBranchMode();
            }

            return this;
        }

        public override void OnEntered()
        {
            var versionYaml = Path.Combine(this.ClientProject.GetLocalPath(), "versionInfo.yaml");
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build();
            var versionInfo = deserializer.Deserialize<VersionInfo>(File.ReadAllText(versionYaml));
            this.defaultVersion = versionInfo.CommerceVersion;
            this.PrintOnEntered(
                "Select Version",
                $"Which version do you want to use? (Press enter for {this.defaultVersion})"
            );
        }

        private class VersionInfo
        {
            public string CommerceVersion { get; set; }
        }
    }
}
