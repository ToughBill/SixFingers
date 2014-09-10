using System;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkstationController.Core.Data;

namespace WorkstationController.Core.UnitTest
{
    [TestClass]
    public class CarrierTests
    {
        private string _xmlFileWithLabwarePath = string.Empty;
        private string _xmlFileWithoutLabwarePath = string.Empty;

        [TestInitialize]
        public void Initialization()
        {
            this._xmlFileWithLabwarePath = Path.Combine(UnitTestHelper.GetTestModuleDirectory(), "testresult", "CarrierSerializeWithLabwareTest.xml");
            this._xmlFileWithoutLabwarePath = Path.Combine(UnitTestHelper.GetTestModuleDirectory(), "testresult", "CarrierSerializeNoLabwareTest.xml");
        }

        [TestMethod]
        public void CarrierSerializeToXmlFileWithLabwaresTest()
        {
            Carrier carrier = new Carrier();
            carrier.Name = "CarrierSerializeTest";
            carrier.XLength = 40;
            carrier.YLength = 500;
            carrier.AllowedLabwareNumber = 3;
            carrier.AllowedLabwareType = 2;
            carrier.PositionOnWorktable = new Point(200, 120);

            Labware labware1 = new Labware();
            labware1.Name = "Labware1";
            labware1.XLength = 85;
            labware1.YLength = 125;
            labware1.Height = 45;
            labware1.WellRadius = 4;
            labware1.NumberOfWellsX = 8;
            labware1.NumberOfWellsY = 12;
            labware1.FirstWellPosition = new Point(8, 10);
            labware1.LastWellPosition = new Point(78, 118);
            labware1.ZTravel = 4200;
            labware1.ZStart = 4000;
            labware1.ZDispense = 3500;
            labware1.ZMax = 50;
            carrier.AddLabware(labware1);

            Labware labware2 = new Labware();
            labware2.Name = "Labware2";
            labware2.XLength = 85;
            labware2.YLength = 125;
            labware2.Height = 45;
            labware2.WellRadius = 4;
            labware2.NumberOfWellsX = 8;
            labware2.NumberOfWellsY = 12;
            labware2.FirstWellPosition = new Point(8, 10);
            labware2.LastWellPosition = new Point(78, 118);
            labware2.ZTravel = 4200;
            labware2.ZStart = 4000;
            labware2.ZDispense = 3500;
            labware2.ZMax = 50;
            carrier.AddLabware(labware2);

            Debug.Assert(carrier.Labwares.Count == 2);

            carrier.Serialize(this._xmlFileWithLabwarePath);
        }

        [TestMethod]
        public void CarrierSerializeToXmlFileNoLabwaresTest()
        {
            Carrier carrier = new Carrier();
            carrier.Name = "CarrierSerializeTest";
            carrier.XLength = 40;
            carrier.YLength = 500;
            carrier.AllowedLabwareNumber = 3;
            carrier.AllowedLabwareType = 2;
            carrier.PositionOnWorktable = new Point(200, 120);

            carrier.Serialize(this._xmlFileWithoutLabwarePath);
        }

        [TestMethod]
        public void CarrierDeserializaFromXmlFile()
        {
            // Carrier with labware
            Carrier carrierWithLabware = Carrier.Create(this._xmlFileWithLabwarePath);
            Assert.AreEqual<int>(carrierWithLabware.Labwares.Count, 2);

            // Carrier without labware
            Carrier carrierWithoutLabware = Carrier.Create(this._xmlFileWithoutLabwarePath);
            Assert.AreEqual<int>(carrierWithoutLabware.Labwares.Count, 0);
        }
    }
}
