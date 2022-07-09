using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Xml.Linq;
using System.Threading;
using System.Text.RegularExpressions;

namespace Oxygen
{
    public partial class WelcomeForm : Form
    {
        public WelcomeForm()
        {
            InitializeComponent();
        }

        private void WelcomeForm_Load(object sender, EventArgs e)
        {
            Global.Document = null;
            Global.JSEngine = null;
            Global.Editor = null;
            Global.SkinConfig = new Data.SkinConfig();
            Global.SkinData = null;

            Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "Oxygen"));

            localSkinRadioButton.Checked = true;
        }

        private void selectLocalSkinButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Oxygen Skin Files (*.skin.zip)|*.skin.zip|Oxygen Saved Skin Files (*.oxygen.xml)|*.oxygen.xml";
            openFileDialog.Title = "Open Skin...";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.CheckFileExists = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if ( File.Exists(openFileDialog.FileName) )
                {
                    nextButton.Enabled = true;
                    localFileNameLabel.Text = Path.GetFileName( openFileDialog.FileName);
                    Global.SkinConfig.skinPath = openFileDialog.FileName;
                }
                else
                {
                    localFileNameLabel.Text = "Invalid file";
                    nextButton.Enabled = false;
                }
            }
            else
            {
                localFileNameLabel.Text = "";
                nextButton.Enabled = false;
            }
        }

        private void localSkinRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (localSkinRadioButton.Checked)
            {
                selectLocalSkinButton.Enabled = true;
                Global.SkinConfig.SkinOrigin = Data.SkinOrigin.Local;
            }
            else
            {
                localFileNameLabel.Text = "";
                Global.SkinConfig.skinPath = "";
                selectLocalSkinButton.Enabled = false;
                nextButton.Enabled = false;
            }
        }

        private void internetSkinRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (internetSkinRadioButton.Checked)
            {
                loadingInternetSkinLabel.Visible = true;
                internetSelectListBox.Enabled = true;
                internetSkinPreviewPictureBox.Enabled = true;

                fetchInternetSkins();
                Global.SkinConfig.SkinOrigin = Data.SkinOrigin.Internet;
            }
            else
            {
                loadingInternetSkinLabel.Visible = false;
                internetSelectListBox.Enabled = false;
                internetSkinPreviewPictureBox.Enabled=false;
                nextButton.Enabled = false;
            }
        }

        private List<Data.SkinData> internetSkins;
        private async void fetchInternetSkins()
        {
            if (internetSkins == null)
            {
                internetSkins = new List<Data.SkinData>();
                XDocument doc = XDocument.Parse(await new System.Net.WebClient().DownloadStringTaskAsync("https://github.com/Piripe/Oxygen/raw/main/public-skins.xml"));

                internetSelectListBox.Items.Clear();

                foreach (XElement skin in doc.Root.Elements())
                {
                    internetSkins.Add(new Data.SkinData() { Title = skin.Attribute("title").Value, DownloadLink = skin.Element("download").Value, InfosLink = skin.Element("infos").Value });
                    internetSelectListBox.Items.Add(internetSkins.Last().Title);
                }
            }

            loadingInternetSkinLabel.Visible = false;
        }
        private async void fetchSelectedInternetSkin(int index)
        {
            XDocument doc = XDocument.Parse(await new System.Net.WebClient().DownloadStringTaskAsync(internetSkins[index].InfosLink));

            internetSkins[index].Title = doc.Root.Element("title").Value;
            internetSelectListBox.Items[index] = internetSkins[index].Title;

            internetSkins[index].Description = doc.Root.Element("description").Value;

            internetSkins[index].Thumbnail = doc.Root.Element("thumbnail").Value;
            internetSkinPreviewPictureBox.ImageLocation = internetSkins[index].Thumbnail;
        }

        private void internetSelectListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (internetSkins[internetSelectListBox.SelectedIndex].Thumbnail == "")
            {
                fetchSelectedInternetSkin(internetSelectListBox.SelectedIndex);
            }
            else
            {
                internetSkinPreviewPictureBox.ImageLocation = internetSkins[internetSelectListBox.SelectedIndex].Thumbnail;
            }
            if (internetSelectListBox.SelectedIndex == -1)
            {
                nextButton.Enabled = false;
            }
            else
            {
                nextButton.Enabled = true;
                Global.SkinConfig.SkinOrigin = Data.SkinOrigin.Internet;
                Global.SkinConfig.skinPath = internetSkins[internetSelectListBox.SelectedIndex].DownloadLink;
            }
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            Forms.ProgressBar progressDialog = new Forms.ProgressBar((Forms.ProgressBar progress) =>
            {
                if (Global.SkinConfig.skinPath.EndsWith(".oxygen.xml"))
                {
                    Modules.ProjectManager.Load(progress, Global.SkinConfig.skinPath);
                }
                else
                {
                    Modules.ProjectManager.GenerateProject(progress);
                }
            });

            progressDialog.ShowDialog();

            Global.Editor = new Forms.Editor();
            Global.Editor.Show();
            Close();
        }

        private void internetSkinPreviewPictureBox_Click(object sender, EventArgs e)
        {
            Forms.PictureDialog previewDialog = new Forms.PictureDialog(internetSkinPreviewPictureBox.ImageLocation);
            previewDialog.ShowDialog();
        }
    }
}
