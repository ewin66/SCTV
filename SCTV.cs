using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using SCTVObjects;
using SCTechUtilities;
using SCTVDeviceVolumeMonitor;

namespace SCTV
{
    public partial class SCTV : Form
    {
        public static MediaHandler myMedia = new MediaHandler();
        public static mediaLibrary myMediaLibrary;
        public static MediaLibrary_Listview mediaLibrary_Datalist;
        public static SCTVCamera.SCTVCameraMain sctvCamera;
        public static DeviceVolumeMonitor deviceMonitor;
        private string defaultPathToSaveTo = "";
        public static SpeechRecognition speechListener;
        Speakers speakers;

        public SCTV()
        {
            try
            {
                if (!SCTVActivation.isActivated())
                {
                    MessageBox.Show("This product needs to be activated.  Call Support");

                    this.Close();
                    return;
                }
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, ex.Source +" : "+ ex.Message);
            }

            try
            {
                InitializeComponent();

                defaultPathToSaveTo = System.Configuration.ConfigurationManager.AppSettings["Media.DefaultPathToSaveTo"];
                if (defaultPathToSaveTo.Trim().Length == 0)
                {
                    //make sure directory exists
                    if (!Directory.Exists(Application.StartupPath + "\\DVD\\"))
                        Directory.CreateDirectory(Application.StartupPath + "\\DVD\\");

                    defaultPathToSaveTo = Application.StartupPath + "\\DVD\\";
                }

                try
                {
                    //watch for inserted dvd/cd's
                    deviceMonitor = new DeviceVolumeMonitor(this.Handle);
                    deviceMonitor.OnVolumeInserted += new DeviceVolumeAction(VolumeInserted);
                    deviceMonitor.OnVolumeRemoved += new DeviceVolumeAction(VolumeRemoved);
                }
                catch (Exception ex)
                {
                    Tools.WriteToFile(Tools.errorFile, ex.Source + " : " + ex.Message);
                }                

                try
                {
                    //listen for voice commands
                    speechListener = new SpeechRecognition("xmlmain.xml");
                    speechListener.executeCommand += new SpeechRecognition.HeardCommand(speechListener_executeCommand);
                    speechListener.Show();
                }
                catch (Exception ex)
                {
                    Tools.WriteToFile(Tools.errorFile, ex.Source + " : " + ex.Message);
                }                

                //volume control
                speakers = new Speakers();

                //mouse hooks
                //private static LowLevelMouseProc _proc = HookCallback;
                //private static IntPtr _hookID = IntPtr.Zero;
                //private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);
                //private const int WH_MOUSE_LL = 14;

                //mouse hooks
                //private enum MouseMessages
                //{
                //    WM_LBUTTONDOWN = 0x0201,
                //    WM_LBUTTONUP = 0x0202,
                //    WM_MOUSEMOVE = 0x0200,
                //    WM_MOUSEWHEEL = 0x020A,
                //    WM_RBUTTONDOWN = 0x0204,
                //    WM_RBUTTONUP = 0x0205
                //}

                //display mediaLibrary
                if (mediaLibrary_Datalist == null)//create mediaLibrary
                {
                    //speechListener.loadGrammarFile("xmlmediaLibrary.xml");

                    mediaLibrary_Datalist = null;
                    mediaLibrary_Datalist = new MediaLibrary_Listview(myMedia);
                    mediaLibrary_Datalist.SpeechListener = speechListener;
                    //myMediaLibrary.KeyUp += new System.Windows.Forms.KeyEventHandler(KeyUpHandler);
                    mediaLibrary_Datalist.ShowDialog(this);
                }
                else
                    mediaLibrary_Datalist.ShowDialog(this);

                this.Close();
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, ex.Source + " : " + ex.Message +" || "+ ex.StackTrace);
                MessageBox.Show("An error has occurred" + Environment.NewLine + "Error: " + ex.Source +": " + ex.Message);
            }
        }

        #region DVD/CD handling
        private void VolumeInserted(int aMask)
        {
            // -------------------------
            // A volume was inserted
            // -------------------------
            //MessageBox.Show("Volume inserted in " + deviceMonitor.MaskToLogicalPaths(aMask));
            //lbEvents.Items.Add("Volume inserted in " + fNative.MaskToLogicalPaths(aMask));
            bool skipDVDMenu = false;
            string discName;
            string driveLetter = deviceMonitor.MaskToLogicalPaths(aMask);
            DriveInfo driveInfo;
            InsertedMedia insertedMedia = new InsertedMedia();

            driveInfo = new DriveInfo(driveLetter);
            
            bool.TryParse(System.Configuration.ConfigurationManager.AppSettings["Media.SkipDVDMenu"], out skipDVDMenu);
            insertedMedia.SkipMenu = skipDVDMenu;
            insertedMedia.Drive = driveInfo;
            insertedMedia.ShowDialog(this);

            discName = insertedMedia.MediaName;

            switch (insertedMedia.MediaState)
            {
                case MediaStateEnum.Play:
                    playRemoveableMedia(driveLetter, insertedMedia.SkipMenu);
                    break;
                case MediaStateEnum.PlayAndRecord:
                    playRemoveableMedia(driveLetter, defaultPathToSaveTo + "\\" + discName + ".CEL", insertedMedia.SkipMenu);
                    break;
                case MediaStateEnum.Record:
                    recordRemoveableMedia(driveLetter, defaultPathToSaveTo + "\\" + discName + ".CEL", insertedMedia.SkipMenu);
                    break;
            }
        }

        private void VolumeRemoved(int aMask)
        {
            // --------------------
            // A volume was removed
            // --------------------
            //MessageBox.Show("Volume removed from " + deviceMonitor.MaskToLogicalPaths(aMask));
            //lbEvents.Items.Add("Volume removed from " + fNative.MaskToLogicalPaths(aMask));
        }

        #endregion

        /// <summary>
        /// handles speech commands
        /// </summary>
        /// <param name="heardPhrase"></param>
        private void speechListener_executeCommand(Phrase heardPhrase)
        {
            //Tools.WriteToFile(Tools.errorFile, "speechListener.speechCommand form1 " + heardPhrase.phrase);

            switch (heardPhrase.phrase.ToLower())
            {
                case "video library":
                    executeMacros("videoKey");
                    break;
                case "cams":
                    executeMacros("videoKey");
                    break;
                case "mute":
                case "volume up":
                case "volume down":
                    executeMacros(heardPhrase.phrase.ToLower());
                    break;
                case "admin":
                    //executeMacros("videoKey");
                    break;
                case "who is the fairest of them all":
                    //executeMacros("videoKey");
                    break;
                case "who is the master":
                    break;
                case "show camera one":
                case "show camera two":
                case "show all cameras":
                case "close camera one":
                case "close camera two":
                case "close all cameras":
                    executeMacros(heardPhrase.phrase.ToLower());
                    break;
            }

            //switch (GUIState)
            //{
            //    case guiState.mediaLibrary:
            //        if (speechListener.speechCommand.IndexOf("category-") >= 0)//they are asking for a category
            //        {
            //            string category = System.Text.RegularExpressions.Regex.Replace(speechListener.speechCommand, "category-", "");
            //            myMediaLibrary.shuffleCategories(myMediaLibrary.categoryButtons.IndexOf(category), "left");

            //            myMediaLibrary.displayCategory(category);
            //        }
            //        else if (speechListener.speechCommand.IndexOf("titleSearch()") >= 0)//they are asking to search titles
            //        {
            //            myMediaLibrary.lblSearch.Visible = true;
            //            myMediaLibrary.lblSearch.Text = "Searching for title: ";
            //            myMediaLibrary.lblSearch.BringToFront();
            //            //				myMedia.dsMedia.Tables[0].DefaultView.RowFilter="title LIKE '"+ System.Text.RegularExpressions.Regex.Replace(speechListener.speechCommand,"titleSearch-","") +"'";
            //            //				myMediaLibrary.displayCategory(myMedia.dsMedia.Tables[0].DefaultView);
            //        }
            //        else if (speechListener.speechCommand.IndexOf("title-") >= 0)//they are searching titles
            //        {
            //            myMediaLibrary.lblSearch.Text = "Found: " + System.Text.RegularExpressions.Regex.Replace(speechListener.speechCommand, "title-", "");
            //        }
            //        else if (speechListener.speechCommand.IndexOf("title-") >= 0)
            //        {
            //            Tools.WriteToFile(Tools.errorFile,"found title to search for");
            //        }
            //        else
            //            executeMacros(speechListener.speechCommand);
            //        break;
            //    case guiState.defaultState:
            //        executeMacros("videoKey");
            //        break;
            //}			
        }

        /// <summary>
        /// Executes macro immediately
        /// </summary>
        /// <param name="macroName"></param>
        public void executeMacros(string macroName)
        {
            bool foundMacro = true;
            //if (changeChannelTimer.Enabled)
            //    changeChannelTimer.Enabled = false;

            //keyStrokeTracker.Clear();
            switch (macroName)
            {
                case "mute":
                case "volume down":
                case "volume up":
                    speakers.Volume(macroName);
                    break;
                //case "channel":
                //    createTVViewer();
                //    break;
                //case "tvKey":
                //    switch (GUIState)
                //    {
                //        case guiState.dvd:
                //            break;
                //        case guiState.music:
                //            break;
                //        case guiState.pictures:
                //            break;
                //        case guiState.radio:
                //            break;
                //        case guiState.TV://switch to the tvList view
                //            if (myTVViewer.Visible)
                //            {
                //                tabControl.SelectedTab = tvListingsTab;
                //                tabControl1.SelectedTab = TVListView;
                //                listView.Select();
                //            }
                //            else
                //            {
                //                createTVViewer();
                //            }
                //            break;
                //        //						case guiState.video://pause any playing videos and show the TV
                //        //							mySplash.Show();
                //        //							myTVViewer.changeChannel(4);
                //        //							break;
                //        default://turn on TV
                //            Tools.WriteToFile(Tools.errorFile, "-----  calling createTVViewer");
                //            createTVViewer();
                //            break;
                //    }
                //    break;
                case "fmKey":
                    MessageBox.Show("Coming Soon!!");
                    break;
                case "musicKey":
                    MessageBox.Show("Coming Soon!!");
                    break;
                case "pictureKey":
                    MessageBox.Show("Coming Soon!!");
                    break;
                case "videoKey":
                    //switch (GUIState)
                    //{
                        //						case guiState.TV:
                        //							if(myTVViewer!=null)
                        //								myTVViewer.Close();
                        //							break;
                        //						case guiState.mediaLibrary:
                        //							// TODO: check if they have a paused/stopped file and start playing it
                        //							break;
                        //default:
                            //							Tools.WriteToFile(Tools.errorFile,"       showing myMediaLibrary       ------------");
                            myMediaLibrary = null;

                            if (myMediaLibrary == null)//create mediaLibrary
                            {


                                //speechListener.executeCommand -= new SCTV.speechRecognition.heardCommand(speechListener_executeCommand);
                                //Tools.WriteToFile(Tools.errorFile,"       creating myMediaLibrary       ------------");
                                //speechListener.loadGrammarFile("xmlmediaLibrary.xml");
                                //speechListener.addRulesToCurrentGrammar("xmlmediaLibrary.xml");
                                //speechListener.addRulesToCurrentGrammar("xmlMedia.xml");
                                myMediaLibrary = null;
                                myMediaLibrary = new mediaLibrary();
                                //myMediaLibrary.KeyUp += new System.Windows.Forms.KeyEventHandler(KeyUpHandler);
                                myMediaLibrary.ShowDialog();
                                //myMediaLibrary.BringToFront();
                                //myMediaLibrary.Select();
                                //myMediaLibrary.Refresh();
                                //myMediaLibrary.Focus();
                                //myMediaLibrary.Visible=true;
                                //Tools.WriteToFile(Tools.errorFile,"       created myMediaLibrary       ------------");
                            }
                            else
                                myMediaLibrary.ShowDialog();

                            break;
                    //}

                    break;
                case "dvdKey":
                    MessageBox.Show("Coming Soon!!");
                    break;
                case "cameras":

                    
                    
                    break;
                case "show camera one":
                    try
                    {
                        if(sctvCamera == null)
                            sctvCamera = new SCTVCamera.SCTVCameraMain();

                        //SCTVCamera.SCTVCameraMain sctvCamera1 = new SCTVCamera.SCTVCameraMain();

                        //sctvCamera.ShowCameraByName("camera one");
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                    break;
                case "show camera two":
                    if (sctvCamera == null)
                        sctvCamera = new SCTVCamera.SCTVCameraMain();

                    //SCTVCamera.SCTVCameraMain sctvCamera2 = new SCTVCamera.SCTVCameraMain();

                    //sctvCamera.ShowCameraByName("camera two");
                    break;
                case "show all cameras":
                    break;
                case "close camera one":
                case "close camera two":
                    foreach (Form form in Application.OpenForms)
                    {
                        if (form.Text.ToLower() == macroName.ToLower().Replace("close ", ""))//stop this camera
                        {
                            form.Dispose();
                            form.Close();

                            break;
                        }
                    }
                    break;
                case "close all cameras":
                    break;
                default:
                    foundMacro = false;
                    break;

            }
            //if (foundMacro)
            //    keyStrokeTracker.Clear();
        }

        private void playRemoveableMedia(string driveLetter, bool skipMenu)
        {
            playRemoveableMedia(driveLetter, "", skipMenu);
        }

        private void playRemoveableMedia(string driveLetter, string fileToRecordTo, bool skipMenu)
        {
            //make sure mediaplayer exists
            FormCollection openForms = Application.OpenForms;

            if (openForms["liquidMediaPlayer"] == null)
            {
                liquidMediaPlayer mediaPlayer = new liquidMediaPlayer();
                mediaPlayer.PlayRemoveableMedia(driveLetter, fileToRecordTo, skipMenu);

                mediaPlayer.ShowDialog();
            }
            else
                openForms["liquidMediaPlayer"].ShowDialog();


        }

        private void recordRemoveableMedia(string driveLetter, string fileToRecordTo, bool skipMenu)
        {
            //make sure mediaplayer exists
            FormCollection openForms = Application.OpenForms;

            if (openForms["liquidMediaPlayer"] == null)
            {
                liquidMediaPlayer mediaPlayer = new liquidMediaPlayer();
                mediaPlayer.RecordRemoveableMedia(driveLetter, fileToRecordTo, skipMenu);

                mediaPlayer.ShowDialog();
            }
            else
                openForms["liquidMediaPlayer"].ShowDialog();
        }
    }
}