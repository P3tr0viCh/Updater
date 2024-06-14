using P3tr0viCh.AppUpdate;
using P3tr0viCh.Utils;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Updater.Properties;

namespace Updater
{
    public partial class Main : Form
    {
        private class Config : SettingsBase<P3tr0viCh.AppUpdate.Config> { }

        private readonly AppUpdate AppUpdate = new AppUpdate();

        private bool InProgress
        {
            set
            {
                var enabled = !value;

                btnCheck.Enabled = enabled;
                btnDownload.Enabled = enabled;
                btnUpdate.Enabled = enabled;
                btnConfig.Enabled = enabled;
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

            DoCheck();

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

            btnCheck.Enabled = status == UpdateStatus.Idle;

            switch (status)
            {
                case UpdateStatus.Idle:
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

        private void ShowConfig()
        {
            if (FrmConfig.ShowDlg(this))
            {
                DoCheck();
            }
        }

        private async void DoCheck()
        {
            InProgress = true;

            try
            {
                AppUpdate.CheckLocalVersion();

                await AppUpdate.CheckLatestVersionAsync();
            }
            catch (Exception e)
            {
                DebugWrite.Error(e);

                Msg.Error(e.Message);
            }
            finally
            {
                InProgress = false;
            }
        }
        
        private async void DoDownload()
        {
            DebugWrite.Line("start");

            InProgress = true;

            try
            {
                await AppUpdate.DownloadAsync();
            }
            catch (Exception e)
            {
                DebugWrite.Error(e);

                Msg.Error(e.Message);
            }
            finally
            {
                InProgress = false;

                DebugWrite.Line("done");
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

            InProgress = true;

            try
            {
                await AppUpdate.UpdateAsync();
            }
            catch (Exception e)
            {
                DebugWrite.Error(e);

                Msg.Error(e.Message);
            }
            finally
            {
                InProgress = false;

                DebugWrite.Line("done");
            }
        }

        private void BtnConfig_Click(object sender, EventArgs e)
        {
            ShowConfig();
        }

        private void BtnCheck_Click(object sender, EventArgs e)
        {
            DoCheck();
        }

        private void BtnDownload_Click(object sender, EventArgs e)
        {
            DoDownload();
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            DoUpdate();
        }
    }
}