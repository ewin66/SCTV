using System;
using System.Windows.Forms;

namespace SCTV
{
	/// <summary>
	/// Summary description for StringParser.
	/// </summary>
	public class StringParser
	{
		public StringParser()
		{
			//
			// TODO: Add constructor logic here
			//
		}

        public int ChannelStringToInt(string channel)
        {
            string temp="";
            int x=0;
            
            x=channel.IndexOf(".",0,channel.Length);
            if (x>0)
            {
                temp=ParseNumberFromString(channel.Remove(x,channel.Length-x));
                return(System.Convert.ToInt32(temp));
            }
            else return(0);
        }

        public string ParseNumberFromString(string Str)
        {
			int x=0;
            string temp="";

			//remove beginning non numeric characters
			for (x=0; x<Str.Length; x++)
			{
				char c=Str[x];
				if (c>='0' && c<='9')
					break;
			}
			Str=Str.Remove(0,x);

			//Remove all non numeric characters trailing the numbers
			for (x=0; x<Str.Length; x++)
            {
				char c=Str[x];
				if (!(c>='0' && c<='9'))
					break;
            }
			temp=Str.Remove(x,Str.Length-x);

            return(temp);
        }		

		public bool CheckForNonNumericChars(string str)
		{
			foreach(char c in str)
			{
				if (!(c>='0' && c<='9'))
					return(false);
			}
			return(true);
		}
		
        public DateTime DateStringToDateTime(string Str)
        {            
            DateTime temp=new DateTime();
            string tempstr="";
            int year,month,day,hour,minute,second;
            
            if (Str!=null && Str!=String.Empty)
            {         
				//20030808210000
                
				//Parse year
                tempstr=Str.Substring(0,4);
                year=System.Convert.ToInt32(tempstr);
                Str=Str.Remove(0,4);
            
                //Parse Month
                tempstr=Str.Substring(0,2);
                month=System.Convert.ToInt32(tempstr);            
                Str=Str.Remove(0,2);
            
                //Parse Day
                tempstr=Str.Substring(0,2);
                day=System.Convert.ToInt32(tempstr);            
                Str=Str.Remove(0,2);
                        
                //Parse Hour
                tempstr=Str.Substring(0,2);
                hour=System.Convert.ToInt32(tempstr);            
                Str=Str.Remove(0,2);
            
                //Parse Minute
                tempstr=Str.Substring(0,2);
                minute=System.Convert.ToInt32(tempstr);            
                Str=Str.Remove(0,2);
            
                //Parse Second
                tempstr=Str.Substring(0,2);
                second=System.Convert.ToInt32(tempstr);            
                Str=Str.Remove(0,2);
            
                temp=new DateTime(year,month,day,hour,minute,second,0);
            }			
            return(temp);
        }
	}
}
