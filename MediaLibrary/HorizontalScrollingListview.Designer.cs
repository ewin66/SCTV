namespace SCTV
{
    partial class HorizontalScrollingListview
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HorizontalScrollingListview));
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("", 0);
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("", 1);
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("", 2);
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("", 3);
            System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem("", 4);
            System.Windows.Forms.ListViewItem listViewItem6 = new System.Windows.Forms.ListViewItem("", 0);
            System.Windows.Forms.ListViewItem listViewItem7 = new System.Windows.Forms.ListViewItem("", 1);
            System.Windows.Forms.ListViewItem listViewItem8 = new System.Windows.Forms.ListViewItem("", 2);
            this.btnScrollLeft = new System.Windows.Forms.Button();
            this.ilImages = new System.Windows.Forms.ImageList(this.components);
            this.btnScrollRight = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.lvMedia = new SCTV.ListView.ListViewFF();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.gbTitle = new System.Windows.Forms.GroupBox();
            this.gbTitle.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnScrollLeft
            // 
            this.btnScrollLeft.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.btnScrollLeft.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.btnScrollLeft.Cursor = System.Windows.Forms.Cursors.NoMoveHoriz;
            this.btnScrollLeft.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            this.btnScrollLeft.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnScrollLeft.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnScrollLeft.ForeColor = System.Drawing.SystemColors.Window;
            this.btnScrollLeft.Location = new System.Drawing.Point(0, 19);
            this.btnScrollLeft.Name = "btnScrollLeft";
            this.btnScrollLeft.Size = new System.Drawing.Size(26, 161);
            this.btnScrollLeft.TabIndex = 4;
            this.btnScrollLeft.Text = "<";
            this.btnScrollLeft.UseVisualStyleBackColor = false;
            this.btnScrollLeft.MouseLeave += new System.EventHandler(this.btnScrollLeft_MouseLeave);
            this.btnScrollLeft.MouseHover += new System.EventHandler(this.btnScrollLeft_MouseHover);
            // 
            // ilImages
            // 
            this.ilImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilImages.ImageStream")));
            this.ilImages.TransparentColor = System.Drawing.Color.Transparent;
            this.ilImages.Images.SetKeyName(0, "cbs.jpg");
            this.ilImages.Images.SetKeyName(1, "gamesgames.jpg");
            this.ilImages.Images.SetKeyName(2, "joost.jpg");
            this.ilImages.Images.SetKeyName(3, "iTunes.jpg");
            this.ilImages.Images.SetKeyName(4, "Pandora.jpg");
            // 
            // btnScrollRight
            // 
            this.btnScrollRight.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnScrollRight.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.btnScrollRight.Cursor = System.Windows.Forms.Cursors.NoMoveHoriz;
            this.btnScrollRight.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            this.btnScrollRight.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnScrollRight.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnScrollRight.ForeColor = System.Drawing.SystemColors.Window;
            this.btnScrollRight.Location = new System.Drawing.Point(1030, 19);
            this.btnScrollRight.Name = "btnScrollRight";
            this.btnScrollRight.Size = new System.Drawing.Size(23, 161);
            this.btnScrollRight.TabIndex = 6;
            this.btnScrollRight.Text = ">";
            this.btnScrollRight.UseVisualStyleBackColor = false;
            this.btnScrollRight.MouseLeave += new System.EventHandler(this.btnScrollRight_MouseLeave);
            this.btnScrollRight.MouseHover += new System.EventHandler(this.btnScrollRight_MouseHover);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.Transparent;
            this.button1.Cursor = System.Windows.Forms.Cursors.NoMoveHoriz;
            this.button1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ForeColor = System.Drawing.SystemColors.Window;
            this.button1.Location = new System.Drawing.Point(1034, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(22, 135);
            this.button1.TabIndex = 6;
            this.button1.Text = ">";
            this.button1.UseVisualStyleBackColor = false;
            // 
            // lvMedia
            // 
            this.lvMedia.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.lvMedia.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvMedia.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvMedia.HideSelection = false;
            this.lvMedia.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3,
            listViewItem4,
            listViewItem5,
            listViewItem6,
            listViewItem7,
            listViewItem8});
            this.lvMedia.LargeImageList = this.ilImages;
            this.lvMedia.Location = new System.Drawing.Point(24, 19);
            this.lvMedia.MarqueeSpeed = 0;
            this.lvMedia.MultiSelect = false;
            this.lvMedia.Name = "lvMedia";
            this.lvMedia.OwnerDraw = true;
            this.lvMedia.Scrollable = false;
            this.lvMedia.Size = new System.Drawing.Size(1007, 161);
            this.lvMedia.SmallImageList = this.ilImages;
            this.lvMedia.StateImageList = this.ilImages;
            this.lvMedia.TabIndex = 5;
            this.lvMedia.UseCompatibleStateImageBehavior = false;
            this.lvMedia.View = System.Windows.Forms.View.List;
            this.lvMedia.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.lvMedia_DrawItem);
            this.lvMedia.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lvMedia_MouseClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 300;
            // 
            // gbTitle
            // 
            this.gbTitle.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbTitle.Controls.Add(this.lvMedia);
            this.gbTitle.Controls.Add(this.btnScrollRight);
            this.gbTitle.Controls.Add(this.btnScrollLeft);
            this.gbTitle.Location = new System.Drawing.Point(0, 0);
            this.gbTitle.Name = "gbTitle";
            this.gbTitle.Size = new System.Drawing.Size(1053, 185);
            this.gbTitle.TabIndex = 7;
            this.gbTitle.TabStop = false;
            // 
            // HorizontalScrollingListview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.gbTitle);
            this.Name = "HorizontalScrollingListview";
            this.Size = new System.Drawing.Size(1056, 188);
            this.gbTitle.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnScrollLeft;
        private ListView.ListViewFF lvMedia;
        private System.Windows.Forms.Button btnScrollRight;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ImageList ilImages;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.GroupBox gbTitle;
    }
}
