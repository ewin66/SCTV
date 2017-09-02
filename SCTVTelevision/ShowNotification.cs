using System;
using System.Xml;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Data;

namespace SCTVTelevision
{
	/// <summary>
	/// Summary description for ShowNotification.
	/// </summary>
	public class ShowNotification
	{
		public enum ERecurranceTypes
		{
			OneTimeOnly,
			EveryTimeItAirs,
			Daily,
			Weekly,
			Monthly,
			Yearly
		}

		private string mTitle;
		private string mDescription;
		private string mChannelID;
		private string mChannelName;
		private DateTime mStartTime;
		private ERecurranceTypes mShowOccurrences=ERecurranceTypes.OneTimeOnly;
		private bool mConflicts;
		private bool mAlreadyNotified; //Since we ignore seconds in the times, notifications could potentially be sent more than once, this flag prevents that

		#region "Properties"
		[CategoryAttribute("Show")]
		[DescriptionAttribute("The name of the show")]
		[ReadOnly(true)]
		public string name
		{
			get { return(mTitle); }
			set { mTitle=value; }
		}

		[CategoryAttribute("Show")]
		[DescriptionAttribute("The name of the show")]
		[ReadOnly(true)]
		public string description
		{
			get { return(mDescription); }
			set { mDescription=value; }
		}

		[CategoryAttribute("Show")]
		[DescriptionAttribute("The channel show will air on")]
		[ReadOnly(true)]
		public string channel
		{
			get { return(mChannelName); }
			set { mChannelName=value; }
		}

		[CategoryAttribute("Show")]
		[DescriptionAttribute("The time the show starts")]
		[ReadOnly(true)]
		public DateTime StartTime
		{
			get { return(mStartTime); }
			set { mStartTime=value; }
		}

		[CategoryAttribute("Show")]
		[DescriptionAttribute("If you want to be notified of this show more than once, set how often the show airs.")]
		public ERecurranceTypes Occurs
		{
			get { return(mShowOccurrences); }
			set { mShowOccurrences=value; }
		}

		[Browsable(false)]
		public string ChannelID
		{
			get { return(mChannelID); }
			set { mChannelID=value; }
		}

		[XmlIgnore()]
		[Browsable(false)]
		public bool Conflicts
		{
			get { return(mConflicts); }
			set { mConflicts=value; }
		}

		[XmlIgnore()]
		[Browsable(false)]
		public bool AlreadyNotified
		{
			get { return(mAlreadyNotified); }
			set { mAlreadyNotified=value; }
		}
		#endregion

		public ShowNotification()
		{
			mTitle="";
			mAlreadyNotified=false;
		}
		
		public ShowNotification(TVProgramme prog)
		{
			mTitle=prog.Title;
			mDescription = prog.Description;
			mChannelID=prog.Channel.ID;
			mChannelName=prog.Channel.DisplayName;
			mStartTime=prog.StartTime;
			mShowOccurrences=ERecurranceTypes.OneTimeOnly;
			mAlreadyNotified=false;
		}

		public bool IsStarting()
		{
			bool ShowStarting=false;
			DateTime Now=System.DateTime.Now;

			switch(mShowOccurrences)
			{
				case ERecurranceTypes.OneTimeOnly:
					if (mStartTime.Hour==Now.Hour && mStartTime.Minute==Now.Minute && mStartTime.Date==Now.Date)
						ShowStarting=true;
					else
						ShowStarting=false;
					break;

				case ERecurranceTypes.Daily:
					if (mStartTime.TimeOfDay.Hours==Now.TimeOfDay.Hours && mStartTime.TimeOfDay.Minutes==Now.TimeOfDay.Minutes)
						ShowStarting=true;
					else
						ShowStarting=false;
					break;

				case ERecurranceTypes.Weekly:
					if (mStartTime.Hour==Now.Hour && mStartTime.Minute==Now.Minute && mStartTime.DayOfWeek==Now.DayOfWeek)
						ShowStarting=true;
					else
						ShowStarting=false;
					break;

				case ERecurranceTypes.Monthly:
					if (mStartTime.Hour==Now.Hour && mStartTime.Minute==Now.Minute && mStartTime.Day==Now.Day)
						ShowStarting=true;
					else
						ShowStarting=false;
					break;

				case ERecurranceTypes.Yearly:
					if (mStartTime.Hour==Now.Hour && mStartTime.Minute==Now.Minute && mStartTime.DayOfYear==Now.DayOfYear)
						ShowStarting=true;
					else
						ShowStarting=false;
					break;
			}
//			Console.WriteLine("show starting "+ ShowStarting.ToString());
			return(ShowStarting);
		}

		public override bool Equals(object obj)
		{
			bool doesequal=false;

			if (!(obj is ShowNotification))
			{
				doesequal=false;
			}
			else
			{
				ShowNotification s=(ShowNotification)obj;

				if (s.Occurs==this.Occurs && s.StartTime==this.StartTime && s.ChannelID==this.ChannelID)
					doesequal=true;
				else
					doesequal=false;
			}

			return(doesequal);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString()
		{
			return base.ToString();
		}

		public static bool operator==(ShowNotification lhs, ShowNotification rhs)
		{
			return(lhs.Equals(rhs));
		}

		public static bool operator!=(ShowNotification lhs, ShowNotification rhs)
		{
			return(!lhs.Equals(rhs));
		}
	}
}
