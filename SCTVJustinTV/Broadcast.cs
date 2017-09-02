using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using DirectX.Capture;
using System.Collections;
using System.Net;
using SCTV;
using SCTechUtilities;
using SCTVObjects;
using System.Threading;

namespace SCTVJustinTV
{
    public partial class Broadcast : Form
    {
        private static Filters filters = null;//new Filters();
        private ArrayList cameras = new ArrayList();
        private ArrayList broadcastingCameras = new ArrayList();

        XmlDocument xmlCameras;
        XmlDocument xmlChannels;
        private string channelConfigPath = Application.StartupPath + "\\config\\Channels.xml";
        private string camerasConfigPath = Application.StartupPath + "\\config\\Cameras.xml";
        private string sdpBasePath = Application.StartupPath + "\\vlc\\vlc.sdp";
        //private string sdpBasePath = "C:\\movies\\vlc\\vlc.sdp";
        //private string jtvChannelName = "clickey";
        //private string vlcPath = @"C:\Program Files\VideoLAN\VLC\vlc.exe";
        //private string jtVlcPath = @"C:\utilities\programming\projects\vlc\jtvlc-win-0.41\jtvlc.exe";
        private string vlcPath = Application.StartupPath + "\\vlc\\vlc.exe";
        private string jtVlcPath = Application.StartupPath + "\\jtvlc\\jtvlc.exe";
        //stream key is found here http://www.justin.tv/broadcast/advanced
        //private string jtvStreamKey = "live_7292794_3tdoPRg1";
        private SCTV.TabCtlEx tabControl;
        private string maxPortNumberInUse = "1224";
        private bool showProcWindows = true;
        StringBuilder jtvlcOutput;
        StringBuilder jtvlcErrorOutput;
        int jtvlcRestarts = 0;
        int jtvlcRestartThreshold = 5;
        int jtvlcMessages = 0;
        int jtvlcMessageThreshold = 5;
        int waitForBroadcast = 0;
        int waitForBroadcastThreshold = 15;
        int waitForVLC = 0;
        int waitForVLCThreshold = 20;
        string fileToBroadcast = "";
        bool testing = false;
        bool displayBroadcast = false;
        ArrayList portNumbersInUse = new ArrayList();
        int broadcastLengthLimit = 100;//minutes before restarting broadcast
        bool displayVLC = false;

        delegate void CloseTabs();
        public delegate void StartNewProcesses(JustinCamera camera);
        public delegate void KillCameraProcesses(JustinCamera camera);

        public Broadcast()
        {
            try
            {
                //if (!SCTVActivation.isActivated())
                //{
                //    MessageBox.Show("This product needs to be activated.  Call Support");

                //    this.Close();
                //    return;
                //}
                //else //create config and logs folders
                //{
                    if (!System.IO.Directory.Exists(Application.StartupPath + "\\config"))
                        System.IO.Directory.CreateDirectory(Application.StartupPath + "\\config");

                    if (!System.IO.Directory.Exists(Application.StartupPath + "\\logs"))
                        System.IO.Directory.CreateDirectory(Application.StartupPath + "\\logs");
                //}
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, ex.Source + " : " + ex.Message);
            }

            InitializeComponent();

            //create tabcontrol
            tabControl = new TabCtlEx();
            tabControl.Anchor = AnchorStyles.Top;
            //tabControl.Anchor = AnchorStyles.Right;
            //tabControl.Anchor = AnchorStyles.Left;

            this.tabControl.ItemSize = new System.Drawing.Size(113, 24);
            this.tabControl.Location = new System.Drawing.Point(0, 100);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(690, 24);
            this.tabControl.CanCloseAllTabs = true;
            tabControl.SelectedIndexChanged += new EventHandler(tabControl_SelectedIndexChanged);
            tabControl.OnClose += new TabCtlEx.OnHeaderCloseDelegate(tabControl_OnClose);

            this.Controls.Add(tabControl);

            tabControl.Visible = false;
            tabControl.BringToFront();

            getNewCameras();
            getCameraInfo();
            populateCBCameras();

            wbCameraDisplay.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(wbCameraDisplay_DocumentCompleted);
            wbCameraDisplay.Navigate(Application.StartupPath + "\\UIMessages\\default.htm");

            fileToBroadcast = System.Configuration.ConfigurationManager.AppSettings["fileToBroadcast"];
            bool.TryParse(System.Configuration.ConfigurationManager.AppSettings["showProcWindows"], out showProcWindows);
            bool.TryParse(System.Configuration.ConfigurationManager.AppSettings["testing"], out testing);
            bool.TryParse(System.Configuration.ConfigurationManager.AppSettings["displayBroadcast"], out displayBroadcast);
            int.TryParse(System.Configuration.ConfigurationManager.AppSettings["broadcastLengthLimit"], out broadcastLengthLimit);
            int.TryParse(System.Configuration.ConfigurationManager.AppSettings["jtvlcMessageThreshold"], out jtvlcMessageThreshold);
            bool.TryParse(System.Configuration.ConfigurationManager.AppSettings["displayVLC"], out displayVLC);

            ////you can loop through and read all the key & key values
            //foreach (string key in System.Configuration.ConfigurationSettings.AppSettings)
            //{
            //    //the below will return the key name
            //    MessageBox.Show(key.ToString());
            //    //the below will return the key value
            //    MessageBox.Show(ConfigurationSettings.AppSettings[key.ToString()].ToString());
            //}

            if (displayBroadcast)
            {
                this.Width = 701;
                this.Height = 908;

                btnRefresh.Visible = true;
            }

            //check form size
            Rectangle formLocation = this.DesktopBounds;
            Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;
            if (formLocation.Bottom > workingArea.Bottom)
            {
                this.Height = workingArea.Height - 100;
                this.Top = workingArea.Top;
            }


