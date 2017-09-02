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
            this.pnlVideo = new System.Windows.Forms.Panel();
            this.lblLoading = new System.Windows.Forms.Label();
            this.pnlMouseControls = new System.Windows.Forms.Panel();
            this.pbVolume = new System.Windows.Forms.ProgressBar();
            this.lblProgress = new System.Windows.Forms.Label();
            this.pbProgress = new System.Windows.Forms.ProgressBar();
            this.btnMute = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnVolumeUp = new System.Windows.Forms.Button();
            this.btnVolumeDown = new System.Windows.Forms.Button();
            this.btnRewind = new System.Windows.Forms.Button();
            this.btnFastForward = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnPlayPause = new System.Windows.Forms.Button();
            this.progressTimer = new System.Windows.Forms.Timer(this.components);
            this.inactivityTimer = new System.Windows.Forms.Timer(this.components);
            this.pnlMouseControls.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlVideo
            // 
            this.pnlVideo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlVideo.Location = new System.Drawing.Point(0, 0);
            this.pnlVideo.Name = "pnlVideo";
            this.pnlVideo.Size = new System.Drawing.Size(860, 626);
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
            this.pnlMouseControls.Controls.Add(this.pbVolume);
            this.pnlMouseControls.Controls.Add(this.lblProgress);
            this.pnlMouseControls.Controls.Add(this.pbProgress);
            this.pnlMouseControls.Controls.Add(this.btnMute);
            this.pnlMouseControls.Controls.Add(this.btnClose);
            this.pnlMouseControls.Controls.Add(this.btnNext);
            this.pnlMouseControls.Controls.Add(this.btnVolumeUp);
            this.pnlMouseControls.Controls.Add(this.btnVolumeDown);
            this.pnlMouseControls.Controls.Add(this.btnRewind);
            this.pnlMouseControls.Controls.Add(this.btnFastForward);
            this.pnlMouseControls.Controls.Add(this.btnStop);
            this.pnlMouseControls.Controls.Add(this.btnPlayPause);
            this.pnlMouseControls.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlMouseControls.Location = new System.Drawing.Point(0, 576);
            this.pnlMouseControls.Name = "pnlMouseControls";
            this.pnlMouseControls.Size = new System.Drawing.Size(860, 50);
            this.pnlMouseControls.TabIndex = 54;
            this.pnlMouseControls.MouseLeave += new System.EventHandler(this.pnlMouseControls_MouseLeave);
            this.pnlMouseControls.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnlMouseControls_MouseMove);
            // 
            // pbVolume
            // 
            this.pbVolume.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pbVolume.BackColor = System.Drawing.Color.LawnGreen;
            this.pbVolume.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbVolume.ForeColor = System.Drawing.Color.DarkGreen;
            this.pbVolume.Location = new System.Drawing.Point(564, 33);
            this.pbVolume.Name = "pbVolume";
            this.pbVolume.Size = new System.Drawing.Size(116, 10);
            this.pbVolume.Step = 1;
            this.pbVolume.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbVolume.TabIndex = 11;
            this.pbVolume.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbVolume_MouseDown);
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
            this.pbProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pbProgress.BackColor = System.Drawing.Color.LightSteelBlue;
            this.pbProgress.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbProgress.Location = new System.Drawing.Point(0, 17);
            this.pbProgress.Name = "pbProgress";
            this.pbProgress.Size = new System.Drawing.Size(860, 10);
            this.pbProgress.Step = 1;
            this.pbProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbProgress.TabIndex = 9;
            this.pbProgress.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbProgress_MouseDown);
            // 
            // btnMute
            // 
            this.btnMute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnMute.Location = new System.Drawing.Point(405, 27);
            this.btnMute.Name = "btnMute";
            this.btnMute.Size = new System.Drawing.Size(26, 23);
            this.btnMute.TabIndex = 8;
            this.btnMute.Text = "M";
            this.btnMute.UseVisualStyleBackColor = true;
            this.btnMute.Click += new System.EventHandler(this.btnMute_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(835, 27);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(25, 23);
            this.btnClose.TabIndex = 7;
            this.btnClose.Text = "X";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnNext
            // 
            this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnNext.Location = new System.Drawing.Point(181, 27);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(32, 23);
            this.btnNext.TabIndex = 6;
            this.btnNext.Text = "| >>";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnVolumeUp
            // 
            this.btnVolumeUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnVolumeUp.Location = new System.Drawing.Point(359, 27);
            this.btnVolumeUp.Name = "btnVolumeUp";
            this.btnVolumeUp.Size = new System.Drawing.Size(30, 23);
            this.btnVolumeUp.TabIndex = 5;
            this.btnVolumeUp.Text = "/\\";
            this.btnVolumeUp.UseVisualStyleBackColor = true;
            this.btnVolumeUp.Click += new System.EventHandler(this.btnVolumeUp_Click);
            // 
            // btnVolumeDown
            // 
            this.btnVolumeDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnVolumeDown.Location = new System.Drawing.Point(446, 27);
            this.btnVolumeDown.Name = "btnVolumeDown";
            this.btnVolumeDown.Size = new System.Drawing.Size(26, 23);
            this.btnVolumeDown.TabIndex = 4;
            this.btnVolumeDown.Text = "\\/";
            this.btnVolumeDown.UseVisualStyleBackColor = true;
            this.btnVolumeDown.Click += new System.EventHandler(this.btnVolumeDown_Click);
            // 
            // btnRewind
            // 
            this.btnRewind.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRewind.Location = new System.Drawing.Point(102, 27);
            this.btnRewind.Name = "btnRewind";
            this.btnRewind.Size = new System.Drawing.Size(38, 23);
            this.btnRewind.TabIndex = 3;
            this.btnRewind.Text = "Rew";
            this.btnRewind.UseVisualStyleBackColor = true;
            this.btnRewind.Click += new System.EventHandler(this.btnRewind_Click);
            // 
            // btnFastForward
            // 
            this.btnFastForward.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnFastForward.Location = new System.Drawing.Point(146, 27);
            this.btnFastForward.Name = "btnFastForward";
            this.btnFastForward.Size = new System.Drawing.Size(29, 23);
            this.btnFastForward.TabIndex = 2;
            this.btnFastForward.Text = "FF";
            this.btnFastForward.UseVisualStyleBackColor = true;
            this.btnFastForward.Click += new System.EventHandler(this.btnFastForward_Click);
            // 
            // btnStop
            // 
            this.btnStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnStop.Location = new System.Drawing.Point(57, 27);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(39, 23);
            this.btnStop.TabIndex = 1;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnPlayPause
            // 
            this.btnPlayPause.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPlayPause.Location = new System.Drawing.Point(0, 27);
            this.btnPlayPause.Name = "btnPlayPause";
            this.btnPlayPause.Size = new System.Drawing.Size(51, 23);
            this.btnPlayPause.TabIndex = 0;
            this.btnPlayPause.Text = "Pause";
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
            // liquidMediaPlayer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(860, 626);
            this.Controls.Add(this.pnlMouseControls);
            this.Controls.Add(this.pnlVideo);
            this.Controls.Add(this.lblLoading);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "liquidMediaPlayer";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "liquidMediaPlayer";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.liquidMediaPlayer_MouseClick);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.liquidMediaPlayer_KeyUp);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.liquidMediaPlayer_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.liquidMediaPlayer_KeyDown);
            this.pnlMouseControls.ResumeLayout(false);
            this.pnlMouseControls.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlVideo;
        private System.Windows.Forms.Label lblLoading;
        private System.Windows.Forms.Panel pnlMouseControls;
        private System.Windows.Forms.Button btnVolumeUp;
        private System.Windows.Forms.Button btnVolumeDown;
        private System.Windows.Forms.Button btnRewind;
        private System.Windows.Forms.Button btnFastForward;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnPlayPause;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnMute;
        private System.Windows.Forms.ProgressBar pbProgress;
        private System.Windows.Forms.Timer progressTimer;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.Timer inactivityTimer;
        private System.Windows.Forms.ProgressBar pbVolume;
    }
}