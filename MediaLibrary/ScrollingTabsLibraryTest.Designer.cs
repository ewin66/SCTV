namespace SCTV
{
    partial class ScrollingTabsLibraryTest
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScrollingTabsLibraryTest));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.horizontalScrollingListview2 = new SCTV.HorizontalScrollingListview();
            this.panel1 = new System.Windows.Forms.Panel();
            this.horizontalScrollingListview3 = new SCTV.HorizontalScrollingListview();
            this.horizontalScrollingListview4 = new SCTV.HorizontalScrollingListview();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "RaidersoOfTheLostArk.jpg");
            this.imageList1.Images.SetKeyName(1, "NotAvailable.jpg");
            this.imageList1.Images.SetKeyName(2, "cbs.jpg");
            this.imageList1.Images.SetKeyName(3, "dictionary.jpg");
            this.imageList1.Images.SetKeyName(4, "espn.jpg");
            this.imageList1.Images.SetKeyName(5, "forgottenBooks.jpg");
            this.imageList1.Images.SetKeyName(6, "gamesgames.jpg");
            // 
            // horizontalScrollingListview2
            // 
            this.horizontalScrollingListview2.BackColor = System.Drawing.Color.Transparent;
            this.horizontalScrollingListview2.Location = new System.Drawing.Point(39, 37);
            this.horizontalScrollingListview2.Name = "horizontalScrollingListview2";
            this.horizontalScrollingListview2.Size = new System.Drawing.Size(1056, 151);
            this.horizontalScrollingListview2.TabIndex = 4;
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.horizontalScrollingListview4);
            this.panel1.Controls.Add(this.horizontalScrollingListview3);
            this.panel1.Controls.Add(this.horizontalScrollingListview2);
            this.panel1.Location = new System.Drawing.Point(38, 38);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1143, 345);
            this.panel1.TabIndex = 7;
            // 
            // horizontalScrollingListview3
            // 
            this.horizontalScrollingListview3.BackColor = System.Drawing.Color.Transparent;
            this.horizontalScrollingListview3.Location = new System.Drawing.Point(39, 208);
            this.horizontalScrollingListview3.Name = "horizontalScrollingListview3";
            this.horizontalScrollingListview3.Size = new System.Drawing.Size(1056, 151);
            this.horizontalScrollingListview3.TabIndex = 5;
            // 
            // horizontalScrollingListview4
            // 
            this.horizontalScrollingListview4.BackColor = System.Drawing.Color.Transparent;
            this.horizontalScrollingListview4.Location = new System.Drawing.Point(39, 365);
            this.horizontalScrollingListview4.Name = "horizontalScrollingListview4";
            this.horizontalScrollingListview4.Size = new System.Drawing.Size(1056, 151);
            this.horizontalScrollingListview4.TabIndex = 6;
            // 
            // ScrollingTabsLibraryTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1193, 630);
            this.Controls.Add(this.panel1);
            this.Name = "ScrollingTabsLibraryTest";
            this.Text = "ScrollingTabsLibraryTest";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ImageList imageList1;
        private HorizontalScrollingListview horizontalScrollingListview1;
        private HorizontalScrollingListview horizontalScrollingListview2;
        private System.Windows.Forms.Panel panel1;
        private HorizontalScrollingListview horizontalScrollingListview4;
        private HorizontalScrollingListview horizontalScrollingListview3;
    }
}