using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace SCTVTelevision
{
	/// <summary>
	/// Summary description for conflictDisplay.
	/// </summary>
	public class conflictDisplay : System.Windows.Forms.Form
	{
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ListView conflictListView;
		private System.Windows.Forms.Label label1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public conflictDisplay(playSchedule pSchedule1,playSchedule pSchedule2)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			addToList(pSchedule1);
			addToList(pSchedule2);
		}

		private void addToList(playSchedule pSchedule)
		{
			TVProgramme prog = (TVProgramme)pSchedule.theProgramme;
			ListViewItem lvItem = new ListViewItem();
			lvItem.Text = prog.Title;
			lvItem.SubItems.Add(prog.StartTime.ToShortTimeString());
//			lvItem.SubItems.Add();
//			lvItem.SubItems.Add(prog.StartTime.ToShortTimeString());
			lvItem.Tag = pSchedule;
//			lvItem.Tag = pSchedule.index;
			conflictListView.Items.Add(lvItem);
//			EventLog.WriteEntry("sctv","prog: "+ prog.Title);
		}

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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.conflictListView = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// conflictListView
			// 
			this.conflictListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																							   this.columnHeader1,
																							   this.columnHeader2});
			this.conflictListView.FullRowSelect = true;
			this.conflictListView.GridLines = true;
			this.conflictListView.Location = new System.Drawing.Point(0, 16);
			this.conflictListView.Name = "conflictListView";
			this.conflictListView.Size = new System.Drawing.Size(504, 72);
			this.conflictListView.TabIndex = 0;
			this.conflictListView.View = System.Windows.Forms.View.Details;
			this.conflictListView.SelectedIndexChanged += new System.EventHandler(this.conflictListView_SelectedIndexChanged);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Title";
			this.columnHeader1.Width = 352;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Time";
			this.columnHeader2.Width = 64;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(0, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(200, 16);
			this.label1.TabIndex = 2;
			this.label1.Text = "Choose the show you want to watch.";
			// 
			// conflictDisplay
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(416, 93);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.conflictListView);
			this.Name = "conflictDisplay";
			this.Text = "Play Schedule Conflicts";
			this.ResumeLayout(false);

		}
		#endregion

		private void conflictListView_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			//set the tag information
			ListView lvSelected = (ListView)sender;
			if(lvSelected.SelectedItems.Count > 0)
			{
				foreach(ListViewItem Li in lvSelected.Items)
				{
					if(Li.Index != lvSelected.SelectedIndices[0])//this is the show to delete
					{
						conflictDisplay.ActiveForm.Tag = Li.Tag;
					}
				}
				this.Close();
			}
		}
	}
}
