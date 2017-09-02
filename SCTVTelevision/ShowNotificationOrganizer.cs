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
	public class ShowNotificationOrganizer
	{
		private ShowNotificationArrayList Notifications=new ShowNotificationArrayList();
		private bool mConflictsExist;

		#region "Delegates"
		public delegate void showNotificationListChanged();
		public event showNotificationListChanged OnListChanged;
		#endregion

		#region "Properties"
		public bool ConflictsExist
		{
			get { return(mConflictsExist); }
		}

		public ShowNotificationArrayList GetList
		{
			get { return(Notifications); }
		}
		#endregion

		public ShowNotificationOrganizer()
		{
			Notifications.Clear();
		}

		#region "Public Methods"
		public void Add(ShowNotificationArrayList list)
		{
//			Console.WriteLine("list count "+ list.Count.ToString());
			foreach(ShowNotification show in list)
			{
//				Console.WriteLine("contains "+ Convert.ToString(Notifications.Contains(show)==false));
				if (Notifications.Contains(show)==false)
				{
					Notifications.Add(show);
//					Console.WriteLine("adding show to notifications");
				}
			}
//			Console.WriteLine("list count2 "+ list.Count.ToString());
			MarkConflicts();
		}

		public void Remove(ShowNotificationArrayList list)
		{
			foreach(ShowNotification show in list)
			{
				Notifications.Remove(show);
			}

			MarkConflicts();
		}

		public void LoadShowNotifications(string filename)
		{
			XmlSerializer serializer=new XmlSerializer(typeof(ShowNotificationArrayList)); 
			FileStream fs=new FileStream(filename, FileMode.Open);

			Notifications=(ShowNotificationArrayList)serializer.Deserialize(fs);
			Notifications.Sort();
			fs.Close();

			if (OnListChanged!=null)
				OnListChanged();
		}

		public void SaveShowNotifications(string filename)
		{
			XmlSerializer serializer=new XmlSerializer(typeof(ShowNotificationArrayList));
			TextWriter writer=new StreamWriter(filename);

			RemoveExpiredNotifications();
			Notifications.Sort();
			serializer.Serialize(writer,Notifications);
			writer.Close();
		}

		public void MarkConflicts()
		{
			int NumConflicts=0;

			Notifications.Sort();
			if (Notifications.Count==1)
			{
				NumConflicts=0;
				Notifications[0].Conflicts=false;
			}
			else
			{
				for (int x=0; x<Notifications.Count-1; x++)
				{
					if (IsConflict(Notifications[x],Notifications[x+1])==true)
					{
						Notifications[x].Conflicts=true;
						Notifications[x+1].Conflicts=true;
						NumConflicts++;
					}
					else
					{
						Notifications[x].Conflicts=false;
						Notifications[x+1].Conflicts=false;
					}
				}
			}
			if (NumConflicts==0)
				mConflictsExist=false;
			else
				mConflictsExist=true;

//			Console.WriteLine("OnListChanged "+ Convert.ToString(OnListChanged!=null));
			if (OnListChanged!=null)
				OnListChanged();
		}
		#endregion

		#region "Private Helper Methods"
		//Adds only one show before looping through to find conflicts
		private void Add(ShowNotification show)
		{
//			Console.WriteLine("adding show to Notifications again");
			//Don't allow duplicates
			if (Notifications.Contains(show)==false)
			{
				Notifications.Add(show);
			}
		}

		private void Remove(ShowNotification show)
		{
			Notifications.Remove(show);
		}

		private void RemoveExpiredNotifications()
		{
			ShowNotificationArrayList ShowsToRemove=new ShowNotificationArrayList();

			foreach(ShowNotification show in Notifications)
			{
				if (show.Occurs==ShowNotification.ERecurranceTypes.OneTimeOnly && show.StartTime<System.DateTime.Now)
					ShowsToRemove.Add(show);
			}

			foreach(ShowNotification show in ShowsToRemove)
			{
				Notifications.Remove(show);
			}
		}
		
		private bool AreTimesEqual_IgnoreSeconds(DateTime first, DateTime second)
		{
			if (first.TimeOfDay.Hours==second.TimeOfDay.Hours && first.TimeOfDay.Minutes==second.TimeOfDay.Minutes)
				return(true);
			else
				return(false);
		}

		private bool IsConflict(ShowNotification show1, ShowNotification show2)
		{
			//one time only Conflict
			if (show1.StartTime==show2.StartTime)
				return(true);

			//if either show occurs daily then compare times and ignore the date
			if (show1.Occurs==ShowNotification.ERecurranceTypes.Daily || show2.Occurs==ShowNotification.ERecurranceTypes.Daily)
			{
				if (AreTimesEqual_IgnoreSeconds(show1.StartTime,show2.StartTime)==true)
					return(true);
			}

			//if either show occurs weekly then compare times and the specific day and ignore the date
			if (show1.Occurs==ShowNotification.ERecurranceTypes.Weekly || show2.Occurs==ShowNotification.ERecurranceTypes.Weekly)
			{
				if (AreTimesEqual_IgnoreSeconds(show1.StartTime,show2.StartTime)==true && show1.StartTime.DayOfWeek==show2.StartTime.DayOfWeek)
					return(true);
			}

			//if either show occurs monthly then compare times and the specific date and ignore the day
			if (show1.Occurs==ShowNotification.ERecurranceTypes.Monthly || show2.Occurs==ShowNotification.ERecurranceTypes.Monthly)
			{
				if (AreTimesEqual_IgnoreSeconds(show1.StartTime,show2.StartTime)==true && show1.StartTime.Day==show2.StartTime.Day)
					return(true);
			}

			//if either show occurs yearly then compare times and the specific date and ignore the year
			if (show1.Occurs==ShowNotification.ERecurranceTypes.Yearly || show2.Occurs==ShowNotification.ERecurranceTypes.Yearly)
			{
				if (AreTimesEqual_IgnoreSeconds(show1.StartTime,show2.StartTime)==true && show1.StartTime.DayOfYear==show2.StartTime.DayOfYear)
					return(true);
			}

			return(false);
		}
		#endregion
	}
}
