using System;
using System.Drawing;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Data;
using System.Xml;
using DirectX.Capture;
using System.Diagnostics;
using System.Threading;
using Microsoft.ApplicationBlocks.ConfigurationManagement;
using Microsoft.ApplicationBlocks.ExceptionManagement;
using System.Runtime.InteropServices;
using System.IO;
using DShowNET;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using System.Configuration;

namespace SCTVTelevision
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	/// 

	#region internal enums
	internal enum PlayState
	{
		Init, Stopped, Paused, Running
	}

	internal enum ClipType
	{
		none, audioVideo, video, audio
	}

	public enum guiState
	{
		TV,video,admin,pictures,radio,music,dvd,mediaLibrary,defaultState
	}
	
	#endregion

	#region [Flags]
	[Flags]
	public enum SoundFlags : int 
	{
		SND_SYNC = 0x0000,  // play synchronously (default) 
		SND_ASYNC = 0x0001,  // play asynchronously 
		SND_NODEFAULT = 0x0002,  // silence (!default) if sound not found 
		SND_MEMORY = 0x0004,  // pszSound points to a memory file
		SND_LOOP = 0x0008,  // loop the sound until next sndPlaySound 
		SND_NOSTOP = 0x0010,  // don't stop any currently playing sound 
		SND_NOWAIT = 0x00002000, // don't wait if the driver is busy 
		SND_ALIAS = 0x00010000, // name is a registry alias 
		SND_ALIAS_ID = 0x00110000, // alias is a predefined ID
		SND_FILENAME = 0x00020000, // name is file name 
		SND_RESOURCE = 0x00040004  // name is resource name or atom 
	}
	#endregion

	public class Form1 : System.Windows.Forms.Form
	{
		#region variables
		private System.Windows.Forms.DataGrid dgListings;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn1;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn2;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn3;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn4;
		private System.Windows.Forms.DataGridTableStyle listings;
		private System.Windows.Forms.TabPage tabPage3;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn5;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn6;
		private System.ComponentModel.IContainer components;

		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem7;
		private System.Windows.Forms.MainMenu mainMenu;
		private System.Windows.Forms.MenuItem mnuExit;
		private System.Windows.Forms.MenuItem mnuDevices;
		private System.Windows.Forms.MenuItem mnuVideoDevices;
		private System.Windows.Forms.MenuItem mnuAudioDevices;
		private System.Windows.Forms.MenuItem mnuVideoCompressors;
		private System.Windows.Forms.MenuItem mnuAudioCompressors;
		private System.Windows.Forms.MenuItem mnuVideoSources;
		private System.Windows.Forms.MenuItem mnuAudioSources;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.MenuItem mnuAudioChannels;
		private System.Windows.Forms.MenuItem mnuAudioSamplingRate;
		private System.Windows.Forms.MenuItem mnuAudioSampleSizes;
		private System.Windows.Forms.MenuItem menuItem5;
		private System.Windows.Forms.MenuItem mnuFrameSizes;
		private System.Windows.Forms.MenuItem mnuFrameRates;
		private System.Windows.Forms.MenuItem menuItem6;
		private System.Windows.Forms.MenuItem mnuPreview;
		private System.Windows.Forms.MenuItem menuItem8;
		private System.Windows.Forms.MenuItem mnuPropertyPages;
		private System.Windows.Forms.MenuItem mnuVideoCaps;
		private System.Windows.Forms.MenuItem mnuAudioCaps;
		private System.Windows.Forms.MenuItem mnuChannel;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem mnuInputType;
		private System.Windows.Forms.Label lblChannel;
		private DataSet dsListings = new DataSet();
		public static ProgrammeOrganizer ListingsOrganizer=new ProgrammeOrganizer();
		public static ShowNotificationOrganizer NotificationOrganizer=new ShowNotificationOrganizer();
		public static ShowNotificationOrganizer RecordingOrganizer=new ShowNotificationOrganizer();
		public static playScheduleOrganizer pScheduleOrganizer=new playScheduleOrganizer();
		public static favoritesOrganizer favoriteOrganizer=new favoritesOrganizer();
		private System.Windows.Forms.TabControl tabControl;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuItem9;
		private System.Windows.Forms.MenuItem menuItem10;
		public static ConfigurationData ProgramConfiguration;
		TaskbarNotifier taskbarNotifier;
        public static string Mode = "Release";

		private const string clipFileFilters =
			"Video Files (avi qt mov mpg mpeg m1v)|*.avi;*.qt;*.mov;*.mpg;*.mpeg;*.m1v|" +
			"Audio files (wav mpa mp2 mp3 au aif aiff snd)|*.wav;*.mpa;*.mp2;*.mp3;*.au;*.aif;*.aiff;*.snd|" +
			"MIDI Files (mid midi rmi)|*.mid;*.midi;*.rmi|" +
			"Image Files (jpg bmp gif tga)|*.jpg;*.bmp;*.gif;*.tga|" +
			"All Files (*.*)|*.*";
		private System.Windows.Forms.ContextMenu rightClickMedia;
		enum MediaSource { TV, file, radio };
		private System.Windows.Forms.MenuItem menuItem12;
		private System.Windows.Forms.MenuItem menuItem13;
		private System.Windows.Forms.MenuItem menuItem14;
		private System.Windows.Forms.MenuItem menuItem15;
		private System.Windows.Forms.MenuItem menuItem16;
		private System.Windows.Forms.MenuItem menuItem17;
		private System.Windows.Forms.MenuItem menuItem18;
		private System.Windows.Forms.MenuItem menuItem19;
		private System.Windows.Forms.MenuItem menuItem20;
		private System.Windows.Forms.MenuItem menuItem21;
		private System.Windows.Forms.MenuItem menuItem22;
		private System.Windows.Forms.MenuItem menuItem23;
		private System.Windows.Forms.MenuItem menuItem24;
		private System.Windows.Forms.MenuItem menuItem25;
		private System.Windows.Forms.MenuItem menuItem26;
		private System.Windows.Forms.MenuItem menuItem27;
		private System.Windows.Forms.MenuItem menuItem28;
		private System.Windows.Forms.MenuItem menuItem29;
		private System.Windows.Forms.MenuItem menuItem30;
		private System.Windows.Forms.MenuItem menuItem31;
//		private SCTV.playScheduleListView playScheduleList;
		
		public static mediaHandler myMedia = new mediaHandler();
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.PropertyGrid ShowNotificationPropertyGrid;
		private System.Windows.Forms.TabControl ScheduleTabControl;
		private System.Windows.Forms.ContextMenu rightClickTVListings;
		private System.Windows.Forms.MenuItem menuItem33;
		private System.Windows.Forms.MenuItem menuItem34;
		private System.Windows.Forms.TabPage playScheduleTab;
		private System.Windows.Forms.TabPage notificationScheduleTab;
		private System.Windows.Forms.TabPage mediaTab;
		private System.Windows.Forms.TabPage schedulesTab;
		private System.Windows.Forms.MenuItem menuItem32;
		private System.Windows.Forms.TabPage tvListingsTab;
        private SCTVTelevision.TVListView listView;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage TVGridView;
		private System.Windows.Forms.TabPage TVListView;
        private SCTVTelevision.TVRichTextBox MiscStuffRichText;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Splitter splitter4;
		private System.Windows.Forms.MenuItem menuItem35;
		private System.Windows.Forms.MenuItem menuItem36;
		private System.Windows.Forms.MenuItem menuItem37;
		private System.Windows.Forms.Timer ShowNotificationTimer;
		private System.Windows.Forms.MenuItem menuItem38;
		private System.Windows.Forms.MenuItem menuItem40;
		private System.Windows.Forms.MenuItem menuItemOnNow;
		private System.Windows.Forms.MenuItem menuItemOnNextFewHours;
		private System.Windows.Forms.MenuItem menuItemMoviesOnNow;
		private System.Windows.Forms.MenuItem menuItemMoviesToday;
		private System.Windows.Forms.MenuItem menuItemMoviesTomorrow;
		private System.Windows.Forms.MenuItem menuItem39;
		private System.Windows.Forms.MenuItem menuItem41;
		private System.Windows.Forms.MenuItem menuItem42;
        private SCTVTelevision.TVShowNotificationListView ShowNotificationListView;
		private System.Windows.Forms.ContextMenu rightClickShowNotification;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.Splitter splitter3;
		private System.Windows.Forms.MenuItem menuItem43;
		private System.Windows.Forms.TextBox OutputTxt;
		private string NotificationsFileName=Directory.GetCurrentDirectory()+"\\Config\\notifications.xml";
		private string RecordingsFileName=Directory.GetCurrentDirectory()+"\\Config\\record.xml";
		private string playSchedulesFileName=Directory.GetCurrentDirectory()+"\\Config\\playSchedules.xml";
		private string favoritesFileName=Directory.GetCurrentDirectory()+"\\Config\\favorites.xml";
		private System.Windows.Forms.MenuItem menuAddToPlaySchedule;
		private System.Windows.Forms.ColumnHeader columnHeader12;
		private System.Windows.Forms.ColumnHeader columnHeader13;
        private SCTVTelevision.TVRichTextBox richTextBoxSchedules;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.ColumnHeader ColTitle;
		private System.Windows.Forms.ColumnHeader ColTime;
        private SCTVTelevision.TVGridView playGridView;
		private System.Windows.Forms.ContextMenu rightClickPlayList;
		private System.Windows.Forms.MenuItem menuItem11;
		private System.Windows.Forms.Timer playScheduleTimer;
		private System.Windows.Forms.TabPage tpFavorites;
        public SCTVTelevision.favoritesListView lvFavorites;
		private System.Windows.Forms.MenuItem menuItem45;
		private System.Windows.Forms.ColumnHeader columnHeader14;
		private System.Windows.Forms.ColumnHeader columnHeader15;
		private System.Windows.Forms.ListView lvShowTimes;
		private System.Windows.Forms.ContextMenu rightClickShowTimes;
		private System.Windows.Forms.MenuItem menuItem46;
		private System.Windows.Forms.MenuItem menuItem47;
		private System.Windows.Forms.ContextMenu rightClickFavorites;
		private System.Windows.Forms.MenuItem menuItem48;
		private System.Windows.Forms.TabPage tpSearch;
		ProcessCaller XMLTVProc;
		private System.Windows.Forms.MenuItem menuItem49;
		private System.Windows.Forms.MenuItem menuItem50;
		private System.Windows.Forms.MenuItem menuItem51;
		private System.Windows.Forms.TabPage tpChannels;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.ListBox lbxChannelLists;
		private System.Windows.Forms.CheckedListBox chbLChannels;
		public TVProgramme[] favorites;
		private System.Windows.Forms.DataGrid dgChannels;
		private System.Windows.Forms.ImageList channelsImageList;
		private System.Windows.Forms.ListView channelListView;
		private System.Windows.Forms.MenuItem menuItem44;
		private System.Windows.Forms.MenuItem menuItem52;
		private System.Windows.Forms.MenuItem menuItem53;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.TabPage tabPage4;
		private System.Windows.Forms.TabControl mediaTabControl;
		private System.Windows.Forms.TabPage tabPage5;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.ColumnHeader columnHeader4;
		private System.Windows.Forms.ColumnHeader columnHeader5;
		private System.Windows.Forms.ColumnHeader columnHeader6;
		private System.Windows.Forms.ColumnHeader columnHeader7;
		private System.Windows.Forms.ColumnHeader columnHeader8;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.TabControl tabControl3;
		private System.Windows.Forms.CheckedListBox lBoxAudioTypes;
		private System.Windows.Forms.CheckedListBox lBoxAudioCat;
		private System.Windows.Forms.GroupBox groupBox6;
		private System.Windows.Forms.GroupBox groupBox7;
		private System.Windows.Forms.TextBox textBox2;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.CheckedListBox lBoxVideoLocations;
		private System.Windows.Forms.GroupBox groupBox8;
		private System.Windows.Forms.TabPage tabPage10;
		private System.Windows.Forms.TabPage tabPage11;
        public SCTVTelevision.mediaGridView videoListView;
        private SCTVTelevision.mediaGridView audioListView;
		private System.Windows.Forms.GroupBox groupBox9;
		private System.Windows.Forms.GroupBox groupBox10;
		private System.Windows.Forms.GroupBox groupBox11;
		private System.Windows.Forms.ComboBox comboBox2;
		private System.Windows.Forms.ListView imagesListView;
		private System.Windows.Forms.ComboBox cmbVideoCategories;
		private System.Windows.Forms.ComboBox cmbAudioCategories;
		private System.Windows.Forms.TextBox txtAddType;
		private System.Windows.Forms.TabControl tcFileTypes;
		public channelListsArrayList channelLists;
		XmlDocument xmlFileTypes = new XmlDocument();
		XmlDocument xmlLocations = new XmlDocument();
		private System.Windows.Forms.TabPage tpVideoCat;
		private System.Windows.Forms.TabPage tpAudioCat;
		private System.Windows.Forms.TabPage tpVideo;
		private System.Windows.Forms.TabPage tpAudio;
		private System.Windows.Forms.Button btnBrowseLocations;
		private System.Windows.Forms.FolderBrowserDialog OFolder;
		private System.Windows.Forms.OpenFileDialog OFile;
		private System.Windows.Forms.ListBox lBoxVideoTypes;
		private System.Windows.Forms.ListBox lBoxVideoCat;
		private System.Windows.Forms.Button btnDeleteCat;
		private System.Windows.Forms.Button btnDeleteType;
		private System.Windows.Forms.Button btnAddCat;
		private System.Windows.Forms.Button btnLookForMedia;
		private System.Windows.Forms.CheckBox subFoldersCheckBox;
		private System.Windows.Forms.GroupBox groupBox12;
		private System.Windows.Forms.GroupBox groupBox13;
		private System.Windows.Forms.ListView playNowListView;
		private System.Windows.Forms.Button btnStartPlayNow;
		private System.Windows.Forms.ColumnHeader columnHeaderTitle;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Button btnDeleteMediaLocation;
		private FileSystemWatcher _objFSW;
		private System.Windows.Forms.CheckBox chbPlayNowRepeat;
		private DataView dvMedia;
		private System.Windows.Forms.Panel pnlChannel;
		private System.Windows.Forms.Button btnChannel;
		private ArrayList keyStrokeTracker = new ArrayList();
		private System.Windows.Forms.Button btnPlayAll;
		private System.Windows.Forms.CheckBox chbRepeatVideoList;
		private SortedList macroList = new SortedList();
		private SortedList mainMenuMacroList = new SortedList();
		private SortedList mediaMacroList = new SortedList();
		private System.Windows.Forms.MenuItem menuItem54;
		private System.Windows.Forms.MenuItem menuItem55;
		private System.Windows.Forms.MenuItem menuItem56;
		private System.Windows.Forms.MenuItem menuItem57;
		private System.Windows.Forms.MenuItem menuItem58;
		private System.Windows.Forms.TabPage tpRecordList;
        private SCTVTelevision.TVShowNotificationListView tvShowRecordListView;
		private System.Windows.Forms.ColumnHeader columnHeader9;
		private System.Windows.Forms.ColumnHeader columnHeader10;
		private System.Windows.Forms.PropertyGrid recordingsPropertyGrid;
		private System.Windows.Forms.Button btnRecordStop;
		private SortedList keyList = new SortedList();
		private System.Windows.Forms.MenuItem menuItem59;
		public static TVViewer myTVViewer;
        //public splashScreen MySplash = new splashScreen();
		private System.Windows.Forms.Timer mouseHideTimer;
		private System.Windows.Forms.Timer changeChannelTimer;
		private System.Windows.Forms.Timer clock;
		public static guiState GUIState;
		private System.Windows.Forms.MenuItem menuItem60;
		private System.Windows.Forms.MenuItem menuItem61;
		private System.Windows.Forms.MenuItem menuItem62;
		private System.Windows.Forms.MenuItem menuItem63;
		private System.Windows.Forms.MenuItem menuItem64;
		private System.Windows.Forms.MenuItem menuItem65;
		private System.Windows.Forms.MenuItem menuItem66;
		private System.Windows.Forms.MenuItem menuItem67;
		DateTime favoritesUpdated = new DateTime();
        //public static speechRecognition speechListener;
        //public static mediaLibrary myMediaLibrary;
		public DateTime theTime = new DateTime();

        //volume variables
        // CONTROL TYPES
        //public const int spkrVolumeControl = MM.MIXERCONTROL_CONTROLTYPE_VOLUME;

        //public const int spkrMuteControl = MM.MIXERCONTROL_CONTROLTYPE_MUTE;

        // COMPONENT TYPES
        //public int spkrComponent = MM.MIXERLINE_COMPONENTTYPE_DST_SPEAKERS;

        // CONTROL IDs
        int spkrVolumeControlID = -1;
        int spkrMuteControlID = -1;

        const int spkrDown = 10;
        const int spkrUp = 11;

        int[] volSteps =    {       0,   2621,   5242,   7863,
                                10484,  13105,  15726,  18347,
                                20968,  23589,  26210,  28831,
                                31452,  34073,  36694,  39315,
                                41936,  44557,  47178,  49799,
                                52420,  55041,  57662,  60283,
                                62904,  65535 };

        int muteStatus = 0;
        private CheckBox chbClearMedia;

        public static string computerNickName = "";//the nickname of the computer used for speech recognition

        private int volumeStep = 35;

        private string defaultPathToSaveTo = "";

        //public static DeviceVolumeMonitor deviceMonitor;
        private Button btnClearMedia;

        ArrayList foundMediaFiles = new ArrayList();

        public ArrayList FoundMediaFiles
        {
            get { return foundMediaFiles; }
        }

        public int VolumeLevel
        {
            get 
            {
                //int volumeLevel = MM.GetVolume(spkrVolumeControl, spkrComponent);
                int returnPercentage = 0;

                //for (int x = 0; x < volSteps.Length; x++)
                //{
                //    if(volSteps[x] > volumeLevel)
                //    {
                //        if (x == 0)
                //            returnPercentage = 0;
                //        else
                //            returnPercentage = 100 / (int)Math.Round((double)(volSteps.Length / x));

                //        break;
                //    }
                //}

                return  returnPercentage;
            }
        }
		
		#endregion

        public delegate void FoundNewMedia(string mediaPath);
        public event FoundNewMedia foundMedia;

		#region static void Main()
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
        //[STAThread]
        //static void Main() 
        //{
        //    try
        //    {
        //        Application.Run(new Form1());
        //    }
        //    catch (Exception ex)
        //    {
        //        EventLog.WriteEntry("error", ex.Message);
        //    }
        //}
		#endregion

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.dgListings = new System.Windows.Forms.DataGrid();
            this.listings = new System.Windows.Forms.DataGridTableStyle();
            this.dataGridTextBoxColumn5 = new System.Windows.Forms.DataGridTextBoxColumn();
            this.dataGridTextBoxColumn6 = new System.Windows.Forms.DataGridTextBoxColumn();
            this.dataGridTextBoxColumn1 = new System.Windows.Forms.DataGridTextBoxColumn();
            this.dataGridTextBoxColumn2 = new System.Windows.Forms.DataGridTextBoxColumn();
            this.dataGridTextBoxColumn3 = new System.Windows.Forms.DataGridTextBoxColumn();
            this.dataGridTextBoxColumn4 = new System.Windows.Forms.DataGridTextBoxColumn();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tvListingsTab = new System.Windows.Forms.TabPage();
            this.MiscStuffRichText = new SCTVTelevision.TVRichTextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.TVGridView = new System.Windows.Forms.TabPage();
            this.TVListView = new System.Windows.Forms.TabPage();
            this.splitter4 = new System.Windows.Forms.Splitter();
            this.panel1 = new System.Windows.Forms.Panel();
            this.listView = new SCTVTelevision.TVListView();
            this.rightClickTVListings = new System.Windows.Forms.ContextMenu();
            this.menuItem35 = new System.Windows.Forms.MenuItem();
            this.menuItem32 = new System.Windows.Forms.MenuItem();
            this.menuItem38 = new System.Windows.Forms.MenuItem();
            this.menuItem45 = new System.Windows.Forms.MenuItem();
            this.menuItem33 = new System.Windows.Forms.MenuItem();
            this.menuItem34 = new System.Windows.Forms.MenuItem();
            this.menuItem40 = new System.Windows.Forms.MenuItem();
            this.menuItemOnNow = new System.Windows.Forms.MenuItem();
            this.menuItemOnNextFewHours = new System.Windows.Forms.MenuItem();
            this.menuItemMoviesOnNow = new System.Windows.Forms.MenuItem();
            this.menuItemMoviesToday = new System.Windows.Forms.MenuItem();
            this.menuItemMoviesTomorrow = new System.Windows.Forms.MenuItem();
            this.tpFavorites = new System.Windows.Forms.TabPage();
            this.lvShowTimes = new System.Windows.Forms.ListView();
            this.columnHeader15 = new System.Windows.Forms.ColumnHeader();
            this.rightClickShowTimes = new System.Windows.Forms.ContextMenu();
            this.menuItem46 = new System.Windows.Forms.MenuItem();
            this.menuItem47 = new System.Windows.Forms.MenuItem();
            this.menuItem54 = new System.Windows.Forms.MenuItem();
            this.menuItem55 = new System.Windows.Forms.MenuItem();
            this.channelsImageList = new System.Windows.Forms.ImageList(this.components);
            this.lvFavorites = new SCTVTelevision.favoritesListView();
            this.columnHeader14 = new System.Windows.Forms.ColumnHeader();
            this.rightClickFavorites = new System.Windows.Forms.ContextMenu();
            this.menuItem48 = new System.Windows.Forms.MenuItem();
            this.menuItem57 = new System.Windows.Forms.MenuItem();
            this.menuItem56 = new System.Windows.Forms.MenuItem();
            this.menuItem58 = new System.Windows.Forms.MenuItem();
            this.tpChannels = new System.Windows.Forms.TabPage();
            this.channelListView = new System.Windows.Forms.ListView();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lbxChannelLists = new System.Windows.Forms.ListBox();
            this.chbLChannels = new System.Windows.Forms.CheckedListBox();
            this.schedulesTab = new System.Windows.Forms.TabPage();
            this.richTextBoxSchedules = new SCTVTelevision.TVRichTextBox();
            this.ScheduleTabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.playScheduleTab = new System.Windows.Forms.TabPage();
            this.notificationScheduleTab = new System.Windows.Forms.TabPage();
            this.splitter3 = new System.Windows.Forms.Splitter();
            this.ShowNotificationListView = new SCTVTelevision.TVShowNotificationListView();
            this.ColTitle = new System.Windows.Forms.ColumnHeader();
            this.ColTime = new System.Windows.Forms.ColumnHeader();
            this.rightClickShowNotification = new System.Windows.Forms.ContextMenu();
            this.menuItem36 = new System.Windows.Forms.MenuItem();
            this.menuItem37 = new System.Windows.Forms.MenuItem();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.ShowNotificationPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.tpRecordList = new System.Windows.Forms.TabPage();
            this.btnRecordStop = new System.Windows.Forms.Button();
            this.recordingsPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.tvShowRecordListView = new SCTVTelevision.TVShowNotificationListView();
            this.columnHeader9 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader10 = new System.Windows.Forms.ColumnHeader();
            this.mediaTab = new System.Windows.Forms.TabPage();
            this.mediaTabControl = new System.Windows.Forms.TabControl();
            this.rightClickMedia = new System.Windows.Forms.ContextMenu();
            this.menuItem39 = new System.Windows.Forms.MenuItem();
            this.menuItem41 = new System.Windows.Forms.MenuItem();
            this.menuAddToPlaySchedule = new System.Windows.Forms.MenuItem();
            this.menuItem42 = new System.Windows.Forms.MenuItem();
            this.menuItem12 = new System.Windows.Forms.MenuItem();
            this.menuItem44 = new System.Windows.Forms.MenuItem();
            this.menuItem53 = new System.Windows.Forms.MenuItem();
            this.menuItem52 = new System.Windows.Forms.MenuItem();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.chbRepeatVideoList = new System.Windows.Forms.CheckBox();
            this.btnPlayAll = new System.Windows.Forms.Button();
            this.groupBox13 = new System.Windows.Forms.GroupBox();
            this.chbPlayNowRepeat = new System.Windows.Forms.CheckBox();
            this.button3 = new System.Windows.Forms.Button();
            this.btnStartPlayNow = new System.Windows.Forms.Button();
            this.playNowListView = new System.Windows.Forms.ListView();
            this.columnHeaderTitle = new System.Windows.Forms.ColumnHeader();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.cmbVideoCategories = new System.Windows.Forms.ComboBox();
            this.videoListView = new SCTVTelevision.mediaGridView();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader6 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader7 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader8 = new System.Windows.Forms.ColumnHeader();
            this.tabPage10 = new System.Windows.Forms.TabPage();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.cmbAudioCategories = new System.Windows.Forms.ComboBox();
            this.audioListView = new SCTVTelevision.mediaGridView();
            this.tabPage11 = new System.Windows.Forms.TabPage();
            this.imagesListView = new System.Windows.Forms.ListView();
            this.groupBox11 = new System.Windows.Forms.GroupBox();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.btnDeleteCat = new System.Windows.Forms.Button();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.btnAddCat = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.tabControl3 = new System.Windows.Forms.TabControl();
            this.tpVideoCat = new System.Windows.Forms.TabPage();
            this.lBoxVideoCat = new System.Windows.Forms.ListBox();
            this.tpAudioCat = new System.Windows.Forms.TabPage();
            this.lBoxAudioCat = new System.Windows.Forms.CheckedListBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btnDeleteType = new System.Windows.Forms.Button();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.txtAddType = new System.Windows.Forms.TextBox();
            this.tcFileTypes = new System.Windows.Forms.TabControl();
            this.tpVideo = new System.Windows.Forms.TabPage();
            this.lBoxVideoTypes = new System.Windows.Forms.ListBox();
            this.tpAudio = new System.Windows.Forms.TabPage();
            this.lBoxAudioTypes = new System.Windows.Forms.CheckedListBox();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.groupBox12 = new System.Windows.Forms.GroupBox();
            this.chbClearMedia = new System.Windows.Forms.CheckBox();
            this.btnLookForMedia = new System.Windows.Forms.Button();
            this.subFoldersCheckBox = new System.Windows.Forms.CheckBox();
            this.btnDeleteMediaLocation = new System.Windows.Forms.Button();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.btnBrowseLocations = new System.Windows.Forms.Button();
            this.lBoxVideoLocations = new System.Windows.Forms.CheckedListBox();
            this.tpSearch = new System.Windows.Forms.TabPage();
            this.dgChannels = new System.Windows.Forms.DataGrid();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.OutputTxt = new System.Windows.Forms.TextBox();
            this.columnHeader12 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader13 = new System.Windows.Forms.ColumnHeader();
            this.rightClickPlayList = new System.Windows.Forms.ContextMenu();
            this.menuItem11 = new System.Windows.Forms.MenuItem();
            this.btnChannel = new System.Windows.Forms.Button();
            this.pnlChannel = new System.Windows.Forms.Panel();
            this.lblChannel = new System.Windows.Forms.Label();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.mainMenu = new System.Windows.Forms.MainMenu(this.components);
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem10 = new System.Windows.Forms.MenuItem();
            this.mnuExit = new System.Windows.Forms.MenuItem();
            this.menuItem13 = new System.Windows.Forms.MenuItem();
            this.menuItem14 = new System.Windows.Forms.MenuItem();
            this.menuItem15 = new System.Windows.Forms.MenuItem();
            this.menuItem16 = new System.Windows.Forms.MenuItem();
            this.menuItem17 = new System.Windows.Forms.MenuItem();
            this.menuItem18 = new System.Windows.Forms.MenuItem();
            this.menuItem19 = new System.Windows.Forms.MenuItem();
            this.menuItem20 = new System.Windows.Forms.MenuItem();
            this.menuItem21 = new System.Windows.Forms.MenuItem();
            this.menuItem22 = new System.Windows.Forms.MenuItem();
            this.menuItem23 = new System.Windows.Forms.MenuItem();
            this.menuItem24 = new System.Windows.Forms.MenuItem();
            this.menuItem25 = new System.Windows.Forms.MenuItem();
            this.menuItem26 = new System.Windows.Forms.MenuItem();
            this.menuItem27 = new System.Windows.Forms.MenuItem();
            this.menuItem28 = new System.Windows.Forms.MenuItem();
            this.menuItem29 = new System.Windows.Forms.MenuItem();
            this.menuItem30 = new System.Windows.Forms.MenuItem();
            this.menuItem31 = new System.Windows.Forms.MenuItem();
            this.mnuDevices = new System.Windows.Forms.MenuItem();
            this.mnuVideoDevices = new System.Windows.Forms.MenuItem();
            this.mnuAudioDevices = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.mnuVideoCompressors = new System.Windows.Forms.MenuItem();
            this.mnuAudioCompressors = new System.Windows.Forms.MenuItem();
            this.menuItem7 = new System.Windows.Forms.MenuItem();
            this.mnuVideoSources = new System.Windows.Forms.MenuItem();
            this.mnuFrameSizes = new System.Windows.Forms.MenuItem();
            this.mnuFrameRates = new System.Windows.Forms.MenuItem();
            this.mnuVideoCaps = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.mnuAudioSources = new System.Windows.Forms.MenuItem();
            this.mnuAudioChannels = new System.Windows.Forms.MenuItem();
            this.mnuAudioSamplingRate = new System.Windows.Forms.MenuItem();
            this.mnuAudioSampleSizes = new System.Windows.Forms.MenuItem();
            this.mnuAudioCaps = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.mnuChannel = new System.Windows.Forms.MenuItem();
            this.mnuInputType = new System.Windows.Forms.MenuItem();
            this.menuItem6 = new System.Windows.Forms.MenuItem();
            this.mnuPropertyPages = new System.Windows.Forms.MenuItem();
            this.menuItem8 = new System.Windows.Forms.MenuItem();
            this.mnuPreview = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.menuItem9 = new System.Windows.Forms.MenuItem();
            this.menuItem43 = new System.Windows.Forms.MenuItem();
            this.menuItem49 = new System.Windows.Forms.MenuItem();
            this.menuItem50 = new System.Windows.Forms.MenuItem();
            this.menuItem51 = new System.Windows.Forms.MenuItem();
            this.menuItem59 = new System.Windows.Forms.MenuItem();
            this.menuItem60 = new System.Windows.Forms.MenuItem();
            this.menuItem61 = new System.Windows.Forms.MenuItem();
            this.menuItem62 = new System.Windows.Forms.MenuItem();
            this.menuItem63 = new System.Windows.Forms.MenuItem();
            this.menuItem64 = new System.Windows.Forms.MenuItem();
            this.menuItem65 = new System.Windows.Forms.MenuItem();
            this.menuItem66 = new System.Windows.Forms.MenuItem();
            this.menuItem67 = new System.Windows.Forms.MenuItem();
            this.ShowNotificationTimer = new System.Windows.Forms.Timer(this.components);
            this.playScheduleTimer = new System.Windows.Forms.Timer(this.components);
            this.OFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.OFile = new System.Windows.Forms.OpenFileDialog();
            this.mouseHideTimer = new System.Windows.Forms.Timer(this.components);
            this.changeChannelTimer = new System.Windows.Forms.Timer(this.components);
            this.clock = new System.Windows.Forms.Timer(this.components);
            this.btnClearMedia = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgListings)).BeginInit();
            this.tabControl.SuspendLayout();
            this.tvListingsTab.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.TVListView.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tpFavorites.SuspendLayout();
            this.tpChannels.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.schedulesTab.SuspendLayout();
            this.ScheduleTabControl.SuspendLayout();
            this.notificationScheduleTab.SuspendLayout();
            this.tpRecordList.SuspendLayout();
            this.mediaTab.SuspendLayout();
            this.mediaTabControl.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox13.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.tabPage10.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.tabPage11.SuspendLayout();
            this.groupBox11.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.tabControl3.SuspendLayout();
            this.tpVideoCat.SuspendLayout();
            this.tpAudioCat.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.tcFileTypes.SuspendLayout();
            this.tpVideo.SuspendLayout();
            this.tpAudio.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.groupBox12.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.tpSearch.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgChannels)).BeginInit();
            this.tabPage3.SuspendLayout();
            this.pnlChannel.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgListings
            // 
            this.dgListings.DataMember = "listings";
            this.dgListings.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgListings.Location = new System.Drawing.Point(0, 208);
            this.dgListings.Name = "dgListings";
            this.dgListings.Size = new System.Drawing.Size(832, 72);
            this.dgListings.TabIndex = 0;
            this.dgListings.TableStyles.AddRange(new System.Windows.Forms.DataGridTableStyle[] {
            this.listings});
            // 
            // listings
            // 
            this.listings.DataGrid = this.dgListings;
            this.listings.GridColumnStyles.AddRange(new System.Windows.Forms.DataGridColumnStyle[] {
            this.dataGridTextBoxColumn5,
            this.dataGridTextBoxColumn6,
            this.dataGridTextBoxColumn1,
            this.dataGridTextBoxColumn2,
            this.dataGridTextBoxColumn3,
            this.dataGridTextBoxColumn4});
            this.listings.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.listings.MappingName = "programme1";
            this.listings.PreferredRowHeight = 30;
            // 
            // dataGridTextBoxColumn5
            // 
            this.dataGridTextBoxColumn5.Format = "";
            this.dataGridTextBoxColumn5.FormatInfo = null;
            this.dataGridTextBoxColumn5.HeaderText = "Channel";
            this.dataGridTextBoxColumn5.MappingName = "channel";
            this.dataGridTextBoxColumn5.Width = 75;
            // 
            // dataGridTextBoxColumn6
            // 
            this.dataGridTextBoxColumn6.Format = "";
            this.dataGridTextBoxColumn6.FormatInfo = null;
            this.dataGridTextBoxColumn6.HeaderText = "Station";
            this.dataGridTextBoxColumn6.MappingName = "Station";
            this.dataGridTextBoxColumn6.Width = 75;
            // 
            // dataGridTextBoxColumn1
            // 
            this.dataGridTextBoxColumn1.Format = "";
            this.dataGridTextBoxColumn1.FormatInfo = null;
            this.dataGridTextBoxColumn1.HeaderText = "Beginning";
            this.dataGridTextBoxColumn1.MappingName = "start";
            this.dataGridTextBoxColumn1.Width = 75;
            // 
            // dataGridTextBoxColumn2
            // 
            this.dataGridTextBoxColumn2.Format = "";
            this.dataGridTextBoxColumn2.FormatInfo = null;
            this.dataGridTextBoxColumn2.HeaderText = "End";
            this.dataGridTextBoxColumn2.MappingName = "stop";
            this.dataGridTextBoxColumn2.Width = 75;
            // 
            // dataGridTextBoxColumn3
            // 
            this.dataGridTextBoxColumn3.Format = "";
            this.dataGridTextBoxColumn3.FormatInfo = null;
            this.dataGridTextBoxColumn3.HeaderText = "Name";
            this.dataGridTextBoxColumn3.MappingName = "title";
            this.dataGridTextBoxColumn3.Width = 75;
            // 
            // dataGridTextBoxColumn4
            // 
            this.dataGridTextBoxColumn4.Format = "";
            this.dataGridTextBoxColumn4.FormatInfo = null;
            this.dataGridTextBoxColumn4.HeaderText = "Description";
            this.dataGridTextBoxColumn4.MappingName = "desc";
            this.dataGridTextBoxColumn4.Width = 75;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tvListingsTab);
            this.tabControl.Controls.Add(this.schedulesTab);
            this.tabControl.Controls.Add(this.mediaTab);
            this.tabControl.Controls.Add(this.tpSearch);
            this.tabControl.Controls.Add(this.tabPage3);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Font = new System.Drawing.Font("Comic Sans MS", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(814, 473);
            this.tabControl.TabIndex = 2;
            // 
            // tvListingsTab
            // 
            this.tvListingsTab.Controls.Add(this.MiscStuffRichText);
            this.tvListingsTab.Controls.Add(this.tabControl1);
            this.tvListingsTab.Location = new System.Drawing.Point(4, 27);
            this.tvListingsTab.Name = "tvListingsTab";
            this.tvListingsTab.Size = new System.Drawing.Size(806, 442);
            this.tvListingsTab.TabIndex = 0;
            this.tvListingsTab.Text = "TV Listings";
            // 
            // MiscStuffRichText
            // 
            this.MiscStuffRichText.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.MiscStuffRichText.Location = new System.Drawing.Point(0, 370);
            this.MiscStuffRichText.Name = "MiscStuffRichText";
            this.MiscStuffRichText.Size = new System.Drawing.Size(806, 72);
            this.MiscStuffRichText.TabIndex = 5;
            this.MiscStuffRichText.Text = "";
            // 
            // tabControl1
            // 
            this.tabControl1.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabControl1.Controls.Add(this.TVGridView);
            this.tabControl1.Controls.Add(this.TVListView);
            this.tabControl1.Controls.Add(this.tpFavorites);
            this.tabControl1.Controls.Add(this.tpChannels);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabControl1.Font = new System.Drawing.Font("Comic Sans MS", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.HotTrack = true;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.ShowToolTips = true;
            this.tabControl1.Size = new System.Drawing.Size(806, 296);
            this.tabControl1.TabIndex = 4;
            // 
            // TVGridView
            // 
            this.TVGridView.Location = new System.Drawing.Point(4, 30);
            this.TVGridView.Name = "TVGridView";
            this.TVGridView.Size = new System.Drawing.Size(798, 262);
            this.TVGridView.TabIndex = 0;
            // 
            // TVListView
            // 
            this.TVListView.Controls.Add(this.splitter4);
            this.TVListView.Controls.Add(this.panel1);
            this.TVListView.Location = new System.Drawing.Point(4, 30);
            this.TVListView.Name = "TVListView";
            this.TVListView.Size = new System.Drawing.Size(798, 262);
            this.TVListView.TabIndex = 1;
            this.TVListView.Text = "List";
            // 
            // splitter4
            // 
            this.splitter4.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter4.Location = new System.Drawing.Point(795, 0);
            this.splitter4.Name = "splitter4";
            this.splitter4.Size = new System.Drawing.Size(3, 262);
            this.splitter4.TabIndex = 6;
            this.splitter4.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.listView);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(798, 262);
            this.panel1.TabIndex = 5;
            // 
            // listView
            // 
            this.listView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listView.ContextMenu = this.rightClickTVListings;
            this.listView.FullRowSelect = true;
            this.listView.GridLines = true;
            this.listView.HideSelection = false;
            this.listView.Location = new System.Drawing.Point(0, 0);
            this.listView.MultiSelect = false;
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(792, 256);
            this.listView.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listView.TabIndex = 3;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.VisibleChanged += new System.EventHandler(this.listView_VisibleChanged);
            this.listView.SelectedIndexChanged += new System.EventHandler(this.listView_SelectedIndexChanged);
            // 
            // rightClickTVListings
            // 
            this.rightClickTVListings.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem35,
            this.menuItem32,
            this.menuItem38,
            this.menuItem45,
            this.menuItem33,
            this.menuItem34,
            this.menuItem40,
            this.menuItemOnNow,
            this.menuItemOnNextFewHours,
            this.menuItemMoviesOnNow,
            this.menuItemMoviesToday,
            this.menuItemMoviesTomorrow});
            // 
            // menuItem35
            // 
            this.menuItem35.Index = 0;
            this.menuItem35.Text = "Watch";
            // 
            // menuItem32
            // 
            this.menuItem32.Index = 1;
            this.menuItem32.Text = "Record";
            this.menuItem32.Click += new System.EventHandler(this.AddNotificationContextMenu_Click);
            // 
            // menuItem38
            // 
            this.menuItem38.Index = 2;
            this.menuItem38.Text = "-";
            // 
            // menuItem45
            // 
            this.menuItem45.Index = 3;
            this.menuItem45.Text = "Add to Favorites";
            this.menuItem45.Click += new System.EventHandler(this.menuItem45_Click);
            // 
            // menuItem33
            // 
            this.menuItem33.Index = 4;
            this.menuItem33.Text = "Add to Play Schedule";
            this.menuItem33.Click += new System.EventHandler(this.menuAddToPlaySchedule_Click);
            // 
            // menuItem34
            // 
            this.menuItem34.Index = 5;
            this.menuItem34.Text = "Add to Notification Schedule";
            this.menuItem34.Click += new System.EventHandler(this.AddNotificationContextMenu_Click);
            // 
            // menuItem40
            // 
            this.menuItem40.Index = 6;
            this.menuItem40.Text = "-";
            // 
            // menuItemOnNow
            // 
            this.menuItemOnNow.Index = 7;
            this.menuItemOnNow.Text = "On Now";
            this.menuItemOnNow.Click += new System.EventHandler(this.TVListViewSelect);
            // 
            // menuItemOnNextFewHours
            // 
            this.menuItemOnNextFewHours.Index = 8;
            this.menuItemOnNextFewHours.Text = "On in Next Few Hours";
            this.menuItemOnNextFewHours.Click += new System.EventHandler(this.TVListViewSelect);
            // 
            // menuItemMoviesOnNow
            // 
            this.menuItemMoviesOnNow.Index = 9;
            this.menuItemMoviesOnNow.Text = "Movies On Now";
            this.menuItemMoviesOnNow.Click += new System.EventHandler(this.TVListViewSelect);
            // 
            // menuItemMoviesToday
            // 
            this.menuItemMoviesToday.Index = 10;
            this.menuItemMoviesToday.Text = "Movies Today";
            this.menuItemMoviesToday.Click += new System.EventHandler(this.TVListViewSelect);
            // 
            // menuItemMoviesTomorrow
            // 
            this.menuItemMoviesTomorrow.Index = 11;
            this.menuItemMoviesTomorrow.Text = "Movies Tomorrow";
            this.menuItemMoviesTomorrow.Click += new System.EventHandler(this.TVListViewSelect);
            // 
            // tpFavorites
            // 
            this.tpFavorites.Controls.Add(this.lvShowTimes);
            this.tpFavorites.Controls.Add(this.lvFavorites);
            this.tpFavorites.Location = new System.Drawing.Point(4, 30);
            this.tpFavorites.Name = "tpFavorites";
            this.tpFavorites.Size = new System.Drawing.Size(798, 262);
            this.tpFavorites.TabIndex = 2;
            this.tpFavorites.Text = "Favorites";
            // 
            // lvShowTimes
            // 
            this.lvShowTimes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader15});
            this.lvShowTimes.ContextMenu = this.rightClickShowTimes;
            this.lvShowTimes.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvShowTimes.FullRowSelect = true;
            this.lvShowTimes.GridLines = true;
            this.lvShowTimes.HideSelection = false;
            this.lvShowTimes.Location = new System.Drawing.Point(248, 0);
            this.lvShowTimes.MultiSelect = false;
            this.lvShowTimes.Name = "lvShowTimes";
            this.lvShowTimes.Size = new System.Drawing.Size(520, 336);
            this.lvShowTimes.StateImageList = this.channelsImageList;
            this.lvShowTimes.TabIndex = 1;
            this.lvShowTimes.UseCompatibleStateImageBehavior = false;
            this.lvShowTimes.View = System.Windows.Forms.View.Details;
            this.lvShowTimes.SelectedIndexChanged += new System.EventHandler(this.lvShowTimes_SelectedIndexChanged);
            // 
            // columnHeader15
            // 
            this.columnHeader15.Text = "Show Times";
            this.columnHeader15.Width = 512;
            // 
            // rightClickShowTimes
            // 
            this.rightClickShowTimes.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem46,
            this.menuItem47,
            this.menuItem54,
            this.menuItem55});
            // 
            // menuItem46
            // 
            this.menuItem46.Index = 0;
            this.menuItem46.Text = "Add To Play Schedule";
            // 
            // menuItem47
            // 
            this.menuItem47.Index = 1;
            this.menuItem47.Text = "Add to Notification Schedule";
            // 
            // menuItem54
            // 
            this.menuItem54.Index = 2;
            this.menuItem54.Text = "-";
            // 
            // menuItem55
            // 
            this.menuItem55.Index = 3;
            this.menuItem55.Text = "Record";
            // 
            // channelsImageList
            // 
            this.channelsImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("channelsImageList.ImageStream")));
            this.channelsImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.channelsImageList.Images.SetKeyName(0, "");
            this.channelsImageList.Images.SetKeyName(1, "");
            // 
            // lvFavorites
            // 
            this.lvFavorites.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader14});
            this.lvFavorites.ContextMenu = this.rightClickFavorites;
            this.lvFavorites.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvFavorites.FullRowSelect = true;
            this.lvFavorites.GridLines = true;
            this.lvFavorites.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvFavorites.HideSelection = false;
            this.lvFavorites.Location = new System.Drawing.Point(0, 0);
            this.lvFavorites.MultiSelect = false;
            this.lvFavorites.Name = "lvFavorites";
            this.lvFavorites.Size = new System.Drawing.Size(240, 336);
            this.lvFavorites.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvFavorites.TabIndex = 0;
            this.lvFavorites.UseCompatibleStateImageBehavior = false;
            this.lvFavorites.View = System.Windows.Forms.View.Details;
            this.lvFavorites.SelectedIndexChanged += new System.EventHandler(this.lvFavorites_SelectedIndexChanged);
            // 
            // columnHeader14
            // 
            this.columnHeader14.Text = "Show";
            this.columnHeader14.Width = 236;
            // 
            // rightClickFavorites
            // 
            this.rightClickFavorites.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem48,
            this.menuItem57,
            this.menuItem56,
            this.menuItem58});
            // 
            // menuItem48
            // 
            this.menuItem48.Index = 0;
            this.menuItem48.Text = "Remove";
            this.menuItem48.Click += new System.EventHandler(this.menuItem48_Click);
            // 
            // menuItem57
            // 
            this.menuItem57.Index = 1;
            this.menuItem57.Text = "-";
            // 
            // menuItem56
            // 
            this.menuItem56.Index = 2;
            this.menuItem56.Text = "Record All";
            // 
            // menuItem58
            // 
            this.menuItem58.Index = 3;
            this.menuItem58.Text = "Notify";
            // 
            // tpChannels
            // 
            this.tpChannels.Controls.Add(this.channelListView);
            this.tpChannels.Controls.Add(this.groupBox3);
            this.tpChannels.Controls.Add(this.chbLChannels);
            this.tpChannels.Location = new System.Drawing.Point(4, 30);
            this.tpChannels.Name = "tpChannels";
            this.tpChannels.Size = new System.Drawing.Size(798, 262);
            this.tpChannels.TabIndex = 3;
            this.tpChannels.Text = "Channels";
            // 
            // channelListView
            // 
            this.channelListView.LabelWrap = false;
            this.channelListView.LargeImageList = this.channelsImageList;
            this.channelListView.Location = new System.Drawing.Point(0, 0);
            this.channelListView.Name = "channelListView";
            this.channelListView.Size = new System.Drawing.Size(360, 368);
            this.channelListView.StateImageList = this.channelsImageList;
            this.channelListView.TabIndex = 4;
            this.channelListView.UseCompatibleStateImageBehavior = false;
            this.channelListView.SelectedIndexChanged += new System.EventHandler(this.channelListView_SelectedIndexChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lbxChannelLists);
            this.groupBox3.Location = new System.Drawing.Point(424, 0);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(152, 232);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Channel Lists";
            // 
            // lbxChannelLists
            // 
            this.lbxChannelLists.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbxChannelLists.ItemHeight = 18;
            this.lbxChannelLists.Location = new System.Drawing.Point(3, 22);
            this.lbxChannelLists.Name = "lbxChannelLists";
            this.lbxChannelLists.Size = new System.Drawing.Size(146, 202);
            this.lbxChannelLists.TabIndex = 2;
            // 
            // chbLChannels
            // 
            this.chbLChannels.Location = new System.Drawing.Point(632, 16);
            this.chbLChannels.MultiColumn = true;
            this.chbLChannels.Name = "chbLChannels";
            this.chbLChannels.Size = new System.Drawing.Size(128, 277);
            this.chbLChannels.TabIndex = 1;
            this.chbLChannels.ThreeDCheckBoxes = true;
            // 
            // schedulesTab
            // 
            this.schedulesTab.Controls.Add(this.richTextBoxSchedules);
            this.schedulesTab.Controls.Add(this.ScheduleTabControl);
            this.schedulesTab.Location = new System.Drawing.Point(4, 27);
            this.schedulesTab.Name = "schedulesTab";
            this.schedulesTab.Size = new System.Drawing.Size(806, 0);
            this.schedulesTab.TabIndex = 4;
            this.schedulesTab.Text = "Schedules";
            // 
            // richTextBoxSchedules
            // 
            this.richTextBoxSchedules.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.richTextBoxSchedules.Location = new System.Drawing.Point(0, -56);
            this.richTextBoxSchedules.Name = "richTextBoxSchedules";
            this.richTextBoxSchedules.Size = new System.Drawing.Size(806, 56);
            this.richTextBoxSchedules.TabIndex = 5;
            this.richTextBoxSchedules.Text = "";
            // 
            // ScheduleTabControl
            // 
            this.ScheduleTabControl.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.ScheduleTabControl.Controls.Add(this.tabPage1);
            this.ScheduleTabControl.Controls.Add(this.playScheduleTab);
            this.ScheduleTabControl.Controls.Add(this.notificationScheduleTab);
            this.ScheduleTabControl.Controls.Add(this.tpRecordList);
            this.ScheduleTabControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.ScheduleTabControl.Font = new System.Drawing.Font("Comic Sans MS", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ScheduleTabControl.HotTrack = true;
            this.ScheduleTabControl.ItemSize = new System.Drawing.Size(42, 24);
            this.ScheduleTabControl.Location = new System.Drawing.Point(0, 0);
            this.ScheduleTabControl.Name = "ScheduleTabControl";
            this.ScheduleTabControl.SelectedIndex = 0;
            this.ScheduleTabControl.ShowToolTips = true;
            this.ScheduleTabControl.Size = new System.Drawing.Size(806, 312);
            this.ScheduleTabControl.TabIndex = 3;
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 28);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(798, 280);
            this.tabPage1.TabIndex = 3;
            this.tabPage1.Text = "Play Grid";
            // 
            // playScheduleTab
            // 
            this.playScheduleTab.Location = new System.Drawing.Point(4, 28);
            this.playScheduleTab.Name = "playScheduleTab";
            this.playScheduleTab.Size = new System.Drawing.Size(798, 280);
            this.playScheduleTab.TabIndex = 1;
            this.playScheduleTab.Text = "Play List";
            // 
            // notificationScheduleTab
            // 
            this.notificationScheduleTab.Controls.Add(this.splitter3);
            this.notificationScheduleTab.Controls.Add(this.ShowNotificationListView);
            this.notificationScheduleTab.Controls.Add(this.splitter1);
            this.notificationScheduleTab.Controls.Add(this.ShowNotificationPropertyGrid);
            this.notificationScheduleTab.Location = new System.Drawing.Point(4, 28);
            this.notificationScheduleTab.Name = "notificationScheduleTab";
            this.notificationScheduleTab.Size = new System.Drawing.Size(798, 280);
            this.notificationScheduleTab.TabIndex = 2;
            this.notificationScheduleTab.Text = "Notification List";
            // 
            // splitter3
            // 
            this.splitter3.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter3.Location = new System.Drawing.Point(403, 0);
            this.splitter3.Name = "splitter3";
            this.splitter3.Size = new System.Drawing.Size(3, 280);
            this.splitter3.TabIndex = 5;
            this.splitter3.TabStop = false;
            // 
            // ShowNotificationListView
            // 
            this.ShowNotificationListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ColTitle,
            this.ColTime});
            this.ShowNotificationListView.ContextMenu = this.rightClickShowNotification;
            this.ShowNotificationListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ShowNotificationListView.FullRowSelect = true;
            this.ShowNotificationListView.GridLines = true;
            this.ShowNotificationListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.ShowNotificationListView.Location = new System.Drawing.Point(0, 0);
            this.ShowNotificationListView.Name = "ShowNotificationListView";
            this.ShowNotificationListView.Size = new System.Drawing.Size(406, 280);
            this.ShowNotificationListView.TabIndex = 3;
            this.ShowNotificationListView.UseCompatibleStateImageBehavior = false;
            this.ShowNotificationListView.View = System.Windows.Forms.View.Details;
            this.ShowNotificationListView.SelectedIndexChanged += new System.EventHandler(this.ShowNotificationListView_SelectedIndexChanged);
            // 
            // ColTitle
            // 
            this.ColTitle.Text = "Title";
            this.ColTitle.Width = 300;
            // 
            // ColTime
            // 
            this.ColTime.Text = "Time";
            this.ColTime.Width = 102;
            // 
            // rightClickShowNotification
            // 
            this.rightClickShowNotification.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem36,
            this.menuItem37});
            // 
            // menuItem36
            // 
            this.menuItem36.Index = 0;
            this.menuItem36.Text = "Remove Notification";
            this.menuItem36.Click += new System.EventHandler(this.ShowNotificationContextMenuRemoveShow_Click);
            // 
            // menuItem37
            // 
            this.menuItem37.Index = 1;
            this.menuItem37.Text = "Add to Play Schedule";
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter1.Location = new System.Drawing.Point(406, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(8, 280);
            this.splitter1.TabIndex = 4;
            this.splitter1.TabStop = false;
            // 
            // ShowNotificationPropertyGrid
            // 
            this.ShowNotificationPropertyGrid.Dock = System.Windows.Forms.DockStyle.Right;
            this.ShowNotificationPropertyGrid.HelpVisible = false;
            this.ShowNotificationPropertyGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
            this.ShowNotificationPropertyGrid.Location = new System.Drawing.Point(414, 0);
            this.ShowNotificationPropertyGrid.Name = "ShowNotificationPropertyGrid";
            this.ShowNotificationPropertyGrid.Size = new System.Drawing.Size(384, 280);
            this.ShowNotificationPropertyGrid.TabIndex = 2;
            this.ShowNotificationPropertyGrid.ToolbarVisible = false;
            this.ShowNotificationPropertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.ShowNotificationPropertyGrid_PropertyValueChanged);
            // 
            // tpRecordList
            // 
            this.tpRecordList.Controls.Add(this.btnRecordStop);
            this.tpRecordList.Controls.Add(this.recordingsPropertyGrid);
            this.tpRecordList.Controls.Add(this.tvShowRecordListView);
            this.tpRecordList.Location = new System.Drawing.Point(4, 28);
            this.tpRecordList.Name = "tpRecordList";
            this.tpRecordList.Size = new System.Drawing.Size(798, 280);
            this.tpRecordList.TabIndex = 4;
            this.tpRecordList.Text = "Record List";
            // 
            // btnRecordStop
            // 
            this.btnRecordStop.Enabled = false;
            this.btnRecordStop.Location = new System.Drawing.Point(232, 216);
            this.btnRecordStop.Name = "btnRecordStop";
            this.btnRecordStop.Size = new System.Drawing.Size(75, 23);
            this.btnRecordStop.TabIndex = 6;
            this.btnRecordStop.Text = "Stop";
            // 
            // recordingsPropertyGrid
            // 
            this.recordingsPropertyGrid.Dock = System.Windows.Forms.DockStyle.Right;
            this.recordingsPropertyGrid.HelpVisible = false;
            this.recordingsPropertyGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
            this.recordingsPropertyGrid.Location = new System.Drawing.Point(390, 0);
            this.recordingsPropertyGrid.Name = "recordingsPropertyGrid";
            this.recordingsPropertyGrid.Size = new System.Drawing.Size(408, 280);
            this.recordingsPropertyGrid.TabIndex = 5;
            this.recordingsPropertyGrid.ToolbarVisible = false;
            // 
            // tvShowRecordListView
            // 
            this.tvShowRecordListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader9,
            this.columnHeader10});
            this.tvShowRecordListView.ContextMenu = this.rightClickShowNotification;
            this.tvShowRecordListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvShowRecordListView.FullRowSelect = true;
            this.tvShowRecordListView.GridLines = true;
            this.tvShowRecordListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.tvShowRecordListView.Location = new System.Drawing.Point(0, 0);
            this.tvShowRecordListView.Name = "tvShowRecordListView";
            this.tvShowRecordListView.Size = new System.Drawing.Size(798, 280);
            this.tvShowRecordListView.TabIndex = 4;
            this.tvShowRecordListView.UseCompatibleStateImageBehavior = false;
            this.tvShowRecordListView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "Title";
            this.columnHeader9.Width = 300;
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "Time";
            this.columnHeader10.Width = 102;
            // 
            // mediaTab
            // 
            this.mediaTab.Controls.Add(this.mediaTabControl);
            this.mediaTab.Location = new System.Drawing.Point(4, 27);
            this.mediaTab.Name = "mediaTab";
            this.mediaTab.Size = new System.Drawing.Size(806, 442);
            this.mediaTab.TabIndex = 5;
            this.mediaTab.Text = "Media";
            // 
            // mediaTabControl
            // 
            this.mediaTabControl.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.mediaTabControl.ContextMenu = this.rightClickMedia;
            this.mediaTabControl.Controls.Add(this.tabPage2);
            this.mediaTabControl.Controls.Add(this.tabPage10);
            this.mediaTabControl.Controls.Add(this.tabPage11);
            this.mediaTabControl.Controls.Add(this.tabPage4);
            this.mediaTabControl.Controls.Add(this.tabPage5);
            this.mediaTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mediaTabControl.Font = new System.Drawing.Font("Comic Sans MS", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mediaTabControl.HotTrack = true;
            this.mediaTabControl.Location = new System.Drawing.Point(0, 0);
            this.mediaTabControl.Multiline = true;
            this.mediaTabControl.Name = "mediaTabControl";
            this.mediaTabControl.SelectedIndex = 0;
            this.mediaTabControl.Size = new System.Drawing.Size(806, 442);
            this.mediaTabControl.TabIndex = 1;
            // 
            // rightClickMedia
            // 
            this.rightClickMedia.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem39,
            this.menuItem41,
            this.menuAddToPlaySchedule,
            this.menuItem42,
            this.menuItem12,
            this.menuItem44,
            this.menuItem53,
            this.menuItem52});
            // 
            // menuItem39
            // 
            this.menuItem39.Index = 0;
            this.menuItem39.Text = "Play";
            this.menuItem39.Click += new System.EventHandler(this.menuItemPlaySelectedMedia);
            // 
            // menuItem41
            // 
            this.menuItem41.Index = 1;
            this.menuItem41.Text = "-";
            // 
            // menuAddToPlaySchedule
            // 
            this.menuAddToPlaySchedule.Index = 2;
            this.menuAddToPlaySchedule.Text = "Add to Play Schedule";
            this.menuAddToPlaySchedule.Click += new System.EventHandler(this.menuAddToPlaySchedule_Click);
            // 
            // menuItem42
            // 
            this.menuItem42.Index = 3;
            this.menuItem42.Text = "-";
            // 
            // menuItem12
            // 
            this.menuItem12.Index = 4;
            this.menuItem12.Text = "Look Up Information";
            // 
            // menuItem44
            // 
            this.menuItem44.Index = 5;
            this.menuItem44.Text = "-";
            // 
            // menuItem53
            // 
            this.menuItem53.Index = 6;
            this.menuItem53.Text = "Add File";
            // 
            // menuItem52
            // 
            this.menuItem52.Index = 7;
            this.menuItem52.Text = "Search For Media";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.chbRepeatVideoList);
            this.tabPage2.Controls.Add(this.btnPlayAll);
            this.tabPage2.Controls.Add(this.groupBox13);
            this.tabPage2.Controls.Add(this.groupBox9);
            this.tabPage2.Controls.Add(this.videoListView);
            this.tabPage2.Location = new System.Drawing.Point(4, 30);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(798, 408);
            this.tabPage2.TabIndex = 0;
            this.tabPage2.Text = "Video";
            // 
            // chbRepeatVideoList
            // 
            this.chbRepeatVideoList.Checked = true;
            this.chbRepeatVideoList.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbRepeatVideoList.Location = new System.Drawing.Point(240, 24);
            this.chbRepeatVideoList.Name = "chbRepeatVideoList";
            this.chbRepeatVideoList.Size = new System.Drawing.Size(72, 24);
            this.chbRepeatVideoList.TabIndex = 5;
            this.chbRepeatVideoList.Text = "Repeat";
            // 
            // btnPlayAll
            // 
            this.btnPlayAll.Location = new System.Drawing.Point(152, 24);
            this.btnPlayAll.Name = "btnPlayAll";
            this.btnPlayAll.Size = new System.Drawing.Size(75, 23);
            this.btnPlayAll.TabIndex = 4;
            this.btnPlayAll.Text = "Play All";
            // 
            // groupBox13
            // 
            this.groupBox13.Controls.Add(this.chbPlayNowRepeat);
            this.groupBox13.Controls.Add(this.button3);
            this.groupBox13.Controls.Add(this.btnStartPlayNow);
            this.groupBox13.Controls.Add(this.playNowListView);
            this.groupBox13.Location = new System.Drawing.Point(352, 0);
            this.groupBox13.Name = "groupBox13";
            this.groupBox13.Size = new System.Drawing.Size(360, 352);
            this.groupBox13.TabIndex = 3;
            this.groupBox13.TabStop = false;
            this.groupBox13.Text = "Play Now";
            // 
            // chbPlayNowRepeat
            // 
            this.chbPlayNowRepeat.Checked = true;
            this.chbPlayNowRepeat.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbPlayNowRepeat.Location = new System.Drawing.Point(192, 24);
            this.chbPlayNowRepeat.Name = "chbPlayNowRepeat";
            this.chbPlayNowRepeat.Size = new System.Drawing.Size(72, 24);
            this.chbPlayNowRepeat.TabIndex = 3;
            this.chbPlayNowRepeat.Text = "Repeat";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(96, 24);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 2;
            this.button3.Text = "Clear";
            // 
            // btnStartPlayNow
            // 
            this.btnStartPlayNow.Location = new System.Drawing.Point(8, 24);
            this.btnStartPlayNow.Name = "btnStartPlayNow";
            this.btnStartPlayNow.Size = new System.Drawing.Size(75, 23);
            this.btnStartPlayNow.TabIndex = 1;
            this.btnStartPlayNow.Text = "Start";
            // 
            // playNowListView
            // 
            this.playNowListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderTitle});
            this.playNowListView.FullRowSelect = true;
            this.playNowListView.GridLines = true;
            this.playNowListView.Location = new System.Drawing.Point(8, 56);
            this.playNowListView.Name = "playNowListView";
            this.playNowListView.Size = new System.Drawing.Size(344, 288);
            this.playNowListView.TabIndex = 0;
            this.playNowListView.UseCompatibleStateImageBehavior = false;
            this.playNowListView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeaderTitle
            // 
            this.columnHeaderTitle.Text = "Title";
            this.columnHeaderTitle.Width = 300;
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.cmbVideoCategories);
            this.groupBox9.Location = new System.Drawing.Point(0, 0);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(136, 64);
            this.groupBox9.TabIndex = 2;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "Categories";
            // 
            // cmbVideoCategories
            // 
            this.cmbVideoCategories.Location = new System.Drawing.Point(8, 24);
            this.cmbVideoCategories.Name = "cmbVideoCategories";
            this.cmbVideoCategories.Size = new System.Drawing.Size(121, 26);
            this.cmbVideoCategories.Sorted = true;
            this.cmbVideoCategories.TabIndex = 1;
            this.cmbVideoCategories.Text = "All";
            this.cmbVideoCategories.SelectedIndexChanged += new System.EventHandler(this.cmbVideoCategories_SelectedIndexChanged);
            // 
            // videoListView
            // 
            this.videoListView.AllowColumnReorder = true;
            this.videoListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7,
            this.columnHeader8});
            this.videoListView.ContextMenu = this.rightClickMedia;
            this.videoListView.Cursor = System.Windows.Forms.Cursors.Hand;
            this.videoListView.Font = new System.Drawing.Font("Comic Sans MS", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.videoListView.FullRowSelect = true;
            this.videoListView.GridLines = true;
            this.videoListView.LabelEdit = true;
            this.videoListView.Location = new System.Drawing.Point(0, 64);
            this.videoListView.Name = "videoListView";
            this.videoListView.Size = new System.Drawing.Size(352, 280);
            this.videoListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.videoListView.TabIndex = 0;
            this.videoListView.UseCompatibleStateImageBehavior = false;
            this.videoListView.View = System.Windows.Forms.View.Details;
            this.videoListView.VisibleChanged += new System.EventHandler(this.videoListView_VisibleChanged);
            this.videoListView.DoubleClick += new System.EventHandler(this.videoListView_DoubleClick);
            this.videoListView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.videoListView_MouseDown);
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Title";
            this.columnHeader2.Width = 300;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Description";
            this.columnHeader3.Width = 300;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Category";
            this.columnHeader4.Width = 65;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Played";
            this.columnHeader5.Width = 40;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Stars";
            this.columnHeader6.Width = 65;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Rating";
            this.columnHeader7.Width = 65;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "Performers";
            this.columnHeader8.Width = 100;
            // 
            // tabPage10
            // 
            this.tabPage10.Controls.Add(this.groupBox10);
            this.tabPage10.Controls.Add(this.audioListView);
            this.tabPage10.Location = new System.Drawing.Point(4, 175);
            this.tabPage10.Name = "tabPage10";
            this.tabPage10.Size = new System.Drawing.Size(798, 0);
            this.tabPage10.TabIndex = 3;
            this.tabPage10.Text = "Audio";
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.cmbAudioCategories);
            this.groupBox10.Location = new System.Drawing.Point(0, 0);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Size = new System.Drawing.Size(136, 64);
            this.groupBox10.TabIndex = 3;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "Categories";
            // 
            // cmbAudioCategories
            // 
            this.cmbAudioCategories.Location = new System.Drawing.Point(8, 24);
            this.cmbAudioCategories.Name = "cmbAudioCategories";
            this.cmbAudioCategories.Size = new System.Drawing.Size(121, 26);
            this.cmbAudioCategories.TabIndex = 1;
            this.cmbAudioCategories.Text = "All";
            this.cmbAudioCategories.SelectionChangeCommitted += new System.EventHandler(this.cmbAudioCategories_SelectionChangeCommitted);
            // 
            // audioListView
            // 
            this.audioListView.FullRowSelect = true;
            this.audioListView.GridLines = true;
            this.audioListView.Location = new System.Drawing.Point(0, 64);
            this.audioListView.Name = "audioListView";
            this.audioListView.Size = new System.Drawing.Size(288, 312);
            this.audioListView.TabIndex = 0;
            this.audioListView.UseCompatibleStateImageBehavior = false;
            this.audioListView.View = System.Windows.Forms.View.Details;
            // 
            // tabPage11
            // 
            this.tabPage11.Controls.Add(this.imagesListView);
            this.tabPage11.Controls.Add(this.groupBox11);
            this.tabPage11.Location = new System.Drawing.Point(4, 175);
            this.tabPage11.Name = "tabPage11";
            this.tabPage11.Size = new System.Drawing.Size(798, 0);
            this.tabPage11.TabIndex = 4;
            this.tabPage11.Text = "Images";
            // 
            // imagesListView
            // 
            this.imagesListView.Location = new System.Drawing.Point(128, 96);
            this.imagesListView.Name = "imagesListView";
            this.imagesListView.Size = new System.Drawing.Size(456, 192);
            this.imagesListView.TabIndex = 4;
            this.imagesListView.UseCompatibleStateImageBehavior = false;
            // 
            // groupBox11
            // 
            this.groupBox11.Controls.Add(this.comboBox2);
            this.groupBox11.Location = new System.Drawing.Point(0, 0);
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.Size = new System.Drawing.Size(136, 64);
            this.groupBox11.TabIndex = 3;
            this.groupBox11.TabStop = false;
            this.groupBox11.Text = "Categories";
            // 
            // comboBox2
            // 
            this.comboBox2.Location = new System.Drawing.Point(8, 24);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(121, 26);
            this.comboBox2.TabIndex = 1;
            this.comboBox2.Text = "All";
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.groupBox5);
            this.tabPage4.Controls.Add(this.groupBox4);
            this.tabPage4.Location = new System.Drawing.Point(4, 175);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(798, 0);
            this.tabPage4.TabIndex = 1;
            this.tabPage4.Text = "File Assoc.";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.btnDeleteCat);
            this.groupBox5.Controls.Add(this.groupBox7);
            this.groupBox5.Controls.Add(this.tabControl3);
            this.groupBox5.Location = new System.Drawing.Point(296, 0);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(280, 216);
            this.groupBox5.TabIndex = 1;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Categories";
            // 
            // btnDeleteCat
            // 
            this.btnDeleteCat.Location = new System.Drawing.Point(128, 184);
            this.btnDeleteCat.Name = "btnDeleteCat";
            this.btnDeleteCat.Size = new System.Drawing.Size(144, 23);
            this.btnDeleteCat.TabIndex = 2;
            this.btnDeleteCat.Text = "Delete Selected";
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.btnAddCat);
            this.groupBox7.Controls.Add(this.textBox2);
            this.groupBox7.Location = new System.Drawing.Point(128, 16);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(144, 96);
            this.groupBox7.TabIndex = 1;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Add a Category";
            // 
            // btnAddCat
            // 
            this.btnAddCat.Location = new System.Drawing.Point(32, 64);
            this.btnAddCat.Name = "btnAddCat";
            this.btnAddCat.Size = new System.Drawing.Size(75, 23);
            this.btnAddCat.TabIndex = 1;
            this.btnAddCat.Text = "Add";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(8, 24);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(128, 26);
            this.textBox2.TabIndex = 0;
            // 
            // tabControl3
            // 
            this.tabControl3.Controls.Add(this.tpVideoCat);
            this.tabControl3.Controls.Add(this.tpAudioCat);
            this.tabControl3.Location = new System.Drawing.Point(8, 24);
            this.tabControl3.Name = "tabControl3";
            this.tabControl3.SelectedIndex = 0;
            this.tabControl3.Size = new System.Drawing.Size(112, 184);
            this.tabControl3.TabIndex = 0;
            // 
            // tpVideoCat
            // 
            this.tpVideoCat.Controls.Add(this.lBoxVideoCat);
            this.tpVideoCat.Location = new System.Drawing.Point(4, 27);
            this.tpVideoCat.Name = "tpVideoCat";
            this.tpVideoCat.Size = new System.Drawing.Size(104, 153);
            this.tpVideoCat.TabIndex = 0;
            this.tpVideoCat.Text = "Video";
            // 
            // lBoxVideoCat
            // 
            this.lBoxVideoCat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lBoxVideoCat.ItemHeight = 18;
            this.lBoxVideoCat.Location = new System.Drawing.Point(0, 0);
            this.lBoxVideoCat.Name = "lBoxVideoCat";
            this.lBoxVideoCat.Size = new System.Drawing.Size(104, 148);
            this.lBoxVideoCat.TabIndex = 0;
            // 
            // tpAudioCat
            // 
            this.tpAudioCat.Controls.Add(this.lBoxAudioCat);
            this.tpAudioCat.Location = new System.Drawing.Point(4, 22);
            this.tpAudioCat.Name = "tpAudioCat";
            this.tpAudioCat.Size = new System.Drawing.Size(104, 158);
            this.tpAudioCat.TabIndex = 1;
            this.tpAudioCat.Text = "Audio";
            // 
            // lBoxAudioCat
            // 
            this.lBoxAudioCat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lBoxAudioCat.Location = new System.Drawing.Point(0, 0);
            this.lBoxAudioCat.Name = "lBoxAudioCat";
            this.lBoxAudioCat.Size = new System.Drawing.Size(104, 151);
            this.lBoxAudioCat.TabIndex = 0;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btnDeleteType);
            this.groupBox4.Controls.Add(this.groupBox6);
            this.groupBox4.Controls.Add(this.tcFileTypes);
            this.groupBox4.Location = new System.Drawing.Point(0, 0);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(288, 216);
            this.groupBox4.TabIndex = 0;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "File Types";
            // 
            // btnDeleteType
            // 
            this.btnDeleteType.Location = new System.Drawing.Point(136, 184);
            this.btnDeleteType.Name = "btnDeleteType";
            this.btnDeleteType.Size = new System.Drawing.Size(144, 23);
            this.btnDeleteType.TabIndex = 2;
            this.btnDeleteType.Text = "Delete Selected";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.button1);
            this.groupBox6.Controls.Add(this.txtAddType);
            this.groupBox6.Location = new System.Drawing.Point(128, 16);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(152, 96);
            this.groupBox6.TabIndex = 1;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Add Extension";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(40, 64);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Add";
            this.button1.Click += new System.EventHandler(this.btnAddFileTypes_Click);
            // 
            // txtAddType
            // 
            this.txtAddType.Location = new System.Drawing.Point(8, 24);
            this.txtAddType.Name = "txtAddType";
            this.txtAddType.Size = new System.Drawing.Size(136, 26);
            this.txtAddType.TabIndex = 0;
            // 
            // tcFileTypes
            // 
            this.tcFileTypes.Controls.Add(this.tpVideo);
            this.tcFileTypes.Controls.Add(this.tpAudio);
            this.tcFileTypes.Location = new System.Drawing.Point(8, 24);
            this.tcFileTypes.Name = "tcFileTypes";
            this.tcFileTypes.SelectedIndex = 0;
            this.tcFileTypes.Size = new System.Drawing.Size(112, 184);
            this.tcFileTypes.TabIndex = 0;
            // 
            // tpVideo
            // 
            this.tpVideo.Controls.Add(this.lBoxVideoTypes);
            this.tpVideo.Location = new System.Drawing.Point(4, 27);
            this.tpVideo.Name = "tpVideo";
            this.tpVideo.Size = new System.Drawing.Size(104, 153);
            this.tpVideo.TabIndex = 0;
            this.tpVideo.Text = "Video";
            // 
            // lBoxVideoTypes
            // 
            this.lBoxVideoTypes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lBoxVideoTypes.ItemHeight = 18;
            this.lBoxVideoTypes.Location = new System.Drawing.Point(0, 0);
            this.lBoxVideoTypes.Name = "lBoxVideoTypes";
            this.lBoxVideoTypes.Size = new System.Drawing.Size(104, 148);
            this.lBoxVideoTypes.TabIndex = 0;
            // 
            // tpAudio
            // 
            this.tpAudio.Controls.Add(this.lBoxAudioTypes);
            this.tpAudio.Location = new System.Drawing.Point(4, 22);
            this.tpAudio.Name = "tpAudio";
            this.tpAudio.Size = new System.Drawing.Size(104, 158);
            this.tpAudio.TabIndex = 1;
            this.tpAudio.Text = "Audio";
            // 
            // lBoxAudioTypes
            // 
            this.lBoxAudioTypes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lBoxAudioTypes.Location = new System.Drawing.Point(0, 0);
            this.lBoxAudioTypes.Name = "lBoxAudioTypes";
            this.lBoxAudioTypes.Size = new System.Drawing.Size(104, 151);
            this.lBoxAudioTypes.TabIndex = 0;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.groupBox12);
            this.tabPage5.Controls.Add(this.btnDeleteMediaLocation);
            this.tabPage5.Controls.Add(this.groupBox8);
            this.tabPage5.Controls.Add(this.lBoxVideoLocations);
            this.tabPage5.Location = new System.Drawing.Point(4, 30);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Size = new System.Drawing.Size(798, 408);
            this.tabPage5.TabIndex = 2;
            this.tabPage5.Text = "Locations";
            // 
            // groupBox12
            // 
            this.groupBox12.Controls.Add(this.btnClearMedia);
            this.groupBox12.Controls.Add(this.chbClearMedia);
            this.groupBox12.Controls.Add(this.btnLookForMedia);
            this.groupBox12.Controls.Add(this.subFoldersCheckBox);
            this.groupBox12.Location = new System.Drawing.Point(381, 160);
            this.groupBox12.Name = "groupBox12";
            this.groupBox12.Size = new System.Drawing.Size(200, 138);
            this.groupBox12.TabIndex = 4;
            this.groupBox12.TabStop = false;
            this.groupBox12.Text = "Look For Media";
            // 
            // chbClearMedia
            // 
            this.chbClearMedia.AutoSize = true;
            this.chbClearMedia.Location = new System.Drawing.Point(16, 78);
            this.chbClearMedia.Name = "chbClearMedia";
            this.chbClearMedia.Size = new System.Drawing.Size(100, 22);
            this.chbClearMedia.TabIndex = 4;
            this.chbClearMedia.Text = "Clear Media";
            this.chbClearMedia.UseVisualStyleBackColor = true;
            // 
            // btnLookForMedia
            // 
            this.btnLookForMedia.Location = new System.Drawing.Point(71, 106);
            this.btnLookForMedia.Name = "btnLookForMedia";
            this.btnLookForMedia.Size = new System.Drawing.Size(64, 23);
            this.btnLookForMedia.TabIndex = 3;
            this.btnLookForMedia.Text = "Look";
            this.btnLookForMedia.Click += new System.EventHandler(this.btnLookForMedia_Click);
            // 
            // subFoldersCheckBox
            // 
            this.subFoldersCheckBox.Checked = true;
            this.subFoldersCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.subFoldersCheckBox.Location = new System.Drawing.Point(16, 48);
            this.subFoldersCheckBox.Name = "subFoldersCheckBox";
            this.subFoldersCheckBox.Size = new System.Drawing.Size(176, 24);
            this.subFoldersCheckBox.TabIndex = 2;
            this.subFoldersCheckBox.Text = "Include Subfolders";
            // 
            // btnDeleteMediaLocation
            // 
            this.btnDeleteMediaLocation.Location = new System.Drawing.Point(152, 160);
            this.btnDeleteMediaLocation.Name = "btnDeleteMediaLocation";
            this.btnDeleteMediaLocation.Size = new System.Drawing.Size(144, 23);
            this.btnDeleteMediaLocation.TabIndex = 2;
            this.btnDeleteMediaLocation.Text = "Delete Selected";
            this.btnDeleteMediaLocation.Click += new System.EventHandler(this.btnDeleteMediaLocation_Click);
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.btnBrowseLocations);
            this.groupBox8.Location = new System.Drawing.Point(8, 160);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(128, 56);
            this.groupBox8.TabIndex = 1;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Add Location";
            // 
            // btnBrowseLocations
            // 
            this.btnBrowseLocations.Location = new System.Drawing.Point(16, 24);
            this.btnBrowseLocations.Name = "btnBrowseLocations";
            this.btnBrowseLocations.Size = new System.Drawing.Size(96, 23);
            this.btnBrowseLocations.TabIndex = 1;
            this.btnBrowseLocations.Text = "Browse...";
            this.btnBrowseLocations.Click += new System.EventHandler(this.btnBrowseLocations_Click);
            // 
            // lBoxVideoLocations
            // 
            this.lBoxVideoLocations.Location = new System.Drawing.Point(0, 0);
            this.lBoxVideoLocations.Name = "lBoxVideoLocations";
            this.lBoxVideoLocations.Size = new System.Drawing.Size(776, 151);
            this.lBoxVideoLocations.TabIndex = 0;
            // 
            // tpSearch
            // 
            this.tpSearch.Controls.Add(this.dgChannels);
            this.tpSearch.Location = new System.Drawing.Point(4, 27);
            this.tpSearch.Name = "tpSearch";
            this.tpSearch.Size = new System.Drawing.Size(806, 0);
            this.tpSearch.TabIndex = 6;
            this.tpSearch.Text = "Search";
            // 
            // dgChannels
            // 
            this.dgChannels.DataMember = "";
            this.dgChannels.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgChannels.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgChannels.Location = new System.Drawing.Point(0, 0);
            this.dgChannels.Name = "dgChannels";
            this.dgChannels.Size = new System.Drawing.Size(806, 0);
            this.dgChannels.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.OutputTxt);
            this.tabPage3.Location = new System.Drawing.Point(4, 27);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(806, 0);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "xmltv output";
            // 
            // OutputTxt
            // 
            this.OutputTxt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OutputTxt.Location = new System.Drawing.Point(0, 0);
            this.OutputTxt.Multiline = true;
            this.OutputTxt.Name = "OutputTxt";
            this.OutputTxt.Size = new System.Drawing.Size(806, 0);
            this.OutputTxt.TabIndex = 0;
            // 
            // columnHeader12
            // 
            this.columnHeader12.Text = "Title";
            // 
            // columnHeader13
            // 
            this.columnHeader13.Text = "Time";
            // 
            // rightClickPlayList
            // 
            this.rightClickPlayList.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem11});
            // 
            // menuItem11
            // 
            this.menuItem11.Index = 0;
            this.menuItem11.Text = "Delete";
            this.menuItem11.Click += new System.EventHandler(this.menuItem11_Click);
            // 
            // btnChannel
            // 
            this.btnChannel.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnChannel.Location = new System.Drawing.Point(502, 24);
            this.btnChannel.Name = "btnChannel";
            this.btnChannel.Size = new System.Drawing.Size(75, 230);
            this.btnChannel.TabIndex = 7;
            this.btnChannel.UseVisualStyleBackColor = false;
            // 
            // pnlChannel
            // 
            this.pnlChannel.BackColor = System.Drawing.Color.Transparent;
            this.pnlChannel.Controls.Add(this.lblChannel);
            this.pnlChannel.Font = new System.Drawing.Font("Comic Sans MS", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnlChannel.ForeColor = System.Drawing.Color.LimeGreen;
            this.pnlChannel.Location = new System.Drawing.Point(616, 216);
            this.pnlChannel.Name = "pnlChannel";
            this.pnlChannel.Size = new System.Drawing.Size(88, 80);
            this.pnlChannel.TabIndex = 6;
            // 
            // lblChannel
            // 
            this.lblChannel.BackColor = System.Drawing.Color.Tomato;
            this.lblChannel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblChannel.ForeColor = System.Drawing.Color.LawnGreen;
            this.lblChannel.Location = new System.Drawing.Point(0, 0);
            this.lblChannel.Name = "lblChannel";
            this.lblChannel.Size = new System.Drawing.Size(88, 80);
            this.lblChannel.TabIndex = 5;
            this.lblChannel.Visible = false;
            // 
            // mainMenu
            // 
            this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1,
            this.menuItem13,
            this.menuItem25,
            this.mnuDevices,
            this.menuItem7,
            this.menuItem2,
            this.menuItem59,
            this.menuItem60,
            this.menuItem67});
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 0;
            this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem10,
            this.mnuExit});
            this.menuItem1.Text = "File";
            // 
            // menuItem10
            // 
            this.menuItem10.Index = 0;
            this.menuItem10.Text = "&Open";
            this.menuItem10.Click += new System.EventHandler(this.menuItem10_Click);
            // 
            // mnuExit
            // 
            this.mnuExit.Index = 1;
            this.mnuExit.Text = "E&xit";
            this.mnuExit.Click += new System.EventHandler(this.mnuExit_Click);
            // 
            // menuItem13
            // 
            this.menuItem13.Index = 1;
            this.menuItem13.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem14,
            this.menuItem15,
            this.menuItem16,
            this.menuItem17,
            this.menuItem18,
            this.menuItem19,
            this.menuItem20,
            this.menuItem21,
            this.menuItem22,
            this.menuItem23,
            this.menuItem24});
            this.menuItem13.Text = "Controls";
            // 
            // menuItem14
            // 
            this.menuItem14.Index = 0;
            this.menuItem14.Shortcut = System.Windows.Forms.Shortcut.CtrlQ;
            this.menuItem14.Text = "Play/Pause";
            // 
            // menuItem15
            // 
            this.menuItem15.Index = 1;
            this.menuItem15.Shortcut = System.Windows.Forms.Shortcut.CtrlW;
            this.menuItem15.Text = "Stop";
            // 
            // menuItem16
            // 
            this.menuItem16.Index = 2;
            this.menuItem16.Shortcut = System.Windows.Forms.Shortcut.AltF3;
            this.menuItem16.Text = "Mute/Unmute";
            // 
            // menuItem17
            // 
            this.menuItem17.Index = 3;
            this.menuItem17.Text = "-";
            // 
            // menuItem18
            // 
            this.menuItem18.Index = 4;
            this.menuItem18.Text = "Single Frame Step";
            // 
            // menuItem19
            // 
            this.menuItem19.Index = 5;
            this.menuItem19.Text = "-";
            // 
            // menuItem20
            // 
            this.menuItem20.Index = 6;
            this.menuItem20.Text = "Half Size";
            // 
            // menuItem21
            // 
            this.menuItem21.Index = 7;
            this.menuItem21.Text = "3/4 Size";
            // 
            // menuItem22
            // 
            this.menuItem22.Index = 8;
            this.menuItem22.Text = "Normal Size";
            // 
            // menuItem23
            // 
            this.menuItem23.Index = 9;
            this.menuItem23.Text = "Double Size";
            // 
            // menuItem24
            // 
            this.menuItem24.Index = 10;
            this.menuItem24.Shortcut = System.Windows.Forms.Shortcut.CtrlA;
            this.menuItem24.Text = "Full Screen";
            // 
            // menuItem25
            // 
            this.menuItem25.Index = 2;
            this.menuItem25.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem26,
            this.menuItem27,
            this.menuItem28,
            this.menuItem29,
            this.menuItem30,
            this.menuItem31});
            this.menuItem25.Text = "Rate";
            // 
            // menuItem26
            // 
            this.menuItem26.Index = 0;
            this.menuItem26.Text = "Increase Playback Rate";
            // 
            // menuItem27
            // 
            this.menuItem27.Index = 1;
            this.menuItem27.Text = "Decrease Playback Rate";
            // 
            // menuItem28
            // 
            this.menuItem28.Index = 2;
            this.menuItem28.Text = "-";
            // 
            // menuItem29
            // 
            this.menuItem29.Index = 3;
            this.menuItem29.Text = "Normal Playback Rate";
            // 
            // menuItem30
            // 
            this.menuItem30.Index = 4;
            this.menuItem30.Text = "Half Playback Rate";
            // 
            // menuItem31
            // 
            this.menuItem31.Index = 5;
            this.menuItem31.Text = "Double Playback Rate";
            // 
            // mnuDevices
            // 
            this.mnuDevices.Index = 3;
            this.mnuDevices.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuVideoDevices,
            this.mnuAudioDevices,
            this.menuItem4,
            this.mnuVideoCompressors,
            this.mnuAudioCompressors});
            this.mnuDevices.Text = "Devices";
            // 
            // mnuVideoDevices
            // 
            this.mnuVideoDevices.Index = 0;
            this.mnuVideoDevices.Text = "Video Devices";
            // 
            // mnuAudioDevices
            // 
            this.mnuAudioDevices.Index = 1;
            this.mnuAudioDevices.Text = "Audio Devices";
            // 
            // menuItem4
            // 
            this.menuItem4.Index = 2;
            this.menuItem4.Text = "-";
            // 
            // mnuVideoCompressors
            // 
            this.mnuVideoCompressors.Index = 3;
            this.mnuVideoCompressors.Text = "Video Compressors";
            // 
            // mnuAudioCompressors
            // 
            this.mnuAudioCompressors.Index = 4;
            this.mnuAudioCompressors.Text = "Audio Compressors";
            // 
            // menuItem7
            // 
            this.menuItem7.Index = 4;
            this.menuItem7.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuVideoSources,
            this.mnuFrameSizes,
            this.mnuFrameRates,
            this.mnuVideoCaps,
            this.menuItem5,
            this.mnuAudioSources,
            this.mnuAudioChannels,
            this.mnuAudioSamplingRate,
            this.mnuAudioSampleSizes,
            this.mnuAudioCaps,
            this.menuItem3,
            this.mnuChannel,
            this.mnuInputType,
            this.menuItem6,
            this.mnuPropertyPages,
            this.menuItem8,
            this.mnuPreview});
            this.menuItem7.Text = "Options";
            // 
            // mnuVideoSources
            // 
            this.mnuVideoSources.Index = 0;
            this.mnuVideoSources.Text = "Video Sources";
            // 
            // mnuFrameSizes
            // 
            this.mnuFrameSizes.Index = 1;
            this.mnuFrameSizes.Text = "Video Frame Size";
            // 
            // mnuFrameRates
            // 
            this.mnuFrameRates.Index = 2;
            this.mnuFrameRates.Text = "Video Frame Rate";
            // 
            // mnuVideoCaps
            // 
            this.mnuVideoCaps.Index = 3;
            this.mnuVideoCaps.Text = "Video Capabilities...";
            // 
            // menuItem5
            // 
            this.menuItem5.Index = 4;
            this.menuItem5.Text = "-";
            // 
            // mnuAudioSources
            // 
            this.mnuAudioSources.Index = 5;
            this.mnuAudioSources.Text = "Audio Sources";
            // 
            // mnuAudioChannels
            // 
            this.mnuAudioChannels.Index = 6;
            this.mnuAudioChannels.Text = "Audio Channels";
            // 
            // mnuAudioSamplingRate
            // 
            this.mnuAudioSamplingRate.Index = 7;
            this.mnuAudioSamplingRate.Text = "Audio Sampling Rate";
            // 
            // mnuAudioSampleSizes
            // 
            this.mnuAudioSampleSizes.Index = 8;
            this.mnuAudioSampleSizes.Text = "Audio Sample Size";
            // 
            // mnuAudioCaps
            // 
            this.mnuAudioCaps.Index = 9;
            this.mnuAudioCaps.Text = "Audio Capabilities...";
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 10;
            this.menuItem3.Text = "-";
            // 
            // mnuChannel
            // 
            this.mnuChannel.Index = 11;
            this.mnuChannel.Text = "TV Tuner Channel";
            // 
            // mnuInputType
            // 
            this.mnuInputType.Index = 12;
            this.mnuInputType.Text = "TV Tuner Input Type";
            // 
            // menuItem6
            // 
            this.menuItem6.Index = 13;
            this.menuItem6.Text = "-";
            // 
            // mnuPropertyPages
            // 
            this.mnuPropertyPages.Index = 14;
            this.mnuPropertyPages.Text = "PropertyPages";
            // 
            // menuItem8
            // 
            this.menuItem8.Index = 15;
            this.menuItem8.Text = "-";
            // 
            // mnuPreview
            // 
            this.mnuPreview.Index = 16;
            this.mnuPreview.Text = "Preview";
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 5;
            this.menuItem2.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem9,
            this.menuItem43,
            this.menuItem49});
            this.menuItem2.Text = "Settings";
            // 
            // menuItem9
            // 
            this.menuItem9.Index = 0;
            this.menuItem9.Text = "Configure";
            this.menuItem9.Click += new System.EventHandler(this.menuItem9_Click);
            // 
            // menuItem43
            // 
            this.menuItem43.Index = 1;
            this.menuItem43.Text = "-";
            // 
            // menuItem49
            // 
            this.menuItem49.Index = 2;
            this.menuItem49.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem50,
            this.menuItem51});
            this.menuItem49.Text = "TV";
            // 
            // menuItem50
            // 
            this.menuItem50.Index = 0;
            this.menuItem50.Text = "Channels";
            // 
            // menuItem51
            // 
            this.menuItem51.Index = 1;
            this.menuItem51.Text = "Update TV Listings";
            this.menuItem51.Click += new System.EventHandler(this.menuUpdateTVListings_Click);
            // 
            // menuItem59
            // 
            this.menuItem59.Index = 6;
            this.menuItem59.Text = "Show Popup";
            this.menuItem59.Click += new System.EventHandler(this.menuItem59_Click);
            // 
            // menuItem60
            // 
            this.menuItem60.Index = 7;
            this.menuItem60.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem61,
            this.menuItem62,
            this.menuItem63,
            this.menuItem64,
            this.menuItem65,
            this.menuItem66});
            this.menuItem60.Text = "Remote";
            // 
            // menuItem61
            // 
            this.menuItem61.Index = 0;
            this.menuItem61.Text = "TV";
            this.menuItem61.Click += new System.EventHandler(this.menuItem61_Click);
            // 
            // menuItem62
            // 
            this.menuItem62.Index = 1;
            this.menuItem62.Text = "FM";
            // 
            // menuItem63
            // 
            this.menuItem63.Index = 2;
            this.menuItem63.Text = "Music";
            // 
            // menuItem64
            // 
            this.menuItem64.Index = 3;
            this.menuItem64.Text = "Pictures";
            // 
            // menuItem65
            // 
            this.menuItem65.Index = 4;
            this.menuItem65.Text = "Video";
            this.menuItem65.Click += new System.EventHandler(this.menuItem65_Click);
            // 
            // menuItem66
            // 
            this.menuItem66.Index = 5;
            this.menuItem66.Text = "DVD";
            this.menuItem66.Click += new System.EventHandler(this.menuItem66_Click);
            // 
            // menuItem67
            // 
            this.menuItem67.Index = 8;
            this.menuItem67.Text = "Test";
            // 
            // mouseHideTimer
            // 
            this.mouseHideTimer.Enabled = true;
            this.mouseHideTimer.Interval = 500;
            // 
            // clock
            // 
            this.clock.Enabled = true;
            this.clock.Interval = 5000;
            this.clock.Tick += new System.EventHandler(this.clock_Tick);
            // 
            // btnClearMedia
            // 
            this.btnClearMedia.Location = new System.Drawing.Point(60, 19);
            this.btnClearMedia.Name = "btnClearMedia";
            this.btnClearMedia.Size = new System.Drawing.Size(75, 23);
            this.btnClearMedia.TabIndex = 5;
            this.btnClearMedia.Text = "Clear Media";
            this.btnClearMedia.UseVisualStyleBackColor = true;
            this.btnClearMedia.Click += new System.EventHandler(this.btnClearMedia_Click);
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(7, 19);
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(814, 473);
            this.Controls.Add(this.tabControl);
            this.Font = new System.Drawing.Font("Comic Sans MS", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.HelpButton = true;
            this.KeyPreview = true;
            this.Menu = this.mainMenu;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.WindowsDefaultBounds;
            this.Text = "SCTV";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseUp);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
            this.Closed += new System.EventHandler(this.Form1_Closed);
            this.VisibleChanged += new System.EventHandler(this.Form1_VisibleChanged);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.KeyUpHandler);
            ((System.ComponentModel.ISupportInitialize)(this.dgListings)).EndInit();
            this.tabControl.ResumeLayout(false);
            this.tvListingsTab.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.TVListView.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.tpFavorites.ResumeLayout(false);
            this.tpChannels.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.schedulesTab.ResumeLayout(false);
            this.ScheduleTabControl.ResumeLayout(false);
            this.notificationScheduleTab.ResumeLayout(false);
            this.tpRecordList.ResumeLayout(false);
            this.mediaTab.ResumeLayout(false);
            this.mediaTabControl.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.groupBox13.ResumeLayout(false);
            this.groupBox9.ResumeLayout(false);
            this.tabPage10.ResumeLayout(false);
            this.groupBox10.ResumeLayout(false);
            this.tabPage11.ResumeLayout(false);
            this.groupBox11.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.tabControl3.ResumeLayout(false);
            this.tpVideoCat.ResumeLayout(false);
            this.tpAudioCat.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.tcFileTypes.ResumeLayout(false);
            this.tpVideo.ResumeLayout(false);
            this.tpAudio.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.groupBox12.ResumeLayout(false);
            this.groupBox12.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.tpSearch.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgChannels)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.pnlChannel.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Form1 constructor
		/// </summary>
        public Form1()
        {

            string newMode = System.Configuration.ConfigurationManager.AppSettings["Mode"];
            if (newMode != null && newMode.Length > 0)
                Mode = newMode;

            //if (Mode == "Release")
            //    MySplash.Show();

            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            //			InputListener inputListener = new InputListener(); 
            //			inputListener.OnMouseMove += new InputListener.MouseMoveHandler( InputListener_MouseMove); 
            //			inputListener.OnMouseButton += new InputListener.MouseButtonHandler( InputListener_MouseButton);               
            //			inputListener.OnKeyPress += new InputListener.KeyPressHandler( InputListener_KeyPress);                  
            //			inputListener.Run();
        }

        /// <summary>
        /// handle form load event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void Form1_Load(object sender, System.EventArgs e)
		{
            try
            {
                if (isActivated())
                {
                    computerNickName = getComputerNickName();

                    int.TryParse(System.Configuration.ConfigurationManager.AppSettings["Volume.Step"], out volumeStep);

                    defaultPathToSaveTo = System.Configuration.ConfigurationManager.AppSettings["Media.DefaultPathToSaveTo"];
                    if (defaultPathToSaveTo == null || defaultPathToSaveTo.Trim().Length == 0)
                    {
                        //defaultPathToSaveTo = Application.StartupPath + "\\Video\\";
                        defaultPathToSaveTo = Application.StartupPath + "\\";
                    }

                    //speechListener = new speechRecognition("xmlmain.xml");//listen for voice commands
                    //speechListener.executeCommand += new SCTV.speechRecognition.heardCommand(speechListener_executeCommand);
                    //speechListener.Show();

                    Microsoft.ApplicationBlocks.ConfigurationManagement.ConfigurationManager.Initialize();//start program default configuration manager

                    favoriteOrganizer.OnListChanged += new SCTVTelevision.favoritesOrganizer.favoritesListChanged(favoriteOrganizer_onListChange);
                    ListingsOrganizer.OnListChanged += new SCTVTelevision.ProgrammeOrganizer.lisitngsListChanged(loadListings);
                    pScheduleOrganizer.OnListChanged += new SCTVTelevision.playScheduleOrganizer.playScheduleListChanged(populatePlayGrid);

                    //			NotificationOrganizer.OnListChanged+= new SCTV.ShowNotificationOrganizer.showNotificationListChanged();
                    //			myTVViewer.KeyDown += new System.Windows.Forms.KeyEventHandler(TVViewer_KeyDown);
                    try
                    {
                        ProgramConfiguration = (ConfigurationData)Microsoft.ApplicationBlocks.ConfigurationManagement.ConfigurationManager.Read("XmlSerializer");
                    }
                    catch (Exception ex)
                    {
                        ExceptionManager.Publish(ex);
                        MessageBox.Show(ex.Message, "Error");
                        EventLog.WriteEntry("SCTV", ex.Message);
                    }

                    //load tv listings
                    loadListings();

                    xmlFileTypes.Load("config/fileTypes.xml");
                    xmlLocations.Load("config/locations.xml");

                    //			lookForMedia();//make sure no media was added while the program was off.

                    updateCLBoxes(lBoxAudioTypes, xmlFileTypes.GetElementsByTagName("audio"));
                    updateCLBoxes(lBoxVideoLocations, xmlLocations.GetElementsByTagName("locations"));
                    updateCLBoxes(lBoxVideoTypes, xmlFileTypes.GetElementsByTagName("video"));

                    if (File.Exists(NotificationsFileName))
                    {
                        try
                        {
                            NotificationOrganizer.LoadShowNotifications(NotificationsFileName);
                            ShowNotificationListView.SetOrganizer = NotificationOrganizer;
                        }
                        catch (Exception ex)
                        {
                            EventLog.WriteEntry("SCTV", ex.Message);
                        }
                    }

                    //if (File.Exists(RecordingsFileName))
                    //{
                    //    try
                    //    {
                    //					RecordingOrganizer.LoadShowNotifications(RecordingsFileName);
                    //					tvShowRecordListView.SetOrganizer=RecordingOrganizer;
                    //    }
                    //    catch(Exception ex)
                    //    {
                    //        EventLog.WriteEntry("sctv",ex.ToString());
                    //    }
                    //}

                    if (File.Exists(playSchedulesFileName))
                    {
                        try
                        {
                            //					pScheduleOrganizer.LoadplaySchedules(playSchedulesFileName);
                            //					playScheduleList.SetOrganizer=pScheduleOrganizer;
                        }
                        catch (Exception ex)
                        {
                            EventLog.WriteEntry("SCTV", ex.Message);
                        }
                    }

                    if (File.Exists(favoritesFileName))
                    {
                        try
                        {
                            //EventLog.WriteEntry("sctv","loading Favorites");
                            favoriteOrganizer.Loadfavoritess(favoritesFileName);
                            lvFavorites.SetOrganizer = favoriteOrganizer;
                        }
                        catch (Exception ex)
                        {
                            EventLog.WriteEntry("SCTV", ex.Message);
                        }
                    }
                    populateMacros();
                    populateKeyStrokes();
                    //			populateMediaView();
                    //			updateCategories(myMedia.getAllCategories("video"));


                    if (File.Exists("config/channelLists.xml"))
                    {
                        populateChannelLists();
                    }
                    else
                    {
                        channelLists = new channelListsArrayList();
                        channelLists.OnListChanged += new SCTVTelevision.channelListsArrayList.channelListsListChanged(channelListChanged);
                        //EventLog.WriteEntry("sctv","adding to channelList");
                        channelLists.Add(ListingsOrganizer.AllChannels);
                        //				EventLog.WriteEntry("sctv","done adding to channelList size: "+ channelLists.Count.ToString());

                        //				createDefaultChannelLists();
                        //				channelLists = new channelListsArrayList("allChannels");
                    }

                    try
                    {
                        taskbarNotifier = new TaskbarNotifier();
                        //taskbarNotifier.SetBackgroundBitmap(Application.StartupPath +"\\images\\skin.bmp",Color.FromArgb(255,0,255));
                        taskbarNotifier.SetBackgroundBitmap(Application.StartupPath + "\\images\\notificationBar\\darkBlueBar.bmp", Color.FromArgb(255, 0, 255));
                        taskbarNotifier.SetCloseBitmap(Application.StartupPath + "\\images\\close.bmp", Color.FromArgb(255, 0, 255), new Point(300, 74));
                        taskbarNotifier.TitleRectangle = new Rectangle(123, 80, 176, 16);
                        taskbarNotifier.ContentRectangle = new Rectangle(116, 97, 197, 22);
                        taskbarNotifier.CloseClickable = true;
                        taskbarNotifier.TitleClickable = true;
                        taskbarNotifier.ContentClickable = true;
                        taskbarNotifier.EnableSelectionRectangle = true;
                        taskbarNotifier.KeepVisibleOnMousOver = true;
                        taskbarNotifier.ReShowOnMouseOver = true;
                        taskbarNotifier.TopMost = true;
                        //			taskbarNotifier.TitleClick+=new EventHandler(TitleClick);
                        //			taskbarNotifier.ContentClick+=new EventHandler(ContentClick);
                        //			taskbarNotifier.CloseClick+=new EventHandler(CloseClick);
                    }
                    catch (Exception ex)
                    {
                        EventLog.WriteEntry("SCTV", ex.Message);
                    }

                    //watch for inserted dvd/cd's
                    //deviceMonitor = new DeviceVolumeMonitor(this.Handle);
                    //deviceMonitor.OnVolumeInserted += new DeviceVolumeAction(VolumeInserted);
                    //deviceMonitor.OnVolumeRemoved += new DeviceVolumeAction(VolumeRemoved);

                    //watch for new/deleted files
                    //startWatchingFiles();

                    if (Mode == "Release")
                        this.Visible = false;

                    //MySplash.Hide();
                    GUIState = guiState.admin;
                    executeMacros("videoKey");
                }
                else //need to log that this is not activated, possibly send info to webservice
                {
                    MessageBox.Show("Invalid Activation.  Please contact customer service.");
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("SCTV", ex.Message);
            }
		}

        /// <summary>
        /// Check activation key or get a new one
        /// </summary>
        /// <returns></returns>
        private bool isActivated()
        {
#if DEBUG
            return true;
#endif

            bool activated = true;
            //string activationKey = System.Configuration.ConfigurationManager.AppSettings["ActivationKey"];
            //GetMachineInfo getMachineInfo = new GetMachineInfo();
            System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            //if (activationKey != null && activationKey.Length > 0)
            //{
            //    //make sure the activation key matches current cpuID, if not delete it
            //    if (Encryption.Decrypt(activationKey) == getMachineInfo.GetCPUId())
            //        activated = true;
            //    else
            //    {
            //        config.AppSettings.Settings["ActivationKey"].Value = "";
            //        config.Save(ConfigurationSaveMode.Modified);
            //        System.Configuration.ConfigurationManager.RefreshSection("appSettings"); 
            //        activated = false;
            //    }
            //}
            
            //if(!activated) //The "ActivationKey" doesn't exist or is no good, they need to activate through webservice
            //{
            //    //popup form to get user info and activate through webservice
            //    //ActivationForm activationForm = new ActivationForm();
            //    DialogResult activationResult = activationForm.ShowDialog();

            //    if (activationResult == DialogResult.OK)
            //    {
            //        activationKey = activationForm.ActivationKey;

            //        //create key if it doesn't exist
            //        if (System.Configuration.ConfigurationManager.AppSettings["ActivationKey"] == null)
            //            System.Configuration.ConfigurationManager.AppSettings.Add("ActivationKey", activationKey);
            //        else
            //        {
            //            config.AppSettings.Settings["ActivationKey"].Value = activationKey;
            //            config.Save(ConfigurationSaveMode.Modified);
            //        }

            //        System.Configuration.ConfigurationManager.RefreshSection("appSettings");

            //        activated = true;
            //    }
            //}

            return activated;
        }

        /// <summary>
        /// gets the computer nickname from xmlMain.xml
        /// </summary>
        /// <returns></returns>
        private string getComputerNickName()
        {
            XmlDocument xMain = new XmlDocument();
            xMain.Load(Application.StartupPath + "\\speech\\vocab\\xmlmain.xml");
            string computerNickName = xMain.SelectSingleNode("/GRAMMAR/RULE[@NAME=\"ComputerName\"]").InnerText;
            return computerNickName;
        }

		#region private void loadListings()
		private void loadListings()
		{
			//load listings from defaultURL
			TimeSpan LoadTime;
			try
			{
				LoadTime=ListingsOrganizer.LoadListings(ProgramConfiguration.DefaultUrl);
				populateTVGrid();
				populateListView();
				if(ListingsOrganizer.AllShowsWithinNDays.Length < ListingsOrganizer.AllChannels.Length*20)//they are getting low on shows - need to update listings file
				{
					updateTVListings();
				}
			}
			catch(TVException ex)
			{
				MessageBox.Show(ex.Message+"\n"+ex.InnerException.Message,"TV Guide Error");
				return;
			}
		}
		#endregion

		#region private DataGridTableStyle CreateDataGridStyle()
		private DataGridTableStyle CreateDataGridStyle() 
		{
			DataGridColumnStyle start;
			DataGridColumnStyle stop;
			DataGridColumnStyle Description;
			DataGridColumnStyle title;
			DataGridColumnStyle channel;
			DataGridColumnStyle Station;
			DataGridTableStyle DGStyle = new DataGridTableStyle();
			DGStyle.MappingName = "programme";
			Color backColor = new Color();

//			PropertyDescriptorCollection pcol = this.BindingContext[dsListings, "programme"].GetItemProperties();
//			Station = new ColumnStyle(pcol["programme"]);
//			title = new DataGridTextBoxColumn();

			backColor = Color.Wheat;
			channel = new ColumnStyle(backColor);
			channel.MappingName = "channel";
			channel.HeaderText = "Channel";
			channel.Width = 30;
			DGStyle.GridColumnStyles.Add(channel);

//			backColor = Color.Blue;
			Station = new ColumnStyle(backColor);
			Station.MappingName = "Station";
			Station.HeaderText = "Station";
			Station.Width = 40;
			DGStyle.GridColumnStyles.Add(Station);

			title = new ColumnStyle(backColor);
			title.MappingName = "title";
			title.HeaderText = "Name";
			title.Width = 200;
			DGStyle.GridColumnStyles.Add(title);

			start = new ColumnStyle(backColor);
			start.MappingName = "start";
			start.HeaderText = "Beginning";
			start.Width = 100;
			DGStyle.GridColumnStyles.Add(start);

			stop = new ColumnStyle(backColor);
			stop.MappingName = "stop";
			stop.HeaderText = "End";
			stop.Width = 100;
			DGStyle.GridColumnStyles.Add(stop);

			Description = new ColumnStyle(backColor);
			Description.MappingName = "desc";
			Description.HeaderText = "Description";
			Description.Width = 300;
			DGStyle.GridColumnStyles.Add(Description);
			return DGStyle;
		}
		#endregion

		#region Main Menu Click handlers
		#region private void menuItem9_Click(object sender, System.EventArgs e)//show the settings form
		private void menuItem9_Click(object sender, System.EventArgs e)//show the settings form
		{
			ProgramSettingsForm SettingsFrm=new ProgramSettingsForm();
			SettingsFrm.Show();
		}
		#endregion

		#region private void menuItem10_Click(object sender, System.EventArgs e) open file dialog
		private void menuItem10_Click(object sender, System.EventArgs e)
		{
			
			OpenFileDialog af = new OpenFileDialog();
			af.Title = "Open Media File...";
			//			af.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
			af.Filter = clipFileFilters;
			if( af.ShowDialog() != DialogResult.OK )
				return;

			//			menuFileCloseClip_Click( null, null );

//			clipFile = af.FileName;
//			PlayClip(clipFile);
			//			if( ! PlayClip() )
			//				menuFileCloseClip_Click( null, null );
		}
		#endregion

		#region private void mnuExit_Click(object sender, System.EventArgs e)
		private void mnuExit_Click(object sender, System.EventArgs e)
		{
//			if ( capture != null )
//				capture.Stop();
//			Application.Exit(); 
		}
		#endregion


		#region private void menuUpdateTVListings_Click(object sender, System.EventArgs e)
		private void menuUpdateTVListings_Click(object sender, System.EventArgs e)
		{
			updateTVListings();
		}
		#endregion
		

		#region Context Menu Events
		#region private void AddNotificationContextMenu_Click(object sender, System.EventArgs e)
		private void AddNotificationContextMenu_Click(object sender, System.EventArgs e)
		{
			//How a tv show is added to the notifications varies depending on which view the user is using, as extracting data from the list is different than from the grid
			//			if (DisplayTabs.SelectedTab==ListTab)
			//			{
			//				ShowNotificationArrayList list=new ShowNotificationArrayList();
			//
			//				foreach(ListViewItem item in listView.SelectedItems)
			//				{
			//					ShowNotification sn=new ShowNotification((TVProgramme)item.Tag);
			//					list.Add(sn);
			//				}
			//
			//				NotificationOrganizer.Add(list);
			//			}
			//			if (DisplayTabs.SelectedTab==GridTab)
			//			{
			//				//nothing yet
			//			}
			//			EventLog.WriteEntry("sctv","selected tab "+ tabControl.SelectedTab.Name.ToString());
			MenuItem currentMenu = (MenuItem)sender;

//			for(int x = 0;x<currentMenu.MenuItems.Count;x++)
//			{
//				if(currentMenu.MenuItems[x].s
//			}
			ShowNotificationArrayList list=new ShowNotificationArrayList();
			ShowNotification sn;
			switch(tabControl.SelectedTab.Name.ToString())
			{
				case "TVGridView":
//					EventLog.WriteEntry("sctv","found tab");
//					foreach(TVProgramme item in gridView.SelectedProgramme)
//					{
//						sn=new ShowNotification((TVProgramme)gridView.SelectedProgramme);
////						EventLog.WriteEntry("sctv","title of selecte program "+ gridView.SelectedProgramme.Title);
//						list.Add(sn);
//					}
					if(currentMenu.Text == "Record")
						RecordingOrganizer.Add(list);
					else
						NotificationOrganizer.Add(list);
					break;
				case "mediaTab"://they are selecting something from their library
					//					EventLog.WriteEntry("sctv","adding to showNofication");
					//					ShowNotificationArrayList list=new ShowNotificationArrayList();
					//					EventLog.WriteEntry("sctv","list length "+ list.Count.ToString());
					//					foreach(ListViewItem item in videoListView.SelectedItems)
					//					{
					//						EventLog.WriteEntry("sctv","adding");
					////						ShowNotification sn=new ShowNotification((DataRow)item.Tag);
					//						ShowNotification sn=new ShowNotification(createTVProgramme((DataRow)item.Tag));
					//						list.Add(sn);
					//					}
					//					EventLog.WriteEntry("sctv","adding to NotificationOrganizer");
					//					EventLog.WriteEntry("sctv","list length2 "+ list.Count.ToString());
					//					NotificationOrganizer.Add(list);
					break;
				case "tvListingsTab"://they selected something from the tvGrid
					if(tabControl1.SelectedTab.Name == "TVListView")
					{
						foreach(ListViewItem item in listView.SelectedItems)
						{
							sn=new ShowNotification((TVProgramme)item.Tag);
							list.Add(sn);
						}
					}
					else
					{
//						sn=new ShowNotification((TVProgramme)gridView.SelectedProgramme);
//						list.Add(sn);
					}
					if(currentMenu.Text == "Record")
						RecordingOrganizer.Add(list);
					else
						NotificationOrganizer.Add(list);
					break;
			}
			

			//			ShowNotificationArrayList list=new ShowNotificationArrayList();
			//			
			//			foreach(ListViewItem item in videoListView.SelectedItems)
			//			{
			//				ShowNotification sn=new ShowNotification((TVProgramme)item.Tag);
			//				list.Add(sn);
			//			}
			//
			//			NotificationOrganizer.Add(list);
		}
		#endregion

		#region private void menuAddToPlaySchedule_Click(object sender, System.EventArgs e)
		private void menuAddToPlaySchedule_Click(object sender, System.EventArgs e)
		{
			playScheduleArrayList list=new playScheduleArrayList();
			playSchedule ps;

			if(tabControl1.SelectedTab.Name == "TVListView")//listView
			{
				foreach(ListViewItem item in listView.SelectedItems)
				{
					ps=new playSchedule((TVProgramme)item.Tag);
					list.Add(ps);
				}
			}
			else//gridView
			{
//				ps=new playSchedule((TVProgramme)gridView.SelectedProgramme);
//				list.Add(ps);
			}
			pScheduleOrganizer.Add(list);
		}
		#endregion

		#region private TVProgramme createTVProgramme(DataRow dr)
		private TVProgramme createTVProgramme(DataRow dr)
		{
			TVProgramme newProgram = new TVProgramme();
			//			newProgram.Description = dr["description"].ToString();
			//			newProgram.Title = dr["name"].ToString();
			return newProgram;
		}
		#endregion

		#region Add To Favorites clicked
		private void menuItem45_Click(object sender, System.EventArgs e)
		{
			favoritesArrayList list=new favoritesArrayList();
			favorites sn;
			switch(tabControl1.SelectedTab.Name)
			{
				case "TVListView":
					sn=new favorites((TVProgramme)listView.SelectedItems[0].Tag);
					list.Add(sn);
					favoriteOrganizer.Add(list);
//					addToFavorites((TVProgramme)listView.SelectedItems[0].Tag);
					break;
				case "TVGridView":
//					sn=new favorites((TVProgramme)gridView.SelectedProgramme);
//					list.Add(sn);
//					favoriteOrganizer.Add(list);
					
//					addToFavorites(gridView.SelectedProgramme);
					break;
			}
		}
		#endregion

		#region private void menuItemPlaySelectedMedia(object sender, System.EventArgs e)
		private void menuItemPlaySelectedMedia(object sender, System.EventArgs e)
		{
			DataRowView drVideoInfo = (DataRowView)videoListView.SelectedItems[0].Tag;
//			clipFile = drVideoInfo["filePath"].ToString();
//			PlayClip(clipFile);
		}
		#endregion
		#endregion
		#endregion

		#region private DateTime createDateTime(string time)
		private DateTime createDateTime(string time)
		{
			string myString = time.Remove(14,6);//removes the zime zone
			string theDate = myString.Remove(0,4);
			string theYear = myString.Substring(0,4);
			string theMonth = myString.Substring(4,2);
			string theDay = myString.Substring(6,2);
			string hh = myString.Substring(8,2);
			string mm = myString.Substring(10,2);
			string ss = myString.Substring(12,2);
			DateTime theTime = Convert.ToDateTime(theDay +"/"+ theMonth +"/"+ theYear +" "+ hh +":"+ mm +":"+ ss);
			return theTime;
		}
		#endregion

		#region population functions
		#region private void populateMediaView()
		private void populateMediaView()
		{
			try
			{
				audioListView.Clear();
//				audioListView.InsertMediaFileRange(myMedia.changeMediaTypes(ClipType.audio.ToString()));
				audioListView.Sort(ProgramConfiguration.DefaultSortField,ProgramConfiguration.DefaultSortMode);
			}
			catch(Exception ex)
			{
				EventLog.WriteEntry("sctv",ex.ToString());
			}

			try
			{
				videoListView.Clear();
//				dvMedia = myMedia.getCategory("All");
				dvMedia.Sort = "title";
				videoListView.InsertMediaFileRange(dvMedia);
				videoListView.Sort(TVComparer.ESortBy.Title,ProgramConfiguration.DefaultSortMode);
			}
			catch(Exception ex)
			{
				EventLog.WriteEntry("sctv",ex.ToString());
			}
		}
		#endregion

		#region private void populateListView()
		private void populateListView()
		{
			listView.Clear();
			listView.InsertTVProgrammeRange(ListingsOrganizer.ShowsRightNow);
//			listView.Sort(ProgramConfiguration.DefaultSortField,ProgramConfiguration.DefaultSortMode);
			listView.Sort(TVComparer.ESortBy.Channel,ProgramConfiguration.DefaultSortMode);
			listView.InsertTVProgrammeRange(ListingsOrganizer.AllShowsWithinNHours);			
		}
		#endregion

		#region private void populateTVGrid()//this populates tvgrid with all channels
		private void populateTVGrid()//this populates tvgrid with all channels
		{
//			gridView.Visible = false;

//			TVProgramme[] myShows = ListingsOrganizer.MoviesToday;
//			ArrayList myChannels=new ArrayList();
//
//			foreach(TVProgramme Prog in myShows)
//			{
////				if (IsProgrammeInRange_Days(Prog,nDays))
//					myChannels.Add(Prog.Channel);
//			}
			
//			gridView.PopulateGrid((TVChannel[])myChannels.ToArray(typeof(TVChannel)),ListingsOrganizer.AllShowsWithinNDays);
//			gridView.PopulateGrid(ListingsOrganizer.AllChannels,ListingsOrganizer.AllShowsWithinNDays,favorites,ProgramConfiguration.FavoritesColor);

//			playGridView.PopulateGrid(pScheduleOrganizer.GetList);

//			gridView.Visible = true;			
		}
		#endregion

		#region public void populatePlayGrid()
		public void populatePlayGrid()
		{
//			playGridView.Visible = false;
//			playGridView.PopulateGrid(pScheduleOrganizer.GetList);
//			playGridView.Visible = true;
		}
		#endregion
		
		#region private void populateMacros()  //these run immediatly when found
		private void populateMacros()
		{
			//comma delimited list of (string) KeyValue's , names

            //volume controls
            mainMenuMacroList.Add("107", "volume up");//plus key on keypad
            mainMenuMacroList.Add("109", "volume down");//minus key on keypad
            mainMenuMacroList.Add("77", "mute");//"m" button

			//media controls
//			mediaMacroList.Add("17,16,66","Rewind");  //rewind
//			mediaMacroList.Add("17,83","Stop");  //Stop
//			mediaMacroList.Add("17,16,70","FF");  //FF
//			mediaMacroList.Add("17,70","Next");  //Next
//			mediaMacroList.Add("17,66","Previous");  //Previous
			mainMenuMacroList.Add("17,80","Play/Pause");  //Play/Pause
//			mediaMacroList.Add("80","Play/Pause");  //P - Play/Pause
			//			macroList.Add(17,16,66);  //Record
			//			macroList.Add("","Mute");  //mute

			//main menu items
			mainMenuMacroList.Add("17,16,18,36","Option");  //home key
			mainMenuMacroList.Add("17,16,18,50","tvKey");  //tv key
			mainMenuMacroList.Add("17,16,18,72","fmKey");  //fm key
			mainMenuMacroList.Add("17,16,18,51","musicKey");  //music key
			mainMenuMacroList.Add("17,16,18,52","pictureKey");  //picture key
			mainMenuMacroList.Add("17,16,18,53","videoKey");  //video key
			mainMenuMacroList.Add("17,16,18,49","dvdKey");  //dvd key

			//generic Key settings for main menu
			mainMenuMacroList.Add("113","tvKey");  //F2 - tv key
			mainMenuMacroList.Add("114","fmKey");  //F3 - fm key
			mainMenuMacroList.Add("116","musicKey");  //F5 - music key
			mainMenuMacroList.Add("117","pictureKey");  //F6 - picture key
			mainMenuMacroList.Add("118","videoKey");  //F7 - video key
			mainMenuMacroList.Add("119","dvdKey");  //F8 - dvd key


			macroList.Add("16,18,70","shuffleKey");  //shuffle key
			macroList.Add("16,18,66","repeatKey");  //repeat key
		}
		#endregion

		#region private void populateKeyStrokes()
		private void populateKeyStrokes()
		{
			//TV channel controls
//			keyList.Add("49","1");  //1
//			keyList.Add("50","2");  //2
//			keyList.Add("51","3");  //3
//			keyList.Add("52","4");  //4
//			keyList.Add("53","5");  //5
//			keyList.Add("54","6");  //6
//			keyList.Add("55","7");  //7
//			keyList.Add("56","8");  //8
//			keyList.Add("57","9");  //9
//			keyList.Add("48","0");  //0
		}
		#endregion
		#endregion

		#region "Show Notification Control Events"
		private void ShowNotificationListView_SelectedIndexChanged(object sender, System.EventArgs e)
		{
//			if (ShowNotificationListView.SelectedItems.Count>0)
//			{
//				ShowNotificationPropertyGrid.SelectedObject=ShowNotificationListView.SelectedItems[0].Tag;
//			}

			if (ShowNotificationListView.SelectedItems.Count>0)
				EventLog.WriteEntry("sctv","tag type = "+ ShowNotificationListView.SelectedItems[0].Tag.GetType().ToString());
//				richTextBoxSchedules.AddProgramme((TVProgramme)ShowNotificationListView.SelectedItems[0].Tag);
		}

		private void ShowNotificationContextMenuRemoveShow_Click(object sender, System.EventArgs e)
		{
			ShowNotificationPropertyGrid.SelectedObject = null;
			
			ShowNotificationArrayList list=new ShowNotificationArrayList();

			foreach(ListViewItem item in ShowNotificationListView.SelectedItems)
			{
				list.Add((ShowNotification)item.Tag);
			}

			NotificationOrganizer.Remove(list);
		}

		private void ShowNotificationTimer_Tick(object sender, System.EventArgs e)
		{
			foreach(ShowNotification show in NotificationOrganizer.GetList)
			{
				DateTime Now=System.DateTime.Now;
//				EventLog.WriteEntry("sctv","show starting: "+ show.IsStarting().ToString());
				if (show.IsStarting() && show.AlreadyNotified==false)
				{
					show.AlreadyNotified=true;
					//notifyIcon.ShowBalloon(HansBlomme.Windows.Forms.NotifyIcon.EBalloonIcon.Info,show.name+" on channel "+show.channel,"Show Starting",350);
//					EventLog.WriteEntry("sctv","popup notification balloon");
//					showNotificationWindow NotifyFrm=new showNotificationWindow();
//					NotifyFrm.Show();
//					taskbarNotifier.CloseClickable=true;
//					taskbarNotifier.TitleClickable=true;
//					taskbarNotifier.ContentClickable=true;
//					taskbarNotifier.EnableSelectionRectangle=true;
//					taskbarNotifier.KeepVisibleOnMousOver=true;	// Added Rev 002
//					taskbarNotifier.ReShowOnMouseOver=true;			// Added Rev 002
//					taskbarNotifier.Show("Testing","This is my content",500,500,5000);
					taskbarNotifier.Show(show);
					if (ProgramConfiguration.PlaySound==true && File.Exists(ProgramConfiguration.NotificationSound))
						PlayWaveFile(ProgramConfiguration.NotificationSound);
				}
			}
		}

		private void ShowNotificationPropertyGrid_PropertyValueChanged(object s, System.Windows.Forms.PropertyValueChangedEventArgs e)
		{
			NotificationOrganizer.MarkConflicts();		
		}
		#endregion

		#region private void favoriteNotificationTimer_Tick(object sender, System.EventArgs e)
		private void favoriteNotificationTimer_Tick(object sender, System.EventArgs e)
		{
			foreach(favorites favorite in favoriteOrganizer.GetList)
			{
				DateTime Now=System.DateTime.Now;
				TVProgramme[] showTimes = ListingsOrganizer.SearchByTitle(favorite.title);
				foreach(TVProgramme prog in showTimes)
				{
					if (favorite.IsStarting(prog) && favorite.AlreadyNotified==false)
					{
						favorite.AlreadyNotified=true;
						taskbarNotifier.Show(prog);
						if (ProgramConfiguration.PlaySound==true && File.Exists(ProgramConfiguration.NotificationSound))
							PlayWaveFile(ProgramConfiguration.NotificationSound);
						break;//TODO: might take this out if there are multiple showings of the same program on different channels at the same time
					}
				}
			}
		}
		#endregion

		#region "GridView Events"
//		private void GridView_OnSelectionChanged(object o, TVGridViewEventArgs e)
//		{
//			MiscStuffRichText.AddProgramme(e.Show);
//		}
		#endregion

		#region media library functions
		#region private void btnAddFileTypes_Click(object sender, System.EventArgs e)
		private void btnAddFileTypes_Click(object sender, System.EventArgs e)
		{
			if(txtAddType.Text.Length > 0) //there is a new file type to add
			{
				if(tcFileTypes.SelectedTab.Name == "tpAudio")
				{
					if(!lBoxAudioTypes.Items.Contains(txtAddType.Text.ToString()))
					{
						lBoxAudioTypes.Items.Add(txtAddType.Text);
						XmlNodeList typesNodes = xmlFileTypes.GetElementsByTagName("audio");
						if(typesNodes.Count > 0)//there were audio types
						{
							XmlNode newNode = xmlFileTypes.CreateNode(XmlNodeType.Element, "type", "");
							newNode.InnerText = txtAddType.Text.ToString().Trim();
							typesNodes[0].AppendChild(newNode);
							xmlFileTypes.Save("config/fileTypes.xml");
						}
					}
				}
				else
				{
					if(!lBoxVideoTypes.Items.Contains(txtAddType.Text.ToString()))
					{
						lBoxVideoTypes.Items.Add(txtAddType.Text);
						XmlNodeList typesNodes = xmlFileTypes.GetElementsByTagName("video");
						if(typesNodes.Count > 0)//there were video types
						{
							XmlNode newNode = xmlFileTypes.CreateNode(XmlNodeType.Element, "type", "");
							newNode.InnerText = txtAddType.Text.ToString().Trim();
							typesNodes[0].AppendChild(newNode);
							xmlFileTypes.Save("config/fileTypes.xml");
						}
					}
					else
						EventLog.WriteEntry("sctv","didn't find");
				}
				txtAddType.Text = "";
				txtAddType.Focus();
			}
		}
		#endregion

		#region private void btnAudio_Click(object sender, System.EventArgs e)
		private void btnAudio_Click(object sender, System.EventArgs e)
		{
//			dgMediaLibrary.DataSource = changeMediaTypes("audio").Tables[0];
//			clipType = ClipType.audio;
		}
		#endregion

		#region private void btnVideo_Click(object sender, System.EventArgs e)
		private void btnVideo_Click(object sender, System.EventArgs e)
		{
//			dgMediaLibrary.DataSource = changeMediaTypes("video").Tables[0];
//			clipType = ClipType.video;
		}
		#endregion

		#region private void cmbVideoCategories_SelectedIndexChanged(object sender, System.EventArgs e)
		private void cmbVideoCategories_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			ComboBox categories = (ComboBox)sender;

			if(categories.SelectedItem.ToString() == "All")
			{
				videoListView.Clear();
//				videoListView.InsertMediaFileRange(myMedia.getCategory("All"));
				videoListView.Sort(ProgramConfiguration.DefaultSortField,ProgramConfiguration.DefaultSortMode);
			}
			else
			{			
				videoListView.Clear();
//				videoListView.InsertMediaFileRange(myMedia.getCategory(categories.SelectedItem.ToString()));
				videoListView.Sort(ProgramConfiguration.DefaultSortField,ProgramConfiguration.DefaultSortMode);
			}
		}
		#endregion

		#region private void updateCategories(DataView dv) updates the category dropdown box with the categories of the currently chosen media type
		private void updateCategories(DataView dv)
		{
			cmbVideoCategories.Items.Clear();
			cmbVideoCategories.Items.Add("All");
//			EventLog.WriteEntry("sctv","updating categories "+ dv.Count.ToString());
			foreach(DataRowView drv in dv)
			{
//				EventLog.WriteEntry("sctv","adding "+ drv["category"].ToString());
				if(drv["category"].ToString().Length > 0 && (cmbVideoCategories.FindString(drv["category"].ToString()) < 1))
					cmbVideoCategories.Items.Add(drv["category"].ToString());
			}
		}
		#endregion
		
		#region file watching functions
		#region private void startWatchingFiles()//starts a file system watcher for each entry in locations
		private void startWatchingFiles()//starts a file system watcher for each entry in locations
		{
			try
			{
				foreach(XmlElement location in xmlLocations.GetElementsByTagName("location"))
				{
					if(location.InnerText.ToString().Length > 0)
						_InitFileSystemWatcher(location.InnerText.ToString());
				}
			}
			catch(Exception ex)
			{
				EventLog.WriteEntry("sctv",ex.ToString());
			}
		}
		#endregion

		#region private void _InitFileSystemWatcher()
		private void _InitFileSystemWatcher(string mediaPath)
		{
			try
			{
                FileInfo fi = new FileInfo(mediaPath);
                if (fi.Exists)
                {
                    // Create a new file system watcher object
                    _objFSW = new FileSystemWatcher();

                    // Set the path to be watched
                    _objFSW.Path = mediaPath;

                    // Set the modification filters
                    //			_objFSW.NotifyFilter = NotifyFilters.LastAccess | 
                    //				NotifyFilters.LastWrite | 
                    //				NotifyFilters.DirectoryName | NotifyFilters.FileName;
                    _objFSW.NotifyFilter = NotifyFilters.DirectoryName | NotifyFilters.FileName;

                    // Set the filter mask (i.e. watch all files)
                    _objFSW.Filter = "*.*";

                    // We want to watch subdirectories as well
                    _objFSW.IncludeSubdirectories = true;

                    // Setup the event handlers
                    _objFSW.Changed += new FileSystemEventHandler(FSW_OnChanged);
                    _objFSW.Created += new FileSystemEventHandler(FSW_OnChanged);
                    _objFSW.Deleted += new FileSystemEventHandler(FSW_OnChanged);
                    _objFSW.Renamed += new RenamedEventHandler(FSW_OnRenamed);

                    // Let's start watching
                    _objFSW.EnableRaisingEvents = true;
                }
			}
			catch(Exception ex)
			{
				EventLog.WriteEntry("sctv",ex.ToString());
			}
		}
		#endregion

		#region private void FSW_OnChanged(object source, FileSystemEventArgs e)

		// This event handler is called when a file/folder has been modified.
		private void FSW_OnChanged(object source, FileSystemEventArgs e)
		{
			changeDelete(e);
		}
		#endregion

		#region private void FSW_OnRenamed(object source, RenamedEventArgs e)
		
		// This event handler is called when a file/folder has been renamed.
		private void FSW_OnRenamed(object source, RenamedEventArgs e)
		{
			updateMedia(e);
		}
		#endregion

		#region private void changeDelete()
		private void changeDelete(FileSystemEventArgs e)//when you need to look for new media in a folder or delete media
		{
			string strTemp = e.FullPath.Substring(0,e.FullPath.LastIndexOf("\\"));
//			EventLog.WriteEntry("sctv","e.FullPath "+ @Regex.Replace(e.FullPath,e.Name,""));
//			EventLog.WriteEntry("sctv","strTemp "+ strTemp);
//			EventLog.WriteEntry("sctv","e.FullPath "+ e.FullPath);
			FindFiles(strTemp,false,null);
		}
		#endregion

		#region private void updateMedia()
		private void updateMedia(RenamedEventArgs e)//when someone renames existing media files outside of this program
		{
//			lblMessage.Text = e.FullPath.ToString();
		}
		#endregion
		#endregion
		
		#region private void updateCLBoxes() //with media content
		private void updateCLBoxes(ListBox currentLB,XmlNodeList nodeList)
		{
			foreach(XmlNode node in nodeList.Item(0))
			{
				if(node.InnerText.ToString().Length > 0)
					currentLB.Items.Add(node.InnerText.ToString());
			}
		}
		#endregion

		#region private void btnSearch_Click(object sender, System.EventArgs e)
		private void btnSearch_Click(object sender, System.EventArgs e)
		{
//			if(txtSearch.Text.Length > 0)//there is a search term
//			{
//				DataTable dt = (DataTable)dgMediaLibrary.DataSource;
//				dt.DefaultView.RowFilter = cmbSearchColumns.SelectedItem.ToString() +" LIKE '%"+ txtSearch.Text +"%'";
//			}
		}
		#endregion

		#region private void updateSearchColumns()
		private void updateSearchColumns()
		{
//			cmbSearchColumns.Items.Clear();
//			DataTable dt = (DataTable)dgMediaLibrary.DataSource;
//			foreach(DataColumn dc in dt.Columns)
//			{
//				cmbSearchColumns.Items.Add(dc.ColumnName.ToString());
//			}
		}
		#endregion

		#region private void cmbAudioCategories_SelectionChangeCommitted(object sender, System.EventArgs e)
		private void cmbAudioCategories_SelectionChangeCommitted(object sender, System.EventArgs e)
		{
			if(cmbAudioCategories.SelectedItem.ToString() == "All")
			{
//				myMedia.changeMediaTypes("audio");
			}
			else
			{			
				//				dsMedia.Tables[0].DefaultView.RowFilter = "category = '"+ cmbAudioCategories.SelectedItem.ToString() +"'";
			}
			//			dgMediaLibrary.Refresh();
		}
		#endregion

		#region private void btnBrowseLocations_Click(object sender, System.EventArgs e)
		private void btnBrowseLocations_Click(object sender, System.EventArgs e)
		{
			if(OFolder.ShowDialog() == DialogResult.OK)
			{
				if(!lBoxVideoLocations.Items.Contains(OFolder.SelectedPath.ToString().Trim()))
				{
					lBoxVideoLocations.Items.Add(OFolder.SelectedPath.ToString());
					XmlNodeList folderNodes = xmlLocations.GetElementsByTagName("locations");
					XmlNode newNode = xmlLocations.CreateNode(XmlNodeType.Element, "location", "");
					newNode.InnerText = OFolder.SelectedPath.ToString().Trim();
//					XmlNode subNode = xmlLocations.CreateNode(XmlNodeType.Element,"subFolders","");
//					subNode.InnerText=subFoldersCheckBox.Checked.ToString();
//					newNode.AppendChild(subNode);
//					newNode.Value = ;
					folderNodes[0].AppendChild(newNode);
					xmlLocations.Save("config/locations.xml");
				}
			}
		}
		#endregion

		#region private void btnDeleteMediaLocation_Click(object sender, System.EventArgs e)
		private void btnDeleteMediaLocation_Click(object sender, System.EventArgs e)
		{
			if(lBoxVideoLocations.SelectedItems.Count > 0)
				lBoxVideoLocations.Items.RemoveAt(lBoxVideoLocations.SelectedIndex);
		}
		#endregion

		/// <summary>
		/// finds new files in the folders specified in config/locations.xml and ads their information to config/media.xml
		/// </summary>
		private void lookForMedia()
		{
			foreach(XmlNode location in xmlLocations["locations"])
			{
                if (location.InnerText.ToString().Length > 0)
                {
                    string dLocation = location.InnerText.ToString().Trim();
                    FindFiles(dLocation, subFoldersCheckBox.Checked, null);
                }
                else
                    EventLog.WriteEntry("lookformedia", "no location");
			}
		}

        public void FindValidFiles(string location)
        {
            try
            {
                DirectoryInfo fl = new DirectoryInfo(location);

                foreach (FileSystemInfo entry in fl.GetFileSystemInfos())
                {
                    if (entry.Extension.Length > 0)//this is a file
                    {
                        //check extension and make sure it's a type we recognize
                        if (compareExtension(entry.Extension))
                            foundMediaFiles.Add(entry.FullName);
                    }
                    else //if (checkSubFolders) //this is a directory and they want to check subfolders
                    {
                        FindValidFiles(location + "\\" + entry.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("FindValidFiles", ex.Message);
            }
            
        }

        /// <summary>
        /// Look for files and update speech files to match
        /// </summary>
        /// <param name="location"></param>
        /// <param name="checkSubFolders"></param>
        /// <param name="tempDoc"></param>
		public void FindFiles(string location,bool checkSubFolders,XmlDocument tempDoc)
		{
			try
			{
				bool foundNewFiles = false;
                string fileName = "";
                string category = "";
                string formattedFileName = "";
				DirectoryInfo fl = new DirectoryInfo(location);
				
                if(tempDoc == null)
				{
					tempDoc = new XmlDocument();
					tempDoc.Load("config/media.xml");
				}

                XmlDocument xMediaSpeech = new XmlDocument();
                xMediaSpeech.Load(Application.StartupPath + "\\speech\\vocab\\xmlAll.xml");
                //FileInfo fiMedia = new FileInfo(Application.StartupPath + "\\speech\\vocab\\xmlMedia.xml");

                //if (fiMedia.Exists && xMediaSpeech != null)
                //    xMediaSpeech.Load(Application.StartupPath + "\\speech\\vocab\\xmlMedia.xml");
                //else
                //    xMediaSpeech.LoadXml(generateMediaSpeechFile().InnerXml);

				DataSet ds = new DataSet();
				ds.ReadXml("config/media.xml");

                if (ds.Tables.Count > 0)//there is media
                {
                    DataColumn[] pk = new DataColumn[1];
                    pk[0] = ds.Tables[0].Columns["filePath"];
                    ds.Tables[0].PrimaryKey = pk;
                }
						
				foreach (FileSystemInfo entry in fl.GetFileSystemInfos())
				{
					if(entry.Extension.Length > 0)//this is a file
					{
						//check extension and make sure it's a type we recognize
						if(compareExtension(entry.Extension))
						{
                            //add to media file
                            if (ds.Tables.Count == 0 || (ds.Tables[0].Rows.Find(location + "\\" + entry.Name.ToString().Trim()) == null && ds.Tables[0].Rows.Find(location + "\\" + entry.Name.ToString().Trim().ToUpper()) == null))//this file name doesn't exist in the media file
							{
                                fileName = System.Text.RegularExpressions.Regex.Replace(entry.Name,entry.Extension,"");
                                formattedFileName = formatNameString(fileName);
								foundNewFiles = true;

								XmlNode newNode = tempDoc.CreateNode(XmlNodeType.Element,"media","");

								XmlNode nameNode = tempDoc.CreateNode(XmlNodeType.Element,"title","");
                                nameNode.InnerText = formattedFileName;
								XmlNode performersNode = tempDoc.CreateNode(XmlNodeType.Element,"performers","");
								XmlNode ratingNode = tempDoc.CreateNode(XmlNodeType.Element,"rating","");
								XmlNode descriptionNode = tempDoc.CreateNode(XmlNodeType.Element,"description","");
								XmlNode starsNode = tempDoc.CreateNode(XmlNodeType.Element,"stars","");
								XmlNode categoryNode = tempDoc.CreateNode(XmlNodeType.Element,"category","");
                                category = formatNameString(getCategory(location));
                                categoryNode.InnerText = category;
								XmlNode timesPlayedNode = tempDoc.CreateNode(XmlNodeType.Element,"timesPlayed","");
								XmlNode filePathNode = tempDoc.CreateNode(XmlNodeType.Element,"filePath","");
								filePathNode.InnerText = location +"\\"+ entry.Name;
                                XmlNode grammarNode = tempDoc.CreateNode(XmlNodeType.Element, "grammar", "");
                                grammarNode.InnerText = formattedFileName;
					
								newNode.AppendChild(nameNode);
								newNode.AppendChild(performersNode);
								newNode.AppendChild(ratingNode);
								newNode.AppendChild(descriptionNode);
								newNode.AppendChild(starsNode);
								newNode.AppendChild(categoryNode);
								newNode.AppendChild(timesPlayedNode);
								newNode.AppendChild(filePathNode);
                                newNode.AppendChild(grammarNode);
								tempDoc["mediaFiles"].AppendChild(newNode);
                            }

                            if (formattedFileName.Trim().Length > 0 || category.Trim().Length > 0)
                            {
                                //add to media speech file
                                insertMediaXml(formattedFileName, ref xMediaSpeech);
                                insertMediaXml(category, ref xMediaSpeech);
                            }
						}
					}
					else if(checkSubFolders) //this is a directory and they want to check subfolders
					{
						FindFiles(location +"\\"+ entry.Name,true,tempDoc);
					}
				}

				if(foundNewFiles) //only save the media file if there was new files added
				{
                    if (ds.Tables.Count > 0)//there is media
                    {
                        DataColumn[] pk = new DataColumn[1];
                        pk[0] = ds.Tables[0].Columns["filePath"];
                        ds.Tables[0].PrimaryKey = pk;
                    }

					//EventLog.WriteEntry("sctv","++++++++++++ found files");
					tempDoc.Save("config/media.xml");
                    xMediaSpeech.Save("speech/vocab/xmlAll.xml");
					myMedia = null; //remove and recreate myMedia with the new files
					myMedia = new mediaHandler();
				}
				//else
					//EventLog.WriteEntry("sctv","-----------  didn't find any files");
			}
			catch(Exception e)
			{
				EventLog.WriteEntry("sctv",e.ToString());
			}
		}
		#endregion

		private string getCategory(string filePath)
		{
			string catName = "";
			catName = filePath.Substring(filePath.LastIndexOf("\\")+1);
//			foreach(XmlElement location in xmlLocations.GetElementsByTagName("location"))
//			{
//				if(location.InnerText.ToString().Length > 0)
//				{
////					catName = @System.Text.RegularExpressions.Regex.Replace(@filePath,@location.InnerText.ToString() +"\\","");
//					catName = filePath.Substring(filePath.LastIndexOf("\\")+1);
//					EventLog.WriteEntry("sctv","found catName "+ catName);
//				}
//				else
//				{
//					EventLog.WriteEntry("sctv","!!!!!!!!!!!!!!!!   didn't find "+ location.InnerText.ToString() +" in "+ filePath);
//					EventLog.WriteEntry("sctv","indexOf "+ filePath.ToLower().IndexOf(location.InnerText.ToLower().ToString()).ToString());
//				}
//					
//				//				System.Text.RegularExpressions.Regex.Replace(filePath,"Z:\\\\Video\\\\","");
//			}
			return catName;
		}

		private bool compareExtension(string extension)
		{
			
			foreach(XmlNode node in xmlFileTypes.GetElementsByTagName("video").Item(0))
			{
				if("."+ node.InnerText.ToLower() == extension.ToLower())//this extension is in the list of file types
				{
					return true;
				}
			}
			return false;
		}

        /// <summary>
        /// Look for new media
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void btnLookForMedia_Click(object sender, System.EventArgs e)
		{
			if(xmlLocations.GetElementsByTagName("locations").Count > 0)
				lookForMedia();
		}

        /// <summary>
        /// clear current media file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClearMedia_Click(object sender, EventArgs e)
        {
            try
            {
                XmlDocument xmlMedia = new XmlDocument();
                xmlMedia.Load("config/media.xml");
                xmlMedia.SelectSingleNode("/mediaFiles").RemoveAll();
                xmlMedia.Save("config/media.xml");
            }
            catch (Exception ex)
            {
                
                throw;
            }
            
        }

        /// <summary>
        /// generate the media xml file for speech
        /// </summary>
        private XmlDocument generateMediaSpeechFile()
        {
            XmlDocument xMedia = new XmlDocument();

            XmlComment xComment = xMedia.CreateComment("409 = american english");
            xMedia.AppendChild(xComment);
            XmlNode grammarNode = xMedia.CreateNode(XmlNodeType.Element, "GRAMMAR", "");

            XmlAttribute grammarAttribute = xMedia.CreateAttribute("LANGID");
            grammarAttribute.Value = "409";
            grammarNode.Attributes.Append(grammarAttribute);

            XmlNode defineNode = xMedia.CreateNode(XmlNodeType.Element, "DEFINE", "");
            
            XmlNode idNode = xMedia.CreateNode(XmlNodeType.Element, "ID", "");
            XmlAttribute idNameAttribute = xMedia.CreateAttribute("Name");
            idNameAttribute.Value = "media";
            XmlAttribute idValAttribute = xMedia.CreateAttribute("VAL");
            idValAttribute.Value = "1";
            idNode.Attributes.Append(idNameAttribute);
            idNode.Attributes.Append(idValAttribute);
            
            defineNode.AppendChild(idNode);

            XmlNode ruleNode = xMedia.CreateNode(XmlNodeType.Element, "RULE", "");
            XmlAttribute ruleNameAttribute = xMedia.CreateAttribute("NAME");
            ruleNameAttribute.Value = "media";
            XmlAttribute ruleIDAttribute = xMedia.CreateAttribute("ID");
            ruleIDAttribute.Value = "media";
            XmlAttribute ruleTopLevelAttribute = xMedia.CreateAttribute("TOPLEVEL");
            ruleTopLevelAttribute.Value = "ACTIVE";

            ruleNode.Attributes.Append(ruleNameAttribute);
            ruleNode.Attributes.Append(ruleIDAttribute);
            ruleNode.Attributes.Append(ruleTopLevelAttribute);

            XmlNode ruleRefNode = xMedia.CreateNode(XmlNodeType.Element, "RULEREF", "");
            XmlAttribute ruleRefNameAttribute = xMedia.CreateAttribute("NAME");
            ruleRefNameAttribute.Value = "ComputerName";
            XmlAttribute ruleRefUrl = xMedia.CreateAttribute("URL");
            ruleRefUrl.Value = "xmlMain.xml";

            ruleRefNode.Attributes.Append(ruleRefNameAttribute);
            ruleRefNode.Attributes.Append(ruleRefUrl);

            ruleNode.AppendChild(ruleRefNode);

            grammarNode.AppendChild(defineNode);
            grammarNode.AppendChild(ruleNode);
            xMedia.AppendChild(grammarNode);

            xMedia.Save(Application.StartupPath + "\\speech\\vocab\\XMLMedia.xml");

            return xMedia;
        }

        /// <summary>
        /// Insert media name into xml file for speech recognition
        /// </summary>
        /// <param name="mediaName"></param>
        private void insertMediaXml(string mediaName,ref XmlDocument xMediaSpeech)
        {
            if (mediaName.Trim().Length > 0)
            {
                bool isDuplicate = false;

                //get media node
                XmlNode mediaNode = xMediaSpeech.SelectSingleNode("/GRAMMAR/RULE[@NAME=\"media\"]");
                XmlNode mediaNodeL = xMediaSpeech.SelectSingleNode("/GRAMMAR/RULE[@NAME=\"media\"]/L");

                if (mediaNodeL == null)
                {
                    mediaNodeL = xMediaSpeech.CreateNode(XmlNodeType.Element, "L", "");

                    //append to media node
                    mediaNode.AppendChild(mediaNodeL);
                }

                foreach (XmlNode media in mediaNodeL.ChildNodes)
                {
                    if (media.InnerText == mediaName)
                    {
                        isDuplicate = true;
                        break;
                    }
                }

                if (!isDuplicate)
                {
                    //create new node to add
                    XmlNode newMediaNode = xMediaSpeech.CreateNode(XmlNodeType.Element, "P", "");
                    newMediaNode.InnerText = mediaName;

                    //append to media node
                    mediaNodeL.AppendChild(newMediaNode);
                }
            }
            else
            {
                //MessageBox.Show("empty media name");
            }
        }

        /// <summary>
        /// Find the places in the string that should have spaces
        /// </summary>
        /// <param name="stringToSpace"></param>
        /// <returns></returns>
        private string formatNameString(string stringToSpace)
        {
            try
            {
                if (stringToSpace != null && stringToSpace.Length > 0)
                {
                    int offSet = 0;
                    int convertedChar = 0;
                    ArrayList foundSpaces = new ArrayList();

                    //replace underscores with space
                    stringToSpace = stringToSpace.Replace("_", " ");

                    //find change from lower case to upper case
                    for (int x = 0; x < stringToSpace.Length; x++)
                    {
                        convertedChar = Convert.ToInt32(stringToSpace[x]);

                        if ((convertedChar >= 65 && convertedChar <= 90) && x > 1)
                        {
                            foundSpaces.Add(x);
                        }
                    }

                    for (int x = 0; x < foundSpaces.Count; x++)
                    {
                        //don't add space if the next letter is capitalized
                        if ((x == foundSpaces.Count-1) || ((int)foundSpaces[x] != (((int)foundSpaces[x + 1]) - 1)))
                        {
                            stringToSpace = stringToSpace.Insert(int.Parse(foundSpaces[x].ToString()) + offSet, " ");
                            offSet++;
                        }
                    }

                    //remove any special characters
                    stringToSpace = Regex.Replace(stringToSpace, @"[^a-zA-Z0-9\s]", "");

                    //capitalize first letter
                    stringToSpace = stringToSpace.Substring(0, 1).ToUpper() + stringToSpace.Substring(1);

                    //remove double spaces
                    string prevString = "";
                    while (stringToSpace != prevString)
                    {
                        prevString = stringToSpace; 
                        stringToSpace = stringToSpace.Replace("  ", " ").Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                
                throw;
            }
            

            return stringToSpace;

                //if ((convertedChar >= 65) && (convertedChar <= 90))
                //{
                //    uLetters++;
                //}

                //if ((convertedPw >= 97) && (convertedPw <= 122))
                //{
                //    lLetters++;
                //}

                //if ((convertedPw >= 48) && (convertedPw <= 57))
                //{
                //    numbers++;
                //}

                //if ((convertedPw >= 33) && (convertedPw <= 47))
                //{
                //    symbols++;
                //}

                //if ((convertedPw >= 58) && (convertedPw <= 64))
                //{
                //    symbols++;
                //}

                //if ((convertedPw >= 91) && (convertedPw <= 96))
                //{
                //    symbols++;
                //}

                //if ((convertedPw >= 123) && (convertedPw <= 126))
                //{
                //    symbols++;
                //}
            //}
            // #### >END< PROCESSING EACH CHARACTER ####

            //lblDetails.Text += "Upper Case Letters: " + uLetters.ToString() + "\n";
            //lblDetails.Text += "Lower Case Letters: " + lLetters.ToString() + "\n";
            //lblDetails.Text += "Numbers: " + numbers.ToString() + "\n";
            //lblDetails.Text += "Symbols: " + symbols.ToString() + "\n";
        }

        /// <summary>
        /// generates a new empty media xml file
        /// </summary>
        private void generateMediaXMLFile()
        {
            XmlDocument xMedia = new XmlDocument();

            XmlNode mediaFilesNode = xMedia.CreateNode(XmlNodeType.Element, "mediaFiles", "");

            xMedia.AppendChild(mediaFilesNode);

            xMedia.Save(Application.StartupPath + "/config/media.xml");
        }

		#region Sound Functions
		#region static extern bool PlaySound
		[DllImport("winmm.dll", SetLastError=true,CallingConvention=CallingConvention.Winapi)]
		static extern bool PlaySound(string pszSound, IntPtr hMod, SoundFlags sf);
		#endregion

		#region private void PlayWaveFile(string filename)
		private void PlayWaveFile(string filename)
		{
			int err=0;

			try
			{
				// play the sound from the selected filename
				if (!PlaySound(filename, IntPtr.Zero, SoundFlags.SND_FILENAME | SoundFlags.SND_ASYNC))
				{
					Exception ex=new Exception("Error playing selected wav file");
					ExceptionManager.Publish(ex);
				}
			}
			catch
			{
				err = Marshal.GetLastWin32Error();

				if (err!=0)
				{
					Exception ex=new Exception("Exception thrown trying to play selected wav file with error code "+err.ToString());
					ExceptionManager.Publish(ex);
				}
			}
		}
		#endregion

        /// <summary>
        /// handles all volume controls
        /// </summary>
        /// <param name="direction"></param>
        private void volume(string direction)
        {
            int v = 0;
            int i = 0;
            int iVol = 0;

            //switch (direction)
            //{
            //    case "volume up":
            //        v = MM.GetVolume(spkrVolumeControl,spkrComponent);

            //        i = -1;
            //        do
            //        {
            //            i++;
            //            iVol = volSteps[i];
            //        }
            //        while ((v >= iVol) && (i < volumeStep));

            //        SetSpkrVolume(spkrVolumeControl, spkrComponent, iVol);
            //        break;
            //    case "volume down":
            //        v = MM.GetVolume(spkrVolumeControl,spkrComponent);

            //        i = volumeStep + 1;
            //        do
            //        {
            //            i--;
            //            iVol = volSteps[i];
            //        }
            //        while ((v <= iVol) && (i > 0));

            //        SetSpkrVolume(spkrVolumeControl, spkrComponent, iVol);
            //        break;
            //    case "mute":
            //        if (muteStatus == 1)
            //            muteStatus = 0;
            //        else
            //            muteStatus = 1;

            //        MM.SetVolume(spkrMuteControl, spkrComponent, muteStatus);
            //        break;
            //}
        }

        public void Volume(double percentage)
        {
            int volIndex = 0;

            //if (percentage > 0)
            //{
            //    volIndex = (int)(Math.Round(volSteps.Length * percentage));

            //    int iVol = volSteps[volIndex];

            //    SetSpkrVolume(spkrVolumeControl, spkrComponent, iVol);
            //}
        }

        /// <summary>
        /// sets speaker volume and updates graphics
        /// </summary>
        /// <param name="control"></param>
        /// <param name="component"></param>
        /// <param name="newVol"></param>
        private void SetSpkrVolume(int control, int component, int newVol)
        {
            // Set spkr volume to the next step value
            //MM.SetVolume(control, component, newVol);

            // Update the graphics
            //rectW = (int)((newVol / 65535.0) * rectWMax + 1);

            // This will prevent the volume bar from disappearing at near zero levels
            //rectW = rectW < 4 ? 4 : rectW;

            //gSpkr.FillRectangle(brushErase, rectX, rectY, rectWMax + 1, rectH);
            //gSpkr.FillRectangle(brushBlue, rectX, rectY, rectW, rectH);
        }
		#endregion

		#region private bool PlayClip(string fileName)
		private bool PlayClip(string fileName)
		{
//			tabControl.SelectedTab = mediaViewerTab;
			this.Cursor = Cursors.WaitCursor;
//			rbFile.Checked = true;//checks the file radio button

//			EventLog.WriteEntry("sctv","fileName "+ fileName);
			try 
			{
//				cleanUp();
				if(chbPlayNowRepeat.Checked)
//					axMediaPlayer.settings.setMode("loop",true);

//				axMediaPlayer.URL = fileName;
//				axMediaPlayer.fullScreen = true;
//				ToggleFullScreen();
				this.Cursor = Cursors.Default;
				return true;
				//				if( ! GetInterfaces() )
				//					return false;
				//				
				//				CheckClipType();
				//				if( clipType == ClipType.none )
				//					return false;
				//
				//				int hr = mediaEvt.SetNotifyWindow( this.Handle, WM_GRAPHNOTIFY, IntPtr.Zero );
				//			
				//				if( (clipType == ClipType.audioVideo) || (clipType == ClipType.video) )
				//				{
				//					videoWin.put_Owner( pnlVideo.Handle );
				//					videoWin.put_WindowStyle( WS_CHILD | WS_CLIPSIBLINGS | WS_CLIPCHILDREN );
				//
				//					InitVideoWindow( 1, 1 );
				////					CheckSizeMenu( menuControlNormal );
				//					GetFrameStepInterface();
				//				}
				//				else
				//					InitPlayerWindow();
				//
				//				hr = mediaCtrl.Run();
				//				if( hr >= 0 )
				//					playState = PlayState.Running;
				//
				//				return hr >= 0;
			}
			catch( Exception ee )
			{
				MessageBox.Show( this, "Could not start clip\r\n" + ee.Message, "DirectShow.NET", MessageBoxButtons.OK, MessageBoxIcon.Stop );
				return false;
			}
			
//			EventLog.WriteEntry("sctv","found code");
//			m_CurrentStatus = MediaStatus.Running;
//			m_CurrentSource = MediaSource.file;
			
//			trackBar1.Maximum = 

			//				UpdateStatusBar();
			//				UpdateToolBar();
			
		}
		#endregion

		#region XMLProc functions
		#region private void XMLTVProc_DataReceived(object sender, DataReceivedEventArgs e)
		private void XMLTVProc_DataReceived(object sender, DataReceivedEventArgs e)
		{
			OutputTxt.AppendText(e.Data+Environment.NewLine);
			this.Refresh();
		}
		#endregion

		#region private void XMLTVProc_completed(object sender, System.EventArgs e)
		private void XMLTVProc_completed(object sender, System.EventArgs e)
		{
			loadListings();
			populateTVGrid();
			populateListView();
			XMLTVProc = null;
		}
		#endregion
		#endregion

		#region private void rbSchedule_CheckedChanged(object sender, System.EventArgs e)
//		private void rbSchedule_CheckedChanged(object sender, System.EventArgs e)
//		{
//			RadioButton myButton = new RadioButton();
//			myButton = (RadioButton)sender;
//			switch(myButton.Name)
//			{
//				case "rbTVSchedule":
//					this.pnlScheduleList.Visible = true;
//					pnlScheduleList.Visible = false;
//					this.pnlNotification.Visible = false;
//					break;
//				case "rbMySchedule":
//					this.pnlScheduleList.Visible = false;
//					pnlScheduleList.Visible = true;
//					this.pnlNotification.Visible = false;
//					break;
//				case "rbNotificationSchedule":
//					this.pnlNotification.Visible = true;
//					this.pnlScheduleList.Visible = false;
//					pnlScheduleList.Visible = false;
//					break;
//			}
//		}
		#endregion

		#region private void videoListView_SelectedIndexChanged(object sender, System.EventArgs e)
		private void videoListView_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (videoListView.SelectedItems.Count>0)
				MiscStuffRichText.AddMedia((DataRow)videoListView.SelectedItems[0].Tag);
		}
		#endregion

		#region mediaList Functions

		#region private void videoListView_DoubleClick(object sender, System.EventArgs e)//play file
		private void videoListView_DoubleClick(object sender, System.EventArgs e)//play file
		{
			//this will edit media
//			mediaGridView myGrid = (mediaGridView)sender;
//			videoListView.editMedia(myGrid.SelectedItems[0]);

			//this will play media clip
//			DataRow drVideoInfo = (DataRow)videoListView.SelectedItems[0].Tag;
//			clipFile = drVideoInfo["filePath"].ToString();
//			PlayClip(clipFile);

			//this will add clip to playNow list
			ListViewItem newItem = new ListViewItem();
			newItem.Text = videoListView.SelectedItems[0].Text;
			newItem.Tag = videoListView.SelectedItems[0].Tag;
			playNowListView.Items.Add(newItem);
			playNowListView.Refresh();
			this.Cursor = Cursors.Default;
		}
		#endregion


		#region private void videoListView_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		private void videoListView_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			videoListView.mediaMouseDown(sender,e);
		}
		#endregion

		public void updateMediaListXML()
		{
//			XmlNode thisNode = 
		}

		
		#endregion

		#region private void TVListViewSelect(object sender, System.EventArgs e)
		private void TVListViewSelect(object sender, System.EventArgs e)
		{
			listView.Clear();
			switch(sender.GetType().ToString())
			{
				case "System.Windows.Forms.MenuItem"://these are menuItems from rightClickMedia context menu
					MenuItem thisItem = (MenuItem)sender;
					switch(thisItem.Index)
					{
						case 0://"menuItemOnNow":
							listView.Clear();
							listView.InsertTVProgrammeRange(ListingsOrganizer.ShowsRightNow);
							break;
						case 1://"menuItemMoviesOnNow":

							break;
						case 2://"menuItemMoviesToday":
							listView.Clear();
							listView.InsertTVProgrammeRange(ListingsOrganizer.MoviesToday);
							break;
						case 3://"menuItemMoviesTomorrow":
							listView.Clear();
							listView.InsertTVProgrammeRange(ListingsOrganizer.MoviesTomorrow);
							break;
						case 4://"menuItemOnNextFewHours":
							listView.Clear();
							listView.InsertTVProgrammeRange(ListingsOrganizer.ShowsRightNow);
							listView.InsertTVProgrammeRange(ListingsOrganizer.AllShowsWithinNHours);
							break;
					}	
					break;
			}
		}
		#endregion

		#region private void listView_SelectedIndexChanged(object sender, System.EventArgs e)
		private void listView_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (listView.SelectedItems.Count>0)
				MiscStuffRichText.AddProgramme((TVProgramme)listView.SelectedItems[0].Tag);
		}
		#endregion

		#region Favorites Functions

		#region private void addToFavorites(TVProgramme favorite)
		private void addToFavorites(TVProgramme favorite)
		{
			
//			ListViewItem lviFavorite = new ListViewItem();
//			lviFavorite.SubItems[0].Text = favorite.Title;
//			lviFavorite.Tag = favorite;
//			if(!lvFavorites.Items.Contains(lviFavorite))//this show is not in the list - add it
//				lvFavorites.Items.Add(lviFavorite);
		}
		#endregion

		#region private void addToFavorites(TVProgramme[] favorites)
		private void addToFavorites(TVProgramme[] favorites)
		{
			foreach(TVProgramme show in favorites)
			{
				addToFavorites(show);
			}
		}
		#endregion

		#region private void lvFavorites_SelectedIndexChanged(object sender, System.EventArgs e)
		private void lvFavorites_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (lvFavorites.SelectedItems.Count>0)
			{
				TVProgramme show = (TVProgramme)lvFavorites.SelectedItems[0].Tag;
				//				MiscStuffRichText.AddProgramme(show);
				findShowTimes(show);
			}
			else
			{
				richTextBoxSchedules.Clear();
			}
		}
		#endregion

		#region private void findShowTimes(TVProgramme show)
		private void findShowTimes(TVProgramme show)
		{
			TVProgramme[] showTimes = ListingsOrganizer.SearchByTitle(show.Title);
			lvShowTimes.Items.Clear();
			foreach(TVProgramme prog in showTimes)
			{
				ListViewItem lviFavorite = new ListViewItem();
				lviFavorite.SubItems[0].Text = prog.StartTime +"-"+ prog.StopTime +"  "+ prog.SubTitle;
				lviFavorite.Tag = prog;
				lvShowTimes.Items.Add(lviFavorite);
			}
		}
		#endregion

//		#region public void saveFavorites(string fileName)
//		public void saveFavorites(string fileName)
//		{
//			XmlSerializer serializer=new XmlSerializer(typeof(TVProgramme[]));
//			TextWriter writer=new StreamWriter(fileName);
//
//			ArrayList favorites=new ArrayList();
//			foreach(ListViewItem lvFavorite in lvFavorites.Items)
//			{
//				favorites.Add(lvFavorite.Tag);
//			}
//
//			serializer.Serialize(writer,(TVProgramme[])favorites.ToArray(typeof(TVProgramme)));
//			writer.Close();
//		}
//		#endregion

		#region public TVProgramme[] loadFavorites(string fileName)
		public TVProgramme[] loadFavorites(string fileName)
		{
			XmlSerializer serializer=new XmlSerializer(typeof(TVProgramme[])); 
			FileStream fs=new FileStream(fileName, FileMode.Open);
			favorites = (TVProgramme[])serializer.Deserialize(fs);
			addToFavorites(favorites);
			fs.Close();
			return favorites;
		}
		#endregion

		#region private void lvShowTimes_SelectedIndexChanged(object sender, System.EventArgs e)
		private void lvShowTimes_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			try
			{
				if (lvShowTimes.SelectedItems.Count>0)
				{
					TVProgramme show = (TVProgramme)lvShowTimes.SelectedItems[0].Tag;
					MiscStuffRichText.AddProgramme(show);
					//				findShowTimes(show);
				}
				else
				{
					MiscStuffRichText.Clear();
					lvShowTimes.Items.Clear();
				}
			}
			catch(Exception ex)
			{
				EventLog.WriteEntry("sctv",ex.ToString());
			}
		}
		#endregion

		#region delete from favorites
		private void menuItem48_Click(object sender, System.EventArgs e)
		{
//			ShowNotificationPropertyGrid.SelectedObject = null;
			
			favoritesArrayList list=new favoritesArrayList();

			foreach(ListViewItem item in lvFavorites.SelectedItems)
			{
				EventLog.WriteEntry("sctv","favorite type "+ item.Tag.GetType().ToString());
				list.Add((TVProgramme)item.Tag);
			}

			favoriteOrganizer.Remove(list);

//			if(lvFavorites.SelectedItems.Count > 0)
//			{
//				lvFavorites.Items.Remove(lvFavorites.SelectedItems[0]);
//				lvShowTimes.Items.Clear();
//			}
		}
		#endregion

		#endregion

		#region PlayList Functions	
		private void getPlaylist_Click(object sender, System.EventArgs e)
		{
//			clipFile = "H:\\Documents and Settings\\Administrator.BOB\\My Documents\\Visual Studio Projects\\TVListings\\bin\\Debug\\Playlists\\playlist.asx";
//			PlayClip(clipFile);
		}

		
		#endregion

		#region playSchedule Functions

		#region private void playScheduleTimer_Tick(object sender, System.EventArgs e)
		private void playScheduleTimer_Tick(object sender, System.EventArgs e)
		{
			DateTime Now=System.DateTime.Now;

			// go through play schedules and check start times
			foreach(playSchedule show in pScheduleOrganizer.GetList)
			{
				if (show.AlreadyNotified==false && show.IsStarting())
				{
					show.AlreadyNotified=true;
//					notifyIcon.ShowBalloon(HansBlomme.Windows.Forms.NotifyIcon.EBalloonIcon.Info,show.name+" on channel "+show.channel,"Show Starting",350);
//					EventLog.WriteEntry("sctv","popup notification balloon");
//					showNotificationWindow NotifyFrm=new showNotificationWindow();
//					NotifyFrm.Show();
//					taskbarNotifier.CloseClickable=true;
//					taskbarNotifier.TitleClickable=true;
//					taskbarNotifier.ContentClickable=true;
//					taskbarNotifier.EnableSelectionRectangle=true;
//					taskbarNotifier.KeepVisibleOnMousOver=true;	// Added Rev 002
//					taskbarNotifier.ReShowOnMouseOver=true;			// Added Rev 002
//					taskbarNotifier.Show("Testing","This is my content",500,500,5000);

					// this will notify
//					taskbarNotifier.Show(show);

					// this will play the programme
					TVProgramme theShow = (TVProgramme)show.theProgramme;
					myTVViewer.changeChannel(Convert.ToInt32(theShow.Channel.DisplayName.Substring(1,theShow.Channel.DisplayName.IndexOf(" ")-1)));

					// this plays a sound
					if (ProgramConfiguration.PlaySound==true && File.Exists(ProgramConfiguration.NotificationSound))
						PlayWaveFile(ProgramConfiguration.NotificationSound);
				}
			}

			foreach(favorites show in favoriteOrganizer.GetList)
			{
				if (show.AlreadyNotified==false && show.IsStarting())
				{
					show.AlreadyNotified=true;

					// this will play the programme
//					TVProgramme theShow = (TVProgramme)show.theProgramme;
//					myTVViewer.changeChannel(Convert.ToInt32(theShow.Channel.DisplayName.Substring(1,theShow.Channel.DisplayName.IndexOf(" ")-1)));

					// this plays a sound
					if (ProgramConfiguration.PlaySound==true && File.Exists(ProgramConfiguration.NotificationSound))
						PlayWaveFile(ProgramConfiguration.NotificationSound);
				}
			}

			foreach(ShowNotification show in RecordingOrganizer.GetList)
			{
//				DateTime Now=System.DateTime.Now;
//
//				if (show.IsStarting() && show.AlreadyNotified==false)
//				{
//					show.AlreadyNotified=true;
//					//record show
//					Capture testCapture = new Capture( filters.VideoInputDevices[0], filters.AudioInputDevices[0] );
//					try{testCapture.VideoCompressor = new Filter( ProgramConfiguration.videoCompressor );}
//					catch{}
//					try{testCapture.AudioCompressor = new Filter( ProgramConfiguration.audioCompressor );}
//					catch{}
//					try{testCapture.FrameRate = Convert.ToDouble(ProgramConfiguration.frameRate);}
//					catch{}
//					//			try{capture.FrameSize = ProgramConfiguration.frameSize;}catch{}
//					//			try{capture.AudioChannels = Convert.toshortProgramConfiguration.audioChannels;}catch{}
//					//			try{capture.AudioSampleSize = (short)ProgramConfiguration.audioSampleSize;}catch{}
//					try{testCapture.AudioSamplingRate = Convert.ToInt32(ProgramConfiguration.audioSamplingRate);}
//					catch{}
//					testCapture.Filename = "test2.avi";
//					testCapture.Start();
//					EventLog.WriteEntry("sctv","recording");
//					captureList.Add("testCapture",testCapture);
//					btnRecordStop.Enabled = true;
//					if (ProgramConfiguration.PlaySound==true && File.Exists(ProgramConfiguration.NotificationSound))
//						PlayWaveFile(ProgramConfiguration.NotificationSound);
//				}
			}
		}
		#endregion

		#region private void playScheduleList_SelectedIndexChanged(object sender, System.EventArgs e)
		private void playScheduleList_SelectedIndexChanged(object sender, System.EventArgs e)
		{
//			if (playScheduleList.SelectedItems.Count>0)
//			{
//				playSchedule ps = (playSchedule)playScheduleList.SelectedItems[0].Tag;
//				richTextBoxSchedules.AddProgramme((TVProgramme)ps.theProgramme);
//			}
		}
		#endregion
		#endregion

		#region PlayGrid Functions
		#region private void playGridView_OnSelectionChanged(object o, SCTV.TVGridViewEventArgs e)
