<<<<<<< .mine
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Diagnostics;
using DShowNET;
using SCTVObjects;
using SharpCaster.Controllers;
using SharpCaster.Extensions;
using SharpCaster.Models;
using SharpCaster.Models.ChromecastStatus;
using SharpCaster.Models.MediaStatus;
using SharpCaster.Services;
using System.Linq;
using System.Threading.Tasks;

namespace SCTV
{
    public partial class liquidMediaPlayer : Form
    {
        #region Variables
        private VLC vlc;
        private string _fileName = string.Empty;
        private string _defaultFile = string.Empty;
        AsynTask _playerTask;
        private ArrayList keyStrokeTracker = new ArrayList();
        private SortedList macroList = new SortedList();
        private System.Windows.Forms.Timer changeChannelTimer;

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern System.Int32 SystemParametersInfo(System.UInt32 uiAction, System.UInt32 uiParam,
            System.IntPtr pvParam, System.UInt32 fWinIni);
        [DllImport("user32", CharSet = CharSet.Auto)]
        private static extern bool SystemParametersInfo(int uAction, int uParam, bool lpvParam, int fuWinIni);
        [DllImport("kernel32.dll")]
        static extern bool SetProcessWorkingSetSize(IntPtr hProcess, int dwMinimumWorkingSetSize, int dwMaximumWorkingSetSize);

        private bool _capturing;
        private Image _dropperBoxed;
        private Image _dropperUnboxed;
        private Cursor _cursorDefault;
        private Cursor _cursorDropper;
        private IntPtr _hPreviousWindow;

        private string _Handle = string.Empty;
        private string _Class = string.Empty;
        private string _Text = string.Empty;
        private string _Style = string.Empty;
        private string _Rect = string.Empty;
        private int inactivityTicker = 0;
        private int inactivityTime = 5;//time in seconds before no action is considered inactivity
        Media currentMedia;

        //[DllImport("gdi32.dll")]
        //public static extern IntPtr ExtCreateRegion(int lpXform, int nCount, byte[] lpRgnData);

        //[DllImport("user32.dll")]
        //internal static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool bRedraw);

        //static private SHDocVw.ShellWindows shellWindows = new SHDocVw.ShellWindowsClass();
        public SHDocVw.ShellWindows SWs;
        public SHDocVw.InternetExplorer IE;

        public delegate void closing();
        public event closing closingForm;

        public delegate void MediaDone();
        public event MediaDone MediaDonePlaying;

        public delegate void ContinousPlaySelected();
        public event ContinousPlaySelected ContinousPlayOn;

        public delegate void Sequels();
        public event Sequels PlaySequels;

        public delegate void startedPlaying();
        public event startedPlaying StartedPlaying;

        public string _grammarvocabFile = "xmlvideoplayer.xml";

        /// <summary> seek interface to set position in stream. </summary>
        private IMediaPosition mediaPos;

        private SCTV.SlidingBar slidingBar = null;

        private Speakers speakers;
        SpeechRecognition speechListener;

        static readonly ChromecastService ChromecastService = ChromecastService.Current;
        static SharpCasterDemoController _controller;
        private readonly ChromecastService _chromecastService;

        bool useDLL = false;//determines if we use the vlc dll or the exe to handle video
        bool playing = true;
        bool playSequels = true;
        bool continousPlay = true;
        int currentVolume = 0;
        int lastPosition = 0;
        int positionStopCounter = 0;

        Media whatsNext;

        MediaHandler myMedia;

        public SpeechRecognition SpeechListener
        {
            set
            {
                speechListener = value;
                speechListener.loadGrammarFile(_grammarvocabFile);
                speechListener.executeCommand += new SpeechRecognition.HeardCommand(speechListener_executeCommand);
            }
        }

        public Media WhatsNext
        {
            get 
            {
                if (whatsNext != null)
                    return whatsNext;
                else
                    return myMedia.GetSequel(currentMedia, continousPlay);
            }

            set { whatsNext = value; }
        }

        public bool PlaySequel
        {
            get { return playSequels; }
            set { playSequels = value; }
        }

        public bool ContinousPlay
        {
            get { return continousPlay; }
            set { continousPlay = value; }
        }

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
        #endregion

        public liquidMediaPlayer()
        {
            InitializeComponent();

            SharpCaster.Test.ChromecastTester chromecast = new SharpCaster.Test.ChromecastTester();
            chromecast.TestingDeviceDiscoveryAsync();
            _chromecastService = ChromecastService.Current;
            var device = _chromecastService.StartLocatingDevices().Result;
            _chromecastService.ConnectToChromecast(device.First()).Wait(2000);

            //deactivate screensaver
            //SystemParametersInfo(17, 0, false, 0);

            //listen for speech commands
            //Form1.speechListener.executeCommand += new speechRecognition.heardCommand(speechListener_executeCommand);

            populateMacros();

            speakers = new Speakers();

            //set volume bar
            //pbVolume.Value = speakers.GetVolume();

            pnlMouseControls.Visible = true;

            //this stopped vlc from getting mouse events in the video window
            pnlCover.BringToFront();
            pnlCover.Focus();

            myMedia = new MediaHandler();

            //create and dock slidingBar
            //Button btnTest = new Button();
            //btnTest.Anchor = AnchorStyles.Right;
            //btnTest.Anchor = AnchorStyles.Bottom;
            //btnTest.Text = "Hi";
            //btnTest.Location = new Point(0, 0);

            //slidingBar = new SCTVControls.SlidingBar();
            //this.Controls.Add(slidingBar);

            //slidingBar.Controls.Add(btnTest);

            //pbProgress.Dock = DockStyle.Top;
            //slidingBar.Controls.Add(pbProgress);
            //pbVolume.Anchor = AnchorStyles.Right;
            //pbVolume.Anchor = AnchorStyles.Bottom;
            //slidingBar.Controls.Add(pbVolume);
            //btnClose.Anchor = AnchorStyles.Right;
            //btnClose.Anchor = AnchorStyles.Bottom;
            //slidingBar.Controls.Add(btnClose);
            //slidingBar.Visible = false;
            //slidingBar.Dock = DockStyle.Bottom;
        }

        private void castVLC(string mediaToPlay)
        {
            //vlc -vvv ./file.mp4 --sout "#chromecast{ip=192.168.1.XX,mux=mp4}"

            

            //DoStuff();

            //ProcessStartInfo startInfo = new ProcessStartInfo(@"C:\Program Files\VideoLAN\VLC\vlc.exe");
            //startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            //startInfo.Arguments = mediaToPlay;
            //startInfo.Arguments += " --fullscreen --quiet";
            //startInfo.Arguments += " --drop-late-frames";
            //startInfo.Arguments += " --disable-screensaver";
            //startInfo.Arguments += " --overlay";
            //startInfo.Arguments += " --rtsp-caching=1200";
            //startInfo.Arguments += " --sout \"#chromecast{ip=10.0.0.198,mux=mp4}\""; //bedroom

            //Process myProcess = new Process();
            //myProcess.StartInfo = startInfo;

            //myProcess.ErrorDataReceived += new DataReceivedEventHandler(myProcess_ErrorDataReceived);
            //myProcess.OutputDataReceived += new DataReceivedEventHandler(myProcess_OutputDataReceived);

            //myProcess.Start();

            //myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;

            //myProcess.Refresh();
        }

        private static async Task DoStuff()
        {
            ChromecastService.ChromeCastClient.ApplicationStarted += Client_ApplicationStarted;
            ChromecastService.ChromeCastClient.VolumeChanged += _client_VolumeChanged;
            ChromecastService.ChromeCastClient.MediaStatusChanged += ChromeCastClient_MediaStatusChanged;
            ChromecastService.ChromeCastClient.ConnectedChanged += ChromeCastClient_Connected;

            System.Console.WriteLine("Started locating chromecasts!");

            try
            {
                var devices = await ChromecastService.StartLocatingDevices();

                if (devices.Count == 0)
                {
                    System.Console.WriteLine("No chromecasts found");
                    return;
                }

                var firstChromecast = devices.First();
                System.Console.WriteLine("Device found " + firstChromecast.FriendlyName);
                ChromecastService.ConnectToChromecast(firstChromecast);
            }
            catch (Exception ex)
            {

                throw;
            }            
        }

        private static async void ChromeCastClient_Connected(object sender, EventArgs e)
        {
            System.Console.WriteLine("Connected to chromecast");
            if (_controller == null)
            {
                _controller = await ChromecastService.ChromeCastClient.LaunchSharpCaster();
            }
        }

        private async static void ChromeCastClient_MediaStatusChanged(object sender, MediaStatus e)
        {
            if (e.PlayerState == PlayerState.Playing)
            {
                await Task.Delay(2000);
                await ChromecastService.ChromeCastClient.DisconnectChromecast();
                _controller = null;
                await Task.Delay(5000);
                var devices = await ChromecastService.StartLocatingDevices();

                if (devices.Count == 0)
                {
                    System.Console.WriteLine("No chromecasts found");
                    return;
                }

                var firstChromecast = devices.First();
                System.Console.WriteLine("Device found " + firstChromecast.FriendlyName);
                ChromecastService.ConnectToChromecast(firstChromecast);
                await Task.Delay(5000);
                _controller = await ChromecastService.ChromeCastClient.LaunchSharpCaster();
                await Task.Delay(4000);
                var track = new Track
                {
                    Name = "English Subtitle",
                    TrackId = 100,
                    Type = "TEXT",
                    SubType = "captions",
                    Language = "en-US",
                    TrackContentId =
               "https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/tracks/DesigningForGoogleCast-en.vtt"
                };
                while (_controller == null)
                {
                    await Task.Delay(500);
                }

                await _controller.LoadMedia("https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/mp4/DesigningForGoogleCast.mp4", "video/mp4", null, "BUFFERED", 0D, null, new[] { track }, new[] { 100 });
            }
        }

        private static void _client_VolumeChanged(object sender, Volume e)
        {
        }

        private static async void Client_ApplicationStarted(object sender, ChromecastApplication e)
        {
            System.Console.WriteLine($"Application {e.DisplayName} has launched");
            var track = new Track
            {
                Name = "English Subtitle",
                TrackId = 100,
                Type = "TEXT",
                SubType = "captions",
                Language = "en-US",
                TrackContentId =
               "https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/tracks/DesigningForGoogleCast-en.vtt"
            };
            while (_controller == null)
            {
                await Task.Delay(500);
            }

            await _controller.LoadMedia("https://commondatastorage.googleapis.com/gtv-videos-bucket/CastVideos/mp4/DesigningForGoogleCast.mp4", "video/mp4", null, "BUFFERED", 0D, null, new[] { track }, new[] { 100 });
        }

