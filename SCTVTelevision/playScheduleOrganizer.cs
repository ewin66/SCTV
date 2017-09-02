using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Collections;
using System.Data;

namespace SCTVTelevision
{
	/// <summary>
	/// Summary description for ShowNotificationOrganizer.
	/// </summary>
	public class playScheduleOrganizer
	{
		#region Variables
		private playScheduleArrayList playSchedules=new playScheduleArrayList();
		private bool mConflictsExist;
		private ArrayList mAllChannels=new ArrayList();
		private ArrayList mAllShows=new ArrayList();
		public int NumConflicts=0;
		#endregion

		#region "Delegates"
		public delegate void playScheduleListChanged();
		public event playScheduleListChanged OnListChanged;
		#endregion

		#region "Properties"
		public bool ConflictsExist
		{
			get { return(mConflictsExist); }
		}

		public playScheduleArrayList GetList
		{
			get { return(playSchedules); }
		}

		public TVChannel[] AllChannels
		{
			get 
			{			
				
				//				mAllChannels.Sort();
				for(int x = 1;x<11;x++)
				{
					mAllChannels.Add(x);
				}
				return((TVChannel[])mAllChannels.ToArray(typeof(TVChannel)));
			}
		}

		public TVProgramme[] AllShows
		{
			get 
			{ 
				BuildList_AllShows();
				return((TVProgramme[])mAllShows.ToArray(typeof(TVProgramme)));
			}
		}

		#endregion

		#region public playScheduleOrganizer()
		public playScheduleOrganizer()
		{
			playSchedules.Clear();
		}
		#endregion

		#region "Public Methods"
		public void Add(playScheduleArrayList list)
		{
			foreach(playSchedule show in list)
			{
//				if(IsConflict(show)==false)//add to play schedule
//				{
					if (playSchedules.Contains(show)==false)
					{
						playSchedules.Add(show);
					}
//				}
//				else//add to notifications
//				{
//					Console.WriteLine("need to add to notifications");
////					playSchedules.g
//					conflictDisplay conflictForm=new conflictDisplay(playSchedules[x],show);
//					conflictForm.Show();
//					conflictForm.Closed += new EventHandler(conflictForm_Closed);
//				}
			}
			MarkConflicts();
		}

		public void Remove(playScheduleArrayList list)
		{
			foreach(playSchedule show in list)
			{
				playSchedules.Remove(show);
//				playSchedules.Remove((playSchedule)show);
				Console.WriteLine("removing show");
			}

			MarkConflicts();
			if (OnListChanged!=null)
				OnListChanged();
		}

		public void LoadplaySchedules(string filename)
		{
			XmlSerializer serializer=new XmlSerializer(typeof(playScheduleArrayList)); 
			FileStream fs=new FileStream(filename, FileMode.Open);

			playSchedules=(playScheduleArrayList)serializer.Deserialize(fs);
			playSchedules.Sort();
			fs.Close();

			if (OnListChanged!=null)
				OnListChanged();
		}

		public void SaveplaySchedules(string filename)
		{
			XmlSerializer serializer=new XmlSerializer(typeof(playScheduleArrayList));
			TextWriter writer=new StreamWriter(filename);

			RemoveExpiredplaySchedules();
			playSchedules.Sort();
			serializer.Serialize(writer,playSchedules);
			writer.Close();
		}

		public void MarkConflicts()
		{
			playSchedules.Sort();
			NumConflicts=0;
			if (playSchedules.Count==1)
			{
				playSchedules[0].Conflicts=false;
				playSchedules[0].index=0;
				TVProgramme theProg = playSchedules[0].theProgramme;
			}
			else
			{
				for (int x=0; x<playSchedules.Count-1; x++)
				{
					
					if (IsConflict(playSchedules[x],playSchedules[x+1])==true)
					{
						conflictDisplay conflictForm=new conflictDisplay(playSchedules[x],playSchedules[x+1]);
						conflictForm.Show();
						conflictForm.Closed += new EventHandler(conflictForm_Closed);


//						TVProgramme theProg = playSchedules[x+1].theProgramme;
//						playSchedules[x].Conflicts=true;
//						playSchedules[x+1].Conflicts=true;
						NumConflicts=0;
//						playSchedules[x+1].index = NumConflicts;
//						Console.WriteLine("setting index2: "+ NumConflicts.ToString() +" "+ theProg.Title);
					}
					else
					{
						TVProgramme theProg = playSchedules[x].theProgramme;
//						playSchedules[x].Conflicts=false;
//						playSchedules[x].index = 0;
						playSchedules[x+1].Conflicts=false;
//						Console.WriteLine("setting index: 0 "+ theProg.Title);
						Console.WriteLine("no Conflict");
						NumConflicts=0;
					}
				}
			}
			if (NumConflicts==0)
				mConflictsExist=false;
			else
				mConflictsExist=true;

			if (OnListChanged!=null)
				OnListChanged();
		}
		#endregion

		#region "Private Helper Methods"
		//Adds only one show before looping through to find conflicts
		private void Add(playSchedule show)
		{
			//Don't allow duplicates
			if (playSchedules.Contains(show)==false)
			{
				playSchedules.Add(show);
			}
		}

