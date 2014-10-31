using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
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
            carrier.Dimension.XLength = 40;
            carrier.Dimension.YLength = 500;
            carrier.Grid = 3;
            carrier.Sites.Add(new Site(0, 0, 0, 10, 10, 1));

            Labware labware1 = new Labware();
            labware1.Label = "Labware1";
            labware1.Dimension.XLength = 85;
            labware1.Dimension.YLength = 125;
            labware1.WellsInfo.WellRadius = 4;
            labware1.WellsInfo.NumberOfWellsX = 8;
            labware1.WellsInfo.NumberOfWellsY = 12;
            labware1.WellsInfo.FirstWellPositionX = 8;
            labware1.WellsInfo.FirstWellPositionY = 10;
            labware1.WellsInfo.LastWellPositionX = 78;
            labware1.WellsInfo.LastWellPositionX = 118;
            labware1.ZValues.ZTravel = 4200;
            labware1.ZValues.ZStart = 4000;
            labware1.ZValues.ZDispense = 3500;
            labware1.ZValues.ZMax = 50;
            carrier.AddLabware(labware1);

            Labware labware2 = new Labware();
            labware2.Label = "Labware2";
            labware2.Dimension.XLength = 85;
            labware2.Dimension.YLength = 125;
            labware2.WellsInfo.WellRadius = 4;
            labware2.WellsInfo.NumberOfWellsX = 8;
            labware2.WellsInfo.NumberOfWellsY = 12;
            labware1.WellsInfo.FirstWellPositionX = 8;
            labware1.WellsInfo.FirstWellPositionY = 10;
            labware1.WellsInfo.LastWellPositionX = 78;
            labware1.WellsInfo.LastWellPositionX = 118;
            labware2.ZValues.ZTravel = 4200;
            labware2.ZValues.ZStart = 4000;
            labware2.ZValues.ZDispense = 3500;
            labware2.ZValues.ZMax = 50;
            carrier.AddLabware(labware2);

            carrier.Serialize(this._xmlFileWithLabwarePath);
        }

        [TestMethod]
        public void CarrierSerializeToXmlFileNoLabwaresTest()
        {
            Carrier carrier = new Carrier();
            carrier.Dimension.XLength = 40;
            carrier.Dimension.YLength = 500;
            carrier.Grid = 3;

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
