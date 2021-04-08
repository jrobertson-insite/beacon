using System;
using System.Diagnostics;
using System.IO;

namespace Beacon
{
    public class SystemHelper
    {
        public static ExecuteResult ExecuteApplication(
            string pathToExe,
            string arguments,
            string workingDirectory = null,
            bool quiet = false)
        {
            if (
                (pathToExe.Contains("/")
                || pathToExe.Contains("\\"))
                && !File.Exists(pathToExe)
            )
            {
                throw new ArgumentException(
                    "There was no application found at " + pathToExe
                );
            }

            var processStartInfo = new ProcessStartInfo(
                pathToExe,
                arguments
            )
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            if (workingDirectory != null)
            {
                processStartInfo.WorkingDirectory = workingDirectory;
            }

            var output = string.Empty;
            using var process = new Process
            {
                EnableRaisingEvents = true,
                StartInfo = processStartInfo
            };

            process.OutputDataReceived += delegate(
                object sender,
                DataReceivedEventArgs args)
            {
                if (args != null)
                {
                    output += args.Data + "\r\n";
                    if (!quiet)
                    {
                        Console.WriteLine(args.Data);
                    }
                }
            };

            process.ErrorDataReceived += delegate(
                object sender,
                DataReceivedEventArgs args)
            {
                if (args != null)
                {
                    output += args.Data + "\r\n";
                    if (!quiet)
                    {
                        Console.WriteLine(args.Data);
                    }
                }
            };

            if (!quiet)
            {
                Console.WriteLine("Executing [{0} {1}]", pathToExe, arguments);
            }

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.StandardInput.Dispose();
            process.WaitForExit();

            return new ExecuteResult
            {
                Output = output,
                ExitCode = process.ExitCode
            };
        }
    }

    public class ExecuteResult
    {
        public string Output { get; set; }
        public int ExitCode { get; set; }
    }
}