            //TODO:
            //auto login for the embed code?? - doesn't work
            //auto play for embed code?? - doesn't work
            //add config setting for testing
                //auto run test avi file
                //check for output from jtvlc
                //check for output from vlc
                //show proc windows
        }

        void tabControl_OnClose(object sender, CloseEventArgs e)
        {
            try
            {
                JustinCamera closedCamera = (JustinCamera)tabControl.TabPages[e.TabIndex].Tag;

                killCameraProcesses(closedCamera);

                //broadcastingCameras.Remove(closedCamera);

                tabControl.TabPages.RemoveAt(e.TabIndex);
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, ex.Message);
            }
        }

        void wbCameraDisplay_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            btnBroadcast.Text = "Broadcast";
            btnBroadcast.Enabled = true;
            btnView.Enabled = true;

            if(broadcastingCameras.Count > 0)
                btnRefresh.Enabled = true;
        }

        void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl.TabCount > 0)
                displayCameraBroadcast((JustinCamera)tabControl.SelectedTab.Tag);
            else
            {
                wbCameraDisplay.Navigate(Application.StartupPath + "\\UIMessages\\Default.htm");

                tabControl.Visible = false;
            }
        }

        private void btnBroadcast_Click(object sender, EventArgs e)
        {
            if (cbCameras.Text != "No Video input devices found" || testing || (fileToBroadcast != null && fileToBroadcast.Length > 0))
            {
                JustinCamera selectedCamera;

                wbCameraDisplay.Navigate(Application.StartupPath +"\\UIMessages\\Loading.htm");

                btnBroadcast.Text = "Loading...";

                tss.Text = "Loading Camera";

                btnBroadcast.Enabled = false;

                btnView.Enabled = false;

                btnRefresh.Enabled = false;

                if (testing || (fileToBroadcast != null && fileToBroadcast.Length > 0))
                {
                    selectedCamera = new JustinCamera("test", "testing", "test channel", null, null);
                }
                else
                    selectedCamera = (JustinCamera)cbCameras.SelectedItem;

                if (!broadcastingCameras.Contains(selectedCamera))
                {
                    broadcast(selectedCamera);

                    if (tabControl.TabCount == 0 || tabControl.TabPages[0].Text.Trim() != "No Cameras Broadcasting")//create new tab
                    {
                        TabPageEx newCamera = new TabPageEx();
                        newCamera.Text = selectedCamera.Name;
                        newCamera.Name = selectedCamera.Name;
                        newCamera.Tag = selectedCamera;
                        tabControl.TabPages.Add(newCamera);
                        tabControl.SelectedTab = newCamera;
                    }
                    else//reset tab text to show camera name
                    {
                        tabControl.TabPages[0].Text = selectedCamera.Name;
                        tabControl.TabPages[0].Name = selectedCamera.Name;
                        tabControl.TabPages[0].Tag = selectedCamera;

                        tabControl.TabPages[0].Text = selectedCamera.Name;
                        tabControl.TabPages[0].Tag = selectedCamera;
                    }
                }
                else
                {
                    if (tabControl.TabCount > 0)
                    {
                        //select tab for selectedCamera
                        tabControl.SelectTab(selectedCamera.Name);
                    }
                }

                //wait for the broadcast to start
                while (!System.IO.File.Exists(Application.StartupPath + "\\logs\\tempJtvOutput_" + selectedCamera.ChannelName + ".txt") && waitForBroadcast < waitForBroadcastThreshold)
                {
                    waitForBroadcast++;

                    Thread.Sleep(4000);
                }

                //only display the camera if it is broadcasting
                if (System.IO.File.Exists(Application.StartupPath + "\\logs\\tempJtvOutput_" + selectedCamera.ChannelName + ".txt"))
                {
                    //give everything time to get started and streaming
                    Thread.Sleep(4000);

                    displayCameraBroadcast(selectedCamera);
                }
                else
                {
                    Tools.WriteToFile(Tools.errorFile, "Didn't find jtvlc file \"" + Application.StartupPath + "\\logs\\tempJtvOutput_" + selectedCamera.ChannelName + ".txt\"");

                    MessageBox.Show("An Error occurred while connecting to the server." + Environment.NewLine + "Try your broadcast again.");
                }
            }
            else
                MessageBox.Show("There are no cameras attached to the system");
        }

        private void broadcast(JustinCamera camera)
        {
            if (broadcastingCameras.Count >= 2)
            {
                MessageBox.Show("You have exceeded the maximum number of cameras allowed by this version." + Environment.NewLine + "Contact support to upgrade.");
                
                return;
            }

            camera.IsBroadcasting = true;

            broadcastingCameras.Add(camera);

            //start new thread for vlc and jtvlc
            //Thread broadcastThread = new Thread(startBroadcastThread);
            //camera.CurrentThread = broadcastThread;
            //broadcastThread.Start(camera);


            //StartNewProcesses newProcs = new StartNewProcesses(startProcesses);
            //IAsyncResult tag = newProcs.BeginInvoke(camera, null, null);

            startBroadcasting(camera);
        }

        private void startBroadcasting(object camera)
        {
            //make sure the camera isn't being viewed locally
            if (((JustinCamera)camera).CameraWindow != null)
                ((JustinCamera)camera).CameraWindow.Close();

            startVlc((JustinCamera)camera);

            this.BringToFront();
            this.Focus();

            startJtVlc((JustinCamera)camera);

            this.BringToFront();
            this.Focus();
            this.Refresh();
            this.Update();
        }

        private void displayCameraBroadcast(JustinCamera camera)
        {
            try
            {
                if (displayBroadcast)
                {
                    tss.Text = "Displaying broadcast of " + camera.Name;
                    tss.Invalidate();

                    WebClient client = new WebClient();
                    String flashObject = client.DownloadString("http://api.justin.tv/api/channel/embed/" + camera.ChannelName + "?volume=50&publisher_guard=sctv&height=595&width=653&autoPlay=true&watermark_position=top_right");

                    flashObject = flashObject.Replace("auto_play=false", "autoplay=true");//doesn't do anything

                    wbCameraDisplay.DocumentText = flashObject;

                    tabControl.Visible = true;
                }
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, ex.Message);
            }
        }

        /// <summary>
        /// iterate filters and add any new cameras to cameras.xml
        /// </summary>
        private void getNewCameras()
        {
            try
            {
                filters = new Filters();
                xmlCameras = new XmlDocument();

                if (System.IO.File.Exists(camerasConfigPath))
                    xmlCameras.Load(camerasConfigPath);

                //iterate filters
                for (int x = 0; x < filters.VideoInputDevices.Count; x++)
                {
                    //search cameras.xml for the filter
                    XmlNode monikerNode = null;

                    if (xmlCameras != null && xmlCameras["Cameras"] != null)
                    {
                        foreach (XmlNode camera in xmlCameras["Cameras"])
                        {
                            monikerNode = camera.SelectSingleNode("monikerString");
                            if (monikerNode != null && monikerNode.InnerText.Trim().Length > 0 && monikerNode.InnerText.Trim() != filters.VideoInputDevices[x].MonikerString)
                                monikerNode = null;
                            else
                                break;
                        }
                    }

                    if (monikerNode == null)//didn't find camera
                    {
                        //add camera xml to cameras.xml

                        string newCameraName = "";

                        NewCameraInfo newCameraInfo = new NewCameraInfo();
                        newCameraInfo.CameraName = filters.VideoInputDevices[x].Name;

                        newCameraInfo.ShowDialog(this);

                        newCameraName = newCameraInfo.CameraName;

                        XmlNode camerasNode = xmlCameras.SelectSingleNode("Cameras");

                        if (camerasNode == null)
                        {
                            XmlNode baseNode = xmlCameras.CreateNode(XmlNodeType.Element, "Cameras", "");

                            xmlCameras.AppendChild(baseNode);
                        }

                        XmlNode newCameraNode = xmlCameras.CreateNode(XmlNodeType.Element, "camera", "");

                        XmlNode nameNode = xmlCameras.CreateNode(XmlNodeType.Element, "name", "");

                        if (newCameraName.Trim().Length == 0)//use filter name for camera
                            nameNode.InnerText = filters.VideoInputDevices[x].Name;
                        else//use user given name
                            nameNode.InnerText = newCameraName.Trim();

                        XmlNode monikerStringNode = xmlCameras.CreateNode(XmlNodeType.Element, "monikerString", "");
                        monikerStringNode.InnerText = filters.VideoInputDevices[x].MonikerString;

                        XmlNode frameSizeNode = xmlCameras.CreateNode(XmlNodeType.Element, "frameSize", "");

                        XmlNode frameRateNode = xmlCameras.CreateNode(XmlNodeType.Element, "frameRate", "");

                        XmlNode channelNode = xmlCameras.CreateNode(XmlNodeType.Element, "channelName", "");
                        channelNode.InnerText = "clickey";

                        XmlNode streamkeyNode = xmlCameras.CreateNode(XmlNodeType.Element, "streamKey", "");
                        streamkeyNode.InnerText = "live_7292794_3tdoPRg1";

                        XmlNode filterNameNode = xmlCameras.CreateNode(XmlNodeType.Element, "filterName", "");
                        filterNameNode.InnerText = filters.VideoInputDevices[x].Name;

                        newCameraNode.AppendChild(nameNode);
                        newCameraNode.AppendChild(monikerStringNode);
                        newCameraNode.AppendChild(frameSizeNode);
                        newCameraNode.AppendChild(frameRateNode);
                        newCameraNode.AppendChild(channelNode);
                        newCameraNode.AppendChild(streamkeyNode);
                        newCameraNode.AppendChild(filterNameNode);

                        xmlCameras["Cameras"].AppendChild(newCameraNode);

                        xmlCameras.Save(camerasConfigPath);
                    }
                }
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, ex.Message);
            }
        }

        /// <summary>
        /// Reads xml file to get info on available cameras
        /// </summary>
        private void getCameraInfo()
        {
            //match list of filters to xml to find available cameras and their properties
            try
            {
                XmlDocument xmlCameras = new XmlDocument();
                if (System.IO.File.Exists(camerasConfigPath))
                    xmlCameras.Load(camerasConfigPath);
                else
                {
                    getNewCameras();

                    xmlCameras.Load(camerasConfigPath);
                }

                XmlNode nameNode;
                XmlNode monikerStringNode;
                XmlNode channelNameNode;
                XmlNode streamKeyNode;
                XmlNode filterNameNode;
                string name = "";
                string moniker = "";
                string channelName = "";
                string streamKey = "";
                string filterName = "";

                cameras.Clear();

                foreach (XmlNode camera in xmlCameras["Cameras"])
                {
                    name = "";
                    moniker = "";
                    channelName = "";
                    streamKey = "";
                    filterName = "";

                    nameNode = camera.SelectSingleNode("name");
                    monikerStringNode = camera.SelectSingleNode("monikerString");
                    channelNameNode = camera.SelectSingleNode("channelName");
                    streamKeyNode = camera.SelectSingleNode("streamKey");
                    filterNameNode = camera.SelectSingleNode("filterName");

                    if (monikerStringNode != null && monikerStringNode.InnerText.ToString().Trim().Length > 0)
                    {
                        if (filters == null)
                            filters = new Filters();

                        moniker = monikerStringNode.InnerText.ToString().Trim();

                        if (nameNode != null && nameNode.InnerText.ToString().Trim().Length > 0)
                            name = nameNode.InnerText.ToString().Trim();

                        if (channelNameNode != null && channelNameNode.InnerText.ToString().Trim().Length > 0)
                            channelName = channelNameNode.InnerText.ToString().Trim();

                        if (streamKeyNode != null && streamKeyNode.InnerText.ToString().Trim().Length > 0)
                            streamKey = streamKeyNode.InnerText.ToString().Trim();

                        if (filterNameNode != null && filterNameNode.InnerText.ToString().Trim().Length > 0)
                            filterName = filterNameNode.InnerText.ToString().Trim();

                        //iterate filters and see if this camera is on
                        for (int x = 0; x < filters.VideoInputDevices.Count; x++)
                        {
                            if (filters.VideoInputDevices[x].MonikerString == moniker) // || filters.AudioInputDevices[x].MonikerString == moniker)//this is our camera
                            {
                                JustinCamera justinCamera = new JustinCamera();
                                justinCamera.Name = name;
                                justinCamera.ChannelName = channelName;
                                justinCamera.StreamKey = streamKey;
                                justinCamera.FilterName = filterName;

                                if (x < filters.VideoInputDevices.Count)
                                    justinCamera.VideoDevice = filters.VideoInputDevices[x];

                                if (x < filters.AudioInputDevices.Count)
                                    justinCamera.AudioDevice = filters.AudioInputDevices[x];

                                cameras.Add(justinCamera);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Tools.WriteToFile(Tools.errorFile, e.Message);
            }
        }

        /// <summary>
        /// fill cbCameras with the available cameras
        /// </summary>
        private void populateCBCameras()
        {
            if (cameras.Count > 0)
            {
                cbCameras.Items.Clear();

                foreach (JustinCamera camera in cameras)
                {
                    if (camera.Name.Trim().Length > 0)
                        cbCameras.Items.Add(camera);
                }

                cbCameras.SelectedIndex = 0;
            }
            else
            {
                //JustinCamera testCamera = new JustinCamera("test Cam1", "live_7292794_3tdoPRg1", "clickey", null, null);
                //cbCameras.Items.Add(testCamera);

                //JustinCamera testCamera2 = new JustinCamera("Justin1010", "live_15399080_jAc7w3mH4Q0IQCW1QStNgjXyekc98f", "justin1010", null, null);
                //cbCameras.Items.Add(testCamera2);

                cbCameras.Items.Clear();
                cbCameras.Items.Add("No Video input devices found");
                cbCameras.SelectedIndex = 0;
            }
        }

        private void getChannels()
        {
            XmlNode channelsNode;

            if (System.IO.File.Exists(channelConfigPath))
            {
                xmlChannels = new XmlDocument();
                xmlChannels.Load(channelConfigPath);
            }
            else
                xmlChannels = new XmlDocument();

            channelsNode = xmlChannels.SelectSingleNode("Channels");

            if (channelsNode == null)
            {
                XmlNode baseNode = xmlChannels.CreateNode(XmlNodeType.Element, "Channels", "");

                xmlChannels.AppendChild(baseNode);

                //channel 1
                XmlNode newChannelNode = xmlChannels.CreateNode(XmlNodeType.Element, "channel", "");

                XmlNode nameNode = xmlChannels.CreateNode(XmlNodeType.Element, "name", "");
                nameNode.InnerText = "clickey";

                XmlNode passwordNode = xmlChannels.CreateNode(XmlNodeType.Element, "password", "");
                passwordNode.InnerText = "sctv";

                XmlNode streamKeyNode = xmlChannels.CreateNode(XmlNodeType.Element, "streamKey", "");
                streamKeyNode.InnerText = "live_7292794_3tdoPRg1";

                XmlNode inUseNode = xmlChannels.CreateNode(XmlNodeType.Element, "inUse", "");
                inUseNode.InnerText = "false";

                newChannelNode.AppendChild(nameNode);
                newChannelNode.AppendChild(passwordNode);
                newChannelNode.AppendChild(streamKeyNode);
                newChannelNode.AppendChild(inUseNode);

                xmlChannels["Channels"].AppendChild(newChannelNode);
                

                //channel 2
                newChannelNode = xmlChannels.CreateNode(XmlNodeType.Element, "channel", "");

                nameNode = xmlChannels.CreateNode(XmlNodeType.Element, "name", "");
                nameNode.InnerText = "justin1010";

                passwordNode = xmlChannels.CreateNode(XmlNodeType.Element, "password", "");
                passwordNode.InnerText = "sctv";

                streamKeyNode = xmlChannels.CreateNode(XmlNodeType.Element, "streamKey", "");
                streamKeyNode.InnerText = "live_15399080_jAc7w3mH4Q0IQCW1QStNgjXyekc98f";

                inUseNode = xmlChannels.CreateNode(XmlNodeType.Element, "inUse", "");
                inUseNode.InnerText = "false";

                newChannelNode.AppendChild(nameNode);
                newChannelNode.AppendChild(passwordNode);
                newChannelNode.AppendChild(streamKeyNode);
                newChannelNode.AppendChild(inUseNode);

                xmlChannels["Channels"].AppendChild(newChannelNode);


                xmlChannels.Save(channelConfigPath);

            }
        }

        private void startVlc(JustinCamera camera)
        {
            //look at video mosaic for vlc 
            //http://wiki.videolan.org/Mosaic
            try
            {
                tss.Text = "Starting capture on " + camera.Name;
                Tools.WriteToFile(Tools.errorFile, "Starting capture (vlc) on " + camera.Name);

                System.Diagnostics.Process tempVlcProcess = new System.Diagnostics.Process();

                int tempPort = 1234;

                if (int.TryParse(maxPortNumberInUse, out tempPort))
                    tempPort = tempPort + 10;

                while (portNumbersInUse.Contains(tempPort))
                    tempPort = tempPort + 10;

                maxPortNumberInUse = tempPort.ToString();

                portNumbersInUse.Add(tempPort.ToString());

                camera.Port = tempPort.ToString();
                camera.SDPPath = sdpBasePath.Replace("vlc.sdp", "vlc_" + camera.Port + ".sdp");

                tempVlcProcess.StartInfo.FileName = vlcPath;

                if (testing && (fileToBroadcast == null || fileToBroadcast.Trim().Length == 0))
                    fileToBroadcast = Application.StartupPath + "\\vlc\\test.avi";

                if (fileToBroadcast == null || fileToBroadcast.Trim().Length == 0 || !System.IO.File.Exists(fileToBroadcast))
                {
                    string transcodeString = "";
                    string displayOutputString = "";
                    string displayModeString = "";

                    //make sure the directory exists to save files to
                    System.IO.Directory.CreateDirectory(Application.StartupPath + "\\videos");

                    //delete any previous sdp files
                    System.IO.File.Delete(camera.SDPPath);

                    //--qt-display-mode={0 (Classic look), 1 (Complete look with information area), 2 (Minimal look with no menus)}
                    //             Selection of the starting mode and look



                    //displayModeString = "--qt-display-mode=2";
                    //transcodeString = "transcode{venc=x264{keyint=60,idrint=2},vcodec=h264,vb=1800,acodec=mp4a,ab=32,channels=2,samplerate=22050}";
                    ////output to rtp and sdp file
                    //displayOutputString = "dst=rtp{dst=127.0.0.1,port=" + tempPort.ToString() + ",ttl=10,sdp=file://" + camera.SDPPath + "}";
                    ////output to local display
                    //displayOutputString += ",dst=display{noaudio}";
                    ////output to file
                    ////displayOutputString += ",dst=standard{access=file,mux=ps,dst=\"" + Application.StartupPath + "\\videos\\" + DateTime.Now.Month + "-" + DateTime.Now.Day + "-" + DateTime.Now.Year + "-" + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second + "_" + camera.Name.Replace(" ", "_") + ".cel\"}";

                    //tempVlcProcess.StartInfo.Arguments = displayModeString +" dshow:// :dshow-vdev=\"" + camera.FilterName + "\" :dshow-adev= --sout=\"#"+ transcodeString +":duplicate{"+ displayOutputString +"}\"";



                    //broadcast a camera, and display while setting vlc to minimal view mode
                    tempVlcProcess.StartInfo.Arguments = "--qt-display-mode=2 dshow:// :dshow-vdev=\"" + camera.FilterName + "\" :dshow-adev= --sout=\"#transcode{venc=x264{keyint=60,idrint=2},vcodec=h264,vb=1800,acodec=mp4a,ab=32,channels=2,samplerate=22050}:duplicate{dst=rtp{dst=127.0.0.1,port=" + tempPort.ToString() + ",ttl=10,sdp=file://" + camera.SDPPath + "},dst=display{noaudio}}\"";

                    //tempVlcProcess.StartInfo.Arguments = "--qt-display-mode=2 dshow:// :dshow-vdev=\"" + camera.FilterName + "\" :dshow-adev= --sout=\"#transcode{venc=x264{keyint=60,idrint=2},vcodec=h264,vb=1800,acodec=mp4a,ab=32,channels=2,samplerate=96000}:rtp{dst=127.0.0.1,port=" + tempPort.ToString() + ",ttl=10,sdp=file://" + camera.SDPPath + "}\"";
                    //tempVlcProcess.StartInfo.Arguments = "--qt-display-mode=2 dshow:// :dshow-vdev=\"" + camera.FilterName + "\" :dshow-adev= --sout=\"#transcode{vcodec=mp4v,vb=1024,fps=30,width=320,acodec=mpga,ab=128,scale=1,channels=2,deinterlace,audio-sync}:duplicate{dst=rtp{mux=ts,dst=127.0.0.1,port=" + tempPort.ToString() + ",ttl=10,sdp=file://" + camera.SDPPath + "},dst=standard{access=file,mux=ps,dst=\"" + Application.StartupPath + "\\videos\\" + DateTime.Now.Month + "-" + DateTime.Now.Day + "-" + DateTime.Now.Year + "-" + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second + "_" + camera.Name.Replace(" ", "_") + ".cel\"},dst=display{noaudio}\"}";
                    
                    //kind of works - UI disappears sometimes
                    //tempVlcProcess.StartInfo.Arguments = "--qt-display-mode=2 --qt-start-minimized dshow:// :dshow-vdev=\"" + camera.FilterName + "\" :dshow-adev= --sout=\"#transcode{vcodec=mp4v,vb=1024,fps=30,width=320,acodec=mpga,ab=128,scale=1,channels=2,deinterlace,audio-sync}:duplicate{dst=rtp{mux=ts,dst=127.0.0.1,port=" + tempPort.ToString() + ",ttl=10,sdp=file://" + camera.SDPPath + "},dst=standard{access=file,mux=ps,dst=\"" + Application.StartupPath + "\\videos\\" + DateTime.Now.Month + "-" + DateTime.Now.Day + "-" + DateTime.Now.Year + "-" + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second + "_" + camera.Name.Replace(" ", "_") + ".cel\"},dst=display{noaudio}\"}";
                    
                    //works 3
                    //tempVlcProcess.StartInfo.Arguments = "dshow:// :dshow-vdev=\"" + camera.FilterName + "\" :dshow-adev= --sout=\"#transcode{vcodec=mp4v,vb=1024,fps=30,width=320,acodec=mpga,ab=128,scale=1,channels=2,deinterlace,audio-sync}:duplicate{dst=standard{access=file,mux=ps,dst=\"C:\\movies\\testing.mpg\"},dst=display{noaudio},dst=rtp{mux=ts,dst=127.0.0.1,port=" + tempPort.ToString() + ",ttl=10,sdp=file://" + camera.SDPPath + "}\"} --extraintf=logger -vvv";
                    //works 3

                    //works 2 - works with justin
                    //tempVlcProcess.StartInfo.Arguments = "dshow:// :dshow-vdev=\"" + camera.FilterName + "\" :dshow-adev= --sout=\"#transcode{venc=x264{keyint=60,idrint=2},vcodec=h264,vb=1800,acodec=mp4a,ab=32,channels=2,samplerate=96000}:rtp{dst=127.0.0.1,port=" + tempPort.ToString() + ",ttl=10,sdp=file://" + camera.SDPPath + "}\"";
                    //works 2

                    //works 1
                    //tempVlcProcess.StartInfo.Arguments = "dshow:// --sout=\"#transcode{venc=x264{keyint=60,idrint=2},vcodec=h264,vb=300,acodec=mp4a,ab=32,channels=2,samplerate=22050}:rtp{dst=127.0.0.1,port=" + tempPort.ToString() + ",ttl=10,sdp=file://" + camera.SDPPath + "}\"";
                    //works
                }
                else
                {
                    if (System.IO.File.Exists(fileToBroadcast))
                    {
                        //broadcast a file
                        //tempVlcProcess.StartInfo.Arguments = "-vvv -I rc \"file:///C:\\movies\\WHATS_NEW_SCOOBY_DOO_VOL_3.avi\" --sout=\"#transcode{venc=x264{keyint=60,idrint=2},vcodec=h264,vb=300,scale=1,acodec=mp4a,ab=32,channels=2,samplerate=22050}:rtp{dst=127.0.0.1,port=" + tempPort.ToString() + ",ttl=10,sdp=file://" + camera.SDPPath + "}\"";
                        tempVlcProcess.StartInfo.Arguments = "-vvv -I rc \"file:///" + fileToBroadcast + "\" --sout=\"#transcode{venc=x264{keyint=60,idrint=2},vcodec=h264,vb=1800,scale=1,acodec=mp4a,ab=32,channels=2,samplerate=96000}:rtp{dst=127.0.0.1,port=" + tempPort.ToString() + ",ttl=10,sdp=file://" + camera.SDPPath + "}\"";
                    }
                    else
                        MessageBox.Show(fileToBroadcast + " does not exist.  Check your config file and try again");
                }

                if (!showProcWindows && !testing && !displayVLC)//hide process windows
                {
                    tempVlcProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                    tempVlcProcess.StartInfo.CreateNoWindow = true;
                }

                tempVlcProcess.StartInfo.UseShellExecute = true;

                //tempVlcProcess.StartInfo.RedirectStandardOutput = true;
                //tempVlcProcess.StartInfo.RedirectStandardError = true;
                //tempVlcProcess.EnableRaisingEvents = true;

                //tempVlcProcess.OutputDataReceived += new System.Diagnostics.DataReceivedEventHandler(myprocess_OutputDataReceived);

                camera.ProcVLC = tempVlcProcess;

                tempVlcProcess.Start();

                //tempVlcProcess.BeginOutputReadLine();

                while (!System.IO.File.Exists(camera.SDPPath) && waitForVLC < waitForVLCThreshold)
                {
                    waitForVLC++;
                    Thread.Sleep(1000);
                }

                if (!System.IO.File.Exists(camera.SDPPath))
                {
                    Tools.WriteToFile(Tools.errorFile, "sdp file not created for " + camera.Name + " at " + camera.SDPPath);
                    //MessageBox.Show("Error connecting to camera." + Environment.NewLine + "Please try your broadcast again");
                }
                else
                    Tools.WriteToFile(Tools.errorFile, "sdp file created successfully for " + camera.Name + " at " + camera.SDPPath);

                Tools.WriteToFile(Tools.errorFile, "Started capture (vlc) of " + camera.Name);

                //tempVlcProcess.WaitForExit(6000);
            }
            catch (Exception ex)
            {
                
                throw;
            }
        }

        private void startJtVlc(JustinCamera camera)
        {
            try
            {
                XmlNode inUse;
                XmlNode streamKey;
                XmlNode channelName; 
                XmlNode channelPassword;
                int findFileAttempts = 0;
                int findFileThreshold = 30;

                Tools.WriteToFile(Tools.errorFile, "Starting broadcast of " + camera.Name);

                while (!System.IO.File.Exists(camera.SDPPath))
                {
                    findFileAttempts++;

                    Thread.Sleep(1000);

                    if (findFileAttempts > findFileThreshold)
                    {
                        Tools.WriteToFile(Tools.errorFile, "didn't find sdp file " + camera.SDPPath +" for camera "+ camera.Name);

                        MessageBox.Show("Error starting broadcast.  Try again. " + Environment.NewLine + "If the problem continues contact support");

                        killAllProcesses();

                        return;
                    }
                }

                tss.Text = "Starting broadcast of " + camera.Name;
                jtvlcOutput = new StringBuilder();
                jtvlcErrorOutput = new StringBuilder();

                //jtVLC
                System.Diagnostics.ProcessStartInfo psi2 = new System.Diagnostics.ProcessStartInfo(jtVlcPath);

                psi2.RedirectStandardOutput = true;
                //psi2.RedirectStandardError = true;

                if (!showProcWindows && !testing)//hide process windows
                {
                    psi2.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;

                    psi2.CreateNoWindow = true;
                }

                psi2.UseShellExecute = false;

                if (xmlChannels == null)
                    getChannels();
                
                foreach (XmlNode channel in xmlChannels["Channels"])//iterate channels and find one that is not in use
                {
                    inUse = channel.SelectSingleNode("inUse");

                    if (inUse == null || inUse.InnerText == "false")
                    {
                        streamKey = channel.SelectSingleNode("streamKey");
                        channelName = channel.SelectSingleNode("name");

                        //psi2.Arguments = camera.ChannelName + " " + camera.StreamKey + " \"" + camera.SDPPath + "\" -d";
                        psi2.Arguments = channelName.InnerText + " " + streamKey.InnerText + " \"" + camera.SDPPath + "\" -d";

                        inUse.InnerText = "true";

                        camera.ChannelName = channelName.InnerText;
                        camera.StreamKey = streamKey.InnerText;

                        break;
                    }
                }

                Tools.WriteToFile(Tools.errorFile, "jtvlc args " + psi2.FileName +" "+ psi2.Arguments);

                System.Diagnostics.Process tempJtvProcess = new System.Diagnostics.Process();
                
                tempJtvProcess = System.Diagnostics.Process.Start(psi2);
                
                tempJtvProcess.OutputDataReceived += new System.Diagnostics.DataReceivedEventHandler(tempJtvProcess_OutputDataReceived);
                tempJtvProcess.ErrorDataReceived += new System.Diagnostics.DataReceivedEventHandler(tempJtvProcess_ErrorDataReceived);
                tempJtvProcess.BeginOutputReadLine();
                
                camera.ProcJtv = tempJtvProcess;
                camera.StartTime = tempJtvProcess.StartTime.ToString();

                Tools.WriteToFile(Tools.errorFile, "Started broadcast of " + camera.Name);

                tss.Text = "Broadcasting " + camera.Name;

                //tempJtvProcess.WaitForExit(2000);
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, ex.Message);
            }
        }

        void tempJtvProcess_ErrorDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            jtvlcErrorOutput.Append(e.Data);

            System.IO.File.AppendAllText(Application.StartupPath + "\\logs\\jtvlcErrorOutput.txt", e.Data);
        }

        void tempJtvProcess_OutputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            try
            {
                if (e.Data != null)
                {
                    System.Diagnostics.Process currentProc = (System.Diagnostics.Process)sender;

                    if (e.Data.Contains("Connected to Wowza successfully"))
                        jtvlcMessages = 0;

                    //if (e.Data.Contains("ERROR"))
                    //{
                    //    //MessageBox.Show("JTVLC Error: " + e.Data);
                    //}
                    //else 

                    if (e.Data.Contains("Aborting..."))
                    {
                        if (jtvlcRestarts < jtvlcRestartThreshold)
                        {
                            //get camera for current process
                            foreach (JustinCamera camera in cameras)
                            {
                                if (camera.ChannelName == currentProc.StartInfo.Arguments.Substring(0, currentProc.StartInfo.Arguments.IndexOf(" ")))
                                {
                                    jtvlcRestarts++;

                                    Tools.WriteToFile(Application.StartupPath + "\\logs\\tempJtvOutput_" + camera.ChannelName + ".txt", "Restarting JtVLC for " + camera.ChannelName);

                                    killAllProcesses();

                                    //restart jtvlc
                                    startJtVlc(camera);

                                    break;
                                }
                            }
                        }
                    }
                    else if (!e.Data.Contains("Receiving data from VLC...") && !e.Data.Contains("Sending data to Wowza...") || jtvlcMessages < jtvlcMessageThreshold || jtvlcMessageThreshold == 0)
                    {
                        if (currentProc.StartInfo.Arguments.Length > 0)
                            Tools.WriteToFile(Application.StartupPath + "\\logs\\tempJtvOutput_" + currentProc.StartInfo.Arguments.Substring(0, currentProc.StartInfo.Arguments.IndexOf(" ")) + ".txt", e.Data);

                        jtvlcMessages++;
                    }
                }
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }
        }

        private void killAllProcesses()
        {
            foreach (JustinCamera camera in broadcastingCameras)
            {
                if (camera.ProcVLC != null && !camera.ProcVLC.HasExited)
                    camera.ProcVLC.Kill();

                foreach (System.Diagnostics.Process p in System.Diagnostics.Process.GetProcessesByName("jtvlc"))
                {
                    try
                    {
                        p.Kill();
                        p.WaitForExit(); // possibly with a timeout
                    }
                    catch (Win32Exception ex)
                    {
                        Tools.WriteToFile(ex);
                    }
                    catch (InvalidOperationException ex)
                    {
                        Tools.WriteToFile(ex);
                    }
                }
            }

            broadcastingCameras.Clear();

            closeTabs();
        }

        private void killCameraProcesses(JustinCamera camera)
        {
            try
            {
                broadcastingCameras.Remove(camera);
                
                //if(camera.CurrentThread != null)
                //    camera.CurrentThread.Abort();

                if (!InvokeRequired)
                {
                    //kill processes for this camera
                    if (camera.ProcVLC != null && !camera.ProcVLC.HasExited)
                        camera.ProcVLC.Kill();

                    if (camera.ProcJtv != null && !camera.ProcJtv.HasExited)
                    {

                        foreach (System.Diagnostics.Process p in System.Diagnostics.Process.GetProcessesByName("jtvlc"))
                        {
                            try
                            {
                                if (p.StartTime.ToString() == camera.StartTime)
                                {
                                    p.Kill();
                                    p.WaitForExit(); // possibly with a timeout
                                }
                            }
                            catch (Win32Exception ex)
                            {
                                Tools.WriteToFile(ex);
                            }
                            catch (InvalidOperationException ex)
                            {
                                Tools.WriteToFile(ex);
                            }
                        }
                    }
                }
                else
                    Invoke(new KillCameraProcesses(killCameraProcesses), new object[] { camera });

                camera.ProcVLC = null;
                camera.ProcJtv = null;

                if (xmlChannels == null)
                    getChannels();

                //set channel to inUse=false so it can be reused
                foreach (XmlNode channel in xmlChannels["Channels"])
                {
                    if (channel["name"].InnerText == camera.ChannelName)
                    {
                        channel["inUse"].InnerText = "false";
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }   
        }

        void closeTabs()
        {
            if (!InvokeRequired)
            {
                tabControl.TabPages.Clear();

                btnRefresh.Enabled = false;
            }
            else
                Invoke(new CloseTabs(closeTabs), new object[] { });
        } 

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            wbCameraDisplay.Refresh();
        }

        private void Broadcast_FormClosing(object sender, FormClosingEventArgs e)
        {
            //kill any processes still running
            killAllProcesses();
        }

        private void btnRefreshCameras_Click(object sender, EventArgs e)
        {
            getCameraInfo();
            populateCBCameras();
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            try
            {
                //make sure camera isn't broadcasting
                killCameraProcesses((JustinCamera)cbCameras.SelectedItem);

                //close camera tab
                foreach (TabPage tp in tabControl.TabPages)
                {
                    if ((JustinCamera)tp.Tag == (JustinCamera)cbCameras.SelectedItem)
                        tabControl.TabPages.Remove(tp);
                }

                if (tabControl.TabCount == 0)
                    wbCameraDisplay.Navigate(Application.StartupPath + "\\UIMessages\\default.htm");

                Thread.Sleep(1500);

                while (((JustinCamera)cbCameras.SelectedItem).ProcVLC != null && !((JustinCamera)cbCameras.SelectedItem).ProcVLC.HasExited)
                    Thread.Sleep(800);

                showCamera((JustinCamera)cbCameras.SelectedItem);
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }
        }

        /// <summary>
        /// displays the camera asked for
        /// </summary>
        /// <param name="cameraToStart"></param>
        private void showCamera(JustinCamera cameraToStart)
        {
            if (!cameraToStart.IsDisplaying || (cameraToStart.CameraWindow != null && cameraToStart.CameraWindow.IsDisposed))
            {
                CameraDisplay cameraDisplay = null;

                //if (filters.VideoInputDevices.Count > 0)
                //{
                //    if (isRunning(cameraToStart.Name))
                //        currentCameraWindow.Close();

                //int cameraWindowCount = getRunningCameraCount();

                //for (int x = 0; x < filters.VideoInputDevices.Count; x++)
                //{
                //if (filters.VideoInputDevices[x].MonikerString == cameraToStart.VideoDevice.MonikerString)//this is the camera to record
                //{
                cameraDisplay = new CameraDisplay(cameraToStart.VideoDevice, cameraToStart.AudioDevice);

                //cameraDisplay.FrameSize = getFrameSize();

                cameraDisplay.Show();

                //cameraDisplay.Left = cameraWindowCount * 450;
                //cameraDisplay.Top = 0;

                cameraDisplay.Text = cameraToStart.Name;
                cameraToStart.IsDisplaying = true;
                cameraToStart.CameraWindow = cameraDisplay;
                //currentCameraWindow = cameraDisplay;

                //lblCameraCapabilities.Text = cameraDisplay.GetCameraCapabilities;

                //break;
                //}
                //}
                //}
                //else
                //    MessageBox.Show("No Video input devices found");

                tss.Text = "Viewing Camera "+ cameraToStart.Name;
            }
            else
            {
                cameraToStart.CameraWindow.BringToFront();
                cameraToStart.CameraWindow.Focus();
            }
        }

        /// <summary>
        /// Make sure the camera info and process info are in sync
        /// </summary>
        private void syncCamerasAndProcesses()
        {
            bool foundProcess = false;
            ArrayList processesToKill = new ArrayList();

            //iterate cameras
            foreach (JustinCamera camera in broadcastingCameras)
            {
                //get vlc processes and make sure starttimes match with camera
                //if no process found remove camera from broadcastingCameras

                foundProcess = false;

                //iterate vlc processes
                foreach (System.Diagnostics.Process p in System.Diagnostics.Process.GetProcessesByName("vlc"))
                {
                    try
                    {
                        if (p.StartTime.ToString() == camera.StartTime)
                        {
                            foundProcess = true;
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Tools.WriteToFile(ex);
                    }
                }

                if (!foundProcess)
                    processesToKill.Add(camera);
            }

            //foreach (JustinCamera camera in processesToKill)
            //    killCameraProcesses(camera);
        }
        
        private void cameraTimer_Tick(object sender, EventArgs e)
        {
            syncCamerasAndProcesses();

            //iterate cameras and check start times - restart any cameras that are running longer than broadcastLengthLimit 
            foreach (JustinCamera camera in broadcastingCameras)
            {
                DateTime dtStartTime = DateTime.Now;

                if (DateTime.TryParse(camera.StartTime, out dtStartTime))
                {
                    TimeSpan dtDifference = DateTime.Now.Subtract(dtStartTime);
                    int broadcastLength = 0;

                    broadcastLength += dtDifference.Minutes;

                    if (dtDifference.Hours > 0)
                        broadcastLength += dtDifference.Hours * 60;

                    if (broadcastLength > broadcastLengthLimit)
                    {
                        //restart camera
                        killCameraProcesses(camera);

                        broadcast(camera);

                        if (tabControl.TabCount == 0 || tabControl.TabPages[0].Text.Trim() != "No Cameras Broadcasting")//create new tab
                        {
                            TabPageEx newCamera = new TabPageEx();
                            newCamera.Text = camera.Name;
                            newCamera.Name = camera.Name;
                            newCamera.Tag = camera;
                            tabControl.TabPages.Add(newCamera);
                            tabControl.SelectedTab = newCamera;
                        }
                        //else//reset tab text to show camera name
                        //{
                        //    tabControl.TabPages[0].Text = camera.Name;
                        //    tabControl.TabPages[0].Name = camera.Name;
                        //    tabControl.TabPages[0].Tag = camera;

                        //    tabControl.TabPages[0].Text = camera.Name;
                        //    tabControl.TabPages[0].Tag = camera;
                        //}
                    }
                }
            }
        }

        private void displayBroadcastToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!displayBroadcastToolStripMenuItem.Checked)
            {
                this.Width = 701;
                this.Height = 908;

                btnRefresh.Visible = true;

                displayBroadcastToolStripMenuItem.Checked = true;
            }
            else
            {
                this.Width = 447;
                this.Height = 150;

                btnRefresh.Visible = false;

                displayBroadcastToolStripMenuItem.Checked = false;  
            }
        }
    }
}