using System;
using System.Collections;
using System.Windows.Forms;

namespace SCTVTelevision
{
	/// <summary>
	/// Summary description for ShowNotificationComparer.
	/// </summary>
	public class ShowNotificationComparer : IComparer
	{
		public ShowNotificationComparer()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		#region IComparer Members

		public int Compare(object x, object y)
		{
			ShowNotification showx=(ShowNotification)x;
			ShowNotification showy=(ShowNotification)y;

			return(showx.StartTime.TimeOfDay.CompareTo(showy.StartTime.TimeOfDay));
		}

		#endregion
	}
}
