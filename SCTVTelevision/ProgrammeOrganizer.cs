using System;
using System.IO;
using System.Net;
using System.Data;
using System.Xml;
using System.Xml.Serialization;
using System.Collections;
using Microsoft.ApplicationBlocks.ConfigurationManagement;
using Microsoft.ApplicationBlocks.ExceptionManagement;

namespace SCTVTelevision
{
	/// <summary>
	/// This class takes the raw array of programmes and builds a bunch of specialized arrays such as
	/// today's movies, tomorrow's movies, what's on right now, listings per channel, etc. These specialized
	/// arrays are stored and only updated when necessary/requested and used to populate the listview.
	/// This class will also optionally sort the data if a sorting method is selected
	/// </summary>
	public class ProgrammeOrganizer
	{	
		private tv Listings;
		XmlSerializer mySerializer=new XmlSerializer(typeof(tv));

		private Hashtable mChannelsHash=new Hashtable();

		private ArrayList mAllChannels=new ArrayList();
		private ArrayList mAllShows=new ArrayList();
		private ArrayList mAllCategories=new ArrayList();
		private ArrayList mMovies=new ArrayList(); //Stores all movies in master list
		private DateTime showTime = DateTime.Now;
		private string showChannel = "";
		private string timeType = "";
		public int channelMin = 10;
		public int channelMax = 1;

		#region "Delegates"
		public delegate void lisitngsListChanged();
		public event lisitngsListChanged OnListChanged;
		#endregion

		#region "Enumerations"
		public enum EFields
		{
			Title,
			Subtitle,
			Description,
			Category
		}
		#endregion

		#region "Properties"
		public TVProgramme[] AllShows
		{
			get 
			{			
				return((TVProgramme[])mAllShows.ToArray(typeof(TVProgramme)));
			}
		}
		
		public TVChannel[] AllChannels
		{
			get 
			{			
				mAllChannels.Sort();
				return((TVChannel[])mAllChannels.ToArray(typeof(TVChannel)));
			}
		}

		public string[] AllCategories
		{
			get 
			{	
				mAllCategories.Sort();
				return((string[])mAllCategories.ToArray(typeof(string)));
			}
		}

		public TVProgramme[] AllShowsWithinNDays
		{
			get 
			{ 
				return(BuildList_AllShowsWithinNDays(Form1.ProgramConfiguration.DaysToDisplay));
			}
		}

		public TVProgramme[] AllShowsWithinNHours
		{
			get 
			{ 
				return(BuildList_AllShowsWithinNHours(Form1.ProgramConfiguration.FewHoursFromNow));
			}
		}

		public TVProgramme[] MoviesAll
		{
			get
			{
				return((TVProgramme[])mMovies.ToArray(typeof(TVProgramme)));
			}
		}
		public TVProgramme[] MoviesToday
		{
			get
			{
				return(BuildList_MoviesByDate(System.DateTime.Now));
			}
		}
		public TVProgramme[] MoviesTomorrow
		{
			get
			{
				DateTime Tomorrow=System.DateTime.Now;
				Tomorrow=Tomorrow.AddDays(1);
				return(BuildList_MoviesByDate(Tomorrow));
			}
		}
		public TVProgramme[] ShowsRightNow
		{
			get
			{
				return(BuildList_ShowsRightNow());
			}
		}
		public TVProgramme[] ShowsOnAt
		{
			get
			{
				return(BuildList_ShowsOnAt(showTime,showChannel,timeType));
			}
		}
		#endregion

		#region "Constructors"
		public ProgrammeOrganizer()
		{
			mChannelsHash.Clear();
			mAllChannels.Clear();
			mAllShows.Clear();
			mMovies.Clear();

			mySerializer.UnknownAttribute+=new XmlAttributeEventHandler(XmlSerializer_UnknownAttribute);
			mySerializer.UnknownElement+=new XmlElementEventHandler(XmlSerializer_UnknownElement);
		}
		#endregion

