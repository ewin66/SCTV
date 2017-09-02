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
	public class favoritesOrganizer
	{
		#region Variables
		private favoritesArrayList favoritess=new favoritesArrayList();
		private bool mConflictsExist;
		private ArrayList mAllChannels=new ArrayList();
		private ArrayList mAllShows=new ArrayList();
		public int NumConflicts=0;
		#endregion

		#region "Delegates"
		public delegate void favoritesListChanged();
		public event favoritesListChanged OnListChanged;
		#endregion

		#region "Properties"
		public bool ConflictsExist
		{
			get { return(mConflictsExist); }
		}

		public favoritesArrayList GetList
		{
			get { return(favoritess); }
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

		public favorites[] AllShows
		{
			get 
			{ 
				BuildList_AllShows();
				return((favorites[])mAllShows.ToArray(typeof(favorites)));
			}
		}

		#endregion

		#region public favoritesOrganizer()
		public favoritesOrganizer()
		{
			favoritess.Clear();
		}
		#endregion

		#region public bool isFavorite(TVProgramme show)
		public bool isFavorite(TVProgramme show)
		{
			favorites theShow=new favorites((TVProgramme)show);
			if(favoritess.Contains(theShow))
			{
				return favoritess.Contains(theShow);
			}
			else
			{
				foreach(favorites fav in favoritess)
				{
					if(theShow.title == fav.title)
						return true;
//					else
//						Console.WriteLine(theShow.title +" doesn't equal "+ fav.title);
				}
			}
			return false;
		}
		#endregion

		#region "Public Methods"
		public void Add(favoritesArrayList list)
		{
			foreach(favorites show in list)
			{
				//				if(IsConflict(show)==false)//add to play schedule
				//				{
				Console.WriteLine("favoritess.Contains(show)  "+ favoritess.Contains(show).ToString());
				if (favoritess.Contains(show)==false)
				{
					favoritess.Add(show);
				}
				//				}
				//				else//add to notifications
				//				{
				//					Console.WriteLine("need to add to notifications");
				////					favoritess.g
				//					conflictDisplay conflictForm=new conflictDisplay(favoritess[x],show);
				//					conflictForm.Show();
				//					conflictForm.Closed += new EventHandler(conflictForm_Closed);
				//				}
			}
//			MarkConflicts();
		}

		public void Remove(favoritesArrayList list)
		{
			foreach(favorites show in list)
			{
//				foreach(favorites fav in favoritess)
//				{
//					if(fav.title == show.title)//this is the favorite to remove
//					{
						this.Remove(show);
						Console.WriteLine("removing show "+ show.title);
//					}
//				}
			}

//			MarkConflicts();
			if (OnListChanged!=null)
				OnListChanged();
		}

		#region public void Loadfavoritess(string filename)
		public void Loadfavoritess(string filename)
		{
			XmlSerializer serializer=new XmlSerializer(typeof(favoritesArrayList)); 
			FileStream fs=new FileStream(filename, FileMode.Open);

			favoritess=(favoritesArrayList)serializer.Deserialize(fs);
			favoritess.Sort();
			fs.Close();
			if (OnListChanged!=null)
				OnListChanged();
		}
		#endregion

		#region public void Savefavoritess(string filename)
		public void Savefavoritess(string filename)
		{
			XmlSerializer serializer=new XmlSerializer(typeof(favoritesArrayList));
			TextWriter writer=new StreamWriter(filename);

			RemoveExpiredfavoritess();
			favoritess.Sort();
			serializer.Serialize(writer,favoritess);
			writer.Close();
		}
		#endregion

		#region public void MarkConflicts()
		public void MarkConflicts()
		{
			favoritess.Sort();
			NumConflicts=0;
			if (favoritess.Count==1)
			{
				favoritess[0].Conflicts=false;
//				favoritess[0].index=0;
//				TVProgramme theProg = favoritess[0].theProgramme;
			}
			else
			{
//				for (int x=0; x<favoritess.Count-1; x++)
//				{
//					
//					if (IsConflict(favoritess[x],favoritess[x+1])==true)
//					{
//						conflictDisplay conflictForm=new conflictDisplay(favoritess[x],favoritess[x+1]);
//						conflictForm.Show();
//						conflictForm.Closed += new EventHandler(conflictForm_Closed);
//
//
//						//						TVProgramme theProg = favoritess[x+1].theProgramme;
//						//						favoritess[x].Conflicts=true;
//						//						favoritess[x+1].Conflicts=true;
//						NumConflicts=0;
//						//						favoritess[x+1].index = NumConflicts;
//						//						Console.WriteLine("setting index2: "+ NumConflicts.ToString() +" "+ theProg.Title);
//					}
//					else
//					{
//						TVProgramme theProg = favoritess[x].theProgramme;
//						//						favoritess[x].Conflicts=false;
//						//						favoritess[x].index = 0;
//						favoritess[x+1].Conflicts=false;
//						//						Console.WriteLine("setting index: 0 "+ theProg.Title);
//						Console.WriteLine("no Conflict");
//						NumConflicts=0;
//					}
//				}
			}
			if (NumConflicts==0)
				mConflictsExist=false;
			else
				mConflictsExist=true;

			if (OnListChanged!=null)
				OnListChanged();
		}
		#endregion
		#endregion

		#region "Private Helper Methods"
		//Adds only one show at a time
		private void Add(favorites show)
		{
			//Don't allow duplicates
//			if (favoritess.Contains(show)==false)
//			{
//				favoritess.Add(show);
//			}
			bool isDuplicate = false;
			foreach(favorites fav in favoritess)
			{
				if(show.title == fav.title)//this is a duplicate
				{
					isDuplicate = true;
					break;
				}
			}
			if(!isDuplicate)
				favoritess.Add(show);
		}

		private void Remove(favorites show)
		{
			int counter = 0;
			foreach(favorites fav in favoritess)
			{
				if(show.title == fav.title)//this is the one to remove
				{
					favoritess.RemoveAt(counter);
					break;
				}
				counter++;
			}
		}

		private void RemoveExpiredfavoritess()
		{
			favoritesArrayList ShowsToRemove=new favoritesArrayList();

//			foreach(favorites show in favoritess)
//			{
//				if (show.Occurs==favorites.ERecurranceTypes.OneTimeOnly && show.StartTime<System.DateTime.Now)
//					ShowsToRemove.Add(show);
//			}

			foreach(favorites show in ShowsToRemove)
			{
				favoritess.Remove(show);
			}
		}
		
		private bool AreTimesEqual_IgnoreSeconds(DateTime first, DateTime second)
		{
			if (first.TimeOfDay.Hours==second.TimeOfDay.Hours && first.TimeOfDay.Minutes==second.TimeOfDay.Minutes)
				return(true);
			else
				return(false);
		}

		private bool IsConflict(favorites show)
		{
			foreach(favorites ps in this.favoritess)
			{
				if(IsConflict(show,ps)==true)
					return(true);
			}
			return(false);
		}

		private bool IsConflict(favorites favorites1, favorites favorites2)
		{
//			TVProgramme show1 = (TVProgramme)favorites1.theProgramme;
//			TVProgramme show2 = (TVProgramme)favorites2.theProgramme;
			//one time only Conflict
//			if (show1.StartTime==show2.StartTime)
//				return(true);

			//if either show occurs daily then compare times and ignore the date
//			if (favorites1.Occurs==favorites.ERecurranceTypes.Daily || favorites2.Occurs==favorites.ERecurranceTypes.Daily)
//			{
//				if (AreTimesEqual_IgnoreSeconds(favorites1.StartTime,favorites2.StartTime)==true)
//					return(true);
//			}

			//if either show occurs weekly then compare times and the specific day and ignore the date
//			if (favorites1.Occurs==favorites.ERecurranceTypes.Weekly || favorites2.Occurs==favorites.ERecurranceTypes.Weekly)
//			{
//				if (AreTimesEqual_IgnoreSeconds(favorites1.StartTime,favorites2.StartTime)==true && favorites1.StartTime.DayOfWeek==favorites2.StartTime.DayOfWeek)
//					return(true);
//			}

			//if either show occurs monthly then compare times and the specific date and ignore the day
//			if (favorites1.Occurs==favorites.ERecurranceTypes.Monthly || favorites2.Occurs==favorites.ERecurranceTypes.Monthly)
//			{
//				if (AreTimesEqual_IgnoreSeconds(favorites1.StartTime,favorites2.StartTime)==true && favorites1.StartTime.Day==favorites2.StartTime.Day)
//					return(true);
//			}

			//if either show occurs yearly then compare times and the specific date and ignore the year
//			if (favorites1.Occurs==favorites.ERecurranceTypes.Yearly || favorites2.Occurs==favorites.ERecurranceTypes.Yearly)
//			{
//				if (AreTimesEqual_IgnoreSeconds(favorites1.StartTime,favorites2.StartTime)==true && favorites1.StartTime.DayOfYear==favorites2.StartTime.DayOfYear)
//					return(true);
//			}

			return(false);
		}

		private void BuildList_AllShows()
		{
			mAllShows.Clear();
			foreach(favorites show in favoritess)
			{
//				mAllShows.Add(show.theProgramme);
			}
		}
		#endregion

		#region private void conflictForm_Closed(object sender, EventArgs e)
		private void conflictForm_Closed(object sender, EventArgs e)
		{
			System.Windows.Forms.Form conflictForm = (System.Windows.Forms.Form)sender;
			favorites ps = (favorites)conflictForm.Tag;
//			TVProgramme prog = (TVProgramme)ps.theProgramme;
//			Console.WriteLine("+++  deleting play schedule "+ ps.index.ToString() +" "+ prog.Title);
			Console.WriteLine("contains: "+ favoritess.Contains(ps));
			favoritess.Remove(ps);
			Console.WriteLine("count: "+ favoritess.Count.ToString());
			foreach(favorites pSchedule in favoritess)
			{
//				TVProgramme show = (TVProgramme)pSchedule.theProgramme;
//				Console.WriteLine("  show: "+ show.Title);
			}
			//			favoritess.Remove((favorites)conflictForm.Tag);
			
			//			foreach(favorites pSchedule in favoritess)
			//			{
			//				if (IsConflict(pSchedule,ps)==true)//this is the favorites to delete and add to notification schedule
			//				{
			//					TVProgramme prog2 = (TVProgramme)pSchedule.theProgramme;
			//					Console.WriteLine("found schedule to delete: "+ prog2.Title);
			//					favoritess.Remove(pSchedule);
			////					favoritesArrayList psAL = new favoritesArrayList();
			////					psAL.Add(pSchedule);
			////					this.Remove(psAL);
			//					break;
			//				}
			//			}
			
			//			ps.Conflicts = true;
			//			favoritesArrayList psAL = new favoritesArrayList();
			//			psAL.Add(ps);
			//			
			//			this.Remove(psAL);
			//			favoritess.Remove(ps);
		}
		#endregion
	}
}
