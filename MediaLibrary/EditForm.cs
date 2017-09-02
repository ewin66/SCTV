using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SCTV
{
    public partial class EditForm : Form
    {
        public string Value
        {
            get
            {
                return txtValue.Text;
            }
        }

        public EditForm(string currentValue)
        {
            InitializeComponent();

            txtValue.Text = currentValue;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}