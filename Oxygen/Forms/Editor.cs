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

namespace Oxygen.Forms
{
    public partial class Editor : Form
    {
        private DateTime lastSave = DateTime.Now;
        public Editor()
        {
            InitializeComponent();
            categoriesListBox.Items.Clear();
            if (Global.Document != null)
            {
                foreach (Data.JS.Elements.Category category in Global.Document.children)
                {
                    categoriesListBox.Items.Add(category.title);
                }
                if (categoriesListBox.Items.Count > 0)
                {
                    categoriesListBox.SelectedIndex = 0;
                }
            }
            updateTitle();

            if (Global.AutoReload | Global.AutoExport)
            {
                if (!devTimer.Enabled) devTimer.Start();
            }
            else
            {
                devTimer.Stop();
            }
            devModeToolStripMenuItem.Visible = Global.DevMode;
            turnOnAutoReloadToolStripMenuItem.Checked = Global.AutoReload;
            autoExportToolStripMenuItem.Checked = Global.AutoExport;
        }
        private void updateTitle()
        {
            if (Global.SkinConfig.skinSave == null)
            {
                Text = String.Format("Oxygen - {0}", Global.SkinData.Title);
            }
            else
            {
                Text = $"Oxygen - {Global.SkinData.Title} ({Global.SkinConfig.skinSave})";
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Editor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.N:
                        newSkinToolStripMenuItem_Click(sender, e);
                        break;
                    case Keys.O:
                        openToolStripMenuItem_Click(sender, e);
                        break;
                    case Keys.S:
                        saveToolStripMenuItem_Click(sender, e);
                        break;
                    case Keys.E:
                        exportSkinToolStripMenuItem_Click(sender, e);
                        break;
                    case Keys.Q:
                        exitToolStripMenuItem_Click(sender, e);
                        break;
                }
            }
        }

        int lastCategory = -1;
        private void categoriesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (categoriesListBox.SelectedIndex == -1) categoriesListBox.SelectedIndex = lastCategory;