//		private void playGridView_OnSelectionChanged(object o, SCTV.TVGridViewEventArgs e)
//		{
//			richTextBoxSchedules.AddProgramme(e.Show);
//		}
		#endregion

		#region private void playGridView_Click(object sender, System.EventArgs e)
//		private void playGridView_Click(object sender, System.EventArgs e)
//		{
//			playGridView pGV = (playGridView)sender;
//			richTextBoxSchedules.AddProgramme(pGV.SelectedProgramme);
//		}
		#endregion

		#endregion

		#region private void gridView_VisibleChanged(object sender, System.EventArgs e)
		private void gridView_VisibleChanged(object sender, System.EventArgs e)
		{
//			if(gridView.Visible)
//				gridView.ScrollToTodaysColumn();
		}		
		#endregion

		#region channelList functions

		public void populateChannelLists()
		{
			//load "allChannels" by default
			
//			chbLChannels.Items.AddRange(channelLists.ToArray());
		}
		
		private void createDefaultChannelLists()
		{
			channelLists.saveChannelLists(ListingsOrganizer.AllChannels);
		}

		private void channelListChanged()//update list view
		{
			channelListView.Items.Clear();
			Array channelArray = channelLists.ToArray(typeof(TVChannel));
//			EventLog.WriteEntry("sctv","channelListChanged size: "+ channelArray.Length.ToString());
			//  need to load current channelList and "check" the ones that are on it
			foreach(TVChannel channel in ListingsOrganizer.AllChannels)
			{
				ListViewItem newItem = new ListViewItem();
				newItem.Text = channel.DisplayName;
				
				newItem.StateImageIndex = 0;
				newItem.ImageIndex = 1;
				
				channelListView.Items.Add(newItem);

			}
		}

		private void channelListView_SelectedIndexChanged(object sender, System.EventArgs e)
		{
//			EventLog.WriteEntry("sctv","sender: "+ sender.GetType().ToString());
			ListView listView = (ListView)sender;
			if(listView.SelectedItems.Count > 0)
			{
				if(listView.SelectedItems[0].StateImageIndex == 0) //this one is not selected...yet
				{
					listView.SelectedItems[0].StateImageIndex = 1;
				}
				else
				{
					listView.SelectedItems[0].StateImageIndex = 0;
				}
			}
		}
		#endregion

		#region private void videoListView_VisibleChanged(object sender, System.EventArgs e)
		private void videoListView_VisibleChanged(object sender, System.EventArgs e)
		{
			mediaGridView myView = (mediaGridView)sender;
			if(myView.Visible && myView.SelectedItems.Count < 1)
			{
				videoListView.Focus();
				videoListView.Select();
			}			
		}
		#endregion

		#region private void changeChannelTimer_Tick(object sender, System.EventArgs e)
		private void changeChannelTimer_Tick(object sender, System.EventArgs e)
		{
			try
			{
				changeChannelTimer.Stop();
				changeChannelTimer.Enabled = false;
				executeKeyStrokes();
			}
			catch(Exception ex)
			{
				EventLog.WriteEntry("sctv","changeChannel Error: "+ ex.ToString());
			}
		}
		#endregion

		#region private void executeKeyStrokes()
		private void executeKeyStrokes()
		{
			string tempKeys = "";
			foreach(int intChannel in keyStrokeTracker)
			{
				if(keyList.Contains(intChannel.ToString()))//this is a valid channel key
					tempKeys += keyList.GetByIndex(keyList.IndexOfKey(intChannel.ToString())).ToString();
				else
					break;
			}
			keyStrokeTracker.Clear();
			try
			{
				if(Convert.ToInt32(tempKeys) > 0)
				{
					myTVViewer.changeChannel(Convert.ToInt32(tempKeys));
				}
			}
			catch(Exception ex)
			{
				EventLog.WriteEntry("sctv","tempKeys: "+ tempKeys);
				EventLog.WriteEntry("sctv",ex.ToString());
			}
		}
		#endregion

        #region private void executeMacros(string macroName)//executes macros immediately
        public void executeMacros(string macroName)
		{
			bool foundMacro = true;
			if(changeChannelTimer.Enabled)
				changeChannelTimer.Enabled = false;
			
			keyStrokeTracker.Clear();
			switch(macroName)
			{
				case "mute":
                case "volume down":
                case "volume up":
                    volume(macroName);
                    break;
				case "channel":
					createTVViewer();
					break;
				case "tvKey":
					switch(GUIState)
					{
						case guiState.dvd:
							break;
						case guiState.music:
							break;
						case guiState.pictures:
							break;
						case guiState.radio:
							break;
						case guiState.TV://switch to the tvList view
							if(myTVViewer.Visible)
							{
								tabControl.SelectedTab = tvListingsTab;
								tabControl1.SelectedTab = TVListView;
								listView.Select();
							}
							else
							{
								createTVViewer();
							}
							break;
//						case guiState.video://pause any playing videos and show the TV
//							mySplash.Show();
//							myTVViewer.changeChannel(4);
//							break;
						default://turn on TV
							EventLog.WriteEntry("sctv","-----  calling createTVViewer");
							createTVViewer();
							break;
					}
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
					switch(GUIState)
					{
//						case guiState.TV:
//							if(myTVViewer!=null)
//								myTVViewer.Close();
//							break;
//						case guiState.mediaLibrary:
//							// TODO: check if they have a paused/stopped file and start playing it
//							break;
						default:
//							EventLog.WriteEntry("sctv","       showing myMediaLibrary       ------------");
                            //myMediaLibrary = null;

                            //if(myMediaLibrary==null)//create mediaLibrary
                            //{


                            //    //speechListener.executeCommand -= new SCTV.speechRecognition.heardCommand(speechListener_executeCommand);
                            //    //EventLog.WriteEntry("sctv","       creating myMediaLibrary       ------------");
                            //    speechListener.loadGrammarFile("xmlmediaLibrary.xml");
                            //    //speechListener.addRulesToCurrentGrammar("xmlmediaLibrary.xml");
                            //    //speechListener.addRulesToCurrentGrammar("xmlMedia.xml");
                            //    myMediaLibrary=null;
                            //    myMediaLibrary = new mediaLibrary();
                            //    myMediaLibrary.KeyUp += new System.Windows.Forms.KeyEventHandler(KeyUpHandler);
                            //    myMediaLibrary.ShowDialog();
                            //    //myMediaLibrary.BringToFront();
                            //    //myMediaLibrary.Select();
                            //    //myMediaLibrary.Refresh();
                            //    //myMediaLibrary.Focus();
                            //    //myMediaLibrary.Visible=true;
                            //    //EventLog.WriteEntry("sctv","       created myMediaLibrary       ------------");
                            //}
                            //else
                            //    myMediaLibrary.ShowDialog();

							break;
					}
					
					break;
				case "dvdKey":
					MessageBox.Show("Coming Soon!!");
					break;
				default:
					foundMacro = false;
					break;
			}
			if(foundMacro)
				keyStrokeTracker.Clear();
		}
		#endregion

		#region private void createTVViewer()
		private void createTVViewer()
		{
            //MySplash.Show();
			myTVViewer=null;
			myTVViewer = new TVViewer();
//				myTVViewer.Disposed+=new EventHandler(myTVViewer_Disposed);
			myTVViewer.KeyUp += new System.Windows.Forms.KeyEventHandler(KeyUpHandler);
			myTVViewer.VisibleChanged += new EventHandler(hideSplashScreen);
			myTVViewer.MouseUp += new System.Windows.Forms.MouseEventHandler(mouseUpHandler);
			myTVViewer.Select();
			myTVViewer.Refresh();
			myTVViewer.Focus();
			myTVViewer.changeChannel(4);
			myTVViewer.Show();
            //MySplash.Hide();
		}
		#endregion

		#region private void Form1_Closed(object sender, System.EventArgs e)
		private void Form1_Closed(object sender, System.EventArgs e)
		{
			if(myTVViewer!=null)
				myTVViewer.Close();

			//Save program configuration to disk
            Microsoft.ApplicationBlocks.ConfigurationManagement.ConfigurationManager.Write("XmlSerializer", (ConfigurationData)ProgramConfiguration);
			NotificationOrganizer.SaveShowNotifications(NotificationsFileName);
			RecordingOrganizer.SaveShowNotifications(RecordingsFileName);
			favoriteOrganizer.Savefavoritess("config/favorites.xml");
//			videoListView.saveMediaListData(myMedia.getCategory("All"));
			
//				if (XMLTVProc.XMLTVProc.IsDone==false)
//					XMLTVProc.Cancel();
			
		}
		#endregion

		#region test functions
		
		#region private void trackBar1_Scroll(object sender, System.EventArgs e)
		private void trackBar1_Scroll(object sender, System.EventArgs e)
		{
//			TrackBar mediaTB = (TrackBar)sender;
// 
//			long curPos = 0; 
//			mediaSeek.GetCurrentPosition(out curPos); 
//
//			DsOptInt64 pos = new DsOptInt64( curPos+50000000 );
//			int hr = mediaCtrl.Pause();
//			hr = mediaSeek.SetPositions( pos, SeekingFlags.AbsolutePositioning, null, SeekingFlags.NoPositioning );
//			hr = mediaCtrl.Run();
		}
		#endregion

		private void button4_Click(object sender, System.EventArgs e)
		{
//			playGridView.Visible = false;
//			playGridView.PopulateGrid(pScheduleOrganizer.GetList);
//			playGridView.Visible = true;
		}

		private void button5_Click(object sender, System.EventArgs e)//clear playSchedules
		{
//			pScheduleOrganizer.Remove(pScheduleOrganizer.GetList);
			//			playGridView.PopulateGrid(pScheduleOrganizer.AllShows);
		}

		private void menuItem11_Click(object sender, System.EventArgs e)
		{
			//			EventLog.WriteEntry("sctv","sender: "+ sender.GetType().ToString());
			
			//			ListView lv = (ListView)sender;
//			playScheduleArrayList psAL = new playScheduleArrayList();
//			psAL.Add((playSchedule)playScheduleList.SelectedItems[0].Tag);
//			pScheduleOrganizer.Remove(psAL);
			//			pScheduleOrganizer.SaveplaySchedules(playSchedulesFileName);
			//			pScheduleOrganizer.LoadplaySchedules(playSchedulesFileName);
			//			playScheduleList.SetOrganizer=pScheduleOrganizer;
		}
		#endregion

		private void menuItem59_Click(object sender, System.EventArgs e)
		{
			taskbarNotifier.Show("Testing","This is my content",500,100,5000);
		}

		private void KeyUpHandler(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			//EventLog.WriteEntry("sctv","guiState "+ GUIState.ToString());
			changeChannelTimer.Enabled = true;
			changeChannelTimer.Start();
			keyStrokeTracker.Add(e.KeyValue);
			string tempKeys = "";
			if(keyStrokeTracker.Count > 0)
			{
				foreach(int intChannel in keyStrokeTracker)
				{
					tempKeys += intChannel.ToString() +",";
				}
				tempKeys = tempKeys.Remove(tempKeys.LastIndexOf(","),1);//removes trailing comma
			}
			if(mainMenuMacroList.Contains(tempKeys))
			{
				keyStrokeTracker.Clear();
				string macroName = mainMenuMacroList.GetByIndex(mainMenuMacroList.IndexOfKey(tempKeys)).ToString();
				switch(sender.GetType().ToString())
				{
					case "SCTV.Form1":
						GUIState = guiState.admin;
						break;
//					case "SCTV.Form1":
//						GUIState = guiState.dvd;
//						break;
//					case "SCTV.Form1":
//						GUIState = guiState.music;
//						break;
//					case "SCTV.Form1":
//						GUIState = guiState.pictures;
//						break;
//					case "SCTV.Form1":
//						GUIState = guiState.radio;
//						break;
					case "SCTV.TVViewer":
						break;
					case "SCTV.mediaLibrary":
						
						break;
					default:

						break;
				}
				executeMacros(macroName);
			}
			//else
				//EventLog.WriteEntry("sctv","didn't find macro for: "+ tempKeys);

			//EventLog.WriteEntry("sctv","KeyCode from TVViewer keyup: "+ e.KeyCode);
			//EventLog.WriteEntry("sctv","KeyValue from "+ sender.GetType().ToString() +" keyup: "+ e.KeyValue);
		}

		#region Input Listeners

		#region private void InputListener_KeyPress( object sender, KeyPressEventArgs e) 
		private void InputListener_KeyPress( object sender, KeyPressEventArgs e) 
		{ 
//			string ctrlKey = "";
//			string shiftKey = "";
//			string menuKey = "";
//			string tempKeys = "";
//			if(e.ModifierKeys == Keys.Shift)
//				shiftKey = "16,";
//			if(e.ModifierKeys == Keys.Control)
//				ctrlKey = "17,";
//			if(e.ModifierKeys == Keys.Menu)
//				menuKey = "18";
//			tempKeys = ctrlKey + shiftKey + menuKey + e.KeyCode;
//
//			if(macroList.Contains(tempKeys))
//			{
//				string macroName = macroList.GetByIndex(macroList.IndexOfKey(tempKeys)).ToString();
//				executeKeyStrokes(macroName);
//			}
//			else
//			{
//				try
//				{
//					EventLog.WriteEntry("sctv","__________     didn't find macro  "+ macroList.GetByIndex(macroList.IndexOfKey(tempKeys)).ToString());
//				}
//				catch{}
//			}
//
			
		}
		#endregion

		#region private void InputListener_MouseButton( object sender, MouseButtonEventArgs e) 
