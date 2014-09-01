using System;
using System.IO;
using System.Reflection;
using System.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkstationController.Core.Data;

namespace WorkstationController.Core.UnitTest
{
    [TestClass]
    public class LabwareTests
    {
        [TestMethod]
        public void LabwareSerializeToXmlFileTest()
        {
            Labware labware = new Labware();
            labware.Name = "LabwareSerializeTest";
            labware.XLength = 85;
            labware.YLength = 125;
            labware.Height = 45;
            labware.WellRadius = 4;
            labware.NumberOfWellsX = 8;
            labware.NumberOfWellsY = 12;
            labware.FirstWellPosition = new Point(8, 10);
            labware.LastWellPosition = new Point(78, 118);
            labware.ZTravel = 4200;
            labware.ZStart = 4000;
            labware.ZDispense = 3500;
            labware.ZMax = 50;

            string testDllPath = Assembly.GetExecutingAssembly().Location;
            string testDllDir = Path.GetDirectoryName(testDllPath);
            string xmlFilePath = Path.Combine(testDllDir, "testresult", "LabwareSerializeTest.xml");
            labware.Serialize(xmlFilePath);
        }
    }
}
