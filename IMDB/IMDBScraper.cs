using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Web;

namespace SCTV
{
    public class IMDBScraper
    {
        private string imdbTitleSearchURL = "http://www.imdb.com/find?s=all&q=";
        private string imdbNumSearchURL = "http://www.imdb.com/title/";
        private string _IMDBNo;
        private string _FileContent;
        private HTMLCodes htmlCodes;

        #region Properties and their fields
        private string _MovieTitle;
        private string _Director;
        private string _Genre;
        private string _ReleaseYear;
        private string _TagLine;
        private Image _thumbnail;

        /// <summary>
        /// The title of the movie
        /// </summary>
        public string MovieTitle
        {
            get { return _MovieTitle; }
        }

        /// <summary>
        /// The name of the director
        /// </summary>
        public string Director
        {
            get { return _Director; }
        }

        /// <summary>
        /// An array of all the genres that apply to the movie
        /// </summary>
        public string Genre
        {
            get { return _Genre; }
        }

        /// <summary>
        /// The date the movie was released
        /// </summary>
        public string ReleaseYear
        {
            get { return _ReleaseYear; }
        }

        /// <summary>
        /// A breaf intro about the movie
        /// </summary>
        public string TagLine
        {
            get { return _TagLine; }
        }

        /// <summary>
        /// A thumbnail of the poster
        /// </summary>
        public Image Thumbnail
        {
            get { return _thumbnail; }
        }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="IMDBNo">The IMDB number of the movie</param>
        /// <param name="MovieName">The movie title</param>
        /// <remarks></remarks>
        //public IMDBScraper(string IMDBNo)
        //{
        //    _IMDBNo = IMDBNo;

        //    //download the whole page, to be able to search it by regex
        //    string URL = imdbNumSearchURL + _IMDBNo + "/";
        //    StreamReader sr = new StreamReader(new WebClient().OpenRead(URL));
        //    _FileContent = sr.ReadToEnd();

        //    htmlCodes = new HTMLCodes();
        //}

        public IMDBScraper(string movieTitle)
        {
            //download the whole page, to be able to search it by regex
            string URL = imdbTitleSearchURL + HttpUtility.HtmlEncode(movieTitle) + "/";
            StreamReader sr = new StreamReader(new WebClient().OpenRead(URL));
            _FileContent = sr.ReadToEnd();

            htmlCodes = new HTMLCodes();
        }

        /// <summary>
        /// Downloads the poster thumbnail of the movie, and saves it in the Images folder
        /// </summary>
        /// <returns>The filename of the downloaded image</returns>
        /// <remarks></remarks>
        public void getPhoto()
        {
            string MovieName = _MovieTitle.Split(' ')[0];

            //find the img tag containing the poster in the page
            string RegExPattern = "<img [^\\>]* " + MovieName +
                ". [^\\>]* src \\s* = \\s* [\\\"\\']? ( [^\\\"\\'\\s>]* )";

            Regex R1 = new Regex(RegExPattern,
                RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

            MatchCollection matches = R1.Matches(_FileContent);

            //find the link in the img tag and download the image and save it in the images folder
            Regex R2 = new Regex("http.{0,}",
                RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

            if (matches.Count > 0)
            {
                Match posterImageUrl = R2.Match(matches[0].Value);
                Image posterImage = Image.FromStream(new WebClient().OpenRead(posterImageUrl.Value));

                _thumbnail = posterImage;
            }
        }

        /// <summary>
        /// Retrieve fields from IMDB, run this sub before using the properites
        /// </summary>
        /// <remarks></remarks>
        public void getInfo()
        {
            //scrape the movie title
            string titlePattern = "<title>.*</title>";
            Regex R1 = new Regex(titlePattern,
                RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

            if (R1.Matches(_FileContent).Count > 0)
            {
                _MovieTitle = R1.Matches(_FileContent)[0].Value;

                //remove the beginning title tag <title>
                _MovieTitle = _MovieTitle.Substring(7);

                //remove the ending title tag </title>
                _MovieTitle = _MovieTitle.Substring(0, _MovieTitle.Length - 8);

                _MovieTitle = htmlCodes.ToText(_MovieTitle);
            }

            //scrape the director
            string directorPattern = "Director[s]*:.* [^\\>]* \\s*> \\s* [^\\<]* ";
            R1 = new Regex(directorPattern,
                RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

            if (R1.Matches(_FileContent).Count > 0)
            {
                _Director = R1.Matches(_FileContent)[0].Value;
                _Director = (_Director.Split('>')[2]).Trim();
            }

            //scrape release year
            string releaseYearPattern = "Date:.* [^\\>]* \\s*> \\s* [^\\<]* ";
            R1 = new Regex(releaseYearPattern,
                RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

            string releaseYear;

            if (R1.Matches(_FileContent).Count > 0)
            {
                releaseYear = R1.Matches(_FileContent)[0].Value;
                R1 = new Regex("\\d{4,4}",
                    RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

                _ReleaseYear = R1.Matches(releaseYear)[0].Value.Trim();
            }

            //scrape movie genre
            string genrePattern = "Genre.*";
            R1 = new Regex(genrePattern,
                RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

            string genre;

            if (R1.Matches(_FileContent).Count > 0)
            {
                genre = R1.Matches(_FileContent)[0].Value;
                MatchCollection genreResults = R1.Matches(_FileContent);
                string[] genreArray = genreResults[1].ToString().Split('>');

                for (int C = 0; C <= genreArray.Length - 1; C++)
                {
                    string seperater = "";

                    if ((C % 2 != 0) & (genreArray[C].Contains("more") == false))
                    {
                        if (_Genre != "" & _Genre != null)
                            seperater = " / ";
                        _Genre += seperater + genreArray[C].Substring(0, genreArray[C].Length - 3);
                    }
                }
            }

            //scrape movie tagline
            string taglinePattern = "Tagline:.* [^\\>]* \\s*> \\s* [^\\<]* ";
            R1 = new Regex(taglinePattern,
                RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

            if (R1.Matches(_FileContent).Count > 0)
            {
                string tagLine = R1.Matches(_FileContent)[0].Value;
                tagLine = tagLine.Split(new char[] { '>', '<' })[2];
                _TagLine = tagLine.Trim();
            }
            else
            {
                _TagLine = "";
            }
        }
    }
}
