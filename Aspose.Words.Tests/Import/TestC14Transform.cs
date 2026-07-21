// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/10/2017 by Ilya Navrotskiy

using System.IO;
using Aspose.Words.RW.OfficeCrypto;
using NUnit.Framework;

namespace Aspose.Words.Tests.Import
{
    /// <summary>
    /// C14Transform tests.
    /// </summary>
    /// <remarks>
    /// https://www.w3.org/TR/2001/REC-xml-c14n-20010315
    /// </remarks>
    [TestFixture]
    public class TestC14Transform
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        /// <summary>
        /// Tests specification XML example - 3.1 PIs, Comments, and Outside of Document Element.
        /// </summary>
        [Test]
        public void TestSpecExample1()
        {
            CheckTransform(@"OfficeCrypto\c14Example1.xml");
        }

        /// <summary>
        /// Relates to TestTransformExample1().
        /// The input XML is slightly changed to check line breaks when element node is at the very start and
        /// also when there are two consequent PIs (processing instructions) in XML.
        /// </summary>
        [Test]
        public void TestSpecExample1A()
        {
            CheckTransform(@"OfficeCrypto\c14Example1A.xml");
        }

        /// <summary>
        /// Relates to TestTransformExample1() 
        /// The difference is prefixes added to XML element nodes and also made one nested XML element with name prefix.
        /// </summary>
        [Test]
        public void TestSpecExample1B()
        {
            CheckTransform(@"OfficeCrypto\c14Example1B.xml");
        }

        /// <summary>
        /// Tests specification XML example - 3.2 Whitespace in Document Content.
        /// </summary>
        [Test]
        public void TestSpecExample2()
        {
            CheckTransform(@"OfficeCrypto\c14Example2.xml");
        }

        /// <summary>
        /// Tests specification XML example - 3.3 Start and End Tags.
        /// </summary>
        [Test, Ignore("Specification example file contains [<!ATTLIST e9 attr CDATA 'default'>] element that cannot be resolved correctly with AnyXmlReader, so skip it for a while")]
        public void TestSpecExample3()
        {
            CheckTransform(@"OfficeCrypto\c14Example3.xml");
        }

        /// <summary>
        /// Relates to TestSpecExample3(), but it is without problematic !ATTLIST element. 
        /// </summary>
        [Test]
        public void TestSpecExample3A()
        {
            CheckTransform(@"OfficeCrypto\c14Example3A.xml");
        }

        /// <summary>
        /// Tests specification XML example - 3.4 Character Modifications and Character References.
        /// </summary>
        [Test, Ignore("Character references are not canonized good now, so skip it for a while as it is not a frequent case.")]
        public void TestSpecExample4()
        {
            CheckTransform(@"OfficeCrypto\c14Example4.xml");
        }

        /// <summary>
        /// Tests specification XML example - 3.5 Entity References.
        /// </summary>
        [Test]
        public void TestSpecExample5()
        {
            CheckTransform(@"OfficeCrypto\c14Example5.xml");
        }

        /// <summary>
        /// Tests propagating namespace.
        /// </summary>
        [Test]
        public void TestPropagatingNamespace()
        {
            CheckTransform(@"OfficeCrypto\c14PropagateNs.xml", "propagated namespace");
        }

        /// <summary>
        /// Checks that AW c14 canonical transformation equals to transformation produced with DotNet library.
        /// </summary>
        private static void CheckTransform(string inputXml)
        {
            CheckTransform(inputXml, null);
        }

        /// <summary>
        /// Checks that AW c14 canonical transformation equals to transformation produced with DotNet library.
        /// Propagates specified namespace.
        /// </summary>
        private static void CheckTransform(string inputXml, string propagatingNamespace)
        {
            string testFileName = TestUtil.BuildTestFileName(inputXml);
            using (Stream xml = new FileStream(testFileName, FileMode.Open, FileAccess.Read))
            {
                string outFileName = TestUtil.BuildOutFileName(testFileName, "", SaveFormat.Unknown);

                C14Transform c14Transform = new C14Transform();
                c14Transform.PropagateNamespace(propagatingNamespace);

                TestUtil.EnsureDirectoryForFileExists(outFileName);
                File.WriteAllBytes(outFileName, c14Transform.Apply(xml).ToArray());

                string goldFileName = TestUtil.BuildGoldFileName(testFileName, "", SaveFormat.Unknown);
                TestUtil.VerifyGoldText(testFileName, outFileName, goldFileName, null, GoldLevel.ExportOnly, null);
            }
        }
    }
}
