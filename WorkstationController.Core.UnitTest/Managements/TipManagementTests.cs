using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkstationController.Core.Data;
using WorkstationController.Core.Managements;

namespace WorkstationController.Core.UnitTest.Managements
{
    [TestClass]
    public class TipManagementTests
    {
        [TestInitialize]
        public void Initialization()
        {

        }

        [TestMethod]
        public void NextBoxAlsoRunOut() //when one diti box has run out, go to next, if nex ditibox is has also run out, throw no tip exception
        {
            //TipManagement

            Layout layout = CreateDitiLayout();
            for (int i = 0; i < layout.DitiInfo.DitiBoxInfos.Count; i++ )
                layout.DitiInfo.DitiBoxInfos[i].count = 0;
            layout.Serialize(_xmlFilePath);
            TipManagement tipManagement = new TipManagement(layout);
            try
            {
                tipManagement.GetTip(DitiType.OneK,1);
            }
            catch (NoEngouhDitiException ex)
            {
                Assert.AreEqual("diti2",ex.LabwareTrait.Label);
                Assert.AreEqual(ex.NeedTipCount, 1);
                return;
            }
            Assert.Fail();
        }


        [TestMethod]
        public void LastBoxHasTips() //when one diti box has run out, go to next, 
        {
            //TipManagement

            Layout layout = CreateDitiLayout();
            TipManagement tipManagement = new TipManagement(layout);
            for (int i = 0; i < 193; i++)
                tipManagement.GetTip(DitiType.OneK, 1);
            string currentDitiBox = tipManagement.GetCurrentDitiBox(DitiType.OneK).Label;
            Assert.AreEqual("diti3", currentDitiBox);
        }

        [TestMethod]
        public void NextBoxHasTips_Diti1000Diti200Mix() //when one diti box has run out, go to next, 
        {
            //TipManagement

            Layout layout = CreateDitiLayoutMix();
            TipManagement tipManagement = new TipManagement(layout);
            for (int i = 0; i < 97; i++)
                tipManagement.GetTip(DitiType.OneK, 1);
            string currentDitiBox = tipManagement.GetCurrentDitiBox(DitiType.OneK).Label;
            Assert.AreEqual("diti3", currentDitiBox);
        }

        [TestMethod]
        public void NextBoxHas200Tips() //when one diti box has run out, go to next, 
        {
            //TipManagement

            Layout layout = CreateDitiLayout();
            TipManagement tipManagement = new TipManagement(layout);
            try
            {
                tipManagement.GetTip(DitiType.TwoHundred, 1);
            }
            catch (NoDitiBoxException ex)
            {
                Assert.AreEqual(DitiType.TwoHundred, ex.DitiType);
            }
            
            
        }


        [TestMethod]
        public void NextBoxHasTips() //when one diti box has run out, go to next, 
        {
            //TipManagement

            Layout layout = CreateDitiLayout();
            TipManagement tipManagement = new TipManagement(layout);
            for (int i = 0; i < 97; i++ )
                tipManagement.GetTip(DitiType.OneK, 1);
            string currentDitiBox = tipManagement.GetCurrentDitiBox(DitiType.OneK).Label;
            Assert.AreEqual("diti2", currentDitiBox);
        }
        [TestMethod]
        public void AllDitiBoxRunOutStartsFromFirstMix()
        {
            var layout = CreateDitiLayoutMix();
            TipManagement tipManagement = new TipManagement(layout);
            try
            {
                for (int i = 0; i < 96 * 2 + 1; i++)
                    tipManagement.GetTip(DitiType.OneK, 1);
            }
            catch (NoEngouhDitiException ex)
            {
                Assert.AreEqual("diti1", ex.LabwareTrait.Label);
                Assert.AreEqual("diti1", tipManagement.GetCurrentDitiBox(DitiType.OneK).Label);
                return;
            }
            Assert.Fail();
        }


        [TestMethod]
        public void AllDitiBoxRunOutStartsFromFirst()
        {
            var layout = CreateDitiLayout();
            TipManagement tipManagement = new TipManagement(layout);
            try
            {
                for (int i = 0; i < 96*3+1; i++)
                    tipManagement.GetTip(DitiType.OneK,1);
            }
            catch(NoEngouhDitiException ex)
            {
                Assert.AreEqual( "diti1",ex.LabwareTrait.Label);
                Assert.AreEqual("diti1", tipManagement.GetCurrentDitiBox(DitiType.OneK).Label);
                return;
            }
            Assert.Fail();
        }

        [TestMethod]
        public void Use3DitiBoxes()
        {
            var layout = CreateDitiLayout();
            DitiInfo ditiInfo = new DitiInfo();
            TipManagement tipManagement = new TipManagement(layout);
            try
            {
                for (int i = 0; i < 96*3; i++)
                    tipManagement.GetTip(DitiType.OneK,1);
                try
                {
                    tipManagement.GetTip(DitiType.OneK,1);
                }
                catch(NoEngouhDitiException ex)
                {
                    layout.DitiInfo.DitiBoxInfos[0].count = 96;
                    tipManagement.GetTip(DitiType.OneK,1);
                    Assert.AreEqual(layout.DitiInfo.DitiBoxInfos[0].count, 95);
                    Assert.AreEqual(tipManagement.GetCurrentDitiBox(DitiType.OneK).Label, "diti1");
                    return;
                }
            }
            catch (NoEngouhDitiException ex)
            {
                Assert.AreEqual(ex.LabwareTrait.Label, "diti2");
                Assert.AreEqual(tipManagement.GetCurrentDitiBox(DitiType.OneK), "diti2");
                return;
            }

            Assert.Fail();

        }

