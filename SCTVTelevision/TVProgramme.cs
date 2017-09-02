using System;
using System.Drawing;

namespace SCTVTelevision
{
	/// <summary>
	/// Summary description for TVProgramme.
	/// </summary>
	public class TVProgramme : programmeType
	{
		private StringParser Parser=new StringParser();

		public string mTitle;
		public string mSubTitle;
		public string mDescription;
		private TVChannel mChannel;
		private string mEpisodeNumber;
		private DateTime mStartTime;
		private DateTime mStopTime;
		public string mCategories; //stores concatenated list of categories 
		public Color mBackColor; //color that should be used to highlight this particular program in the display

		#region "Properties"
		public string Title
		{
			get { return(mTitle); }
		}
		public string SubTitle
		{
			get { return(mSubTitle); }
		}
		public string Description
		{
			get { return(mDescription); }
		}
		public TVChannel Channel
		{
			get { return(mChannel); }
			set { mChannel=value; }
		}
		public string EpisodeNumber
		{
			get { return(mEpisodeNumber); }
		}
		public DateTime StartTime
		{
			get { return(mStartTime); }
		}
		public DateTime StopTime
		{
			get { return(mStopTime); }
		}
		public string Categories
		{
			get { return(mCategories); }
		}
		public Color BackColor
		{
			get { return(mBackColor); }
			set { mBackColor=value; }
		}
		#endregion

		#region "Constructors"
		public TVProgramme()
		{
			mTitle="";
			mSubTitle="";
			mDescription="";
			mEpisodeNumber="";
			mCategories="";
			mBackColor=Color.White;
		}

		public TVProgramme(programmeType prog)
		{			
			mBackColor=Color.White;
			InitializeFromProgrammeType(prog);
		}

		public TVProgramme(programmeType prog, Color Col)
		{
			Console.WriteLine("prog "+ prog.ToString());
			mBackColor=Col;
			InitializeFromProgrammeType(prog);
		}
		#endregion

		private void InitializeFromProgrammeType(programmeType prog)
		{
			this.credits=prog.credits;
			this.date=prog.date;
			this.category=prog.category;
			this.language=prog.language;
			this.origlanguage=prog.origlanguage;
			this.length=prog.length;
			this.icon=prog.icon;
			this.url=prog.url;
			this.country=prog.country;
			this.episodenum=prog.episodenum;
			this.video=prog.video;
			this.audio=prog.audio;
			this.previouslyshown=prog.previouslyshown;
			this.premiere=prog.premiere;
			this.lastchance=prog.lastchance;
			this.rating=prog.rating;
			this.starrating=prog.starrating;
			this.pdcstart=prog.pdcstart;
			this.vpsstart=prog.vpsstart;
			this.showview=prog.showview;
			this.videoplus=prog.videoplus;
			this.clumpidx=prog.clumpidx;
			this.channel=prog.channel;

			if (prog.title!=null)
			{
				string tempstr="";
				foreach(titleType t in prog.title)
				{
					tempstr+=t.Value;
				}                
				mTitle=tempstr;
			}
			else mTitle="N/A";

			if (prog.subtitle!=null)
			{
				string tempstr="";
				foreach(subtitleType t in prog.subtitle)
				{
					tempstr+=t.Value;
				}                
				mSubTitle=tempstr;
			}
			else mSubTitle="N/A";

			if (prog.desc!=null)
			{
				string tempstr="";
				foreach(descType t in prog.desc)
				{
					tempstr+=t.Value+", ";
				}                
				mDescription=tempstr;
			}
			else mDescription="N/A";
            
			if (prog.category!=null)
			{
				string tempstr="";
				foreach(categoryType t in prog.category)
				{
					tempstr+=t.Value+", ";
				}                
				mCategories=tempstr;
			}
			else mCategories="N/A";

			mStartTime=Parser.DateStringToDateTime(prog.start);
			mStopTime=Parser.DateStringToDateTime(prog.stop);		

			if (this.episodenum!=null)
				mEpisodeNumber=this.episodenum.Value;
			else mEpisodeNumber="N/A";
		}

		public bool IsMovie()
		{
			if (this.rating!=null)
			{
				foreach(ratingType r in this.rating)
				{
					if (r.system=="MPAA")
					{
						return(true);
					}
				}
			}
			return(false);
		}
	}
}
