namespace SCTVJustinTV
{
    partial class BroadcastDisplay
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
            this.wbJustin = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // wbJustin
            // 
            this.wbJustin.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wbJustin.Location = new System.Drawing.Point(0, 0);
            this.wbJustin.MinimumSize = new System.Drawing.Size(20, 20);
            this.wbJustin.Name = "wbJustin";
            this.wbJustin.Size = new System.Drawing.Size(739, 605);
            this.wbJustin.TabIndex = 0;
            // 
            // BroadcastDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(739, 605);
            this.Controls.Add(this.wbJustin);
            this.Name = "BroadcastDisplay";
            this.Text = "BroadcastDisplay";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser wbJustin;
    }
}