		#region "XmlSerializer Events"
		private void XmlSerializer_UnknownAttribute(object sender, XmlAttributeEventArgs e)
		{
			//EventLog.WriteEntry("sctv","Unknown Attribute {0} {1} {2}",e.LineNumber,e.LinePosition,e.Attr);
		}
		private void XmlSerializer_UnknownElement(object sender, XmlElementEventArgs e)
		{
			//EventLog.WriteEntry("sctv","unknown element");
		}
		#endregion

		#region "Listing Load Methods"
		private void LoadListingsFromUrl(string location)
		{
			Stream myStream;
			WebClient myWebClient=new WebClient();

			//Clear Everything First
			mChannelsHash.Clear();
			mAllChannels.Clear();
			mAllShows.Clear();
			mMovies.Clear();

			myStream=myWebClient.OpenRead(location);
			Listings=(tv)mySerializer.Deserialize(myStream);

			myStream.Close();
		}

		public TimeSpan LoadListings(string url)
		{
			DateTime Start,Finish;
			try 
			{
				Start=System.DateTime.Now;
				LoadListingsFromUrl(url);
				Finish=System.DateTime.Now;
			}
			catch (Exception e) 
			{
				Exception TVEx=new TVException("Error encountered when trying to load the TV listings.",e);
				ExceptionManager.Publish(TVEx);

				throw(TVEx);
			}

			BuildList_AllChannels();
			BuildHash_AllChannels();

			BuildList_AllShows();
			BuildList_AllMovies();

			return(Finish-Start);
		}
		#endregion

		#region "Internal Array Building Methods"
		private void BuildList_AllChannels()
		{
			mAllChannels.Clear();
			foreach(channelType chtype in Listings.channel)
			{
				TVChannel Chan=new TVChannel(chtype);
				mAllChannels.Add(Chan);
				if(Convert.ToInt32(Chan.DisplayName.Substring(0,Chan.DisplayName.IndexOf(" "))) < channelMin)
					channelMin = Convert.ToInt32(Chan.DisplayName.Substring(0,Chan.DisplayName.IndexOf(" ")));
				if(Convert.ToInt32(Chan.DisplayName.Substring(0,Chan.DisplayName.IndexOf(" "))) > channelMax)
					channelMax = Convert.ToInt32(Chan.DisplayName.Substring(0,Chan.DisplayName.IndexOf(" ")));
			}
		}

		private void BuildHash_AllChannels()
		{
			foreach(TVChannel chan in mAllChannels)
			{
				mChannelsHash.Add(chan.ID,chan);
			}
		}

		private void BuildList_AllShows()
		{
			mAllShows.Clear();
			mAllCategories.Clear();
			foreach(programmeType pgtype in Listings.programme)
			{
				TVProgramme Prog=new TVProgramme(pgtype);
				Prog.Channel=(TVChannel)mChannelsHash[Prog.channel];
				mAllShows.Add(Prog);
				if(Prog.Categories.Length>0 && Prog.Categories.Trim()!="N/A")
				{
					foreach(categoryType catType in Prog.category)//add the categories to mAllCategories
					{
						if(!mAllCategories.Contains(catType.Value))//no duplicates
							mAllCategories.Add(catType.Value);
					}
				}
			}
		}

