<<<<<<< .mine
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using SCTVObjects;
using System.IO;
using System.Diagnostics;
using System.Globalization;
using SCTVCamera;
using SCTV;
//using SCTVGames;
using System.Net;
using System.Threading;
using System.Runtime.Remoting.Messaging;
using System.Drawing.Text;
using System.Configuration;
using System.Xml;
using System.Windows.Forms.VisualStyles;

namespace SCTV
{
    // delegates used to call MainForm functions from worker thread
    public delegate void DelegateAddString(String s);
    public delegate void DelegateThreadFinished();

    public partial class MediaLibrary_Listview : Form
    {
        ArrayList fileSystemWatchers = new ArrayList();
        ArrayList categories = new ArrayList();
        DataView dvMedia = new DataView();
        MediaHandler myMedia;
        liquidMediaPlayer mediaWindow;
        object selectedControl;
        ArrayList playlist = new ArrayList();
        PlayList playlistForm;
        SpeechRecognition speechListener;
        public string _grammarvocabFile = "xmlmediaLibrary.xml";
        WebBrowser browser;
        SCTV.MainForm safeSurf;
        private string defaultPathToSaveTo = "";
        private Speakers speakers;
        string featuredOnlineContentURL = "http://hulu.com/tv";
        Color alphabetTabColor = Color.LightSkyBlue;
        Color mediaTypeTabColor = Color.LightGreen;
        Color categoryTabColor = Color.MediumOrchid;
        Color listViewBGColor = Color.MediumOrchid;
        private bool playSequels = false;
        private bool continousPlay = false;
        Media currentMedia;
        private bool embedBrowser = false;
        private int loadOnDemandChunk = 5;
        DataTable dtRemainingResults;
        int toolTipTimerTick = 0;
        int toolTipDisplayTick = 0;
        int toolTipDelay = 2; //in seconds
        int toolTipDisplay = 0;//in seconds - zero is stay on until mouse is off item
        Media currentToolTipMedia = null;
        int titleMaxLength = 28;
        MediaToolTip toolTip;
        int textHeight = 0;
        ArrayList selectedListviewItems = new ArrayList();
        SelectablePanel pnlMediaLists = null;
        string previouslySelectedMediaType = "";
        string previouslySelectedAlphabet = "";
        string previouslySelectedCategory = "";

        // worker thread
        Thread m_WorkerThread;

        // events used to stop worker thread
        ManualResetEvent m_EventStopThread;
        ManualResetEvent m_EventThreadStopped;

        // Delegate instances used to call user interface functions 
        // from worker thread:
        //public DelegateAddString m_DelegateAddString;
        public DelegateThreadFinished m_DelegateThreadFinished;

        // create a delegate of MethodInvoker poiting to
        // our Foo function.
        MethodInvoker simpleDelegate;

        /// <summary>
        /// The name of the vocab file to load for this form
        /// </summary>
        protected string GrammarFile
        {
            get { return _grammarvocabFile; }
            set
            {
                _grammarvocabFile = value;
            }
        }

        public SpeechRecognition SpeechListener
        {
            set
            {
                speechListener = value;

                if (speechListener != null)
                {
                    speechListener.loadGrammarFile(_grammarvocabFile);
                    speechListener.executeCommand += new SpeechRecognition.HeardCommand(speechListener_executeCommand);
                }
            }
        }

        void speechListener_executeCommand(Phrase thePhrase)
        {
            executePhrase(thePhrase);
        }

        //mouse hooks
        //private static LowLevelMouseProc _proc = HookCallback;
        //private static IntPtr _hookID = IntPtr.Zero;
        //private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);
        //private const int WH_MOUSE_LL = 14;

        //mouse hooks
        //private enum MouseMessages
        //{
        //    WM_LBUTTONDOWN = 0x0201,
        //    WM_LBUTTONUP = 0x0202,
        //    WM_MOUSEMOVE = 0x0200,
        //    WM_MOUSEWHEEL = 0x020A,
        //    WM_RBUTTONDOWN = 0x0204,
        //    WM_RBUTTONUP = 0x0205
        //}

