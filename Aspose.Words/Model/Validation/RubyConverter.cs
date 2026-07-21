// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/08/2015 by Alexey Morozov

using System.Collections.Generic;
using System.Text.RegularExpressions;
using Aspose.Common;
using Aspose.Words.Drawing;
using Aspose.Words.Fields;
using Aspose.Words.Math;

namespace Aspose.Words.Validation
{
    /// <summary>
    /// Implements conversions of ruby runs for save formats that do not support ruby tags.
    /// </summary>
    internal class RubyConverter
    {
        internal RubyConverter(DocumentValidator validator, SaveInfo saveInfo)
        {
            mSaveInfo = saveInfo;
            mValidator = validator;
        }

        /// <summary>
        /// Converts ruby run into form that is appropriate for current save format.
        /// </summary>
        internal void Convert(Run rubyRun)
        {
            switch (mSaveInfo.SaveFormat)
            {
                case SaveFormat.Doc:
                case SaveFormat.Dot:
                case SaveFormat.Rtf:
                    InsertTempNodes(ConvertToFieldNodes(rubyRun), rubyRun);
                    break;

                case SaveFormat.Docx:
                case SaveFormat.Dotx:
                case SaveFormat.Dotm:
                case SaveFormat.Docm:
                case SaveFormat.FlatOpc:
                case SaveFormat.FlatOpcMacroEnabled:
                case SaveFormat.FlatOpcTemplate:
                case SaveFormat.FlatOpcTemplateMacroEnabled:
                case SaveFormat.WordML:
                    // Do nothing.
                    break;

                case SaveFormat.Text:
                    // Do nothing.
                    break;

                case SaveFormat.Html:
                case SaveFormat.Mhtml:
                    // Do nothing.
                    break;
                default:
                    // WORDSNET-26731 Ruby run with EQ superscript should be treated as EQField.
                    Ruby ruby = (Ruby)rubyRun.RunPr[FontAttr.Ruby];
                    Node[] nodes = gSubscriptRegex.IsMatch(ruby.Base.Text)
                        ? ConvertToFieldNodes(rubyRun)
                        : new Node[] { ConvertToOfficeMath(rubyRun) };

                    InsertTempNodes(nodes, rubyRun);
                    break;
            }
        }

        internal void Revert()
        {
            foreach (Node node in mTempNodes)
                node.Remove();
        }

        /// <summary>
        /// Transforms ruby EQ field to Ruby object. Original field is removed.
        /// </summary>
        /// <dev>
        /// The details on switches and instructions:
        /// https://docs.microsoft.com/en-us/archive/blogs/murrays/word-eq-field-and-east-asian-formatting
        /// https://support.microsoft.com/en-us/office/field-codes-eq-equation-field-27300091-3780-4b88-836f-ae49ecde4692
        /// </dev>
        internal static void TransformEquation(FieldBundle fieldBundle)
        {
            Ruby ruby = ConvertToRuby(fieldBundle);
            if (ruby == null)
                return;

            // Make holder run for Ruby.
            DocumentBase doc = fieldBundle.Start.Document;
            Run rubyRun = new Run(doc, "", fieldBundle.Start.RunPr.Clone());
            rubyRun.RunPr.SetAttr(FontAttr.Ruby, ruby);

            fieldBundle.Start.InsertPrevious(rubyRun);

            NodeRemover.Remove(fieldBundle.Start, true, fieldBundle.End, true, NodeJoinMode.DontJoin);
        }

        private static Ruby ConvertToRuby(FieldBundle fieldBundle)
        {
            Field field = fieldBundle.GetField();
            return ConvertToRuby(field);
        }

        internal static Ruby ConvertToRuby(Field field)
        {
            const int jcGroup = 1;
            const int hpsGroup = 3;
            const int distGroup = 5;
            const int r1Group = 6;
            const int r2Group = 7;

            string fieldCode = field.GetFieldCode();

            MatchCollection matches = gEqFieldRegex.Matches(fieldCode);

            // We need exactly one match.
            if (matches.Count != 1)
                return null;

            GroupCollection groups = matches[0].Groups;

            // WORDSNET-25920 There should not be '\do' subscript switch in base of Ruby.
            if (gSubscriptRegex.IsMatch(groups[r2Group].Value))
                return null;

            Ruby ruby = new Ruby();

            // Fill general properties.
            if (groups[jcGroup].Success)
                ruby.Alignment = (RubyAlignment)FormatterPal.ParseInt(groups[jcGroup].Value);

            if (groups[hpsGroup].Success)
                ruby.TopSize = FormatterPal.ParseInt(groups[hpsGroup].Value);

            ruby.Distance = FormatterPal.ParseInt(groups[distGroup].Value) * 2;

            ruby.BaseSize = (int)InlineHelper.FetchAttr(field.Start, FontAttr.Size);
            ruby.Language = (Language)InlineHelper.FetchAttr(field.Start, FontAttr.LocaleIdFarEast);

            NodeRange fieldCodeRange = field.GetFieldCodeRange();

            // Make copy for top run in ruby.
            AddRubyChunks(groups[r1Group].Value, groups[r1Group].Index, fieldCodeRange, ruby.Top);

            // Make copy for base run in ruby.
            AddRubyChunks(groups[r2Group].Value, groups[r2Group].Index, fieldCodeRange, ruby.Base);

            return ruby;
        }

