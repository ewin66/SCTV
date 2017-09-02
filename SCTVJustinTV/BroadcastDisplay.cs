using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;

namespace SCTVJustinTV
{
    public partial class BroadcastDisplay : Form
    {
        public BroadcastDisplay()
        {
            InitializeComponent();

            WebClient client = new WebClient();
            String flashObject = client.DownloadString("http://api.justin.tv/api/channel/embed/gaming_master01?volume=50");
            flashObject = flashObject.Replace("height=\"295\" width=\"353\"", "height=\"595\" width=\"653\"");
            flashObject = flashObject.Replace("auto_play=false", "auto_play=true");//doesn't do anything
            wbJustin.DocumentText = flashObject;

            //wbJustin.Url = new Uri("http://api.justin.tv/api/channel/embed/gaming_master01?volume=50");
            //wbJustin.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(wbJustin_DocumentCompleted);
            
            //wbJustin.
            //<object type="application/x-shockwave-flash" height="300" width="400" id="live_embed_player_flash" data="http://www.justin.tv/widgets/live_embed_player.swf?channel=gaming_master01" bgcolor="#000000"><param name="allowFullScreen" value="true" /><param name="allowScriptAccess" value="always" /><param name="allowNetworking" value="all" /><param name="movie" value="http://www.justin.tv/widgets/live_embed_player.swf" /><param name="flashvars" value="channel=gaming_master01&auto_play=false&start_volume=25" /></object><a href="http://www.justin.tv/gaming_master01#r=5fnS1iQ~&s=em" class="trk" style="padding:2px 0px 4px; display:block; width:345px; font-weight:normal; font-size:10px; text-decoration:underline; text-align:center;">Watch live video from Mr.Bonkers J Pinciotti on Justin.tv</a>
        }

    }
}