//        private void InputListener_MouseButton( object sender, MouseButtonEventArgs e) 
//        { 
//            if (e.buttonState == MouseButtonState.Pressed) 
//            { 
////				EventLog.WriteEntry("sctv","Pressed "+ e.Button.ToString());
////				tbStatus.Text += "Pressed " + 
////					e.Button.ToString() + "\r\n"; 
//            } 
//            else 
//            { 
//                //EventLog.WriteEntry("sctv","Released "+ e.Button.ToString());
////				tbStatus.Text += "Released " + 
////					e.Button.ToString() + "\r\n"; 
////				EventLog.WriteEntry("sctv","guiState "+ GUIState.ToString());
////				switch(GUIState)
////				{
////					case guiState.admin:
////						break;
////					case guiState.dvd:
////						break;
////					case guiState.music:
////						break;
////					case guiState.pictures:
////						break;
////					case guiState.radio:
////						break;
////					case guiState.TV:
////						EventLog.WriteEntry("sctv","++++++++++++++   executing \"enter\"");
////						//					MessageBox.Show("You need to press \"Toggle\"");
////						myTVViewer.executeKeyStrokes("Enter");
////						//					if(TVMacroList.Contains(tempKeys))
////						//					{
////						//						string macroName = TVMacroList.GetByIndex(TVMacroList.IndexOfKey(tempKeys)).ToString();
////						//						executeKeyStrokes("TVViewer_"+ macroName);
////						//					}
////						//					else
////						//						EventLog.WriteEntry("sctv","didn't find TV macro");
////						break;
////					case guiState.video:
////						break;
////					default:
////
////						break;
////				}
//            } 
////			tbStatus.SelectionStart = tbStatus.Text.Length; 
////			tbStatus.ScrollToCaret(); 
//        }
		#endregion

		#region private void InputListener_MouseMove( object sender, MouseMoveEventArgs e) 
