using System.IO;
using TeamCitySharp.DomainEntities;

namespace Beacon
{
    public static class GitHelper
    {
        public static bool IsClean(string path)
        {
            var result = SystemHelper.ExecuteApplication("git", $"-C {path} status");
            return result.Output.Contains("nothing to commit, working tree clean");
        }

        public static string CheckoutBranch(string path, string branch, bool quiet = false)
        {
            var result = SystemHelper.ExecuteApplication(
                "git",
                $"-C {path} checkout {branch}",
                null,
                quiet
            );
            return result.ExitCode != 0 ? result.Output : null;
        }

        public static string CloneOrPull(string path, string gitUrl, bool quiet = false)
        {
            new DirectoryInfo(path).EnsureExists();

            if (Directory.Exists(Path.Combine(path, ".git")))
            {
                return Pull(path, quiet);
            }

            var result = SystemHelper.ExecuteApplication(
                "git",
                $"clone {gitUrl} {path}",
                null,
                quiet
            );

            return result.ExitCode != 0 ? result.Output : null;
        }

        public static string Pull(string path, bool quiet = false)
        {
            var result = SystemHelper.ExecuteApplication(
                "git",
                $"-C {path} pull --all",
                null,
                quiet
            );
            return result.ExitCode != 0 ? result.Output : null;
        }

        public static bool TagExists(string commerceRepo, string line)
        {
            var result = SystemHelper.ExecuteApplication(
                "git",
                $"-" + $"C {commerceRepo} tag -l {line}"
            );
            return result.Output.ContainsIgnoreCase(line);
        }
    }
}
