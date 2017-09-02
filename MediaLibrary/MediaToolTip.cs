using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System;
using SCTVObjects;

namespace SCTV
{
    public partial class MediaToolTip : GlassWindow
    {
        string title = "";
        string coverImagePath = "";
        string description = "";
        private Bitmap bmp;
        Media mediaToShow;
        
        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        public string CoverImagePath
        {
            get { return coverImagePath; }
            set { coverImagePath = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public MediaToolTip(Media MediaToShow)
        {
            InitializeComponent();

            this.BackgroundImagePath = "images\\tooltip.gif";

            mediaToShow = MediaToShow;
        }

        private void MediaToolTip_Load(object sender, EventArgs e)
        {
            lblTitle.Text = mediaToShow.Title;

            if(mediaToShow.Rating.Trim().Length > 0)
                lblTitle.Text += " ("+ mediaToShow.Rating +")";

            txtDescription.Text = mediaToShow.Description;

            txtFunFacts.Text = mediaToShow.Description;            

            if (System.IO.File.Exists(mediaToShow.coverImage))
                pbCoverImage.ImageLocation = mediaToShow.coverImage;
            else
                pbCoverImage.ImageLocation = "images\\media\\coverimages\\notavailable.jpg";
        }

        private void MediaToolTip_Paint(object sender, PaintEventArgs e)
        {
           
        }

        private void llMore_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (llMore.Text == "more...")
            {
                this.AutoScroll = true;
                llMore.Text = "less...";
            }
            else
            {
                this.AutoScroll = false;
                llMore.Text = "more...";
            }
        }
    }
}