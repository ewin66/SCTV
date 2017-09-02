using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SCTVObjects;

namespace MyTest
{
    public partial class Form2 : GlassWindow
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void llMore_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (llMore.Text == "more...")
            {
                this.AutoScroll = true;
                llMore.Text = "less...";
            }
            else
            {
                this.AutoScroll = false;
                llMore.Text = "more...";
            }
        }
    }
}