namespace SCTV
{
    partial class IMDBInfo
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
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.directorTextBox = new System.Windows.Forms.TextBox();
            this.genreTextBox = new System.Windows.Forms.TextBox();
            this.releaseYearTextBox = new System.Windows.Forms.TextBox();
            this.tagLineTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.imdbNoTextBox = new System.Windows.Forms.TextBox();
            this.getDetailsButton = new System.Windows.Forms.Button();
            this.thumbnailPictureBox = new System.Windows.Forms.PictureBox();
            this.label6 = new System.Windows.Forms.Label();
            this.titleTextBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.thumbnailPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(33, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Director(s):";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(52, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Genre:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(17, 93);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(74, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Release Year:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(39, 145);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(52, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "Tag Line:";
            // 
            // directorTextBox
            // 
            this.directorTextBox.Location = new System.Drawing.Point(97, 38);
            this.directorTextBox.Name = "directorTextBox";
            this.directorTextBox.Size = new System.Drawing.Size(297, 20);
            this.directorTextBox.TabIndex = 7;
            // 
            // genreTextBox
            // 
            this.genreTextBox.Location = new System.Drawing.Point(97, 64);
            this.genreTextBox.Name = "genreTextBox";
            this.genreTextBox.Size = new System.Drawing.Size(297, 20);
            this.genreTextBox.TabIndex = 9;
            // 
            // releaseYearTextBox
            // 
            this.releaseYearTextBox.Location = new System.Drawing.Point(97, 90);
            this.releaseYearTextBox.Name = "releaseYearTextBox";
            this.releaseYearTextBox.Size = new System.Drawing.Size(297, 20);
            this.releaseYearTextBox.TabIndex = 11;
            // 
            // tagLineTextBox
            // 
            this.tagLineTextBox.Location = new System.Drawing.Point(97, 142);
            this.tagLineTextBox.Multiline = true;
            this.tagLineTextBox.Name = "tagLineTextBox";
            this.tagLineTextBox.Size = new System.Drawing.Size(297, 60);
            this.tagLineTextBox.TabIndex = 13;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 119);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "IMDB Number:";
            // 
            // imdbNoTextBox
            // 
            this.imdbNoTextBox.Location = new System.Drawing.Point(97, 116);
            this.imdbNoTextBox.Name = "imdbNoTextBox";
            this.imdbNoTextBox.Size = new System.Drawing.Size(297, 20);
            this.imdbNoTextBox.TabIndex = 1;
            // 
            // getDetailsButton
            // 
            this.getDetailsButton.Location = new System.Drawing.Point(233, 226);
            this.getDetailsButton.Name = "getDetailsButton";
            this.getDetailsButton.Size = new System.Drawing.Size(75, 23);
            this.getDetailsButton.TabIndex = 2;
            this.getDetailsButton.Text = "Get Details";
            this.getDetailsButton.UseVisualStyleBackColor = true;
            this.getDetailsButton.Click += new System.EventHandler(this.getDetailsButton_Click);
            // 
            // thumbnailPictureBox
            // 
            this.thumbnailPictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.thumbnailPictureBox.Location = new System.Drawing.Point(409, 15);
            this.thumbnailPictureBox.Name = "thumbnailPictureBox";
            this.thumbnailPictureBox.Size = new System.Drawing.Size(118, 161);
            this.thumbnailPictureBox.TabIndex = 14;
            this.thumbnailPictureBox.TabStop = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(61, 15);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(30, 13);
            this.label6.TabIndex = 4;
            this.label6.Text = "Title:";
            // 
            // titleTextBox
            // 
            this.titleTextBox.Location = new System.Drawing.Point(97, 12);
            this.titleTextBox.Name = "titleTextBox";
            this.titleTextBox.Size = new System.Drawing.Size(297, 20);
            this.titleTextBox.TabIndex = 5;
            // 
            // mainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(541, 261);
            this.Controls.Add(this.titleTextBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.thumbnailPictureBox);
            this.Controls.Add(this.getDetailsButton);
            this.Controls.Add(this.imdbNoTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tagLineTextBox);
            this.Controls.Add(this.releaseYearTextBox);
            this.Controls.Add(this.genreTextBox);
            this.Controls.Add(this.directorTextBox);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Name = "mainForm";
            this.Text = "Get Movie Details";
            ((System.ComponentModel.ISupportInitialize)(this.thumbnailPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox directorTextBox;
        private System.Windows.Forms.TextBox genreTextBox;
        private System.Windows.Forms.TextBox releaseYearTextBox;
        private System.Windows.Forms.TextBox tagLineTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox imdbNoTextBox;
        private System.Windows.Forms.Button getDetailsButton;
        private System.Windows.Forms.PictureBox thumbnailPictureBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox titleTextBox;

    }
}

