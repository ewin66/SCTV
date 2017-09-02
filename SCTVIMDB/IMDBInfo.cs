using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SCTVObjects;

namespace SCTV
{
    public partial class IMDBInfo : Form
    {
        string movieTitle = "";
        Media foundMedia = new Media();
        Media mediaToSearchFor = new Media();

        public string MovieTitle
        {
            set
            {
                movieTitle = value;

                txtTitleResult.Text = movieTitle;
            }

            get
            {
                return movieTitle;
            }
        }

        public Media FoundMedia
        {
            set
            {
                foundMedia = value; 
            }

            get
            {
                return foundMedia;
            }
        }

        public Media MediaToSearchFor
        {
            set
            {
                mediaToSearchFor = value;

                populateForm(value);
            }
        }

        public IMDBInfo()
        {
            InitializeComponent();
        }

        /// <summary>
        /// populate search ui with given media
        /// </summary>
        /// <param name="value"></param>
        private void populateForm(Media media)
        {
            if (media.filename == null)
            {
                string[] filepath = media.filePath.Split('\\');
                media.filename = filepath[filepath.Length - 1];
            }

            if (media.filename != null)
            {
                string filename = media.filename.Replace("_"," ");
                filename = filename.Substring(0, filename.IndexOf("."));

                txtTitleSearch.Text = filename;
            }
        }

        private void getDetailsButton_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            
            if (txtTitleSearch.Text.Trim().Length > 0)
            {
                IMDBScraper imdb = new IMDBScraper();

                string textToSearch = txtTitleSearch.Text.Replace("_", " ");

                //foundMedia = imdb.getInfoByTitle(textToSearch +" "+ textToSearch + " 2", false);
                foundMedia = imdb.getInfoByTitle(textToSearch, false);

                if (foundMedia != null)
                {
                    if (mediaToSearchFor != null && mediaToSearchFor.filePath != null)
                    {
                        //update foundMedia
                        foundMedia.filename = mediaToSearchFor.filename;
                        foundMedia.filePath = mediaToSearchFor.filePath;
                    }

                    txtTitleResult.Text = foundMedia.Title;
                    txtDirectorResult.Text = foundMedia.Director;
                    txtGenreResult.Text = foundMedia.category;
                    txtReleaseYearResult.Text = foundMedia.ReleaseYear;
                    txtTaglineResult.Text = foundMedia.Description;
                    txtImdbNumResult.Text = foundMedia.IMDBNum;
                    txtRatingResult.Text = foundMedia.Rating;
                    txtRatingReasonResult.Text = foundMedia.RatingDescription;

                    thumbnailPictureBox.ImageLocation = foundMedia.coverImage;
                }
                else
                    txtTitleResult.Text = "No Results";
            }
            else
            {
                MessageBox.Show("You must enter something to search for");
            }

            Cursor = Cursors.Default;
        }

        private void btnSave_Click(object sender, EventArgs e)
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
