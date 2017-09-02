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
        private string camerasConfigPath = Application.StartupPath + "\\config\\Cameras.xml";
        private string sdpBasePath = "C:\\movies\\vlc\\vlc.sdp";
        //private string jtvChannelName = "clickey";
        private string vlcPath = @"C:\Program Files\VideoLAN\VLC\vlc.exe";
        private string jtVlcPath = @"C:\utilities\programming\projects\vlc\jtvlc-win-0.41\jtvlc.exe";
        //stream key is found here http://www.justin.tv/broadcast/advanced
        //private string jtvStreamKey = "live_7292794_3tdoPRg1";
        StringBuilder procOutput;
        int jtVlcRetries = 0;
        private SCTV.TabCtlEx tabControl;
        private string maxPortNumberInUse = "1224";
        private bool showProcWindows = true;

        public Broadcast()
        {
            try
            {
                if (!SCTVActivation.isActivated())
                {
                    MessageBox.Show("This product needs to be activated.  Call Support");

                    this.Close();
                    return;
                }
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, ex.Source + " : " + ex.Message);
            }

            InitializeComponent();

            //create tabcontrol
            tabControl = new TabCtlEx();
            tabControl.Anchor = AnchorStyles.Top;
            tabControl.Anchor = AnchorStyles.Right;
            tabControl.Anchor = AnchorStyles.Left;

            this.tabControl.ItemSize = new System.Drawing.Size(113, 24);
            this.tabControl.Location = new System.Drawing.Point(0, 95);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(665, 24);
            this.tabControl.CanCloseAllTabs = true;
            tabControl.SelectedIndexChanged += new EventHandler(tabControl_SelectedIndexChanged);
            tabControl.OnClose += new TabCtlEx.OnHeaderCloseDelegate(tabControl_OnClose);

            this.Controls.Add(tabControl);

            tabControl.Visible = false;
            tabControl.BringToFront();

            //getNewCameras();
            getCameraInfo();
            populateCBCameras();

            wbCameraDisplay.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(wbCameraDisplay_DocumentCompleted);
            wbCameraDisplay.Navigate(Application.StartupPath + "\\UIMessages\\default.htm");

            //TODO:
            //read output of vlc to make sure it is streaming before starting jtvlc
            //auto login for the embed code??
            //auto play for embed code
            //start processes on a new thread
        }

        void tabControl_OnClose(object sender, CloseEventArgs e)
        {
            JustinCamera closedCamera = (JustinCamera)tabControl.TabPages[e.TabIndex].Tag;

            //kill processes for this camera
            if (closedCamera.ProcVLC != null && !closedCamera.ProcVLC.HasExited)
                closedCamera.ProcVLC.Kill();

            if (closedCamera.ProcJtv != null && !closedCamera.ProcJtv.HasExited)
                closedCamera.ProcJtv.Kill();

            broadcastingCameras.Remove(closedCamera);

            tabControl.TabPages.RemoveAt(e.TabIndex);
        }

        void wbCameraDisplay_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            btnBroadcast.Text = "Broadcast";
            btnBroadcast.Enabled = true;
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
            if (cbCameras.Text != "No Video input devices found")
            {
                wbCameraDisplay.Navigate(Application.StartupPath +"\\UIMessages\\Loading.htm");

                btnBroadcast.Text = "Loading...";

                tss.Text = "Loading Camera";

                btnBroadcast.Enabled = false;

                JustinCamera selectedCamera = (JustinCamera)cbCameras.SelectedItem;

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
                    //select tab for selectedCamera
                    tabControl.SelectTab(selectedCamera.Name);
                }

                displayCameraBroadcast(selectedCamera);
            }
            else
                MessageBox.Show("There are no cameras attached to the system");
        }

        private void broadcast(JustinCamera camera)
        {
            //VLC
            //file:
            //"C:\Program Files\VideoLAN\VLC\vlc.exe" -vvv -I rc "file:///C:\movies\Magnetic Track.flv" --sout="#transcode{venc=x264{keyint=60,idrint=2},vcodec=h264,vb=300,scale=1,acodec=mp4a,ab=32,channels=2,samplerate=22050}:rtp{dst=127.0.0.1,port=1234,sdp=file://C:\movies\vlc\vlc.sdp}
            //camera:
            //"C:\Program Files\VideoLAN\VLC\vlc.exe" dshow:// --sout="#transcode{venc=x264{keyint=60,idrint=2},vcodec=h264,vb=300,acodec=mp4a,ab=32,channels=2,samplerate=22050}:rtp{dst=127.0.0.1,port=1234,sdp=file://C:\movies\vlc\vlc.sdp}" 


            //jtVLC
            //"C:\utilities\programming\projects\vlc\jtvlc-win-0.41\jtvlc.exe" clickey live_7292794_3tdoPRg1 "C:\movies\vlc\vlc.sdp" -d

            camera.IsBroadcasting = true;

            broadcastingCameras.Add(camera);

            procOutput = new StringBuilder("");

            startVlc(ref camera);

            startJtVlc(ref camera);
        }

        void myprocess_OutputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                procOutput.AppendLine(e.Data);

                System.IO.File.AppendAllText(@"C:\movies\vlc\outputdata.txt", e.Data);
            }
        }

        void proc_OutputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                procOutput.AppendLine(e.Data);

                System.IO.File.AppendAllText(@"C:\movies\vlc\outputdata.txt", e.Data);
            }
        }

        private void displayCameraBroadcast(JustinCamera camera)
        {
            try
            {
                tss.Text = "Displaying broadcast of " + camera.Name;
                tss.Invalidate();

                WebClient client = new WebClient();
                String flashObject = client.DownloadString("http://api.justin.tv/api/channel/embed/" + camera.ChannelName + "?volume=50&publisher_guard=sctv&height=595&width=653&autoPlay=true&watermark_position=top_right");

                flashObject = flashObject.Replace("auto_play=false", "autoplay=true");//doesn't do anything

                wbCameraDisplay.DocumentText = flashObject;

                tabControl.Visible = true;
            }
            catch (Exception ex)
            {

                throw;
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
                JustinCamera testCamera = new JustinCamera("test Cam1", "live_7292794_3tdoPRg1", "clickey", null, null);
                cbCameras.Items.Add(testCamera);

                JustinCamera testCamera2 = new JustinCamera("Justin1010", "live_15399080_jAc7w3mH4Q0IQCW1QStNgjXyekc98f", "justin1010", null, null);
                cbCameras.Items.Add(testCamera2);

                //cbCameras.Items.Add("No Video input devices found");
                cbCameras.SelectedIndex = 0;
            }

            //if (filters != null && filters.VideoInputDevices.Count > 0)
            //{
            //    for (int x = 0; x < filters.VideoInputDevices.Count; x++)
            //        cbCameras.Items.Add(filters.VideoInputDevices[x].Name);

            //    cbCameras.SelectedItem = filters.VideoInputDevices[0].Name;
            //    //lblStatus.Text = (string)cbCameras.SelectedItem;
            //}
            //else
            //{
            //    cbCameras.Items.Add("No Video input devices found");
            //    cbCameras.SelectedIndex = 0;
            //}
        }

        private void startVlc(ref JustinCamera camera)
        {
            //look at video mosaic for vlc 
            //http://wiki.videolan.org/Mosaic
            try
            {
                tss.Text = "Starting capture on " + camera.Name;

                

                System.Diagnostics.Process tempVlcProcess = new System.Diagnostics.Process();

                int tempPort = 1234;

                if (int.TryParse(maxPortNumberInUse, out tempPort))
                    tempPort = tempPort + 10;

                maxPortNumberInUse = tempPort.ToString();

                camera.Port = tempPort.ToString();
                camera.SDPPath = sdpBasePath.Replace("vlc.sdp", "vlc_" + camera.Port + ".sdp");

                tempVlcProcess.StartInfo.FileName = vlcPath;

                //broadcast a camera
                //tempVlcProcess.StartInfo.Arguments = "dshow://  :dshow-vdev=\"IBM PC Camera\" :dshow-adev=";
                //tempVlcProcess.StartInfo.Arguments = "dshow://  :dshow-vdev=\""+ camera.FilterName +"\" :dshow-adev=";
                //tempVlcProcess.StartInfo.Arguments = "dshow:// :dshow-vdev=\"" + camera.FilterName + "\" :dshow-adev= --sout=\"#transcode{venc=x264{keyint=60,idrint=2},vcodec=h264,vb=300,acodec=mp4a,ab=32,channels=2,samplerate=22050}:rtp{dst=127.0.0.1,port=" + tempPort.ToString() + ",ttl=10,sdp=file://" + camera.SDPPath + "}\"";

                //works
                //tempVlcProcess.StartInfo.Arguments = "dshow:// --sout=\"#transcode{venc=x264{keyint=60,idrint=2},vcodec=h264,vb=300,acodec=mp4a,ab=32,channels=2,samplerate=22050}:rtp{dst=127.0.0.1,port=" + tempPort.ToString() + ",ttl=10,sdp=file://" + camera.SDPPath + "}\"";
                //works

                //broadcast a file
                tempVlcProcess.StartInfo.Arguments = "-vvv -I rc \"file:///C:\\movies\\WHATS_NEW_SCOOBY_DOO_VOL_3.avi\" --sout=\"#transcode{venc=x264{keyint=60,idrint=2},vcodec=h264,vb=300,scale=1,acodec=mp4a,ab=32,channels=2,samplerate=22050}:rtp{dst=127.0.0.1,port=" + tempPort.ToString() + ",ttl=10,sdp=file://" + camera.SDPPath + "}\"";

                if (!showProcWindows)//hide process windows
                {
                    tempVlcProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                    tempVlcProcess.StartInfo.CreateNoWindow = true;
                }

                tempVlcProcess.StartInfo.UseShellExecute = false;

                //tempVlcProcess.StartInfo.RedirectStandardOutput = true;
                //tempVlcProcess.StartInfo.RedirectStandardError = true;
                //tempVlcProcess.EnableRaisingEvents = true;

                //tempVlcProcess.OutputDataReceived += new System.Diagnostics.DataReceivedEventHandler(myprocess_OutputDataReceived);

                camera.ProcVLC = tempVlcProcess;

                tempVlcProcess.Start();

                //tempVlcProcess.BeginOutputReadLine();

                tempVlcProcess.WaitForExit(5000);
            }
            catch (Exception ex)
            {
                
                throw;
            }
            

            




            ////***working
            //vlcProcess = new System.Diagnostics.Process();

            ////lbStatus.Items.Add("Starting install on " + destination + " for " + installerPath);

            //vlcProcess.StartInfo.FileName = vlcPath;

            ////myprocess.StartInfo.Arguments = @"\\" + destination + " -u " + userName + " -p " + password + " msiexec.exe -i \"" + installerPath + "\" /quiet /l* \"" + logPathBase + destination + "_INSTALL.txt\" SilentInstall=true";
            //vlcProcess.StartInfo.Arguments = "-vvv -I rc \"file:///C:\\movies\\SMOKIN_ACES_2_ASSASSINS_BALL.avi\" --sout=\"#transcode{venc=x264{keyint=60,idrint=2},vcodec=h264,vb=300,scale=1,acodec=mp4a,ab=32,channels=2,samplerate=22050}:rtp{dst=127.0.0.1,port=1234,sdp=file://" + sdpPath + "}\"";

            //vlcProcess.StartInfo.CreateNoWindow = true;
            //vlcProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            ////myprocess.StartInfo.RedirectStandardOutput = true;
            ////myprocess.StartInfo.RedirectStandardError = true;
            //vlcProcess.StartInfo.UseShellExecute = true;
            ////myprocess.EnableRaisingEvents = true;

            ////myprocess.Destination = destination;
            ////myprocess.InstallerPath = installerPath;
            ////myprocess.LogPath = logPathBase + destination + "_INSTALL.txt";
            ////myprocess.UserName = userName;
            ////myprocess.Password = password;

            ////myprocess.OutputDataReceived += new System.Diagnostics.DataReceivedEventHandler(myprocess_OutputDataReceived);

            //vlcProcess.Start();

            ////myprocess.BeginOutputReadLine();

            //vlcProcess.WaitForExit(5000);

            ////***working





            //System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo(vlcPath);
            //psi.RedirectStandardOutput = true;
            ////psi.RedirectStandardError = true;
            //psi.CreateNoWindow = true;
            //psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            //psi.UseShellExecute = false;
            ////psi.Arguments = "-vvv -I rc \"file:///C:\\movies\\SMOKIN_ACES_2_ASSASSINS_BALL\\SMOKIN_ACES_2_ASSASSINS_BALL.avi\" --sout=\"#transcode{venc=x264{keyint=60,idrint=2},vcodec=h264,vb=300,scale=1,acodec=mp4a,ab=32,channels=2,samplerate=22050}:rtp{dst=127.0.0.1,port=1234,sdp=file://C:\\movies\\vlc\\vlc.sdp}\"";
            //psi.Arguments = "-vvv -I rc \"file:///C:\\movies\\SMOKIN_ACES_2_ASSASSINS_BALL.avi\" --sout=\"#transcode{venc=x264{keyint=60,idrint=2},vcodec=h264,vb=300,scale=1,acodec=mp4a,ab=32,channels=2,samplerate=22050}:rtp{dst=127.0.0.1,port=1234,sdp=file://" + sdpPath + "}\"";


            //System.Diagnostics.Process proc = new System.Diagnostics.Process();

            //proc.StartInfo = psi;
            //proc.EnableRaisingEvents = true;
            //proc.OutputDataReceived += new System.Diagnostics.DataReceivedEventHandler(proc_OutputDataReceived);
            //proc.Start();

            //System.Diagnostics.Process proc = System.Diagnostics.Process.Start(psi);
            //System.IO.StreamReader myOutput = proc.StandardOutput;
            //System.IO.StreamReader myOutputError = proc.StandardError;


            //proc.BeginOutputReadLine();

            //proc.WaitForExit();
            //proc.Close();




            //if (proc.HasExited)
            //{
            //    //string output = myOutput.ReadToEnd();
            //    string errorOutput = myOutputError.ReadToEnd();
            //}
        }

        private void startJtVlc(ref JustinCamera camera)
        {
            try
            {
                tss.Text = "Starting broadcast of " + camera.Name;

                //jtVLC
                System.Diagnostics.ProcessStartInfo psi2 = new System.Diagnostics.ProcessStartInfo(jtVlcPath);

                psi2.RedirectStandardOutput = true;
                psi2.RedirectStandardError = true;

                if (!showProcWindows)//hide process windows
                {
                    psi2.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;

                    psi2.CreateNoWindow = true;
                }

                psi2.UseShellExecute = false;
                psi2.Arguments = camera.ChannelName + " " + camera.StreamKey + " \"" + camera.SDPPath + "\" -d";

                System.Diagnostics.Process tempJtvProcess = new System.Diagnostics.Process();

                tempJtvProcess = System.Diagnostics.Process.Start(psi2);
                //System.IO.StreamReader myOutput2 = proc2.StandardOutput;
                //System.IO.StreamReader myOutput2Error = proc2.StandardError;

                tempJtvProcess.OutputDataReceived += new System.Diagnostics.DataReceivedEventHandler(tempJtvProcess_OutputDataReceived);
                tempJtvProcess.ErrorDataReceived += new System.Diagnostics.DataReceivedEventHandler(tempJtvProcess_ErrorDataReceived);
                tempJtvProcess.BeginOutputReadLine();
                
                camera.ProcJtv = tempJtvProcess;

                tempJtvProcess.WaitForExit(2000);

                //if (tempJtvProcess.HasExited)
                //{
                //    //string output2 = myOutput2.ReadToEnd();
                //    //string output2Error = myOutput2Error.ReadToEnd();

                //    //procOutput.AppendLine(output2);

                //    if (jtVlcRetries < 4)
                //    {
                //        jtVlcRetries++;
                //        startJtVlc(ref camera);
                //    }
                //}







                ////***working

                ////jtVLC
                //System.Diagnostics.ProcessStartInfo psi2 = new System.Diagnostics.ProcessStartInfo(jtVlcPath);
                ////psi2.RedirectStandardOutput = true;
                ////psi2.RedirectStandardError = true;
                //psi2.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                //psi2.CreateNoWindow = true;
                //psi2.UseShellExecute = true;
                //psi2.Arguments = camera.ChannelName + " " + camera.StreamKey + " \"" + sdpPath + "\" -d";
                //jtvProcess = System.Diagnostics.Process.Start(psi2);
                ////System.IO.StreamReader myOutput2 = proc2.StandardOutput;
                ////System.IO.StreamReader myOutput2Error = proc2.StandardError;

                //jtvProcess.WaitForExit(2000);

                //if (jtvProcess.HasExited)
                //{
                //    //string output2 = myOutput2.ReadToEnd();
                //    //string output2Error = myOutput2Error.ReadToEnd();

                //    //procOutput.AppendLine(output2);

                //    if (jtVlcRetries < 4)
                //    {
                //        jtVlcRetries++;
                //        startJtVlc(camera);
                //    }
                //}

                ////***working
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, ex.Message);
            }
        }

        void tempJtvProcess_ErrorDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            System.IO.File.AppendAllText("c:\\jtvlcOutput.txt", e.Data);
            //throw new Exception("The method or operation is not implemented.");
        }

        void tempJtvProcess_OutputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            System.IO.File.AppendAllText("c:\\jtvlcOutput.txt", e.Data);
            //throw new Exception("The method or operation is not implemented.");
        }

        private void Broadcast_FormClosing(object sender, FormClosingEventArgs e)
        {
            //kill any processes still running
            foreach (JustinCamera camera in broadcastingCameras)
            {
                if (camera.ProcVLC != null && !camera.ProcVLC.HasExited)
                    camera.ProcVLC.Kill();

                if (camera.ProcJtv != null && !camera.ProcJtv.HasExited)
                    camera.ProcJtv.Kill();
            }
        }
    }
}