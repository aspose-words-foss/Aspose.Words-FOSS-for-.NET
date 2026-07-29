// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/10/2013 by Evgeny Ivanov

using Aspose.Words.Fields;
using Aspose.Words.Fields.Expressions;
using NUnit.Framework;

namespace Aspose.Words.Tests.Fields.Expressions
{
    /// <summary>
    /// Tests for <see cref="ComparisonEvaluator"/>
    /// </summary>
    [TestFixture]
    public class ComparisonEvaluatorTest
    {
        /// <summary>
        /// String with string should be compared correctly
        /// </summary>
        [Test]
        public void CompareStringWithStringTest()
        {
            // Arrange
            DocumentBuilder builder = new DocumentBuilder();
            FieldIf field = (FieldIf)builder.InsertField(FieldType.FieldIf, false);
            field.SetFieldCode("IF \"TEST\" = \"TEST\"");
            FieldCodeComparison comparison = new FieldCodeComparison(field.FieldCodeCache);

            // Act
            Constant result = ComparisonEvaluator.Evaluate(field, comparison);

            // Assert
            Assert.That(result, Is.InstanceOf(typeof(BooleanConstant)));
            Assert.That(result.ValueBoolean, Is.EqualTo(true));
        }

        /// <summary>
        /// Bookmark with string should be compared as bookmark string result and string
        /// </summary>
        [Test]
        public void CompareBookmarkWithStringTest()
        {
            // Arrange
            DocumentBuilder builder = new DocumentBuilder();
            builder.StartBookmark("TEST_BM");
            builder.Write("TEST");
            builder.EndBookmark("TEST_BM");

            FieldIf field = (FieldIf)builder.InsertField(FieldType.FieldIf, false);
            field.SetFieldCode("IF TEST_BM = \"TEST\"");
            FieldCodeComparison comparison = new FieldCodeComparison(field.FieldCodeCache);

            // Act
            Constant result = ComparisonEvaluator.Evaluate(field, comparison);

            // Assert
            Assert.That(result, Is.InstanceOf(typeof(BooleanConstant)));
            Assert.That(result.ValueBoolean, Is.EqualTo(true));
        }

        /// <summary>
        /// String with Bookmark should be compared as bookmark string result and string
        /// </summary>
        [Test]
        public void CompareStringWithBookmarkTest()
        {
            // Arrange
            DocumentBuilder builder = new DocumentBuilder();
            builder.StartBookmark("TEST_BM");
            builder.Write("TEST");
            builder.EndBookmark("TEST_BM");
            FieldIf field = (FieldIf)builder.InsertField(FieldType.FieldIf, false);
            field.SetFieldCode("IF \"TEST\" = TEST_BM ");
            FieldCodeComparison comparison = new FieldCodeComparison(field.FieldCodeCache);

            // Act
            Constant result = ComparisonEvaluator.Evaluate(field, comparison);

            // Assert
            Assert.That(result, Is.InstanceOf(typeof(BooleanConstant)));
            Assert.That(result.ValueBoolean, Is.EqualTo(true));
        }

        /// <summary>
        /// String in quotes with binary expressions should not be compared as doubles
        /// </summary>
        [Test]
        public void CompareStringWithBinaryExpressionTest()
        {
            // Arrange
            DocumentBuilder builder = new DocumentBuilder();
            FieldIf field = (FieldIf)builder.InsertField(FieldType.FieldIf, false);
            field.SetFieldCode("IF \"4\" = 2+2");
            FieldCodeComparison comparison = new FieldCodeComparison(field.FieldCodeCache);

            // Act
            Constant result = ComparisonEvaluator.Evaluate(field, comparison);

            // Assert
            Assert.That(result, Is.InstanceOf(typeof(BooleanConstant)));
            Assert.That(result.ValueBoolean, Is.EqualTo(false));
        }

