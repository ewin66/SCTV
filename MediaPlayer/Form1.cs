using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MediaPlayer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            try
            {
                //LibVlc vlc = new LibVlc();
                //vlc.Initialize();
                //vlc.VideoOutput = pbVideo;
                //vlc.PlaylistClear();
                //string[] Options = new string[] { ":sout=#duplicate{dst=display,dst=std {access=udp,mux=ts,dst=224.100.0.1:1234}}" };
                //vlc.AddTarget(@"dvd://e:\", Options);
                //vlc.Play();
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.ToString());
            }
        }
    }
}