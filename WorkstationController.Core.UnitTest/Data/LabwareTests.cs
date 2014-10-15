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

            labware.TypeName = "LabwareSerializeTest";
            labware.Dimension.XLength = 800;
            labware.Dimension.YLength = 125;
            labware.WellsInfo.WellRadius = 4;
            labware.WellsInfo.NumberOfWellsX = 8;
            labware.WellsInfo.NumberOfWellsY = 12;
            labware.WellsInfo.FirstWellPosition = new Point(8, 10);
            labware.WellsInfo.LastWellPosition = new Point(78, 118);

            labware.ZValues.ZTravel = 300;
            labware.ZValues.ZStart = 600;
            labware.ZValues.ZDispense = 1000;
            labware.ZValues.ZMax = 1600;

            labware.Serialize(this._xmlFilePath);
        }

        [TestMethod]
        public void LabwareDesrializeFromXmlFileTest()
        {
            Labware labware = Labware.Create(this._xmlFilePath);
            Assert.AreEqual<string>(labware.TypeName, "LabwareSerializeTest");
        }
    }
}
