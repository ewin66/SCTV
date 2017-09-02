using System;
using System.Collections;
using System.Data;

namespace SCTVTelevision
{
	/// <summary>
	/// Generic app wide comparer class used to compare various objects used througout the program
	/// </summary>
	public class TVComparer : IComparer
	{
		public enum eMediaSortBy { title, description, rating, stars };
		public enum ESortBy { StartTime, StopTime, Title, Channel };
		public enum ESortMode { Ascending, Descending };

		private eMediaSortBy mediaSortBy;
		private ESortBy mSortBy;
		private ESortMode mSortMode;
		
		#region "Constructors"
		public TVComparer()
		{
			mSortBy=ESortBy.StartTime;
			mSortMode=ESortMode.Ascending;
		}
		#endregion

		#region "Properties"
		public ESortBy SortBy
		{
			get { return(mSortBy); }
			set { mSortBy=value; }
		}
		public ESortMode SortMode
		{
			get { return(mSortMode); }
			set { mSortMode=value; }
		}
		#endregion

		#region IComparer Members

		public int Compare(object x, object y)
		{			
			return(0);
		}

		public int Compare(TVProgramme x, TVProgramme y)
		{
			switch(mSortBy)
			{
				case ESortBy.StartTime:
					if (mSortMode==ESortMode.Ascending)
						return(y.StartTime.CompareTo(x.StartTime));
					else
						return(x.StartTime.CompareTo(y.StartTime));

				case ESortBy.StopTime:
					if (mSortMode==ESortMode.Ascending)
						return(y.StopTime.CompareTo(x.StopTime));
					else
						return(x.StopTime.CompareTo(y.StopTime));

				case ESortBy.Title:
					if (mSortMode==ESortMode.Ascending)
						return(y.Title.CompareTo(x.Title));
					else
						return(x.Title.CompareTo(y.Title));

				case ESortBy.Channel:
					if (mSortMode==ESortMode.Ascending)
						return(y.Channel.CompareTo(x.Channel));
					else
						return(x.Channel.CompareTo(y.Channel));
			}
			return(0);
		}

		public int Compare(DataRow x, DataRow y)
		{
			switch(mediaSortBy)
			{
				case eMediaSortBy.title:
					if (mSortMode==ESortMode.Ascending)
						return(y["title"].ToString().CompareTo(x["title"].ToString()));
					else
						return(x["title"].ToString().CompareTo(y["title"].ToString()));

				case eMediaSortBy.description:
					if (mSortMode==ESortMode.Ascending)
						return(y["description"].ToString().CompareTo(x["description"].ToString()));
					else
						return(x["description"].ToString().CompareTo(y["description"].ToString()));

				case eMediaSortBy.rating:
					if (mSortMode==ESortMode.Ascending)
						return(y["rating"].ToString().CompareTo(x["rating"].ToString()));
					else
						return(x["rating"].ToString().CompareTo(y["rating"].ToString()));

				case eMediaSortBy.stars:
					if (mSortMode==ESortMode.Ascending)
						return(y["stars"].ToString().CompareTo(x["stars"].ToString()));
					else
						return(x["stars"].ToString().CompareTo(y["stars"].ToString()));
			}
			return(0);
		}

		public int Compare(TVChannel x, TVChannel y)
		{
			switch(mSortBy)
			{
				case ESortBy.Channel:
					if (mSortMode==ESortMode.Ascending)
						return(x.DisplayName.CompareTo(y.DisplayName));
					else
						return(y.DisplayName.CompareTo(x.DisplayName));

				/*case ESortBy.ChannelName:
					if (mSortMode==ESortMode.Ascending)
						return(x.Name.CompareTo(y.Name));
					else
						return(y.Name.CompareTo(x.Name));*/
			}
			return(0);
		}
		#endregion
	}
}
