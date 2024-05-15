using P3tr0viCh.Utils;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Windows.Forms.Design;
using static P3tr0viCh.Utils.Converters;

namespace Updater
{
    public class InfoBase
    {
        public class GitHubRelease
        {
            [TypeConverter(typeof(ExpandableObjectEmptyConverter))]
            public GitHub.Project Project { get; } = new GitHub.Project();
            [DisplayName("Архив с обновлением")]
            [Description("")]
            public string File { get; set; } = string.Empty;
        }

        [Category("Общее")]
        [DisplayName("Программа")]
        [Description("Расположение исполняемого файла.\n" +
            "Файл (и все остальные компоненты обновляемой программы) должен находиться в каталоге «latest», " +
            "который располагается в каталоге обновляемой программы.\n" +
            "Например:\n" +
            "c:\\Program Files\\Updater\\latest\\Updater.exe")]
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string LocalFile { get; set; }

        [Category("Расположение обновления")]
        [DisplayName("ГитХаб")]
        [Description("")]
        [TypeConverter(typeof(ExpandableObjectEmptyConverter))]
        public GitHubRelease GitHubReleaseInfo { get; } = new GitHubRelease();

        public void Check()
        {
            if (LocalFile.IsEmpty() ||
                GitHubReleaseInfo is null ||
                GitHubReleaseInfo.Project is null ||
                GitHubReleaseInfo.File.IsEmpty() ||
                GitHubReleaseInfo.Project.Owner.IsEmpty() ||
                GitHubReleaseInfo.Project.Repo.IsEmpty()) throw new NullReferenceException();

            if (!File.Exists(LocalFile)) throw new FileNotFoundException();

            var parentDir = Utils.Directory.GetParentName(LocalFile);

            if (parentDir != Utils.Directory.LatestDirName) throw new FileNotFoundException("file wrong location");
        }
    }
}