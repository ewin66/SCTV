using System;
using System.Collections;
using System.Windows.Forms;

namespace SCTVTelevision
{
	/// <summary>
	/// Summary description for ShowNotificationComparer.
	/// </summary>
	public class favoritesComparer : IComparer
	{
		public favoritesComparer()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		#region IComparer Members

		public int Compare(object x, object y)
		{
			favorites showx=(favorites)x;
			favorites showy=(favorites)y;
			return 0;
//			return(showx.StartTime.TimeOfDay.CompareTo(showy.StartTime.TimeOfDay));
		}

		#endregion
	}
}
