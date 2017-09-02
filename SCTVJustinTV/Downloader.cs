using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Xml;
using System.IO;
using System.Collections;

namespace SCTVJustinTV
{
    public partial class Downloader : Form
    {
        string channel = "";
        string numFilesToDownload = "10";
        string startingDirectory = "c:\\";
        string saveToPath = Application.StartupPath +"\\JustinDownloads\\";
        string archivePath = Application.StartupPath + "\\JustinArchives\\";
        int downloadCount = 0;
        int downloadMax = 3;
        ArrayList downloadQueue = new ArrayList();
        ArrayList futureDownloadQueue = new ArrayList();
        JustinArchive currentDownload;
        WebClient downloadClient;
        DateTime startTime;
        DateTime endTime;

        public string Channel
        {
            set 
            { 
                txtChannel.Text = channel;
            }

            get { return channel; }
        }

        public string NumFilesToDownload
        {
            set {numFilesToDownload = value;}
            get { return numFilesToDownload;}
        }

        public string SaveToPath
        {
            set { saveToPath = value; }
        }

        public DateTime StartTime
        {
            set { startTime = value; }
        }

        public DateTime EndTime
        {
            set { endTime = value; }
        }

        public Downloader()
        {
            InitializeComponent();

            //create directory if it doesn't exist
            if (!Directory.Exists(archivePath))
                Directory.CreateDirectory(archivePath);

            dateTimePickerEndTime.Value = DateTime.Now.AddMinutes(60);

            archiveTimer.Start();
        }

        public Downloader(string channelName, DateTime startTime, string saveTo, string startingFile)
        {
            channel = channelName;
            saveToPath = saveTo;

            downloadClient = new WebClient();
            downloadClient.DownloadFileCompleted +=new AsyncCompletedEventHandler(downloadClient_DownloadFileCompleted);

            //downloadChannel(channel, numFilesToDownload, saveToPath, startingFile);
        }

