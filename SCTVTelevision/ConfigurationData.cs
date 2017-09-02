using System;
using System.ComponentModel;
using System.Drawing;
using System.Xml;
using System.Xml.Serialization;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace SCTVTelevision
{
	/// <summary>
	/// Summary description for ConfigurationData.
	/// </summary>
	public class ConfigurationData
	{
		public enum Displays
		{
			List,
			Grid
		}
		public enum ColorFormat
		{
			NamedColor,
			ARGBColor
		}
		public enum XMLTVGrabber
		{
			tv_grab_na,
			tv_grab_de,
			tv_grab_dk,
			tv_grab_es,
			tv_grab_fi,
			tv_grab_hu,
			tv_grab_it,
			tv_grab_nl,
			tv_grab_nl_wolf,
			tv_grab_sn,
			tv_grab_uk,
			tv_grab_uk_rt
		}

        private string mDefaultUrl;
        private bool mLoadListingsOnStartup;
        private string mIconDir;
        private int mDaysToDisplay;
		private int mFewHoursFromNow;
		private Color mMovieColor;
		private Color mChildrenColor;
		private Color mFavoritesColor;
		private Color mTalkColor;
		private Color mNewsColor;
		private Color mSportsColor;
		private Color mDefaultColor;
		private TVComparer.ESortBy mDefaultSortField;
		private TVComparer.ESortMode mDefaultSortMode;
		private Displays mDefaultDisplay;
		private string mNotificationSound;
		private bool mNotificationPlaySound;
		private string mVideoDevice;
		private string mAudioDevice;
		private string mVideoCompressor;
		private string mAudioCompressor;
		private string mFrameRate;
		private string mFrameSize;
		private string mAudioChannels;
		private string mAudioSampleSize;
		private string mAudioSamplingRate;
		private string mPages;

		private string mChannelList;

		//Settings for running XMLTV
		private string mXMLTVEXELocation;
		private string mXMLTVConfigLocation;
		private string mXMLTVOutputFile;
		private XMLTVGrabber mGrabber;
		private int mDaysToDownload;

		private CustomHighlightCollection mUserHighlights;

		public ConfigurationData()
		{
			//
			// TODO: Add constructor logic here
			//
		}

        #region "Properties"

		#region "Application"
        [CategoryAttribute("Application")]
        [DescriptionAttribute("Default URL to retrieve listings from. Can also be a file located on the hard disk.")]
        public string DefaultUrl
        {
            get { return(mDefaultUrl); }
            set { mDefaultUrl=value; }
        }

        [CategoryAttribute("Application")]
        [DescriptionAttribute("If true program will load listings automatically when the program starts")]
        public bool LoadListingsOnStartup
        {
            get { return(mLoadListingsOnStartup); }
            set { mLoadListingsOnStartup=value; }
        }

        [CategoryAttribute("Application")]
        [DescriptionAttribute("Location where Channel icons are stored")]
        public string IconDir
        {
            get { return(mIconDir); }
            set { mIconDir=value; }
        }

		[CategoryAttribute("Application")]
		[DescriptionAttribute("The video device to use at startup")]
		public string videoDevice
		{
			get { return(mVideoDevice); }
			set { mVideoDevice=value; }
		}

		[CategoryAttribute("Application")]
		[DescriptionAttribute("The Audio Device to use at startup")]
		public string audioDevice
		{
			get { return(mAudioDevice); }
			set { mAudioDevice=value; }
		}

		[CategoryAttribute("Application")]
		[DescriptionAttribute("The Video compressor")]
		public string videoCompressor
		{
			get { return(mVideoCompressor); }
			set { mVideoCompressor=value; }
		}

		[CategoryAttribute("Application")]
		[DescriptionAttribute("The Audio compressor")]
		public string audioCompressor
		{
			get { return(mAudioCompressor); }
			set { mAudioCompressor=value; }
		}

		[CategoryAttribute("Application")]
		[DescriptionAttribute("The Frame Rate")]
		public string frameRate
		{
			get { return(mFrameRate); }
			set { mFrameRate=value; }
		}

		[CategoryAttribute("Application")]
		[DescriptionAttribute("The Frame Size")]
		public string frameSize
		{
			get { return(mFrameSize); }
			set { mFrameSize=value; }
		}

		[CategoryAttribute("Application")]
		[DescriptionAttribute("The Audio Channels")]
		public string audioChannels
		{
			get { return(mAudioChannels); }
			set { mAudioChannels=value; }
		}

		[CategoryAttribute("Application")]
		[DescriptionAttribute("The Audio Sample Size")]
		public string audioSampleSize
		{
			get { return(mAudioSampleSize); }
			set { mAudioSampleSize=value; }
		}

		[CategoryAttribute("Application")]
		[DescriptionAttribute("The Audio Sampling Rate")]
		public string audioSamplingRate
		{
			get { return(mAudioSamplingRate); }
			set { mAudioSamplingRate=value; }
		}

		[CategoryAttribute("Application")]
		[DescriptionAttribute("The property pages")]
		public string pages
		{
			get { return(mPages); }
			set { mPages=value; }
		}
		#endregion

		#region "Display"
		[CategoryAttribute("Display")]
		[DescriptionAttribute("The default display to use when the program starst up")]
		public Displays DefaultDisplay
		{
			get { return(mDefaultDisplay); }
			set { mDefaultDisplay=value; }
		}

        [CategoryAttribute("Display")]
        [DescriptionAttribute("The number of days that should be displayed in the list at a time")]
        public int DaysToDisplay
        {
            get { return(mDaysToDisplay); }
            set 
			{
				if (value<1)
					mDaysToDisplay=1;
				else
					mDaysToDisplay=value; 
			}
        }

		[CategoryAttribute("Display")]
		[DescriptionAttribute("The number of hours ahead to display when Few Hours From Now is selected")]
		public int FewHoursFromNow
		{
			get { return(mFewHoursFromNow); }
			set 
			{
				if (value<1)
					mFewHoursFromNow=1;
				else
					mFewHoursFromNow=value; 
			}
		}

		[CategoryAttribute("Display")]
		[DescriptionAttribute("Default field to sort shows by when they are loaded")]
		public TVComparer.ESortBy DefaultSortField
		{
			get { return(mDefaultSortField); }
			set { mDefaultSortField=value; }
		}	

		[CategoryAttribute("Display")]
		[DescriptionAttribute("Default Sorting Mode")]
		public TVComparer.ESortMode DefaultSortMode
		{
			get { return(mDefaultSortMode); }
			set { mDefaultSortMode=value; }
		}			
		#endregion

		#region "XMLTV Settings"
		[CategoryAttribute("XMLTV")]
		[DescriptionAttribute("The location of the XMLTV.EXE program")]
		[EditorAttribute(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(UITypeEditor))]
		public string XMLTVEXELocation
		{
			get { return(mXMLTVEXELocation); }
			set { mXMLTVEXELocation=value; }
		}

		[CategoryAttribute("XMLTV")]
		[DescriptionAttribute("The configuration file XMLTV should use. This file is normally stored in a folder called .xmltv in the directory where xmltv itself is installed.")]
		[EditorAttribute(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(UITypeEditor))]
		public string XMLTVConfigurationFile
		{
			get { return(mXMLTVConfigLocation); }
			set { mXMLTVConfigLocation=value; }
		}

		[CategoryAttribute("XMLTV")]
		[DescriptionAttribute("Selects which country's grabber XMLTV should use (tv_grab_na for North America)")]
		public XMLTVGrabber XMLTVCountry
		{
			get { return(mGrabber); }
			set { mGrabber=value; }
		}

		[CategoryAttribute("XMLTV")]
		[DescriptionAttribute("The number of days to download listings for")]
		public int XMLTVDaysToDownload
		{
			get { return(mDaysToDownload); }
			set 
			{
				if (value<1)
					mDaysToDownload=1;
				else
					mDaysToDownload=value; 
			}
		}

		[CategoryAttribute("XMLTV")]
		[DescriptionAttribute("Name of the file XMLTV should write the listings to")]
		[EditorAttribute(typeof(FileNameSelector), typeof(UITypeEditor))]
		public string XMLTVOutputFile
		{
			get { return(mXMLTVOutputFile); }
			set 			
			{ 
				if (value=="")
					mXMLTVOutputFile="listings.xml";
				else
					mXMLTVOutputFile=value; 
			}
		}
		#endregion

		#region "Custom Show Highlighting"
		[XmlIgnore()] 
		[CategoryAttribute("Custom Show Highlighting")]
		[DescriptionAttribute("Color to highlight Movies in the listing")]
		public Color MovieColor
		{
			get { return(mMovieColor); }
			set { mMovieColor=value; }
		}
		[XmlIgnore()]
		[CategoryAttribute("Custom Show Highlighting")]
		[DescriptionAttribute("Color to highlight Children in the listing")]
		public Color ChildrenColor
		{
			get { return(mChildrenColor); }
			set { mChildrenColor=value; }
		}
		[XmlIgnore()]
		[CategoryAttribute("Custom Show Highlighting")]
		[DescriptionAttribute("Color to highlight Favorites in the listing")]
		public Color FavoritesColor
		{
			get { return(mFavoritesColor); }
			set { mFavoritesColor=value; }
		}
		[XmlIgnore()]
		[CategoryAttribute("Custom Show Highlighting")]
		[DescriptionAttribute("Color to highlight Talk in the listing")]
		public Color TalkColor
		{
			get { return(mTalkColor); }
			set { mTalkColor=value; }
		}
		[XmlIgnore()]
		[CategoryAttribute("Custom Show Highlighting")]
		[DescriptionAttribute("Color to highlight News in the listing")]
		public Color NewsColor
		{
			get { return(mNewsColor); }
			set { mNewsColor=value; }
		}
		[XmlIgnore()]
		[CategoryAttribute("Custom Show Highlighting")]
		[DescriptionAttribute("Color to highlight Sports in the listing")]
		public Color SportsColor
		{
			get { return(mSportsColor); }
			set { mSportsColor=value; }
		}
		[XmlIgnore()] 
		[CategoryAttribute("Custom Show Highlighting")]
		[DescriptionAttribute("Color to highlight Everything else in the listing")]
		public Color DefaultColor
		{
			get { return(mDefaultColor); }
			set { mDefaultColor=value; }
		}

		[XmlElement("MovieColor")]
		[Browsable(false)]
		public string XmlMovieColor
		{
			get
			{
				return this.SerializeColor(mMovieColor);
			}
			set
			{
				mMovieColor=this.DeserializeColor(value);
			}
		}

		[XmlElement("ChildrenColor")]
		[Browsable(false)]
		public string XmlChildrenColor
		{
			get
			{
				return this.SerializeColor(mChildrenColor);
			}
			set
			{
				mChildrenColor=this.DeserializeColor(value);
			}
		}

		[XmlElement("FavoritesColor")]
		[Browsable(false)]
		public string XmlFavoritesColor
		{
			get
			{
				return this.SerializeColor(mFavoritesColor);
			}
			set
			{
				mFavoritesColor=this.DeserializeColor(value);
			}
		}

		[XmlElement("TalkColor")]
		[Browsable(false)]
		public string XmlTalkColor
		{
			get
			{
				return this.SerializeColor(mTalkColor);
			}
			set
			{
				mTalkColor=this.DeserializeColor(value);
			}
		}

		[XmlElement("NewsColor")]
		[Browsable(false)]
		public string XmlNewsColor
		{
			get
			{
				return this.SerializeColor(mNewsColor);
			}
			set
			{
				mNewsColor=this.DeserializeColor(value);
			}
		}

		[XmlElement("SportsColor")]
		[Browsable(false)]
		public string XmlSportsColor
		{
			get
			{
				return this.SerializeColor(mSportsColor);
			}
			set
			{
				mSportsColor=this.DeserializeColor(value);
			}
		}

		[XmlElement("DefaultColor")]
		[Browsable(false)]
		public string XmlDefaultColor
		{
			get
			{
				return this.SerializeColor(mDefaultColor);
			}
			set
			{
				mDefaultColor=this.DeserializeColor(value);
			}
		}

		[CategoryAttribute("Custom Show Highlighting")]
		[DescriptionAttribute("Collection of custom filters used to apply different colors to shows in the list/grid view.")]
//		[EditorAttribute(typeof(System.ComponentModel.Design.CollectionEditor), typeof(UITypeEditor))]
		public CustomHighlightCollection CustomHighlights
		{
			get { return(mUserHighlights); }
			set { mUserHighlights=value; }
		}
		#endregion

		#region "Notifications"
		[CategoryAttribute("Notifications")]
		[DescriptionAttribute("Wave file to play when a reminder pops up.")]
		[EditorAttribute(typeof(WAVFileNameSelector), typeof(UITypeEditor))]
		public string NotificationSound
		{
			get { return(mNotificationSound); }
			set { mNotificationSound=value; }
		}

		[CategoryAttribute("Notifications")]
		[DescriptionAttribute("If true then the selected wave file will be played everytime a show reminder pops up.")]
		public bool PlaySound
		{
			get { return(mNotificationPlaySound); }
			set { mNotificationPlaySound=value; }
		}
		#endregion

		[CategoryAttribute("Channel List")]
		[DescriptionAttribute("The default Channel List to display for the TV Listings")]
		public string defaultChannelList
		{
			get { return(mChannelList); }
			set { mChannelList=value; }
		}

		#endregion

		#region "Color Serialization Methods"
		public string SerializeColor(Color color)
		{
			if( color.IsNamedColor )
				return string.Format("{0}:{1}", 
					ColorFormat.NamedColor, color.Name);
			else
				return string.Format("{0}:{1}:{2}:{3}:{4}", 
					ColorFormat.ARGBColor, 
					color.A, color.R, color.G, color.B);
		}

		public Color DeserializeColor(string color)
		{
			byte a, r, g, b;

			string [] pieces = color.Split(new char[] {':'});
		
			ColorFormat colorType = (ColorFormat) 
				Enum.Parse(typeof(ColorFormat), pieces[0], true);

			switch(colorType)
			{
				case ColorFormat.NamedColor:
					return Color.FromName(pieces[1]);

				case ColorFormat.ARGBColor:
					a = byte.Parse(pieces[1]);
					r = byte.Parse(pieces[2]);
					g = byte.Parse(pieces[3]);
					b = byte.Parse(pieces[4]);
			
					return Color.FromArgb(a, r, g, b);
			}
			return Color.Empty;
		}
		#endregion
	}
}
