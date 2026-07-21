// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/10/2013 by Evgeny Ivanov

using Aspose.Words.Fields;
using Aspose.Words.Fields.Expressions;
using NUnit.Framework;

namespace Aspose.Words.Tests.Fields.Expressions
{
    /// <summary>
    /// Tests for <see cref="ExpressionParser"/>
    /// </summary>
    [TestFixture]
    public class ExpressionParserTest : TestFieldsBase
    {
        /// <summary>
        /// Digits should be parsed as DoubleConstant
        /// </summary>
        [Test]
        public void DoubleConstantParseTest()
        {
            // Arrange
            Field fieldStub = new FieldUnknown();
            FieldContext fieldContext = new FieldContext(fieldStub);

            // Act
            ExecutionQueue executionQueue = ExpressionParser.Parse(fieldContext, "3", new FieldExpressionParserBehavior());

            // Assert
            Assert.That(executionQueue.Count, Is.EqualTo(1));

            IExecutionItem executionItem = executionQueue.Dequeue();
            Assert.That(executionItem, Is.InstanceOf(typeof(DoubleConstant)));
            Assert.That(((DoubleConstant)executionItem).ValueDouble, Is.EqualTo(3));
        }

        /// <summary>
        /// Addition expression should be parsed as 3 execution items: 2 double constants and 1 addition operation
        /// </summary>
        [Test]
        public void AdditionOperationParseTest()
        {
            // Arrange
            Field fieldStub = new FieldUnknown();
            FieldContext fieldContext = new FieldContext(fieldStub);

            // Act
            ExecutionQueue executionQueue = ExpressionParser.Parse(fieldContext, "3 + 5", new FieldExpressionParserBehavior());

            // Assert
            Assert.That(executionQueue.Count, Is.EqualTo(3));

            IExecutionItem executionItem = executionQueue.Dequeue();
            Assert.That(executionItem, Is.InstanceOf(typeof(DoubleConstant)));
            Assert.That(((DoubleConstant)executionItem).ValueDouble, Is.EqualTo(3));

            IExecutionItem executionItem2 = executionQueue.Dequeue();
            Assert.That(executionItem2, Is.InstanceOf(typeof(DoubleConstant)));
            Assert.That(((DoubleConstant)executionItem2).ValueDouble, Is.EqualTo(5));

            IExecutionItem executionItem3 = executionQueue.Dequeue();
            Assert.That(executionItem3, Is.InstanceOf(typeof(AdditionOperator)));
        }

        /// <summary>
        /// Equality expression, containing bookmark reference, should be parsed correctly
        /// </summary>
        [Test]
        public void EqualityOperatorWithBookmarkReferenceTest()
        {
            // Arrange
            DocumentBuilder builder = new DocumentBuilder();
            builder.StartBookmark("TEST_BM");
            builder.EndBookmark("TEST_BM");

            Field field = builder.InsertField(FieldType.FieldIf, true);
            FieldContext fieldContext = new FieldContext(field);

            // Act
            ExecutionQueue executionQueue = ExpressionParser.Parse(fieldContext, "TEST_BM = \"TEST\"", new FieldExpressionParserBehavior());

            // Assert
            Assert.That(executionQueue.Count, Is.EqualTo(3));

            IExecutionItem executionItem = executionQueue.Dequeue();
            Assert.That(executionItem, Is.InstanceOf(typeof(BookmarkReference)));

            IExecutionItem executionItem2 = executionQueue.Dequeue();
            Assert.That(executionItem2, Is.InstanceOf(typeof(StringConstant)));
            Assert.That(((StringConstant)executionItem2).ValueString, Is.EqualTo("\"TEST\""));

            IExecutionItem executionItem3 = executionQueue.Dequeue();
            Assert.That(executionItem3, Is.InstanceOf(typeof(EqualityOperator)));
        }

        /// <summary>
        /// Equality expression, containing bookmark reference, should be parsed correctly
        /// </summary>
        [Test]
        public void EqualityOperatorWithBookmarkReferenceInOperand2Test()
        {
            // Arrange
            DocumentBuilder builder = new DocumentBuilder();
            builder.StartBookmark("TEST_BM");
            builder.EndBookmark("TEST_BM");
            Field field = builder.InsertField(FieldType.FieldIf, true);
            FieldContext fieldContext = new FieldContext(field);

            // Act
            ExecutionQueue executionQueue = ExpressionParser.Parse(fieldContext, "\"TEST\" = TEST_BM", new FieldExpressionParserBehavior());

            // Assert
            Assert.That(executionQueue.Count, Is.EqualTo(3));

            IExecutionItem executionItem = executionQueue.Dequeue();
            Assert.That(executionItem, Is.InstanceOf(typeof(StringConstant)));
            Assert.That(((StringConstant)executionItem).ValueString, Is.EqualTo("\"TEST\""));

            IExecutionItem executionItem2 = executionQueue.Dequeue();
            Assert.That(executionItem2, Is.InstanceOf(typeof(BookmarkReference)));

            IExecutionItem executionItem3 = executionQueue.Dequeue();
            Assert.That(executionItem3, Is.InstanceOf(typeof(EqualityOperator)));
        }

