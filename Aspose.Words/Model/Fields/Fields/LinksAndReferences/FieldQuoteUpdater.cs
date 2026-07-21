// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/04/2017 by Edward Voronov

using System;
using System.Collections.Generic;
using Aspose.Common;
using Aspose.Words.Math;
using Aspose.Words.Tables;

namespace Aspose.Words.Fields
{
    internal class FieldQuoteUpdater
    {
        internal FieldQuoteUpdater(Field field)
        {
            Debug.Assert(field.Type == FieldType.FieldQuote || field.Type == FieldType.FieldShape);
            mField = field;
        }

        internal FieldUpdateAction Update()
        {
            CollectResult();

            if (mErrorText != null)
                return new FieldUpdateActionInsertErrorMessage(mField, mErrorText);

            if (mResult != null)
                return new FieldUpdateActionApplyResult(mField, new NodeRangeFieldResult(mResult));

            return new FieldUpdateActionApplyResult(mField, string.Empty);
        }

        /// <summary>
        /// Concatenates parts provided by the field arguments.
        /// </summary>
        /// <remarks>
        /// There is an undocumented feature for the QUOTE field. Its field code can include a variable count of arguments.
        /// A result is then achieved through the concatenation of argument without any delimiter.
        /// </remarks>
        private void CollectResult()
        {
            mErrorText = null;
            mResult = null;

            mField.FieldCodeCache.IsolateElements();

            // Use global variable for C++ porting. If use local variable parent node of result is garbaged and NullReferenceException is thrown in C++.
            mBuilder = new ResultBuilder(mField.Document, mField.Start.ParentParagraph);

            // WORDSNET-19717 It seems like the QUOTE field still applies CHARFORMAT to encoded chars, but not to text parts.
            CharFormatProvider formatProvider = new CharFormatProvider(mField);
            Inline formatSource = formatProvider.GetSourceNode();

            foreach (FieldArgument argument in mField.FieldCodeCache.Arguments)
            {
                // If the argument text is enclosed in double quotes or the argument represents a single field's result range
                // pass the text part without any changes.
                if (argument.IsInDoubleQuotes || argument.IsSingleFieldResult)
                {
                    AppendArgumentRange(mBuilder, argument);
                    continue;
                }

                CharHolder ch = new CharHolder();
                ExtractResult result = TryExtractChar(argument, ch);
                switch (result)
                {
                    case ExtractResult.Extracted:
                    {
                        // Append extracted char.
                        Run run = new Run(mField.Document, ch.Value.ToString(), formatSource.RunPr);
                        mBuilder.Append(run);
                        break;
                    }
                    case ExtractResult.Skipped:
                    {
                        // The text part does not represent a char code.
                        AppendArgument(mBuilder, argument);
                        break;
                    }
                    default:
                    {
                        mErrorText = GetErrorText(result);
                        return;
                    }
                }
            }

            if (mBuilder.IsEmpty)
                return;

            mResult = mBuilder.GetResultRange();
        }

        private void AppendArgument(ResultBuilder builder, FieldArgument argument)
        {
            Table table = FindTable(argument.Range);
            if (table != null)
            {
                // Skip leading unquoted tables.
                if (builder.IsEmpty)
                    return;

                AppendTableContent(builder, table);
                return;
            }

            AppendArgumentRange(builder, argument);
        }

        private static Table FindTable(NodeRange range)
        {
            foreach (Node node in range)
            {
                if (node.NodeType == NodeType.Table)
                    return (Table)node;
            }

            return null;
        }

        private void AppendTableContent(ResultBuilder builder, Table table)
        {
            Paragraph paragraph = new Paragraph(mField.Document);
            foreach (Row row in table.Rows)
            {
                foreach (Cell cell in row.Cells)
                {
                    foreach (Node node in new NodeRange(cell, cell))
                    {
                        if (node is IInline)
                            paragraph.AppendChild(node.Clone(false));
                    }
                }
            }

            if (!paragraph.HasChildNodes)
                return;

            builder.Append(new NodeRange(paragraph.FirstChild, paragraph.LastChild));
        }

        private static void AppendArgumentRange(ResultBuilder builder, FieldArgument argument)
        {
            builder.Append(argument.Range);

            if (!argument.IsInDoubleQuotes)
                return;

            Run first = builder.GetResultBySource(argument.Range.Start.Node) as Run;
            Run last = builder.GetResultBySource(argument.Range.End.Node) as Run;

            TrimDoubleQuote(first, true);
            TrimDoubleQuote(last, false);
        }

        private static void TrimDoubleQuote(Run run, bool start)
        {
            if (run == null || string.IsNullOrEmpty(run.Text))
                return;

            int index = start ? 0 : run.Text.Length - 1;
            if (!FieldCodeParser.IsDoubleQuote(run.Text[index]))
                return;

            run.Text = run.Text.Remove(index, 1);
        }

