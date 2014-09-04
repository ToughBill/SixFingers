using System;
using System.IO;
using System.Reflection;
using System.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkstationController.Core.Data;

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;

namespace WorkstationController.Core.UnitTest
{
    [TestClass]
    public class LabwareTests
    {
        private string _xmlFilePath = string.Empty;

        [TestInitialize]
        public void Initialization()
        {
            this._xmlFilePath = Path.Combine(UnitTestHelper.GetTestModuleDirectory(), "testresult", "LabwareSerializeTest.xml");
        }

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

            labware.Serialize(this._xmlFilePath);
        }

        [TestMethod]
        public void LabwareDesrializeFromXmlFileTest()
        {
            Labware labware = Labware.Create(this._xmlFilePath);
            Assert.AreEqual<string>(labware.Name, "LabwareSerializeTest");
        }
    }
}