        private Layout CreateMP3Layout()
        {
            var diti1 = CreateDiti1000("plate1", 1);
            var diti2 = CreateDiti1000("plate2", 2);
            var diti3 = CreateDiti1000("plate3", 3);
            Layout layout = new Layout();
            Carrier ditiCarrier = new Carrier(BuildInCarrierType.Diti);
            ditiCarrier.AddLabware(diti1);
            ditiCarrier.AddLabware(diti2);
            ditiCarrier.AddLabware(diti3);
            ditiCarrier.GridID = 1;
            layout.AddCarrier(ditiCarrier);
            _xmlFilePath = Path.Combine(UnitTestHelper.GetTestModuleDirectory(), "testresult", "TestRecipe.xml");
            layout.Serialize(_xmlFilePath);
            return layout;
        }

        private Layout CreateDitiLayout()
        {
            var diti1 = CreateDiti1000("diti1", 1);
            var diti2 = CreateDiti1000("diti2", 2);
            var diti3 = CreateDiti1000("diti3", 3);
            Layout layout = new Layout();
            Carrier ditiCarrier = new Carrier(BuildInCarrierType.Diti);
            ditiCarrier.AddLabware(diti1);
            ditiCarrier.AddLabware(diti2);
            ditiCarrier.AddLabware(diti3);
            ditiCarrier.GridID = 1;
            layout.AddCarrier(ditiCarrier);
            _xmlFilePath = Path.Combine(UnitTestHelper.GetTestModuleDirectory(), "testresult", "TestRecipe.xml");
            layout.Serialize(_xmlFilePath);
            return layout;
        }

        private Layout CreateDitiLayoutMix()
        {
            var diti1 = CreateDiti1000("diti1", 1);
            var diti2 = CreateDiti200("diti2", 2);
            var diti3 = CreateDiti1000("diti3", 3);
            Layout layout = new Layout();
            Carrier ditiCarrier = new Carrier(BuildInCarrierType.Diti);
            ditiCarrier.AddLabware(diti1);
            ditiCarrier.AddLabware(diti2);
            ditiCarrier.AddLabware(diti3);
            ditiCarrier.GridID = 1;
            layout.AddCarrier(ditiCarrier);
            _xmlFilePath = Path.Combine(UnitTestHelper.GetTestModuleDirectory(), "testresult", "TestRecipe.xml");
            layout.Serialize(_xmlFilePath);
            return layout;
        }

      


        public Labware CreateMicroPlate(string label, int site)
        {
            Labware labware = new Labware();
            labware.TypeName = LabwareBuildInType.Plate96_05ML.ToString();
            labware.Dimension.XLength = 125.5;
            labware.Dimension.YLength = 85;
            labware.WellsInfo.WellRadius = 4;
            labware.WellsInfo.NumberOfWellsX = 8;
            labware.WellsInfo.NumberOfWellsY = 12;
            labware.WellsInfo.FirstWellPositionX = 8;
            labware.WellsInfo.FirstWellPositionY = 10;
            labware.WellsInfo.LastWellPositionX = 78;
            labware.WellsInfo.LastWellPositionX = 118;
            labware.SiteID = site;
            labware.ZValues.ZTravel = 300;
            labware.ZValues.ZStart = 600;
            labware.ZValues.ZDispense = 1000;
            labware.ZValues.ZMax = 1600;
            labware.Label = label;
            return labware;
        }

        private Labware CreateDiti200(string label, int site)
        {
            Labware labware = new Labware();
            labware.TypeName = LabwareBuildInType.Diti200.ToString();
            labware.Dimension.XLength = 125.5;
            labware.Dimension.YLength = 85;
            labware.WellsInfo.WellRadius = 4;
            labware.WellsInfo.NumberOfWellsX = 8;
            labware.WellsInfo.NumberOfWellsY = 12;
            labware.WellsInfo.FirstWellPositionX = 8;
            labware.WellsInfo.FirstWellPositionY = 10;
            labware.WellsInfo.LastWellPositionX = 78;
            labware.WellsInfo.LastWellPositionX = 118;
            labware.SiteID = site;
            labware.ZValues.ZTravel = 300;
            labware.ZValues.ZStart = 600;
            labware.ZValues.ZDispense = 1000;
            labware.ZValues.ZMax = 1600;
            labware.Label = label;
            return labware;
        }

        public Labware CreateDiti1000(string label, int site)
        {
            Labware labware = new Labware();
            labware.TypeName = LabwareBuildInType.Diti1000.ToString();
            labware.Dimension.XLength = 125.5;
            labware.Dimension.YLength = 85;
            labware.WellsInfo.WellRadius = 4;
            labware.WellsInfo.NumberOfWellsX = 8;
            labware.WellsInfo.NumberOfWellsY = 12;
            labware.WellsInfo.FirstWellPositionX = 8;
            labware.WellsInfo.FirstWellPositionY = 10;
            labware.WellsInfo.LastWellPositionX = 78;
            labware.WellsInfo.LastWellPositionX = 118;
            labware.SiteID = site; 
            labware.ZValues.ZTravel = 300;
            labware.ZValues.ZStart = 600;
            labware.ZValues.ZDispense = 1000;
            labware.ZValues.ZMax = 1600;
            labware.Label = label;
            return labware;
        }

        public string _xmlFilePath { get; set; }
    }
}
