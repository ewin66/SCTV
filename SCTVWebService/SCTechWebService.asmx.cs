using System;
using System.Data;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.IO;

namespace SCTechWebService
{
    /// <summary>
    /// Summary description for SCTVWebService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    public class SCTVWebService : System.Web.Services.WebService
    {
        string baseFTPPath = @"C:\webs\ElectroData\Video\";
        
        [WebMethod]
        public string Activate(int userID, string appName, string cpuID, string firstName, string lastName, string address, string city, string state, string country, string username, string password)
        {
            string activationKey = "";
            ActivationInfo activationInfo = new ActivationInfo();

            try
            {
                EventLog.WriteEntry("SCTVWebService", "activate for "+ username +", "+ cpuID);
                if (username.Trim().Length > 0 && password.Trim().Length > 0)
                {
                    //get activation key
                    activationKey = SCTechUtilities.Encryption.Encrypt(cpuID);

                    //fill object
                    activationInfo.FirstName = firstName;
                    activationInfo.LastName = lastName;
                    activationInfo.Address = address;
                    activationInfo.City = city;
                    activationInfo.State = state;
                    activationInfo.Country = country;
                    activationInfo.ActivationKey = activationKey;
                    activationInfo.IP = HttpContext.Current.Request.UserHostAddress;
                    activationInfo.UserName = username;
                    activationInfo.Password = password;
                    activationInfo.AppName = appName;

                    //create ftp folder from activationKey
                    string directoryToCreate = activationKey;
                    directoryToCreate = directoryToCreate.Replace("/", "");
                    directoryToCreate = directoryToCreate.Replace("!", "");
                    directoryToCreate = directoryToCreate.Replace("\\", "");

                    Directory.CreateDirectory(baseFTPPath + directoryToCreate);

                    //create txt file that is the name of the userID
                    File.Create(baseFTPPath + directoryToCreate + "\\" + userID + ".txt");

                    //save activation info data
                    saveActivationInfo(activationInfo);
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("SCTVWebService", ex.StackTrace);
                return "Error "+ ex.StackTrace;
            }
            
            return activationKey;
        }

        private void saveActivationInfo(ActivationInfo activationInfo)
        {
            //save username, password and activationKey


        }

        [WebMethod]
        public Media IMDBInfoByTitle(string title)
        {
            Media foundMedia;

            if (title.Trim().Length > 0)
            {
                SCTVObjects.IMDBScraper imdb = new SCTVObjects.IMDBScraper();

                string textToSearch = title.Trim().Replace("_", " ");

                foundMedia = imdb.getInfoByTitle(textToSearch + " " + textToSearch + " 2", false);

                if (foundMedia != null)
                {
                    if (mediaToSearchFor != null && mediaToSearchFor.filePath != null)
                    {
                        //update foundMedia
                        foundMedia.filename = mediaToSearchFor.filename;
                        foundMedia.filePath = mediaToSearchFor.filePath;
                    }
                }
                //else
                //    txtTitleResult.Text = "No Results";
            }
            else
            {
                MessageBox.Show("You must enter something to search for");
            }
        }
    }

    public class ActivationInfo
    {
        private string firstName;
        private string lastName;
        private string address;
        private string city;
        private string state;
        private string country;
        private string ip;
        private string activationKey;
        private string userName;
        private string password;
        private string appName;

        public string FirstName
        {
            get { return firstName; }
            set { firstName = value; }
        }

        public string LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }

        public string Address
        {
            get { return address; }
            set { address = value; }
        }

        public string City
        {
            get { return city; }
            set { city = value; }
        }

        public string State
        {
            get { return state; }
            set { state = value; }
        }

        public string Country
        {
            get { return country; }
            set { country = value; }
        }

        public string IP
        {
            get { return ip; }
            set { ip = value; }
        }

        public string ActivationKey
        {
            get { return activationKey; }
            set { activationKey = value; }
        }

        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public string AppName
        {
            get { return appName; }
            set { appName = value; }
        }
    }
}
