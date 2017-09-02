using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DirectX.Capture;
using SCTVFTP;
using System.IO;

namespace SCTVCamera
{
    public partial class CameraDisplay : Form
    {
        private string recordingFile = "";
        int recordingTimerTicks = 0;
        int recordingTimerTickThreshold = 15;//number of ticks before new video is created for web interface
        int recordingFileTracker = 0;//the number to append to the name of the recorded files
        bool deleteFileAfterFTP = true;
        //private Filters filters = new Filters();
        private Capture capture = null;
        //private Capture capture2 = null;
        private string status = "Stopped";
        Size frameSize = new Size();
        double frameRate = 5;
        string cameraCapabilities = "";
        int minFreeDriveSpace = 1073741824; //one gig
        bool ftp = false; //whether or not to ftp the files
        StreamWriter log = null;

        public string GetCameraCapabilities
        {
            get
            {
                return getCameraCapabilities();
            }
        }

        public string Status
        {
            get{    return status;  }
        }

        public Size FrameSize
        {
            get 
            {
                if (capture != null)
                    return capture.FrameSize;
                else
                    return new Size();
            }
            set 
            { 
                frameSize = value;

                if (capture != null && frameSize.Height > 0 && frameSize.Width > 0)
                    capture.FrameSize = frameSize;
            }
        }

        public double FrameRate
        {
            get
            {
                if (capture != null)
                    return capture.FrameRate;
                else
                    return 0;
            }
            set
            {
                frameRate = value;
            }
        }

        public Capture Camera
        {
            get
            {
                return capture;
            }
        }

        public bool FTP
        {
            get { return ftp; }
            set { ftp = value; }
        }

