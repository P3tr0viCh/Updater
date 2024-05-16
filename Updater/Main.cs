﻿using P3tr0viCh.AppUpdate;
using P3tr0viCh.Utils;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Updater.Properties;
using static P3tr0viCh.AppUpdate.AppUpdate;
using static Updater.Enums;

namespace Updater
{
    public partial class Main : Form
    {
        private class Config : SettingsBase<AppUpdate.Config> { }

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
                await Config.Default.CheckVersionsAsync();

                if (Config.Default.LocalVersion is null && Config.Default.LatestVersion is null)
                {
                    Operation = Operation.Check;
                }
                else
                {
                    if (Config.Default.LocalVersion is null)
                    {
                        Operation = Operation.Install;
                    }
                    else
                    {
                        if (Config.Default.LatestVersion is null)
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
            if (!Config.Default.Status.IsIdle())
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

                Config.Default.Check();

                Config.Default.Status.StatusChanged += Update_StatusChanged;
            }
            catch (Exception e)
            {
                DebugWrite.Error(e);

                return false;
            }

            return true;
        }

        private void Update_StatusChanged(object sender, Status status)
        {
            DebugWrite.Line(status.ToString());

            btnOperation.Enabled = status == Status.Idle;

            switch (status)
            {
                case Status.Idle:
                    btnOperation.Text = Resources.TextBtnCheck;

                    lblLocalVersion.Text = Config.Default.LocalVersion is null ?
                        Resources.TextVersionNotExists : Config.Default.LocalVersion.ToString();
                    lblLatestVersion.Text = Config.Default.LatestVersion is null ?
                        Resources.TextVersionNotExists : Config.Default.LatestVersion.ToString();

                    break;
                case Status.Check:
                    break;
                case Status.CheckLocal:
                    lblName.Text = Misc.GetFileTitle(Config.Default.LocalFile);

                    lblLocalVersion.Text = Resources.TextVersionReading;

                    break;
                case Status.CheckLatest:
                    lblLatestVersion.Text = Resources.TextVersionReading;

                    break;
                case Status.Download:
                    lblLatestVersion.Text = Resources.TextVersionDownloading;

                    break;
                case Status.ArchiveExtract:
                    lblLatestVersion.Text = Resources.TextVersionArchiveExtracting;

                    break;
                case Status.Update:
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
            OpenPath(Path.GetDirectoryName(Config.Default.LocalFile));
        }

        private void LinkRelease_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var uri = Config.Default.GetLatestRelease();

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
                case Operation.Install:
                    AppUpdate();
                    break;
            }
        }

        private async void AppUpdate()
        {
            DebugWrite.Line("start");

            if (Config.Default.IsLatestVersion())
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
                await Config.Default.UpdateAsync();
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