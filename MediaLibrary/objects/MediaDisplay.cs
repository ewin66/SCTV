using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SCTV
{
    public partial class MediaDisplay : Form
    {
        public MediaDisplay()
        {
            InitializeComponent();
        }

        public void AddMedia(Media mediaToDisplay)
        {
            PictureBox pb = new PictureBox();
            pb.ImageLocation = "";

            ReadOnlyRichTextBox rtb = new ReadOnlyRichTextBox();
            rtb.ScrollBars = RichTextBoxScrollBars.None;
            rtb.BorderStyle = BorderStyle.None;
            rtb.WordWrap = true;
            rtb.BackColor = Color.White;
            rtb.Cursor = Cursors.Default;
            rtb.AddMedia(mediaToDisplay);


            tlpMedia.Controls.Add(pb);
            tlpMedia.Controls.Add(rtb);
        }
    }
}