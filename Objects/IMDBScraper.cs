using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Web;
using System.Collections;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Configuration;

namespace SCTVObjects
{
    public class IMDBScraper
    {
        private string imdbTitleSearchURL = "http://www.imdb.com/find?s=all&q=";
        private string imdbNumSearchURL = "http://www.imdb.com/title/";
        private string _FileContent;
        private HTMLCodes htmlCodes;

        #region Properties and their fields
        private string _MovieTitle;
        private string _Director;
        private string _Genre;
        private string _ReleaseYear;
        private string _TagLine;
        private string _rating;
        private string _ratingDescription;
        private string _imdbNum;
        private string _coverImage;
        private string _description;
        private System.Drawing.Image _thumbnail;
        private string titleMatch = "";
        private string exactTitleMatch = "";
        private string popularTitleMatch = "";
        private string partialTitleMatch = "";
        private string directorMatch = "";
        private string releaseYearMatch = "";
        private string genreMatch = "";
        private string taglineMatch = "";
        private string descriptionMatch = "";
        private string ratingMatch = "";
        private string ratingDescriptionMatch = "";
        private string titleSearchURL = "";
        private string numberSearchURL = "";
        private string _goofs = "";
        private string _trivia = "";
        private string _shortDescription = "";
        private string _stars = "";

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
        /// A breaf description about the movie
        /// </summary>
        public string Description
        {
            get { return _description; }
        }

        /// <summary>
        /// A thumbnail of the poster
        /// </summary>
        public System.Drawing.Image Thumbnail
        {
            get { return _thumbnail; }
        }

        public string Rating
        {
            get { return _rating; }
        }

        public string RatingDescription
        {
            get { return _ratingDescription; }
        }

        public string IMDBNum
        {
            get { return _imdbNum; }
        }

        public string CoverImage
        {
            get { return _coverImage; }
        }

        public string Goofs
        {
            get { return _goofs; }
        }

        public string Trivia
        {
            get { return _trivia; }
        }

        public string ShortDescription
        {
            get { return _shortDescription; }
        }

        public string Stars
        {
            get { return _stars; }
        }
        #endregion

        public IMDBScraper()
        {
            initVariables();
        }

        private void initVariables()
        {
            titleMatch = ConfigurationManager.AppSettings["IMDBInfo.TitleMatch"];
            exactTitleMatch = ConfigurationManager.AppSettings["IMDBInfo.ExactTitleMatch"];
            popularTitleMatch = ConfigurationManager.AppSettings["IMDBInfo.PopularTitleMatch"];
            partialTitleMatch = ConfigurationManager.AppSettings["IMDBInfo.PartialTitleMatch"];
            directorMatch = ConfigurationManager.AppSettings["IMDBInfo.DirectorMatch"];
            releaseYearMatch = ConfigurationManager.AppSettings["IMDBInfo.ReleaseYearMatch"];
            genreMatch = ConfigurationManager.AppSettings["IMDBInfo.GenreMatch"];
            taglineMatch = ConfigurationManager.AppSettings["IMDBInfo.TaglineMatch"];
            descriptionMatch = ConfigurationManager.AppSettings["IMDBInfo.DescriptionMatch"];
            ratingMatch = ConfigurationManager.AppSettings["IMDBInfo.RatingMatch"];
            ratingDescriptionMatch = ConfigurationManager.AppSettings["IMDBInfo.RatingDescriptionMatch"];
            titleSearchURL = ConfigurationManager.AppSettings["IMDBInfo.TitleSearchURL"];
            numberSearchURL = ConfigurationManager.AppSettings["IMDBInfo.NumberSearchURL"];

            if (titleSearchURL != null)
                imdbTitleSearchURL = titleSearchURL;

            if (numberSearchURL != null)
                imdbNumSearchURL = numberSearchURL;
        }

        /// <summary>
        /// Get IMDB info by the IMDB number
        /// </summary>
        /// <param name="imdbNum">imdb number</param>
        public Media getInfoByNumber(Media media)
        {
            //download the whole page, to be able to search it by regex
            string URL = imdbNumSearchURL + HttpUtility.UrlEncode(media.IMDBNum) + "/";
            StreamReader sr = new StreamReader(new WebClient().OpenRead(URL));
            _FileContent = sr.ReadToEnd();

            _imdbNum = media.IMDBNum;

            htmlCodes = new HTMLCodes();

            return getInfo(media, false);
        }