//        private void InputListener_MouseMove( object sender, MouseMoveEventArgs e) 
//        { 
////			EventLog.WriteEntry("sctv","found mouse move - showing cursor");
////			Cursor.Show();
////			mouseHideTimer.Enabled = false;
////			mouseHideTimer.Enabled = true;
//////			mouseHideTimer.Stop();
////			mouseHideTimer.Start();
////			tbStatus.Text += "Moved " + e.X.ToString() + 
////				"," + e.Y.ToString() + "\r\n"; 
////			tbStatus.SelectionStart = tbStatus.Text.Length; 
////			tbStatus.ScrollToCaret(); 
//        } 
		#endregion

		#endregion

		#region private void Form1_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		private void Form1_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			GUIState = guiState.admin;
//			Cursor.Show();
		}
		#endregion

		#region private void changeChannelTimer_Tick_1(object sender, System.EventArgs e)
		private void changeChannelTimer_Tick_1(object sender, System.EventArgs e)
		{
			try
			{
				changeChannelTimer.Stop();
				changeChannelTimer.Enabled = false;
				executeKeyStrokes();
			}
			catch(Exception ex)
			{
				EventLog.WriteEntry("sctv","changeChannel Error: "+ ex.ToString());
			}
		}
		#endregion

		private void Form1_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{

		}

		private void hideSplashScreen(object sender, System.EventArgs e)//triggers when visibility changes for TVViewer
		{
            //MySplash.Hide();
		}

		private void clock_Tick(object sender, System.EventArgs e)
		{

		}

		#region private void updateTVListings()
		private void updateTVListings()
		{
			if(XMLTVProc==null)
			{
				XMLTVProc=new ProcessCaller(this);
				XMLTVProc.StdOutReceived+=new SCTVTelevision.ProcessCaller.DataReceivedHandler(XMLTVProc_DataReceived);
                XMLTVProc.StdErrReceived += new SCTVTelevision.ProcessCaller.DataReceivedHandler(XMLTVProc_DataReceived);
				XMLTVProc.Completed += new System.EventHandler(XMLTVProc_completed);
				XMLTVProc.FileName=Application.StartupPath +"\\myTV\\xmltv.exe";
				XMLTVProc.Arguments="tv_grab_na_dd --config-file \""+ Application.StartupPath +"\\myTV\\.xmltv\\tv_grab_na_dd.conf\" --days "+ ProgramConfiguration.XMLTVDaysToDownload.ToString() +" --output \""+ Application.StartupPath +"\\config\\myTV.xml\"";
				XMLTVProc.Start();
			}
		}
		#endregion

		#region private void listView_VisibleChanged(object sender, System.EventArgs e)
		private void listView_VisibleChanged(object sender, System.EventArgs e)
		{
			listView.Items.Clear();
			listView.InsertTVProgrammeRange(ListingsOrganizer.ShowsRightNow);
			listView.InsertTVProgrammeRange(ListingsOrganizer.AllShowsWithinNHours);
			listView.Select();
			if(ListingsOrganizer.AllShowsWithinNDays.Length < ListingsOrganizer.AllChannels.Length*10)//they are getting low on shows - need to update listings file
			{
				updateTVListings();
			}
		}
		#endregion

		private void favoriteOrganizer_onListChange()
		{
//			foreach(favorites newFavorite in favoriteOrganizer.AllShows)
//			{
//				
//			}
			lvFavorites.Clear();
			lvFavorites.PopulateListView();
//			lvFavorites.InsertTVProgrammeRange(favoriteOrganizer.AllShows);
			//			listView.Sort(ProgramConfiguration.DefaultSortField,ProgramConfiguration.DefaultSortMode);
//			lvFavorites.Sort(TVComparer.ESortBy.Title,ProgramConfiguration.DefaultSortMode);
			lvFavorites.Sort();
		}

		private void menuItem61_Click(object sender, System.EventArgs e)
		{
			executeMacros("tvKey");
		}

		private void menuItem66_Click(object sender, System.EventArgs e)
		{
			executeMacros("dvdKey");
		}

		private void menuItem65_Click(object sender, System.EventArgs e)
		{
			executeMacros("videoKey");
		}

		private void mouseUpHandler(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			//EventLog.WriteEntry("sctv","found mouseUp in TVViewer "+ e.Button.ToString());
			switch(e.Button.ToString())
			{
				case "Middle":// = "Enter" key
					myTVViewer.executeKeyStrokes("Enter");
					break;
				case "Left":
					myTVViewer.executeKeyStrokes("previousChannel");
					break;
				case "Right":
					myTVViewer.executeKeyStrokes("play");
					break;
			}
		}

		private void Form1_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			//EventLog.WriteEntry("sctv","finally found mouseUp");
		}

		private void Form1_VisibleChanged(object sender, System.EventArgs e)
		{
			if(Form1.GUIState == guiState.admin)
				Cursor.Show();
		}

		private void myTVViewer_Disposed(object sender, EventArgs e)
		{
			if(myTVViewer!=null)
			{
				myTVViewer.Dispose();
				myTVViewer = null;
				//EventLog.WriteEntry("sctv","$$$$  Disposed myTVViewer");
			}
		}

        //private void speechListener_executeCommand(Phrase heardPhrase)
        //{
        //    EventLog.WriteEntry("sctv","speechListener.speechCommand form1 " + heardPhrase.phrase);
            
        //    switch (heardPhrase.phrase.ToLower())
        //    {
        //        case "video library":
        //            executeMacros("videoKey");
        //            break;
        //        case "mute":
        //        case "volume up":
        //        case "volume down":
        //            executeMacros(heardPhrase.phrase.ToLower());
        //            break;
        //        case "admin":
        //            //executeMacros("videoKey");
        //            break;
        //        case "who is the fairest of them all":
        //            //executeMacros("videoKey");
        //            break;
        //        case "who is the master":
        //            break;
        //    }

        //    //switch (GUIState)
        //    //{
        //    //    case guiState.mediaLibrary:
        //    //        if (speechListener.speechCommand.IndexOf("category-") >= 0)//they are asking for a category
        //    //        {
        //    //            string category = System.Text.RegularExpressions.Regex.Replace(speechListener.speechCommand, "category-", "");
        //    //            myMediaLibrary.shuffleCategories(myMediaLibrary.categoryButtons.IndexOf(category), "left");

        //    //            myMediaLibrary.displayCategory(category);
        //    //        }
        //    //        else if (speechListener.speechCommand.IndexOf("titleSearch()") >= 0)//they are asking to search titles
        //    //        {
        //    //            myMediaLibrary.lblSearch.Visible = true;
        //    //            myMediaLibrary.lblSearch.Text = "Searching for title: ";
        //    //            myMediaLibrary.lblSearch.BringToFront();
        //    //            //				myMedia.dsMedia.Tables[0].DefaultView.RowFilter="title LIKE '"+ System.Text.RegularExpressions.Regex.Replace(speechListener.speechCommand,"titleSearch-","") +"'";
        //    //            //				myMediaLibrary.displayCategory(myMedia.dsMedia.Tables[0].DefaultView);
        //    //        }
        //    //        else if (speechListener.speechCommand.IndexOf("title-") >= 0)//they are searching titles
        //    //        {
        //    //            myMediaLibrary.lblSearch.Text = "Found: " + System.Text.RegularExpressions.Regex.Replace(speechListener.speechCommand, "title-", "");
        //    //        }
        //    //        else if (speechListener.speechCommand.IndexOf("title-") >= 0)
        //    //        {
        //    //            EventLog.WriteEntry("sctv","found title to search for");
        //    //        }
        //    //        else
        //    //            executeMacros(speechListener.speechCommand);
        //    //        break;
        //    //    case guiState.defaultState:
        //    //        executeMacros("videoKey");
        //    //        break;
        //    //}			
        //}

        private void VolumeInserted(int aMask)
        {
            // -------------------------
            // A volume was inserted
            // -------------------------
            //MessageBox.Show("Volume inserted in " + deviceMonitor.MaskToLogicalPaths(aMask));
            //lbEvents.Items.Add("Volume inserted in " + fNative.MaskToLogicalPaths(aMask));

            //string discName;
            //DriveInfo driveInfo;
            //InsertedMedia insertedMedia = new InsertedMedia();
            //insertedMedia.ShowDialog(this);
            //string driveLetter = deviceMonitor.MaskToLogicalPaths(aMask);

            //switch (insertedMedia.MediaState)
            //{
            //    case MediaStateEnum.Play:
            //        playRemoveableMedia(driveLetter);
            //        break;
            //    case MediaStateEnum.PlayAndRecord:
            //        driveInfo = new DriveInfo(driveLetter);
            //        discName = driveInfo.VolumeLabel;

            //        discName = discName.Replace("<", "");
            //        discName = discName.Replace(">", "");
            //        discName = discName.Replace("[", "");
            //        discName = discName.Replace("]", "");
            //        //Regex.Replace(targetString, @"\W*", "");

            //        playRemoveableMedia(driveLetter, defaultPathToSaveTo +"\\"+ discName +".CEL");
            //        break;
            //    case MediaStateEnum.Record:
            //        driveInfo = new DriveInfo(driveLetter);
            //        discName = driveInfo.VolumeLabel;
            //        recordRemoveableMedia(driveLetter, defaultPathToSaveTo + "\\" + discName + ".CEL");
            //        break;
            //}
        }

        private void VolumeRemoved(int aMask)
        {
            // --------------------
            // A volume was removed
            // --------------------
            //MessageBox.Show("Volume removed from " + deviceMonitor.MaskToLogicalPaths(aMask));
            //lbEvents.Items.Add("Volume removed from " + fNative.MaskToLogicalPaths(aMask));
        }

        private void playRemoveableMedia(string driveLetter)
        {
            playRemoveableMedia(driveLetter, "");            
        }

        private void playRemoveableMedia(string driveLetter, string fileToRecordTo)
        {
            //make sure mediaplayer exists
            //FormCollection openForms = Application.OpenForms;

            //if (openForms["liquidMediaPlayer"] == null)
            //{
            //    liquidMediaPlayer mediaPlayer = new liquidMediaPlayer();
            //    mediaPlayer.PlayRemoveableMedia(driveLetter, fileToRecordTo);

            //    mediaPlayer.ShowDialog();
            //}
            //else
            //    openForms["liquidMediaPlayer"].ShowDialog();
        }

        private void recordRemoveableMedia(string driveLetter, string fileToRecordTo)
        {
            //make sure mediaplayer exists
            //FormCollection openForms = Application.OpenForms;

            //if (openForms["liquidMediaPlayer"] == null)
            //{
            //    liquidMediaPlayer mediaPlayer = new liquidMediaPlayer();
            //    mediaPlayer.RecordRemoveableMedia(driveLetter, fileToRecordTo);

            //    mediaPlayer.ShowDialog();
            //}
            //else
            //    openForms["liquidMediaPlayer"].ShowDialog();
        }
	}
}
