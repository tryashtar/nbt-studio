using System;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace NbtStudio
{
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
}
