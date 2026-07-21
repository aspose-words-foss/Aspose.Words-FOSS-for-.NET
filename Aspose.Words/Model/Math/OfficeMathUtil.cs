// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/02/2017 by Andrey Noskov

using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.RW.Factories;
using Aspose.Words.Validation;

namespace Aspose.Words.Math
{
    internal class OfficeMathUtil
    {
        /// <summary>
        /// ctor.
        /// </summary>
        private OfficeMathUtil(OfficeMath srcOMath)
        {
            mSrcOMath = srcOMath;
        }

        /// <summary>
        ///  Changes display format type to <see cref="OfficeMathDisplayType.Inline"/>.
        /// </summary>
        internal static void ChangeToInline(OfficeMath srcOMath)
        {
            OfficeMathUtil officeMathUtil = new OfficeMathUtil(srcOMath);
            officeMathUtil.ChangeToInlineCore();
        }

        /// <summary>
        /// Changes display format type to <see cref="OfficeMathDisplayType.Display"/>.
        /// </summary>
        internal static void ChangeToDisplay(OfficeMath srcOMath)
        {
            OfficeMathUtil officeMathUtil = new OfficeMathUtil(srcOMath);
            officeMathUtil.ChangeToDisplayCore();
        }

        /// <summary>
        /// Converts specified shape to office math object if needed.
        /// </summary>
        /// <remarks>
        /// The method can to be called from the document validator or postloader.
        /// </remarks>
        internal static void ConvertShapeToOfficeMath(Shape shape, IWarningCallback warningCallback)
        {
            byte[] equationXMLVal = RetrieveEquationXML(shape);
            if (!shape.IsTopLevel || (equationXMLVal == null))
                return;

            // Inform customer that shape was not converted to OfficeMath object.
            WarningUtil.Warn(warningCallback, WarningType.DataLoss,
                WarningSource.Validator, WarningStrings.InvalidOfficeMathXml);
        }

        /// <summary>
        /// Retrieves "EquationXML" attribute and tries to cast value to "byte[]" type;
        /// </summary>
        internal static byte[] RetrieveEquationXML(Shape shape)
        {
            Debug.Assert(shape != null);

            // Note: for RTF format "EquationXML" attribute uses as flag (value type is string), not as XML container.
            // Additionally check "TestSaveWarningsForShapePr".
            return shape.ShapePr[ShapeAttr.EquationXML] as byte[];
        }

        /// <summary>
        /// Check if this object must be rendered in the inline mode.
        /// </summary>
        internal static bool IsInline(OfficeMath officeMath)
        {
            OfficeMath topLevelMath = officeMath.GetTopLevelOfficeMath();
            MathObjectType type = topLevelMath.MathObjectType;
            switch (type)
            {
                // WORDSNET-16567 If a parent paragraph has a run with text inside,
                // then MathPara objects must be displayed in inline mode.
                case MathObjectType.OMathPara:
                // WORDSNET-19216 OMath object may be "not inline".
                case MathObjectType.OMath:
                    return IsOfficeMathInLine(topLevelMath);
                default:
                    return false;
            }
        }

        private void ChangeToInlineCore()
        {
            // Display format type has effect for top level Office Math only.
            if (mSrcOMath.IsTopLevel)
            {
                // Parent MathObjectOMathPara have to be removed and all child nodes preserved.
                if (mSrcOMath.MathObject.MathObjectType == MathObjectType.OMathPara)
                {
                    OfficeMath firstChild = (OfficeMath)mSrcOMath.FirstChild;
                    mSrcOMath.MathObject = firstChild.MathObject;

                    // Copy child nodes to current Office Math.
                    while (firstChild.HasChildNodes)
                        mSrcOMath.AppendChild(firstChild.FirstChild);

                    firstChild.Remove();

                    ProcessInlineOMathSiblings();
                }
            }
        }

        private void ChangeToDisplayCore()
        {
            // Display format type has effect for top level Office Math only.
            if (mSrcOMath.IsTopLevel)
            {
                MathObject mathObject = mSrcOMath.MathObject;

                // Current OMath have to be wrapped into MathObjectOMathPara.
                if (mathObject.MathObjectType != MathObjectType.OMathPara)
                {
                    // Create new office math paragraph.
                    MathObjectOMathPara oMathPara = new MathObjectOMathPara();

                    // Create OfficeMath using current MathObject.
                    OfficeMath dstOMath = new OfficeMath(mSrcOMath.Document, mathObject);

                    // Set OfficeMath paragraph to current MathObject.
                    mSrcOMath.MathObject = oMathPara;

                    // Copy child nodes.
                    while (mSrcOMath.HasChildNodes)
                        dstOMath.AppendChild(mSrcOMath.FirstChild);

                    // Append office math to the office math paragraph.
                    mSrcOMath.AppendChild(dstOMath);

                    ProcessDisplayOMathSiblings(dstOMath, true);
                    ProcessDisplayOMathSiblings(dstOMath, false);
                }
            }
        }

