using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SCTVObjects;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Collections;
using System.Drawing.Text;
using System.IO;
using System.Reflection;

namespace MyTest
{
    public partial class TestHarness : Form
    {
        glassFormTest myForm;
        System.Drawing.Image testImage;
        System.Drawing.Image testImageShadow;
        System.Drawing.Image testImageReflection;
        PrivateFontCollection pfc = new PrivateFontCollection();
        TriviaBar triviaBar;
        static SCTV.FlashPlayer flashPlayer = new SCTV.FlashPlayer();
        static ArrayList games;
        static int gameIndex = 0;
        static string currentGamePath = "";

        public delegate void DoneWithScreenShot();
        public event DoneWithScreenShot doneWithScreenShot;

        public TestHarness()
        {
            InitializeComponent();
            populateTabs();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //displayImage();

            //initFonts();

            //SCTVObjects.Fonts fonts = new Fonts();

            //scrollingTabs1.Font = fonts.CustomFont("graveyrd.ttf");
        }

        private void populateTabs()
        {
            ArrayList tabs = new ArrayList();
            tabs.Add("test 1");
            tabs.Add("test 2");
            tabs.Add("3");
            tabs.Add("test 4");
            tabs.Add("test 5");
            tabs.Add("test 6");
            tabs.Add("test 7");
            tabs.Add("test 8");
            tabs.Add("test 9");
            tabs.Add("test 10");

            //scrollingTabs1.Tabs = tabs;
            ////scrollingTabs1.SelectedTabs = "test 1";
            //scrollingTabs1.SelectedTabIndexes = "2";
            //scrollingTabs1.MultiSelect = false;
            //SCTVObjects.Fonts fonts = new Fonts();

            //scrollingTabs1.Font = fonts.CustomFont("ChocolateDropsNF");
        }

        private void displayImage()
        {
            testImage = System.Drawing.Image.FromFile("images/RaidersoOfTheLostArk.jpg");
            testImageShadow = (System.Drawing.Image)testImage.Clone();
            testImageReflection = (System.Drawing.Image)testImage.Clone();



            ////shadow
            ////Image i = Image.FromFile(@"C:\Data\CustomerProjects\Inteevo\ Projects\AlterEgo\CharacterEditor\Samples\Lady1\La d__0001.png");
            ////System.Drawing.Image.Image shadow = (System.Drawing.Image.Image)i.Clone();
            //System.Drawing.Image resultImage = new Bitmap(testImage.Width+7, testImage.Height+7);
            //Graphics resultCanvas = Graphics.FromImage(resultImage);
            //Graphics shadowCanvas = Graphics.FromImage(testImageShadow);

            ////Need to preserve the alpha in this next line
            //resultCanvas.FillRectangle(Brushes.Black, new Rectangle(7, 7, 300, 400));


            ////reflection
            ////testImageReflection.RotateFlip(RotateFlipType.RotateNoneFlipY);
            //pbTestImage.Image = testImage;

            //int intWidth = Convert.ToInt16(pbTestImage.Width);
            //int intHeight = Convert.ToInt16(pbTestImage.Height);

            ////banner.GradientDirection = Convert.ToInt16(rcbDirection.SelectedValue);
            ////LinearGradientMode gradientMode;
            ////if (banner.GradientDirection == 1)
            ////    gradientMode = LinearGradientMode.Vertical;
            ////else
            ////    gradientMode = LinearGradientMode.Horizontal;

            ////using (Bitmap bitmap = new Bitmap(intWidth, intHeight))
            //    //using (Graphics graphics = Graphics.FromImage(testImage))
            //    //    using (LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, intWidth, intHeight),
            //    //           Color.AliceBlue, Color.Blue, LinearGradientMode.ForwardDiagonal))
            //    //    {
            //    //        //brush.SetSigmaBellShape(intWidth / 100, intHeight / 100);
            //    //        graphics.FillRectangle(brush, new Rectangle(0, 0, intWidth, intHeight));
            //    //        System.IO.MemoryStream stream = new System.IO.MemoryStream();
            //    //        testImage.Save(stream, ImageFormat.Jpeg);
            //    //        Byte[] bytes = stream.ToArray();

            //    //        pbTestImage.Image = testImage;
            //    //    }

            

            ////resultCanvas.FillRectangle(
            //resultCanvas.DrawImage(testImage, new Point(0, 0));
            ////pictureBox1.Image = resultImage;





            

            ////MakeShadow((Bitmap)testImageClone);






            ////pbTestImage.Image = resultImage;
        }