        /// <summary>
        /// Start vlc exe
        /// </summary>
        private void startVLC(string mediaToPlay)
        {
            try
            {
                //ProcessStartInfo startInfo = new ProcessStartInfo(Application.StartupPath + "\\vlc\\vlc.exe");
                ProcessStartInfo startInfo = new ProcessStartInfo(@"C:\Program Files\VideoLAN\VLC\vlc.exe");
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;

                //Process.Start(startInfo);

                //startInfo.Arguments = "dvd://E:\\ --fullscreen --quiet";

                startInfo.Arguments = mediaToPlay;

                startInfo.Arguments += " --fullscreen --quiet";

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

                myProcess.ErrorDataReceived += new DataReceivedEventHandler(myProcess_ErrorDataReceived);
                myProcess.OutputDataReceived += new DataReceivedEventHandler(myProcess_OutputDataReceived);

                myProcess.Start();

                myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;

                myProcess.Refresh();
            }
            catch (Exception ex)
            {

            }
        }

        void myProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            //throw new Exception("The method or operation is not implemented.");
        }

        void myProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            //throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Record given media
        /// </summary>
        /// <param name="driveLetter"></param>
        /// <param name="fileToRecordTo"></param>
        public void RecordMedia(string driveLetter, string fileToRecordTo, bool skipMenu)
        {
            try
            {
                int vHandle = 0;
                ArrayList mediaToPlay;

                if (!skipMenu)
                {
                    vHandle = this.pnlVideo.Handle.ToInt32();

                    if (vHandle > 0 && fileToRecordTo.Length > 0)
                    {
                        mediaToPlay = new ArrayList();
                        mediaToPlay.Add(driveLetter);

                        vlc = new VLC();

                        vlc.SetOutputWindow(vHandle);
                        string[] options;

                        options = ToScreen(fileToRecordTo, false);

                        _playerTask = new PlayerTask(10, 0, vlc, options, mediaToPlay);
                        // _playerTask.Completed += new AsynTaskCompletedEventHandler(playerTask_Completed);
                        _playerTask.Start();

                        progressTimer.Start();

                        //Panel pnlVideoScreen = new Panel();
                        //pnlVideoScreen.Dock = DockStyle.Fill;
                        //pnlVideoScreen.BackColor = Color.Transparent;
                        //pnlVideoScreen.Click += new EventHandler(pnlVideoScreen_Click);

                        //pnlVideo.Controls.Add(pnlVideoScreen);
                        //pnlVideoScreen.BringToFront();
                    }
                }
                else
                {
                    startVLC("dvdsimple://" + driveLetter);
                }
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }
        }

        public void PlayMedia(ArrayList mediaToPlay, string fileToRecordTo, string lastPlayPosition)
        {
            PlayMedia(mediaToPlay, fileToRecordTo, lastPlayPosition);
        }

        /// <summary>
        /// Play given media
        /// </summary>
        /// <param name="mediaToPlay">arrayList of media objects to play</param>
        /// <param name="fileToRecordTo">if blank nothing will record</param>
        public void PlayMedia(ArrayList mediaToPlay, string fileToRecordTo, string lastPlayPosition, string mediaType)
        {
            try
            {
                int vHandle = 0;
                int resumePosition = 0;
                
                if (useDLL)
                {
                    int x = 0;
                    while (x < 100 && File.Exists(fileToRecordTo))
                    {
                        x++;

                        if(x == 1)
                            fileToRecordTo = fileToRecordTo.Replace(".", "_" + x.ToString() + ".");
                        else
                            fileToRecordTo = fileToRecordTo.Replace("_" + (x-1).ToString() + ".", "_" + x.ToString() + ".");
                    }

                    if (x == 100)
                    {
                        MessageBox.Show("There is no room for this file (" + fileToRecordTo + ")");
                        return;
                    }
                    pnlVideo.Enabled = false;
                    vHandle = this.pnlVideo.Handle.ToInt32();
                    if (vHandle > 0)
                    {
                        vlc = new VLC(mediaType);

                        vlc.SetOutputWindow(vHandle);
                        string[] options;
                        //string[] options = { "" };

                        if (fileToRecordTo.Length > 0)
                            options = ToScreen(fileToRecordTo);
                        else
                            options = ToScreen();

                        vlc.SetVolume(200);
                        //vlc.MediaType(mediaType);

                        if (int.TryParse(lastPlayPosition, out resumePosition))
                        {
                            DialogResult result = MessageBox.Show("Would you like to resume from where you left off before?", "Resume", MessageBoxButtons.YesNo);

                            if(result == DialogResult.Yes)
                                vlc.SetPositionSecs(resumePosition);
                        }
                        //--audio-visual=visual --effect-list=spectrometer
                        //vlc.setVariable("effect-list", 1);
                        //vlc.SetConfigVariable(":effect-list", "spectrometer");
                        //vlc.setVariable("--audio-visual", 1);
                        //vlc.setVariable("--effect-list", 1);

                        //vlc.SetConfigVariable("audio-visual","visual");
                        //vlc.addTarget("effect-list=spectrometer");

                        _playerTask = new PlayerTask(10, 0, vlc, options, mediaToPlay);
                        _playerTask.Completed += new AsynTaskCompletedEventHandler(playerTask_Completed);
                        _playerTask.Start();

                        progressTimer.Start();
                        inactivityTimer.Start();
                        //MessageBox.Show("starting inactivity timer");

                        //trying to get click on the video window
                        //Panel pnlVideoScreen = new Panel();

                        ////pnlVideoScreen.BackColor = Color.Transparent;

                        //pnlVideoScreen.Click += new EventHandler(pnlVideoScreen_Click);

                        //pnlVideo.Controls.Add(pnlVideoScreen);
                        //pnlVideoScreen.Dock = DockStyle.Fill;
                        //pnlVideoScreen.BringToFront();


                        //pnlVideo.Click += new EventHandler(pnlVideoScreen_Click);
                        //pnlVideo.SendToBack();




                        //                        ######################################## VLC
                        //#  Hot keys
                        //#       --key-fullscreen <integer> Fullscreen
                        //#       --key-play-pause <integer> Play/Pause
                        //#       --key-faster <integer>     Faster
                        //#       --key-slower <integer>     Slower
                        //#       --key-next <integer>       Next
                        //#       --key-prev <integer>       Previous
                        //#       --key-stop <integer>       Stop
                        //#       --key-jump-10sec <integer> Jump 10 seconds backwards
                        //#       --key-jump+10sec <integer> Jump 10 seconds forward
                        //#       --key-jump-1min <integer>  Jump 1 minute backwards
                        //#       --key-jump+1min <integer>  Jump 1 minute forward
                        //#       --key-jump-5min <integer>  Jump 5 minutes backwards
                        //#       --key-jump+5min <integer>  Jump 5 minutes forward
                        //#       --key-quit <integer>       Quit
                        //#       --key-vol-up <integer>     Volume up
                        //#       --key-vol-down <integer>   Volume down
                        //#       --key-vol-mute <integer>   Mute
                        //#       --key-audio-track <integer>
                        //#       Cycle audio track
                        //#       --key-subtitle-track <integer>
                        //#       Cycle subtitle track
                        //#

                        //ushort skey = 0x53; //S key
                        ushort ctrlPKey = 0x10; //ctrl + P 
                        ushort ctrlSKey = 0x13; //ctrl + S 
                        vlc.setVariable("key-play-pause", (int)ctrlPKey);
                        vlc.setVariable("key-stop", (int)ctrlSKey);
                        //vlc.setVariable("key-jump+5min ", (int)ctrlSKey);
                        
                        //vlc.setVariable("key-pressed", (int)skey);
                    }
                }
                else //use exe
                {
                    //startVLC(mediaToPlay[0].ToString());
                    castVLC(mediaToPlay[0].ToString());

                    //SCTVObjects.TriviaBar.ShowTriviaBar((Media)mediaToPlay[0]);
                }

                playing = true;
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }
        }

        public void PlayClip(Media mediaToPlay)
        {
            try
            {
                currentMedia = mediaToPlay;
                int starRating = 0;
                ArrayList mediaPaths = new ArrayList();

                string mediaToPlayFilePath = mediaToPlay.filePath;

                if (mediaToPlayFilePath.Contains("|"))
                {
                    mediaToPlayFilePath = mediaToPlayFilePath.Substring(0, mediaToPlayFilePath.IndexOf("|"));

                    string[] filePaths = mediaToPlay.filePath.Split('|');

                    foreach(string filepath in filePaths)
                    {
                        mediaPaths.Add(filepath);
                    }
                }
                else
                    mediaPaths.Add(mediaToPlayFilePath);

                //if justindownload get all related files
                //if (mediaToPlay.filePath.ToLower().Contains("justindownloads"))
                //{
                //    string currentFilePath = "";
                //    string currentTitle = mediaToPlay.filePath;
                //    currentTitle = currentTitle.Substring(0, currentTitle.IndexOf("_"));
                //    currentTitle = currentTitle.Substring(currentTitle.LastIndexOf("\\") + 1);

                //    //get related justin files
                //    DataTable sortableDt = MediaHandler.dsMedia.Tables[0].Copy();

                //    sortableDt.DefaultView.RowFilter = "filepath LIKE '*justindownloads*' and filepath LIKE '*" + currentTitle + "*'";

                //    sortableDt.DefaultView.Sort = "SortBy, Title";

                //    foreach (DataRowView dr in sortableDt.DefaultView)
                //    {
                //        currentFilePath = dr["filePath"].ToString();

                //        if(currentFilePath != mediaToPlay.filePath)//keep out the mediaToPlay it has already been added as the first item
                //            mediaPaths.Add(currentFilePath);
                //    }
                //}

                lblMediaTitle.Text = mediaToPlay.Title;

                int.TryParse(mediaToPlay.Stars, out starRating);

                StarRating.Rating = starRating;

                if (whatsNext == null)
                    whatsNext = myMedia.GetSequel(currentMedia, continousPlay);

                if(whatsNext != null)
                    lblWhatsNext.Text = "Next On SCTV: " + whatsNext.Title;

                PlayMedia(mediaPaths, "", mediaToPlay.LastPlayPosition, mediaToPlay.MediaType);
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }
        }

        public void PlayCategory(string category)
        {
            try
            {
                DataView dvMedia = myMedia.GetCategoryMedia(category,"","");
                ArrayList filesToPlay = new ArrayList();

                foreach (DataRowView dv in dvMedia)//go through the new category results
                {
                    string filePath = dv["filePath"].ToString();

                    if (filePath.Contains("|"))
                    {
                        string[] filePaths = filePath.Split('|');

                        foreach(string file in filePaths)
                            filesToPlay.Add(file);
                    }
                    else
                        filesToPlay.Add(filePath);
                }

                PlayMedia(filesToPlay, "", "","");
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }
        }

        public void PlayRemoveableMedia(string driveLetter, bool skipMenu)
        {
            PlayRemoveableMedia(driveLetter, "", skipMenu);
        }

        public void PlayRemoveableMedia(string driveLetter, string fileToRecordTo, bool skipMenu)
        {
            ArrayList mediaToPlay = new ArrayList();

            if(skipMenu)
                driveLetter = "dvdsimple://"+ driveLetter +"\\";
            else
                driveLetter = "dvd://" + driveLetter + "\\";

            mediaToPlay.Add(driveLetter);
            PlayMedia(mediaToPlay, fileToRecordTo, "", "");
        }

