using P3tr0viCh.Utils;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Updater.Properties;
using static Updater.Enums;

namespace Updater
{
    public partial class Main : Form
    {
        private class Info : SettingsBase<InfoBase> { }

        private Version localVersion = null;
        private Version releaseVersion = null;

        private string releaseVersionGitHub = string.Empty;

        private readonly ProgramStatus status = new ProgramStatus();
        public ProgramStatus ProgramStatus { get { return status; } }

        private Operation operation;
        public Operation Operation
        {
            get
            {
                return operation;
            }
            set
            {
                operation = value;

                switch (operation)
                {
                    case Operation.Check:
                        btnOperation.Text = Resources.TextBtnCheck;
                        break;
                    case Operation.Update:
                        btnOperation.Text = Resources.TextBtnUpdate;
                        break;
                    case Operation.Install:
                        btnOperation.Text = Resources.TextBtnInstall;
                        break;
                }
            }
        }

        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            lblName.Text = Resources.TextUpdaterName;

            lblLocalVersion.Text = string.Empty;
            lblReleaseVersion.Text = string.Empty;

            Info.Directory = Files.ExecutableDirectory();
            Info.FileName = Files.SettingsFileName();

            //File.Delete(Info.FileName);

            ProgramStatus.StatusChanged += ProgramStatusStatusChanged;

            if (!LoadAndCheck())
            {
                Close();
            }
        }

        private bool LoadAndCheck()
        {
            while (!LoadInfo())
            {
                if (!CreateInfo())
                {
                    return false;
                }
            }

            Check();

            return true;
        }

        private bool CreateInfo()
        {
            if (Msg.Question(Resources.QuestionFileInfoBad))
            {
                if (FrmInfo.ShowDlg(this))
                {
                    return true;
                }
            }

            return false;
        }

        private async void Check()
        {
            Operation = Operation.Check;

            try
            {
                CheckLocalVersion();

                await CheckReleaseVersionAsync();

                if (localVersion is null && releaseVersion is null)
                {
                    Operation = Operation.Check;
                }
                else
                {
                    if (localVersion is null)
                    {
                        Operation = Operation.Install;
                    }
                    else
                    {
                        if (releaseVersion is null)
                        {
                            Operation = Operation.Check;
                        }
                        else
                        {
                            Operation = Operation.Update;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Operation = Operation.Check;

                Utils.WriteError(e);

                Msg.Error(e.Message);
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ProgramStatus.Current != Status.Idle)
            {
                if (!Msg.Question(Resources.QuestionClosing))
                {
                    e.Cancel = true;
                }
            }
        }

        private void ProgramStatusStatusChanged(object sender, Status status)
        {
            if (status == Status.Idle)
            {
                btnOperation.Enabled = true;
                btnOperation.Text = Resources.TextBtnCheck;

                lblLocalVersion.Text = localVersion is null ? Resources.TextVersionNotExists : localVersion.ToString();
                lblReleaseVersion.Text = releaseVersion is null ? Resources.TextVersionNotExists : releaseVersion.ToString();
            }
            else
            {
                btnOperation.Enabled = false;

                switch (status)
                {
                    case Status.CheckLocal:
                        lblLocalVersion.Text = Resources.TextVersionReading;
                        break;
                    case Status.CheckRelease:
                        lblReleaseVersion.Text = Resources.TextVersionReading;
                        break;
                    case Status.Download:
                        lblReleaseVersion.Text = Resources.TextVersionDownloading;
                        break;
                    case Status.ZipExtract:
                        lblReleaseVersion.Text = Resources.TextVersionZipExtracting;
                        break;
                }
            }
        }

        private bool LoadInfo()
        {
            try
            {
                Info.Load();
                Info.Default.Check();
            }
            catch (Exception e)
            {
                Utils.WriteError(e);

                return false;
            }

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
                Utils.WriteError(e);

                Msg.Error(e.Message);
            }
        }

        private void LinkLocal_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenPath(Path.GetDirectoryName(Info.Default.LocalFile));
        }

        private void LinkRelease_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var uri = GitHub.GetLatestRelease(Info.Default.GitHubReleaseInfo.Project);

            OpenPath(uri.AbsoluteUri);
        }

