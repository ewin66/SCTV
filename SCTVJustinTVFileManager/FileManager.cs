using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Collections;
using System.Net;
using System.IO;
using SCTechUtilities;
using SCTVObjects;
using System.Diagnostics;

namespace SCTVJustinTVFileManager
{
    public partial class FileManager : Form
    {
        //TODO:
        //create callback for downloadRecord
        //use vlc to play local flv files


        ArrayList archiveRecordings = new ArrayList();
        ArrayList localRecordings = new ArrayList();
        ArrayList allRecordings = new ArrayList();
        private VLC vlc;
        AsynTask _playerTask;
        ArrayList listViewGroups = new ArrayList();
        ListViewGroup lvig = null;

        enum recordType 
        {
            Archive,
            Local
        };


        public FileManager()
        {
            try
            {
                InitializeComponent();

                getLocalFiles();

                getArchiveFiles();

                fillListView();
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }
        }

        private void fillListView()
        {
            try
            {
                lvArchives.Items.Clear();

                //iterate archive files
                foreach (JustinRecord record in archiveRecordings)
                    lvArchives.Items.Add(createListViewItem(record));

                lvAllFiles.Items.Clear();

                //iterate all files
                foreach (JustinRecord record in allRecordings)
                    lvAllFiles.Items.Add(createListViewItem(record));

                lvLocalFiles.Items.Clear();

                //iterate local files
                foreach (JustinRecord record in localRecordings)
                    lvLocalFiles.Items.Add(createListViewItem(record));
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }
        }

        private void getLocalFiles()
        {
            if (Directory.Exists(Application.StartupPath + "\\videoFiles"))
                foreach (string fileName in Directory.GetFiles(Application.StartupPath + "\\videoFiles"))
                    createRecord(fileName, recordType.Local);
        }

        private void getArchiveFiles()
        {
            string[] channels = { "clickey", "justin1010" };

            //iterate channels to get all videos
            foreach (string channel in channels)
            {
                //get archive xml

                //http://api.justin.tv/api/channel/archives/clickey.xml?limit=50
                WebClient client = new WebClient();
                String xmlString = client.DownloadString("http://api.justin.tv/api/channel/archives/clickey.xml?limit=50");
                XmlDocument records = new XmlDocument();
                records.LoadXml(xmlString);

                //iterate record nodes in xml doc
                foreach (XmlNode record in records.SelectNodes("/records/record"))
                    createRecord(record, recordType.Archive);
            }
        }

        private void downloadRecord(JustinRecord justinRecord)
        {
            Directory.CreateDirectory(Application.StartupPath + "\\videoFiles");

            if(!File.Exists(Application.StartupPath + "\\videoFiles\\"+ justinRecord.file_name))
            {
                WebClient client = new WebClient();
                client.DownloadDataCompleted += new DownloadDataCompletedEventHandler(client_DownloadDataCompleted);
                client.DownloadFileAsync(new Uri(justinRecord.video_file_url), Application.StartupPath + "\\videoFiles\\"+ justinRecord.start_time.Replace(":","-") +"_"+ justinRecord.id +"_"+ justinRecord.file_name.Substring(justinRecord.file_name.LastIndexOf("/")));

                downloadImage(justinRecord);

                localRecordings.Add(justinRecord);
            }
        }

        void client_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            //refresh listview to display the newly downloaded file
            fillListView();
        }

