using System;
using System.Collections;
using System.Windows.Forms;

namespace SCTVTelevision
{
	/// <summary>
	/// Summary description for ShowNotificationComparer.
	/// </summary>
	public class playScheduleComparer : IComparer
	{
		public playScheduleComparer()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		#region IComparer Members

		public int Compare(object x, object y)
		{
			playSchedule showx=(playSchedule)x;
			playSchedule showy=(playSchedule)y;
			return 0;
//			return(showx.StartTime.TimeOfDay.CompareTo(showy.StartTime.TimeOfDay));
		}

		#endregion
	}
}