        private void CheckLocalVersion()
        {
            Utils.WriteDebug("start");

            var starting = ProgramStatus.Start(Status.CheckLocal);

            localVersion = null;

            try
            {
                lblName.Text = Path.GetFileNameWithoutExtension(Info.Default.LocalFile);

                if (File.Exists(Info.Default.LocalFile))
                {
                    var info = Utils.GetFileVersionInfo(Info.Default.LocalFile);

                    if (info == null)
                    {
                        return;
                    }

                    lblName.Text = Utils.GetFileTitle(info);

                    localVersion = Utils.GetFileVersion(info);
                }
            }
            finally
            {
                ProgramStatus.Stop(starting);
            }
        }

        private async Task CheckReleaseVersionAsync()
        {
            Utils.WriteDebug("start");

            var starting = ProgramStatus.Start(Status.CheckRelease);

            releaseVersion = null;

            try
            {
                releaseVersionGitHub = await GitHub.GetReleaseVersionAsync(Info.Default.GitHubReleaseInfo.Project);

                var tempVersion = new Version(releaseVersionGitHub);

                releaseVersion = new Version(tempVersion.Major, tempVersion.Minor,
                    tempVersion.Build == -1 ? 0 : tempVersion.Build,
                    tempVersion.Revision == -1 ? 0 : tempVersion.Revision);
            }
            finally
            {
                ProgramStatus.Stop(starting);
            }
        }

        private void BtnOperation_Click(object sender, EventArgs e)
        {
            switch (Operation)
            {
                case Operation.Check:
                    Check();
                    break;
                case Operation.Update:
                case Operation.Install:
                    InstallOrUpdate();
                    break;
            }
        }

        private async Task DownloadAsync(string archiveFileName)
        {
            var starting = ProgramStatus.Start(Status.Download);

            try
            {
                await GitHub.DownloadAsync(Info.Default.GitHubReleaseInfo.Project,
                    releaseVersionGitHub, Info.Default.GitHubReleaseInfo.File, archiveFileName);
            }
            finally
            {
                Utils.WriteDebug("done");

                ProgramStatus.Stop(starting);
            }
        }

        private async Task ZipExtractAsync(string archiveFileName, string destinationDir)
        {
            var starting = ProgramStatus.Start(Status.ZipExtract);

            try
            {
                await Utils.ZipExtractAsync(archiveFileName, destinationDir);
            }
            finally
            {
                ProgramStatus.Stop(starting);
            }
        }

        private async void InstallOrUpdate()
        {
            if (releaseVersion.CompareTo(localVersion) != 1)
            {
                if (!Msg.Question(Resources.QuestionVersionCompare))
                {
                    return;
                }
            }

            var starting = ProgramStatus.Start(Status.InstallOrUpdate);

            try
            {
                var programRoot = Path.GetDirectoryName(Path.GetDirectoryName(Info.Default.LocalFile));

                var archiveFileName = Path.Combine(programRoot, Info.Default.GitHubReleaseInfo.File);

                await DownloadAsync(archiveFileName);

                var tempDir = Utils.Directory.CreateTemp(programRoot);

                await ZipExtractAsync(archiveFileName, tempDir);

                var moveDir = Utils.Directory.CreateMoveDir(tempDir, Info.Default.LocalFile);

                var latestDir = Utils.Directory.CreateLatest(programRoot, Info.Default.LocalFile);

                Utils.Directory.Move(moveDir, latestDir);

                Utils.Directory.Delete(tempDir);

#if !DEBUG
                Files.DirectoryDelete(archiveFileName);
#endif
                localVersion = releaseVersion;
            }
            catch (Exception e)
            {
                Utils.WriteError(e);

                Msg.Error(e.Message);
            }
            finally
            {
                Utils.WriteDebug("done");

                ProgramStatus.Stop(starting);

                Operation = Operation.Check;
            }
        }
    }
}