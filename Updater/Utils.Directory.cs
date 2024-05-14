using P3tr0viCh.Utils;
using System.IO;
using System;

namespace Updater
{
    public static partial class Utils
    {
        public static class Directory
        {
            public const string TempDirName = "temp";
            public const string LatestDirName = "latest";

            public static void Rename(string sourceDirFullName, string destDirOnlyName)
            {
                WriteDebug($"{sourceDirFullName} > {destDirOnlyName}");

                Files.DirectoryRename(sourceDirFullName, destDirOnlyName);
            }

            public static void Move(string sourceDirName, string destDirName)
            {
                WriteDebug($"{sourceDirName} > {destDirName}");

                System.IO.Directory.Move(sourceDirName, destDirName);
            }
            
            public static void Delete(string path)
            {
                WriteDebug($"{path}");

                Files.DirectoryDelete(path);
            }

            public static string CreateTemp(string localPath)
            {
                var tempDir = Path.Combine(localPath, TempDirName);

                Files.DirectoryDelete(tempDir);

                System.IO.Directory.CreateDirectory(tempDir);

                return tempDir;
            }

            public static string CreateLatest(string localPath, string localName)
            {
                var latestDir = Path.Combine(localPath, LatestDirName);

                if (System.IO.Directory.Exists(latestDir))
                {
                    var fileName = Path.Combine(latestDir, localName);

                    if (File.Exists(fileName))
                    {
                        var version = GetFileVersion(fileName);

                        var versionName = version.ToString();

                        var versionDir = Path.Combine(localPath, versionName);

                        if (System.IO.Directory.Exists(versionDir))
                        {
                            versionName += "_" + DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
                        }

                        Rename(latestDir, versionName);
                    }
                }

                return latestDir;
            }
        }
    }
}