namespace SCTVCamera
{
    partial class SCTVCameraMain
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
            this.cbCameras = new System.Windows.Forms.ComboBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnStop = new System.Windows.Forms.Button();
            this.gbStatus = new System.Windows.Forms.GroupBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnView = new System.Windows.Forms.Button();
            this.tcCamera = new System.Windows.Forms.TabControl();
            this.tpCameraName = new System.Windows.Forms.TabPage();
            this.btnFTP = new System.Windows.Forms.Button();
            this.btnRecordCamera = new System.Windows.Forms.Button();
            this.btnRecordForWeb = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.txtRecordPath = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.txtFrameRate = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cbFrameSize = new System.Windows.Forms.ComboBox();
            this.gbCameraName = new System.Windows.Forms.GroupBox();
            this.txtCameraName = new System.Windows.Forms.TextBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.lblCameraCapabilities = new System.Windows.Forms.Label();
            this.btnCaptureFrame = new System.Windows.Forms.Button();
            this.groupBox3.SuspendLayout();
            this.gbStatus.SuspendLayout();
            this.tcCamera.SuspendLayout();
            this.tpCameraName.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.gbCameraName.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbCameras
            // 
            this.cbCameras.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbCameras.DisplayMember = "Name";
            this.cbCameras.FormattingEnabled = true;
            this.cbCameras.Location = new System.Drawing.Point(12, 21);
            this.cbCameras.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cbCameras.Name = "cbCameras";
            this.cbCameras.Size = new System.Drawing.Size(331, 24);
            this.cbCameras.Sorted = true;
            this.cbCameras.TabIndex = 5;
            this.cbCameras.Text = "Choose a camera to control";
            this.cbCameras.ValueMember = "VideoDevice.MonikerString";
            this.cbCameras.SelectedIndexChanged += new System.EventHandler(this.cbCameras_SelectedIndexChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.cbCameras);
            this.groupBox3.Location = new System.Drawing.Point(16, 10);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox3.Size = new System.Drawing.Size(352, 58);
            this.groupBox3.TabIndex = 6;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Available Cameras";
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(122, 240);
            this.btnStop.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(100, 28);
            this.btnStop.TabIndex = 3;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // gbStatus
            // 
            this.gbStatus.Controls.Add(this.lblStatus);
            this.gbStatus.Location = new System.Drawing.Point(55, 26);
            this.gbStatus.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gbStatus.Name = "gbStatus";
            this.gbStatus.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gbStatus.Size = new System.Drawing.Size(233, 41);
            this.gbStatus.TabIndex = 2;
            this.gbStatus.TabStop = false;
            this.gbStatus.Text = "Status";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblStatus.Location = new System.Drawing.Point(4, 19);
            this.lblStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(61, 17);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "Stopped";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // btnView
            // 
            this.btnView.Location = new System.Drawing.Point(32, 96);
            this.btnView.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnView.Name = "btnView";
            this.btnView.Size = new System.Drawing.Size(116, 28);
            this.btnView.TabIndex = 0;
            this.btnView.Text = "View Camera";
            this.btnView.UseVisualStyleBackColor = true;
            this.btnView.Click += new System.EventHandler(this.btnView_Click);
            // 
            // tcCamera
            // 
            this.tcCamera.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tcCamera.Controls.Add(this.tpCameraName);
            this.tcCamera.Controls.Add(this.tabPage2);
            this.tcCamera.Controls.Add(this.tabPage3);
            this.tcCamera.Location = new System.Drawing.Point(16, 80);
            this.tcCamera.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tcCamera.Name = "tcCamera";
            this.tcCamera.SelectedIndex = 0;
            this.tcCamera.Size = new System.Drawing.Size(352, 326);
            this.tcCamera.TabIndex = 8;
            // 
            // tpCameraName
            // 
            this.tpCameraName.Controls.Add(this.btnCaptureFrame);
            this.tpCameraName.Controls.Add(this.btnFTP);
            this.tpCameraName.Controls.Add(this.btnRecordCamera);
            this.tpCameraName.Controls.Add(this.btnRecordForWeb);
            this.tpCameraName.Controls.Add(this.gbStatus);
            this.tpCameraName.Controls.Add(this.btnStop);
            this.tpCameraName.Controls.Add(this.btnView);
            this.tpCameraName.Location = new System.Drawing.Point(4, 25);
            this.tpCameraName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tpCameraName.Name = "tpCameraName";
            this.tpCameraName.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tpCameraName.Size = new System.Drawing.Size(344, 297);
            this.tpCameraName.TabIndex = 0;
            this.tpCameraName.Text = "Camera Name";
            this.tpCameraName.UseVisualStyleBackColor = true;
            // 
            // btnFTP
            // 
            this.btnFTP.Location = new System.Drawing.Point(197, 155);
            this.btnFTP.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnFTP.Name = "btnFTP";
            this.btnFTP.Size = new System.Drawing.Size(100, 28);
            this.btnFTP.TabIndex = 6;
            this.btnFTP.Text = "FTP to Web";
            this.btnFTP.UseVisualStyleBackColor = true;
            this.btnFTP.Click += new System.EventHandler(this.btnFTP_Click);
            // 
            // btnRecordCamera
            // 
            this.btnRecordCamera.Location = new System.Drawing.Point(187, 96);
            this.btnRecordCamera.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnRecordCamera.Name = "btnRecordCamera";
            this.btnRecordCamera.Size = new System.Drawing.Size(123, 28);
            this.btnRecordCamera.TabIndex = 5;
            this.btnRecordCamera.Text = "Record Camera";
            this.btnRecordCamera.UseVisualStyleBackColor = true;
            this.btnRecordCamera.Click += new System.EventHandler(this.btnRecordCamera_Click);
            // 
            // btnRecordForWeb
            // 
            this.btnRecordForWeb.Location = new System.Drawing.Point(24, 155);
            this.btnRecordForWeb.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnRecordForWeb.Name = "btnRecordForWeb";
            this.btnRecordForWeb.Size = new System.Drawing.Size(131, 28);
            this.btnRecordForWeb.TabIndex = 4;
            this.btnRecordForWeb.Text = "Record For Web";
            this.btnRecordForWeb.UseVisualStyleBackColor = true;
            this.btnRecordForWeb.Click += new System.EventHandler(this.btnRecordForWeb_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox5);
            this.tabPage2.Controls.Add(this.groupBox4);
            this.tabPage2.Controls.Add(this.groupBox2);
            this.tabPage2.Controls.Add(this.gbCameraName);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage2.Size = new System.Drawing.Size(344, 297);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Edit Camera";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.button1);
            this.groupBox5.Controls.Add(this.txtRecordPath);
            this.groupBox5.Location = new System.Drawing.Point(40, 196);
            this.groupBox5.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox5.Size = new System.Drawing.Size(267, 87);
            this.groupBox5.TabIndex = 4;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Record path";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(83, 52);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(100, 28);
            this.button1.TabIndex = 1;
            this.button1.Text = "Browse";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // txtRecordPath
            // 
            this.txtRecordPath.Location = new System.Drawing.Point(4, 20);
            this.txtRecordPath.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtRecordPath.Name = "txtRecordPath";
            this.txtRecordPath.Size = new System.Drawing.Size(253, 22);
            this.txtRecordPath.TabIndex = 0;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.txtFrameRate);
            this.groupBox4.Location = new System.Drawing.Point(40, 128);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox4.Size = new System.Drawing.Size(267, 60);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Frame Rate";
            // 
            // txtFrameRate
            // 
            this.txtFrameRate.Location = new System.Drawing.Point(7, 23);
            this.txtFrameRate.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtFrameRate.Name = "txtFrameRate";
            this.txtFrameRate.Size = new System.Drawing.Size(251, 22);
            this.txtFrameRate.TabIndex = 0;
            this.txtFrameRate.Text = "5";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cbFrameSize);
            this.groupBox2.Location = new System.Drawing.Point(40, 66);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox2.Size = new System.Drawing.Size(267, 54);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Frame Size";
            // 
            // cbFrameSize
            // 
            this.cbFrameSize.FormattingEnabled = true;
            this.cbFrameSize.Location = new System.Drawing.Point(7, 18);
            this.cbFrameSize.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cbFrameSize.Name = "cbFrameSize";
            this.cbFrameSize.Size = new System.Drawing.Size(251, 24);
            this.cbFrameSize.TabIndex = 0;
            this.cbFrameSize.Text = "Default";
            this.cbFrameSize.SelectedIndexChanged += new System.EventHandler(this.cbFrameSize_SelectedIndexChanged);
            // 
            // gbCameraName
            // 
            this.gbCameraName.Controls.Add(this.txtCameraName);
            this.gbCameraName.Location = new System.Drawing.Point(40, 5);
            this.gbCameraName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gbCameraName.Name = "gbCameraName";
            this.gbCameraName.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gbCameraName.Size = new System.Drawing.Size(267, 54);
            this.gbCameraName.TabIndex = 0;
            this.gbCameraName.TabStop = false;
            this.gbCameraName.Text = "Camera Name";
            // 
            // txtCameraName
            // 
            this.txtCameraName.Location = new System.Drawing.Point(7, 20);
            this.txtCameraName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtCameraName.Name = "txtCameraName";
            this.txtCameraName.Size = new System.Drawing.Size(253, 22);
            this.txtCameraName.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.lblCameraCapabilities);
            this.tabPage3.Location = new System.Drawing.Point(4, 25);
            this.tabPage3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(344, 297);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Camera Capabilities";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // lblCameraCapabilities
            // 
            this.lblCameraCapabilities.AutoSize = true;
            this.lblCameraCapabilities.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCameraCapabilities.Location = new System.Drawing.Point(0, 0);
            this.lblCameraCapabilities.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCameraCapabilities.Name = "lblCameraCapabilities";
            this.lblCameraCapabilities.Size = new System.Drawing.Size(277, 17);
            this.lblCameraCapabilities.TabIndex = 0;
            this.lblCameraCapabilities.Text = "Camera must be playing to see capabilities";
            this.lblCameraCapabilities.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // btnCaptureFrame
            // 
            this.btnCaptureFrame.Location = new System.Drawing.Point(106, 200);
            this.btnCaptureFrame.Name = "btnCaptureFrame";
            this.btnCaptureFrame.Size = new System.Drawing.Size(133, 24);
            this.btnCaptureFrame.TabIndex = 7;
            this.btnCaptureFrame.Text = "Capture Frame";
            this.btnCaptureFrame.UseVisualStyleBackColor = true;
            this.btnCaptureFrame.Click += new System.EventHandler(this.btnCaptureFrame_Click);
            // 
            // SCTVCameraMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 412);
            this.Controls.Add(this.tcCamera);
            this.Controls.Add(this.groupBox3);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "SCTVCameraMain";
            this.Opacity = 0;
            this.Text = "SCTV Camera";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SCTVCameraMain_FormClosing);
            this.groupBox3.ResumeLayout(false);
            this.gbStatus.ResumeLayout(false);
            this.gbStatus.PerformLayout();
            this.tcCamera.ResumeLayout(false);
            this.tpCameraName.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.gbCameraName.ResumeLayout(false);
            this.gbCameraName.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cbCameras;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox gbStatus;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnView;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.TabControl tcCamera;
        private System.Windows.Forms.TabPage tpCameraName;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox gbCameraName;
        private System.Windows.Forms.TextBox txtCameraName;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox cbFrameSize;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Label lblCameraCapabilities;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox txtFrameRate;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox txtRecordPath;
        private System.Windows.Forms.Button btnRecordCamera;
        private System.Windows.Forms.Button btnRecordForWeb;
        private System.Windows.Forms.Button btnFTP;
        private System.Windows.Forms.Button btnCaptureFrame;
    }
}