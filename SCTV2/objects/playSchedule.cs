using System;
using System.Xml;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;

namespace SCTV
{
	/// <summary>
	/// Object that holds play time and channel along with all TVProgramme information.
	/// </summary>
	public class playSchedule
	{
		public enum ERecurranceTypes
		{
			OneTimeOnly,
			Daily,
			Weekly,
			Monthly,
			Yearly
		}

		private string mTitle;
		private string mDescription;
		private DateTime mStartTime;
		private ERecurranceTypes mShowOccurrences=ERecurranceTypes.OneTimeOnly;
		private bool mConflicts;
		private bool mAlreadyNotified; //Since we ignore seconds in the times, notifications could potentially be sent more than once, this flag prevents that
		private TVProgramme mProgramme;
		private int mIndex;

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
		[DescriptionAttribute("The description of the show")]
		[ReadOnly(true)]
		public string description
		{
			get { return(mDescription); }
			set { mDescription=value; }
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

		[CategoryAttribute("Show")]
		[DescriptionAttribute("The raw show object")]
		public TVProgramme theProgramme
		{
			get { return(mProgramme); }
			set { mProgramme=value; }
		}

		[CategoryAttribute("Show")]
		[DescriptionAttribute("The play index of this schedule")]
		public int index
		{
			get { return(mIndex); }
			set { mIndex=value; }
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

		public playSchedule()
		{
			mTitle="";
			mAlreadyNotified=false;
		}
		
		public playSchedule(TVProgramme prog)
		{
			mTitle=prog.Title;
			mDescription = prog.Description;
			mStartTime=prog.StartTime;
//			mStartTime=DateTime.Now;
			mShowOccurrences=ERecurranceTypes.OneTimeOnly;
			mAlreadyNotified=false;
			mProgramme = prog;
			mIndex = 0;
		}

		public bool IsStarting()
		{
			bool ShowStarting=false;
			DateTime Now=System.DateTime.Now;
			TVProgramme prog = mProgramme;
			switch(mShowOccurrences)
			{
				case ERecurranceTypes.OneTimeOnly:
					if (mStartTime.Hour==Now.Hour && mStartTime.Minute==Now.Minute && mStartTime.Date==Now.Date)
					{
						ShowStarting=true;
					}
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
				EventLog.WriteEntry("sctv","show starting "+ ShowStarting.ToString());
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
				playSchedule s=(playSchedule)obj;

				if (s.Occurs==this.Occurs && s.StartTime==this.StartTime)
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

		public static bool operator==(playSchedule lhs, playSchedule rhs)
		{
			return(lhs.Equals(rhs));
		}

		public static bool operator!=(playSchedule lhs, playSchedule rhs)
		{
			return(!lhs.Equals(rhs));
		}
	}
}