        private static ExtractResult TryExtractChar(FieldArgument argument, CharHolder ch)
        {
            string argumentText = argument.GetNormalizedText();

            // Try to parse a hexadecimal char code.
            ExtractResult result = CollectCharByHexadecimalCode(argumentText, ch);
            if (result != ExtractResult.Skipped)
                return result;

            // Try to parse a decimal char code.
            return CollectCharByDecimalCode(argumentText, ch);
        }

        /// <summary>
        /// Checks whether the specified text represents a valid hexadecimal char code and appends
        /// the corresponding character to the specified string builder if successful.
        /// </summary>
        /// <remarks>
        /// See http://msdn.microsoft.com/en-us/library/ff534940(v=office.12).aspx for any details.
        /// </remarks>
        private static ExtractResult CollectCharByHexadecimalCode(string text, CharHolder ch)
        {
            if (!StringUtil.StartsWithOrdinalIgnoreCase(text, HexPrefix))
                return ExtractResult.Skipped;

            int hexPrefixLength = HexPrefix.Length;
            if (text.Length == hexPrefixLength)
                return ExtractResult.ErrorDigitExpected;

            const int maxCharCodeLength = 4;
            int charCodeLength = System.Math.Min(text.Length - hexPrefixLength, maxCharCodeLength);
            int charCode = FormatterPal.TryParseHex(text.Substring(hexPrefixLength, charCodeLength));
            return CollectCharByCode(charCode, ExtractResult.ErrorInvalidHexDigit, ch);
        }

        /// <summary>
        /// Checks whether the specified text represents a valid decimal char code and appends
        /// the corresponding character to the specified string builder if successful.
        /// </summary>
        /// <remarks>
        /// See http://msdn.microsoft.com/en-us/library/ff534940(v=office.12).aspx for any details.
        /// </remarks>
        private static ExtractResult CollectCharByDecimalCode(string text, CharHolder ch)
        {
            if (!StringUtil.HasChars(text) || !StringUtil.IsDigit(text[0]))
                return ExtractResult.Skipped;

            const int maxCharCodeLength = 3;
            int charCodeLength = System.Math.Min(text.Length, maxCharCodeLength);
            string charCodeText = (text.Length > charCodeLength) ? text.Substring(0, charCodeLength) : text;
            int charCode = FormatterPal.TryParseInt(charCodeText);
            return CollectCharByCode(charCode, ExtractResult.ErrorDigitExpected, ch);
        }

        /// <summary>
        /// Checks whether the specified char code is valid and appends
        /// the corresponding character to the specified string builder if successful.
        /// </summary>
        /// <remarks>
        /// See http://msdn.microsoft.com/en-us/library/ff534940(v=office.12).aspx for any details.
        /// </remarks>
        private static ExtractResult CollectCharByCode(int charCode, ExtractResult parseErrorResult, CharHolder ch)
        {
            if (charCode == int.MinValue)
                return parseErrorResult;

            ch.Value = (char)charCode;
            if (ch.Value == ControlChar.ParagraphBreakChar)
                return ExtractResult.ErrorParagraphBreakChar;

            return ExtractResult.Extracted;
        }

        /// <summary>
        /// Returns an error message for an erroneous <see cref="ExtractResult"/> value.
        /// </summary>
        /// <param name="errorResult"></param>
        /// <returns></returns>
        private static string GetErrorText(ExtractResult errorResult)
        {
            switch (errorResult)
            {
                case ExtractResult.ErrorDigitExpected:
                    return "Error! Digit expected.";
                case ExtractResult.ErrorInvalidHexDigit:
                    return "Error! Not a valid Hex digit.";
                case ExtractResult.ErrorParagraphBreakChar:
                    return "Error! Cannot insert return character.";
                default:
                    throw new ArgumentOutOfRangeException("errorResult");
            }
        }

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly Field mField;

        private NodeRange mResult;
        private ResultBuilder mBuilder;
        private string mErrorText;

        /// <summary>
        /// Gets the common hexadecimal string prefix: "0x".
        /// </summary>
        private const string HexPrefix = "0x";

        private enum ExtractResult
        {
            Extracted,
            Skipped,
            ErrorDigitExpected,
            ErrorInvalidHexDigit,
            ErrorParagraphBreakChar
        }

        private class CharHolder
        {
            internal CharHolder()
            {
                Value = '\0';
            }

            internal char Value
            {
                [CodePorting.Translator.Cs2Cpp.CppConstMethod]
                get;
                set;
            }
        }

        private class ResultBuilder : INodeCloningListener
        {
            void INodeCloningListener.NotifyNodeCloned(Node source, Node clone)
            {
                mSourceToResultDictionary[source] = clone;
                mResultToSourceDictionary[clone] = source;
            }

            internal Node GetResultBySource(Node source)
            {
                return mSourceToResultDictionary.GetValueOrNull(source);
            }

            internal bool IsEmpty { get; private set; }

            internal ResultBuilder(DocumentBase document, Paragraph startParentParagraph)
            {
                IsEmpty = true;
                Run run = new Run(document);
                Paragraph paragraph = (Paragraph)startParentParagraph.Clone(false);
                Body body = new Body(document);
                Section section = new Section(document);

                mDocument = document;
                mRoot = new ShadowDocument(document);
                mRoot.AppendChild(section);
                section.AppendChild(body);
                body.AppendChild(paragraph);
                paragraph.AppendChild(run);
                mRefNode = run;
            }