        public void RecordRemoveableMedia(string driveLetter, string fileToRecordTo, bool skipMenu)
        {
            RecordMedia(driveLetter, fileToRecordTo, skipMenu);
        }

        public void playerTask_Completed(object sender, AsynTaskCompletedEventArgs e)
        {
            //MessageBox.Show("done playing");

            //object[] state = (object[])e.Result;
            //label1.Text = (string)state[0];

            //executeMacros("close");
        }

        public void stop()
        {
            try
            {
                vlc.playlistClear();
                vlc.addTarget("vlc:quit");
                vlc.play();
                vlc.stop();
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }
        }

        public void pausePlay()
        {
            try
            {
                if (vlc.playing)
                {
                    vlc.pause();
                    progressTimer.Stop();
                }
                else
                {
                    vlc.play();
                    progressTimer.Start();
                }
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }
        }

        #region private void mediaPlayer_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        private void mediaPlayer_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            try
            {
                //changeChannelTimer.Enabled = true;
                //changeChannelTimer.Start();
                keyStrokeTracker.Add(e.KeyValue);
                string tempKeys = "";
                if (keyStrokeTracker.Count > 0)
                {
                    foreach (int intChannel in keyStrokeTracker)
                    {
                        tempKeys += intChannel.ToString() + ",";
                    }
                    tempKeys = tempKeys.Remove(tempKeys.LastIndexOf(","), 1);//removes trailing comma	
                }
                if (macroList.Contains(tempKeys))
                {
                    keyStrokeTracker.Clear();
                    string macroName = macroList.GetByIndex(macroList.IndexOfKey(tempKeys)).ToString();
                    //Tools.WriteToFile(Tools.errorFile,"executing macro " + macroName);
                    executeMacros(macroName);
                }
                else
                    Tools.WriteToFile(Tools.errorFile,"didn't find mediaPlayer macro for: " + tempKeys);

                //Tools.WriteToFile(Tools.errorFile,"KeyCode from mediaPlayer keyup: " + e.KeyCode);
                //Tools.WriteToFile(Tools.errorFile, "KeyValue from mediaPlayer keyup: " + e.KeyValue);
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }
        }
        #endregion

        #region private void changeChannelTimer_Tick(object sender, System.EventArgs e)
        private void changeChannelTimer_Tick(object sender, System.EventArgs e)
        {
            try
            {
                keyStrokeTracker.Clear();
                changeChannelTimer.Stop();
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }
        }
        #endregion

        #region void ToggleFullScreen()
        /// <summary> full-screen toggle. </summary>
        void ToggleFullScreen()
        {
            //try
            //{
            //    if ((clipType != ClipType.video) && (clipType != ClipType.audioVideo))
            //        return;
            //    if (videoWin == null)
            //        return;

            //    int mode;
            //    int hr = videoWin.get_FullScreenMode(out mode);
            //    Tools.WriteToFile(Tools.errorFile,"mode 1 in ToggleFullScreen " + mode.ToString());
            //    if (mode == DsHlp.OAFALSE)
            //    {
            //        hr = videoWin.get_MessageDrain(out drainWin);
            //        hr = videoWin.put_MessageDrain(this.Handle);
            //        mode = DsHlp.OATRUE;
            //        hr = videoWin.put_FullScreenMode(mode);
            //        if (hr >= 0)
            //            fullScreen = true;
            //    }
            //    else
            //    {
            //        mode = DsHlp.OAFALSE;
            //        hr = videoWin.put_FullScreenMode(mode);
            //        if (hr >= 0)
            //            fullScreen = false;
            //        hr = videoWin.put_MessageDrain(drainWin);
            //        //				this.BringToFront();
            //        //				this.Refresh();
            //    }
            //    videoWin.get_FullScreenMode(out mode);
            //    Tools.WriteToFile(Tools.errorFile,"mode 2 in ToggleFullScreen " + mode.ToString());
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
        }
        #endregion

        #region private void displayMediaInfo()
        private void displayMediaInfo()
        {
            try
            {
                //if (!txtMediaInfo.Visible)
                //{
                //    foreach (DataRowView dr in Form1.myMedia.searchByFilePath(clipFile))
                //    {
                //        FontStyle newFontStyle;
                //        Font currentFont = txtMediaInfo.SelectionFont;

                //        newFontStyle = FontStyle.Bold;
                //        txtMediaInfo.SelectionFont = new Font(currentFont.FontFamily, currentFont.Size, newFontStyle);
                //        txtMediaInfo.Text = dr["title"].ToString();
                //        txtDurationTime.Text = getDuration();
                //    }
                //    if (txtMediaInfo.Text.Length < 1)
                //        txtMediaInfo.Text = "didn't find media info";
                //    txtMediaInfo.Visible = true;
                //    txtMediaInfo.BringToFront();
                //}
                //else
                //    txtMediaInfo.Visible = false;
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }
        }
        #endregion

        #region Clock functions
        public string GetTime()
        {
            return DateTime.Now.ToShortTimeString();
        }

        private void clock_Tick(object sender, System.EventArgs e)
        {
            //txtClock.Text = GetTime();
        }
        #endregion

        #region private void currentPositionTimer_Tick(object sender, System.EventArgs e)
        private void currentPositionTimer_Tick(object sender, System.EventArgs e)
        {
            //txtCurrentPositionTime.Text = getCurrentPosition();
        }
        #endregion

        #region private string getCurrentPosition()
        private string getCurrentPosition()
        {
            try
            {
                double currentPosition;
                //mediaPos.get_CurrentPosition(out currentPosition);
                return "";// convertMilliseconds((int)currentPosition);
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
                return "";
            }
        }
        #endregion

        #region private string getDuration()
        private string getDuration()
        {
            try
            {
                double duration;
                //mediaPos.get_Duration(out duration);
                return "";// convertMilliseconds((int)duration);
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
                return "";
            }
        }
        #endregion

        #region private string convertMilliseconds(int milliSeconds)
        private string convertMilliseconds(int milliSeconds)
        {
            try
            {
                int s = milliSeconds;
                int h = s / 3600;
                int m = (s - (h * 3600)) / 60;
                s = s - (h * 3600 + m * 60);
                return String.Format("{0:D2}:{1:D2}:{2:D2}", h, m, s);
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
                return "";
            }
        }
        #endregion

        private void liquidMediaPlayer_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                //if (playState == PlayState.Init)
                //    return;

                //switch (e.KeyCode)
                //{
                //    //				case Keys.P:
                //    //				{	menuControlPause_Click( null, null ); break; }
                //    //				case Keys.S:
                //    //				{	menuControlStop_Click( null, null ); break; }
                //    case Keys.M:
                //        { menuControlMute_Click(null, null); break; }

                    //				case Keys.D1:
                    //				case Keys.Space:
                    //				{	menuControlStep_Click( null, null ); break; }
                    //				case Keys.D2:
                    //				{	StepFrames( 2 ); break; }
                    //				case Keys.D3:
                    //				{	StepFrames( 3 ); break; }
                    //				case Keys.D4:
                    //				{	StepFrames( 4 ); break; }
                    //				case Keys.D5:
                    //				{	StepFrames( 5 ); break; }
                    //				case Keys.D6:
                    //				{	StepFrames( 6 ); break; }
                    //				case Keys.D7:
                    //				{	StepFrames( 7 ); break; }
                    //				case Keys.D8:
                    //				{	StepFrames( 8 ); break; }
                    //				case Keys.D9:
                    //				{	StepFrames( 9 ); break; }

                    //				case Keys.H:
                    //				{	menuControlHalf_Click( null, null ); break; }
                    //				case Keys.T:
                    //				{	menuControlThreeQ_Click( null, null ); break; }
                    //				case Keys.N:
                    //				{	menuControlNormal_Click( null, null ); break; }
                    //				case Keys.D:
                    //				{	menuControlDouble_Click( null, null ); break; }
                    //
                    //				case Keys.F:
                    //				{	ToggleFullScreen(); break; }
                    //				case Keys.Enter:
                    //				{	ToggleFullScreen(); break; }
                    //				case Keys.Escape:
                    //				{
                    //					if( fullScreen )
                    //						ToggleFullScreen();
                    //					break;
                    //				}

                    //				case Keys.Left:
                    //				{	menuRateDecr_Click( null, null ); break; }
                    //				case Keys.Right:
                    //				{	menuRateIncr_Click( null, null ); break; }
                    //				case Keys.Up:
                    //				{	menuRateDouble_Click( null, null ); break; }
                    //				case Keys.Down:
                    //				{	menuRateHalf_Click( null, null ); break; }
                    //				case Keys.Back:
                    //				{	menuRateNormal_Click( null, null ); break; }
            //    }
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }
        }

        /// <summary>
        /// handle key up events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void liquidMediaPlayer_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                //changeChannelTimer.Enabled = true;
                //changeChannelTimer.Start();
                keyStrokeTracker.Add(e.KeyValue);
                string tempKeys = "";
                if (keyStrokeTracker.Count > 0)
                {
                    foreach (int intChannel in keyStrokeTracker)
                    {
                        tempKeys += intChannel.ToString() + ",";
                    }
                    tempKeys = tempKeys.Remove(tempKeys.LastIndexOf(","), 1);//removes trailing comma	
                }
                if (macroList.Contains(tempKeys))
                {
                    keyStrokeTracker.Clear();
                    string macroName = macroList.GetByIndex(macroList.IndexOfKey(tempKeys)).ToString();
                    //Tools.WriteToFile(Tools.errorFile,"executing macro " + macroName);
                    executeMacros(macroName);
                }
                else
                    Tools.WriteToFile(Tools.errorFile,"didn't find mediaPlayer macro for: " + tempKeys);

                //Tools.WriteToFile(Tools.errorFile,"KeyCode from mediaPlayer keyup: " + e.KeyCode);
                //Tools.WriteToFile(Tools.errorFile,"KeyValue from mediaPlayer keyup: " + e.KeyValue);
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }
        }

        /// <summary>
        /// these run immediatly when found
        /// </summary>
        private void populateMacros()
        {
            try
            {
                //comma delimited list of (string) KeyValue's , names

                //media controls
                macroList.Add("17,16,66", "Rewind");  //rewind
                macroList.Add("17,83", "Stop");  //Stop
                macroList.Add("17,16,70", "FF");  //FF
                macroList.Add("17,70", "Next");  //Next
                macroList.Add("17,66", "Previous");  //Previous
                macroList.Add("17,80", "Play/Pause");  //Play/Pause
                //			macroList.Add(17,16,66);  //Record
                //			macroList.Add("","Mute");  //mute

                //main menu items
                macroList.Add("17,16,18,36", "Option");  //home key
                macroList.Add("17,16,18,50", "tvKey");  //tv key
                macroList.Add("17,16,18,72", "fmKey");  //fm key
                macroList.Add("17,16,18,51", "musicKey");  //music key
                macroList.Add("17,16,18,52", "pictureKey");  //picture key
                macroList.Add("17,16,18,53", "videoKey");  //video key
                macroList.Add("17,16,18,49", "dvdKey");  //dvd key
                macroList.Add("17,16,18,70", "shuffleKey");  //shuffle key
                macroList.Add("17,16,18,66", "repeatKey");  //repeat key
                macroList.Add("13", "Enter");  //Enter
                macroList.Add("27", "Escape");  //Escape - close
                //			macroList.Add("166","previousChannel");  //Browser Back
                macroList.Add("37", "leftArrow");  //left Arrow - 
                macroList.Add("38", "upArrow");  //up arrow - 
                macroList.Add("39", "rightArrow");  //right Arrow - 
                macroList.Add("40", "downArrow");  //down Arrow - 
                macroList.Add("9", "tab");  //Tab

                //generic Key settings for main menu
                macroList.Add("113", "tvKey");  //F2 - tv key
                macroList.Add("114", "fmKey");  //F3 - fm key
                macroList.Add("116", "musicKey");  //F5 - music key
                macroList.Add("117", "pictureKey");  //F6 - picture key
                macroList.Add("118", "videoKey");  //F7 - video key
                macroList.Add("119", "dvdKey");  //F8 - dvd key
                
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }
        }

        /// <summary>
        /// executes macros immediately
        /// </summary>
        /// <param name="macroName"></param>
        private void executeMacros(string macroName)
        {
            try
            {
                int currentPosition = 0;

                //Tools.WriteToFile(Tools.errorFile,"calling macro in liquidMedia " + macroName);
                keyStrokeTracker.Clear();
                switch (macroName)
                {
                    case "pause":
                    case "play":
                    case "pausePlay":
                        this.pausePlay();
                        break;
                    case "stop":
                        progressTimer.Stop();
                        executeMacros("close");
                        break;
                    case "close":
                        this.Close();
                        break;
                    case "Rewind":
                        currentPosition = vlc.GetPositionSecs();
                        vlc.SetPositionSecs(currentPosition - 10);
                        break;
                    case "FF":
                        currentPosition = vlc.GetPositionSecs();
                        vlc.SetPositionSecs(currentPosition + 10);
                        break;
                    case "Next":
                        if(vlc != null)
                            vlc.next();
                        break;
                    case "Previous":
                        if (vlc != null)
                            vlc.previous();
                        break;
                    case "ChannelUp":
                        break;
                    case "ChannelDown":
                        break;
                    case "channel":
                        break;
                    case "Enter":
                        displayMediaInfo();
                        break;
                    case "Option":
                        this.Close();
                        break;
                    case "Space":
                        break;
                    case "Back":
                        break;
                    case "tab"://add a bookmark

                        //					ToggleFullScreen();
                        break;
                    case "leftArrow":
                        //Tools.WriteToFile(Tools.errorFile,"stepping frames left");
                        //timeSkip(-25);
                        break;
                    case "upArrow":
                        //Tools.WriteToFile(Tools.errorFile,"stepping frames up");
                        //timeSkip(200);
                        break;
                    case "rightArrow":
                        //Tools.WriteToFile(Tools.errorFile,"stepping frames right");
                        //timeSkip(25);
                        break;
                    case "downArrow":
                        //Tools.WriteToFile(Tools.errorFile,"stepping frames down");
                        //timeSkip(-200);
                        break;
                    case "Escape": //Escape - close
                        executeMacros("close");
                        break;
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
                Tools.WriteToFile(ex);
            }
        }

        public void executePhrase(Phrase phrase)
        {
            //executeMacros(phrase.phrase);

            switch (phrase.phrase.ToLower())
            {
                case ("pause"):
                case ("play"):
                case ("stop"):
                case ("close"):
                    executeMacros(phrase.phrase);
                    break;
            }
        }

        /// <summary>
        /// Execute speech commands for MediaPlayer
        /// </summary>
        private void speechListener_executeCommand(Phrase thePhrase)
        {
            if (thePhrase.RuleName != "")// && accuracy >= accuracyLimit)
            {
                //EventLog.WriteEntry("SCTV Speech player", thePhrase.phrase + " - " + thePhrase.Accuracy.ToString());

                //if(accuracy >= accuracyLimit)
                //if(thePhrase.Accuracy > 30)
                executePhrase(thePhrase);
                //else
                //    EventLog.WriteEntry("SCTV Speech player", thePhrase.phrase + " - " + thePhrase.Accuracy.ToString());
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            try
            {
                vlc.stop();

                vlc = null;

                GC.Collect();

                //stop listening for speech commands from this form
                //Form1.speechListener.executeCommand -= new speechRecognition.heardCommand(speechListener_executeCommand);

                base.OnClosing(e);

                this.Dispose();

                //alert any other forms that this one is closing
                //closingForm();
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }
        }

        private void pnlVideo_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                executeMacros("close");
        }

        private void liquidMediaPlayer_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                this.Close();
        }

        private void btnPlayPause_Click(object sender, EventArgs e)
        {
            executeMacros("play");
            
            if (playing)
                btnPlayPause.Image = global::MediaPlayer.Properties.Resources.Pause_WhiteOnDKBlue_button;
            else
                btnPlayPause.Image = global::MediaPlayer.Properties.Resources.Play_WhiteOnDKBlue_button;

            playing = !playing;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            this.playSequels = false;
            this.continousPlay = false;

            executeMacros("close");
        }

        private void btnRewind_Click(object sender, EventArgs e)
        {
            executeMacros("Rewind");
        }

        private void btnFastForward_Click(object sender, EventArgs e)
        {
            executeMacros("FF");
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            executeMacros("Next");
        }

        private void btnVolumeUp_Click(object sender, EventArgs e)
        {
            executeMacros("volume up");
        }

        private void btnVolumeDown_Click(object sender, EventArgs e)
        {
            executeMacros("volume down");
        }

        private void btnMute_Click(object sender, EventArgs e)
        {
            executeMacros("mute");
            //ushort skey = 0x53; //S key
            //vlc.setVariable("key-jump+short", (int)skey);
            //vlc.setVariable("key-pressed", (int)skey);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.playSequels = false;
            this.continousPlay = false;

            executeMacros("close");
        }

        void ModifyRate(double rateAdjust)
        {
            try
            {
                if ((mediaPos == null) || (rateAdjust == 0.0))
                    return;

                double rate;
                int hr = mediaPos.get_Rate(out rate);
                if (hr < 0)
                    return;
                rate += rateAdjust;
                hr = mediaPos.put_Rate(rate);
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }
        }

        private void progressTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                double newPBPosition;
                float position = vlc.GetPosition();
                int currentPosition = vlc.GetPositionSecs();
                
                if(currentPosition == 0)
                    currentPosition = (int)(vlc.GetPosition() * 10000);

                int length = vlc.GetLengthSecs();
                
                if (currentPosition > 1)
                {
                    this.lblLoading.Visible = false;

                    if(length > 0)
                        newPBPosition = (double)currentPosition / (double)length * 100;
                    else
                        newPBPosition = (double)currentPosition / 100;

                    //update progress bar
                    if (pbProgress.Value >= pbProgress.Maximum)
                        pbProgress.Value = 0;

                    pbProgress.Value = (int)newPBPosition;

                    //if (mediaSplash != null && currentPosition > 2) //the clip has started
                    if (SplashScreenNew.SplashForm != null) //the clip has started
                    {
                        SplashScreenNew.CloseForm();
                        //mediaSplash.Close();

                        if (this.StartedPlaying != null)
                            this.StartedPlaying();

                        this.Show();
                        this.TopMost = true;
                    }
                }

                //if (currentPosition < lastPosition || lastPosition == currentPosition || currentPosition < 0)//close player - we are at the end of the video
                if(currentPosition == -20 && lastPosition == -20)
                {
                    positionStopCounter++;

                    if (positionStopCounter > 3)//when we have checked 3 times we will end
                    {
                        try
                        {
                            if (playSequels)
                                this.PlaySequels();

                            if (continousPlay)
                                this.ContinousPlayOn();

                            if (this.MediaDonePlaying != null)
                                this.MediaDonePlaying();
                        }
                        catch (Exception ex)
                        {
                            Tools.WriteToFile(ex);
                        }                        

                        positionStopCounter = 0;

                        executeMacros("close");
                    }
                }

                //update progress label
                TimeSpan timeSpan = TimeSpan.FromSeconds((double)currentPosition);
                lblProgress.Text = timeSpan.Hours + ":" + timeSpan.Minutes + ":" + timeSpan.Seconds;

                //if (lastPosition != currentPosition)
                //    positionStopCounter = 0;

                lastPosition = currentPosition;
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }
        }

        private void pbProgress_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                double percentage = (double)e.X / (double)pbProgress.Width;
                int newPosition = (int)(vlc.GetLengthSecs() * percentage);

                //if (newPosition == 0)
                //    newPosition = (int)(vlc.GetPosition() * 10000 * percentage);

                vlc.SetPositionSecs(newPosition);
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }
        }

        private void inactivityTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                //make sure mouse isn't on mouse controls
                if (Cursor.Position.Y < pnlMouseControls.Location.Y)
                    inactivityTicker++;

                //hide mouse controls
                if (inactivityTicker > inactivityTime)
                {
                    int distanceToMove = pnlMouseControls.Height + 2;
                    int distanceMoved = 0;

                    inactivityTimer.Stop();

                    //hide pnlMouseControls
                    while (pnlMouseControls.Height > 10)
                    {
                        pnlMouseControls.Height = pnlMouseControls.Height - 1;
                        distanceMoved += 1;
                    }

                    //covers the mouse controls with a black panel
                    pnlCover.Visible = true;
                    pnlCover.BringToFront();
                    pnlCover.Focus();

                    //hide mouse on right side of screen
                    Cursor.Position = new Point(pnlMouseControls.Width, Cursor.Position.Y);
                }
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }
        }

        private void liquidMediaPlayer_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                //update lastplayposition
                currentMedia.LastPlayPosition = vlc.GetPositionSecs().ToString();
                myMedia.UpdateMediaInfo(currentMedia);

                //make sure cursor is visible
                Cursor.Show();

                //call event
                this.closingForm();
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }
        }

        /// <summary>
        /// show mouse controls
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pnlMouseControls_MouseMove(object sender, MouseEventArgs e)
        {
            pnlCover.Visible = false;
            pnlMouseControls.Visible = true;

            //show pnlMouseControls
            pnlMouseControls.Height = 50;

            pnlMouseControls.BackColor = Color.Transparent;

            inactivityTimer.Start();

            inactivityTicker = 0;
        }

        /// <summary>
        /// start timer if mouse is off of mouse controls
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pnlMouseControls_MouseLeave(object sender, EventArgs e)
        {
            ////start timer if mouse is off of mouse controls
            //if (Cursor.Position.Y < pnlMouseControls.Location.Y)
            //    inactivityTimer.Start();
        }

        public string[] ToScreen()
        {
            return ToScreen("", true);
        }
        
        public string[] ToScreen(string fileToRecordTo)
        {
            return ToScreen(fileToRecordTo, true);
        }

        public string[] ToScreen(string fileToRecordTo, bool displayToScreen)
        {
            //:--fullscreen --no-mouse-events  //found these but can't seem to get them to work
            string[] options = new string[3];

            options[0] = "config=\"~/Application Data/vlc/vlcrc\"";
            options[1] = ":file-caching=500 :sout-udp-caching=500";
            //options[2] = ":audio-visual=visual :effect-list=spectrometer";
            //options[3] = "--audio-visual=visual --effect-list=spectrum";
            //options[3] = "--audio-filter spectrum --audio-visual visual --effect-list spectrum";
            //options[2] = ":audio-visual=#visualize";
            //options[3] = ":effect-list=#scope";
            //options[5] = ":audio-filter=spectrum";
            //options[3] = ":audio-visual=#visual";
            //options[4] = ":effect-list=#scope";

            if (fileToRecordTo.Length > 2)
            {
                if (displayToScreen)//display output to screen and record
                {
                        //options[2] = ":sout=#transcode{vcodec=mp4v,vb=512,scale=1}:duplicate{dst=display,dst=std{access=file,mux=ts,dst=\"" + fileToRecordTo + "\"}}";
                        ////options[2] = ":sout=#transcode{vcodec=mp4v,vb=3000,scale=1}:duplicate{dst=display,dst=std{access=file,mux=ts,dst=\"" + fileToRecordTo + "\"}}";



                        ////worked - 3gig for indiana jones - no pixelation
                        options[2] = ":sout=#duplicate{dst=display,dst=std{access=file,mux=ts,dst=\"" + fileToRecordTo + "\"}}";
                }
                else //record only
                    options[2] = ":sout=#duplicate{dst=std{access=file,mux=ts,dst=\"" + fileToRecordTo + "\"}}";
            }
            else//play only
                options[2] = ":audio-visual=visual :effect-list=spectrometer";
            //options[2] = ":sout=#duplicate{dst=display}";
            //options[2] = ":sout=#duplicate{dst=display,audio-visual=visual,effect-list=spectrum}";

            return options;
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //show context menu
                CameraContextMenuStrip.Show(this, this.PointToClient(Cursor.Position));
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void camera1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SCTVCamera.SCTVCameraMain camera = new SCTVCamera.SCTVCameraMain();
            camera.SpeechListener = speechListener;
            //camera.ShowCameraByName("camera 1");
        }

        private void camera2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SCTVCamera.SCTVCameraMain camera = new SCTVCamera.SCTVCameraMain();
            camera.SpeechListener = speechListener;
            //camera.ShowCameraByName("camera 2");
        }

        private void pnlVideoScreen_Click(object sender, EventArgs e)
        {
            MessageBox.Show("click");
        }

        private void chbSequels_CheckedChanged(object sender, EventArgs e)
        {
            playSequels = chbSequels.Checked;
        }

        private void chbContinousPlay_CheckedChanged(object sender, EventArgs e)
        {
            continousPlay = chbContinousPlay.Checked;
        }

        private void starRating_RatingValueChanged(object sender, RatingChangedEventArgs e)
        {
            //update xml with new rating
            if (myMedia == null)
                myMedia = new MediaHandler();

            currentMedia.Stars = StarRating.Rating.ToString();
            myMedia.UpdateMediaInfo(currentMedia);
        }

        //display info about the currently playing media
        private void btnMediaInfo_Click(object sender, EventArgs e)
        {
            SCTVObjects.MediaDetails details = new MediaDetails(currentMedia);
            details.ReadOnly = true;
            details.ShowDialog(this);
        }

        private void liquidMediaPlayer_FormClosed(object sender, FormClosedEventArgs e)
        {
            //this.Dispose(true);
            //this.Dispose();

            //this.pnlVideo.Handle = null;
            //GC.Collect();
        }

        private void lblWhatsNext_MouseClick(object sender, MouseEventArgs e)
        {
            if(myMedia == null)
                 myMedia = new MediaHandler();

            whatsNext = myMedia.GetSequel(currentMedia, true);
            lblWhatsNext.Text = "Next On SCTV: " + whatsNext.Title;
        }
    }
}||||||| .r0
=======
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Diagnostics;
using DShowNET;
using SCTVObjects;