        /// <summary>
        /// Double with binary expressions should be compared as doubles
        /// </summary>
        [Test]
        public void CompareDoubleWithBinaryExpressionTest()
        {
            // Arrange
            DocumentBuilder builder = new DocumentBuilder();
            FieldIf field = (FieldIf)builder.InsertField(FieldType.FieldIf, false);
            field.SetFieldCode("IF 4 = 2+2");
            FieldCodeComparison comparison = new FieldCodeComparison(field.FieldCodeCache);

            // Act
            Constant result = ComparisonEvaluator.Evaluate(field, comparison);

            // Assert
            Assert.That(result, Is.InstanceOf(typeof(BooleanConstant)));
            Assert.That(result.ValueBoolean, Is.EqualTo(true));
        }

        /// <summary>
        /// WORDSNET-11795 IF fields in bookmark text not resolving properly.
        /// </summary>
        [Test]
        [TestCase("\"AAA\" = \"AAA\"", true)]
        [TestCase("AAA = \"AAA\"", true)]
        [TestCase("\"AAA\" = AAA", true)]
        [TestCase("AAA = AAA", true)]
        [TestCase("\"BBB\" = \"BBB\"", true)]
        [TestCase("BBB = \"BBB\"", false)]
        [TestCase("\"BBB\" = BBB", false)]
        [TestCase("BBB = BBB", true)]
        [TestCase("\"bookmark text\" = \"BBB\"", false)]
        [TestCase("\"bookmark text\" = BBB", true)]
        [TestCase("\"BBB\" = \"bookmark text\"", false)]
        [TestCase("BBB = \"bookmark text\"", true)]
        [TestCase("\"AAA\" = \"BBB\"", false)]
        [TestCase("AAA = \"BBB\"", false)]
        [TestCase("\"AAA\" = BBB", false)]
        [TestCase("AAA = BBB", false)]
        [TestCase("\"BBB\" = \"AAA\"", false)]
        [TestCase("BBB = \"AAA\"", false)]
        [TestCase("\"BBB\" = AAA", false)]
        [TestCase("BBB = AAA", false)]
        [TestCase("\"AAA\" = \"CCC\"", false)]
        [TestCase("AAA = \"CCC\"", false)]
        [TestCase("\"AAA\" = CCC", false)]
        [TestCase("AAA = CCC", false)]
        [TestCase("\"CCC\" = \"AAA\"", false)]
        [TestCase("CCC = \"AAA\"", false)]
        [TestCase("\"CCC\" = AAA", false)]
        [TestCase("CCC = AAA", false)]
        [TestCase("\"BBB\" = \"DDD\"", false)]
        [TestCase("BBB = \"DDD\"", false)]
        [TestCase("\"BBB\" = DDD", false)]
        [TestCase("BBB = DDD", true)]
        [TestCase("\"4\" = 2*2", false)]
        [TestCase("4 = \"2*2\"", true)]
        [TestCase("\"4\" = \"2*2\"", false)]
        [TestCase("4 = 2*2", true)]
        [TestCase("\"2*2\" = 4", false)]
        [TestCase("2*2 = \"4\"", true)]
        [TestCase("\"2*2\" = \"4\"", false)]
        [TestCase("2*2 = 4", true)]
        [TestCase("EEE = \"this--is+(not/+^expression(\"", true)]
        [TestCase("\"EEE\" = \"this--is+(not/+^expression(\"", false)]
        [TestCase("\"this--is+(not/+^expression(\" = EEE", true)]
        [TestCase("\"this--is+(not/+^expression(\" = \"EEE\"", false)]
        [TestCase("BBB+3 = 3+BBB", false)]
        [TestCase("BBB+3 = BBB+3", true)]
        [TestCase("AAA+3 = 3+AAA", false)]
        [TestCase("AAA+3 = AAA+3", true)]
        [TestCase("\"BBB+3\" = 3+BBB", false)]
        [TestCase("\"BBB+3\" = BBB+3", true)]
        [TestCase("\"AAA+3\" = 3+AAA", false)]
        [TestCase("\"AAA+3\" = AAA+3", true)]
        [TestCase("BBB+3 = \"3+BBB\"", false)]
        [TestCase("BBB+3 = \"BBB+3\"", true)]
        [TestCase("AAA+3 = \"3+AAA\"", false)]
        [TestCase("AAA+3 = \"AAA+3\"", true)]
        [TestCase("FFF = 2+3", true)]
        [TestCase("\"1+2+3\" = GGG", false)]
        [TestCase("FFF = \"2+3\"", false)]
        [TestCase("\"1 2 3\" = GGG", true)]
        [TestCase("GGG = \"1 2 3\"", true)]
        [TestCase("\"6\" = GGG", false)]
        [TestCase("GGG = \"6\"", false)]
        [TestCase("GGG = \"1+2+3\"", false)]
        [TestCase("GGG = 1+2+3", true)]
        public void TestJira11795(string expression, bool expectedResult)
        {
            // Arrange
            DocumentBuilder builder = new DocumentBuilder();

            builder.StartBookmark("BBB");
            builder.StartBookmark("DDD");
            builder.Write("bookmark text");
            builder.EndBookmark("BBB");
            builder.EndBookmark("DDD");

            builder.StartBookmark("EEE");
            builder.Write("this--is+(not/+^expression(");
            builder.EndBookmark("EEE");

            builder.StartBookmark("FFF");
            builder.Write("5");
            builder.EndBookmark("FFF");

            builder.StartBookmark("GGG");
            builder.Write("1 2 3");
            builder.EndBookmark("GGG");

            builder.Writeln();

            Field field = builder.InsertField(string.Format("IF {0} ", expression), null);
            FieldCodeComparison comparison = new FieldCodeComparison(field.FieldCodeCache);

            // Act
            Constant result = ComparisonEvaluator.Evaluate(field, comparison);

            // Assert
            Assert.That(result, Is.InstanceOf(typeof(BooleanConstant)));
            Assert.That(result.ValueBoolean, Is.EqualTo(expectedResult));
        }

