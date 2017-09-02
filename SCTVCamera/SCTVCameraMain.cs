using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
//using dshow;
//using dshow.Core;
using SCTV;
using System.Collections;
using DirectX.Capture;
using SCTVObjects;
using System.Xml;
using SCTechUtilities;

namespace SCTVCamera
{
    public partial class SCTVCameraMain : Form
    {
        //static dshow.FilterCollection cameras;
        public string grammarvocabFile = "xmlCameras.xml";
        private static ArrayList keyStrokeTracker = new ArrayList();
        private Capture capture = null;
        private Capture capture2 = null;
        private static Filters filters = new Filters();
        private string currentCameraName = "";
        private CameraDisplay currentCameraWindow;
        private SecurityCamera currentCamera;
        private string recordingPath = Application.StartupPath +"\\video\\";
        private string camerasConfigPath = Application.StartupPath +"\\config\\Cameras.xml";
        private ArrayList cameras = new ArrayList();
        XmlDocument xmlCameras;
        bool ftp = false;

        private Speakers speakers;
        SpeechRecognition speechListener;

        public SpeechRecognition SpeechListener
        {
            set
            {
                speechListener = value;
                speechListener.loadGrammarFile(grammarvocabFile);
                speechListener.executeCommand += new SpeechRecognition.HeardCommand(speechListener_executeCommand);
            }
        }

        /// <summary>
        /// The name of the vocab file to load for this form
        /// </summary>
        protected string GrammarFile
        {
            get { return grammarvocabFile; }
            set
            {
                grammarvocabFile = value;
            }
        }

        //public static dshow.FilterCollection Cameras
        //{
        //    get
        //    {
        //        if (cameras == null)
        //            cameras = new dshow.FilterCollection(FilterCategory.VideoInputDevice);

        //        return cameras;
        //    }
        //}

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //try
            //{
            //    if (!SCTVActivation.isActivated())
            //    {
            //        MessageBox.Show("This product needs to be activated.  Call Support");

            //        return;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Tools.WriteToFile(Tools.errorFile, ex.Source + " : " + ex.Message);
            //}

            SCTVCameraMain cameraMain = new SCTVCameraMain();

            //make form visible
            cameraMain.Opacity = 100;
            
            Application.Run(cameraMain);

#if debug
    //CameraAdmin cameraAdmin = new CameraAdmin();
    //cameraAdmin.Show();
#endif
        }

        public SCTVCameraMain()
        {
            InitializeComponent();

            try
            {
                //- need to handle(read/write) camera xml file to be able to give cameras attributes
                //- handle multiple cameras and recording when needed

                getNewCameras();

                getCameraInfo();

                populateCBCameras();

                populateCBFrameSize();
            }
            catch (Exception ex)
            {
                throw;
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
                    getNewCameras();

                XmlNode nameNode;
                XmlNode monikerStringNode;
                XmlNode recordPathNode;
                string name = "";
                string moniker = "";
                string recordPath = "";

                cameras.Clear();

                foreach (XmlNode camera in xmlCameras["Cameras"])
                {
                    name = "";
                    moniker = "";
                    recordPath = "";

                    nameNode = camera.SelectSingleNode("name");
                    monikerStringNode = camera.SelectSingleNode("monikerString");
                    recordPathNode = camera.SelectSingleNode("recordPath");

                    if (monikerStringNode != null && monikerStringNode.InnerText.ToString().Trim().Length > 0)
                    {
                        moniker = monikerStringNode.InnerText.ToString().Trim();

                        if (nameNode != null && nameNode.InnerText.ToString().Trim().Length > 0)
                            name = nameNode.InnerText.ToString().Trim();

                        if (recordPathNode != null && recordPathNode.InnerText.ToString().Trim().Length > 0)
                            recordPath = nameNode.InnerText.ToString().Trim();

                        //iterate filters and see if this camera is on
                        for (int x = 0; x < filters.VideoInputDevices.Count; x++)
                        {
                            if (filters.VideoInputDevices[x].MonikerString == moniker) // || filters.AudioInputDevices[x].MonikerString == moniker)//this is our camera
                            {
                                SecurityCamera securityCamera = new SecurityCamera();
                                securityCamera.Name = name;
                                securityCamera.RecordPath = recordPath;

                                if(x < filters.VideoInputDevices.Count)
                                    securityCamera.VideoDevice = filters.VideoInputDevices[x];

                                if(x < filters.AudioInputDevices.Count)
                                    securityCamera.AudioDevice = filters.AudioInputDevices[x];

                                cameras.Add(securityCamera);
                            }
                        }
                    }
                }                
            }
            catch (Exception e)
            {
                Tools.WriteToFile(Tools.errorFile, e.ToString());
            }
        }

