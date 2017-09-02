using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;

public partial class Default : System.Web.UI.Page
{
    string previousMedia = "";
    int userID = 0;
    //string defaultMediaLocation = @"C:\webs\ElectroData\Video\";
    string defaultMediaLocation = "";
    int retries = 0;
    int retryThreshold = 3;
    string currentCamera = "";
    ArrayList cameras = new ArrayList();
    string usersFolder = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        defaultMediaLocation = Server.MapPath(".") +"\\";

        //get previous media played
        previousMedia = Request.QueryString["prevMedia"];

        if (int.TryParse(Request.QueryString["id"], out userID))
        {
            usersFolder = findUsersFolder(userID);
            //Response.Write("<br>USERSfolder " + usersFolder);

            if (usersFolder.Trim().Length > 0)//they have activated with sctv
            {
                //defaultMediaLocation = usersFolder;
                usersFolder = usersFolder.Replace(defaultMediaLocation +"\\", "");

                //Response.Write("<BR>defaultMediaLocation " + defaultMediaLocation);
                //Response.Write("<br>USERSfolder " + usersFolder);
                
                if (!IsPostBack)
                {
                    //populate ddCameras
                    populateCameras();

                    //display current camera's media links
                    displayLinks();
                }

                if (ddVideos.SelectedIndex > 0)//the user has selected a video
                {
                    if (ddVideos.SelectedValue.Contains(ddCameras.SelectedValue))
                        WVC1.FilePath = "http://" + ddVideos.SelectedValue;
                }
                else if (previousMedia != null && previousMedia.Trim().Length > 0)
                {
                    WVC1.FilePath = findNextFile(previousMedia);

                    //WVC1.FilePath = @"c:\movies\test_9142009742PM_1.avi";
                }
                else if (ddCameras.SelectedIndex == 0)
                {
                    FileInfo foundFile = findNewestFile(usersFolder, currentCamera);

                    if (foundFile != null)
                    {
                        //Response.Write("<BR>found file " + foundFile.Name);
                        //Response.Write("<BR>url http://" + Request.Url.Host + @"/video/" + foundFile.Name);

                        ////WVC1.FilePath = "http://" + Request.Url.Host + @"/webcam/video/videoCacheTest.wpl";

                        //select current camera
                        ddCameras.SelectedValue = currentCamera;

                        //update UI with newly selected camera
                        ddCameras_SelectedIndexChanged(null, null);

                        //select current video
                        ddVideos.SelectedValue = getFileURL(foundFile.Name);

                        //play video
                        //WVC1.FilePath = "http://" + Request.Url.Host +":"+ Request.Url.Port + @"/webvideosite/"+ usersFolder.Replace(defaultMediaLocation,"") +"/" + foundFile.Name;
                        WVC1.FilePath = "http://" + getFileURL(foundFile.Name);
                    }
                }

                lblCurrentMedia.Text = WVC1.FilePath;
            }
            else
                Response.Write("<BR>You need to register your SCTV to see your video");
        }
        else
            Response.Write("<BR>Error finding video!!<BR>Contact Administrator!");
    }

    /// <summary>
    /// Display the current camera's media links
    /// </summary>
    private void displayLinks()
    {
        //ddVideos.Items.Clear();

        ddVideos.Items.Add("Select a Video");

        //iterate files and parse cameras names to display links to the current cameras files
        foreach (FileInfo fi in new DirectoryInfo(usersFolder).GetFiles())
        {
            if (fi.Name.Contains(ddCameras.Text + "_"))
            {
                string mediaURL = getFileURL(fi.Name);

                ddVideos.Items.Add(new ListItem(fi.Name.Replace(fi.Extension, ""), mediaURL));
            }
        }
    }

    /// <summary>
    /// populate ddCameras
    /// </summary>
    private void populateCameras()
    {
        string currentCamera = "";

        //ddCameras.Items.Clear();

        ddCameras.Items.Add("Select a Camera");

        //iterate files and parse cameras names
        foreach (FileInfo fi in new DirectoryInfo(usersFolder).GetFiles())
        {
            if (fi.Name.Contains("_"))
            {
                currentCamera = fi.Name.Substring(0, fi.Name.IndexOf("_"));

                if (!cameras.Contains(currentCamera))
                {
                    cameras.Add(currentCamera);

                    ddCameras.Items.Add(currentCamera);
                }
            }
        }

        if (cameras.Count == 0)
            ddCameras.Items.Add("No Cameras Available");
    }

    private string findUsersFolder(int userID)
    {
        string usersFolder = "";

        //iterate directories and look for the <userID>.txt file
        foreach (string directory in Directory.GetDirectories(defaultMediaLocation))
        {
            if (File.Exists(directory +"\\"+ userID.ToString() + ".txt"))
            {
                usersFolder = directory;
                break;
            }
        }
        
        return usersFolder;
    }

    private string getFileURL(string filename)
    {
        string fileURL = Request.Url.Host + ":" + Request.Url.Port + "/" + Request.Url.PathAndQuery.Replace(Request.Url.Query, "").Replace("Default.aspx", "") + usersFolder.Replace(defaultMediaLocation, "") + "/" + filename;

        return fileURL;
    }

    /// <summary>
    /// Find the next file in the prevFile series
    /// </summary>
    /// <param name="prevFile">The file that just played</param>
    /// <returns>The next file</returns>
    private string findNextFile(string prevFile)
    {
        //file name format
        //[name]_[series(long date)]_[sequence#].avi

        string retFileName = "";
        string nextFileName = "";
        string nextFileURL = "";
        string[] prevFileInfo = prevFile.Split('_');

        //increment sequence# and look for existing file
        //nextFileName = defaultMediaLocation +"\\"+ prevFileInfo[0] + "_" + prevFileInfo[1] + "_" + (int.Parse(prevFileInfo[2]) + 1) + ".avi";
        //nextFileURL = "http://" + Request.Url.Host + "/" + usersFolder.Replace(defaultMediaLocation, "") + "/" + prevFileInfo[0] + "_" + prevFileInfo[1] + "_" + (int.Parse(prevFileInfo[2]) + 1) + ".avi";

        nextFileName = prevFileInfo[0] + "_" + prevFileInfo[1] + "_" + (int.Parse(prevFileInfo[2]) + 1) + ".avi";
        nextFileURL = "http://" + getFileURL(prevFileInfo[0] + "_" + prevFileInfo[1] + "_" + (int.Parse(prevFileInfo[2]) + 1) + ".avi");

        //make sure file exists
        if (File.Exists(usersFolder +"\\"+ nextFileName))
        {
            //need to make sure file is done being written to
            if (File.GetLastWriteTime(nextFileName) < DateTime.Now.AddMilliseconds(-2))
                retFileName = nextFileURL;
            else if(retries < retryThreshold)//wait and try again
            {
                retries++;

                timeout(100);

                System.Diagnostics.EventLog.WriteEntry("timeout called", "calling timeout");

                retFileName = findNextFile(prevFile);
            }

            retries = 0;
        }
        
        //if(retFileName.Length == 0)
        //    Response.Write("<BR> didn't find <BR>" + nextFileName);

        return retFileName;
    }

    /// <summary>
    /// Find the newest file in the given directory
    /// </summary>
    /// <param name="directory">directory to find newest file in</param>
    /// <returns>FileInfo of newest file in directory</returns>
    private FileInfo findNewestFile(string directory, string cameraToFind)
    {
        FileInfo retNewestFile = null;
        //Response.Write("<BR>finding newest file in " + directory);
        //iterate files and find the newest with a filesize > 0
        foreach (FileInfo fi in new DirectoryInfo(directory).GetFiles())
        {
            //Response.Write("<BR>found file " + fi.FullName);
            //Response.Write("<BR>fi.LastWriteTime " + fi.LastWriteTime);

            //if(retNewestFile != null)
            //    Response.Write("<BR>retNewestFile.LastWriteTime " + retNewestFile.LastWriteTime);

            //Response.Write("<BR>fi.Length " + fi.Length);

            //if ((retNewestFile == null || (fi.LastWriteTime > retNewestFile.LastWriteTime && fi.LastWriteTime < DateTime.Now.AddMilliseconds(-30))) && fi.Length > 0)
            //    retNewestFile = fi;

            if (fi.FullName.ToLower().Contains(cameraToFind.ToLower() +"_") && (retNewestFile == null || fi.LastWriteTime > retNewestFile.LastWriteTime) && fi.Length > 0)
                retNewestFile = fi;
        }

        if (retNewestFile != null && currentCamera.Length == 0 && retNewestFile.Name.Contains("_"))
            currentCamera = retNewestFile.Name.Substring(0, retNewestFile.Name.IndexOf("_"));

        //if (retNewestFile != null)
        //{
        //    Response.Write("<BR>retNewestFile1 " + retNewestFile.FullName);
        //    int previousFileNum;
        //    string[] retNewestFileInfo = retNewestFile.Name.Split('_');
        //    Response.Write("<BR>prev file num " + retNewestFileInfo[2].Replace(".avi", ""));

        //    if (int.TryParse(retNewestFileInfo[2].Replace(".avi", ""), out previousFileNum))
        //    {
        //        previousFileNum = previousFileNum - 1;
        //    }

        //    retNewestFile = new FileInfo(retNewestFile.FullName.Replace(retNewestFileInfo[2], previousFileNum.ToString()));
        //}

        //Response.Write("<BR>retNewestFile " + retNewestFile.FullName);
        return retNewestFile;
    }

    /// <summary>
    /// Find the newest files for each found series in the given directory
    /// </summary>
    /// <param name="directory"></param>
    /// <returns></returns>
    private FileInfo[] findNewestFiles(string directory)
    {
        FileInfo[] retNewestFiles = null;
        ArrayList newestFiles = new ArrayList();

        //iterate files and find the newest with a filesize > 0
        foreach (FileInfo fi in new DirectoryInfo(directory).GetFiles())
        {
            //if ((retNewestFiles == null || fi.LastWriteTime > retNewestFile.LastWriteTime) && fi.Length > 0)
            //    newestFiles.Add(fi);
        }

        if (newestFiles.Count > 0)
            newestFiles.CopyTo(retNewestFiles);

        return retNewestFiles;
    }

    protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        //WVC1.FilePath = DropDownList1.SelectedValue.ToString();
    }

    private void timeout(int timeoutLength)
    {
        for (int x = 0; x <= (timeoutLength*1000); x++)
        { }
    }

    /// <summary>
    /// Change cameras
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ddCameras_SelectedIndexChanged(object sender, EventArgs e)
    {
        ddVideos.Items.Clear();

        displayLinks();

        pnlVideos.Visible = true;
    }
}
