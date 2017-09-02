using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.Diagnostics;
using System.Reflection;
using System.IO;

namespace ProjectInstaller
{
    [RunInstaller(true)]
    public partial class ProjInstaller  : InstallerBase.ProjInstaller
    {
        public ProjInstaller()
        {
            InitializeComponent();
        }
    }
}