        public MediaLibrary_Listview(MediaHandler media)
        {
            try
            {
                InitializeComponent();

                speakers = new Speakers();

                //watch for new files
                foreach (string location in media.Locations)
                {
                    try
                    {
                        string newLocation = location;

                        if (newLocation.Contains(","))
                            newLocation = newLocation.Split(',')[0];                          

                        initFileSystemWatcher(newLocation);
                    }
                    catch { }                                                
                }

                //get embed browser value from config
                bool.TryParse(System.Configuration.ConfigurationManager.AppSettings["MediaLibrary.EmbedBrowser"], out embedBrowser);

                //get loadOnDemandChunk value from config
                if (!int.TryParse(System.Configuration.ConfigurationManager.AppSettings["MediaLibrary.loadOnDemandChunk"], out loadOnDemandChunk))
                    loadOnDemandChunk = 20;

                try
                {
                    mediaTypeTabColor = Color.FromName(System.Configuration.ConfigurationManager.AppSettings["MediaLibrary.MediaTypeTabColor"]);
                }
                catch
                { }

                try
                {
                    categoryTabColor = Color.FromName(System.Configuration.ConfigurationManager.AppSettings["MediaLibrary.CategoryTabColor"]);
                }
                catch
                { }

                try
                {
                    listViewBGColor = Color.FromName(System.Configuration.ConfigurationManager.AppSettings["MediaLibrary.ListviewBGColor"]);
                }
                catch
                { }

                //set volume bar
                //pbVolume.Value = speakers.GetVolume();

                defaultPathToSaveTo = System.Configuration.ConfigurationManager.AppSettings["Media.DefaultPathToSaveTo"];

                if (defaultPathToSaveTo.Trim().Length == 0)
                {
                    //make sure directory exists
                    if (!Directory.Exists(Application.StartupPath + "\\Media\\"))
                        Directory.CreateDirectory(Application.StartupPath + "\\Media\\");

                    defaultPathToSaveTo = Application.StartupPath + "\\Media\\";
                }

                myMedia = media;

                //speechListener = new SpeechRecognition("xmlmediaLibrary.xml");
                //speechListener.executeCommand += new SpeechRecognition.HeardCommand(speechListener_executeCommand);
                //speechListener.Show();

                //mouse hooks
                //_hookID = SetHook(_proc);
                
                //lvFF.BackColor = listViewBGColor;

                initializeAdvancedFilters();

                this.TopMost = false;

                //scrollingTabsMediaTypes.MultiSelect = false;
                scrollingTabsMediaTypes.CanDeselect = false;
                scrollingTabsMediaTypes.SelectionChanged += new SCTV.Controls.ScrollingTabs.selectionChanged(scrollingTabsMediaTypes_SelectionChanged);
                scrollingTabsAlphabet.SelectionChanged += new SCTV.Controls.ScrollingTabs.selectionChanged(scrollingTabsAlphabet_SelectionChanged);

                ArrayList alphabet = new ArrayList();

                alphabet.Add("All");

                for (char c = 'A'; c <= 'Z'; c++)
                    alphabet.Add(c.ToString());

                scrollingTabsAlphabet.Tabs = alphabet;
                scrollingTabsAlphabet.SelectedTabs = "All";

                toolStripStatusLabel.Text = "SCTV created by Chad Lickey - Version:" + Application.ProductVersion;
                
                //SCTVObjects.Fonts fonts = new Fonts();

                //scrollingTabsGenre.Font = fonts.CustomFont("ChocolateDropsNF");
                //scrollingTabsAlphabet.Font = fonts.CustomFont("GhostWriter");
                //scrollingTabsMediaTypes.Font = fonts.CustomFont("Grange");

                displayMedia();
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, "MediaLibrary_Listview error: " + ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        void ThreadProc()
        {
            try
            {
                this.Invoke((MethodInvoker)delegate ()
                            {
                                SetupWizard setupWizard = new SetupWizard();
                                setupWizard.StartPosition = FormStartPosition.CenterScreen;
                                setupWizard.SourcePaths = myMedia.Locations;
                                setupWizard.ShowDialog(this);
                            });
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, "ThreadProc: " + ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        void scrollingTabsAlphabet_SelectionChanged()
        {
            if (previouslySelectedAlphabet != scrollingTabsAlphabet.SelectedTabs)
            {
                txtSearch.Text = "";
                btnCancelSearch.Visible = false;

                displayCategory(scrollingTabsGenre.SelectedTabs, scrollingTabsAlphabet.SelectedTabs, scrollingTabsMediaTypes.SelectedTabs);

                previouslySelectedAlphabet = scrollingTabsAlphabet.SelectedTabs;
            }
        }

        void scrollingTabsMediaTypes_SelectionChanged()
        {
            if (previouslySelectedMediaType != scrollingTabsMediaTypes.SelectedTabs)
            {
                lvFF.Controls.Clear();
                txtSearch.Text = "";
                btnCancelSearch.Visible = false;

                string selectedGenres = scrollingTabsGenre.SelectedTabs;

                //if (scrollingTabsMediaTypes.SelectedTabs == "Online")
                //    scrollingTabsGenre.ShowCheckbox = false;
                //else
                //    scrollingTabsGenre.ShowCheckbox = true;

                displayCategories(scrollingTabsMediaTypes.SelectedTabs);

                if (selectedGenres.Length == 0)
                {
                    scrollingTabsGenre.SelectedTabIndexes = "0";
                }

                displayCategory(scrollingTabsGenre.SelectedTabs, scrollingTabsAlphabet.SelectedTabs, scrollingTabsMediaTypes.SelectedTabs);

                previouslySelectedMediaType = scrollingTabsMediaTypes.SelectedTabs;
            }
        }

        private void initFileSystemWatcher(string mediaPath)
        {
            try
            {
                //FileInfo fi = new FileInfo(mediaPath);
                DirectoryInfo di = new DirectoryInfo(mediaPath);
                if (di.Exists)
                {
                    FileSystemWatcher fileWatcher = new FileSystemWatcher();

                    fileWatcher.Path = mediaPath;
                    fileWatcher.NotifyFilter = NotifyFilters.LastWrite;
                    fileWatcher.IncludeSubdirectories = true;
                    fileWatcher.Created += new FileSystemEventHandler(fileWatcher_Created);
                    fileWatcher.Changed += new FileSystemEventHandler(fileWatcher_Changed);
                    fileWatcher.EnableRaisingEvents = true;

                    fileSystemWatchers.Add(fileWatcher);
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("sctv", ex.ToString());
            }
        }

        void fileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            ////refresh media
            //RefreshToolStripButton.Enabled = false;

            //simpleDelegate = new MethodInvoker(lookForMedia);

            //// Calling lookForMedia Async
            //simpleDelegate.BeginInvoke(new AsyncCallback(ThreadFinished), null);

            myMedia.AddMediaFile(new FileInfo(e.FullPath), true);

            myMedia.GetMedia();

            //displayMedia();
            displayCategory(scrollingTabsGenre.SelectedTabs, scrollingTabsAlphabet.SelectedTabs, scrollingTabsMediaTypes.SelectedTabs);
        }

        void fileWatcher_Created(object sender, FileSystemEventArgs e)
        {
            myMedia.AddMediaFile(new FileInfo(e.FullPath), true);
        }

        //mouse hooks
        //private static IntPtr SetHook(LowLevelMouseProc proc)
        //{
        //    using (Process curProcess = Process.GetCurrentProcess())
        //    using (ProcessModule curModule = curProcess.MainModule)
        //    {
        //        return SetWindowsHookEx(WH_MOUSE_LL, proc,
        //            GetModuleHandle(curModule.ModuleName), 0);
        //    }
        //}

        //mouse hooks
        //private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        //{
        //    if (nCode >= 0 &&
        //        MouseMessages.WM_LBUTTONDOWN == (MouseMessages)wParam)
        //    {
        //        MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
        //        Console.WriteLine(hookStruct.pt.x + ", " + hookStruct.pt.y);
        //    }
        //    return CallNextHookEx(_hookID, nCode, wParam, lParam);
        //}

        /// <summary>
        /// get media and display
        /// </summary>
        private void displayMedia()
        {
            try
            {
                displayMediaTypes();

                displayCategories(scrollingTabsMediaTypes.SelectedTabs);

                if (scrollingTabsGenre.SelectedTabs.Length == 0)
                    scrollingTabsGenre.SelectedTabIndexes = "0";

                if (scrollingTabsMediaTypes.SelectedTabs.Length == 0)
                    scrollingTabsMediaTypes.SelectedTabIndexes = "0";

                displayCategory(scrollingTabsGenre.SelectedTabs, scrollingTabsAlphabet.SelectedTabs, scrollingTabsMediaTypes.SelectedTabs);

                SCTVObjects.SplashScreenNew.CloseForm();
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, "displayMedia error: " + ex.Message);
            }
        }

        /// <summary>
        /// queries dsMedia to find all the categories for the given mediaType
        /// </summary>
        /// <param name="mediaType"></param>
        private void displayCategories(string mediaType)
        {
            try
            {
                string correctCase = "";
                string selectedTabs = scrollingTabsGenre.SelectedTabs;

                //display categories
                categories = new ArrayList();

                foreach (DataRowView drv in myMedia.GetAllCategories(mediaType))
                {
                    string[] catArray;
                    string categoryString = drv["category"].ToString();

                    if (categoryString.Contains("/") && mediaType.ToLower() != "games")
                        catArray = categoryString.Split('/');
                    else
                        catArray = categoryString.Split('|');

                    for (int x = 0; x < catArray.Length; x++)
                    {
                        correctCase = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(catArray[x].Trim());
                        if (!categories.Contains(correctCase) && correctCase.Length > 0)//keep out duplicates
                            categories.Add(correctCase);
                    }
                }

                //add internet before sort so it will be sorted
                if (mediaType.ToLower() == "online")
                    categories.Add("Internet");

                categories.Sort();//alphabetize

                switch (mediaType.ToLower())
                {
                    case "games":

                        break;
                    case "music":

                        break;
                    case "online":

                        break;
                    case "movies":
                        categories.Insert(0, "Home");
                        categories.Insert(1, "New");
                        categories.Insert(2, "Recent");
                        categories.Insert(3, "Star Rating");
                        categories.Insert(4, "Misc");
                        categories.Insert(5, "Popular");
                        categories.Insert(6, "Not-So Popular");
                        categories.Insert(7, "All");
                        break;
                    default:

                        break;
                }

                //add utility pages to categories
                if (mediaType.ToLower() != "online")
                {
                    scrollingTabsGenre.MultiSelect = true;
                }
                //else if (mediaType.ToLower() == "online")
                //{
                //    categories.Insert(0, "Home");

                //    scrollingTabsGenre.MultiSelect = false;
                //}

                //categories.Insert(1, "Playlist");

                if (mediaType.ToLower() == "online")
                    scrollingTabsGenre.ShowCheckbox = false;
                else
                    scrollingTabsGenre.ShowCheckbox = true;

                scrollingTabsGenre.Tabs = categories;

                if (!scrollingTabsGenre.Tabs.Contains(selectedTabs))
                    selectedTabs = "";

                if (selectedTabs.Trim().Length == 0)
                {
                    if (mediaType.ToLower() == "online")
                        scrollingTabsGenre.SelectedTabs = "TV";
                    else
                        scrollingTabsGenre.SelectedTabIndexes = "0";
                }
                else
                    scrollingTabsGenre.SelectedTabs = selectedTabs;
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, "displayCategories" + ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        private void displayMediaTypes()
        {
            //display mediaTypes
            try
            {
                string correctCase = "";
                string selectedTabs = scrollingTabsMediaTypes.SelectedTabs;
                ArrayList mediaTypes = new ArrayList();

                //foreach (DataRowView drv in myMedia.GetAllMediaTypes())
                foreach (string type in myMedia.GetAllMediaTypes())
                {
                    string[] typeArray = type.ToString().Split('/');

                    for (int x = 0; x < typeArray.Length; x++)
                    {
                        correctCase = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(typeArray[x].Trim());
                        if (!mediaTypes.Contains(correctCase) && correctCase.Trim().Length > 0)//keep out duplicates
                            mediaTypes.Add(correctCase);
                    }
                    
                    //correctCase = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(type.Trim());
                    //if (!mediaTypes.Contains(correctCase))//keep out duplicates
                    //    mediaTypes.Add(correctCase);
                }

                mediaTypes.Add("Online");
                mediaTypes.Add("Games");
                mediaTypes.Sort();

                scrollingTabsMediaTypes.Tabs = mediaTypes;

                if (selectedTabs.Trim().Length == 0)
                {
                    scrollingTabsMediaTypes.SelectedTabIndexes = "0";

                    //if(scrollingTabsMediaTypes.SelectedTabs == "Online")
                    //    sc
                }
                else
                    scrollingTabsMediaTypes.SelectedTabs = selectedTabs;
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, "displayMediaTypes"+ ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        void btnMediaType_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                LinearGradientBrush bgBrush = null;
                Button selectedButton = (Button)sender;
                int textWidth = (int)(e.Graphics.MeasureString(selectedButton.Text, selectedButton.Font).Width);

                selectedButton.Width = textWidth + 8;

                //set button tag
                if (selectedButton.Tag == null || !(bool)selectedButton.Tag)
                    bgBrush = new LinearGradientBrush(e.ClipRectangle, Color.OldLace, Color.LightBlue, LinearGradientMode.Horizontal);
                else
                    bgBrush = new LinearGradientBrush(e.ClipRectangle, Color.LightGreen, Color.OldLace, LinearGradientMode.Vertical);

                if (bgBrush != null)
                    using (bgBrush)
                        e.Graphics.FillRectangle(bgBrush, e.ClipRectangle.X - 10, e.ClipRectangle.Y, e.ClipRectangle.Width + 20, e.ClipRectangle.Height);

                e.Graphics.DrawString(selectedButton.Text, selectedButton.Font, Brushes.Black, 2, 4);
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }
        }

        void btnMediaType_Click(object sender, EventArgs e)
        {
            try
            {
                Button selectedButton = (Button)sender;

                //set button tag
                if (selectedButton.Tag == null || !(bool)selectedButton.Tag)
                    selectedButton.Tag = true;
                else
                    selectedButton.Tag = false;

                selectedButton.Invalidate();

                //display selected categories
                applyAdvancedFilters(scrollingTabsGenre.SelectedTabs, scrollingTabsMediaTypes.SelectedTabs, scrollingTabsAlphabet.SelectedTabs);
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }
        }

        /// <summary>
        /// find all media in selected category and starts with selected letter and display them
        /// </summary>
        /// <param name="category"></param>
        public void displayCategory(string category, string startsWith, string mediaType)
        {
            //if (mediaType.ToLower() == "online")
            //    dvMedia = new DataView();
            //else

            dvMedia = myMedia.GetCategoryMedia(category, startsWith, mediaType, btnFilterOutResults.Text == "+");

            if (category.ToLower() == "home")
            {
                string selectedMediaType = scrollingTabsMediaTypes.SelectedTabs.ToLower();
                int horizontalScrollHeight = 190;
                int horizontalScrollGap = horizontalScrollHeight+10;
                Point locationOnForm = lvFF.FindForm().PointToClient(lvFF.Parent.PointToScreen(lvFF.Location));
                locationOnForm = new Point(locationOnForm.X, locationOnForm.Y - 115);

                lvFF.Controls.Clear();
                lvFF.Items.Clear();

                pnlMediaLists = new SelectablePanel();
                //pnlMediaLists.BackColor = Color.Red;
                lvFF.Controls.Add(pnlMediaLists);
                pnlMediaLists.TabIndex = 0;
                pnlMediaLists.Anchor = (AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom);
                pnlMediaLists.Width = lvFF.Width;
                pnlMediaLists.Height = lvFF.Height;
                pnlMediaLists.Location = locationOnForm;
                pnlMediaLists.Select();

                //if(selectedMediaType != "movies" && selectedMediaType != "games")
                //    selectedMediaType = "movies";

                HorizontalScrollingListview actionList = new HorizontalScrollingListview();
                actionList.DisplayCategory("new", "", selectedMediaType);
                pnlMediaLists.Controls.Add(actionList);
                actionList.Location = locationOnForm;
                actionList.Anchor = (AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top);
                actionList.Width = lvFF.Width;
                actionList.Height = horizontalScrollHeight;

                HorizontalScrollingListview adventureList = new HorizontalScrollingListview();
                adventureList.DisplayCategory("recent", "", selectedMediaType);
                pnlMediaLists.Controls.Add(adventureList);
                adventureList.Location = new Point(locationOnForm.X, locationOnForm.Y + horizontalScrollGap);
                adventureList.Anchor = (AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top);
                adventureList.Width = lvFF.Width;
                adventureList.Height = horizontalScrollHeight;

                HorizontalScrollingListview adventure2List = new HorizontalScrollingListview();
                adventure2List.DisplayCategory("popular", "", selectedMediaType);
                pnlMediaLists.Controls.Add(adventure2List);
                adventure2List.Location = new Point(locationOnForm.X, locationOnForm.Y + horizontalScrollGap*2);
                adventure2List.Anchor = (AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top);
                adventure2List.Width = lvFF.Width;
                adventure2List.Height = horizontalScrollHeight;

                HorizontalScrollingListview adventure3List = new HorizontalScrollingListview();
                adventure3List.DisplayCategory("action", "", selectedMediaType);
                pnlMediaLists.Controls.Add(adventure3List);
                adventure3List.Location = new Point(locationOnForm.X, locationOnForm.Y + horizontalScrollGap*3);
                adventure3List.Anchor = (AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top);
                adventure3List.Width = lvFF.Width;
                adventure3List.Height = horizontalScrollHeight;

                HorizontalScrollingListview adventure4List = new HorizontalScrollingListview();
                adventure4List.DisplayCategory("adventure", "", selectedMediaType);
                pnlMediaLists.Controls.Add(adventure4List);
                adventure4List.Location = new Point(locationOnForm.X, locationOnForm.Y + horizontalScrollGap*4);
                adventure4List.Anchor = (AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top);
                adventure4List.Width = lvFF.Width;
                adventure4List.Height = horizontalScrollHeight;

                HorizontalScrollingListview adventure5List = new HorizontalScrollingListview();
                adventure5List.DisplayCategory("comedy", "", selectedMediaType);
                pnlMediaLists.Controls.Add(adventure5List);
                adventure5List.Location = new Point(locationOnForm.X, locationOnForm.Y + horizontalScrollGap*5);
                adventure5List.Anchor = (AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top);
                adventure5List.Width = lvFF.Width;
                adventure5List.Height = horizontalScrollHeight;

                HorizontalScrollingListview adventure6List = new HorizontalScrollingListview();
                adventure6List.DisplayCategory("drama", "", selectedMediaType);
                pnlMediaLists.Controls.Add(adventure6List);
                adventure6List.Location = new Point(locationOnForm.X, locationOnForm.Y + horizontalScrollGap*6);
                adventure6List.Anchor = (AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top);
                adventure6List.Width = lvFF.Width;
                adventure6List.Height = horizontalScrollHeight;

                pnlMediaLists.AutoScroll = true;
            }
            else
            {
                displayCategory(dvMedia, category, mediaType);
            }
        }
        
        /// <summary>
        /// find all tv episodes in selected show and display them
        /// </summary>
        /// <param name="category"></param>
        public void displayTVEpisodes(string seriesIMDBNum)
        {
            dvMedia = myMedia.GetTVShowMedia(seriesIMDBNum);

            displayCategory(dvMedia, "TV", "TV", true);
        }

        /// <summary>
        /// Get albums for selected artist
        /// </summary>
        /// <param name="artistID"></param>
        public void displayAlbums(string artistID)
        {
            dvMedia = myMedia.GetAlbumsMedia(artistID);

            displayCategory(dvMedia, "Music", "Music", true);
        }

        /// <summary>
        /// Get songs for selected album
        /// </summary>
        /// <param name="releaseID"></param>
        public void displaySongs(string releaseID)
        {
            dvMedia = myMedia.GetSongsMedia(releaseID);

            displayCategory(dvMedia, "Music", "Music", true);
        }

        /// <summary>
        /// find all media in selected category and display them
        /// </summary>
        /// <param name="dvMedia"></param>
        public void displayCategory(DataView dvMedia, string category, string mediaType)
        {
            displayCategory(dvMedia, category, mediaType, true);
        }

        public void displayCategory(DataView dvMedia, string category, string mediaType, bool clearCurrentMedia)
        {
            try
            {
                string imageKey = "";
                string spaces = "";
                string currentPlaylist = "";

                if (category.Length == 0)
                    category = "All";

                //make sure the browser is destroyed since we aren't using it any more
                if (embedBrowser)
                {
                    if (browser != null)
                    {
                        browser.Dispose();
                        browser = null;
                    }

                    if (safeSurf != null)
                    {
                        safeSurf.Dispose();
                        safeSurf = null;
                    }
                }

                //lblOnlineContent.Visible = false;

                //clear media
                if (clearCurrentMedia)
                {
                    //clear controls - the media details
                    lvFF.Controls.Clear();
                    lvFF.Items.Clear();

                    dtRemainingResults = null;
                }

                if (dvMedia.Count > 0)
                {
                    ListViewGroup lvig = null;

                    if (category.ToLower() == "playlist-removeForNow")
                    {
                        foreach (DataRowView dv in dvMedia)
                        {
                            if (dv["playlistName"].ToString() != currentPlaylist)
                            {
                                currentPlaylist = dv["playlistName"].ToString();

                                lvig = new ListViewGroup(currentPlaylist);
                                lvFF.Groups.Add(lvig);
                            }

                            PlaylistItem newItem = new PlaylistItem();
                            newItem.PlaylistName = currentPlaylist;
                            newItem.MediaSource = dv["mediaSource"].ToString();

                            //add item
                            ListViewItem listViewItem1 = new ListViewItem();

                            string mediaSource = "  "+ dv["MediaSource"].ToString().Trim();

                            //make title titleMaxLength characters
                            //titleMaxLength = lvFF.Columns[0].Width - lvFF.SmallImageList.ImageSize.Width - 60;

                            if (mediaSource.Length > titleMaxLength)
                            {
                                mediaSource = mediaSource.Substring(0, titleMaxLength - 3) + "...";
                                //title = title.Substring(0, titleMaxLength) + Environment.NewLine + title.Substring(titleMaxLength).PadRight(titleMaxLength, Convert.ToChar(" "));
                            }
                            else
                                mediaSource = mediaSource.PadRight(titleMaxLength, Convert.ToChar(" "));

                            listViewItem1.Text = mediaSource;
                            listViewItem1.Tag = newItem;

                            string imagePath = "";

                            //if (dv["coverImage"] != null)
                            //    imagePath = dv["coverImage"].ToString().Trim();

                            if (!File.Exists(imagePath))
                            {
                                if (File.Exists(Application.StartupPath + "\\" + imagePath))
                                    imagePath = Application.StartupPath + "\\" + imagePath;
                            }

                            //get image info
                            if (imagePath.Length > 0 && File.Exists(imagePath))
                            {
                                imageKey = imagePath;

                                if (!ilMediaImages.Images.ContainsKey(imageKey))
                                {
                                    //if (File.Exists(imageKey))
                                    //{
                                    Bitmap newImage = new Bitmap(imageKey);
                                    ilMediaImages.Images.Add(imageKey, newImage);//add new image
                                    //}
                                    //else
                                    //    MessageBox.Show("Image not found " + Environment.NewLine + imageKey);
                                }
                            }
                            else
                                imageKey = "defaultImage";

                            listViewItem1.ImageKey = imageKey;

                            if (lvig != null)
                                listViewItem1.Group = lvig;

                            listViewItem1.ToolTipText = newItem.MediaSource;

                            //if (media.Description.Length > 0)
                            //    listViewItem1.ToolTipText += "\n" + media.Description;
                            //listViewItem1.ToolTipText += "\n" + dv["url"].ToString();

                            listViewItem1.ToolTipText += "\nPlaylist: " + newItem.PlaylistName;
                            //lvFF.Items.Add(listViewItem1);
                            lvFF.Items.Add(listViewItem1);

                            //if (lvFF.Items.Count > 0 && lvFF.Items.Count % 50 == 0)
                            //    lvFF.Update();

                            if (lvFF.Items.Count > 0 && lvFF.Items.Count % 50 == 0)
                                lvFF.Update();
                        }
                    }
                    else
                    {
                        switch (mediaType.ToLower())
                        {
                            case "online":
                                dvMedia.RowFilter = "category LIKE '*" + category + "*'";

                                lvig = new ListViewGroup(category);
                                lvFF.Groups.Add(lvig);

                                foreach (DataRowView dv in dvMedia)
                                {
                                    //create mediaType item
                                    OnlineMediaType media = new OnlineMediaType();
                                    media.Name = dv["name"].ToString();
                                    media.Category = dv["category"].ToString();
                                    media.Description = dv["description"].ToString();
                                    media.type = dv["mediaCategory"].ToString();
                                    media.URL = dv["url"].ToString();
                                    media.CoverImage = Application.StartupPath + dv["coverImage"].ToString();

                                    //add item
                                    ListViewItem listViewItem1 = new ListViewItem();

                                    string title = "  "+ dv["name"].ToString().Trim();

                                    //make title 35 characters
                                    //titleMaxLength = lvFF.Columns[0].Width - lvFF.SmallImageList.ImageSize.Width - 60;

                                    if (title.Length > titleMaxLength)
                                    {
                                        title = title.Substring(0, titleMaxLength - 3) + "...";
                                        //title = title.Substring(0, titleMaxLength) + Environment.NewLine + title.Substring(titleMaxLength).PadRight(titleMaxLength, Convert.ToChar(" "));
                                    }
                                    else
                                        title = title.PadRight(titleMaxLength, Convert.ToChar(" "));

                                    listViewItem1.Text = title;
                                    listViewItem1.Tag = media;

                                    string imagePath = "";

                                    if (dv["coverImage"] != null)
                                        imagePath = dv["coverImage"].ToString().Trim();

                                    if (!File.Exists(imagePath))
                                    {
                                        if (File.Exists(Application.StartupPath + "\\" + imagePath))
                                            imagePath = Application.StartupPath + "\\" + imagePath;
                                    }

                                    //get image info
                                    if (imagePath.Length > 0 && File.Exists(imagePath))
                                    {
                                        imageKey = imagePath;

                                        if (!ilMediaImages.Images.ContainsKey(imageKey))
                                        {
                                            //if (File.Exists(imageKey))
                                            //{
                                            Bitmap newImage = new Bitmap(imageKey);
                                            ilMediaImages.Images.Add(imageKey, newImage);//add new image
                                            //}
                                            //else
                                            //    MessageBox.Show("Image not found " + Environment.NewLine + imageKey);
                                        }
                                    }
                                    else
                                        imageKey = "defaultImage";

                                    listViewItem1.ImageKey = imageKey;
                                    listViewItem1.Group = lvig;
                                    listViewItem1.ToolTipText = media.Name;
                                    listViewItem1.Name = media.Name;

                                    if (media.Description.Length > 0)
                                        listViewItem1.ToolTipText += "\n" + media.Description;
                                    listViewItem1.ToolTipText += "\n" + dv["url"].ToString();

                                    //lvFF.Items.Add(listViewItem1);
                                    lvFF.Items.Add(listViewItem1);

                                    //if (lvFF.Items.Count > 0 && lvFF.Items.Count % 50 == 0)
                                    //    lvFF.Update();

                                    if (lvFF.Items.Count > 0 && lvFF.Items.Count % 50 == 0)
                                        lvFF.Update();
                                }
                                break;
                            case "games":
                                //get id of selected category
                                MediaHandler.dsGameCategories.Tables[0].DefaultView.RowFilter = "name = '" + category + "'";
                                string categoryID = MediaHandler.dsGameCategories.Tables[0].DefaultView[0]["categoryID"].ToString();

                                //get games in the selected category
                                MediaHandler.dsGames.Tables[0].DefaultView.RowFilter = "categoryID = '" + categoryID + "' and display = 'True'";

                                if (lvFF.Items.Count == 0)
                                    lvFF.Groups.Clear();

                                //ilMediaImages.Images.Clear();

                                //dvMedia.RowFilter = "category LIKE '*" + category + "*'";

                                //lvig = new ListViewGroup(category);
                                //lvFF.Groups.Add(lvig);

                                if (ilMediaImages.Images.Count == 0 || !ilMediaImages.Images.ContainsKey("defaultImage"))
                                {
                                    //clear any images that might be in the list
                                    ilMediaImages.Images.Clear();

                                    //add the default image to the list
                                    Bitmap defaultImage = new Bitmap(Application.StartupPath + "\\images\\gameThumbnails\\_comingSoon.gif");
                                    ilMediaImages.Images.Add("defaultImage", defaultImage);//add default image
                                }

                                int listViewStartCount = lvFF.Items.Count;
                                int newItemCounter = 0;

                                lvFF.BeginUpdate();

                                foreach (DataRowView dv in MediaHandler.dsGames.Tables[0].DefaultView)
                                {
                                    //get category of current game
                                    //MediaHandler.dsGameCategories.Tables[0].DefaultView.RowFilter = "gameID = '"+ dv["categoryID"].ToString() +"'";

                                    //MediaHandler.dsGameCategories.Tables[0].DefaultView[0]

                                    //create game item
                                    //Game theGame = new Game();
                                    Media theGame = MediaHandler.CreateMedia(dv);
                                    //theGame.Title = dv["title"].ToString();
                                    theGame.category = category;
                                    //theGame.Description = dv["description"].ToString();
                                    //theGame.Type = dv["mediaCategory"].ToString();
                                    theGame.filePath = Application.StartupPath +"\\games\\"+ dv["filename"].ToString();
                                    theGame.coverImage = Application.StartupPath + "\\images\\gameThumbnails\\" + dv["coverImage"].ToString();

                                    //create media item
                                    //Media media = MediaHandler.CreateMedia(dv);
                                    theGame.MediaType = "Games";

                                    //add item
                                    ListViewItem listViewItem1 = new ListViewItem();

                                    string title = "  "+ theGame.Title;

                                    //make title 35 characters
                                    //titleMaxLength = lvFF.Columns[0].Width - lvFF.SmallImageList.ImageSize.Width - 60;

                                    if (title.Length > titleMaxLength)
                                    {
                                        title = title.Substring(0, titleMaxLength - 3) + "...";
                                        //title = title.Substring(0, titleMaxLength) + Environment.NewLine + title.Substring(titleMaxLength).PadRight(titleMaxLength, Convert.ToChar(" "));
                                    }
                                    else
                                        title = title.PadRight(titleMaxLength, Convert.ToChar(" "));

                                    listViewItem1.Text = title;
                                    //listViewItem1.Tag = media;

                                    //string imagePath = "";

                                    //if (theGame.Thumbnail != null)
                                    //    imagePath = Application.StartupPath + ;

                                    //if (!File.Exists(imagePath))
                                    //{
                                    //    if (File.Exists(Application.StartupPath + "\\" + imagePath))
                                    //        imagePath = Application.StartupPath + "\\" + imagePath;
                                    //}

                                    //get image info
                                    if (theGame.coverImage.Length > 0 && File.Exists(theGame.coverImage))
                                    {
                                        imageKey = theGame.coverImage;

                                        if (!ilMediaImages.Images.ContainsKey(imageKey))
                                        {
                                            try
                                            {
                                                //if (File.Exists(imageKey))
                                                //{
                                                Bitmap newImage = new Bitmap(imageKey);
                                                ilMediaImages.Images.Add(imageKey, newImage);//add new image

                                                theGame.ImageListIndex = Convert.ToString(ilMediaImages.Images.Count - 1);

                                                //}
                                                //else
                                                //  MessageBox.Show("Image not found " + Environment.NewLine + imageKey);
                                            }
                                            catch (Exception ex)
                                            {
                                                //the image file was invalid
                                                imageKey = "defaultImage";

                                                theGame.ImageListIndex = "0";
                                            }
                                        }
                                        else
                                            theGame.ImageListIndex = ilMediaImages.Images.IndexOfKey(imageKey).ToString();
                                    }
                                    else
                                    {
                                        imageKey = "defaultImage";

                                        theGame.ImageListIndex = "0";
                                    }

                                    listViewItem1.Tag = theGame;
                                    listViewItem1.ImageKey = imageKey;
                                    listViewItem1.Group = lvig;
                                    listViewItem1.ToolTipText = theGame.Title;
                                    listViewItem1.Name = theGame.Title;

                                    if (theGame.Description.Length > 0)
                                        listViewItem1.ToolTipText += "\n" + theGame.Description;
                                    //listViewItem1.ToolTipText += "\n" + dv["url"].ToString();

                                    lvFF.Items.Add(listViewItem1);

                                    newItemCounter++;

                                    if (lvFF.Items.Count > 0 && lvFF.Items.Count % 50 == 0)
                                        lvFF.Update();
                                }

                                int tempIndex = 0;

                                for (int i = listViewStartCount; i < newItemCounter; i++)
                                {
                                    ListViewItem lvi = this.lvFF.Items[i];

                                    if (!int.TryParse(((Media)lvi.Tag).ImageListIndex, out tempIndex))
                                        tempIndex = 0;

                                    lvi.ImageIndex = tempIndex;

                                    this.lvFF.Invalidate(lvi.Bounds);
                                    this.Update();
                                }

                                lvFF.EndUpdate();
                                break;
                            default:
                                lvFF.BeginUpdate();

                                //ListViewGroup lvig = new ListViewGroup(dvMedia[0]["category"].ToString());
                                bool foundGroup = false;
                                
                                if (lvFF.Items.Count == 0)
                                    lvFF.Groups.Clear();

                                if ((category.ToLower() != "star rating" && mediaType.ToLower() != "justin downloads"))
                                {
                                    //foreach (ListViewGroup group in lvFF.Groups)
                                    //{
                                    //    if (group.Header != null && group.Header == category)
                                    //    {
                                    //        foundGroup = true;
                                    //        lvig = new ListViewGroup(category);
                                    //        break;
                                    //    }
                                    //}

                                    //if (!foundGroup || lvig == null)
                                    //{

                                    if (category.Contains("|"))
                                        btnFilterOutResults.Visible = true;
                                    else
                                        btnFilterOutResults.Visible = false;

                                    if(btnFilterOutResults.Text == "+")
                                        category = category.Replace("|", " and ");
                                    else
                                        category = category.Replace("|", " or ");

                                    lvig = new ListViewGroup("     "+ category);
                                    //lvFF.Groups.Add(lvig);
                                    lvFF.Groups.Add(lvig);

                                    //    foundGroup = true;
                                    //}
                                }
                                else
                                {
                                    lvFF.Groups.Clear();
                                }

                                if (ilMediaImages.Images.Count == 0 || !ilMediaImages.Images.ContainsKey("defaultImage"))
                                {
                                    //clear any images that might be in the list
                                    ilMediaImages.Images.Clear();

                                    //add the default image to the list
                                    Bitmap defaultImage = new Bitmap(Application.StartupPath + "\\images\\Media\\coverImages\\notAvailable.jpg");
                                    ilMediaImages.Images.Add("defaultImage", defaultImage);//add default image
                                }

                                //int listViewStartCount = lvFF.Items.Count;
                                listViewStartCount = lvFF.Items.Count;
                                newItemCounter = 0;
                                dtRemainingResults = dvMedia.Table.Clone();
                                
                                foreach (DataRowView dv in dvMedia)//go through the new category results
                                {
                                    //just load enough items to equal loadOnDemandChunk to decrease load times
                                    if (lvFF.Items.Count >= loadOnDemandChunk + listViewStartCount)
                                    {
                                        //load the remaining items in dvMedia into a temp datatable to load when needed
                                        //string[] rows = new string[dvMedia.Count];
                                        //dvMedia.Table.Rows.CopyTo(rows, newItemCounter);

                                        dtRemainingResults.ImportRow(dv.Row);
                                    }
                                    else
                                    {
                                        if (category.ToLower() == "star rating")
                                        {
                                            foundGroup = false;

                                            foreach (ListViewGroup group in lvFF.Groups)
                                            {
                                                if (group.Name == dv["stars"].ToString().Trim())
                                                {
                                                    foundGroup = true;

                                                    break;
                                                }
                                            }

                                            if (!foundGroup || lvFF.Groups.Count == 0)
                                            {
                                                //set the group to show the star rating
                                                if (dvMedia.Table.Columns.Contains("stars") && dv["stars"] != null)
                                                {
                                                    lvig = new ListViewGroup(dv["stars"].ToString() + " Stars");
                                                    lvig.Name = dv["stars"].ToString();
                                                }
                                                else
                                                {
                                                    lvig = new ListViewGroup("0 Stars");
                                                    lvig.Name = "0 Stars";
                                                }

                                                lvFF.Groups.Add(lvig);
                                            }
                                        }

                                        if (category.ToLower() == "justin downloads" && mediaType.ToLower() != "movies")
                                        {
                                            foundGroup = false;

                                            foreach (ListViewGroup group in lvFF.Groups)
                                            {
                                                if (group.Name == dv["SortBy"].ToString().Trim())
                                                {
                                                    foundGroup = true;

                                                    lvig = group;

                                                    break;
                                                }
                                            }

                                            if (!foundGroup || lvFF.Groups.Count == 0)
                                            {
                                                //set the group to display the show name
                                                lvig = new ListViewGroup(dv["SortBy"].ToString().Trim());
                                                lvig.Name = dv["SortBy"].ToString().Trim();

                                                lvFF.Groups.Add(lvig);
                                            }
                                        }

                                        if (category.ToLower() == "playlist")
                                        {
                                            //set the group to display the playlist name
                                            lvig = new ListViewGroup(dv["Playlists"].ToString().Trim());
                                            lvig.Name = dv["Playlists"].ToString().Trim();

                                            lvFF.Groups.Add(lvig);
                                        }

                                        //create media item
                                        Media media = MediaHandler.CreateMedia(dv);

                                        //add item
                                        ListViewItem listViewItem1 = new ListViewItem();

                                        string title = "  "+ dv["title"].ToString().Trim();

                                        //make title 25 characters
                                        //titleMaxLength = lvFF.Columns[0].Width - lvFF.SmallImageList.ImageSize.Width - 60;

                                        if (title.Length > titleMaxLength)
                                        {
                                            title = title.Substring(0, titleMaxLength - 3) + "...";
                                            //title = title.Substring(0, titleMaxLength) + Environment.NewLine + title.Substring(titleMaxLength).PadRight(titleMaxLength, Convert.ToChar(" "));
                                        }
                                        else
                                            title = title.PadRight(titleMaxLength, Convert.ToChar(" "));

                                        listViewItem1.Text = title;
                                        listViewItem1.Tag = media;

                                        //get image info
                                        if (dv["coverImage"] != null && dv["coverImage"].ToString().Length > 0 && File.Exists(dv["coverImage"].ToString()))
                                        {
                                            imageKey = dv["coverImage"].ToString();

                                            if (!ilMediaImages.Images.ContainsKey(imageKey))
                                            {
                                                try
                                                {
                                                    //if (File.Exists(imageKey))
                                                    //{
                                                    Bitmap newImage = new Bitmap(imageKey);
                                                    ilMediaImages.Images.Add(imageKey, newImage);//add new image

                                                    media.ImageListIndex = Convert.ToString(ilMediaImages.Images.Count - 1);

                                                    //}
                                                    //else
                                                    //  MessageBox.Show("Image not found " + Environment.NewLine + imageKey);
                                                }
                                                catch (Exception ex)
                                                {
                                                    //the image file was invalid
                                                    imageKey = "defaultImage";

                                                    media.ImageListIndex = "0";
                                                }
                                            }
                                            else
                                                media.ImageListIndex = ilMediaImages.Images.IndexOfKey(imageKey).ToString();
                                        }
                                        else
                                        {
                                            imageKey = "defaultImage";

                                            media.ImageListIndex = "0";
                                        }

                                        //listViewItem1.ImageKey = imageKey;
                                        
                                        listViewItem1.Group = lvig;
                                        listViewItem1.ToolTipText = media.Title;

                                        if (media.Rating.Length > 0)
                                            listViewItem1.ToolTipText += " (" + media.Rating + ")";
                                        //else
                                         //    listViewItem1.ToolTipText += " (NA)";

                                        if (media.TagLine != null && media.TagLine.Length > 0)
                                            listViewItem1.ToolTipText += "\n" + media.TagLine;

                                        if (media.RatingDescription != null && media.RatingDescription.Length > 0)
                                        {
                                            listViewItem1.ToolTipText += "\n" + "(" + media.Rating + ")" + media.RatingDescription;
                                        }

                                        if (media.Description != null && media.Description.Length > 0)
                                            listViewItem1.ToolTipText += "\n" + media.Description;

                                        listViewItem1.ToolTipText += "\n" + dv["filePath"].ToString();
                                        
                                        lvFF.Items.Add(listViewItem1);

                                        newItemCounter++;
                                    }
                                }

                                tempIndex = 0;

                                for (int i = listViewStartCount; i < newItemCounter; i++)
                                {
                                    ListViewItem lvi = this.lvFF.Items[i];

                                    if (!int.TryParse(((Media)lvi.Tag).ImageListIndex, out tempIndex))
                                        tempIndex = 0;

                                    lvi.ImageIndex = tempIndex;

                                    this.lvFF.Invalidate(lvi.Bounds);
                                    this.Update();
                                }

                                lvFF.EndUpdate();

                                break;
                        }
                    }
                }
                else
                {
                    if (category.ToLower() == "home" || category == "")
                    {
                        try
                        {
                            //show edit box
                            //SourcePathForm sourcePathForm = new SourcePathForm();
                            //sourcePathForm.SourcePaths = myMedia.Locations;
                            //sourcePathForm.ShowDialog(this);

                            ////update source paths
                            //myMedia.AddLocations(sourcePathForm.SourcePaths, true);

                            //lblOnlineContent.Visible = true;
                            //lblOnlineContent.BringToFront();

                            //displayBrowser();

                            //safeSurf.URL = new Uri("http://hulu.com");

                            //safeSurf.BringToFront();

                            //lblOnlineContent.BringToFront();
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message.Contains("The remote name could not be resolved:"))//the site is not available
                            {
                                Label lblNoResults = new Label();
                                lblNoResults.Text = "Choose your online content";
                                lblNoResults.Top = 50;
                                lblNoResults.Left = 50;
                                lblNoResults.Width = 250;
                                lblNoResults.BackColor = Color.White;
                                //lvFF.Controls.Add(lblNoResults);
                                lvFF.Controls.Add(lblNoResults);
                            }
                            else
                                Tools.WriteToFile(ex);
                        }
                    }
                    else if (category.ToLower() == "internet")//display browser
                    {
                        displayBrowser();
                    }
                    else
                    {
                        ListViewGroup lvig = new ListViewGroup("     "+ scrollingTabsGenre.SelectedTabs);
                        lvFF.Groups.Add(lvig);

                        ListViewItem listViewItem1 = new ListViewItem();
                        listViewItem1.Group = lvig; 
                        lvFF.Items.Add(listViewItem1);

                        Label lblNoResults = new Label();
                        lblNoResults.Text = "No Results";
                        lblNoResults.Top = 50;
                        lblNoResults.Left = 50;
                        lblNoResults.Width = 150;
                        lblNoResults.BackColor = Color.White;
                        lvFF.Controls.Add(lblNoResults);
                    }
                }
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }
        }

        private void initializeAdvancedFilters()
        {
            //get mediatypes
            string correctCase = "";

            ArrayList mediaTypes = new ArrayList();

            try
            {
                //foreach (DataRowView drv in myMedia.GetAllMediaTypes())
                ArrayList allMediaTypes = myMedia.GetAllMediaTypes();

                foreach (string type in allMediaTypes)
                {
                    string[] typeArray = type.ToString().Split('/');

                    for (int x = 0; x < typeArray.Length; x++)
                    {
                        correctCase = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(typeArray[x].Trim());
                        if (!mediaTypes.Contains(correctCase) && correctCase.Trim().Length > 0)//keep out duplicates
                            mediaTypes.Add(correctCase);
                    }

                    //correctCase = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(type.Trim());
                    //if (!mediaTypes.Contains(correctCase))//keep out duplicates
                    //    mediaTypes.Add(correctCase);
                }

                mediaTypes.Add("Online");
                mediaTypes.Sort();

                //set media types
                foreach (string tempType in mediaTypes)
                    mediaTypeToolStripMenuItem.DropDownItems.Add(tempType, null, mediaTypeToolStripMenuItem_Click);
                
                setAdvancedGenreFilters(mediaTypes);
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, "initializeAdvancedFilters error: " + ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        private void lvMedia_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int timesPlayed = 0;

            foreach (ListViewItem selectedItem in lvFF.SelectedItems)
            {
                if (selectedItem.Tag is Media)
                {
                    Media media = (Media)selectedItem.Tag;

                    if (scrollingTabsGenre.SelectedTabs.ToLower().Contains("playlist"))
                    {
                        //start playing playlist from item clicked
                        try
                        {
                            ArrayList newPlaylist = new ArrayList();
                            //PlaylistItem selectedPlaylistItem = (PlaylistItem)selectedItem.Tag;

                            //newPlaylist.Add(media.filePath);

                            //iterate playlist
                            foreach (DataRow dr in myMedia.GetPlaylist(media.Playlists).Tables[0].Rows)
                            {
                                if (!newPlaylist.Contains(dr["mediaSource"].ToString()))
                                    newPlaylist.Add(dr["mediaSource"].ToString());
                            }


                            //Cursor.Current = Cursors.WaitCursor;

                            //splashScreen mediaSplash = new splashScreen();

                            ////if (Form1.Mode == "Release")
                            ////{
                            //mediaSplash.SplashMessage2 = media.Playlists;

                            //mediaSplash.Show();

                            
                            //}

                            //this.Hide();
                            mediaWindow = createMediaWindow(media);
                            //						mediaWindow.playVideo(selectedLabel.Tag.ToString());
                            //mediaWindow.PlayClip(selectedMedia);
                            mediaWindow.PlayMedia(newPlaylist, "", "");

                            //if (Form1.Mode == "Release")
                            //{
                            //mediaSplash.Close();
                            
                            //}

                            //mediaWindow.ShowDialog(this);
                            //mediaWindow.Show();
                            //SplashScreenNew.CloseForm();
                            //Cursor.Current = Cursors.Arrow;

                            //mediaWindow.TopMost = true;

                        }
                        catch (Exception ex)
                        {
                            Tools.WriteToFile(Tools.errorFile, "Play playlist error: " + ex.Message);
                        }
                    }
                    else
                    {
                        //update timesPlayed and lastplayed
                        int.TryParse(media.TimesPlayed, out timesPlayed);
                        media.TimesPlayed = Convert.ToString(timesPlayed + 1);
                        media.LastPlayed = DateTime.Now.ToString();
                        myMedia.UpdateMediaInfo(media);

                        if (media.MediaType.ToLower().Contains("pictures"))
                        {
                            //display picture
                            //pbPictures.ImageLocation = media.filePath;
                            //System.Drawing.Image thePicture = System.Drawing.Image.FromFile(media.filePath);

                            pbPictures.BackgroundImage = System.Drawing.Image.FromFile(media.filePath);
                            pbPictures.Visible = true;
                            btnClosePicture.Visible = true;
                            btnClosePicture.BringToFront();
                        }
                        else
                        {
                            //play media
                            playMedia(media);
                        }
                    }
                }
                else if (selectedItem.Tag is OnlineChannel)
                {
                    OnlineChannel channel = (OnlineChannel)selectedItem.Tag;

                    //play channel
                    SCTVJustinTV.JustinTV justinTV = new SCTVJustinTV.JustinTV("");
                    justinTV.PlayChannel(channel);

                    justinTV.ShowDialog(this);
                }
                else if (selectedItem.Tag is OnlineMediaType)
                {
                    OnlineMediaType onlineMedia = (OnlineMediaType)selectedItem.Tag;

                    //display media
                    displayBrowser();

                    string url = onlineMedia.URL;

                    if (!url.ToLower().StartsWith("http://"))
                        url = "http://" + url;

                    safeSurf.URL = new Uri(url);

                    if (url.ToLower().Contains("justin.tv"))
                        safeSurf.ShowJustinRecordButton = true;
                }
                else if (selectedItem.Tag is PlaylistItem)
                {
                    //start playing playlist from item clicked
                    try
                    {
                        ArrayList newPlaylist = new ArrayList();
                        PlaylistItem selectedPlaylistItem = (PlaylistItem)selectedItem.Tag;

                        newPlaylist.Add(selectedPlaylistItem.MediaSource);

                        //iterate playlist
                        foreach (DataRow dr in myMedia.GetPlaylist(selectedPlaylistItem.PlaylistName).Tables[0].Rows)
                        {
                            if(!newPlaylist.Contains(dr["mediaSource"].ToString()))
                                newPlaylist.Add(dr["mediaSource"].ToString());
                        }


                        //Cursor.Current = Cursors.WaitCursor;

                        //splashScreen mediaSplash = new splashScreen();

                        ////if (Form1.Mode == "Release")
                        ////{
                        //mediaSplash.SplashMessage2 = selectedPlaylistItem.PlaylistName;

                        //mediaSplash.Show();

                        //SplashScreenNew.ShowSplashScreen();

                        //}

                        //this.Hide();
                        //mediaWindow = createMediaWindow();
                        ////						mediaWindow.playVideo(selectedLabel.Tag.ToString());
                        ////mediaWindow.PlayClip(selectedMedia);
                        //mediaWindow.PlayMedia(newPlaylist, "", "");

                        //if (Form1.Mode == "Release")
                        //{
                        //mediaSplash.Close();
                        //SplashScreenNew.CloseForm();
                        //}

                        //mediaWindow.ShowDialog(this);
                        //mediaWindow.Show();

                        //Cursor.Current = Cursors.Arrow;

                        //mediaWindow.TopMost = true;

                    }
                    catch (Exception ex)
                    {
                        Tools.WriteToFile(Tools.errorFile, "Play playlist error: " + ex.Message);
                    }
                }
                else
                    MessageBox.Show("Media type not supported");
            }
        }

        private void displayBrowser()
        {
            if (safeSurf == null || safeSurf.IsDisposed)
            {
                safeSurf = new MainForm();

                //safeSurf.Dock = DockStyle.Fill;
                safeSurf.FormBorder = FormBorderStyle.Sizable;
                safeSurf.ShowMenuStrip = false;
                safeSurf.TopLevel = true;
                safeSurf.ShowVolumeControl = true;
                safeSurf.ShowAddressBar = false;
                this.TopMost = false;
                //safeSurf.Parent = this;

                if (embedBrowser)
                {
                    safeSurf.Dock = DockStyle.Fill;
                    safeSurf.Parent = this;
                    lvFF.Controls.Add(safeSurf);
                }

                safeSurf.Show();
            }
        }

        /// <summary>
        /// play selected media
        /// </summary>
        public void playMedia(Media selectedMedia)//play selected media
        {
            try
            {
                int timesPlayed = 0;

                //update timesPlayed and lastplayed
                int.TryParse(selectedMedia.TimesPlayed, out timesPlayed);
                selectedMedia.TimesPlayed = Convert.ToString(timesPlayed + 1);
                selectedMedia.LastPlayed = DateTime.Now.ToString();
                myMedia.UpdateMediaInfo(selectedMedia);

                //string selectedMedia = getSelectedMediaFileName();

                string selectedFilePath = selectedMedia.filePath;

                if (selectedFilePath.Contains("|"))
                    selectedFilePath = selectedFilePath.Substring(0, selectedFilePath.IndexOf("|"));

                if (File.Exists(selectedFilePath))
                {
                    if (selectedMedia.MediaType.ToLower().Contains("document") || selectedMedia.MediaType.ToLower().Contains("books"))
                    {
                        //display media
                        displayBrowser();

                        string url = selectedFilePath;

                        if (!url.ToLower().StartsWith("file:\\\\"))
                            url = "file:\\\\" + url;

                        safeSurf.URL = new Uri(url);

                        SplashScreenNew.CloseForm();
                    }
                    else
                    {
                        //Cursor.Current = Cursors.WaitCursor;

                        //splashScreen mediaSplash = new splashScreen();

                        ////if (Form1.Mode == "Release")
                        ////{
                        //mediaSplash.SplashMessage2 = selectedMedia.Title;

                        //mediaSplash.Show();

                        //mediaSplash.Refresh();



                        //this.Refresh();
                        //}

                        //this.Hide();
                        mediaWindow = createMediaWindow(selectedMedia);

                        //mediaWindow.MediaSplash = mediaSplash;
                        mediaWindow.PlaySequels += new liquidMediaPlayer.Sequels(mediaWindow_PlaySequels);
                        mediaWindow.ContinousPlayOn += new liquidMediaPlayer.ContinousPlaySelected(mediaWindow_ContinousPlay);
                        mediaWindow.StartedPlaying += new liquidMediaPlayer.startedPlaying(mediaWindow_StartedPlaying);
                        currentMedia = selectedMedia;
                        //						mediaWindow.playVideo(selectedLabel.Tag.ToString());
                        mediaWindow.PlayClip(selectedMedia);

                        //mediaWindow.Show();
                        //mediaWindow.TopMost = true;

                        //Cursor.Current = Cursors.Arrow;


                    }
                }
                else
                {
                    Tools.WriteToFile(Tools.errorFile, "---- Missing File : " + selectedMedia + " ----");
                    MessageBox.Show("Missing File - " + selectedMedia);
                }
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, "PlayMedia error: " + ex.Message);
            }
        }

        void mediaWindow_StartedPlaying()
        {
            //mediaWindow.Show();
            //mediaWindow.TopMost = true;

            //SplashScreenNew.CloseForm();
            //mediaSplash.Close();
        }

        void mediaWindow_PlaySequels()
        {
            playSequels = true;
        }

        void mediaWindow_ContinousPlay()
        {
            continousPlay = true;
        }

        void mediaWindow_MediaDonePlaying()
        {
            //check for sequel
            if (mediaWindow.PlaySequel || mediaWindow.ContinousPlay)
            {
                Media theSequel;

                if (mediaWindow.WhatsNext == null)
                    theSequel = myMedia.GetSequel(currentMedia, continousPlay);
                else
                    theSequel = mediaWindow.WhatsNext;

                if (theSequel != null)
                    playMedia(theSequel);
            }
        }

        private liquidMediaPlayer createMediaWindow(Media selectedMedia)
        {
            try
            {
                this.Invalidate();

                if (mediaWindow != null)
                {
                    mediaWindow.stop();
                    mediaWindow.Dispose();
                    mediaWindow = null;
                }

                mediaWindow = new liquidMediaPlayer();

                if(speechListener != null)
                    mediaWindow.SpeechListener = speechListener;

                //mediaWindow.Closed += new EventHandler(mediaWindow_Closed);
                mediaWindow.closingForm += new liquidMediaPlayer.closing(mediaWindow_MediaDonePlaying);
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, "createMediaWindow error: " + ex.Message);
            }

            return mediaWindow;
        }

        private void _MouseDown(object sender, MouseEventArgs e)
        {
            //find right click
            if (e.Button == MouseButtons.Right)
            {
                selectedControl = sender;

                //show context menu
                //itemContextMenu.Show(this, this.PointToClient(Cursor.Position));
            }
        }

        private int getLeftSide()
        {
            decimal currentRow = (decimal)(lvFF.Items.Count / lvFF.Columns.Count);
            int currentColumn = 0;
            int remainder;
            remainder = lvFF.Items.Count % lvFF.Columns.Count;

            if (remainder > 0)
            {
                currentRow++;
                currentColumn = remainder;
            }
            else
                currentColumn = lvFF.Columns.Count;

            //return (currentColumn - 1) * 500 + lvFF.SmallImageList.ImageSize.Width;
            return (currentColumn - 1) * 500 + 60;
        }

        private int getTop()
        {
            int currentRow = (int)(lvFF.Items.Count / lvFF.Columns.Count);
            int currentColumn = 0;
            int remainder;
            remainder = lvFF.Items.Count % lvFF.Columns.Count;

            if (remainder > 0)
            {
                currentRow++;
                currentColumn = remainder;
            }

            return (currentRow - 1) * lvFF.SmallImageList.ImageSize.Height + 40;
        }

        #region toolstrip methods

        /// <summary>
        /// Refresh media
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void refreshMedia_Click(object sender, EventArgs e)
        {
            RefreshToolStripButton.Enabled = false;
            refreshMediaToolStripMenuItem.Enabled = false;

            pictureBox1.Visible = true;
            toolStripStatusLabel1.Text = "Refreshing Media";

            simpleDelegate = new MethodInvoker(lookForMedia);

            // Calling lookForMedia Async
            simpleDelegate.BeginInvoke(new AsyncCallback(ThreadFinished), null);
        }

        private void lookForMedia()
        {
            myMedia.lookForMedia(true);
        }

        // Set initial state of controls.
        // Called from worker thread using delegate and Control.Invoke
        private void ThreadFinished(IAsyncResult ar)
        {
            // update the grid a thread safe fashion!
            MethodInvoker updateMedia = delegate
            {
                displayMedia();

                scrollingTabsMediaTypes.SelectedTabIndexes = "0";

                scrollingTabsMediaTypes_SelectionChanged();

                //displayCategory(scrollingTabsGenre.SelectedTabs, scrollingTabsAlphabet.SelectedTabs, scrollingTabsMediaTypes.SelectedTabs);

                RefreshToolStripButton.Enabled = true;
                refreshMediaToolStripMenuItem.Enabled = true;

                pictureBox1.Visible = false;
                toolStripStatusLabel1.Text = "Done Refreshing Media";
            };

            if (this.InvokeRequired)
                this.Invoke(updateMedia);
            else
                updateMedia();
        }

        /// <summary>
        /// Set source for media
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SourceToolStripButton_Click(object sender, EventArgs e)
        {
            //show edit box
            SourcePathForm sourcePathForm = new SourcePathForm();
            sourcePathForm.SourcePaths = myMedia.Locations;
            sourcePathForm.ShowDialog(this);
            
            //update source paths
            myMedia.AddLocations(sourcePathForm.SourcePaths, true);

            if (myMedia.UpdatedLocations)//the locations updated so check for new media files
            {
                UpdateMedia();
            }
        }

        public void UpdateMedia()
        {
            RefreshToolStripButton.Enabled = false;
            refreshMediaToolStripMenuItem.Enabled = false;

            pictureBox1.Visible = true;
            toolStripStatusLabel1.Text = "Refreshing Media";

            simpleDelegate = new MethodInvoker(lookForMedia);

            // Calling lookForMedia Async
            simpleDelegate.BeginInvoke(new AsyncCallback(ThreadFinished), null);
        }

        /// <summary>
        /// show security form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void securityToolStripButton_Click(object sender, EventArgs e)
        {
            try
            {
                //show context menu
                //CameraContextMenuStrip.Show(this, this.PointToClient(Cursor.Position));

                SCTVCameraMain SCTVCamera = new SCTVCameraMain();
                SCTVCamera.Opacity = 100;
                SCTVCamera.Show();
            }
            catch (Exception ex)
            {
                if (ex.InnerException.Message == "No devices of the category")
                    MessageBox.Show("There are no cameras connected");
            }
        }

        private void gamesToolStripButton_Click(object sender, EventArgs e)
        {
            //display games UI
            SCTVGames.Games games = new SCTVGames.Games();
            games.ShowDialog(this);
        }

        private void playListToolStripButton_Click(object sender, EventArgs e)
        {
            if (playlistForm == null)//show playlist
            {
                playlistForm = new PlayList(playlist);

                DialogResult result = playlistForm.ShowDialog(this);

                if (result == DialogResult.OK)//play playlist
                {
                    ArrayList arrSelectedMedia = new ArrayList();

                    foreach (Media playlistMedia in playlistForm.Playlist)
                    {
                        if (File.Exists(playlistMedia.filePath))
                            arrSelectedMedia.Add(playlistMedia.filePath);
                    }

                    if (arrSelectedMedia.Count > 0)
                    {
                        //splashScreen mediaSplash = new splashScreen();

                        ////if (Form1.Mode == "Release")
                        ////{
                        //mediaSplash.SplashMessage2 = playlistForm.Text;

                        //mediaSplash.ShowDialog(this);

                        //SplashScreenNew.ShowSplashScreen();

                        //}

                        //this.Hide();
                        mediaWindow = createMediaWindow((Media)playlistForm.Playlist[0]);
                        //						mediaWindow.playVideo(selectedLabel.Tag.ToString());
                        mediaWindow.PlayMedia(arrSelectedMedia, "","");

                        //if (Form1.Mode == "Release")
                        //{
                        //mediaSplash.Close();
                        //SplashScreenNew.CloseForm();
                        //}

                        //mediaWindow.ShowDialog(this);
                        //mediaWindow.Show();

                        //mediaWindow.TopMost = true;
                    }
                    else
                    {
                        //Tools.WriteToFile(Tools.errorFile, "---- Missing File : " + selectedMedia + " ----");

                        //MessageBox.Show("Missing File - " + selectedMedia);
                    }
                }
            }
            else //close playlist
            {
                playlistForm.Close();
            }
        }

        private void searchToolStripButton_Click(object sender, EventArgs e)
        {
            if (pnlSearch.Visible)
            {
                pnlSearch.Visible = false;

                lvFF.Controls.Clear();
                lvFF.Items.Clear();

                //reload selected category
                displayCategory(scrollingTabsGenre.SelectedTabs, scrollingTabsAlphabet.SelectedTabs, scrollingTabsMediaTypes.SelectedTabs);
            }
            else
            {
                pnlSearch.Visible = true;
                pnlSearch.BringToFront();
                txtSearch1.Focus();

                //lvFF.Controls.Clear();
                //lvFF.Items.Clear();

                lvFF.Controls.Clear();
                lvFF.Items.Clear();

                Label lblNoResults = new Label();
                lblNoResults.Text = "No Results";
                lblNoResults.Top = 50;
                lblNoResults.Left = 50;
                lblNoResults.Width = 150;
                lblNoResults.BackColor = Color.White;
                //lvFF.Controls.Add(lblNoResults);
                lvFF.Controls.Add(lblNoResults);
            }
        }

        private void toolStripButtonAdvancedFilters_Click(object sender, EventArgs e)
        {
            //if (!DynamicMediaFilter.Visible)
            //{
            //    DynamicMediaFilter.FiltersUpdated += new SCTV.AdvancedMediaFilter.AdvancedMediaFilter.UpdateFilters(DynamicMediaFilter_FiltersUpdated);
            //    DynamicMediaFilter.Visible = true;
            //    DynamicMediaFilter.Dock = DockStyle.Top;

            //    DynamicMediaFilter.BringToFront();
            //}
            //else
            //    DynamicMediaFilter.Visible = false;

            //scrollingTabsAlphabet.Visible = !DynamicMediaFilter.Visible;
            //scrollingTabsGenre.Visible = !DynamicMediaFilter.Visible;
            //scrollingTabsMediaTypes.Visible = !DynamicMediaFilter.Visible;
            
            //volume1.BringToFront();
            //volume1.Visible = true;
        }

        #endregion

        void DynamicMediaFilter_FiltersUpdated(string SelectedGenres, string SelectedMediaTypes, string SelectedStartsWith)
        {
            applyAdvancedFilters(SelectedGenres, SelectedMediaTypes, SelectedStartsWith);
        }

        #region category context menu methods
        /// <summary>
        /// play selected category
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void playCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Media mediaToInsert = new Media();
            ArrayList arrSelectedMedia = new ArrayList();
            int timesPlayed = 0;

            foreach (ListViewItem AllItems in lvFF.Items)
            {
                mediaToInsert = (Media)AllItems.Tag;

                if (File.Exists(mediaToInsert.filePath))
                {
                    //update timesPlayed and lastPlayed
                    int.TryParse(mediaToInsert.TimesPlayed, out timesPlayed);
                    mediaToInsert.TimesPlayed = Convert.ToString(timesPlayed + 1);
                    mediaToInsert.LastPlayed = DateTime.Now.ToString();

                    myMedia.UpdateMediaInfo(mediaToInsert);

                    arrSelectedMedia.Add(mediaToInsert);
                }
            }

            if (arrSelectedMedia.Count > 0)
            {
                //splashScreen mediaSplash = new splashScreen();

                ////if (Form1.Mode == "Release")
                ////{
                //mediaSplash.SplashMessage2 = mediaToInsert.Title;

                //mediaSplash.ShowDialog(this);

                //SplashScreenNew.ShowSplashScreen();

                //}

                //this.Hide();
                mediaWindow = createMediaWindow(mediaToInsert);
                //						mediaWindow.playVideo(selectedLabel.Tag.ToString());
                mediaWindow.PlayMedia(arrSelectedMedia, "", "");

                //if (Form1.Mode == "Release")
                //{
                //mediaSplash.Close();
                //SplashScreenNew.CloseForm();
                //}

                //mediaWindow.ShowDialog(this);
                //mediaWindow.Show();

                //mediaWindow.TopMost = true;
            }
            else
            {
                Tools.WriteToFile(Tools.errorFile, "---- Missing File : " + mediaToInsert + " ----");

                MessageBox.Show("Missing File - " + mediaToInsert);
            }
        }

        private void editCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Coming Soon");
        }

        private void deleteCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Coming Soon");
        }
        #endregion

