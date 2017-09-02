namespace SCTV
{
    partial class MediaToolTip
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
            this.pbCoverImage = new System.Windows.Forms.PictureBox();
            this.lblTitle = new System.Windows.Forms.Label();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lbCast = new System.Windows.Forms.ListBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtFunFacts = new System.Windows.Forms.TextBox();
            this.llMore = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.pbCoverImage)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // pbCoverImage
            // 
            this.pbCoverImage.BackColor = System.Drawing.SystemColors.Control;
            this.pbCoverImage.Location = new System.Drawing.Point(50, 34);
            this.pbCoverImage.Name = "pbCoverImage";
            this.pbCoverImage.Size = new System.Drawing.Size(98, 140);
            this.pbCoverImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbCoverImage.TabIndex = 0;
            this.pbCoverImage.TabStop = false;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Papyrus", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(55, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(113, 33);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "Star Wars";
            // 
            // txtDescription
            // 
            this.txtDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDescription.BackColor = System.Drawing.SystemColors.Control;
            this.txtDescription.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtDescription.Location = new System.Drawing.Point(6, 19);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.ReadOnly = true;
            this.txtDescription.Size = new System.Drawing.Size(422, 115);
            this.txtDescription.TabIndex = 2;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.txtDescription);
            this.groupBox1.Location = new System.Drawing.Point(154, 34);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(434, 140);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Description";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.lbCast);
            this.groupBox2.Location = new System.Drawing.Point(50, 180);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(538, 141);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Cast";
            // 
            // lbCast
            // 
            this.lbCast.BackColor = System.Drawing.SystemColors.Control;
            this.lbCast.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lbCast.FormattingEnabled = true;
            this.lbCast.Location = new System.Drawing.Point(6, 19);
            this.lbCast.Name = "lbCast";
            this.lbCast.Size = new System.Drawing.Size(388, 104);
            this.lbCast.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.txtFunFacts);
            this.groupBox3.Location = new System.Drawing.Point(50, 327);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(538, 245);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Fun Facts";
            // 
            // txtFunFacts
            // 
            this.txtFunFacts.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFunFacts.BackColor = System.Drawing.SystemColors.Control;
            this.txtFunFacts.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtFunFacts.Location = new System.Drawing.Point(6, 19);
            this.txtFunFacts.Multiline = true;
            this.txtFunFacts.Name = "txtFunFacts";
            this.txtFunFacts.ReadOnly = true;
            this.txtFunFacts.Size = new System.Drawing.Size(526, 220);
            this.txtFunFacts.TabIndex = 2;
            // 
            // llMore
            // 
            this.llMore.AutoSize = true;
            this.llMore.BackColor = System.Drawing.SystemColors.ControlLight;
            this.llMore.Location = new System.Drawing.Point(543, 24);
            this.llMore.Name = "llMore";
            this.llMore.Size = new System.Drawing.Size(39, 13);
            this.llMore.TabIndex = 3;
            this.llMore.TabStop = true;
            this.llMore.Text = "more...";
            this.llMore.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llMore_LinkClicked);
            // 
            // MediaToolTip
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = false;
            this.ClientSize = new System.Drawing.Size(617, 215);
            this.Controls.Add(this.llMore);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.pbCoverImage);
            this.Name = "MediaToolTip";
            this.Text = "Media Details";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.MediaToolTip_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.MediaToolTip_Paint);
            ((System.ComponentModel.ISupportInitialize)(this.pbCoverImage)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pbCoverImage;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox lbCast;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txtFunFacts;
        private System.Windows.Forms.LinkLabel llMore;
    }
}