        [Test]
        public void SpacingShouldSepareteOperandsTestCase1()
        {
            // arrange
            Field fieldStub = new FieldFormula();
            FieldContext fieldContext = new FieldContext(fieldStub);

            // act
            ExecutionQueue executionQueue = ExpressionParser.Parse(fieldContext, "=123 6/2", new FieldExpressionParserBehavior());

            // assert
            Assert.That(executionQueue.Count, Is.EqualTo(4));

            IExecutionItem item1 = executionQueue.Dequeue();
            Assert.That(item1, Is.InstanceOf(typeof(DoubleConstant)));
            Assert.That(((Constant)item1).ValueDouble, Is.EqualTo(123));

            IExecutionItem item2 = executionQueue.Dequeue();
            Assert.That(item2, Is.InstanceOf(typeof(DoubleConstant)));
            Assert.That(((Constant)item2).ValueDouble, Is.EqualTo(6));

            IExecutionItem item3 = executionQueue.Dequeue();
            Assert.That(item3, Is.InstanceOf(typeof(DoubleConstant)));
            Assert.That(((Constant)item3).ValueDouble, Is.EqualTo(2));

            IExecutionItem item4 = executionQueue.Dequeue();
            Assert.That(item4, Is.InstanceOf(typeof(DivisionOperator)));
        }

        [Test]
        public void SpacingShouldSepareteOperandsTestCase2()
        {
            // arrange
            Field fieldStub = new FieldFormula();
            FieldContext fieldContext = new FieldContext(fieldStub);

            // act
            ExecutionQueue executionQueue = ExpressionParser.Parse(fieldContext, "=123  321", new FieldExpressionParserBehavior());

            // assert
            Assert.That(executionQueue.Count, Is.EqualTo(2));

            IExecutionItem item1 = executionQueue.Dequeue();
            Assert.That(item1, Is.InstanceOf(typeof(DoubleConstant)));
            Assert.That(((Constant)item1).ValueDouble, Is.EqualTo(123));

            IExecutionItem item2 = executionQueue.Dequeue();
            Assert.That(item2, Is.InstanceOf(typeof(DoubleConstant)));
            Assert.That(((Constant)item2).ValueDouble, Is.EqualTo(321));
        }

        [Test]
        public void SpacingShouldSepareteOperandsTestCase3()
        {
            Field fieldStub = new FieldFormula();
            FieldContext fieldContext = new FieldContext(fieldStub);

            // act
            ExecutionQueue executionQueue = ExpressionParser.Parse(fieldContext, "= 1 2 ", new FieldExpressionParserBehavior());

            // assert
            Assert.That(executionQueue.Count, Is.EqualTo(2));

            IExecutionItem item1 = executionQueue.Dequeue();
            Assert.That(item1, Is.InstanceOf(typeof(DoubleConstant)));
            Assert.That(((Constant)item1).ValueDouble, Is.EqualTo(1));

            IExecutionItem item2 = executionQueue.Dequeue();
            Assert.That(item2, Is.InstanceOf(typeof(DoubleConstant)));
            Assert.That(((Constant)item2).ValueDouble, Is.EqualTo(2));
        }

        [Test]
        public void NormalExpressionTestCase1()
        {
            // arrange
            Field fieldStub = new FieldFormula();
            FieldContext fieldContext = new FieldContext(fieldStub);

            // act
            ExecutionQueue executionQueue = ExpressionParser.Parse(fieldContext, "=123 + 6 / 2", new FieldExpressionParserBehavior());

            // assert
            Assert.That(executionQueue.Count, Is.EqualTo(5));

            IExecutionItem item1 = executionQueue.Dequeue();
            Assert.That(item1, Is.InstanceOf(typeof(DoubleConstant)));
            Assert.That(((Constant)item1).ValueDouble, Is.EqualTo(123));

            IExecutionItem item2 = executionQueue.Dequeue();
            Assert.That(item2, Is.InstanceOf(typeof(DoubleConstant)));
            Assert.That(((Constant)item2).ValueDouble, Is.EqualTo(6));

            IExecutionItem item3 = executionQueue.Dequeue();
            Assert.That(item3, Is.InstanceOf(typeof(DoubleConstant)));
            Assert.That(((Constant)item3).ValueDouble, Is.EqualTo(2));

            IExecutionItem item4 = executionQueue.Dequeue();
            Assert.That(item4, Is.InstanceOf(typeof(DivisionOperator)));

            IExecutionItem item5 = executionQueue.Dequeue();
            Assert.That(item5, Is.InstanceOf(typeof(AdditionOperator)));
        }

