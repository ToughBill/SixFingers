using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkstationController.Core.Data;

namespace WorkstationController.Core.UnitTest
{
    [TestClass]
    public class LiquidClassTests
    {
        private string _xmlFileLiquidClass = string.Empty;

        [TestInitialize]
        public void Initialization()
        {
            this._xmlFileLiquidClass = Path.Combine(UnitTestHelper.GetTestModuleDirectory(), "testresult", "LiquidClassSerializeTest.xml");
        }

        [TestMethod]
        public void LiquidClassSerializeTest()
        {
            LiquidClass liquidClass = new LiquidClass();
            liquidClass.Name = "LiquidClass Serialization Test";

            liquidClass.AspirationSinglePipetting.AspirationSpeed = 400;
            liquidClass.AspirationSinglePipetting.ConditioningVolume = 100;
            liquidClass.AspirationSinglePipetting.Delay = 200;
            liquidClass.AspirationSinglePipetting.ExcessVolume = 20;
            liquidClass.AspirationSinglePipetting.LeadingAirgap = 15;
            liquidClass.AspirationSinglePipetting.SystemTrailingAirgap = 10;
            liquidClass.AspirationSinglePipetting.TrailingAirgap = 50;


            liquidClass.DispenseSinglePipetting.DispenseSpeed = 300;
            liquidClass.DispenseSinglePipetting.Delay = 150;
            liquidClass.DispenseSinglePipetting.TrailingAirgapAfterDispense = true;

            liquidClass.Serialize(this._xmlFileLiquidClass);
        }

        [TestMethod]
        public void LiquidClassDeserializeTest()
        {
            LiquidClass liquidClass = LiquidClass.Create(this._xmlFileLiquidClass);

            Assert.AreEqual<int>(liquidClass.AspirationSinglePipetting.AspirationSpeed, 400);
            Assert.AreEqual<int>(liquidClass.AspirationSinglePipetting.ConditioningVolume, 100);
            Assert.AreEqual<int>(liquidClass.AspirationSinglePipetting.Delay, 200);
            Assert.AreEqual<int>(liquidClass.AspirationSinglePipetting.ExcessVolume, 20);
            Assert.AreEqual<int>(liquidClass.AspirationSinglePipetting.LeadingAirgap, 15);
            Assert.AreEqual<int>(liquidClass.AspirationSinglePipetting.SystemTrailingAirgap, 10);
            Assert.AreEqual<int>(liquidClass.AspirationSinglePipetting.TrailingAirgap, 50);

            Assert.AreEqual<int>(liquidClass.DispenseSinglePipetting.DispenseSpeed, 300);
            Assert.AreEqual<int>(liquidClass.DispenseSinglePipetting.Delay, 150);
            Assert.AreEqual<bool>(liquidClass.DispenseSinglePipetting.TrailingAirgapAfterDispense, true);
        }
    }
}