        public CameraDisplay()
        {
            try
            {
                InitializeComponent();

                initVariables();

                log = File.CreateText(Application.StartupPath + "\\ftpLog.log");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public CameraDisplay(DirectX.Capture.Filter cameraToStart, DirectX.Capture.Filter audioToStart)
        {
            try
            {
                InitializeComponent();

                initVariables();

                startCamera(cameraToStart, audioToStart);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void initVariables()
        {
            int.TryParse(System.Configuration.ConfigurationManager.AppSettings["Camera.WebRecordingLength"], out recordingTimerTickThreshold);
            bool.TryParse(System.Configuration.ConfigurationManager.AppSettings["Camera.DeleteFilesAfterFTP"], out deleteFileAfterFTP);            
        }

        private void startCamera(DirectX.Capture.Filter videoDevice, DirectX.Capture.Filter audioDevice)
        {
            if (videoDevice != null || audioDevice != null)
            {
                if (capture != null)
                {
                    capture.Stop();
                    capture.Dispose();
                }

                capture = new Capture(videoDevice, audioDevice);

                try
                {
//if(frameSize.Width > 0 && frameSize.Height > 0)
                //    capture.FrameSize = frameSize;

                capture.FrameSize = new Size(capture.VideoCaps.MinFrameSize.Width, capture.VideoCaps.MinFrameSize.Height);
                }
                catch (Exception ex)
                {
                    
                }

                try
                {
                    capture.FrameRate = frameRate;
                }
                catch (Exception ex)
                {
                    
                }

                //capture.CaptureComplete += new EventHandler(OnCaptureComplete);
                //CameraDisplay cameraDisplay = new CameraDisplay();
                capture.PreviewWindow = pnlDisplay;
                //cameraDisplay.Show();

                status = "Playing";
            }
        }

        public void RecordCamera(DirectX.Capture.Filter videoDevice, DirectX.Capture.Filter audioDevice, string fileName, bool splitVideo)
        {
            if (videoDevice != null || audioDevice != null)
            {
                if (capture != null)
                {
                    capture.Stop();
                    capture.Dispose();

                    GC.Collect();
                }

                recordingFile = fileName;

                //create path if it doesn't exist
                System.IO.FileInfo fi = new System.IO.FileInfo(recordingFile);

                if (!fi.Directory.Exists)
                    fi.Directory.Create();

                capture = new Capture(videoDevice, audioDevice);

                if(fileName.Contains("."))
                    capture.Filename = fileName.Replace(".", "_" + DateTime.Now.ToShortDateString().Replace("/", "") + DateTime.Now.ToShortTimeString().Replace(":", "").Replace(" ", "") + DateTime.Now.Millisecond.ToString() + "_1.");
                else
                    capture.Filename = fileName + "_" + DateTime.Now.ToShortDateString().Replace("/", "") + DateTime.Now.ToShortTimeString().Trim().Replace(":", "") + DateTime.Now.Millisecond.ToString() + "_1.avi";

                //use minimum frame size
                try
                {
                    if (frameSize.Height > 0 && frameSize.Width > 0)
                        capture.FrameSize = frameSize;

                    //capture.FrameSize = new Size(capture.VideoCaps.MinFrameSize.Width, capture.VideoCaps.MinFrameSize.Height);
                }
                catch (Exception ex)
                {
                    
                }

                try
                {
                    capture.FrameRate = frameRate;
                }
                catch (Exception ex)
                {
                    
                }
               
                //capture.CaptureComplete += new EventHandler(OnCaptureComplete);
                //CameraDisplay cameraDisplay = new CameraDisplay();
                capture.PreviewWindow = pnlDisplay;
                //cameraDisplay.Show();

                //check disc space before capturing
                if (checkDiscSpace(capture.Filename))
                {
                    //start new capture
                    capture.Start();

                    if (splitVideo)
                    {
                        recordingTimer.Enabled = true;
                        recordingTimer.Start();
                    }
                    else
                        recordingTimer.Enabled = false;

                    status = "Recording";
                }
            }
        }

        private string getCameraCapabilities()
        {
            string retString = "";

            try
            {
                if (capture != null)
                {
                    retString = String.Format(
                        "Video Device Capabilities\n" +
                        "--------------------------------\n\n" +
                        "Input Size:\t\t{0} x {1}\n" +
                        "\n" +
                        "Min Frame Size:\t\t{2} x {3}\n" +
                        "Max Frame Size:\t\t{4} x {5}\n" +
                        "Frame Size Granularity X:\t{6}\n" +
                        "Frame Size Granularity Y:\t{7}\n" +
                        "\n" +
                        "Min Frame Rate:\t\t{8:0.000} fps\n" +
                        "Max Frame Rate:\t\t{9:0.000} fps\n",
                        capture.VideoCaps.InputSize.Width, capture.VideoCaps.InputSize.Height,
                        capture.VideoCaps.MinFrameSize.Width, capture.VideoCaps.MinFrameSize.Height,
                        capture.VideoCaps.MaxFrameSize.Width, capture.VideoCaps.MaxFrameSize.Height,
                        capture.VideoCaps.FrameSizeGranularityX,
                        capture.VideoCaps.FrameSizeGranularityY,
                        capture.VideoCaps.MinFrameRate,
                        capture.VideoCaps.MaxFrameRate);
                    //MessageBox.Show(s);

                    //lblCameraCapabilities.Text = s;
                }
            }
            catch (Exception ex)
            {
                retString = "Unable display video capabilities. Please submit a bug report.\n\n" + ex.Message + "\n\n" + ex.ToString();
            }

            return retString;
        }

        /// <summary>
        /// clean up resources
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CameraDisplay_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (capture != null)
            {
                capture.Stop();
                capture.Dispose();

                GC.Collect();
            }

            if (log != null)
                log.Dispose();
        }

        /// <summary>
        /// Stop current capture and start new one with new filename
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void recordingTimer_Tick(object sender, EventArgs e)
        {
            string newFileName = "";
            
            recordingTimerTicks++;

            if (recordingTimerTicks > recordingTimerTickThreshold)
            {
                recordingTimer.Enabled = false;

                if (capture != null)
                {
                    recordingFileTracker++;

                    if (capture.Filename.Contains("_"))
                    {
                        string[] fileNameInfo = capture.Filename.Split('_');

                        int sequenceNum;
                        if (int.TryParse(fileNameInfo[2].Substring(0, fileNameInfo[2].IndexOf(".")), out sequenceNum))
                            sequenceNum++;
                        else
                            sequenceNum = 1;

                        newFileName = fileNameInfo[0] + "_" + fileNameInfo[1] + "_" + sequenceNum +".avi";
                    }
                    else
                        newFileName = capture.Filename.Replace(".", "_" + recordingFileTracker.ToString() + ".");

                    //stop previous capture
                    capture.Stop();
                    log.WriteLine("ftp: " + ftp);
                    if (ftp)
                    {
                        try
                        {
                            //get activation key
                            string activationKey = System.Configuration.ConfigurationManager.AppSettings["ActivationKey"];

                            if (activationKey != null)
                            {
                                log.WriteLine("activationKey " + activationKey);

                                activationKey = activationKey.Replace("/", "");
                                activationKey = activationKey.Replace("!", "");
                                activationKey = activationKey.Replace("\\", "");

                                //ftp file to server
                                SCTVFTPForm sctvFtpForm = new SCTVFTPForm();
                                sctvFtpForm.FTPServerIP = "electrodatallc.com/electrodata/" + activationKey + "/";

                                log.WriteLine("ftpServerIP "+ sctvFtpForm.FTPServerIP);

                                sctvFtpForm.FTPUserID = "bob";
                                sctvFtpForm.FTPPassword = "bob";
                                sctvFtpForm.DeleteFileAfterFTP = deleteFileAfterFTP;
                                sctvFtpForm.Upload(capture.Filename);

                                log.WriteLine("fileName to upload: " + capture.Filename);

                                log.Flush();
                            }
                            else
                                throw new Exception("Activation Key is not valid");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("FTP ERROR! " + Environment.NewLine + ex.Message);
                        }
                    }

                    //give new filename
                    capture.Filename = newFileName;

                    try
                    {
                        if (frameSize.Width > 0 && frameSize.Height > 0)
                            capture.FrameSize = frameSize;

                        //capture.FrameSize = new Size(capture.VideoCaps.MinFrameSize.Width, capture.VideoCaps.MinFrameSize.Height);
                    }
                    catch (Exception ex)
                    {
                        
                    }

                    try
                    {
                        capture.FrameRate = frameRate;
                    }
                    catch (Exception ex)
                    {
                        
                    }

                    if (checkDiscSpace(capture.Filename))
                    {
                        //start new capture
                        capture.Start();
                    }
                }

                recordingTimerTicks = 0;

                recordingTimer.Enabled = true;
            }
        }

        /// <summary>
        /// Check to see if there is free disc space
        /// </summary>
        /// <param name="driveToCheck"></param>
        /// <returns></returns>
        private bool checkDiscSpace(string driveToCheck)
        {
            bool enoughSpace = true;

            DriveInfo di = new DriveInfo(driveToCheck);
            if (di.AvailableFreeSpace < minFreeDriveSpace)
            {
                switch (MessageBox.Show("There is not enough disc space.", "Disc Space", MessageBoxButtons.OKCancel))
                {
                    case DialogResult.Cancel:
                        enoughSpace = false;
                        break;
                    case DialogResult.OK:
                        enoughSpace = true;
                        break;
                }
            }

            return enoughSpace;
        }
    }
}