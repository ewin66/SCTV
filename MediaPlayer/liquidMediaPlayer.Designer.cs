<<<<<<< .mine
namespace SCTV
{
    partial class liquidMediaPlayer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(liquidMediaPlayer));
            this.pnlVideo = new System.Windows.Forms.Panel();
            this.lblLoading = new System.Windows.Forms.Label();
            this.pnlMouseControls = new System.Windows.Forms.Panel();
            this.lblWhatsNext = new System.Windows.Forms.Label();
            this.btnMediaInfo = new System.Windows.Forms.Button();
            this.pnlCover = new System.Windows.Forms.Panel();
            this.volume1 = new SCTVControls.Volume.Volume();
            this.StarRating = new SCTV.StarRating();
            this.lblMediaTitle = new System.Windows.Forms.Label();
            this.chbContinousPlay = new System.Windows.Forms.CheckBox();
            this.chbSequels = new System.Windows.Forms.CheckBox();
            this.btnSecurity = new System.Windows.Forms.Button();
            this.btnMinimize = new System.Windows.Forms.Button();
            this.lblProgress = new System.Windows.Forms.Label();
            this.pbProgress = new System.Windows.Forms.ProgressBar();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnRewind = new System.Windows.Forms.Button();
            this.btnFastForward = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnPlayPause = new System.Windows.Forms.Button();
            this.progressTimer = new System.Windows.Forms.Timer(this.components);
            this.inactivityTimer = new System.Windows.Forms.Timer(this.components);
            this.CameraContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.camera1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.camera2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlMouseControls.SuspendLayout();
            this.CameraContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlVideo
            // 
            this.pnlVideo.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.pnlVideo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlVideo.Location = new System.Drawing.Point(0, 0);
            this.pnlVideo.Name = "pnlVideo";
            this.pnlVideo.Size = new System.Drawing.Size(1075, 626);
            this.pnlVideo.TabIndex = 53;
            this.pnlVideo.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pnlVideo_MouseClick);
            // 
            // lblLoading
            // 
            this.lblLoading.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLoading.Location = new System.Drawing.Point(0, 0);
            this.lblLoading.Name = "lblLoading";
            this.lblLoading.Size = new System.Drawing.Size(860, 573);
            this.lblLoading.TabIndex = 0;
            this.lblLoading.Text = "Loading...";
            this.lblLoading.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblLoading.Visible = false;
            // 
            // pnlMouseControls
            // 
            this.pnlMouseControls.BackColor = System.Drawing.Color.Transparent;
            this.pnlMouseControls.Controls.Add(this.lblWhatsNext);
            this.pnlMouseControls.Controls.Add(this.btnMediaInfo);
            this.pnlMouseControls.Controls.Add(this.pnlCover);
            this.pnlMouseControls.Controls.Add(this.volume1);
            this.pnlMouseControls.Controls.Add(this.StarRating);
            this.pnlMouseControls.Controls.Add(this.lblMediaTitle);
            this.pnlMouseControls.Controls.Add(this.chbContinousPlay);
            this.pnlMouseControls.Controls.Add(this.chbSequels);
            this.pnlMouseControls.Controls.Add(this.btnSecurity);
            this.pnlMouseControls.Controls.Add(this.btnMinimize);
            this.pnlMouseControls.Controls.Add(this.lblProgress);
            this.pnlMouseControls.Controls.Add(this.pbProgress);
            this.pnlMouseControls.Controls.Add(this.btnClose);
            this.pnlMouseControls.Controls.Add(this.btnNext);
            this.pnlMouseControls.Controls.Add(this.btnRewind);
            this.pnlMouseControls.Controls.Add(this.btnFastForward);
            this.pnlMouseControls.Controls.Add(this.btnStop);
            this.pnlMouseControls.Controls.Add(this.btnPlayPause);
            this.pnlMouseControls.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlMouseControls.Location = new System.Drawing.Point(0, 576);
            this.pnlMouseControls.Name = "pnlMouseControls";
            this.pnlMouseControls.Size = new System.Drawing.Size(1075, 50);
            this.pnlMouseControls.TabIndex = 54;
            this.pnlMouseControls.MouseLeave += new System.EventHandler(this.pnlMouseControls_MouseLeave);
            this.pnlMouseControls.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnlMouseControls_MouseMove);
            // 
            // lblWhatsNext
            // 
            this.lblWhatsNext.AutoSize = true;
            this.lblWhatsNext.Location = new System.Drawing.Point(683, 3);
            this.lblWhatsNext.Name = "lblWhatsNext";
            this.lblWhatsNext.Size = new System.Drawing.Size(65, 13);
            this.lblWhatsNext.TabIndex = 63;
            this.lblWhatsNext.Text = "What\'s Next";
            this.lblWhatsNext.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lblWhatsNext_MouseClick);
            // 
            // btnMediaInfo
            // 
            this.btnMediaInfo.FlatAppearance.BorderSize = 0;
            this.btnMediaInfo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMediaInfo.Image = global::MediaPlayer.Properties.Resources.info_icon;
            this.btnMediaInfo.Location = new System.Drawing.Point(491, 0);
            this.btnMediaInfo.Name = "btnMediaInfo";
            this.btnMediaInfo.Size = new System.Drawing.Size(18, 16);
            this.btnMediaInfo.TabIndex = 62;
            this.btnMediaInfo.UseVisualStyleBackColor = true;
            this.btnMediaInfo.Click += new System.EventHandler(this.btnMediaInfo_Click);
            // 
            // pnlCover
            // 
            this.pnlCover.BackColor = System.Drawing.Color.Black;
            this.pnlCover.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlCover.ForeColor = System.Drawing.Color.Black;
            this.pnlCover.Location = new System.Drawing.Point(0, 40);
            this.pnlCover.Name = "pnlCover";
            this.pnlCover.Size = new System.Drawing.Size(1075, 10);
            this.pnlCover.TabIndex = 12;
            this.pnlCover.Visible = false;
            this.pnlCover.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnlMouseControls_MouseMove);
            // 
            // volume1
            // 
            this.volume1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.volume1.BackColor = System.Drawing.Color.Transparent;
            this.volume1.Location = new System.Drawing.Point(592, 28);
            this.volume1.Name = "volume1";
            this.volume1.Size = new System.Drawing.Size(236, 22);
            this.volume1.TabIndex = 61;
            // 
            // StarRating
            // 
            this.StarRating.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.StarRating.BackColor = System.Drawing.Color.Transparent;
            this.StarRating.ControlLayout = SCTV.StarRating.Layouts.Horizontal;
            this.StarRating.Location = new System.Drawing.Point(434, 27);
            this.StarRating.Margin = new System.Windows.Forms.Padding(0);
            this.StarRating.Name = "StarRating";
            this.StarRating.Padding = new System.Windows.Forms.Padding(1);
            this.StarRating.Rating = 0;
            this.StarRating.Size = new System.Drawing.Size(102, 22);
            this.StarRating.TabIndex = 60;
            this.StarRating.WrapperPanelBorderStyle = System.Windows.Forms.BorderStyle.None;
            this.StarRating.RatingValueChanged += new SCTV.StarRating.RatingValueChangedEventHandler(this.starRating_RatingValueChanged);
            // 
            // lblMediaTitle
            // 
            this.lblMediaTitle.AutoSize = true;
            this.lblMediaTitle.Location = new System.Drawing.Point(514, 1);
            this.lblMediaTitle.Name = "lblMediaTitle";
            this.lblMediaTitle.Size = new System.Drawing.Size(85, 13);
            this.lblMediaTitle.TabIndex = 59;
            this.lblMediaTitle.Text = "Currently Playing";
            // 
            // chbContinousPlay
            // 
            this.chbContinousPlay.AutoSize = true;
            this.chbContinousPlay.Checked = true;
            this.chbContinousPlay.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbContinousPlay.Location = new System.Drawing.Point(322, 27);
            this.chbContinousPlay.Name = "chbContinousPlay";
            this.chbContinousPlay.Size = new System.Drawing.Size(96, 17);
            this.chbContinousPlay.TabIndex = 58;
            this.chbContinousPlay.Text = "Continous Play";
            this.chbContinousPlay.UseVisualStyleBackColor = true;
            this.chbContinousPlay.CheckedChanged += new System.EventHandler(this.chbContinousPlay_CheckedChanged);
            // 
            // chbSequels
            // 
            this.chbSequels.AutoSize = true;
            this.chbSequels.Checked = true;
            this.chbSequels.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbSequels.Location = new System.Drawing.Point(229, 27);
            this.chbSequels.Name = "chbSequels";
            this.chbSequels.Size = new System.Drawing.Size(87, 17);
            this.chbSequels.TabIndex = 57;
            this.chbSequels.Text = "Play Sequels";
            this.chbSequels.UseVisualStyleBackColor = true;
            this.chbSequels.CheckedChanged += new System.EventHandler(this.chbSequels_CheckedChanged);
            // 
            // btnSecurity
            // 
            this.btnSecurity.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSecurity.Location = new System.Drawing.Point(855, 27);
            this.btnSecurity.Name = "btnSecurity";
            this.btnSecurity.Size = new System.Drawing.Size(75, 23);
            this.btnSecurity.TabIndex = 56;
            this.btnSecurity.Text = "Security";
            this.btnSecurity.UseVisualStyleBackColor = true;
            this.btnSecurity.Visible = false;
            this.btnSecurity.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnMinimize
            // 
            this.btnMinimize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMinimize.FlatAppearance.BorderSize = 0;
            this.btnMinimize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMinimize.Image = global::MediaPlayer.Properties.Resources.Minimize_whiteOnDKBlue_button;
            this.btnMinimize.Location = new System.Drawing.Point(1017, 27);
            this.btnMinimize.Name = "btnMinimize";
            this.btnMinimize.Size = new System.Drawing.Size(27, 23);
            this.btnMinimize.TabIndex = 0;
            this.btnMinimize.UseVisualStyleBackColor = true;
            this.btnMinimize.Click += new System.EventHandler(this.btnMinimize_Click);
            // 
            // lblProgress
            // 
            this.lblProgress.AutoSize = true;
            this.lblProgress.Location = new System.Drawing.Point(3, 4);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(0, 13);
            this.lblProgress.TabIndex = 10;
            // 
            // pbProgress
            // 
            this.pbProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbProgress.BackColor = System.Drawing.Color.LightSteelBlue;
            this.pbProgress.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbProgress.Location = new System.Drawing.Point(0, 17);
            this.pbProgress.Name = "pbProgress";
            this.pbProgress.Size = new System.Drawing.Size(1075, 10);
            this.pbProgress.Step = 1;
            this.pbProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbProgress.TabIndex = 9;
            this.pbProgress.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbProgress_MouseDown);
            this.pbProgress.MouseLeave += new System.EventHandler(this.pnlMouseControls_MouseLeave);
            this.pbProgress.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnlMouseControls_MouseMove);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Image = global::MediaPlayer.Properties.Resources.Power_WhiteOnDKBlue_button;
            this.btnClose.Location = new System.Drawing.Point(1050, 27);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(25, 23);
            this.btnClose.TabIndex = 7;
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnNext
            // 
            this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnNext.FlatAppearance.BorderSize = 0;
            this.btnNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNext.Image = global::MediaPlayer.Properties.Resources.Next_whiteOnDKBlue_button;
            this.btnNext.Location = new System.Drawing.Point(159, 27);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(27, 23);
            this.btnNext.TabIndex = 6;
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnRewind
            // 
            this.btnRewind.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRewind.FlatAppearance.BorderSize = 0;
            this.btnRewind.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRewind.Image = global::MediaPlayer.Properties.Resources.Rewind_WhiteOnDKBlue_button;
            this.btnRewind.Location = new System.Drawing.Point(91, 27);
            this.btnRewind.Name = "btnRewind";
            this.btnRewind.Size = new System.Drawing.Size(27, 23);
            this.btnRewind.TabIndex = 3;
            this.btnRewind.UseVisualStyleBackColor = true;
            this.btnRewind.Click += new System.EventHandler(this.btnRewind_Click);
            // 
            // btnFastForward
            // 
            this.btnFastForward.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnFastForward.FlatAppearance.BorderSize = 0;
            this.btnFastForward.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFastForward.Image = global::MediaPlayer.Properties.Resources.FF_WhiteOnDKBlue_button;
            this.btnFastForward.Location = new System.Drawing.Point(124, 27);
            this.btnFastForward.Name = "btnFastForward";
            this.btnFastForward.Size = new System.Drawing.Size(29, 23);
            this.btnFastForward.TabIndex = 2;
            this.btnFastForward.UseVisualStyleBackColor = true;
            this.btnFastForward.Click += new System.EventHandler(this.btnFastForward_Click);
            // 
            // btnStop
            // 
            this.btnStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnStop.FlatAppearance.BorderSize = 0;
            this.btnStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStop.Image = global::MediaPlayer.Properties.Resources.Stop_WhiteOnDKBlue_button;
            this.btnStop.Location = new System.Drawing.Point(57, 27);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(28, 23);
            this.btnStop.TabIndex = 1;
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnPlayPause
            // 
            this.btnPlayPause.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPlayPause.FlatAppearance.BorderSize = 0;
            this.btnPlayPause.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPlayPause.Image = global::MediaPlayer.Properties.Resources.Pause_WhiteOnDKBlue_button;
            this.btnPlayPause.Location = new System.Drawing.Point(25, 27);
            this.btnPlayPause.Name = "btnPlayPause";
            this.btnPlayPause.Size = new System.Drawing.Size(26, 23);
            this.btnPlayPause.TabIndex = 0;
            this.btnPlayPause.UseVisualStyleBackColor = true;
            this.btnPlayPause.Click += new System.EventHandler(this.btnPlayPause_Click);
            // 
            // progressTimer
            // 
            this.progressTimer.Interval = 1000;
            this.progressTimer.Tick += new System.EventHandler(this.progressTimer_Tick);
            // 
            // inactivityTimer
            // 
            this.inactivityTimer.Interval = 1000;
            this.inactivityTimer.Tick += new System.EventHandler(this.inactivityTimer_Tick);
            // 
            // CameraContextMenuStrip
            // 
            this.CameraContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.camera1ToolStripMenuItem,
            this.camera2ToolStripMenuItem});
            this.CameraContextMenuStrip.Name = "CameraContextMenuStrip";
            this.CameraContextMenuStrip.Size = new System.Drawing.Size(125, 48);
            // 
            // camera1ToolStripMenuItem
            // 
            this.camera1ToolStripMenuItem.Name = "camera1ToolStripMenuItem";
            this.camera1ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.camera1ToolStripMenuItem.Text = "Camera 1";
            this.camera1ToolStripMenuItem.Click += new System.EventHandler(this.camera1ToolStripMenuItem_Click);
            // 
            // camera2ToolStripMenuItem
            // 
            this.camera2ToolStripMenuItem.Name = "camera2ToolStripMenuItem";
            this.camera2ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.camera2ToolStripMenuItem.Text = "Camera 2";
            this.camera2ToolStripMenuItem.Click += new System.EventHandler(this.camera2ToolStripMenuItem_Click);
            // 
            // liquidMediaPlayer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1075, 626);
            this.Controls.Add(this.pnlMouseControls);
            this.Controls.Add(this.pnlVideo);
            this.Controls.Add(this.lblLoading);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "liquidMediaPlayer";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "liquidMediaPlayer";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.liquidMediaPlayer_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.liquidMediaPlayer_FormClosed);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.liquidMediaPlayer_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.liquidMediaPlayer_KeyUp);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.liquidMediaPlayer_MouseClick);
            this.pnlMouseControls.ResumeLayout(false);
            this.pnlMouseControls.PerformLayout();
            this.CameraContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlVideo;
        private System.Windows.Forms.Label lblLoading;
        private System.Windows.Forms.Panel pnlMouseControls;
        private System.Windows.Forms.Button btnRewind;
        private System.Windows.Forms.Button btnFastForward;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnPlayPause;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.ProgressBar pbProgress;
        private System.Windows.Forms.Timer progressTimer;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.Timer inactivityTimer;
        private System.Windows.Forms.Panel pnlCover;
        private System.Windows.Forms.Button btnMinimize;
        private System.Windows.Forms.Button btnSecurity;
        private System.Windows.Forms.ContextMenuStrip CameraContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem camera1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem camera2ToolStripMenuItem;
        private System.Windows.Forms.CheckBox chbSequels;
        private System.Windows.Forms.CheckBox chbContinousPlay;
        private System.Windows.Forms.Label lblMediaTitle;
        private StarRating StarRating;
        private SCTVControls.Volume.Volume volume1;
        private System.Windows.Forms.Button btnMediaInfo;
        private System.Windows.Forms.Label lblWhatsNext;
    }
}||||||| .r0
=======
namespace SCTV
{
    partial class liquidMediaPlayer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(liquidMediaPlayer));
            this.pnlVideo = new System.Windows.Forms.Panel();
            this.lblLoading = new System.Windows.Forms.Label();
            this.pnlMouseControls = new System.Windows.Forms.Panel();
            this.lblWhatsNext = new System.Windows.Forms.Label();
            this.btnMediaInfo = new System.Windows.Forms.Button();
            this.pnlCover = new System.Windows.Forms.Panel();
            this.volume1 = new SCTVControls.Volume.Volume();
            this.StarRating = new SCTV.StarRating();
            this.lblMediaTitle = new System.Windows.Forms.Label();
            this.chbContinousPlay = new System.Windows.Forms.CheckBox();
            this.chbSequels = new System.Windows.Forms.CheckBox();
            this.btnSecurity = new System.Windows.Forms.Button();
            this.btnMinimize = new System.Windows.Forms.Button();
            this.lblProgress = new System.Windows.Forms.Label();
            this.pbProgress = new System.Windows.Forms.ProgressBar();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnRewind = new System.Windows.Forms.Button();
            this.btnFastForward = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnPlayPause = new System.Windows.Forms.Button();
            this.progressTimer = new System.Windows.Forms.Timer(this.components);
            this.inactivityTimer = new System.Windows.Forms.Timer(this.components);
            this.CameraContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.camera1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.camera2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlMouseControls.SuspendLayout();
            this.CameraContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlVideo
            // 
            this.pnlVideo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlVideo.Location = new System.Drawing.Point(0, 0);
            this.pnlVideo.Name = "pnlVideo";
            this.pnlVideo.Size = new System.Drawing.Size(1075, 626);
            this.pnlVideo.TabIndex = 53;
            this.pnlVideo.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pnlVideo_MouseClick);
            // 
            // lblLoading
            // 
            this.lblLoading.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLoading.Location = new System.Drawing.Point(0, 0);
            this.lblLoading.Name = "lblLoading";
            this.lblLoading.Size = new System.Drawing.Size(860, 573);
            this.lblLoading.TabIndex = 0;
            this.lblLoading.Text = "Loading...";
            this.lblLoading.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblLoading.Visible = false;
            // 
            // pnlMouseControls
            // 
            this.pnlMouseControls.BackColor = System.Drawing.Color.Transparent;
            this.pnlMouseControls.Controls.Add(this.lblWhatsNext);
            this.pnlMouseControls.Controls.Add(this.btnMediaInfo);
            this.pnlMouseControls.Controls.Add(this.pnlCover);
            this.pnlMouseControls.Controls.Add(this.volume1);
            this.pnlMouseControls.Controls.Add(this.StarRating);
            this.pnlMouseControls.Controls.Add(this.lblMediaTitle);
            this.pnlMouseControls.Controls.Add(this.chbContinousPlay);
            this.pnlMouseControls.Controls.Add(this.chbSequels);
            this.pnlMouseControls.Controls.Add(this.btnSecurity);
            this.pnlMouseControls.Controls.Add(this.btnMinimize);
            this.pnlMouseControls.Controls.Add(this.lblProgress);
            this.pnlMouseControls.Controls.Add(this.pbProgress);
            this.pnlMouseControls.Controls.Add(this.btnClose);
            this.pnlMouseControls.Controls.Add(this.btnNext);
            this.pnlMouseControls.Controls.Add(this.btnRewind);
            this.pnlMouseControls.Controls.Add(this.btnFastForward);
            this.pnlMouseControls.Controls.Add(this.btnStop);
            this.pnlMouseControls.Controls.Add(this.btnPlayPause);
            this.pnlMouseControls.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlMouseControls.Location = new System.Drawing.Point(0, 576);
            this.pnlMouseControls.Name = "pnlMouseControls";
            this.pnlMouseControls.Size = new System.Drawing.Size(1075, 50);
            this.pnlMouseControls.TabIndex = 54;
            this.pnlMouseControls.MouseLeave += new System.EventHandler(this.pnlMouseControls_MouseLeave);
            this.pnlMouseControls.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnlMouseControls_MouseMove);
            // 
            // lblWhatsNext
            // 
            this.lblWhatsNext.AutoSize = true;
            this.lblWhatsNext.Location = new System.Drawing.Point(683, 3);
            this.lblWhatsNext.Name = "lblWhatsNext";
            this.lblWhatsNext.Size = new System.Drawing.Size(65, 13);
            this.lblWhatsNext.TabIndex = 63;
            this.lblWhatsNext.Text = "What\'s Next";
            this.lblWhatsNext.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lblWhatsNext_MouseClick);
            // 
            // btnMediaInfo
            // 
            this.btnMediaInfo.FlatAppearance.BorderSize = 0;
            this.btnMediaInfo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMediaInfo.Image = global::MediaPlayer.Properties.Resources.info_icon;
            this.btnMediaInfo.Location = new System.Drawing.Point(491, 0);
            this.btnMediaInfo.Name = "btnMediaInfo";
            this.btnMediaInfo.Size = new System.Drawing.Size(18, 16);
            this.btnMediaInfo.TabIndex = 62;
            this.btnMediaInfo.UseVisualStyleBackColor = true;
            this.btnMediaInfo.Click += new System.EventHandler(this.btnMediaInfo_Click);
            // 
            // pnlCover
            // 
            this.pnlCover.BackColor = System.Drawing.Color.Black;
            this.pnlCover.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlCover.ForeColor = System.Drawing.Color.Black;
            this.pnlCover.Location = new System.Drawing.Point(0, 40);
            this.pnlCover.Name = "pnlCover";
            this.pnlCover.Size = new System.Drawing.Size(1075, 10);
            this.pnlCover.TabIndex = 12;
            this.pnlCover.Visible = false;
            this.pnlCover.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnlMouseControls_MouseMove);
            // 
            // volume1
            // 
            this.volume1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.volume1.BackColor = System.Drawing.Color.Transparent;
            this.volume1.Location = new System.Drawing.Point(595, 27);
            this.volume1.Name = "volume1";
            this.volume1.Size = new System.Drawing.Size(236, 22);
            this.volume1.TabIndex = 61;
            // 
            // StarRating
            // 
            this.StarRating.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.StarRating.BackColor = System.Drawing.Color.Transparent;
            this.StarRating.ControlLayout = SCTV.StarRating.Layouts.Horizontal;
            this.StarRating.Location = new System.Drawing.Point(434, 27);
            this.StarRating.Margin = new System.Windows.Forms.Padding(0);
            this.StarRating.Name = "StarRating";
            this.StarRating.Padding = new System.Windows.Forms.Padding(1);
            this.StarRating.Rating = 0;
            this.StarRating.Size = new System.Drawing.Size(102, 22);
            this.StarRating.TabIndex = 60;
            this.StarRating.WrapperPanelBorderStyle = System.Windows.Forms.BorderStyle.None;
            this.StarRating.RatingValueChanged += new SCTV.StarRating.RatingValueChangedEventHandler(this.starRating_RatingValueChanged);
            // 
            // lblMediaTitle
            // 
            this.lblMediaTitle.AutoSize = true;
            this.lblMediaTitle.Location = new System.Drawing.Point(514, 1);
            this.lblMediaTitle.Name = "lblMediaTitle";
            this.lblMediaTitle.Size = new System.Drawing.Size(85, 13);
            this.lblMediaTitle.TabIndex = 59;
            this.lblMediaTitle.Text = "Currently Playing";
            // 
            // chbContinousPlay
            // 
            this.chbContinousPlay.AutoSize = true;
            this.chbContinousPlay.Checked = true;
            this.chbContinousPlay.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbContinousPlay.Location = new System.Drawing.Point(322, 27);
            this.chbContinousPlay.Name = "chbContinousPlay";
            this.chbContinousPlay.Size = new System.Drawing.Size(96, 17);
            this.chbContinousPlay.TabIndex = 58;
            this.chbContinousPlay.Text = "Continous Play";
            this.chbContinousPlay.UseVisualStyleBackColor = true;
            this.chbContinousPlay.CheckedChanged += new System.EventHandler(this.chbContinousPlay_CheckedChanged);
            // 
            // chbSequels
            // 
            this.chbSequels.AutoSize = true;
            this.chbSequels.Checked = true;
            this.chbSequels.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbSequels.Location = new System.Drawing.Point(229, 27);
            this.chbSequels.Name = "chbSequels";
            this.chbSequels.Size = new System.Drawing.Size(87, 17);
            this.chbSequels.TabIndex = 57;
            this.chbSequels.Text = "Play Sequels";
            this.chbSequels.UseVisualStyleBackColor = true;
            this.chbSequels.CheckedChanged += new System.EventHandler(this.chbSequels_CheckedChanged);
            // 
            // btnSecurity
            // 
            this.btnSecurity.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSecurity.Location = new System.Drawing.Point(855, 27);
            this.btnSecurity.Name = "btnSecurity";
            this.btnSecurity.Size = new System.Drawing.Size(75, 23);
            this.btnSecurity.TabIndex = 56;
            this.btnSecurity.Text = "Security";
            this.btnSecurity.UseVisualStyleBackColor = true;
            this.btnSecurity.Visible = false;
            this.btnSecurity.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnMinimize
            // 
            this.btnMinimize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMinimize.FlatAppearance.BorderSize = 0;
            this.btnMinimize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMinimize.Image = global::MediaPlayer.Properties.Resources.Minimize_whiteOnDKBlue_button;
            this.btnMinimize.Location = new System.Drawing.Point(1017, 27);
            this.btnMinimize.Name = "btnMinimize";
            this.btnMinimize.Size = new System.Drawing.Size(27, 23);
            this.btnMinimize.TabIndex = 0;
            this.btnMinimize.UseVisualStyleBackColor = true;
            this.btnMinimize.Click += new System.EventHandler(this.btnMinimize_Click);
            // 
            // lblProgress
            // 
            this.lblProgress.AutoSize = true;
            this.lblProgress.Location = new System.Drawing.Point(3, 4);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(0, 13);
            this.lblProgress.TabIndex = 10;
            // 
            // pbProgress
            // 
            this.pbProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbProgress.BackColor = System.Drawing.Color.LightSteelBlue;
            this.pbProgress.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbProgress.Location = new System.Drawing.Point(0, 17);
            this.pbProgress.Name = "pbProgress";
            this.pbProgress.Size = new System.Drawing.Size(1075, 10);
            this.pbProgress.Step = 1;
            this.pbProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbProgress.TabIndex = 9;
            this.pbProgress.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbProgress_MouseDown);
            this.pbProgress.MouseLeave += new System.EventHandler(this.pnlMouseControls_MouseLeave);
            this.pbProgress.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnlMouseControls_MouseMove);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Image = global::MediaPlayer.Properties.Resources.Power_WhiteOnDKBlue_button;
            this.btnClose.Location = new System.Drawing.Point(1050, 27);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(25, 23);
            this.btnClose.TabIndex = 7;
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnNext
            // 
            this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnNext.FlatAppearance.BorderSize = 0;
            this.btnNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNext.Image = global::MediaPlayer.Properties.Resources.Next_whiteOnDKBlue_button;
            this.btnNext.Location = new System.Drawing.Point(159, 27);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(27, 23);
            this.btnNext.TabIndex = 6;
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnRewind
            // 
            this.btnRewind.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRewind.FlatAppearance.BorderSize = 0;
            this.btnRewind.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRewind.Image = global::MediaPlayer.Properties.Resources.Rewind_WhiteOnDKBlue_button;
            this.btnRewind.Location = new System.Drawing.Point(91, 27);
            this.btnRewind.Name = "btnRewind";
            this.btnRewind.Size = new System.Drawing.Size(27, 23);
            this.btnRewind.TabIndex = 3;
            this.btnRewind.UseVisualStyleBackColor = true;
            this.btnRewind.Click += new System.EventHandler(this.btnRewind_Click);
            // 
            // btnFastForward
            // 
            this.btnFastForward.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnFastForward.FlatAppearance.BorderSize = 0;
            this.btnFastForward.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFastForward.Image = global::MediaPlayer.Properties.Resources.FF_WhiteOnDKBlue_button;
            this.btnFastForward.Location = new System.Drawing.Point(124, 27);
            this.btnFastForward.Name = "btnFastForward";
            this.btnFastForward.Size = new System.Drawing.Size(29, 23);
            this.btnFastForward.TabIndex = 2;
            this.btnFastForward.UseVisualStyleBackColor = true;
            this.btnFastForward.Click += new System.EventHandler(this.btnFastForward_Click);
            // 
            // btnStop
            // 
            this.btnStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnStop.FlatAppearance.BorderSize = 0;
            this.btnStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStop.Image = global::MediaPlayer.Properties.Resources.Stop_WhiteOnDKBlue_button;
            this.btnStop.Location = new System.Drawing.Point(57, 27);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(28, 23);
            this.btnStop.TabIndex = 1;
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnPlayPause
            // 
            this.btnPlayPause.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPlayPause.FlatAppearance.BorderSize = 0;
            this.btnPlayPause.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPlayPause.Image = global::MediaPlayer.Properties.Resources.Play_WhiteOnDKBlue_button;
            this.btnPlayPause.Location = new System.Drawing.Point(25, 27);
            this.btnPlayPause.Name = "btnPlayPause";
            this.btnPlayPause.Size = new System.Drawing.Size(26, 23);
            this.btnPlayPause.TabIndex = 0;
            this.btnPlayPause.UseVisualStyleBackColor = true;
            this.btnPlayPause.Click += new System.EventHandler(this.btnPlayPause_Click);
            // 
            // progressTimer
            // 
            this.progressTimer.Interval = 1000;
            this.progressTimer.Tick += new System.EventHandler(this.progressTimer_Tick);
            // 
            // inactivityTimer
            // 
            this.inactivityTimer.Interval = 1000;
            this.inactivityTimer.Tick += new System.EventHandler(this.inactivityTimer_Tick);
            // 
            // CameraContextMenuStrip
            // 
            this.CameraContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.camera1ToolStripMenuItem,
            this.camera2ToolStripMenuItem});
            this.CameraContextMenuStrip.Name = "CameraContextMenuStrip";
            this.CameraContextMenuStrip.Size = new System.Drawing.Size(125, 48);
            // 
            // camera1ToolStripMenuItem
            // 
            this.camera1ToolStripMenuItem.Name = "camera1ToolStripMenuItem";
            this.camera1ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.camera1ToolStripMenuItem.Text = "Camera 1";
            this.camera1ToolStripMenuItem.Click += new System.EventHandler(this.camera1ToolStripMenuItem_Click);
            // 
            // camera2ToolStripMenuItem
            // 
            this.camera2ToolStripMenuItem.Name = "camera2ToolStripMenuItem";
            this.camera2ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.camera2ToolStripMenuItem.Text = "Camera 2";
            this.camera2ToolStripMenuItem.Click += new System.EventHandler(this.camera2ToolStripMenuItem_Click);
            // 
            // liquidMediaPlayer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1075, 626);
            this.Controls.Add(this.pnlMouseControls);
            this.Controls.Add(this.pnlVideo);
            this.Controls.Add(this.lblLoading);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "liquidMediaPlayer";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "liquidMediaPlayer";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.liquidMediaPlayer_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.liquidMediaPlayer_FormClosed);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.liquidMediaPlayer_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.liquidMediaPlayer_KeyUp);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.liquidMediaPlayer_MouseClick);
            this.pnlMouseControls.ResumeLayout(false);
            this.pnlMouseControls.PerformLayout();
            this.CameraContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlVideo;
        private System.Windows.Forms.Label lblLoading;
        private System.Windows.Forms.Panel pnlMouseControls;
        private System.Windows.Forms.Button btnRewind;
        private System.Windows.Forms.Button btnFastForward;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnPlayPause;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.ProgressBar pbProgress;
        private System.Windows.Forms.Timer progressTimer;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.Timer inactivityTimer;
        private System.Windows.Forms.Panel pnlCover;
        private System.Windows.Forms.Button btnMinimize;
        private System.Windows.Forms.Button btnSecurity;
        private System.Windows.Forms.ContextMenuStrip CameraContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem camera1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem camera2ToolStripMenuItem;
        private System.Windows.Forms.CheckBox chbSequels;
        private System.Windows.Forms.CheckBox chbContinousPlay;
        private System.Windows.Forms.Label lblMediaTitle;
        private StarRating StarRating;
        private SCTVControls.Volume.Volume volume1;
        private System.Windows.Forms.Button btnMediaInfo;
        private System.Windows.Forms.Label lblWhatsNext;
    }
}>>>>>>> .r6
