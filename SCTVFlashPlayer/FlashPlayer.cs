using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SCTV
{
    public partial class FlashPlayer : Form
    {
        public delegate void DocumentLoaded();
        public event DocumentLoaded documentLoaded;

        string gameToPlay = "";
        string title = "";
        System.Threading.Thread waitThread;

        public string GameToPlay
        {
            set
            {
                gameToPlay = value;

                playGame(value);
            }
        }

        public string Title
        {
            set
            {
                title = value;
            }
        }

        public FlashPlayer()
        {
            InitializeComponent();
        }

        private void playGame(string gamePath)
        {
            Uri gameUrl = new Uri(gamePath);
            webBrowser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser_DocumentCompleted);
            webBrowser.Url = gameUrl;

            if (title.Length > 0)
                this.title = title;
            else
                this.title = gameUrl.Segments[(gameUrl.Segments.Length)-1];
        }

        void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            waitThread = new System.Threading.Thread(wait);
            waitThread.Start();
        }

        private void wait()
        {
            try
            {
                //webBrowser.Invalidate();
            System.Threading.Thread.Sleep(8000);
            waitThread.Join(8000);

            documentLoaded();
            }
            catch (Exception ex)
            {
                //SCTVObjects.Tools.WriteToFile(ex);
            }
            
        }
    }
}