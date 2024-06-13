using P3tr0viCh.AppUpdate;
using P3tr0viCh.Utils;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Updater.Properties;
using static Updater.Enums;

namespace Updater
{
    public partial class Main : Form
    {
        private class Config : SettingsBase<P3tr0viCh.AppUpdate.Config> { }

        private readonly AppUpdate AppUpdate = new AppUpdate();

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
            lblLatestVersion.Text = string.Empty;

            Config.Directory = Files.ExecutableDirectory();
            Config.FileName = Files.SettingsFileName();

            //File.Delete(Info.FileName);

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
            var fileExists = File.Exists(Config.FilePath);

            if (!fileExists || Msg.Question(Resources.QuestionFileInfoBad))
            {
                if (FrmConfig.ShowDlg(this))
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
                AppUpdate.CheckLocalVersion();

                await AppUpdate.CheckLatestVersionAsync();

                if (AppUpdate.Versions.Local is null && AppUpdate.Versions.Latest is null)
                {
                    Operation = Operation.Check;
                }
                else
                {
                    if (AppUpdate.Versions.Latest is null)
                    {
                        Operation = Operation.Check;
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

                DebugWrite.Error(e);

                Msg.Error(e.Message);
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!AppUpdate.Status.IsIdle())
            {
                if (!Msg.Question(Resources.QuestionClosing))
                {
                    e.Cancel = true;
                }
            }
        }

        private bool LoadInfo()
        {
            try
            {
                if (!Config.Load())
                {
                    throw Config.LastError;
                }

                AppUpdate.Config = Config.Default;

                AppUpdate.Check();

                AppUpdate.Status.StatusChanged += Update_StatusChanged;
            }
            catch (Exception e)
            {
                DebugWrite.Error(e);

                return false;
            }

            return true;
        }

        private void Update_StatusChanged(object sender, UpdateStatus status)
        {
            DebugWrite.Line(status.ToString());

            btnOperation.Enabled = status == UpdateStatus.Idle;

            switch (status)
            {
                case UpdateStatus.Idle:
                    btnOperation.Text = Resources.TextBtnCheck;

                    lblLocalVersion.Text = AppUpdate.Versions.Local is null ?
                        Resources.TextVersionNotExists : AppUpdate.Versions.Local.ToString();
                    lblLatestVersion.Text = AppUpdate.Versions.Latest is null ?
                        Resources.TextVersionNotExists : AppUpdate.Versions.Latest.ToString();

                    break;
                case UpdateStatus.Check:
                    break;
                case UpdateStatus.CheckLocal:
                    lblName.Text = Misc.GetFileTitle(AppUpdate.Config.LocalFile);

                    lblLocalVersion.Text = Resources.TextVersionReading;

                    break;
                case UpdateStatus.CheckLatest:
                    lblLatestVersion.Text = Resources.TextVersionReading;

                    break;
                case UpdateStatus.Download:
                    lblLatestVersion.Text = Resources.TextVersionDownloading;

                    break;
                case UpdateStatus.ArchiveExtract:
                    lblLatestVersion.Text = Resources.TextVersionArchiveExtracting;

                    break;
                case UpdateStatus.Update:
                    break;
                default:
                    break;
            }
        }

        private void OpenPath(string path)
        {
            try
            {
                Process.Start(path);
            }
            catch (Exception e)
            {
                DebugWrite.Error(e);

                Msg.Error(e.Message);
            }
        }

        private void LinkLocal_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenPath(Path.GetDirectoryName(AppUpdate.Config.LocalFile));
        }

        private void LinkRelease_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var uri = AppUpdate.GetLatestRelease();

            OpenPath(uri.AbsoluteUri);
        }

        private void BtnOperation_Click(object sender, EventArgs e)
        {
            switch (Operation)
            {
                case Operation.Check:
                    Check();
                    break;
                case Operation.Update:
                    DoUpdate();
                    break;
            }
        }

        private async void DoUpdate()
        {
            DebugWrite.Line("start");

            if (AppUpdate.Versions.IsLatest())
            {
                DebugWrite.Line("already latest");

                if (!Msg.Question(Resources.QuestionVersionCompare))
                {
                    DebugWrite.Line("cancel");

                    return;
                }
            }

            try
            {
                //await AppUpdate.UpdateAsync();
                await AppUpdate.DownloadAsync();
            }
            catch (Exception e)
            {
                DebugWrite.Error(e);

                Msg.Error(e.Message);
            }
            finally
            {
                DebugWrite.Line("done");

                Operation = Operation.Check;
            }
        }

        private void ShowConfig()
        {
            if (FrmConfig.ShowDlg(this))
            {
                Check();
            }
        }

        private void BtnConfig_Click(object sender, EventArgs e)
        {
            ShowConfig();
        }
    }
}