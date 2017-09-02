using System;

namespace SCTVTelevision
{
	/// <summary>
	/// Summary description for DataReceivedEventArgs.
	/// </summary>
	public class DataReceivedEventArgs : EventArgs
	{
		public string Data;

		public DataReceivedEventArgs(string s)
		{
			Data=s;
		}
	}
}