        /// <summary>
        /// Process nodes before and after office math node with <see cref="OfficeMathDisplayType.Inline"/>.
        /// </summary>
        private void ProcessInlineOMathSiblings()
        {
            Run nextRun = GetNextRun();
            Run previousRun = GetPreviousRun();

            // If last child of the office math is run which ends with line break,
            // its line break has to be removed.
            if (mSrcOMath.LastChild.NodeType == NodeType.Run)
            {
                Run lastRun = (Run)mSrcOMath.LastChild;

                if (EndsWithLineBreak(lastRun))
                    TrimLastChar(lastRun); // Remove line break.

                // MSW considers Office Math as inline when parent paragraph contains at least one run.
                // Create and add new empty run if required.
                Run newRun = new Run(mSrcOMath.Document, ControlChar.Space);
                newRun.RunPr = lastRun.RunPr.Clone();

                if ((nextRun == null) || !StartsWithSpace(nextRun))
                    mSrcOMath.InsertNext(newRun);
            }

            if ((previousRun != null) && EndsWithLineBreak(previousRun))
            {
                TrimLastChar(previousRun); // Remove line break.

                if ((previousRun.Text.Length == 0) && (previousRun.PreviousSibling.NodeType == NodeType.Run))
                {
                    Run prePreviousRun = (Run)previousRun.PreviousSibling;

                    // White space has to be added to the end of the last run
                    // if previous run hasn't a white space at the end.
                    if (!EndsWithSpace(prePreviousRun))
                        previousRun.Text = previousRun.Text + ControlChar.Space; // Add white space.
                }
                else
                {
                    previousRun.Text = previousRun.Text + ControlChar.Space; // Add white space.
                }
            }
        }

        /// <summary>
        /// Process nodes before and after office math node with <see cref="OfficeMathDisplayType.Display"/>.
        /// </summary>
        /// <param name="dstOMath">Destination display Office math node.</param>
        /// <param name="isNextSibling">True - when it is next sibling node.</param>
        private void ProcessDisplayOMathSiblings(OfficeMath dstOMath, bool isNextSibling)
        {
            Node siblingNode = isNextSibling ? mSrcOMath.NextSibling : mSrcOMath.PreviousSibling;

            if (siblingNode != null)
            {
                NodeType siblingNodeType = siblingNode.NodeType;

                // MSW inserts line break only before nodes of these types.
                if ((siblingNodeType == NodeType.Run) ||
                    (siblingNodeType == NodeType.Shape) ||
                    (siblingNodeType == NodeType.GroupShape))
                {
                    Run lineBreakRun = new Run(mSrcOMath.Document, ControlChar.LineBreak);

                    if (siblingNodeType == NodeType.Run)
                    {
                        Run run = (Run)siblingNode;

                        lineBreakRun = (Run)run.Clone(false);
                        lineBreakRun.Text = ControlChar.LineBreak;

                        // Cut the first/last char (SpaceChar).
                        if ((StartsWithSpace(run) && isNextSibling))
                            TrimFirstChar(run);

                        if (EndsWithSpace(run) && !isNextSibling)
                            TrimLastChar(run);
                    }

                    InsertLineBreakRun(dstOMath, lineBreakRun, isNextSibling);
                }
            }
        }

        /// <summary>
        /// Inserts lineBreak run to the and of the office math or before office math node.
        /// </summary>
        private void InsertLineBreakRun(OfficeMath dstOMath, Run lineBreakRun, bool isNextSibling)
        {
            if (isNextSibling)
                dstOMath.AppendChild(lineBreakRun);
            else
                mSrcOMath.InsertPrevious(lineBreakRun);
        }

        private Run GetNextRun()
        {
            return mSrcOMath.NextSibling as Run;
        }

        private Run GetPreviousRun()
        {
            return mSrcOMath.PreviousSibling as Run;
        }

        private static bool EndsWithSpace(Run run)
        {
            return (run != null) && (run.Text.Length > 0) && (run.Text[run.Text.Length - 1] == ControlChar.SpaceChar);
        }

        private static bool StartsWithSpace(Run run)
        {
            return (run != null) && (run.Text.Length > 0) && (run.Text[0] == ControlChar.SpaceChar);
        }

        private static bool EndsWithLineBreak(Run run)
        {
            return (run != null) && (run.Text.Length > 0) && (run.Text[run.Text.Length - 1] == ControlChar.LineBreakChar);
        }

        private static bool StartsWithLineBreak(Run run)
        {
            return (run != null) && (run.Text.Length > 0) && (run.Text[0] == ControlChar.LineBreakChar);
        }

        private static void TrimLastChar(Run run)
        {
            if (run != null && run.Text.Length > 0)
                run.Text = run.Text.Substring(0, run.Text.Length - 1);
        }

        private static void TrimFirstChar(Run run)
        {
            if (run != null && run.Text.Length > 0)
                run.Text = run.Text.Substring(1, run.Text.Length - 1);
        }

