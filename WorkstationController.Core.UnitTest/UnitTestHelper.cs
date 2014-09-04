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
            return Path.GetDirectoryName(testDllPath);
        }
    }
}
