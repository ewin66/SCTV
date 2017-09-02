using System;
using System.Collections.Generic;
using System.Text;

namespace SCTVCamera
{
    public class SecurityCamera
    {
        string name = "";
        string recordPath = "";
        DirectX.Capture.Filter videoDevice;
        DirectX.Capture.Filter audioDevice;
        
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string RecordPath
        {
            get { return recordPath; }
            set { recordPath = value; }
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

        public SecurityCamera()
        {
            
        }

        public SecurityCamera(string Name, string RecordPath, DirectX.Capture.Filter VideoDevice, DirectX.Capture.Filter AudioDevice)
        {
            name = Name;
            recordPath = RecordPath;
            videoDevice = VideoDevice;
            audioDevice = AudioDevice;
        }
    }
}
