using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;

namespace NbtStudio
{
    public class GitHubVersion : Version
    {
        private readonly string ChangelogURL;
        public string Changelog { get; private set; }
        private readonly List<(string name, string url)> Assets = new();
        public GitHubVersion(JObject json)
        {
            string tag_name = json["tag_name"]?.ToString();
            if (tag_name is null)
                throw new InvalidDataException("Couldn't find tag name");
            ParseDots(tag_name[(1 + tag_name.IndexOf('v'))..]);
            var assets = json["assets"] as JArray;
            if (assets is not null)
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
            if (Changelog is null && ChangelogURL is not null)
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
