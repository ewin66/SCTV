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
	public class playScheduleListView : System.Windows.Forms.ListView
	{
		private ColumnHeader Col=new ColumnHeader();
		private playScheduleOrganizer playSchedules;

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#region "Properties"
		public playScheduleOrganizer SetOrganizer
		{
			set 
			{ 
				playSchedules=value; 
				playSchedules.OnListChanged+=new SCTVTelevision.playScheduleOrganizer.playScheduleListChanged(playSchedules_OnListChanged);
			}
		}
		#endregion

		public playScheduleListView()
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
			Col.Text="Shows To Play";
			Col.Width=200;			
			this.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {this.Col});
		}

		private void PopulateListView()
		{
			foreach(playSchedule show in playSchedules.GetList)
			{
				if (show.Conflicts==true)
					InsertplaySchedule(show,Color.Red);
				else
					InsertplaySchedule(show,Color.White);
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
		public void InsertplaySchedule(playSchedule show, Color col)
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
			this.Enter+=new EventHandler(playScheduleListView_Enter);
		}
		#endregion

		#region "Internal Events"
		private void playSchedules_OnListChanged()
		{
			Clear();
			PopulateListView();
		}

		private void playScheduleListView_Enter(object sender, EventArgs e)
		{
			if (this.SelectedItems.Count==0 && this.Items.Count!=0)
			{
				this.Items[0].Selected=true;
			}
		}
		#endregion
	}
}
