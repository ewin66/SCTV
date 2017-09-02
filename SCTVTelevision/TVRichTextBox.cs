using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace SCTVTelevision
{
	/// <summary>
	/// Summary description for TVRichTextBox.
	/// </summary>
	public class TVRichTextBox : System.Windows.Forms.RichTextBox
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		public string channel;

		public TVRichTextBox()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call

		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		public void AddMedia(DataRow drShow)
		{
			FontStyle newFontStyle;
			Font currentFont=this.SelectionFont;

			this.Clear();

			//Title
			newFontStyle=FontStyle.Bold;
			this.SelectionFont = new Font(currentFont.FontFamily,currentFont.Size,newFontStyle);
			this.AppendText("\tTitle");

			newFontStyle=FontStyle.Regular;
			this.SelectionFont = new Font(currentFont.FontFamily,currentFont.Size,newFontStyle);
			this.AppendText(" "+ drShow["title"] +" ");

			//stars
			newFontStyle=FontStyle.Bold;
			this.SelectionFont = new Font(currentFont.FontFamily,currentFont.Size,newFontStyle);
			this.AppendText("\tStars");

			newFontStyle=FontStyle.Regular;
			this.SelectionFont = new Font(currentFont.FontFamily,currentFont.Size,newFontStyle);
			this.AppendText(" "+ drShow["stars"] +" ");

			//rating
			newFontStyle=FontStyle.Bold;
			this.SelectionFont = new Font(currentFont.FontFamily,currentFont.Size,newFontStyle);
			this.AppendText("\tRating");

			newFontStyle=FontStyle.Regular;
			this.SelectionFont = new Font(currentFont.FontFamily,currentFont.Size,newFontStyle);
			this.AppendText(" "+ drShow["rating"] +" ");
			
			//Category
			newFontStyle=FontStyle.Bold;
			this.SelectionFont = new Font(currentFont.FontFamily,currentFont.Size,newFontStyle);
			this.AppendText("\tCategory");

			newFontStyle=FontStyle.Regular;
			this.SelectionFont = new Font(currentFont.FontFamily,currentFont.Size,newFontStyle);
			this.AppendText(" "+ drShow["category"] +" ");

			//Description
			newFontStyle=FontStyle.Bold;
			this.SelectionFont = new Font(currentFont.FontFamily,currentFont.Size,newFontStyle);
			this.AppendText("\nDescription");

			newFontStyle=FontStyle.Regular;
			this.SelectionFont = new Font(currentFont.FontFamily,currentFont.Size,newFontStyle);
			this.AppendText(" "+ drShow["description"] +" ");
		}

		public void AddProgramme(TVProgramme Prog)
		{
			if (Prog.IsMovie())
				AddMovie(Prog);
			else AddShow(Prog);
		}
			
		public void AddShow(TVProgramme Prog)
		{
			FontStyle newFontStyle;
			Font currentFont=this.SelectionFont;

			this.Clear();

			//Start and end times
			newFontStyle=FontStyle.Bold;
			this.SelectionFont=new Font(currentFont.FontFamily,currentFont.Size,newFontStyle);
			this.AppendText(Prog.StartTime.ToShortTimeString()+" - "+Prog.StopTime.ToShortTimeString());

			//Title
			newFontStyle=FontStyle.Bold;
			this.SelectionFont = new Font(currentFont.FontFamily,currentFont.Size,newFontStyle);
			this.AppendText("\tTitle");

			newFontStyle=FontStyle.Regular;
			this.SelectionFont = new Font(currentFont.FontFamily,currentFont.Size,newFontStyle);
			this.AppendText(" "+Prog.Title+" ");

			//Subtitle
			newFontStyle=FontStyle.Bold;
			this.SelectionFont = new Font(currentFont.FontFamily,currentFont.Size,newFontStyle);
			this.AppendText("\tSubTitle");

			newFontStyle=FontStyle.Regular;
			this.SelectionFont = new Font(currentFont.FontFamily,currentFont.Size,newFontStyle);
			this.AppendText(" "+Prog.SubTitle+" ");

			//Channel
			newFontStyle=FontStyle.Bold;
			this.SelectionFont = new Font(currentFont.FontFamily,currentFont.Size,newFontStyle);
			this.AppendText("\tChannel");

			newFontStyle=FontStyle.Regular;
			this.SelectionFont = new Font(currentFont.FontFamily,currentFont.Size,newFontStyle);
			this.AppendText(" "+Prog.Channel.DisplayName+" ");
			channel = Prog.Channel.DisplayName;
			
			//Category
			newFontStyle=FontStyle.Bold;
			this.SelectionFont = new Font(currentFont.FontFamily,currentFont.Size,newFontStyle);
			this.AppendText("\tCategory");

			newFontStyle=FontStyle.Regular;
			this.SelectionFont = new Font(currentFont.FontFamily,currentFont.Size,newFontStyle);
			this.AppendText(" "+Prog.Categories+" ");

			//Description
			newFontStyle=FontStyle.Bold;
			this.SelectionFont = new Font(currentFont.FontFamily,currentFont.Size,newFontStyle);
			this.AppendText("\nDescription");

			newFontStyle=FontStyle.Regular;
			this.SelectionFont = new Font(currentFont.FontFamily,currentFont.Size,newFontStyle);
			this.AppendText(" "+Prog.Description+" ");
		}

		public void AddMovie(TVProgramme Prog)
		{
			FontStyle newFontStyle;
			Font currentFont=this.SelectionFont;
			string IMDBUrl;

			this.Clear();

			//Start and end times
			newFontStyle=FontStyle.Bold;
			this.SelectionFont=new Font(currentFont.FontFamily,currentFont.Size,newFontStyle);
			this.AppendText(Prog.StartTime.ToShortTimeString()+" - "+Prog.StopTime.ToShortTimeString());

			//Title
			newFontStyle=FontStyle.Bold;
			this.SelectionFont = new Font(currentFont.FontFamily,currentFont.Size,newFontStyle);
			this.AppendText("\tTitle");

			newFontStyle=FontStyle.Regular;
			this.SelectionFont = new Font(currentFont.FontFamily,currentFont.Size,newFontStyle);
			this.AppendText(" "+Prog.Title+" ");

			//Rating
			if (Prog.rating!=null)
			{
				newFontStyle=FontStyle.Bold;
				this.SelectionFont = new Font(currentFont.FontFamily,currentFont.Size,newFontStyle);
				this.AppendText("\tRating");

				newFontStyle=FontStyle.Regular;
				this.SelectionFont = new Font(currentFont.FontFamily,currentFont.Size,newFontStyle);

				foreach(ratingType r in Prog.rating)
				{
					if (r.system=="MPAA")
						this.AppendText(" "+r.value+" ");
				}
			}

			//Channel
			newFontStyle=FontStyle.Bold;
			this.SelectionFont = new Font(currentFont.FontFamily,currentFont.Size,newFontStyle);
			this.AppendText("\tChannel");

			newFontStyle=FontStyle.Regular;
			this.SelectionFont = new Font(currentFont.FontFamily,currentFont.Size,newFontStyle);
			this.AppendText(" "+Prog.Channel.DisplayName+" ");
			
			//Date
			newFontStyle=FontStyle.Bold;
			this.SelectionFont = new Font(currentFont.FontFamily,currentFont.Size,newFontStyle);
			this.AppendText("\tDate");

			newFontStyle=FontStyle.Regular;
			this.SelectionFont = new Font(currentFont.FontFamily,currentFont.Size,newFontStyle);
			this.AppendText(" "+Prog.date+" ");

			//Description
			newFontStyle=FontStyle.Bold;
			this.SelectionFont = new Font(currentFont.FontFamily,currentFont.Size,newFontStyle);
			this.AppendText("\nDescription");

			newFontStyle=FontStyle.Regular;
			this.SelectionFont = new Font(currentFont.FontFamily,currentFont.Size,newFontStyle);
			this.AppendText(" "+Prog.Description+"\n");	
		
			IMDBUrl="http://us.imdb.com/find?q="+Prog.Title.Replace(" ","+");
			this.AppendText(IMDBUrl);
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion
	}
}
