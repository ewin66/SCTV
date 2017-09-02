using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class justinTV : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        tblEmbedCode.Text = "<object type=\"application/x-shockwave-flash\" height=\"595\" width=\"653\" id=\"live_embed_player_flash\" data=\"http://www.justin.tv/widgets/live_embed_player.swf?channel=clickey\" bgcolor=\"#000000\"><param name=\"allowFullScreen\" value=\"true\" /><param name=\"allowscriptaccess\" value=\"always\" /><param name=\"movie\" value=\"http://www.justin.tv/widgets/live_embed_player.swf\" /><param name=\"flashvars\" value=\"publisherGuard=sctv&start_volume=50&watermark_position=top_right&channel=clickey&auto_play=true\" /></object>";
    }
    protected void ddCameras_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddCameras.SelectedIndex == 0)
        {
            tblEmbedCode.Text = "<object type=\"application/x-shockwave-flash\" height=\"595\" width=\"653\" id=\"live_embed_player_flash\" data=\"http://www.justin.tv/widgets/live_embed_player.swf?channel=clickey\" bgcolor=\"#000000\"><param name=\"allowFullScreen\" value=\"true\" /><param name=\"allowscriptaccess\" value=\"always\" /><param name=\"movie\" value=\"http://www.justin.tv/widgets/live_embed_player.swf\" /><param name=\"flashvars\" value=\"publisherGuard=sctv&start_volume=50&watermark_position=top_right&channel=clickey&auto_play=true\" /></object>";
        }
        else
        {
            tblEmbedCode.Text = "<object type=\"application/x-shockwave-flash\" height=\"595\" width=\"653\" id=\"live_embed_player_flash\" data=\"http://www.justin.tv/widgets/live_embed_player.swf?channel=justin1010\" bgcolor=\"#000000\"><param name=\"allowFullScreen\" value=\"true\" /><param name=\"allowscriptaccess\" value=\"always\" /><param name=\"movie\" value=\"http://www.justin.tv/widgets/live_embed_player.swf\" /><param name=\"flashvars\" value=\"publisherGuard=sctv&start_volume=50&watermark_position=top_right&channel=justin1010&auto_play=true\" /></object>";
        }
    }
}
