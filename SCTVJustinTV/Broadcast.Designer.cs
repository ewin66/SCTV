namespace SCTVJustinTV
{
    partial class Broadcast
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Broadcast));
            this.btnBroadcast = new System.Windows.Forms.Button();
            this.cbCameras = new System.Windows.Forms.ComboBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnRefreshCameras = new System.Windows.Forms.Button();
            this.wbCameraDisplay = new System.Windows.Forms.WebBrowser();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tss = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnView = new System.Windows.Forms.Button();
            this.cameraTimer = new System.Windows.Forms.Timer(this.components);
            this.contextMenuMain = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.displayBroadcastToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox3.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.contextMenuMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnBroadcast
            // 
            this.btnBroadcast.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnBroadcast.Location = new System.Drawing.Point(222, 68);
            this.btnBroadcast.Name = "btnBroadcast";
            this.btnBroadcast.Size = new System.Drawing.Size(75, 23);
            this.btnBroadcast.TabIndex = 2;
            this.btnBroadcast.Text = "Broadcast";
            this.btnBroadcast.UseVisualStyleBackColor = true;
            this.btnBroadcast.Click += new System.EventHandler(this.btnBroadcast_Click);
            // 
            // cbCameras
            // 
            this.cbCameras.DisplayMember = "Name";
            this.cbCameras.FormattingEnabled = true;
            this.cbCameras.Location = new System.Drawing.Point(6, 19);
            this.cbCameras.Name = "cbCameras";
            this.cbCameras.Size = new System.Drawing.Size(326, 21);
            this.cbCameras.TabIndex = 3;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.groupBox3.Controls.Add(this.btnRefreshCameras);
            this.groupBox3.Controls.Add(this.cbCameras);
            this.groupBox3.Location = new System.Drawing.Point(30, 8);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(378, 52);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Cameras";
            // 
            // btnRefreshCameras
            // 
            this.btnRefreshCameras.FlatAppearance.BorderSize = 0;
            this.btnRefreshCameras.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnRefreshCameras.Image = ((System.Drawing.Image)(resources.GetObject("btnRefreshCameras.Image")));
            this.btnRefreshCameras.Location = new System.Drawing.Point(348, 18);
            this.btnRefreshCameras.Name = "btnRefreshCameras";
            this.btnRefreshCameras.Size = new System.Drawing.Size(21, 21);
            this.btnRefreshCameras.TabIndex = 8;
            this.btnRefreshCameras.UseVisualStyleBackColor = true;
            this.btnRefreshCameras.Click += new System.EventHandler(this.btnRefreshCameras_Click);
            // 
            // wbCameraDisplay
            // 
            this.wbCameraDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.wbCameraDisplay.Location = new System.Drawing.Point(0, 119);
            this.wbCameraDisplay.MinimumSize = new System.Drawing.Size(20, 20);
            this.wbCameraDisplay.Name = "wbCameraDisplay";
            this.wbCameraDisplay.Size = new System.Drawing.Size(439, 20);
            this.wbCameraDisplay.TabIndex = 5;
            this.wbCameraDisplay.Url = new System.Uri("", System.UriKind.Relative);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tss});
            this.statusStrip1.Location = new System.Drawing.Point(0, 94);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(439, 22);
            this.statusStrip1.TabIndex = 6;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tss
            // 
            this.tss.Name = "tss";
            this.tss.Size = new System.Drawing.Size(130, 17);
            this.tss.Text = "No Cameras Broadcasting";
            // 
            // btnRefresh
            // 
            this.btnRefresh.Enabled = false;
            this.btnRefresh.FlatAppearance.BorderSize = 0;
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnRefresh.Image = ((System.Drawing.Image)(resources.GetObject("btnRefresh.Image")));
            this.btnRefresh.Location = new System.Drawing.Point(12, 70);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(21, 21);
            this.btnRefresh.TabIndex = 7;
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Visible = false;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnView
            // 
            this.btnView.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnView.Location = new System.Drawing.Point(141, 68);
            this.btnView.Name = "btnView";
            this.btnView.Size = new System.Drawing.Size(75, 23);
            this.btnView.TabIndex = 8;
            this.btnView.Text = "View";
            this.btnView.UseVisualStyleBackColor = true;
            this.btnView.Click += new System.EventHandler(this.btnView_Click);
            // 
            // cameraTimer
            // 
            this.cameraTimer.Enabled = true;
            this.cameraTimer.Interval = 1000;
            this.cameraTimer.Tick += new System.EventHandler(this.cameraTimer_Tick);
            // 
            // contextMenuMain
            // 
            this.contextMenuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.displayBroadcastToolStripMenuItem});
            this.contextMenuMain.Name = "contextMenuMain";
            this.contextMenuMain.Size = new System.Drawing.Size(171, 48);
            // 
            // displayBroadcastToolStripMenuItem
            // 
            this.displayBroadcastToolStripMenuItem.Name = "displayBroadcastToolStripMenuItem";
            this.displayBroadcastToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.displayBroadcastToolStripMenuItem.Text = "Display Broadcast";
            this.displayBroadcastToolStripMenuItem.Click += new System.EventHandler(this.displayBroadcastToolStripMenuItem_Click);
            // 
            // Broadcast
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(439, 116);
            this.ContextMenuStrip = this.contextMenuMain;
            this.Controls.Add(this.btnView);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.btnBroadcast);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.wbCameraDisplay);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Broadcast";
            this.Text = "Broadcast";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Broadcast_FormClosing);
            this.groupBox3.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.contextMenuMain.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnBroadcast;
        private System.Windows.Forms.ComboBox cbCameras;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.WebBrowser wbCameraDisplay;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel tss;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnRefreshCameras;
        private System.Windows.Forms.Button btnView;
        private System.Windows.Forms.Timer cameraTimer;
        private System.Windows.Forms.ContextMenuStrip contextMenuMain;
        private System.Windows.Forms.ToolStripMenuItem displayBroadcastToolStripMenuItem;
    }
}