namespace SCTV
{
    public partial class liquidMediaPlayer : Form
    {
        #region Variables
        private VLC vlc;
        private string _fileName = string.Empty;
        private string _defaultFile = string.Empty;
        AsynTask _playerTask;
        private ArrayList keyStrokeTracker = new ArrayList();
        private SortedList macroList = new SortedList();
        private System.Windows.Forms.Timer changeChannelTimer;

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern System.Int32 SystemParametersInfo(System.UInt32 uiAction, System.UInt32 uiParam,
            System.IntPtr pvParam, System.UInt32 fWinIni);
        [DllImport("user32", CharSet = CharSet.Auto)]
        private static extern bool SystemParametersInfo(int uAction, int uParam, bool lpvParam, int fuWinIni);
        [DllImport("kernel32.dll")]
        static extern bool SetProcessWorkingSetSize(IntPtr hProcess, int dwMinimumWorkingSetSize, int dwMaximumWorkingSetSize);

        private bool _capturing;
        private Image _dropperBoxed;
        private Image _dropperUnboxed;
        private Cursor _cursorDefault;
        private Cursor _cursorDropper;
        private IntPtr _hPreviousWindow;

        private string _Handle = string.Empty;
        private string _Class = string.Empty;
        private string _Text = string.Empty;
        private string _Style = string.Empty;
        private string _Rect = string.Empty;
        private int inactivityTicker = 0;
        private int inactivityTime = 5;//time in seconds before no action is considered inactivity
        Media currentMedia;