        #region item context menu methods
        /// <summary>
        /// play selected item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void playToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ArrayList checkedItems = new ArrayList();

            foreach (ListViewItem li in lvFF.Items)
            {
                if (li.Checked)
                    checkedItems.Add(li);
            }

            if (checkedItems.Count == 0)
            {
                if (lvFF.SelectedItems.Count > 0)
                    checkedItems.Add(lvFF.SelectedItems[0]);
            }

            if (((Media)((ListViewItem)checkedItems[0]).Tag).filePath.Trim().Length == 0)
                MessageBox.Show("You must select an individual episode to play");
            else
            {
                int timesPlayed = 0;

                foreach (ListViewItem selectedItem in checkedItems)
                {
                    Media media = (Media)selectedItem.Tag;

                    //update timesPlayed
                    int.TryParse(media.TimesPlayed, out timesPlayed);
                    media.TimesPlayed = Convert.ToString(timesPlayed + 1);
                    media.LastPlayed = DateTime.Now.ToString();
                    myMedia.UpdateMediaInfo(media);

                    playMedia(media);
                }
            }
        }

        private void showDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (((Media)lvFF.SelectedItems[0].Tag).filePath.Trim().Length == 0)
                    MessageBox.Show("You must select an individual episode to see details");
                else
                {
                    foreach (ListViewItem selectedItem in lvFF.SelectedItems)
                    {
                        if (selectedItem.Tag is Media)
                        {
                            if (toolTip != null)
                                toolTip.Close();

                            toolTip = new MediaToolTip((Media)selectedItem.Tag);
                            //toolTip.ActiveArea = new Rectangle(40, 10, 600, 400);
                            toolTip.TitleBarSize = 0;
                            toolTip.GlassOpacity = 1;
                            toolTip.Location = Cursor.Position;
                            toolTip.DisplayMaximize = false;
                            toolTip.DisplayMinimize = false;
                            toolTip.Show();
                        }

                        break;
                    }



                    ////get tabcontrol
                    //System.Windows.Forms.ListView selectedLV = (System.Windows.Forms.ListView)selectedControl;

                    ////get selected tab
                    //if (selectedLV.SelectedItems.Count > 0)
                    //{

                    //    //pop up imdbInfo to handle the edit
                    //    //SCTVObjects.MediaDetails mediaDetails = new SCTVObjects.MediaDetails((Media)lvFF.SelectedItems[0].Tag, MediaHandler.GetAllCategories(tcMediaTypes.SelectedTab.Text), MediaHandler.GetAllMediaTypes());
                    //    SCTVObjects.MediaDetails mediaDetails = new SCTVObjects.MediaDetails((Media)lvFF.SelectedItems[0].Tag, MediaHandler.GetAllCategories(scrollingTabsMediaTypes.SelectedTabs), MediaHandler.GetAllMediaTypes());
                    //    DialogResult result = mediaDetails.ShowDialog(this);

                    //    if (result == DialogResult.OK)
                    //    {
                    //        //update media file
                    //        MediaHandler.UpdateMediaInfo(mediaDetails.MediaToEdit);

                    //        //update in memory dataset
                    //        MediaHandler.GetMedia();

                    //        //display updated data
                    //        displayMedia();
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, "editToolStripMenuItem_Click error: " + ex.Message);
            }
        }

        /// <summary>
        /// Remove this media item from the current category
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removeFromCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ArrayList checkedItems = new ArrayList();

                foreach (ListViewItem li in lvFF.Items)
                {
                    if (li.Checked)
                        checkedItems.Add(li);
                }

                if(checkedItems.Count == 0)
                {
                    if(lvFF.SelectedItems.Count > 0)
                        checkedItems.Add(lvFF.SelectedItems[0]);
                }

                if (checkedItems.Count > 0)
                {
                    if (((Media)((ListViewItem)checkedItems[0]).Tag).filePath.Trim().Length == 0)
                        MessageBox.Show("You must select an individual episode to remove");
                    else
                    {
                        if (MessageBox.Show("Are you sure you want to remove the selected item from the current category?", "Remove from category?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            Media mediaDetails = (Media)((ListViewItem)checkedItems[0]).Tag;
                            string currentCategory = scrollingTabsGenre.SelectedTabs;

                            if (mediaDetails.category.IndexOf(currentCategory + " /") > -1)
                                mediaDetails.category = mediaDetails.category.Replace(currentCategory + " /", "");
                            else if (mediaDetails.category.IndexOf("/ " + currentCategory) > -1)
                                mediaDetails.category = mediaDetails.category.Replace("/ " + currentCategory, "");
                            else
                                mediaDetails.category = mediaDetails.category.Replace(currentCategory, "");

                            //update media file
                            myMedia.UpdateMediaInfo(mediaDetails);

                            //update in memory dataset
                            myMedia.GetMedia();

                            //display updated data
                            displayMedia();
                        }
                    }
                }
                else
                    MessageBox.Show("You must select something to remove");
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, "removeFromCategoryToolStripMenuItem_Click error: " + ex.Message);
            }
            
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ArrayList checkedItems = new ArrayList();

                foreach (ListViewItem li in lvFF.Items)
                {
                    if (li.Checked)
                        checkedItems.Add(li);
                }

                if (checkedItems.Count == 0)
                {
                    if (lvFF.SelectedItems.Count > 0)
                        checkedItems.Add(lvFF.SelectedItems[0]);
                }

                if (((Media)((ListViewItem)checkedItems[0]).Tag).filePath.Trim().Length == 0)
                    MessageBox.Show("You must select an individual episode to delete");
                else
                {
                    myMedia.DeleteMedia((Media)((ListViewItem)checkedItems[0]).Tag);

                    string mediaTitle = ((Media)((ListViewItem)checkedItems[0]).Tag).Title;

                    //display updated data
                    displayMedia();

                    MessageBox.Show(mediaTitle + Environment.NewLine + "Deleted Successfully");
                }
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, "deleteToolStripMenuItem_Click error: " + ex.Message);
            }
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                //ToolStripMenuItem contextMenu = (ToolStripMenuItem)sender;

                //edit selected control
                //if (contextMenu.Owner.Name == "EditcontextMenu")//media was selected
                //{

                ArrayList checkedItems = new ArrayList();

                foreach (ListViewItem li in lvFF.Items)
                {
                    if (li.Checked)
                        checkedItems.Add(li);
                }

                if (checkedItems.Count == 0)
                {
                    if (lvFF.SelectedItems.Count > 0)
                        checkedItems.Add(lvFF.SelectedItems[0]);
                }

                //get selected items
                if (checkedItems.Count > 0)
                {
                    if (((Media)((ListViewItem)checkedItems[0]).Tag).filePath.Trim().Length == 0)
                        MessageBox.Show("You must select an individual episode to edit");
                    else if(checkedItems.Count == 1)
                    {
                        //pop up imdbInfo to handle the edit
                        SCTVObjects.MediaDetails mediaDetails = new SCTVObjects.MediaDetails((Media)((ListViewItem)checkedItems[0]).Tag, myMedia.GetAllCategories(scrollingTabsMediaTypes.SelectedTabs), myMedia.GetAllMediaTypes());
                        mediaDetails.StartPosition = FormStartPosition.CenterScreen;
                        //mediaDetails.GlassOpacity = .9;
                        mediaDetails.Show();

                        DialogResult result = mediaDetails.DialogResult;

                        if (result == DialogResult.OK)
                        {
                            //display updated data
                            displayMedia();
                        }
                    }
                    else //edit multiple media items
                    {
                        ArrayList checkedMedia = new ArrayList();

                        foreach (ListViewItem item in checkedItems)
                            checkedMedia.Add(item.Tag);

                        //pop up window to handle the multiple item edit
                        SCTVObjects.MediaDetailsMultiple mediaDetails = new SCTVObjects.MediaDetailsMultiple(checkedMedia, myMedia.GetAllCategories(scrollingTabsMediaTypes.SelectedTabs), myMedia.GetAllMediaTypes());
                        //mediaDetails.GlassOpacity = .9;
                        mediaDetails.ClearIMDBNum = true;
                        mediaDetails.AutoUpdate = true;
                        mediaDetails.StartPosition = FormStartPosition.CenterScreen;
                        mediaDetails.Show();

                        DialogResult result = mediaDetails.DialogResult;

                        if (result == DialogResult.OK)
                        {
                            //display updated data
                            displayMedia();
                        }
                    }
                }
                //}
                //else if (contextMenu.Name == "categoryContextMenu")//category tab was selected
                //{
                //    //get tabcontrol
                //    TabControl selectedTC = (TabControl)sender;

                //    //get selected tab
                //    TabPage selectedTP = selectedTC.SelectedTab;

                //    if (selectedTP != null)
                //    {
                //        //show edit box
                //        //EditForm editForm = new EditForm(selectedTP.Text);
                //        //editForm.ShowDialog(this);

                //        //update selected tab
                //        //selectedTP.Text = editForm.Value;
                //    }
                //}
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, "editToolStripMenuItem_Click error: " + ex.Message);
            }
        }

        /// <summary>
        /// Search for updated media info giving the user choices on multiple results
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void advancedInfoSearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ArrayList checkedItems = new ArrayList();

                foreach (ListViewItem li in lvFF.Items)
                {
                    if (li.Checked)
                        checkedItems.Add(li);
                }

                if (checkedItems.Count == 0)
                {
                    if (lvFF.SelectedItems.Count > 0)
                        checkedItems.Add(lvFF.SelectedItems[0]);
                }

                if (checkedItems.Count > 0)
                {
                    if (((Media)lvFF.SelectedItems[0].Tag).filePath.Trim().Length == 0)
                        MessageBox.Show("You must select an individual episode to search");
                    else if(checkedItems.Count == 1)
                    {
                        //pop up imdbInfo to handle the edit
                        SCTVObjects.MediaDetails mediaDetails = new SCTVObjects.MediaDetails((Media)lvFF.SelectedItems[0].Tag, myMedia.GetAllCategories(scrollingTabsMediaTypes.SelectedTabs), myMedia.GetAllMediaTypes());
                        //mediaDetails.GlassOpacity = .9;
                        mediaDetails.StartPosition = FormStartPosition.CenterScreen;
                        mediaDetails.AutoUpdate = true;
                        mediaDetails.Show();

                        //update in memory dataset
                        //MediaHandler.GetMedia();

                        //display updated data
                        displayMedia();



                        ////pop up imdbInfo to handle the search
                        //IMDBInfo imdbInfo = new IMDBInfo();
                        //imdbInfo.MediaToSearchFor = (Media)lvFF.SelectedItems[0].Tag;
                        //DialogResult result = imdbInfo.ShowDialog(this);

                        //if (result == DialogResult.OK)
                        //{
                        //    //update media file
                        //    MediaHandler.UpdateMediaInfo(imdbInfo.FoundMedia);

                        //    //update in memory dataset
                        //    MediaHandler.GetMedia();

                        //    //display updated data
                        //    displayMedia();
                        //}
                    }
                    else //edit multiple media items
                    {
                        ArrayList checkedMedia = new ArrayList();

                        foreach (ListViewItem item in checkedItems)
                            checkedMedia.Add(item.Tag);

                        //pop up window to handle the multiple item edit
                        SCTVObjects.MediaDetailsMultiple mediaDetails = new SCTVObjects.MediaDetailsMultiple(checkedMedia, myMedia.GetAllCategories(scrollingTabsMediaTypes.SelectedTabs), myMedia.GetAllMediaTypes());
                        //mediaDetails.GlassOpacity = .9;
                        mediaDetails.ClearIMDBNum = true;
                        mediaDetails.AutoUpdate = true;
                        mediaDetails.StartPosition = FormStartPosition.CenterScreen;
                        mediaDetails.Show();

                        DialogResult result = mediaDetails.DialogResult;

                        if (result == DialogResult.OK)
                        {
                            //display updated data
                            displayMedia();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, "advancedInfoSearchToolStripMenuItem_Click: " + ex.Message);
            }
        }

        private void addToPlaylistToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ArrayList checkedItems = new ArrayList();

            foreach (ListViewItem li in lvFF.Items)
            {
                if (li.Checked)
                    checkedItems.Add(li);
            }

            if (checkedItems.Count == 0)
            {
                if (lvFF.SelectedItems.Count > 0)
                    checkedItems.Add(lvFF.SelectedItems[0]);
            }

            foreach (ListViewItem li in checkedItems)
                if (li.Tag is Media)
                {
                    myMedia.AddToPlaylist((Media)li.Tag);
                    //playlist.Add((Media)li.Tag);
                }
                else
                    MessageBox.Show("Type not supported yet");
        }
        #endregion

        private void btnSearch_Click(object sender, EventArgs e)
        {
            search(txtSearch1.Text);
        }

        private void search(string stringToSearchFor)
        {
            if (stringToSearchFor.Trim().Length > 0)
            {
                btnCancelSearch.Visible = true;
                lvFF.Controls.Clear();
                lvFF.Items.Clear();

                dvMedia = myMedia.SearchByTitle(stringToSearchFor);

                if (dvMedia.Count > 0)
                    displayCategory(dvMedia, "", "");
                else
                {
                    Label lblNoResults = new Label();
                    lblNoResults.Text = "No Results";
                    lblNoResults.Top = 50;
                    lblNoResults.Left = 50;
                    lblNoResults.Width = 150;
                    lblNoResults.BackColor = Color.White;
                    //lvFF.Controls.Add(lblNoResults);

                    lvFF.Controls.Add(lblNoResults);
                }
            }
            else
                MessageBox.Show("Enter a title to search for");
        }

        /// <summary>
        /// execute given phrase
        /// </summary>
        /// <param name="phrase"></param>
        public void executePhrase(Phrase phrase)
        {
            if (phrase.Accuracy > 10)
            {
                //check for media rule
                if (phrase.RuleName == "media")
                {
                    string mediaVerbs = "";
                    string mediaName = phrase.phrase;
                    int spaceIndex = 0;
                    int numElements = phrase.NumberOfElements;
                    //remove children items from phrase
                    for (int x = 0; x < numElements; x++)
                    {
                        spaceIndex = mediaName.IndexOf(" ");
                        mediaVerbs += mediaName.Substring(0, spaceIndex) + " ";
                        mediaName = mediaName.Substring(spaceIndex).Trim();
                    }

                    switch (mediaVerbs.ToLower().Trim())
                    {
                        case "play":
                            //look at categories and see if we have a match
                            if (categories.Contains(mediaName))
                            {
                                //play media in category
                                //playCategory(mediaName);
                            }
                            else
                            {
                                //scrollToMedia(mediaName);
                                //playMedia();
                            }
                            break;
                        case "scroll to":
                        case "show me":
                        case "show":
                        case "find":
                        case "display":
                        default:
                            //look at categories and see if we have a match
                            //if (isCategory(mediaName))
                            if (categories.Contains(mediaName))
                            {
                                //scroll to category
                                //scrollToCategory(mediaName);
                            }
                            else
                            {
                                try
                                {
                                    //scroll to media
                                    //scrollToMedia(mediaName);
                                }
                                catch (Exception ex)
                                {

                                    throw;
                                }
                            }
                            break;
                    }
                }
                else
                {
                    switch (phrase.phrase)
                    {
                        case "left":
                        case "right":
                        case "up":
                        case "down":
                            //EventLog.WriteEntry("sctv", "executePhrase library " + phrase.phrase);
                            //executeMacros(phrase.phrase);
                            break;
                        case "scroll left":
                        case "scroll right":
                        case "scroll up":
                        case "scroll down":
                            //scrollDirection = phrase.phrase.Replace("scroll ", "");
                            //scrollTimer.Enabled = true;
                            break;
                        case "stop":
                        case "scroll stop":
                            //scrollTimer.Enabled = false;
                            break;
                        case "play":
                            //executeMacros("Play/Pause");//play selected file
                            break;
                        default:
                            EventLog.WriteEntry("sctv", "Couldn't execute phrase " + phrase.phrase);
                            break;
                    }
                }
            }
            else
                EventLog.WriteEntry("sctv", "sctv accuracy= " + phrase.Accuracy.ToString() + " phrase " + phrase.phrase);
        }

        /// <summary>
        /// executes macros immediately
        /// </summary>
        /// <param name="macroName"></param>
        private void executeMacros(string macroName)
        {
            try
            {
                //Tools.WriteToFile(Tools.errorFile,"calling macro in liquidMedia " + macroName);
                //keyStrokeTracker.Clear();
                switch (macroName)
                {
                    case "mute":
                    case "volume down":
                    case "volume up":
                        speakers.Volume(macroName);

                        //pbVolume.Value = speakers.GetVolume();
                        break;
                }
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, "executeMacros error: " + ex.Message);
            }
        }

        private void JustinTVToolStripButton_Click(object sender, EventArgs e)
        {
            SCTVJustinTV.JustinTV justinTV = new SCTVJustinTV.JustinTV("");

            lvFF.Controls.Clear();
            lvFF.Items.Clear();

            ListViewGroup lvig = new ListViewGroup("JustinTV");
            lvFF.Groups.Add(lvig);

            foreach (OnlineChannel channel in justinTV.Channels)
            {
                ListViewItem listViewItem1 = new ListViewItem();
                listViewItem1.Text = channel.ShowTitle;
                listViewItem1.Group = lvig;
                listViewItem1.ToolTipText = channel.ChannelTitle;
                listViewItem1.Tag = channel;
                lvFF.Items.Add(listViewItem1);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            pnlSearch.Visible = false;

            lvFF.Controls.Clear();
            lvFF.Items.Clear();

            //reload selected category
            displayCategory(scrollingTabsGenre.SelectedTabs, scrollingTabsAlphabet.SelectedTabs, scrollingTabsMediaTypes.SelectedTabs);
        }

        private void RecordToolStripButton_Click(object sender, EventArgs e)
        {
            DriveInfo driveInfo = new DriveInfo("d");
            string discName = driveInfo.VolumeLabel;

            discName = System.Text.RegularExpressions.Regex.Replace(discName, @"\W*", "");

            //recordRemoveableMedia("D", Application.ExecutablePath + "\\video\\" + discName + ".CEL", false);
            recordRemoveableMedia("D", Application.ExecutablePath + "\\video\\" + discName + ".avi", false);
        }

        private void playRemoveableMedia(string driveLetter, bool skipMenu)
        {
            playRemoveableMedia(driveLetter, "", skipMenu);
        }

        private void playRemoveableMedia(string driveLetter, string fileToRecordTo, bool skipMenu)
        {
            //make sure mediaplayer exists
            liquidMediaPlayer mediaPlayer = new liquidMediaPlayer();
            mediaPlayer.PlayRemoveableMedia(driveLetter, fileToRecordTo, skipMenu);

            mediaPlayer.ShowDialog();
        }

        private void recordRemoveableMedia(string driveLetter, string fileToRecordTo, bool skipMenu)
        {
            //make sure mediaplayer exists

            liquidMediaPlayer mediaPlayer = new liquidMediaPlayer();
            mediaPlayer.RecordRemoveableMedia(driveLetter, fileToRecordTo, skipMenu);

            mediaPlayer.ShowDialog();

            mediaPlayer.TopMost = true;
        }

        private void btnClosePicture_Click(object sender, EventArgs e)
        {
            pbPictures.Visible = false;
            btnClosePicture.Visible = false;
            pbPictures.ImageLocation = "";
        }

        private void toolStripButtonDVD_Click(object sender, EventArgs e)
        {
            string driveLetter = "E:";
            System.IO.DriveInfo[] drives = System.IO.DriveInfo.GetDrives();

            foreach (System.IO.DriveInfo drive in drives)
            {
                if (drive.DriveType == System.IO.DriveType.CDRom)
                    driveLetter = drive.RootDirectory.FullName.Substring(0,drive.RootDirectory.FullName.IndexOf(":")+1);

                //Floppies will appear as DriveType.Removable, same as USB drives.
            }

            bool skipDVDMenu = false;
            string discName;
            
            DriveInfo driveInfo;
            InsertedMedia insertedMedia = new InsertedMedia();

            driveInfo = new DriveInfo(driveLetter);

            bool.TryParse(System.Configuration.ConfigurationManager.AppSettings["Media.SkipDVDMenu"], out skipDVDMenu);
            insertedMedia.SkipMenu = skipDVDMenu;
            insertedMedia.Drive = driveInfo;
            insertedMedia.ShowDialog(this);

            discName = insertedMedia.MediaName;

            switch (insertedMedia.MediaState)
            {
                case MediaStateEnum.Play:
                    playRemoveableMedia(driveLetter, insertedMedia.SkipMenu);
                    break;
                case MediaStateEnum.PlayAndRecord:
                    //playRemoveableMedia(driveLetter, defaultPathToSaveTo + "\\" + discName + ".CEL", insertedMedia.SkipMenu);
                    playRemoveableMedia(driveLetter, defaultPathToSaveTo + "\\" + discName + ".avi", insertedMedia.SkipMenu);
                    break;
                case MediaStateEnum.Record:
                    //recordRemoveableMedia(driveLetter, defaultPathToSaveTo + "\\" + discName + ".CEL", insertedMedia.SkipMenu);
                    recordRemoveableMedia(driveLetter, defaultPathToSaveTo + "\\" + discName + ".avi", insertedMedia.SkipMenu);
                    break;
            }
        }

        private void showSequelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ArrayList checkedItems = new ArrayList();

                foreach (ListViewItem li in lvFF.Items)
                {
                    if (li.Checked)
                        checkedItems.Add(li);
                }

                if (checkedItems.Count == 0)
                {
                    if (lvFF.SelectedItems.Count > 0)
                        checkedItems.Add(lvFF.SelectedItems[0]);
                }

                if (((Media)((ListViewItem)checkedItems[0]).Tag).filePath.Trim().Length == 0)
                    MessageBox.Show("You must select an individual episode to show a sequel");
                else
                {
                    foreach (ListViewItem selectedItem in checkedItems)
                    {
                        Media media = (Media)((ListViewItem)checkedItems[0]).Tag;
                        Media sequel = myMedia.GetSequel(media, true);

                        if (sequel != null)
                            MessageBox.Show(sequel.Title);
                        else
                            MessageBox.Show("Didn't find sequel");

                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, "showSequelToolStripMenuItem_Click error: " + ex.Message);
            }
        }

        private void removeAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for(int x = 1; x < toolStripDynamicFilter.Items.Count; x++)
                toolStripDynamicFilter.Items.RemoveAt(x);
        }

        /// <summary>
        /// Handle clicks on the media types dropdown list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mediaTypeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripDropDownButton newItem = new ToolStripDropDownButton();
            newItem.Text = ((ToolStripDropDownItem)sender).Text;
            newItem.Tag = ((ToolStripDropDownItem)sender).OwnerItem.Text;
            newItem.DropDownItems.Add("Delete", null, deleteItem_click);

            toolStripDynamicFilter.Items.Add(newItem);

            applyAdvancedFilters();
        }

        /// <summary>
        /// Handle clicks on the genre dropdown list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void genreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripDropDownButton newItem = new ToolStripDropDownButton();
            newItem.Text = ((ToolStripDropDownItem)sender).Text;
            newItem.Tag = ((ToolStripDropDownItem)sender).OwnerItem.Text;
            newItem.DropDownItems.Add("Delete",null,deleteItem_click);

            toolStripDynamicFilter.Items.Add(newItem);

            applyAdvancedFilters();
        }

        /// <summary>
        /// Handle clicks on the alpha/numeric dropdown list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void alphaNumericToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripDropDownButton newItem = new ToolStripDropDownButton();
            newItem.Text = ((ToolStripDropDownItem)sender).Text;
            newItem.Tag = ((ToolStripDropDownItem)sender).OwnerItem.Text;
            newItem.DropDownItems.Add("Delete", null, deleteItem_click);

            toolStripDynamicFilter.Items.Add(newItem);

            applyAdvancedFilters();
        }

        private void setAdvancedGenreFilters(ArrayList mediaTypes)
        {
            ArrayList allGenres = new ArrayList();

            foreach (string type in mediaTypes)
                allGenres = setAdvancedGenreFilters(type, false);

            allGenres.Sort();

            //set genre
            foreach (string tempCat in allGenres)
                genreToolStripMenuItem1.DropDownItems.Add(tempCat, null, genreToolStripMenuItem_Click);
        }

        private ArrayList setAdvancedGenreFilters(string mediaType, bool addToToolStripMenu)
        {
            //get genre
            string correctCase = "";

            //display categories
            categories = new ArrayList();

            foreach (DataRowView drv in myMedia.GetAllCategories(mediaType))
            {
                string[] catArray;
                string categoryString = drv["category"].ToString();

                if (categoryString.Contains("/"))
                    catArray = categoryString.Split('/');
                else
                    catArray = categoryString.Split('|');

                for (int x = 0; x < catArray.Length; x++)
                {
                    correctCase = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(catArray[x].Trim());
                    if (!categories.Contains(correctCase) && correctCase.Length > 0)//keep out duplicates
                        categories.Add(correctCase);
                }
            }

            //add internet before sort so it will be sorted
            if (mediaType.ToLower() == "online")
                categories.Add("Internet");

            categories.Sort();//alphabetize

            //add utility pages to categories
            if (mediaType.ToLower() != "online")
            {
                if(!categories.Contains("New"))
                    categories.Insert(0, "New");

                if (!categories.Contains("Recent"))
                    categories.Insert(1, "Recent");

                if (!categories.Contains("Star Rating"))
                    categories.Insert(2, "Star Rating");

                if (!categories.Contains("Misc"))
                    categories.Insert(3, "Misc");

                if (!categories.Contains("Popular"))
                    categories.Insert(4, "Popular");

                if (!categories.Contains("Not-So Popular"))
                    categories.Insert(5, "Not-So Popular");

                if (!categories.Contains("All"))
                    categories.Insert(6, "All");
            }
            //else if (mediaType.ToLower() == "online")
            //{
            //    if (!categories.Contains("Home"))
            //        categories.Insert(0, "Home");
            //}

            //if (!categories.Contains("Playlist"))
            //    categories.Insert(1, "Playlist");


            if (addToToolStripMenu)
            {
                //set genre
                foreach (string tempCat in categories)
                    genreToolStripMenuItem1.DropDownItems.Add(tempCat, null, genreToolStripMenuItem_Click);
            }

            return categories;
        }

        private void deleteItem_click(object sender, EventArgs e)
        {
            toolStripDynamicFilter.Items.Remove(((ToolStripMenuItem)sender).OwnerItem);

            applyAdvancedFilters();
        }

        /// <summary>
        /// display media according to what filters are selected
        /// </summary>
        private void applyAdvancedFilters()
        {
            string selectedGenres = "";
            string selectedMediaTypes = "";
            string selectedStartsWith = "";

            foreach (ToolStripItem filter in toolStripDynamicFilter.Items)
            {
                if (filter.Text != "Filters" && filter.Text.Length > 0)
                {
                    switch (filter.Tag.ToString())
                    {
                        case "Genre":
                            if (selectedGenres.Length > 0)
                                selectedGenres = selectedGenres + "|";

                            selectedGenres += filter.Text;
                            break;
                        case "Media Type":
                            if (selectedMediaTypes.Length > 0)
                                selectedMediaTypes = selectedMediaTypes + "|";

                            selectedMediaTypes += filter.Text;
                            break;
                        case "Alpha/Numeric":
                            selectedStartsWith = filter.Text;
                            break;
                    }
                }
            }

            //display filtered media
            DialogResult result = DialogResult.OK;

            if (selectedGenres.Length == 0)
                selectedGenres = "All";

            if (selectedStartsWith.Length == 0)
                selectedStartsWith = "All";

            if (selectedStartsWith == "All" && selectedGenres == "All")
                result = MessageBox.Show("You are about to display \"All\" of your media, this may take several minutes." + Environment.NewLine + "Do you wish to continue?", "Selecting All media", MessageBoxButtons.OKCancel);

            if (result == DialogResult.Cancel)
            {
                //force "no results" display
                selectedGenres = "";
                selectedStartsWith = "";
                selectedMediaTypes = "";
            }

            lvFF.Controls.Clear();
            lvFF.Items.Clear();

            displayCategory(selectedGenres, selectedStartsWith, selectedMediaTypes);
        }

        /// <summary>
        /// display media according to what filters are selected
        /// </summary>
        private void applyAdvancedFilters(string SelectedGenres, string SelectedMediaTypes, string SelectedStartsWith)
        {
            //display filtered media
            DialogResult result = DialogResult.OK;

            if (SelectedGenres.Length == 0)
                SelectedGenres = "All";

            if (SelectedStartsWith.Length == 0)
                SelectedStartsWith = "All";

            if (SelectedStartsWith == "All" && SelectedGenres == "All")
                result = MessageBox.Show("You are about to display \"All\" of your media, this may take several minutes." + Environment.NewLine + "Do you wish to continue?", "Selecting All media", MessageBoxButtons.OKCancel);

            if (result == DialogResult.Cancel)
            {
                //force "no results" display
                SelectedGenres = "";
                SelectedStartsWith = "";
                SelectedMediaTypes = "";
            }

            //clear lvFF
            lvFF.Controls.Clear();
            lvFF.Items.Clear();

            displayCategory(SelectedGenres, SelectedStartsWith, SelectedMediaTypes);
        }

        //private void lvMedia_DrawItem(object sender, DrawListViewItemEventArgs e)
        //{
        //    //check to see if item is the last item in the listview and add more if necessary and available

        //    if ((e.State & ListViewItemStates.Selected) != 0)
        //    {
        //        // Draw the background and focus rectangle for a unselected item.
        //        //e.Graphics.FillRectangle(Brushes.Maroon, e.Bounds);
        //        e.DrawFocusRectangle();
        //    }
        //    else
        //    {
        //        // Draw the background for a selected item.

        //        using (LinearGradientBrush brush =
        //            new LinearGradientBrush(e.Bounds, Color.AliceBlue,
        //            Color.Blue, LinearGradientMode.Horizontal))
        //        {
        //            e.Graphics.FillRectangle(brush, e.Bounds);
        //            //e.Graphics.FillEllipse(brush, e.Bounds);
        //        }
        //    }

        //    // Draw the item text for views other than the Details view.
        //    if (lvFF.View != View.Details)
        //    {
        //        int textHeight = (int)(e.Graphics.MeasureString("WWW", lvFF.Font).Height + 4);
        //        //e.Graphics.DrawImage(System.Drawing.Image.FromFile(((Media)e.Item.Tag).coverImage), new Point(e.Item.Bounds.Top + 2, e.Item.Bounds.Left + 2));//   e.Item.ImageList.Images[e.Item.ImageIndex],e.Bounds);
        //        //e.Graphics.DrawImage(System.Drawing.Image.FromFile(((Media)e.Item.Tag).coverImage), new Point(e.Item.Bounds.X + 10, e.Item.Bounds.Y + textHeight));//   e.Item.ImageList.Images[e.Item.ImageIndex],e.Bounds);

        //        //Rectangle destRect = new Rectangle(e.Item.Bounds.X + 10, e.Item.Bounds.Y + textHeight, 87, 140 - textHeight);
        //        Rectangle destRect = new Rectangle(e.Item.Bounds.X + 10, e.Item.Bounds.Y + 10, 87, 140);
        //        string coverImagePath = "";



        //        if (e.Item.Tag is Media)
        //            coverImagePath = ((Media)e.Item.Tag).coverImage;
        //        else if (e.Item.Tag is OnlineMediaType)
        //            coverImagePath = ((OnlineMediaType)e.Item.Tag).CoverImage;

        //        if (!File.Exists(coverImagePath))
        //            coverImagePath = Application.StartupPath + "\\images\\media\\coverImages\\notavailable.jpg";

        //        e.Graphics.DrawImage(System.Drawing.Image.FromFile(coverImagePath), destRect);//   e.Item.ImageList.Images[e.Item.ImageIndex],e.Bounds);

        //        e.DrawText();
        //    }

        //    if (e.ItemIndex >= (lvFF.Items.Count - 10))
        //    {
        //        //this is the last item - load another set of items if available
        //        if (dtRemainingResults != null && dtRemainingResults.Rows.Count > 0)
        //        {
        //            displayCategory(dtRemainingResults.DefaultView, scrollingTabsGenre.SelectedTabs, scrollingTabsMediaTypes.SelectedTabs, false);
        //        }
        //    }
        //}

        private void lvFF_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            //check to see if item is the last item in the listview and add more if necessary and available

            int textHeight = (int)(e.Graphics.MeasureString("WWW", lvFF.Font).Height + 4);
            Brush myBrush = Brushes.Black;
            Rectangle rectBottom;
            Rectangle rectBackground;

            if ((e.State & ListViewItemStates.Selected) != 0)
            {
                // Draw the background and focus rectangle for a unselected item.
                //e.Graphics.FillRectangle(Brushes.Maroon, e.Bounds);
                
                //e.DrawFocusRectangle();

                //draw background gradient
                //LinearGradientBrush bgBrush = null;

                //if (e.Bounds.Width > 0 && e.Bounds.Height > 0)
                //{
                //    rectBackground = new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width + 4, e.Bounds.Height - 35);
                //    rectBottom = new Rectangle(e.Bounds.X, rectBackground.Location.Y + rectBackground.Height - 2, e.Bounds.Width + 4, e.Item.Bounds.Height - rectBackground.Height);

                //    //check to see if this item is the last one on the row and extend it's background to the right edge
                //    if (e.Item.Bounds.X + (e.Item.Bounds.Width * 2) >= lvFF.Width)
                //    {
                //        rectBackground.Width = rectBackground.Width * 2;
                //        rectBottom.Width = rectBottom.Width * 2;
                //    }

                //    bgBrush = new LinearGradientBrush(rectBackground, Color.Transparent, Color.SlateGray, LinearGradientMode.Vertical);

                //    if (bgBrush != null)
                //    {
                //        using (bgBrush)
                //        {
                //            e.Graphics.FillRectangle(bgBrush, rectBackground);
                //            //e.Graphics.FillEllipse(brush, e.Bounds);
                //        }
                //    }

                //    bgBrush = new LinearGradientBrush(rectBottom, Color.SlateGray, Color.Transparent, LinearGradientMode.Vertical);

                //    if (bgBrush != null)
                //        using (bgBrush)
                //            e.Graphics.FillRectangle(bgBrush, rectBottom);
                //}
            }
            else
            {
                // Draw the background for a selected item.

                string rating = "";

                if(e.Item.Tag is Media)
                    rating = ((Media)e.Item.Tag).Rating.ToLower();

                LinearGradientBrush bgBrush = null;

                if (rating.Contains("pg-13") || rating.Contains("nc-17"))//blue background
                {
                    bgBrush = new LinearGradientBrush(e.Bounds, Color.AliceBlue, Color.LightBlue, LinearGradientMode.Horizontal);
                }
                else if (rating == "pg" || rating == "g")//green background
                {
                    bgBrush = new LinearGradientBrush(e.Bounds, Color.OldLace, Color.LightGreen, LinearGradientMode.Horizontal);
                }
                else if (rating == "r")//red background
                {
                    bgBrush = new LinearGradientBrush(e.Bounds, Color.OldLace, Color.LightSalmon, LinearGradientMode.Horizontal);
                }
                else//purple background
                {
                    bgBrush = new LinearGradientBrush(e.Bounds, Color.OldLace, Color.MediumPurple, LinearGradientMode.Horizontal);
                }

                if (bgBrush != null)
                {
                    using (bgBrush)
                        e.Graphics.FillRectangle(bgBrush, e.Bounds);
                }

                if (e.Item.Bounds.Width > 0 && e.Item.Bounds.Height > 0)
                {
                    //draw media info
                    Rectangle rec = e.Item.Bounds;
                    rec.Height = e.Item.Bounds.Height - textHeight;
                    rec.Width = e.Item.Bounds.Width - 97;
                    rec.X = rec.X + 100;
                    rec.Y = rec.Y + textHeight;

                    e.Graphics.DrawString(e.Item.ToolTipText, this.Font, myBrush, rec, StringFormat.GenericDefault);

                    //draw option links
                    //rec.Height = 20;
                    //rec.Y = rec.Y + rec.Height + 1;

                    //e.Graphics.DrawString("more...", this.Font, myBrush, rec, StringFormat.GenericDefault);
                }
            }

            // Draw the item text for views other than the Details view.
            if (lvFF.View != View.Details)
            {
                Rectangle coverImageRect = new Rectangle(e.Item.Bounds.X + 10, e.Item.Bounds.Y + textHeight, 87, 140);
                string coverImagePath = "";
                
                if (e.Item.Tag is Media)
                {
                    coverImagePath = ((Media)e.Item.Tag).coverImage;
                }
                else if (e.Item.Tag is OnlineMediaType)
                    coverImagePath = ((OnlineMediaType)e.Item.Tag).CoverImage;

                if (!File.Exists(coverImagePath))
                    coverImagePath = Application.StartupPath + "\\images\\media\\coverImages\\notavailable.jpg";

                System.Drawing.Image coverImage = System.Drawing.Image.FromFile(coverImagePath);

                //System.Drawing.Image tmpImage = new Bitmap(coverImage,coverImage.Width, coverImage.Height);

                //draw cover image outline
                try
                {
                    if (!e.Item.Checked)
                        using (Graphics g = Graphics.FromImage(coverImage))
                            g.DrawRectangle(Pens.Black, 0, 0, coverImage.Width - 1, coverImage.Height - 1);
                    else
                    {
                        Pen widePen = new Pen(Color.Yellow, 10);

                        using (Graphics g = Graphics.FromImage(coverImage))
                            g.DrawRectangle(widePen, 0, 0, coverImage.Width - 1, coverImage.Height - 1);
                    }
                }
                catch (Exception ex)
                {
                    Tools.WriteToFile(ex);
                }

                //e.Graphics.DrawRectangle(Pens.Black, 0, 0, tmpImage.Width - 1, tmpImage.Height - 1);

                //using (Graphics g = e.Graphics)
                //    g.DrawRectangle(Pens.Black, 0, 0, tmpImage.Width - 1, tmpImage.Height - 1);

                //draw cover image
                e.Graphics.DrawImage(coverImage, coverImageRect);

                //draw mirror image
                //if (rectBottom != null)
                //{
                //    System.Drawing.Imaging.ColorMatrix matrix = new System.Drawing.Imaging.ColorMatrix();
                //    matrix.Matrix33 = 0.3f; //opacity 0 = completely transparent, 1 = completely opaque

                //    System.Drawing.Imaging.ImageAttributes attributes = new System.Drawing.Imaging.ImageAttributes();
                //    attributes.SetColorMatrix(matrix, System.Drawing.Imaging.ColorMatrixFlag.Default, System.Drawing.Imaging.ColorAdjustType.Bitmap);

                //    if (coverImageRect.Width > 0 && coverImageRect.Height > 0)
                //    {
                //        Rectangle coverImageMirrorRectDest = new Rectangle(coverImageRect.X, coverImageRect.Y + coverImageRect.Height - 2, coverImageRect.Width, 25);
                //        Rectangle coverImageMirrorRectSrc = new Rectangle(coverImageRect.X, coverImageRect.Height - coverImageMirrorRectDest.Height, coverImageRect.Width, coverImageMirrorRectDest.Height);
                //        System.Drawing.Image coverImageMirror = System.Drawing.Image.FromFile(coverImagePath);
                //        coverImageMirror.RotateFlip(RotateFlipType.RotateNoneFlipY);

                //        e.Graphics.DrawImage(coverImageMirror, coverImageMirrorRectDest, 0, 0, coverImageMirror.Width, coverImageMirror.Height, GraphicsUnit.Pixel, attributes);
                //    }
                //}

                bool value = false;
                value = e.Item.Checked;

                if(e.Item.Tag is Media && ((Media)e.Item.Tag).MediaType.ToLower() != "games")
                    CheckBoxRenderer.DrawCheckBox(e.Graphics, new Point(e.Bounds.Left, e.Bounds.Top), 
                        value ? System.Windows.Forms.VisualStyles.CheckBoxState.CheckedNormal :
                            System.Windows.Forms.VisualStyles.CheckBoxState.UncheckedNormal);

                //if (e.Item.Selected)
                //    e.Item.Selected = true;

                e.DrawText();
            }

            if (e.ItemIndex >= (lvFF.Items.Count - 5))
            {
                //this is the last item - load another set of items if available
                if (dtRemainingResults != null && dtRemainingResults.Rows.Count > 0)
                {
                    displayCategory(dtRemainingResults.DefaultView, scrollingTabsGenre.SelectedTabs, scrollingTabsMediaTypes.SelectedTabs, false);
                }
            }
        }

        private void lvFF_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //HandleClickEvents();
        }

        private void lvFF_MouseClick(object sender, MouseEventArgs e)
        {
            HandleClickEvents(sender,e);

            //don't know if I need the below line or not
            //Point localPoint = listView.PointToClient(mousePosition);

            //ListViewItem lvItem = lvFF.GetItemAt(e.X, e.Y);
            //Rectangle itemOptions = new Rectangle(lvItem.Bounds.X + 100, lvItem.Bounds.Y + lvItem.Bounds.Height - 20, lvItem.Bounds.Width - 100, 20);

            //if (itemOptions.Contains(e.Location))
            //{
            //    //the below code needs to be executed by a link in the details of an item

            //    if (e.Button == MouseButtons.Left)
            //    {
            //        foreach (ListViewItem selectedItem in lvFF.SelectedItems)
            //        {
            //            if (selectedItem.Tag is Media)
            //            {
            //                if (toolTip != null)
            //                    toolTip.Close();

            //                toolTip = new MediaToolTip((Media)selectedItem.Tag);
            //                //toolTip.ActiveArea = new Rectangle(40, 10, 600, 400);
            //                toolTip.TitleBarSize = 0;
            //                toolTip.GlassOpacity = 1;
            //                toolTip.Location = Cursor.Position;
            //                toolTip.DisplayMaximize = false;
            //                toolTip.DisplayMinimize = false;
            //                toolTip.Show();
            //            }
            //        }
            //    }
            //}
        }

        private Rectangle GetCheckBoxRectangle(ListViewItem selectedItem)
        {
            Rectangle result = Rectangle.Empty;

            using (Graphics g = this.CreateGraphics())
            {
                Size sz = CheckBoxRenderer.GetGlyphSize(g, CheckBoxState.UncheckedNormal);
                result = new Rectangle(new Point(selectedItem.Bounds.X, selectedItem.Bounds.Y), sz);
            }
            return result;
        }

        private void HandleClickEvents(object sender, MouseEventArgs e)
        {
            ListViewItem currentSelectedItem = lvFF.GetItemAt(e.X, e.Y);

            if (e.Button != MouseButtons.Right && GetCheckBoxRectangle(currentSelectedItem).Contains(e.Location))//check / uncheck the item checkbox
            {
                currentSelectedItem.Checked = !currentSelectedItem.Checked;
                currentSelectedItem.Selected = currentSelectedItem.Checked;

                lvFF.RedrawItems(currentSelectedItem.Index, currentSelectedItem.Index, false);
            }
            else if (e.Button != MouseButtons.Right && !lvFF.MultiSelect)
            {
                foreach (ListViewItem selectedItem in lvFF.SelectedItems)
                {
                    if (selectedItem.Tag is Media)
                    {
                        Media media = (Media)selectedItem.Tag;

                        if (scrollingTabsGenre.SelectedTabs.ToLower().Contains("playlist"))
                        {
                            //start playing playlist from item clicked
                            try
                            {
                                ArrayList newPlaylist = new ArrayList();
                                //PlaylistItem selectedPlaylistItem = (PlaylistItem)selectedItem.Tag;

                                //newPlaylist.Add(media.filePath);

                                //iterate playlist
                                foreach (DataRow dr in myMedia.GetPlaylist(media.Playlists).Tables[0].Rows)
                                {
                                    if (!newPlaylist.Contains(dr["mediaSource"].ToString()))
                                        newPlaylist.Add(dr["mediaSource"].ToString());
                                }


                                //Cursor.Current = Cursors.WaitCursor;

                                //splashScreen mediaSplash = new splashScreen();

                                ////if (Form1.Mode == "Release")
                                ////{
                                //mediaSplash.SplashMessage2 = media.Playlists;

                                //mediaSplash.Show();

                                //SplashScreenNew.ShowSplashScreen();

                                //}

                                //this.Hide();
                                mediaWindow = createMediaWindow(media);
                                //						mediaWindow.playVideo(selectedLabel.Tag.ToString());
                                //mediaWindow.PlayClip(selectedMedia);
                                mediaWindow.PlayMedia(newPlaylist, "", "");
                                //if (Form1.Mode == "Release")
                                //{
                                //mediaSplash.Close();
                                //SplashScreenNew.CloseForm();
                                //}

                                //mediaWindow.ShowDialog(this);
                                //mediaWindow.Show();

                                ////Cursor.Current = Cursors.Arrow;

                                //mediaWindow.TopMost = true;

                            }
                            catch (Exception ex)
                            {
                                Tools.WriteToFile(Tools.errorFile, "Play playlist error: " + ex.Message);
                            }
                        }
                        else
                        {
                            switch(media.MediaType.ToLower())
                            {
                                case "tv":
                                    string selectedGenres = scrollingTabsGenre.SelectedTabs;

                                    if (media.IMDBNum != null && media.IMDBNum.Length == 0)//display episodes
                                    {
                                        displayTVEpisodes(media.SeriesIMDBNum);

                                        SplashScreenNew.CloseForm();
                                    }
                                    else
                                    {
                                        SplashScreenNew.ShowSplashScreen(media);

                                        //play media
                                        playMedia(media);
                                    }
                                    break;
                                case "music":
                                    if(media.ReleaseID == null || media.ReleaseID.Length == 0)//display albums
                                    {
                                        displayAlbums(media.ArtistID);

                                        SplashScreenNew.CloseForm();
                                    }
                                    else if (media.RecordingID == null || media.RecordingID.Length == 0)//display songs
                                    {
                                        displaySongs(media.ReleaseID);

                                        SplashScreenNew.CloseForm();
                                    }
                                    else //play song
                                    {
                                        SplashScreenNew.ShowSplashScreen(media);

                                        //play media
                                        playMedia(media);
                                    }
                                    break;
                                case "pictures":
                                    //display picture
                                    //pbPictures.ImageLocation = media.filePath;
                                    //System.Drawing.Image thePicture = System.Drawing.Image.FromFile(media.filePath);

                                    pbPictures.BackgroundImage = System.Drawing.Image.FromFile(media.filePath);
                                    pbPictures.Visible = true;
                                    btnClosePicture.Visible = true;
                                    btnClosePicture.BringToFront();
                                    break;
                                case "games":
                                    SCTVFlash.FlashPlayer flashPlayer = new SCTVFlash.FlashPlayer();
                                    flashPlayer.WindowState = FormWindowState.Maximized;
                                    
                                    //flashPlayer.documentLoaded += new SCTV.FlashPlayer.DocumentLoaded(flashPlayer_documentLoaded);

                                    string url = media.filePath;

                                    if (!url.ToLower().StartsWith("file:\\\\"))
                                        url = "file:\\\\" + url;

                                    flashPlayer.GameToPlay = url;

                                    flashPlayer.Show(this);
                                    break;
                                default:
                                    SplashScreenNew.ShowSplashScreen(media);

                                    //play media
                                    playMedia(media);
                                    break;
                            }
                        }
                    }
                    else if (selectedItem.Tag is OnlineChannel)
                    {
                        OnlineChannel channel = (OnlineChannel)selectedItem.Tag;

                        //play channel
                        SCTVJustinTV.JustinTV justinTV = new SCTVJustinTV.JustinTV("");
                        justinTV.PlayChannel(channel);

                        justinTV.ShowDialog(this);
                    }
                    else if (selectedItem.Tag is OnlineMediaType)
                    {
                        OnlineMediaType onlineMedia = (OnlineMediaType)selectedItem.Tag;

                        //display media
                        displayBrowser();

                        string url = onlineMedia.URL;

                        if (!url.ToLower().StartsWith("http://"))
                            url = "http://" + url;

                        safeSurf.URL = new Uri(url);

                        if (url.ToLower().Contains("justin.tv"))
                            safeSurf.ShowJustinRecordButton = true;
                    }
                    else if (selectedItem.Tag is PlaylistItem)
                    {
                        //start playing playlist from item clicked
                        try
                        {
                            ArrayList newPlaylist = new ArrayList();
                            PlaylistItem selectedPlaylistItem = (PlaylistItem)selectedItem.Tag;

                            newPlaylist.Add(selectedPlaylistItem.MediaSource);

                            //iterate playlist
                            foreach (DataRow dr in myMedia.GetPlaylist(selectedPlaylistItem.PlaylistName).Tables[0].Rows)
                            {
                                if (!newPlaylist.Contains(dr["mediaSource"].ToString()))
                                    newPlaylist.Add(dr["mediaSource"].ToString());
                            }


                            //Cursor.Current = Cursors.WaitCursor;

                            //splashScreen mediaSplash = new splashScreen();

                            ////if (Form1.Mode == "Release")
                            ////{
                            //mediaSplash.SplashMessage2 = selectedPlaylistItem.PlaylistName;

                            //mediaSplash.Show();

                            //SplashScreenNew.ShowSplashScreen();

                            //}

                            //this.Hide();
                            //mediaWindow = createMediaWindow();
                            ////						mediaWindow.playVideo(selectedLabel.Tag.ToString());
                            ////mediaWindow.PlayClip(selectedMedia);
                            //mediaWindow.PlayMedia(newPlaylist, "", "");

                            //if (Form1.Mode == "Release")
                            //{
                            //mediaSplash.Close();
                            //SplashScreenNew.CloseForm();
                            //}

                            //mediaWindow.ShowDialog(this);
                            //mediaWindow.Show();

                            ////Cursor.Current = Cursors.Arrow;

                            //mediaWindow.TopMost = true;

                        }
                        catch (Exception ex)
                        {
                            Tools.WriteToFile(Tools.errorFile, "Play playlist error: " + ex.Message);
                        }
                    }
                    //else if (selectedItem.Tag is Game)
                    //{
                    //    Game gameToPlay = (Game)selectedItem.Tag;

                    //    SCTVFlash.FlashPlayer flashPlayer = new SCTVFlash.FlashPlayer();
                    //    flashPlayer.BringToFront();
                    //    //flashPlayer.documentLoaded += new SCTV.FlashPlayer.DocumentLoaded(flashPlayer_documentLoaded);

                    //    string url = gameToPlay.Location;

                    //    if (!url.ToLower().StartsWith("file:\\\\"))
                    //        url = "file:\\\\" + url;

                    //    flashPlayer.GameToPlay = url;

                    //    flashPlayer.Show();

                    //    SplashScreenNew.CloseForm();
                    //}
                    else
                        MessageBox.Show("Media type not supported");
                }
            }
            //else if(lvFF.MultiSelect)//handle multi select
            //{
            //    ListViewItem lvItem = lvFF.GetItemAt(e.X, e.Y);

            //    //check to see if the selected item is in our list
            //    if(selectedListviewItems.Contains(lvItem))//the item is already selected - deselect it
            //    {
            //        selectedListviewItems.Remove(lvItem);
            //        lvItem.Selected = false;
            //    }
            //    else //the item is not selected yet
            //        selectedListviewItems.Add(lvItem);

            //    foreach (ListViewItem listItem in selectedListviewItems)
            //        listItem.Selected = true;
            //}
        }

        private void scrollingTabsGenre_SelectionChanged()
        {
            if (previouslySelectedCategory != scrollingTabsGenre.SelectedTabs)
            {
                lvFF.Controls.Clear();
                lvFF.Items.Clear();
                txtSearch.Text = "";
                btnCancelSearch.Visible = false;

                //string selectedTabs = "";
                //string[] tabs = scrollingTabsGenre.SelectedTabs.Split('|');

                //if (tabs.Length > 1)
                //{
                //    foreach (string genre in tabs)
                //    {
                //        if (genre.ToLower() != "new" && genre.ToLower() != "recent" && genre.ToLower() != "star rating" && genre.ToLower() != "misc" 
                //            && genre.ToLower() != "popular" && genre.ToLower() != "not-so popular" && genre.ToLower() != "all")
                //        {
                //            if (selectedTabs.Length > 0)
                //                selectedTabs += "|";

                //            selectedTabs += genre;
                //        }
                //    }

                //    scrollingTabsGenre.SelectedTabs = selectedTabs;
                //}
                //else
                //    selectedTabs = scrollingTabsGenre.SelectedTabs;

                //displayCategory(selectedTabs, scrollingTabsAlphabet.SelectedTabs, scrollingTabsMediaTypes.SelectedTabs);

                displayCategory(scrollingTabsGenre.SelectedTabs, scrollingTabsAlphabet.SelectedTabs, scrollingTabsMediaTypes.SelectedTabs);

                previouslySelectedCategory = scrollingTabsGenre.SelectedTabs;
            }
        }

        private void notNewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ArrayList checkedItems = new ArrayList();

            foreach (ListViewItem li in lvFF.Items)
            {
                if (li.Checked)
                    checkedItems.Add(li);
            }

            if (checkedItems.Count == 0)
            {
                if (lvFF.SelectedItems.Count > 0)
                    checkedItems.Add(lvFF.SelectedItems[0]);
            }

            if (((Media)((ListViewItem)checkedItems[0]).Tag).filePath.Trim().Length == 0)//it's a tv series
                MessageBox.Show("You must select an individual episode to set not new");
            else
            {
                //mark item as not new by giving it a timeplayed value greater than 0
                foreach (ListViewItem selectedItem in checkedItems)
                {
                    Media selectedMedia = (Media)selectedItem.Tag;
                    selectedMedia.TimesPlayed = "1";
                    myMedia.UpdateMediaInfo(selectedMedia);
                }
            }
        }

        private void browseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //browse for new files to add to the library
            OpenFileDialog dialog = new OpenFileDialog();
            //dialog.Filter =
            //   "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            dialog.InitialDirectory = "\\\\fileserver\\video2\\";
            dialog.Title = "Select a file";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                FileInfo fi = new FileInfo(dialog.FileName);
                myMedia.AddMediaFile(fi, true);
            }
        }

        private void advancedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //show advanced settings box
            AdvancedSettings advancedSettings = new AdvancedSettings("","");
            advancedSettings.ShowDialog(this);
        }

        private void mergeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ArrayList checkedItems = new ArrayList();

                foreach(ListViewItem li in lvFF.Items)
                {
                    if (li.Checked)
                        checkedItems.Add(li);
                }

                if (checkedItems.Count > 1)//make sure at least 2 items are selected
                {
                    Media media1 = null;
                    Media media2 = null;

                    //iterate selected items and send to merge
                    foreach (ListViewItem listItem in checkedItems)
                    {
                        if (media1 == null)
                            media1 = (Media)listItem.Tag;
                        else
                        {
                            media2 = (Media)listItem.Tag;

                            media1 = myMedia.MergeMedia(media1, media2);
                        }
                    }
                }

                //display updated data
                displayMedia();

                MessageBox.Show( "Merged Successfully");
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, "mergeToolStripMenuItem_Click error: " + ex.Message);
            }
        }

        private void btnMerge_Click(object sender, EventArgs e)
        {
            try
            {
                if (lvFF.CheckedItems.Count > 1)//make sure at least 2 items are selected
                {
                    Media media1 = null;
                    Media media2 = null;

                    //iterate selected items and send to merge
                    foreach (ListViewItem listItem in lvFF.CheckedItems)
                    {
                        if (media1 == null)
                            media1 = (Media)listItem.Tag;
                        else
                        {
                            media2 = (Media)listItem.Tag;

                            media1 = myMedia.MergeMedia(media1, media2);
                        }
                    }

                    //display updated data
                    displayMedia();

                    //btnSelect.BackColor = Color.FromArgb(255, 240, 240, 240);
                    lvFF.MultiSelect = false;
                    //btnMerge.Visible = false;

                    //deselect all items
                    foreach (ListViewItem listItem in lvFF.CheckedItems)
                        listItem.Checked = false;

                    MessageBox.Show("Merged Successfully");
                }
                else
                    MessageBox.Show("Need to select 2 items to merge");
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, "btnMerge_Click error: " + ex.Message);
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            //if (btnSelect.BackColor != Color.Green)
            //{
            //    btnSelect.BackColor = Color.Green;
            //    btnMerge.BackColor = Color.Green;
            //    lvFF.MultiSelect = true;
            //    btnMerge.Visible = true;
            //}
            //else
            //{
            //    btnSelect.BackColor = Color.FromArgb(255, 240, 240, 240);
            //    lvFF.MultiSelect = false;
            //    btnMerge.Visible = false;

            //    //deselect all items
            //    foreach (ListViewItem listItem in lvFF.SelectedItems)
            //        listItem.Selected = false;
            //}
        }

        private void cleanMediaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            myMedia.CleanMedia();

            displayMedia();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("SCTV created by Chad Lickey"+ Environment.NewLine +"Version:"+ Application.ProductVersion);
        }

        private void setupWizardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //get locations

            //refresh media

            //display new updated library

            //correct any incorrect matches

            //play media


            SetupWizard setupWizard = new SetupWizard();
            setupWizard.StartPosition = FormStartPosition.CenterScreen;
            setupWizard.SourcePaths = myMedia.Locations;
            setupWizard.ShowDialog(this);
        }

        private void autoUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                //ToolStripMenuItem contextMenu = (ToolStripMenuItem)sender;

                //edit selected control
                //if (contextMenu.Owner.Name == "EditcontextMenu")//media was selected
                //{

                ArrayList checkedItems = new ArrayList();

                foreach (ListViewItem li in lvFF.Items)
                {
                    if (li.Checked)
                        checkedItems.Add(li);
                }

                if (checkedItems.Count == 0)
                {
                    if (lvFF.SelectedItems.Count > 0)
                        checkedItems.Add(lvFF.SelectedItems[0]);
                }

                //get selected items
                if (checkedItems.Count > 0)
                {
                    if (((Media)((ListViewItem)checkedItems[0]).Tag).filePath.Trim().Length == 0)
                        MessageBox.Show("You must select an individual episode to update");
                    else if(checkedItems.Count == 1)
                    {
                        //pop up imdbInfo to handle the edit
                        SCTVObjects.MediaDetails mediaDetails = new SCTVObjects.MediaDetails((Media)((ListViewItem)checkedItems[0]).Tag, myMedia.GetAllCategories(scrollingTabsMediaTypes.SelectedTabs), myMedia.GetAllMediaTypes());
                        //mediaDetails.GlassOpacity = .9;
                        mediaDetails.ClearIMDBNum = true;
                        mediaDetails.AutoUpdate = true;
                        mediaDetails.StartPosition = FormStartPosition.CenterScreen;
                        mediaDetails.Show();

                        DialogResult result = mediaDetails.DialogResult;

                        if (result == DialogResult.OK)
                        {
                            //display updated data
                            displayMedia();
                        }
                    }
                    else //edit multiple media items
                    {
                        ArrayList checkedMedia = new ArrayList();

                        foreach(ListViewItem item in checkedItems)
                            checkedMedia.Add(item.Tag);

                        //pop up window to handle the multiple item edit
                        SCTVObjects.MediaDetailsMultiple mediaDetails = new SCTVObjects.MediaDetailsMultiple(checkedMedia, myMedia.GetAllCategories(scrollingTabsMediaTypes.SelectedTabs), myMedia.GetAllMediaTypes());
                        //mediaDetails.GlassOpacity = .9;
                        mediaDetails.ClearIMDBNum = true;
                        mediaDetails.AutoUpdate = true;
                        mediaDetails.StartPosition = FormStartPosition.CenterScreen;
                        mediaDetails.Show();

                        DialogResult result = mediaDetails.DialogResult;

                        if (result == DialogResult.OK)
                        {
                            //display updated data
                            displayMedia();
                        }
                    }
                }
                //}
                //else if (contextMenu.Name == "categoryContextMenu")//category tab was selected
                //{
                //    //get tabcontrol
                //    TabControl selectedTC = (TabControl)sender;

                //    //get selected tab
                //    TabPage selectedTP = selectedTC.SelectedTab;

                //    if (selectedTP != null)
                //    {
                //        //show edit box
                //        //EditForm editForm = new EditForm(selectedTP.Text);
                //        //editForm.ShowDialog(this);

                //        //update selected tab
                //        //selectedTP.Text = editForm.Value;
                //    }
                //}
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, "autoUpdateToolStripMenuItem_Click error: " + ex.Message);
            }
        }

        private void MediaLibrary_Listview_Load(object sender, EventArgs e)
        {
            try
            {
                int timesStarted = 0;

                XmlNode TimesStartedNode = myMedia.XMLSettings.SelectSingleNode("/settings/TimesStarted");

                int.TryParse(TimesStartedNode.InnerText, out timesStarted);

                if (timesStarted <= 1)
                {
                    new Thread(new ThreadStart(ThreadProc)).Start();
                }
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, "MediaLibrary_Listview_Load error: " + ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)//enter key is pressed
            {
                search(txtSearch.Text);
            }
        }

        private void btnSearchNew_Click(object sender, EventArgs e)
        {
            search(txtSearch.Text);
        }

        private void btnFilterOutResults_Click(object sender, EventArgs e)
        {
            if (btnFilterOutResults.Text == "+")
            {
                btnFilterOutResults.Text = "X";
            }
            else
            {
                btnFilterOutResults.Text = "+";
            }

            displayCategory(scrollingTabsGenre.SelectedTabs, scrollingTabsAlphabet.SelectedTabs, scrollingTabsMediaTypes.SelectedTabs);
        }

        private void btnCancelSearch_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            btnCancelSearch.Visible = false;

            displayCategory(scrollingTabsGenre.SelectedTabs, scrollingTabsAlphabet.SelectedTabs, scrollingTabsMediaTypes.SelectedTabs);
        }
    }
}||||||| .r0
=======
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using SCTVObjects;
using System.IO;
using System.Diagnostics;
using System.Globalization;
using SCTVCamera;
using SCTV;
using System.Net;
using System.Threading;
using System.Runtime.Remoting.Messaging;
using System.Drawing.Text;

