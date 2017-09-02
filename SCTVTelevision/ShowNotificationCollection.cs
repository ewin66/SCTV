using System;
using System.Collections;

namespace SCTVTelevision
{
	/// <summary>
	/// Summary description for ShowNotificationCollection.
	/// </summary>
	public class ShowNotificationCollection : CollectionBase
	{
		public ShowNotificationCollection()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public ShowNotification this[int index]  
		{
			get  
			{
				return((ShowNotification)List[index]);
			}
			set  
			{
				List[index]=value;
			}
		}

		public int Add(ShowNotification value)
		{
			return(List.Add(value));
		}

		public int IndexOf(ShowNotification value )  
		{
			return(List.IndexOf(value));
		}

		public void Insert(int index, ShowNotification value)
		{
			List.Insert(index, value);
		}

		public void InsertBefore(ShowNotification value)
		{
			if (Contains(value)==true)
			{
				int x=IndexOf(value);
				Insert(x-1,value);
			}
		}

		public void InsertAfter(ShowNotification value)
		{
			if (Contains(value)==true)
			{
				int x=IndexOf(value);
				Insert(x+1,value);
			}
		}

		public void Remove(ShowNotification value)
		{
			List.Remove(value);
		}

		public bool Contains(ShowNotification value)
		{
			// If value is not of type ShowNotification, this will return false.
			return( List.Contains(value));
		}

		private bool IsConflict(ShowNotification show1, ShowNotification show2)
		{
			//one time only Conflict
			if (show1.StartTime==show2.StartTime)
				return(true);

			//if either show occurs daily then compare times and ignore the date
			if (show1.Occurs==ShowNotification.ERecurranceTypes.Daily || show2.Occurs==ShowNotification.ERecurranceTypes.Daily)
			{
				if (show1.StartTime.TimeOfDay==show2.StartTime.TimeOfDay)
					return(true);
			}

			//if either show occurs weekly then compare times and the specific day and ignore the date
			if (show1.Occurs==ShowNotification.ERecurranceTypes.Weekly || show2.Occurs==ShowNotification.ERecurranceTypes.Weekly)
			{
				if (show1.StartTime.TimeOfDay==show2.StartTime.TimeOfDay && show1.StartTime.DayOfWeek==show2.StartTime.DayOfWeek)
					return(true);
			}

			//if either show occurs monthly then compare times and the specific date and ignore the day
			if (show1.Occurs==ShowNotification.ERecurranceTypes.Monthly || show2.Occurs==ShowNotification.ERecurranceTypes.Monthly)
			{
				if (show1.StartTime.TimeOfDay==show2.StartTime.TimeOfDay && show1.StartTime.Day==show2.StartTime.Day)
					return(true);
			}

			//if either show occurs yearly then compare times and the specific date and ignore the year
			if (show1.Occurs==ShowNotification.ERecurranceTypes.Yearly || show2.Occurs==ShowNotification.ERecurranceTypes.Yearly)
			{
				if (show1.StartTime.TimeOfDay==show2.StartTime.TimeOfDay && show1.StartTime.DayOfYear==show2.StartTime.DayOfYear)
					return(true);
			}

			return(false);
		}

		#region "Internal Events"
		protected override void OnInsert(int index, Object value)  
		{
			if (value.GetType()!=Type.GetType("TVGuide.ShowNotification"))
				throw new ArgumentException("value must be of type ShowNotification.", "value");
		}

		protected override void OnRemove(int index, Object value)
		{
			if (value.GetType()!=Type.GetType("TVGuide.ShowNotification"))
				throw new ArgumentException("value must be of type ShowNotification.", "value");
		}

		protected override void OnSet(int index, Object oldValue, Object newValue)  
		{
			if (newValue.GetType()!=Type.GetType("TVGuide.ShowNotification"))
				throw new ArgumentException( "newValue must be of type ShowNotification.", "newValue");
		}

		protected override void OnValidate(Object value)  
		{
			if (value.GetType()!=Type.GetType("TVGuide.ShowNotification"))
				throw new ArgumentException( "value must be of type ShowNotification.");
		}
		#endregion
	}
}
