using System;
using System.Collections;
using System.Data;

namespace SCTVTelevision
{
	/// <summary>
	/// Used to store show play times. The primary purpose of this class is to allow storing of show play schedules and to eliminate the need to constantly cast objects
	/// to the proper type. With this class the playSchedule object can be used natively.
	/// </summary>
	public class playScheduleArrayList : ArrayList
	{
		public playScheduleArrayList():base()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public new playSchedule this[int index]  
		{
			get  
			{				
				return((playSchedule)base[index]);
			}
			set  
			{
				base[index]=value;
			}
		}

		#region "Public Overloaded Methods"
		public int Add(playSchedule value)
		{
			return(base.Add(value));
		}

		public void Remove(playSchedule obj)
		{
			Console.WriteLine("contains: "+ base.Contains(obj).ToString());
			base.Remove(obj);
		}

		public bool Contains(playSchedule item)
		{
			return(base.Contains(item));
		}

		public int IndexOf(playSchedule value)
		{
			return(base.IndexOf (value));
		}
		#endregion

		#region "Public Overridden Methods"
		public override void Sort()
		{
			playScheduleComparer comp=new playScheduleComparer();
			base.Sort(0,this.Count,comp);
		}
		#endregion
	}
}
