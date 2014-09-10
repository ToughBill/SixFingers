using System.IO;
using System.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkstationController.Core.Data;
using WorkstationController.Core.Utility;

namespace WorkstationController.Core.UnitTest
{
    [TestClass]
    public class SerializationHelperTests
    {
        [TestMethod]
        public void LabwareSerializationTest()
        {
            Labware labware = new Labware();
            labware.Name = "LabwareSerializeTest";
            labware.XLength = 85;
            labware.YLength = 125;
            labware.Height = 45;
            labware.WellRadius = 4;
            labware.NumberOfWellsX = 8;
            labware.NumberOfWellsY = 12;
            labware.PositionOnCarrier = new Point(5, 12);
            labware.FirstWellPosition = new Point(8, 10);
            labware.LastWellPosition = new Point(78, 118);
            labware.ZTravel = 4200;
            labware.ZStart = 4000;
            labware.ZDispense = 3500;
            labware.ZMax = 50;

            string xmlFileName = Path.Combine(UnitTestHelper.GetTestModuleDirectory(), "testresult", "LabwareSerializeTest.xml");

            SerializationHelper.Serialize<Labware>(xmlFileName, labware);
            Labware labware_deserialized = SerializationHelper.Deserialize<Labware>(xmlFileName);
        }

        [TestMethod]
        public void CarrierNoLabwareSerializationTest()
        {
            Carrier carrier = new Carrier();
            carrier.Name = "CarrierSerializeTest";
            carrier.XLength = 40;
            carrier.YLength = 500;
            carrier.AllowedLabwareNumber = 3;
            carrier.AllowedLabwareType = 2;
            carrier.PositionOnWorktable = new Point(200, 120);

            string xmlFileName = Path.Combine(UnitTestHelper.GetTestModuleDirectory(), "testresult", "CarrierNoLabwareSerializeTest.xml");

            SerializationHelper.Serialize<Carrier>(xmlFileName, carrier);
            Carrier carrier_deserialized = SerializationHelper.Deserialize<Carrier>(xmlFileName);
        }

        [TestMethod]
        public void CarrierWithLabwaresSerializationTest()
        {
            Carrier carrier = new Carrier();
            carrier.Name = "CarrierSerializeTest";
            carrier.XLength = 40;
            carrier.YLength = 500;
            carrier.AllowedLabwareNumber = 3;
            carrier.AllowedLabwareType = 2;
            carrier.PositionOnWorktable = new Point(200, 120);

            #region Labware1
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
            #endregion

            #region Labware2
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
            #endregion

            string xmlFileName = Path.Combine(UnitTestHelper.GetTestModuleDirectory(), "testresult", "CarrierWithLabwaresSerializeTest.xml");

            SerializationHelper.Serialize<Carrier>(xmlFileName, carrier);
            Carrier carrier_deserialized = SerializationHelper.Deserialize<Carrier>(xmlFileName);
        }

        [TestMethod]
        public void LayoutWithCarrierSerializationTest()
        {
            // Test in LayoutTest.cs
        }

        [TestMethod]
        public void LayoutNoCarrierSerializationTest()
        {
            // Test in LayoutTest.cs
        }
    }
}
