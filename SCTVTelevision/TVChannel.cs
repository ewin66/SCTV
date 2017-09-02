using System;
using System.Drawing;
using System.Web;
using System.Net;
using System.IO;
//using Microsoft.ApplicationBlocks.ExceptionManagement;

namespace SCTVTelevision
{
	/// <summary>
	/// Summary description for TVChannel.
	/// </summary>
	public class TVChannel : channelType, IComparable
	{
        private StringParser Parser=new StringParser();
		
		private string mDisplayName;
        private string mIconUrl;
        private string mID;
        private Image mIcon;

        #region "Properties"
        public string IconUrl
        {
            get { return(mIconUrl); }
        }
        public string ID
        {
            get { return(mID); }
        }
        public string DisplayName
        {
            get { return(mDisplayName); }
        }
        public Image Icon
        {
            get { return(mIcon); }
        }
        #endregion

		public TVChannel()
		{
			mDisplayName="";
            mIconUrl="";
            mID="";
		}

        public TVChannel(channelType chan)
        {
			mID=chan.id;
            if (chan.displayname!=null)
            {
				mDisplayName=chan.displayname[0].Value;
            }
			else mDisplayName="No Display Name";

            if (chan.icon!=null && chan.icon.Length>0)
            {
                mIconUrl=chan.icon[0].src;
            }
            else mIconUrl=null;
        }

        public void DownloadIcon()
        {
            if (mIconUrl!=null)
            {
                try
                {
                    WebRequest wreq=WebRequest.Create(mIconUrl);
                    HttpWebResponse httpResponse=(HttpWebResponse)wreq.GetResponse();
                    Stream stream=httpResponse.GetResponseStream();
                    mIcon=Image.FromStream(stream);

//                    mIcon.Save(Form1.ProgramConfiguration.IconDir+"\\"+mID+".icon");
                }
                catch(Exception ex)
                {
//                    ExceptionManager.Publish(ex);
                }
            }
        }

        public Image GetImage(string urlFile)
        {
            WebRequest wreq = WebRequest.Create( urlFile );
            HttpWebResponse httpResponse = (HttpWebResponse) wreq.GetResponse();
            Stream stream = httpResponse.GetResponseStream();
            return Image.FromStream(stream);
		}

		#region IComparable Members

		//Channels are always sorted by display name in ascending order
		/*public int CompareTo(object obj)
		{
			TVChannel rhs=(TVChannel)obj;
			return(this.DisplayName.CompareTo(rhs.DisplayName));
		}*/

		public int CompareTo(object obj)
		{
			TVChannel rhs=(TVChannel)obj;

			// Try to extract a numeric prefix on this
			int me = 0, it = 0;
			try
			{
				int i = this.DisplayName.IndexOf(' ', 0);
				string mePrefix = this.DisplayName.Substring(0, i);
				me = Convert.ToInt32(mePrefix);
			}
			// catch any errors that occur in conversion, 
			// and set numeric value to 0
			catch(Exception exc)
			{
//				ExceptionManager.Publish(exc);
				me = 0;
			}

			// Try to extract a numeric prefix on the object passed in
			try
			{
				int i = rhs.DisplayName.IndexOf(' ', 0);
				string itPrefix = rhs.DisplayName.Substring(0, i);
				it = Convert.ToInt32(itPrefix);
			}
				// catch any errors that occur in conversion, 
				// and set numeric value to 0
			catch(Exception exc)
			{
//				ExceptionManager.Publish(exc);
				it = 0;
			}

			// If both have numeric prefixes, compare those
			if(me > 0 && it > 0)
				return me.CompareTo(it);

				// If neither have numeric prefixes, do a string compare
			else if(me == 0 && it == 0)
				return(this.DisplayName.CompareTo(rhs.DisplayName));

				// If only one has a numeric prefix, put it first
			else
				return it.CompareTo(me);
		}

		#endregion
	}
}
