using Newtonsoft.Json;
using P3tr0viCh.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows.Forms;
using Updater.Properties;

namespace Updater
{
    public partial class Main : Form
    {
        private Info Info;

        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            lblReleaseVersion.Text = string.Empty;

            Info = new Info(Files.SettingsFileName());

            if (!LoadInfo())
            {
                Close();

                return;
            }

            CheckLocalVersion();

            CheckReleaseVersionAsync();
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private bool LoadInfo()
        {
            var msg = string.Empty;

            try
            {
                Info.Load();
            }
            catch (FileNotFoundException)
            {
                msg = Resources.ErrorFileInfoNotExists;
            }
            catch (Exception)
            {
                msg = Resources.ErrorFileInfoBad;
            }

            if (!string.IsNullOrEmpty(msg))
            {
                Msg.Error(msg);

                return false;
            }

            lblName.Text = Info.Name;

            return true;
        }

        private void OpenPath(string path)
        {
            try
            {
                Process.Start(path);
            }
            catch (Exception e)
            {
                Msg.Error(e.Message);
            }
        }

        private void LinkLocal_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenPath(Info.LocalPath);
        }

        private void LinkRelease_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var baseUri = new Uri(Resources.GitHubUrl);
            var uri = new Uri(baseUri, $"{Info.ReleaseOwner}/{Info.ReleaseRepo}/releases/latest");

            OpenPath(uri.AbsoluteUri);
        }

        private void CheckLocalVersion()
        {
            lblLocalVersion.Text = Resources.TextReadingVersion;

            btnUpdate.Text = Resources.TextBtnInstall;

            try
            {
                var directories = Directory.GetDirectories(Info.LocalPath);

                if (directories.Length == 0)
                {
                    lblLocalVersion.Text = Resources.TextLocalNotExists;

                    btnUpdate.Text = Resources.TextBtnInstall;

                    return;
                }
#if DEBUG
                var names = string.Join(", ", directories);
                Debug.WriteLine($"local directories: {names}");
#endif
            }
            catch (Exception e)
            {
                Msg.Error(e.Message);
            }
        }

        private class GitHubTags
        {
            public string name = string.Empty;
        }

        private async void CheckReleaseVersionAsync()
        {
            lblReleaseVersion.Text = Resources.TextReadingVersion;

            try
            {
                using (var client = new HttpClient(new HttpClientHandler
                {
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                }))
                {
                    client.BaseAddress = new Uri(Resources.GitHubApiUrl);

                    var assemblyDecorator = new Misc.AssemblyDecorator();

                    var header = new ProductHeaderValue(Files.ExecutableName(), assemblyDecorator.Version.ToString());

                    client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(header));

                    if (assemblyDecorator.IsDebug)
                    {
                        client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("(debug build)"));
                    }

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));

                    var response = await client.GetAsync($"/repos/{Info.ReleaseOwner}/{Info.ReleaseRepo}/tags");

                    response.EnsureSuccessStatusCode();

                    var result = await response.Content.ReadAsStringAsync();

                    Debug.WriteLine(result);

                    var tags = JsonConvert.DeserializeObject<List<GitHubTags>>(result);

#if DEBUG                    
                    var names = string.Join(", ", tags.Select(tag => tag.name));
                    Debug.WriteLine($"github tags: {names}");
#endif

                    if (tags == null || tags.Count == 0)
                    {
                        throw new Exception("empty tags");
                    }

                    lblReleaseVersion.Text = tags[0].name;
                }
            }
            catch (Exception e)
            {
                Msg.Error(e.Message);
            }
        }
    }
}
