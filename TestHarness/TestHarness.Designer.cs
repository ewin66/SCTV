namespace MyTest
{
    partial class TestHarness
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
            this.button1 = new System.Windows.Forms.Button();
            this.pbTestImage = new System.Windows.Forms.PictureBox();
            this.btnShowSplashScreen = new System.Windows.Forms.Button();
            this.txtResources = new System.Windows.Forms.TextBox();
            this.btnTrivia = new System.Windows.Forms.Button();
            this.btnMakeThumbnail = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pbTestImage)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 22);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(127, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Create Glass Window";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // pbTestImage
            // 
            this.pbTestImage.Location = new System.Drawing.Point(164, 22);
            this.pbTestImage.Name = "pbTestImage";
            this.pbTestImage.Size = new System.Drawing.Size(349, 191);
            this.pbTestImage.TabIndex = 1;
            this.pbTestImage.TabStop = false;
            this.pbTestImage.Paint += new System.Windows.Forms.PaintEventHandler(this.pbTestImage_Paint);
            // 
            // btnShowSplashScreen
            // 
            this.btnShowSplashScreen.Location = new System.Drawing.Point(12, 64);
            this.btnShowSplashScreen.Name = "btnShowSplashScreen";
            this.btnShowSplashScreen.Size = new System.Drawing.Size(127, 23);
            this.btnShowSplashScreen.TabIndex = 2;
            this.btnShowSplashScreen.Text = "Splash Screen";
            this.btnShowSplashScreen.UseVisualStyleBackColor = true;
            this.btnShowSplashScreen.Click += new System.EventHandler(this.btnShowSplashScreen_Click);
            // 
            // txtResources
            // 
            this.txtResources.Location = new System.Drawing.Point(98, 335);
            this.txtResources.Multiline = true;
            this.txtResources.Name = "txtResources";
            this.txtResources.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtResources.Size = new System.Drawing.Size(536, 206);
            this.txtResources.TabIndex = 4;
            // 
            // btnTrivia
            // 
            this.btnTrivia.Location = new System.Drawing.Point(12, 93);
            this.btnTrivia.Name = "btnTrivia";
            this.btnTrivia.Size = new System.Drawing.Size(127, 23);
            this.btnTrivia.TabIndex = 5;
            this.btnTrivia.Text = "Trivia";
            this.btnTrivia.UseVisualStyleBackColor = true;
            this.btnTrivia.Click += new System.EventHandler(this.btnTrivia_Click);
            // 
            // btnMakeThumbnail
            // 
            this.btnMakeThumbnail.Location = new System.Drawing.Point(12, 122);
            this.btnMakeThumbnail.Name = "btnMakeThumbnail";
            this.btnMakeThumbnail.Size = new System.Drawing.Size(127, 23);
            this.btnMakeThumbnail.TabIndex = 6;
            this.btnMakeThumbnail.Text = "Make Thumbnail";
            this.btnMakeThumbnail.UseVisualStyleBackColor = true;
            this.btnMakeThumbnail.Click += new System.EventHandler(this.btnMakeThumbnail_Click);
            // 
            // TestHarness
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(736, 659);
            this.Controls.Add(this.btnMakeThumbnail);
            this.Controls.Add(this.btnTrivia);
            this.Controls.Add(this.txtResources);
            this.Controls.Add(this.btnShowSplashScreen);
            this.Controls.Add(this.pbTestImage);
            this.Controls.Add(this.button1);
            this.Name = "TestHarness";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbTestImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.PictureBox pbTestImage;
        private System.Windows.Forms.Button btnShowSplashScreen;
        private System.Windows.Forms.TextBox txtResources;
        private System.Windows.Forms.Button btnTrivia;
        private System.Windows.Forms.Button btnMakeThumbnail;

    }
}