        private ListViewItem createListViewItem(JustinRecord record)
        {
            //add item
            ListViewItem newListViewItem = new ListViewItem();

            try
            {
                string startTimeGroup = "";

                if (record.start_time.Contains("-"))
                    record.start_time = record.start_time.Replace("-",":");
                    
                startTimeGroup = record.start_time.Substring(0, record.start_time.IndexOf(":"));
                
                startTimeGroup = startTimeGroup.Substring(0, startTimeGroup.LastIndexOf(" "));

                string[] startTimeInfo = record.start_time.Replace("UTC", "GMT").Split(' ');
                string tempStartTime = startTimeInfo[0] + " " + startTimeInfo[1] + " " + startTimeInfo[2] + " " + startTimeInfo[5] + " " + startTimeInfo[3] + " " + startTimeInfo[4];
                DateTime dtStartTime = DateTime.SpecifyKind(DateTime.Parse(tempStartTime), DateTimeKind.Utc).ToLocalTime();

                string title = dtStartTime.ToShortTimeString();

                if(record.length != null)
                    title += " - " + dtStartTime.AddMinutes(double.Parse(record.length)).ToShortTimeString();

                //start_time = Fri Nov 19 17:07:59 UTC 2010

                //make title 25 characters
                int titleMaxLength = 25;
                titleMaxLength = lvArchives.Columns[0].Width - lvArchives.SmallImageList.ImageSize.Width - 60;

                if (titleMaxLength < 25)
                    titleMaxLength = 25;

                if (title.Trim().Length > titleMaxLength)
                {
                    title = title.Substring(0, titleMaxLength - 3) + "...";
                    //title = title.Substring(0, titleMaxLength) + Environment.NewLine + title.Substring(titleMaxLength).PadRight(titleMaxLength, Convert.ToChar(" "));
                }
                else
                    title = title.Trim().PadRight(titleMaxLength, Convert.ToChar(" "));

                newListViewItem.Text = title;
                newListViewItem.Tag = record;

                if (!listViewGroups.Contains(startTimeGroup))
                {
                    lvig = new ListViewGroup(startTimeGroup);
                    lvig.Name = startTimeGroup;

                    listViewGroups.Add(startTimeGroup);

                    lvArchives.Groups.Add(lvig);
                }

                string imageKey = "";

                if (record.image_url_medium.ToLower().Contains("http:"))
                    imageKey = downloadImage(record);
                else
                    imageKey = record.image_url_medium;

                if (!ilImages.Images.ContainsKey(imageKey))
                {
                    try
                    {
                        //if (File.Exists(imageKey))
                        //{
                        Bitmap newImage = new Bitmap(imageKey);
                        ilImages.Images.Add(imageKey, newImage);//add new image
                        //}
                        //else
                        //  MessageBox.Show("Image not found " + Environment.NewLine + imageKey);
                    }
                    catch (Exception ex)
                    {
                        throw;
                        //the image file was invalid
                        //imageKey = "defaultImage";
                    }
                }

                newListViewItem.ImageKey = imageKey;
                newListViewItem.Group = lvig;
                newListViewItem.ToolTipText = record.file_name;
                newListViewItem.ToolTipText += "\n" + record.start_time;
                newListViewItem.ToolTipText += "\n" + record.video_file_url;
            }
            catch (Exception ex)
            {
                
            }
            

            return newListViewItem;
        }

        private string downloadImage(JustinRecord record)
        {
            Bitmap bitmap; 

            try
            {
                if (!File.Exists(Application.StartupPath + "\\videoFileImages\\" + record.image_url_medium.Substring(record.image_url_medium.LastIndexOf("/") + 1)))
                {
                    WebClient client = new WebClient();
                    Stream stream = client.OpenRead(record.image_url_medium);
                    bitmap = new Bitmap(stream);
                    stream.Flush();
                    stream.Close();

                    //make sure the directory exists
                    Directory.CreateDirectory(Application.StartupPath + "\\videoFileImages\\");

                    //save the image
                    bitmap.Save(Application.StartupPath + "\\videoFileImages\\" + record.image_url_medium.Substring(record.image_url_medium.LastIndexOf("/") + 1));
                }
            }
            catch (Exception e)
            {
                throw;
            }

            return Application.StartupPath + "\\videoFileImages\\" + record.image_url_medium.Substring(record.image_url_medium.LastIndexOf("/") + 1);
        }

        /// <summary>
        /// create a justinRecord from an XmlNode
        /// </summary>
        /// <param name="record"></param>
        /// <param name="RecordType"></param>
        private void createRecord(XmlNode record, recordType RecordType)
        {
            JustinRecord justinRecord = new JustinRecord();
            justinRecord = (JustinRecord)CreateObjects.FromXmlNodeWithChildNodes(record, justinRecord);

            switch (RecordType)
            {
                case recordType.Archive:
                    archiveRecordings.Add(justinRecord);
                    break;
                case recordType.Local:
                    localRecordings.Add(justinRecord);
                    break;
            }
            
            if (!allRecordings.Contains(justinRecord))
                allRecordings.Add(justinRecord);
        }

