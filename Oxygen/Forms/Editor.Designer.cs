namespace Oxygen.Forms
{
    partial class Editor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Editor));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.filesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newSkinToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportSkinToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.devModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openErrorConsoleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.selectFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.turnOnAutoReloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autoExportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.categoriesListBox = new System.Windows.Forms.ListBox();
            this.settingsPanel = new System.Windows.Forms.Panel();
            this.linkPreviewLabel = new System.Windows.Forms.Label();
            this.devTimer = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.Control;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.filesToolStripMenuItem,
            this.devModeToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(848, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "editorMenuStrip";
            // 
            // filesToolStripMenuItem
            // 
            this.filesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newSkinToolStripMenuItem,
            this.openToolStripMenuItem,
            this.toolStripSeparator1,
            this.saveToolStripMenuItem,
            this.exportSkinToolStripMenuItem,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
            this.filesToolStripMenuItem.Name = "filesToolStripMenuItem";
            this.filesToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.filesToolStripMenuItem.Text = "File";
            this.filesToolStripMenuItem.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // newSkinToolStripMenuItem
            // 
            this.newSkinToolStripMenuItem.Name = "newSkinToolStripMenuItem";
            this.newSkinToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+N";
            this.newSkinToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.newSkinToolStripMenuItem.Text = "New...";
            this.newSkinToolStripMenuItem.Click += new System.EventHandler(this.newSkinToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+O";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.openToolStripMenuItem.Text = "Open...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(179, 6);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+S";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.saveToolStripMenuItem.Text = "Save...";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // exportSkinToolStripMenuItem
            // 
            this.exportSkinToolStripMenuItem.Name = "exportSkinToolStripMenuItem";
            this.exportSkinToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+E";
            this.exportSkinToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.exportSkinToolStripMenuItem.Text = "Export Skin...";
            this.exportSkinToolStripMenuItem.Click += new System.EventHandler(this.exportSkinToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(179, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+Q";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // devModeToolStripMenuItem
            // 
            this.devModeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openErrorConsoleToolStripMenuItem,
            this.toolStripSeparator4,
            this.selectFolderToolStripMenuItem,
            this.toolStripSeparator3,
            this.turnOnAutoReloadToolStripMenuItem,
            this.autoExportToolStripMenuItem});
            this.devModeToolStripMenuItem.Name = "devModeToolStripMenuItem";
            this.devModeToolStripMenuItem.Size = new System.Drawing.Size(73, 20);
            this.devModeToolStripMenuItem.Text = "Dev Mode";
            this.devModeToolStripMenuItem.Visible = false;
            // 
            // openErrorConsoleToolStripMenuItem
            // 
            this.openErrorConsoleToolStripMenuItem.Name = "openErrorConsoleToolStripMenuItem";
            this.openErrorConsoleToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.openErrorConsoleToolStripMenuItem.Text = "Open Error Console";
            this.openErrorConsoleToolStripMenuItem.Click += new System.EventHandler(this.openErrorConsoleToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(174, 6);
            // 
            // selectFolderToolStripMenuItem
            // 
            this.selectFolderToolStripMenuItem.Name = "selectFolderToolStripMenuItem";
            this.selectFolderToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.selectFolderToolStripMenuItem.Text = "Select Folder...";
            this.selectFolderToolStripMenuItem.Click += new System.EventHandler(this.selectFolderToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(174, 6);
            // 
            // turnOnAutoReloadToolStripMenuItem
            // 
            this.turnOnAutoReloadToolStripMenuItem.CheckOnClick = true;
            this.turnOnAutoReloadToolStripMenuItem.Name = "turnOnAutoReloadToolStripMenuItem";
            this.turnOnAutoReloadToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.turnOnAutoReloadToolStripMenuItem.Text = "Auto reload";
            this.turnOnAutoReloadToolStripMenuItem.Click += new System.EventHandler(this.turnOnAutoReloadToolStripMenuItem_Click);
            // 
            // autoExportToolStripMenuItem
            // 
            this.autoExportToolStripMenuItem.CheckOnClick = true;
            this.autoExportToolStripMenuItem.Name = "autoExportToolStripMenuItem";
            this.autoExportToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.autoExportToolStripMenuItem.Text = "Auto export";
            this.autoExportToolStripMenuItem.Click += new System.EventHandler(this.autoExportToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.aboutToolStripMenuItem.Text = "About...";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // categoriesListBox
            // 
            this.categoriesListBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.categoriesListBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.categoriesListBox.Font = new System.Drawing.Font("Segoe UI Variable Display", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.categoriesListBox.FormattingEnabled = true;
            this.categoriesListBox.ItemHeight = 19;
            this.categoriesListBox.Location = new System.Drawing.Point(0, 24);
            this.categoriesListBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.categoriesListBox.Name = "categoriesListBox";
            this.categoriesListBox.Size = new System.Drawing.Size(181, 464);
            this.categoriesListBox.TabIndex = 1;
            this.categoriesListBox.SelectedIndexChanged += new System.EventHandler(this.categoriesListBox_SelectedIndexChanged);
            // 
            // settingsPanel
            // 
            this.settingsPanel.AutoScroll = true;
            this.settingsPanel.AutoScrollMargin = new System.Drawing.Size(0, 24);
            this.settingsPanel.AutoScrollMinSize = new System.Drawing.Size(-100, 0);
            this.settingsPanel.BackColor = System.Drawing.SystemColors.Control;
            this.settingsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.settingsPanel.Font = new System.Drawing.Font("Segoe UI Variable Display", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.settingsPanel.Location = new System.Drawing.Point(181, 24);
            this.settingsPanel.Margin = new System.Windows.Forms.Padding(2);
            this.settingsPanel.Name = "settingsPanel";
            this.settingsPanel.Size = new System.Drawing.Size(667, 464);
            this.settingsPanel.TabIndex = 2;
            // 
            // linkPreviewLabel
            // 
            this.linkPreviewLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.linkPreviewLabel.AutoEllipsis = true;
            this.linkPreviewLabel.AutoSize = true;
            this.linkPreviewLabel.BackColor = System.Drawing.Color.White;
            this.linkPreviewLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.linkPreviewLabel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.linkPreviewLabel.Font = new System.Drawing.Font("Segoe UI Variable Display", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.linkPreviewLabel.Location = new System.Drawing.Point(0, 472);
            this.linkPreviewLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.linkPreviewLabel.MaximumSize = new System.Drawing.Size(78, 16);
            this.linkPreviewLabel.Name = "linkPreviewLabel";
            this.linkPreviewLabel.Size = new System.Drawing.Size(2, 16);
            this.linkPreviewLabel.TabIndex = 3;
            this.linkPreviewLabel.Tag = "0";
            this.linkPreviewLabel.Visible = false;
            // 
            // devTimer
            // 
            this.devTimer.Interval = 2000;
            this.devTimer.Tick += new System.EventHandler(this.devTimer_Tick);
            // 
            // Editor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(848, 488);
            this.Controls.Add(this.linkPreviewLabel);
            this.Controls.Add(this.settingsPanel);
            this.Controls.Add(this.categoriesListBox);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Segoe UI Variable Display", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MinimumSize = new System.Drawing.Size(532, 314);
            this.Name = "Editor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Editor_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Editor_KeyDown);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem filesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newSkinToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exportSkinToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        public System.Windows.Forms.ListBox categoriesListBox;
        public System.Windows.Forms.Panel settingsPanel;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        internal System.Windows.Forms.Label linkPreviewLabel;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem devModeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem turnOnAutoReloadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autoExportToolStripMenuItem;
        private System.Windows.Forms.Timer devTimer;
        private System.Windows.Forms.ToolStripMenuItem selectFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem openErrorConsoleToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator4;
    }
}