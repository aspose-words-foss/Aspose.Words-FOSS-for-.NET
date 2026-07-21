// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/01/2016 by Edward Voronov

using System.Collections.Generic;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Builds a complex field argument consisting of fields, nodes, and plain text.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    public class FieldArgumentBuilder : IFieldBuildingBlock
    {
        /// <summary>
        /// Initializes an instance of the <see cref="FieldArgumentBuilder"/> class.
        /// </summary>
        public FieldArgumentBuilder()
        {
            mBuildingBlocks = new List<IFieldBuildingBlock>();
        }

        /// <summary>
        /// Adds a plain text to the argument.
        /// </summary>
        public FieldArgumentBuilder AddText(string text)
        {
            mBuildingBlocks.Add(new TextFieldBuildingBlock(text));

            return this;
        }

        /// <summary>
        /// Adds a node to the argument.
        /// </summary>
        /// <remarks>
        /// Only text level nodes are supported at the moment.
        /// </remarks>
        public FieldArgumentBuilder AddNode(Inline node)
        {
            mBuildingBlocks.Add(new NodeFieldBuildingBlock(node));

            return this;
        }

        /// <summary>
        /// Adds a field represented by a <see cref="FieldBuilder"/> to the argument.
        /// </summary>
        public FieldArgumentBuilder AddField(FieldBuilder fieldBuilder)
        {
            mBuildingBlocks.Add(fieldBuilder);

            return this;
        }

        void IFieldBuildingBlock.BuildBlock(DocumentBuilder documentBuilder)
        {
            foreach (IFieldBuildingBlock buildingBlock in mBuildingBlocks)
                buildingBlock.BuildBlock(documentBuilder);
        }

        private readonly List<IFieldBuildingBlock> mBuildingBlocks;
    }
}
