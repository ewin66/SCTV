using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace SCTVTelevision
{
	/// <summary>
	/// Summary description for TVShowNotificationListView.
	/// </summary>
	public class TVShowNotificationListView : System.Windows.Forms.ListView
	{
		private ColumnHeader Col=new ColumnHeader();
		private ShowNotificationOrganizer Notifications;

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#region "Properties"
		public ShowNotificationOrganizer SetOrganizer
		{
			set 
			{ 
				Notifications=value; 
				Notifications.OnListChanged+=new SCTVTelevision.ShowNotificationOrganizer.showNotificationListChanged(Notifications_OnListChanged);
			}
		}
		#endregion

		public TVShowNotificationListView()
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
			this.GridLines=false;
			this.MultiSelect=true;
			this.Scrollable=true;
			this.HeaderStyle=ColumnHeaderStyle.Nonclickable;
		}

		private void InsertColumns()
		{
			Col.Text="Shows To Notify Me Of";
			Col.Width=200;			
			this.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {this.Col});
		}

		private void PopulateListView()
		{
			foreach(ShowNotification show in Notifications.GetList)
			{
				if (show.Conflicts==true)
					InsertShowNotification(show,Color.Red);
				else
					InsertShowNotification(show,Color.White);
			}
		}
		#endregion 

		#region "Overloaded Methods"
		public new void Clear()
		{
			base.Items.Clear();
		}
		#endregion

		#region "Specialized Item Insertion"
		public void InsertShowNotification(ShowNotification show, Color col)
		{
			ListViewItem item=new ListViewItem();

			item.BackColor=col;
			item.Text=show.name;        
			item.Tag=show;

			this.Items.AddRange(new ListViewItem[]{item});
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
			this.Enter+=new EventHandler(TVShowNotificationListView_Enter);
		}
		#endregion

		#region "Internal Events"
		private void Notifications_OnListChanged()
		{
			Clear();
			PopulateListView();
		}

		private void TVShowNotificationListView_Enter(object sender, EventArgs e)
		{
			if (this.SelectedItems.Count==0 && this.Items.Count!=0)
			{
				this.Items[0].Selected=true;
			}
		}
		#endregion
	}
}
