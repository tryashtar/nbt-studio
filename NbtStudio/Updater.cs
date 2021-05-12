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

namespace NbtStudio
{
    public static class Updater
    {
        public static AvailableUpdate CheckForUpdates()
        {
#if DEBUG
            return null;
#endif
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
                if (log != null)
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
            return new Version(Assembly.GetExecutingAssembly().GetName().Version);
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
                if (latest == null || version > latest)
                    latest = version;
            }
            return latest;
        }

        public static IEnumerable<T> GetNewerVersions<T>(Version current, IEnumerable<T> all) where T : Version
        {
            return all.Where(x => x > current).OrderByDescending(x => x);
        }
    }

    public class AvailableUpdate : IDisposable
    {
        public readonly GitHubVersion Version;
        public readonly string Changelog;
        private readonly WebClient Client;
        public AvailableUpdate(GitHubVersion version, WebClient client, string changelog)
        {
            Version = version;
            Changelog = changelog;
            Client = client;
        }

        public void Update()
        {
            string path = Application.ExecutablePath;
            string temporary = TemporaryPath(path);
            if (File.Exists(temporary))
                File.Delete(temporary);
            File.Move(path, temporary);
            Version.DownloadAssets(Client, Path.GetDirectoryName(path));
        }

        public static string TemporaryPath(string exe)
        {
            var folder = Path.GetDirectoryName(exe);
            var name = Path.GetFileNameWithoutExtension(exe);
            var ext = Path.GetExtension(exe);
            return Path.Combine(folder, name + "_OLD" + ext);
        }

        public void Decline()
        {
            Dispose();
        }

        public void Dispose()
        {
            Client.Dispose();
        }
    }

    public class Version : IComparable<Version>
    {
        private int[] Dots;
        public Version(params int[] dots)
        {
            Dots = dots;
        }

        public Version(string str)
        {
            ParseDots(str);
        }

        public Version(System.Version version)
        {
            Dots = new int[] { version.Major, version.Minor, version.Build, version.Revision };
        }

        protected void ParseDots(string str)
        {
            Dots = str.Split('.').Select(x => int.Parse(x)).ToArray();
        }

        public static bool operator >(Version v1, Version v2)
        {
            return v1.CompareTo(v2) > 0;
        }

        public static bool operator <(Version v1, Version v2)
        {
            return v1.CompareTo(v2) < 0;
        }

        public int CompareTo(Version other)
        {
            for (int i = 0; i < Math.Min(Dots.Length, other.Dots.Length); i++)
            {
                int compare = Dots[i].CompareTo(other.Dots[i]);
                if (compare != 0)
                    return compare;
            }
            return Dots.Length.CompareTo(other.Dots.Length);
        }

        public override string ToString() => ToString(true);
        public string ToString(bool trailing)
        {
            if (trailing)
                return String.Join(".", Dots);
            else
            {
                int stop = Dots.Length;
                for (int i = Dots.Length - 1; i >= 0; i--)
                {
                    if (Dots[i] != 0)
                    {
                        stop = i + 1;
                        break;
                    }
                }
                return String.Join(".", Dots.Take(stop));
            }
        }
    }

    public class GitHubVersion : Version
    {
        private readonly string ChangelogURL;
        public string Changelog { get; private set; }
        private readonly List<(string name, string url)> Assets = new List<(string, string)>();
        public GitHubVersion(JObject json)
        {
            string tag_name = json["tag_name"]?.ToString();
            if (tag_name == null)
                throw new InvalidDataException("Couldn't find tag name");
            ParseDots(tag_name.Substring(1 + tag_name.IndexOf('v')));
            var assets = json["assets"] as JArray;
            if (assets != null)
            {
                foreach (JObject item in assets)
                {
                    string name = item["name"].ToString();
                    string address = item["browser_download_url"].ToString();
                    if (name.Contains("changelog"))
                        ChangelogURL = address;
                    else
                        Assets.Add((name, address));
                }
            }
        }

        public string DownloadChangelog(WebClient client)
        {
            if (Changelog == null && ChangelogURL != null)
            {
                var uri = new Uri(ChangelogURL);
                Changelog = client.DownloadString(uri);
            }
            return Changelog;
        }

        public void DownloadAssets(WebClient client, string folder)
        {
            foreach (var (name, url) in Assets)
            {
                var uri = new Uri(url);
                client.DownloadFile(uri, Path.Combine(folder, name));
            }
        }
    }
}
