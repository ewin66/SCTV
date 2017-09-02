using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;



namespace WebVideo
{
    [DefaultProperty("FilePath")]
    [ToolboxData("<{0}:WVC runat=server></{0}:WVC>")]
    public class WVC : WebControl
    {
    
    
#region Declarations

        private string mFilePath;
        private bool mShowStatusBar;
        private bool mShowControls;
        private bool mShowPositionControls;
        private bool mShowTracker;

#endregion


        
#region Properties

        [Category("File URL")]
        [Browsable(true)]
        [Description("Set path to source file.")]
        [Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string FilePath
        {
            get
            {
                return mFilePath;
            }
            set
            {
                if (value == string.Empty)
                {
                    mFilePath = string.Empty;
                }
                else
                {
                    int tilde = -1;
                    tilde = value.IndexOf('~');
                    if (tilde != -1)
                    {
                        mFilePath = value.Substring((tilde + 2)).Trim();
                    }
                    else
                    {
                        mFilePath = value;
                    }
                }
            }
        }   // end FilePath property


        [Category("Media Player")]
        [Browsable(true)]
        [Description("Show or hide the tracker.")]
        public bool ShowTracker
        {
            get
            {
                return mShowTracker;
            }
            set
            {
                mShowTracker = value;
            }
        }


        [Category("Media Player")]
        [Browsable(true)]
        [Description("Show or hide the position controls.")]
        public bool ShowPositionControls
        {
            get
            {
                return mShowPositionControls;
            }
            set
            {
                mShowPositionControls = value;
            }
        }


        [Category("Media Player")]
        [Browsable(true)]
        [Description("Show or hide the controls.")]
        public bool ShowControls
        {
            get
            {
                return mShowControls;
            }
            set
            {
                mShowControls = value;
            }
        }


        [Category("Media Player")]
        [Browsable(true)]
        [Description("Show or hide the status bar.")]
        public bool ShowStatusBar
        {
            get
            {
                return mShowStatusBar;
            }
            set
            {
                mShowStatusBar = value;
            }
        }



#endregion


        
#region "Rendering"

        protected override void RenderContents(HtmlTextWriter writer)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<object ID=MediaPlayer classid=clsid:22D6F312-B0F6-11D0-94AB-0080C74C7E95 ");
                sb.Append("codebase=http://activex.microsoft.com/activex/controls/mplayer/en/nsmp2inf.cab#Version=5,1,52,701 Width = " + Width.Value.ToString() + " Height = " + Height.Value.ToString() + "type=application/x-oleobject align=absmiddle");
                sb.Append("standby='Loading Microsoft+reg; Windows+reg; Media Player components...' id=mp1 /> ");
                sb.Append("<param name=FileName value='" + FilePath.ToString() + "'> ");
                sb.Append("<param name=ShowStatusBar value=" + ShowStatusBar.ToString() + "> ");
                sb.Append("<param name=ShowPositionControls value=" + ShowPositionControls.ToString() + "> ");
                sb.Append("<param name=ShowTracker value=" + ShowTracker.ToString() + "> ");
                sb.Append("<param name=ShowControls value=" + ShowControls.ToString() + "> ");
                sb.Append("<embed name=MediaPlayer src='" + FilePath.ToString() + "' ");
                sb.Append("pluginspage=http://www.microsoft.com/Windows/MediaPlayer type=application/x-mplayer2 ");
                sb.Append("Width = " + Width.Value.ToString() + " ");
                sb.Append("Height = " + Height.Value.ToString());
                sb.Append(" autostart = 1");
                sb.Append(" /></embed></object>");

                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                writer.Write(sb.ToString());
                writer.RenderEndTag();
            }
            catch
            {
                // with no properties set, this will render "Display PDF Control" in a
                // a box on the page
                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                writer.Write("Display WVT Control");
                writer.RenderEndTag();
            }  // end try-catch
        }   // end RenderContents



#endregion

    
    
    }
}
