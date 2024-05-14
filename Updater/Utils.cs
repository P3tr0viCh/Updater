using System;
using System.Diagnostics;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Updater
{
    public static partial class Utils
    {
        public static Version GetFileVersion(string filePath)
        {
            Version version = null;

            try
            {
                version = new Version(FileVersionInfo.GetVersionInfo(filePath).FileVersion);
            }
            catch (Exception e)
            {
                WriteError(e);
            }

            WriteDebug($"file: {filePath}, version: {version}");

            return version;
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