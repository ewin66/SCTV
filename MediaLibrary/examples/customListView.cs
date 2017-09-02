class MyForm3 : Form 
{ 
   RasterImageList imageList; 
   RasterCodecs codecs; 
   public MyForm3(string title) 
   { 
      Text = title; 
      // Set the size of the form 
      Size = new Size(400, 200); 
 
      // Create a new RasterImageList control 
      imageList = new RasterImageList(); 
      imageList.Dock = DockStyle.Fill; 
      imageList.SelectionMode = RasterImageListSelectionMode.Single; 
      imageList.Size = Size; 
      Controls.Add(imageList); 
      imageList.BringToFront(); 
 
      codecs = new RasterCodecs(); 
 
      // Create three items 
      string imagesPath = LeadtoolsExamples.Common.ImagesPath.Path; 
 
      for(int i = 0; i < 3; i++) 
      { 
         // Load the image 
         int index = i + 1; 
         string imageFileName = imagesPath + "Image" + index.ToString() + ".cmp"; 
         RasterImage image = codecs.Load(imageFileName, 0, CodecsLoadByteOrder.BgrOrGray, 1, 1); 
         RasterImageListItem item = new RasterImageListItem(image, 1, "Item" + index.ToString()); 
 
         // Select the first item 
         if(i == 0) 
            item.Selected = true; 
 
         // Add the item to the image list 
         imageList.Items.Add(item); 
      } 
 
      // Change the item size 
      imageList.ItemSize = new Size(200, 200); 
 
      // Change the item image size 
      imageList.ItemImageSize = new Size(120, 120); 
 
      // We are going to draw the items ourselves 
      imageList.ViewStyle = RasterImageListViewStyle.OwnerDraw; 
 
      // Add a handler to the DrawItem event 
      imageList.DrawItem += new EventHandler<RasterImageListDrawItemEventArgs>(rasterImageList_DrawItem); 
 
      // Add the RasterImageList to the control collection. 
      Controls.Add(imageList); 
   } 
 
   private void rasterImageList_DrawItem(object sender, RasterImageListDrawItemEventArgs e) 
   { 
      RasterImageListItem item = e.Item; 
      RasterImageList imageList = item.ImageList; 
      Graphics g = e.Graphics; 
 
      // get the item rectangle 
      Rectangle rc = imageList.GetItemRectangle(item); 
 
      // sanity check 
      if(rc.IsEmpty) 
         return; 
 
      // we want to draw a 1 pixel black rectangle around the item 
      // then we fill the inside of the rectangle with white if the item 
      // is not selected or lightgray if it is 
 
      g.DrawRectangle(Pens.Black, rc.Left, rc.Top, rc.Width - 1, rc.Height - 1); 
 
      // we used up 1 pixel 
      rc.Inflate(-1, -1); 
 
      Brush b; 
      if(item.Selected) 
         b = Brushes.LightGray; 
      else 
         b = Brushes.White; 
      g.FillRectangle(b, rc); 
 
      // calculate the rectangles for image and text 
      if(imageList.ShowItemText) 
      { 
         // text is visible 
         // draw the text at the bottom of the item 
         int textHeight = (int)(g.MeasureString("WWW", imageList.Font).Height + 4); 
         Rectangle textRect = Rectangle.FromLTRB( 
            rc.Left, 
            rc.Bottom - textHeight, 
            rc.Right, 
            rc.Bottom); 
 
         if(!textRect.IsEmpty) 
         { 
            StringFormat sf = new StringFormat(); 
            sf.Alignment = StringAlignment.Center; 
            sf.LineAlignment = StringAlignment.Center; 
            sf.Trimming = StringTrimming.EllipsisPath; 
            sf.FormatFlags = StringFormatFlags.NoWrap; 
 
            g.DrawString( 
               item.Text, 
               imageList.Font, 
               Brushes.Black, 
               textRect, 
               sf); 
            sf.Dispose(); 
 
            // we need to update the item rectangle for the space 
            // we used up to draw the text 
            rc.Height -= textRect.Height; 
         } 
      } 
 
      // rc is the image rectangle 
      if(!rc.IsEmpty) 
      { 
         // now rc holds the rectangle to draw the image into 
 
         // first, set the correct page 
         int savePage = -1; 
         if(item.Image.Page != item.Page) 
         { 
            // the page is different 
 
            // save current image page so we can set it back when we are done 
            savePage = item.Image.Page; 
 
            // disable the image events, we are going to set the page back, 
            // so we do not want anybody subscribing to this image Changed 
            // event to know we changed it. 
            item.Image.DisableEvents(); 
 
            // set new page 
            item.Image.Page = item.Page; 
         } 
 
         try 
         { 
            // we want to center the image into whatever left of rc 
            Size itemImageSize = imageList.ItemImageSize; 
            Rectangle imageRect = new Rectangle( 
               rc.Left + (rc.Width - itemImageSize.Width) / 2, 
               rc.Top + (rc.Height - itemImageSize.Height) / 2, 
               itemImageSize.Width, 
               itemImageSize.Height); 
 
            // we want to keep the aspect ratio 
            imageRect = RasterImageList.GetFixedAspectRatioImageRectangle( 
               item.Image.ImageWidth, 
               item.Image.ImageHeight, 
               imageRect); 
 
            // draw the image 
            item.Image.Paint(g, imageRect, imageList.PaintProperties); 
 
            // finally, draw a black rectangle around the image 
            imageRect.Inflate(1, 1); 
            g.DrawRectangle( 
               Pens.Black, 
               imageRect.Left, 
               imageRect.Top, 
               imageRect.Width - 1, 
               imageRect.Height - 1); 
         } 
         finally 
         { 
            // reset the old page 
            if(savePage != -1) 
            { 
               item.Image.Page = savePage; 
 
               // re-enable the events 
               item.Image.EnableEvents(); 
            } 
         } 
      } 
   } 
} 
 
public void RasterImageList_DrawItem(string title) 
{ 
   RasterCodecs.Startup(); 
   MyForm3 form = new MyForm3(title); 
   form.ShowDialog(); 
   RasterCodecs.Shutdown(); 
}