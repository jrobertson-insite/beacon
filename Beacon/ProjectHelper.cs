using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Beacon
{
    public class ProjectHelper
    {
        public static void DoStuff()
        {
            var clientProject = ApplicationState.SetupClientState.ClientProject;
            var clientPath = ApplicationState.SetupClientState.ClientProject.GetLocalPath();
            var clientName = ApplicationState.SetupClientState.ClientProject.Name;
            var commercePath = ApplicationState.CommerceRepo;

            var tasks = new List<Task>();

            void DoSpire()
            {
                SystemHelper.ExecuteApplication(
                    "xcopy",
                    $"{Path.Combine(clientPath, @"src\FrontEnd\modules\blueprints")} {Path.Combine(commercePath, @"FrontEnd\modules\blueprints")} /E/Y",
                    null,
                    true
                );
                SystemHelper.ExecuteApplication(
                    "xcopy",
                    $"{Path.Combine(clientPath, @"src\FrontEnd\modules\blueprints-shell")} {Path.Combine(commercePath, @"FrontEnd\modules\blueprints-shell")} /E/Y",
                    null,
                    true
                );
                SystemHelper.ExecuteApplication(
                    "cmd",
                    "/c npm install",
                    Path.Combine(commercePath, @"FrontEnd"),
                    true
                );

                while (ApplicationState.SetupClientState.BlueprintName == null)
                {
                    Thread.Sleep(100);
                }

                // kick this off in a new thread and don't wait for it, it is a window that stays open.
                // ideally we'd somehow know when it gets to startup complete, but that's harder to do
                Task.Factory.StartNew(
                    () =>
                    {
                        using var process = Process.Start(
                            new ProcessStartInfo
                            {
                                FileName = "npm",
                                UseShellExecute = true,
                                WindowStyle = ProcessWindowStyle.Normal,
                                Arguments =
                                    $"run start {ApplicationState.SetupClientState.BlueprintName}",
                                WorkingDirectory = Path.Combine(commercePath, @"FrontEnd")
                            }
                        );
                    }
                );
            }

            void DoClassic()
            {
                var webCsProj = Path.Combine(
                    commercePath,
                    "Legacy",
                    "InsiteCommerce.Web",
                    "InsiteCommerce.Web.csproj"
                );
                var text = File.ReadAllLines(webCsProj).ToList();
                text.Insert(
                    text.Count - 2,
                    @$"   <ItemGroup>
    <Reference Include=""Extensions, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"">
      <HintPath>..\..\..\clients\{clientName}\dist\Extensions.dll</HintPath>
    </Reference>
  </ItemGroup>"
                );
                File.WriteAllText(webCsProj, string.Join(Environment.NewLine, text));

                if (!clientProject.HasSpire)
                {
                    SystemHelper.ExecuteApplication(
                        "xcopy",
                        $"{Path.Combine(clientPath, @"src\InsiteCommerce.Web\Themes")} {Path.Combine(commercePath, @"Legacy\InsiteCommerce.Web\Themes")} /E/Y",
                        null,
                        true
                    );
                    // TODO grunt build themes
                }

                // TODO where do I find this?
                var result = SystemHelper.ExecuteApplication(
                    @"C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\msbuild.exe",
                    @"C:\projects\insite-commerce\Legacy\InsiteCommerce.sln -m",
                    null,
                    true
                );
                if (result.ExitCode != 0)
                {
                    // TODO what now!
                    // property for SetupFailure with a message?
                }
            }

            tasks.Add(Task.Factory.StartNew(DoClassic));

            if (clientProject.HasSpire)
            {
                tasks.Add(Task.Factory.StartNew(DoSpire));
            }

            Task.WaitAll(tasks.ToArray());

            ApplicationState.SetupClientState.SetupComplete = true;
        }
    }
}