namespace SCTV
{
    // delegates used to call MainForm functions from worker thread
    public delegate void DelegateAddString(String s);
    public delegate void DelegateThreadFinished();

    public partial class MediaLibrary_Listview : Form
    {
        ArrayList fileSystemWatchers = new ArrayList();
        ArrayList categories = new ArrayList();
        DataView dvMedia = new DataView();
        MediaHandler myMedia;
        liquidMediaPlayer mediaWindow;
        object selectedControl;
        ArrayList playlist = new ArrayList();
        PlayList playlistForm;
        SpeechRecognition speechListener;
        public string _grammarvocabFile = "xmlmediaLibrary.xml";
        WebBrowser browser;
        SCTV.MainForm safeSurf;
        private string defaultPathToSaveTo = "";
        private Speakers speakers;
        string featuredOnlineContentURL = "http://hulu.com/tv";
        Color alphabetTabColor = Color.LightSkyBlue;
        Color mediaTypeTabColor = Color.LightGreen;
        Color categoryTabColor = Color.MediumOrchid;
        Color listViewBGColor = Color.MediumOrchid;
        private bool playSequels = false;
        private bool continousPlay = false;
        Media currentMedia;
        private bool embedBrowser = false;
        private int loadOnDemandChunk = 5;
        DataTable dtRemainingResults;
        int toolTipTimerTick = 0;
        int toolTipDisplayTick = 0;
        int toolTipDelay = 2; //in seconds
        int toolTipDisplay = 0;//in seconds - zero is stay on until mouse is off item
        Media currentToolTipMedia = null;
        int titleMaxLength = 28;
        MediaToolTip toolTip;
        int textHeight = 0;