        /// <summary>
        /// iterate filters and add any new cameras to cameras.xml
        /// </summary>
        private void getNewCameras()
        {
            try
            {
                xmlCameras = new XmlDocument();

                if(System.IO.File.Exists(camerasConfigPath))
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

                        XmlNode camerasNode = xmlCameras.SelectSingleNode("Cameras");

                        if (camerasNode == null)
                        {
                            XmlNode baseNode = xmlCameras.CreateNode(XmlNodeType.Element, "Cameras", "");

                            xmlCameras.AppendChild(baseNode);
                        }

                        XmlNode newCameraNode = xmlCameras.CreateNode(XmlNodeType.Element, "camera", "");

                        XmlNode nameNode = xmlCameras.CreateNode(XmlNodeType.Element, "name", "");
                        nameNode.InnerText = filters.VideoInputDevices[x].Name;

                        XmlNode monikerStringNode = xmlCameras.CreateNode(XmlNodeType.Element, "monikerString", "");
                        monikerStringNode.InnerText = filters.VideoInputDevices[x].MonikerString;

                        XmlNode recordPathNode = xmlCameras.CreateNode(XmlNodeType.Element, "recordPath", "");
                        recordPathNode.InnerText = "";

                        XmlNode frameSizeNode = xmlCameras.CreateNode(XmlNodeType.Element, "frameSize", "");

                        XmlNode frameRateNode = xmlCameras.CreateNode(XmlNodeType.Element, "frameRate", "");

                        newCameraNode.AppendChild(nameNode);
                        newCameraNode.AppendChild(monikerStringNode);
                        newCameraNode.AppendChild(recordPathNode);
                        newCameraNode.AppendChild(frameSizeNode);
                        newCameraNode.AppendChild(frameRateNode);

                        xmlCameras["Cameras"].AppendChild(newCameraNode);
                        xmlCameras.Save(camerasConfigPath);
                    }
                }
            }
            catch (Exception ex)
            {
                
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

                foreach (SecurityCamera camera in cameras)
                {
                    if (camera.Name.Trim().Length > 0)
                        cbCameras.Items.Add(camera);
                }

                //cbCameras.SelectedIndex = 0;
            }
            else
                MessageBox.Show("No Video input devices found");

            //if (filters.VideoInputDevices.Count > 0)
            //{
                

            //    for (int x = 0; x < filters.VideoInputDevices.Count; x++)
            //        cbCameras.Items.Add(filters.VideoInputDevices[x].Name);

            //    cbCameras.SelectedItem = filters.VideoInputDevices[0].Name;
            //    //lblStatus.Text = (string)cbCameras.SelectedItem;
            //}
            //else
            //    MessageBox.Show("No Video input devices found");
        }

        private void populateCBFrameSize()
        {
            //populate cbFrameSize
            cbFrameSize.Items.Clear();
            cbFrameSize.Items.Add("160 x 120");
            cbFrameSize.Items.Add("320 x 240");
            cbFrameSize.Items.Add("640 x 480");
            cbFrameSize.Items.Add("1024 x 768");

            cbFrameSize.SelectedIndex = 1;
            
            if (currentCameraWindow != null)
                if (cbFrameSize.Items.Contains(currentCameraWindow.FrameSize.Width + " x " + currentCameraWindow.FrameSize.Height))
                    cbFrameSize.SelectedItem = currentCameraWindow.FrameSize.Width + " x " + currentCameraWindow.FrameSize.Height;
        }

        private void startVLCCapture()
        {
            VLCCameraDisplay vlcCamera = new VLCCameraDisplay();
            vlcCamera.Record();
        }

        /// <summary>
        /// record given camera
        /// </summary>
        /// <param name="cameraToRecord"></param>
        public void RecordCamera(SecurityCamera cameraToRecord, bool enableWebInterface)
        {
            CameraDisplay cameraDisplay = null;

            if (cameraToRecord == null)
            {
                MessageBox.Show("You must select a camera to record");
                return;
            }
            
            if (filters.VideoInputDevices.Count > 0)
            {
                if (isRunning(cameraToRecord.Name))
                    currentCameraWindow.Close();
                
                int cameraWindowCount = getRunningCameraCount();

                for (int x = 0; x < filters.VideoInputDevices.Count; x++)
                {
                    if (filters.VideoInputDevices[x].MonikerString == cameraToRecord.VideoDevice.MonikerString)//this is the camera to record
                    {
                        saveCameraSettings();

                        cameraDisplay = new CameraDisplay();
                        cameraDisplay.FrameSize = getFrameSize();
                        cameraDisplay.FTP = ftp;
                        cameraDisplay.RecordCamera(filters.VideoInputDevices[x], filters.AudioInputDevices[0], recordingPath + cameraToRecord.Name +".avi", enableWebInterface);

                        cameraDisplay.Show();

                        //position cameraWindow
                        cameraDisplay.Left = cameraWindowCount * 250;
                        cameraDisplay.Top = 0;

                        cameraDisplay.Text = cameraToRecord.Name;
                        cameraDisplay.Tag = filters.VideoInputDevices[x].MonikerString;

                        currentCameraWindow = cameraDisplay;

                        lblCameraCapabilities.Text = cameraDisplay.GetCameraCapabilities;

                        break;
                    }
                }
            }
            else
                MessageBox.Show("No Video input devices found");
        }

        /// <summary>
        /// Save settings from UI to XML
        /// </summary>
        private void saveCameraSettings()
        {
            bool madeChanges = false;

            if (xmlCameras == null)
                getCameraInfo();

            if (xmlCameras != null && currentCamera != null)
            {
                foreach (XmlNode camera in xmlCameras["Cameras"])
                {
                    XmlNode monikerNode = camera.SelectSingleNode("monikerString");

                    //see if this is our camera
                    if (monikerNode != null && monikerNode.InnerText.Trim().Length > 0 && monikerNode.InnerText.Trim() == currentCamera.VideoDevice.MonikerString)
                    {
                        XmlNode nameNode = camera.SelectSingleNode("name");
                        if (nameNode != null)
                            nameNode.InnerText = txtCameraName.Text;
                        else
                        {
                            nameNode = xmlCameras.CreateNode(XmlNodeType.Element, "name", "");
                            nameNode.InnerText = txtCameraName.Text;

                            camera.AppendChild(nameNode);
                        }

                        XmlNode frameSizeNode = camera.SelectSingleNode("frameSize");
                        if (frameSizeNode != null)
                            frameSizeNode.InnerText = cbFrameSize.Text;
                        else
                        {
                            frameSizeNode = xmlCameras.CreateNode(XmlNodeType.Element, "frameSize", "");
                            frameSizeNode.InnerText = cbFrameSize.Text;

                            camera.AppendChild(frameSizeNode);
                        }

                        XmlNode frameRateNode = camera.SelectSingleNode("frameRate");
                        if (frameRateNode != null)
                            frameRateNode.InnerText = txtFrameRate.Text;
                        else
                        {
                            frameRateNode = xmlCameras.CreateNode(XmlNodeType.Element, "frameRate", "");
                            frameRateNode.InnerText = txtFrameRate.Text;

                            camera.AppendChild(frameRateNode);
                        }

                        madeChanges = true;

                        break;
                    }
                }
            }

            if (madeChanges)
            {
                xmlCameras.Save(camerasConfigPath);

                getCameraInfo();

                populateCBCameras();
            }
        }

        public void StartCameraByName(string cameraToStart)
        {
            CameraDisplay cameraDisplay = new CameraDisplay(filters.VideoInputDevices[0], filters.AudioInputDevices[0]);
            
        }

        /// <summary>
        /// displays the camera asked for
        /// </summary>
        /// <param name="cameraToStart"></param>
        public void ShowCamera(SecurityCamera cameraToStart)
        {
            CameraDisplay cameraDisplay = null;
            
            if (filters.VideoInputDevices.Count > 0)
            {
                if (isRunning(cameraToStart.Name))
                    currentCameraWindow.Close();

                int cameraWindowCount = getRunningCameraCount();

                for (int x = 0; x < filters.VideoInputDevices.Count; x++)
                {
                    if (filters.VideoInputDevices[x].MonikerString == cameraToStart.VideoDevice.MonikerString)//this is the camera to record
                    {
                        cameraDisplay = new CameraDisplay(filters.VideoInputDevices[x], filters.AudioInputDevices[0]);

                        cameraDisplay.FrameSize = getFrameSize();
                        
                        cameraDisplay.Show();

                        cameraDisplay.Left = cameraWindowCount * 450;
                        cameraDisplay.Top = 0;

                        cameraDisplay.Text = cameraToStart.Name;

                        currentCameraWindow = cameraDisplay;

                        lblCameraCapabilities.Text = cameraDisplay.GetCameraCapabilities;

                        break;
                    }
                }
            }
            else
                MessageBox.Show("No Video input devices found");
        }

        /// <summary>
        /// displays all available cameras starting a window for each
        /// </summary>
        public void ShowAllCameras()
        {
            if (filters.VideoInputDevices.Count > 0)
            {
                for (int x = 0; x < filters.VideoInputDevices.Count; x++)
                {
                    if (isRunning(filters.VideoInputDevices[x].Name))
                        currentCameraWindow.Close();

                    CameraDisplay cameraDisplay = new CameraDisplay(filters.VideoInputDevices[x], filters.AudioInputDevices[x]);
                    cameraDisplay.Text = "Camera " + x.ToString();
                    cameraDisplay.Show();

                    currentCameraWindow = cameraDisplay;
                }
            }
            else
                MessageBox.Show("No Video input devices found");
        }

        //public static void initCameras()
        //{
        //    try
        //    {
        //        foreach (dshow.Filter camera in Cameras)
        //        {
        //            initCamera(camera);
        //            break;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

        /// <summary>
        /// stops the given camera
        /// </summary>
        /// <param name="cameraToStop"></param>
        public void StopCameraByName(string cameraToStop)
        {
            foreach (Form form in Application.OpenForms)
            {
                if (form.Text.ToLower() == cameraToStop.ToLower())//stop this camera
                {
                    form.Close();

                    break;
                }
            }
        }

        /// <summary>
        /// Start specific camera
        /// </summary>
        /// <param name="cameraToStart"></param>
        //public static SCTVCameraWindow initCamera(dshow.Filter cameraToStart)
        //{
        //    SCTVCameraWindow newCameraWindow = new SCTVCameraWindow();
        //    newCameraWindow.CreateCamera(cameraToStart);
        //    newCameraWindow.KeyUp += new System.Windows.Forms.KeyEventHandler(newCameraWindow_KeyUp);
        //    newCameraWindow.Show();

        //    return newCameraWindow;
        //}

        /// <summary>
        /// Start specific camera by name
        /// </summary>
        /// <param name="cameraToStart"></param>
        //public static SCTVCameraWindow initCamera(string cameraToStart)
        //{
        //    SCTVCameraWindow newCameraWindow = new SCTVCameraWindow();

        //    foreach (dshow.Filter camera in Cameras)
        //    {
        //        if (camera.Name.ToLower() == cameraToStart.ToLower())
        //        {
        //            newCameraWindow.CreateCamera(camera);
        //            newCameraWindow.KeyUp += new System.Windows.Forms.KeyEventHandler(newCameraWindow_KeyUp);
        //            newCameraWindow.Show();
        //            break;
        //        }
        //    }

        //    return newCameraWindow;
        //}

        static void newCameraWindow_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
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
                //if (macroList.Contains(tempKeys))
                //{
                //    keyStrokeTracker.Clear();
                //    string macroName = macroList.GetByIndex(macroList.IndexOfKey(tempKeys)).ToString();
                //    //Tools.WriteToFile(Tools.errorFile,"executing macro " + macroName);
                //    executeMacros(macroName);
                //}
                //else
                //    Tools.WriteToFile(Tools.errorFile, "didn't find camera macro for: " + tempKeys);

                //Tools.WriteToFile(Tools.errorFile,"KeyCode from mediaPlayer keyup: " + e.KeyCode);
                //Tools.WriteToFile(Tools.errorFile,"KeyValue from mediaPlayer keyup: " + e.KeyValue);
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, "newCameraWindow_KeyUp error: " + ex.Message);
            }
        }

        /// <summary>
        /// Execute speech commands for cameras
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

        /// <summary>
        /// execute phrases
        /// </summary>
        /// <param name="phrase"></param>
        public void executePhrase(Phrase phrase)
        {
            switch (phrase.phrase.ToLower())
            {
                case ("close"):
                    executeMacros(phrase.phrase);
                    break;
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
                //Tools.WriteToFile(Tools.errorFile,"calling macro in camera " + macroName);
                keyStrokeTracker.Clear();
                switch (macroName.ToLower())
                {
                    case "show camera one":
                        //ShowCameraByName("camera 1");
                        break;
                    case "show camera two":
                        //ShowCameraByName("camera 2");
                        break;
                    case "show all cameras":
                        ShowAllCameras();
                        break;
                    default:
                        Tools.WriteToFile(Tools.errorFile, "executeMacros didnt' find macro " + macroName);
                        break;
                }
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, "executeMacros error: " + ex.Source + ": " + ex.Message);
            }
        }

        #region UI Functions

        private void btnView_Click(object sender, EventArgs e)
        {
            if (currentCamera != null)
            {
                saveCameraSettings();

                //if (chbEnableWebInterface.Checked)
                //RecordCamera(currentCamera, true);
                //else
                ShowCamera(currentCamera);

                updateCurrentCameraInfo();

                lblStatus.Text = "Viewing";
            }
        }

        private void updateCurrentCameraInfo()
        {
            //add min framesize
            cbFrameSize.Items.Insert(0, currentCameraWindow.Camera.VideoCaps.MinFrameSize.Width.ToString() + " x " + currentCameraWindow.Camera.VideoCaps.MinFrameSize.Height.ToString());

            //add max framesize
            cbFrameSize.Items.Insert(cbFrameSize.Items.Count, currentCameraWindow.Camera.VideoCaps.MaxFrameSize.Width.ToString() + " x " + currentCameraWindow.Camera.VideoCaps.MaxFrameSize.Height.ToString());

            //validate framerate and adjust if needed
            int uiFrameRate = 5;
            int.TryParse(txtFrameRate.Text, out uiFrameRate);

            if (uiFrameRate < currentCameraWindow.Camera.VideoCaps.MinFrameRate)
                uiFrameRate = (int)currentCameraWindow.Camera.VideoCaps.MinFrameRate;

            txtFrameRate.Text = uiFrameRate.ToString();
        }

        private void btnRecordCamera_Click(object sender, EventArgs e)
        {
            RecordCamera(currentCamera, false);

            lblStatus.Text = "Recording";
        }

        private void btnRecordForWeb_Click(object sender, EventArgs e)
        {
            ftp = false; 

            RecordCamera(currentCamera, true);

            lblStatus.Text = "Recording For Web";
        }

        private void btnFTP_Click(object sender, EventArgs e)
        {
            ftp = true;

            RecordCamera(currentCamera, true);

            lblStatus.Text = "FTP to Web";
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            StopCameraByName(currentCameraName);

            lblStatus.Text = "Stopped";

            saveCameraSettings();
        }

        private void cbCameras_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentCamera = (SecurityCamera)cbCameras.SelectedItem;
            currentCameraName = currentCamera.Name;

            //update UI with camera name
            txtCameraName.Text = currentCameraName;
            tpCameraName.Text = currentCameraName;
            gbCameraName.Text = currentCameraName;

            if (isRunning(currentCameraName))
            {
                lblStatus.Text = currentCameraWindow.Status;

                lblCameraCapabilities.Text = currentCameraWindow.GetCameraCapabilities;

                updateCurrentCameraInfo();
            }
            else
            {
                lblStatus.Text = "Stopped";

                lblCameraCapabilities.Text = "Camera must be playing to see capabilities";
            }
        }

        private void chbEnableWebInterface_CheckedChanged(object sender, EventArgs e)
        {
            //if checked need to start recording
            if (currentCameraWindow != null)
            {
                RecordCamera(currentCamera, true);
                
                //switch (currentCameraWindow.Status)
                //{
                //    case "Recording":
                //        RecordCamera(currentCamera, chbEnableWebInterface.Checked);
                //        break;
                //    case "Playing":
                //        if (chbEnableWebInterface.Checked)//actually start recording but dont tell the user
                //            RecordCamera(currentCamera, chbEnableWebInterface.Checked);
                //        else
                //            ShowCamera(currentCamera);  
                //        break;
                //}
            }
        }

        #endregion

        /// <summary>
        /// Tells if given camera is running
        /// </summary>
        /// <param name="cameraToCheck"></param>
        /// <returns></returns>
        private bool isRunning(string cameraToCheck)
        {
            foreach (Form form in Application.OpenForms)
            {
                if (form.Text.ToLower() == cameraToCheck.ToLower())//the camera is running
                {
                    currentCameraWindow = (CameraDisplay)form;

                    return true;
                }
            }

            currentCameraWindow = null;

            return false;
        }

        /// <summary>
        /// get the number of running cameras
        /// </summary>
        /// <returns></returns>
        private int getRunningCameraCount()
        {
            int runningCameraCount = 0;

            foreach (Form form in Application.OpenForms)
            {
                if (form is CameraDisplay)
                    runningCameraCount++;
            }

            return runningCameraCount;
        }

        /// <summary>
        /// update framesize of current camera
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbFrameSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (currentCameraWindow != null)
                saveCameraSettings();
        }

        /// <summary>
        /// get selected frame size from cbFrameSize
        /// </summary>
        /// <returns></returns>
        private Size getFrameSize()
        {
            Size newFrameSize = new Size();

            //get size
            string[] size = cbFrameSize.Text.Split('x');
            int width = 0;
            int height = 0;

            if (size.Length == 2)
            {
                int.TryParse(size[0], out width);
                int.TryParse(size[1], out height);
            }

            newFrameSize = new Size(width, height);

            return newFrameSize;
        }

        /// <summary>
        /// Make sure we aren't in the middle of ftp'ing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SCTVCameraMain_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void btnCaptureFrame_Click(object sender, EventArgs e)
        {
            capture.FrameEvent2 += new Capture.HeFrame(CaptureDone);
            capture.GrapImg();
        }

        private void CaptureDone(System.Drawing.Bitmap e)
        {
            //this.pictureBox.Image = e;
        }
    }
}