        private static void MakeShadow(Bitmap dest)
        {
            BitmapData bits = dest.LockBits(new Rectangle(0, 0, dest.Width,
                dest.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            IntPtr bottomScanLine = bits.Scan0;
            int bytesPerRow = bits.Width * 4;

            unsafe
            {
                byte* pixelValue = (byte*)bottomScanLine.ToPointer();
                for (int count = 0; count < bits.Width * bits.Height; count++)
                {
                    pixelValue[0] = 0;
                    pixelValue[1] = 0;
                    pixelValue[2] = 0;
                    pixelValue = pixelValue + 4;
                }
            }
            dest.UnlockBits(bits);
        }

        /// <summary>
        /// Creates a glass window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            this.TopMost = true;

            if (myForm != null)
                myForm.Close();

            myForm = new glassFormTest();
            //myForm.GlassOpacity = 1;
            //myForm.BackgroundImagePath = "images/tooltip.gif";
            //myForm.Location = Cursor.Position;
            //myForm.DisplayMaximize = false;
            //myForm.DisplayMinimize = false;
            //myForm.ActiveArea = new Rectangle(40, 10, 600, 400);
            myForm.Show();


            
        }

        private void pbTestImage_Paint(object sender, PaintEventArgs e)
        {
            //LinearGradientBrush bgBrush = null;

            //Rectangle rectBackground = new Rectangle(e.ClipRectangle.X, e.ClipRectangle.Y + 7, e.ClipRectangle.Width, e.ClipRectangle.Height);

            //bgBrush = new LinearGradientBrush(rectBackground, Color.White, Color.Silver, LinearGradientMode.Vertical);

            //if (bgBrush != null)
            //{
            //    using (bgBrush)
            //    {
            //        e.Graphics.FillRectangle(bgBrush, rectBackground);
            //        //e.Graphics.FillEllipse(brush, e.Bounds);
            //    }
            //}

            //pbTestImage.Image = testImage;
            //pbTestImage.BringToFront();


            bool bold = false;

            bool regular = false;

            bool italic = false;

            e.Graphics.PageUnit = GraphicsUnit.Point;

            SolidBrush b = new SolidBrush(Color.Black);

            float y = 5;

            System.Drawing.Font fn;

            foreach (FontFamily ff in pfc.Families)
            {
                if (ff.IsStyleAvailable(FontStyle.Regular))
                {
                    regular = true;

                    fn = new Font(ff, 18, FontStyle.Regular);

                    e.Graphics.DrawString(fn.Name, fn, b, 5, y, StringFormat.GenericTypographic);

                    fn.Dispose();

                    y += 20;
                }

                if (ff.IsStyleAvailable(FontStyle.Bold))
                {
                    bold = true;

                    fn = new Font(ff, 18, FontStyle.Bold);

                    e.Graphics.DrawString(fn.Name, fn, b, 5, y, StringFormat.GenericTypographic);

                    fn.Dispose();

                    y += 20;
                }

                if (ff.IsStyleAvailable(FontStyle.Italic))
                {
                    italic = true;

                    fn = new Font(ff, 18, FontStyle.Italic);

                    e.Graphics.DrawString(fn.Name, fn, b, 5, y, StringFormat.GenericTypographic);

                    fn.Dispose();

                    y += 20;
                }

                if (bold && italic)
                {
                    fn = new Font(ff, 18, FontStyle.Bold | FontStyle.Italic);

                    e.Graphics.DrawString(fn.Name, fn, b, 5, y, StringFormat.GenericTypographic);

                    fn.Dispose();

                    y += 20;
                }

                fn = new Font(ff, 18, FontStyle.Underline);

                e.Graphics.DrawString(fn.Name, fn, b, 5, y, StringFormat.GenericTypographic);

                fn.Dispose();

                y += 20;

                fn = new Font(ff, 18, FontStyle.Strikeout);

                e.Graphics.DrawString(fn.Name, fn, b, 5, y, StringFormat.GenericTypographic);

                fn.Dispose();
            }

            b.Dispose();
        }

        private void btnShowSplashScreen_Click(object sender, EventArgs e)
        {
            
            Media newMedia = new Media();
            newMedia.coverImage = "images\\RaidersoOfTheLostArk.jpg";
            newMedia.Description = "my test description";
            newMedia.Goofs = "||Continuity: During Tai-Lung's escape scene, he tosses 4 spears into the air. When he jumps up he only kicks 3 spears, but 4 are shown crashing into the prison wall.|||Continuity: After Po accidentally breaks the Urn of Whispering Warriors, the pieces move around on the floor.|||Continuity: Po gets all dirty when when he finally lands in the arena, but after he is selected as the Dragon Warrior, he is clean again.||||||Continuity: When Master Shifu throws two clay discs into the air for Master Tigress to demonstrate the proper use of a split kick, you can see more on the ground. The aerial view is missing the other discs.|||Continuity: When Po wakes up in the beginning of the movie a throwing star falls down and lands on the floor. Later when he tries to throw it back up on the wall you see the original throwing star is still in the wall.|||Continuity: When Po tries to use the bamboo to vault into the ceremony he makes a big crack in the wall - however this disappears right away as it is never to be seen again.|||Continuity: Before Po catches the last dumpling, he jumps over and behind Shifu. In the following close shot, we see Shifu looking straight up as if Po was still right above him. Moreover in this close shot, Po's shadow passes over Shifu. This is inconsistent too, as Po's shadow clearly points away from Shifu in the next shot. As Po was already behind Shifu, so was his shadow.|||Factual errors: Tai Lung, a snow leopard, is at times heard emitting a tiger-like roar - a sound snow leopards are unable to make due to their lack of a larynx.||||";
            newMedia.Title = "Testing";

            if (SCTVObjects.SplashScreenNew.SplashForm != null)
                SCTVObjects.SplashScreenNew.CloseForm();
            else
            {
                SCTVObjects.SplashScreenNew.ShowSplashScreen(newMedia);
            }

        }

        private void scrollingTabs1_SelectionChanged()
        {

        }

        private void initFonts()
        {
            SCTVObjects.Fonts fonts = new Fonts();

            foreach (string name in fonts.GetFonts())
            {
                txtResources.Text += name;
                txtResources.Text += Environment.NewLine;
            }
            //string[] names = this.GetType().Assembly.GetManifestResourceNames();


            //foreach (string name in names)
            //{
            //    txtResources.Text += name;
            //    txtResources.Text += Environment.NewLine;
            //}




            //Stream fontStream = this.GetType().Assembly.GetManifestResourceStream("MyTest.fonts.Comicsmash.ttf");

            //if (fontStream != null)
            //{
            //    byte[] fontdata = new byte[fontStream.Length];

            //    fontStream.Read(fontdata, 0, (int)fontStream.Length);

            //    fontStream.Close();

            //    unsafe
            //    {

            //        fixed (byte* pFontData = fontdata)
            //        {

            //            pfc.AddMemoryFont((System.IntPtr)pFontData, fontdata.Length);

            //        }

            //    }
            //}
        }

        private void btnTrivia_Click(object sender, EventArgs e)
        {
            if (SCTVObjects.TriviaBar.TriviaForm != null)
                SCTVObjects.TriviaBar.CloseForm();
            else
            {
                Media media = new Media();
                media.Title = "test media";
                media.Trivia = "||Continuity: The first time the cab is hit, the front grille with the Hyundai logo is smashed in, and disappears. In the next scene, the grille reappears and the car has less damage than the scene before. In the warehouse scene, the grille disappears one final time.|||Continuity: When the white van emerges from the warehouse, the passenger side mirror strikes the warehouse door. In the next shot of van's exterior, the passenger side mirror is fully extended. In subsequent shots, the mirror is flipped back.|||Continuity: In the scene with the falling van, the sleeping Arthur is shown without the headphones originally placed on his head. Before the van hits the water, he's wearing the headphones again.||||||Continuity: One car explosion is shown twice. First, the car explodes in a fireball, filling the frame. Minutes later, the same car explodes in the same fireball, but as part of a bigger shot from further away, in the bottom half of the frame.|||Continuity: When Cobb and Ariadne are sitting at a caf�, her hand movements from shot to shot are out of sync, as she adjusts her hair or picks up her coffee cup.|||Continuity: When Eames is chasing down the Hummer with skiers trailing behind it, the number of skiers on the ropes alternates between five and six in different shots.|||Continuity: When Arthur collects Cobb in Tokyo, they leave the hotel room and go to the roof for the helicopter at night. When they are on the roof, it is day.|||Continuity: When Saito enters the helicopter and tells Cobb about the inception plan, hard rain is hitting the helicopter window. Subsequent shots show no rain.|||Continuity: When Arthur and Ariadne are looking at the hotel rooms in the second layer of dreams, Ariadne's bun keeps moving from the back of her head to the top of her head.|||Crew or equipment visible: When Fischer's cab is being hijacked, numerous production markings are visible on the street.|||Continuity: In the first scene, when the guard says he had only these, and places Cobb's gun and totem on the table, the totem is tilted with the longer end up and it is stable enough not to fall down. When Cobb is brought in, the totem is lying stable, with its long end down and short end up.|||Revealing mistakes: At the beginning and end of the film when Cobb meets Saito in limbo, the full-eye contacts to make Leonardo DiCaprio's and 'Ken Watanabe''s eyes look glossy and bloodshot are visible in the close-up shots.|||Continuity: When Ariadne and Cobb are sitting in a caf�, a man in a yellow jacket passes behind Ariadne. The camera then switches to Cobb. When it switches back to Ariadne, the man in the yellow jacket passes once again.|||Revealing mistakes: When the team busts into room 528 in the hotel, the door opens fractionally before Cobb's foot hits it in the POV shot.|||Continuity: In the warehouse scene between Cobb and Ariadne, a Nikon DSLR camera on a desk behind Cobb disappears and reappears between shots.|||Continuity: When Arthur is tying up the team in the hotel scene, Eames' arms are tightly bound to his sides. When Arthur is pushing them down the corridors, Eames' arms float freely. Then, in the shot looking down into the elevator, his arms are once again bound tightly.|||Incorrectly regarded as goofs: The immigration form Cobb is given in the plane towards the end of the movie is the white I-94 form given to those coming to the US on a visa. While it's true that US citizens need only fill out a smaller blue customs declaration form, Cobb clearly declines the form.|||Errors made by characters (possibly deliberate errors by the filmmakers): Dom slipped the sedative into Fischer's drink on the plane. The flight attendant, who was in on the plot, could have slipped it in before with much less chance of detection.|||Revealing mistakes: When Saito uses his teeth to pull the safety pin from a fragmentation grenade (extremely difficult to do in reality) the safety level (also known as the spoon) does not fly off and remains in place even after the grenade is thrown several feet into the opening of a vent.|||Incorrectly regarded as goofs: The characters dream during a 10-hour flight from Sydney to Los Angeles on a Boeing 747. In real life, a 747 flight from Sydney to LAX takes at least 13.5 hours. The characters need 10 hours to complete the inception, regardless of how long the flight takes.|||Incorrectly regarded as goofs: When the bus on the bridge hits the guardrail, it flies horizontally for some time, but the passengers immediately become weightless. That is correct; weightlessness is caused by acceleration, not by speed. Falling is a vertical acceleration, and gravity will immediately cause the bus to fall as soon as it is no longer supported by the bridge.|||Continuity: Shots of the van falling are interspersed with several stages of the dreams. The van backtracks in several shots.|||Crew or equipment visible: When Cobb confronts Mal at the hotel, in front of the open window with billowing curtains, a crew member's fingertips can be seen through the sheer fabric.|||Continuity: When Arthur puts the cab in reverse and rams a bodyguard/projection, the gun flies out of the victim's hand and falls a few feet away. In the next shot, the gun is next to the dead body.|||Incorrectly regarded as goofs: Saito is the CEO of a large competitor to Fischer's corporation, however Fischer does not appear to recognize or acknowledge Saito both in his dream or on the plane. However, it's not unreasonable that Fischer, who is depicted as not being that involved in his father's business, would not recognize Saito and if he did might not think anything of it other than that it was a coincidence that his competitor was also on the plane.|||Revealing mistakes: When Ariadne starts changing her first dream with Cobb, she moves two mirrors. Each time she steps over an invisible step, probably a part of a green screen.|||Continuity: When Cobb first meets Ariadne and begins her interview drawing puzzles, she is wearing a gray henley shirt with buttons underneath her jacket. For the rest of the interview, she is wearing a different gray pullover blouse without buttons.|||Continuity: In the rotating hallway room scene when Arthur is fighting a projection and they slide into a room. When everything finally comes to a stop and he shoots the projection he curls up to his left. But when he's getting up, we see him straighten up from his right.|||Continuity: When Yusuf drives the white van with frequent jerks, the same shot of the dreaming passengers moving inside and trying to gain balance with one hand while dreaming is shown twice.|||Revealing mistakes: During the city bending scene, you can clearly see reflections in the windows, showing the buildings in place, not bending.|||Factual errors: Towards the end of the movie, one of the characters throws a frag grenade which explodes in a fireball; frag grenades don't do that.|||Continuity: Once they put Fischer to sleep on the 747 we see the flight attendant close the curtain and get a small silver suitcase from a food compartment on the opposite side from the curtain. When she hands the suitcase to Arthur not only is the curtain open again but we can't see the food compartments - it's a completely different set.|||Continuity: In the first scene, Saito spins Cobb's totem in the clockwise direction, but, again when the same scene is shown at the end of the movie, the totem can clearly be seen to be spinning in the anticlockwise direction.|||Revealing mistakes: When Dom shoots Arthur to wake him from the first dream, Arthur has no bullet-hole in his forehead until he is shown lying on the ground.|||Continuity: When Cobb returns home at the end of the film, his children run toward him and he is seen picking up Phillipa. The scene cuts to Miles before cutting back to Cobb, now holding James instead.|||Continuity: When Mal shoots Arthur in his leg blood starts oozing out but in the next scene there is no blood in his leg.|||Crew or equipment visible: In the first level dream, at the LA downtown area, when the freight trains emerges and rams the vehicles on the road, a close-up on Cobb shows a crew member in the back seat.|||Revealing mistakes: Water not moving or rippling in Cobb's cityscape visible just after he spins Mal's top and closes the safe door. He and Mal are walking outside, and in the background cityscape the water is stationary.|||Errors in geography: The flight where the inception takes place is one from Sydney, Australia to Los Angeles, California. During the scene with the plane taking off, snow is visible on the ground. It hasn't snowed in Sydney since 1836.||||||&gt;&gt;&gt; WARNING: Here Be Spoilers |Goofs below here contain information that may give away important plot points. You may not want to read any further if you've not already seen this title.|||||Incorrectly regarded as goofs: SPOILER: When Cobb and Mal commit suicide in front of the train, they look young, although we have been told that they had already spent decades in limbo by that point. This is deliberate, an artifact of Cobb's happy recollections. A later scene in which he reflects on their life together revisits this shot, and the actors are aged to show Cobb's corrected memory of the events.||||";

                SCTVObjects.TriviaBar.ShowTriviaBar(media);

                //triviaBar = new TriviaBar();
                //triviaBar.TopMost = true;
                //triviaBar.Show();
            }
        }

        private void btnMakeThumbnail_Click(object sender, EventArgs e)
        {
            games = new ArrayList();
            getGames();


            if (games.Count > 0)
            {
                //flashPlayer = new SCTV.FlashPlayer();
                flashPlayer.TopMost = true;
                flashPlayer.documentLoaded += new SCTV.FlashPlayer.DocumentLoaded(flashPlayer_documentLoaded);
                flashPlayer.Show();

                displayGame(games[0].ToString());
            }
        }

        /// <summary>
        /// fill games arraylist
        /// </summary>
        private void getGames()
        {
            gameIndex = 0;

            foreach (string fileName in Directory.GetFiles(Application.StartupPath + "\\games"))
                games.Add(fileName);
        }

        private void displayGame(string gamePath)
        {
            //if (flashPlayer != null)
            //{
            //    //flashPlayer.Dispose();
            //    flashPlayer = null;

            //}
            //flashPlayer = new SCTV.FlashPlayer();
            //flashPlayer.TopMost = true;
            //flashPlayer.documentLoaded += new SCTV.FlashPlayer.DocumentLoaded(flashPlayer_documentLoaded);
            //flashPlayer.Show();

            currentGamePath = gamePath;
            
            flashPlayer.GameToPlay = gamePath;
        }

        private void takeScreenShot(object gameName, Rectangle gameBounds)
        {
            try
            {
                //Bitmap bmpScreenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, PixelFormat.Format32bppArgb);
                Bitmap bmpScreenshot = new Bitmap(gameBounds.Width, gameBounds.Height - 28, PixelFormat.Format32bppArgb);
                // Create a graphics object from the bitmap
                Graphics gfxScreenshot = Graphics.FromImage(bmpScreenshot);
                // Take the screenshot from the upper left corner to the right bottom corner
                Size gameSize = new Size(gameBounds.Width, gameBounds.Height - 28);
                //gfxScreenshot.CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, 0, 0, gameSize, CopyPixelOperation.SourceCopy);
                gfxScreenshot.CopyFromScreen(gameBounds.X, gameBounds.Y + 28, 0, 0, gameSize, CopyPixelOperation.SourceCopy);
                // Save the screenshot to the specified path that the user has chosen
                string savePath = Application.StartupPath + "\\images\\games\\thumbnails" + gameName.ToString();

                try
                {
                    bmpScreenshot.Save(savePath, ImageFormat.Jpeg);
                }
                catch (Exception ex)
                {
                    //displayGame(games[gameIndex].ToString());
                    //takeScreenShot(gameName, gameBounds);
                    //return;
                }
                

                //gfxScreenshot.Dispose();
                //gfxScreenshot = null;

                //bmpScreenshot.Dispose();
                //bmpScreenshot = null;

                gameIndex++;

                if (gameIndex < games.Count)
                {
                    string newGameName = games[gameIndex].ToString().Substring(games[gameIndex].ToString().LastIndexOf("\\"));

                    while (File.Exists(Application.StartupPath + "\\images\\games\\thumbnails" + newGameName.Replace("swf", "jpeg")))
                    {
                        gameIndex++;

                        newGameName = games[gameIndex].ToString().Substring(games[gameIndex].ToString().LastIndexOf("\\"));
                    }

                    displayGame(games[gameIndex].ToString());
                }
                else
                {
                    flashPlayer.Close();
                }
            }
            catch (Exception ex)
            {
                //Tools.WriteToFile(ex);
            }
        }

        void flashPlayer_documentLoaded()
        {
            //System.Threading.Thread.CurrentThread.Join();
            //take the screenshot
            //takeScreenShot(games[gameIndex].ToString().Substring(games[gameIndex].ToString().LastIndexOf("\\")).Replace(".swf", ".jpeg"), flashPlayer.Bounds);
            takeScreenShot(currentGamePath.Substring(currentGamePath.LastIndexOf("\\")).Replace(".swf", ".jpeg"), flashPlayer.Bounds);
            
        }
    }
}