        // worker thread
        Thread m_WorkerThread;

        // events used to stop worker thread
        ManualResetEvent m_EventStopThread;
        ManualResetEvent m_EventThreadStopped;

        // Delegate instances used to call user interface functions 
        // from worker thread:
        //public DelegateAddString m_DelegateAddString;
        public DelegateThreadFinished m_DelegateThreadFinished;

        // create a delegate of MethodInvoker poiting to
        // our Foo function.
        MethodInvoker simpleDelegate;

        /// <summary>
        /// The name of the vocab file to load for this form
        /// </summary>
        protected string GrammarFile
        {
            get { return _grammarvocabFile; }
            set
            {
                _grammarvocabFile = value;
            }
        }

        public SpeechRecognition SpeechListener
        {
            set
            {
                speechListener = value;

                if (speechListener != null)
                {
                    speechListener.loadGrammarFile(_grammarvocabFile);
                    speechListener.executeCommand += new SpeechRecognition.HeardCommand(speechListener_executeCommand);
                }
            }
        }

        void speechListener_executeCommand(Phrase thePhrase)
        {
            executePhrase(thePhrase);
        }

        //mouse hooks
        //private static LowLevelMouseProc _proc = HookCallback;
        //private static IntPtr _hookID = IntPtr.Zero;
        //private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);
        //private const int WH_MOUSE_LL = 14;

        //mouse hooks
        //private enum MouseMessages
        //{
        //    WM_LBUTTONDOWN = 0x0201,
        //    WM_LBUTTONUP = 0x0202,
        //    WM_MOUSEMOVE = 0x0200,
        //    WM_MOUSEWHEEL = 0x020A,
        //    WM_RBUTTONDOWN = 0x0204,
        //    WM_RBUTTONUP = 0x0205
        //}

        public MediaLibrary_Listview(MediaHandler media)
        {
            try
            {
                InitializeComponent();

                speakers = new Speakers();

                //watch for new files
                foreach (string location in media.Locations)
                {
                    try
                    {
                        initFileSystemWatcher(location);
                    }
                    catch { }                                                
                }

                //get embed browser value from config
                bool.TryParse(System.Configuration.ConfigurationManager.AppSettings["MediaLibrary.EmbedBrowser"], out embedBrowser);

                //get loadOnDemandChunk value from config
                if (!int.TryParse(System.Configuration.ConfigurationManager.AppSettings["MediaLibrary.loadOnDemandChunk"], out loadOnDemandChunk))
                    loadOnDemandChunk = 20;

                try
                {
                    mediaTypeTabColor = Color.FromName(System.Configuration.ConfigurationManager.AppSettings["MediaLibrary.MediaTypeTabColor"]);
                }
                catch
                { }

                try
                {
                    categoryTabColor = Color.FromName(System.Configuration.ConfigurationManager.AppSettings["MediaLibrary.CategoryTabColor"]);
                }
                catch
                { }

                try
                {
                    listViewBGColor = Color.FromName(System.Configuration.ConfigurationManager.AppSettings["MediaLibrary.ListviewBGColor"]);
                }
                catch
                { }

                //set volume bar
                //pbVolume.Value = speakers.GetVolume();

                defaultPathToSaveTo = System.Configuration.ConfigurationManager.AppSettings["Media.DefaultPathToSaveTo"];
                if (defaultPathToSaveTo.Trim().Length == 0)
                {
                    //make sure directory exists
                    if (!Directory.Exists(Application.StartupPath + "\\DVD\\"))
                        Directory.CreateDirectory(Application.StartupPath + "\\DVD\\");

                    defaultPathToSaveTo = Application.StartupPath + "\\DVD\\";
                }

                myMedia = media;

                //speechListener = new SpeechRecognition("xmlmediaLibrary.xml");
                //speechListener.executeCommand += new SpeechRecognition.HeardCommand(speechListener_executeCommand);
                //speechListener.Show();

                //mouse hooks
                //_hookID = SetHook(_proc);


                //lvFF.BackColor = listViewBGColor;

                displayMedia();

                initializeAdvancedFilters();

                this.TopMost = false;

                scrollingTabsMediaTypes.SelectionChanged += new SCTV.Controls.ScrollingTabs.selectionChanged(scrollingTabsMediaTypes_SelectionChanged);
                scrollingTabsAlphabet.SelectionChanged += new SCTV.Controls.ScrollingTabs.selectionChanged(scrollingTabsAlphabet_SelectionChanged);

                ArrayList alphabet = new ArrayList();

                alphabet.Add("All");

                for (char c = 'A'; c <= 'Z'; c++)
                    alphabet.Add(c.ToString());

                scrollingTabsAlphabet.Tabs = alphabet;
                scrollingTabsAlphabet.SelectedTabs = "All";

                //SCTVObjects.Fonts fonts = new Fonts();

                //scrollingTabsGenre.Font = fonts.CustomFont("ChocolateDropsNF");
                //scrollingTabsAlphabet.Font = fonts.CustomFont("GhostWriter");
                //scrollingTabsMediaTypes.Font = fonts.CustomFont("Grange");
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }
        }

        void scrollingTabsAlphabet_SelectionChanged()
        {
            displayCategory(scrollingTabsGenre.SelectedTabs, scrollingTabsAlphabet.SelectedTabs, scrollingTabsMediaTypes.SelectedTabs);
        }

        void scrollingTabsMediaTypes_SelectionChanged()
        {
            string selectedGenres = scrollingTabsGenre.SelectedTabs;

            if (selectedGenres.Length == 0)
            {
                if (scrollingTabsMediaTypes.SelectedTabs.ToLower().Contains("online"))
                    scrollingTabsGenre.SelectedTabs = "TV";
                else
                    scrollingTabsGenre.SelectedTabIndexes = "0";
            }

            displayCategories(scrollingTabsMediaTypes.SelectedTabs);

            displayCategory(scrollingTabsGenre.SelectedTabs, scrollingTabsAlphabet.SelectedTabs, scrollingTabsMediaTypes.SelectedTabs);
        }

        private void initFileSystemWatcher(string mediaPath)
        {
            try
            {
                //FileInfo fi = new FileInfo(mediaPath);
                DirectoryInfo di = new DirectoryInfo(mediaPath);
                if (di.Exists)
                {
                    FileSystemWatcher fileWatcher = new FileSystemWatcher();

                    fileWatcher.Path = mediaPath;
                    fileWatcher.NotifyFilter = NotifyFilters.LastWrite;
                    fileWatcher.IncludeSubdirectories = true;
                    fileWatcher.Created += new FileSystemEventHandler(fileWatcher_Created);
                    fileWatcher.Changed += new FileSystemEventHandler(fileWatcher_Changed);
                    fileWatcher.EnableRaisingEvents = true;

                    fileSystemWatchers.Add(fileWatcher);
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("sctv", ex.ToString());
            }
        }

        void fileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            ////refresh media
            //RefreshToolStripButton.Enabled = false;

            //simpleDelegate = new MethodInvoker(lookForMedia);

            //// Calling lookForMedia Async
            //simpleDelegate.BeginInvoke(new AsyncCallback(ThreadFinished), null);

            myMedia.AddMediaFile(new FileInfo(e.FullPath), true);

            MediaHandler.GetMedia();

            //displayMedia();
            displayCategory(scrollingTabsGenre.SelectedTabs, scrollingTabsAlphabet.SelectedTabs, scrollingTabsMediaTypes.SelectedTabs);
        }

        void fileWatcher_Created(object sender, FileSystemEventArgs e)
        {
            myMedia.AddMediaFile(new FileInfo(e.FullPath), true);
        }

        //mouse hooks
        //private static IntPtr SetHook(LowLevelMouseProc proc)
        //{
        //    using (Process curProcess = Process.GetCurrentProcess())
        //    using (ProcessModule curModule = curProcess.MainModule)
        //    {
        //        return SetWindowsHookEx(WH_MOUSE_LL, proc,
        //            GetModuleHandle(curModule.ModuleName), 0);
        //    }
        //}

        //mouse hooks
        //private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        //{
        //    if (nCode >= 0 &&
        //        MouseMessages.WM_LBUTTONDOWN == (MouseMessages)wParam)
        //    {
        //        MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
        //        Console.WriteLine(hookStruct.pt.x + ", " + hookStruct.pt.y);
        //    }
        //    return CallNextHookEx(_hookID, nCode, wParam, lParam);
        //}

        /// <summary>
        /// get media and display
        /// </summary>
        private void displayMedia()
        {
            try
            {
                displayMediaTypes();

                displayCategories(scrollingTabsMediaTypes.SelectedTabs);

                if (scrollingTabsGenre.SelectedTabs.Length == 0)
                    scrollingTabsGenre.SelectedTabIndexes = "0";

                if (scrollingTabsMediaTypes.SelectedTabs.Length == 0)
                    scrollingTabsMediaTypes.SelectedTabIndexes = "0";

                displayCategory(scrollingTabsGenre.SelectedTabs, scrollingTabsAlphabet.SelectedTabs, scrollingTabsMediaTypes.SelectedTabs);

                SCTVObjects.SplashScreenNew.CloseForm();
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, "displayMedia error: " + ex.Message);
            }
        }

        /// <summary>
        /// queries dsMedia to find all the categories for the given mediaType
        /// </summary>
        /// <param name="mediaType"></param>
        private void displayCategories(string mediaType)
        {
            string correctCase = "";

            //display categories
            categories = new ArrayList();

            foreach (DataRowView drv in MediaHandler.GetAllCategories(mediaType))
            {
                string[] catArray; 
                string categoryString = drv["category"].ToString();
                
                if(categoryString.Contains("/"))
                    catArray = categoryString.Split('/');
                else
                    catArray = categoryString.Split('|');

                for (int x = 0; x < catArray.Length; x++)
                {
                    correctCase = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(catArray[x].Trim());
                    if (!categories.Contains(correctCase) && correctCase.Length > 0)//keep out duplicates
                        categories.Add(correctCase);
                }
            }

            //add internet before sort so it will be sorted
            if (mediaType.ToLower() == "online")
                categories.Add("Internet");

            categories.Sort();//alphabetize

            //add utility pages to categories
            if (mediaType.ToLower() != "online")
            {
                categories.Insert(0, "New");
                categories.Insert(1, "Recent");
                categories.Insert(2, "Star Rating");
                categories.Insert(3, "Misc");
                categories.Insert(4, "Popular");
                categories.Insert(5, "Not-So Popular");
                categories.Insert(6, "All");

                scrollingTabsGenre.MultiSelect = true;
            }
            else if (mediaType.ToLower() == "online")
            {
                categories.Insert(0, "Home");

                scrollingTabsGenre.MultiSelect = false;
            }

            categories.Insert(1, "Playlist");

            scrollingTabsGenre.Tabs = categories;
        }