        /// <summary>
        /// Converts Ruby to EQ field nodes.
        /// </summary>
        internal static Node[] ConvertToFieldNodes(Run rubyRun)
        {
            Ruby ruby = (Ruby)rubyRun.RunPr[FontAttr.Ruby];
            DocumentBase doc = rubyRun.Document;

            // 1 - field start
            // 2 - field code start
            //     top runs
            // 3 - field code middle
            //     base runs
            // 4 - field code end
            // 5 - field separator
            // 6 - field end
            int nodeIdx = 0;
            Node[] fieldNodes = new Node[6 + ruby.Top.Count + ruby.Base.Count];

            RunPr baseRunPr = new RunPr();
            baseRunPr.SetAttr(FontAttr.Size, ruby.BaseSize);

            fieldNodes[nodeIdx++] = new FieldStart(doc, baseRunPr.Clone(), FieldType.FieldEquation);

            string fieldCodeStart = string.Format("EQ \\* jc{0} \\* \"Font:{1}\" \\* hps{2} \\o\\ad(\\s\\up {3}(",
                                                  (int)ruby.Alignment,
                                                  rubyRun.Font.NameAscii,
                                                  ruby.TopSize,
                                                  ruby.Distance / 2);
            fieldNodes[nodeIdx++] = new Run(doc, fieldCodeStart, baseRunPr.Clone());

            foreach (RubyChunk chunk in ruby.Top)
                fieldNodes[nodeIdx++] = new Run(doc, chunk.Text, chunk.RunPr.Clone());

            fieldNodes[nodeIdx++] = new Run(doc, "),", baseRunPr.Clone());

            foreach (RubyChunk chunk in ruby.Base)
                fieldNodes[nodeIdx++] = new Run(doc, chunk.Text, chunk.RunPr.Clone());

            fieldNodes[nodeIdx++] = new Run(doc, ")", baseRunPr.Clone());
            fieldNodes[nodeIdx++] = new FieldSeparator(doc, baseRunPr.Clone(), FieldType.FieldEquation);
            fieldNodes[nodeIdx] = new FieldEnd(doc, baseRunPr.Clone(), FieldType.FieldEquation, true);

            return fieldNodes;
        }

        private static void AddRubyChunks(string text, int index, NodeRange range, RubyChunkCollection chunks)
        {
            int pos = 0;
            int remains = text.Length;

            while (remains > 0)
            {
                Run run = GetRunAt(range, index + pos);

                // Run position.
                int runStart = GetRunStart(range, run);

                // Local position in run.
                int localPos = index + pos - runStart;

                RubyChunk chunk = new RubyChunk();

                int lenFromThisRun = System.Math.Min(run.Text.Length - localPos, remains);
                chunk.Text = run.Text.Substring(localPos, lenFromThisRun);
                chunk.RunPr =  run.RunPr.Clone();
                chunks.Add(chunk);

                pos += lenFromThisRun;
                remains -= lenFromThisRun;
            }
        }

