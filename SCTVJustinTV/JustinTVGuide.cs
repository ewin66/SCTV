using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Net;
using SCTVObjects;

namespace SCTVJustinTV
{
    public partial class JustinTVGuide : Form
    {
        XmlDocument xmlTVGuide = null;
        DataSet dsStreams = new DataSet();
        string selectedUrl = "";

        public string SelectedUrl
        {
            get { return selectedUrl; }
        }

        public JustinTVGuide()
        {
            InitializeComponent();
        }

        public DataSet GetTVGuide(string mediaType, string category)
        {
            XmlNodeReader xmlReader = new XmlNodeReader(downloadTVGuideFile(mediaType, category));

            DataSet dsGuide = new DataSet();
            dsGuide.ReadXml(xmlReader);

            return dsGuide;
        }

        public DataSet GetTVGuide(string category)
        {
            XmlNodeReader xmlReader = new XmlNodeReader(downloadTVGuideFile(category));

            DataSet dsGuide = new DataSet();
            dsGuide.ReadXml(xmlReader);

            return dsGuide;
        }

        private void displayTVGuide()
        {
            XmlNodeReader xmlReader = new XmlNodeReader(downloadTVGuideFile("entertainment"));

            dsStreams.ReadXml(xmlReader);

            DataColumn index = new DataColumn("index", System.Type.GetType("System.String"));
            dsStreams.Tables["stream"].Columns.Add(index);

            int indexCount = 0;

            foreach (DataRow dr in dsStreams.Tables["stream"].Rows)
            {
                dr["index"] = indexCount.ToString();
                indexCount++;
            }

            dgStreams.AutoGenerateColumns = false;
            dgStreams.DataMember = "stream";
            dgStreams.DataSource = dsStreams;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            displayTVGuide();
        }

        private XmlDocument downloadTVGuideFile(string category)
        {
            return downloadTVGuideFile("", category);
        }

        /// <summary>
        /// download tvGuide
        /// </summary>
        /// <returns></returns>
        private XmlDocument downloadTVGuideFile(string mediaType, string category)
        {
            string apiURL = "";

            if(mediaType.Trim().Length > 0)
                apiURL = "http://api.justin.tv/api/stream/list.xml?category=" + category + "&language=en&subcategory=" + mediaType;
            else
                apiURL = "http://api.justin.tv/api/stream/list.xml?category="+ category +"&language=en";

            xmlTVGuide = new XmlDocument();
            xmlTVGuide.LoadXml(new WebClient().DownloadString(apiURL));

            //xmlTVGuide.Save(archivePath + channelName + ".xml");

            return xmlTVGuide;
        }

        private void parseTVGuideFile()
        {
            foreach (XmlNode stream in xmlTVGuide["streams"].ChildNodes)
            {
                TVGuide tvGuide = new TVGuide();

                //tvGuide.Name = stream.SelectSingleNode("name").InnerText;
                //tvGuide.Category = stream.SelectSingleNode("category").InnerText;
            }
        }

        /// <summary>
        /// get url of selected stream
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgStreams_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                foreach (DataGridViewRow selectedRow in dgStreams.SelectedRows)
                {
                    selectedUrl = dsStreams.Tables["channel"].Rows[int.Parse(selectedRow.Cells["index"].Value.ToString())]["channel_url"].ToString();
                    this.DialogResult = DialogResult.OK;
                    //this.Close();
                    break;
                }
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }
        }

        
    }
}