        /// <summary>
        /// Checks if <see cref="OfficeMath"/> must be rendered in the inline mode.
        /// </summary>
        /// <param name="topLevelMath">specified <see cref="OfficeMath"/></param>
        /// <returns>"True" if <see cref="OfficeMath"/> must be rendered in the inline mode, otherwise - "false"</returns>
        private static bool IsOfficeMathInLine(OfficeMath topLevelMath)
        {
            if (topLevelMath.IsConvertedFromRuby || (topLevelMath.ParentParagraph == null))
                return false;

            // WORDSNET-26665 Formula in structured document tag should be rendered as "Inline".
            Paragraph para = (Paragraph)topLevelMath.GetAncestor(NodeType.Paragraph);

            if ((para != null) && (para.ParentNode != null) && (para.ParentNode.NodeType == NodeType.StructuredDocumentTag))
                return true;

            // All elements before of after the line break char should be ignored.
            int startCheck = GetIndexForOfficeMathCheck(topLevelMath, true);
            int endCheck = GetIndexForOfficeMathCheck(topLevelMath, false);

            for (int i = startCheck; i < endCheck; i++)
            {
                Node currentNode = topLevelMath.ParentParagraph.GetChildNodes(NodeType.Any, false)[i];

                // If paragraph contains Shape, OfficeMath object is "inline".
                if (NodeUtil.IsDrawingObject(currentNode))
                    return true;

                Run run = currentNode as Run;

                // If paragraph contains Run, OfficeMath object is "inline".
                if ((run != null) && StringUtil.HasChars(run.Text))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns the start or end index for iterating over the paragraph children,
        /// taking into account the line break char.
        /// </summary>
        /// <remarks>
        /// The 4-steps check:
        /// 1. Checks, if the first run of the see cref="OfficeMath" starts with the line break char.
        /// 2. Checks, if the last run of the see cref="OfficeMath" ends with the line break char.
        /// 3. Checks, if the previous run ends with the line break char.
        /// 4. Checks, if the next run starts with the line break char.
        /// </remarks>
        /// <param name="officeMath">Specified <see cref="OfficeMath"/></param>
        /// <param name="isStartIndex">Determines which index (start or end) should be returned</param>
        /// <returns>Start or end index of the paragraph child</returns>
        private static int GetIndexForOfficeMathCheck(OfficeMath officeMath, bool isStartIndex)
        {
            int indexMath = officeMath.ParentParagraph.GetChildNodes(NodeType.Any, false).IndexOf(officeMath);
            int index = isStartIndex ? 0 : officeMath.ParentParagraph.GetChildNodes(NodeType.Any, false).Count;
            int checkedIndex = isStartIndex ? indexMath + 1 : indexMath;

            // Check, if the first (or last) child of the office math starts (or ends) with the line break char.
            if (DoesNodeStartOrEndWithLineBreak(officeMath, isStartIndex))
                return checkedIndex;

            Node neighborNode = isStartIndex ? officeMath.PreviousSibling : officeMath.NextSibling;

            // Check, if the neighboring children of the paragraph contains the line break char.
            if (DoesNodeStartOrEndWithLineBreak(neighborNode, !isStartIndex))
                return checkedIndex;

            return index;
        }

        /// <summary>
        /// Checks if the specified <see cref="Node"/> or the first (or last) child starts (or ends) with the line break char.
        /// </summary>
        /// <param name="node">The specified <see cref="Node"/></param>
        /// <param name="fromBeginning">Determines whether to check the beginning or the end</param>
        /// <returns>"True" if <see cref="Node"/> starts (ends) with the line break char, otherwise - "false"</returns>
        internal static bool DoesNodeStartOrEndWithLineBreak(Node node, bool fromBeginning)
        {
            if (node == null)
                return false;

            bool isLineBreakFound = false;

            switch (node.NodeType)
            {
                case NodeType.OfficeMath:
                    OfficeMath officeMath = (OfficeMath)node;
                    if (officeMath.HasChildNodes)
                    {
                        Node officeNode = fromBeginning ? officeMath.FirstChild : officeMath.LastChild;
                        isLineBreakFound = DoesNodeStartOrEndWithLineBreak(officeNode, fromBeginning);
                    }
                    break;
                // WORDSNET-16567 If a parent paragraph has a run with text inside,
                // then OfficeMath objects must be displayed in inline mode.
                case NodeType.Run:
                    isLineBreakFound = DoesRunStartOrEndWithLineBreak(node, fromBeginning);
                    break;
                default:
                    break;
            }

            return isLineBreakFound;
        }

        /// <summary>
        /// Checks if the specified node type is a run and the run starts (or ends) with the line break char.
        /// </summary>
        /// <param name="node">Specified <see cref="Node"/></param>
        /// <param name="startsWith">Determines whether to check the beginning or end of the node text</param>
        /// <returns>"True" if node is a run and the run text starts (or ends) with line break, otherwise - "false"</returns>
        private static bool DoesRunStartOrEndWithLineBreak(Node node, bool startsWith)
        {
            Run run = node as Run;
            return startsWith ? StartsWithLineBreak(run) : EndsWithLineBreak(run);
        }

        private readonly OfficeMath mSrcOMath;
    }
}
