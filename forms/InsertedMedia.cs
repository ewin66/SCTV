using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SCTV
{
    public enum MediaStateEnum
    {
        Cancel, Play, PlayAndRecord, Record
    }

    public partial class InsertedMedia : Form
    {
        public InsertedMedia()
        {
            InitializeComponent(); 
        }

        public MediaStateEnum MediaState
        {
            get{return mediaState;}
        }

        public bool SkipMenu
        {
            get { return chbSkipMenu.Checked; }
            set { chbSkipMenu.Checked = value; }
        }

        private MediaStateEnum mediaState;

        private void btnPlay_Click(object sender, EventArgs e)
        {
            mediaState = MediaStateEnum.Play;
            this.Close();
        }

        private void btnPlayAndRecord_Click(object sender, EventArgs e)
        {
            mediaState = MediaStateEnum.PlayAndRecord;
            this.Close();
        }

        private void btnRecord_Click(object sender, EventArgs e)
        {
            //mediaState = MediaStateEnum.Record;
            mediaState = MediaStateEnum.Cancel;
            MessageBox.Show("Coming Soon!!");
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mediaState = MediaStateEnum.Cancel;
            this.Close();
        }
    }
}