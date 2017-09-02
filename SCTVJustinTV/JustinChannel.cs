using System;
using System.Collections.Generic;
using System.Text;

namespace SCTVJustinTV
{
    class JustinChannel
    {
        string channelName = "";
        string channelPassword = "";
        string streamKey = "";
        bool inUse = false;

        public string ChannelName
        {
            get { return channelName; }
            set { channelName = value; }
        }

        public string ChannelPassword
        {
            get { return channelPassword; }
            set { channelPassword = value; }
        }

        public string StreamKey
        {
            get { return streamKey; }
            set { streamKey = value; }
        }

        public bool InUse
        {
            get { return inUse; }
            set { inUse = value; }
        }

        public JustinChannel(string ChannelName, string ChannelPassword, string StreamKey, bool InUse)
        {
            channelName = ChannelName;
            channelPassword = ChannelPassword;
            streamKey = StreamKey;
            inUse = InUse;
        }
    }
}