        /// <summary>
        /// Converts Ruby to OfficeMath object.
        /// </summary>
        private static OfficeMath ConvertToOfficeMath(Run rubyRun)
        {
            Ruby ruby = (Ruby)rubyRun.RunPr[FontAttr.Ruby];
            DocumentBase doc = rubyRun.Document;

            OfficeMath mathPara = new OfficeMath(doc, new MathObjectOMathPara(), rubyRun.RunPr);
            OfficeMath math = new OfficeMath(doc, new MathObjectOMath(), rubyRun.RunPr);

            MathObjectMatrix objMatrix = new MathObjectMatrix();
            // WORDSNET-17442 The vertical justification of the matrix representing Ruby, should be "bottom".
            objMatrix.BaseJustification = MathBaseJustification.Bottom;
            objMatrix.ColumnSpacingRule = MathSpacingRule.Exactly;
            objMatrix.ColumnGap = 0;

            AddColumnPr(objMatrix, ruby.Alignment);

            OfficeMath officeMathMatrix = new OfficeMath(doc, objMatrix, rubyRun.RunPr);
            OfficeMath rowTop = new OfficeMath(doc, new MathObjectMatrixRow(), rubyRun.RunPr);
            OfficeMath rowBase = new OfficeMath(doc, new MathObjectMatrixRow(), rubyRun.RunPr);
            officeMathMatrix.AppendChild(rowTop);
            officeMathMatrix.AppendChild(rowBase);

            math.AppendChild(officeMathMatrix);
            mathPara.AppendChild(math);
            mathPara.IsConvertedFromRuby = true;

            // Add the top text.
            AddRowArgument(doc, ruby, rowTop, true);
            // Add the base text.
            AddRowArgument(doc, ruby, rowBase, false);

            return mathPara;
        }

        /// <summary>
        /// Adds a new columnPr in the matrix ColumnPrCollection
        /// </summary>
        /// <param name="objMatrix">The specified <see cref="MathObjectMatrix"/>></param>
        /// <param name="alignment">The specified <see cref="RubyAlignment"/>></param>
        private static void AddColumnPr(MathObjectMatrix objMatrix, RubyAlignment alignment)
        {
            MathMatrixColumnPr columnPr = new MathMatrixColumnPr();
            columnPr.HorizontalAlignment = GetHorizontalAlignment(alignment);
            objMatrix.ColumnPrCollection.Add(columnPr);
        }

        /// <summary>
        /// Returns a column alignment, using <see cref="RubyAlignment"/>
        /// </summary>
        /// <returns><see cref="HorizontalAlignment"/></returns>
        private static HorizontalAlignment GetHorizontalAlignment(RubyAlignment alignment)
        {
            switch (alignment)
            {
                case RubyAlignment.Left:
                     return HorizontalAlignment.Left;
                case RubyAlignment.Right:
                case RubyAlignment.RightVertical:
                    return HorizontalAlignment.Right;
                default:
                    return HorizontalAlignment.Center;
            }
        }

        /// <summary>
        /// Adds argument to the specified <see cref="OfficeMath"/>
        /// </summary>
        /// <param name="doc">The specified <see cref="DocumentBase"/></param>
        /// <param name="ruby">The specified <see cref="Ruby"/></param>
        /// <param name="row">The specified <see cref="OfficeMath"/></param>
        /// <param name="top">If "true" ruby.Top is used, otherwise ruby.Base is used</param>
        private static void AddRowArgument(DocumentBase doc, Ruby ruby, OfficeMath row, bool top)
        {
            RunPr runPr;
            string text;

            // WORDSNET-21509 ruby.Top ('w:rt' tag - Phonetic Guide Text) can be empty, just exit.
            // WORDSNET-25876 rubyBase can be empty as well
            if ((top && (ruby.Top.Count == 0)) || (!top && (ruby.Base.Count == 0)))
                return;

            if (top)
            {
                runPr = ruby.Top[0].RunPr.Clone();
                runPr.Size = ruby.TopSize;
                text = ruby.Top.Text;
            }
            else
            {
                runPr = ruby.Base[0].RunPr.Clone();
                runPr.Size = ruby.BaseSize;
                text = ruby.Base.Text;
            }

            if (ruby.Alignment == RubyAlignment.DistributeLetter || ruby.Alignment == RubyAlignment.DistributeSpace)
                AddTextToRowIfAlignmentIsDistribute(doc, ruby, row, top, text, runPr);
            else
                AddTextToRow(text, doc, runPr, row);
        }

        /// <summary>
        /// Add text if RubyAlignment is "DistributeLetter" or "DistributeSpace".
        /// </summary>
        private static void AddTextToRowIfAlignmentIsDistribute(DocumentBase doc, Ruby ruby, OfficeMath row, bool top,
            string text, RunPr runPr)
        {
            if (ruby.Top.Text.Length == ruby.Base.Text.Length)
            {
                // Add each letter in a separate column.
                for (int i = 0; i < text.Length; i++)
                {
                    // Add columnPr for the additional column.
                    if (i > 0 && top)
                    {
                        OfficeMath officeMatrix = (OfficeMath)row.ParentNode;
                        AddColumnPr(((MathObjectMatrix)officeMatrix.MathObject), ruby.Alignment);
                    }

                    AddTextToRow(text.Substring(i, 1), doc, runPr, row);
                }
            }
            else
            {
                runPr.Spacing = GetSpacing(ruby, top);
                AddTextToRow(text, doc, runPr, row);
            }
        }

