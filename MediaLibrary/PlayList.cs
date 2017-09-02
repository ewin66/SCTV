using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace SCTV
{
    public partial class PlayList : Form
    {
        ArrayList _playlist = new ArrayList();

        public ArrayList Playlist
        {
            set
            {
                _playlist = value;

                populatePlaylist(_playlist);
            }

            get
            {
                return _playlist;
            }
        }

        public PlayList()
        {
            InitializeComponent();
        }

        public PlayList(ArrayList playlist)
        {
            InitializeComponent();

            _playlist = playlist;

            populatePlaylist(playlist);
        }

        private void populatePlaylist(ArrayList playlist)
        {
            foreach (Media media in playlist)
            {
                ListViewItem li = new ListViewItem();
                li.Text = media.Title;
                li.Tag = media;
                lvPlaylist.Items.Add(li);
            }
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}