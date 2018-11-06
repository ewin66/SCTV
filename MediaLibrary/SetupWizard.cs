using SCTVObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SCTV
{
    public partial class SetupWizard : Form
    {
        string lastBrowseLocation = "";
        ArrayList sourcePaths = new ArrayList();
        ArrayList origSourcePaths = new ArrayList();
        MediaHandler myMedia = new MediaHandler();

        public ArrayList SourcePaths
        {
            get
            {
                return sourcePaths;
            }

            set
            {
                sourcePaths = value;
                origSourcePaths = value;

                bindToListView(sourcePaths);
            }
        }

        public SetupWizard()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (lastBrowseLocation.LastIndexOf("\\") != lastBrowseLocation.Length - 1)
                lastBrowseLocation += "\\";

            if (lastBrowseLocation.Trim().Length > 0)
                folderBrowserDialog1.SelectedPath = lastBrowseLocation;
            else
                folderBrowserDialog1.SelectedPath = Application.StartupPath + "\\";

            DialogResult result = folderBrowserDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                txtSource.Text = this.folderBrowserDialog1.SelectedPath;
                lastBrowseLocation = this.folderBrowserDialog1.SelectedPath;
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem li in lvLocations.SelectedItems)
            {
                //remove item from arraylist
                sourcePaths.Remove(li.Text);

                //remove item from listview
                lvLocations.Items.Remove(li);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (txtSource.Text.Trim().Length > 0 && !contains(txtSource.Text))// !sourcePaths.Contains(txtSource.Text))
            {
                if (cbMediaTypes.SelectedItem != null && cbMediaTypes.SelectedItem.ToString().Trim().Length > 0)
                {
                    sourcePaths.Add(txtSource.Text + "," + cbMediaTypes.SelectedItem.ToString().Trim());
                    lvLocations.Items.Add(txtSource.Text + "," + cbMediaTypes.SelectedItem.ToString().Trim());
                }
                else
                {
                    sourcePaths.Add(txtSource.Text);
                    lvLocations.Items.Add(txtSource.Text);
                }

                lvLocations.Refresh();

                txtSource.Text = "";
            }
            else
                MessageBox.Show("This location already exists");
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            switch(((Button)sender).Name)
            {
                case "btnNextLocations":
                    //update source paths
                    myMedia.AddLocations(sourcePaths, true);

                    if (myMedia.UpdatedLocations)//the locations updated so check for new media files
                    {
                        MediaLibrary_Listview mediaLibrary = (MediaLibrary_Listview)this.Owner;
                        mediaLibrary.UpdateMedia();
                    }
                    break;
            }

            tabControl1.SelectedIndex = tabControl1.SelectedIndex + 1;
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = tabControl1.SelectedIndex - 1;
        }

        /// <summary>
        /// bind arraylist to listview
        /// </summary>
        /// <param name="dataSource"></param>
        private void bindToListView(ArrayList dataSource)
        {
            try
            {
                foreach (string location in dataSource)
                {
                    string newLocation = location;

                    //if (newLocation.Contains(","))
                    //    newLocation = newLocation.Split(',')[0];

                    lvLocations.Items.Add(newLocation);
                }
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, "bindToListView error: " + ex.Message);
            }
        }

        private bool contains(string txtToCheck)
        {
            bool isDuplicate = false;

            foreach (string source in sourcePaths)
            {
                if (source.Contains(","))
                {
                    if (txtToCheck == source.Split(',')[0])
                    {
                        isDuplicate = true;
                        break;
                    }
                }
                else
                    if (source.ToLower().Contains(txtToCheck.ToLower()))
                {
                    isDuplicate = true;
                    break;
                }
            }

            return isDuplicate;
        }

        private void btnHaveFun_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