        /// <summary>
        /// Calculates a spacing between letters.
        /// </summary>
        /// <remarks>
        /// This experimental method is trying to mimic the MS Office behavior.
        /// </remarks>
        /// <param name="ruby">The specified <see cref="Ruby"/></param>
        /// <param name="top">If "true" calculates spacing for the top, otherwise for the base</param>
        /// <returns>The spacing value</returns>
        private static int GetSpacing(Ruby ruby, bool top)
        {
            double widthBase = ruby.Base.Text.Length * ruby.BaseSize;
            double widthTop = ruby.Top.Text.Length * ruby.TopSize;
            double delta = widthBase - widthTop;
            int range = (ruby.Alignment == RubyAlignment.DistributeLetter) ? 5 : 4;
            int length = top ? ruby.Top.Text.Length : ruby.Base.Text.Length;
            int spacing = ((int)System.Math.Abs(delta) / length) * range;

            if (((delta > 0) && top) || ((delta < 0) && !top))
                return spacing;

            return 0;
        }

        /// <summary>
        /// Adds the specified text to the specified <see cref="OfficeMath"/>.
        /// </summary>
        /// <param name="text">The specified text</param>
        /// <param name="doc">The specified <see cref="DocumentBase"/></param>
        /// <param name="runPr">The specified <see cref="RunPr"/></param>
        /// <param name="row">The specified <see cref="OfficeMath"/></param>
        private static void AddTextToRow(string text, DocumentBase doc, RunPr runPr, OfficeMath row)
        {
            Run argRun = new Run(doc, text, runPr);
            OfficeMath arg = new OfficeMath(doc, new MathObjectArgumentBase(MathObjectType.Argument));
            row.AppendChild(arg);
            arg.AppendChild(argRun);
        }

        /// <summary>
        /// Get run from NodeRange at given text position.
        /// </summary>
        private static Run GetRunAt(NodeRange range, int index)
        {
            int pos = 0;
            foreach (Node node in range)
            {
                if ((pos + node.GetTextLength() > index) && node.NodeType == NodeType.Run)
                    return (Run)node;

                pos += node.GetTextLength();
            }

            return range.End.Node as Run;
        }

        private static int GetRunStart(NodeRange range, Run run)
        {
            int pos = 0;
            foreach (Node node in range)
            {
                if (node == run)
                    return pos;

                pos += node.GetTextLength();
            }

            return 0;
        }

        /// <summary>
        /// Inserts temporary nodes after given reference node.
        /// </summary>
        private void InsertTempNodes(Node[] nodes, Node refNode)
        {
            foreach (Node node in nodes)
            {
                InsertTempNode(node, refNode);
                refNode = node;
            }
        }

        /// <summary>
        /// Inserts temporary node after given reference node.
        /// </summary>
        private void InsertTempNode(Node node, Node refNode)
        {
            refNode.InsertNext(node);
            mTempNodes.Add(node);
            node.Accept(mValidator);
        }

        private readonly SaveInfo mSaveInfo;
        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly DocumentValidator mValidator;

        /// <summary>
        /// List of nodes which are inserted temporary and should be removed after document saving.
        /// </summary>
        private readonly List<Node> mTempNodes = new List<Node>();

        // WORDSNET-23839 The switches in EQ field ("jc", "Font" and "hps") are optional. The regex is adjusted accordingly.
        private static readonly Regex gEqFieldRegex = new Regex(
            @"EQ\s*" +
            @"(?:\\\*\s*jc([0-5]{1}))?\s*" + // Optional "\* jc" switch.
            @"(?:\\\*\s*\x22Font\:(.*)\x22)?\s*" + // Optional "\* Font" switch.
            @"(?:\\\*\s*hps([0-9]+))?\s*" + // Optional "\* hps" switch.
            @"\\o\s*(\\a[ldr]{1}\s*)?\(\\s\s*\\up\s*([0-9]+)\s*\((.*)\)\s*,\s*(.*)\)", // Instructions.
            RegexOptions.Compiled);

        /// <summary>
        /// Regex matches to subscript switch of EQ field (\s\do).
        /// See ISO-IEC-29500 p4, 14.9.4.6 EQ for details.
        /// </summary>
        private static readonly Regex gSubscriptRegex = new Regex(@"\\s\\do\s*(\d+)");
    }
}