        //[DllImport("gdi32.dll")]
        //public static extern IntPtr ExtCreateRegion(int lpXform, int nCount, byte[] lpRgnData);

        //[DllImport("user32.dll")]
        //internal static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool bRedraw);

        //static private SHDocVw.ShellWindows shellWindows = new SHDocVw.ShellWindowsClass();
        public SHDocVw.ShellWindows SWs;
        public SHDocVw.InternetExplorer IE;

        public delegate void closing();
        public event closing closingForm;

        public delegate void MediaDone();
        public event MediaDone MediaDonePlaying;

        public delegate void ContinousPlaySelected();
        public event ContinousPlaySelected ContinousPlayOn;

        public delegate void Sequels();
        public event Sequels PlaySequels;

        public delegate void startedPlaying();
        public event startedPlaying StartedPlaying;

        public string _grammarvocabFile = "xmlvideoplayer.xml";

        /// <summary> seek interface to set position in stream. </summary>
        private IMediaPosition mediaPos;

        private SCTV.SlidingBar slidingBar = null;

        private Speakers speakers;
        SpeechRecognition speechListener;

        bool useDLL = true;//determines if we use the vlc dll or the exe to handle video

        bool playSequels = true;
        bool continousPlay = true;
        int currentVolume = 0;
        int lastPosition = 0;
        int positionStopCounter = 0;

        Media whatsNext;

        MediaHandler myMedia;

        public SpeechRecognition SpeechListener
        {
            set
            {
                speechListener = value;
                speechListener.loadGrammarFile(_grammarvocabFile);
                speechListener.executeCommand += new SpeechRecognition.HeardCommand(speechListener_executeCommand);
            }
        }

        public Media WhatsNext
        {
            get 
            {
                if (whatsNext != null)
                    return whatsNext;
                else
                    return myMedia.GetSequel(currentMedia, continousPlay);
            }

            set { whatsNext = value; }
        }

        public bool PlaySequel
        {
            get { return playSequels; }
            set { playSequels = value; }
        }

        public bool ContinousPlay
        {
            get { return continousPlay; }
            set { continousPlay = value; }
        }

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
        #endregion

        public liquidMediaPlayer()
        {
            InitializeComponent();

            //deactivate screensaver
            //SystemParametersInfo(17, 0, false, 0);

            //listen for speech commands
            //Form1.speechListener.executeCommand += new speechRecognition.heardCommand(speechListener_executeCommand);
            
            populateMacros();

            speakers = new Speakers();

            //set volume bar
            //pbVolume.Value = speakers.GetVolume();

            pnlMouseControls.Visible = true;

            myMedia = new MediaHandler();

            //create and dock slidingBar
            //Button btnTest = new Button();
            //btnTest.Anchor = AnchorStyles.Right;
            //btnTest.Anchor = AnchorStyles.Bottom;
            //btnTest.Text = "Hi";
            //btnTest.Location = new Point(0, 0);

            //slidingBar = new SCTVControls.SlidingBar();
            //this.Controls.Add(slidingBar);

            //slidingBar.Controls.Add(btnTest);

            //pbProgress.Dock = DockStyle.Top;
            //slidingBar.Controls.Add(pbProgress);
            //pbVolume.Anchor = AnchorStyles.Right;
            //pbVolume.Anchor = AnchorStyles.Bottom;
            //slidingBar.Controls.Add(pbVolume);
            //btnClose.Anchor = AnchorStyles.Right;
            //btnClose.Anchor = AnchorStyles.Bottom;
            //slidingBar.Controls.Add(btnClose);
            //slidingBar.Visible = false;
            //slidingBar.Dock = DockStyle.Bottom;
        }

        /// <summary>
        /// Start vlc exe
        /// </summary>
        private void startVLC(string mediaToPlay)
        {
            try
            {
                //ProcessStartInfo startInfo = new ProcessStartInfo(Application.StartupPath + "\\vlc\\vlc.exe");
                ProcessStartInfo startInfo = new ProcessStartInfo(@"C:\Program Files\VideoLAN\VLC\vlc.exe");
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;

                //Process.Start(startInfo);

                //startInfo.Arguments = "dvd://E:\\ --fullscreen --quiet";

                startInfo.Arguments = mediaToPlay;

                startInfo.Arguments += " --fullscreen --quiet";

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

                myProcess.ErrorDataReceived += new DataReceivedEventHandler(myProcess_ErrorDataReceived);
                myProcess.OutputDataReceived += new DataReceivedEventHandler(myProcess_OutputDataReceived);

                myProcess.Start();

                myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;

                myProcess.Refresh();
            }
            catch (Exception ex)
            {

            }
        }

        void myProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            //throw new Exception("The method or operation is not implemented.");
        }

