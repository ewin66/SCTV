using System;
using System.Data;
using System.Text;
using System.Xml;
using System.IO;
using Microsoft.Xml.XQuery;
using System.Collections;
using System.Diagnostics;

namespace SCTVTelevision
{
	/// <summary>
	/// Summary description for mediaHandler.
	/// </summary>
	public class mediaHandler
	{
		#region "Delegates"
		public delegate void mediaListChanged();
		public event mediaListChanged OnListChanged;
		#endregion

		#region variables
		public DataSet dsMedia = new DataSet();
		public DataSet dsMediaTypes = new DataSet();
		public string currentCategory="";
		XmlDocument xmlFileTypes = new XmlDocument();
		XmlDocument xmlLocations = new XmlDocument();
		ArrayList currentMediaExtensions = new ArrayList();
		#endregion

		#region public mediaHandler()
		public mediaHandler()
		{
			xmlFileTypes.Load("config/fileTypes.xml");
			xmlLocations.Load("config/locations.xml");
		}
		#endregion

		#region private DataSet getMedia() fills dsMedia with media records from /config/media.xml
		public DataSet getMedia()
		{
			try
			{
				dsMedia.Clear();
				dsMedia.ReadXml(new StringReader(xmlQuery("media.xml","getMedia.xqu").InnerXml.ToString()));

                if(dsMedia.Tables.Count > 0)
				    dsMedia.Tables[0].DefaultView.Sort = "title";//alphabetize

				//			return dsMedia;
				//			setDgMediaLibraryLayout(dsMedia);
				//			EventLog.WriteEntry("sctv","read xml file");
			}
			catch(Exception e)
			{
				EventLog.WriteEntry("sctv",e.ToString());
			}
			return dsMedia;
		}
		#endregion

		#region private DataSet changeMediaTypes(string mediaType)
		public DataView changeMediaTypes(string newMediaType)
		{
			while(dsMedia.Tables.Count < 1)//check for the existance of tables in dsMedia
			{
				getMedia();
			}

			while(dsMediaTypes.Tables.Count < 1)//check for the existance of tables in dsMediaTypes
			{
				getMediaTypes(newMediaType);
			}

			string strLIKE = null;
			int rowCount = 0;

			foreach(DataRow dr in dsMediaTypes.Tables[0].Rows) //get rows that belong to this media type
			{
				EventLog.WriteEntry("sctv","mediaType "+ dr[0].ToString());
				rowCount++;
				strLIKE += "[filePath] LIKE '%."+ dr[0].ToString() +"'";
				currentMediaExtensions.Add(dr[0].ToString());
				if(rowCount < dr.Table.Rows.Count)//this is not the last row add an "OR" to the end of the string
					strLIKE += " OR ";
			}
			dsMedia.Tables[0].DefaultView.RowFilter = strLIKE;
//			updateCategories(dsMedia.Tables[0].DefaultView);
//			return dsMedia;
			return dsMedia.Tables[0].DefaultView;
		}
		#endregion

		#region public DataView getCategory(string category)
		public DataView getCategory(string category)
		{
			currentCategory=category;
            if (dsMedia.Tables.Count < 1)//check for the existance of tables in dsMedia
                getMedia();
            else
            {
                DataTable dtEmptyTable = new DataTable();
                dsMedia.Tables.Add(dtEmptyTable);
            }

			if(category == "All")
				dsMedia.Tables[0].DefaultView.RowFilter = "";
			else
				dsMedia.Tables[0].DefaultView.RowFilter = "category = '"+ category +"'";
			return dsMedia.Tables[0].DefaultView;
		}
		#endregion

		#region public DataView getAllCategories(string mediaType)
		public DataView getAllCategories(string mediaType)
		{
			try
			{
				if(dsMedia.Tables.Count < 1)//check for the existance of tables in dsMedia
					getMedia();

                if (dsMedia.Tables.Count > 0)
                    dsMedia.Tables[0].DefaultView.RowFilter = "len(category) > 0";
                else
                {
                    //create empty dataTable
                    DataTable dtEmptyTable = new DataTable();
                    dsMedia.Tables.Add(dtEmptyTable);
                }
			}
			catch(Exception ex)
			{
				EventLog.WriteEntry("sctv",ex.ToString());
			}

            return dsMedia.Tables[0].DefaultView;
		}
		#endregion

		public DataView searchByFilePath(string filePath)
		{
			try
			{
				if(dsMedia.Tables.Count < 1)
				{
					getMedia();
				}
				dsMedia.Tables[0].DefaultView.RowFilter = "filePath = '"+ filePath +"'";
				return dsMedia.Tables[0].DefaultView;
			}
			catch(Exception ex)
			{
				EventLog.WriteEntry("sctv","searchByFilePath ERROR "+ ex.ToString());
				return null;
			}
		}

		public DataView searchByTitle(string title)
		{
			try
			{
				if(dsMedia.Tables.Count < 1)
				{
					getMedia();
				}
				dsMedia.Tables[0].DefaultView.RowFilter = "title LIKE '%"+ title +"'";
				return dsMedia.Tables[0].DefaultView;
			}
			catch(Exception ex)
			{
				EventLog.WriteEntry("sctv","searchByFilePath ERROR "+ ex.ToString());
				return null;
			}
		}

		#region private DataSet getMediaTypes(string mediaType)
		public void getMediaTypes(string mediaType)
		{
			dsMediaTypes.Clear();
			dsMediaTypes.ReadXml(new StringReader(xmlQuery("fileTypes.xml","(document(\"theFile\")//fileTypes//"+ mediaType +"//type)").InnerXml.ToString()));
		}
		#endregion

