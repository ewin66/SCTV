using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SCTVObjects;
using System.IO;
using System.Collections;
using System.Windows.Forms.VisualStyles;

namespace SCTV
{
    public partial class HorizontalScrollingListview : UserControl
    {
        int titleMaxLength = 28;
        MediaHandler mediaHandler = new MediaHandler();
        string selectedGenres = "";
        liquidMediaPlayer mediaWindow;
        Media currentMedia;
        private bool continousPlay = false;
        private bool playSequels = false;

        public HorizontalScrollingListview()
        {
            InitializeComponent();

            this.Height = 175;
        }

        private void btnScrollRight_MouseHover(object sender, EventArgs e)
        {
            lvMedia.ScrollLeft();
        }

        private void btnScrollRight_MouseLeave(object sender, EventArgs e)
        {
            lvMedia.MarqueeSpeed = 0;
        }

        private void btnScrollLeft_MouseHover(object sender, EventArgs e)
        {
            lvMedia.ScrollRight();
        }

        private void btnScrollLeft_MouseLeave(object sender, EventArgs e)
        {
            lvMedia.MarqueeSpeed = 0;
        }

        private DataView getTopRows(DataView dvDataView, int numRows)
        {
            try
            {
                DataTable dtReturnData = dvDataView.Table.Clone();
                int numRowsToReturn = numRows;

                if (numRows > dvDataView.Table.Rows.Count)
                    numRowsToReturn = dvDataView.Table.Rows.Count;

                for (int x = 0; x < numRowsToReturn; x++)
                    dtReturnData.ImportRow(dvDataView.Table.Rows[x]);

                return dtReturnData.DefaultView;
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile,ex.Message + Environment.NewLine + ex.StackTrace);

                return dvDataView;
            }
        }