        void myProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            //throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Record given media
        /// </summary>
        /// <param name="driveLetter"></param>
        /// <param name="fileToRecordTo"></param>
        public void RecordMedia(string driveLetter, string fileToRecordTo, bool skipMenu)
        {
            try
            {
                int vHandle = 0;
                ArrayList mediaToPlay;

                if (!skipMenu)
                {
                    vHandle = this.pnlVideo.Handle.ToInt32();

                    if (vHandle > 0 && fileToRecordTo.Length > 0)
                    {
                        mediaToPlay = new ArrayList();
                        mediaToPlay.Add(driveLetter);

                        vlc = new VLC();

                        vlc.SetOutputWindow(vHandle);
                        string[] options;

                        options = ToScreen(fileToRecordTo, false);

                        _playerTask = new PlayerTask(10, 0, vlc, options, mediaToPlay);
                        // _playerTask.Completed += new AsynTaskCompletedEventHandler(playerTask_Completed);
                        _playerTask.Start();

                        progressTimer.Start();

                        //Panel pnlVideoScreen = new Panel();
                        //pnlVideoScreen.Dock = DockStyle.Fill;
                        //pnlVideoScreen.BackColor = Color.Transparent;
                        //pnlVideoScreen.Click += new EventHandler(pnlVideoScreen_Click);

                        //pnlVideo.Controls.Add(pnlVideoScreen);
                        //pnlVideoScreen.BringToFront();
                    }
                }
                else
                {
                    startVLC("dvdsimple://" + driveLetter);
                }
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }
        }

        /// <summary>
        /// Play given media
        /// </summary>
        /// <param name="mediaToPlay">arrayList of media objects to play</param>
        /// <param name="fileToRecordTo">if blank nothing will record</param>
        public void PlayMedia(ArrayList mediaToPlay, string fileToRecordTo, string lastPlayPosition)
        {
            try
            {
                int vHandle = 0;
                int resumePosition = 0;
                
                if (useDLL)
                {
                    int x = 0;
                    while (x < 100 && File.Exists(fileToRecordTo))
                    {
                        x++;

                        if(x == 1)
                            fileToRecordTo = fileToRecordTo.Replace(".", "_" + x.ToString() + ".");
                        else
                            fileToRecordTo = fileToRecordTo.Replace("_" + (x-1).ToString() + ".", "_" + x.ToString() + ".");
                    }

                    if (x == 100)
                    {
                        MessageBox.Show("There is no room for this file (" + fileToRecordTo + ")");
                        return;
                    }

                    vHandle = this.pnlVideo.Handle.ToInt32();
                    if (vHandle > 0)
                    {
                        vlc = new VLC();

                        vlc.SetOutputWindow(vHandle);
                        string[] options;
                        //string[] options = { "" };

                        if (fileToRecordTo.Length > 0)
                            options = ToScreen(fileToRecordTo);
                        else
                            options = ToScreen();

                        vlc.SetVolume(200);

                        if (int.TryParse(lastPlayPosition, out resumePosition))
                        {
                            DialogResult result = MessageBox.Show("Would you like to resume from where you left off before?", "Resume", MessageBoxButtons.YesNo);

                            if(result == DialogResult.Yes)
                                vlc.SetPositionSecs(resumePosition);
                        }

                        _playerTask = new PlayerTask(10, 0, vlc, options, mediaToPlay);
                        _playerTask.Completed += new AsynTaskCompletedEventHandler(playerTask_Completed);
                        _playerTask.Start();

                        progressTimer.Start();
                        inactivityTimer.Start();

                        //trying to get click on the video window
                        //Panel pnlVideoScreen = new Panel();
                        
                        //pnlVideoScreen.BackColor = Color.Transparent;
                         
                        //pnlVideoScreen.Click += new EventHandler(pnlVideoScreen_Click);

                        //pnlVideo.Controls.Add(pnlVideoScreen);
                        //pnlVideoScreen.Dock = DockStyle.Fill;
                        //pnlVideoScreen.BringToFront();


                        //pnlVideo.Click += new EventHandler(pnlVideoScreen_Click);



//                        ######################################## VLC
//#  Hot keys
//#       --key-fullscreen <integer> Fullscreen
//#       --key-play-pause <integer> Play/Pause
//#       --key-faster <integer>     Faster
//#       --key-slower <integer>     Slower
//#       --key-next <integer>       Next
//#       --key-prev <integer>       Previous
//#       --key-stop <integer>       Stop
//#       --key-jump-10sec <integer> Jump 10 seconds backwards
//#       --key-jump+10sec <integer> Jump 10 seconds forward
//#       --key-jump-1min <integer>  Jump 1 minute backwards
//#       --key-jump+1min <integer>  Jump 1 minute forward
//#       --key-jump-5min <integer>  Jump 5 minutes backwards
//#       --key-jump+5min <integer>  Jump 5 minutes forward
//#       --key-quit <integer>       Quit
//#       --key-vol-up <integer>     Volume up
//#       --key-vol-down <integer>   Volume down
//#       --key-vol-mute <integer>   Mute
//#       --key-audio-track <integer>
//#       Cycle audio track
//#       --key-subtitle-track <integer>
//#       Cycle subtitle track
//#

                        //ushort skey = 0x53; //S key
                        ushort ctrlPKey = 0x10; //ctrl + P 
                        ushort ctrlSKey = 0x13; //ctrl + S 
                        vlc.setVariable("key-play-pause", (int)ctrlPKey);
                        vlc.setVariable("key-stop", (int)ctrlSKey);
                        //vlc.setVariable("key-jump+5min ", (int)ctrlSKey);
                        
                        //vlc.setVariable("key-pressed", (int)skey);
                    }
                }
                else //use exe
                {
                    startVLC(mediaToPlay[0].ToString());

                    SCTVObjects.TriviaBar.ShowTriviaBar((Media)mediaToPlay[0]);
                }
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }
        }

        public void PlayClip(Media mediaToPlay)
        {
            try
            {
                currentMedia = mediaToPlay;
                int starRating = 0;
                ArrayList mediaPaths = new ArrayList();

                mediaPaths.Add(mediaToPlay.filePath);

                //if justindownload get all related files
                //if (mediaToPlay.filePath.ToLower().Contains("justindownloads"))
                //{
                //    string currentFilePath = "";
                //    string currentTitle = mediaToPlay.filePath;
                //    currentTitle = currentTitle.Substring(0, currentTitle.IndexOf("_"));
                //    currentTitle = currentTitle.Substring(currentTitle.LastIndexOf("\\") + 1);

                //    //get related justin files
                //    DataTable sortableDt = MediaHandler.dsMedia.Tables[0].Copy();

                //    sortableDt.DefaultView.RowFilter = "filepath LIKE '*justindownloads*' and filepath LIKE '*" + currentTitle + "*'";

                //    sortableDt.DefaultView.Sort = "SortBy, Title";

                //    foreach (DataRowView dr in sortableDt.DefaultView)
                //    {
                //        currentFilePath = dr["filePath"].ToString();

                //        if(currentFilePath != mediaToPlay.filePath)//keep out the mediaToPlay it has already been added as the first item
                //            mediaPaths.Add(currentFilePath);
                //    }
                //}

                lblMediaTitle.Text = mediaToPlay.Title;

                int.TryParse(mediaToPlay.Stars, out starRating);

                StarRating.Rating = starRating;

                if (whatsNext == null)
                    whatsNext = myMedia.GetSequel(currentMedia, continousPlay);

                lblWhatsNext.Text = "Next On SCTV: " + whatsNext.Title;

                PlayMedia(mediaPaths, "", mediaToPlay.LastPlayPosition);
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }
        }

        public void PlayCategory(string category)
        {
            try
            {
                //DataView dvMedia = Form1.myMedia.getCategory(category);
                //ArrayList filesToPlay = new ArrayList();

                //foreach (DataRowView dv in dvMedia)//go through the new category results
                //    filesToPlay.Add(dv["filePath"].ToString());

                //PlayMedia(filesToPlay, "");
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }
        }

        public void PlayRemoveableMedia(string driveLetter, bool skipMenu)
        {
            PlayRemoveableMedia(driveLetter, "", skipMenu);
        }

        public void PlayRemoveableMedia(string driveLetter, string fileToRecordTo, bool skipMenu)
        {
            ArrayList mediaToPlay = new ArrayList();

            if(skipMenu)
                driveLetter = "dvdsimple://"+ driveLetter +"\\";
            else
                driveLetter = "dvd://" + driveLetter + "\\";

            mediaToPlay.Add(driveLetter);
            PlayMedia(mediaToPlay, fileToRecordTo, "");
        }

        public void RecordRemoveableMedia(string driveLetter, string fileToRecordTo, bool skipMenu)
        {
            RecordMedia(driveLetter, fileToRecordTo, skipMenu);
        }

        public void playerTask_Completed(object sender, AsynTaskCompletedEventArgs e)
        {
            //MessageBox.Show("done playing");

            //object[] state = (object[])e.Result;
            //label1.Text = (string)state[0];

            //executeMacros("close");
        }

        public void stop()
        {
            try
            {
                vlc.playlistClear();
                vlc.addTarget("vlc:quit");
                vlc.play();
                vlc.stop();
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }
        }

        public void pausePlay()
        {
            try
            {
                if (vlc.playing)
                {
                    vlc.pause();
                    progressTimer.Stop();
                }
                else
                {
                    vlc.play();
                    progressTimer.Start();
                }
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }
        }