        /// <summary>
        /// create a justinRecord from filename
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="RecordType"></param>
        private void createRecord(string fileName, recordType RecordType)
        {
            if (fileName.ToLower().Contains(".flv"))
            {
                JustinRecord justinRecord = new JustinRecord();
                justinRecord.file_name = fileName;

                string[] recordInfo = fileName.Substring(fileName.LastIndexOf("\\") + 1).Split('_');

                //parse name to get time info
                justinRecord.start_time = recordInfo[0]; //fileName.Substring(fileName.LastIndexOf("\\")+1, fileName.IndexOf("_") - (fileName.LastIndexOf("\\")+1));

                justinRecord.image_url_medium = Application.StartupPath + "\\videofileImages\\archive-" + recordInfo[1] + "-320x240.jpg";

                justinRecord.id = recordInfo[1];

                justinRecord.stream_name = recordInfo[2].Substring(recordInfo[2].LastIndexOf("-") + 1) + "_" + recordInfo[3] + "_" + recordInfo[4];

                switch (RecordType)
                {
                    case recordType.Archive:
                        archiveRecordings.Add(justinRecord);
                        break;
                    case recordType.Local:
                        localRecordings.Add(justinRecord);
                        break;
                }

                if (!allRecordings.Contains(justinRecord))
                    allRecordings.Add(justinRecord);
            }
            else
                Tools.WriteToFile(Tools.errorFile, "Invalid file type: " + fileName);
        }

        private void lvArchives_DoubleClick(object sender, EventArgs e)
        {
            JustinRecord justinRecord = null;

            //get the selected record
            foreach (ListViewItem selectedItem in lvArchives.SelectedItems)
            {
                //only getting the first one for now
                justinRecord = (JustinRecord)selectedItem.Tag;
                break;
            }

            //play the selected record
            if (justinRecord != null)
            {
                wbViewer.DocumentText = "<object type=\"application/x-shockwave-flash\" height=\"300\" width=\"400\" id=\"clip_embed_player_flash\" data=\"http://www.justin.tv/widgets/archive_embed_player.swf\" bgcolor=\"#000000\"><param name=\"movie\" value=\"http://www.justin.tv/widgets/archive_embed_player.swf\" /><param name=\"allowScriptAccess\" value=\"always\" /><param name=\"allowNetworking\" value=\"all\" /><param name=\"allowFullScreen\" value=\"true\" /><param name=\"flashvars\" value=\"auto_play=true&start_volume=25&title=Broadcasting LIVE on Justin.tv&channel="+ justinRecord.stream_name +"&archive_id="+ justinRecord.id +"\" /></object><br /><a href=\"http://www.justin.tv/clickey#r=q0dXFRA~&s=em\" class=\"trk\" style=\"padding:2px 0px 4px; display:block; width: 320px; font-weight:normal; font-size:10px; text-decoration:underline; text-align:center;\">Watch live video from clickey on Justin.tv</a>";
                
                tcFiles.SelectedIndex = 3;
            }
        }

        private void lvLocalFiles_DoubleClick(object sender, EventArgs e)
        {
            ArrayList mediaToPlay = new ArrayList();
            JustinRecord justinRecord = null;

            //get the selected records
            foreach (ListViewItem selectedItem in lvLocalFiles.SelectedItems)
            {
                justinRecord = (JustinRecord)selectedItem.Tag;

                mediaToPlay.Add(justinRecord.file_name);
            }

            //play the selected records
            if (justinRecord != null)
                startVLC(justinRecord.file_name);
        }

        private void lvAllFiles_DoubleClick(object sender, EventArgs e)
        {
            JustinRecord justinRecord = null;

            //get the selected records
            foreach (ListViewItem selectedItem in lvAllFiles.SelectedItems)
            {
                justinRecord = (JustinRecord)selectedItem.Tag;
            }

            //play the selected records
            if (justinRecord != null)
            {
                if (justinRecord.video_file_url != null && justinRecord.video_file_url.ToLower().Contains("http:"))
                {
                    wbViewer.DocumentText = "<object type=\"application/x-shockwave-flash\" height=\"300\" width=\"400\" id=\"clip_embed_player_flash\" data=\"http://www.justin.tv/widgets/archive_embed_player.swf\" bgcolor=\"#000000\"><param name=\"movie\" value=\"http://www.justin.tv/widgets/archive_embed_player.swf\" /><param name=\"allowScriptAccess\" value=\"always\" /><param name=\"allowNetworking\" value=\"all\" /><param name=\"allowFullScreen\" value=\"true\" /><param name=\"flashvars\" value=\"auto_play=true&start_volume=25&title=Broadcasting LIVE on Justin.tv&channel=" + justinRecord.stream_name + "&archive_id=" + justinRecord.id + "\" /></object><br /><a href=\"http://www.justin.tv/clickey#r=q0dXFRA~&s=em\" class=\"trk\" style=\"padding:2px 0px 4px; display:block; width: 320px; font-weight:normal; font-size:10px; text-decoration:underline; text-align:center;\">Watch live video from clickey on Justin.tv</a>";

                    tcFiles.SelectedIndex = 3;
                }
                else
                    startVLC(justinRecord.file_name);
            }
        }

