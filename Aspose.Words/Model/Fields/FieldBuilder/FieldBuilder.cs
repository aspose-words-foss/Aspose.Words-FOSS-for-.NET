// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/01/2016 by Edward Voronov

using System;
using System.Collections.Generic;
using Aspose.Common;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Builds a field from field code tokens (arguments and switches).
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    public class FieldBuilder : IFieldBuildingBlock
    {
        /// <summary>
        /// Initializes an instance of the <see cref="FieldBuilder"/> class.
        /// </summary>
        /// <param name="fieldType">The type of the field to build.</param>
        public FieldBuilder(FieldType fieldType)
        {
            string fieldCode = FieldUtil.FetchFieldCodeSafe(fieldType);
            if (fieldCode == null)
                throw new ArgumentException(string.Format("Field type '{0}' is not supported.", FieldUtil.FieldTypeToString(fieldType)));

            mStart = new FieldStartFieldBuildingBlock(fieldType);
            mEnd = new FieldEndFieldBuildingBlock(fieldType);

            mBuildingBlocks = new List<IFieldBuildingBlock>();
            mBuildingBlocks.Add(mStart);
            mBuildingBlocks.Add(DelimiterFieldBuildingBlock.Instance);
            mBuildingBlocks.Add(new TextFieldBuildingBlock(fieldCode));
            mBuildingBlocks.Add(DelimiterFieldBuildingBlock.Instance);
            mBuildingBlocks.Add(mEnd);
        }

        /// <summary>
        /// Adds a field's argument.
        /// </summary>
        /// <param name="argument">The argument value.</param>
        public FieldBuilder AddArgument(string argument)
        {
            string encodedArgument = FieldCodeUpdater.EncodeArgument(argument);

            AppendBuildingBlockWithDelimiter(new TextFieldBuildingBlock(encodedArgument));

            return this;
        }

        /// <summary>
        /// Adds a field's argument.
        /// </summary>
        /// <param name="argument">The argument value.</param>
        public FieldBuilder AddArgument(int argument)
        {
            return AddArgument(FormatterPal.IntToStrCurrentCulture(argument));
        }

        /// <summary>
        /// Adds a field's argument.
        /// </summary>
        /// <param name="argument">The argument value.</param>
        public FieldBuilder AddArgument(double argument)
        {
            return AddArgument(FormatterPal.DoubleToStrCurrentCulture(argument));
        }

        /// <summary>
        /// Adds a child field represented by another <see cref="FieldBuilder"/> to the field's code.
        /// </summary>
        /// <remarks>
        /// This overload is used when the argument consists of a single child field.
        /// </remarks>
        public FieldBuilder AddArgument(FieldBuilder argument)
        {
            AppendBuildingBlockWithDelimiter(argument);

            return this;
        }

        /// <summary>
        /// Adds a field's argument represented by <see cref="FieldArgumentBuilder"/> to the field's code.
        /// </summary>
        /// <remarks>
        /// This overload is used when the argument consists of a mixture of different parts such as child fields, nodes, and plain text.
        /// </remarks>
        public FieldBuilder AddArgument(FieldArgumentBuilder argument)
        {
            AppendBuildingBlockWithQuotesAndDelimiter(argument);

            return this;
        }

        /// <summary>
        /// Adds a field's switch.
        /// </summary>
        /// <remarks>
        /// This overload adds a flag (switch without argument).
        /// </remarks>
        /// <param name="switchName">The switch name.</param>
        public FieldBuilder AddSwitch(string switchName)
        {
            return AddSwitch(switchName, null);
        }

        /// <summary>
        /// Adds a field's switch.
        /// </summary>
        /// <param name="switchName">The switch name.</param>
        /// <param name="switchArgument">The switch value.</param>
        public FieldBuilder AddSwitch(string switchName, string switchArgument)
        {
            const string backslash = @"\";

            string switchStr = StringUtil.StartsWithOrdinalIgnoreCase(switchName, backslash)
                                 ? switchName
                                 : backslash + switchName;

            AppendBuildingBlockWithDelimiter(new TextFieldBuildingBlock(switchStr));

            if (StringUtil.HasChars(switchArgument))
                AddArgument(switchArgument);

            return this;
        }

        /// <summary>
        /// Adds a field's switch.
        /// </summary>
        /// <param name="switchName">The switch name.</param>
        /// <param name="switchArgument">The switch value.</param>
        public FieldBuilder AddSwitch(string switchName, int switchArgument)
        {
            return AddSwitch(switchName, FormatterPal.IntToStrCurrentCulture(switchArgument));
        }

        /// <summary>
        /// Adds a field's switch.
        /// </summary>
        /// <param name="switchName">The switch name.</param>
        /// <param name="switchArgument">The switch value.</param>
        public FieldBuilder AddSwitch(string switchName, double switchArgument)
        {
            return AddSwitch(switchName, FormatterPal.DoubleToStrCurrentCulture(switchArgument));
        }

        /// <summary>
        /// Builds and inserts a field into the document before the specified inline node.
        /// </summary>
        /// <returns>A <see cref="Field"/> object that represents the inserted field.</returns>
        public Field BuildAndInsert(Inline refNode)
        {
            return BuildAndInsertInternal(refNode);
        }

        /// <summary>
        /// Builds and inserts a field into the document to the end of the specified paragraph.
        /// </summary>
        /// <returns>A <see cref="Field"/> object that represents the inserted field.</returns>
        public Field BuildAndInsert(Paragraph refNode)
        {
            return BuildAndInsertInternal(refNode);
        }

        private Field BuildAndInsertInternal(Node refNode)
        {
            DocumentBuilder documentBuilder = new DocumentBuilder(refNode.FetchDocument());
            documentBuilder.MoveTo(refNode);

            ((IFieldBuildingBlock)this).BuildBlock(documentBuilder);

            return FieldFactory.CreateField(mStart.FieldStart, mEnd.FieldSeparator, mEnd.FieldEnd);
        }

        void IFieldBuildingBlock.BuildBlock(DocumentBuilder documentBuilder)
        {
            foreach (IFieldBuildingBlock buildingBlock in mBuildingBlocks)
                buildingBlock.BuildBlock(documentBuilder);
        }

        private void AppendBuildingBlock(IFieldBuildingBlock buildingBlock)
        {
            mBuildingBlocks.Insert(mBuildingBlocks.Count - 1, buildingBlock);
        }

        private void AppendBuildingBlockWithDelimiter(IFieldBuildingBlock buildingBlock)
        {
            AppendBuildingBlock(buildingBlock);
            AppendBuildingBlock(DelimiterFieldBuildingBlock.Instance);
        }

        private void AppendBuildingBlockWithQuotesAndDelimiter(IFieldBuildingBlock buildingBlock)
        {
            AppendBuildingBlock(QuoteFieldBuildingBlock.Instance);
            AppendBuildingBlock(buildingBlock);
            AppendBuildingBlock(QuoteFieldBuildingBlock.Instance);
            AppendBuildingBlock(DelimiterFieldBuildingBlock.Instance);
        }

        private readonly List<IFieldBuildingBlock> mBuildingBlocks;
        private readonly FieldStartFieldBuildingBlock mStart;
        private readonly FieldEndFieldBuildingBlock mEnd;
    }
}
