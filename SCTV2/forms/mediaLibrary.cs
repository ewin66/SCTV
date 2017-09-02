using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SCTV
{
    public partial class mediaLibrary : Form
    {
        public mediaLibrary()
        {
            InitializeComponent();

            displayMedia();
        }

        private void displayMedia()
        {
            dgvMedia.AutoGenerateColumns = false;
            dgvMedia.DataSource = SCTV.myMedia.getMedia();
            dgvMedia.DataMember = "media";

            dgvMedia.Columns.Clear(); 
            DataGridViewTextBoxColumn dgridColID = new DataGridViewTextBoxColumn(); 
            dgridColID.HeaderText = "ID"; 
            dgridColID.Name = "ColCustomerID"; 
            dgridColID.Width = 20; 
            dgridColID.DataPropertyName = "CustomerID";
            dgvMedia.Columns.Add(dgridColID);
        }

        /// <summary>
        /// play selected media
        /// </summary>
        //public void playMedia()//play selected media
        //{
        //    try
        //    {
        //        string selectedMedia = getSelectedMediaFileName();

        //        if (File.Exists(selectedMedia))
        //        {
        //            splashScreen mediaSplash = new splashScreen();

        //            if (Form1.Mode == "Release")
        //            {
        //                mediaSplash.SplashMessage2 = selectedMedia;

        //                mediaSplash.Show();
        //                mediaSplash.TopMost = true;
        //            }

        //            //this.Hide();
        //            createMediaWindow();
        //            //						mediaWindow.playVideo(selectedLabel.Tag.ToString());
        //            mediaWindow.PlayClip(selectedMedia);

        //            if (Form1.Mode == "Release")
        //            {
        //                mediaSplash.Close();
        //                mediaWindow.TopMost = true;
        //                mediaWindow.Focus();
        //            }

        //            mediaWindow.Show();
        //        }
        //        else
        //        {
        //            EventLog.WriteEntry("sctv", "---- Missing File : " + selectedMedia + " ----");
        //            MessageBox.Show("Missing File - " + selectedMedia);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        EventLog.WriteEntry("sctv", "PlayMedia error: " + ex.Message);
        //    }
        //}

        //private void createMediaWindow()
        //{
        //    //EventLog.WriteEntry("sctv", "in createMediaWindow");
        //    try
        //    {
        //        if (mediaWindow == null)
        //        {
        //            mediaWindow = new liquidMediaPlayer();
        //            mediaWindow.Closed += new EventHandler(mediaWindow_Closed);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        EventLog.WriteEntry("sctv", "createMediaWindow error: " + ex.Message);
        //    }
        //}
    }
}