﻿using System;
using System.IO;
using System.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkstationController.Core.Data;

namespace WorkstationController.Core.UnitTest
{
    [TestClass]
    public class WorktableTests
    {
        private string _xmlFileWorktablePath = string.Empty;

        [TestInitialize]
        public void Initialization()
        {
            this._xmlFileWorktablePath = Path.Combine(UnitTestHelper.GetTestModuleDirectory(), "testresult", "WorktableSerializeTest.xml");
        }

        [TestMethod]
        public void WorktableToXmlFileTest()
        {
            Worktable worktable = new Worktable();

            worktable.Size = new Size(2000, 1000);
            worktable.FirstRowPinSize = new Size(5, 10);
            worktable.SecondRowPinSize = new Size(3, 8);
            worktable.ThirdRowPinSize = new Size(3, 8);
            worktable.FirstPinPosition = new Point(100, 80);
            worktable.YPosRows2Pin = 400;
            worktable.YPosRows3Pin = 800;

            worktable.Serialize(this._xmlFileWorktablePath);
        }

        [TestMethod]
        public void WorktableFromXmlFileTest()
        {
            Worktable worktable = Worktable.Create(this._xmlFileWorktablePath);

            Assert.AreEqual<Size>(worktable.Size, new Size(2000, 1000));
            Assert.AreEqual<Point>(worktable.FirstPinPosition, new Point(100, 80));
            Assert.AreEqual<double>(worktable.YPosRows3Pin, 800);
        }
    }
}