        /// <summary>
        /// WORDSNET-6498 BookmarkReference.Evaluate method potential bugs.
        /// </summary>
        [Test]
        [TestCase("6 = 6", true)]
        [TestCase("6 = \"6\"", true)]
        [TestCase("6 = test", true)]
        [TestCase("6 = \"test\"", false)]
        [TestCase("6 = \"1 2 3\"", true)]
        [TestCase("6 = 1+2+3", true)]
        [TestCase("6 = \"1+2+3\"", true)]
        [TestCase("6 = 2+4", true)]
        [TestCase("6 = \"2+4\"", true)]
        [TestCase("\"6\" = 6", true)]
        [TestCase("\"6\" = \"6\"", true)]
        [TestCase("\"6\" = test", false)]
        [TestCase("\"6\" = \"test\"", false)]
        [TestCase("\"6\" = \"1 2 3\"", false)]
        [TestCase("\"6\" = 1+2+3", false)]
        [TestCase("\"6\" = \"1+2+3\"", false)]
        [TestCase("\"6\" = 2+4", false)]
        [TestCase("\"6\" = \"2+4\"", false)]
        [TestCase("test = 6", true)]
        [TestCase("test = \"6\"", false)]
        [TestCase("test = test", true)]
        [TestCase("test = \"test\"", false)]
        [TestCase("test = \"1 2 3\"", true)]
        [TestCase("test = 1+2+3", true)]
        [TestCase("test = \"1+2+3\"", false)]
        [TestCase("test = 2+4", true)]
        [TestCase("test = \"2+4\"", false)]
        [TestCase("\"test\" = 6", false)]
        [TestCase("\"test\" = \"6\"", false)]
        [TestCase("\"test\" = test", false)]
        [TestCase("\"test\" = \"test\"", true)]
        [TestCase("\"test\" = \"1 2 3\"", false)]
        [TestCase("\"test\" = 1+2+3", false)]
        [TestCase("\"test\" = \"1+2+3\"", false)]
        [TestCase("\"test\" = 2+4", false)]
        [TestCase("\"test\" = \"2+4\"", false)]
        [TestCase("\"1 2 3\" = 6", false)]
        [TestCase("\"1 2 3\" = \"6\"", false)]
        [TestCase("\"1 2 3\" = test", true)]
        [TestCase("\"1 2 3\" = \"test\"", false)]
        [TestCase("\"1 2 3\" = \"1 2 3\"", true)]
        [TestCase("\"1 2 3\" = 1+2+3", false)]
        [TestCase("\"1 2 3\" = \"1+2+3\"", false)]
        [TestCase("\"1 2 3\" = 2+4", false)]
        [TestCase("\"1 2 3\" = \"2+4\"", false)]
        [TestCase("1+2+3 = 6", true)]
        [TestCase("1+2+3 = \"6\"", true)]
        [TestCase("1+2+3 = test", true)]
        [TestCase("1+2+3 = \"test\"", false)]
        [TestCase("1+2+3 = \"1 2 3\"", true)]
        [TestCase("1+2+3 = 1+2+3", true)]
        [TestCase("1+2+3 = \"1+2+3\"", true)]
        [TestCase("1+2+3 = 2+4", true)]
        [TestCase("1+2+3 = \"2+4\"", true)]
        [TestCase("\"1+2+3\" = 6", false)]
        [TestCase("\"1+2+3\" = \"6\"", false)]
        [TestCase("\"1+2+3\" = test", false)]
        [TestCase("\"1+2+3\" = \"test\"", false)]
        [TestCase("\"1+2+3\" = \"1 2 3\"", false)]
        [TestCase("\"1+2+3\" = 1+2+3", true)]
        [TestCase("\"1+2+3\" = \"1+2+3\"", true)]
        [TestCase("\"1+2+3\" = 2+4", false)]
        [TestCase("\"1+2+3\" = \"2+4\"", false)]
        [TestCase("2+4 = 6", true)]
        [TestCase("2+4 = \"6\"", true)]
        [TestCase("2+4 = test", true)]
        [TestCase("2+4 = \"test\"", false)]
        [TestCase("2+4 = \"1 2 3\"", true)]
        [TestCase("2+4 = 1+2+3", true)]
        [TestCase("2+4 = \"1+2+3\"", true)]
        [TestCase("2+4 = 2+4", true)]
        [TestCase("2+4 = \"2+4\"", true)]
        [TestCase("\"2+4\" = 6", false)]
        [TestCase("\"2+4\" = \"6\"", false)]
        [TestCase("\"2+4\" = test", false)]
        [TestCase("\"2+4\" = \"test\"", false)]
        [TestCase("\"2+4\" = \"1 2 3\"", false)]
        [TestCase("\"2+4\" = 1+2+3", false)]
        [TestCase("\"2+4\" = \"1+2+3\"", false)]
        [TestCase("\"2+4\" = 2+4", true)]
        [TestCase("\"2+4\" = \"2+4\"", true)]
        [TestCase("true = \"true\"", true)]
        [TestCase("false = \"false\"", true)]
        public void TestJira6498(string expression, bool expectedResult)
        {
            DocumentBuilder builder = new DocumentBuilder();

            builder.StartBookmark("test");
            builder.Write("1 2 3");
            builder.EndBookmark("test");

            Field field = builder.InsertField(string.Format("IF {0} ", expression), null);
            FieldCodeComparison comparison = new FieldCodeComparison(field.FieldCodeCache);

            Constant result = ComparisonEvaluator.Evaluate(field, comparison);

            Assert.That(result, Is.InstanceOf(typeof(BooleanConstant)));
            Assert.That(result.ValueBoolean, Is.EqualTo(expectedResult));
        }

