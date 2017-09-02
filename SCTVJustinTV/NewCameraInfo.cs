using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SCTVJustinTV
{
    public partial class NewCameraInfo : Form
    {
        private string cameraName = "";

        public string CameraName
        {
            get { return cameraName; }
            set 
            { 
                cameraName = value;

                txtCameraName.Text = cameraName;
            }
        }

        public NewCameraInfo()
        {
            InitializeComponent();

            txtCameraName.Text = cameraName;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            cameraName = txtCameraName.Text;

            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}