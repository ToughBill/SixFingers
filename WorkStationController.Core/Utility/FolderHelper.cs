using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using WorkstationController.Core.Properties;

namespace WorkstationController.Core.Utility
{
    public class FolderHelper
    {
        static public string GetExeFolder()
        {
            return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        }

        static public string GetLayoutFolder()
        {
            string sFilePath =  Path.Combine(GetExeFolder(), Resources.LayoutFolder);
            if (!Directory.Exists(sFilePath))
                Directory.CreateDirectory(sFilePath);
            return sFilePath;
        }
    }
}