        /// <summary>
        /// Get IMDB info by the IMDB number
        /// </summary>
        /// <param name="imdbNum">imdb number</param>
        public Media getInfoByNumber(string imdbNumber)
        {
            //download the whole page, to be able to search it by regex
            string URL = imdbNumSearchURL + HttpUtility.UrlEncode(imdbNumber) + "/";
            StreamReader sr = new StreamReader(new WebClient().OpenRead(URL));
            _FileContent = sr.ReadToEnd();

            //_imdbNum = media.IMDBNum;

            htmlCodes = new HTMLCodes();

            Media media = new Media();
            media.IMDBNum = imdbNumber;

            return getInfo(media, true);
        }

        /// <summary>
        /// Get IMDB info by the media title
        /// </summary>
        /// <param name="title">Media title</param>
        /// <param name="bestMatch">Whether to automatically choose first popular result match or let the user choose</param>
        public Media getInfoByTitle(string title, bool bestMatch)
        {
            try
            {
                title = SCTVObjects.MediaHandler.FormatNameString(title);

                if (title.IndexOf("(") > 0)
                    title = title.Substring(0, title.IndexOf("("));

                //title = title.Replace("_", " ");

                //download the whole page, to be able to search it by regex
                string URL = imdbTitleSearchURL + HttpUtility.UrlEncode(title);
                StreamReader sr = new StreamReader(new WebClient().OpenRead(URL));
                _FileContent = sr.ReadToEnd();

                htmlCodes = new HTMLCodes();

                Media media = new Media();
                media.Title = title;

                return getInfo(media, bestMatch);
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);

                Media media = new Media();
                media.Title = title;

                return media;
            }                    
        }

        /// <summary>
        /// Downloads the poster thumbnail of the movie, and saves it in the Images folder
        /// </summary>
        /// <returns>The filename of the downloaded image</returns>
        /// <remarks></remarks>
        public string getPhoto(Media media)
        {
            string photoPath = "";
            string MovieName = media.Title.Split(' ')[0];
            string tempString = "";
            int startString = 0;
            int endString = 0;
            string imageURL = "";

            tempString = findValue(_FileContent, "<div class=\"poster\">", ".jpg", true);
            tempString = findValue(tempString, "src=", ".jpg", true);

            //startString = _FileContent.IndexOf("<img src=\"http://ia.media-imdb.com/images/");

            //tempString = _FileContent.Substring(startString);

            //endString = tempString.IndexOf(".jpg\"") + 4;

            //tempString = tempString.Substring(0, endString);

            imageURL = tempString.Replace("src=", "");
            imageURL = imageURL.Replace("\"", "");

            tempString = tempString.Substring(tempString.LastIndexOf("/") + 1);

            //if (tempString.ToLower().Contains("poster"))//this is the photo we are looking for
            //{
            //    tempString = tempString.Substring(tempString.IndexOf("http://ia.media-imdb.com/images"));
            //    tempString = tempString.Substring(0, tempString.IndexOf(".jpg"));

                photoPath = tempString;
            //}            


            //find the img tag containing the poster in the page
            //string RegExPattern = "<img [^\\>]* " + photoPath +
            //    ". [^\\>]* src \\s* = \\s* [\\\"\\']? ( [^\\\"\\'\\s>]* )";

            //Regex R1 = new Regex(RegExPattern,
            //    RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

            //MatchCollection matches = R1.Matches(_FileContent);

            ////find the link in the img tag and download the image and save it in the images folder
            //Regex R2 = new Regex("http.{0,}",
            //    RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

            //if (matches.Count > 0)
            //{
            //    Match posterImageUrl = R2.Match(matches[0].Value);
            _coverImage = System.Windows.Forms.Application.StartupPath + "\\images\\media\\coverImages\\" + photoPath;
            photoPath = System.Windows.Forms.Application.StartupPath + "\\images\\media\\coverImages\\" + photoPath;

            if (!File.Exists(photoPath))
            {
                System.Drawing.Image posterImage = System.Drawing.Image.FromStream(new WebClient().OpenRead(imageURL));
                //    string imageName = media.IMDBNum;

                //    if (imageName.Trim().Length == 0)
                //        imageName = media.filename;



                SaveJpeg(photoPath, posterImage, 100);

                _thumbnail = posterImage;
            }
                //}

            return photoPath;
        }

