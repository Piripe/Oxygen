namespace Oxygen
{
    partial class WelcomeForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WelcomeForm));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.localSkinRadioButton = new System.Windows.Forms.RadioButton();
            this.internetSkinRadioButton = new System.Windows.Forms.RadioButton();
            this.selectLocalSkinButton = new System.Windows.Forms.Button();
            this.internetSelectListBox = new System.Windows.Forms.ListBox();
            this.nextButton = new System.Windows.Forms.Button();
            this.loadingInternetSkinLabel = new System.Windows.Forms.Label();
            this.localFileNameLabel = new System.Windows.Forms.Label();
            this.internetSkinPreviewPictureBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.internetSkinPreviewPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Font = new System.Drawing.Font("Segoe UI Variable Display Semib", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(13, 79);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(494, 49);
            this.label1.TabIndex = 0;
            this.label1.Text = "Welcome";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Font = new System.Drawing.Font("Segoe UI Variable Display", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 128);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(496, 71);
            this.label2.TabIndex = 1;
            this.label2.Text = "First, you need to select if you already have a local Oxygen skin, otherwise you " +
    "can select one from the interwebz";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Image = global::Oxygen.Properties.Resources.icon_x64;
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(496, 64);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // localSkinRadioButton
            // 
            this.localSkinRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.localSkinRadioButton.AutoSize = true;
            this.localSkinRadioButton.Location = new System.Drawing.Point(19, 200);
            this.localSkinRadioButton.Name = "localSkinRadioButton";
            this.localSkinRadioButton.Size = new System.Drawing.Size(120, 25);
            this.localSkinRadioButton.TabIndex = 0;
            this.localSkinRadioButton.TabStop = true;
            this.localSkinRadioButton.Text = "Select locally";
            this.localSkinRadioButton.UseVisualStyleBackColor = true;
            this.localSkinRadioButton.CheckedChanged += new System.EventHandler(this.localSkinRadioButton_CheckedChanged);
            // 
            // internetSkinRadioButton
            // 
            this.internetSkinRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.internetSkinRadioButton.AutoSize = true;
            this.internetSkinRadioButton.Location = new System.Drawing.Point(19, 243);
            this.internetSkinRadioButton.Name = "internetSkinRadioButton";
            this.internetSkinRadioButton.Size = new System.Drawing.Size(208, 25);
            this.internetSkinRadioButton.TabIndex = 1;
            this.internetSkinRadioButton.TabStop = true;
            this.internetSkinRadioButton.Text = "Select from the interwebz";
            this.internetSkinRadioButton.UseVisualStyleBackColor = true;
            this.internetSkinRadioButton.CheckedChanged += new System.EventHandler(this.internetSkinRadioButton_CheckedChanged);
            // 
            // selectLocalSkinButton
            // 
            this.selectLocalSkinButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.selectLocalSkinButton.Enabled = false;
            this.selectLocalSkinButton.Location = new System.Drawing.Point(145, 196);
            this.selectLocalSkinButton.Name = "selectLocalSkinButton";
            this.selectLocalSkinButton.Size = new System.Drawing.Size(75, 32);
            this.selectLocalSkinButton.TabIndex = 2;
            this.selectLocalSkinButton.Text = "Select...";
            this.selectLocalSkinButton.UseVisualStyleBackColor = true;
            this.selectLocalSkinButton.Click += new System.EventHandler(this.selectLocalSkinButton_Click);
            // 
            // internetSelectListBox
            // 
            this.internetSelectListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.internetSelectListBox.Enabled = false;
            this.internetSelectListBox.FormattingEnabled = true;
            this.internetSelectListBox.ItemHeight = 21;
            this.internetSelectListBox.Location = new System.Drawing.Point(19, 274);
            this.internetSelectListBox.Name = "internetSelectListBox";
            this.internetSelectListBox.Size = new System.Drawing.Size(316, 109);
            this.internetSelectListBox.TabIndex = 3;
            this.internetSelectListBox.SelectedIndexChanged += new System.EventHandler(this.internetSelectListBox_SelectedIndexChanged);
            // 
            // nextButton
            // 
            this.nextButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.nextButton.Enabled = false;
            this.nextButton.Font = new System.Drawing.Font("Segoe UI Variable Display", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nextButton.Location = new System.Drawing.Point(420, 389);
            this.nextButton.Name = "nextButton";
            this.nextButton.Size = new System.Drawing.Size(88, 32);
            this.nextButton.TabIndex = 4;
            this.nextButton.Text = "Next";
            this.nextButton.UseVisualStyleBackColor = true;
            this.nextButton.Click += new System.EventHandler(this.nextButton_Click);
            // 
            // loadingInternetSkinLabel
            // 
            this.loadingInternetSkinLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.loadingInternetSkinLabel.BackColor = System.Drawing.Color.White;
            this.loadingInternetSkinLabel.Font = new System.Drawing.Font("Segoe UI Variable Display", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.loadingInternetSkinLabel.Location = new System.Drawing.Point(20, 275);
            this.loadingInternetSkinLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.loadingInternetSkinLabel.Name = "loadingInternetSkinLabel";
            this.loadingInternetSkinLabel.Size = new System.Drawing.Size(314, 107);
            this.loadingInternetSkinLabel.TabIndex = 8;
            this.loadingInternetSkinLabel.Text = "Loading...";
            this.loadingInternetSkinLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.loadingInternetSkinLabel.Visible = false;
            // 
            // localFileNameLabel
            // 
            this.localFileNameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.localFileNameLabel.AutoEllipsis = true;
            this.localFileNameLabel.Location = new System.Drawing.Point(226, 202);
            this.localFileNameLabel.Name = "localFileNameLabel";
            this.localFileNameLabel.Size = new System.Drawing.Size(280, 23);
            this.localFileNameLabel.TabIndex = 9;
            // 
            // internetSkinPreviewPictureBox
            // 
            this.internetSkinPreviewPictureBox.BackColor = System.Drawing.Color.Black;
            this.internetSkinPreviewPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.internetSkinPreviewPictureBox.Enabled = false;
            this.internetSkinPreviewPictureBox.ImageLocation = "";
            this.internetSkinPreviewPictureBox.InitialImage = global::Oxygen.Properties.Resources.loading;
            this.internetSkinPreviewPictureBox.Location = new System.Drawing.Point(341, 274);
            this.internetSkinPreviewPictureBox.Name = "internetSkinPreviewPictureBox";
            this.internetSkinPreviewPictureBox.Size = new System.Drawing.Size(165, 109);
            this.internetSkinPreviewPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.internetSkinPreviewPictureBox.TabIndex = 10;
            this.internetSkinPreviewPictureBox.TabStop = false;
            this.internetSkinPreviewPictureBox.Click += new System.EventHandler(this.internetSkinPreviewPictureBox_Click);
            // 
            // WelcomeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(520, 431);
            this.Controls.Add(this.internetSkinPreviewPictureBox);
            this.Controls.Add(this.localFileNameLabel);
            this.Controls.Add(this.loadingInternetSkinLabel);
            this.Controls.Add(this.nextButton);
            this.Controls.Add(this.internetSelectListBox);
            this.Controls.Add(this.selectLocalSkinButton);
            this.Controls.Add(this.internetSkinRadioButton);
            this.Controls.Add(this.localSkinRadioButton);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Segoe UI Variable Display", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "WelcomeForm";
            this.Text = "Welcome";
            this.Load += new System.EventHandler(this.WelcomeForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.internetSkinPreviewPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.RadioButton localSkinRadioButton;
        private System.Windows.Forms.RadioButton internetSkinRadioButton;
        private System.Windows.Forms.Button selectLocalSkinButton;
        private System.Windows.Forms.ListBox internetSelectListBox;
        private System.Windows.Forms.Button nextButton;
        private System.Windows.Forms.Label loadingInternetSkinLabel;
        private System.Windows.Forms.Label localFileNameLabel;
        private System.Windows.Forms.PictureBox internetSkinPreviewPictureBox;
    }
}

