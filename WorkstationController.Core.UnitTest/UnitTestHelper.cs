using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace WorkstationController.Core.UnitTest
{
    public static class UnitTestHelper
    {
        public static string GetTestModuleDirectory()
        {
            string testDllPath = Assembly.GetExecutingAssembly().Location;
            string folder = Path.GetDirectoryName(testDllPath);
            if(!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            string testResultFolder = folder + "testresult\\";
            if (!Directory.Exists(testResultFolder))
                Directory.CreateDirectory(testResultFolder);
            return folder;
        }
    }
}
