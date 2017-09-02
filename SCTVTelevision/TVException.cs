using System;
using System.Windows.Forms;

namespace SCTVTelevision
{
	/// <summary>
	/// Summary description for TVException.
	/// </summary>
	public class TVException : ApplicationException
	{
		public TVException(string Message):base(Message)
		{
		}

		public TVException(string Message,Exception inner):base(Message,inner)
		{
		}

		#region "Properties"
		public string Version
		{
			get { return(Application.ProductVersion); }
		}
		/*
		public ConfigurationData Configuration
		{
			get { return(TVGuideMainForm.ProgramConfiguration); }
		}*/
		#endregion
	}
}
