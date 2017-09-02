<%@ Page Language="C#" AutoEventWireup="true" CodeFile="justinTV.aspx.cs" Inherits="justinTV" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Security Video</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <table width=100% border=0 cellpadding=0 cellspacing=0>
        <tr>
            <td align=center>
                <table border=1>
                    <tr>
                        <td>
                            Cameras
                        </td>
                    </tr>
                    <tr>
                        <td align=center>
                            <asp:DropDownList AutoPostBack=true ID="ddCameras" runat="server" Width="277px" OnSelectedIndexChanged="ddCameras_SelectedIndexChanged">
                                <asp:ListItem Selected="True">Camera 1</asp:ListItem>
                                <asp:ListItem>Camera 2</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align=center>
                <asp:Table runat=server ID="tbl">
                    <asp:TableRow>
                        <asp:TableCell runat=server ID="tblEmbedCode">
                            
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
            </td>
        </tr>
    </table>
    </div>
    </form>
</body>
</html>
