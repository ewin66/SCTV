using System;
using System.Drawing;
using System.Xml;
using System.Xml.Serialization;
using System.ComponentModel;

namespace SCTVTelevision
{
	/// <summary>
	/// Summary description for CustomHighlight.
	/// </summary>
	public class CustomHighlight
	{
		public enum SearchableFields
		{
			Title,
			SubTitle,
			Description,
			Channel,
			Category
		}

		public enum ColorFormat
		{
			NamedColor,
			ARGBColor
		}

		private string mSearchStr;
		private Color mColor;
		private SearchableFields mFieldToSearch;

		public CustomHighlight()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public CustomHighlight(string s, Color c, SearchableFields sf)
		{
			mSearchStr=s;
			mColor=c;
			mFieldToSearch=sf;
		}

		#region "Overrides"
		public override string ToString()
		{
			string str = this.mSearchStr;

			if (str == null || str.Length < 1) 
			{
				return(this.mFieldToSearch.ToString());
			}

			return str + " in " + this.mFieldToSearch.ToString();
		}

		#endregion

		#region "Properties"
		[CategoryAttribute("Highlight")]
		[DescriptionAttribute("This is the string that the search field will be searched for and if found, the matching program will be highlighted the specified color")]
		public string SearchString
		{
			get { return(mSearchStr); }
			set { mSearchStr=value; }
		}

		[XmlIgnore()] 
		[CategoryAttribute("Highlight")]
		[DescriptionAttribute("This is the color to highlight shows that match the specified search string")]
		public Color Highlight
		{
			get { return(mColor); }
			set { mColor=value; }
		}

		[XmlElement("Highlight")]
		[Browsable(false)]
		public string XmlColor
		{
			get
			{
				return this.SerializeColor(mColor);
			}
			set
			{
				mColor=this.DeserializeColor(value);
			}
		}

		[CategoryAttribute("Highlight")]
		[DescriptionAttribute("This is the field in which to search for the search string.")]
		public SearchableFields FieldToSearch
		{
			get { return(mFieldToSearch); }
			set { mFieldToSearch=value; }
		}
		#endregion

		#region "Color Serialization Methods"
		public string SerializeColor(Color color)
		{
			if( color.IsNamedColor )
				return string.Format("{0}:{1}", 
					ColorFormat.NamedColor, color.Name);
			else
				return string.Format("{0}:{1}:{2}:{3}:{4}", 
					ColorFormat.ARGBColor, 
					color.A, color.R, color.G, color.B);
		}

		public Color DeserializeColor(string color)
		{
			byte a, r, g, b;

			string [] pieces = color.Split(new char[] {':'});
		
			ColorFormat colorType = (ColorFormat) 
				Enum.Parse(typeof(ColorFormat), pieces[0], true);

			switch(colorType)
			{
				case ColorFormat.NamedColor:
					return Color.FromName(pieces[1]);

				case ColorFormat.ARGBColor:
					a = byte.Parse(pieces[1]);
					r = byte.Parse(pieces[2]);
					g = byte.Parse(pieces[3]);
					b = byte.Parse(pieces[4]);
			
					return Color.FromArgb(a, r, g, b);
			}
			return Color.Empty;
		}
		#endregion
	}
}
