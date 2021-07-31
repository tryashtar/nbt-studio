using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using TryashtarUtils.Utility;

namespace NbtStudio
{
    public class Updater
    {
        private Task<IFailable<AvailableUpdate>> UpdateChecker;
        public IFailable<AvailableUpdate> Update { get; private set; }

        public void StartCheckingAsync()
        {
            if (UpdateChecker is not null && !UpdateChecker.IsCompleted)
                return;
            UpdateChecker = new Task<IFailable<AvailableUpdate>>(() => new Failable<AvailableUpdate>(CheckForUpdates, "Checking for update)"));
            UpdateChecker.Start();
            UpdateChecker.ContinueWith(x =>
            {
                Update = x.Result;
            });
        }

        public void ContinueWith(Action<IFailable<AvailableUpdate>> callback)
        {
            if (UpdateChecker is null)
                return;
            UpdateChecker.ContinueWith(x => callback(x.Result), TaskScheduler.FromCurrentSynchronizationContext());
        }

        private AvailableUpdate CheckForUpdates()
        {
            var old = AvailableUpdate.TemporaryPath(Application.ExecutablePath);
            if (File.Exists(old))
                File.Delete(old);
            var current = GetCurrentVersion();
            var client = new WebClient();
            client.Headers.Add(HttpRequestHeader.UserAgent, "NBT Studio update checker");
            bool success = false;
            try
            {
                var versions = GetAllVersions(client);
                var newer = GetNewerVersions(current, versions);
                if (!newer.Any())
                    return null;
                var latest = GetLatestVersion(newer);
                string changelog = MergeChangelogs(client, newer);
                success = true;
                return new AvailableUpdate(latest, client, changelog);
            }
            finally
            {
                // if we succeeded, the AvailableUpdate still needs to use the web client
                if (!success)
                    client.Dispose();
            }
        }

        private static string MergeChangelogs(WebClient client, IEnumerable<GitHubVersion> versions)
        {
            var builder = new StringBuilder();
            foreach (var version in versions)
            {
                string log = version.DownloadChangelog(client);
                if (log is not null)
                {
                    builder.Append(version.ToString(false) + ":" + Environment.NewLine);
                    builder.Append(log);
                    if (!log.EndsWith("\n"))
                        builder.Append(Environment.NewLine);
                    builder.Append(Environment.NewLine);
                }
            }
            return builder.ToString();
        }

        public static Version GetCurrentVersion()
        {
            return new Version(Application.ProductVersion);
        }

        public static string GitHubUrl()
        {
            return "https://github.com/tryashtar/nbt-studio/releases";
        }

        public static List<GitHubVersion> GetAllVersions(WebClient client)
        {
            var versions = new List<GitHubVersion>();
            var uri = new Uri("https://api.github.com/repos/tryashtar/nbt-studio/releases");
            string data = client.DownloadString(uri);
            var releases = JArray.Parse(data);
            foreach (JObject item in releases)
            {
                try { versions.Add(new GitHubVersion(item)); }
                catch { }
            }
            return versions;
        }

        public static T GetLatestVersion<T>(IEnumerable<T> versions) where T : Version
        {
            T latest = null;
            foreach (var version in versions)
            {
                if (latest is null || version > latest)
                    latest = version;
            }
            return latest;
        }

        public static IEnumerable<T> GetNewerVersions<T>(Version current, IEnumerable<T> all) where T : Version
        {
            return all.Where(x => x > current).OrderByDescending(x => x);
        }
    }
}
