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

        [DllImport("gdi32.dll")]
        public static extern IntPtr ExtCreateRegion(int lpXform, int nCount, byte[] lpRgnData);

        [DllImport("user32.dll")]
        internal static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool bRedraw);

        //static private SHDocVw.ShellWindows shellWindows = new SHDocVw.ShellWindowsClass();
        public SHDocVw.ShellWindows SWs;
        public SHDocVw.InternetExplorer IE;

        public delegate void closing();
        public event closing closingForm;

        public string _grammarvocabFile = "xmlMediaPlayer";

        /// <summary> seek interface to set position in stream. </summary>
        private IMediaPosition mediaPos;

        private SCTVControls.SlidingBar slidingBar = null;

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

            //display loading screen
            //if (Form1.Mode == "Release")
            //{
            //    lblLoading.Visible = true;
            //    lblLoading.BringToFront();
            //}

            //deactivate screensaver
            //SystemParametersInfo(17, 0, false, 0);

            //Form1.GUIState = guiState.video;

            //listen for speech commands
            //Form1.speechListener.executeCommand += new speechRecognition.heardCommand(speechListener_executeCommand);

            //watch for removeable media
            //Form1.deviceMonitor.OnVolumeInserted += new DeviceVolumeAction(deviceMonitor_OnVolumeInserted);
            //Form1.deviceMonitor.OnVolumeRemoved += new DeviceVolumeAction(deviceMonitor_OnVolumeRemoved);
            
            //this.TopMost = true;

            populateMacros();

            //set volume bar
            //Form1 form1 = (Form1)Application.OpenForms["Form1"];
            //pbVolume.Value = form1.VolumeLevel;

            pnlMouseControls.Visible = false;

            //create and dock slidingBar
            //Button btnTest = new Button();
            //btnTest.Anchor = AnchorStyles.Right;
            //btnTest.Anchor = AnchorStyles.Bottom;
            //btnTest.Text = "Hi";
            //btnTest.Location = new Point(0, 0);

            slidingBar = new SCTVControls.SlidingBar();
            this.Controls.Add(slidingBar);

            //slidingBar.Controls.Add(btnTest);

            pbProgress.Dock = DockStyle.Top;
            slidingBar.Controls.Add(pbProgress);
            pbVolume.Anchor = AnchorStyles.Right;
            pbVolume.Anchor = AnchorStyles.Bottom;
            slidingBar.Controls.Add(pbVolume);
            btnClose.Anchor = AnchorStyles.Right;
            btnClose.Anchor = AnchorStyles.Bottom;
            slidingBar.Controls.Add(btnClose);
            slidingBar.Visible = true;
            slidingBar.Dock = DockStyle.Bottom;
        }

        /// <summary>
        /// called when removeable media is removed
        /// </summary>
        /// <param name="aMask"></param>
        void deviceMonitor_OnVolumeRemoved(int aMask)
        {
            //make sure we aren't trying to play the media that has been removed

        }

        /// <summary>
        /// called when removeable media is inserted
        /// </summary>
        /// <param name="aMask"></param>
        //void deviceMonitor_OnVolumeInserted(int aMask)
        //{
        //    switch (MessageBox.Show("Do you want to play DVD?", "Play DVD?", MessageBoxButtons.YesNo))
        //    {
        //        case DialogResult.Yes:
        //            PlayRemoveableMedia(Form1.deviceMonitor.MaskToLogicalPaths(aMask));
        //            break;
        //        case DialogResult.No:
        //            break;
        //    }
        //}

        /// <summary>
        /// Record given media
        /// </summary>
        /// <param name="driveLetter"></param>
        /// <param name="fileToRecordTo"></param>
        public void RecordMedia(string driveLetter, string fileToRecordTo)
        {
            try
            {
                int vHandle = 0;
                ArrayList mediaToPlay;

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
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("sctv PlayMedia", "PlayMedia error: " + ex.Message);
            }
        }

        /// <summary>
        /// Play given media
        /// </summary>
        /// <param name="mediaToPlay">arrayList of media to play</param>
        /// <param name="fileToRecordTo">if blank nothing will record</param>
        public void PlayMedia(ArrayList mediaToPlay, string fileToRecordTo)
        {
            try
            {
                int vHandle = 0;

                vHandle = this.pnlVideo.Handle.ToInt32();

                if (vHandle > 0)
                {
                    vlc = new VLC();

                    vlc.SetOutputWindow(vHandle);
                    string[] options;
                    //string[] options = { "" };

                    if(fileToRecordTo.Length > 0)
                        options = ToScreen(fileToRecordTo);
                    else
                        options = ToScreen();

                    _playerTask = new PlayerTask(10, 0, vlc, options, mediaToPlay);
                    // _playerTask.Completed += new AsynTaskCompletedEventHandler(playerTask_Completed);
                    _playerTask.Start();

                    progressTimer.Start();
                    inactivityTimer.Start();

                    //ushort skey = 0x53; //S key
                    //vlc.setVariable("key-play-pause", (int)skey);
                    //vlc.setVariable("key-pressed", (int)skey);
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("sctv PlayMedia", "PlayMedia error: " + ex.Message);
            }
        }

        public void PlayClip(string fileToPlay)
        {
            try
            {
                ArrayList media = new ArrayList();
                media.Add(fileToPlay);

                PlayMedia(media, "");
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("sctv playClip", "PlayClip error: " + ex.Message);
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
                EventLog.WriteEntry("sctv PlayCategory", "PlayCategory error: " + ex.Message);
            }
        }

        public void PlayRemoveableMedia(string driveLetter)
        {
            PlayRemoveableMedia(driveLetter, "");
        }

        public void PlayRemoveableMedia(string driveLetter, string fileToRecordTo)
        {
            ArrayList mediaToPlay = new ArrayList();
            mediaToPlay.Add(driveLetter);
            PlayMedia(mediaToPlay, fileToRecordTo);
        }

        public void RecordRemoveableMedia(string driveLetter, string fileToRecordTo)
        {
            RecordMedia(driveLetter, fileToRecordTo);
        }

        public void playerTask_Completed(object sender, AsynTaskCompletedEventArgs e)
        {
            object[] state = (object[])e.Result;
            //label1.Text = (string)state[0];

            executeMacros("close");
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
                //Debug.WriteLine(ex);
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
                //Debug.WriteLine(ex);
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
                    //EventLog.WriteEntry("sctv","executing macro " + macroName);
                    executeMacros(macroName);
                }
                else
                    EventLog.WriteEntry("sctv","didn't find mediaPlayer macro for: " + tempKeys);

                //EventLog.WriteEntry("sctv","KeyCode from mediaPlayer keyup: " + e.KeyCode);
                //EventLog.WriteEntry("sctv","KeyValue from mediaPlayer keyup: " + e.KeyValue);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
                MessageBox.Show(ex.Message);
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
            //    EventLog.WriteEntry("sctv","mode 1 in ToggleFullScreen " + mode.ToString());
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
            //    EventLog.WriteEntry("sctv","mode 2 in ToggleFullScreen " + mode.ToString());
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
                MessageBox.Show(ex.Message);
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
                MessageBox.Show(ex.Message);
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
                MessageBox.Show(ex.Message);
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
                MessageBox.Show(ex.Message);
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
                MessageBox.Show(ex.Message);
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
                    //EventLog.WriteEntry("sctv","executing macro " + macroName);
                    executeMacros(macroName);
                }
                else
                    EventLog.WriteEntry("sctv","didn't find mediaPlayer macro for: " + tempKeys);

                //EventLog.WriteEntry("sctv","KeyCode from mediaPlayer keyup: " + e.KeyCode);
                //EventLog.WriteEntry("sctv","KeyValue from mediaPlayer keyup: " + e.KeyValue);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
                MessageBox.Show(ex.Message);
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
                //EventLog.WriteEntry("sctv","calling macro in liquidMedia " + macroName);
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
                    case "mute":
                    case "volume down":
                    case "volume up":
                        //Form1 form1 = (Form1)Application.OpenForms["Form1"];
                        //form1.executeMacros(macroName);
                        break;
                    case "Next":
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
                    case "tvKey":
                        this.Close();
                        break;
                    case "fmKey":
                        MessageBox.Show("Coming Soon!!");
                        break;
                    case "musicKey":
                        MessageBox.Show("Coming Soon!!");
                        break;
                    case "pictureKey":
                        MessageBox.Show("Coming Soon!!");
                        break;
                    case "videoKey":
                        this.Close();
                        break;
                    case "dvdKey":
                        MessageBox.Show("Coming Soon!!");
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
                        //EventLog.WriteEntry("sctv","stepping frames left");
                        //timeSkip(-25);
                        break;
                    case "upArrow":
                        //EventLog.WriteEntry("sctv","stepping frames up");
                        //timeSkip(200);
                        break;
                    case "rightArrow":
                        //EventLog.WriteEntry("sctv","stepping frames right");
                        //timeSkip(25);
                        break;
                    case "downArrow":
                        //EventLog.WriteEntry("sctv","stepping frames down");
                        //timeSkip(-200);
                        break;
                    case "Escape": //Escape - close
                        executeMacros("close");
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //public void executePhrase(Phrase phrase)
        //{
        //    //dont execute volume here it is being caught by form1 and handled
        //    if (!phrase.phrase.ToLower().Contains("volume") && !phrase.phrase.ToLower().Contains("mute"))
        //        executeMacros(phrase.phrase);
        //    //switch (phrase.phrase)
        //    //{
        //    //    case("pause"):
        //    //    case("Play"):
        //    //        break;
        //    //    case("stop"):
        //    //        break;
        //    //    case("close"):
        //    //        break;
        //    //}
        //}

        /// <summary>
        /// Execute speech commands for MediaPlayer
        /// </summary>
        //private void speechListener_executeCommand(Phrase thePhrase)
        //{
        //    if (thePhrase.RuleName != "")// && accuracy >= accuracyLimit)
        //    {
        //        EventLog.WriteEntry("SCTV Speech player", thePhrase.phrase + " - " + thePhrase.Accuracy.ToString());

        //        //if(accuracy >= accuracyLimit)
        //        //if(thePhrase.Accuracy > 30)
        //            executePhrase(thePhrase);
        //        //else
        //        //    EventLog.WriteEntry("SCTV Speech player", thePhrase.phrase + " - " + thePhrase.Accuracy.ToString());
        //    }
        //}

        protected override void OnClosing(CancelEventArgs e)
        {
            vlc.stop();
            //Form1.GUIState = guiState.defaultState;

            //stop listening for speech commands from this form
            //Form1.speechListener.executeCommand -= new speechRecognition.heardCommand(speechListener_executeCommand);

            base.OnClosing(e);

            //alert any other forms that this one is closing
            //closingForm();
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
                btnPlayPause.Text = "Pause";
            else
                btnPlayPause.Text = "Play";
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
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
            //executeMacros("mute");
            ushort skey = 0x53; //S key
            vlc.setVariable("key-jump+short", (int)skey);
            vlc.setVariable("key-pressed", (int)skey);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
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
                MessageBox.Show(ex.Message);
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

                if (currentPosition > 0)
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
                }

                //update progress label
                lblProgress.Text = currentPosition.ToString() + " secs";
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("SCTV - timerTick", ex.Message);
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
                EventLog.WriteEntry("pbProgress_MouseDown", ex.Message);
            }
        }

        private void inactivityTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                inactivityTicker++;

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

                    //pnlMouseControls.Visible = false;
                    pnlMouseControls.BackColor = Color.Black;

                    //hide mouse on right side of screen
                    Cursor.Position = new Point(pnlMouseControls.Width, Cursor.Position.Y);
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("inactivityTimer_Tick", ex.Message);
            }
        }

        /// <summary>
        /// volume control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pbVolume_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                double percentage = (double)e.X / (double)pbVolume.Width;

                //Form1 form1 = (Form1)Application.OpenForms["Form1"];
                //form1.Volume(percentage);

                //update volume bar
                pbVolume.Value = (int)Math.Round(percentage * 100);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("pbVolume_MouseDown", ex.Message);
            }
        }

        private void liquidMediaPlayer_FormClosing(object sender, FormClosingEventArgs e)
        {
            //make sure cursor is visible
            Cursor.Show();
        }

        /// <summary>
        /// show mouse controls
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pnlMouseControls_MouseMove(object sender, MouseEventArgs e)
        {
            pnlMouseControls.Visible = true;

            //show pnlMouseControls
            pnlMouseControls.Height = 50;

            pnlMouseControls.Visible = true;

            pnlMouseControls.BackColor = Color.Transparent;

            inactivityTimer.Stop();

            inactivityTicker = 0;
        }

        /// <summary>
        /// start timer if mouse is off of mouse controls
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pnlMouseControls_MouseLeave(object sender, EventArgs e)
        {
            //start timer if mouse is off of mouse controls
            if (Cursor.Position.Y < pnlMouseControls.Location.Y)
                inactivityTimer.Start();
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
                    options[2] = ":sout=#transcode{vcodec=mp4v,vb=512,scale=1}:duplicate{dst=display,dst=std{access=file,mux=ts,dst=\"" + fileToRecordTo + "\"}}";
                    //options[2] = ":sout=#transcode{vcodec=mp4v,vb=3000,scale=1}:duplicate{dst=display,dst=std{access=file,mux=ts,dst=\"" + fileToRecordTo + "\"}}";

                    //worked - 3gig for indiana jones - no pixelation
                    options[2] = ":sout=#duplicate{dst=display,dst=std{access=file,mux=ts,dst=\"" + fileToRecordTo + "\"}}";
                }
                else //record only
                    options[2] = ":sout=#duplicate{dst=std{access=file,mux=ts,dst=\"" + fileToRecordTo + "\"}}";
            }
            else//play only
                options[2] = ":sout=#duplicate{dst=display}";

            return options;
        }
    }
}