        private void displayMediaTypes()
        {
            string correctCase = "";

            //display mediaTypes
            ArrayList mediaTypes = new ArrayList();
            try
            {
                //foreach (DataRowView drv in myMedia.GetAllMediaTypes())
                foreach (string type in MediaHandler.GetAllMediaTypes())
                {
                    string[] typeArray = type.ToString().Split('/');

                    for (int x = 0; x < typeArray.Length; x++)
                    {
                        correctCase = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(typeArray[x].Trim());
                        if (!mediaTypes.Contains(correctCase) && correctCase.Trim().Length > 0)//keep out duplicates
                            mediaTypes.Add(correctCase);
                    }


                    //correctCase = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(type.Trim());
                    //if (!mediaTypes.Contains(correctCase))//keep out duplicates
                    //    mediaTypes.Add(correctCase);
                }

                mediaTypes.Add("Online");
                mediaTypes.Sort();
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, ex.StackTrace + Environment.NewLine + ex.Message);
            }

            scrollingTabsMediaTypes.Tabs = mediaTypes;
            scrollingTabsMediaTypes.SelectedTabIndexes = "0";
        }

        void btnMediaType_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                LinearGradientBrush bgBrush = null;
                Button selectedButton = (Button)sender;
                int textWidth = (int)(e.Graphics.MeasureString(selectedButton.Text, selectedButton.Font).Width);

                selectedButton.Width = textWidth + 8;

                //set button tag
                if (selectedButton.Tag == null || !(bool)selectedButton.Tag)
                    bgBrush = new LinearGradientBrush(e.ClipRectangle, Color.OldLace, Color.LightBlue, LinearGradientMode.Horizontal);
                else
                    bgBrush = new LinearGradientBrush(e.ClipRectangle, Color.LightGreen, Color.OldLace, LinearGradientMode.Vertical);

                if (bgBrush != null)
                    using (bgBrush)
                        e.Graphics.FillRectangle(bgBrush, e.ClipRectangle.X - 10, e.ClipRectangle.Y, e.ClipRectangle.Width + 20, e.ClipRectangle.Height);

                e.Graphics.DrawString(selectedButton.Text, selectedButton.Font, Brushes.Black, 2, 4);
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }
        }

        void btnMediaType_Click(object sender, EventArgs e)
        {
            try
            {
                Button selectedButton = (Button)sender;

                //set button tag
                if (selectedButton.Tag == null || !(bool)selectedButton.Tag)
                    selectedButton.Tag = true;
                else
                    selectedButton.Tag = false;

                selectedButton.Invalidate();

                //display selected categories
                applyAdvancedFilters(scrollingTabsGenre.SelectedTabs, scrollingTabsMediaTypes.SelectedTabs, scrollingTabsAlphabet.SelectedTabs);
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }
        }

        /// <summary>
        /// find all media in selected category and starts with selected letter and display them
        /// </summary>
        /// <param name="category"></param>
        public void displayCategory(string category, string startsWith, string mediaType)
        {
            //if (mediaType.ToLower() == "online")
            //    dvMedia = new DataView();
            //else
                dvMedia = myMedia.GetCategoryMedia(category, startsWith, mediaType);
            
            displayCategory(dvMedia, category, mediaType);
        }

        /// <summary>
        /// find all media in selected category and display them
        /// </summary>
        /// <param name="dvMedia"></param>
        public void displayCategory(DataView dvMedia, string category, string mediaType)
        {
            displayCategory(dvMedia, category, mediaType, true);
        }

        public void displayCategory(DataView dvMedia, string category, string mediaType, bool clearCurrentMedia)
        {
            try
            {
                string imageKey = "";
                string spaces = "";
                string currentPlaylist = "";

                if (category.Length == 0)
                    category = "All";

                //make sure the browser is destroyed since we aren't using it any more
                if (embedBrowser)
                {
                    if (browser != null)
                    {
                        browser.Dispose();
                        browser = null;
                    }

                    if (safeSurf != null)
                    {
                        safeSurf.Dispose();
                        safeSurf = null;
                    }
                }

                //lblOnlineContent.Visible = false;

                //clear media
                if (clearCurrentMedia)
                {
                    //clear controls - the media details
                    lvFF.Controls.Clear();
                    lvFF.Items.Clear();

                    dtRemainingResults = null;
                }

                if (dvMedia.Count > 0)
                {
                    ListViewGroup lvig = null;

                    if (category.ToLower() == "playlist-removeForNow")
                    {
                        foreach (DataRowView dv in dvMedia)
                        {
                            if (dv["playlistName"].ToString() != currentPlaylist)
                            {
                                currentPlaylist = dv["playlistName"].ToString();

                                lvig = new ListViewGroup(currentPlaylist);
                                lvFF.Groups.Add(lvig);
                            }

                            PlaylistItem newItem = new PlaylistItem();
                            newItem.PlaylistName = currentPlaylist;
                            newItem.MediaSource = dv["mediaSource"].ToString();

                            //add item
                            ListViewItem listViewItem1 = new ListViewItem();

                            string mediaSource = dv["MediaSource"].ToString();

                            //make title titleMaxLength characters
                            //titleMaxLength = lvFF.Columns[0].Width - lvFF.SmallImageList.ImageSize.Width - 60;

                            if (mediaSource.Trim().Length > titleMaxLength)
                            {
                                mediaSource = mediaSource.Substring(0, titleMaxLength - 3) + "...";
                                //title = title.Substring(0, titleMaxLength) + Environment.NewLine + title.Substring(titleMaxLength).PadRight(titleMaxLength, Convert.ToChar(" "));
                            }
                            else
                                mediaSource = mediaSource.Trim().PadRight(titleMaxLength, Convert.ToChar(" "));

                            listViewItem1.Text = mediaSource;
                            listViewItem1.Tag = newItem;

                            string imagePath = "";

                            //if (dv["coverImage"] != null)
                            //    imagePath = dv["coverImage"].ToString().Trim();

                            if (!File.Exists(imagePath))
                            {
                                if (File.Exists(Application.StartupPath + "\\" + imagePath))
                                    imagePath = Application.StartupPath + "\\" + imagePath;
                            }

                            //get image info
                            if (imagePath.Length > 0 && File.Exists(imagePath))
                            {
                                imageKey = imagePath;

                                if (!ilMediaImages.Images.ContainsKey(imageKey))
                                {
                                    //if (File.Exists(imageKey))
                                    //{
                                    Bitmap newImage = new Bitmap(imageKey);
                                    ilMediaImages.Images.Add(imageKey, newImage);//add new image
                                    //}
                                    //else
                                    //    MessageBox.Show("Image not found " + Environment.NewLine + imageKey);
                                }
                            }
                            else
                                imageKey = "defaultImage";

                            listViewItem1.ImageKey = imageKey;

                            if (lvig != null)
                                listViewItem1.Group = lvig;

                            listViewItem1.ToolTipText = newItem.MediaSource;

                            //if (media.Description.Length > 0)
                            //    listViewItem1.ToolTipText += "\n" + media.Description;
                            //listViewItem1.ToolTipText += "\n" + dv["url"].ToString();

                            listViewItem1.ToolTipText += "\nPlaylist: " + newItem.PlaylistName;
                            //lvFF.Items.Add(listViewItem1);
                            lvFF.Items.Add(listViewItem1);

                            //if (lvFF.Items.Count > 0 && lvFF.Items.Count % 50 == 0)
                            //    lvFF.Update();

                            if (lvFF.Items.Count > 0 && lvFF.Items.Count % 50 == 0)
                                lvFF.Update();
                        }
                    }
                    else
                    {
                        switch (mediaType.ToLower())
                        {
                            case "online":
                                dvMedia.RowFilter = "category LIKE '*" + category + "*'";

                                lvig = new ListViewGroup(category);
                                lvFF.Groups.Add(lvig);

                                foreach (DataRowView dv in dvMedia)
                                {
                                    //create mediaType item
                                    OnlineMediaType media = new OnlineMediaType();
                                    media.Name = dv["name"].ToString();
                                    media.Category = dv["category"].ToString();
                                    media.Description = dv["description"].ToString();
                                    media.type = dv["mediaCategory"].ToString();
                                    media.URL = dv["url"].ToString();
                                    media.CoverImage = Application.StartupPath + dv["coverImage"].ToString();

                                    //add item
                                    ListViewItem listViewItem1 = new ListViewItem();

                                    string title = dv["name"].ToString();

                                    //make title 35 characters
                                    //titleMaxLength = lvFF.Columns[0].Width - lvFF.SmallImageList.ImageSize.Width - 60;

                                    if (title.Trim().Length > titleMaxLength)
                                    {
                                        title = title.Substring(0, titleMaxLength - 3) + "...";
                                        //title = title.Substring(0, titleMaxLength) + Environment.NewLine + title.Substring(titleMaxLength).PadRight(titleMaxLength, Convert.ToChar(" "));
                                    }
                                    else
                                        title = title.Trim().PadRight(titleMaxLength, Convert.ToChar(" "));

                                    listViewItem1.Text = title;
                                    listViewItem1.Tag = media;

                                    string imagePath = "";

                                    if (dv["coverImage"] != null)
                                        imagePath = dv["coverImage"].ToString().Trim();

                                    if (!File.Exists(imagePath))
                                    {
                                        if (File.Exists(Application.StartupPath + "\\" + imagePath))
                                            imagePath = Application.StartupPath + "\\" + imagePath;
                                    }

                                    //get image info
                                    if (imagePath.Length > 0 && File.Exists(imagePath))
                                    {
                                        imageKey = imagePath;

                                        if (!ilMediaImages.Images.ContainsKey(imageKey))
                                        {
                                            //if (File.Exists(imageKey))
                                            //{
                                            Bitmap newImage = new Bitmap(imageKey);
                                            ilMediaImages.Images.Add(imageKey, newImage);//add new image
                                            //}
                                            //else
                                            //    MessageBox.Show("Image not found " + Environment.NewLine + imageKey);
                                        }
                                    }
                                    else
                                        imageKey = "defaultImage";

                                    listViewItem1.ImageKey = imageKey;
                                    listViewItem1.Group = lvig;
                                    listViewItem1.ToolTipText = media.Name;
                                    listViewItem1.Name = media.Name;

                                    if (media.Description.Length > 0)
                                        listViewItem1.ToolTipText += "\n" + media.Description;
                                    listViewItem1.ToolTipText += "\n" + dv["url"].ToString();

                                    //lvFF.Items.Add(listViewItem1);
                                    lvFF.Items.Add(listViewItem1);

                                    //if (lvFF.Items.Count > 0 && lvFF.Items.Count % 50 == 0)
                                    //    lvFF.Update();

                                    if (lvFF.Items.Count > 0 && lvFF.Items.Count % 50 == 0)
                                        lvFF.Update();
                                }
                                break;
                            case "game":
                                dvMedia.RowFilter = "category LIKE '*" + category + "*'";

                                lvig = new ListViewGroup(category);
                                lvFF.Groups.Add(lvig);

                                foreach (DataRowView dv in dvMedia)
                                {
                                    //create game item
                                    Game theGame = new Game();
                                    theGame.Name = dv["name"].ToString();
                                    theGame.Genre = dv["genre"].ToString();
                                    theGame.Description = dv["description"].ToString();
                                    theGame.Type = dv["mediaCategory"].ToString();
                                    theGame.Location = dv["Location"].ToString();
                                    theGame.Thumbnail = Application.StartupPath + dv["thumbnail"].ToString();

                                    //add item
                                    ListViewItem listViewItem1 = new ListViewItem();

                                    string title = dv["name"].ToString();

                                    //make title 35 characters
                                    //titleMaxLength = lvFF.Columns[0].Width - lvFF.SmallImageList.ImageSize.Width - 60;

                                    if (title.Trim().Length > titleMaxLength)
                                    {
                                        title = title.Substring(0, titleMaxLength - 3) + "...";
                                        //title = title.Substring(0, titleMaxLength) + Environment.NewLine + title.Substring(titleMaxLength).PadRight(titleMaxLength, Convert.ToChar(" "));
                                    }
                                    else
                                        title = title.Trim().PadRight(titleMaxLength, Convert.ToChar(" "));

                                    listViewItem1.Text = title;
                                    listViewItem1.Tag = theGame;

                                    string imagePath = "";

                                    if (dv["thumbnail"] != null)
                                        imagePath = dv["thumbnail"].ToString().Trim();

                                    if (!File.Exists(imagePath))
                                    {
                                        if (File.Exists(Application.StartupPath + "\\" + imagePath))
                                            imagePath = Application.StartupPath + "\\" + imagePath;
                                    }

                                    //get image info
                                    if (imagePath.Length > 0 && File.Exists(imagePath))
                                    {
                                        imageKey = imagePath;

                                        if (!ilMediaImages.Images.ContainsKey(imageKey))
                                        {
                                            //if (File.Exists(imageKey))
                                            //{
                                            Bitmap newImage = new Bitmap(imageKey);
                                            ilMediaImages.Images.Add(imageKey, newImage);//add new image
                                            //}
                                            //else
                                            //    MessageBox.Show("Image not found " + Environment.NewLine + imageKey);
                                        }
                                    }
                                    else
                                        imageKey = "defaultImage";

                                    listViewItem1.ImageKey = imageKey;
                                    listViewItem1.Group = lvig;
                                    listViewItem1.ToolTipText = theGame.Name;
                                    listViewItem1.Name = theGame.Name;

                                    if (theGame.Description.Length > 0)
                                        listViewItem1.ToolTipText += "\n" + theGame.Description;
                                    listViewItem1.ToolTipText += "\n" + dv["url"].ToString();

                                    lvFF.Items.Add(listViewItem1);

                                    if (lvFF.Items.Count > 0 && lvFF.Items.Count % 50 == 0)
                                        lvFF.Update();
                                }
                                break;
                            default:
                                //ListViewGroup lvig = new ListViewGroup(dvMedia[0]["category"].ToString());
                                bool foundGroup = false;

                                //if (lvFF.Items.Count == 0)
                                //    lvFF.Groups.Clear();

                                if (lvFF.Items.Count == 0)
                                    lvFF.Groups.Clear();

                                if ((category.ToLower() != "star rating" && mediaType.ToLower() != "justin downloads"))
                                {
                                    //foreach (ListViewGroup group in lvFF.Groups)
                                    //{
                                    //    if (group.Header != null && group.Header == category)
                                    //    {
                                    //        foundGroup = true;
                                    //        lvig = new ListViewGroup(category);
                                    //        break;
                                    //    }
                                    //}

                                    //if (!foundGroup || lvig == null)
                                    //{
                                    lvig = new ListViewGroup(category);
                                    //lvFF.Groups.Add(lvig);
                                    lvFF.Groups.Add(lvig);

                                    //    foundGroup = true;
                                    //}
                                }
                                else
                                {
                                    lvFF.Groups.Clear();
                                }

                                if (ilMediaImages.Images.Count == 0 || !ilMediaImages.Images.ContainsKey("defaultImage"))
                                {
                                    //clear any images that might be in the list
                                    ilMediaImages.Images.Clear();

                                    //add the default image to the list
                                    Bitmap defaultImage = new Bitmap(Application.StartupPath + "\\images\\Media\\coverImages\\notAvailable.jpg");
                                    ilMediaImages.Images.Add("defaultImage", defaultImage);//add default image
                                }

                                //int listViewStartCount = lvFF.Items.Count;
                                int listViewStartCount = lvFF.Items.Count;
                                int newItemCounter = 0;
                                dtRemainingResults = dvMedia.Table.Clone();

                                foreach (DataRowView dv in dvMedia)//go through the new category results
                                {
                                    //just load enough items to equal loadOnDemandChunk to decrease load times
                                    if (lvFF.Items.Count >= loadOnDemandChunk + listViewStartCount)
                                    {
                                        //load the remaining items in dvMedia into a temp datatable to load when needed
                                        //string[] rows = new string[dvMedia.Count];
                                        //dvMedia.Table.Rows.CopyTo(rows, newItemCounter);

                                        dtRemainingResults.ImportRow(dv.Row);
                                    }
                                    else
                                    {
                                        if (category.ToLower() == "star rating")
                                        {
                                            foundGroup = false;

                                            foreach (ListViewGroup group in lvFF.Groups)
                                            {
                                                if (group.Name == dv["stars"].ToString().Trim())
                                                {
                                                    foundGroup = true;

                                                    break;
                                                }
                                            }

                                            if (!foundGroup || lvFF.Groups.Count == 0)
                                            {
                                                //set the group to show the star rating
                                                if (dvMedia.Table.Columns.Contains("stars") && dv["stars"] != null)
                                                {
                                                    lvig = new ListViewGroup(dv["stars"].ToString() + " Stars");
                                                    lvig.Name = dv["stars"].ToString();
                                                }
                                                else
                                                {
                                                    lvig = new ListViewGroup("0 Stars");
                                                    lvig.Name = "0 Stars";
                                                }

                                                lvFF.Groups.Add(lvig);
                                            }
                                        }

                                        if (category.ToLower() == "justin downloads" && mediaType.ToLower() != "movies")
                                        {
                                            foundGroup = false;

                                            foreach (ListViewGroup group in lvFF.Groups)
                                            {
                                                if (group.Name == dv["SortBy"].ToString().Trim())
                                                {
                                                    foundGroup = true;

                                                    lvig = group;

                                                    break;
                                                }
                                            }

                                            if (!foundGroup || lvFF.Groups.Count == 0)
                                            {
                                                //set the group to display the show name
                                                lvig = new ListViewGroup(dv["SortBy"].ToString().Trim());
                                                lvig.Name = dv["SortBy"].ToString().Trim();

                                                lvFF.Groups.Add(lvig);
                                            }
                                        }

                                        if (category.ToLower() == "playlist")
                                        {
                                            //set the group to display the playlist name
                                            lvig = new ListViewGroup(dv["Playlists"].ToString().Trim());
                                            lvig.Name = dv["Playlists"].ToString().Trim();

                                            lvFF.Groups.Add(lvig);
                                        }

                                        //create media item
                                        Media media = MediaHandler.CreateMedia(dv);

                                        //add item
                                        ListViewItem listViewItem1 = new ListViewItem();

                                        string title = dv["title"].ToString();

                                        //make title 25 characters
                                        //titleMaxLength = lvFF.Columns[0].Width - lvFF.SmallImageList.ImageSize.Width - 60;

                                        if (title.Trim().Length > titleMaxLength)
                                        {
                                            title = title.Substring(0, titleMaxLength - 3) + "...";
                                            //title = title.Substring(0, titleMaxLength) + Environment.NewLine + title.Substring(titleMaxLength).PadRight(titleMaxLength, Convert.ToChar(" "));
                                        }
                                        else
                                            title = title.Trim().PadRight(titleMaxLength, Convert.ToChar(" "));

                                        listViewItem1.Text = title;
                                        listViewItem1.Tag = media;

                                        //get image info
                                        if (dv["coverImage"] != null && dv["coverImage"].ToString().Length > 0 && File.Exists(dv["coverImage"].ToString()))
                                        {
                                            imageKey = dv["coverImage"].ToString();

                                            if (!ilMediaImages.Images.ContainsKey(imageKey))
                                            {
                                                try
                                                {
                                                    //if (File.Exists(imageKey))
                                                    //{
                                                    Bitmap newImage = new Bitmap(imageKey);
                                                    ilMediaImages.Images.Add(imageKey, newImage);//add new image

                                                    media.ImageListIndex = Convert.ToString(ilMediaImages.Images.Count - 1);

                                                    //}
                                                    //else
                                                    //  MessageBox.Show("Image not found " + Environment.NewLine + imageKey);
                                                }
                                                catch (Exception ex)
                                                {
                                                    //the image file was invalid
                                                    imageKey = "defaultImage";

                                                    media.ImageListIndex = "0";
                                                }
                                            }
                                            else
                                                media.ImageListIndex = ilMediaImages.Images.IndexOfKey(imageKey).ToString();
                                        }
                                        else
                                        {
                                            imageKey = "defaultImage";

                                            media.ImageListIndex = "0";
                                        }

                                        //listViewItem1.ImageKey = imageKey;

                                        

                                        listViewItem1.Group = lvig;
                                        listViewItem1.ToolTipText = media.Title;

                                        if (media.Rating.Length > 0)
                                            listViewItem1.ToolTipText += " (" + media.Rating + ")";
                                        else
                                            listViewItem1.ToolTipText += " (NA)";

                                        if (media.Description.Length > 0)
                                            listViewItem1.ToolTipText += "\n" + media.TagLine;

                                        if (media.RatingDescription.Length > 0)
                                        {
                                            listViewItem1.ToolTipText += "\n" + "(" + media.Rating + ")" + media.RatingDescription;
                                        }

                                        listViewItem1.ToolTipText += "\n" + dv["filePath"].ToString();
                                        
                                        lvFF.Items.Add(listViewItem1);

                                        newItemCounter++;
                                    }
                                }

                                int tempIndex = 0;

                                for (int i = listViewStartCount; i < newItemCounter; i++)
                                {
                                    ListViewItem lvi = this.lvFF.Items[i];

                                    if (!int.TryParse(((Media)lvi.Tag).ImageListIndex, out tempIndex))
                                        tempIndex = 0;

                                    lvi.ImageIndex = tempIndex;

                                    this.lvFF.Invalidate(lvi.Bounds);
                                    this.Update();
                                }

                                break;
                        }
                    }
                }
                else
                {
                    if (category.ToLower() == "home" || category == "")
                    {
                        try
                        {
                            //lblOnlineContent.Visible = true;
                            //lblOnlineContent.BringToFront();

                            //displayBrowser();

                            //safeSurf.URL = new Uri("http://hulu.com");

                            //safeSurf.BringToFront();

                            //lblOnlineContent.BringToFront();
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message.Contains("The remote name could not be resolved:"))//the site is not available
                            {
                                Label lblNoResults = new Label();
                                lblNoResults.Text = "Choose your online content";
                                lblNoResults.Top = 50;
                                lblNoResults.Left = 50;
                                lblNoResults.Width = 250;
                                lblNoResults.BackColor = Color.White;
                                //lvFF.Controls.Add(lblNoResults);
                                lvFF.Controls.Add(lblNoResults);
                            }
                            else
                                Tools.WriteToFile(ex);
                        }
                    }
                    else if (category.ToLower() == "internet")//display browser
                    {
                        displayBrowser();
                    }
                    else
                    {
                        ListViewGroup lvig = new ListViewGroup(scrollingTabsGenre.SelectedTabs);
                        lvFF.Groups.Add(lvig);

                        ListViewItem listViewItem1 = new ListViewItem();
                        listViewItem1.Group = lvig; 
                        lvFF.Items.Add(listViewItem1);

                        Label lblNoResults = new Label();
                        lblNoResults.Text = "No Results";
                        lblNoResults.Top = 50;
                        lblNoResults.Left = 50;
                        lblNoResults.Width = 150;
                        lblNoResults.BackColor = Color.White;
                        lvFF.Controls.Add(lblNoResults);
                    }
                }
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }
        }

        private void initializeAdvancedFilters()
        {
            //get mediatypes
            string correctCase = "";

            ArrayList mediaTypes = new ArrayList();

            try
            {
                //foreach (DataRowView drv in myMedia.GetAllMediaTypes())
                foreach (string type in MediaHandler.GetAllMediaTypes())
                {
                    string[] typeArray = type.ToString().Split('/');

                    for (int x = 0; x < typeArray.Length; x++)
                    {
                        correctCase = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(typeArray[x].Trim());
                        if (!mediaTypes.Contains(correctCase) && correctCase.Trim().Length > 0)//keep out duplicates
                            mediaTypes.Add(correctCase);
                    }


                    //correctCase = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(type.Trim());
                    //if (!mediaTypes.Contains(correctCase))//keep out duplicates
                    //    mediaTypes.Add(correctCase);
                }

                mediaTypes.Add("Online");
                mediaTypes.Sort();
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }

            //set media types
            foreach (string tempType in mediaTypes)
                mediaTypeToolStripMenuItem.DropDownItems.Add(tempType,null,mediaTypeToolStripMenuItem_Click);


            setAdvancedGenreFilters(mediaTypes);
        }

        private void lvMedia_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int timesPlayed = 0;

            foreach (ListViewItem selectedItem in lvFF.SelectedItems)
            {
                if (selectedItem.Tag is Media)
                {
                    Media media = (Media)selectedItem.Tag;

                    if (scrollingTabsGenre.SelectedTabs.ToLower().Contains("playlist"))
                    {
                        //start playing playlist from item clicked
                        try
                        {
                            ArrayList newPlaylist = new ArrayList();
                            //PlaylistItem selectedPlaylistItem = (PlaylistItem)selectedItem.Tag;

                            //newPlaylist.Add(media.filePath);

                            //iterate playlist
                            foreach (DataRow dr in myMedia.GetPlaylist(media.Playlists).Tables[0].Rows)
                            {
                                if (!newPlaylist.Contains(dr["mediaSource"].ToString()))
                                    newPlaylist.Add(dr["mediaSource"].ToString());
                            }


                            //Cursor.Current = Cursors.WaitCursor;

                            //splashScreen mediaSplash = new splashScreen();

                            ////if (Form1.Mode == "Release")
                            ////{
                            //mediaSplash.SplashMessage2 = media.Playlists;

                            //mediaSplash.Show();

                            
                            //}

                            //this.Hide();
                            mediaWindow = createMediaWindow(media);
                            //						mediaWindow.playVideo(selectedLabel.Tag.ToString());
                            //mediaWindow.PlayClip(selectedMedia);
                            mediaWindow.PlayMedia(newPlaylist, "", "");

                            //if (Form1.Mode == "Release")
                            //{
                            //mediaSplash.Close();
                            
                            //}

                            //mediaWindow.ShowDialog(this);
                            //mediaWindow.Show();
                            //SplashScreenNew.CloseForm();
                            //Cursor.Current = Cursors.Arrow;

                            //mediaWindow.TopMost = true;

                        }
                        catch (Exception ex)
                        {
                            Tools.WriteToFile(Tools.errorFile, "Play playlist error: " + ex.Message);
                        }
                    }
                    else
                    {
                        //update timesPlayed and lastplayed
                        int.TryParse(media.TimesPlayed, out timesPlayed);
                        media.TimesPlayed = Convert.ToString(timesPlayed + 1);
                        media.LastPlayed = DateTime.Now.ToString();
                        MediaHandler.UpdateMediaInfo(media);

                        if (media.MediaType.ToLower().Contains("pictures"))
                        {
                            //display picture
                            //pbPictures.ImageLocation = media.filePath;
                            //System.Drawing.Image thePicture = System.Drawing.Image.FromFile(media.filePath);

                            pbPictures.BackgroundImage = System.Drawing.Image.FromFile(media.filePath);
                            pbPictures.Visible = true;
                            btnClosePicture.Visible = true;
                            btnClosePicture.BringToFront();
                        }
                        else
                        {
                            //play media
                            playMedia(media);
                        }
                    }
                }
                else if (selectedItem.Tag is OnlineChannel)
                {
                    OnlineChannel channel = (OnlineChannel)selectedItem.Tag;

                    //play channel
                    SCTVJustinTV.JustinTV justinTV = new SCTVJustinTV.JustinTV("");
                    justinTV.PlayChannel(channel);

                    justinTV.ShowDialog(this);
                }
                else if (selectedItem.Tag is OnlineMediaType)
                {
                    OnlineMediaType onlineMedia = (OnlineMediaType)selectedItem.Tag;

                    //display media
                    displayBrowser();

                    string url = onlineMedia.URL;

                    if (!url.ToLower().StartsWith("http://"))
                        url = "http://" + url;

                    safeSurf.URL = new Uri(url);

                    if (url.ToLower().Contains("justin.tv"))
                        safeSurf.ShowJustinRecordButton = true;
                }
                else if (selectedItem.Tag is PlaylistItem)
                {
                    //start playing playlist from item clicked
                    try
                    {
                        ArrayList newPlaylist = new ArrayList();
                        PlaylistItem selectedPlaylistItem = (PlaylistItem)selectedItem.Tag;

                        newPlaylist.Add(selectedPlaylistItem.MediaSource);

                        //iterate playlist
                        foreach (DataRow dr in myMedia.GetPlaylist(selectedPlaylistItem.PlaylistName).Tables[0].Rows)
                        {
                            if(!newPlaylist.Contains(dr["mediaSource"].ToString()))
                                newPlaylist.Add(dr["mediaSource"].ToString());
                        }


                        //Cursor.Current = Cursors.WaitCursor;

                        //splashScreen mediaSplash = new splashScreen();

                        ////if (Form1.Mode == "Release")
                        ////{
                        //mediaSplash.SplashMessage2 = selectedPlaylistItem.PlaylistName;

                        //mediaSplash.Show();

                        //SplashScreenNew.ShowSplashScreen();

                        //}

                        //this.Hide();
                        //mediaWindow = createMediaWindow();
                        ////						mediaWindow.playVideo(selectedLabel.Tag.ToString());
                        ////mediaWindow.PlayClip(selectedMedia);
                        //mediaWindow.PlayMedia(newPlaylist, "", "");

                        //if (Form1.Mode == "Release")
                        //{
                        //mediaSplash.Close();
                        //SplashScreenNew.CloseForm();
                        //}

                        //mediaWindow.ShowDialog(this);
                        //mediaWindow.Show();

                        //Cursor.Current = Cursors.Arrow;

                        //mediaWindow.TopMost = true;

                    }
                    catch (Exception ex)
                    {
                        Tools.WriteToFile(Tools.errorFile, "Play playlist error: " + ex.Message);
                    }
                }
                else
                    MessageBox.Show("Media type not supported");
            }
        }

        private void displayBrowser()
        {
            if (safeSurf == null || safeSurf.IsDisposed)
            {
                safeSurf = new MainForm();

                //safeSurf.Dock = DockStyle.Fill;
                safeSurf.FormBorder = FormBorderStyle.Sizable;
                safeSurf.ShowMenuStrip = false;
                safeSurf.TopLevel = true;
                safeSurf.ShowVolumeControl = true;
                safeSurf.ShowAddressBar = false;
                this.TopMost = false;
                //safeSurf.Parent = this;

                if (embedBrowser)
                {
                    safeSurf.Dock = DockStyle.Fill;
                    safeSurf.Parent = this;
                    lvFF.Controls.Add(safeSurf);
                }

                safeSurf.Show();
            }
        }

        /// <summary>
        /// play selected media
        /// </summary>
        public void playMedia(Media selectedMedia)//play selected media
        {
            try
            {
                //string selectedMedia = getSelectedMediaFileName();

                if (File.Exists(selectedMedia.filePath))
                {
                    if (selectedMedia.MediaType.ToLower().Contains("document") || selectedMedia.MediaType.ToLower().Contains("books"))
                    {
                        //display media
                        displayBrowser();

                        string url = selectedMedia.filePath;

                        if (!url.ToLower().StartsWith("file:\\\\"))
                            url = "file:\\\\" + url;

                        safeSurf.URL = new Uri(url);

                        SplashScreenNew.CloseForm();
                    }
                    else
                    {
                        //Cursor.Current = Cursors.WaitCursor;

                        //splashScreen mediaSplash = new splashScreen();

                        ////if (Form1.Mode == "Release")
                        ////{
                        //mediaSplash.SplashMessage2 = selectedMedia.Title;

                        //mediaSplash.Show();

                        //mediaSplash.Refresh();



                        //this.Refresh();
                        //}

                        //this.Hide();
                        mediaWindow = createMediaWindow(selectedMedia);

                        //mediaWindow.MediaSplash = mediaSplash;
                        mediaWindow.PlaySequels += new liquidMediaPlayer.Sequels(mediaWindow_PlaySequels);
                        mediaWindow.ContinousPlayOn += new liquidMediaPlayer.ContinousPlaySelected(mediaWindow_ContinousPlay);
                        mediaWindow.StartedPlaying += new liquidMediaPlayer.startedPlaying(mediaWindow_StartedPlaying);
                        currentMedia = selectedMedia;
                        //						mediaWindow.playVideo(selectedLabel.Tag.ToString());
                        mediaWindow.PlayClip(selectedMedia);

                        //mediaWindow.Show();
                        //mediaWindow.TopMost = true;

                        //Cursor.Current = Cursors.Arrow;


                    }
                }
                else
                {
                    Tools.WriteToFile(Tools.errorFile, "---- Missing File : " + selectedMedia + " ----");
                    MessageBox.Show("Missing File - " + selectedMedia);
                }
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, "PlayMedia error: " + ex.Message);
            }
        }

        void mediaWindow_StartedPlaying()
        {
            //mediaWindow.Show();
            //mediaWindow.TopMost = true;

            //SplashScreenNew.CloseForm();
            //mediaSplash.Close();
        }

        void mediaWindow_PlaySequels()
        {
            playSequels = true;
        }

        void mediaWindow_ContinousPlay()
        {
            continousPlay = true;
        }

        void mediaWindow_MediaDonePlaying()
        {
            //check for sequel
            if (mediaWindow.PlaySequel || mediaWindow.ContinousPlay)
            {
                Media theSequel;

                if (mediaWindow.WhatsNext == null)
                    theSequel = myMedia.GetSequel(currentMedia, continousPlay);
                else
                    theSequel = mediaWindow.WhatsNext;

                if (theSequel != null)
                    playMedia(theSequel);
            }
        }

        private liquidMediaPlayer createMediaWindow(Media selectedMedia)
        {
            try
            {
                this.Invalidate();

                if (mediaWindow != null)
                {
                    mediaWindow.stop();
                    mediaWindow.Dispose();
                    mediaWindow = null;
                }

                mediaWindow = new liquidMediaPlayer();

                if(speechListener != null)
                    mediaWindow.SpeechListener = speechListener;

                //mediaWindow.Closed += new EventHandler(mediaWindow_Closed);
                mediaWindow.closingForm += new liquidMediaPlayer.closing(mediaWindow_MediaDonePlaying);
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, "createMediaWindow error: " + ex.Message);
            }

            return mediaWindow;
        }

        private void _MouseDown(object sender, MouseEventArgs e)
        {
            //find right click
            if (e.Button == MouseButtons.Right)
            {
                selectedControl = sender;

                //show context menu
                //itemContextMenu.Show(this, this.PointToClient(Cursor.Position));
            }
        }

        private int getLeftSide()
        {
            decimal currentRow = (decimal)(lvFF.Items.Count / lvFF.Columns.Count);
            int currentColumn = 0;
            int remainder;
            remainder = lvFF.Items.Count % lvFF.Columns.Count;

            if (remainder > 0)
            {
                currentRow++;
                currentColumn = remainder;
            }
            else
                currentColumn = lvFF.Columns.Count;

            //return (currentColumn - 1) * 500 + lvFF.SmallImageList.ImageSize.Width;
            return (currentColumn - 1) * 500 + 60;
        }

        private int getTop()
        {
            int currentRow = (int)(lvFF.Items.Count / lvFF.Columns.Count);
            int currentColumn = 0;
            int remainder;
            remainder = lvFF.Items.Count % lvFF.Columns.Count;

            if (remainder > 0)
            {
                currentRow++;
                currentColumn = remainder;
            }

            return (currentRow - 1) * lvFF.SmallImageList.ImageSize.Height + 40;
        }

        #region toolstrip methods

        /// <summary>
        /// Refresh media
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void refreshMedia_Click(object sender, EventArgs e)
        {
            RefreshToolStripButton.Enabled = false;
            refreshMediaToolStripMenuItem.Enabled = false;

            simpleDelegate = new MethodInvoker(lookForMedia);

            // Calling lookForMedia Async
            simpleDelegate.BeginInvoke(new AsyncCallback(ThreadFinished), null);
        }

        private void lookForMedia()
        {
            myMedia.lookForMedia(true);
        }

        // Set initial state of controls.
        // Called from worker thread using delegate and Control.Invoke
        private void ThreadFinished(IAsyncResult ar)
        {
            // update the grid a thread safe fashion!
            MethodInvoker updateMedia = delegate
            {
                displayMedia();

                RefreshToolStripButton.Enabled = true;
                refreshMediaToolStripMenuItem.Enabled = true;
            };

            //if (tcMediaTypes.InvokeRequired)
            //    tcMediaTypes.Invoke(updateMedia);
            //else
            //    updateMedia();
        }

        /// <summary>
        /// Set source for media
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SourceToolStripButton_Click(object sender, EventArgs e)
        {
            //show edit box
            SourcePathForm sourcePathForm = new SourcePathForm();
            sourcePathForm.SourcePaths = myMedia.Locations;
            sourcePathForm.ShowDialog(this);

            //update source paths
            myMedia.AddLocations(sourcePathForm.SourcePaths, true);
        }

        /// <summary>
        /// show security form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void securityToolStripButton_Click(object sender, EventArgs e)
        {
            try
            {
                //show context menu
                //CameraContextMenuStrip.Show(this, this.PointToClient(Cursor.Position));

                SCTVCameraMain SCTVCamera = new SCTVCameraMain();
                SCTVCamera.Opacity = 100;
                SCTVCamera.Show();
            }
            catch (Exception ex)
            {
                if (ex.InnerException.Message == "No devices of the category")
                    MessageBox.Show("There are no cameras connected");
            }
        }

        private void gamesToolStripButton_Click(object sender, EventArgs e)
        {
            //display games UI
            //SCTVGames.Games games = new SCTVGames.Games();
            //games.ShowDialog(this);
        }

        private void playListToolStripButton_Click(object sender, EventArgs e)
        {
            if (playlistForm == null)//show playlist
            {
                playlistForm = new PlayList(playlist);

                DialogResult result = playlistForm.ShowDialog(this);

                if (result == DialogResult.OK)//play playlist
                {
                    ArrayList arrSelectedMedia = new ArrayList();

                    foreach (Media playlistMedia in playlistForm.Playlist)
                    {
                        if (File.Exists(playlistMedia.filePath))
                            arrSelectedMedia.Add(playlistMedia.filePath);
                    }

                    if (arrSelectedMedia.Count > 0)
                    {
                        //splashScreen mediaSplash = new splashScreen();

                        ////if (Form1.Mode == "Release")
                        ////{
                        //mediaSplash.SplashMessage2 = playlistForm.Text;

                        //mediaSplash.ShowDialog(this);

                        //SplashScreenNew.ShowSplashScreen();

                        //}

                        //this.Hide();
                        mediaWindow = createMediaWindow((Media)playlistForm.Playlist[0]);
                        //						mediaWindow.playVideo(selectedLabel.Tag.ToString());
                        mediaWindow.PlayMedia(arrSelectedMedia, "","");

                        //if (Form1.Mode == "Release")
                        //{
                        //mediaSplash.Close();
                        //SplashScreenNew.CloseForm();
                        //}

                        //mediaWindow.ShowDialog(this);
                        //mediaWindow.Show();

                        //mediaWindow.TopMost = true;
                    }
                    else
                    {
                        //Tools.WriteToFile(Tools.errorFile, "---- Missing File : " + selectedMedia + " ----");

                        //MessageBox.Show("Missing File - " + selectedMedia);
                    }
                }
            }
            else //close playlist
            {
                playlistForm.Close();
            }
        }

        private void searchToolStripButton_Click(object sender, EventArgs e)
        {
            if (pnlSearch.Visible)
            {
                pnlSearch.Visible = false;

                lvFF.Controls.Clear();
                lvFF.Items.Clear();

                //reload selected category
                displayCategory(scrollingTabsGenre.SelectedTabs, scrollingTabsAlphabet.SelectedTabs, scrollingTabsMediaTypes.SelectedTabs);
            }
            else
            {
                pnlSearch.Visible = true;
                pnlSearch.BringToFront();
                txtSearch.Focus();

                //lvFF.Controls.Clear();
                //lvFF.Items.Clear();

                lvFF.Controls.Clear();
                lvFF.Items.Clear();

                Label lblNoResults = new Label();
                lblNoResults.Text = "No Results";
                lblNoResults.Top = 50;
                lblNoResults.Left = 50;
                lblNoResults.Width = 150;
                lblNoResults.BackColor = Color.White;
                //lvFF.Controls.Add(lblNoResults);
                lvFF.Controls.Add(lblNoResults);
            }
        }

        private void toolStripButtonAdvancedFilters_Click(object sender, EventArgs e)
        {
            //if (!DynamicMediaFilter.Visible)
            //{
            //    DynamicMediaFilter.FiltersUpdated += new SCTV.AdvancedMediaFilter.AdvancedMediaFilter.UpdateFilters(DynamicMediaFilter_FiltersUpdated);
            //    DynamicMediaFilter.Visible = true;
            //    DynamicMediaFilter.Dock = DockStyle.Top;

            //    DynamicMediaFilter.BringToFront();
            //}
            //else
            //    DynamicMediaFilter.Visible = false;

            //scrollingTabsAlphabet.Visible = !DynamicMediaFilter.Visible;
            //scrollingTabsGenre.Visible = !DynamicMediaFilter.Visible;
            //scrollingTabsMediaTypes.Visible = !DynamicMediaFilter.Visible;
            
            //volume1.BringToFront();
            //volume1.Visible = true;
        }

        #endregion

        void DynamicMediaFilter_FiltersUpdated(string SelectedGenres, string SelectedMediaTypes, string SelectedStartsWith)
        {
            applyAdvancedFilters(SelectedGenres, SelectedMediaTypes, SelectedStartsWith);
        }

        #region category context menu methods
        /// <summary>
        /// play selected category
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void playCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Media mediaToInsert = new Media();
            ArrayList arrSelectedMedia = new ArrayList();
            int timesPlayed = 0;

            foreach (ListViewItem AllItems in lvFF.Items)
            {
                mediaToInsert = (Media)AllItems.Tag;

                if (File.Exists(mediaToInsert.filePath))
                {
                    //update timesPlayed and lastPlayed
                    int.TryParse(mediaToInsert.TimesPlayed, out timesPlayed);
                    mediaToInsert.TimesPlayed = Convert.ToString(timesPlayed + 1);
                    mediaToInsert.LastPlayed = DateTime.Now.ToString();

                    MediaHandler.UpdateMediaInfo(mediaToInsert);

                    arrSelectedMedia.Add(mediaToInsert);
                }
            }

            if (arrSelectedMedia.Count > 0)
            {
                //splashScreen mediaSplash = new splashScreen();

                ////if (Form1.Mode == "Release")
                ////{
                //mediaSplash.SplashMessage2 = mediaToInsert.Title;

                //mediaSplash.ShowDialog(this);

                //SplashScreenNew.ShowSplashScreen();

                //}

                //this.Hide();
                mediaWindow = createMediaWindow(mediaToInsert);
                //						mediaWindow.playVideo(selectedLabel.Tag.ToString());
                mediaWindow.PlayMedia(arrSelectedMedia, "", "");

                //if (Form1.Mode == "Release")
                //{
                //mediaSplash.Close();
                //SplashScreenNew.CloseForm();
                //}

                //mediaWindow.ShowDialog(this);
                //mediaWindow.Show();

                //mediaWindow.TopMost = true;
            }
            else
            {
                Tools.WriteToFile(Tools.errorFile, "---- Missing File : " + mediaToInsert + " ----");

                MessageBox.Show("Missing File - " + mediaToInsert);
            }
        }

        private void editCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Coming Soon");
        }

        private void deleteCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Coming Soon");
        }
        #endregion

        #region item context menu methods
        /// <summary>
        /// play selected item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void playToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int timesPlayed = 0;

            foreach (ListViewItem selectedItem in lvFF.SelectedItems)
            {
                Media media = (Media)selectedItem.Tag;

                //update timesPlayed
                int.TryParse(media.TimesPlayed, out timesPlayed);
                media.TimesPlayed = Convert.ToString(timesPlayed + 1);
                media.LastPlayed = DateTime.Now.ToString();
                MediaHandler.UpdateMediaInfo(media);

                playMedia(media);
            }
        }

        private void showDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (ListViewItem selectedItem in lvFF.SelectedItems)
                {
                    if (selectedItem.Tag is Media)
                    {
                        if (toolTip != null)
                            toolTip.Close();

                        toolTip = new MediaToolTip((Media)selectedItem.Tag);
                        //toolTip.ActiveArea = new Rectangle(40, 10, 600, 400);
                        toolTip.TitleBarSize = 0;
                        toolTip.GlassOpacity = 1;
                        toolTip.Location = Cursor.Position;
                        toolTip.DisplayMaximize = false;
                        toolTip.DisplayMinimize = false;
                        toolTip.Show();
                    }

                    break;
                }



                ////get tabcontrol
                //System.Windows.Forms.ListView selectedLV = (System.Windows.Forms.ListView)selectedControl;

                ////get selected tab
                //if (selectedLV.SelectedItems.Count > 0)
                //{

                //    //pop up imdbInfo to handle the edit
                //    //SCTVObjects.MediaDetails mediaDetails = new SCTVObjects.MediaDetails((Media)lvFF.SelectedItems[0].Tag, MediaHandler.GetAllCategories(tcMediaTypes.SelectedTab.Text), MediaHandler.GetAllMediaTypes());
                //    SCTVObjects.MediaDetails mediaDetails = new SCTVObjects.MediaDetails((Media)lvFF.SelectedItems[0].Tag, MediaHandler.GetAllCategories(scrollingTabsMediaTypes.SelectedTabs), MediaHandler.GetAllMediaTypes());
                //    DialogResult result = mediaDetails.ShowDialog(this);

                //    if (result == DialogResult.OK)
                //    {
                //        //update media file
                //        MediaHandler.UpdateMediaInfo(mediaDetails.MediaToEdit);

                //        //update in memory dataset
                //        MediaHandler.GetMedia();

                //        //display updated data
                //        displayMedia();
                //    }
                //}
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, "editToolStripMenuItem_Click error: " + ex.Message);
            }
        }

        /// <summary>
        /// Remove this media item from the current category
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removeFromCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lvFF.SelectedItems.Count > 0)
            {
                if (MessageBox.Show("Are you sure you want to remove the selected item from the current category?", "Remove from category?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Media mediaDetails = (Media)lvFF.SelectedItems[0].Tag;
                    string currentCategory = scrollingTabsGenre.SelectedTabs;

                    if (mediaDetails.category.IndexOf(currentCategory + " /") > -1)
                        mediaDetails.category = mediaDetails.category.Replace(currentCategory + " /", "");
                    else if (mediaDetails.category.IndexOf("/ " + currentCategory) > -1)
                        mediaDetails.category = mediaDetails.category.Replace("/ " + currentCategory, "");
                    else
                        mediaDetails.category = mediaDetails.category.Replace(currentCategory, "");

                    //update media file
                    MediaHandler.UpdateMediaInfo(mediaDetails);

                    //update in memory dataset
                    MediaHandler.GetMedia();

                    //display updated data
                    displayMedia();
                }
            }
            else
                MessageBox.Show("You must select something to remove");
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Coming Soon");
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                //ToolStripMenuItem contextMenu = (ToolStripMenuItem)sender;

                //edit selected control
                //if (contextMenu.Owner.Name == "EditcontextMenu")//media was selected
                //{

                //get selected items
                if (lvFF.SelectedItems.Count > 0)
                {
                    //pop up imdbInfo to handle the edit
                    SCTVObjects.MediaDetails mediaDetails = new SCTVObjects.MediaDetails((Media)lvFF.SelectedItems[0].Tag, MediaHandler.GetAllCategories(scrollingTabsMediaTypes.SelectedTabs), MediaHandler.GetAllMediaTypes());
                    mediaDetails.GlassOpacity = .9;
                    mediaDetails.Show();

                    DialogResult result = mediaDetails.DialogResult;

                    if (result == DialogResult.OK)
                    {
                        //display updated data
                        displayMedia();
                    }
                }
                //}
                //else if (contextMenu.Name == "categoryContextMenu")//category tab was selected
                //{
                //    //get tabcontrol
                //    TabControl selectedTC = (TabControl)sender;

                //    //get selected tab
                //    TabPage selectedTP = selectedTC.SelectedTab;

                //    if (selectedTP != null)
                //    {
                //        //show edit box
                //        //EditForm editForm = new EditForm(selectedTP.Text);
                //        //editForm.ShowDialog(this);

                //        //update selected tab
                //        //selectedTP.Text = editForm.Value;
                //    }
                //}
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, "editToolStripMenuItem_Click error: " + ex.Message);
            }
        }

        /// <summary>
        /// Search for updated media info giving the user choices on multiple results
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void advancedInfoSearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (lvFF.SelectedItems.Count > 0)
                {
                    //pop up imdbInfo to handle the edit
                    SCTVObjects.MediaDetails mediaDetails = new SCTVObjects.MediaDetails((Media)lvFF.SelectedItems[0].Tag, MediaHandler.GetAllCategories(scrollingTabsMediaTypes.SelectedTabs), MediaHandler.GetAllMediaTypes());
                    mediaDetails.GlassOpacity = .9;
                    mediaDetails.AutoUpdate = true;
                    mediaDetails.Show();

                    //update in memory dataset
                    //MediaHandler.GetMedia();

                    //display updated data
                    displayMedia();



                    ////pop up imdbInfo to handle the search
                    //IMDBInfo imdbInfo = new IMDBInfo();
                    //imdbInfo.MediaToSearchFor = (Media)lvFF.SelectedItems[0].Tag;
                    //DialogResult result = imdbInfo.ShowDialog(this);

                    //if (result == DialogResult.OK)
                    //{
                    //    //update media file
                    //    MediaHandler.UpdateMediaInfo(imdbInfo.FoundMedia);

                    //    //update in memory dataset
                    //    MediaHandler.GetMedia();

                    //    //display updated data
                    //    displayMedia();
                    //}
                }
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, "advancedInfoSearchToolStripMenuItem_Click: " + ex.Message);
            }
        }

        private void addToPlaylistToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem li in lvFF.SelectedItems)
                if (li.Tag is Media)
                {
                    myMedia.AddToPlaylist((Media)li.Tag);
                    //playlist.Add((Media)li.Tag);
                }
                else
                    MessageBox.Show("Type not supported yet");
        }
        #endregion

        private void btnSearch_Click(object sender, EventArgs e)
        {
            lvFF.Controls.Clear();
            lvFF.Items.Clear();

            dvMedia = myMedia.SearchByTitle(txtSearch.Text);

            if (dvMedia.Count > 0)
                displayCategory(dvMedia, "", "");
            else
            {
                Label lblNoResults = new Label();
                lblNoResults.Text = "No Results";
                lblNoResults.Top = 50;
                lblNoResults.Left = 50;
                lblNoResults.Width = 150;
                lblNoResults.BackColor = Color.White;
                //lvFF.Controls.Add(lblNoResults);

                lvFF.Controls.Add(lblNoResults);
            }
        }

        /// <summary>
        /// execute given phrase
        /// </summary>
        /// <param name="phrase"></param>
        public void executePhrase(Phrase phrase)
        {
            if (phrase.Accuracy > 10)
            {
                //check for media rule
                if (phrase.RuleName == "media")
                {
                    string mediaVerbs = "";
                    string mediaName = phrase.phrase;
                    int spaceIndex = 0;
                    int numElements = phrase.NumberOfElements;
                    //remove children items from phrase
                    for (int x = 0; x < numElements; x++)
                    {
                        spaceIndex = mediaName.IndexOf(" ");
                        mediaVerbs += mediaName.Substring(0, spaceIndex) + " ";
                        mediaName = mediaName.Substring(spaceIndex).Trim();
                    }

                    switch (mediaVerbs.ToLower().Trim())
                    {
                        case "play":
                            //look at categories and see if we have a match
                            if (categories.Contains(mediaName))
                            {
                                //play media in category
                                //playCategory(mediaName);
                            }
                            else
                            {
                                //scrollToMedia(mediaName);
                                //playMedia();
                            }
                            break;
                        case "scroll to":
                        case "show me":
                        case "show":
                        case "find":
                        case "display":
                        default:
                            //look at categories and see if we have a match
                            //if (isCategory(mediaName))
                            if (categories.Contains(mediaName))
                            {
                                //scroll to category
                                //scrollToCategory(mediaName);
                            }
                            else
                            {
                                try
                                {
                                    //scroll to media
                                    //scrollToMedia(mediaName);
                                }
                                catch (Exception ex)
                                {

                                    throw;
                                }
                            }
                            break;
                    }
                }
                else
                {
                    switch (phrase.phrase)
                    {
                        case "left":
                        case "right":
                        case "up":
                        case "down":
                            //EventLog.WriteEntry("sctv", "executePhrase library " + phrase.phrase);
                            //executeMacros(phrase.phrase);
                            break;
                        case "scroll left":
                        case "scroll right":
                        case "scroll up":
                        case "scroll down":
                            //scrollDirection = phrase.phrase.Replace("scroll ", "");
                            //scrollTimer.Enabled = true;
                            break;
                        case "stop":
                        case "scroll stop":
                            //scrollTimer.Enabled = false;
                            break;
                        case "play":
                            //executeMacros("Play/Pause");//play selected file
                            break;
                        default:
                            EventLog.WriteEntry("sctv", "Couldn't execute phrase " + phrase.phrase);
                            break;
                    }
                }
            }
            else
                EventLog.WriteEntry("sctv", "sctv accuracy= " + phrase.Accuracy.ToString() + " phrase " + phrase.phrase);
        }

        /// <summary>
        /// executes macros immediately
        /// </summary>
        /// <param name="macroName"></param>
        private void executeMacros(string macroName)
        {
            try
            {
                //Tools.WriteToFile(Tools.errorFile,"calling macro in liquidMedia " + macroName);
                //keyStrokeTracker.Clear();
                switch (macroName)
                {
                    case "mute":
                    case "volume down":
                    case "volume up":
                        speakers.Volume(macroName);

                        //pbVolume.Value = speakers.GetVolume();
                        break;
                }
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, "executeMacros error: " + ex.Message);
            }
        }

        private void JustinTVToolStripButton_Click(object sender, EventArgs e)
        {
            SCTVJustinTV.JustinTV justinTV = new SCTVJustinTV.JustinTV("");

            lvFF.Controls.Clear();
            lvFF.Items.Clear();

            ListViewGroup lvig = new ListViewGroup("JustinTV");
            lvFF.Groups.Add(lvig);

            foreach (OnlineChannel channel in justinTV.Channels)
            {
                ListViewItem listViewItem1 = new ListViewItem();
                listViewItem1.Text = channel.ShowTitle;
                listViewItem1.Group = lvig;
                listViewItem1.ToolTipText = channel.ChannelTitle;
                listViewItem1.Tag = channel;
                lvFF.Items.Add(listViewItem1);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            pnlSearch.Visible = false;

            lvFF.Controls.Clear();
            lvFF.Items.Clear();

            //reload selected category
            displayCategory(scrollingTabsGenre.SelectedTabs, scrollingTabsAlphabet.SelectedTabs, scrollingTabsMediaTypes.SelectedTabs);
        }

        private void RecordToolStripButton_Click(object sender, EventArgs e)
        {
            DriveInfo driveInfo = new DriveInfo("d");
            string discName = driveInfo.VolumeLabel;

            discName = System.Text.RegularExpressions.Regex.Replace(discName, @"\W*", "");

            recordRemoveableMedia("D", Application.ExecutablePath + "\\video\\" + discName + ".CEL", false);
        }

        private void playRemoveableMedia(string driveLetter, bool skipMenu)
        {
            playRemoveableMedia(driveLetter, "", skipMenu);
        }

        private void playRemoveableMedia(string driveLetter, string fileToRecordTo, bool skipMenu)
        {
            //make sure mediaplayer exists
            liquidMediaPlayer mediaPlayer = new liquidMediaPlayer();
            mediaPlayer.PlayRemoveableMedia(driveLetter, fileToRecordTo, skipMenu);

            mediaPlayer.ShowDialog();
        }

        private void recordRemoveableMedia(string driveLetter, string fileToRecordTo, bool skipMenu)
        {
            //make sure mediaplayer exists

            liquidMediaPlayer mediaPlayer = new liquidMediaPlayer();
            mediaPlayer.RecordRemoveableMedia(driveLetter, fileToRecordTo, skipMenu);

            mediaPlayer.ShowDialog();

            mediaPlayer.TopMost = true;
        }

        private void btnClosePicture_Click(object sender, EventArgs e)
        {
            pbPictures.Visible = false;
            btnClosePicture.Visible = false;
            pbPictures.ImageLocation = "";
        }

        private void toolStripButtonDVD_Click(object sender, EventArgs e)
        {
            string driveLetter = "E:";
            System.IO.DriveInfo[] drives = System.IO.DriveInfo.GetDrives();

            foreach (System.IO.DriveInfo drive in drives)
            {
                if (drive.DriveType == System.IO.DriveType.CDRom)
                    driveLetter = drive.RootDirectory.FullName.Substring(0,drive.RootDirectory.FullName.IndexOf(":")+1);

                //Floppies will appear as DriveType.Removable, same as USB drives.
            }

            bool skipDVDMenu = false;
            string discName;
            
            DriveInfo driveInfo;
            InsertedMedia insertedMedia = new InsertedMedia();

            driveInfo = new DriveInfo(driveLetter);

            bool.TryParse(System.Configuration.ConfigurationManager.AppSettings["Media.SkipDVDMenu"], out skipDVDMenu);
            insertedMedia.SkipMenu = skipDVDMenu;
            insertedMedia.Drive = driveInfo;
            insertedMedia.ShowDialog(this);

            discName = insertedMedia.MediaName;

            switch (insertedMedia.MediaState)
            {
                case MediaStateEnum.Play:
                    playRemoveableMedia(driveLetter, insertedMedia.SkipMenu);
                    break;
                case MediaStateEnum.PlayAndRecord:
                    playRemoveableMedia(driveLetter, defaultPathToSaveTo + "\\" + discName + ".CEL", insertedMedia.SkipMenu);
                    break;
                case MediaStateEnum.Record:
                    recordRemoveableMedia(driveLetter, defaultPathToSaveTo + "\\" + discName + ".CEL", insertedMedia.SkipMenu);
                    break;
            }
        }

        private void showSequelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem selectedItem in lvFF.SelectedItems)
            {
                Media media = (Media)lvFF.SelectedItems[0].Tag;
                Media sequel = myMedia.GetSequel(media, true);

                if (sequel != null)
                    MessageBox.Show(sequel.Title);
                else
                    MessageBox.Show("Didn't find sequel");

                break;
            }
        }

        private void removeAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for(int x = 1; x < toolStripDynamicFilter.Items.Count; x++)
                toolStripDynamicFilter.Items.RemoveAt(x);
        }

        /// <summary>
        /// Handle clicks on the media types dropdown list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mediaTypeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripDropDownButton newItem = new ToolStripDropDownButton();
            newItem.Text = ((ToolStripDropDownItem)sender).Text;
            newItem.Tag = ((ToolStripDropDownItem)sender).OwnerItem.Text;
            newItem.DropDownItems.Add("Delete", null, deleteItem_click);

            toolStripDynamicFilter.Items.Add(newItem);

            applyAdvancedFilters();
        }

        /// <summary>
        /// Handle clicks on the genre dropdown list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void genreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripDropDownButton newItem = new ToolStripDropDownButton();
            newItem.Text = ((ToolStripDropDownItem)sender).Text;
            newItem.Tag = ((ToolStripDropDownItem)sender).OwnerItem.Text;
            newItem.DropDownItems.Add("Delete",null,deleteItem_click);

            toolStripDynamicFilter.Items.Add(newItem);

            applyAdvancedFilters();
        }

        /// <summary>
        /// Handle clicks on the alpha/numeric dropdown list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void alphaNumericToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripDropDownButton newItem = new ToolStripDropDownButton();
            newItem.Text = ((ToolStripDropDownItem)sender).Text;
            newItem.Tag = ((ToolStripDropDownItem)sender).OwnerItem.Text;
            newItem.DropDownItems.Add("Delete", null, deleteItem_click);

            toolStripDynamicFilter.Items.Add(newItem);

            applyAdvancedFilters();
        }

        private void setAdvancedGenreFilters(ArrayList mediaTypes)
        {
            ArrayList allGenres = new ArrayList();

            foreach (string type in mediaTypes)
                allGenres = setAdvancedGenreFilters(type, false);

            allGenres.Sort();

            //set genre
            foreach (string tempCat in allGenres)
                genreToolStripMenuItem1.DropDownItems.Add(tempCat, null, genreToolStripMenuItem_Click);
        }

        private ArrayList setAdvancedGenreFilters(string mediaType, bool addToToolStripMenu)
        {
            //get genre
            string correctCase = "";

            //display categories
            categories = new ArrayList();

            foreach (DataRowView drv in MediaHandler.GetAllCategories(mediaType))
            {
                string[] catArray;
                string categoryString = drv["category"].ToString();

                if (categoryString.Contains("/"))
                    catArray = categoryString.Split('/');
                else
                    catArray = categoryString.Split('|');

                for (int x = 0; x < catArray.Length; x++)
                {
                    correctCase = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(catArray[x].Trim());
                    if (!categories.Contains(correctCase) && correctCase.Length > 0)//keep out duplicates
                        categories.Add(correctCase);
                }
            }

            //add internet before sort so it will be sorted
            if (mediaType.ToLower() == "online")
                categories.Add("Internet");

            categories.Sort();//alphabetize

            //add utility pages to categories
            if (mediaType.ToLower() != "online")
            {
                if(!categories.Contains("New"))
                    categories.Insert(0, "New");

                if (!categories.Contains("Recent"))
                    categories.Insert(1, "Recent");

                if (!categories.Contains("Star Rating"))
                    categories.Insert(2, "Star Rating");

                if (!categories.Contains("Misc"))
                    categories.Insert(3, "Misc");

                if (!categories.Contains("Popular"))
                    categories.Insert(4, "Popular");

                if (!categories.Contains("Not-So Popular"))
                    categories.Insert(5, "Not-So Popular");

                if (!categories.Contains("All"))
                    categories.Insert(6, "All");
            }
            else if (mediaType.ToLower() == "online")
            {
                if (!categories.Contains("Home"))
                    categories.Insert(0, "Home");
            }

            if (!categories.Contains("Playlist"))
                categories.Insert(1, "Playlist");


            if (addToToolStripMenu)
            {
                //set genre
                foreach (string tempCat in categories)
                    genreToolStripMenuItem1.DropDownItems.Add(tempCat, null, genreToolStripMenuItem_Click);
            }

            return categories;
        }

        private void deleteItem_click(object sender, EventArgs e)
        {
            toolStripDynamicFilter.Items.Remove(((ToolStripMenuItem)sender).OwnerItem);

            applyAdvancedFilters();
        }

        /// <summary>
        /// display media according to what filters are selected
        /// </summary>
        private void applyAdvancedFilters()
        {
            string selectedGenres = "";
            string selectedMediaTypes = "";
            string selectedStartsWith = "";

            foreach (ToolStripItem filter in toolStripDynamicFilter.Items)
            {
                if (filter.Text != "Filters" && filter.Text.Length > 0)
                {
                    switch (filter.Tag.ToString())
                    {
                        case "Genre":
                            if (selectedGenres.Length > 0)
                                selectedGenres = selectedGenres + "|";

                            selectedGenres += filter.Text;
                            break;
                        case "Media Type":
                            if (selectedMediaTypes.Length > 0)
                                selectedMediaTypes = selectedMediaTypes + "|";

                            selectedMediaTypes += filter.Text;
                            break;
                        case "Alpha/Numeric":
                            selectedStartsWith = filter.Text;
                            break;
                    }
                }
            }

            //display filtered media
            DialogResult result = DialogResult.OK;

            if (selectedGenres.Length == 0)
                selectedGenres = "All";

            if (selectedStartsWith.Length == 0)
                selectedStartsWith = "All";

            if (selectedStartsWith == "All" && selectedGenres == "All")
                result = MessageBox.Show("You are about to display \"All\" of your media, this may take several minutes." + Environment.NewLine + "Do you wish to continue?", "Selecting All media", MessageBoxButtons.OKCancel);

            if (result == DialogResult.Cancel)
            {
                //force "no results" display
                selectedGenres = "";
                selectedStartsWith = "";
                selectedMediaTypes = "";
            }

            lvFF.Controls.Clear();
            lvFF.Items.Clear();

            displayCategory(selectedGenres, selectedStartsWith, selectedMediaTypes);
        }

        /// <summary>
        /// display media according to what filters are selected
        /// </summary>
        private void applyAdvancedFilters(string SelectedGenres, string SelectedMediaTypes, string SelectedStartsWith)
        {
            //display filtered media
            DialogResult result = DialogResult.OK;

            if (SelectedGenres.Length == 0)
                SelectedGenres = "All";

            if (SelectedStartsWith.Length == 0)
                SelectedStartsWith = "All";

            if (SelectedStartsWith == "All" && SelectedGenres == "All")
                result = MessageBox.Show("You are about to display \"All\" of your media, this may take several minutes." + Environment.NewLine + "Do you wish to continue?", "Selecting All media", MessageBoxButtons.OKCancel);

            if (result == DialogResult.Cancel)
            {
                //force "no results" display
                SelectedGenres = "";
                SelectedStartsWith = "";
                SelectedMediaTypes = "";
            }

            //clear lvFF
            lvFF.Controls.Clear();
            lvFF.Items.Clear();

            displayCategory(SelectedGenres, SelectedStartsWith, SelectedMediaTypes);
        }

        //private void lvMedia_DrawItem(object sender, DrawListViewItemEventArgs e)
        //{
        //    //check to see if item is the last item in the listview and add more if necessary and available

        //    if ((e.State & ListViewItemStates.Selected) != 0)
        //    {
        //        // Draw the background and focus rectangle for a unselected item.
        //        //e.Graphics.FillRectangle(Brushes.Maroon, e.Bounds);
        //        e.DrawFocusRectangle();
        //    }
        //    else
        //    {
        //        // Draw the background for a selected item.

        //        using (LinearGradientBrush brush =
        //            new LinearGradientBrush(e.Bounds, Color.AliceBlue,
        //            Color.Blue, LinearGradientMode.Horizontal))
        //        {
        //            e.Graphics.FillRectangle(brush, e.Bounds);
        //            //e.Graphics.FillEllipse(brush, e.Bounds);
        //        }
        //    }

        //    // Draw the item text for views other than the Details view.
        //    if (lvFF.View != View.Details)
        //    {
        //        int textHeight = (int)(e.Graphics.MeasureString("WWW", lvFF.Font).Height + 4);
        //        //e.Graphics.DrawImage(System.Drawing.Image.FromFile(((Media)e.Item.Tag).coverImage), new Point(e.Item.Bounds.Top + 2, e.Item.Bounds.Left + 2));//   e.Item.ImageList.Images[e.Item.ImageIndex],e.Bounds);
        //        //e.Graphics.DrawImage(System.Drawing.Image.FromFile(((Media)e.Item.Tag).coverImage), new Point(e.Item.Bounds.X + 10, e.Item.Bounds.Y + textHeight));//   e.Item.ImageList.Images[e.Item.ImageIndex],e.Bounds);

        //        //Rectangle destRect = new Rectangle(e.Item.Bounds.X + 10, e.Item.Bounds.Y + textHeight, 87, 140 - textHeight);
        //        Rectangle destRect = new Rectangle(e.Item.Bounds.X + 10, e.Item.Bounds.Y + 10, 87, 140);
        //        string coverImagePath = "";



        //        if (e.Item.Tag is Media)
        //            coverImagePath = ((Media)e.Item.Tag).coverImage;
        //        else if (e.Item.Tag is OnlineMediaType)
        //            coverImagePath = ((OnlineMediaType)e.Item.Tag).CoverImage;

        //        if (!File.Exists(coverImagePath))
        //            coverImagePath = Application.StartupPath + "\\images\\media\\coverImages\\notavailable.jpg";

        //        e.Graphics.DrawImage(System.Drawing.Image.FromFile(coverImagePath), destRect);//   e.Item.ImageList.Images[e.Item.ImageIndex],e.Bounds);

        //        e.DrawText();
        //    }

        //    if (e.ItemIndex >= (lvFF.Items.Count - 10))
        //    {
        //        //this is the last item - load another set of items if available
        //        if (dtRemainingResults != null && dtRemainingResults.Rows.Count > 0)
        //        {
        //            displayCategory(dtRemainingResults.DefaultView, scrollingTabsGenre.SelectedTabs, scrollingTabsMediaTypes.SelectedTabs, false);
        //        }
        //    }
        //}

        private void lvFF_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            //check to see if item is the last item in the listview and add more if necessary and available

            int textHeight = (int)(e.Graphics.MeasureString("WWW", lvFF.Font).Height + 4);
            Brush myBrush = Brushes.Black;
            Rectangle rectBottom;
            Rectangle rectBackground;

            if ((e.State & ListViewItemStates.Selected) != 0)
            {
                // Draw the background and focus rectangle for a unselected item.
                //e.Graphics.FillRectangle(Brushes.Maroon, e.Bounds);
                
                //e.DrawFocusRectangle();

                LinearGradientBrush bgBrush = null;

                if (e.Bounds.Width > 0 && e.Bounds.Height > 0)
                {
                    rectBackground = new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width + 4, e.Bounds.Height - 35);
                    rectBottom = new Rectangle(e.Bounds.X, rectBackground.Location.Y + rectBackground.Height - 2, e.Bounds.Width + 4, e.Item.Bounds.Height - rectBackground.Height);

                    //check to see if this item is the last one on the row and extend it's background to the right edge
                    if (e.Item.Bounds.X + (e.Item.Bounds.Width * 2) >= lvFF.Width)
                    {
                        rectBackground.Width = rectBackground.Width * 2;
                        rectBottom.Width = rectBottom.Width * 2;
                    }

                    bgBrush = new LinearGradientBrush(rectBackground, Color.Transparent, Color.SlateGray, LinearGradientMode.Vertical);

                    if (bgBrush != null)
                    {
                        using (bgBrush)
                        {
                            e.Graphics.FillRectangle(bgBrush, rectBackground);
                            //e.Graphics.FillEllipse(brush, e.Bounds);
                        }
                    }

                    bgBrush = new LinearGradientBrush(rectBottom, Color.SlateGray, Color.Transparent, LinearGradientMode.Vertical);

                    if (bgBrush != null)
                        using (bgBrush)
                            e.Graphics.FillRectangle(bgBrush, rectBottom);
                }
            }
            else
            {
                // Draw the background for a selected item.

                string rating = "";

                if(e.Item.Tag is Media)
                    rating = ((Media)e.Item.Tag).Rating.ToLower();

                LinearGradientBrush bgBrush = null;

                if (rating.Contains("pg-13") || rating.Contains("nc-17"))//blue background
                {
                    bgBrush = new LinearGradientBrush(e.Bounds, Color.AliceBlue, Color.LightBlue, LinearGradientMode.Horizontal);
                }
                else if (rating == "pg" || rating == "g")//green background
                {
                    bgBrush = new LinearGradientBrush(e.Bounds, Color.OldLace, Color.LightGreen, LinearGradientMode.Horizontal);
                }
                else if (rating == "r")//red background
                {
                    bgBrush = new LinearGradientBrush(e.Bounds, Color.OldLace, Color.LightSalmon, LinearGradientMode.Horizontal);
                }
                else//purple background
                {
                    bgBrush = new LinearGradientBrush(e.Bounds, Color.OldLace, Color.MediumPurple, LinearGradientMode.Horizontal);
                }

                if (bgBrush != null)
                {
                    using (bgBrush)
                        e.Graphics.FillRectangle(bgBrush, e.Bounds);
                }

                if (e.Item.Bounds.Width > 0 && e.Item.Bounds.Height > 0)
                {
                    //draw media info
                    Rectangle rec = e.Item.Bounds;
                    rec.Height = e.Item.Bounds.Height - textHeight;
                    rec.Width = e.Item.Bounds.Width - 97;
                    rec.X = rec.X + 100;
                    rec.Y = rec.Y + textHeight;

                    e.Graphics.DrawString(e.Item.ToolTipText, this.Font, myBrush, rec, StringFormat.GenericDefault);

                    //draw option links
                    //rec.Height = 20;
                    //rec.Y = rec.Y + rec.Height + 1;

                    //e.Graphics.DrawString("more...", this.Font, myBrush, rec, StringFormat.GenericDefault);
                }
            }

            // Draw the item text for views other than the Details view.
            if (lvFF.View != View.Details)
            {
                Rectangle coverImageRect = new Rectangle(e.Item.Bounds.X + 10, e.Item.Bounds.Y + textHeight, 87, 140);
                string coverImagePath = "";
                
                if (e.Item.Tag is Media)
                {
                    coverImagePath = ((Media)e.Item.Tag).coverImage;
                }
                else if (e.Item.Tag is OnlineMediaType)
                    coverImagePath = ((OnlineMediaType)e.Item.Tag).CoverImage;

                if (!File.Exists(coverImagePath))
                    coverImagePath = Application.StartupPath + "\\images\\media\\coverImages\\notavailable.jpg";

                System.Drawing.Image coverImage = System.Drawing.Image.FromFile(coverImagePath);

                System.Drawing.Image tmpImage = new Bitmap(coverImage,coverImage.Width, coverImage.Height);

                //draw black outline
                try
                {
                    using (Graphics g = Graphics.FromImage(coverImage))
                        g.DrawRectangle(Pens.Black, 0, 0, coverImage.Width - 1, coverImage.Height - 1);
                }
                catch (Exception ex)
                {
                    Tools.WriteToFile(ex);
                }

                //e.Graphics.DrawRectangle(Pens.Black, 0, 0, tmpImage.Width - 1, tmpImage.Height - 1);

                //using (Graphics g = e.Graphics)
                //    g.DrawRectangle(Pens.Black, 0, 0, tmpImage.Width - 1, tmpImage.Height - 1);

                //draw cover image
                e.Graphics.DrawImage(coverImage, coverImageRect);

                //draw mirror image
                //if (rectBottom != null)
                //{
                //    System.Drawing.Imaging.ColorMatrix matrix = new System.Drawing.Imaging.ColorMatrix();
                //    matrix.Matrix33 = 0.3f; //opacity 0 = completely transparent, 1 = completely opaque

                //    System.Drawing.Imaging.ImageAttributes attributes = new System.Drawing.Imaging.ImageAttributes();
                //    attributes.SetColorMatrix(matrix, System.Drawing.Imaging.ColorMatrixFlag.Default, System.Drawing.Imaging.ColorAdjustType.Bitmap);

                //    if (coverImageRect.Width > 0 && coverImageRect.Height > 0)
                //    {
                //        Rectangle coverImageMirrorRectDest = new Rectangle(coverImageRect.X, coverImageRect.Y + coverImageRect.Height - 2, coverImageRect.Width, 25);
                //        Rectangle coverImageMirrorRectSrc = new Rectangle(coverImageRect.X, coverImageRect.Height - coverImageMirrorRectDest.Height, coverImageRect.Width, coverImageMirrorRectDest.Height);
                //        System.Drawing.Image coverImageMirror = System.Drawing.Image.FromFile(coverImagePath);
                //        coverImageMirror.RotateFlip(RotateFlipType.RotateNoneFlipY);

                //        e.Graphics.DrawImage(coverImageMirror, coverImageMirrorRectDest, 0, 0, coverImageMirror.Width, coverImageMirror.Height, GraphicsUnit.Pixel, attributes);
                //    }
                //}

                e.DrawText();
            }

            if (e.ItemIndex >= (lvFF.Items.Count - 5))
            {
                //this is the last item - load another set of items if available
                if (dtRemainingResults != null && dtRemainingResults.Rows.Count > 0)
                {
                    displayCategory(dtRemainingResults.DefaultView, scrollingTabsGenre.SelectedTabs, scrollingTabsMediaTypes.SelectedTabs, false);
                }
            }
        }

        private void lvFF_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int timesPlayed = 0;

            foreach (ListViewItem selectedItem in lvFF.SelectedItems)
            {
                if (selectedItem.Tag is Media)
                {
                    Media media = (Media)selectedItem.Tag;

                    SplashScreenNew.ShowSplashScreen(media);

                    if (scrollingTabsGenre.SelectedTabs.ToLower().Contains("playlist"))
                    {
                        //start playing playlist from item clicked
                        try
                        {
                            ArrayList newPlaylist = new ArrayList();
                            //PlaylistItem selectedPlaylistItem = (PlaylistItem)selectedItem.Tag;

                            //newPlaylist.Add(media.filePath);

                            //iterate playlist
                            foreach (DataRow dr in myMedia.GetPlaylist(media.Playlists).Tables[0].Rows)
                            {
                                if (!newPlaylist.Contains(dr["mediaSource"].ToString()))
                                    newPlaylist.Add(dr["mediaSource"].ToString());
                            }


                            //Cursor.Current = Cursors.WaitCursor;

                            //splashScreen mediaSplash = new splashScreen();

                            ////if (Form1.Mode == "Release")
                            ////{
                            //mediaSplash.SplashMessage2 = media.Playlists;

                            //mediaSplash.Show();

                            //SplashScreenNew.ShowSplashScreen();

                            //}

                            //this.Hide();
                            mediaWindow = createMediaWindow(media);
                            //						mediaWindow.playVideo(selectedLabel.Tag.ToString());
                            //mediaWindow.PlayClip(selectedMedia);
                            mediaWindow.PlayMedia(newPlaylist, "", "");
                            //if (Form1.Mode == "Release")
                            //{
                            //mediaSplash.Close();
                            //SplashScreenNew.CloseForm();
                            //}

                            //mediaWindow.ShowDialog(this);
                            //mediaWindow.Show();

                            ////Cursor.Current = Cursors.Arrow;

                            //mediaWindow.TopMost = true;

                        }
                        catch (Exception ex)
                        {
                            Tools.WriteToFile(Tools.errorFile, "Play playlist error: " + ex.Message);
                        }
                    }
                    else
                    {
                        //update timesPlayed and lastplayed
                        int.TryParse(media.TimesPlayed, out timesPlayed);
                        media.TimesPlayed = Convert.ToString(timesPlayed + 1);
                        media.LastPlayed = DateTime.Now.ToString();
                        MediaHandler.UpdateMediaInfo(media);

                        if (media.MediaType.ToLower().Contains("pictures"))
                        {
                            //display picture
                            //pbPictures.ImageLocation = media.filePath;
                            //System.Drawing.Image thePicture = System.Drawing.Image.FromFile(media.filePath);

                            pbPictures.BackgroundImage = System.Drawing.Image.FromFile(media.filePath);
                            pbPictures.Visible = true;
                            btnClosePicture.Visible = true;
                            btnClosePicture.BringToFront();
                        }
                        else
                        {
                            //play media
                            playMedia(media);
                        }
                    }
                }
                else if (selectedItem.Tag is OnlineChannel)
                {
                    OnlineChannel channel = (OnlineChannel)selectedItem.Tag;

                    //play channel
                    SCTVJustinTV.JustinTV justinTV = new SCTVJustinTV.JustinTV("");
                    justinTV.PlayChannel(channel);

                    justinTV.ShowDialog(this);
                }
                else if (selectedItem.Tag is OnlineMediaType)
                {
                    OnlineMediaType onlineMedia = (OnlineMediaType)selectedItem.Tag;

                    //display media
                    displayBrowser();

                    string url = onlineMedia.URL;

                    if (!url.ToLower().StartsWith("http://"))
                        url = "http://" + url;

                    safeSurf.URL = new Uri(url);

                    if (url.ToLower().Contains("justin.tv"))
                        safeSurf.ShowJustinRecordButton = true;
                }
                else if (selectedItem.Tag is PlaylistItem)
                {
                    //start playing playlist from item clicked
                    try
                    {
                        ArrayList newPlaylist = new ArrayList();
                        PlaylistItem selectedPlaylistItem = (PlaylistItem)selectedItem.Tag;

                        newPlaylist.Add(selectedPlaylistItem.MediaSource);

                        //iterate playlist
                        foreach (DataRow dr in myMedia.GetPlaylist(selectedPlaylistItem.PlaylistName).Tables[0].Rows)
                        {
                            if (!newPlaylist.Contains(dr["mediaSource"].ToString()))
                                newPlaylist.Add(dr["mediaSource"].ToString());
                        }


                        //Cursor.Current = Cursors.WaitCursor;

                        //splashScreen mediaSplash = new splashScreen();

                        ////if (Form1.Mode == "Release")
                        ////{
                        //mediaSplash.SplashMessage2 = selectedPlaylistItem.PlaylistName;

                        //mediaSplash.Show();

                        //SplashScreenNew.ShowSplashScreen();

                        //}

                        //this.Hide();
                        //mediaWindow = createMediaWindow();
                        ////						mediaWindow.playVideo(selectedLabel.Tag.ToString());
                        ////mediaWindow.PlayClip(selectedMedia);
                        //mediaWindow.PlayMedia(newPlaylist, "", "");

                        //if (Form1.Mode == "Release")
                        //{
                        //mediaSplash.Close();
                        //SplashScreenNew.CloseForm();
                        //}

                        //mediaWindow.ShowDialog(this);
                        //mediaWindow.Show();

                        ////Cursor.Current = Cursors.Arrow;

                        //mediaWindow.TopMost = true;

                    }
                    catch (Exception ex)
                    {
                        Tools.WriteToFile(Tools.errorFile, "Play playlist error: " + ex.Message);
                    }
                }
                else if (selectedItem.Tag is Game)
                {
                    Game gameToPlay = (Game)selectedItem.Tag;

                    SCTV.FlashPlayer flashPlayer = new SCTV.FlashPlayer();
                    flashPlayer.BringToFront();
                    //flashPlayer.documentLoaded += new SCTV.FlashPlayer.DocumentLoaded(flashPlayer_documentLoaded);
                    
                    string url = gameToPlay.Location;

                    if (!url.ToLower().StartsWith("file:\\\\"))
                        url = "file:\\\\" + url;

                    flashPlayer.GameToPlay = url;

                    flashPlayer.Show();

                    SplashScreenNew.CloseForm();
                }
                else
                    MessageBox.Show("Media type not supported");
            }
        }

        private void lvFF_MouseClick(object sender, MouseEventArgs e)
        {
            //don't know if I need the below line or not
            //Point localPoint = listView.PointToClient(mousePosition);

            //ListViewItem lvItem = lvFF.GetItemAt(e.X, e.Y);
            //Rectangle itemOptions = new Rectangle(lvItem.Bounds.X + 100, lvItem.Bounds.Y + lvItem.Bounds.Height - 20, lvItem.Bounds.Width - 100, 20);

            //if (itemOptions.Contains(e.Location))
            //{
            //    //the below code needs to be executed by a link in the details of an item

            //    if (e.Button == MouseButtons.Left)
            //    {
            //        foreach (ListViewItem selectedItem in lvFF.SelectedItems)
            //        {
            //            if (selectedItem.Tag is Media)
            //            {
            //                if (toolTip != null)
            //                    toolTip.Close();

            //                toolTip = new MediaToolTip((Media)selectedItem.Tag);
            //                //toolTip.ActiveArea = new Rectangle(40, 10, 600, 400);
            //                toolTip.TitleBarSize = 0;
            //                toolTip.GlassOpacity = 1;
            //                toolTip.Location = Cursor.Position;
            //                toolTip.DisplayMaximize = false;
            //                toolTip.DisplayMinimize = false;
            //                toolTip.Show();
            //            }
            //        }
            //    }
            //}
        }

        private void scrollingTabsGenre_SelectionChanged()
        {
            lvFF.Controls.Clear();
            lvFF.Items.Clear();

            //string selectedTabs = "";
            //string[] tabs = scrollingTabsGenre.SelectedTabs.Split('|');

            //if (tabs.Length > 1)
            //{
            //    foreach (string genre in tabs)
            //    {
            //        if (genre.ToLower() != "new" && genre.ToLower() != "recent" && genre.ToLower() != "star rating" && genre.ToLower() != "misc" 
            //            && genre.ToLower() != "popular" && genre.ToLower() != "not-so popular" && genre.ToLower() != "all")
            //        {
            //            if (selectedTabs.Length > 0)
            //                selectedTabs += "|";

            //            selectedTabs += genre;
            //        }
            //    }

            //    scrollingTabsGenre.SelectedTabs = selectedTabs;
            //}
            //else
            //    selectedTabs = scrollingTabsGenre.SelectedTabs;

            //displayCategory(selectedTabs, scrollingTabsAlphabet.SelectedTabs, scrollingTabsMediaTypes.SelectedTabs);

            displayCategory(scrollingTabsGenre.SelectedTabs, scrollingTabsAlphabet.SelectedTabs, scrollingTabsMediaTypes.SelectedTabs);
        }

        private void notNewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //mark item as not new by giving it a timeplayed value greater than 0
            foreach (ListViewItem selectedItem in lvFF.SelectedItems)
            {
                Media selectedMedia = (Media)selectedItem.Tag;
                selectedMedia.TimesPlayed = "1";
                MediaHandler.UpdateMediaInfo(selectedMedia);
            }
        }

        private void browseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //browse for new files to add to the library
            OpenFileDialog dialog = new OpenFileDialog();
            //dialog.Filter =
            //   "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            dialog.InitialDirectory = "\\\\fileserver\\video2\\";
            dialog.Title = "Select a file";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                FileInfo fi = new FileInfo(dialog.FileName);
                myMedia.AddMediaFile(fi, true);
            }

        }
    }
}>>>>>>> .r5
