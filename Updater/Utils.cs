using System;
using System.Diagnostics;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Updater
{
    public static partial class Utils
    {
        public static FileVersionInfo GetFileVersionInfo(string filePath)
        {
            FileVersionInfo info = null;

            try
            {
                info = FileVersionInfo.GetVersionInfo(filePath);
            }
            catch (Exception e)
            {
                WriteError(e);
            }

            return info;
        }

        public static string GetFileTitle(FileVersionInfo info)
        {
            var title = string.Empty;

            try
            {
                return info.FileDescription;
            }
            catch (Exception e)
            {
                WriteError(e);
            }

            return title;
        }

        public static Version GetFileVersion(FileVersionInfo info)
        {
            Version version = null;

            try
            {
                version = new Version(info.FileVersion);
            }
            catch (Exception e)
            {
                WriteError(e);
            }

            return version;
        }
        
        public static Version GetFileVersion(string filePath)
        {
            return GetFileVersion(GetFileVersionInfo(filePath));
        }

        public static void WriteDebug(string s, [CallerMemberName] string memberName = "")
        {
            Debug.WriteLine(memberName + ": " + s);
        }

        public static void WriteError(Exception e, [CallerMemberName] string memberName = "")
        {
            if (e == null) return;

            WriteError(e.Message, memberName);

            WriteError(e.InnerException, memberName);
        }

        public static void WriteError(string err, [CallerMemberName] string memberName = "")
        {
            var error = string.Format("{0} fail: {1}", memberName, err);

            Debug.WriteLine(error);
        }

        public static async Task ZipExtractAsync(string archiveFileName, string destinationDir)
        {
            WriteDebug($"{archiveFileName} > {destinationDir}");

            try
            {
                await Task.Run(() =>
                {
                    ZipFile.ExtractToDirectory(archiveFileName, destinationDir);
                });
            }
            finally
            {
                WriteDebug("done");
            }
        }
    }
}