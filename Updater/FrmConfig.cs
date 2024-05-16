using P3tr0viCh.AppUpdate;
using P3tr0viCh.Utils;
using System;
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
            try
            {
                Config.Default.Check();
            }
            catch (Exception e)
            {
                var msg = e.Message;

                if (e is NullReferenceException)
                {
                    msg = Resources.ErrorValueEmpty;
                }
                else
                {
                    if (e is LocalFileNotFoundException)
                    {
                        msg = Resources.ErrorValueEmpty;
                    }
                    else
                    {
                        if (e is LocalFileWrongLocationException)
                        {
                            msg = Resources.ErrorFileWrongLocation;
                        }
                    }
                }

                Msg.Error(msg);

                return false;
            }

            return true;
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
                await Config.Default.CheckVersionsAsync();

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