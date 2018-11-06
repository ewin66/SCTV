using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SCTVObjects;

namespace SCTV
{
    public partial class AdvancedSettings : Form
    {
        string mediaXMLFilePath = "";
        string originalMediaFilePath = "";
        string mediaCoverImagePath = "";
        string originalMediaCoverImagePath = "";
        MediaHandler myMedia = new MediaHandler();

        public string MediaFilePath
        {
            get { return mediaXMLFilePath; }
            set { mediaXMLFilePath = value; }
        }

        public string MediaCoverImagePath
        {
            get { return mediaCoverImagePath; }
            set { mediaCoverImagePath = value; }
        }

        public AdvancedSettings(string currentMediaFilePath, string currentMediaCoverImagePath)
        {
            InitializeComponent();

            if (currentMediaFilePath.Trim().Length == 0)
                currentMediaFilePath = myMedia.MediaXMLFilePath;

            if (currentMediaCoverImagePath.Trim().Length == 0)
                currentMediaCoverImagePath = myMedia.ImagesXMLPath;

            mediaXMLFilePath = currentMediaFilePath;
            originalMediaFilePath = currentMediaFilePath;
            mediaCoverImagePath = currentMediaCoverImagePath;
            originalMediaCoverImagePath = currentMediaCoverImagePath;
            
            txtMediaFilePath.Text = mediaXMLFilePath;
            txtImagesXMLPath.Text = mediaCoverImagePath;
            //MediaHandler.ImagesDefaultPath = "";
            //MediaHandler.LocationFilePath = "";
            //MediaHandler.MediaTypesPath = "";
            //MediaHandler.MediaXMLFileDefaultPath = "";
            //MediaHandler.PlaylistXMLDefaultPath = "";
            //MediaHandler.PlaylistXMLFilePath = "";
        }

        private void AdvancedSettings_FormClosed(object sender, FormClosedEventArgs e)
        {
            //save the values if they changed
            myMedia.MediaXMLFilePath = txtMediaFilePath.Text.Trim();
            mediaXMLFilePath = txtMediaFilePath.Text.Trim();

            myMedia.ImagesXMLPath = txtImagesXMLPath.Text.Trim();
            mediaCoverImagePath = txtImagesXMLPath.Text.Trim();

            //MediaHandler.ImagesDefaultPath = "";
            //MediaHandler.LocationFilePath = "";
            //MediaHandler.MediaTypesPath = "";
            //MediaHandler.MediaXMLFileDefaultPath = "";
            //MediaHandler.PlaylistXMLDefaultPath = "";
            //MediaHandler.PlaylistXMLFilePath = "";
            
            
        }

        private void btnBrowseForFilePath_Click(object sender, EventArgs e)
        {
            string senderName = ((Button)sender).Name;

            Control[] foundControls = this.Controls.Find(senderName.Replace("btnBrowse", "txt"),true);
            Control txtControl = null;

            if (foundControls.Count() > 0)
                txtControl = foundControls[0];

            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = Application.StartupPath;
            openFileDialog1.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 0;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    txtControl.Text = openFileDialog1.FileName;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void btnDone_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
