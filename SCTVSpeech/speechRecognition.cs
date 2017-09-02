using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Runtime.InteropServices;
using SpeechLib;
using System.Text;
using System.Diagnostics;
using System.Xml;
using System.Configuration;
using SCTVObjects;

namespace SCTV
{
	/// <summary>
	/// 
	/// </summary>
	public class SpeechRecognition : System.Windows.Forms.Form
	{
		public delegate void HeardCommand(Phrase thePhrase);
		public event HeardCommand executeCommand;

        public Phrase heardPhrase;
        private MenuItem menuItem15;
        private MenuItem menuItem16;

        private string _currentGrammarFile;

		#region API functions
		/// <summary>
		/// API functions
		/// </summary>
		[DllImport("user32.Dll")]
		public static extern int keybd_event(byte ch,byte scan,int flag,int info);

		[DllImport("user32.dll")]
		static extern IntPtr GetForegroundWindow();

		[DllImport("user32.Dll")]
		public static extern IntPtr GetMenu(IntPtr hwnd);
		
		[DllImport("user32.dll")]
		static extern IntPtr GetSubMenu(IntPtr hMnu, int nPos);

		[DllImport("user32.dll")]
		static extern int GetMenuItemCount(IntPtr hMnu);

		[DllImport("user32.dll")]
		static extern int GetMenuString(IntPtr hMnu, int uIDItem, StringBuilder text, int nMaxCount, int uFlag);

		[DllImport("user32.dll")]
		static extern int GetCursorPos(out Point pnt);

		[DllImport("user32.dll")]
		static extern int GetMenuState(IntPtr hMnu, int uIDItem, int uFlags);
		#endregion

		#region Variables
		///<summary>
		///Global variables
		///</summary>

		//struct for phrase list
		//used by the function SAPIGrammarFromArrayList(ArrayList phraseList)
		public struct command
		{
			public string ruleName;
			public string phrase;
		}

		//SAPI
		private SpeechLib.SpSharedRecoContext		objRecoContext;
		private SpeechLib.ISpeechRecoGrammar		grammar;
		private SpeechLib.ISpeechGrammarRule		rule=null;
		private SpeechLib.ISpeechGrammarRuleState   state;

		//character
		private AgentObjects.IAgentCtlCharacterEx agent1;
		Point pntMouse=new Point(0,0);

		//forms
		//they needed to be global couse they could be created and closed in different places
//		frmAbout frmAbout1=null;
		frmCommands frmCommands1=null;
//		frmFavorites frmFavorites1=null;
		frmAccuracyChange frmAccuracyChange1=null;
//		frmProfileChange frmProfileChange1=null;

		//misc
		string appPath="";
		byte keyHolding=0;
		int accuracyMinLimit=0;		//how accurate the phrase has to be before considered "recognized"
		string firstRecognition="";
		string previousGrammar="XMLDeactivated.xml";	//default commands
		public string speechCommand="";
		public command command1;
		private ArrayList defaultCommandList = new ArrayList();
		private bool isListernerActive=false;

		/// <summary>
		/// Form components
		/// </summary>
		private AxAgentObjects.AxAgent axAgent1;
        private System.Windows.Forms.ProgressBar MicVolume;
		private System.Windows.Forms.ContextMenu contextMenu1;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuItem5;
		private System.Windows.Forms.MenuItem menuItem6;
		private System.Windows.Forms.Timer timer2;
		private System.Windows.Forms.MenuItem menuItem7;
		private System.Windows.Forms.MenuItem menuItem8;
		private System.Windows.Forms.MenuItem menuItem9;
		private System.Windows.Forms.MenuItem menuItem10;
		private System.Windows.Forms.MenuItem menuItem12;
		private System.Windows.Forms.MenuItem menuItem13;
        private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.MenuItem menuItem11;
		private System.Windows.Forms.MenuItem menuItem14;
		private System.Windows.Forms.Timer listeningTimer;
		private System.Windows.Forms.Label lblRecognitionDisplay;
		private System.Windows.Forms.Timer recognitionDisplayTimer;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;

        private string computerNickName = "";

		#endregion

		#region public speechRecognition()
		public SpeechRecognition()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
		}

        public SpeechRecognition(string grammarFile)
        {
            InitializeComponent();
            _currentGrammarFile = grammarFile;

            computerNickName = getComputerNickName(grammarFile);

            //get accuracyMinLimit if it exists
            int.TryParse(System.Configuration.ConfigurationManager.AppSettings["Speech.MinAccuracy"],out accuracyMinLimit);
        }
		#endregion

        /// <summary>
        /// gets the computer nickname from xmlMain.xml
        /// </summary>
        /// <returns></returns>
        private string getComputerNickName(string grammarFile)
        {
            XmlDocument xMain = new XmlDocument();
            xMain.Load(Application.StartupPath + "\\config\\speech\\vocab\\" + grammarFile);
            string computerNickName = xMain.SelectSingleNode("/GRAMMAR/RULE[@NAME=\"ComputerName\"]").InnerText;
            return computerNickName;
        }

