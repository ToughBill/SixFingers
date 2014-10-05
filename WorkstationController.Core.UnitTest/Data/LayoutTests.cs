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
            carrier1.Label = "Carrier1";
            carrier1.XLength = 40;
            carrier1.YLength = 500;
            carrier1.Grid = 1;

            Labware labware11 = new Labware();
            labware11.TypeName = "Labware11";
            labware11.Dimension.XLength = 800;
            labware11.Dimension.YLength = 125;
            labware11.WellsInfo.WellRadius = 4;
            labware11.WellsInfo.NumberOfWellsX = 8;
            labware11.WellsInfo.NumberOfWellsY = 12;
            labware11.WellsInfo.FirstWellPosition = new Point(8, 10);
            labware11.WellsInfo.LastWellPosition = new Point(78, 118);
            //value bigger => lower
            labware11.ZValues.ZTravel = 300;
            labware11.ZValues.ZStart = 600;
            labware11.ZValues.ZDispense = 1000;
            labware11.ZValues.ZMax = 1600;
            carrier1.AddLabware(labware11);

            Labware labware12 = new Labware();
            labware11.TypeName = "Labware12";
            labware11.Dimension.XLength = 800;
            labware11.Dimension.YLength = 125;
            labware11.WellsInfo.WellRadius = 4;
            labware11.WellsInfo.NumberOfWellsX = 8;
            labware11.WellsInfo.NumberOfWellsY = 12;
            labware11.WellsInfo.FirstWellPosition = new Point(8, 10);
            labware11.WellsInfo.LastWellPosition = new Point(78, 118);
            //value bigger => lower
            labware11.ZValues.ZTravel = 300;
            labware11.ZValues.ZStart = 600;
            labware11.ZValues.ZDispense = 1000;
            labware11.ZValues.ZMax = 1600;
            carrier1.AddLabware(labware12);

            layout.AddCarrier(carrier1);
            #endregion

            #region Add Carrier2
            Carrier carrier2 = new Carrier();
            carrier2.Label = "Carrier2";
            carrier2.XLength = 40;
            carrier2.YLength = 500;
            carrier2.Grid = 4;

            Labware labware21 = new Labware();
            labware21.TypeName = "Labware21";
            labware21.Dimension.XLength = 800;
            labware21.Dimension.YLength = 125;
            labware21.WellsInfo.WellRadius = 4;
            labware21.WellsInfo.NumberOfWellsX = 8;
            labware21.WellsInfo.NumberOfWellsY = 12;
            labware21.WellsInfo.FirstWellPosition = new Point(8, 10);
            labware21.WellsInfo.LastWellPosition = new Point(78, 118);
            //value bigger => lower
            labware21.ZValues.ZTravel = 300;
            labware21.ZValues.ZStart = 600;
            labware21.ZValues.ZDispense = 1000;
            labware21.ZValues.ZMax = 1600;
            carrier2.AddLabware(labware21);

            Labware labware22 = new Labware();
            labware22.TypeName = "labware22";
            labware22.Dimension.XLength = 800;
            labware22.Dimension.YLength = 125;
            labware22.WellsInfo.WellRadius = 4;
            labware22.WellsInfo.NumberOfWellsX = 8;
            labware22.WellsInfo.NumberOfWellsY = 12;
            labware22.WellsInfo.FirstWellPosition = new Point(8, 10);
            labware22.WellsInfo.LastWellPosition = new Point(78, 118);
            //value bigger => lower
            labware22.ZValues.ZTravel = 300;
            labware22.ZValues.ZStart = 600;
            labware22.ZValues.ZDispense = 1000;
            labware22.ZValues.ZMax = 1600;
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
            Assert.AreEqual<string>(layoutWithCarrier.Carriers[0].Label, "Carrier1");
            Assert.AreEqual<string>(layoutWithCarrier.Carriers[1].Label, "Carrier2");

            // Without Carrier
            Layout layoutWihtoutCarrier = Layout.Create(this._xmlFileWithoutCarrierPath);
            Assert.AreEqual<int>(layoutWihtoutCarrier.Carriers.Count, 0);
        }
    }
}