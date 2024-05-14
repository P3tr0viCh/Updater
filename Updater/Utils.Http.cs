using P3tr0viCh.Utils;
using System.Net.Http.Headers;
using System.Net.Http;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using Updater.Properties;
using System.Threading.Tasks;
using System.Linq;
using System.IO;

namespace Updater
{
    public static partial class Utils
    {
        public static class Http
        {
            private static void SetClientBaseAddress(HttpClient client, string address)
            {
                client.BaseAddress = new Uri(address);
            }

            private static void SetClientHeader(HttpClient client)
            {
                var assemblyDecorator = new Misc.AssemblyDecorator();

                var header = new ProductHeaderValue(Files.ExecutableName(), assemblyDecorator.Version.ToString());

                client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(header));

                if (assemblyDecorator.IsDebug)
                {
                    client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("(debug build)"));
                }
            }

            private static void SetClientMediaType(HttpClient client, string mediaType)
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
            }

            private class GitHubTags
            {
                public string name = string.Empty;
            }

            public static async Task<string> CheckReleaseVersionAsync(string owner, string repo)
            {
                using (var client = new HttpClient(new HttpClientHandler
                {
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                }))
                {
                    SetClientBaseAddress(client, Resources.GitHubApiUrl);
                    SetClientHeader(client);
                    SetClientMediaType(client, "application/vnd.github+json");

                    var response = await client.GetAsync($"/repos/{owner}/{repo}/tags");

                    response.EnsureSuccessStatusCode();

                    var result = await response.Content.ReadAsStringAsync();

                    WriteDebug(result);

                    var tags = JsonConvert.DeserializeObject<List<GitHubTags>>(result);

#if DEBUG
                    var names = string.Join(", ", tags.Select(tag => tag.name));
                    WriteDebug($"github tags: {names}");
#endif

                    if (tags == null || tags.Count == 0)
                    {
                        throw new Exception("empty tags");
                    }

                    return tags[0].name;
                }
            }

            public static async Task DownloadAsync(string owner, string repo, string version,
                string fileName, string downloadFileName)
            {
                using (var client = new HttpClient())
                {
                    SetClientBaseAddress(client, Resources.GitHubUrl);
                    SetClientHeader(client);
                    SetClientMediaType(client, "application/vnd.github.raw+json");

                    var downloadPath = $"/{owner}/{repo}/releases/download/{version}/{fileName}";

                    WriteDebug(downloadPath);

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
}