		#region private void BuildList_AllMovies()
		private void BuildList_AllMovies()
		{
			mMovies.Clear();
			foreach(TVProgramme Prog in mAllShows)
			{
//				EventLog.WriteEntry("sctv",Prog.Categories);
				//Movie highlights override custom highlights
				if (Prog.IsMovie())
				{
					Prog.BackColor=Form1.ProgramConfiguration.MovieColor;
					mMovies.Add(Prog);
				}
				else if(Prog.Categories.ToLower().IndexOf("news") >= 0)//this is news
				{	
					Prog.BackColor = Form1.ProgramConfiguration.NewsColor;
					
				}
				else if(Prog.Categories.ToLower().IndexOf("talk") >= 0)//this is talk
				{	
					Prog.BackColor = Form1.ProgramConfiguration.TalkColor;
					
				}
				else if(Prog.Categories.ToLower().IndexOf("children") >= 0)//this is a children's show
				{	
					Prog.BackColor = Form1.ProgramConfiguration.ChildrenColor;
					
				}
				else if(Prog.Categories.ToLower().IndexOf("favorites") >= 0)//this is a Favorites
				{	
					Prog.BackColor = Form1.ProgramConfiguration.FavoritesColor;
					
				}
				else if(Prog.Categories.ToLower().IndexOf("sports") >= 0)//this is a sports show
				{	
					Prog.BackColor = Form1.ProgramConfiguration.SportsColor;
					
				}
				else
				{
					Prog.BackColor = Form1.ProgramConfiguration.DefaultColor;
					//Custom show highlighting code
					foreach(CustomHighlight ch in Form1.ProgramConfiguration.CustomHighlights)
					{
						if (DoesProgramMatchSearchString(ch.FieldToSearch,Prog,ch.SearchString)==true)
							Prog.BackColor=ch.Highlight;
					}
				}
			}
		}
		#endregion
		#endregion

		#region "Array Building Methods"
		private TVProgramme[] BuildList_AllShowsWithinNDays(int nDays)
		{
			ArrayList Shows=new ArrayList();

			foreach(TVProgramme Prog in mAllShows)
			{
				if (IsProgrammeInRange_Days(Prog,nDays))
					Shows.Add(Prog);
			}
			return((TVProgramme[])Shows.ToArray(typeof(TVProgramme)));
		}

		private TVProgramme[] BuildList_AllShowsWithinNHours(int nHours)
		{
			ArrayList Shows=new ArrayList();

			foreach(TVProgramme Prog in mAllShows)
			{
				if (IsProgrammeInRange_Hours(Prog,nHours))
					Shows.Add(Prog);
			}
			return((TVProgramme[])Shows.ToArray(typeof(TVProgramme)));
		}

		private TVProgramme[] BuildList_MoviesByDate(DateTime dt)
		{
			ArrayList TodaysMovies=new ArrayList();

			foreach(TVProgramme Prog in mAllShows)
			{
				if (Prog.IsMovie() && Prog.StartTime.Date==dt.Date)
				{
					Prog.BackColor=Form1.ProgramConfiguration.MovieColor;
					TodaysMovies.Add(Prog);
				}				
			}
			return((TVProgramme[])TodaysMovies.ToArray(typeof(TVProgramme)));
		}

		private TVProgramme[] BuildList_ShowsByDate(DateTime dt)
		{
			ArrayList ShowsByDate=new ArrayList();

			foreach(TVProgramme Prog in mAllShows)
			{
				if (Prog.StartTime.Date==dt.Date)
					ShowsByDate.Add(Prog);
			}
			return((TVProgramme[])ShowsByDate.ToArray(typeof(TVProgramme)));
		}

		public TVProgramme[] BuildList_ShowsByChannel(TVChannel Channel, int nDays)
		{
			ArrayList ShowsByChannel=new ArrayList();

			foreach(TVProgramme Prog in mAllShows)
			{
				if (Prog.Channel==Channel && IsProgrammeInRange_Days(Prog,nDays))
					ShowsByChannel.Add(Prog);
			}
			return((TVProgramme[])ShowsByChannel.ToArray(typeof(TVProgramme)));
		}

		private TVProgramme[] BuildList_ShowsRightNow()
		{
			DateTime Today=System.DateTime.Now;
			ArrayList ShowsRightNow=new ArrayList();

			foreach(TVProgramme Prog in mAllShows)
			{
				if (Prog.StartTime<=Today && Prog.StopTime>=Today)
					ShowsRightNow.Add(Prog);
			}
			return((TVProgramme[])ShowsRightNow.ToArray(typeof(TVProgramme)));
		}

