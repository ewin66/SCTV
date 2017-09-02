namespace SCTVJustinTVFileManager
{
    partial class FileManager
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
            this.tcFiles = new System.Windows.Forms.TabControl();
            this.tpOnlineFiles = new System.Windows.Forms.TabPage();
            this.lvArchives = new System.Windows.Forms.ListView();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader6 = new System.Windows.Forms.ColumnHeader();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.downloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.playToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ilImages = new System.Windows.Forms.ImageList(this.components);
            this.tpLocalFiles = new System.Windows.Forms.TabPage();
            this.lvLocalFiles = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.tpAllFiles = new System.Windows.Forms.TabPage();
            this.lvAllFiles = new System.Windows.Forms.ListView();
            this.columnHeader10 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader11 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader12 = new System.Windows.Forms.ColumnHeader();
            this.tpViewer = new System.Windows.Forms.TabPage();
            this.wbViewer = new System.Windows.Forms.WebBrowser();
            this.columnHeader7 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader8 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader9 = new System.Windows.Forms.ColumnHeader();
            this.tcFiles.SuspendLayout();
            this.tpOnlineFiles.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.tpLocalFiles.SuspendLayout();
            this.tpAllFiles.SuspendLayout();
            this.tpViewer.SuspendLayout();
            this.SuspendLayout();
            // 
            // tcFiles
            // 
            this.tcFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tcFiles.Controls.Add(this.tpOnlineFiles);
            this.tcFiles.Controls.Add(this.tpLocalFiles);
            this.tcFiles.Controls.Add(this.tpAllFiles);
            this.tcFiles.Controls.Add(this.tpViewer);
            this.tcFiles.Location = new System.Drawing.Point(0, 24);
            this.tcFiles.Name = "tcFiles";
            this.tcFiles.SelectedIndex = 0;
            this.tcFiles.Size = new System.Drawing.Size(644, 341);
            this.tcFiles.TabIndex = 0;
            this.tcFiles.SelectedIndexChanged += new System.EventHandler(this.tcFiles_SelectedIndexChanged);
            // 
            // tpOnlineFiles
            // 
            this.tpOnlineFiles.Controls.Add(this.lvArchives);
            this.tpOnlineFiles.Location = new System.Drawing.Point(4, 22);
            this.tpOnlineFiles.Name = "tpOnlineFiles";
            this.tpOnlineFiles.Padding = new System.Windows.Forms.Padding(3);
            this.tpOnlineFiles.Size = new System.Drawing.Size(636, 315);
            this.tpOnlineFiles.TabIndex = 1;
            this.tpOnlineFiles.Text = "Online Files";
            this.tpOnlineFiles.UseVisualStyleBackColor = true;
            // 
            // lvArchives
            // 
            this.lvArchives.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.lvArchives.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6});
            this.lvArchives.ContextMenuStrip = this.contextMenuStrip1;
            this.lvArchives.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvArchives.LargeImageList = this.ilImages;
            this.lvArchives.Location = new System.Drawing.Point(3, 3);
            this.lvArchives.MultiSelect = false;
            this.lvArchives.Name = "lvArchives";
            this.lvArchives.Size = new System.Drawing.Size(630, 309);
            this.lvArchives.SmallImageList = this.ilImages;
            this.lvArchives.TabIndex = 1;
            this.lvArchives.UseCompatibleStateImageBehavior = false;
            this.lvArchives.DoubleClick += new System.EventHandler(this.lvArchives_DoubleClick);
            // 
            // columnHeader4
            // 
            this.columnHeader4.Width = 100;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Width = 100;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Width = 100;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.downloadToolStripMenuItem,
            this.playToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(133, 48);
            // 
            // downloadToolStripMenuItem
            // 
            this.downloadToolStripMenuItem.Name = "downloadToolStripMenuItem";
            this.downloadToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.downloadToolStripMenuItem.Text = "Download";
            this.downloadToolStripMenuItem.Click += new System.EventHandler(this.downloadToolStripMenuItem_Click);
            // 
            // playToolStripMenuItem
            // 
            this.playToolStripMenuItem.Name = "playToolStripMenuItem";
            this.playToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.playToolStripMenuItem.Text = "Play";
            this.playToolStripMenuItem.Click += new System.EventHandler(this.playToolStripMenuItem_Click);
            // 
            // ilImages
            // 
            this.ilImages.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.ilImages.ImageSize = new System.Drawing.Size(91, 140);
            this.ilImages.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // tpLocalFiles
            // 
            this.tpLocalFiles.Controls.Add(this.lvLocalFiles);
            this.tpLocalFiles.Location = new System.Drawing.Point(4, 22);
            this.tpLocalFiles.Name = "tpLocalFiles";
            this.tpLocalFiles.Padding = new System.Windows.Forms.Padding(3);
            this.tpLocalFiles.Size = new System.Drawing.Size(636, 315);
            this.tpLocalFiles.TabIndex = 0;
            this.tpLocalFiles.Text = "Local Files";
            this.tpLocalFiles.UseVisualStyleBackColor = true;
            // 
            // lvLocalFiles
            // 
            this.lvLocalFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.lvLocalFiles.ContextMenuStrip = this.contextMenuStrip1;
            this.lvLocalFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvLocalFiles.LargeImageList = this.ilImages;
            this.lvLocalFiles.Location = new System.Drawing.Point(3, 3);
            this.lvLocalFiles.MultiSelect = false;
            this.lvLocalFiles.Name = "lvLocalFiles";
            this.lvLocalFiles.Size = new System.Drawing.Size(630, 309);
            this.lvLocalFiles.SmallImageList = this.ilImages;
            this.lvLocalFiles.TabIndex = 0;
            this.lvLocalFiles.UseCompatibleStateImageBehavior = false;
            this.lvLocalFiles.DoubleClick += new System.EventHandler(this.lvLocalFiles_DoubleClick);
            // 
            // tpAllFiles
            // 
            this.tpAllFiles.Controls.Add(this.lvAllFiles);
            this.tpAllFiles.Location = new System.Drawing.Point(4, 22);
            this.tpAllFiles.Name = "tpAllFiles";
            this.tpAllFiles.Size = new System.Drawing.Size(636, 315);
            this.tpAllFiles.TabIndex = 2;
            this.tpAllFiles.Text = "All Files";
            this.tpAllFiles.UseVisualStyleBackColor = true;
            // 
            // lvAllFiles
            // 
            this.lvAllFiles.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.lvAllFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader10,
            this.columnHeader11,
            this.columnHeader12});
            this.lvAllFiles.ContextMenuStrip = this.contextMenuStrip1;
            this.lvAllFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvAllFiles.LargeImageList = this.ilImages;
            this.lvAllFiles.Location = new System.Drawing.Point(0, 0);
            this.lvAllFiles.MultiSelect = false;
            this.lvAllFiles.Name = "lvAllFiles";
            this.lvAllFiles.Size = new System.Drawing.Size(636, 315);
            this.lvAllFiles.SmallImageList = this.ilImages;
            this.lvAllFiles.TabIndex = 2;
            this.lvAllFiles.UseCompatibleStateImageBehavior = false;
            this.lvAllFiles.DoubleClick += new System.EventHandler(this.lvAllFiles_DoubleClick);
            // 
            // columnHeader10
            // 
            this.columnHeader10.Width = 100;
            // 
            // columnHeader11
            // 
            this.columnHeader11.Width = 100;
            // 
            // columnHeader12
            // 
            this.columnHeader12.Width = 100;
            // 
            // tpViewer
            // 
            this.tpViewer.Controls.Add(this.wbViewer);
            this.tpViewer.Location = new System.Drawing.Point(4, 22);
            this.tpViewer.Name = "tpViewer";
            this.tpViewer.Size = new System.Drawing.Size(636, 315);
            this.tpViewer.TabIndex = 3;
            this.tpViewer.Text = "Viewer";
            this.tpViewer.UseVisualStyleBackColor = true;
            // 
            // wbViewer
            // 
            this.wbViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wbViewer.Location = new System.Drawing.Point(0, 0);
            this.wbViewer.MinimumSize = new System.Drawing.Size(20, 20);
            this.wbViewer.Name = "wbViewer";
            this.wbViewer.Size = new System.Drawing.Size(636, 315);
            this.wbViewer.TabIndex = 0;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Width = 175;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Width = 175;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Width = 175;
            // 
            // FileManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(642, 363);
            this.Controls.Add(this.tcFiles);
            this.Name = "FileManager";
            this.Text = "Security File Manager";
            this.tcFiles.ResumeLayout(false);
            this.tpOnlineFiles.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.tpLocalFiles.ResumeLayout(false);
            this.tpAllFiles.ResumeLayout(false);
            this.tpViewer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tcFiles;
        private System.Windows.Forms.TabPage tpLocalFiles;
        private System.Windows.Forms.TabPage tpOnlineFiles;
        private System.Windows.Forms.ListView lvLocalFiles;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.TabPage tpAllFiles;
        private System.Windows.Forms.ListView lvArchives;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ImageList ilImages;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.TabPage tpViewer;
        private System.Windows.Forms.WebBrowser wbViewer;
        private System.Windows.Forms.ListView lvAllFiles;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.ColumnHeader columnHeader11;
        private System.Windows.Forms.ColumnHeader columnHeader12;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem downloadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem playToolStripMenuItem;
    }
}

