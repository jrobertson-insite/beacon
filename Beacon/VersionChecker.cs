using System;
using System.Net.Http;
using Newtonsoft.Json;

namespace Beacon
{
    public class VersionChecker
    {
        public static void CheckForNewVersion()
        {
            var httpClient = new HttpClient();
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                "https://api.nuget.org/v3/registration5-semver1/beacon/index.json"
            );
            var response = httpClient.SendAsync(request).Result;
            var result = JsonConvert.DeserializeObject<NugetResponse>(
                response.Content.ReadAsStringAsync().Result
            );
            var latestVersion = new Version(result.Items[0].Upper);
            var currentVersion = typeof(VersionChecker).Assembly.GetName().Version;
            if (latestVersion.CompareTo(currentVersion) > 0)
            {
                Console.WriteLine(
                    $"You are currently running version {currentVersion} of beacon. A new version {latestVersion} is available."
                );
                Console.WriteLine("Run the following to get it.");
                Console.WriteLine("dotnet tool update -g beacon");
                Console.WriteLine("Press any key to continue");
                Console.ReadKey(true);
            }
        }

        private class NugetResponse
        {
            public NugetItem[] Items { get; set; }
        }

        private class NugetItem
        {
            public string Upper { get; set; }
        }
    }
}
