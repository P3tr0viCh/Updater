using P3tr0viCh.Utils;
using System.IO;
using System;
using Updater.Properties;

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

            public static string CreateTemp(string programRoot)
            {
                var tempDir = Path.Combine(programRoot, TempDirName);

                WriteDebug(tempDir);

                Files.DirectoryDelete(tempDir);

                System.IO.Directory.CreateDirectory(tempDir);

                return tempDir;
            }

            public static string CreateMoveDir(string tempDir, string fileName)
            {
                WriteDebug(tempDir);

                var moveDir = tempDir;

                var fileNameOnly = Path.GetFileName(fileName);

                if (!File.Exists(Path.Combine(tempDir, fileNameOnly)))
                {
                    var directories = System.IO.Directory.GetDirectories(tempDir);

                    if (directories.Length == 1)
                    {
                        if (File.Exists(Path.Combine(directories[0], fileNameOnly)))
                        {
                            moveDir = directories[0];
                        }
                        else
                        {
                            throw new FileNotFoundException(Resources.ExceptionFileNotFoundInArchive);
                        }
                    }
                    else
                    {
                        throw new FileNotFoundException(Resources.ExceptionFileNotFoundInArchive);
                    }
                }
                
                WriteDebug(moveDir);

                return moveDir;
            }

            public static string CreateLatest(string programRoot, string fileName)
            {
                WriteDebug(programRoot);

                var latestDir = Path.Combine(programRoot, LatestDirName);

                var versionName = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

                if (File.Exists(fileName))
                {
                    var version = GetFileVersion(fileName);

                    versionName = version.ToString();

                    var versionDir = Path.Combine(programRoot, versionName);

                    if (System.IO.Directory.Exists(versionDir))
                    {
                        versionName += "_" + DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
                    }

                }

                Rename(latestDir, versionName);

                return latestDir;
            }

            public static string GetParentName(string fileName)
            {
                return Path.GetFileName(Path.GetDirectoryName(fileName));
            }
        }
    }
}