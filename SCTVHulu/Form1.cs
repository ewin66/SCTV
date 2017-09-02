using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SCTVHulu
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            //if (Page.IndexOf("<title>IMDb") != -1)
            //{
            //    //Normal result as list of movies
            //    MatchCollection vMC_LI = rMovieSearch_LI.Matches(Page);
            //    if (vMC_LI.Count < 1)
            //    {
            //        //Parsing error!!!no LI found!!!
            //    }
            //    foreach (Match vM_LI in vMC_LI)
            //    {
            //        MatchCollection vMC_MainValues = rMovieSearch_MainValues.Matches(vM_LI.Groups["LI"].Value);
            //        if (vMC_MainValues.Count != 1)
            //        {
            //            //Parsing error!!!WTF?
            //        }
            //        foreach (Match vM_MainValues in vMC_MainValues)
            //        {
            //            Movie m = new Movie();
            //            m.iCode = long.Parse(vM_MainValues.Groups["Code"].Value);
            //            m.lTitles.Add(new Movie.MovieTitle(vM_MainValues.Groups["Title"].Value, false));
            //            String sYear = vM_MainValues.Groups["Year"].Value;
            //            if (sYear == "????")
            //            {
            //                m.iYear = 0;
            //            }
            //            else
            //            {
            //                m.iYear = int.Parse(sYear);
            //            }
            //            MatchCollection vMC_AKA = rMovieSearch_AKA.Matches(vM_MainValues.Groups["Akas"].Value);
            //            foreach (Match vM_AKA in vMC_AKA)
            //            {
            //                m.lTitles.Add(new Movie.MovieTitle(vM_AKA.Groups["Title"].Value, true));
            //            }
            //            result.Add(m);
            //        }
            //    }
            //}
        }
    }
}