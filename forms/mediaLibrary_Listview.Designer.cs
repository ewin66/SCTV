namespace SCTV
{
    partial class MediaLibrary_Listview
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MediaLibrary_Listview));
            this.lvMedia = new System.Windows.Forms.ListView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.tcCategories = new System.Windows.Forms.TabControl();
            this.SuspendLayout();
            // 
            // lvMedia
            // 
            this.lvMedia.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.lvMedia.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lvMedia.Font = new System.Drawing.Font("Comic Sans MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvMedia.HideSelection = false;
            this.lvMedia.HoverSelection = true;
            this.lvMedia.LargeImageList = this.imageList1;
            this.lvMedia.Location = new System.Drawing.Point(0, 44);
            this.lvMedia.MultiSelect = false;
            this.lvMedia.Name = "lvMedia";
            this.lvMedia.Size = new System.Drawing.Size(357, 222);
            this.lvMedia.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvMedia.TabIndex = 0;
            this.lvMedia.TileSize = new System.Drawing.Size(200, 200);
            this.lvMedia.UseCompatibleStateImageBehavior = false;
            this.lvMedia.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lvMedia_MouseDoubleClick);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "boba-fett.jpg");
            this.imageList1.Images.SetKeyName(1, "RoadRunner.bmp");
            this.imageList1.Images.SetKeyName(2, "autumnSkull.jpg");
            this.imageList1.Images.SetKeyName(3, "darth-maul.3.jpg");
            // 
            // tcCategories
            // 
            this.tcCategories.Dock = System.Windows.Forms.DockStyle.Top;
            this.tcCategories.HotTrack = true;
            this.tcCategories.ItemSize = new System.Drawing.Size(58, 40);
            this.tcCategories.Location = new System.Drawing.Point(0, 0);
            this.tcCategories.Name = "tcCategories";
            this.tcCategories.SelectedIndex = 0;
            this.tcCategories.Size = new System.Drawing.Size(357, 43);
            this.tcCategories.TabIndex = 1;
            this.tcCategories.SelectedIndexChanged += new System.EventHandler(this.tcCategories_SelectedIndexChanged);
            // 
            // MediaLibrary_Listview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(357, 266);
            this.Controls.Add(this.tcCategories);
            this.Controls.Add(this.lvMedia);
            this.Name = "MediaLibrary_Listview";
            this.Text = "mediaLibrary";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lvMedia;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.TabControl tcCategories;

    }
}