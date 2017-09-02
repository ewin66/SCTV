using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace SCTV
{
    public partial class MediaDetails : Form
    {
        string movieTitle = "";
        Media mediaToEdit = new Media();
        
        public Media MediaToEdit
        {
            set
            {
                mediaToEdit = value; 
            }

            get
            {
                return mediaToEdit;
            }
        }

        public MediaDetails(Media media, DataView availableCategories, ArrayList availableMediaCategories)
        {
            InitializeComponent();

            MediaToEdit = media;

            populateForm(media, availableCategories, availableMediaCategories);
        }

        /// <summary>
        /// populate search ui with given media
        /// </summary>
        /// <param name="value"></param>
        private void populateForm(Media media, DataView availableCategories, ArrayList availableMediaCategories)
        {
            try
            {
                ArrayList categories = new ArrayList();
                ArrayList mediaCategories = new ArrayList();

                txtTitleResult.Text = media.Title;
                txtDirectorResult.Text = media.Director;

                string[] catArray;
                if(media.category.Contains("/"))
                    catArray = media.category.Split('/');
                else
                    catArray = media.category.Split('|');

                string[] medCatArray;
                if(media.MediaType.Contains("/"))
                    medCatArray = media.MediaType.Split('/');
                else
                    medCatArray = media.MediaType.Split('|');

                foreach (DataRowView drv in availableCategories)
                {
                    string catString = drv["category"].ToString();

                    string[] tempCatArray;

                    if (catString.Contains("/"))
                        tempCatArray = catString.Split('/');
                    else
                        tempCatArray = catString.Split('|');

                    foreach (string cat in tempCatArray)
                    {
                        if (!categories.Contains(cat.Trim()) && cat.Trim().Length > 0)
                            categories.Add(cat.Trim());
                    }
                }

                //add current categories
                foreach (string category in catArray)
                {
                    if(category.Trim().Length > 0)
                        lbCurrentGenres.Items.Add(category.Trim());

                    if (categories.Contains(category.Trim()))
                        categories.Remove(category.Trim());
                }

                categories.TrimToSize();

                //add available categories
                foreach (string category in categories)
                    lbAvailableGenres.Items.Add(category.Trim());

                foreach (string catString in availableMediaCategories)
                {
                    //check for both "/" and "|"
                    string[] tempCatArray;

                    if (catString.Contains("/"))
                        tempCatArray = catString.Split('/');
                    else
                        tempCatArray = catString.Split('|');

                    foreach (string cat in tempCatArray)
                    {
                        if (!mediaCategories.Contains(cat.Trim()) && cat.Trim().Length > 0)
                            mediaCategories.Add(cat.Trim());
                    }
                }

                //add current mediaCategories
                foreach (string category in medCatArray)
                {
                    if (category.Trim().Length > 0)
                        lbSelectedCategories.Items.Add(category.Trim());

                    if (mediaCategories.Contains(category.Trim()))
                        mediaCategories.Remove(category.Trim());
                }

                mediaCategories.TrimToSize();

                //add available mediaCategories
                foreach (string category in mediaCategories)
                    lbAvailableCategories.Items.Add(category.Trim());

                txtReleaseYearResult.Text = media.ReleaseYear;
                txtTaglineResult.Text = media.Description;
                txtImdbNumResult.Text = media.IMDBNum;
                txtRatingResult.Text = media.Rating;
                txtRatingReasonResult.Text = media.RatingDescription;
                thumbnailPictureBox.ImageLocation = media.coverImage;
            }
            catch (Exception ex)
            {

            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string selectedCategories = "";
            string selectedMediaTypes = "";

            mediaToEdit.Title = txtTitleResult.Text;
            mediaToEdit.Director = txtDirectorResult.Text;

            foreach (string category in lbCurrentGenres.Items)
            {
                if (category.Trim().Length > 0)
                    selectedCategories += category.Trim() + " / ";
            }

            mediaToEdit.category = selectedCategories;
            mediaToEdit.ReleaseYear = txtReleaseYearResult.Text;
            mediaToEdit.Description = txtTaglineResult.Text;
            mediaToEdit.IMDBNum = txtImdbNumResult.Text;
            mediaToEdit.Rating = txtRatingReasonResult.Text;
            mediaToEdit.RatingDescription = txtRatingReasonResult.Text;

            foreach (string category in lbSelectedCategories.Items)
            {
                if (category.Trim().Length > 0)
                    selectedMediaTypes += category.Trim() + " / ";
            }

            MediaToEdit.MediaType = selectedMediaTypes;

            this.DialogResult = DialogResult.OK;

            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAddGenre_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    ArrayList movedItems = new ArrayList();

            //    foreach (string selectedItem in lbAvailableGenres.SelectedItems)
            //    {
            //        lbCurrentGenres.Items.Add(selectedItem);

            //        movedItems.Add(selectedItem);
            //    }

            //    foreach(string itemToRemove in movedItems)
            //        lbAvailableGenres.Items.Remove(itemToRemove);
            //}
            //catch (Exception ex)
            //{
            //    throw;
            //}

            moveListboxItems(lbAvailableGenres, lbCurrentGenres);
        }

        private void btnRemoveGenre_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    ArrayList movedItems = new ArrayList();

            //    foreach (string selectedItem in lbCurrentGenres.SelectedItems)
            //    {
            //        lbAvailableGenres.Items.Add(selectedItem);

            //        movedItems.Add(selectedItem);
            //    }

            //    foreach(string itemToRemove in movedItems)
            //        lbCurrentGenres.Items.Remove(itemToRemove);
            //}
            //catch (Exception ex)
            //{

            //    throw;
            //}

            moveListboxItems(lbCurrentGenres, lbAvailableGenres);
        }

        private void btnRemoveCategory_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    ArrayList movedItems = new ArrayList();

            //    foreach (string selectedItem in lbSelectedCategories.SelectedItems)
            //    {
            //        lbAvailableCategories.Items.Add(selectedItem);

            //        movedItems.Add(selectedItem);
            //    }

            //    foreach (string itemToRemove in movedItems)
            //        lbSelectedCategories.Items.Remove(itemToRemove);
            //}
            //catch (Exception ex)
            //{

            //    throw;
            //}

            moveListboxItems(lbSelectedCategories, lbAvailableCategories);
        }

        private void btnAddCategory_Click(object sender, EventArgs e)
        {
            moveListboxItems(lbAvailableCategories, lbSelectedCategories);
        }

        /// <summary>
        /// Move selected item to next listbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lbAvailableCategories_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            moveListboxItems(lbAvailableCategories, lbSelectedCategories);
        }

        private void lbSelectedCategories_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            moveListboxItems(lbSelectedCategories, lbAvailableCategories);
        }

        /// <summary>
        /// Move selected item to next listbox
        /// </summary>
        /// <param name="lbSource"></param>
        /// <param name="lbDestination"></param>
        /// <param name="valueToMove"></param>
        private void moveListboxItems(ListBox lbSource, ListBox lbDestination)
        {
            try
            {
                ArrayList movedItems = new ArrayList();

                foreach (string selectedItem in lbSource.SelectedItems)
                {
                    lbDestination.Items.Add(selectedItem);

                    movedItems.Add(selectedItem);
                }

                foreach (string itemToRemove in movedItems)
                    lbSource.Items.Remove(itemToRemove);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private void lbAvailableGenres_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            moveListboxItems(lbAvailableGenres, lbCurrentGenres);
        }

        private void lbCurrentGenres_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            moveListboxItems(lbCurrentGenres, lbAvailableGenres);
        }

        private void btnAddNewCategory_Click(object sender, EventArgs e)
        {
            AddNewItem addItem = new AddNewItem();

            DialogResult result = addItem.ShowDialog(this);

            if (result == DialogResult.Yes)
                lbSelectedCategories.Items.Add(addItem.ItemToAdd);
        }

        private void btnAddNewGenre_Click(object sender, EventArgs e)
        {
            AddNewItem addItem = new AddNewItem();

            DialogResult result = addItem.ShowDialog(this);

            if(result == DialogResult.Yes)
                lbCurrentGenres.Items.Add(addItem.ItemToAdd);
        }
    }
}
