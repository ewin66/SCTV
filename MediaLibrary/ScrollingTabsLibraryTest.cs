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
    //static class transparent
    //{
    //    public static void ToTransparent(this System.Windows.Forms.Button Button, System.Drawing.Color TransparentColor)
    //    {
    //        Bitmap bmp = ((Bitmap)Button.Image);
    //        bmp.MakeTransparent(TransparentColor);
    //        int x = (Button.Width - bmp.Width) / 2;
    //        int y = (Button.Height - bmp.Height) / 2;
    //        Graphics gr = Button.CreateGraphics();
    //        gr.DrawImage(bmp, x, y);
    //    }
    //}

    public partial class ScrollingTabsLibraryTest : Form
    {
        public ScrollingTabsLibraryTest()
        {
            InitializeComponent();
            
            horizontalScrollingListview2.DisplayCategory("action", "", "movies");

            //var pos = this.PointToScreen(button1.Location);
            //pos = lvTest.PointToClient(pos);
            //button1.Parent = lvTest;
            //button1.Location = pos;
            //SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            //button1.BackColor = Color.Transparent;
            //button1.Visible = false;
            //lvTest.Controls.Add(button1);

            //SCTV.TransparentControl btnTransparent = new SCTV.TransparentControl();

            //btnTransparent.Location = new Point(10,10);
            //this.Controls.Add(btnTransparent);
            ////lvTest.Controls.Add(btnTransparent);
            
            //btnTransparent.Text = "testing";
            ////btnTransparent.Refresh();
            //btnTransparent.Opacity = 50;

            //Bitmap bmp = new Bitmap(100, 100);
            //button2.Image = bmp;
            //button2.ToTransparent(Color.Red);

            //ArrayList testTabs = new ArrayList();
            //testTabs.Add("test1");
            //testTabs.Add("test2");
            //testTabs.Add("test3");
            //testTabs.Add("test4");
            //testTabs.Add("test5");

            //SCTV.Controls.ScrollingTabs scrollingTabs = new SCTV.Controls.ScrollingTabs();
            //scrollingTabs.Tabs = testTabs;

            ////dgTesting.Rows[0].Cells[0].control

            //ListViewItem li = new ListViewItem();

        }

        private void button1_MouseHover(object sender, EventArgs e)
        {
            //iterate items in listview and find the first one to the right that is not visible and make it visible
            //lvTest.

            bool foundVisible = false;            

            //foreach(ListViewItem item in lvTest.Items)
            //{
            //    if(foundVisible)
            //    {
            //        //if (!lvTest.ClientRectangle.IntersectsWith(item.GetBounds(ItemBoundsPortion.Entire)))
            //        //{
            //        //lvTest.SetScrollX(50);
            //        lvTest.ScrollLeft();
            //            //lvTest.AutoScrollOffset = new Point(-50, 0);
            //            //listBox1.AutoScrollOffset = new Point(listBox1.AutoScrollOffset.X, 10);
            //            //item.EnsureVisible();
            //            //item.Selected = true;
            //            //item.Position = new Point(item.Position.X + 100, item.Position.Y);
            //            break;
            //        //}
            //    }
            //    else if (lvTest.ClientRectangle.IntersectsWith(item.GetBounds(ItemBoundsPortion.Entire)))
            //    {
            //        foundVisible = true;

            //    }
                    
            //}
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            //lvTest.MarqueeSpeed = 0;
        }

        private void button2_MouseHover(object sender, EventArgs e)
        {
            bool foundVisible = false;

            //foreach (ListViewItem item in lvTest.Items)
            //{
                
            //    if (lvTest.ClientRectangle.IntersectsWith(item.GetBounds(ItemBoundsPortion.Entire)))
            //    {
            //        foundVisible = true;
            //        lvTest.ScrollRight();
            //        break;
            //    }

            //}
        }

        private void button2_MouseLeave(object sender, EventArgs e)
        {
            //lvTest.MarqueeSpeed = 0;
        }
    }
}