		/// <summary>
		/// will query an xml document given the document name and query or a query file name
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="query"></param>
		/// <returns></returns>
		private XmlDocument xmlQuery(string fileName,string query)
		{
			XmlDocument xmlResults = new XmlDocument();
			try 
			{ 
				fileName = "Config\\"+ fileName;
				// create the collection
				XQueryNavigatorCollection col = new XQueryNavigatorCollection();                        
				// add the file to the collection
				// the file will be referenced in the query statement by its alias
				col.AddNavigator( fileName,"theFile"); 
				//				if(fileName == "Config\\media.xml")
				//				{
				//					col.AddNavigator("Config\\fileTypes.xml", "fileTypes");
				//				}
				// copy the query out from the file
				String q = String.Empty;
				if ( query.IndexOf(".xqu") > 0)//this is a query file 
				{                
					StreamReader sr = new StreamReader("XQu\\"+ query);
					q = sr.ReadToEnd();
					sr.Close();
				}
				else
				{
					q = query;
				}
//				EventLog.WriteEntry("sctv",q.ToString());
				// compile the query
				XQueryExpression expr = new XQueryExpression(q);
				
				xmlResults.LoadXml("<results>"+ expr.Execute(col).ToXml() +"</results>");
				
				//				foreach(XmlNode node in xmlResults)
				//				{
				//					foreach(XmlElement elem in node)
				//					{
				//						lblMessage.Text += elem["Name"].InnerText.ToString();
				//					}
				//				}
			}
			catch ( Exception e ) 
			{
//				lblMessage.Text = e.ToString();
				EventLog.WriteEntry("sctv","File: "+ fileName +"     "+ e.ToString());
			}
//			EventLog.WriteEntry("sctv","Results: "+ expr.Execute(col).ToXml().ToString());
			return xmlResults;
		}

//		#region private void lookForMedia() finds new files in the folders specified in config/locations.xml and ads their information to config/media.xml
//		private void lookForMedia()
//		{
//			XmlDocument tempDoc = new XmlDocument();
//			DataSet ds = new DataSet();
//			tempDoc.Load("config/media.xml");
//			ds.ReadXml("config/media.xml");
//			DataColumn[] pk =  new DataColumn[1];
//			pk[0] = ds.Tables[0].Columns["filePath"];
//			ds.Tables[0].PrimaryKey = pk;
//			foreach(XmlNode location in xmlLocations["locations"])
//			{
//				if(location.InnerText.ToString().Length > 0)
//				{
//					string dLocation = location.InnerText.ToString().Trim();
//					try
//					{
//						DirectoryInfo fl = new DirectoryInfo(dLocation);
//					
//						foreach (FileSystemInfo entry in fl.GetFileSystemInfos())
//						{
//							if(ds.Tables[0].Rows.Find(dLocation +"\\"+ entry.Name.ToString().Trim()) == null)//this file name doesn't exist in the media file
//							{
//								XmlNode newNode = tempDoc.CreateNode(XmlNodeType.Element,"media","");
//
//								XmlNode nameNode = tempDoc.CreateNode(XmlNodeType.Element,"title","");
//								nameNode.InnerText = System.Text.RegularExpressions.Regex.Replace(entry.Name,entry.Extension,"");
//								XmlNode performersNode = tempDoc.CreateNode(XmlNodeType.Element,"performers","");
//								XmlNode ratingNode = tempDoc.CreateNode(XmlNodeType.Element,"rating","");
//								XmlNode descriptionNode = tempDoc.CreateNode(XmlNodeType.Element,"description","");
//								XmlNode starsNode = tempDoc.CreateNode(XmlNodeType.Element,"stars","");
//								XmlNode categoryNode = tempDoc.CreateNode(XmlNodeType.Element,"category","");
//								
//								EventLog.WriteEntry("sctv","calling findCategory "+ dLocation +" , "+ entry.Name);
//
//								categoryNode.InnerText = findCategory(dLocation,entry.Name);
//								XmlNode timesPlayedNode = tempDoc.CreateNode(XmlNodeType.Element,"timesPlayed","");
//								XmlNode filePathNode = tempDoc.CreateNode(XmlNodeType.Element,"filePath","");
//								filePathNode.InnerText = dLocation +"\\"+ entry.Name;
//						
//								newNode.AppendChild(nameNode);
//								newNode.AppendChild(performersNode);
//								newNode.AppendChild(ratingNode);
//								newNode.AppendChild(descriptionNode);
//								newNode.AppendChild(starsNode);
//								newNode.AppendChild(categoryNode);
//								newNode.AppendChild(timesPlayedNode);
//								newNode.AppendChild(filePathNode);
//								tempDoc["mediaFiles"].AppendChild(newNode);
//							}
//						}
//					}
//					catch(Exception e)
//					{
//						//						lblMessage.Text = e.ToString();
//					}
//				}
//				tempDoc.Save("config/media.xml");
//			}
//			//			dsMedia.Clear();
//			//			dgMediaLibrary.DataSource = changeMediaTypes("video").Tables[0];
//			//			changeMediaTypes(mediaType);
//			getMedia();
//		}
//		#endregion

//		private string findCategory(string location,string entryName)
//		{
//			string currentCategory = System.Text.RegularExpressions.Regex.Replace(location,"Z:\\Video\\","");
//			EventLog.WriteEntry("sctv","found category: "+ currentCategory);
//			if(currentCategory != entryName)//if the file is in a subfolder make the name of the subfolder the category name
//			{
//				currentCategory = System.Text.RegularExpressions.Regex.Replace(currentCategory,entryName,"");
//				return currentCategory;
//			}
//			else
//				return null;
//		}
	}
}
