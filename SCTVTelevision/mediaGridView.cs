using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Xml;
using System.Diagnostics;

namespace SCTVTelevision
{
	/// <summary>
	/// Summary description for mediaGridView.
	/// </summary>
	public class mediaGridView : System.Windows.Forms.ListView
	{
		#region Variables
		private ColumnHeader notify=new ColumnHeader();
		private ColumnHeader title=new ColumnHeader();
		private ColumnHeader description=new ColumnHeader();
		private ColumnHeader rating=new ColumnHeader();
		private ColumnHeader stars=new ColumnHeader();
		private ColumnHeader performers=new ColumnHeader();
		private ColumnHeader category=new ColumnHeader();
		private ColumnHeader timesPlayed=new ColumnHeader();
		private ListViewItem li;
		private int X=0;
		private int Y=0;
		private string subItemText ;
		private int subItemSelected = 0 ;		
		private System.Windows.Forms.TextBox  editBox = new System.Windows.Forms.TextBox();

		public bool isEditable;
		
		private static TVComparer TVComp=new TVComparer();

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		#endregion

		#region public mediaGridView()
		public mediaGridView()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call

			SetProperties();
			InsertColumns();
			makeEditable();
		}
		#endregion

		#region private void makeEditable()
		private void makeEditable()
		{
			editBox.Size  = new System.Drawing.Size(0,0);
			editBox.Location = new System.Drawing.Point(0,0);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {this.editBox});			
			editBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.EditOver);
			editBox.LostFocus += new System.EventHandler(this.FocusOver);
			editBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			editBox.BackColor = Color.LightYellow; 
			editBox.BorderStyle = BorderStyle.Fixed3D;
			editBox.Hide();
			editBox.Text = "";
		}
		#endregion

		#region "ListView Initialization Code"
		#region private void SetProperties()
		private void SetProperties()
		{
			this.View=View.Details;
			this.FullRowSelect=true;
			this.GridLines=true;
			this.Scrollable=true;
			this.isEditable = false;
//			this.DoubleClick += new System.EventHandler(this.mediaDoubleClick);
		}
		#endregion

		#region private void InsertColumns()
		private void InsertColumns()
		{
			notify.Text="Notify Me";
			notify.Width=60;
			
			title.Text = "Title";
			title.Width = 400;

			description.Text = "Description";
			description.Width = 300;
	
			rating.Text = "Rating";
			rating.Width = 65;

			stars.Text = "Stars";
			stars.Width = 65;

			performers.Text = "Performers";
			performers.Width = 100;	
		
			category.Text = "Category";
			category.Width = 65;

			timesPlayed.Text = "Played";
			timesPlayed.Width = 40;

			if (this.CheckBoxes==true)
				this.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {this.notify,this.stars,this.rating,this.performers,this.title,this.description,this.category,this.timesPlayed});
			else
				this.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {this.title,this.description,this.category,this.timesPlayed,this.stars,this.rating,this.performers});
		}
		#endregion

		
		#endregion 

		#region Overloaded Methods
		public new void Clear()
		{
			base.Clear();
			InsertColumns();
		}

		public void Sort(TVComparer.ESortBy SortBy, TVComparer.ESortMode SortMode)
		{
			TVComp.SortBy=SortBy;
			TVComp.SortMode=SortMode;

			this.ListViewItemSorter=new ListViewItemComparer();
			base.Sort();
		}
		#endregion

		#region Specialized Item Insertion
		public void InsertMediaFile(DataRow drMedia)
		{
			ListViewItem item=new ListViewItem();            
//			item.BackColor=media.BackColor;
			
			if (this.CheckBoxes==true)
			{
				item.Text="";
				item.SubItems.Add(drMedia["title"].ToString());
			}
			else
			{
				item.Text=drMedia["title"].ToString();
			}

			item.SubItems.Add(drMedia["Description"].ToString());
			item.SubItems.Add(drMedia["Performers"].ToString());
			item.SubItems.Add(drMedia["Rating"].ToString());
			item.SubItems.Add(drMedia["Stars"].ToString());
        
			item.Tag=drMedia;
			this.Items.AddRange(new ListViewItem[]{item});
		}

		public void InsertMediaFile(DataRowView drMedia)
		{
			ListViewItem item=new ListViewItem();            
			//			item.BackColor=media.BackColor;
			
			if (this.CheckBoxes==true)
			{
				item.Text="";
				item.SubItems.Add(drMedia["title"].ToString());
			}
			else
			{
				item.Text=drMedia["title"].ToString();
			}

			item.SubItems.Add(drMedia["Description"].ToString());
			item.SubItems.Add(drMedia["Performers"].ToString());
			item.SubItems.Add(drMedia["Rating"].ToString());
			item.SubItems.Add(drMedia["Stars"].ToString());
        
			item.Tag=drMedia;
			this.Items.AddRange(new ListViewItem[]{item});
		}

		public void InsertMediaFileRange(DataView dvMedia)
		{
			this.BeginUpdate();
			EventLog.WriteEntry("sctv","inserting media");
			foreach(DataRowView dr in dvMedia)
			{
				InsertMediaFile(dr);
			}
			this.EndUpdate();
		}

		public void InsertMediaFileRange(ListViewItem[] items)
		{
//			mediaGridView tempView = new mediaGridView();
//			tempView = (mediaGridView)arList;
			foreach(ListViewItem thisItem in items)
			{
				DataRow dr = (DataRow)thisItem.Tag;
//				EventLog.WriteEntry("sctv","dr "+ dr.ItemArray.ToString());
				InsertMediaFile(dr);
			}
		}
		#endregion

		#region protected override void Dispose( bool disposing )
		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#endregion

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// mediaGridView
			// 
			this.Leave += new System.EventHandler(this.mediaGridView_Leave);
			this.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.TVListView_ColumnClick);

		}
		#endregion

		#region Event Handlers
		#region private void TVListView_ColumnClick(object sender, ColumnClickEventArgs e)
		private void TVListView_ColumnClick(object sender, ColumnClickEventArgs e)
		{
			//This column is not used for sorting
			if (this.Columns[e.Column]==notify)
				return;

			//This column is not used for sorting
			if (this.Columns[e.Column]==stars)
				return;

			if (this.Columns[e.Column]==rating)
				TVComp.SortBy=TVComparer.ESortBy.StartTime;

			if (this.Columns[e.Column]==performers)
				TVComp.SortBy=TVComparer.ESortBy.StopTime;

			if (this.Columns[e.Column]==title)
				TVComp.SortBy=TVComparer.ESortBy.Title;

			if (this.Columns[e.Column]==description)
				TVComp.SortBy=TVComparer.ESortBy.Channel;

			this.ListViewItemSorter=new ListViewItemComparer();		
			this.Sort();
		}
		#endregion

		#region private void EditOver(object sender, System.Windows.Forms.KeyPressEventArgs e)
		private void EditOver(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if ( e.KeyChar == 13 ) 
			{
				li.SubItems[subItemSelected].Text = editBox.Text;
				editBox.Hide();
			}

			if ( e.KeyChar == 27 ) 
				editBox.Hide();

//			saveMediaListData();
		}
		#endregion

		#region private void FocusOver(object sender, System.EventArgs e)
		private void FocusOver(object sender, System.EventArgs e)
		{
			li.SubItems[subItemSelected].Text = editBox.Text;
			editBox.Hide();
//			saveMediaListData();
		}
		#endregion
		#endregion

		#region public void saveMediaListData()
		public void saveMediaListData(DataView dvMediaList)
		{
			try
			{
				XmlDocument tempDoc = new XmlDocument();
				XmlNode mainNode = tempDoc.CreateElement("","mediaFiles","");
				tempDoc.AppendChild(mainNode);
//				foreach(ListViewItem listItem in this.Items)
				foreach(DataRowView dr in dvMediaList)
				{
//					DataRow dr = (DataRow)listItem.Tag;

					XmlNode newNode = tempDoc.CreateNode(XmlNodeType.Element,"media","");
					mainNode.AppendChild(newNode);
					XmlNode nameNode = tempDoc.CreateNode(XmlNodeType.Element,"title","");
//					nameNode.InnerText = listItem.SubItems[0].Text;
					nameNode.InnerText = dr["title"].ToString();
					XmlNode performersNode = tempDoc.CreateNode(XmlNodeType.Element,"performers","");
					performersNode.InnerText = dr["performers"].ToString();
					XmlNode ratingNode = tempDoc.CreateNode(XmlNodeType.Element,"rating","");
					ratingNode.InnerText = dr["rating"].ToString();
					XmlNode descriptionNode = tempDoc.CreateNode(XmlNodeType.Element,"description","");
//					descriptionNode.InnerText = listItem.SubItems[1].Text;
					descriptionNode.InnerText = dr["description"].ToString();
					XmlNode starsNode = tempDoc.CreateNode(XmlNodeType.Element,"stars","");
					starsNode.InnerText = dr["stars"].ToString();
					XmlNode categoryNode = tempDoc.CreateNode(XmlNodeType.Element,"category","");
					categoryNode.InnerText = dr["category"].ToString();
					XmlNode timesPlayedNode = tempDoc.CreateNode(XmlNodeType.Element,"timesPlayed","");
					timesPlayedNode.InnerText = dr["timesPlayed"].ToString();
					XmlNode filePathNode = tempDoc.CreateNode(XmlNodeType.Element,"filePath","");
					filePathNode.InnerText = dr["filePath"].ToString();

					newNode.AppendChild(nameNode);
					newNode.AppendChild(performersNode);
					newNode.AppendChild(ratingNode);
					newNode.AppendChild(descriptionNode);
					newNode.AppendChild(starsNode);
					newNode.AppendChild(categoryNode);
					newNode.AppendChild(timesPlayedNode);
					newNode.AppendChild(filePathNode);					
				}
				tempDoc.Save("config\\media.xml");
			}
			catch(XmlException ex)
			{
				EventLog.WriteEntry("sctv","exception "+ ex.ToString());
			}

			// Serialize
			//			FileStream fsWrite = File.Open("Info.lvi", FileMode.Create,
			//				FileAccess.ReadWrite);
			//			BinaryFormatter bf = new BinaryFormatter();
			//			bf.Serialize( fsWrite, new ArrayList(mediaListView.Items) );
			//			fsWrite.Close();
			
			// Deserialize
			//			FileStream fsRead = File.Open("Info.lvi", FileMode.Open,
			//				FileAccess.ReadWrite);
			//			bf = new BinaryFormatter();
			//			mediaListView.Items.AddRange( ( ListViewItem[] )
			//				((ArrayList)bf.Deserialize( fsRead )).ToArray( typeof(
			//				ListViewItem) ) );
			//			fsRead.Close();
		}
		#endregion

		#region public bool editMedia(ListViewItem theItem)
		public bool editMedia(ListViewItem theItem)
		{
			li = theItem;
			// Check the subitem clicked .
			int nStart = X ;
			int spos = 0 ; 
			int epos = this.Columns[0].Width ;
			for ( int i=0; i < this.Columns.Count ; i++)
			{
				if (nStart > spos && nStart < epos) 
				{
					subItemSelected = i ;
					break; 
				}
				
				spos = epos ; 
				epos += this.Columns[i].Width;
			}
			subItemText = li.SubItems[subItemSelected].Text ;

			string colName = this.Columns[subItemSelected].Text ;
			if(subItemSelected > 0 && colName != "play")
			{
//			if ( colName == "Continent" ) 
//			{
//				Rectangle r = new Rectangle(spos , li.Bounds.Y , epos , li.Bounds.Bottom);
//				cmbBox.Size  = new System.Drawing.Size(epos - spos , li.Bounds.Bottom-li.Bounds.Top);
//				cmbBox.Location = new System.Drawing.Point(spos , li.Bounds.Y);
//				cmbBox.Show() ;
//				cmbBox.Text = subItemText;
//				cmbBox.SelectAll() ;
//				cmbBox.Focus();
//			}
//			else
//			{
//				Rectangle r = new Rectangle(spos , li.Bounds.Y , epos , li.Bounds.Bottom);
				editBox.Size  = new System.Drawing.Size(epos - spos , li.Bounds.Bottom-li.Bounds.Top-10);
				editBox.Location = new System.Drawing.Point(spos , li.Bounds.Y);
				editBox.Show() ;
				editBox.Text = subItemText;
				editBox.SelectAll() ;
				editBox.Focus();
				return true;
			}
			else
				return false;
		}
		#endregion

		#region public void mediaMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		public void mediaMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			X = e.X ;
			Y = e.Y ;
		}
		#endregion

		#region private void mediaGridView_Leave(object sender, System.EventArgs e)
		private void mediaGridView_Leave(object sender, System.EventArgs e)
		{
////			saveMediaListData();
		}
		#endregion

		#region "ListView's Item Comparer - Uses TVComparer's Comparison Functions"
		protected class ListViewItemComparer : IComparer 
		{
			public ListViewItemComparer() 
			{
				FlipSortingOrder();
			}

			private void FlipSortingOrder()
			{
				//Flip sorting mode
				switch(TVComp.SortMode)
				{
					case TVComparer.ESortMode.Ascending:
						TVComp.SortMode=TVComparer.ESortMode.Descending;
						break;
					case TVComparer.ESortMode.Descending:
						TVComp.SortMode=TVComparer.ESortMode.Ascending;
						break;
				}
			}

			public int Compare(object x, object y) 
			{
				ListViewItem Item=(ListViewItem)x;
				DataRowView Prog1=(DataRowView)((ListViewItem)x).Tag;
				DataRowView Prog2=(DataRowView)((ListViewItem)y).Tag;

				return(TVComp.Compare(Prog1,Prog2));
			}
		}
		#endregion
	}
}
