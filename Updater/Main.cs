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
        private Info Info;

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
            lblLocalVersion.Text = string.Empty;
            lblReleaseVersion.Text = string.Empty;

            Info = new Info(Files.SettingsFileName());

            ProgramStatus.StatusChanged += ProgramStatusStatusChanged;

            if (!LoadInfo())
            {
                Close();

                return;
            }

            Check();
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
                        Operation = Operation.Update;
                    }
                }
            }
            catch (Exception e)
            {
                Operation = Operation.Check;

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
            if (localVersion is null)
            {
                OpenPath(Info.LocalPath);
            }
            else
            {
                OpenPath(Path.Combine(Info.LocalPath, Utils.Directory.LatestDirName));
            }
        }

        private void LinkRelease_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var baseUri = new Uri(Resources.GitHubUrl);
            var uri = new Uri(baseUri, $"{Info.ReleaseOwner}/{Info.ReleaseRepo}/releases/latest");

            OpenPath(uri.AbsoluteUri);
        }

        private void CheckLocalVersion()
        {
            var starting = ProgramStatus.Start(Status.CheckLocal);

            localVersion = null;

            try
            {
                var currentFileName = Path.Combine(Info.LocalPath, Utils.Directory.LatestDirName, Info.LocalFile);

                if (File.Exists(currentFileName))
                {
                    localVersion = Utils.GetFileVersion(currentFileName);
                }
            }
            finally
            {
                ProgramStatus.Stop(starting);
            }
        }

        private async Task CheckReleaseVersionAsync()
        {
            var starting = ProgramStatus.Start(Status.CheckRelease);

            releaseVersion = null;

            try
            {
                releaseVersionGitHub = await Utils.Http.CheckReleaseVersionAsync(Info.ReleaseOwner, Info.ReleaseRepo);

                releaseVersion = new Version(releaseVersionGitHub);
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
                await Utils.Http.DownloadAsync(Info.ReleaseOwner, Info.ReleaseRepo,
                    releaseVersionGitHub, Info.ReleaseFile, archiveFileName);
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
            var starting = ProgramStatus.Start(Status.InstallOrUpdate);

            try
            {
                var archiveFileName = Path.Combine(Info.LocalPath, Info.ReleaseFile);

                await DownloadAsync(archiveFileName);

                var tempDir = Utils.Directory.CreateTemp(Info.LocalPath);

                await ZipExtractAsync(archiveFileName, tempDir);

                var moveDir = tempDir;

                if (!File.Exists(Path.Combine(tempDir, Info.LocalFile)))
                {
                    var directories = Directory.GetDirectories(tempDir);

                    if (directories.Length == 1)
                    {
                        if (File.Exists(Path.Combine(directories[0], Info.LocalFile)))
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

                var latestDir = Utils.Directory.CreateLatest(Info.LocalPath, Info.LocalFile);

                Utils.Directory.Move(moveDir, latestDir);

                Utils.Directory.Delete(tempDir);

#if !DEBUG
                Files.DirectoryDelete(archiveFileName);
#endif
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