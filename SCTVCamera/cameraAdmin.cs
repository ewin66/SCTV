using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SCTVCamera
{
    public partial class CameraAdmin : Form
    {
        public CameraAdmin()
        {
            InitializeComponent();
        }

        private void btnStream_Click(object sender, EventArgs e)
        {
            VLCCameraDisplay vlcCamera = new VLCCameraDisplay();
            vlcCamera.Show();
        }
    }
}