        /// <summary>
        /// Retrieve fields from IMDB, run this sub before using the properites
        /// </summary>
        /// <remarks></remarks>
        public Media getInfo(Media media, bool bestMatch)
        {
            Media mediaResult = media;

            try
            {
                string matchContent = "";
                ArrayList searchResults = new ArrayList();

                //scrape the movie title
                _MovieTitle = getTitle(_FileContent);

                if (_MovieTitle == "IMDb Search" || _MovieTitle == "Find - IMDb")//there were multiple results
                {
                    if (bestMatch)
                    {
                        ArrayList matches = getMultipleMatchTitles(_FileContent);

                        if (matches.Count > 0)
                        {
                            string searchTitle = MediaHandler.FormatNameString(media.Title).ToLower();
                            searchTitle = searchTitle.Replace(" ", "");

                            foreach (Media foundMedia in matches)
                            {
                                //try to match the title with search string
                                string tempTitle = MediaHandler.FormatNameString(foundMedia.Title).ToLower();
                                tempTitle = tempTitle.Replace(" ", "");

                                //look for the entire title
                                if (tempTitle.Contains(searchTitle))
                                {
                                    mediaResult = foundMedia;
                                    break;
                                }
                            }

                            //didn't find match - look for half the title
                            if (mediaResult == null)
                            {
                                foreach (Media foundMedia in matches)
                                {
                                    string tempTitle = MediaHandler.FormatNameString(foundMedia.Title).ToLower();
                                    tempTitle = tempTitle.Replace(" ", "");

                                    if (tempTitle.Contains(searchTitle.Substring(0, searchTitle.Length / 2)))
                                    {
                                        mediaResult = foundMedia;
                                        break;
                                    }
                                }
                            }

                            //pick the first one since we didn't find a match
                            if (mediaResult == null)
                                mediaResult = (Media)matches[0];

                            mediaResult = getInfoByNumber(mediaResult);
                        }
                    }
                    else
                    {
                        searchResults = getMultipleMatchTitles(_FileContent);

                        //display results
                        MultipleMatches multipleMatches = new MultipleMatches(searchResults);
                        DialogResult result = multipleMatches.ShowDialog();
                        multipleMatches.BringToFront();

                        if (result == DialogResult.OK)
                            mediaResult = multipleMatches.MediaResult;
                    }

                    ////get exact matches
                    //string exactMatchPattern = @"Titles\ \(Exact\ Matches\).*?</p>";
                    ////string exactMatchPattern = @"title_exact.*?</td>";
                    //Regex R1 = new Regex(exactMatchPattern,
                    //    RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

                    ////get popular titles
                    //string popularMatchPattern = @"Popular\ Titles.*?</p>";
                    //Regex R2 = new Regex(popularMatchPattern,
                    //    RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

                    ////if(R2.Matches(_FileContent).Count > 0)
                    ////    matchContent = R2.Matches(_FileContent)[0].Value;

                    //if (R2.Matches(_FileContent).Count > 0)
                    //    searchResults.AddRange(getMultipleMatchTitles(R2.Matches(_FileContent)[0].Value,0));

                    ////if (R1.Matches(_FileContent).Count > 0)
                    ////    matchContent += R1.Matches(_FileContent)[0].Value;

                    //for (int x = 0; x < R1.Matches(_FileContent).Count; x++)
                    //    searchResults.AddRange(getMultipleMatchTitles(R1.Matches(_FileContent)[x].Value, x));




                    //if (matchContent.Trim().Length > 0)
                    //{
                    //    //display matches
                    //    mediaResult = displayMultipleMatches(matchContent, bestMatch);//the first one is the popular match
                    //}
                    //else //if (mediaResult == null)// || mediaResult.filePath == null || mediaResult.filePath.Length == 0)
                    //{
                    //    //find the results and their movie number and let user choose which is correct
                    //    string paragraphPattern = "<p>.*</p>";
                    //    R1 = new Regex(paragraphPattern,
                    //        RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

                    //    if (R1.Matches(_FileContent).Count > 0)
                    //    {
                    //        //display matches
                    //        mediaResult = displayMultipleMatches(R1.Matches(_FileContent)[0].Value, bestMatch);//the first one is the popular match
                    //    }
                    //    else
                    //        _MovieTitle = "No Results";
                    //}
                }
                else
                {
                    mediaResult = new Media();
                    mediaResult.Title = _MovieTitle;

                    //scrape the director
                    _Director = getDirectors(_FileContent);
                    mediaResult.Director = HttpUtility.UrlDecode(HttpUtility.UrlEncode(_Director).Replace("%0a", "")).Trim();

                    //scrape release year
                    _ReleaseYear = getReleaseYear(_FileContent);
                    mediaResult.ReleaseYear = _ReleaseYear;

                    //scrape movie genre
                    _Genre = getGenre(_FileContent);
                    mediaResult.category = _Genre;

                    //scrape movie tagline
                    _TagLine = getTagline(_FileContent);
                    mediaResult.TagLine = HttpUtility.UrlDecode(HttpUtility.UrlEncode(_TagLine).Replace("%0a", "")).Trim();

                    //scrape movie description
                    _description = getDescription(_FileContent);
                    mediaResult.Description = System.Web.HttpUtility.UrlDecode(System.Web.HttpUtility.UrlEncode(_description).Replace("%0a", "")).Trim();

                    //scrape rating
                    _rating = getRating(_FileContent);
                    mediaResult.Rating = _rating;

                    //scrape rating description
                    _ratingDescription = getRatingDescription(_FileContent);
                    mediaResult.RatingDescription = _ratingDescription;

                    //get mediaType from title
                    mediaResult.MediaType = getMediaType(media.Title);

                    if (_imdbNum != null && _imdbNum.Trim().Length > 0)
                        mediaResult.IMDBNum = _imdbNum;
                    else
                    {
                        mediaResult.IMDBNum = getIMDBNum(_FileContent);
                        _imdbNum = mediaResult.IMDBNum;
                    }

                    mediaResult.Stars = getStars(_FileContent);

                    mediaResult.coverImage = getPhoto(mediaResult);

                    mediaResult.Goofs = HttpUtility.UrlDecode(HttpUtility.UrlEncode(getGoofs(_imdbNum)).Replace("%0a", "|")).Trim();

                    mediaResult.Trivia = HttpUtility.UrlDecode(HttpUtility.UrlEncode(getTrivia(_imdbNum)).Replace("%0a", "|")).Trim();

                    mediaResult.ShortDescription = getShortDescription(_FileContent);

                    _goofs = mediaResult.Goofs;
                    _trivia = mediaResult.Trivia;
                    _shortDescription = mediaResult.ShortDescription;
                    _stars = mediaResult.Stars;
                }
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }
            
            return mediaResult;
        }

