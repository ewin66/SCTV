using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using Microsoft.ApplicationBlocks.ExceptionManagement;

namespace SCTVTelevision
{
	/// <summary>
	/// Summary description for TVGridView.
	/// </summary>
	public class TVGridView : System.Windows.Forms.UserControl
	{
		public AxMSFlexGridLib.AxMSFlexGrid FlexGrid;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private Hashtable Shows=new Hashtable();
		private ArrayList ColumnTimeSlots=new ArrayList();
		private TVProgramme mSelectedProgramme;
		private int NowColumn; //Column of shows that are on RIGHT NOW, used to automatically scroll the grid
		private int NumColumns;
		private const int TimeSlotMinutes=30;

		#region "Custom Events"
		public delegate void SelectionChanged(object o, TVGridViewEventArgs e);
		public event SelectionChanged OnSelectionChanged;
		#endregion		

		#region "Properties"
		public TVProgramme SelectedProgramme
		{
			get { return(mSelectedProgramme); }
		}
		#endregion

		public TVGridView()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			Shows.Clear();	
			ColumnTimeSlots.Clear();			
			NowColumn=0;
			NumColumns=0;
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(TVGridView));
			this.FlexGrid = new AxMSFlexGridLib.AxMSFlexGrid();
			((System.ComponentModel.ISupportInitialize)(this.FlexGrid)).BeginInit();
			this.SuspendLayout();
			// 
			// FlexGrid
			// 
			this.FlexGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.FlexGrid.Location = new System.Drawing.Point(0, 0);
			this.FlexGrid.Name = "FlexGrid";
			this.FlexGrid.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("FlexGrid.OcxState")));
			this.FlexGrid.Size = new System.Drawing.Size(150, 150);
			this.FlexGrid.TabIndex = 0;
			this.FlexGrid.SelChange += new System.EventHandler(this.FlexGrid_SelChange);
			// 
			// TVGridView
			// 
			this.Controls.Add(this.FlexGrid);
			this.Name = "TVGridView";
			((System.ComponentModel.ISupportInitialize)(this.FlexGrid)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		#region "Public Methods"
		public void ScrollToTodaysColumn()
		{
			DateTime currentTime=System.DateTime.Now;
			TimeSpan difference;
			int ColSpan=0;
			
			if (NowColumn>0)
			{
				difference=currentTime-(DateTime)ColumnTimeSlots[NowColumn];
				ColSpan=(int)difference.TotalMinutes/TimeSlotMinutes;
			}

			NowColumn+=ColSpan;
			if (NowColumn>0)
			{
				if (NowColumn<NumColumns)
					FlexGrid.LeftCol=NowColumn;
				else
					FlexGrid.LeftCol=NumColumns-1;
			}
		}
		#endregion

		#region "Specialized Item Insertion"
		public void PopulateGrid(TVChannel[] Channels, TVProgramme[] Shows, TVProgramme[] favorites, Color favoritesColor)
		{
			int Length;
			Hashtable ChannelTable=new Hashtable();
			Hashtable ShowTimeTable=new Hashtable();
			DateTime Today=System.DateTime.Now;
			DateTime LowerBound=new DateTime(Today.Year,Today.Month,Today.Day,0,0,0,0);
			DateTime UpperBound=LowerBound.AddDays(Form1.ProgramConfiguration.DaysToDisplay);
			DateTime Iterator=new DateTime(Today.Year,Today.Month,Today.Day,0,0,0,0);
			TimeSpan BoundDifference=UpperBound-LowerBound;
			int OldDistanceFromZero=0;

			NowColumn=0;
			NumColumns=(int)BoundDifference.TotalMinutes/30+1;

			this.Shows.Clear();
			FlexGrid.Clear();
			FlexGrid.Cols=NumColumns;
			Length=FindLongestChannelName(Channels);
			FlexGrid.set_ColWidth(0,Length*100);

			//Populate Channel Names into grid
			FlexGrid.set_TextArray(0,"Channels");
			FlexGrid.Rows=Channels.Length+1;			
			for(int x=0; x<Channels.Length; x++)
			{
				FlexGrid.set_TextMatrix(x+1,0,Channels[x].DisplayName);				

				try
				{
					ChannelTable.Add(Channels[x].ID,x+1);
				}
				catch(Exception ex)
				{
					TVException tvex=new TVException("There is an error in your XMLTV configuration. Listings have been downloaded for multiple channels of the same number so I don't know how to distinguish between them.",ex);
					ExceptionManager.Publish(tvex);

					MessageBox.Show(tvex.Message,"TV Guide Error");
				}
			}

			//Populate time slots in 30 minute increments
			for(int x=0; x<NumColumns-1; x++)
			{				
				TimeSpan ClosestMatch; //Used to determine the closest matching 30 minute interval to the current time
				int DistanceFromZero=0;

				if (Iterator.Hour==0 && Iterator.Minute==0)
				{
					FlexGrid.set_TextMatrix(0,x+1,Iterator.ToShortDateString()+"   "+Iterator.ToLongTimeString());
				}
				else
				{
					FlexGrid.set_TextMatrix(0,x+1,Iterator.ToLongTimeString());
				}
				FlexGrid.set_ColAlignment(x+1,2);

				ColumnTimeSlots.Add(Iterator);
				ShowTimeTable.Add(Iterator,x+1);

				FlexGrid.set_ColWidth(x+1,2000);
				Iterator=Iterator.AddMinutes(TimeSlotMinutes);

				//Determine which column is closest to the current time
				ClosestMatch=Today-Iterator;
				DistanceFromZero=System.Math.Abs(0-(int)ClosestMatch.TotalMinutes);
				if (OldDistanceFromZero<DistanceFromZero) //if we were closer to a difference of zero in the previous calculation, just set the olddistance for the next comparison
				{
					OldDistanceFromZero=DistanceFromZero;
				}
				else
				{
					OldDistanceFromZero=DistanceFromZero;
					NowColumn=x+1;
				}
			}

			FlexGrid.Cols++;
			
			foreach(TVProgramme prog in Shows)
			{
				int Row=0,Col=0;
				
				try
				{
					Row=(int)ChannelTable[prog.Channel.ID];
					Col=(int)ShowTimeTable[RoundTime_ToNearest30MinInterval(prog.StartTime)];
				}
				catch(Exception ex)
				{
//					string msgdata="Show: "+prog.StartTime.ToShortDateString()+" "+prog.StartTime.ToLongTimeString()+"-"+prog.StopTime.ToShortDateString()+" "+prog.StopTime.ToLongTimeString()+" "+prog.Title+" "+prog.Channel.ToString();
//					TVException tvex=new TVException("Could not find show in hashtable because it doesn't fit into a 30 minute category. Data: "+msgdata,ex);
					//ExceptionManager.Publish(tvex);
//					Console.WriteLine(msgdata);
				}

				if (Row>0 && Col>0)
				{
					string Key;
					TimeSpan ShowLength=prog.StopTime-prog.StartTime;

					FlexGrid.set_TextMatrix(Row,Col,prog.Title);
					FlexGrid.Col=Col;
					FlexGrid.Row=Row;
//					foreach(favorites theFavorite in Form1.favoriteOrganizer.AllShows)
//					{
//						TVProgramme theShow = (TVProgramme);
//						if(theShow.Title == prog.Title)//this is one of the favorites
//						{
//							FlexGrid.CellBackColor=favoritesColor;
//							break;
//						}
//						else
//						{
//							FlexGrid.CellBackColor=prog.BackColor;
//						}
//					}
					Key="Col"+Col.ToString()+"Row"+Row.ToString();
					this.Shows[Key]=prog;

					if (ShowLength.TotalMinutes>35 && Col<NumColumns-1)
					{
						int NumTimeSlots=(int)ShowLength.TotalMinutes/30;
						int Mod=(int)ShowLength.TotalMinutes%30;
						int Limit=0;
						
						if (Mod!=0 && Mod>5)
							NumTimeSlots++;

						FlexGrid.set_MergeCol(Col,true);
						FlexGrid.set_MergeRow(Row,true);

						if (Col+NumTimeSlots>NumColumns)
							Limit=NumColumns;
						else Limit=Col+NumTimeSlots;

						for(int x=Col; x<=Limit; x++)
						{
							Key="Col"+x.ToString()+"Row"+Row.ToString();
							this.Shows[Key]=prog;

							try
							{
								FlexGrid.set_TextMatrix(Row,x,prog.Title +" - "+ prog.Description);
								FlexGrid.Col=x;
								FlexGrid.Row=Row;
								FlexGrid.CellBackColor=prog.BackColor;
							}
							catch(Exception ex)
							{
								ExceptionManager.Publish(ex);
								Console.WriteLine("error: {0} {1} {2} {3} {4} {5}",NumTimeSlots,prog.StartTime.ToShortDateString(),prog.StartTime.ToLongTimeString(),prog.Title,prog.Channel.ToString(),ShowLength.TotalMinutes);
							}
						}	
					}
				}
			}

			if (NowColumn==0)
				NowColumn++;

			FlexGrid.LeftCol=NowColumn;
		}
//
//		public void PopulateGrid(TVProgramme[] Shows)//this is for populating the playSchedule Grid
//		{
//			int Length = 3;
//			Hashtable ChannelTable=new Hashtable();
//			Hashtable ShowTimeTable=new Hashtable();
//			DateTime Today=System.DateTime.Now;
//			DateTime LowerBound=new DateTime(Today.Year,Today.Month,Today.Day,0,0,0,0);
//			DateTime UpperBound=LowerBound.AddDays(Form1.ProgramConfiguration.DaysToDisplay);
//			DateTime Iterator=new DateTime(Today.Year,Today.Month,Today.Day,0,0,0,0);
//			TimeSpan BoundDifference=UpperBound-LowerBound;
//			int OldDistanceFromZero=0;
//
//			NowColumn=0;
//			NumColumns=(int)BoundDifference.TotalMinutes/30+1;
//
//			this.Shows.Clear();
//			FlexGrid.Clear();
//			FlexGrid.Cols=NumColumns;
////			Length=FindLongestChannelName(Channels);
//			FlexGrid.set_ColWidth(0,Length*100);
//
//			//Populate playSchedules into grid
//			FlexGrid.set_TextArray(0,"Schedules");
////			FlexGrid.Rows=Channels.Length+1;			
//			FlexGrid.Rows=10;			
//			for(int x=0; x<9; x++)
//			{
//				FlexGrid.set_TextMatrix(x+1,0,x.ToString());				
//
//				try
//				{
//					ChannelTable.Add(x.ToString(),x+1);
//				}
//				catch(Exception ex)
//				{
//					TVException tvex=new TVException("There is an error in your XMLTV configuration. Listings have been downloaded for multiple channels of the same number so I don't know how to distinguish between them.",ex);
//					ExceptionManager.Publish(tvex);
//
//					MessageBox.Show(tvex.Message,"TV Guide Error");
//				}
//			}
//
//			//Populate time slots in 30 minute increments
//			for(int x=0; x<NumColumns-1; x++)
//			{				
//				TimeSpan ClosestMatch; //Used to determine the closest matching 30 minute interval to the current time
//				int DistanceFromZero=0;
//
//				if (Iterator.Hour==0 && Iterator.Minute==0)
//				{
//					FlexGrid.set_TextMatrix(0,x+1,Iterator.ToShortDateString()+"   "+Iterator.ToLongTimeString());
//				}
//				else
//				{
//					FlexGrid.set_TextMatrix(0,x+1,Iterator.ToLongTimeString());
//				}
//				FlexGrid.set_ColAlignment(x+1,2);
//
//				ColumnTimeSlots.Add(Iterator);
//				ShowTimeTable.Add(Iterator,x+1);
//
//				FlexGrid.set_ColWidth(x+1,2000);
//				Iterator=Iterator.AddMinutes(TimeSlotMinutes);
//
//				//Determine which column is closest to the current time
//				ClosestMatch=Today-Iterator;
//				DistanceFromZero=System.Math.Abs(0-(int)ClosestMatch.TotalMinutes);
//				if (OldDistanceFromZero<DistanceFromZero) //if we were closer to a difference of zero in the previous calculation, just set the olddistance for the next comparison
//				{
//					OldDistanceFromZero=DistanceFromZero;
//				}
//				else
//				{
//					OldDistanceFromZero=DistanceFromZero;
//					NowColumn=x+1;
//				}
//			}
//
//			FlexGrid.Cols++;
//			
//			foreach(TVProgramme prog in Shows)
//			{
//				int Row=0,Col=0,progCount=0;
//				Console.WriteLine("Show: "+prog.StartTime.ToShortDateString()+" "+prog.StartTime.ToLongTimeString()+"-"+prog.StopTime.ToShortDateString()+" "+prog.StopTime.ToLongTimeString()+" "+prog.Title+" "+prog.Channel.ToString());
//				try
//				{
////					Row=(int)ChannelTable[prog.Channel.ID];
//					Console.WriteLine("prog.channel.id: "+ prog.Channel.ID);
//					Row=(int)ChannelTable[progCount.ToString()];
//					Col=(int)ShowTimeTable[RoundTime_ToNearest30MinInterval(prog.StartTime)];
//				}
//				catch(Exception ex)
//				{
////					string msgdata="Show: "+prog.StartTime.ToShortDateString()+" "+prog.StartTime.ToLongTimeString()+"-"+prog.StopTime.ToShortDateString()+" "+prog.StopTime.ToLongTimeString()+" "+prog.Title+" "+prog.Channel.ToString();
////					TVException tvex=new TVException("Could not find show in hashtable because it doesn't fit into a 30 minute category. Data: "+msgdata,ex);
////					ExceptionManager.Publish(tvex);
////					Console.WriteLine(msgdata);
//					Console.WriteLine("error: "+ ex.ToString());
//				}
//
//				if (Row>0 && Col>0)
//				{
//					string Key;
//					TimeSpan ShowLength=prog.StopTime-prog.StartTime;
//
//					FlexGrid.set_TextMatrix(Row,Col,prog.Title);
//					FlexGrid.Col=Col;
//					FlexGrid.Row=Row;
////					if(prog.
//					FlexGrid.CellBackColor=prog.BackColor;
//
//					Key="Col"+Col.ToString()+"Row"+Row.ToString();
//					this.Shows[Key]=prog;
//
//					if (ShowLength.TotalMinutes>35 && Col<NumColumns-1)
//					{
//						int NumTimeSlots=(int)ShowLength.TotalMinutes/30;
//						int Mod=(int)ShowLength.TotalMinutes%30;
//						int Limit=0;
//						
//						if (Mod!=0 && Mod>5)
//							NumTimeSlots++;
//
//						FlexGrid.set_MergeCol(Col,true);
//						FlexGrid.set_MergeRow(Row,true);
//
//						if (Col+NumTimeSlots>NumColumns)
//							Limit=NumColumns;
//						else Limit=Col+NumTimeSlots;
//
//						for(int x=Col; x<=Limit; x++)
//						{
//							Key="Col"+x.ToString()+"Row"+Row.ToString();
//							this.Shows[Key]=prog;
//
//							try
//							{
//								FlexGrid.set_TextMatrix(Row,x,prog.Title +" - "+ prog.Description);
//								FlexGrid.Col=x;
//								FlexGrid.Row=Row;
//								FlexGrid.CellBackColor=prog.BackColor;
//							}
//							catch(Exception ex)
//							{
//								ExceptionManager.Publish(ex);
//								Console.WriteLine("error: {0} {1} {2} {3} {4} {5}",NumTimeSlots,prog.StartTime.ToShortDateString(),prog.StartTime.ToLongTimeString(),prog.Title,prog.Channel.ToString(),ShowLength.TotalMinutes);
//							}
//						}	
//					}
//				}
//				progCount++;
//			}
//
//			if (NowColumn==0)
//				NowColumn++;
//
//			FlexGrid.LeftCol=NowColumn;
//		}

		public void PopulateGrid(playScheduleArrayList playSchedules)//this is for populating the playSchedule Grid
		{
			int Length = 3;
			Hashtable ChannelTable=new Hashtable();
			Hashtable ShowTimeTable=new Hashtable();
			DateTime Today=System.DateTime.Now;
			DateTime LowerBound=new DateTime(Today.Year,Today.Month,Today.Day,0,0,0,0);
			DateTime UpperBound=LowerBound.AddDays(Form1.ProgramConfiguration.DaysToDisplay);
			DateTime Iterator=new DateTime(Today.Year,Today.Month,Today.Day,0,0,0,0);
			TimeSpan BoundDifference=UpperBound-LowerBound;
			int OldDistanceFromZero=0;

			NowColumn=0;
			NumColumns=(int)BoundDifference.TotalMinutes/30+1;

			this.Shows.Clear();
			FlexGrid.Clear();
			FlexGrid.Cols=NumColumns;
			//			Length=FindLongestChannelName(Channels);
			FlexGrid.set_ColWidth(0,Length*100);

			//Populate playSchedules into grid
			FlexGrid.set_TextArray(0,"Schedules");
			//			FlexGrid.Rows=Channels.Length+1;	
			FlexGrid.Rows=10;			
			for(int x=0; x<9; x++)
			{
				FlexGrid.set_TextMatrix(x+1,0,x.ToString());				

				try
				{
					ChannelTable.Add(x.ToString(),x+1);
				}
				catch(Exception ex)
				{
					TVException tvex=new TVException("There is an error in your XMLTV configuration. Listings have been downloaded for multiple channels of the same number so I don't know how to distinguish between them.",ex);
					ExceptionManager.Publish(tvex);

					MessageBox.Show(tvex.Message,"TV Guide Error");
				}
			}

			//Populate time slots in 30 minute increments
			for(int x=0; x<NumColumns-1; x++)
			{				
				TimeSpan ClosestMatch; //Used to determine the closest matching 30 minute interval to the current time
				int DistanceFromZero=0;

				if (Iterator.Hour==0 && Iterator.Minute==0)
				{
					FlexGrid.set_TextMatrix(0,x+1,Iterator.ToShortDateString()+"   "+Iterator.ToLongTimeString());
				}
				else
				{
					FlexGrid.set_TextMatrix(0,x+1,Iterator.ToLongTimeString());
				}
				FlexGrid.set_ColAlignment(x+1,2);

				ColumnTimeSlots.Add(Iterator);
				ShowTimeTable.Add(Iterator,x+1);

				FlexGrid.set_ColWidth(x+1,2000);
				Iterator=Iterator.AddMinutes(TimeSlotMinutes);

				//Determine which column is closest to the current time
				ClosestMatch=Today-Iterator;
				DistanceFromZero=System.Math.Abs(0-(int)ClosestMatch.TotalMinutes);
				if (OldDistanceFromZero<DistanceFromZero) //if we were closer to a difference of zero in the previous calculation, just set the olddistance for the next comparison
				{
					OldDistanceFromZero=DistanceFromZero;
				}
				else
				{
					OldDistanceFromZero=DistanceFromZero;
					NowColumn=x+1;
				}
			}

			FlexGrid.Cols++;
			
			foreach(playSchedule pSchedule in playSchedules)
			{
				TVProgramme prog = (TVProgramme)pSchedule.theProgramme;
				int Row=0,Col=0,progCount=0;
				Console.WriteLine("gffd Show: "+prog.StartTime.ToShortDateString()+" "+prog.StartTime.ToLongTimeString()+"-"+prog.StopTime.ToShortDateString()+" "+prog.StopTime.ToLongTimeString()+" "+prog.Title+" "+prog.Channel.ToString());
				try
				{
					//					Row=(int)ChannelTable[prog.Channel.ID];
					Console.WriteLine("prog.channel.id: "+ prog.Channel.ID);
//					Row=(int)ChannelTable[progCount.ToString()];
					Row=(int)ChannelTable[pSchedule.index.ToString()];
					Col=(int)ShowTimeTable[RoundTime_ToNearest30MinInterval(prog.StartTime)];
				}
				catch(Exception ex)
				{
					//					string msgdata="Show: "+prog.StartTime.ToShortDateString()+" "+prog.StartTime.ToLongTimeString()+"-"+prog.StopTime.ToShortDateString()+" "+prog.StopTime.ToLongTimeString()+" "+prog.Title+" "+prog.Channel.ToString();
					//					TVException tvex=new TVException("Could not find show in hashtable because it doesn't fit into a 30 minute category. Data: "+msgdata,ex);
					//					ExceptionManager.Publish(tvex);
					//					Console.WriteLine(msgdata);
					Console.WriteLine("error: "+ ex.ToString());
				}

				if (Row>0 && Col>0)
				{
					string Key;
					TimeSpan ShowLength=prog.StopTime-prog.StartTime;

					FlexGrid.set_TextMatrix(Row,Col,pSchedule.index.ToString() +" "+ prog.Title);
					FlexGrid.Col=Col;
					FlexGrid.Row=Row;

					if(pSchedule.Conflicts==true)
					{
						FlexGrid.CellBackColor = Color.Red;
						Console.WriteLine("marking conflict red for "+ pSchedule.index.ToString() +" "+ prog.Title);
					}
					else
						FlexGrid.CellBackColor=prog.BackColor;

					Key="Col"+Col.ToString()+"Row"+Row.ToString();
					this.Shows[Key]=prog;

					if (ShowLength.TotalMinutes>35 && Col<NumColumns-1)
					{
						int NumTimeSlots=(int)ShowLength.TotalMinutes/30;
						int Mod=(int)ShowLength.TotalMinutes%30;
						int Limit=0;
						
						if (Mod!=0 && Mod>5)
							NumTimeSlots++;

						FlexGrid.set_MergeCol(Col,true);
						FlexGrid.set_MergeRow(Row,true);

						if (Col+NumTimeSlots>NumColumns)
							Limit=NumColumns;
						else Limit=Col+NumTimeSlots;

						for(int x=Col; x<=Limit; x++)
						{
							Key="Col"+x.ToString()+"Row"+Row.ToString();
							this.Shows[Key]=prog;

							try
							{
								FlexGrid.set_TextMatrix(Row,x,pSchedule.index.ToString() +" "+ prog.Title +" - "+ prog.Description);
								FlexGrid.Col=x;
								FlexGrid.Row=Row;
								if(pSchedule.Conflicts)
									FlexGrid.CellBackColor = Color.Red;
								else
									FlexGrid.CellBackColor = prog.BackColor;

							}
							catch(Exception ex)
							{
								ExceptionManager.Publish(ex);
								Console.WriteLine("error: {0} {1} {2} {3} {4} {5}",NumTimeSlots,prog.StartTime.ToShortDateString(),prog.StartTime.ToLongTimeString(),prog.Title,prog.Channel.ToString(),ShowLength.TotalMinutes);
							}
						}	
					}
				}
				progCount++;
			}

			if (NowColumn==0)
				NowColumn++;

			FlexGrid.LeftCol=NowColumn;
		}
		#endregion

		#region "Internal Methods"
		private DateTime RoundTime_ToNearest30MinInterval(DateTime TimeToRound)
		{
			DateTime RoundedTime=TimeToRound;

			//if the show starts a few minutes before or a few minutes after 30 minutes past the hour, we'll say it starts at 30 minutes past the hour
			if (TimeToRound.Minute>=20 && TimeToRound.Minute<=45)
				RoundedTime=new DateTime(TimeToRound.Year,TimeToRound.Month,TimeToRound.Day,TimeToRound.Hour,30,0,0);

			//If show starts just a few minutes past the hour, we'll say it starts on the hour
			if (TimeToRound.Minute<=10)
				RoundedTime=new DateTime(TimeToRound.Year,TimeToRound.Month,TimeToRound.Day,TimeToRound.Hour,0,0,0);

			//If show starts 5 minutes or less to the next hour, say it starts on that next hour
			if (TimeToRound.Minute>=55)
				RoundedTime=new DateTime(TimeToRound.Year,TimeToRound.Month,TimeToRound.Day,TimeToRound.Hour+1,0,0,0);

			return(RoundedTime);
		}

		private int FindLongestChannelName(TVChannel[] channels)
		{
			int length=0;

			foreach(TVChannel chan in channels)
			{
				if (chan.DisplayName.Length>length)
					length=chan.DisplayName.Length;
			}

			return(length);
		}
		#endregion

		#region "Internal Events"
		private void FlexGrid_SelChange(object sender, System.EventArgs e)
		{			
			int Col,Row;
			TVProgramme prog;

			Col=this.FlexGrid.Col;
			Row=this.FlexGrid.Row;

			string Key="Col"+Col.ToString()+"Row"+Row.ToString();
			prog=(TVProgramme)Shows[Key];
			mSelectedProgramme=prog;

			TVGridViewEventArgs tve=new TVGridViewEventArgs(prog);

//			Console.WriteLine("Key: {0}",Key);

			if (OnSelectionChanged!=null)
				OnSelectionChanged(this.FlexGrid,tve);
		}
		#endregion
	}

	#region public class TVGridViewEventArgs : EventArgs
	public class TVGridViewEventArgs : EventArgs
	{
		public TVProgramme Show;

		public TVGridViewEventArgs(TVProgramme p)
		{
			Show=p;
		}
	}
	#endregion
}
