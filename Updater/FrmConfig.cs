using P3tr0viCh.AppUpdate;
using P3tr0viCh.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Updater.Properties;

namespace Updater
{
    public partial class FrmConfig : Form
    {
        private class Config : SettingsBase<AppUpdate.Config> { }

        public FrmConfig()
        {
            InitializeComponent();
        }

        public static bool ShowDlg(Form owner)
        {
            using (var frm = new FrmConfig())
            {
                return frm.ShowDialog(owner) == DialogResult.OK;
            }
        }

        private void FrmInfo_Load(object sender, EventArgs e)
        {
            Config.Directory = Files.ExecutableDirectory();
            Config.FileName = Files.SettingsFileName();

            propertyGrid.SelectedObject = Config.Default;

            propertyGrid.ExpandAllGridItems();

            //            openFileDialog.Filter = Resources.FileOpenFilterInfo;
        }

        public static bool CheckText(string value)
        {
            if (!value.IsEmpty()) return true;

            Msg.Error(Resources.ErrorValueEmpty);

            return false;
        }

        private bool CheckData()
        {
            var msg = string.Empty;

            try
            {
                Config.Default.Check();
            }
            catch (NullReferenceException)
            {
                msg = Resources.ErrorValueEmpty;
            }
            catch (LocalFileNotFoundException)
            {
                msg = Resources.ErrorFileNotExists;
            }
            catch (LocalFileBadFormatException)
            {
                msg = Resources.ErrorFileBadFormat;
            }
            catch (LocalFileWrongLocationException)
            {
                var path = Path.Combine(Directory.GetParent(Config.Default.LocalFile).FullName,
                    Config.Default.LocalVersion.ToString(), 
                    Path.GetFileName(Config.Default.LocalFile));

                msg = string.Format(Resources.ErrorFileWrongLocation, path);
            }
            catch (Exception e)
            {
                msg = e.Message;
            }

            if (msg.IsEmpty()) return true;

            Msg.Error(Resources.ErrorCheckConfig, msg);

            return false;
        }

    private bool SaveData()
    {
        try
        {
            if (!Config.Save())
            {
                throw Config.LastError;
            }
        }
        catch (Exception e)
        {
            DebugWrite.Error(e);

            Msg.Error(Resources.ErrorFileSave);
        }

        return true;
    }

    private bool ApplyData()
    {
        return CheckData() && SaveData();
    }

    private void BtnCancel_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void BtnSave_Click(object sender, EventArgs e)
    {
        if (ApplyData())
        {
            DialogResult = DialogResult.OK;
        }
    }

    private async void CheckConfigAsync()
    {
        btnCheck.Enabled = false;
        btnSave.Enabled = false;

        propertyGrid.Enabled = false;

        try
        {
            if (!CheckData()) return;

            await Config.Default.CheckLatestVersionAsync();

            Msg.Info(string.Format(Resources.CheckConfigOk,
                Config.Default.LocalVersion, Config.Default.LatestVersion));
        }
        catch (Exception e)
        {
            DebugWrite.Error(e);

            Msg.Error(Resources.ErrorCheckConfig, e.Message);
        }
        finally
        {
            btnCheck.Enabled = true;
            btnSave.Enabled = true;

            propertyGrid.Enabled = true;
        }
    }

    private void BtnCheck_Click(object sender, EventArgs e)
    {
        CheckConfigAsync();
    }
}
}