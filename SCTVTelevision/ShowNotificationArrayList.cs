using System;
using System.Collections;
using System.Data;

namespace SCTVTelevision
{
	/// <summary>
	/// Used to store show notifications. The primary purpose of this class is to allow storing of show notifications and to eliminate the need to constantly cast objects
	/// to the proper type. With this class the ShowNotification object can be used natively.
	/// </summary>
	public class ShowNotificationArrayList : ArrayList
	{
		public ShowNotificationArrayList():base()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public new ShowNotification this[int index]  
		{
			get  
			{				
				return((ShowNotification)base[index]);
			}
			set  
			{
				base[index]=value;
			}
		}

		#region "Public Overloaded Methods"
		public int Add(ShowNotification value)
		{
			return(base.Add(value));
		}

		public void Remove(ShowNotification obj)
		{
			base.Remove(obj);
		}

		public bool Contains(ShowNotification item)
		{
			return(base.Contains(item));
		}

		public int IndexOf(ShowNotification value)
		{
			return(base.IndexOf (value));
		}
		#endregion

		#region "Public Overridden Methods"
		public override void Sort()
		{
			ShowNotificationComparer comp=new ShowNotificationComparer();
			base.Sort(0,this.Count,comp);
		}
		#endregion
	}
}
