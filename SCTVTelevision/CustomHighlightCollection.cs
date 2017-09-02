using System;
using System.Collections;

namespace SCTVTelevision
{
	/// <summary>
	/// Summary description for CustomHighlightCollection.
	/// </summary>
	public class CustomHighlightCollection : CollectionBase
	{
		public CustomHighlightCollection()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public CustomHighlight this[int index]  
		{
			get  
			{
				return((CustomHighlight)List[index]);
			}
			set  
			{
				List[index]=value;
			}
		}

		public int Add(CustomHighlight value)
		{
			return(List.Add(value ));
		}

		public int IndexOf(CustomHighlight value )  
		{
			return(List.IndexOf(value));
		}

		public void Insert(int index, CustomHighlight value)
		{
			List.Insert(index, value);
		}

		public void Remove(CustomHighlight value)
		{
			List.Remove(value);
		}

		public bool Contains(CustomHighlight value)
		{
			// If value is not of type CustomHighlight, this will return false.
			return( List.Contains(value));
		}

		#region "Internal Events"
		protected override void OnInsert(int index, Object value)  
		{
			if (value.GetType()!=Type.GetType("SCTV.CustomHighlight"))
				throw new ArgumentException("value must be of type CustomHighlight.", "value");
		}

		protected override void OnRemove(int index, Object value)
		{
			if (value.GetType()!=Type.GetType("SCTV.CustomHighlight"))
				throw new ArgumentException("value must be of type CustomHighlight.", "value");
		}

		protected override void OnSet(int index, Object oldValue, Object newValue)  
		{
			if (newValue.GetType()!=Type.GetType("SCTV.CustomHighlight"))
				throw new ArgumentException( "newValue must be of type CustomHighlight.", "newValue");
		}

		protected override void OnValidate(Object value)  
		{
			if (value.GetType()!=Type.GetType("SCTV.CustomHighlight"))
				throw new ArgumentException( "value must be of type CustomHighlight.");
		}
		#endregion
	}
}
