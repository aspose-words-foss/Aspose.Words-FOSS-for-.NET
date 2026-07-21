// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/04/2021 by Alexey Noskov

using System.Collections.Generic;
using Aspose.Words.Fields;

namespace Aspose.Words.Validation
{
    /// <summary>
    /// Implements conversions of EQ fields to OfficeMath nodes for export to flow-format HTML.
    /// </summary>
    internal class EQFieldConverter
    {
        internal EQFieldConverter(DocumentValidator validator, SaveInfo saveInfo)
        {
            mSaveInfo = saveInfo;
            mValidator = validator;
        }

        /// <summary>
        /// Converts EQ fields into form that is appropriate for current save format.
        /// </summary>
        internal void Convert(Field field)
        {
            if (field.Start.FieldType != FieldType.FieldEquation)
                return;

            switch (mSaveInfo.SaveFormat)
            {
                case SaveFormat.Html:
                case SaveFormat.Mhtml:
                case SaveFormat.Epub:
                case SaveFormat.Azw3:
                case SaveFormat.Mobi:
                    InsertTempNode(GetFakeResultNode(field), field.End);
                    break;
                default:
                    // Do nothing.
                    break;
            }
        }

        /// <summary>
        /// Reverts changes performed by this converter.
        /// </summary>
        internal void Revert()
        {
            foreach (Node node in mTempNodes)
                node.Remove();
        }

        private static Node GetFakeResultNode(Field field)
        {
            // Fake result of an EQ field is NodeRange of a single node (normally, OfficeMath).
            NodeRange fakeResult = field.GetFakeResult();
            return fakeResult.Start.Node;
        }

        /// <summary>
        /// Inserts the specified node after the reference node. Remembers the inserted node for futher reverting.
        /// </summary>
        private void InsertTempNode(Node node, Node refNode)
        {
            // WORDSNET-22406 In case an EQ field has no content we just skip it. We don't insert fake empty run, because
            // it will be removed from the model by the document validator and this will lead to an exception later when we'll
            // try to revert changes and remove the fake run.
            Run nodeAsRun = node as Run;
            if ((nodeAsRun != null) && (nodeAsRun.Text == ""))
            {
                return;
            }

            refNode.InsertNext(node);
            mTempNodes.Add(node);
            node.Accept(mValidator);
        }

        private readonly SaveInfo mSaveInfo;
        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly DocumentValidator mValidator;

        /// <summary>
        /// List of fake result nodes which are inserted temporarily and should be removed after the document is saved.
        /// </summary>
        private readonly List<Node> mTempNodes = new List<Node>();
    }
}
