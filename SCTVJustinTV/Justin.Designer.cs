namespace SCTVJustinTV
{
    partial class JustinTV
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
            this.wbJustinTV = new System.Windows.Forms.WebBrowser();
            this.pnlMessage = new System.Windows.Forms.Panel();
            this.lblMessage = new System.Windows.Forms.Label();
            this.pnlMessage.SuspendLayout();
            this.SuspendLayout();
            // 
            // wbJustinTV
            // 
            this.wbJustinTV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wbJustinTV.Location = new System.Drawing.Point(0, 0);
            this.wbJustinTV.Margin = new System.Windows.Forms.Padding(4);
            this.wbJustinTV.MinimumSize = new System.Drawing.Size(27, 25);
            this.wbJustinTV.Name = "wbJustinTV";
            this.wbJustinTV.Size = new System.Drawing.Size(389, 327);
            this.wbJustinTV.TabIndex = 0;
            // 
            // pnlMessage
            // 
            this.pnlMessage.Controls.Add(this.lblMessage);
            this.pnlMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMessage.Location = new System.Drawing.Point(0, 0);
            this.pnlMessage.Margin = new System.Windows.Forms.Padding(4);
            this.pnlMessage.Name = "pnlMessage";
            this.pnlMessage.Size = new System.Drawing.Size(389, 327);
            this.pnlMessage.TabIndex = 1;
            this.pnlMessage.Visible = false;
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblMessage.Location = new System.Drawing.Point(0, 0);
            this.lblMessage.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(58, 17);
            this.lblMessage.TabIndex = 0;
            this.lblMessage.Text = "No URL";
            this.lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // JustinTV
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(389, 327);
            this.Controls.Add(this.pnlMessage);
            this.Controls.Add(this.wbJustinTV);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "JustinTV";
            this.Text = "Justin.TV";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.pnlMessage.ResumeLayout(false);
            this.pnlMessage.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser wbJustinTV;
        private System.Windows.Forms.Panel pnlMessage;
        private System.Windows.Forms.Label lblMessage;
    }
}