        /// <summary>
        /// Download content from the given channel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDownload_Click(object sender, EventArgs e)
        {
            //get dates from UI
            startTime = new DateTime(dateTimePickerStartDate.Value.Year, dateTimePickerStartDate.Value.Month, dateTimePickerStartDate.Value.Day, dateTimePickerStartTime.Value.Hour, dateTimePickerStartTime.Value.Minute, dateTimePickerStartTime.Value.Second);
            endTime = new DateTime(dateTimePickerEndDate.Value.Year, dateTimePickerEndDate.Value.Month, dateTimePickerEndDate.Value.Day, dateTimePickerEndTime.Value.Hour, dateTimePickerEndTime.Value.Minute, dateTimePickerEndTime.Value.Second);

            if (startTime > endTime)
            {
                MessageBox.Show("Start time must be before end time");
                return;
            }

            channel = txtChannel.Text;
            
            downloadClient = new WebClient();
            downloadClient.DownloadFileCompleted += new AsyncCompletedEventHandler(downloadClient_DownloadFileCompleted);

            downloadChannel(channel, txtShowTitle.Text, saveToPath, startTime, endTime);
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                string folderName = "";
                
                folderBrowserDialog1.SelectedPath = startingDirectory;
                //openFileDialog1.InitialDirectory = startingDirectory;

                DialogResult result = folderBrowserDialog1.ShowDialog();

                if (result == DialogResult.OK)
                {
                    folderName = this.folderBrowserDialog1.SelectedPath;

                    //txtSaveLocation.Text = folderName;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void downloadChannel(string channelName, string videoTitle, string saveTo, DateTime startTime, DateTime endTime)
        {
            JustinArchive justinMedia = new JustinArchive();
            FileInfo fi = new FileInfo(saveTo);
            bool newFutureDownload = true;

            //make sure saveTo path exists
            if (!fi.Directory.Exists)
                fi.Directory.Create();

            //archive this justinArchive if it's end time goes into the future
            if(endTime > DateTime.Now)//archive
            {
                JustinArchive futureToRemove = null;
                justinMedia = new JustinArchive();
                justinMedia.EndTime = endTime.ToLongDateString() +" "+ endTime.ToLongTimeString();
                justinMedia.StartTime = startTime.ToLongDateString() +" "+ startTime.ToLongTimeString();
                justinMedia.ChannelName = channelName;
                justinMedia.Name = videoTitle;

                foreach (JustinArchive future in futureDownloadQueue)
                {
                    if (future.EndTime == justinMedia.EndTime && future.StartTime == justinMedia.StartTime)
                    {
                        newFutureDownload = false;
                        break;
                    }
                }

                if (newFutureDownload)
                    futureDownloadQueue.Add(justinMedia);                    
            }

            //get list of available video for channel
            XmlDocument xmlArchives = downloadArchiveFile(channelName);

            //read xmlArchives into a dataset
            //set the video_file_url as primary key and sort
            //try
            //{
            //    DataSet dsArchives = new DataSet();
            //    dsArchives.ReadXml(new XmlNodeReader(xmlArchives));

            //    //DataColumn[] pk = new DataColumn[1];
            //    //pk[0] = dsArchives.Tables[0].Columns["video_file_url"];
            //    //dsArchives.Tables[0].PrimaryKey = pk;

            //    dsArchives.Tables[0].DefaultView.Sort = "video_file_url asc";

            //    dsArchives.Tables[0].DefaultView.Table.WriteXml(Application.StartupPath + "\\dsArchives.xml");

            //    //XmlDocument archives = new XmlDocument();
            //    //archives.LoadXml(dsArchives.GetXml());
            //    //archives.Save(Application.StartupPath + "\\xmlArchives.xml");

            //    //xmlArchives = new XmlDocument();
            //    //xmlArchives.LoadXml(dsArchives.GetXml());
            //}
            //catch (Exception ex)
            //{
            //    xmlArchives = downloadArchiveFile(channelName);
            //}

            //MessageBox.Show("xml files written");
            //return;

            //queue each file starting with the newest
            //foreach (XmlNode archive in xmlArchives["archives"].ChildNodes)

            //queue each file starting with the oldest - so the files can start to be watched as soon as the oldest is downloaded
            for(int x = xmlArchives["records"].ChildNodes.Count-1; x >= 0 ; x--)
            //foreach (XmlNode archive in xmlArchives["archives"].ChildNodes)
            {
                //string videoURL = archive.SelectSingleNode("video_file_url").InnerText;
                string videoURL = xmlArchives["records"].ChildNodes[x].SelectSingleNode("video_file_url").InnerText;
                string fileName = videoTitle +"_|_"+ videoURL.Substring(videoURL.LastIndexOf("/") + 1);
                //string archiveStartTime = archive.SelectSingleNode("start_time").InnerText;
                string archiveStartTime = xmlArchives["records"].ChildNodes[x].SelectSingleNode("start_time").InnerText;
                DateTime archiveStartDate = new DateTime();

                DateTime.TryParse(archiveStartTime, out archiveStartDate);

                if (archiveStartDate.AddMinutes(5) > startTime && archiveStartDate < endTime)//then we are between the start time and end time so download
                {
                    justinMedia = new JustinArchive();
                    justinMedia.URL = videoURL;
                    justinMedia.SaveToPath = saveTo + fileName;

                    if (!File.Exists(justinMedia.SaveToPath))//dont download file if it already exists
                        downloadQueue.Add(justinMedia);
                }
            }

            //if the times are already past remove the item from futurDownloadQueue
            if (DateTime.Now > startTime && DateTime.Now > endTime)
            {

            }

            //start downloading the first one
            if (downloadQueue.Count > 0)
            {
                lbDownloadQueue.Items.Add(channelName + " - " + startTime + "-" + endTime);

                currentDownload = justinMedia;
                downloadFile(((JustinArchive)downloadQueue[0]).URL, ((JustinArchive)downloadQueue[0]).SaveToPath);
            }
            else
                MessageBox.Show("Nothing to download");
        }

        private void downloadChannel(string channelName, string numFiles, string saveTo, string startingFile)
        {
            //use start_time and length to determine what videos we might want

            JustinArchive justinMedia = new JustinArchive();
            FileInfo fi = new FileInfo(saveTo);

            //make sure saveTo path exists
            if (!fi.Directory.Exists)
                fi.Directory.Create();

            //get list of available video for channel
            string apiURL = "http://api.justin.tv/api/channel/archives/" + channelName + ".xml?limit=" + numFiles;
            XmlDocument xmlArchives = new XmlDocument();
            xmlArchives.LoadXml(new WebClient().DownloadString(apiURL));

            //download each file starting with the newest
            foreach (XmlNode archive in xmlArchives["archives"].ChildNodes)
            {
                string videoURL = archive.SelectSingleNode("video_file_url").InnerText;
                string fileName = videoURL.Substring(videoURL.LastIndexOf("/")+1);

                justinMedia = new JustinArchive();
                justinMedia.URL = videoURL;
                justinMedia.SaveToPath = saveTo + fileName;

                if (!File.Exists(justinMedia.SaveToPath))
                {
                    if(!downloadQueue.Contains(justinMedia))
                        downloadQueue.Add(justinMedia);
                    
                    if (justinMedia.URL.ToLower().Contains(startingFile.ToLower()))//we have reached the start of what we are recording
                        break;
                }
            }

            //start downloading the first one
            if (downloadQueue.Count > 0)
            {
                currentDownload = justinMedia;
                downloadFile(((JustinArchive)downloadQueue[0]).URL, ((JustinArchive)downloadQueue[0]).SaveToPath);
            }
        }

        void downloadClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            //downloadQueue.Remove(currentDownload);
            downloadQueue.RemoveAt(0);
            currentDownload = null;

            //start downloading next archive
            if (downloadQueue.Count > 0)
            {
                currentDownload = (JustinArchive)downloadQueue[0];
                downloadFile(((JustinArchive)downloadQueue[0]).URL, ((JustinArchive)downloadQueue[0]).SaveToPath);
            }
            else
            {
                currentDownload = null;

                MessageBox.Show("Downloading complete");
            }
        }

        private void downloadFile(string url, string saveTo)
        {
            downloadClient.DownloadFileAsync(new Uri(url), saveTo);
        }

        /// <summary>
        /// download archive files for favorite channels
        /// </summary>
        /// <returns></returns>
        private XmlDocument downloadArchiveFile(string channelName)
        {
            XmlDocument xmlArchives = null;

            string apiURL = "http://api.justin.tv/api/channel/archives/" + channelName + ".xml?limit=100";
            xmlArchives = new XmlDocument();
            xmlArchives.LoadXml(new WebClient().DownloadString(apiURL));

            xmlArchives.Save(archivePath + channelName + ".xml");

            return xmlArchives;
        }

        /// <summary>
        /// download archive files for favorite channels
        /// </summary>
        /// <returns></returns>
        private string getChannelDescription(string channelName)
        {
            string description = "";

            string apiURL = "http://api.justin.tv/api/channel/show/" + channelName + ".xml";
            XmlDocument xmlChannelInfo = new XmlDocument();
            xmlChannelInfo.LoadXml(new WebClient().DownloadString(apiURL));

            description = xmlChannelInfo.SelectSingleNode("/channel/description").InnerText;

            return description;
        }

        private void archiveTimer_Tick(object sender, EventArgs e)
        {
            ArrayList downloadsToRemove = new ArrayList();

            //download archives of favorite channels - determine favorite by what channels have been downloaded from already
            //downloadArchiveFile(archive);

            if (currentDownload == null)
            {
                //check for archives in the futureDownloadQueue
                foreach (JustinArchive archive in futureDownloadQueue)
                {
                    DateTime archiveStartTime = new DateTime();
                    DateTime archiveEndTime = new DateTime();

                    if (DateTime.TryParse(archive.StartTime, out archiveStartTime))
                    {
                        DateTime.TryParse(archive.EndTime, out archiveEndTime);

                        downloadChannel(archive.ChannelName, archive.Name, saveToPath, archiveStartTime, archiveEndTime);

                        downloadsToRemove.Add(archive);
                    }
                }

                foreach (JustinArchive archive in downloadsToRemove)
                    futureDownloadQueue.Remove(archive);
            }
        }

        private void txtChannel_TextChanged(object sender, EventArgs e)
        {
            channel = txtChannel.Text;

            if (channel.IndexOf("#") > 0)
                channel = channel.Substring(0, channel.IndexOf("#"));

            txtShowTitle.Text = getChannelDescription(channel);
        }
    }
}