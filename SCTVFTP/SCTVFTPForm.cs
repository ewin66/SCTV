using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Threading;
using System.Collections;

namespace SCTVFTP
{
    public partial class SCTVFTPForm : Form
    {
        string ftpServerIP = "";
        string ftpUserID = "";
        string ftpPassword = "";
        StreamWriter log = null;
        BackgroundWorker bw;
        static ArrayList uploads = new ArrayList();
        static bool uploading = false;
        static int numUploads = 0;
        bool deleteFileAfterFTP = false;
        
        public string FTPServerIP
        {
            get
            {
                return ftpServerIP;
            }

            set
            {
                ftpServerIP = value;
            }
        }

        public string FTPUserID
        {
            get
            {
                return ftpUserID;
            }

            set
            {
                ftpUserID = value;
            }
        }

        public string FTPPassword
        {
            get
            {
                return ftpPassword;
            }

            set
            {
                ftpPassword = value;
            }
        }

        public bool DeleteFileAfterFTP
        {
            set { deleteFileAfterFTP = value; }
        }

        public SCTVFTPForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //ftpServerIP = "192.168.1.21";
            //ftpUserID = "administrator";
            //ftpPassword = "xxxxx";
            txtServerIP.Text = ftpServerIP;
            txtUsername.Text = ftpUserID;
            txtPassword.Text = ftpPassword;
            this.Text += ftpServerIP;

            btnFTPSave.Enabled = false;
        }

        public static void UploadFile(FTPObj ftpObj)
        {
            FTPUpload(ftpObj);
        }
        
