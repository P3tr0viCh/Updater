using P3tr0viCh.Utils;
using System;
using System.IO;
using System.Windows.Forms;
using Updater.Properties;

namespace Updater
{
    public partial class FrmInfo : Form
    {
        private class Info : SettingsBase<InfoBase> { }

        public FrmInfo()
        {
            InitializeComponent();
        }

        public static bool ShowDlg(Form owner)
        {
            using (var frm = new FrmInfo())
            {
                return frm.ShowDialog(owner) == DialogResult.OK;
            }
        }

        private void FrmInfo_Load(object sender, EventArgs e)
        {
            Info.Directory = Files.ExecutableDirectory();
            Info.FileName = Files.SettingsFileName();

            propertyGrid.SelectedObject = Info.Default;

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
            if (!File.Exists(Info.Default.LocalFile))
            {
                Msg.Error(Resources.ErrorFileNotExists);
                return false;
            }

            var parentDirName = Utils.Directory.GetParentName(Info.Default.LocalFile);
            
            if (parentDirName != Utils.Directory.LatestDirName)
            {
                Msg.Error(Resources.ErrorFileWrongLocation);

                return false;
            }

            return
                CheckText(Info.Default.GitHubReleaseInfo.Project.Owner) &&
                CheckText(Info.Default.GitHubReleaseInfo.Project.Repo) &&
                CheckText(Info.Default.GitHubReleaseInfo.File);
        }

        private bool SaveData()
        {
            try
            {
                if (!Info.Save())
                {
                    throw Info.LastError;
                }
            }
            catch (Exception e)
            {
                Utils.WriteError(e);

                Msg.Error(Resources.ErrorFileSave);
            }

            return true;
        }

        private bool ApplyData()
        {
            return CheckData() && SaveData();
        }

        private void BtnCancel_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        private void BtnSave_Click(object sender, System.EventArgs e)
        {
            if (ApplyData())
            {
                DialogResult = DialogResult.OK;
            }
        }

        private void BtnCheck_Click(object sender, System.EventArgs e)
        {
            Msg.Info();
        }
    }
}