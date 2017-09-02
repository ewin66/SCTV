using System;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Data;

namespace SCTVTelevision
{
	/// <summary>
	/// Summary description for channelListsArrayList.
	/// </summary>
	public class channelListsArrayList : ArrayList
	{
//		private ArrayList mChannelList=new ArrayList();

		public delegate void channelListsListChanged();
		public event channelListsListChanged OnListChanged;

		public channelListsArrayList():base()
		{
			
		}

		public channelListsArrayList(string channelListName):base()
		{
			findChannelList(channelListName);
		}	
	
		public void Add(TVChannel[] channels)
		{
			
			foreach(TVChannel channel in channels)
			{
				//Console.WriteLine("adding to channelLists "+ channel.DisplayName);
				if(!base.Contains(channel))
					base.Add(channel);
			}
			//if(OnListChanged==null)
			OnListChanged();
			//Console.WriteLine("done adding to channelLists size: "+ base.Count.ToString());
		}

		public void saveChannelLists(TVChannel[] channels)
		{
			try
			{
				TVChannel[] tempList = getAllChannelLists();
//				tempList.AddRange(channels);
				XmlSerializer serializer=new XmlSerializer(typeof(TVChannel));
				TextWriter writer=new StreamWriter("config/channelLists.xml");

//				tempList.Sort();
				serializer.Serialize(writer,channels);
				writer.Close();
			}
			catch(Exception exc)
			{
				Console.WriteLine("Exception: "+ exc.ToString());
			}
		}

		private TVChannel[] getAllChannelLists()
		{
			XmlSerializer serializer=new XmlSerializer(typeof(TVChannel)); 
			FileStream fs=new FileStream("config/channelLists.xml", FileMode.Open);

			TVChannel[] tempList=(TVChannel[])serializer.Deserialize(fs);
			
//			tempList.Sort();
			fs.Close();
			return tempList;

			//			if (OnListChanged!=null)
			//				OnListChanged();
		}

		private void findChannelList(string channelListName)//loads base with the list name specified
		{

			TVChannel[] tempList = getAllChannelLists();
			base.Add(tempList);
//			foreach(TVChannel channel in tempList)
//			{
//				if((string)channelList[0] == channelListName)
//				{
//					base.Clear();
//					base.Add(channelList);
//					break;
//				}
//			}
		}
	}
}
