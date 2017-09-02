<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Default" %>

<%@ Register Assembly="WebVideo" Namespace="WebVideo" TagPrefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">

<script language="javascript">
//   status
// 0 - stopped
// 1 - paused
// 2 - playing

var prevPlayState = 0;
var curPlayState = 0;
var currentMedia = "";

//alert("host "+ location.host);
//alert("pathname "+ location.pathname);

//start checking player status
window.setInterval("checkPlayerStatus()",1000);

function checkPlayerStatus()//check the status of media player
{

    document.form1.mediaPlayerStatus.value = document.form1.MediaPlayer.PlayState;
    
    curPlayState = document.form1.MediaPlayer.PlayState;
    
    if(curPlayState != prevPlayState)//playState has changed
    {
        if(prevPlayState == 2 && curPlayState == 0)//they were watching something and it's now stopped - check for the next video
        {
            checkForNextVideo();
        }
        else if (curPlayState == 2 && currentMedia == "")//get current media
        {
            currentMedia = document.form1.MediaPlayer.GetMediaInfoString(8);
        }
    }
    
    prevPlayState = curPlayState;
}

function checkForNextVideo()//check for the next video and play it if found
{
    location.href = "http://"+ location.host + location.pathname +"?ID=<%= Request.QueryString["id"] %>&prevMedia="+ currentMedia;
}
</script>

    <title>SCTV Security Camera</title>
</head>
<body style="padding-right: 0px; padding-left: 0px; padding-bottom: 0px; margin: 0px; color: white; padding-top: 0px; font-family: Calibri" bgcolor="#000000">
    <form id="form1" runat="server">
    <center>
        <table border="0" cellpadding="0" cellspacing="0">
            <tr>
                <td valign="top" style="width: 433px">
                    <a href="Default.aspx">View Live Cam</a>
                    <br />
                    Cameras<br />
                    <asp:DropDownList ID="ddCameras" runat="server" AutoPostBack="True" Width="432px" Visible="true" OnSelectedIndexChanged="ddCameras_SelectedIndexChanged">
                    </asp:DropDownList>             
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Panel ID="pnlVideos" runat="server" Width="425px" Visible="false">
                    Available Videos<br />
                    <asp:DropDownList ID="ddVideos" runat="server" AutoPostBack="True" Width="431px">
                    </asp:DropDownList>
                    </asp:Panel>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblCurrentMedia" runat="server" Width="600px"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <cc1:WVC ID="WVC1" runat="server" BackColor="Black" BorderStyle="Ridge" FilePath=""
                    Height="600px" ShowControls="True" ShowPositionControls="True" ShowStatusBar="True"
                    ShowTracker="True" Width="700px" />
                </td>                
            </tr>
        </table>
        </center>
        <asp:TextBox ID="mediaPlayerStatus" runat="server" Visible="true" ReadOnly="true" Width="0" Height="0"></asp:TextBox>
    </form>
</body>
</html>