        /// <summary>
        /// WORDSNET-23980 IF field with wildcard is updated improperly.
        /// </summary>
        [Test]
        [TestCase("1c", "\"1c\"", true)]
        [TestCase("\"1c\"", "\"1c\"", true)]
        [TestCase("\"1c\"", "1c", true)]
        [TestCase("1c", "1c", true)]
        [TestCase("2", "\"2*\"", true)]
        [TestCase("\"2\"", "\"2*\"", true)]
        [TestCase("\"2\"", "2*", false)]
        [TestCase("2", "2*", false)]
        [TestCase("a", "\"a*\"", true)]
        [TestCase("\"a\"", "\"a*\"", true)]
        [TestCase("\"a\"", "a*", true)]
        [TestCase("a", "a*", true)]
        [TestCase("1c", "\"1c*\"", true)]
        [TestCase("\"1c\"", "\"1c*\"", true)]
        [TestCase("\"1c\"", "1c*", false)]
        [TestCase("1c", "1c*", false)]
        [TestCase("\"12foo5\"", "\"4+8\"", false)]
        [TestCase("\"12foo5\"", "4+8", false)]
        [TestCase("\"12foo5\"", "\"4bar+8\"", false)]
        [TestCase("\"12foo5\"", "4bar+8", false)]
        [TestCase("12foo5", "\"4+bar8\"", false)]
        [TestCase("\"12foo5\"", "\"4+bar8\"", false)]
        [TestCase("\"12foo5\"", "4+bar8", false)]
        [TestCase("12foo5", "4+bar8", false)]
        [TestCase("\"2foo5\"", "\"2*5\"", true)]
        [TestCase("\"2foo5\"", "2*5", false)]
        [TestCase("2foo5", "2*5", false)]
        [TestCase("10blah", "\"2*5\"", true)]
        [TestCase("\"10blah\"", "\"2*5\"", false)]
        [TestCase("\"10blah\"", "2*5", false)]
        [TestCase("10blah", "2*5", true)]
        [TestCase("2foo*3", "\"2foo*3\"", true)]
        [TestCase("2foo*3", "\"2foo*\"", true)]
        [TestCase("2foo*3", "\"2foo3\"", false)]
        [TestCase("2foo*3", "\"2foo\"", false)]
        [TestCase("2foo*", "\"2foo*3\"", true)]
        [TestCase("2foo*", "\"2foo*\"", true)]
        [TestCase("2foo*", "\"2foo3\"", false)]
        [TestCase("2foo*", "\"2foo\"", false)]
        [TestCase("2foo3", "\"2foo*\"", true)]
        [TestCase("2foo3", "\"2foo3\"", true)]
        [TestCase("2foo", "\"2foo*3\"", false)]
        [TestCase("2foo", "\"2foo*\"", true)]
        [TestCase("2foo", "\"2foo\"", true)]
        [TestCase("3foo+3", "\"6\"", true)]
        [TestCase("2foo*4", "\"8\"", true)]
        [TestCase("3foo+3", "\"2foo*3\"", true)]
        [TestCase("2foo*4", "\"2foo*\"", true)]
        [TestCase("12foo5", "\"4+8\"", true, Ignore = "Not supported")]
        [TestCase("12foo5", "\"4bar+8\"", true, Ignore = "Not supported")]
        [TestCase("12foo5", "4+8", true, Ignore = "Not supported")]
        [TestCase("12foo5", "4bar+8", true, Ignore = "Not supported")]
        [TestCase("2foo5", "\"2*5\"", false, Ignore = "Not supported")]
        [TestCase("2foo3", "\"2foo*3\"", false, Ignore = "Not supported")]
        [TestCase("2foo3", "\"2foo\"", true, Ignore = "Not supported")]
        [TestCase("2foo", "\"2foo3\"", true, Ignore = "Not supported")]
        [TestCase("2foo13", "\"2foo*3\"", false, Ignore = "Not supported")]
        public void Test23980(string left, string right, bool expectedResult)
        {
            DocumentBuilder builder = new DocumentBuilder();
            Field field = builder.InsertField(string.Format("IF {0} = {1}", left, right), null);
            FieldCodeComparison comparison = new FieldCodeComparison(field.FieldCodeCache);

            Constant result = ComparisonEvaluator.Evaluate(field, comparison);

            Assert.That(result, Is.InstanceOf(typeof(BooleanConstant)));
            Assert.That(result.ValueBoolean, Is.EqualTo(expectedResult));
        }
    }
}
