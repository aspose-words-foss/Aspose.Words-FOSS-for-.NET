// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/12/2012 by Alexey Butalov

using System.Reflection;
using Aspose.JavaAttributes;
using Aspose.TestFx.GoldComparers.FailedTestCollector;
using NUnit.Framework;

namespace Aspose.Tests.Base
{
    [TestFixture]
    [JavaManual("Wait for porting FailedGoldTestStorageEngine.")]
    public class TestFailedGoldTestStorageEngine
    {
        [Test]
        public void TestGetTestMethod()
        {
            MethodBase testMethod = FailedGoldTestUtil.GetTestMethod();
            Assert.That(testMethod, IsNot.Null());
            Assert.That(testMethod.Name, Is.EqualTo("TestGetTestMethod"));
        }

        [Test]
        public void TestGetTestMethodFromThread()
        {
            ThreadWorker worker = new ThreadWorker();
            worker.Start();
            worker.Join();

            Assert.That(worker.TestName, Is.EqualTo("DoWork"));
        }

        private class ThreadWorker : ThreadPal
        {
            protected override void DoWork()
            {
                MethodBase testMethod = FailedGoldTestUtil.GetTestMethod();
                if (testMethod != null)
                    TestName = testMethod.Name;
            }

            internal string TestName = string.Empty;
        }
    }
}