        [Test]
        public void NormalExpressionTestCase2()
        {
            // arrange
            Field fieldStub = new FieldFormula();
            FieldContext fieldContext = new FieldContext(fieldStub);

            // act
            ExecutionQueue executionQueue = ExpressionParser.Parse(fieldContext, "=123 + 321", new FieldExpressionParserBehavior());

            // assert
            Assert.That(executionQueue.Count, Is.EqualTo(3));

            IExecutionItem item1 = executionQueue.Dequeue();
            Assert.That(item1, Is.InstanceOf(typeof(DoubleConstant)));
            Assert.That(((Constant)item1).ValueDouble, Is.EqualTo(123));

            IExecutionItem item2 = executionQueue.Dequeue();
            Assert.That(item2, Is.InstanceOf(typeof(DoubleConstant)));
            Assert.That(((Constant)item2).ValueDouble, Is.EqualTo(321));

            IExecutionItem item3 = executionQueue.Dequeue();
            Assert.That(item3, Is.InstanceOf(typeof(AdditionOperator)));
        }

        [Test]
        public void NormalExpressionTestCase3()
        {
            Field fieldStub = new FieldFormula();
            FieldContext fieldContext = new FieldContext(fieldStub);

            // act
            ExecutionQueue executionQueue = ExpressionParser.Parse(fieldContext, "= 1 + 2", new FieldExpressionParserBehavior());

            // assert
            Assert.That(executionQueue.Count, Is.EqualTo(3));

            IExecutionItem item1 = executionQueue.Dequeue();
            Assert.That(item1, Is.InstanceOf(typeof(DoubleConstant)));
            Assert.That(((Constant)item1).ValueDouble, Is.EqualTo(1));

            IExecutionItem item2 = executionQueue.Dequeue();
            Assert.That(item2, Is.InstanceOf(typeof(DoubleConstant)));
            Assert.That(((Constant)item2).ValueDouble, Is.EqualTo(2));

            IExecutionItem item3 = executionQueue.Dequeue();
            Assert.That(item3, Is.InstanceOf(typeof(AdditionOperator)));
        }

        [Test]
        public void CurrencySymbolCanBeSeparetedFromOperandTestCase1()
        {
            Field fieldStub = new FieldFormula();
            FieldContext fieldContext = new FieldContext(fieldStub);

            // act
            ExecutionQueue executionQueue = ExpressionParser.Parse(fieldContext, "= 1 + $ 2", new FieldExpressionParserBehavior());

            // assert
            Assert.That(executionQueue.Count, Is.EqualTo(3));

            IExecutionItem item1 = executionQueue.Dequeue();
            Assert.That(item1, Is.InstanceOf(typeof(DoubleConstant)));
            Assert.That(((Constant)item1).ValueDouble, Is.EqualTo(1));

            IExecutionItem item2 = executionQueue.Dequeue();
            Assert.That(item2, Is.InstanceOf(typeof(DoubleConstant)));
            Assert.That(((Constant)item2).ValueDouble, Is.EqualTo(2));
            Assert.That(((DoubleConstant)item2).IsCurrency, Is.True);

            IExecutionItem item3 = executionQueue.Dequeue();
            Assert.That(item3, Is.InstanceOf(typeof(AdditionOperator)));
        }

        [Test]
        public void CurrencySymbolCanBeSeparetedFromOperandTestCase2()
        {
            Field fieldStub = new FieldFormula();
            FieldContext fieldContext = new FieldContext(fieldStub);

            // act
            ExecutionQueue executionQueue = ExpressionParser.Parse(fieldContext,  "= 1 $ 2 ", new FieldExpressionParserBehavior());

            // assert
            Assert.That(executionQueue.Count, Is.EqualTo(2));

            IExecutionItem item1 = executionQueue.Dequeue();
            Assert.That(item1, Is.InstanceOf(typeof(DoubleConstant)));
            Assert.That(((Constant)item1).ValueDouble, Is.EqualTo(1));
            Assert.That(((DoubleConstant)item1).IsCurrency, Is.True);

            IExecutionItem item2 = executionQueue.Dequeue();
            Assert.That(item2, Is.InstanceOf(typeof(DoubleConstant)));
            Assert.That(((Constant)item2).ValueDouble, Is.EqualTo(2));
        }
    }
}
