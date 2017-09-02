using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace SCTVTelevision
{
	/// <summary>
	/// Summary description for TVListView.
	/// </summary>
	public class TVListView : System.Windows.Forms.ListView
	{
		private ColumnHeader ColNotify=new ColumnHeader();
		private ColumnHeader ColDate=new ColumnHeader();
		private ColumnHeader ColStartTime=new ColumnHeader();
		private ColumnHeader ColEndTime=new ColumnHeader();
		private ColumnHeader ColTitle=new ColumnHeader();
		private ColumnHeader ColChannel=new ColumnHeader();

		private static TVComparer TVComp=new TVComparer();

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public TVListView()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call

			SetProperties();
			InsertColumns();
		}

		#region "ListView Initialization Code"
		private void SetProperties()
		{
			this.View=View.Details;
			this.FullRowSelect=true;
			this.GridLines=true;
			this.Scrollable=true;
			this.Sort();
		}

		private void InsertColumns()
		{
			ColNotify.Text="Notify Me";
			ColNotify.Width=60;
	
			ColChannel.Text = "Channel";
			ColChannel.Width = 90;
			
			ColTitle.Text = "Title";
			ColTitle.Width = 250;

			ColStartTime.Text = "Start Time";
			ColStartTime.Width = 100;

			ColEndTime.Text = "End Time";
			ColEndTime.Width = 100;

			ColDate.Text = "Date";
			ColDate.Width = 160;			

			if (this.CheckBoxes==true)
				this.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {this.ColNotify,this.ColDate,this.ColStartTime,this.ColEndTime,this.ColTitle,this.ColChannel});
			else
				this.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {this.ColChannel,this.ColTitle,this.ColStartTime,this.ColEndTime,this.ColDate});
		}
		#endregion 

		#region "Overloaded Methods"
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

		#region "Specialized Item Insertion"
		public void InsertTVProgramme(TVProgramme programme)
		{
			ListViewItem item=new ListViewItem();            
			item.BackColor=programme.BackColor;

//			if (this.CheckBoxes==true)
//			{
//				item.Text="";
//				item.SubItems.Add(programme.StartTime.ToLongDateString());
//			}
//			else
//			{
//				item.Text=programme.StartTime.ToLongDateString();
//			}
			item.Text=programme.Channel.DisplayName;
//			item.SubItems.Add(programme.Channel.DisplayName);
			item.SubItems.Add(programme.Title);
			item.SubItems.Add(programme.StartTime.ToShortTimeString());
			item.SubItems.Add(programme.StopTime.ToShortTimeString());
			item.SubItems.Add(programme.StartTime.DayOfWeek.ToString());
			item.Tag=programme;

			this.Items.AddRange(new ListViewItem[]{item});
		}

		public void InsertTVProgrammeRange(TVProgramme[] programme)
		{
			this.BeginUpdate();
			foreach(TVProgramme prog in programme)
			{
				InsertTVProgramme(prog);
			}
			this.EndUpdate();
		}
		#endregion

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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
			this.ColumnClick+=new ColumnClickEventHandler(TVListView_ColumnClick);
		}
		#endregion

		#region "Event Handlers"
		private void TVListView_ColumnClick(object sender, ColumnClickEventArgs e)
		{
			//This column is not used for sorting
			if (this.Columns[e.Column]==ColNotify)
				return;

			//This column is not used for sorting
			if (this.Columns[e.Column]==ColDate)
				return;

			if (this.Columns[e.Column]==ColStartTime)
				TVComp.SortBy=TVComparer.ESortBy.StartTime;

			if (this.Columns[e.Column]==ColEndTime)
				TVComp.SortBy=TVComparer.ESortBy.StopTime;

			if (this.Columns[e.Column]==ColTitle)
				TVComp.SortBy=TVComparer.ESortBy.Title;

			if (this.Columns[e.Column]==ColChannel)
				TVComp.SortBy=TVComparer.ESortBy.Channel;

			this.ListViewItemSorter=new ListViewItemComparer();		
			this.Sort();
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
				TVProgramme Prog1=(TVProgramme)((ListViewItem)x).Tag;
				TVProgramme Prog2=(TVProgramme)((ListViewItem)y).Tag;

				return(TVComp.Compare(Prog1,Prog2));
			}
		}
		#endregion
	}
}