		#region protected override void Dispose( bool disposing )
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#endregion

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SpeechRecognition));
            this.axAgent1 = new AxAgentObjects.AxAgent();
            this.MicVolume = new System.Windows.Forms.ProgressBar();
            this.contextMenu1 = new System.Windows.Forms.ContextMenu();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.menuItem11 = new System.Windows.Forms.MenuItem();
            this.menuItem14 = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.menuItem6 = new System.Windows.Forms.MenuItem();
            this.menuItem7 = new System.Windows.Forms.MenuItem();
            this.menuItem12 = new System.Windows.Forms.MenuItem();
            this.menuItem10 = new System.Windows.Forms.MenuItem();
            this.menuItem9 = new System.Windows.Forms.MenuItem();
            this.menuItem8 = new System.Windows.Forms.MenuItem();
            this.menuItem15 = new System.Windows.Forms.MenuItem();
            this.menuItem13 = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.menuItem16 = new System.Windows.Forms.MenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.listeningTimer = new System.Windows.Forms.Timer(this.components);
            this.lblRecognitionDisplay = new System.Windows.Forms.Label();
            this.recognitionDisplayTimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.axAgent1)).BeginInit();
            this.SuspendLayout();
            // 
            // axAgent1
            // 
            this.axAgent1.Enabled = true;
            this.axAgent1.Location = new System.Drawing.Point(48, 40);
            this.axAgent1.Name = "axAgent1";
            this.axAgent1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axAgent1.OcxState")));
            this.axAgent1.Size = new System.Drawing.Size(32, 32);
            this.axAgent1.TabIndex = 0;
            this.axAgent1.TabStop = false;
            this.axAgent1.Visible = false;
            this.axAgent1.DragStart += new AxAgentObjects._AgentEvents_DragStartEventHandler(this.axAgent1_DragStart);
            this.axAgent1.ClickEvent += new AxAgentObjects._AgentEvents_ClickEventHandler(this.axAgent1_ClickEvent);
            this.axAgent1.DragComplete += new AxAgentObjects._AgentEvents_DragCompleteEventHandler(this.axAgent1_DragComplete);
            // 
            // MicVolume
            // 
            this.MicVolume.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.MicVolume.BackColor = System.Drawing.Color.Black;
            this.MicVolume.ContextMenu = this.contextMenu1;
            this.MicVolume.Location = new System.Drawing.Point(30, 0);
            this.MicVolume.Name = "MicVolume";
            this.MicVolume.Size = new System.Drawing.Size(112, 16);
            this.MicVolume.TabIndex = 3;
            // 
            // contextMenu1
            // 
            this.contextMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem3,
            this.menuItem11,
            this.menuItem14,
            this.menuItem5,
            this.menuItem4,
            this.menuItem6,
            this.menuItem7,
            this.menuItem12,
            this.menuItem10,
            this.menuItem9,
            this.menuItem8,
            this.menuItem15,
            this.menuItem13,
            this.menuItem1,
            this.menuItem2,
            this.menuItem16});
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 0;
            this.menuItem3.Text = "Stop listen";
            this.menuItem3.Click += new System.EventHandler(this.menuItem3_Click);
            // 
            // menuItem11
            // 
            this.menuItem11.Index = 1;
            this.menuItem11.Text = "-";
            // 
            // menuItem14
            // 
            this.menuItem14.Checked = true;
            this.menuItem14.Index = 2;
            this.menuItem14.Text = "Use agent";
            this.menuItem14.Click += new System.EventHandler(this.menuItem14_Click);
            // 
            // menuItem5
            // 
            this.menuItem5.Index = 3;
            this.menuItem5.Text = "-";
            // 
            // menuItem4
            // 
            this.menuItem4.Index = 4;
            this.menuItem4.Text = "Add favorites";
            this.menuItem4.Click += new System.EventHandler(this.menuItem4_Click);
            // 
            // menuItem6
            // 
            this.menuItem6.Index = 5;
            this.menuItem6.Text = "Change character";
            this.menuItem6.Click += new System.EventHandler(this.menuItem6_Click);
            // 
            // menuItem7
            // 
            this.menuItem7.Index = 6;
            this.menuItem7.Text = "Change accuracy limit";
            this.menuItem7.Click += new System.EventHandler(this.menuItem7_Click);
            // 
            // menuItem12
            // 
            this.menuItem12.Index = 7;
            this.menuItem12.Text = "-";
            // 
            // menuItem10
            // 
            this.menuItem10.Index = 8;
            this.menuItem10.Text = "Change user profile";
            this.menuItem10.Click += new System.EventHandler(this.menuItem10_Click);
            // 
            // menuItem9
            // 
            this.menuItem9.Index = 9;
            this.menuItem9.Text = "Mic training wizard...";
            this.menuItem9.Click += new System.EventHandler(this.menuItem9_Click);
            // 
            // menuItem8
            // 
            this.menuItem8.Index = 10;
            this.menuItem8.Text = "User training wizard...";
            this.menuItem8.Click += new System.EventHandler(this.menuItem8_Click);
            // 
            // menuItem15
            // 
            this.menuItem15.Index = 11;
            this.menuItem15.Text = "-";
            // 
            // menuItem13
            // 
            this.menuItem13.Index = 12;
            this.menuItem13.Text = "Profile proporties wizard...";
            this.menuItem13.Click += new System.EventHandler(this.menuItem13_Click);
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 13;
            this.menuItem1.Text = "-";
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 14;
            this.menuItem2.Text = "Exit";
            this.menuItem2.Click += new System.EventHandler(this.menuItem2_Click);
            // 
            // menuItem16
            // 
            this.menuItem16.Index = 15;
            this.menuItem16.Text = "Visible";
            this.menuItem16.Click += new System.EventHandler(this.menuItem16_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "Agent *.acs|*.acs";
            // 
            // timer1
            // 
            this.timer1.Interval = 10;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick_1);
            // 
            // timer2
            // 
            this.timer2.Interval = 500;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // listeningTimer
            // 
            this.listeningTimer.Interval = 10000;
            this.listeningTimer.Tick += new System.EventHandler(this.listeningTimer_Tick);
            // 
            // lblRecognitionDisplay
            // 
            this.lblRecognitionDisplay.Location = new System.Drawing.Point(0, 0);
            this.lblRecognitionDisplay.Name = "lblRecognitionDisplay";
            this.lblRecognitionDisplay.Size = new System.Drawing.Size(24, 16);
            this.lblRecognitionDisplay.TabIndex = 9;
            // 
            // recognitionDisplayTimer
            // 
            this.recognitionDisplayTimer.Interval = 2500;
            this.recognitionDisplayTimer.Tick += new System.EventHandler(this.recognitionDisplayTimer_Tick);
            // 
            // SpeechRecognition
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(150, 61);
            this.Controls.Add(this.lblRecognitionDisplay);
            this.Controls.Add(this.MicVolume);
            this.Controls.Add(this.axAgent1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SpeechRecognition";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Speech Recognition";
            this.TopMost = true;
            this.TransparencyKey = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.Leave += new System.EventHandler(this.speechRecognition_Leave);
            ((System.ComponentModel.ISupportInitialize)(this.axAgent1)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Create the main object (SpSharedRecoContext), init the gramar
		/// And activating the events ...
		/// 1.Recognition	=> if any phrase recognized
		/// 2.AudioLevel	=> to watch the mic volume
		/// </summary>
		private void initSAPI()
		{
			try
			{
				objRecoContext = new SpeechLib.SpSharedRecoContext();
				objRecoContext.AudioLevel+= new _ISpeechRecoContextEvents_AudioLevelEventHandler(RecoContext_VUMeter);
				objRecoContext.Recognition+= new _ISpeechRecoContextEvents_RecognitionEventHandler(RecoContext_Recognition);
				objRecoContext.EventInterests=SpeechLib.SpeechRecoEvents.SRERecognition | SpeechLib.SpeechRecoEvents.SREAudioLevel;

                //create grammar interface with ID = 0
                grammar = objRecoContext.CreateGrammar(0);
			}
			catch(Exception ex)
			{
                Tools.WriteToFile(Tools.errorFile, "Speech is probably not installed "+ ex.Message);
				//MessageBox.Show("Exception \n"+ex.ToString(),"Error - initSAPI");
			}
		}
		
		/// <summary>
		/// Loading grammar from file
		/// And resetting all previous rules (delete all previous rules !)
		/// </summary>
		/// <param name="FileName">FilName add to the global variable appPath</param>
		public void loadGrammarFile(string FileName)
		{
			//Tools.WriteToFile(Tools.errorFile,"loading grammar file: "+ FileName);
			try
			{
                //notify user
                //label1.Text = FileName;
                //grammar.CmdLoadFromFile(appPath + "speech\\vocab\\" + FileName, SpeechLib.SpeechLoadOption.SLODynamic);
                grammar.CmdLoadFromFile(appPath + "config\\speech\\vocab\\xmlAll.xml", SpeechLib.SpeechLoadOption.SLODynamic);
                grammar.CmdSetRuleIdState(0, SpeechRuleState.SGDSActive);

                //activate rules for each filename
                switch (FileName.ToLower())
                {
                    case "xmlmedialibrary.xml":
                        grammar.CmdSetRuleState("scroll", SpeechRuleState.SGDSActive);
                        grammar.CmdSetRuleState("playPause", SpeechRuleState.SGDSActive);
                        grammar.CmdSetRuleState("Close", SpeechRuleState.SGDSActive);
                        grammar.CmdSetRuleState("media", SpeechRuleState.SGDSActive);
                        break;
                    case "xmlvideoplayer.xml":
                        grammar.CmdSetRuleState("stop", SpeechRuleState.SGDSActive);
                        grammar.CmdSetRuleState("playPause", SpeechRuleState.SGDSActive);
                        grammar.CmdSetRuleState("Close", SpeechRuleState.SGDSActive);
                        break;
                    case "xmlcameras.xml":
                        grammar.CmdSetRuleState("camera", SpeechRuleState.SGDSActive);
                        break;
                }

                
				//save as previous grammar for commands list
                //if (FileName != "XMLCommands.xml")
                //    previousGrammar="speech\\vocab\\"+ FileName;
			}
			catch(Exception ex)
			{
                //MessageBox.Show("Error loading file "+appPath +"speech\\vocab\\"+ FileName+"\n","Error - SAPIGrammarFromFile");
                Tools.WriteToFile(Tools.errorFile, "sctv file load: "+ ex.Message);
			}
		}

        public void addRulesToCurrentGrammar(string grammarFileName)
        {
            command rule;
            string[] innerXml;
            string innerXmlString = "";
            ArrayList phraseList = new ArrayList();//holds the command objects to send to SAPIGrammarFromArrayList
            XmlDocument xGrammar = new XmlDocument();
            xGrammar.Load(appPath + "config\\speech\\vocab\\" + grammarFileName);

            //iterate rule nodes and add to current grammar
            foreach (XmlNode ruleNode in xGrammar.SelectNodes("GRAMMAR/RULE"))
            {
                rule = new command();
                rule.ruleName = ruleNode.Attributes["NAME"].Value;
                foreach (XmlNode phraseNode in ruleNode.ChildNodes)
                {
                    if (phraseNode.InnerXml != null && phraseNode.InnerXml.Length > 0)
                    {
                        innerXmlString = phraseNode.InnerXml.Replace("<L", "");
                        innerXmlString = innerXmlString.Replace("<P", "");
                        innerXmlString = innerXmlString.Replace("</P", "");
                        innerXml = innerXmlString.Split(new char[] { '>' });
                        for (int x = 0; x < innerXml.Length; x++)
                        {
                            if(innerXml[x].Length > 0)
                            {
                            rule.phrase = innerXml[x];
                            phraseList.Add(rule);
                            }
                        }
                    }
                }
            }
            SAPIGrammarFromArrayList(phraseList);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="phraseList"></param>
		public void addGrammarArrayList(ArrayList phraseList)
		{
			//			add titles to grammar
			try
			{
				char[] sep = {'|'};
				string[] phraseInfo;
				for(int x=0;x<phraseList.Count;x++)
				{
					phraseInfo = phraseList[x].ToString().Split(sep);
					if(phraseInfo.Length==2)
					{
						command1.ruleName=phraseInfo[0];
						command1.phrase=phraseInfo[1];

						if(!phraseList.Contains(command1))//don't add duplicates
						{
							phraseList.Add(command1);
						}
					}
				}
				SAPIGrammarFromArrayList(phraseList);
			}
			catch(Exception ex)
			{
				Tools.WriteToFile(Tools.errorFile,ex.ToString());
			}
		}
		
		/// <summary>
		/// Adding new ruleName and phrase programmatically (not from grammar file)
		/// To an existing grammar (Adding to previous rules ! without deleting them)
		/// Can be add rules to exist grammar from file or from first added rules
		/// </summary>
		/// <param name="phraseList">
		/// ArrayList (phraseList) contains struct command objects that contains ...
		/// 1. ruleName		(rule Name must be individual)
		/// 2. phrase		(word to be recognized)
		/// </param>
		private void SAPIGrammarFromArrayList(ArrayList phraseList)
		{
			object propertyValue="";
			command command1;
			//Tools.WriteToFile(Tools.errorFile,"in SAPIGrammarFromArrayList");
			int i;
			try
			{
				for (i=0;i<phraseList.Count;i++)
				{
					command1=(command)phraseList[i];

					//add new rule with ID = i+100
					rule=grammar.Rules.Add(command1.ruleName, SpeechRuleAttributes.SRATopLevel, i+100);

					//add new word to the rule
					state=rule.InitialState;
					propertyValue="";
					state.AddWordTransition(null,command1.phrase," ", SpeechGrammarWordType.SGLexical, "", 0, ref propertyValue, 1F);

					//commit rules
					grammar.Rules.Commit();
					//make rule active (needed for each rule)
					grammar.CmdSetRuleState(command1.ruleName, SpeechRuleState.SGDSActive);
				}
			}
			catch(Exception ex)
			{
				Tools.WriteToFile(Tools.errorFile,ex.ToString());
			}
		}
		
        /// <summary>
		/// main objRecoContext event
		/// launched when engine recognized a phrase
		/// </summary>
		/// <param name="e">contained information on the phrase that been recognized</param>
		public void RecoContext_Recognition(int StreamNumber, object StreamPosition, SpeechRecognitionType RecognitionType,	ISpeechRecoResult e)
		{
            //if(isListernerActive && Form1.GUIState!=guiState.mediaLibrary)
            //{
            //    listeningTimer.Enabled=false;
            //    listeningTimer.Enabled=true;
            //}

			//command command1;
			//calculate accuracy
            float accuracy=(float)e.PhraseInfo.Elements.Item(0).EngineConfidence;

            ////change accuracyMax dynamicly
            //if (accuracyMax<accuracy)
            //    accuracyMax=accuracy;

            if (accuracy<0)
                accuracy=0;

            accuracy=(int)((float)accuracy/100*100);
            //label2.Text="Accuracy "+accuracy.ToString()+ "%";
			//Tools.WriteToFile(Tools.errorFile,"@@@ # matching phrases "+ e.PhraseInfo.Elements.Count.ToString());
//			Tools.WriteToFile(Tools.errorFile,"alternates "+ e.Alternates(1,1,1));
//			if(e.PhraseInfo.Elements.Count>1)
//			{
//				
//				for(int counter=0;counter<e.Alternates(1,1,1).Count;counter++)
//				{
//					float tempAccuracy=(float)e.Alternates(1,1,1).Item(counter).PhraseInfo.Elements.Item(0).EngineConfidence;
//					Tools.WriteToFile(Tools.errorFile,"matching alternates "+ e.Alternates(1,1,1).Item(counter).PhraseInfo.GetText(counter,-1,true) +"  "+ tempAccuracy +"%");
//				}
//			}
			//get phrase
			string phrase=e.PhraseInfo.GetText(0,-1,true);
			//make sure it's in lower case (for safer use only)
			//phrase=phrase.ToLower();
            //Tools.WriteToFile("sctv phrase","phrase= "+ phrase +"  accuracy= "+ accuracy);
            
			//if recognized any ...
            if (phrase != "" && accuracy >= accuracyMinLimit)
			{
                //heardPhrase.ComputerName = computerNickName;
                heardPhrase = new Phrase(e, computerNickName);

                //Tools.WriteToFile("speech & accuracy", "phrase " + phrase +" - Accuracy="+ heardPhrase.Accuracy);

                if (heardPhrase.Accuracy > 0)
                {
                    SpeechRecognized();
                    
                    //Only if agent enabled
                    if (menuItem14.Checked == true)
                    {
                        agent1.StopAll("");
                        //					agent1.Speak(phrase,"");
                    }

                    #region commands that are to be moved to the appropriate forms
                    //                ArrayList phraseList=new ArrayList();

                    //                switch (e.PhraseInfo.Rule.Name)		//rule name (not the phrase !)
                    //                {
                    //                    case "Activate":
                    //                    {
                    //                        if(accuracy>50)//set accuracy level for this command
                    //                        {
                    //                            listeningTimer.Enabled=true;
                    //                            isListernerActive=true;
                    //                            //load grammar
                    //                            switch(Form1.GUIState)
                    //                            {
                    //                                case guiState.mediaLibrary:
                    //                                    loadGrammarFile("XMLMediaLibrary.xml");
                    //                                    break;
                    //                                case guiState.TV:
                    //                                    loadGrammarFile("XMLTV.xml");
                    //                                    break;
                    //                                case guiState.video:
                    //                                    loadGrammarFile("XMLVideo.xml");
                    //                                    break;
                    //                                default:
                    //                                    loadGrammarFile("XMLActivated.xml");
                    //                                    break;
                    //                            }

                    //                            //notify user
                    //                            label1.Text="Activated";

                    //                            //Only if agent enabled
                    //                            if (menuItem14.Checked==true)
                    //                            {
                    //                                //animate character
                    //                                //							agent1.Play("StartListening");
                    //                                //							agent1.Speak("I'm listening","");
                    //                            }
                    //                        }
                    //                        break;
                    //                    }
                    //                    case "Deactivate":
                    //                    {
                    //                        deactivateListener();
                    //                        break;
                    //                    }
                    //                    case "Start":
                    //                    {
                    //                        keybd_event((byte)Keys.LWin,0,0,0);	//key down
                    //                        keybd_event((byte)Keys.LWin,0,2,0);	//key up

                    //                        //load grammar
                    ////						SAPIGrammarFromFile("XMLStart.xml");

                    //                        //notify user
                    //                        label1.Text="Start";
                    //                        break;
                    //                    }
                    //                    case "Right":
                    //                    {
                    //                        if(accuracy>40)
                    //                        {
                    //                            keybd_event((byte)Keys.Right,0,0,0);	//key down
                    //                            keybd_event((byte)Keys.Right,0,2,0);	//key up
                    //                        }
                    //                        break;
                    //                    }
                    //                    case "Left":
                    //                    {
                    //                        if(accuracy>40)
                    //                        {
                    //                            keybd_event((byte)Keys.Left,0,0,0);	//key down
                    //                            keybd_event((byte)Keys.Left,0,2,0);	//key up
                    //                        }
                    //                        break;
                    //                    }
                    //                    case "Up":
                    //                    {
                    //                        if(accuracy>40)
                    //                        {
                    //                            keybd_event((byte)Keys.Up,0,0,0);		//key down
                    //                            keybd_event((byte)Keys.Up,0,2,0);		//key up
                    //                        }
                    //                        break;
                    //                    }
                    //                    case "Down":
                    //                    {
                    //                        if(accuracy>20)
                    //                        {
                    //                            keybd_event((byte)Keys.Down,0,0,0);	//key down
                    //                            keybd_event((byte)Keys.Down,0,2,0);	//key up
                    //                        }
                    //                        break;
                    //                    }
                    //                    case "Enter":
                    //                    {
                    //                        keybd_event((byte)Keys.Enter,0,0,0);	//key down
                    //                        keybd_event((byte)Keys.Enter,0,2,0);	//key up
                    //                        break;
                    //                    }
                    //                    case "Escape":
                    //                    {
                    //                        if(accuracy>40)
                    //                        {
                    //                            keybd_event((byte)Keys.Escape,0,0,0);	//key down
                    //                            keybd_event((byte)Keys.Escape,0,2,0);	//key up
                    //                            keybd_event((byte)Keys.LMenu,0,2,0);	//key up

                    //                            //load grammar (used to reset grammar in case it contains menu stuff ...)
                    //                            loadGrammarFile("XMLActivated.xml");

                    //                            //notify user
                    //                            label1.Text="Activated";
                    //                        }
                    //                        break;
                    //                    }
                    //                    case "PureEscape":
                    //                    {
                    //                        keybd_event((byte)Keys.Escape,0,0,0);	//key down
                    //                        keybd_event((byte)Keys.Escape,0,2,0);	//key up
                    //                        break;
                    //                    }
                    //                    case "Alt":
                    //                    {
                    //                        keybd_event((byte)Keys.LMenu,0,0,0);	//key down
                    //                        keybd_event((byte)Keys.LMenu,0,2,0);	//key up

                    //                        //check if there is any menu and hook it
                    //                        IntPtr hWnd=GetForegroundWindow();
                    //                        IntPtr hMnu=GetMenu(hWnd);
                    //                        int mnuCnt=GetMenuItemCount(hMnu);

                    //                        if (mnuCnt!=0)
                    //                        {
                    //                            //Only if agent enabled
                    //                            if (menuItem14.Checked==true)
                    //                            {
                    //                                //animate character
                    //                                agent1.Play("DoMagic1");
                    //                                agent1.Think("Hooking menu ...");
                    //                            }

                    //                            //add menu to grammar
                    //                            hookMenu(hMnu);

                    //                            //Only if agent enabled
                    //                            if (menuItem14.Checked==true)
                    //                            {
                    //                                //animate character
                    //                                agent1.Play("Idle1_1");
                    //                            }
                    //                        }
                    //                        else
                    //                        {
                    //                            //load grammar
                    //                            loadGrammarFile("XMLActivated.xml");

                    //                            //notify user
                    //                            label1.Text="Activated";
                    //                        }
                    //                        break;
                    //                    }
                    //                    case "Tab":
                    //                    {
                    //                        keybd_event((byte)Keys.Tab,0,0,0);	//key down
                    //                        keybd_event((byte)Keys.Tab,0,2,0);	//key up
                    //                        break;
                    //                    }
                    //                    case "ShiftTab":
                    //                    {
                    //                        keybd_event((byte)Keys.LShiftKey,0,0,0);	//key down
                    //                        keybd_event((byte)Keys.Tab,0,0,0);		//key down
                    //                        keybd_event((byte)Keys.Tab,0,2,0);		//key up
                    //                        keybd_event((byte)Keys.LShiftKey,0,2,0);	//key up
                    //                        break;
                    //                    }
                    //                    case "CloseProgram":
                    //                    {
                    //                        Close();
                    //                        break;
                    //                    }
                    //                    case "ShowAbout":
                    //                    {
                    ////						if (frmAbout1==null)
                    ////						{
                    ////							//show frmAbout
                    ////							frmAbout1=new frmAbout();
                    ////							frmAbout1.Closed+=new EventHandler(frmAbout1_Closed);
                    ////							//send user profile
                    ////							frmAbout1.Tag=(string)objRecoContext.Recognizer.Profile.GetDescription(0);
                    ////							frmAbout1.Show();
                    ////						}

                    //                        //load grammar
                    //                        loadGrammarFile("XMLAbout.xml");

                    //                        //notify user
                    //                        label1.Text="About Speech Recognition";
                    //                        break;
                    //                    }
                    //                    case "CloseAbout":
                    //                    {
                    //                        //close frmAbout
                    ////						if (frmAbout1!=null)
                    ////						{
                    ////							frmAbout1.Close();
                    ////							frmAbout1=null;
                    ////						}
                    //                        break;
                    //                    }
                    //                    case "ShowCommands":
                    //                    {
                    //                        if (frmCommands1==null)
                    //                        {
                    //                            //show frmAbout
                    //                            frmCommands1=new frmCommands();
                    //                            frmCommands1.Closed+=new EventHandler(frmCommands1_Closed);
                    //                            //send grammar
                    //                            frmCommands1.Tag=label1.Text;
                    //                            frmCommands1.Show();
                    //                        }

                    //                        //load grammar
                    //                        loadGrammarFile("XMLCommands.xml");
                    //                        break;
                    //                    }
                    //                    case "CloseCommands":
                    //                    {
                    //                        //close frmCommands
                    //                        if (frmCommands1!=null)
                    //                        {
                    //                            frmCommands1.Close();
                    //                            frmCommands1=null;
                    //                        }
                    //                        break;
                    //                    }
                    //                    case "ShowFavorites":
                    //                    {
                    ////						if (frmFavorites1==null)
                    ////						{
                    ////							//show frmFavorites
                    ////							frmFavorites1=new frmFavorites();
                    ////							frmFavorites1.Closed+=new EventHandler(frmFavorites1_Closed);
                    ////							//send file name
                    ////							frmFavorites1.Tag=appPath+"XMLFavorites.xml";
                    ////							frmFavorites1.Show();
                    ////						}

                    //                        //load grammar
                    //                        loadGrammarFile("XMLFavorites.xml");

                    //                        //notify user
                    //                        label1.Text="Favorites";
                    //                        break;
                    //                    }
                    //                    case "CloseFavorites":
                    //                    {
                    //                        //show frmAbout
                    ////						if (frmFavorites1!=null)
                    ////						{
                    ////							frmFavorites1.Close();
                    ////							frmFavorites1=null;
                    ////						}
                    //                        break;
                    //                    }
                    //                    case "CloseForm":
                    //                    {
                    //                        IntPtr hWnd=GetForegroundWindow();

                    //                        //make sure we are not closing our program ...
                    //                        if (hWnd!=this.Handle)
                    //                        {
                    //                            keybd_event((byte)Keys.LMenu,0,0,0);	//key down
                    //                            keybd_event((byte)Keys.F4,0,0,0);		//key down
                    //                            keybd_event((byte)Keys.LMenu,0,2,0);	//key up
                    //                            keybd_event((byte)Keys.F4,0,2,0);		//key up
                    //                        }
                    //                        break;
                    //                    }
                    //                    case "Programs":
                    //                    case "Documents":
                    //                    case "Settings":
                    //                    case "Search":
                    //                    case "Help":
                    //                    case "Run":
                    //                    {
                    //                        keybd_event((byte)(e.PhraseInfo.Rule.Name[0]),0,0,0);	//key down
                    //                        keybd_event((byte)(e.PhraseInfo.Rule.Name[0]),0,2,0);	//key up

                    //                        //load grammar
                    //                        loadGrammarFile("XMLActivated.xml");

                    //                        //notify user
                    //                        label1.Text="Activated";
                    //                        break;
                    //                    }
                    //                    case "RunProgram":
                    //                    {
                    //                        //show frmAbout
                    ////						if (frmFavorites1!=null)
                    ////						{
                    ////							frmFavorites1.Close();
                    ////							frmFavorites1=null;
                    ////						}

                    //                        try
                    //                        {
                    //                            System.Diagnostics.Process.Start(phrase);
                    //                        }
                    //                        catch
                    //                        {
                    //                            //Only if agent enabled
                    //                            if (menuItem14.Checked==true)
                    //                            {
                    //                                agent1.Speak("Could not run : "+phrase,"");
                    //                            }
                    //                        }

                    //                        //load grammar
                    //                        loadGrammarFile("XMLActivated.xml");

                    //                        //notify user
                    //                        label1.Text="Activated";
                    //                        break;
                    //                    }
                    //                    case "SwitchProgram":
                    //                    {
                    //                        keybd_event((byte)Keys.LMenu,0,0,0);	//key down
                    //                        keybd_event((byte)Keys.Tab,0,0,0);	//key down
                    //                        keybd_event((byte)Keys.Tab,0,2,0);	//key up

                    //                        //load grammar
                    ////						SAPIGrammarFromFile("XMLSwitchProgram.xml");
                    //                        loadGrammarFile("XMLNumericState.xml");
                    //                        //notify user
                    //                        label1.Text="Switch Program";
                    //                        break;
                    //                    }
                    //                    case "SwitchEnter":
                    //                    {
                    //                        keybd_event((byte)Keys.LMenu,0,2,0);	//key up

                    //                        //load grammar
                    //                        loadGrammarFile("XMLActivated.xml");

                    //                        //notify user
                    //                        label1.Text="Activated";
                    //                        break;
                    //                    }

                    //                    case "HoldKey":
                    //                    {
                    //                        //load grammar
                    //                        loadGrammarFile("XMLStickyKeys.xml");

                    //                        //notify user
                    //                        label1.Text="Press key";
                    //                        break;
                    //                    }

                    //                    case "ReleaseKey":
                    //                    {
                    //                        timer2.Enabled=false;

                    //                        //load grammar
                    //                        loadGrammarFile("XMLActivated.xml");

                    //                        //notify user
                    //                        label1.Text="Activated";
                    //                        break;
                    //                    }

                    //                    case "HoldRight":
                    //                    {
                    //                        keyHolding=(byte)Keys.Right;
                    //                        timer2.Enabled=true;
                    //                        break;
                    //                    }
                    //                    case "HoldLeft":
                    //                    {
                    //                        keyHolding=(byte)Keys.Left;
                    //                        timer2.Enabled=true;
                    //                        break;
                    //                    }
                    //                    case "HoldUp":
                    //                    {
                    //                        keyHolding=(byte)Keys.Up;
                    //                        timer2.Enabled=true;
                    //                        break;
                    //                    }
                    //                    case "HoldDown":
                    //                    {
                    //                        keyHolding=(byte)Keys.Down;
                    //                        timer2.Enabled=true;
                    //                        break;
                    //                    }
                    //                    case "PageUp":
                    //                    {
                    //                        keybd_event((byte)Keys.PageUp,0,0,0);	//key down
                    //                        keybd_event((byte)Keys.PageUp,0,2,0);	//key up
                    //                        break;
                    //                    }
                    //                    case "Yes":
                    //                    {
                    //                        keybd_event((byte)Keys.Y,0,0,0);	//key down
                    //                        keybd_event((byte)Keys.Y,0,2,0);	//key up
                    //                        break;
                    //                    }
                    //                    case "No":
                    //                    {
                    //                        keybd_event((byte)Keys.N,0,0,0);	//key down
                    //                        keybd_event((byte)Keys.N,0,2,0);	//key up
                    //                        break;
                    //                    }
                    //                    case "BackSpace":
                    //                    {
                    //                        keybd_event((byte)Keys.Back,0,0,0);	//key down
                    //                        keybd_event((byte)Keys.Back,0,2,0);	//key up
                    //                        break;
                    //                    }
                    //                    case "Space":
                    //                    {
                    //                        keybd_event((byte)Keys.Space,0,0,0);	//key down
                    //                        keybd_event((byte)Keys.Space,0,2,0);	//key up
                    //                        break;
                    //                    }
                    //                    case "ShutDown":
                    //                    {
                    ////						Shell32.ShellClass a=new Shell32.ShellClass();
                    ////						a.ShutdownWindows();

                    //                        //load grammar
                    //                        loadGrammarFile("XMLShutDown.xml");

                    //                        //notify user
                    //                        label1.Text="Shut Down";
                    //                        break;
                    //                    }
                    //                    case "ActivateWithoutAnimation":
                    //                    {
                    //                        //load grammar
                    //                        loadGrammarFile("XMLActivated.xml");

                    //                        //notify user
                    //                        label1.Text="Activated";
                    //                        break;
                    //                    }
                    //                    case "EnterNumericState":
                    //                    {
                    //                        //load grammar
                    //                        loadGrammarFile("XMLNumericState.xml");

                    //                        //notify user
                    //                        label1.Text="Numeric State...";
                    //                        break;
                    //                    }
                    //                    case "Zero":
                    //                    case "One":
                    //                    case "Two":
                    //                    case "Three":
                    //                    case "Four":
                    //                    case "Five":
                    //                    case "Six":
                    //                    case "Seven":
                    //                    case "Eight":
                    //                    case "Nine":
                    //                    {
                    //                        byte k=(byte)e.PhraseInfo.GetText(0,-1,false)[0];

                    //                        keybd_event((byte)(k+'0'),0,0,0);	//key down
                    //                        keybd_event((byte)(k+'0'),0,2,0);	//key up
                    //                        break;
                    //                    }
                    //                    case "Plus":
                    //                    {
                    //                        keybd_event((byte)Keys.Add,0,0,0);	//key down
                    //                        keybd_event((byte)Keys.Add,0,2,0);	//key up
                    //                        break;
                    //                    }
                    //                    case "Minus":
                    //                    {
                    //                        keybd_event((byte)Keys.Subtract,0,0,0);	//key down
                    //                        keybd_event((byte)Keys.Subtract,0,2,0);	//key up
                    //                        break;
                    //                    }
                    //                    case "Div":
                    //                    {
                    //                        keybd_event((byte)Keys.Divide,0,0,0);	//key down
                    //                        keybd_event((byte)Keys.Divide,0,2,0);	//key up
                    //                        break;
                    //                    }
                    //                    case "Mul":
                    //                    {
                    //                        keybd_event((byte)Keys.Multiply,0,0,0);	//key down
                    //                        keybd_event((byte)Keys.Multiply,0,2,0);	//key up
                    //                        break;
                    //                    }
                    //                    case "Equal":
                    //                    {
                    //                        keybd_event(187,0,0,0);	//key down
                    //                        keybd_event(187,0,2,0);	//key up
                    //                        break;
                    //                    }
                    //                    case "EnterAlphabeticState":
                    //                    {
                    //                        //load grammar
                    //                        loadGrammarFile("XMLAlphabeticState.xml");

                    //                        //notify user
                    //                        label1.Text="Alphabetic State...";
                    //                        break;
                    //                    }
                    //                    case "abcA":case "abcB":case "abcC":case "abcD":case "abcE":case "abcF":case "abcG":
                    //                    case "abcH":case "abcI":case "abcJ":case "abcK":case "abcL":case "abcM":case "abcN":
                    //                    case "abcO":case "abcP":case "abcQ":case "abcR":case "abcS":case "abcT":case "abcU":
                    //                    case "abcV":case "abcW":case "abcX":case "abcY":case "abcZ":
                    //                    {
                    //                        firstRecognition=phrase;
                    //                        string str1=phrase;
                    //                        str1=str1.ToUpper();
                    //                        keybd_event((byte)(str1[0]),0,0,0);	//key down
                    //                        keybd_event((byte)(str1[0]),0,2,0);	//key up
                    //                        break;
                    //                    }
                    //                    case "At":
                    //                    {
                    //                        keybd_event((byte)Keys.LShiftKey,0,0,0);	//key down
                    //                        keybd_event((byte)Keys.D2,0,0,0);			//key down
                    //                        keybd_event((byte)Keys.D2,0,2,0);			//key up
                    //                        keybd_event((byte)Keys.LShiftKey,0,2,0);	//key up
                    //                        break;
                    //                    }
                    //                    case "UnderLine":
                    //                    {
                    //                        keybd_event((byte)Keys.LShiftKey,0,0,0);	//key down
                    //                        keybd_event((byte)Keys.OemMinus,0,0,0);		//key down
                    //                        keybd_event((byte)Keys.OemMinus,0,2,0);		//key up
                    //                        keybd_event((byte)Keys.LShiftKey,0,2,0);	//key up
                    //                        break;
                    //                    }
                    //                    case "Dash":
                    //                    {
                    //                        keybd_event((byte)Keys.Subtract,0,0,0);		//key down
                    //                        keybd_event((byte)Keys.Subtract,0,2,0);		//key up
                    //                        break;
                    //                    }
                    //                    case "Dot":
                    //                    {
                    //                        keybd_event(190,0,0,0);	//key down
                    //                        keybd_event(190,0,2,0);	//key up
                    //                        break;
                    //                    }
                    //                    case "BackSlash":
                    //                    {
                    //                        keybd_event((byte)Keys.Divide,0,0,0);	//key down
                    //                        keybd_event((byte)Keys.Divide,0,2,0);	//key up
                    //                        break;
                    //                    }
                    //                    case "AlphabeticStateNo":
                    //                    {
                    //                        //delete the first letter
                    //                        keybd_event((byte)Keys.Back,0,0,0);	//key down
                    //                        keybd_event((byte)Keys.Back,0,2,0);	//key up

                    //                        //write the replacement letter
                    //                        string str1=firstRecognition;

                    //                        //fix miss recognition
                    //                        switch(firstRecognition)
                    //                        {
                    //                            case "a": str1="h"; break;
                    //                            case "b": str1="d"; break;
                    //                            case "c": str1="t"; break;
                    //                            case "d": str1="p"; break;
                    //                            case "f": str1="x"; break;
                    //                            case "h": str1="f"; break;
                    //                            case "m": str1="n"; break;
                    //                            case "n": str1="l"; break;
                    //                            case "l": str1="m"; break;
                    //                            case "p": str1="v"; break;
                    //                            case "u": str1="q"; break;
                    //                            case "v": str1="t"; break;
                    //                            case "e": str1="b"; break;
                    //                            case "j": str1="k"; break;
                    //                        }

                    //                        firstRecognition=str1;
                    //                        str1=str1.ToUpper();

                    //                        keybd_event((byte)(str1[0]),0,0,0);	//key down
                    //                        keybd_event((byte)(str1[0]),0,2,0);	//key up
                    //                        break;
                    //                    }
                    //                    case "mediaLibrary":
                    //                    {
                    //                        speechCommand="videoKey";
                    //                        executeCommand();//causes Form1 to execute the macro named speechCommand
                    //                        loadGrammarFile("XMLMediaLibrary.xml");


                    ////						StringBuilder mnuStr=new StringBuilder(50);
                    //                        foreach(DataRow dr in Form1.myMedia.getMedia().Tables[0].Rows)
                    //                        {
                    ////							add categories to grammar
                    //                            if(dr["category"].ToString().Length>0)
                    //                            {
                    //                                command1.ruleName="category-"+ dr["category"].ToString();
                    //                                command1.phrase= "category "+ dr["category"].ToString();

                    //                                if(!phraseList.Contains(command1))
                    //                                {
                    //                                    phraseList.Add(command1);
                    //                                    Tools.WriteToFile(Tools.errorFile,"+++++++  adding category "+ dr["category"].ToString());
                    //                                }
                    //                            }
                    ////							if(dr["title"].ToString().Length>0)
                    ////							{
                    ////								command1.ruleName="title-"+ dr["title"].ToString();
                    ////								command1.phrase=dr["title"].ToString();
                    ////
                    ////								if(!phraseList.Contains(command1))
                    ////								{
                    ////									phraseList.Add(command1);
                    ////									Tools.WriteToFile(Tools.errorFile,"+++++++  adding title "+ dr["title"].ToString());
                    ////								}
                    ////							}
                    //                        }						


                    //                        //add the phraseList to grammar
                    //                        SAPIGrammarFromArrayList(phraseList);
                    //                        break;	
                    //                    }
                    //                    case "TV":
                    //                    { 
                    //                        if(accuracy>10)
                    //                        {
                    //                            speechCommand="tvKey";
                    //                            executeCommand();

                    //                            loadGrammarFile("XMLTV.xml");
                    //                            foreach(string tvCategory in Form1.ListingsOrganizer.AllCategories)
                    //                            {
                    //    //							add categories to grammar
                    //                                if(tvCategory.Length>0)
                    //                                {
                    //                                    command1.ruleName="TVCategorySearch";
                    //                                    command1.phrase= "category "+ tvCategory;

                    //                                    if(!phraseList.Contains(command1))
                    //                                    {
                    //                                        phraseList.Add(command1);
                    //                                        Tools.WriteToFile(Tools.errorFile,"+++++++  adding TV category "+ tvCategory);
                    //                                    }
                    //                                }
                    //                            }	
                    //                            //add the phraseList to grammar
                    //                            SAPIGrammarFromArrayList(phraseList);
                    //                        }
                    //                        else
                    //                            Tools.WriteToFile(Tools.errorFile,"not accurate enough");
                    //                        break;
                    //                    }
                    //                    case "playPause":
                    //                    { 
                    //                        Tools.WriteToFile(Tools.errorFile,"found play/pause");
                    //                        if(accuracy>30)
                    //                        {
                    //                            Form1.myMediaLibrary.scrollTimer.Enabled=false;
                    //                            speechCommand="Play/Pause";

                    //                            if(Form1.GUIState!=guiState.video)
                    //                            {
                    ////								speechCommand="shuffle";
                    //                                loadGrammarFile("XMLVideoPlayer.xml");
                    //                            }							
                    //                            executeCommand();
                    //                        }
                    //                        break;
                    //                    }
                    //                    case "Mute":
                    //                    { 

                    ////						speechCommand="mute";
                    ////							executeCommand();
                    //                        break;
                    //                    }
                    //                    case "stop":
                    //                    { 

                    //                        speechCommand="Stop";
                    //                        executeCommand();
                    //                        break;
                    //                    }
                    //                    case "volumeDown":
                    //                    { 

                    ////						speechCommand="tvKey";
                    ////							executeCommand();
                    //                        break;
                    //                    }
                    //                    case "volumeUp":
                    //                    { 

                    ////						speechCommand="tvKey";
                    ////							executeCommand();
                    //                        break;
                    //                    }
                    //                    case "Next":
                    //                    { 

                    ////						speechCommand="tvKey";
                    ////							executeCommand();
                    //                        break;
                    //                    }
                    //                    case "Back":
                    //                    { 

                    ////						speechCommand="tvKey";
                    ////							executeCommand();
                    //                        break;
                    //                    }
                    //                    case "Rewind":
                    //                    { 

                    ////						speechCommand="tvKey";
                    ////							executeCommand();
                    //                        break;
                    //                    }
                    //                    case "FF":
                    //                    { 

                    ////						speechCommand="tvKey";
                    ////							executeCommand();
                    //                        break;
                    //                    }
                    //                    case "king":
                    //                        if(accuracy>=5)
                    //                        {
                    //                            if (menuItem14.Checked==true)
                    //                            {
                    //                                Tools.WriteToFile(Tools.errorFile,"getting ready to speak");
                    //                                agent1.Speak("Chad is the master","");
                    //                                Tools.WriteToFile(Tools.errorFile,"spoke");
                    //                                //							SpeechVoiceSpeakFlags SpFlags = SpeechVoiceSpeakFlags.SVSFlagsAsync;
                    //                                //							SpVoice Voice = new SpVoice();
                    //                                //
                    //                                //							Voice.Speak("The time is bla bla bla", SpFlags);
                    //                                //
                    //                                //							Voice.WaitUntilDone(Timeout.Infinite);
                    //                            }
                    //                            else
                    //                                MessageBox.Show("Chad is the Master!");
                    //                        }
                    //                        break;
                    //                    case "stacy":
                    //                        if(accuracy>=5)
                    //                        {
                    //                            if (menuItem14.Checked==true)
                    //                                agent1.Speak("Stacy of course!","");
                    //                            else
                    //                                MessageBox.Show("Stacy of course!");
                    //                        }
                    //                        break;
                    //                    case "turnOnAgent":
                    //                        try
                    //                        {
                    ////							axAgent1.Characters.Load("agentAssistant",appPath+"speech\\vocab\\characters\\wartnose.acs");
                    ////							agent1=axAgent1.Characters.Character("agentAssistant");
                    ////							agent1.Left=(short)this.Left;
                    ////							agent1.Top=(short)this.Top;

                    //                            //show character
                    //                            agent1.Show(false);
                    ////							agent1.Play("Greet");
                    //                            //				agent1.Play("Explain");
                    //                            //				agent1.Speak("I'm under your control.","");
                    //                            //				agent1.Play("Acknowledge");
                    //                            //				agent1.Speak("just activate me when ever you want...","");
                    //                            agent1.Play("Wave");
                    ////							agent1.Hide(false);
                    //                        }
                    //                        catch(Exception ex)
                    //                        {
                    //                            Tools.WriteToFile(Tools.errorFile,"path: "+ appPath+"speech\\vocab\\characters\\wartnose.acs");
                    //                            Tools.WriteToFile(Tools.errorFile,ex.ToString());
                    //                        }
                    //                        break;
                    //                    case "turnOffAgent":
                    //                        //Only if agent enabled
                    //                        if (menuItem14.Checked==true)
                    //                        {
                    //                            //animate character
                    //                            agent1.Play("Wave");
                    //                            agent1.StopAll("");
                    //                            agent1.Hide(false);
                    //                        }
                    //                        break;
                    //                    case "titleSearch"://they are search through the currently selected category for a title
                    //                        speechCommand="titleSearch()";						
                    //                        executeCommand();

                    //                        loadGrammarFile("XMLMediaLibrary.xml");

                    //                        //clear any existing rules
                    ////						grammar.Reset(409);
                    ////						Tools.WriteToFile(Tools.errorFile,"reset grammar");

                    //                        foreach(DataRowView dr in Form1.myMedia.dsMedia.Tables[0].DefaultView)
                    //                        {
                    //                            //							add titles to grammar
                    //                            if(dr["title"].ToString().Length>0)
                    //                            {
                    //                                command1.ruleName="title-"+ dr["title"].ToString();
                    //                                command1.phrase=dr["title"].ToString();

                    //                                if(!phraseList.Contains(command1))
                    //                                {
                    //                                    phraseList.Add(command1);
                    //                                    Tools.WriteToFile(Tools.errorFile,"+++++++  adding title "+ dr["title"].ToString());
                    //                                }
                    //                            }
                    //                        }

                    ////						grammar.Rules.Commit();

                    //                        //add the phraseList to grammar
                    //                        SAPIGrammarFromArrayList(phraseList);
                    //                        Tools.WriteToFile(Tools.errorFile,"added rules");
                    //                        break;
                    //                    case "scroll":
                    //                        if(phrase=="stop")
                    //                            Form1.myMediaLibrary.scrollTimer.Enabled=false;
                    //                        else
                    //                        {
                    //                            Form1.myMediaLibrary.scrollDirection=System.Text.RegularExpressions.Regex.Replace(phrase,"scroll ","");
                    //                            Form1.myMediaLibrary.scrollTimer.Enabled=true;
                    //                        }
                    //                        break;
                    //                    case "scan":
                    //                        if(accuracy>5)
                    //                        {
                    //                            if(phrase=="stop")
                    //                                Form1.myTVViewer.scanChannelTimer.Enabled=false;
                    //                            else
                    //                            {
                    //                                Form1.myTVViewer.channelScanDirection=System.Text.RegularExpressions.Regex.Replace(phrase,"scan ","");
                    //                                Form1.myTVViewer.scanChannelTimer.Enabled=true;
                    //                            }
                    //                        }
                    //                        else
                    //                            Tools.WriteToFile(Tools.errorFile,"not accurate enough");
                    //                        break;
                    //                    case "futureAirDates":
                    //                        Form1.myTVViewer.executeKeyStrokes("Next");
                    //                        break;
                    //                    case "curentShowInformation":
                    //                        Form1.myTVViewer.executeKeyStrokes("Enter");
                    //                        break;
                    //                    case "previousChannel":
                    //                        Form1.myTVViewer.executeKeyStrokes("previousChannel");
                    //                        break;
                    //                    case "channelUp":
                    //                        break;
                    //                    case "channelDown":
                    //                        break;
                    //                    case "TVCategorySearch":
                    //                        Form1.myTVViewer.categorySearch(System.Text.RegularExpressions.Regex.Replace(phrase,"category ",""));
                    //                        break;
                    //                    default: //else press the key
                    //                    {
                    //                        if(accuracy>10)
                    //                        {
                    //                            Tools.WriteToFile(Tools.errorFile,"-------------------  didn't find macro for "+ e.PhraseInfo.Rule.Name);
                    //                            if(e.PhraseInfo.Rule.Name.IndexOf("titleSearch-")>0)//this is a title search add titles to grammar
                    //                            {
                    //                                MessageBox.Show("found title "+ e.PhraseInfo.Rule.Name);
                    //                                //							Form1.myMediaLibrary.grammarList
                    //                            }
                    //                            speechCommand=e.PhraseInfo.Rule.Name;						
                    //                            executeCommand();
                    //                        }
                    //                        break;
                    //                    }
                    //                }
                    #endregion

                    speechCommand = e.PhraseInfo.Rule.Name;
                    executeCommand(heardPhrase);
                }
			}
            else //if not recognized ...
			{
                if (accuracy < accuracyMinLimit)
                {
                    lblRecognitionDisplay.BackColor = Color.Red;
                    lblRecognitionDisplay.Visible = true;
                    heardPhrase = null;
                    //Tools.WriteToFile(Tools.errorFile,"------------------"+ accuracy.ToString() +"%  didn't recognize "+ phrase);
                    //Only if agent enabled
                    if (menuItem14.Checked == true)
                    {
                        //animate character
                        agent1.Play("Decline");
                    }
                }
			}
		}

		/// <summary>
		/// objRecoContext event
		/// launched every time there is change in the mic volume
		/// and updates the progressBar1.value
		/// </summary>
		/// <param name="e">current mic volume</param>
		public void RecoContext_VUMeter(int StreamNumber, object StreamPosition, int e)
		{
			MicVolume.Value=e;
		}

        /// <summary>
        /// Handles anything that needs to be done when speech is recognized
        /// </summary>
        public void SpeechRecognized()
        {
            recognitionDisplayTimer.Enabled=true;

			lblRecognitionDisplay.BackColor=Color.Green;
			lblRecognitionDisplay.Visible=true;
			lblRecognitionDisplay.BringToFront();
        }

		#region private void frmMain_Load(object sender, System.EventArgs e)
		private void frmMain_Load(object sender, System.EventArgs e)
		{
			//app path
			appPath=Application.StartupPath+@"\";	//to use from the debuger "\..\..\"

			//initialize SAPI objects
			initSAPI();

			//loading initial grammar
            loadGrammarFile(_currentGrammarFile);

			menuItem14.Checked=false;
			//reposition frmMain
//			this.Left=(short)(Screen.GetBounds(this).Width-Width);
//			this.Top=(short)(Screen.GetBounds(this).Height-Height-50);

			//loading character
			try
			{
				axAgent1.Characters.Load("agentAssistant",appPath+"config\\speech\\vocab\\characters\\wartnose.acs");
				agent1=axAgent1.Characters.Character("agentAssistant");

				//reposition character
//				agent1.Left=(short)this.Left;
				agent1.Left=(short)Convert.ToInt32(this.Right-128);
//				agent1.Top=(short)this.Top;
				agent1.Top=(short)Convert.ToInt32(this.Bottom-192);

				//Deactivate AutoPopupMenu, we'll use our context menu ...
				agent1.AutoPopupMenu=false;
			}
			catch
			{
                Tools.WriteToFile(Tools.errorFile, "sctv load character didn't load path: " + appPath + "config\\speech\\vocab\\characters\\wartnose.acs");
//				MessageBox.Show("Error loading character\nThe agent will be disabled...","Error - character");
//				menuItem14.Checked=false;
			}

			//Only if agent enabled
			if (menuItem14.Checked==true)
			{
				//show character
				agent1.Show(false);
//				agent1.Play("Greet");
//				agent1.Play("Explain");
//				agent1.Speak("I'm under your control.","");
//				agent1.Play("Acknowledge");
//				agent1.Speak("just activate me when ever you want...","");
//				agent1.Play("Wave");
//				agent1.Hide(false);
			}
			
			//hide the program from the taskbar
			this.ShowInTaskbar=false;
		}
		#endregion

		#region private void axAgent1_ClickEvent(object sender, AxAgentObjects._AgentEvents_ClickEvent e)
		/// <summary>
		/// because agent1 dos not support context menu
		/// we show it in the main form in the clicked place
		/// </summary>
		private void axAgent1_ClickEvent(object sender, AxAgentObjects._AgentEvents_ClickEvent e)
		{
			if (e.button==2)
			{
				Point pnt=new Point(e.x-this.Left,e.y-this.Top);
				contextMenu1.Show(this,pnt);
			}
		}
		#endregion

		/// <summary>
		/// hooking menu and adding all items to the grammar
		/// </summary>
		/// <param name="hMnu">Menu handle</param>
		private void hookMenu(IntPtr hMnu)
		{
			//reset grammar
			loadGrammarFile("XMLActivated.xml");

			int mnuCnt=GetMenuItemCount(hMnu);
			int mnuStrCnt=0;

			if (mnuCnt!=0)
			{
				//add menu to grammar
				int i;
				command command1;
				StringBuilder mnuStr=new StringBuilder(50);
				ArrayList phraseList=new ArrayList();

				for (i=0;i<mnuCnt;i++)
				{
					//get sting from menu ... to mnuString
					GetMenuString(hMnu,i,mnuStr,50,-1);

					//make sure its not a separator
					if (mnuStr.ToString()!="")
					{
						//save in commnd1.ruleName only the underlined letter
						command1.ruleName=mnuStr.ToString();
						command1.ruleName=command1.ruleName[command1.ruleName.IndexOf('&')+1].ToString();

						//save in command1.phrase the word (without &)
						command1.phrase=mnuStr.ToString();
						if (command1.phrase.IndexOf('&')!=-1)
							command1.phrase=command1.phrase.Remove(command1.phrase.IndexOf('&'),1);
						//also delete every thing after tab ...
						if (command1.phrase.IndexOf('\t')!=-1)
							command1.phrase=command1.phrase.Remove(command1.phrase.IndexOf('\t'),command1.phrase.Length-command1.phrase.IndexOf('\t'));

						//or ...
						if (command1.phrase.IndexOf('.')!=-1)
							command1.phrase=command1.phrase.Remove(command1.phrase.IndexOf('.'),command1.phrase.Length-command1.phrase.IndexOf('.'));

						phraseList.Add(command1);

						//also count actual menu (without separators)
						mnuStrCnt++;
					}
				}

				//add the phraseList (menu) to grammar
				SAPIGrammarFromArrayList(phraseList);

				//notify user
                //label1.Text="Menu... "+mnuStrCnt.ToString();
			}
		}

		#region private void hookSubmenu(string menuLetter)
		/// <summary>
		/// hook submenu and add all items to grammar
		/// </summary>
		/// <param name="menuLetter">the letter after "&" that activate the menu</param>
		private void hookSubmenu(string menuLetter)
		{
			//check if there is any menu and hook it
			IntPtr hWnd=GetForegroundWindow();
			IntPtr hMnu=GetMenu(hWnd);
			int mnuCnt=GetMenuItemCount(hMnu);
			StringBuilder mnuStr=new StringBuilder(50);
			IntPtr hSubMnu;
			string str;
			int i=0;

			if (mnuCnt!=0)
			{
				//find menu position
				for (i=0;i<mnuCnt;i++)
				{
					//get sting from menu ... to mnuString
					GetMenuString(hMnu,i,mnuStr,50,-1);
					str=mnuStr.ToString();
					str=str[str.IndexOf('&')+1].ToString();

					//check if this is the menu ...
					if (menuLetter==str)
					{
						//get submenu
						hSubMnu=GetSubMenu(hMnu,i);

						//deal with it as a menu
						hookMenu(hSubMnu);                        

						//item found, we don't need to check other menu items
						break;
					}
				}
			}
		}
		#endregion

		#region Drag Functions

		/// <summary>
		/// activate timer for start tracking agent location
		/// </summary>
		private void axAgent1_DragStart(object sender, AxAgentObjects._AgentEvents_DragStartEvent e)
		{
            timer1.Enabled=true;
		}

		/// <summary>
		/// deactivate timer and stop tracking
		/// </summary>
		private void axAgent1_DragComplete(object sender, AxAgentObjects._AgentEvents_DragCompleteEvent e)
		{
			timer1.Enabled=false;
		}

		/// <summary>
		/// locate agent position and update the main form according to it
		/// </summary>
		private void timer1_Tick_1(object sender, System.EventArgs e)
		{
			//reposition frmMain
			this.Left=agent1.Left;
			this.Top=agent1.Top;
		}
		#endregion

		#region Menu Functions
		/// <summary>
		/// "exit" pressed from context menu
		/// </summary>
		private void menuItem2_Click(object sender, System.EventArgs e)
		{
			//close the program
			Close();
		}

		/// <summary>
		/// "Stop listen"/"Start listen" pressed from context menu
		/// </summary>
		private void menuItem3_Click(object sender, System.EventArgs e)
		{
			if (contextMenu1.MenuItems[0].Text=="Stop listen")
			{
				contextMenu1.MenuItems[0].Text="Start listen";
                //label1.ForeColor=Color.Red;
                //label2.ForeColor=Color.Red;

				//pause dictation
				grammar.State=SpeechLib.SpeechGrammarState.SGSDisabled;
			}
			else
			{
				contextMenu1.MenuItems[0].Text="Stop listen";
                //label1.ForeColor=Color.White;
                //label2.ForeColor=Color.White;

				//resume dictation
				grammar.State=SpeechLib.SpeechGrammarState.SGSEnabled;
			}
		}

		/// <summary>
		/// "Add favirites" pressed from context menu
		/// </summary>
		private void menuItem4_Click(object sender, System.EventArgs e)
		{
			//stop listen (only if it not already)
			if (menuItem3.Text=="Stop listen")
			{
				menuItem3_Click(sender,e);
			}

//			frmAddFavorites frm1=new frmAddFavorites();
			// send file name to the form
//			frm1.Tag=appPath+"XMLFavorites.xml";
//			frm1.Show();
		}

		/// <summary>
		/// "Change character" pressed from context menu
		/// </summary>
		private void menuItem6_Click(object sender, System.EventArgs e)
		{
			if (openFileDialog1.ShowDialog()==DialogResult.OK)
			{
				//Only if agent enabled
				if (menuItem14.Checked==true)
				{
					//unload first character
					axAgent1.Characters.Unload("agentAssistant");
				}

				//loading new character
				try
				{
					axAgent1.Characters.Load("agentAssistant",openFileDialog1.FileName);
					agent1=axAgent1.Characters.Character("agentAssistant");

					//reposition character
					agent1.Left=(short)this.Left;
					agent1.Top=(short)this.Top;

					//show character
					agent1.Show(false);
					agent1.Play("Greet");
					agent1.Play("Explain");
//					agent1.Speak("I'm under your control.","");
					agent1.Play("Acknowledge");
//					agent1.Speak("just activate me when ever you want...","");
					agent1.Play("Wave");
					agent1.Hide(false);

					//update context menu
					menuItem14.Checked=true;

					//loading initial grammar
					loadGrammarFile("XMLDeactivated.xml");

					//notify user
                    //label1.Text="Deactivated";
                    //label2.Text="Accuracy 0%";
				}
				catch
				{
                    Tools.WriteToFile(Tools.errorFile, "path: " + appPath + "config\\speech\\vocab\\characters\\merlin.acs");
					MessageBox.Show("Error loading character\nThe agent will be disabled...","Error - character");
					menuItem14.Checked=false;
				}
			}
		}

		/// <summary>
		/// show accuracy change form
		/// </summary>
		private void menuItem7_Click(object sender, System.EventArgs e)
		{
			frmAccuracyChange1=new frmAccuracyChange();
			frmAccuracyChange1.Tag=accuracyMinLimit;
			//also catch the closed event to see result returned
			frmAccuracyChange1.Closed+=new EventHandler(frmAccuracyChange1_Closed);
			frmAccuracyChange1.Show();
		}

		/// <summary>
		/// check the result returned after closing frmAccuracyChange1
		/// if result != -1 change accuracyLimit
		/// </summary>
		private void frmAccuracyChange1_Closed(object sender, System.EventArgs e)
		{
			if ((int)frmAccuracyChange1.Tag!=-1)
				accuracyMinLimit=(int)frmAccuracyChange1.Tag;
		}

		/// <summary>
		/// User training wizard
		/// </summary>
		private void menuItem8_Click(object sender, System.EventArgs e)
		{
			object str1="";

			if (objRecoContext.Recognizer.IsUISupported("UserTraining",ref str1)==true)
			{
				//stop listen (only if it not already)
				if (menuItem3.Text=="Stop listen")
				{
					menuItem3_Click(sender,e);
				}

				objRecoContext.Recognizer.DisplayUI((int)this.Handle,"SR","UserTraining",ref str1);
			}
			else
				MessageBox.Show("User training wizard not supported !","ERROR - activating wizard");
		}

		/// <summary>
		/// Mic training wizard
		/// </summary>
		private void menuItem9_Click(object sender, System.EventArgs e)
		{
			object str1="";

			if (objRecoContext.Recognizer.IsUISupported("MicTraining",ref str1)==true)
			{
				//stop listen (only if it not already)
				if (menuItem3.Text=="Stop listen")
				{
					menuItem3_Click(sender,e);
				}

				objRecoContext.Recognizer.DisplayUI((int)this.Handle,"SR","MicTraining",ref str1);
			}
			else
				MessageBox.Show("Mic training wizard not supported !","ERROR - activating wizard");
		}

		/// <summary>
		/// Change user profile
		/// </summary>
		private void menuItem10_Click(object sender, System.EventArgs e)
		{
			//reset
			initSAPI();

			//			frmProfileChange1=new frmProfileChange();
			//			frmProfileChange1.Closed+=new EventHandler(frmProfileChange1_Closed);
			//			//send objRecoContext
			//			frmProfileChange1.Tag=objRecoContext;
			//			frmProfileChange1.Show();
		}

		/// <summary>
		/// Profile proporties wizard
		/// </summary>
		private void menuItem13_Click(object sender, System.EventArgs e)
		{
			object str1="";

			if (objRecoContext.Recognizer.IsUISupported("RecoProfileProperties",ref str1)==true)
			{
				//stop listen (only if it not already)
				if (menuItem3.Text=="Stop listen")
				{
					menuItem3_Click(sender,e);
				}

				objRecoContext.Recognizer.DisplayUI((int)this.Handle,"SR","RecoProfileProperties",ref str1);
			}
			else
				MessageBox.Show("Profile Properties wizard not supported !","ERROR - activating wizard");
		}

		private void menuItem14_Click(object sender, System.EventArgs e)
		{
			if (menuItem14.Checked==true)
			{
				menuItem14.Checked=false;
				agent1.StopAll("");
				agent1.Hide(false);
			}
			else
			{
				if (agent1!=null)
				{
					menuItem14.Checked=true;
					agent1.Show(false);
				}
				else
					MessageBox.Show("There is no agent loaded...\nYou will need to load agent first.","Error - character");
			}
		}

		#endregion
		
		#region Timer Functions

		#region private void timer2_Tick(object sender, System.EventArgs e)
		/// <summary>
		/// activating "press key" and simulate press every 0.5 sec
		/// </summary>
		private void timer2_Tick(object sender, System.EventArgs e)
		{
			keybd_event(keyHolding,0,0,0);	//key down
			keybd_event(keyHolding,0,2,0);	//key up
		}
		#endregion
		
		#region private void listeningTimer_Tick(object sender, System.EventArgs e)
		private void listeningTimer_Tick(object sender, System.EventArgs e)
		{
			deactivateListener();
		}
		#endregion
		
		#region private void recognitionDisplayTimer_Tick(object sender, System.EventArgs e)
		private void recognitionDisplayTimer_Tick(object sender, System.EventArgs e)
		{
			lblRecognitionDisplay.BackColor=Color.Blue;
		}
		#endregion

		#endregion

		private void deactivateListener()
		{
			try
			{
				listeningTimer.Enabled=false;
                //if(Form1.GUIState!=guiState.mediaLibrary)// it is always listening in mediaLibrary mode unless you specifically shut it off
                //{
                    //recognitionDisplayTimer.Enabled=false;
                    //isListernerActive=false;
                    //lblRecognitionDisplay.Visible=false;
                    //lblRecognitionDisplay.BackColor=Color.Blue;
				
                    ////load grammar
                    //loadGrammarFile("XMLDeactivated.xml");

                    ////notify user
                    ////label1.Text="Deactivated";
                    //if (menuItem14.Checked==true)
                    //    agent1.Think("Thinking...");
                //}
			}
			catch(Exception ex)
			{
				Tools.WriteToFile(Tools.errorFile,ex.ToString());
			}
		}
		
		#region form closing functions to load grammar
		/// <summary>
		/// load grammar after closing form
		/// must be done here (not in frmAbout and not in recognition event)
		/// because it could be closed by speech or by pressing button
		/// </summary>
		private void frmAbout1_Closed(object sender, System.EventArgs e)
		{
			deactivateListener();
		}

		/// <summary>
		/// load grammar after closing form
		/// must be done here (not in frmCommands and not in recognition event)
		/// because it could be closed by speech or by pressing button
		/// </summary>
		private void frmCommands1_Closed(object sender, System.EventArgs e)
		{
			//load previous grammar
			loadGrammarFile(previousGrammar);
			frmCommands1=null;
		}

		/// <summary>
		/// load grammar after closing form
		/// must be done here (not in frmFavorites and not in recognition event)
		/// because it could be closed by speech or by pressing button
		/// </summary>
		private void frmFavorites1_Closed(object sender, System.EventArgs e)
		{
			//load grammar
			loadGrammarFile("XMLActivated.xml");
//			frmFavorites1=null;

			//notify user
            //label1.Text="Activated";
		}

		/// <summary>
		/// only after closing form we can laod grammar again
		/// </summary>
		private void frmProfileChange1_Closed(object sender, System.EventArgs e)
		{
			deactivateListener();
		}
		#endregion

        /// <summary>
        /// Keep form on top
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void speechRecognition_Leave(object sender, EventArgs e)
        {
            this.TopMost = true;
        }

        private void menuItem16_Click(object sender, EventArgs e)
        {
            menuItem6.Checked = !menuItem6.Checked;

            MicVolume.Visible = menuItem6.Checked;
        }
	}
}