            internal void Append(Node run)
            {
                Paragraph paragraph = new Paragraph(mDocument);
                paragraph.AppendChild(run);
                Append(new NodeRange(run, run));
            }

            internal void Append(NodeRange range)
            {
                IsEmpty = false;

                if (CopyRangeWithinOfficeMath(range))
                    return;

                CopyRange(range, mRefNode);
            }

            private bool CopyRangeWithinOfficeMath(NodeRange range)
            {
                OfficeMath officeMath = GetTopLevelOfficeMathAncestor(range);
                if (officeMath == null)
                    return false;

                CompositeNode startParent = range.Start.Node.ParentNode;
                CompositeNode endParent = range.End.Node.ParentNode;
                if (!NodeUtil.IsAncestorOrSelf(endParent, startParent))
                    return false;

                Node dummyRefNode = CloneAncestorOfficeMath(range.Start.Node, officeMath);
                CopyRange(range, dummyRefNode);
                dummyRefNode.Remove();

                return true;
            }

            private static OfficeMath GetTopLevelOfficeMathAncestor(NodeRange range)
            {
                OfficeMath startAncestor = GetTopLevelOfficeMathAncestor(range.Start.Node);
                OfficeMath endAncestor = GetTopLevelOfficeMathAncestor(range.End.Node);
                return startAncestor == endAncestor
                    ? startAncestor
                    : null;
            }

            private static OfficeMath GetTopLevelOfficeMathAncestor(Node node)
            {
                CompositeNode officeMath = node.GetAncestor(NodeType.OfficeMath);
                if (officeMath == null)
                    return null;

                return ((OfficeMath)officeMath).GetTopLevelOfficeMath();
            }

            private Node CloneAncestorOfficeMath(Node node, OfficeMath officeMath)
            {
                Node dummyRefNode = null;

                CompositeNode top = null;
                CompositeNode parent = node.ParentNode;

                while (true)
                {
                    CompositeNode clone = (CompositeNode)parent.Clone(false, this);

                    if (dummyRefNode == null)
                        dummyRefNode = clone.AppendChild(node.Clone(false));

                    if (top != null)
                        clone.AppendChild(top);

                    top = clone;

                    if (parent == officeMath)
                        break;

                    parent = parent.ParentNode;
                }

                mRefNode.InsertPrevious(top);

                return dummyRefNode;
            }

            private void CopyRange(NodeRange range, Node refNode)
            {
                const NodeCopierOptions options = NodeCopierOptions.SkipCrossStructureAnnotations | NodeCopierOptions.CloneNode;
                NodeCopier.Copy(range, refNode, null, this, options);
            }

            internal NodeRange GetResultRange()
            {
                FinalizeBuilding();

                return FieldUtil.BuildFieldResultNodeRange(mRoot, mRoot);
            }

            private void FinalizeBuilding()
            {
                foreach (Node node in NodeFinder.FindNodes(new NodeRange(mRoot, mRoot), NodeType.OfficeMath))
                {
                    OfficeMath current = (OfficeMath)node;

                    OfficeMath previous = current.PreviousSibling as OfficeMath;
                    if (previous == null)
                        continue;

                    Node currentSource = mResultToSourceDictionary[current];
                    Node previousSource = mResultToSourceDictionary[previous];
                    if (currentSource != previousSource && !current.IsTopLevel)
                        continue;

                    if (MoveChildNodes(current, previous))
                        current.Remove();
                }
            }

            private static bool MoveChildNodes(CompositeNode source, CompositeNode target)
            {
                Node[] children = source.GetChildNodes(NodeType.Any, false).ToArray();
                foreach (Node child in children)
                {
                    if (!target.CanInsert(child))
                        return false;

                    target.AppendChild(child);
                }

                return true;
            }

            private readonly Dictionary<Node, Node> mSourceToResultDictionary = new Dictionary<Node, Node>();
            private readonly Dictionary<Node, Node> mResultToSourceDictionary = new Dictionary<Node, Node>();

            private readonly DocumentBase mDocument;
            private readonly ShadowDocument mRoot;
            private readonly Run mRefNode;
        }

        /// <summary>
        /// Shadow document node that holds multi-section QUOTE field result.
        /// </summary>
        private class ShadowDocument : CompositeNode
        {
            internal ShadowDocument(DocumentBase document)
            {
                SetDocument(document);
            }

            public override NodeType NodeType
            {
                get { return NodeType.Document; }
            }

            public override bool Accept(DocumentVisitor visitor)
            {
                return true;
            }

            public override VisitorAction AcceptStart(DocumentVisitor visitor)
            {
                return VisitorAction.Continue;
            }

            public override VisitorAction AcceptEnd(DocumentVisitor visitor)
            {
                return VisitorAction.Continue;
            }
        }
    }
}
