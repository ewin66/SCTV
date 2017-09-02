using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using DirectX.Capture;
using DShowNET;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace SCTVTelevision
{
	/// <summary>
	/// Summary description for tvViewer.
	/// </summary>
	public class TVViewer : System.Windows.Forms.Form
	{
		#region variables
		
		//		public delegate void DataReceivedHandler(object sender, DataReceivedEventArgs e);
		
		public System.Windows.Forms.Panel pnlMedia;
		public System.Windows.Forms.Label lblChannel;
		private System.Windows.Forms.Timer channelDisplayTimer;
		private System.ComponentModel.IContainer components;
		ArrayList keyStrokeTracker = new ArrayList();
		public Capture capture = null;
		private Filters filters;
		SortedList keyList = new SortedList();
		SortedList macroList = new SortedList();
		public static bool showInfoVisible = false;
		public TVProgramme currentShowInfo;
		public DateTime currentShowInfoStartTime = DateTime.Now;
		public string currentShowInfoChannel;
		public int previousChannel = 4;

		private System.Windows.Forms.Timer changeChannelTimer;
		public System.Windows.Forms.Label lblShowTitle;
		private System.Windows.Forms.Timer showTitleTimer;
		private System.Windows.Forms.Timer clock;
		private System.Windows.Forms.RichTextBox showInfo;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label lblNotify;
		private System.Windows.Forms.Label lblPlay;
		private System.Windows.Forms.Label lblCancel;
		private System.Windows.Forms.RichTextBox conflictInfo;
		private System.Windows.Forms.Label txtClock;
		private IBasicAudio	basicAudio;
		private int	savedVolume = -5000;
		private System.Windows.Forms.Label lblFavorite;
		private System.Windows.Forms.Panel pnlIcons;
		private System.Windows.Forms.RichTextBox txtMessage;
		private System.Windows.Forms.Panel pnlInformation;
		private System.Windows.Forms.ListView lvFutureAirDates;
		private System.Windows.Forms.ColumnHeader columnHeader15;
		private System.Windows.Forms.ColumnHeader record;
		private System.Windows.Forms.ImageList buttonImageList;
		public System.Windows.Forms.Timer scanChannelTimer;
		private IGraphBuilder graphBuilder;
		public string channelScanDirection = "";

		#endregion

		#region public TVViewer()
		public TVViewer()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
		}
		#endregion

		#region private void mediaViewer_Load(object sender, System.EventArgs e)
		private void mediaViewer_Load(object sender, System.EventArgs e)
		{
			populateMacros();
			populateKeyStrokes();
			txtClock.Text=GetTime();
			GetInterfaces();
		}
		#endregion

		#region TV Functions
		#region private void turnOnTV()
		public void turnOnTV()
		{
			if(capture == null)
			{
				//try to initiate a capture card
				try
				{
					filters = new Filters();
				}
				catch(Exception exc)
				{
					EventLog.WriteEntry("sctv","Filters exception: "+ exc.ToString());
				}

				try
				{
					this.Cursor = Cursors.WaitCursor;
					//					ProgramConfiguration=(ConfigurationData)ConfigurationManager.Read("XmlSerializer");
					if(Form1.ProgramConfiguration.videoDevice.Length > 0)
						capture = new Capture(  new Filter( Form1.ProgramConfiguration.videoDevice), new Filter( Form1.ProgramConfiguration.audioDevice) );
					else
						capture = new Capture( filters.VideoInputDevices[0], filters.AudioInputDevices[0] ); 
					
				}
				catch(Exception e)
				{
					capture = new Capture( filters.VideoInputDevices[0], filters.AudioInputDevices[0] ); 
				}
			
				//			if(ProgramConfiguration.videoDevice == null)//there is no TV object
				//			{
				//				EventLog.WriteEntry("sctv","capture is null");
				//				cleanUp();
				//				try
				//				{
				//#if DEBUG
				//					ProgramConfiguration=(ConfigurationData)ConfigurationManager.Read("XmlSerializer");
				//					//				capture = new Capture( filters.VideoInputDevices[1], filters.AudioInputDevices[0] ); 
				//					if(ProgramConfiguration.videoDevice.Length > 0)
				//						capture = new Capture(  new Filter( ProgramConfiguration.videoDevice), new Filter( ProgramConfiguration.audioDevice) );
				//					else
				//						capture = new Capture( filters.VideoInputDevices[0], filters.AudioInputDevices[0] ); 
				//					capture.CaptureComplete += new EventHandler( OnCaptureComplete );
				//					
				//#endif
				//				}
				//				catch(Exception e)
				//				{
				//					//				capture = new Capture( filters.VideoInputDevices[0], filters.AudioInputDevices[0] );
				//					EventLog.WriteEntry("sctv",e.ToString());
				//				}
				//
				try{capture.VideoCompressor = new Filter( Form1.ProgramConfiguration.videoCompressor );}
				catch(Exception ex)
				{
					EventLog.WriteEntry("sctv","VideoCompressor "+ ex.ToString());
				}
			
//				try{capture.VideoCompressor = filters.VideoCompressors[0];}
//				catch{}
//				try{capture.AudioCompressor = filters.AudioCompressors[0];}
//				catch{}
				try{capture.AudioCompressor = new Filter( Form1.ProgramConfiguration.audioCompressor );}
				catch(Exception ex)
				{
					EventLog.WriteEntry("sctv","AudioCompressor "+ ex.ToString());
				}
//				try{capture.FrameRate = Convert.ToDouble(Form1.ProgramConfiguration.frameRate);}
//				catch{}
				//			try{capture.FrameSize = ProgramConfiguration.frameSize;}catch{}
				//			try{capture.AudioChannels = Convert.toshortProgramConfiguration.audioChannels;}catch{}
				//			try{capture.AudioSampleSize = (short)ProgramConfiguration.audioSampleSize;}catch{}
//				try{capture.AudioSamplingRate = Convert.ToInt32(Form1.ProgramConfiguration.audioSamplingRate);}
//				catch{}
				//			ArrayList pages = new ArrayList();
				//			pages = ProgramConfiguration.pages.Split(new Char[","]);
				//			ArrayList pages = ProgramConfiguratio;

				//			foreach ( PropertyPage p in capture.PropertyPages )
				//			{
				//				if ( p.SupportsPersisting )
				//				{
				//					p.State = (byte[]) pages[0];
				//					pages.RemoveAt( 0 );
				//				}
				//			}
				//			}
				//			else
				//			{
				//				EventLog.WriteEntry("sctv","capture is not null - "+ capture.ToString());
				//			}
				capture.CaptureComplete += new EventHandler( OnCaptureComplete );
				capture.PreviewWindow = pnlMedia;
				pnlMedia.Select();
//				this.BringToFront();
//				this.Show();
//				this.Select();
                //Form1.GUIState = guiState.TV;
			}
		}
		#endregion
		
		#region Channel functions
		#region private string formatChannels(string channelInfo)
		private string formatChannels(string channelInfo)
		{
			channelInfo = channelInfo.Substring(1,channelInfo.IndexOf(".")-1);//channelInfo is now the channel number and name
			string channel = "";
			char[] charArray = channelInfo.ToCharArray();
			for(int x=0;x<charArray.Length;x++)
			{
				if(System.Text.RegularExpressions.Regex.IsMatch(charArray[x].ToString(),"[0-9]"))//this is a number
				{
					channel += charArray[x];
				}
				else
					x = charArray.Length;
			}
			string station = System.Text.RegularExpressions.Regex.Replace(channelInfo,channel,"");
			return channel +"-"+ station;
		}
		#endregion

		#region public void displayChannel()
		public void displayChannel()
		{
			lblChannel.Text = capture.Tuner.Channel.ToString();
			displayShowTitle(Form1.ListingsOrganizer.whatsPlaying(capture.Tuner.Channel.ToString()));
			pnlInformation.Visible = false;
		}
		#endregion

		public void changeChannel(int channel)
		{
			turnOnTV();
			
			if(channel < Form1.ListingsOrganizer.channelMin)
				channel = Form1.ListingsOrganizer.channelMax;
			else if(channel > Form1.ListingsOrganizer.channelMax)
				channel = Form1.ListingsOrganizer.channelMin;
			previousChannel = capture.Tuner.Channel;
			capture.Tuner.Channel = channel;
			displayChannel();
		}
		#endregion

		#region "Watch" button to watch currently selected channel
		private void watch(object sender, System.EventArgs e)
		{
			//			keyStrokeTracker.Add(Convert.ToInt32(MiscStuffRichText.channel.Substring(0,MiscStuffRichText.channel.IndexOf(" "))));
			executeKeyStrokes("channel");
			//			changeChannel(Convert.ToInt32(MiscStuffRichText.channel.Substring(0,MiscStuffRichText.channel.IndexOf(" "))));
		}
		#endregion

		private void TVViewer_VisibleChanged(object sender, System.EventArgs e)
		{
			if(this.Visible)
			{
				Cursor.Hide();
                //Form1.GUIState = SCTV.guiState.TV;
			}
			else
			{
				Cursor.Show();
			}
		}

		#region private void TVViewer_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		private void TVViewer_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			changeChannelTimer.Enabled = true;
			changeChannelTimer.Start();
			keyStrokeTracker.Add(e.KeyValue);
			string tempKeys = "";
			string tempChannelKeys = "";
			if(keyStrokeTracker.Count > 0)
			{
				foreach(int intChannel in keyStrokeTracker)
				{
					tempKeys += intChannel.ToString() +",";
					if(keyList.Contains(intChannel.ToString()))
					{					
						tempChannelKeys += keyList.GetByIndex(keyList.IndexOfKey(intChannel.ToString())).ToString();
						lblChannel.Text = tempChannelKeys;
					}
				}
				tempKeys = tempKeys.Remove(tempKeys.LastIndexOf(","),1);//removes trailing comma	
			}
			if(macroList.Contains(tempKeys))
			{
				keyStrokeTracker.Clear();
				string macroName = macroList.GetByIndex(macroList.IndexOfKey(tempKeys)).ToString();
				EventLog.WriteEntry("sctv","executing macro "+ macroName);
				executeKeyStrokes(macroName);
			}
			else
				EventLog.WriteEntry("sctv","didn't find TVViewer macro for: "+ tempKeys);

			EventLog.WriteEntry("sctv","KeyCode from TVViewer keyup: "+ e.KeyCode);
			EventLog.WriteEntry("sctv","KeyValue from TVViewer keyup: "+ e.KeyValue);
		}
		#endregion

		#region private void TVViewer_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)// handles mouse buttons as if they were keystrokes - in case "toggle" has been hit
		private void TVViewer_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)// handles mouse buttons as if they were keystrokes - in case "toggle" has been hit
		{
			EventLog.WriteEntry("sctv","found mouseUp in TVViewer form "+ e.Button.ToString());
//			switch(e.Button.ToString())
//			{
//				case "Middle":// = "Enter" key
//					executeKeyStrokes("Enter");
//					break;
//				case "Left":
//					executeKeyStrokes("previousChannel");
//					break;
//				case "Right":
//					executeKeyStrokes("play");
//					break;
//			}
		}
		#endregion

		#region private void OnCaptureComplete(object sender, EventArgs e)
		private void OnCaptureComplete(object sender, EventArgs e)
		{
			// Demonstrate the Capture.CaptureComplete event.
			EventLog.WriteEntry("sctv", "Capture complete." );
			displayChannel();
		}
		#endregion
		#endregion

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(TVViewer));
			this.pnlMedia = new System.Windows.Forms.Panel();
			this.pnlInformation = new System.Windows.Forms.Panel();
			this.showInfo = new System.Windows.Forms.RichTextBox();
			this.txtClock = new System.Windows.Forms.Label();
			this.pnlIcons = new System.Windows.Forms.Panel();
			this.lblFavorite = new System.Windows.Forms.Label();
			this.txtMessage = new System.Windows.Forms.RichTextBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.conflictInfo = new System.Windows.Forms.RichTextBox();
			this.lblCancel = new System.Windows.Forms.Label();
			this.lblPlay = new System.Windows.Forms.Label();
			this.lblNotify = new System.Windows.Forms.Label();
			this.lvFutureAirDates = new System.Windows.Forms.ListView();
			this.record = new System.Windows.Forms.ColumnHeader();
			this.columnHeader15 = new System.Windows.Forms.ColumnHeader();
			this.buttonImageList = new System.Windows.Forms.ImageList(this.components);
			this.lblShowTitle = new System.Windows.Forms.Label();
			this.lblChannel = new System.Windows.Forms.Label();
			this.channelDisplayTimer = new System.Windows.Forms.Timer(this.components);
			this.changeChannelTimer = new System.Windows.Forms.Timer(this.components);
			this.showTitleTimer = new System.Windows.Forms.Timer(this.components);
			this.clock = new System.Windows.Forms.Timer(this.components);
			this.scanChannelTimer = new System.Windows.Forms.Timer(this.components);
			this.pnlMedia.SuspendLayout();
			this.pnlInformation.SuspendLayout();
			this.pnlIcons.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// pnlMedia
			// 
			this.pnlMedia.BackColor = System.Drawing.Color.Black;
			this.pnlMedia.Controls.Add(this.pnlInformation);
			this.pnlMedia.Controls.Add(this.lblShowTitle);
			this.pnlMedia.Controls.Add(this.lblChannel);
			this.pnlMedia.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlMedia.Location = new System.Drawing.Point(0, 0);
			this.pnlMedia.Name = "pnlMedia";
			this.pnlMedia.Size = new System.Drawing.Size(571, 299);
			this.pnlMedia.TabIndex = 0;
			// 
			// pnlInformation
			// 
			this.pnlInformation.Controls.Add(this.showInfo);
			this.pnlInformation.Controls.Add(this.txtClock);
			this.pnlInformation.Controls.Add(this.pnlIcons);
			this.pnlInformation.Controls.Add(this.txtMessage);
			this.pnlInformation.Controls.Add(this.panel1);
			this.pnlInformation.Controls.Add(this.lvFutureAirDates);
			this.pnlInformation.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnlInformation.Location = new System.Drawing.Point(0, 195);
			this.pnlInformation.Name = "pnlInformation";
			this.pnlInformation.Size = new System.Drawing.Size(571, 104);
			this.pnlInformation.TabIndex = 9;
			this.pnlInformation.Visible = false;
			this.pnlInformation.VisibleChanged += new System.EventHandler(this.pnlInformation_VisibleChanged);
			// 
			// showInfo
			// 
			this.showInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.showInfo.BackColor = System.Drawing.Color.LightSteelBlue;
			this.showInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.showInfo.CausesValidation = false;
			this.showInfo.Font = new System.Drawing.Font("Comic Sans MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.showInfo.ForeColor = System.Drawing.Color.Navy;
			this.showInfo.Location = new System.Drawing.Point(0, 32);
			this.showInfo.Name = "showInfo";
			this.showInfo.ReadOnly = true;
			this.showInfo.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
			this.showInfo.Size = new System.Drawing.Size(642, 88);
			this.showInfo.TabIndex = 5;
			this.showInfo.TabStop = false;
			this.showInfo.Text = "";
			this.showInfo.Visible = false;
			this.showInfo.VisibleChanged += new System.EventHandler(this.showInfo_VisibleChanged);
			// 
			// txtClock
			// 
			this.txtClock.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.txtClock.BackColor = System.Drawing.Color.Navy;
			this.txtClock.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.txtClock.Font = new System.Drawing.Font("Comic Sans MS", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.txtClock.ForeColor = System.Drawing.Color.LightSteelBlue;
			this.txtClock.Location = new System.Drawing.Point(0, 0);
			this.txtClock.Name = "txtClock";
			this.txtClock.Size = new System.Drawing.Size(120, 33);
			this.txtClock.TabIndex = 4;
			this.txtClock.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// pnlIcons
			// 
			this.pnlIcons.Controls.Add(this.lblFavorite);
			this.pnlIcons.Location = new System.Drawing.Point(128, 0);
			this.pnlIcons.Name = "pnlIcons";
			this.pnlIcons.Size = new System.Drawing.Size(192, 32);
			this.pnlIcons.TabIndex = 9;
			this.pnlIcons.Visible = false;
			// 
			// lblFavorite
			// 
			this.lblFavorite.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lblFavorite.Image = ((System.Drawing.Image)(resources.GetObject("lblFavorite.Image")));
			this.lblFavorite.Location = new System.Drawing.Point(0, 0);
			this.lblFavorite.Name = "lblFavorite";
			this.lblFavorite.Size = new System.Drawing.Size(32, 32);
			this.lblFavorite.TabIndex = 7;
			this.lblFavorite.Visible = false;
			// 
			// txtMessage
			// 
			this.txtMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.txtMessage.BackColor = System.Drawing.Color.LightSteelBlue;
			this.txtMessage.Font = new System.Drawing.Font("Comic Sans MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.txtMessage.ForeColor = System.Drawing.Color.Navy;
			this.txtMessage.Location = new System.Drawing.Point(128, 0);
			this.txtMessage.Name = "txtMessage";
			this.txtMessage.Size = new System.Drawing.Size(440, 32);
			this.txtMessage.TabIndex = 10;
			this.txtMessage.Text = "";
			this.txtMessage.Visible = false;
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.panel1.Controls.Add(this.conflictInfo);
			this.panel1.Controls.Add(this.lblCancel);
			this.panel1.Controls.Add(this.lblPlay);
			this.panel1.Controls.Add(this.lblNotify);
			this.panel1.Location = new System.Drawing.Point(24, 40);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(520, 56);
			this.panel1.TabIndex = 6;
			this.panel1.Visible = false;
			// 
			// conflictInfo
			// 
			this.conflictInfo.BackColor = System.Drawing.Color.LightSteelBlue;
			this.conflictInfo.Font = new System.Drawing.Font("Comic Sans MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.conflictInfo.ForeColor = System.Drawing.Color.Navy;
			this.conflictInfo.Location = new System.Drawing.Point(16, 0);
			this.conflictInfo.Name = "conflictInfo";
			this.conflictInfo.Size = new System.Drawing.Size(488, 32);
			this.conflictInfo.TabIndex = 3;
			this.conflictInfo.Text = "";
			// 
			// lblCancel
			// 
			this.lblCancel.BackColor = System.Drawing.Color.Navy;
			this.lblCancel.Font = new System.Drawing.Font("Comic Sans MS", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblCancel.ForeColor = System.Drawing.Color.LightSteelBlue;
			this.lblCancel.Location = new System.Drawing.Point(368, 32);
			this.lblCancel.Name = "lblCancel";
			this.lblCancel.TabIndex = 2;
			this.lblCancel.Text = "Cancel";
			this.lblCancel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lblPlay
			// 
			this.lblPlay.BackColor = System.Drawing.Color.Navy;
			this.lblPlay.Font = new System.Drawing.Font("Comic Sans MS", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblPlay.ForeColor = System.Drawing.Color.LightSteelBlue;
			this.lblPlay.Location = new System.Drawing.Point(208, 32);
			this.lblPlay.Name = "lblPlay";
			this.lblPlay.TabIndex = 1;
			this.lblPlay.Text = "Play";
			this.lblPlay.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lblNotify
			// 
			this.lblNotify.BackColor = System.Drawing.Color.Navy;
			this.lblNotify.Font = new System.Drawing.Font("Comic Sans MS", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblNotify.ForeColor = System.Drawing.Color.LightSteelBlue;
			this.lblNotify.Location = new System.Drawing.Point(48, 32);
			this.lblNotify.Name = "lblNotify";
			this.lblNotify.TabIndex = 0;
			this.lblNotify.Text = "Notify";
			this.lblNotify.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lvFutureAirDates
			// 
			this.lvFutureAirDates.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lvFutureAirDates.BackColor = System.Drawing.Color.LightSteelBlue;
			this.lvFutureAirDates.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																							   this.record,
																							   this.columnHeader15});
			this.lvFutureAirDates.Font = new System.Drawing.Font("Comic Sans MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lvFutureAirDates.ForeColor = System.Drawing.Color.Navy;
			this.lvFutureAirDates.FullRowSelect = true;
			this.lvFutureAirDates.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.lvFutureAirDates.HideSelection = false;
			this.lvFutureAirDates.Location = new System.Drawing.Point(0, 32);
			this.lvFutureAirDates.MultiSelect = false;
			this.lvFutureAirDates.Name = "lvFutureAirDates";
			this.lvFutureAirDates.Size = new System.Drawing.Size(568, 72);
			this.lvFutureAirDates.SmallImageList = this.buttonImageList;
			this.lvFutureAirDates.TabIndex = 10;
			this.lvFutureAirDates.View = System.Windows.Forms.View.Details;
			this.lvFutureAirDates.Visible = false;
			this.lvFutureAirDates.VisibleChanged += new System.EventHandler(this.lvFutureAirDates_VisibleChanged);
			// 
			// record
			// 
			this.record.Width = 34;
			// 
			// columnHeader15
			// 
			this.columnHeader15.Text = "Show Times";
			this.columnHeader15.Width = 512;
			// 
			// buttonImageList
			// 
			this.buttonImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			this.buttonImageList.ImageSize = new System.Drawing.Size(32, 32);
			this.buttonImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("buttonImageList.ImageStream")));
			this.buttonImageList.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// lblShowTitle
			// 
			this.lblShowTitle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblShowTitle.BackColor = System.Drawing.Color.LightSteelBlue;
			this.lblShowTitle.Font = new System.Drawing.Font("Comic Sans MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblShowTitle.ForeColor = System.Drawing.Color.Navy;
			this.lblShowTitle.Location = new System.Drawing.Point(331, 8);
			this.lblShowTitle.Name = "lblShowTitle";
			this.lblShowTitle.Size = new System.Drawing.Size(232, 48);
			this.lblShowTitle.TabIndex = 1;
			this.lblShowTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lblShowTitle.Visible = false;
			this.lblShowTitle.TextChanged += new System.EventHandler(this.lblShowTitle_TextChanged);
			// 
			// lblChannel
			// 
			this.lblChannel.BackColor = System.Drawing.Color.LightSteelBlue;
			this.lblChannel.Font = new System.Drawing.Font("Comic Sans MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblChannel.ForeColor = System.Drawing.Color.Navy;
			this.lblChannel.Location = new System.Drawing.Point(8, 8);
			this.lblChannel.Name = "lblChannel";
			this.lblChannel.Size = new System.Drawing.Size(48, 32);
			this.lblChannel.TabIndex = 0;
			this.lblChannel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lblChannel.Visible = false;
			this.lblChannel.TextChanged += new System.EventHandler(this.lblChannel_TextChanged);
			// 
			// channelDisplayTimer
			// 
			this.channelDisplayTimer.Enabled = true;
			this.channelDisplayTimer.Interval = 3000;
			this.channelDisplayTimer.Tick += new System.EventHandler(this.channelDisplayTimer_Tick);
			// 
			// changeChannelTimer
			// 
			this.changeChannelTimer.Interval = 3000;
			this.changeChannelTimer.Tick += new System.EventHandler(this.changeChannelTimer_Tick);
			// 
			// showTitleTimer
			// 
			this.showTitleTimer.Enabled = true;
			this.showTitleTimer.Interval = 3000;
			this.showTitleTimer.Tick += new System.EventHandler(this.showTitleTimer_Tick);
			// 
			// clock
			// 
			this.clock.Enabled = true;
			this.clock.Interval = 1000;
			this.clock.Tick += new System.EventHandler(this.clock_Tick);
			// 
			// scanChannelTimer
			// 
			this.scanChannelTimer.Interval = 3000;
			this.scanChannelTimer.Tick += new System.EventHandler(this.scanChannelTimer_Tick);
			// 
			// TVViewer
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.Color.Black;
			this.ClientSize = new System.Drawing.Size(571, 299);
			this.ControlBox = false;
			this.Controls.Add(this.pnlMedia);
			this.KeyPreview = true;
			this.Name = "TVViewer";
			this.TransparencyKey = System.Drawing.Color.Fuchsia;
			this.Closing += new System.ComponentModel.CancelEventHandler(this.mediaViewer_Closing);
			this.Load += new System.EventHandler(this.mediaViewer_Load);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TVViewer_MouseUp);
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TVViewer_KeyUp);
			this.VisibleChanged += new System.EventHandler(this.TVViewer_VisibleChanged);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.TVViewer_Paint);
			this.pnlMedia.ResumeLayout(false);
			this.pnlInformation.ResumeLayout(false);
			this.pnlIcons.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		#region private void lblChannel_TextChanged(object sender, System.EventArgs e)
		private void lblChannel_TextChanged(object sender, System.EventArgs e)
		{
			channelDisplayTimer.Stop();
			lblChannel.Visible = true;
			channelDisplayTimer.Start();
		}
		#endregion

		#region private void channelDisplayTimer_Tick(object sender, System.EventArgs e)
		private void channelDisplayTimer_Tick(object sender, System.EventArgs e)
		{
			lblChannel.Visible = false;
		}
		#endregion

		#region private void populateMacros()  //these run immediatly when found
		private void populateMacros()
		{
			//comma delimited list of (string) KeyValue's , names

			//media controls
			macroList.Add("17,16,66","Rewind");  //rewind
			macroList.Add("17,83","Stop");  //Stop
			macroList.Add("17,16,70","FF");  //FF
			macroList.Add("17,70","Next");  //Next
			macroList.Add("17,66","Previous");  //Previous
			macroList.Add("17,80","Play/Pause");  //Play/Pause
			macroList.Add("17,82","Record");  //Record
			//			macroList.Add("","Mute");  //mute
		
			//main menu items
			macroList.Add("17,16,18,36","Option");  //home key
			macroList.Add("33","ChannelUp");  //Channel up
			macroList.Add("32","Space");  //Space Bar - channel up
			macroList.Add("8","Back");  //BackSpace - channel down
			macroList.Add("34","ChannelDown");  //Channel Down
			macroList.Add("17,16,18,50","tvKey");  //tv key
			macroList.Add("17,16,18,72","fmKey");  //fm key
			macroList.Add("17,16,18,51","musicKey");  //music key
			macroList.Add("17,16,18,52","pictureKey");  //picture key
			macroList.Add("17,16,18,53","videoKey");  //video key
			macroList.Add("17,16,18,49","dvdKey");  //dvd key
			macroList.Add("17,16,18,70","shuffleKey");  //shuffle key
			macroList.Add("17,16,18,66","repeatKey");  //repeat key
			macroList.Add("13","Enter");  //Enter
			macroList.Add("166","previousChannel");  //Browser Back
			macroList.Add("37","leftArrow");  //left Arrow - moves through show info
			macroList.Add("38","upArrow");  //up arrow - moves through show info
			macroList.Add("39","rightArrow");  //right Arrow - moves through show info
			macroList.Add("40","downArrow");  //down Arrow - moves through show info
			macroList.Add("9","Play");  //Tab - plays show displayed in showInfo
			macroList.Add("27","tvKey");  //Escape - toggles TV visibility
			macroList.Add("175","volumeChange");  //volume up - make sure it's not muted
			macroList.Add("174","volumeChange");  //volume down - make sure it's not muted

			//generic Key settings for main menu
			macroList.Add("113","tvKey");  //F2 - tv key
			macroList.Add("114","fmKey");  //F3 - fm key
			macroList.Add("116","musicKey");  //F5 - music key
			macroList.Add("117","pictureKey");  //F6 - picture key
			macroList.Add("118","videoKey");  //F7 - video key
			macroList.Add("119","dvdKey");  //F8 - dvd key
		}
		#endregion

		#region private void populateKeyStrokes()
		private void populateKeyStrokes()
		{
			//TV channel controls
			keyList.Add("49","1");  //1
			keyList.Add("50","2");  //2
			keyList.Add("51","3");  //3
			keyList.Add("52","4");  //4
			keyList.Add("53","5");  //5
			keyList.Add("54","6");  //6
			keyList.Add("55","7");  //7
			keyList.Add("56","8");  //8
			keyList.Add("57","9");  //9
			keyList.Add("48","0");  //0
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
				changeChannel(Convert.ToInt32(tempKeys));
			}
			catch(Exception ex)
			{
				EventLog.WriteEntry("sctv",ex.ToString());
			}
		}
		#endregion

		#region private void executeKeyStrokes(string macroName)//executes macros immediately
		public void executeKeyStrokes(string macroName)
		{
			TVProgramme tempShow = new TVProgramme();
			favoritesArrayList favoritesList;
			favorites fn;
			keyStrokeTracker.Clear();
			switch(macroName)
			{
				case "Rewind":
					break;
				case "FF":
					break;
				case "Record":
					
//						try{capture.FrameRate = Convert.ToDouble(Form1.ProgramConfiguration.frameRate);}
//						catch{}
//						try{capture.AudioSamplingRate = Convert.ToInt32(Form1.ProgramConfiguration.audioSamplingRate);}
//						catch{}
						
					try
					{
//						capture.FrameRate = 29.997; // NTSC
						capture.FrameRate = 15;
					}
					catch(Exception ex)
					{
						EventLog.WriteEntry("sctv","**** RECORDING FrameRate "+ ex.ToString());
					}
					try
					{
						capture.FrameSize = new Size( 160, 120 );   // 640x480
//						capture.FrameSize = new Size( 320, 240 );   // 640x480
//						capture.FrameSize = new Size( 640, 480 );   // 640x480
					}
					catch(Exception ex)
					{
						EventLog.WriteEntry("sctv","**** RECORDING FrameSize "+ ex.ToString());
					}
					try
					{
//						capture.AudioSamplingRate = 11025;
						capture.AudioSamplingRate = 44100;          // 44.1 kHz
					}
					catch(Exception ex)
					{
						EventLog.WriteEntry("sctv","**** RECORDING AudioSamplingRate "+ ex.ToString());
					}
					try
					{
						capture.AudioSampleSize = 16;               // 16-bit
					}
					catch(Exception ex)
					{
						EventLog.WriteEntry("sctv","**** RECORDING AudioSampleSize "+ ex.ToString());
					}
					try
					{
						capture.AudioChannels = 2;                  // Stereo
					}
					catch(Exception ex)
					{
						EventLog.WriteEntry("sctv","**** RECORDING AudioChannels "+ ex.ToString());
					}
					
					try
					{
						capture.Filename = "h:\\testing_"+ System.DateTime.Now.ToLongDateString() +".avi";
						//						capture.Filename = "h:\\testing.avi";
						capture.Start();
					}
					catch(Exception ex)
					{
						EventLog.WriteEntry("sctv","**** RECORDING start "+ ex.ToString());
					}
					
					break;
				case "Stop"://remove from favorites
					favoritesList=new favoritesArrayList();
					if(showInfo.Visible)//remove the currentShowInfo show
					{
						fn=new favorites(currentShowInfo);
						favoritesList.Add(fn);
						EventLog.WriteEntry("sctv","------------  Removing "+ fn.title);
						Form1.favoriteOrganizer.Remove(favoritesList);
						lblFavorite.Hide();
					}
					else//remove the current show being watched
					{
						capture.Stop();
//						fn=new favorites(Form1.ListingsOrganizer.whatsPlaying(this.capture.Tuner.Channel.ToString()));
//						favoritesList.Add(fn);
//						Form1.favoriteOrganizer.Remove(favoritesList);
					}
					break;
				case "Mute":
					//					mute();
					break;
				case "Next":
					currentShowInfo = Form1.ListingsOrganizer.whatsPlaying(this.capture.Tuner.Channel.ToString());
					currentShowInfoStartTime = currentShowInfo.StartTime;
					displayFutureAirDates(currentShowInfo);
					break;
				case "Previous":
					
					break;
				case "Play/Pause"://add to favorites
					favoritesList=new favoritesArrayList();
					if(showInfo.Visible)//add the currentShowInfo show
					{
						fn=new favorites(currentShowInfo);
						favoritesList.Add(fn);
						Form1.favoriteOrganizer.Add(favoritesList);
						lblFavorite.Show();
					}
					else//add the current show being watched
					{
						fn=new favorites(Form1.ListingsOrganizer.whatsPlaying(this.capture.Tuner.Channel.ToString()));
						favoritesList.Add(fn);
						Form1.favoriteOrganizer.Add(favoritesList);
					}
//					favoritesArrayList favoritesList=new favoritesArrayList();
//					favorites fn;
//					if(showInfo.Visible)//add the currentShowInfo show
//					{
//						fn=new favorites(currentShowInfo);
//						favoritesList.Add(fn);
//						Form1.favoriteOrganizer.Add(favoritesList);
//					}
//					else//add the current show being watched
//					{
//						fn=new favorites(Form1.ListingsOrganizer.whatsPlaying(this.capture.Tuner.Channel.ToString()));
//						favoritesList.Add(fn);
//						Form1.favoriteOrganizer.Add(favoritesList);
//					}
					break;
				case "ChannelUp":
					changeChannel(capture.Tuner.Channel + 1);
					break;
				case "ChannelDown":
					changeChannel(capture.Tuner.Channel - 1);
					break;
				case "channel":
					changeChannel(Convert.ToInt32(keyStrokeTracker[0]));
					break;
				case "tvKey":
//					this.Visible = false;
//					cleanUp();
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
//					if(panel1.Visible)//choose selected option on panel1
//					{
//						if(lblNotify.BackColor==Color.Red)//add to notification list
//						{
//							ShowNotificationArrayList list=new ShowNotificationArrayList();
//							ShowNotification sn;
//							sn=new ShowNotification(currentShowInfo);
//							list.Add(sn);
//							Form1.NotificationOrganizer.Add(list);
//						}
//						else if(lblPlay.BackColor==Color.Red)//add to play list
//						{
//							playScheduleArrayList list=new playScheduleArrayList();
//							playSchedule ps;
//							ps=new playSchedule(currentShowInfo);
//							list.Add(ps);
//							Form1.pScheduleOrganizer.Add(list);
//						}
//						panel1.Visible=false;
//					}
//					else//toggle showInfo
//					{
						panel1.Visible=false;
						currentShowInfo = Form1.ListingsOrganizer.whatsPlaying(this.capture.Tuner.Channel.ToString());
						currentShowInfoStartTime = currentShowInfo.StartTime;
						displayShowInfo(currentShowInfo);
//					}
					break;
				case "Option":
					cleanUp();
					break;
				case "Space":
					changeChannel(capture.Tuner.Channel + 1);
					break;
				case "Back":
					changeChannel(capture.Tuner.Channel - 1);
					break;
				case "previousChannel":
					changeChannel(previousChannel);
					break;
				case "leftArrow":
					if(showInfo.Visible)//display previous show info
					{
						tempShow = Form1.ListingsOrganizer.whatsPlaying(currentShowInfoChannel,currentShowInfo.StartTime,"StopTime");
						changeShowInfo(tempShow);
						currentShowInfoStartTime = tempShow.StartTime;
					}

					if(panel1.Visible)//go through the options on panel1
					{
						if(lblNotify.BackColor == Color.Red)//the notify is selected
						{
							lblPlay.BackColor = Color.Navy;
							lblNotify.BackColor = Color.Navy;
							lblCancel.BackColor = Color.Red;
						}
						else if(lblPlay.BackColor == Color.Red)//the play button is selected
						{
							lblPlay.BackColor = Color.Navy;
							lblNotify.BackColor = Color.Red;
							lblCancel.BackColor = Color.Navy;
						}
						else if(lblCancel.BackColor == Color.Red)//cancel button is selected
						{
							lblPlay.BackColor = Color.Red;
							lblNotify.BackColor = Color.Navy;
							lblCancel.BackColor = Color.Navy;
						}
					}
					break;
				case "upArrow":
					if(showInfo.Visible)//display 1 channel up's show info
					{
						try
						{
							if(Convert.ToInt32(currentShowInfoChannel)+1 < Form1.ListingsOrganizer.channelMin)
								currentShowInfoChannel = Form1.ListingsOrganizer.channelMax.ToString();
							else if(Convert.ToInt32(currentShowInfoChannel)+1 > Form1.ListingsOrganizer.channelMax)
								currentShowInfoChannel = Form1.ListingsOrganizer.channelMin.ToString();
							else
								currentShowInfoChannel = Convert.ToString(Convert.ToInt32(currentShowInfoChannel)+1);
							changeShowInfo(Form1.ListingsOrganizer.whatsPlaying(currentShowInfoChannel,currentShowInfoStartTime,"onNow"));
						}
						catch(Exception ex)
						{
							EventLog.WriteEntry("sctv",ex.ToString());
						}
					}
					break;
				case "rightArrow":
					if(showInfo.Visible)//display next show info
					{
						tempShow = Form1.ListingsOrganizer.whatsPlaying(currentShowInfoChannel,currentShowInfo.StopTime,"StartTime");
						changeShowInfo(tempShow);
						currentShowInfoStartTime = tempShow.StartTime;
					}
					if(panel1.Visible)//go through the options on panel1
					{
						if(lblNotify.BackColor == Color.Red)//the notify is selected
						{
							lblPlay.BackColor = Color.Red;
							lblNotify.BackColor = Color.Navy;
							lblCancel.BackColor = Color.Navy;
						}
						else if(lblPlay.BackColor == Color.Red)//the play button is selected
						{
							lblPlay.BackColor = Color.Navy;
							lblNotify.BackColor = Color.Navy;
							lblCancel.BackColor = Color.Red;
						}
						else if(lblCancel.BackColor == Color.Red)//cancel button is selected
						{
							lblPlay.BackColor = Color.Navy;
							lblNotify.BackColor = Color.Red;
							lblCancel.BackColor = Color.Navy;
						}
					}
					break;
				case "downArrow":
					if(showInfo.Visible)//display 1 channel down's show info
					{
						try
						{
							if(Convert.ToInt32(currentShowInfoChannel)-1 < Form1.ListingsOrganizer.channelMin)
								currentShowInfoChannel = Form1.ListingsOrganizer.channelMax.ToString();
							else if(Convert.ToInt32(currentShowInfoChannel)-1 > Form1.ListingsOrganizer.channelMax)
								currentShowInfoChannel = Form1.ListingsOrganizer.channelMin.ToString();
							else
								currentShowInfoChannel = Convert.ToString(Convert.ToInt32(currentShowInfoChannel)-1);
							changeShowInfo(Form1.ListingsOrganizer.whatsPlaying(currentShowInfoChannel,currentShowInfoStartTime,"onNow"));
						}
						catch(Exception ex)
						{
							EventLog.WriteEntry("sctv",ex.ToString());
						}
					}
					break;
				case "Escape": //Escape - hide/show tv
//					if(myTVViewer.Visible)
//						myTVViewer.Visible = false;
//					else
//						myTVViewer.Visible = true;
					break;
				case "Play": //Play - play currently selected show in showInfo
					watch(currentShowInfo);
					break;
				case "volumeChange":  //make sure mute is off
					int hr, currentVolume;
		
					if( (graphBuilder != null) && (basicAudio != null) )
					{
						hr = basicAudio.get_Volume( out currentVolume );
						if( hr == 0 )
						{
							if( currentVolume != -10000 )   // midi may use -9640 ???
							{
								savedVolume = currentVolume;
								currentVolume = -10000;
								//				menuControlMute.Checked = true;
							}
							else
							{
								currentVolume = savedVolume;
								//				menuControlMute.Checked = false;
							}
					
							hr = basicAudio.put_Volume( currentVolume );
							hr += 1;
						}
					}
					break;
			}
		}
		#endregion

		#region void ModifyRate( double rateAdjust )
//		void ModifyRate( double rateAdjust )
//		{
//			if( (mediaPos == null) || (rateAdjust == 0.0) )
//				return;
//
//			double rate;
//			int hr = mediaPos.get_Rate( out rate );
//			if( hr < 0 )
//				return;
//			rate += rateAdjust;
//			hr = mediaPos.put_Rate( rate );
//		}
		#endregion
		
		#region private void lblShowTitle_TextChanged(object sender, System.EventArgs e)
		private void lblShowTitle_TextChanged(object sender, System.EventArgs e)
		{
			if(lblShowTitle.Visible)
			{
				showTitleTimer.Stop();
				showTitleTimer.Start();
			}
			else
			{
				lblShowTitle.Visible = true;
				showTitleTimer.Start();
			}
		}
		#endregion

		#region public void displayShowTitle(TVProgramme show)
		public void displayShowTitle(TVProgramme show)
		{
			if(show != null)
				lblShowTitle.Text = show.Title;
			else
			{
				lblShowTitle.Text = "";
				lblShowTitle.Visible = false;
				showTitleTimer.Stop();
			}
		}
		#endregion

		#region public void displayShowInfo(TVProgramme show)
		public void displayShowInfo(TVProgramme show)
		{
			currentShowInfoChannel = capture.Tuner.Channel.ToString();
			if(show != null)
			{
				if(showInfo.Visible)//hide show info
				{
					showInfo.Visible = false;
					pnlInformation.Visible=false;
				}
				else//display show info
				{
					currentShowInfo = show;
					formatShowInfo(show);
					showInfo.Visible = true;
					pnlInformation.Visible=true;
				}
			}
			else
			{
				if(showInfo.Visible)
				{
					showInfo.Visible = false;
					pnlInformation.Visible=false;
				}
				else
				{
					//					showInfo.Text = "Show info not available";
					formatShowInfo();
					showInfo.Visible = true;
					pnlInformation.Visible=true;
				}
			}
		}
		#endregion

		#region public void changeShowInfo(TVProgramme show)
		public void changeShowInfo(TVProgramme show)
		{
			showInfo.Text = "";
			if(show != null)
			{
				currentShowInfo = show;
				formatShowInfo(show);
			}
			else
			{
//				showInfo.Text = "Show info not available";
				formatShowInfo();
			}
		}
		#endregion

		#region private void formatShowInfo(TVProgramme show)
		private void formatShowInfo(TVProgramme show)
		{
			FontStyle newFontStyle;
			Font currentFont=showInfo.SelectionFont;
			
			newFontStyle=FontStyle.Bold;
			showInfo.SelectionFont=new Font(currentFont.FontFamily,currentFont.Size,newFontStyle);
			showInfo.Text = "  "+ show.Channel.DisplayName;

			newFontStyle=FontStyle.Regular;
			showInfo.SelectionFont=new Font(currentFont.FontFamily,currentFont.Size,newFontStyle);
			showInfo.AppendText("  "+ show.StartTime.ToShortTimeString() +"-"+ show.StopTime.ToShortTimeString());

			newFontStyle=FontStyle.Bold;
			showInfo.SelectionFont=new Font(currentFont.FontFamily,currentFont.Size,newFontStyle);
			showInfo.AppendText(" "+ show.Title);

			newFontStyle=FontStyle.Regular;
			showInfo.SelectionFont=new Font(currentFont.FontFamily,currentFont.Size,newFontStyle);
			if(show.rating!=null)
				showInfo.AppendText(" ("+ show.rating[0].value +")");
			showInfo.AppendText(" "+ show.date);
			
			if(show.SubTitle.Length > 0 && show.SubTitle.Trim() != "N/A")
			{
				newFontStyle=FontStyle.Italic;
				showInfo.SelectionFont=new Font(currentFont.FontFamily,currentFont.Size,newFontStyle);
				showInfo.AppendText(" - "+ show.SubTitle);
			}

			newFontStyle=FontStyle.Regular;
			showInfo.SelectionFont=new Font(currentFont.FontFamily,currentFont.Size,newFontStyle);
			showInfo.AppendText("\n"+ show.Description);

			if(Form1.favoriteOrganizer.isFavorite(show))
			{
				lblFavorite.Visible = true;
			}
			else
			{
				lblFavorite.Visible = false;
			}
		}
		#endregion

		#region private void formatShowInfo()
		private void formatShowInfo()
		{
			FontStyle newFontStyle;
			Font currentFont=showInfo.SelectionFont;
			
			newFontStyle=FontStyle.Bold;
			showInfo.SelectionFont=new Font(currentFont.FontFamily,currentFont.Size,newFontStyle);
			showInfo.Text = "  "+ currentShowInfoChannel;

			newFontStyle=FontStyle.Italic;
			showInfo.SelectionFont=new Font(currentFont.FontFamily,currentFont.Size,newFontStyle);
			showInfo.AppendText("  Show info not available for "+ currentShowInfoStartTime.ToShortTimeString());
		}
		#endregion

		#region private void displayFutureAirDates(TVProgramme show)
		private void displayFutureAirDates(TVProgramme show)
		{
			if(!lvFutureAirDates.Visible)
			{
				if(lvFutureAirDates.Items.Count>0)
					lvFutureAirDates.Items[0].SubItems.Clear();
				TVProgramme[] showTimes = Form1.ListingsOrganizer.SearchByTitle(show.Title);
				string strSubItemText;
				FontStyle newFontStyle;
				Font currentFont=showInfo.SelectionFont;
				foreach(TVProgramme prog in showTimes)
				{
					ListViewItem lviFavorite = new ListViewItem();
					strSubItemText="";
					lviFavorite.SubItems[0].Text = " ";
					if(Form1.myMedia.searchByFilePath(prog.Title +"_"+ prog.date +".avi").Table.Rows.Count>0)
					lviFavorite.ImageIndex=0;
					strSubItemText += prog.StartTime.DayOfWeek +" "+ prog.StartTime.Month +"/"+ prog.StartTime.Day;
					strSubItemText += " "+ prog.StartTime.ToShortTimeString() +"-"+ prog.StopTime.ToShortTimeString();
					if(prog.SubTitle != "N/A")
						strSubItemText += " "+ prog.SubTitle;
					lviFavorite.SubItems.Add(strSubItemText);
					lviFavorite.Tag = prog;
					lvFutureAirDates.Items.Add(lviFavorite);
				}
				lvFutureAirDates.Columns[1].Width=pnlInformation.Width-lvFutureAirDates.Columns[0].Width-10;
				txtMessage.Text = "Future Air Dates for ";
				newFontStyle=FontStyle.Bold;
				txtMessage.SelectionFont=new Font(currentFont.FontFamily,currentFont.Size,newFontStyle);
				txtMessage.AppendText(show.Title);
				pnlInformation.Visible=true;
				lvFutureAirDates.Visible=true;			
			}
			else
			{
				pnlInformation.Visible=false;
			}
		}
		#endregion

		#region Timer Functions

		#region private void changeChannelTimer_Tick(object sender, System.EventArgs e)
		private void changeChannelTimer_Tick(object sender, System.EventArgs e)
		{
			try
			{
				changeChannelTimer.Stop();
				executeKeyStrokes();
			}
			catch(Exception ex)
			{
				EventLog.WriteEntry("sctv","changeChannel Error: "+ ex.ToString());
			}
		}
		#endregion

		#region private void showTitleTimer_Tick(object sender, System.EventArgs e)
		private void showTitleTimer_Tick(object sender, System.EventArgs e)
		{
			lblShowTitle.Visible = false;
			showTitleTimer.Stop();
		}
		#endregion
		#endregion

		#region Clock functions
		public string GetTime() 
		{ 
			return DateTime.Now.ToShortTimeString();
		} 

		private void clock_Tick(object sender, System.EventArgs e)
		{
			txtClock.Text=GetTime();
		}
		#endregion

		#region private void TVViewer_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		private void TVViewer_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
            //Form1.GUIState = SCTV.guiState.TV;
//			this.Refresh();
//			EventLog.WriteEntry("sctv","Refreshing TV");
		}
		#endregion

		#region public void watch(TVProgramme show)
		public void watch(TVProgramme show)
		{
			bool foundShow = false;
			//check to see if the show is on now - if not ask what they want to do
			foreach(TVProgramme currentShow in Form1.ListingsOrganizer.ShowsRightNow)
			{
				if(currentShow == show)//this show is playing now
				{
					changeChannel(Convert.ToInt32(show.Channel.DisplayName.Substring(0,show.Channel.DisplayName.IndexOf(" "))));
					foundShow = true;
				}
			}
			if(!foundShow)//this shows not on now - popup form to ask what they want to do with this show
			{
				panel1.BringToFront();
				showInfo.Visible = false;
				panel1.Show();
				lblNotify.BackColor = Color.Red;
				lblPlay.BackColor=Color.Navy;
				lblCancel.BackColor=Color.Navy;

				FontStyle newFontStyle;
				Font currentFont=conflictInfo.SelectionFont;
				
				newFontStyle=FontStyle.Bold;
				conflictInfo.SelectionFont=new Font(currentFont.FontFamily,currentFont.Size,newFontStyle);
				conflictInfo.Text = show.Title;

				newFontStyle=FontStyle.Regular;
				conflictInfo.SelectionFont=new Font(currentFont.FontFamily,currentFont.Size,newFontStyle);
				conflictInfo.AppendText(" is not on until ");

				newFontStyle=FontStyle.Bold;
				conflictInfo.SelectionFont=new Font(currentFont.FontFamily,currentFont.Size,newFontStyle);
				conflictInfo.AppendText("  "+ show.StartTime.ToShortTimeString());
			}
		}
		#endregion

		#region bool GetInterfaces()
		bool GetInterfaces()
		{
			Type comtype = null;
			object comobj = null;
			try 
			{
				comtype = Type.GetTypeFromCLSID( Clsid.FilterGraph );
				if( comtype == null )
					throw new NotSupportedException( "DirectX (8.1 or higher) not installed?" );
				comobj = Activator.CreateInstance( comtype );
				graphBuilder = (IGraphBuilder) comobj; comobj = null;
			
//				int hr = graphBuilder.RenderFile( clipFile, null );
//				if( hr < 0 )
//					Marshal.ThrowExceptionForHR( hr );
				basicAudio	= graphBuilder as IBasicAudio;
				return true;
			}
			catch( Exception ee )
			{
				MessageBox.Show( this, "Could not get interfaces\r\n" + ee.Message, "DirectShow.NET", MessageBoxButtons.OK, MessageBoxIcon.Stop );
				return false;
			}
			finally
			{
				if( comobj != null )
					Marshal.ReleaseComObject( comobj ); comobj = null;
			}
			return true;
		}
		#endregion
		
		#region private void showInfo_VisibleChanged(object sender, System.EventArgs e)
		private void showInfo_VisibleChanged(object sender, System.EventArgs e)
		{
			pnlIcons.Visible = showInfo.Visible;
			if(showInfo.Visible)
				lvFutureAirDates.Visible = false;
//			else
//				lvFutureAirDates.Visible = true;
		}
		#endregion

		#region private void pnlInformation_VisibleChanged(object sender, System.EventArgs e)
		private void pnlInformation_VisibleChanged(object sender, System.EventArgs e)
		{
			txtClock.Visible = pnlInformation.Visible;
		}
		#endregion

		#region private void lvFutureAirDates_VisibleChanged(object sender, System.EventArgs e)
		private void lvFutureAirDates_VisibleChanged(object sender, System.EventArgs e)
		{
			txtMessage.Visible = lvFutureAirDates.Visible;
			if(lvFutureAirDates.Visible)
			{
				lvFutureAirDates.Items[0].ImageIndex=0;
				lvFutureAirDates.Items[0].Selected=true;
				lvFutureAirDates.Refresh();
				lvFutureAirDates.Focus();
				showInfo.Visible = false;
			}
		}
		#endregion

		#region public void categorySearch(string category)
		public void categorySearch(string category)
		{
			lvFutureAirDates.Clear();
			string strSubItemText;
			FontStyle newFontStyle;
			Font currentFont=showInfo.SelectionFont;

			foreach(TVProgramme prog in Form1.ListingsOrganizer.SearchByCategory(category))
			{
				ListViewItem lviFavorite = new ListViewItem();
				strSubItemText="";
				lviFavorite.SubItems[0].Text = " ";
				if(Form1.myMedia.searchByFilePath(prog.Title +"_"+ prog.date +".avi").Table.Rows.Count>0)
					lviFavorite.ImageIndex=0;
				strSubItemText += prog.StartTime.DayOfWeek +" "+ prog.StartTime.Month +"/"+ prog.StartTime.Day;
				strSubItemText += " "+ prog.StartTime.ToShortTimeString() +"-"+ prog.StopTime.ToShortTimeString();
				if(prog.SubTitle != "N/A")
					strSubItemText += " "+ prog.SubTitle;
				lviFavorite.SubItems.Add(strSubItemText);
				lviFavorite.Tag = prog;
				lvFutureAirDates.Items.Add(lviFavorite);
			}

			lvFutureAirDates.Columns[1].Width=pnlInformation.Width-lvFutureAirDates.Columns[0].Width-10;
			txtMessage.Text = "Category Search Results for: "+ category +" at "+ DateTime.Now.ToShortTimeString();
			newFontStyle=FontStyle.Bold;
			txtMessage.SelectionFont=new Font(currentFont.FontFamily,currentFont.Size,newFontStyle);
			pnlInformation.Visible=true;
			lvFutureAirDates.Visible=true;		
		}
		#endregion

		#region private void scanChannelTimer_Tick(object sender, System.EventArgs e)
		private void scanChannelTimer_Tick(object sender, System.EventArgs e)
		{
			if(showInfo.Visible)//scan through the show info
			{
				this.executeKeyStrokes(this.channelScanDirection +"Arrow");
			}
			else//scan through channels
			{
				if(this.channelScanDirection=="up")
					this.executeKeyStrokes("ChannelUp");
				else
					this.executeKeyStrokes("ChannelDown");
			}
		}
		#endregion
		
		#region private void cleanUp()
		public void cleanUp()
		{
			//destroys the tv tuner capture object
			if(capture != null)
			{
				capture.Dispose();
				capture = null;
			}
			
			//destroy audio objects
			try 
			{
				basicAudio = null;

				if( graphBuilder != null )
					Marshal.ReleaseComObject( graphBuilder ); graphBuilder = null;
			}
			catch( Exception )
			{}
		}
		#endregion
		
		#region private void mediaViewer_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		private void mediaViewer_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			EventLog.WriteEntry("sctv","*******   found mediaViewer_Closing   *******************");
			Cursor.Show();
			cleanUp();
		}
		#endregion

		#region protected override void Dispose( bool disposing )
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#endregion

	}
}
