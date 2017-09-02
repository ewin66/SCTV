using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using SCTVObjects;

namespace SCTVCamera
{
    public partial class VLCCameraDisplay : Form
    {
        VLC vlc;
        AsynTask _playerTask;

        public VLCCameraDisplay()
        {
            InitializeComponent();

            Record();
        }

        public void Record()
        {
            try
            {
                int vHandle = 0;
                
                vHandle = this.pnlVideo.Handle.ToInt32();

                if (vHandle > 0)
                {
                    vlc = new VLC();

                    vlc.SetOutputWindow(vHandle);
                    string[] options;

                    options = ToScreen(false);

                    _playerTask = new PlayerTask(10, 0, vlc, options,@"c:\movies\vlcCameraOutput.avi");
                     _playerTask.Completed += new AsynTaskCompletedEventHandler(playerTask_Completed);
                    _playerTask.Start();
                }
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, "PlayMedia error: " + ex.Message);
            }
        }

        public string[] ToScreen(bool displayToScreen)
        {
            string[] options = new string[3];

            options[0] = "config=\"~/Application Data/vlc/vlcrc\"";
            options[1] = ":file-caching=300 :sout-udp-caching=200";

            //options[2] = ":sout=#transcode{vcodec=h264,vb=800,scale=1,acodec=mp4a,ab=128,channels=2,samplerate=44100}:duplicate{dst=std{access=http,mux=ts,dst=0.0.0.0:8080},dst=display}";
            //options[2] = " :dshow-vdev=IBM PC Camera :dshow-adev=:sout=#transcode{vcodec=h264,vb=800,scale=1,acodec=mp4a,ab=128,channels=2,samplerate=44100}:std{access=http,mux=ts,dst=0.0.0.0:8080}";
            //options[2] = "-vvv -I rc --ttl 12 :dshow-vdev=IBM PC Camera :sout #std{mux=ts,access=file,dst=C:\\movies\\webcamVideo.avi}";
            options[2] = ":dshow-vdev=IBM PC Camera :sout=#transcode{venc=ffmpeg,vcodec=VIDEO_CODEC_HERE,vb=VIDEO_BITRATE_GERE,acodec=AUDIO_CODEC_HERE,ab=AUDIO_BITRATE_HERE}:std{mux=avi,access=file,dst=C:\\movies\\webcamvideo.avi}";
            //if (fileToRecordTo.Length > 2)
            //{
            //    if (displayToScreen)//display output to screen and record
            //    {
            //        options[2] = ":sout=#transcode{vcodec=mp4v,vb=512,scale=1}:duplicate{dst=display,dst=std{access=file,mux=ts,dst=\"" + fileToRecordTo + "\"}}";
            //        //options[2] = ":sout=#transcode{vcodec=mp4v,vb=3000,scale=1}:duplicate{dst=display,dst=std{access=file,mux=ts,dst=\"" + fileToRecordTo + "\"}}";

            //        //worked - 3gig for indiana jones - no pixelation
            //        options[2] = ":sout=#duplicate{dst=display,dst=std{access=file,mux=ts,dst=\"" + fileToRecordTo + "\"}}";
            //    }
            //    else //record only
            //        options[2] = ":sout=#duplicate{dst=std{access=file,mux=ts,dst=\"" + fileToRecordTo + "\"}}";
            //}
            //else//play only
            //    options[2] = ":sout=#duplicate{dst=display}";

            return options;
        }

        public void playerTask_Completed(object sender, AsynTaskCompletedEventArgs e)
        {
            //MessageBox.Show("done playing");

            //object[] state = (object[])e.Result;
            //label1.Text = (string)state[0];

            //executeMacros("close");
        }
    }
}