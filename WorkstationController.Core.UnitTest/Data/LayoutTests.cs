using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkstationController.Core.Data;

namespace WorkstationController.Core.UnitTest
{
    [TestClass]
    public class LayoutTests
    {
        private string _xmlFileWithCarrierPath = string.Empty;
        private string _xmlFileWithoutCarrierPath = string.Empty;

        [TestInitialize]
        public void Initialization()
        {
            this._xmlFileWithCarrierPath = Path.Combine(UnitTestHelper.GetTestModuleDirectory(), "testresult", "LayoutSerializeWithCarriersTest.xml");
            this._xmlFileWithoutCarrierPath = Path.Combine(UnitTestHelper.GetTestModuleDirectory(), "testresult", "LayoutSerializeNoCarriersTest.xml");
        }

        [TestMethod]
        public void LayoutSerializeToXmlFileNoCarriersTest()
        {
            Layout layout = new Layout();
            layout.Name = "LayoutSerializeTest";

            layout.Serialize(this._xmlFileWithoutCarrierPath);
        }

        [TestMethod]
        public void LayoutSerializeToXmlFileWithCarriersTest()
        {
            Layout layout = new Layout();
            layout.Name = "LayoutSerializeTest";

            #region Add Carrier1
            Carrier carrier1 = new Carrier();
            carrier1.Name = "Carrier1";
            carrier1.XLength = 40;
            carrier1.YLength = 500;
            carrier1.AllowedLabwareType = 2;
            carrier1.Grid = 1;

            Labware labware11 = new Labware();
            labware11.Name = "Labware11";
            labware11.XLength = 85;
            labware11.YLength = 125;
            labware11.Height = 45;
            labware11.WellRadius = 4;
            labware11.NumberOfWellsX = 8;
            labware11.NumberOfWellsY = 12;
            labware11.FirstWellPosition = new Point(8, 10);
            labware11.LastWellPosition = new Point(78, 118);
            labware11.ZTravel = 4200;
            labware11.ZStart = 4000;
            labware11.ZDispense = 3500;
            labware11.ZMax = 50;
            carrier1.AddLabware(labware11);

            Labware labware12 = new Labware();
            labware12.Name = "Labware12";
            labware12.XLength = 85;
            labware12.YLength = 125;
            labware12.Height = 45;
            labware12.WellRadius = 4;
            labware12.NumberOfWellsX = 8;
            labware12.NumberOfWellsY = 12;
            labware12.FirstWellPosition = new Point(8, 10);
            labware12.LastWellPosition = new Point(78, 118);
            labware12.ZTravel = 4200;
            labware12.ZStart = 4000;
            labware12.ZDispense = 3500;
            labware12.ZMax = 50;
            carrier1.AddLabware(labware12);

            layout.AddCarrier(carrier1);
            #endregion

            #region Add Carrier2
            Carrier carrier2 = new Carrier();
            carrier2.Name = "Carrier2";
            carrier2.XLength = 40;
            carrier2.YLength = 500;
            carrier2.Grid = 4;

            Labware labware21 = new Labware();
            labware21.Name = "Labware21";
            labware21.XLength = 85;
            labware21.YLength = 125;
            labware21.Height = 45;
            labware21.WellRadius = 4;
            labware21.NumberOfWellsX = 8;
            labware21.NumberOfWellsY = 12;
            labware21.FirstWellPosition = new Point(8, 10);
            labware21.LastWellPosition = new Point(78, 118);
            labware21.ZTravel = 4200;
            labware21.ZStart = 4000;
            labware21.ZDispense = 3500;
            labware21.ZMax = 50;
            carrier2.AddLabware(labware21);

            Labware labware22 = new Labware();
            labware22.Name = "Labware22";
            labware22.XLength = 85;
            labware22.YLength = 125;
            labware22.Height = 45;
            labware22.WellRadius = 4;
            labware22.NumberOfWellsX = 8;
            labware22.NumberOfWellsY = 12;
            labware22.FirstWellPosition = new Point(8, 10);
            labware22.LastWellPosition = new Point(78, 118);
            labware22.ZTravel = 4200;
            labware22.ZStart = 4000;
            labware22.ZDispense = 3500;
            labware22.ZMax = 50;
            carrier2.AddLabware(labware22);

            layout.AddCarrier(carrier2);
            #endregion

            Debug.Assert(layout.Carriers.Count == 2);

            layout.Serialize(this._xmlFileWithCarrierPath);
        }

        [TestMethod]
        public void LayoutDeserializeFromXmlFileTest()
        {
            // With Carrier
            Layout layoutWithCarrier = Layout.Create(this._xmlFileWithCarrierPath);
            Assert.AreEqual<int>(layoutWithCarrier.Carriers.Count, 2);
            Assert.AreEqual<string>(layoutWithCarrier.Carriers[0].Name, "Carrier1");
            Assert.AreEqual<string>(layoutWithCarrier.Carriers[1].Name, "Carrier2");

            // Without Carrier
            Layout layoutWihtoutCarrier = Layout.Create(this._xmlFileWithoutCarrierPath);
            Assert.AreEqual<int>(layoutWihtoutCarrier.Carriers.Count, 0);
        }
    }
}