        /// <summary>
        /// find all media in selected category and starts with selected letter and display them
        /// </summary>
        /// <param name="category"></param>
        public void DisplayCategory(string category, string startsWith, string mediaType)
        {
            try
            {
                int mediaFindCounter = 0;
                selectedGenres = category;
                lvMedia.Items.Clear();
                mediaType = mediaType.ToLower();

                //if (mediaType.ToLower() == "online")
                //    dvMedia = new DataView();
                //else
                DataView dvMedia = mediaHandler.GetCategoryMedia(category, startsWith, mediaType, true);

                if (dvMedia != null && dvMedia.Table != null)
                {
                    if (dvMedia.Table.Rows.Count == 0)
                    {
                        DataView dvCategories = mediaHandler.GetAllCategories(mediaType);

                        while (dvMedia.Table.Rows.Count == 0 && mediaFindCounter < dvCategories.Table.Rows.Count)
                        {
                            dvMedia = mediaHandler.GetCategoryMedia(dvCategories.Table.Rows[mediaFindCounter]["category"].ToString(), startsWith, mediaType, true);

                            mediaFindCounter++;
                        }
                    }

                    displayCategory(getTopRows(dvMedia, 20), category, mediaType);
                }

                gbTitle.Text = MediaHandler.FormatNameString(category.Replace("|", " and "));
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, ex);
            }            
        }

        private void displayCategory(DataView dvMedia, string category, string mediaType)
        {
            try
            {
                string imageKey = "";
                string spaces = "";
                string currentPlaylist = "";

                if (category.Length == 0)
                    category = "All";
                
                if (dvMedia.Count > 0)
                {
                    ListViewGroup lvig = null;

                    if (category.ToLower() == "playlist-removeForNow")
                    {
                        foreach (DataRowView dv in dvMedia)
                        {
                            if (dv["playlistName"].ToString() != currentPlaylist)
                            {
                                currentPlaylist = dv["playlistName"].ToString();

                                lvig = new ListViewGroup(currentPlaylist);
                                lvMedia.Groups.Add(lvig);
                            }

                            PlaylistItem newItem = new PlaylistItem();
                            newItem.PlaylistName = currentPlaylist;
                            newItem.MediaSource = dv["mediaSource"].ToString();

                            //add item
                            ListViewItem listViewItem1 = new ListViewItem();

                            string mediaSource = "  " + dv["MediaSource"].ToString().Trim();

                            //make title titleMaxLength characters
                            //titleMaxLength = lvMedia.Columns[0].Width - lvMedia.SmallImageList.ImageSize.Width - 60;

                            if (mediaSource.Length > titleMaxLength)
                            {
                                mediaSource = mediaSource.Substring(0, titleMaxLength - 3) + "...";
                                //title = title.Substring(0, titleMaxLength) + Environment.NewLine + title.Substring(titleMaxLength).PadRight(titleMaxLength, Convert.ToChar(" "));
                            }
                            else
                                mediaSource = mediaSource.PadRight(titleMaxLength, Convert.ToChar(" "));

                            listViewItem1.Text = mediaSource;
                            listViewItem1.Tag = newItem;

                            string imagePath = "";

                            //if (dv["coverImage"] != null)
                            //    imagePath = dv["coverImage"].ToString().Trim();

                            if (!File.Exists(imagePath))
                            {
                                if (File.Exists(Application.StartupPath + "\\" + imagePath))
                                    imagePath = Application.StartupPath + "\\" + imagePath;
                            }

                            //get image info
                            if (imagePath.Length > 0 && File.Exists(imagePath))
                            {
                                imageKey = imagePath;

                                if (!ilImages.Images.ContainsKey(imageKey))
                                {
                                    //if (File.Exists(imageKey))
                                    //{
                                    Bitmap newImage = new Bitmap(imageKey);
                                    ilImages.Images.Add(imageKey, newImage);//add new image
                                    //}
                                    //else
                                    //    MessageBox.Show("Image not found " + Environment.NewLine + imageKey);
                                }
                            }
                            else
                                imageKey = "defaultImage";

                            listViewItem1.ImageKey = imageKey;

                            if (lvig != null)
                                listViewItem1.Group = lvig;

                            listViewItem1.ToolTipText = newItem.MediaSource;

                            //if (media.Description.Length > 0)
                            //    listViewItem1.ToolTipText += "\n" + media.Description;
                            //listViewItem1.ToolTipText += "\n" + dv["url"].ToString();

                            listViewItem1.ToolTipText += "\nPlaylist: " + newItem.PlaylistName;
                            //lvMedia.Items.Add(listViewItem1);
                            lvMedia.Items.Add(listViewItem1);

                            //if (lvMedia.Items.Count > 0 && lvMedia.Items.Count % 50 == 0)
                            //    lvMedia.Update();

                            if (lvMedia.Items.Count > 0 && lvMedia.Items.Count % 50 == 0)
                                lvMedia.Update();
                        }
                    }
                    else
                    {
                        switch (mediaType.ToLower())
                        {
                            case "online":
                                dvMedia.RowFilter = "category LIKE '*" + category + "*'";

                                lvig = new ListViewGroup(category);
                                lvMedia.Groups.Add(lvig);

                                foreach (DataRowView dv in dvMedia)
                                {
                                    //create mediaType item
                                    OnlineMediaType media = new OnlineMediaType();
                                    media.Name = dv["name"].ToString();
                                    media.Category = dv["category"].ToString();
                                    media.Description = dv["description"].ToString();
                                    media.type = dv["mediaCategory"].ToString();
                                    media.URL = dv["url"].ToString();
                                    media.CoverImage = Application.StartupPath + dv["coverImage"].ToString();

                                    //add item
                                    ListViewItem listViewItem1 = new ListViewItem();

                                    string title = "  " + dv["name"].ToString().Trim();

                                    //make title 35 characters
                                    //titleMaxLength = lvMedia.Columns[0].Width - lvMedia.SmallImageList.ImageSize.Width - 60;

                                    if (title.Length > titleMaxLength)
                                    {
                                        title = title.Substring(0, titleMaxLength - 3) + "...";
                                        //title = title.Substring(0, titleMaxLength) + Environment.NewLine + title.Substring(titleMaxLength).PadRight(titleMaxLength, Convert.ToChar(" "));
                                    }
                                    else
                                        title = title.PadRight(titleMaxLength, Convert.ToChar(" "));

                                    listViewItem1.Text = title;
                                    listViewItem1.Tag = media;

                                    string imagePath = "";

                                    if (dv["coverImage"] != null)
                                        imagePath = dv["coverImage"].ToString().Trim();

                                    if (!File.Exists(imagePath))
                                    {
                                        if (File.Exists(Application.StartupPath + "\\" + imagePath))
                                            imagePath = Application.StartupPath + "\\" + imagePath;
                                    }

                                    //get image info
                                    if (imagePath.Length > 0 && File.Exists(imagePath))
                                    {
                                        imageKey = imagePath;

                                        if (!ilImages.Images.ContainsKey(imageKey))
                                        {
                                            //if (File.Exists(imageKey))
                                            //{
                                            Bitmap newImage = new Bitmap(imageKey);
                                            ilImages.Images.Add(imageKey, newImage);//add new image
                                            //}
                                            //else
                                            //    MessageBox.Show("Image not found " + Environment.NewLine + imageKey);
                                        }
                                    }
                                    else
                                        imageKey = "defaultImage";

                                    listViewItem1.ImageKey = imageKey;
                                    listViewItem1.Group = lvig;
                                    listViewItem1.ToolTipText = media.Name;
                                    listViewItem1.Name = media.Name;

                                    if (media.Description.Length > 0)
                                        listViewItem1.ToolTipText += "\n" + media.Description;
                                    listViewItem1.ToolTipText += "\n" + dv["url"].ToString();

                                    //lvMedia.Items.Add(listViewItem1);
                                    lvMedia.Items.Add(listViewItem1);

                                    //if (lvMedia.Items.Count > 0 && lvMedia.Items.Count % 50 == 0)
                                    //    lvMedia.Update();

                                    if (lvMedia.Items.Count > 0 && lvMedia.Items.Count % 50 == 0)
                                        lvMedia.Update();
                                }
                                break;
                            case "games":
                                //get id of selected category
                                MediaHandler.dsGameCategories.Tables[0].DefaultView.RowFilter = "name = '" + category + "'";
                                string categoryID = MediaHandler.dsGameCategories.Tables[0].DefaultView[0]["categoryID"].ToString();

                                //get games in the selected category
                                MediaHandler.dsGames.Tables[0].DefaultView.RowFilter = "categoryID = '" + categoryID + "' and display = 'True'";

                                if (lvMedia.Items.Count == 0)
                                    lvMedia.Groups.Clear();

                                //ilImages.Images.Clear();

                                //dvMedia.RowFilter = "category LIKE '*" + category + "*'";

                                //lvig = new ListViewGroup(category);
                                //lvMedia.Groups.Add(lvig);

                                if (ilImages.Images.Count == 0 || !ilImages.Images.ContainsKey("defaultImage"))
                                {
                                    //clear any images that might be in the list
                                    ilImages.Images.Clear();

                                    //add the default image to the list
                                    Bitmap defaultImage = new Bitmap(Application.StartupPath + "\\images\\gameThumbnails\\_comingSoon.gif");
                                    ilImages.Images.Add("defaultImage", defaultImage);//add default image
                                }

                                int listViewStartCount = lvMedia.Items.Count;
                                int newItemCounter = 0;

                                lvMedia.BeginUpdate();

                                foreach (DataRowView dv in MediaHandler.dsGames.Tables[0].DefaultView)
                                {
                                    //get category of current game
                                    //MediaHandler.dsGameCategories.Tables[0].DefaultView.RowFilter = "gameID = '"+ dv["categoryID"].ToString() +"'";

                                    //MediaHandler.dsGameCategories.Tables[0].DefaultView[0]

                                    //create game item
                                    //Game theGame = new Game();
                                    Media theGame = MediaHandler.CreateMedia(dv);
                                    //theGame.Title = dv["title"].ToString();
                                    theGame.category = category;
                                    //theGame.Description = dv["description"].ToString();
                                    //theGame.Type = dv["mediaCategory"].ToString();
                                    theGame.filePath = Application.StartupPath + "\\games\\" + dv["filename"].ToString();
                                    theGame.coverImage = Application.StartupPath + "\\images\\gameThumbnails\\" + dv["coverImage"].ToString();

                                    //create media item
                                    //Media media = MediaHandler.CreateMedia(dv);
                                    theGame.MediaType = "Games";

                                    //add item
                                    ListViewItem listViewItem1 = new ListViewItem();

                                    string title = "  " + theGame.Title;

                                    //make title 35 characters
                                    //titleMaxLength = lvMedia.Columns[0].Width - lvMedia.SmallImageList.ImageSize.Width - 60;

                                    if (title.Length > titleMaxLength)
                                    {
                                        title = title.Substring(0, titleMaxLength - 3) + "...";
                                        //title = title.Substring(0, titleMaxLength) + Environment.NewLine + title.Substring(titleMaxLength).PadRight(titleMaxLength, Convert.ToChar(" "));
                                    }
                                    else
                                        title = title.PadRight(titleMaxLength, Convert.ToChar(" "));

                                    listViewItem1.Text = title;
                                    //listViewItem1.Tag = media;

                                    //string imagePath = "";

                                    //if (theGame.Thumbnail != null)
                                    //    imagePath = Application.StartupPath + ;

                                    //if (!File.Exists(imagePath))
                                    //{
                                    //    if (File.Exists(Application.StartupPath + "\\" + imagePath))
                                    //        imagePath = Application.StartupPath + "\\" + imagePath;
                                    //}

                                    //get image info
                                    if (theGame.coverImage.Length > 0 && File.Exists(theGame.coverImage))
                                    {
                                        imageKey = theGame.coverImage;

                                        if (!ilImages.Images.ContainsKey(imageKey))
                                        {
                                            try
                                            {
                                                //if (File.Exists(imageKey))
                                                //{
                                                Bitmap newImage = new Bitmap(imageKey);
                                                ilImages.Images.Add(imageKey, newImage);//add new image

                                                theGame.ImageListIndex = Convert.ToString(ilImages.Images.Count - 1);

                                                //}
                                                //else
                                                //  MessageBox.Show("Image not found " + Environment.NewLine + imageKey);
                                            }
                                            catch (Exception ex)
                                            {
                                                //the image file was invalid
                                                imageKey = "defaultImage";

                                                theGame.ImageListIndex = "0";
                                            }
                                        }
                                        else
                                            theGame.ImageListIndex = ilImages.Images.IndexOfKey(imageKey).ToString();
                                    }
                                    else
                                    {
                                        imageKey = "defaultImage";

                                        theGame.ImageListIndex = "0";
                                    }

                                    listViewItem1.Tag = theGame;
                                    listViewItem1.ImageKey = imageKey;
                                    listViewItem1.Group = lvig;
                                    listViewItem1.ToolTipText = theGame.Title;
                                    listViewItem1.Name = theGame.Title;

                                    if (theGame.Description.Length > 0)
                                        listViewItem1.ToolTipText += "\n" + theGame.Description;
                                    //listViewItem1.ToolTipText += "\n" + dv["url"].ToString();

                                    lvMedia.Items.Add(listViewItem1);

                                    newItemCounter++;

                                    if (lvMedia.Items.Count > 0 && lvMedia.Items.Count % 50 == 0)
                                        lvMedia.Update();
                                }

                                int tempIndex = 0;

                                for (int i = listViewStartCount; i < newItemCounter; i++)
                                {
                                    ListViewItem lvi = this.lvMedia.Items[i];

                                    if (!int.TryParse(((Media)lvi.Tag).ImageListIndex, out tempIndex))
                                        tempIndex = 0;

                                    lvi.ImageIndex = tempIndex;

                                    this.lvMedia.Invalidate(lvi.Bounds);
                                    this.Update();
                                }

                                lvMedia.EndUpdate();
                                break;
                            default:
                                lvMedia.BeginUpdate();

                                //ListViewGroup lvig = new ListViewGroup(dvMedia[0]["category"].ToString());
                                bool foundGroup = false;

                                if (lvMedia.Items.Count == 0)
                                    lvMedia.Groups.Clear();

                                if ((category.ToLower() != "star rating"))
                                {
                                    //foreach (ListViewGroup group in lvMedia.Groups)
                                    //{
                                    //    if (group.Header != null && group.Header == category)
                                    //    {
                                    //        foundGroup = true;
                                    //        lvig = new ListViewGroup(category);
                                    //        break;
                                    //    }
                                    //}

                                    //if (!foundGroup || lvig == null)
                                    //{

                                    //if (category.Contains("|"))
                                    //    btnFilterOutResults.Visible = true;
                                    //else
                                    //    btnFilterOutResults.Visible = false;

                                    //if (btnFilterOutResults.Text == "+")
                                    //    category = category.Replace("|", " and ");
                                    //else
                                        category = category.Replace("|", " or ");

                                    lvig = new ListViewGroup("     " + category);
                                    //lvMedia.Groups.Add(lvig);
                                    lvMedia.Groups.Add(lvig);

                                    //    foundGroup = true;
                                    //}
                                }
                                else
                                {
                                    lvMedia.Groups.Clear();
                                }

                                if (ilImages.Images.Count == 0 || !ilImages.Images.ContainsKey("defaultImage"))
                                {
                                    //clear any images that might be in the list
                                    ilImages.Images.Clear();

                                    //add the default image to the list
                                    Bitmap defaultImage = new Bitmap(Application.StartupPath + "\\images\\Media\\coverImages\\notAvailable.jpg");
                                    ilImages.Images.Add("defaultImage", defaultImage);//add default image
                                }

                                //int listViewStartCount = lvMedia.Items.Count;
                                listViewStartCount = lvMedia.Items.Count;
                                newItemCounter = 0;
                                //dtRemainingResults = dvMedia.Table.Clone();

                                foreach (DataRowView dv in dvMedia)//go through the new category results
                                {
                                    //just load enough items to equal loadOnDemandChunk to decrease load times
                                    //if (lvMedia.Items.Count >= loadOnDemandChunk + listViewStartCount)
                                    //{
                                    //    //load the remaining items in dvMedia into a temp datatable to load when needed
                                    //    //string[] rows = new string[dvMedia.Count];
                                    //    //dvMedia.Table.Rows.CopyTo(rows, newItemCounter);

                                    //    dtRemainingResults.ImportRow(dv.Row);
                                    //}
                                    //else
                                    //{
                                        if (category.ToLower() == "star rating")
                                        {
                                            foundGroup = false;

                                            foreach (ListViewGroup group in lvMedia.Groups)
                                            {
                                                if (group.Name == dv["stars"].ToString().Trim())
                                                {
                                                    foundGroup = true;

                                                    break;
                                                }
                                            }

                                            if (!foundGroup || lvMedia.Groups.Count == 0)
                                            {
                                                //set the group to show the star rating
                                                if (dvMedia.Table.Columns.Contains("stars") && dv["stars"] != null)
                                                {
                                                    lvig = new ListViewGroup(dv["stars"].ToString() + " Stars");
                                                    lvig.Name = dv["stars"].ToString();
                                                }
                                                else
                                                {
                                                    lvig = new ListViewGroup("0 Stars");
                                                    lvig.Name = "0 Stars";
                                                }

                                                lvMedia.Groups.Add(lvig);
                                            }
                                        }
                                        
                                        if (category.ToLower() == "playlist")
                                        {
                                            //set the group to display the playlist name
                                            lvig = new ListViewGroup(dv["Playlists"].ToString().Trim());
                                            lvig.Name = dv["Playlists"].ToString().Trim();

                                            lvMedia.Groups.Add(lvig);
                                        }

                                        //create media item
                                        Media media = MediaHandler.CreateMedia(dv);

                                        //add item
                                        ListViewItem listViewItem1 = new ListViewItem();

                                        string title = "  " + dv["title"].ToString().Trim();

                                        //make title 25 characters
                                        //titleMaxLength = lvMedia.Columns[0].Width - lvMedia.SmallImageList.ImageSize.Width - 60;

                                        if (title.Length > titleMaxLength)
                                        {
                                            title = title.Substring(0, titleMaxLength - 3) + "...";
                                            //title = title.Substring(0, titleMaxLength) + Environment.NewLine + title.Substring(titleMaxLength).PadRight(titleMaxLength, Convert.ToChar(" "));
                                        }
                                        else
                                            title = title.PadRight(titleMaxLength, Convert.ToChar(" "));

                                        listViewItem1.Text = title;
                                        listViewItem1.Tag = media;

                                        //get image info
                                        if (dv["coverImage"] != null && dv["coverImage"].ToString().Length > 0 && File.Exists(dv["coverImage"].ToString()))
                                        {
                                            imageKey = dv["coverImage"].ToString();

                                            if (!ilImages.Images.ContainsKey(imageKey))
                                            {
                                                try
                                                {
                                                    //if (File.Exists(imageKey))
                                                    //{
                                                    Bitmap newImage = new Bitmap(imageKey);
                                                    ilImages.Images.Add(imageKey, newImage);//add new image

                                                    media.ImageListIndex = Convert.ToString(ilImages.Images.Count - 1);

                                                    //}
                                                    //else
                                                    //  MessageBox.Show("Image not found " + Environment.NewLine + imageKey);
                                                }
                                                catch (Exception ex)
                                                {
                                                    //the image file was invalid
                                                    imageKey = "defaultImage";

                                                    media.ImageListIndex = "0";
                                                }
                                            }
                                            else
                                                media.ImageListIndex = ilImages.Images.IndexOfKey(imageKey).ToString();
                                        }
                                        else
                                        {
                                            imageKey = "defaultImage";

                                            media.ImageListIndex = "0";
                                        }

                                        //listViewItem1.ImageKey = imageKey;

                                        listViewItem1.Group = lvig;
                                        listViewItem1.ToolTipText = media.Title;

                                        if (media.Rating.Length > 0)
                                            listViewItem1.ToolTipText += " (" + media.Rating + ")";
                                        //else
                                        //    listViewItem1.ToolTipText += " (NA)";

                                        if (media.TagLine != null && media.TagLine.Length > 0)
                                            listViewItem1.ToolTipText += "\n" + media.TagLine;

                                        if (media.RatingDescription != null && media.RatingDescription.Length > 0)
                                        {
                                            listViewItem1.ToolTipText += "\n" + "(" + media.Rating + ")" + media.RatingDescription;
                                        }

                                        if (media.Description != null && media.Description.Length > 0)
                                            listViewItem1.ToolTipText += "\n" + media.Description;

                                        listViewItem1.ToolTipText += "\n" + dv["filePath"].ToString();
                                        
                                        lvMedia.Items.Add(listViewItem1);

                                        newItemCounter++;
                                    //}
                                }

                                tempIndex = 0;

                                for (int i = listViewStartCount; i < newItemCounter; i++)
                                {
                                    ListViewItem lvi = this.lvMedia.Items[i];

                                    if (!int.TryParse(((Media)lvi.Tag).ImageListIndex, out tempIndex))
                                        tempIndex = 0;

                                    lvi.ImageIndex = tempIndex;

                                    this.lvMedia.Invalidate(lvi.Bounds);
                                    this.Update();
                                }

                                lvMedia.EndUpdate();

                                break;
                        }
                    }
                }
                else
                {
                    if (category.ToLower() == "home" || category == "")
                    {
                        try
                        {
                            //show edit box
                            //SourcePathForm sourcePathForm = new SourcePathForm();
                            //sourcePathForm.SourcePaths = myMedia.Locations;
                            //sourcePathForm.ShowDialog(this);

                            ////update source paths
                            //myMedia.AddLocations(sourcePathForm.SourcePaths, true);

                            //lblOnlineContent.Visible = true;
                            //lblOnlineContent.BringToFront();

                            //displayBrowser();

                            //safeSurf.URL = new Uri("http://hulu.com");

                            //safeSurf.BringToFront();

                            //lblOnlineContent.BringToFront();
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message.Contains("The remote name could not be resolved:"))//the site is not available
                            {
                                Label lblNoResults = new Label();
                                lblNoResults.Text = "Choose your online content";
                                lblNoResults.Top = 50;
                                lblNoResults.Left = 50;
                                lblNoResults.Width = 250;
                                lblNoResults.BackColor = Color.White;
                                //lvMedia.Controls.Add(lblNoResults);
                                lvMedia.Controls.Add(lblNoResults);
                            }
                            else
                                Tools.WriteToFile(ex);
                        }
                    }
                    else
                    {
                        //ListViewGroup lvig = new ListViewGroup("     " + scrollingTabsGenre.SelectedTabs);
                        //lvMedia.Groups.Add(lvig);

                        ListViewItem listViewItem1 = new ListViewItem();
                        //listViewItem1.Group = lvig;
                        lvMedia.Items.Add(listViewItem1);

                        Label lblNoResults = new Label();
                        lblNoResults.Text = "No Results";
                        lblNoResults.Top = 50;
                        lblNoResults.Left = 50;
                        lblNoResults.Width = 150;
                        lblNoResults.BackColor = Color.White;
                        lvMedia.Controls.Add(lblNoResults);
                    }
                }
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }
        }

        private void lvMedia_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            //check to see if item is the last item in the listview and add more if necessary and available

            int textHeight = (int)(e.Graphics.MeasureString("WWW", lvMedia.Font).Height + 4);
            Brush myBrush = Brushes.Black;
            Rectangle rectBottom;
            Rectangle rectBackground;

            if ((e.State & ListViewItemStates.Selected) != 0)
            {
                // Draw the background and focus rectangle for a unselected item.
                //e.Graphics.FillRectangle(Brushes.Maroon, e.Bounds);

                //e.DrawFocusRectangle();

                //draw background gradient
                //LinearGradientBrush bgBrush = null;

                //if (e.Bounds.Width > 0 && e.Bounds.Height > 0)
                //{
                //    rectBackground = new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width + 4, e.Bounds.Height - 35);
                //    rectBottom = new Rectangle(e.Bounds.X, rectBackground.Location.Y + rectBackground.Height - 2, e.Bounds.Width + 4, e.Item.Bounds.Height - rectBackground.Height);

                //    //check to see if this item is the last one on the row and extend it's background to the right edge
                //    if (e.Item.Bounds.X + (e.Item.Bounds.Width * 2) >= lvMedia.Width)
                //    {
                //        rectBackground.Width = rectBackground.Width * 2;
                //        rectBottom.Width = rectBottom.Width * 2;
                //    }

                //    bgBrush = new LinearGradientBrush(rectBackground, Color.Transparent, Color.SlateGray, LinearGradientMode.Vertical);

                //    if (bgBrush != null)
                //    {
                //        using (bgBrush)
                //        {
                //            e.Graphics.FillRectangle(bgBrush, rectBackground);
                //            //e.Graphics.FillEllipse(brush, e.Bounds);
                //        }
                //    }

                //    bgBrush = new LinearGradientBrush(rectBottom, Color.SlateGray, Color.Transparent, LinearGradientMode.Vertical);

                //    if (bgBrush != null)
                //        using (bgBrush)
                //            e.Graphics.FillRectangle(bgBrush, rectBottom);
                //}

                if (e.Item.Bounds.Width > 0 && e.Item.Bounds.Height > 0)
                {
                    //draw media info
                    Rectangle rec = e.Item.Bounds;
                    rec.Height = e.Item.Bounds.Height;// - textHeight;
                    rec.Width = e.Item.Bounds.Width - 97;
                    rec.X = rec.X + 100;
                    rec.Y = rec.Y;// + textHeight;

                    e.Graphics.DrawString(((Media)e.Item.Tag).Title, this.Font, myBrush, rec, StringFormat.GenericDefault);

                    //draw option links
                    //rec.Height = 20;
                    //rec.Y = rec.Y + rec.Height + 1;

                    //e.Graphics.DrawString("more...", this.Font, myBrush, rec, StringFormat.GenericDefault);
                }
            }
            else
            {
                // Draw the background for a selected item.

                string rating = "";

                if (e.Item.Tag is Media)
                    rating = ((Media)e.Item.Tag).Rating.ToLower();

                LinearGradientBrush bgBrush = null;

                if (rating.Contains("pg-13") || rating.Contains("nc-17"))//blue background
                {
                    bgBrush = new LinearGradientBrush(e.Bounds, Color.AliceBlue, Color.LightBlue, LinearGradientMode.Horizontal);
                }
                else if (rating == "pg" || rating == "g")//green background
                {
                    bgBrush = new LinearGradientBrush(e.Bounds, Color.OldLace, Color.LightGreen, LinearGradientMode.Horizontal);
                }
                else if (rating == "r")//red background
                {
                    bgBrush = new LinearGradientBrush(e.Bounds, Color.OldLace, Color.LightSalmon, LinearGradientMode.Horizontal);
                }
                else//purple background
                {
                    bgBrush = new LinearGradientBrush(e.Bounds, Color.OldLace, Color.MediumPurple, LinearGradientMode.Horizontal);
                }

                if (bgBrush != null)
                {
                    using (bgBrush)
                        e.Graphics.FillRectangle(bgBrush, e.Bounds);
                }

                if (e.Item.Bounds.Width > 0 && e.Item.Bounds.Height > 0)
                {
                    //draw media info
                    Rectangle rec = e.Item.Bounds;
                    rec.Height = e.Item.Bounds.Height;// - textHeight;
                    rec.Width = e.Item.Bounds.Width - 97;
                    rec.X = rec.X + 100;
                    rec.Y = rec.Y;// + textHeight;

                    e.Graphics.DrawString(e.Item.ToolTipText, this.Font, myBrush, rec, StringFormat.GenericDefault);
                    //e.Graphics.DrawString(e.Item.Text, this.Font, myBrush, rec, StringFormat.GenericDefault);
                    
                    //draw option links
                    //rec.Height = 20;
                    //rec.Y = rec.Y + rec.Height + 1;

                    //e.Graphics.DrawString("more...", this.Font, myBrush, rec, StringFormat.GenericDefault);
                }
            }

            // Draw the item text for views other than the Details view.
            if (lvMedia.View != View.Details)
            {
                //Rectangle coverImageRect = new Rectangle(e.Item.Bounds.X, e.Item.Bounds.Y + textHeight, 87, 140);
                Rectangle coverImageRect = new Rectangle(e.Item.Bounds.X, e.Item.Bounds.Y, 87, 140);
                string coverImagePath = "";

                if (e.Item.Tag is Media)
                {
                    coverImagePath = ((Media)e.Item.Tag).coverImage;
                }
                else if (e.Item.Tag is OnlineMediaType)
                    coverImagePath = ((OnlineMediaType)e.Item.Tag).CoverImage;

                if (!File.Exists(coverImagePath))
                    coverImagePath = Application.StartupPath + "\\images\\media\\coverImages\\notavailable.jpg";

                System.Drawing.Image coverImage = System.Drawing.Image.FromFile(coverImagePath);

                //System.Drawing.Image tmpImage = new Bitmap(coverImage,coverImage.Width, coverImage.Height);

                //draw cover image outline
                try
                {
                    if (!e.Item.Checked)
                        using (Graphics g = Graphics.FromImage(coverImage))
                            g.DrawRectangle(Pens.Black, 0, 0, coverImage.Width - 1, coverImage.Height - 1);
                    else
                    {
                        Pen widePen = new Pen(Color.Yellow, 10);

                        using (Graphics g = Graphics.FromImage(coverImage))
                            g.DrawRectangle(widePen, 0, 0, coverImage.Width - 1, coverImage.Height - 1);
                    }
                }
                catch (Exception ex)
                {
                    Tools.WriteToFile(ex);
                }

                //e.Graphics.DrawRectangle(Pens.Black, 0, 0, tmpImage.Width - 1, tmpImage.Height - 1);

                //using (Graphics g = e.Graphics)
                //    g.DrawRectangle(Pens.Black, 0, 0, tmpImage.Width - 1, tmpImage.Height - 1);

                //draw cover image
                e.Graphics.DrawImage(coverImage, coverImageRect);

                //draw mirror image
                //if (rectBottom != null)
                //{
                //    System.Drawing.Imaging.ColorMatrix matrix = new System.Drawing.Imaging.ColorMatrix();
                //    matrix.Matrix33 = 0.3f; //opacity 0 = completely transparent, 1 = completely opaque

                //    System.Drawing.Imaging.ImageAttributes attributes = new System.Drawing.Imaging.ImageAttributes();
                //    attributes.SetColorMatrix(matrix, System.Drawing.Imaging.ColorMatrixFlag.Default, System.Drawing.Imaging.ColorAdjustType.Bitmap);

                //    if (coverImageRect.Width > 0 && coverImageRect.Height > 0)
                //    {
                //        Rectangle coverImageMirrorRectDest = new Rectangle(coverImageRect.X, coverImageRect.Y + coverImageRect.Height - 2, coverImageRect.Width, 25);
                //        Rectangle coverImageMirrorRectSrc = new Rectangle(coverImageRect.X, coverImageRect.Height - coverImageMirrorRectDest.Height, coverImageRect.Width, coverImageMirrorRectDest.Height);
                //        System.Drawing.Image coverImageMirror = System.Drawing.Image.FromFile(coverImagePath);
                //        coverImageMirror.RotateFlip(RotateFlipType.RotateNoneFlipY);

                //        e.Graphics.DrawImage(coverImageMirror, coverImageMirrorRectDest, 0, 0, coverImageMirror.Width, coverImageMirror.Height, GraphicsUnit.Pixel, attributes);
                //    }
                //}

                //bool value = false;
                //value = e.Item.Checked;

                //if (e.Item.Tag is Media && ((Media)e.Item.Tag).MediaType.ToLower() != "games")
                //    CheckBoxRenderer.DrawCheckBox(e.Graphics, new Point(e.Bounds.Left, e.Bounds.Top),
                //        value ? System.Windows.Forms.VisualStyles.CheckBoxState.CheckedNormal :
                //            System.Windows.Forms.VisualStyles.CheckBoxState.UncheckedNormal);

                e.Item.Text = "";
                e.DrawText();
            }
        }

        private void HandleClickEvents(object sender, MouseEventArgs e)
        {
            ListViewItem currentSelectedItem = lvMedia.GetItemAt(e.X, e.Y);

            if (e.Button != MouseButtons.Right && GetCheckBoxRectangle(currentSelectedItem).Contains(e.Location))//check / uncheck the item checkbox
            {
                currentSelectedItem.Checked = !currentSelectedItem.Checked;
                currentSelectedItem.Selected = currentSelectedItem.Checked;

                lvMedia.RedrawItems(currentSelectedItem.Index, currentSelectedItem.Index, false);
            }
            //else if (e.Button != MouseButtons.Right && !lvMedia.MultiSelect)
            else if (e.Button != MouseButtons.Right)
            {
                foreach (ListViewItem selectedItem in lvMedia.SelectedItems)
                {
                    if (selectedItem.Tag is Media)
                    {
                        Media media = (Media)selectedItem.Tag;

                        if (selectedGenres.ToLower().Contains("playlist"))
                        {
                            //start playing playlist from item clicked
                            try
                            {
                                ArrayList newPlaylist = new ArrayList();
                                //PlaylistItem selectedPlaylistItem = (PlaylistItem)selectedItem.Tag;

                                //newPlaylist.Add(media.filePath);

                                //iterate playlist
                                foreach (DataRow dr in mediaHandler.GetPlaylist(media.Playlists).Tables[0].Rows)
                                {
                                    if (!newPlaylist.Contains(dr["mediaSource"].ToString()))
                                        newPlaylist.Add(dr["mediaSource"].ToString());
                                }


                                //Cursor.Current = Cursors.WaitCursor;

                                //splashScreen mediaSplash = new splashScreen();

                                ////if (Form1.Mode == "Release")
                                ////{
                                //mediaSplash.SplashMessage2 = media.Playlists;

                                //mediaSplash.Show();

                                //SplashScreenNew.ShowSplashScreen();

                                //}

                                //this.Hide();





                                //mediaWindow = createMediaWindow(media);





                                //						mediaWindow.playVideo(selectedLabel.Tag.ToString());
                                //mediaWindow.PlayClip(selectedMedia);





                                //mediaWindow.PlayMedia(newPlaylist, "", "");





                                //if (Form1.Mode == "Release")
                                //{
                                //mediaSplash.Close();
                                //SplashScreenNew.CloseForm();
                                //}

                                //mediaWindow.ShowDialog(this);
                                //mediaWindow.Show();

                                ////Cursor.Current = Cursors.Arrow;

                                //mediaWindow.TopMost = true;

                            }
                            catch (Exception ex)
                            {
                                Tools.WriteToFile(Tools.errorFile, "Play playlist error: " + ex.Message);
                            }
                        }
                        else
                        {
                            switch (media.MediaType.ToLower())
                            {
                                case "tv":
                                    //string selectedGenres = scrollingTabsGenre.SelectedTabs;

                                    if (media.IMDBNum != null && media.IMDBNum.Length == 0)//display episodes
                                    {
                                        displayTVEpisodes(media.SeriesIMDBNum);

                                        SplashScreenNew.CloseForm();
                                    }
                                    else
                                    {
                                        SplashScreenNew.ShowSplashScreen(media);

                                        //play media
                                        playMedia(media);
                                    }
                                    break;
                                case "music":
                                    if (media.ReleaseID == null || media.ReleaseID.Length == 0)//display albums
                                    {
                                        displayAlbums(media.ArtistID);

                                        SplashScreenNew.CloseForm();
                                    }
                                    else if (media.RecordingID == null || media.RecordingID.Length == 0)//display songs
                                    {
                                        displaySongs(media.ReleaseID);

                                        SplashScreenNew.CloseForm();
                                    }
                                    else //play song
                                    {
                                        SplashScreenNew.ShowSplashScreen(media);

                                        //play media
                                        playMedia(media);
                                    }
                                    break;
                                case "pictures":
                                    //display picture
                                    //pbPictures.ImageLocation = media.filePath;
                                    //System.Drawing.Image thePicture = System.Drawing.Image.FromFile(media.filePath);

                                    //pbPictures.BackgroundImage = System.Drawing.Image.FromFile(media.filePath);
                                    //pbPictures.Visible = true;
                                    //btnClosePicture.Visible = true;
                                    //btnClosePicture.BringToFront();
                                    break;
                                case "games":
                                    SCTVFlash.FlashPlayer flashPlayer = new SCTVFlash.FlashPlayer();
                                    flashPlayer.WindowState = FormWindowState.Maximized;

                                    //flashPlayer.documentLoaded += new SCTV.FlashPlayer.DocumentLoaded(flashPlayer_documentLoaded);

                                    string url = media.filePath;

                                    if (!url.ToLower().StartsWith("file:\\\\"))
                                        url = "file:\\\\" + url;

                                    flashPlayer.GameToPlay = url;

                                    flashPlayer.Show(this);
                                    break;
                                default:
                                    SplashScreenNew.ShowSplashScreen(media);

                                    //play media
                                    playMedia(media);
                                    break;
                            }
                        }
                    }
                    else if (selectedItem.Tag is OnlineChannel)
                    {
                        //OnlineChannel channel = (OnlineChannel)selectedItem.Tag;

                        ////play channel
                        //SCTVJustinTV.JustinTV justinTV = new SCTVJustinTV.JustinTV("");
                        //justinTV.PlayChannel(channel);

                        //justinTV.ShowDialog(this);
                    }
                    else if (selectedItem.Tag is OnlineMediaType)
                    {
                        //OnlineMediaType onlineMedia = (OnlineMediaType)selectedItem.Tag;

                        ////display media
                        //displayBrowser();

                        //string url = onlineMedia.URL;

                        //if (!url.ToLower().StartsWith("http://"))
                        //    url = "http://" + url;

                        //safeSurf.URL = new Uri(url);

                        //if (url.ToLower().Contains("justin.tv"))
                        //    safeSurf.ShowJustinRecordButton = true;
                    }
                    else if (selectedItem.Tag is PlaylistItem)
                    {
                        //start playing playlist from item clicked
                        try
                        {
                            ArrayList newPlaylist = new ArrayList();
                            PlaylistItem selectedPlaylistItem = (PlaylistItem)selectedItem.Tag;

                            newPlaylist.Add(selectedPlaylistItem.MediaSource);

                            //iterate playlist
                            foreach (DataRow dr in mediaHandler.GetPlaylist(selectedPlaylistItem.PlaylistName).Tables[0].Rows)
                            {
                                if (!newPlaylist.Contains(dr["mediaSource"].ToString()))
                                    newPlaylist.Add(dr["mediaSource"].ToString());
                            }


                            //Cursor.Current = Cursors.WaitCursor;

                            //splashScreen mediaSplash = new splashScreen();

                            ////if (Form1.Mode == "Release")
                            ////{
                            //mediaSplash.SplashMessage2 = selectedPlaylistItem.PlaylistName;

                            //mediaSplash.Show();

                            //SplashScreenNew.ShowSplashScreen();

                            //}

                            //this.Hide();
                            //mediaWindow = createMediaWindow();
                            ////						mediaWindow.playVideo(selectedLabel.Tag.ToString());
                            ////mediaWindow.PlayClip(selectedMedia);
                            //mediaWindow.PlayMedia(newPlaylist, "", "");

                            //if (Form1.Mode == "Release")
                            //{
                            //mediaSplash.Close();
                            //SplashScreenNew.CloseForm();
                            //}

                            //mediaWindow.ShowDialog(this);
                            //mediaWindow.Show();

                            ////Cursor.Current = Cursors.Arrow;

                            //mediaWindow.TopMost = true;

                        }
                        catch (Exception ex)
                        {
                            Tools.WriteToFile(Tools.errorFile, "Play playlist error: " + ex.Message);
                        }
                    }
                    //else if (selectedItem.Tag is Game)
                    //{
                    //    Game gameToPlay = (Game)selectedItem.Tag;

                    //    SCTVFlash.FlashPlayer flashPlayer = new SCTVFlash.FlashPlayer();
                    //    flashPlayer.BringToFront();
                    //    //flashPlayer.documentLoaded += new SCTV.FlashPlayer.DocumentLoaded(flashPlayer_documentLoaded);

                    //    string url = gameToPlay.Location;

                    //    if (!url.ToLower().StartsWith("file:\\\\"))
                    //        url = "file:\\\\" + url;

                    //    flashPlayer.GameToPlay = url;

                    //    flashPlayer.Show();

                    //    SplashScreenNew.CloseForm();
                    //}
                    else
                        MessageBox.Show("Media type not supported");
                }
            }
            //else if(lvMedia.MultiSelect)//handle multi select
            //{
            //    ListViewItem lvItem = lvMedia.GetItemAt(e.X, e.Y);

            //    //check to see if the selected item is in our list
            //    if(selectedListviewItems.Contains(lvItem))//the item is already selected - deselect it
            //    {
            //        selectedListviewItems.Remove(lvItem);
            //        lvItem.Selected = false;
            //    }
            //    else //the item is not selected yet
            //        selectedListviewItems.Add(lvItem);

            //    foreach (ListViewItem listItem in selectedListviewItems)
            //        listItem.Selected = true;
            //}
        }

        private Rectangle GetCheckBoxRectangle(ListViewItem selectedItem)
        {
            Rectangle result = Rectangle.Empty;

            using (Graphics g = this.CreateGraphics())
            {
                Size sz = CheckBoxRenderer.GetGlyphSize(g, CheckBoxState.UncheckedNormal);
                result = new Rectangle(new Point(selectedItem.Bounds.X, selectedItem.Bounds.Y), sz);
            }
            return result;
        }

        /// <summary>
        /// find all tv episodes in selected show and display them
        /// </summary>
        /// <param name="category"></param>
        private void displayTVEpisodes(string seriesIMDBNum)
        {
            DataView dvMedia = mediaHandler.GetTVShowMedia(seriesIMDBNum);

            displayCategory(dvMedia, "TV", "TV");
        }

        /// <summary>
        /// play selected media
        /// </summary>
        private void playMedia(Media selectedMedia)//play selected media
        {
            try
            {
                int timesPlayed = 0;

                //update timesPlayed and lastplayed
                int.TryParse(selectedMedia.TimesPlayed, out timesPlayed);
                selectedMedia.TimesPlayed = Convert.ToString(timesPlayed + 1);
                selectedMedia.LastPlayed = DateTime.Now.ToString();
                mediaHandler.UpdateMediaInfo(selectedMedia);

                //string selectedMedia = getSelectedMediaFileName();

                string selectedFilePath = selectedMedia.filePath;

                if (selectedFilePath.Contains("|"))
                    selectedFilePath = selectedFilePath.Substring(0, selectedFilePath.IndexOf("|"));

                if (File.Exists(selectedFilePath))
                {
                    if (selectedMedia.MediaType.ToLower().Contains("document") || selectedMedia.MediaType.ToLower().Contains("books"))
                    {
                        //display media
                        //displayBrowser();

                        //string url = selectedFilePath;

                        //if (!url.ToLower().StartsWith("file:\\\\"))
                        //    url = "file:\\\\" + url;

                        //safeSurf.URL = new Uri(url);

                        //SplashScreenNew.CloseForm();
                    }
                    else
                    {
                        //Cursor.Current = Cursors.WaitCursor;

                        //splashScreen mediaSplash = new splashScreen();

                        ////if (Form1.Mode == "Release")
                        ////{
                        //mediaSplash.SplashMessage2 = selectedMedia.Title;

                        //mediaSplash.Show();

                        //mediaSplash.Refresh();



                        //this.Refresh();
                        //}

                        //this.Hide();
                        mediaWindow = createMediaWindow(selectedMedia);

                        //mediaWindow.MediaSplash = mediaSplash;
                        mediaWindow.PlaySequels += new liquidMediaPlayer.Sequels(mediaWindow_PlaySequels);
                        mediaWindow.ContinousPlayOn += new liquidMediaPlayer.ContinousPlaySelected(mediaWindow_ContinousPlay);
                        mediaWindow.StartedPlaying += new liquidMediaPlayer.startedPlaying(mediaWindow_StartedPlaying);
                        currentMedia = selectedMedia;
                        //						mediaWindow.playVideo(selectedLabel.Tag.ToString());
                        mediaWindow.PlayClip(selectedMedia);

                        //mediaWindow.Show();
                        //mediaWindow.TopMost = true;

                        //Cursor.Current = Cursors.Arrow;


                    }
                }
                else
                {
                    Tools.WriteToFile(Tools.errorFile, "---- Missing File : " + selectedMedia + " ----");
                    MessageBox.Show("Missing File - " + selectedMedia);
                }
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, "PlayMedia error: " + ex.Message);
            }
        }
        
        private liquidMediaPlayer createMediaWindow(Media selectedMedia)
        {
            try
            {
                this.Invalidate();

                if (mediaWindow != null)
                {
                    mediaWindow.stop();
                    mediaWindow.Dispose();
                    mediaWindow = null;
                }

                mediaWindow = new liquidMediaPlayer();

                //if (speechListener != null)
                //    mediaWindow.SpeechListener = speechListener;

                //mediaWindow.Closed += new EventHandler(mediaWindow_Closed);
                mediaWindow.closingForm += new liquidMediaPlayer.closing(mediaWindow_MediaDonePlaying);
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, "createMediaWindow error: " + ex.Message);
            }

            return mediaWindow;
        }

        void mediaWindow_MediaDonePlaying()
        {
            //check for sequel
            if (mediaWindow.PlaySequel || mediaWindow.ContinousPlay)
            {
                Media theSequel;

                if (mediaWindow.WhatsNext == null)
                    theSequel = mediaHandler.GetSequel(currentMedia, continousPlay);
                else
                    theSequel = mediaWindow.WhatsNext;

                if (theSequel != null)
                    playMedia(theSequel);
            }
        }

        void mediaWindow_StartedPlaying()
        {
            //mediaWindow.Show();
            //mediaWindow.TopMost = true;

            //SplashScreenNew.CloseForm();
            //mediaSplash.Close();
        }

        void mediaWindow_PlaySequels()
        {
            playSequels = true;
        }

        void mediaWindow_ContinousPlay()
        {
            continousPlay = true;
        }

        /// <summary>
        /// Get albums for selected artist
        /// </summary>
        /// <param name="artistID"></param>
        private void displayAlbums(string artistID)
        {
            DataView dvMedia = mediaHandler.GetAlbumsMedia(artistID);

            displayCategory(dvMedia, "Music", "Music");
        }

        /// <summary>
        /// Get songs for selected album
        /// </summary>
        /// <param name="releaseID"></param>
        private void displaySongs(string releaseID)
        {
            DataView dvMedia = mediaHandler.GetSongsMedia(releaseID);

            displayCategory(dvMedia, "Music", "Music");
        }

        private void lvMedia_MouseClick(object sender, MouseEventArgs e)
        {
            HandleClickEvents(sender, e);
        }
    }
}
