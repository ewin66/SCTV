using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using SCTVObjects;
using System.IO;
using System.Text.RegularExpressions;

namespace SCTVJustinTV
{
    public partial class JustinTV : Form
    {
        string url = "";
        ArrayList channels = new ArrayList();
        ArrayList availableChannels = new ArrayList();

        public ArrayList Channels
        {
            get
            {
                return channels;
            }
        }

        public string URL
        {
            get
            {
                return url;
            }

            set
            {
                url = value;
            }
        }

        public JustinTV(string URL)
        {
            InitializeComponent();

            url = URL;

            if (url.Length > 0)
            {
                PlayChannel(createChannel(url));
            }
            else
            {
                populateChannels();

                pnlMessage.Visible = true;
                pnlMessage.BringToFront();
            }
        }

        public void PlayChannel(OnlineChannel channel)
        {
            wbJustinTV.Url = new Uri(channel.PopUpURL);
            pnlMessage.Visible = false; 
        }

        private void populateChannels()
        {
            channels.Add(createChannel("Seinfeld", "http://www.justin.tv/andrewbernard", "", "http://www.justin.tv/andrewbernard/popout"));
            channels.Add(createChannel("Tales from the Crypt", "http://www.justin.tv/talls_from", "", "http://www.justin.tv/talls_from/popout"));
            channels.Add(createChannel("Big Bang Theory", "http://www.justin.tv/littlebangtheory/old", "littlebangtheory", "http://www.justin.tv/littlebangtheory/popout"));
            channels.Add(createChannel("Simpsons", "http://www.justin.tv/movi3zon3hd", "Simpsons", "http://www.justin.tv/movi3zon3hd/popout"));
            //channels.Add(createChannel(""));
            //channels.Add(createChannel(""));
            //channels.Add(createChannel(""));
        }

        private OnlineChannel createChannel(string url)
        {
            return createChannel(url, url, url, "");
        }

        private OnlineChannel createChannel(string showTitle, string url, string channelTitle, string popoutURL)
        {
            OnlineChannel channel = new OnlineChannel();
            channel.ShowTitle = showTitle;
            channel.URL = url;
            channel.ChannelTitle = channelTitle;
            channel.PopUpURL = popoutURL;

            return channel;
        }

        private string getChannelInfo(string url)
        {
            //download the whole page, to be able to search it by regex
            //StreamReader sr = new StreamReader(new WebClient().OpenRead(url));
            //string fileContent = sr.ReadToEnd();

            //htmlCodes = new HTMLCodes();

            //return parseFileInfo(fileContent);

            return "";
        }

        private string parseFileInfo(string fileInfo)
        {
            string matchContent = "";

            //get exact matches
            string exactMatchPattern = @"Titles\ \(Exact\ Matches\).*?</p>";
            Regex R1 = new Regex(exactMatchPattern,
                RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

            //get popular titles
            string popularMatchPattern = @"Popular\ Titles.*?</p>";
            Regex R2 = new Regex(popularMatchPattern,
                RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

            //if (R2.Matches(_FileContent).Count > 0)
            //    matchContent = R2.Matches(_FileContent)[0].Value;

            //if (R1.Matches(_FileContent).Count > 0)
            //    matchContent += R1.Matches(_FileContent)[0].Value;

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

            //return fileInfo;

            return "";
        }
    }
}