		private TVProgramme[] BuildList_ShowsOnAt(DateTime showTime,string channel,string TimeType)
		{
			ArrayList ShowsRightNow=new ArrayList();

			foreach(TVProgramme Prog in mAllShows)
			{
				if(timeType == "StartTime")//on next
				{
					if (Prog.StartTime>=showTime && Prog.StopTime>=showTime && Prog.StartTime.Date==showTime.Date && Prog.Channel.DisplayName.Substring(0,Prog.Channel.DisplayName.IndexOf(" ")) == channel)
					{
						ShowsRightNow.Add(Prog);
					}
				}
				else if(timeType == "onNow")//on now
				{
					if (Prog.StartTime<=showTime && Prog.StopTime>showTime && Prog.StartTime.Date==showTime.Date && Prog.Channel.DisplayName.Substring(0,Prog.Channel.DisplayName.IndexOf(" ")) == channel)
					{
						ShowsRightNow.Add(Prog);
					}
				}
				else//on previous
				{
					if (Prog.StopTime==showTime && Prog.StartTime.Date==showTime.Date && Prog.Channel.DisplayName.Substring(0,Prog.Channel.DisplayName.IndexOf(" ")) == channel)
					{
						ShowsRightNow.Add(Prog);
					}
				}
			}
			return((TVProgramme[])ShowsRightNow.ToArray(typeof(TVProgramme)));
		}
		#endregion

		#region "Search Functions"
		public TVProgramme[] SearchByTitle(string searchtext)
		{
			TVProgramme[] ShowsToSearch=this.AllShowsWithinNDays;
			ArrayList SearchResults=new ArrayList();

			foreach(TVProgramme Prog in ShowsToSearch)
			{
				//Eliminate case discrepancies
				string Temp1=Prog.Title.ToUpper();
				string Temp2=searchtext.ToUpper();

				if (Temp1.IndexOf(Temp2)>=0)
					SearchResults.Add(Prog);
			}

			return((TVProgramme[])SearchResults.ToArray(typeof(TVProgramme)));
		}

		public TVProgramme[] SearchByCategory(string searchtext)
		{
			TVProgramme[] ShowsToSearch=this.ShowsRightNow;
			ArrayList SearchResults=new ArrayList();

			foreach(TVProgramme prog in ShowsToSearch)
			{
				foreach(categoryType catType in prog.category)
				{
					//Eliminate case discrepancies
					string Temp1=catType.Value.ToUpper();
					string Temp2=searchtext.ToUpper();

					if (Temp1.IndexOf(Temp2)>=0)
					{
						SearchResults.Add(prog);
						break;
					}
				}
			}
			return((TVProgramme[])SearchResults.ToArray(typeof(TVProgramme)));
		}

		public TVProgramme[] SearchBySubtitle(string searchtext)
		{
			TVProgramme[] ShowsToSearch=this.AllShowsWithinNDays;
			ArrayList SearchResults=new ArrayList();

			foreach(TVProgramme Prog in ShowsToSearch)
			{
				//Eliminate case discrepancies
				string Temp1=Prog.SubTitle.ToUpper();
				string Temp2=searchtext.ToUpper();

				if (Temp1.IndexOf(Temp2)>=0)
					SearchResults.Add(Prog);
			}

			return((TVProgramme[])SearchResults.ToArray(typeof(TVProgramme)));
		}

		public bool MeetsFieldCriteria(TVProgramme prog, EFields Field, string text)
		{
			bool found=false;
			string temp="";

			switch(Field)
			{
				case EFields.Category:
					temp=prog.Categories.ToUpper();
					if (temp.IndexOf(text.ToUpper())>=0)
						found=true;
					break;
				case EFields.Description:
					temp=prog.Description.ToUpper();
					if (temp.IndexOf(text.ToUpper())>=0)
						found=true;
					break;
				case EFields.Title:
					temp=prog.Title.ToUpper();
					if (temp.IndexOf(text.ToUpper())>=0)
						found=true;
					break;
				case EFields.Subtitle:
					temp=prog.SubTitle.ToUpper();
					if (temp.IndexOf(text.ToUpper())>=0)
						found=true;
					break;
			}

			return(found);
		}

