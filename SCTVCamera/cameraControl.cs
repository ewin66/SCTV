using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace SCTVCamera
{
    public partial class cameraControl : UserControl
    {
        string cameraName = "";
        string cameraStatus = "";

        string CameraName
        {
            get { return cameraName; }

            set 
            { 
                cameraName = value;

                gbCamera.Text = value;
            }
        }

        string CameraStatus
        {
            get { return cameraStatus; }

            set 
            { 
                cameraStatus = value;

                lblStatus.Text = value;
            }
        }

        public cameraControl()
        {
            InitializeComponent();
        }
    }
}
