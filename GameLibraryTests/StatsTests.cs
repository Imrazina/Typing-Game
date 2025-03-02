using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace GameLibrary.Tests
{
    [TestClass()]
    public class StatsTests
    {
        private Stats stats = new();

        [TestMethod()]
        public void NoHits()
        {
            Assert.AreEqual(0, stats.Correct);
            Assert.AreEqual(0, stats.Missed);
        }

        [TestMethod()]
        public void OneCorrectHit()
        {
            stats.Update(true);

            Assert.AreEqual(1, stats.Correct);
            Assert.AreEqual(0, stats.Missed);
            Assert.AreEqual(100, stats.Accuracy);
        }

        [TestMethod()]
        public void OneMissedHit()
        {
            stats.Update(false);

            Assert.AreEqual(0, stats.Correct);
            Assert.AreEqual(1, stats.Missed);
            Assert.AreEqual(0, stats.Accuracy);
        }

        [TestMethod()]
        public void OneCorrectOneMissedHit()
        {
            stats.Update(true);
            stats.Update(false);

            Assert.AreEqual(1, stats.Correct);
            Assert.AreEqual(1, stats.Missed);
            Assert.AreEqual(50, stats.Accuracy);
        }


        [TestMethod()]
        public void TwoCorrectFourMissedHit()
        {
            stats.Update(true);
            stats.Update(false);
            stats.Update(true);
            stats.Update(false);
            stats.Update(false);
            stats.Update(false);

            Assert.AreEqual(2, stats.Correct);
            Assert.AreEqual(4, stats.Missed);
            Assert.AreEqual(33, stats.Accuracy);
        }

        [TestMethod()]
        public void TwoCorrectNineMissedHit()
        {
            stats.Update(true);
            stats.Update(true);
            stats.Update(false);
            stats.Update(false);
            stats.Update(false);
            stats.Update(false);
            stats.Update(false);
            stats.Update(false);
            stats.Update(false);
            stats.Update(false);
            stats.Update(false);

            Assert.AreEqual(2, stats.Correct);
            Assert.AreEqual(9, stats.Missed);
            Assert.AreEqual(18, stats.Accuracy);
        }

        [TestMethod()]
        public void TwoCorrectTenMissedHit()
        {
            stats.Update(true);
            stats.Update(true);
            stats.Update(false);
            stats.Update(false);
            stats.Update(false);
            stats.Update(false);
            stats.Update(false);
            stats.Update(false);
            stats.Update(false);
            stats.Update(false);
            stats.Update(false);
            stats.Update(false);

            Assert.AreEqual(2, stats.Correct);
            Assert.AreEqual(10, stats.Missed);
            Assert.AreEqual(17, stats.Accuracy);
        }

        [TestMethod()]
        public void ResetMethodShouldResetStats()
        {
            stats.Update(true);
            stats.Update(true);
            stats.Update(false);
            stats.Update(false);
            stats.Update(false);

            stats.Reset();

            Assert.AreEqual(0, stats.Correct);
            Assert.AreEqual(0, stats.Missed);

            stats.Update(true);

            Assert.AreEqual(1, stats.Correct);
            Assert.AreEqual(0, stats.Missed);
            Assert.AreEqual(100, stats.Accuracy);
        }

        [TestMethod()]
        public void UpdatedStatsIsCalledOnCorrectHit()
        {
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            stats.UpdatedStats += (sender, args) =>
            {
                Assert.AreEqual(1, args.Correct);
                Assert.AreEqual(0, args.Missed);
                Assert.AreEqual(100, args.Accuracy);

                autoResetEvent.Set();
            };

            stats.Update(true);

            Assert.IsTrue(autoResetEvent.WaitOne(2000));
        }

        [TestMethod()]
        public void UpdatedStatsIsCalledOnMultipleHits()
        {
            int eventsCounted = 0;
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            stats.UpdatedStats += (sender, args) =>
            {
                eventsCounted++;
                autoResetEvent.Set();
            };
            
            stats.Update(true);

            Assert.IsTrue(autoResetEvent.WaitOne(2000));
            autoResetEvent.Reset();

            stats.Update(true);

            Assert.IsTrue(autoResetEvent.WaitOne(2000));
            autoResetEvent.Reset();

            stats.Update(false);

            Assert.IsTrue(autoResetEvent.WaitOne(2000));
            autoResetEvent.Reset();

            stats.Update(true);

            Assert.IsTrue(autoResetEvent.WaitOne(2000));
            autoResetEvent.Reset();

            Assert.AreEqual(4, eventsCounted);
        }
    }
}