            if (categoriesListBox.SelectedIndex != lastCategory)
            {
                settingsPanel.Controls.Clear();

                int elementLocationY = 12;
                foreach (Data.JS.IElement element in Global.Document.children[categoriesListBox.SelectedIndex].children)
                {
                    elementLocationY += element.AddControl(settingsPanel, elementLocationY);
                }
                lastCategory = categoriesListBox.SelectedIndex;
            }
        }

        private void exportSkinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            noReload = true;
            if (Global.SkinConfig.skinExportPath == null)
            {
                FolderBrowserDialog folderDialog = new FolderBrowserDialog();
                folderDialog.Description = "Select the location of your Steam/skins folder";
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    Global.SkinConfig.skinExportPath = folderDialog.SelectedPath;
                    ProgressBar progressDialog = new ProgressBar((ProgressBar progressBar) => { Modules.Exporter.ExportSkin(progressBar, folderDialog.SelectedPath); });
                    progressDialog.ShowDialog();
                }
            }
            else
            {
                ProgressBar progressDialog = new ProgressBar((ProgressBar progressBar) => { Modules.Exporter.ExportSkin(progressBar, Global.SkinConfig.skinExportPath); });
                progressDialog.ShowDialog();
            }
            noReload = false;
        }

        bool forceClose = false;
        private void Editor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!forceClose)
            {
                if (lastSave.Subtract(DateTime.Now).Seconds > 15)
                {
                    if (MessageBox.Show("Are you sure bro!?", "Close Oxygen...", MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        e.Cancel = true;
                    }
                }
            }
        }

        private void newSkinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = lastSave.Subtract(DateTime.Now).Seconds > 30 ? MessageBox.Show("Do you want to save before creating new project?", "Save Skin...", MessageBoxButtons.YesNoCancel) : DialogResult.No;
            switch (result)
            {
                case DialogResult.Yes:
                    saveToolStripMenuItem_Click(sender, e);
                    new WelcomeForm().Show();
                    forceClose = true;
                    Close();
                    break;
                case DialogResult.No:
                    new WelcomeForm().Show();
                    forceClose = true;
                    Close();
                    break;
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {

            noReload = true;
            if (Global.SkinConfig.skinSave == null)
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Filter = "Oxygen Saved Skin Files (oxygen.xml)|*.oxygen.xml";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    Global.SkinConfig.skinSave = dialog.FileName.EndsWith(".oxygen.xml") ? dialog.FileName : dialog.FileName + ".oxygen.xml";
                    Modules.ProjectManager.Save(Global.SkinConfig.skinSave);
                    updateTitle();
                    lastSave = DateTime.Now;
                }
            }
            else
            {
                Modules.ProjectManager.Save(Global.SkinConfig.skinSave);
                lastSave = DateTime.Now;
            }
            noReload = false;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = lastSave.Subtract(DateTime.Now).Seconds > 30 ? MessageBox.Show("Do you want to save before opening a project?", "Save Skin...", MessageBoxButtons.YesNoCancel) : DialogResult.No;
            switch (result)
            {
                case DialogResult.Yes:
                    saveToolStripMenuItem_Click(sender, e);
                    Open();
                    break;
                case DialogResult.No:
                    Open();
                    break;
            }
        }
        private void Open()
        {

            noReload = true;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Oxygen Saved Skin Files (*.oxygen.xml)|*.oxygen.xml|Oxygen Skin Files (*.skin.zip)|*.skin.zip";
            openFileDialog.Title = "Open Skin...";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.CheckFileExists = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (File.Exists(openFileDialog.FileName))
                {
                    Global.Document = null;
                    Global.JSEngine = null;
                    Global.Editor = null;
                    Global.SkinConfig = new Data.SkinConfig();
                    Global.SkinData = null;

                    Global.SkinConfig.SkinOrigin = Data.SkinOrigin.Local;
                    Global.SkinConfig.skinPath = openFileDialog.FileName;

                    ProgressBar progressDialog = new ProgressBar((ProgressBar progress) =>
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

                    Global.Editor = new Editor();
                    Global.Editor.Size = this.Size;
                    Global.Editor.Show();
                    Global.Editor.Location = this.Location;
                    forceClose = true;
                    Close();
                }
            }

            noReload = false;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            noReload = true;
            new About().ShowDialog();
            noReload = false;
        }


        private bool noReload = false;
        private void devTimer_Tick(object sender, EventArgs e)
        {
            if ((Global.AutoReload | Global.AutoExport) && Global.SkinConfig.SkinOrigin == Data.SkinOrigin.Local)
            {
                DateTime getWriteTime(string path)
                {
                    if (Directory.Exists(path))
                    {
                        DateTime bestTime = DateTime.MinValue;
                        foreach (string file in Directory.GetFiles(path))
                        {
                            FileInfo fileInfo = new FileInfo(file);
                            if (bestTime < fileInfo.LastWriteTime)
                            {
                                bestTime = fileInfo.LastWriteTime;
                            }
                        }
                        foreach (string dir in Directory.GetDirectories(path))
                        {
                            DateTime dirTime = getWriteTime(dir);
                            if (bestTime < dirTime)
                            {
                                bestTime = dirTime;
                            }
                        }
                        return bestTime;
                    }
                    else
                    {
                        return new FileInfo(path).LastWriteTime;
                    }
                }

                DateTime WriteTime = getWriteTime(Global.SkinConfig.skinPath);



                if (WriteTime != Global.AutoReloadLastWrite)
                {
                    Global.AutoReloadLastWrite = WriteTime;
                    if(Global.AutoReload)
                    {
                        if (!noReload)
                        {
                            Console.WriteLine("Reloading...");

                            Global.Document = null;
                            Global.JSEngine = null;
                            Global.Editor = null;
                            Global.SkinData = null;

                            ProgressBar progressDialog = new ProgressBar((ProgressBar progress) =>
                            {
                                Modules.ProjectManager.GenerateProject(progress);
                            });

                            progressDialog.ShowDialog();
                        }
                    }
                    if (Global.AutoExport)
                    {
                        Console.WriteLine("Building...");
                        exportSkinToolStripMenuItem_Click(sender, e);
                    }
                    if (Global.AutoReload)
                    {

                        if (!noReload)
                        {
                            Global.Editor = new Editor();
                            Global.Editor.Size = this.Size;
                            Global.Editor.categoriesListBox.SelectedIndex = Math.Min(Global.Editor.categoriesListBox.Items.Count-1,categoriesListBox.SelectedIndex);
                            Global.Editor.Show();
                            Global.Editor.Location = this.Location;
                            forceClose = true;
                            Close();
                        }
                    }
                }
            }
        }

        private void turnOnAutoReloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Global.AutoReload = turnOnAutoReloadToolStripMenuItem.Checked;
            if (Global.AutoReload | Global.AutoExport)
            {
                if (!devTimer.Enabled) devTimer.Start();
            }
            else
            {
                devTimer.Stop();
            }
        }

        private void autoExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (autoExportToolStripMenuItem.Checked & Global.SkinConfig.skinExportPath == null)
            {
                noReload = true;
                FolderBrowserDialog folderDialog = new FolderBrowserDialog();
                folderDialog.Description = "Select the location of your Steam/skins folder";
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    Global.SkinConfig.skinExportPath = folderDialog.SelectedPath;
                }
                else
                {
                    autoExportToolStripMenuItem.Checked = false;
                }
                noReload = false;
            }
            Global.AutoExport = autoExportToolStripMenuItem.Checked;
            if (Global.AutoReload | Global.AutoExport)
            {
                if (!devTimer.Enabled) devTimer.Start();
            }
            else
            {
                devTimer.Stop();
            }
        }

        private void selectFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            noReload = true;
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            folderDialog.Description = "Select skin folder";
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                if (Directory.Exists(folderDialog.SelectedPath))
                {
                    Global.SkinConfig.SkinOrigin = Data.SkinOrigin.Local;
                    Global.SkinConfig.skinPath = folderDialog.SelectedPath;

                    if (!Global.AutoReload)
                    {
                        Global.Document = null;
                        Global.JSEngine = null;
                        Global.Editor = null;
                        Global.SkinData = null;


                        ProgressBar progressDialog = new ProgressBar((ProgressBar progress) =>
                        {
                            Modules.ProjectManager.GenerateProject(progress);
                        });

                        progressDialog.ShowDialog();

                        Global.Editor = new Editor();
                        Global.Editor.Size = this.Size;
                        Global.Editor.Show();
                        Global.Editor.Location = this.Location;
                        forceClose = true;
                        Close();
                    }
                    noReload = false;
                }
            }
        }

        private void openErrorConsoleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Global.ErrorConsole.Show();
        }
    }
}