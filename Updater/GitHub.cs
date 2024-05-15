using Newtonsoft.Json;
using P3tr0viCh.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Updater
{
    public static class GitHub
    {
        private const string Url = "https://github.com";
        private const string UrlApi = "https://api.github.com";

        private const string UrlLatestRelease = "{0}/{1}/releases/latest";
        private const string UrlTags = "/repos/{0}/{1}/tags";
        private const string UrlDownloadFile = "/{0}/{1}/releases/download/{2}/{3}";

        private const string MediaTypeTags = "application/vnd.github+json";
        private const string MediaTypeDownload = "application/vnd.github.raw+json";

        [DisplayName("Проект")]
        public class Project
        {
            [DisplayName("Владелец")]
            [Description("Owner")]
            public string Owner { get; set; } = string.Empty;
            [DisplayName("Репозиторий")]
            public string Repo { get; set; } = string.Empty;
        }

        private class Tags
        {
            public string name = string.Empty;
        }

        public static Uri GetLatestRelease(Project project)
        {
            var baseUri = new Uri(Url);
            return new Uri(baseUri, string.Format(UrlLatestRelease, project.Owner, project.Repo));
        }

        public static async Task<string> GetReleaseVersionAsync(Project project)
        {
            using (var client = new HttpClient(new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            }))
            {
                Http.SetClientBaseAddress(client, UrlApi);
                Http.SetClientHeader(client);
                Http.SetClientMediaType(client, MediaTypeTags);

                Utils.WriteDebug(client.DefaultRequestHeaders.UserAgent.ToString());

                var requestUri = string.Format(UrlTags, project.Owner, project.Repo);

                Utils.WriteDebug(requestUri);

                var response = await client.GetAsync(requestUri);

                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsStringAsync();

                Utils.WriteDebug(result);

                var tags = JsonConvert.DeserializeObject<List<Tags>>(result);

#if DEBUG
                var names = string.Join(", ", tags.Select(tag => tag.name));
                Utils.WriteDebug($"github tags: {names}");
#endif

                if (tags == null || tags.Count == 0)
                {
                    throw new Exception("empty tags");
                }

                return tags[0].name;
            }
        }

        public static async Task DownloadAsync(Project project, string version,
            string fileName, string downloadFileName)
        {
            using (var client = new HttpClient())
            {
                Http.SetClientBaseAddress(client, Url);
                Http.SetClientHeader(client);
                Http.SetClientMediaType(client, MediaTypeDownload);

                var downloadPath = string.Format(UrlDownloadFile, project.Owner, project.Repo, version, fileName);

                Utils.WriteDebug(downloadPath);

                using (var response = await client.GetAsync(downloadPath))
                {
                    response.EnsureSuccessStatusCode();

                    using (var stream = await response.Content.ReadAsStreamAsync())
                    using (var fileStream = new FileStream(downloadFileName, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        await stream.CopyToAsync(fileStream);
                    }
                }
            }
        }
    }
}