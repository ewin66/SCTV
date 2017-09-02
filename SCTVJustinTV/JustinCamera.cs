using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace SCTVJustinTV
{
    public class JustinCamera
    {
        string name = "";
        string streamKey = "";
        string channelName = "";
        string password = "";
        DirectX.Capture.Filter videoDevice;
        DirectX.Capture.Filter audioDevice;
        bool isBroadcasting = false;
        bool isDisplaying = false;
        Process procVLC;
        Process procJtv;
        string port = "";
        string sdpPath = "";
        string filterName = "";
        System.Threading.Thread currentThread;
        CameraDisplay cameraWindow = null;
        string startTime = "";
        
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string StreamKey
        {
            get { return streamKey; }
            set { streamKey = value; }
        }

        public string ChannelName
        {
            get { return channelName; }
            set { channelName = value; }
        }

        public DirectX.Capture.Filter VideoDevice
        {
            get { return videoDevice; }
            set { videoDevice = value; }
        }

        public DirectX.Capture.Filter AudioDevice
        {
            get { return audioDevice; }
            set { audioDevice = value; }
        }

        public bool IsBroadcasting
        {
            get { return isBroadcasting; }
            set { isBroadcasting = value; }
        }

        public bool IsDisplaying
        {
            get { return isDisplaying; }
            set { isDisplaying = value; }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public Process ProcVLC
        {
            get { return procVLC; }
            set { procVLC = value; }
        }

        public Process ProcJtv
        {
            get { return procJtv; }
            set { procJtv = value; }
        }

        public string Port
        {
            get { return port; }
            set { port = value; }
        }

        public string SDPPath
        {
            get { return sdpPath; }
            set { sdpPath = value; }
        }

        public string FilterName
        {
            get { return filterName; }
            set { filterName = value; }
        }

        public string StartTime
        {
            get { return startTime; }
            set { startTime = value; }
        }

        public System.Threading.Thread CurrentThread
        {
            get { return currentThread; }
            set { currentThread = value; }
        }

        public CameraDisplay CameraWindow
        {
            get { return cameraWindow; }
            set { cameraWindow = value; }
        }

        public JustinCamera()
        {
            
        }

        public JustinCamera(string Name, string StreamKey, string ChannelName, DirectX.Capture.Filter VideoDevice, DirectX.Capture.Filter AudioDevice)
        {
            name = Name;
            streamKey = StreamKey;
            channelName = ChannelName;
            videoDevice = VideoDevice;
            audioDevice = AudioDevice;
        }
    }
}