        #region private void mediaPlayer_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        private void mediaPlayer_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            try
            {
                //changeChannelTimer.Enabled = true;
                //changeChannelTimer.Start();
                keyStrokeTracker.Add(e.KeyValue);
                string tempKeys = "";
                if (keyStrokeTracker.Count > 0)
                {
                    foreach (int intChannel in keyStrokeTracker)
                    {
                        tempKeys += intChannel.ToString() + ",";
                    }
                    tempKeys = tempKeys.Remove(tempKeys.LastIndexOf(","), 1);//removes trailing comma	
                }
                if (macroList.Contains(tempKeys))
                {
                    keyStrokeTracker.Clear();
                    string macroName = macroList.GetByIndex(macroList.IndexOfKey(tempKeys)).ToString();
                    //Tools.WriteToFile(Tools.errorFile,"executing macro " + macroName);
                    executeMacros(macroName);
                }
                else
                    Tools.WriteToFile(Tools.errorFile,"didn't find mediaPlayer macro for: " + tempKeys);

                //Tools.WriteToFile(Tools.errorFile,"KeyCode from mediaPlayer keyup: " + e.KeyCode);
                //Tools.WriteToFile(Tools.errorFile, "KeyValue from mediaPlayer keyup: " + e.KeyValue);
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }
        }
        #endregion

        #region private void changeChannelTimer_Tick(object sender, System.EventArgs e)
        private void changeChannelTimer_Tick(object sender, System.EventArgs e)
        {
            try
            {
                keyStrokeTracker.Clear();
                changeChannelTimer.Stop();
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }
        }
        #endregion

        #region void ToggleFullScreen()
        /// <summary> full-screen toggle. </summary>
        void ToggleFullScreen()
        {
            //try
            //{
            //    if ((clipType != ClipType.video) && (clipType != ClipType.audioVideo))
            //        return;
            //    if (videoWin == null)
            //        return;

            //    int mode;
            //    int hr = videoWin.get_FullScreenMode(out mode);
            //    Tools.WriteToFile(Tools.errorFile,"mode 1 in ToggleFullScreen " + mode.ToString());
            //    if (mode == DsHlp.OAFALSE)
            //    {
            //        hr = videoWin.get_MessageDrain(out drainWin);
            //        hr = videoWin.put_MessageDrain(this.Handle);
            //        mode = DsHlp.OATRUE;
            //        hr = videoWin.put_FullScreenMode(mode);
            //        if (hr >= 0)
            //            fullScreen = true;
            //    }
            //    else
            //    {
            //        mode = DsHlp.OAFALSE;
            //        hr = videoWin.put_FullScreenMode(mode);
            //        if (hr >= 0)
            //            fullScreen = false;
            //        hr = videoWin.put_MessageDrain(drainWin);
            //        //				this.BringToFront();
            //        //				this.Refresh();
            //    }
            //    videoWin.get_FullScreenMode(out mode);
            //    Tools.WriteToFile(Tools.errorFile,"mode 2 in ToggleFullScreen " + mode.ToString());
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
        }
        #endregion

        #region private void displayMediaInfo()
        private void displayMediaInfo()
        {
            try
            {
                //if (!txtMediaInfo.Visible)
                //{
                //    foreach (DataRowView dr in Form1.myMedia.searchByFilePath(clipFile))
                //    {
                //        FontStyle newFontStyle;
                //        Font currentFont = txtMediaInfo.SelectionFont;

                //        newFontStyle = FontStyle.Bold;
                //        txtMediaInfo.SelectionFont = new Font(currentFont.FontFamily, currentFont.Size, newFontStyle);
                //        txtMediaInfo.Text = dr["title"].ToString();
                //        txtDurationTime.Text = getDuration();
                //    }
                //    if (txtMediaInfo.Text.Length < 1)
                //        txtMediaInfo.Text = "didn't find media info";
                //    txtMediaInfo.Visible = true;
                //    txtMediaInfo.BringToFront();
                //}
                //else
                //    txtMediaInfo.Visible = false;
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }
        }
        #endregion

        #region Clock functions
        public string GetTime()
        {
            return DateTime.Now.ToShortTimeString();
        }

        private void clock_Tick(object sender, System.EventArgs e)
        {
            //txtClock.Text = GetTime();
        }
        #endregion

        #region private void currentPositionTimer_Tick(object sender, System.EventArgs e)
        private void currentPositionTimer_Tick(object sender, System.EventArgs e)
        {
            //txtCurrentPositionTime.Text = getCurrentPosition();
        }
        #endregion

        #region private string getCurrentPosition()
        private string getCurrentPosition()
        {
            try
            {
                double currentPosition;
                //mediaPos.get_CurrentPosition(out currentPosition);
                return "";// convertMilliseconds((int)currentPosition);
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
                return "";
            }
        }
        #endregion

        #region private string getDuration()
        private string getDuration()
        {
            try
            {
                double duration;
                //mediaPos.get_Duration(out duration);
                return "";// convertMilliseconds((int)duration);
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
                return "";
            }
        }
        #endregion

        #region private string convertMilliseconds(int milliSeconds)
        private string convertMilliseconds(int milliSeconds)
        {
            try
            {
                int s = milliSeconds;
                int h = s / 3600;
                int m = (s - (h * 3600)) / 60;
                s = s - (h * 3600 + m * 60);
                return String.Format("{0:D2}:{1:D2}:{2:D2}", h, m, s);
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
                return "";
            }
        }
        #endregion

        private void liquidMediaPlayer_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                //if (playState == PlayState.Init)
                //    return;

                //switch (e.KeyCode)
                //{
                //    //				case Keys.P:
                //    //				{	menuControlPause_Click( null, null ); break; }
                //    //				case Keys.S:
                //    //				{	menuControlStop_Click( null, null ); break; }
                //    case Keys.M:
                //        { menuControlMute_Click(null, null); break; }

                    //				case Keys.D1:
                    //				case Keys.Space:
                    //				{	menuControlStep_Click( null, null ); break; }
                    //				case Keys.D2:
                    //				{	StepFrames( 2 ); break; }
                    //				case Keys.D3:
                    //				{	StepFrames( 3 ); break; }
                    //				case Keys.D4:
                    //				{	StepFrames( 4 ); break; }
                    //				case Keys.D5:
                    //				{	StepFrames( 5 ); break; }
                    //				case Keys.D6:
                    //				{	StepFrames( 6 ); break; }
                    //				case Keys.D7:
                    //				{	StepFrames( 7 ); break; }
                    //				case Keys.D8:
                    //				{	StepFrames( 8 ); break; }
                    //				case Keys.D9:
                    //				{	StepFrames( 9 ); break; }

                    //				case Keys.H:
                    //				{	menuControlHalf_Click( null, null ); break; }
                    //				case Keys.T:
                    //				{	menuControlThreeQ_Click( null, null ); break; }
                    //				case Keys.N:
                    //				{	menuControlNormal_Click( null, null ); break; }
                    //				case Keys.D:
                    //				{	menuControlDouble_Click( null, null ); break; }
                    //
                    //				case Keys.F:
                    //				{	ToggleFullScreen(); break; }
                    //				case Keys.Enter:
                    //				{	ToggleFullScreen(); break; }
                    //				case Keys.Escape:
                    //				{
                    //					if( fullScreen )
                    //						ToggleFullScreen();
                    //					break;
                    //				}

                    //				case Keys.Left:
                    //				{	menuRateDecr_Click( null, null ); break; }
                    //				case Keys.Right:
                    //				{	menuRateIncr_Click( null, null ); break; }
                    //				case Keys.Up:
                    //				{	menuRateDouble_Click( null, null ); break; }
                    //				case Keys.Down:
                    //				{	menuRateHalf_Click( null, null ); break; }
                    //				case Keys.Back:
                    //				{	menuRateNormal_Click( null, null ); break; }
            //    }
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }
        }

        /// <summary>
        /// handle key up events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void liquidMediaPlayer_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                //changeChannelTimer.Enabled = true;
                //changeChannelTimer.Start();
                keyStrokeTracker.Add(e.KeyValue);
                string tempKeys = "";
                if (keyStrokeTracker.Count > 0)
                {
                    foreach (int intChannel in keyStrokeTracker)
                    {
                        tempKeys += intChannel.ToString() + ",";
                    }
                    tempKeys = tempKeys.Remove(tempKeys.LastIndexOf(","), 1);//removes trailing comma	
                }
                if (macroList.Contains(tempKeys))
                {
                    keyStrokeTracker.Clear();
                    string macroName = macroList.GetByIndex(macroList.IndexOfKey(tempKeys)).ToString();
                    //Tools.WriteToFile(Tools.errorFile,"executing macro " + macroName);
                    executeMacros(macroName);
                }
                else
                    Tools.WriteToFile(Tools.errorFile,"didn't find mediaPlayer macro for: " + tempKeys);

                //Tools.WriteToFile(Tools.errorFile,"KeyCode from mediaPlayer keyup: " + e.KeyCode);
                //Tools.WriteToFile(Tools.errorFile,"KeyValue from mediaPlayer keyup: " + e.KeyValue);
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }
        }

        /// <summary>
        /// these run immediatly when found
        /// </summary>
        private void populateMacros()
        {
            try
            {
                //comma delimited list of (string) KeyValue's , names

                //media controls
                macroList.Add("17,16,66", "Rewind");  //rewind
                macroList.Add("17,83", "Stop");  //Stop
                macroList.Add("17,16,70", "FF");  //FF
                macroList.Add("17,70", "Next");  //Next
                macroList.Add("17,66", "Previous");  //Previous
                macroList.Add("17,80", "Play/Pause");  //Play/Pause
                //			macroList.Add(17,16,66);  //Record
                //			macroList.Add("","Mute");  //mute

                //main menu items
                macroList.Add("17,16,18,36", "Option");  //home key
                macroList.Add("17,16,18,50", "tvKey");  //tv key
                macroList.Add("17,16,18,72", "fmKey");  //fm key
                macroList.Add("17,16,18,51", "musicKey");  //music key
                macroList.Add("17,16,18,52", "pictureKey");  //picture key
                macroList.Add("17,16,18,53", "videoKey");  //video key
                macroList.Add("17,16,18,49", "dvdKey");  //dvd key
                macroList.Add("17,16,18,70", "shuffleKey");  //shuffle key
                macroList.Add("17,16,18,66", "repeatKey");  //repeat key
                macroList.Add("13", "Enter");  //Enter
                macroList.Add("27", "Escape");  //Escape - close
                //			macroList.Add("166","previousChannel");  //Browser Back
                macroList.Add("37", "leftArrow");  //left Arrow - 
                macroList.Add("38", "upArrow");  //up arrow - 
                macroList.Add("39", "rightArrow");  //right Arrow - 
                macroList.Add("40", "downArrow");  //down Arrow - 
                macroList.Add("9", "tab");  //Tab

                //generic Key settings for main menu
                macroList.Add("113", "tvKey");  //F2 - tv key
                macroList.Add("114", "fmKey");  //F3 - fm key
                macroList.Add("116", "musicKey");  //F5 - music key
                macroList.Add("117", "pictureKey");  //F6 - picture key
                macroList.Add("118", "videoKey");  //F7 - video key
                macroList.Add("119", "dvdKey");  //F8 - dvd key
                
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }
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
                keyStrokeTracker.Clear();
                switch (macroName)
                {
                    case "pause":
                    case "play":
                    case "pausePlay":
                        this.pausePlay();
                        break;
                    case "stop":
                        progressTimer.Stop();
                        //this.stop();
                        executeMacros("close");
                        break;
                    case "close":
                        this.Close();
                        break;
                    case "Rewind":
                        //ModifyRate(-0.25);
                        break;
                    case "FF":
                        ModifyRate(0.25);
                        break;
                    case "Next":
                        executeMacros("close");
                        //					myMediaPlayer.Ctlcontrols.next();
                        break;
                    case "Previous":
                        //					myMediaPlayer.Ctlcontrols.previous();
                        break;
                    case "ChannelUp":
                        break;
                    case "ChannelDown":
                        break;
                    case "channel":
                        break;
                    case "Enter":
                        displayMediaInfo();
                        break;
                    case "Option":
                        this.Close();
                        break;
                    case "Space":
                        break;
                    case "Back":
                        break;
                    case "tab"://add a bookmark

                        //					ToggleFullScreen();
                        break;
                    case "leftArrow":
                        //Tools.WriteToFile(Tools.errorFile,"stepping frames left");
                        //timeSkip(-25);
                        break;
                    case "upArrow":
                        //Tools.WriteToFile(Tools.errorFile,"stepping frames up");
                        //timeSkip(200);
                        break;
                    case "rightArrow":
                        //Tools.WriteToFile(Tools.errorFile,"stepping frames right");
                        //timeSkip(25);
                        break;
                    case "downArrow":
                        //Tools.WriteToFile(Tools.errorFile,"stepping frames down");
                        //timeSkip(-200);
                        break;
                    case "Escape": //Escape - close
                        executeMacros("close");
                        break;
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
                Tools.WriteToFile(ex);
            }
        }

        public void executePhrase(Phrase phrase)
        {
            //executeMacros(phrase.phrase);

            switch (phrase.phrase.ToLower())
            {
                case ("pause"):
                case ("play"):
                case ("stop"):
                case ("close"):
                    executeMacros(phrase.phrase);
                    break;
            }
        }

        /// <summary>
        /// Execute speech commands for MediaPlayer
        /// </summary>
        private void speechListener_executeCommand(Phrase thePhrase)
        {
            if (thePhrase.RuleName != "")// && accuracy >= accuracyLimit)
            {
                //EventLog.WriteEntry("SCTV Speech player", thePhrase.phrase + " - " + thePhrase.Accuracy.ToString());

                //if(accuracy >= accuracyLimit)
                //if(thePhrase.Accuracy > 30)
                executePhrase(thePhrase);
                //else
                //    EventLog.WriteEntry("SCTV Speech player", thePhrase.phrase + " - " + thePhrase.Accuracy.ToString());
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            try
            {
                vlc.stop();

                vlc = null;

                GC.Collect();

                //stop listening for speech commands from this form
                //Form1.speechListener.executeCommand -= new speechRecognition.heardCommand(speechListener_executeCommand);

                base.OnClosing(e);

                this.Dispose();

                //alert any other forms that this one is closing
                //closingForm();
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }
        }

        private void pnlVideo_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                executeMacros("close");
            }
        }

        private void liquidMediaPlayer_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                this.Close();
        }

        private void btnPlayPause_Click(object sender, EventArgs e)
        {
            executeMacros("play");

            if (btnPlayPause.Text == "Play")
            {
                btnPlayPause.Image = global::MediaPlayer.Properties.Resources.Pause_WhiteOnDKBlue_button;
                btnPlayPause.Text = "Pause";
            }
            else
            {
                btnPlayPause.Image = global::MediaPlayer.Properties.Resources.Play_WhiteOnDKBlue_button;
                btnPlayPause.Text = "Play";
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            this.playSequels = false;
            this.continousPlay = false;

            executeMacros("close");
        }

        private void btnRewind_Click(object sender, EventArgs e)
        {
            executeMacros("Rewind");
        }

        private void btnFastForward_Click(object sender, EventArgs e)
        {
            executeMacros("FF");
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            executeMacros("Next");
        }

        private void btnVolumeUp_Click(object sender, EventArgs e)
        {
            executeMacros("volume up");
        }

        private void btnVolumeDown_Click(object sender, EventArgs e)
        {
            executeMacros("volume down");
        }

        private void btnMute_Click(object sender, EventArgs e)
        {
            executeMacros("mute");
            //ushort skey = 0x53; //S key
            //vlc.setVariable("key-jump+short", (int)skey);
            //vlc.setVariable("key-pressed", (int)skey);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.playSequels = false;
            this.continousPlay = false;

            executeMacros("close");
        }

        void ModifyRate(double rateAdjust)
        {
            try
            {
                if ((mediaPos == null) || (rateAdjust == 0.0))
                    return;

                double rate;
                int hr = mediaPos.get_Rate(out rate);
                if (hr < 0)
                    return;
                rate += rateAdjust;
                hr = mediaPos.put_Rate(rate);
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }
        }

        private void progressTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                double newPBPosition;
                float position = vlc.GetPosition();
                int currentPosition = vlc.GetPositionSecs();
                
                if(currentPosition == 0)
                    currentPosition = (int)(vlc.GetPosition() * 10000);

                int length = vlc.GetLengthSecs();

                

                if (currentPosition > 1)
                {
                    this.lblLoading.Visible = false;

                    if(length > 0)
                        newPBPosition = (double)currentPosition / (double)length * 100;
                    else
                        newPBPosition = (double)currentPosition / 100;

                    //update progress bar
                    if (pbProgress.Value >= pbProgress.Maximum)
                        pbProgress.Value = 0;

                    pbProgress.Value = (int)newPBPosition;

                    //if (mediaSplash != null && currentPosition > 2) //the clip has started
                    if (SplashScreenNew.SplashForm != null) //the clip has started
                    {
                        SplashScreenNew.CloseForm();
                        //mediaSplash.Close();

                        if (this.StartedPlaying != null)
                            this.StartedPlaying();

                        this.Show();
                        this.TopMost = true;
                    }
                }

                //if (currentPosition < lastPosition || lastPosition == currentPosition || currentPosition < 0)//close player - we are at the end of the video
                if(currentPosition == -20 && lastPosition == -20)
                {
                    positionStopCounter++;

                    if (positionStopCounter > 3)//when we have checked 3 times we will end
                    {
                        try
                        {
                            if (playSequels)
                                this.PlaySequels();

                            if (continousPlay)
                                this.ContinousPlayOn();

                            if (this.MediaDonePlaying != null)
                                this.MediaDonePlaying();
                        }
                        catch (Exception ex)
                        {
                            Tools.WriteToFile(ex);
                        }                        

                        positionStopCounter = 0;

                        executeMacros("close");
                    }
                }

                //update progress label
                TimeSpan timeSpan = TimeSpan.FromSeconds((double)currentPosition);
                lblProgress.Text = timeSpan.Hours + ":" + timeSpan.Minutes + ":" + timeSpan.Seconds;

                //if (lastPosition != currentPosition)
                //    positionStopCounter = 0;

                lastPosition = currentPosition;
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }
        }

        private void pbProgress_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                double percentage = (double)e.X / (double)pbProgress.Width;
                int newPosition = (int)(vlc.GetLengthSecs() * percentage);

                //if (newPosition == 0)
                //    newPosition = (int)(vlc.GetPosition() * 10000 * percentage);

                vlc.SetPositionSecs(newPosition);
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }
        }

        private void inactivityTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                //make sure mouse isn't on mouse controls
                //if (Cursor.Position.Y < pnlMouseControls.Location.Y)
                //    inactivityTicker++;

                //hide mouse controls
                if (inactivityTicker > inactivityTime)
                {
                    int distanceToMove = pnlMouseControls.Height + 2;
                    int distanceMoved = 0;

                    inactivityTimer.Stop();

                    //hide pnlMouseControls
                    while (pnlMouseControls.Height > 10)
                    {
                        pnlMouseControls.Height = pnlMouseControls.Height - 1;
                        distanceMoved += 1;
                    }

                    pnlCover.Visible = true;
                    pnlCover.BringToFront();
                    pnlCover.Focus();

                    //hide mouse on right side of screen
                    Cursor.Position = new Point(pnlMouseControls.Width, Cursor.Position.Y);
                }
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }
        }

        private void liquidMediaPlayer_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                //update lastplayposition
                currentMedia.LastPlayPosition = vlc.GetPositionSecs().ToString();
                MediaHandler.UpdateMediaInfo(currentMedia);

                //make sure cursor is visible
                Cursor.Show();

                //call event
                this.closingForm();
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }
        }

        /// <summary>
        /// show mouse controls
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pnlMouseControls_MouseMove(object sender, MouseEventArgs e)
        {
            pnlCover.Visible = false;
            pnlMouseControls.Visible = true;

            //show pnlMouseControls
            pnlMouseControls.Height = 50;

            pnlMouseControls.BackColor = Color.Transparent;

            inactivityTimer.Start();

            inactivityTicker = 0;
        }

        /// <summary>
        /// start timer if mouse is off of mouse controls
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pnlMouseControls_MouseLeave(object sender, EventArgs e)
        {
            ////start timer if mouse is off of mouse controls
            //if (Cursor.Position.Y < pnlMouseControls.Location.Y)
            //    inactivityTimer.Start();
        }

        public string[] ToScreen()
        {
            return ToScreen("", true);
        }
        
        public string[] ToScreen(string fileToRecordTo)
        {
            return ToScreen(fileToRecordTo, true);
        }

        public string[] ToScreen(string fileToRecordTo, bool displayToScreen)
        {
            string[] options = new string[3];

            options[0] = "config=\"~/Application Data/vlc/vlcrc\"";
            options[1] = ":file-caching=300 :sout-udp-caching=200";

            if (fileToRecordTo.Length > 2)
            {
                if (displayToScreen)//display output to screen and record
                {
                        //options[2] = ":sout=#transcode{vcodec=mp4v,vb=512,scale=1}:duplicate{dst=display,dst=std{access=file,mux=ts,dst=\"" + fileToRecordTo + "\"}}";
                        ////options[2] = ":sout=#transcode{vcodec=mp4v,vb=3000,scale=1}:duplicate{dst=display,dst=std{access=file,mux=ts,dst=\"" + fileToRecordTo + "\"}}";

                        ////worked - 3gig for indiana jones - no pixelation
                        options[2] = ":sout=#duplicate{dst=display,dst=std{access=file,mux=ts,dst=\"" + fileToRecordTo + "\"}}";
                }
                else //record only
                    options[2] = ":sout=#duplicate{dst=std{access=file,mux=ts,dst=\"" + fileToRecordTo + "\"}}";
            }
            else//play only
                options[2] = ":sout=#duplicate{dst=display}";

            return options;
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //show context menu
                CameraContextMenuStrip.Show(this, this.PointToClient(Cursor.Position));
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void camera1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SCTVCamera.SCTVCameraMain camera = new SCTVCamera.SCTVCameraMain();
            camera.SpeechListener = speechListener;
            //camera.ShowCameraByName("camera 1");
        }

        private void camera2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SCTVCamera.SCTVCameraMain camera = new SCTVCamera.SCTVCameraMain();
            camera.SpeechListener = speechListener;
            //camera.ShowCameraByName("camera 2");
        }

        private void pnlVideoScreen_Click(object sender, EventArgs e)
        {
            MessageBox.Show("click");
        }

        private void chbSequels_CheckedChanged(object sender, EventArgs e)
        {
            playSequels = chbSequels.Checked;
        }

        private void chbContinousPlay_CheckedChanged(object sender, EventArgs e)
        {
            continousPlay = chbContinousPlay.Checked;
        }

        private void starRating_RatingValueChanged(object sender, RatingChangedEventArgs e)
        {
            //update xml with new rating
            if (myMedia == null)
                myMedia = new MediaHandler();

            currentMedia.Stars = StarRating.Rating.ToString();
            MediaHandler.UpdateMediaInfo(currentMedia);
        }

        //display info about the currently playing media
        private void btnMediaInfo_Click(object sender, EventArgs e)
        {
            SCTVObjects.MediaDetails details = new MediaDetails(currentMedia);
            details.ReadOnly = true;
            details.ShowDialog(this);
        }

        private void liquidMediaPlayer_FormClosed(object sender, FormClosedEventArgs e)
        {
            //this.Dispose(true);
            //this.Dispose();

            //this.pnlVideo.Handle = null;
            //GC.Collect();
        }

        private void lblWhatsNext_MouseClick(object sender, MouseEventArgs e)
        {
            if(myMedia == null)
                 myMedia = new MediaHandler();

            whatsNext = myMedia.GetSequel(currentMedia, true);
            lblWhatsNext.Text = "Next On SCTV: " + whatsNext.Title;
        }
    }
}>>>>>>> .r6