        public static void FTPUpload(object objFTP)
        {
            numUploads++;

            FTPObj ftpObj = (FTPObj)objFTP;

            DateTime startTime = DateTime.Now;
            FileInfo fileInf = new FileInfo(ftpObj.Filename);
            string uri = "ftp://" + ftpObj.FtpIP + fileInf.Name;
            FtpWebRequest reqFTP;

            //try
            //{
            //    log = File.CreateText(Application.StartupPath + "\\ftpLog2.log");
            //    log.WriteLine("ftp request: " + uri);
            //    log.WriteLine("filename " + ftpObj.Filename);
            //    log.WriteLine("ftpIP " + ftpObj.FtpIP);
            //    log.WriteLine("username " + ftpObj.Username);
            //    log.WriteLine("password " + ftpObj.Password);
            //    log.Flush();
            //}
            //catch (Exception ex)
            //{

            //}

            //lblElapsedTime.Text = "Elapsed Time ";

            // Create FtpWebRequest object from the Uri provided
            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));

            

            // Provide the WebPermission Credentials
            reqFTP.Credentials = new NetworkCredential(ftpObj.Username, ftpObj.Password);

            // By default KeepAlive is true, where the control connection is not closed
            // after a command is executed.
            reqFTP.KeepAlive = false;

            // Specify the command to be executed.
            reqFTP.Method = WebRequestMethods.Ftp.UploadFile;

            reqFTP.UsePassive = true;

            // Specify the data transfer type.
            reqFTP.UseBinary = true;

            // Notify the server about the size of the uploaded file
            reqFTP.ContentLength = fileInf.Length;

            // The buffer size is set to 2kb
            int buffLength = 2048;
            byte[] buff = new byte[buffLength];
            int contentLen;

            // Opens a file stream (System.IO.FileStream) to read the file to be uploaded
            FileStream fs = fileInf.OpenRead();

            try
            {
                // Stream to which the file to be upload is written
                Stream strm = reqFTP.GetRequestStream();

                // Read from the file stream 2kb at a time
                contentLen = fs.Read(buff, 0, buffLength);

                // Till Stream content ends
                while (contentLen != 0)
                {
                    //log.WriteLine("looping through content");
                    // Write Content from the file stream to the FTP Upload Stream
                    strm.Write(buff, 0, contentLen);
                    contentLen = fs.Read(buff, 0, buffLength);
                }
                
                // Close the file stream and the Request Stream
                reqFTP = null;
                strm.Close();
                fs.Close();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Upload Error");
            }

            //lblElapsedTime.Text += (startTime - DateTime.Now);

            //log.Close();
        }

        public void Upload(string filename)
        {
            Upload(new FTPObj(filename, ftpServerIP, ftpUserID, ftpPassword));
        }

        /// <summary>
        /// Method to upload the specified file to the specified FTP Server
        /// </summary>
        /// <param name="filename">file full name to be uploaded</param>
        public void Upload(FTPObj ftpObj)
        {
            uploads.Add(ftpObj);

            if (!uploading)
            {
                uploading = true;

                if (bw == null)
                {
                    bw = new BackgroundWorker();
                    bw.WorkerReportsProgress = true;
                    bw.WorkerSupportsCancellation = true;

                    bw.DoWork += bw_DoWork;
                    bw.ProgressChanged += bw_ProgressChanged;
                    bw.RunWorkerCompleted += bw_RunWorkerCompleted;
                }

                bw.RunWorkerAsync(uploads[0]);
            }
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            //for (int i = 0; i <= 100; i += 20)
            //{

            int count = 0;
            string filename = "";

            //while(uploads.Count > 0)
            //{
                count++;
                filename = ((FTPObj)e.Argument).Filename;
                //filename = ((FTPObj)uploads[0]).Filename;

                if (bw.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                bw.ReportProgress(count);

                FTPUpload((FTPObj)e.Argument);
                //FTPUpload((FTPObj)uploads[0]);

                bw.ReportProgress(count);

                //uploads.RemoveAt(0);

                //Thread.Sleep(1000);
            //}
            //}
            //e.Result = "completed "+ filename;    // This gets passed to RunWorkerCompleted
                e.Result = e.Argument;
        }

        private void bw_RunWorkerCompleted(object sender,RunWorkerCompletedEventArgs e)
        {
            uploading = false;

            if (e.Cancelled)
                MessageBox.Show("You cancelled!");
            else if (e.Error != null)
                MessageBox.Show("Worker exception: " + e.Error.ToString());
            //else
            //    MessageBox.Show("Complete - " + (string)e.Result);      // from DoWork

            uploads.RemoveAt(0);

            if (uploads.Count > 0)
                Upload((FTPObj)uploads[0]);
        }

        private void bw_ProgressChanged(object sender,ProgressChangedEventArgs e)
        {
            Console.WriteLine("Reached " + e.ProgressPercentage + "%");
        }

        public void DeleteFTP(string fileName)
        {
            try
            {
                string uri = "ftp://" + ftpServerIP + "/" + fileName;
                FtpWebRequest reqFTP;
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerIP + fileName));

                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.KeepAlive = false;
                reqFTP.Method = WebRequestMethods.Ftp.DeleteFile;

                string result = String.Empty;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                long size = response.ContentLength;
                Stream datastream = response.GetResponseStream();
                StreamReader sr = new StreamReader(datastream);
                result = sr.ReadToEnd();
                sr.Close();
                datastream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "FTP 2.0 Delete");
            }
        }

        private string[] GetFilesDetailList()
        {
            string[] downloadFiles;
            try
            {
                StringBuilder result = new StringBuilder();
                FtpWebRequest ftp;
                ftp = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerIP + "/"));
                ftp.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                ftp.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                WebResponse response = ftp.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string line = reader.ReadLine();
                while (line != null)
                {
                    result.Append(line);
                    result.Append("\n");
                    line = reader.ReadLine();
                }
                
                result.Remove(result.ToString().LastIndexOf("\n"), 1);
                reader.Close();
                response.Close();
                return result.ToString().Split('\n');
                //MessageBox.Show(result.ToString().Split('\n'));
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
                downloadFiles = null;
                return downloadFiles;
            }
        }

        public string[] GetFileList()
        {
            string[] downloadFiles;
            StringBuilder result = new StringBuilder();
            FtpWebRequest reqFTP;
            try
            {
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerIP + "/"));
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                WebResponse response = reqFTP.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                //MessageBox.Show(reader.ReadToEnd());
                string line = reader.ReadLine();
                while (line != null)
                {
                    result.Append(line);
                    result.Append("\n");
                    line = reader.ReadLine();
                }
                result.Remove(result.ToString().LastIndexOf('\n'), 1);
                reader.Close();
                response.Close();
                //MessageBox.Show(response.StatusDescription);
                return result.ToString().Split('\n');
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
                downloadFiles = null;
                return downloadFiles;
            }
        }
        private void Download(string filePath, string fileName)
        {
            FtpWebRequest reqFTP;
            try
            {
                //filePath = <<The full path where the file is to be created.>>, 
                //fileName = <<Name of the file to be created(Need not be the name of the file on FTP server).>>
                FileStream outputStream = new FileStream(filePath + "\\" + fileName, FileMode.Create);

                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerIP + "/" + fileName));
                reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();
                long cl = response.ContentLength;
                int bufferSize = 2048;
                int readCount;
                byte[] buffer = new byte[bufferSize];

                readCount = ftpStream.Read(buffer, 0, bufferSize);
                while (readCount > 0)
                {
                    outputStream.Write(buffer, 0, readCount);
                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                }

                ftpStream.Close();
                outputStream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void btnUpload_Click(object sender, EventArgs e)
        {
            OpenFileDialog opFilDlg = new OpenFileDialog();
            if (opFilDlg.ShowDialog() == DialogResult.OK)
            {
                Upload(new FTPObj(opFilDlg.FileName, ftpServerIP, ftpUserID, ftpPassword));
            }
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fldDlg = new FolderBrowserDialog();
            if (txtUpload.Text.Trim().Length > 0)
            {
                if (fldDlg.ShowDialog() == DialogResult.OK)
                {
                    Download(fldDlg.SelectedPath, txtUpload.Text.Trim());
                }
            }
            else
            {
                MessageBox.Show("Please enter the File name to download");
            }
        }

        private void btnLstFiles_Click(object sender, EventArgs e)
        {
            string[] filenames = GetFileList();
            lstFiles.Items.Clear();

            if (filenames != null)
            {
                foreach (string filename in filenames)
                {
                    lstFiles.Items.Add(filename);
                }
            }
        }

        private void btndelete_Click(object sender, EventArgs e)
        {
            OpenFileDialog fldDlg = new OpenFileDialog();
            if (txtUpload.Text.Trim().Length > 0)
            {
                DeleteFTP(txtUpload.Text.Trim());
            }
            else
            {
                MessageBox.Show("Please enter the File name to delete");
            }
        }

        private long GetFileSize(string filename)
        {
            FtpWebRequest reqFTP;
            long fileSize = 0;
            try
            {
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerIP + "/" + filename));
                reqFTP.Method = WebRequestMethods.Ftp.GetFileSize;
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();
                fileSize = response.ContentLength;
                
                ftpStream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return fileSize;
        }

        private void Rename(string currentFilename, string newFilename)
        {
            FtpWebRequest reqFTP;
            try
            {
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerIP + "/" + currentFilename));
                reqFTP.Method = WebRequestMethods.Ftp.Rename;
                reqFTP.RenameTo = newFilename;
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();
                
                ftpStream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MakeDir(string dirName)
        {
            FtpWebRequest reqFTP;
            try
            {
                // dirName = name of the directory to create.
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerIP + "/" + dirName));
                reqFTP.Method = WebRequestMethods.Ftp.MakeDirectory;
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();

                ftpStream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void btnFileSize_Click(object sender, EventArgs e)
        {
            long size = GetFileSize(txtUpload.Text.Trim());
            MessageBox.Show(size.ToString()+" bytes");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Rename(txtCurrentFilename.Text.Trim(), txtNewFilename.Text.Trim());
        }

        private void btnewDir_Click(object sender, EventArgs e)
        {
            MakeDir(txtNewDir.Text.Trim());
        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
            btnFTPSave.Enabled = true;
        }

        private void txtServerIP_TextChanged(object sender, EventArgs e)
        {
            btnFTPSave.Enabled = true;
        }

        private void txtUsername_TextChanged(object sender, EventArgs e)
        {
            btnFTPSave.Enabled = true;
        }

        private void btnFTPSave_Click(object sender, EventArgs e)
        {
            ftpServerIP = txtServerIP.Text.Trim();
            ftpUserID = txtUsername.Text.Trim();
            ftpPassword = txtPassword.Text.Trim();
            btnFTPSave.Enabled = false;
        }

        private void btnFileDetailList_Click(object sender, EventArgs e)
        {
            string[] filenames = GetFilesDetailList();
            lstFiles.Items.Clear();
            foreach (string filename in filenames)
            {
                lstFiles.Items.Add(filename);
            }
        }

        private void lstFiles_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }

    public class FTPObj
    {
        string filename = "";
        string ftpIP = "";
        string username = "";
        string password = "";

        public string Filename
        {
            get { return filename; }
            set { filename = value; }
        }

        public string FtpIP
        {
            get { return ftpIP; }
            set { ftpIP = value; }
        }

        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public FTPObj(string name, string ip, string user, string pass)
        {
            filename = name;
            ftpIP = ip;
            username = user;
            password = pass;
        }
    }
}