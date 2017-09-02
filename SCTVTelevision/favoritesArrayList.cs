using System;
using System.Collections;
using System.Data;

namespace SCTVTelevision
{
	/// <summary>
	/// Used to store show play times. The primary purpose of this class is to allow storing of show play schedules and to eliminate the need to constantly cast objects
	/// to the proper type. With this class the favorites object can be used natively.
	/// </summary>
	public class favoritesArrayList : ArrayList
	{
		public favoritesArrayList():base()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public new favorites this[int index]  
		{
			get  
			{				
				return((favorites)base[index]);
			}
			set  
			{
				base[index]=value;
			}
		}

		#region "Public Overloaded Methods"
		public int Add(favorites value)
		{
			return(base.Add(value));
		}

		public void Remove(favorites obj)
		{
//			Console.WriteLine("contains: "+ base.Contains(obj).ToString());
			base.Remove(obj);
		}

		public bool Contains(favorites item)
		{
			return(base.Contains(item));
		}

		public int IndexOf(favorites value)
		{
			return(base.IndexOf (value));
		}
		#endregion

		#region "Public Overridden Methods"
		public override void Sort()
		{
			favoritesComparer comp=new favoritesComparer();
			base.Sort(0,this.Count,comp);
		}
		#endregion
	}
}
