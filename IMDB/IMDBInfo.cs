using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SCTV
{
    public partial class IMDBInfo : Form
    {
        string movieTitle = "";

        public string MovieTitle
        {
            set
            {
                movieTitle = value;

                titleTextBox.Text = movieTitle;
            }

            get
            {
                return movieTitle;
            }
        }

        public IMDBInfo()
        {
            InitializeComponent();
        }

        private void getDetailsButton_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;

            if (imdbNoTextBox.Text.Trim().Length > 0)
            {
                IMDBScraper imdb = new IMDBScraper(imdbNoTextBox.Text);
            }
            else if (titleTextBox.Text.Trim().Length > 0)
            {
                IMDBScraper imdb = new IMDBScraper(titleTextBox.Text.Replace("_"," "));

                imdb.getInfo();

                titleTextBox.Text = imdb.MovieTitle;
                directorTextBox.Text = imdb.Director;
                genreTextBox.Text = imdb.Genre;
                releaseYearTextBox.Text = imdb.ReleaseYear;
                tagLineTextBox.Text = imdb.TagLine;

                imdb.getPhoto();
                thumbnailPictureBox.Image = imdb.Thumbnail;
            }
            else
            {
                MessageBox.Show("You must enter something to search on");
            }

            Cursor = Cursors.Default;
        }
    }
}