        /// <summary>
        /// find and display multiple matches
        /// </summary>
        /// <param name="matchContent"></param>
        /// <returns></returns>
        private Media displayMultipleMatches(string matchContent, bool bestMatch)
        {
            //find the imdbNumber and titles and display
            ArrayList searchResults = new ArrayList();
            string foundImdbNum = "";
            Media foundMatch = null;
            Media selectedMedia = null;
            int matchIndex = 0;

            //string linkPattern = @"(<a.*?>.*?</a>)";
            string linkPattern = @"find-title-.*?/title_popular/images/b.gif.*?</td>";//popular title match
            Regex R1 = new Regex(linkPattern,
                RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

            if (R1.Matches(matchContent).Count == 0)
            {
                linkPattern = @"find-title-.*?</td></tr></table>";

                R1 = new Regex(linkPattern,
                    RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
            }

            if (R1.Matches(matchContent).Count == 0)
            {
                linkPattern = @"find-title-.*?<br>&#160;";

                R1 = new Regex(linkPattern,
                    RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
            }

            foreach (Match match in R1.Matches(matchContent))
            {
                Media media = new Media();

                media.IMDBNum = getIMDBNum(match.Groups[0].Value);

                foundImdbNum = media.IMDBNum;

                media.Title = getMultipleMatchTitle(matchContent, matchIndex);

                if (media.Title.Trim().Length > 0)
                {
                    searchResults.Add(media);

                    if (bestMatch)
                        break;
                }

                matchIndex++;
            }

            if (bestMatch)
            {
                if (searchResults.Count > 0)
                {
                    selectedMedia = (Media)searchResults[0];

                    //get info
                    if (foundImdbNum != null && foundImdbNum.Length > 0)
                        foundMatch = getInfoByNumber(selectedMedia);
                }
            }
            else
            {
                //display results
                MultipleMatches multipleMatches = new MultipleMatches(searchResults);
                DialogResult result = multipleMatches.ShowDialog();

                if (result == DialogResult.OK)
                    foundMatch = multipleMatches.MediaResult;
            }

            return foundMatch;
        }

        private string getMediaType(string stringToSearch)
        {
            string foundMediaType = "";

            if (stringToSearch.ToLower().Contains("<small>(tv series)</small>") || stringToSearch.ToLower().Contains("(tv)"))
                foundMediaType = "TV";
            else
                foundMediaType = "Movies";

            return foundMediaType;
        }

        private string getTitle(string content)
        {
            string title = "";

            try
            {
                string titlePattern = "<title>.*</title>";

                if (titleMatch != null)
                    titlePattern = titleMatch;

                Regex R1 = new Regex(titlePattern,
                    RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

                if (R1.Matches(content).Count > 0)
                {
                    title = R1.Matches(content)[0].Value;

                    //remove the beginning title tag <title>
                    title = title.Substring(7);

                    //remove the ending title tag </title>
                    title = title.Substring(0, title.Length - 8);
                }

                if(title.IndexOf("(") > 0)
                    title = title.Substring(0, title.IndexOf("("));
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }

            return htmlCodes.ToText(title);
        }

        private string getMultipleMatchTitle(string content, int matchIndex)
        {
            string title = "";

            try
            {
                //first look for an exact title match
                string titlePattern = "find-title-" + (matchIndex + 1) + "/title_exact/images/b.gif.*?</td></tr></table>";
                Regex R1 = new Regex(titlePattern,
                    RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

                if (R1.Matches(content).Count == 0)//now look for a popular match
                {
                    titlePattern = "title_popular/images/b.gif.*?<br>";
                    R1 = new Regex(titlePattern,
                        RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
                }

                if (R1.Matches(content).Count == 0)//try a second pattern for popular match
                {
                    titlePattern = "/find-title-" + (matchIndex + 1) + "/.*?</td></tr></table>";

                    R1 = new Regex(titlePattern,
                        RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
                }

                if (R1.Matches(content).Count > 0)
                {
                    title = R1.Matches(content)[0].Value;

                    //remove the beginning link tags
                    title = title.Substring(title.IndexOf(">") + 1);

                    //remove the ending tags
                    //title = title.Substring(0, title.Length - 4);
                    title = title.Substring(0, title.IndexOf("    "));

                    title = title.Replace("</a>", "");

                    title = htmlCodes.ToText(title).Trim();
                }
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }

            return title;
        }

        private ArrayList getMultipleMatchTitles(string content)
        {
            string title = "";
            ArrayList foundMatches = new ArrayList();

            try
            {
                string titlePattern = "";

                //first look for popular matches
                if (popularTitleMatch != null)
                    titlePattern = popularTitleMatch;
                else
                    titlePattern = "title_popular.*?<p";

                Regex R1 = new Regex(titlePattern,
                    RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

                foreach (Match match in R1.Matches(content))
                {
                    title = match.Value;

                    if (title.Contains("     "))
                        title = title.Substring(0, title.IndexOf("     "));

                    Media tempMedia = new Media();
                    tempMedia.Title = htmlCodes.ToText(match.Value).Trim();
                    tempMedia.Title = tempMedia.Title.Replace("<p", "");
                    tempMedia.IMDBNum = getIMDBNum(title);
                    
                    foundMatches.Add(tempMedia);
                }

                //now look for exact title matches
                if(exactTitleMatch !=null)
                    titlePattern = exactTitleMatch;
                else
                    titlePattern = "title_exact.*?<p";

                R1 = new Regex(titlePattern,
                    RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

                foreach (Match match in R1.Matches(content))
                {
                    title = match.Value;

                    if (title.Contains("      "))
                        title = title.Substring(0, title.IndexOf("      "));

                    Media tempMedia = new Media();
                    tempMedia.Title = htmlCodes.ToText(title).Trim();
                    tempMedia.IMDBNum = getIMDBNum(match.Value);
                    foundMatches.Add(tempMedia);
                }

                //now look for partial matches
                if (partialTitleMatch != null)
                    titlePattern = partialTitleMatch;
                else
                    titlePattern = "title_substring.*?<p";

                R1 = new Regex(titlePattern,
                    RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

                foreach (Match match in R1.Matches(content))
                {
                    title = match.Value;

                    if (title.Contains("      "))
                        title = title.Substring(0, title.IndexOf("      "));

                    Media tempMedia = new Media();
                    tempMedia.Title = htmlCodes.ToText(title).Trim();
                    tempMedia.IMDBNum = getIMDBNum(match.Value);
                    foundMatches.Add(tempMedia);
                }

                //now look for aprox matches
                //if (partialTitleMatch != null)
                //    titlePattern = partialTitleMatch;
                //else
                    titlePattern = "title_approx.*?<div";

                R1 = new Regex(titlePattern,
                    RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

                foreach (Match match in R1.Matches(content))
                {
                    title = match.Value;

                    if (title.Contains("      "))
                        title = title.Substring(0, title.IndexOf("      "));

                    Media tempMedia = new Media();
                    tempMedia.Title = htmlCodes.ToText(title).Trim();
                    tempMedia.IMDBNum = getIMDBNum(match.Value);
                    foundMatches.Add(tempMedia);
                }
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }

            return foundMatches;
        }

        private string getIMDBNum(string content)
        {
            string foundImdbNum = "";

            try
            {
                //string imdbPattern = @"link=/title/tt.*?/";
                string imdbPattern = @"/title/tt.*?/";

                Match m2 = Regex.Match(content, imdbPattern,
                    RegexOptions.Singleline);

                if (m2.Success)
                {
                    string theLink = m2.Groups[0].Value;

                    //need to parse and get movie number
                    foundImdbNum = theLink.Replace("/title/", "");
                    foundImdbNum = foundImdbNum.Replace("/", "");
                }
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }

            return htmlCodes.ToText(foundImdbNum);
        }

        private string getDirectors(string content)
        {
            string director = "";

            try
            {
                //string directorPattern = "Director[s]*:.* [^\\>]* \\s*> \\s* [^\\<]* ";

                //if (directorMatch != null)
                //    directorPattern = directorMatch;

                //Regex R1 = new Regex(directorPattern,
                //    RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

                //if (R1.Matches(content).Count > 0)
                //{
                //    director = R1.Matches(content)[0].Value;
                //    director = (director.Split('>')[2]).Trim();
                //}

                director = findValue(content, "Director:", "</a>");
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }

            return htmlCodes.ToText(director);
        }

        private string getStars(string content)
        {
            string stars = "";

            try
            {
                //string directorPattern = "Director[s]*:.* [^\\>]* \\s*> \\s* [^\\<]* ";

                //if (directorMatch != null)
                //    directorPattern = directorMatch;

                //Regex R1 = new Regex(directorPattern,
                //    RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

                //if (R1.Matches(content).Count > 0)
                //{
                //    director = R1.Matches(content)[0].Value;
                //    director = (director.Split('>')[2]).Trim();
                //}

                stars = findValue(content, "Stars:", "</div>");
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }

            return htmlCodes.ToText(stars);
        }

        private string getIMDBRating(string content)
        {
            string rating = "";

            try
            {
                rating = findValue(content, "<span class=\"rating-rating\">", "</span></span>");
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }

            return htmlCodes.ToText(rating);
        }

        private string getReleaseYear(string content)
        {
            string releaseYear = "";

            try
            {
                //string releaseYearPattern = "Date:.* [^\\>]* \\s*> \\s* [^\\<]* ";

                //if (releaseYearMatch != null)
                //    releaseYearPattern = releaseYearMatch;

                //Regex R1 = new Regex(releaseYearPattern,
                //    RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

                //if (R1.Matches(content).Count > 0)
                //{
                //    releaseYear = R1.Matches(content)[0].Value;
                //    R1 = new Regex("\\d{4,4}",
                //        RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

                //    releaseYear = R1.Matches(releaseYear)[0].Value.Trim();
                //}

                releaseYear = findValue(content, "<span>(<a href=\"/year/", "/\">");
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }

            return htmlCodes.ToText(releaseYear);
        }

        private string getGenre(string content)
        {
            string genre = "";
            string tempGenre = "";
            string[] genreArray;

            try
            {
                tempGenre = findValue(content, "<a    onclick=\"(new Image()).src='/rg/title-overview/genre/images", "&nbsp;&nbsp;-&nbsp;&nbsp;", true);

                if (tempGenre.Contains("|"))
                {
                    string startString = "";
                    genreArray = tempGenre.Split('|');

                    foreach (string value in genreArray)
                    {
                        if (genre.Length > 0)
                        {
                            genre += "|";
                            //startString = @"/Sections/Genres/";
                        }
                        //else
                            startString = "       >";

                        genre += findValue(value, startString, "</a>");
                    }
                }

                //string genrePattern = "Sections/Genres.*";
                //Regex R1 = new Regex(genrePattern,
                //    RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

                //if (R1.Matches(content).Count > 0)
                //{
                //    genre = R1.Matches(content)[0].Value;

                //    MatchCollection genreResults = R1.Matches(content);
                //    string[] genreArray = genreResults[0].ToString().Split('|');

                //    for (int C = 0; C <= genreArray.Length - 1; C++)
                //    {
                //        string seperater = "";

                //        if ((C % 2 != 0) & (genreArray[C].Contains("more") == false))
                //        {
                //            if (genre != "" & genre != null)
                //                seperater = " / ";
                //            genre += seperater + genreArray[C].Substring(0, genreArray[C].Length - 3);
                //        }
                //    }
                //}

                //genre = genre.Replace("Genre:</h5> / ", "");
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }

            return htmlCodes.ToText(genre);
        }

        private string getTagline(string content)
        {
            string tagline = "";

            try
            {
                string taglinePattern = "Tagline:.* [^\\>]* \\s*> \\s* [^\\<]* ";

                if (taglineMatch != null)
                    taglinePattern = taglineMatch;

                Regex R1 = new Regex(taglinePattern,
                    RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

                if (R1.Matches(content).Count > 0)
                {
                    string tagLine = R1.Matches(content)[0].Value;
                    tagLine = tagLine.Split(new char[] { '>', '<' })[2];
                    tagline = tagLine.Trim();
                }
                else
                {
                    tagline = "";
                }
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }

            return htmlCodes.ToText(tagline);
        }

        private string getShortDescription(string content)
        {
            string shortDescription = "";

            try
            {
                shortDescription = findValue(content, "<p>", "</p>");
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }

            return htmlCodes.ToText(shortDescription);
        }

        private string getDescription(string content)
        {
            string description = "";
            
            try
            {
                description = findValue(content, "Storyline", "<em class=\"nobr\">");
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }

            return htmlCodes.ToText(description);
        }

        private string getGoofs(string imdbNumber)
        {
            string goofs = "";

            try
            {
                //download the whole page, to be able to search it
                StreamReader sr = new StreamReader(new WebClient().OpenRead("http://www.imdb.com/title/"+ imdbNumber +"/goofs"));
                string goofContent = sr.ReadToEnd();


                goofs = findValue(goofContent, "<ul class=\"trivia\">", "<hr/>");

                goofs = htmlCodes.ToText(goofs);
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);

                Tools.WriteToFile(Tools.errorFile, "url to search: http://www.imdb.com/title/" + imdbNumber + "/goofs");
            }

            return goofs;
        }

        private string getTrivia(string imdbNumber)
        {
            string trivia = "";

            try
            {
                //download the whole page, to be able to search it
                StreamReader sr = new StreamReader(new WebClient().OpenRead("http://www.imdb.com/title/" + imdbNumber + "/trivia"));
                string triviaContent = sr.ReadToEnd();

                trivia = findValue(triviaContent, " class=\"soda\">", "<!-- sid: t-channel : MIDDLE_CENTER -->");

                trivia = htmlCodes.ToText(trivia);
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);

                Tools.WriteToFile(Tools.errorFile, "url to search: http://www.imdb.com/title/" + imdbNumber + "/trivia");
            }

            return trivia;
        }

        private string getRating(string content)
        {
            string rating = "";

            try
            {
                string ratingPattern = @"\nRated.*\n</div>";

                if (ratingMatch != null)
                    ratingPattern = ratingMatch;

                Regex R1 = new Regex(ratingPattern,
                    RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);

                if (R1.Matches(content).Count > 0)
                {
                    rating = R1.Matches(content)[0].Value;

                    //remove all lines before rating
                    rating = rating.Substring(rating.IndexOf("\nRated") + 6).Trim();

                    //remove all lines after rating
                    rating = rating.Substring(0, rating.IndexOf(" for")).Trim();
                }
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }

            return htmlCodes.ToText(rating);
        }

        private string getRatingDescription(string content)
        {
            string ratingDescription = "";

            try
            {
                string ratingDescriptionPattern = @"\nRated.*\n</div>";

                if (ratingDescriptionMatch != null)
                    ratingDescriptionPattern = ratingDescriptionMatch;

                Regex R1 = new Regex(ratingDescriptionPattern,
                    RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);

                if (R1.Matches(content).Count > 0)
                {
                    ratingDescription = R1.Matches(content)[0].Value;

                    //remove all lines before rating
                    ratingDescription = ratingDescription.Substring(ratingDescription.IndexOf("\nRated") + 6).Trim();

                    //find reason
                    ratingDescription = ratingDescription.Substring(ratingDescription.IndexOf(" for ") + 5);
                    ratingDescription = ratingDescription.Substring(0, ratingDescription.IndexOf("\n</div>")).Trim();
                }
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }

            return htmlCodes.ToText(ratingDescription);
        }

                          /// <summary> 
        /// Saves an image as a jpeg image, with the given quality 
        /// </summary> 
        /// <param name="path">Path to which the image would be saved.</param> 
        // <param name="quality">An integer from 0 to 100, with 100 being the 
        /// highest quality</param> 
        public static void SaveJpeg(string path, System.Drawing.Image img, int quality) 
        {
            try
            {
                if (!File.Exists(path))
                {
                    if (quality < 0 || quality > 100)
                        throw new ArgumentOutOfRangeException("quality must be between 0 and 100.");

                    //Image newImage = new Image();
                    //newImage = img;

                    // Encoder parameter for image quality 
                    EncoderParameter qualityParam =
                        new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
                    // Jpeg image codec 
                    ImageCodecInfo jpegCodec = GetEncoderInfo("image/jpeg");

                    EncoderParameters encoderParams = new EncoderParameters(1);
                    encoderParams.Param[0] = qualityParam;

                    img.Save(path, jpegCodec, encoderParams);
                    //img.Save(path);
                }
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }
        } 

        /// <summary> 
        /// Returns the image codec with the given mime type 
        /// </summary> 
        private static ImageCodecInfo GetEncoderInfo(string mimeType) 
        { 
            // Get image codecs for all image formats 
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders(); 

            // Find the correct image codec 
            for(int i=0; i<codecs.Length; i++) 
                if(codecs[i].MimeType == mimeType) 
                    return codecs[i]; 
            return null; 
        }

        private string findValue(string stringToParse, string startPattern, string endPattern)
        {
            return findValue(stringToParse, startPattern, endPattern, false);
        }

        private string findValue(string stringToParse, string startPattern, string endPattern, bool returnSearchPatterns)
        {
            int start = 0;
            int end = 0;
            string foundValue = "";

            start = stringToParse.IndexOf(startPattern);

            if (start > -1)
            {
                if(!returnSearchPatterns)
                    stringToParse = stringToParse.Substring(start + startPattern.Length);
                else
                    stringToParse = stringToParse.Substring(start);

                end = stringToParse.IndexOf(endPattern);

                if (end > 0)
                {
                    if (returnSearchPatterns)
                        foundValue = stringToParse.Substring(0, end + endPattern.Length);
                    else
                        foundValue = stringToParse.Substring(0, end);
                }
            }

            return foundValue;
        }
    }
}
