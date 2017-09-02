using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace MyTest
{
    class takeScreenShot
    {
        string gamePath = "";

        public delegate void Done();
        public event Done done;

        public takeScreenShot(string GamePath, Rectangle pictureArea)
        {
            gamePath = GamePath;

            //System.Threading.Thread screenShotThread = new System.Threading.Thread(takeScreenShot);
            //screenShotThread.Start(games[gameIndex].ToString().Substring(games[gameIndex].ToString().LastIndexOf("\\")).Replace(".swf", ".jpeg"));
            
            

            done();
        }

        private void takeTheShot()
        {
            ////Bitmap bmpScreenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, PixelFormat.Format32bppArgb);
            //Bitmap bmpScreenshot = new Bitmap(flashPlayer.Bounds.Width, flashPlayer.Bounds.Height, PixelFormat.Format32bppArgb);
            //// Create a graphics object from the bitmap
            //Graphics gfxScreenshot = Graphics.FromImage(bmpScreenshot);
            //// Take the screenshot from the upper left corner to the right bottom corner
            //Size gameSize = new Size(flashPlayer.Width, flashPlayer.Height - 15);
            ////gfxScreenshot.CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, 0, 0, gameSize, CopyPixelOperation.SourceCopy);
            //gfxScreenshot.CopyFromScreen(flashPlayer.Bounds.X, flashPlayer.Bounds.Y + 15, 0, 0, gameSize, CopyPixelOperation.SourceCopy);
            //// Save the screenshot to the specified path that the user has chosen
            //string savePath = Application.StartupPath + "\\images\\games\\thumbnails" + gameName.ToString();
            //bmpScreenshot.Save(savePath, ImageFormat.Jpeg);
        }
    }
}