		private void Remove(playSchedule show)
		{
			Console.WriteLine("found remove 2");
			TVProgramme prog = (TVProgramme)show.theProgramme;
			Console.WriteLine("removing show2 "+ prog.Title);
			playSchedules.Remove((playSchedule)show);
		}

		private void RemoveExpiredplaySchedules()
		{
			playScheduleArrayList ShowsToRemove=new playScheduleArrayList();

			foreach(playSchedule show in playSchedules)
			{
				if (show.Occurs==playSchedule.ERecurranceTypes.OneTimeOnly && show.StartTime<System.DateTime.Now)
					ShowsToRemove.Add(show);
			}

			foreach(playSchedule show in ShowsToRemove)
			{
				playSchedules.Remove(show);
			}
		}
		
		private bool AreTimesEqual_IgnoreSeconds(DateTime first, DateTime second)
		{
			if (first.TimeOfDay.Hours==second.TimeOfDay.Hours && first.TimeOfDay.Minutes==second.TimeOfDay.Minutes)
				return(true);
			else
				return(false);
		}

		private bool IsConflict(playSchedule show)
		{
			foreach(playSchedule ps in this.playSchedules)
			{
				if(IsConflict(show,ps)==true)
					return(true);
			}
			return(false);
		}

		private bool IsConflict(playSchedule playSchedule1, playSchedule playSchedule2)
		{
			TVProgramme show1 = (TVProgramme)playSchedule1.theProgramme;
			TVProgramme show2 = (TVProgramme)playSchedule2.theProgramme;
			//one time only Conflict
			if (show1.StartTime==show2.StartTime)
				return(true);

			//if either show occurs daily then compare times and ignore the date
			if (playSchedule1.Occurs==playSchedule.ERecurranceTypes.Daily || playSchedule2.Occurs==playSchedule.ERecurranceTypes.Daily)
			{
				if (AreTimesEqual_IgnoreSeconds(playSchedule1.StartTime,playSchedule2.StartTime)==true)
					return(true);
			}

			//if either show occurs weekly then compare times and the specific day and ignore the date
			if (playSchedule1.Occurs==playSchedule.ERecurranceTypes.Weekly || playSchedule2.Occurs==playSchedule.ERecurranceTypes.Weekly)
			{
				if (AreTimesEqual_IgnoreSeconds(playSchedule1.StartTime,playSchedule2.StartTime)==true && playSchedule1.StartTime.DayOfWeek==playSchedule2.StartTime.DayOfWeek)
					return(true);
			}

			//if either show occurs monthly then compare times and the specific date and ignore the day
			if (playSchedule1.Occurs==playSchedule.ERecurranceTypes.Monthly || playSchedule2.Occurs==playSchedule.ERecurranceTypes.Monthly)
			{
				if (AreTimesEqual_IgnoreSeconds(playSchedule1.StartTime,playSchedule2.StartTime)==true && playSchedule1.StartTime.Day==playSchedule2.StartTime.Day)
					return(true);
			}

			//if either show occurs yearly then compare times and the specific date and ignore the year
			if (playSchedule1.Occurs==playSchedule.ERecurranceTypes.Yearly || playSchedule2.Occurs==playSchedule.ERecurranceTypes.Yearly)
			{
				if (AreTimesEqual_IgnoreSeconds(playSchedule1.StartTime,playSchedule2.StartTime)==true && playSchedule1.StartTime.DayOfYear==playSchedule2.StartTime.DayOfYear)
					return(true);
			}

			return(false);
		}

		private void BuildList_AllShows()
		{
			mAllShows.Clear();
			foreach(playSchedule show in playSchedules)
			{
				mAllShows.Add(show.theProgramme);
			}
		}
		#endregion

		private void conflictForm_Closed(object sender, EventArgs e)
		{
			System.Windows.Forms.Form conflictForm = (System.Windows.Forms.Form)sender;
			playSchedule ps = (playSchedule)conflictForm.Tag;
			TVProgramme prog = (TVProgramme)ps.theProgramme;
			Console.WriteLine("+++  deleting play schedule "+ ps.index.ToString() +" "+ prog.Title);
			Console.WriteLine("contains: "+ playSchedules.Contains(ps));
			playSchedules.Remove(ps);
			Console.WriteLine("count: "+ playSchedules.Count.ToString());
			foreach(playSchedule pSchedule in playSchedules)
			{
				TVProgramme show = (TVProgramme)pSchedule.theProgramme;
				Console.WriteLine("  show: "+ show.Title);
			}
//			playSchedules.Remove((playSchedule)conflictForm.Tag);
			
//			foreach(playSchedule pSchedule in playSchedules)
//			{
//				if (IsConflict(pSchedule,ps)==true)//this is the playSchedule to delete and add to notification schedule
//				{
//					TVProgramme prog2 = (TVProgramme)pSchedule.theProgramme;
//					Console.WriteLine("found schedule to delete: "+ prog2.Title);
//					playSchedules.Remove(pSchedule);
////					playScheduleArrayList psAL = new playScheduleArrayList();
////					psAL.Add(pSchedule);
////					this.Remove(psAL);
//					break;
//				}
//			}
			
//			ps.Conflicts = true;
//			playScheduleArrayList psAL = new playScheduleArrayList();
//			psAL.Add(ps);
//			
//			this.Remove(psAL);
//			playSchedules.Remove(ps);
		}
	}
}
