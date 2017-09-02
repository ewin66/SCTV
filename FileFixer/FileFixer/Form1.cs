using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace FileFixer
{
    public partial class Form1 : Form
    {
        string sourceFilePath = @"C:\utilities\programming\projects\SCTV2\config\media_fromsctv.xml";
        string outputFilePath = "";//@"c:\sctv\media_New2.xml";

        public Form1()
        {
            InitializeComponent();

            txtFileToFix.Text = sourceFilePath;

            if(outputFilePath.Length > 0)
                txtOutputFile.Text = outputFilePath;
            else
            {
                string tempOutputFilePath = sourceFilePath;
                int fileCount = 0;

                while(File.Exists(tempOutputFilePath))
                {
                    fileCount++;
                    
                    if (tempOutputFilePath.Contains("_" + Convert.ToString(fileCount - 1) + ".xml"))
                        tempOutputFilePath = tempOutputFilePath.Replace("_" + Convert.ToString(fileCount - 1).ToString() + ".xml", "_" + fileCount.ToString() + ".xml");
                    else
                        tempOutputFilePath = tempOutputFilePath.Replace(".xml", "_" + fileCount.ToString() + ".xml");
                }

                txtOutputFile.Text = tempOutputFilePath;
            }
        }

        private void btnFix_Click(object sender, EventArgs e)
        {
            btnFix.Enabled = false;

            if (txtFileToFix.Text.Length > 0)
                sourceFilePath = txtFileToFix.Text;

            outputFilePath = txtOutputFile.Text;

            fixFile();

            btnFix.Enabled = true;
        }

        private void fixFile()
        {
            //string filePath = @"c:\sctv\media_new3.xml";

            string line;
            string newLine;
            string tag = "";

            if (File.Exists(sourceFilePath))
            {

                StreamReader sourceFile = null;
                StreamWriter destFile = new StreamWriter(outputFilePath);

                try
                {
                    sourceFile = new StreamReader(sourceFilePath);

                    while ((line = sourceFile.ReadLine()) != null)
                    {
                        if (line.Length > 100)
                        {
                            tag = line.Substring(line.LastIndexOf("<"));

                            line = line.Substring(0, 100);

                            if(line.LastIndexOf("|") > 0)
                                line = line.Substring(0, line.LastIndexOf("|")) + tag;
                        }

                        while (line.Contains("  "))
                            line = line.Replace("  ", " ");

                        destFile.WriteLine(line);
                        //Console.WriteLine(line);

                    }

                }

                finally
                {

                    if (sourceFile != null)
                        sourceFile.Close();

                    if (destFile != null)
                        destFile.Close();
                }
            }

        }
    }
}