		public bool MeetsDateRangeCriteria(TVProgramme prog, DateTime Min, DateTime Max)
		{
			if (prog.StartTime.Date>=Min.Date && prog.StartTime.Date<=Max.Date)
				return(true);
			else
				return(false);
		}

		public bool MeetsTimeRangeCriteria(TVProgramme prog, DateTime Min, DateTime Max)
		{
			if (prog.StartTime.TimeOfDay>=Min.TimeOfDay && prog.StartTime.TimeOfDay<=Max.TimeOfDay)
				return(true);
			else
				return(false);
		}

		public bool MeetsChannelNumCriteria(TVProgramme prog, string Channel)
		{
			string temp1,temp2;

			temp1=prog.Channel.DisplayName.ToUpper().Trim();
			temp2=Channel.ToUpper();
			if ((temp1.IndexOf(temp2)==0) && (temp1.IndexOf(" ") == (temp1.IndexOf(temp2)+temp2.Length)))
				return(true);
			else
				return(false);
		}

		public TVProgramme whatsPlaying(string channel)
		{
			TVProgramme[] onNow = this.ShowsRightNow;
			
			foreach(TVProgramme prog in onNow)
			{
				if(MeetsChannelNumCriteria(prog,channel))
					return prog;
			}
			return null;
		}

		public TVProgramme whatsPlaying(string channel,DateTime startTime,string TimeType)
		{
			showTime = startTime;
			showChannel = channel;
			timeType = TimeType;
			TVProgramme[] showsFound = this.ShowsOnAt;
			foreach(TVProgramme prog in showsFound)
			{
				if(MeetsChannelNumCriteria(prog,channel))
					return prog;
			}
			return null;
		}
		#endregion

		#region "Private Helper Functions"
		//Returns true if programme airtime is somewhere between today and ndays from now
		private bool IsProgrammeInRange_Days(TVProgramme prog, int ndays)
		{
			DateTime Today=System.DateTime.Now;
			DateTime UpperBound=System.DateTime.Now;

			UpperBound=UpperBound.AddDays(ndays);
			
			if (prog.StartTime.Date>=Today.Date && prog.StartTime.Date<UpperBound.Date)
				return(true);
			else
				return(false);
		}

		private bool IsProgrammeInRange_Hours(TVProgramme prog, int nHours)
		{
			DateTime Today=System.DateTime.Now;
			DateTime UpperBound=System.DateTime.Now;

			UpperBound=UpperBound.AddHours(nHours);
			
			if (prog.StartTime>=Today && prog.StartTime<UpperBound)
				return(true);
			else
				return(false);
		}

		private bool DoesProgramMatchSearchString(CustomHighlight.SearchableFields field, TVProgramme prog, string str)
		{
			string ProgTemp="";
			string SearchTemp="";
			bool found=false;

			switch(field)
			{
				case CustomHighlight.SearchableFields.Title:
					ProgTemp=prog.Title.ToUpper();
					SearchTemp=str.ToUpper();
					if (ProgTemp.IndexOf(SearchTemp)>-1)
						found=true;
					break;
				case CustomHighlight.SearchableFields.SubTitle:
					ProgTemp=prog.SubTitle.ToUpper();
					SearchTemp=str.ToUpper();
					if (ProgTemp.IndexOf(SearchTemp)>-1)
						found=true;
					break;
				case CustomHighlight.SearchableFields.Description:
					ProgTemp=prog.Description.ToUpper();
					SearchTemp=str.ToUpper();
					if (ProgTemp.IndexOf(SearchTemp)>-1)
						found=true;
					break;
				case CustomHighlight.SearchableFields.Category:
					ProgTemp=prog.Categories.ToUpper();
					SearchTemp=str.ToUpper();
					if (ProgTemp.IndexOf(SearchTemp)>-1)
						found=true;
					break;
				case CustomHighlight.SearchableFields.Channel:
					ProgTemp=prog.Channel.ToString().ToUpper();
					SearchTemp=str.ToUpper();
					if (ProgTemp.IndexOf(SearchTemp)>-1)
						found=true;
					break;
			}

			return(found);
		}
		#endregion
	}
}