        private void tcFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(tcFiles.SelectedIndex != 3)
                wbViewer.Url = null;
        }

        #region VLC functions

        /// <summary>
        /// Start vlc exe
        /// </summary>
        private void startVLC(string mediaToPlay)
        {
            try
            {
                //ProcessStartInfo startInfo = new ProcessStartInfo(Application.StartupPath + "\\vlc\\vlc.exe");
                ProcessStartInfo startInfo = new ProcessStartInfo(Application.StartupPath +"\\vlc\\vlc.exe");
                //startInfo.WindowStyle = ProcessWindowStyle.Hidden;

                //Process.Start(startInfo);

                //startInfo.Arguments = "dvd://E:\\ --fullscreen --quiet";

                startInfo.Arguments = "\""+ mediaToPlay +"\"";

                //startInfo.Arguments += " --fullscreen";
                startInfo.Arguments += " --quiet";

                startInfo.Arguments += " --drop-late-frames";
                startInfo.Arguments += " --disable-screensaver";
                startInfo.Arguments += " --overlay";
                //startInfo.Arguments += " dummy";
                //startInfo.Arguments += " --fast-mutex";
                //startInfo.Arguments += " --win9x-cv-method=1";
                startInfo.Arguments += " --rtsp-caching=1200";
                ////startInfo.Arguments += "--plugin-path=" + vlcInstallDirectory + @"\plugins";
                //startInfo.Arguments += " --snapshot-path=" + Application.StartupPath + "\\vlc\\snapshots";
                //startInfo.Arguments += " --snapshot-prefix=snap_";
                //startInfo.Arguments += " --key-snapshot=S";
                //startInfo.Arguments += " --snapshot-format=png";

                Process myProcess = new Process();
                myProcess.StartInfo = startInfo;

                //myProcess.ErrorDataReceived += new DataReceivedEventHandler(myProcess_ErrorDataReceived);
                //myProcess.OutputDataReceived += new DataReceivedEventHandler(myProcess_OutputDataReceived);

                myProcess.Start();

                myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;

                myProcess.Refresh();
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }
        }

        #endregion

        #region Context menu functions

        private void downloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            JustinRecord justinRecord = null;

            switch (tcFiles.SelectedTab.Name)
            {
                case "tpLocalFiles":
                    foreach (ListViewItem selectedItem in lvLocalFiles.SelectedItems)
                    {
                        //only getting the first one for now
                        justinRecord = (JustinRecord)selectedItem.Tag;
                        break;
                    }

                    break;

                case "tpOnlineFiles":
                    foreach (ListViewItem selectedItem in lvArchives.SelectedItems)
                    {
                        //only getting the first one for now
                        justinRecord = (JustinRecord)selectedItem.Tag;
                        break;
                    }
                    break;
            }

            if(justinRecord != null)
                downloadRecord(justinRecord);
        }

        private void playToolStripMenuItem_Click(object sender, EventArgs e)
        {
            JustinRecord justinRecord = null;

            //get the selected record
            foreach (ListViewItem selectedItem in lvArchives.SelectedItems)
            {
                //only getting the first one for now
                justinRecord = (JustinRecord)selectedItem.Tag;
                break;
            }

            //play the selected record
            if (justinRecord != null)
            {
                wbViewer.DocumentText = "<object type=\"application/x-shockwave-flash\" height=\"300\" width=\"400\" id=\"clip_embed_player_flash\" data=\"http://www.justin.tv/widgets/archive_embed_player.swf\" bgcolor=\"#000000\"><param name=\"movie\" value=\"http://www.justin.tv/widgets/archive_embed_player.swf\" /><param name=\"allowScriptAccess\" value=\"always\" /><param name=\"allowNetworking\" value=\"all\" /><param name=\"allowFullScreen\" value=\"true\" /><param name=\"flashvars\" value=\"auto_play=false&start_volume=25&title=Broadcasting LIVE on Justin.tv&channel=" + justinRecord.stream_name + "&archive_id=" + justinRecord.id + "\" /></object><br /><a href=\"http://www.justin.tv/clickey#r=q0dXFRA~&s=em\" class=\"trk\" style=\"padding:2px 0px 4px; display:block; width: 320px; font-weight:normal; font-size:10px; text-decoration:underline; text-align:center;\">Watch live video from clickey on Justin.tv</a>";

                tcFiles.SelectedIndex = 3;
            }
        }
        #endregion
    }
}