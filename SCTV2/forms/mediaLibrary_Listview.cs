using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SCTV
{
    public partial class MediaLibrary_Listview : Form
    {
        ArrayList categories = new ArrayList();
        DataView dvMedia = new DataView();

        public MediaLibrary_Listview()
        {
            InitializeComponent();

            displayMedia();
        }

        private void displayMedia()
        {
            //add group
            //listView1.Groups.Add(new ListViewGroup("Friends", HorizontalAlignment.Left));



            //add item
            //ListViewItem listViewItem1 = new ListViewItem();
            //ListViewSubItem listViewSubItem1 = new ListViewSubItem();

            //listViewItem1.Text = "Joe";
            //listViewSubItem1.Text = "123 Somewhere Street";

            //listViewItem1.SubItems.Add(listViewSubItem1);
            //listView1.Items.Add(listViewItem1);


            categories.Clear();
            foreach (DataRowView drv in SCTV.myMedia.getAllCategories("video"))
            {
                if (!categories.Contains(drv["category"].ToString()))//keep out duplicates
                    categories.Add(drv["category"].ToString());
            }

            categories.Sort();//alphabetize

            try
            {
                foreach (string tempCat in categories)
                {
                    tcCategories.TabPages.Add(tempCat);
                }

                displayCategory(tcCategories.TabPages[0].Text);
            }
            catch (Exception ex)
            {

            }
        }

        private void tcCategories_SelectedIndexChanged(object sender, EventArgs e)
        {
            displayCategory(tcCategories.SelectedTab.Text);
        }

        /// <summary>
        /// find all in selected category and display them
        /// </summary>
        /// <param name="category"></param>
        public void displayCategory(string category)
        {
            dvMedia = SCTV.myMedia.getCategory(category);
            displayCategory(dvMedia);
        }

        /// <summary>
        /// find all in selected category and display them
        /// </summary>
        /// <param name="dvMedia"></param>
        public void displayCategory(DataView dvMedia)
        {
            //clear media
            lvMedia.Items.Clear();

            ListViewGroup lvig = new ListViewGroup(dvMedia[0]["category"].ToString());
            lvMedia.Groups.Add(lvig);

            foreach (DataRowView dv in dvMedia)//go through the new category results
            {
                //add item
                ListViewItem listViewItem1 = new ListViewItem();
                listViewItem1.Text = dv["title"].ToString();
                listViewItem1.Tag = dv;
                listViewItem1.Group = lvig;
                lvMedia.Items.Add(listViewItem1);
            }
        }

        private void lvMedia_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            foreach(ListViewItem selectedItem in lvMedia.SelectedItems)
            {
                DataRowView dv = (DataRowView)selectedItem.Tag;

            }
        }

        /// <summary>
        /// play selected media
        /// </summary>
        //public void playMedia()//play selected media
        //{
        //    try
        //    {
        //        string selectedMedia = getSelectedMediaFileName();

        //        if (File.Exists(selectedMedia))
        //        {
        //            splashScreen mediaSplash = new splashScreen();

        //            if (Form1.Mode == "Release")
        //            {
        //                mediaSplash.SplashMessage2 = selectedMedia;

        //                mediaSplash.Show();
        //                mediaSplash.TopMost = true;
        //            }

        //            //this.Hide();
        //            createMediaWindow();
        //            //						mediaWindow.playVideo(selectedLabel.Tag.ToString());
        //            mediaWindow.PlayClip(selectedMedia);

        //            if (Form1.Mode == "Release")
        //            {
        //                mediaSplash.Close();
        //                mediaWindow.TopMost = true;
        //                mediaWindow.Focus();
        //            }

        //            mediaWindow.Show();
        //        }
        //        else
        //        {
        //            EventLog.WriteEntry("sctv", "---- Missing File : " + selectedMedia + " ----");
        //            MessageBox.Show("Missing File - " + selectedMedia);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        EventLog.WriteEntry("sctv", "PlayMedia error: " + ex.Message);
        //    }
        //}

        //private void createMediaWindow()
        //{
        //    //EventLog.WriteEntry("sctv", "in createMediaWindow");
        //    try
        //    {
        //        if (mediaWindow == null)
        //        {
        //            mediaWindow = new liquidMediaPlayer();
        //            mediaWindow.Closed += new EventHandler(mediaWindow_Closed);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        EventLog.WriteEntry("sctv", "createMediaWindow error: " + ex.Message);
        //    }
        //}
    }
}