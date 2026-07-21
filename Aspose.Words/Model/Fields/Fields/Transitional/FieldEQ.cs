// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/10/2011 by Dmitry Vorobyev

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Aspose.Common;
using Aspose.Words.Drawing;
using Aspose.Words.Math;
using Aspose.Words.RW.Html.Parser.IEConditionalExpressions;
using Aspose.Words.Tables;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements the EQ field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    public class FieldEQ : Field
    {
        /// <summary>
        /// Returns Office Math object corresponded to the EQ field.
        /// </summary>
        /// <returns>
        /// Returns <c>null</c> if field code is empty or invalid, otherwise an <see cref="OfficeMath"/> instance.
        /// </returns>
        public OfficeMath AsOfficeMath()
        {
            return EqFieldCodeToOfficeMath() as OfficeMath;
        }

        internal override NodeRange GetFakeResult()
        {
            Node eq = EqFieldCodeToOfficeMath();
            Paragraph paragraph = GetFakePara();
            paragraph.AppendChild(eq);

            return new NodeRange(eq, eq);
        }

        /// <summary>
        /// Creates <see cref="Paragraph"/> for Office Math object.
        /// </summary>
        private Paragraph GetFakePara()
        {
            Paragraph paragraph = new Paragraph(Document, Start.ParentParagraph.ParaPr.Clone(), new RunPr());

            Cell parentCell = Start.ParentParagraph.ParentCell;
            // WORDSNET-28592 Save cell settings for created Office Math object.
            if (parentCell != null)
            {
                Table table = new Table(Document);
                Row row = new Row(Document);
                table.AppendChild(row);
                Cell cell = new Cell(Document, parentCell.CellPr);
                cell.AppendChild(paragraph);
                row.AppendChild(cell);
            }

            return paragraph;
        }

        /// <summary>
        /// Parses EQ field code and converts it to <see cref="OfficeMath"/> node,
        /// that can be rendered using the corresponding ToAps converter.
        /// </summary>
        private Node EqFieldCodeToOfficeMath()
        {
            // EQ field consists of set of \control Word tokens and data enclosed in parentheses.
            // First token specifies which type of math we have, it can be:
            // \a \b \d \f \i \l \o \r \s and \x.
            // Each can be followed by its optional tokens, that specifies options of the current math.
            // Main token and optional tokens are followed by data enclosed in parentheses.
            // Data items are separated by coma or semicolon (depending on culture).
            // Each data item can be a new token.

            RichString fieldCode = FieldEQCodeParser.Parse(this);

            try
            {
                // WORDSNET-8831 If we encounter an empty EQ field, we should show nothing,
                // that is why return an empty run here.
                fieldCode = fieldCode.Trim();
                if (fieldCode.Length <= 0)
                    return new Run(Document);

                mRunPr = Start.RunPr.Clone();

                // Enclose whole formula in OfficeMath paragraph and OMath.
                // This is how most of normal OfficeMaths look.
                OfficeMath para = new OfficeMath(Document, new MathObjectOMathPara(), mRunPr);
                OfficeMath oMath = new OfficeMath(Document, new MathObjectOMath(), mRunPr);
                // Set flag that indicates the OfficeMath is the result of EQ field conversion.
                // Flag is set only for top level OfficeMath.
                para.IsConvertedFromEQ = true;
                para.AppendChild(oMath);
                AddChildren(oMath, ReadMathNodes(fieldCode));

                return para;
            }
            catch
            {
                // Return "Error!", if the code did not manage to convert EQ field to OffceMath.
                Run errorRun = new Run(Document, "Error!");
                errorRun.RunPr = Start.RunPr.Clone();
                errorRun.Font.Bold = true;
                return errorRun;
            }
        }

        /// <summary>
        /// The core method for reading EQ field, reads field code character by character
        /// and builds the corresponding nodes collection.
        /// </summary>
        private List<Node> ReadMathNodes(RichString fieldCode)
        {
            List<Node> mathNodes = new List<Node>();

            // There is nothing to parse, return empty collection.
            if (fieldCode == null || fieldCode.Length == 0)
                return mathNodes;

            StringBuilder currentToken = new StringBuilder();
            RichStringBuilder currentData = new RichStringBuilder();
            List<string> tokens = new List<string>();
            List<RichString> data = new List<RichString>();
            List<RichChar> openBrackets = new List<RichChar>();

            for (int i = 0; i < fieldCode.Length; i++)
            {
                RichChar c = fieldCode.GetInternal(i);

                // WORDSNET-28618 If field EQ contains underline settings, the mathematical formula must also be underlined.
                SetUnderlineSettings(c);
           
                if (IsTokenStart(c.Character) && (openBrackets.Count == 0))
                {
                    ReadTokenStart(c, currentToken, tokens);
                }
                else if (IsDataStart(c.Character))
                {
                    ReadDataStart(c, currentToken, tokens, openBrackets, currentData);
                }
                else if (IsDataEnd(c.Character))
                {
                    // WORDSNET-28652 Empty brackets () in a displacement EQ field indicate that the displacement is
                    // applied to the following character.
                    if (tokens.Count > 0 && IsDisplacementToken(tokens[tokens.Count - 1]) &&
                        (openBrackets.Count == 1) && (i < fieldCode.Length - 1))
                    {
                        data.Add(fieldCode.GetInternal(i + 1).ToRichString());
                        i++;
                    }

                    ReadDataEnd(c, currentToken, tokens, openBrackets, currentData, data, mathNodes);
                }
                else if (IsDataSeparator(c.Character))
                {
                    ReadDataSeparator(c, openBrackets, currentData, data);
                }
                else
                {
                    ReadText(c, currentToken, openBrackets, currentData, mathNodes);
                }
            }

            return mathNodes;
        }

        /// <summary>
        /// Indicates whether the current token specifies left or right displacement
        /// </summary>
        private static bool IsDisplacementToken(string token)
        {
            return token.StartsWith(LeftDisplacement, StringComparison.Ordinal) ||
                token.StartsWith(RightDisplacement, StringComparison.Ordinal);
        }

        /// <summary>
        /// Sets underline settings for <see cref="RunPr"/> for the math formula.
        /// </summary>
        private void SetUnderlineSettings(RichChar c)
        {
            if (c.RunPr.Underline == Underline.None)
                return;

            mRunPr.Underline = c.RunPr.Underline;
            mRunPr.UnderlineColor = c.RunPr.UnderlineColor;
        }

        private static void ReadTokenStart(RichChar c, StringBuilder currentToken, ICollection<string> tokens)
        {
            if (currentToken.Length != 0)
            {
                tokens.Add(currentToken.ToString().Trim());
                currentToken.Length = 0;
            }

            currentToken.Append(c.Character);
        }

        private static void ReadDataStart(
            RichChar c,
            StringBuilder currentToken,
            ICollection<string> tokens,
            ICollection<RichChar> openBrackets,
            RichStringBuilder currentData)
        {
            // If length of the current token is 1, it means we just opened it,
            // and open parenthesis should be considered as a part of token.
            if (currentToken.Length == 1)
            {
                currentToken.Append(c.Character);
                return;
            }

            if ((currentToken.Length != 0) && (openBrackets.Count == 0))
            {
                tokens.Add(currentToken.ToString().Trim());
                currentToken.Length = 0;
            }

            if (openBrackets.Count > 0)
                currentData.AppendInternal(c);

            openBrackets.Add(c);
        }

        private void ReadDataEnd(
            RichChar c,
            StringBuilder currentToken,
            IList<string> tokens,
            IList<RichChar> openBrackets,
            RichStringBuilder currentData,
            IList<RichString> data,
            ICollection<Node> mathNodes)
        {
            // If length of the current token is 1, it means we just opened it,
            // and close parenthesis should be considered as a part of token.
            if (currentToken.Length == 1)
            {
                currentToken.Append(c.Character);
                return;
            }

            if (openBrackets.Count == 1)
            {
                data.Add(currentData.ToRichString());
                currentData.Clear();
                currentToken.Length = 0;

                // WORDSNET-11226 Brackets can contain common text.
                if (tokens.Count != 0)
                    mathNodes.Add(EqToOfficeMath(tokens, data));
                else
                    AddText(openBrackets[0], data[0], c, mathNodes);

                tokens.Clear();
                data.Clear();
            }

            openBrackets.RemoveAt(openBrackets.Count - 1);

            if (openBrackets.Count > 0)
                currentData.AppendInternal(c);
        }

        private static void ReadDataSeparator(
            RichChar c,
            ICollection<RichChar> openBrackets,
            RichStringBuilder currentData,
            ICollection<RichString> data)
        {
            if (openBrackets.Count == 1)
            {
                // WORDSNET-28687 Whitespace inside brackets is significant and must be preserved; therefore, the string
                // should not be trimmed.
                data.Add(currentData.ToRichString());
                currentData.Clear();
            }
            else
            {
                currentData.AppendInternal(c);
            }
        }

        private void ReadText(
            RichChar c,
            StringBuilder currentToken,
            ICollection<RichChar> openBrackets,
            RichStringBuilder currentData,
            ICollection<Node> mathNodes)
        {
            if (openBrackets.Count > 0)
                currentData.AppendInternal(c);
            else if (currentToken.Length != 0)
                currentToken.Append(c.Character);
            else
                AddText(c, mathNodes);
        }

        private void AddText(RichChar c, ICollection<Node> mathNodes)
        {
            Run run = new Run(Document, c.Character.ToString());
            run.RunPr = c.RunPr;
            mathNodes.Add(run);
        }

        private void AddText(RichChar openBracket, RichString text, RichChar closeBracket,
            ICollection<Node> mathNodes)
        {
            AddText(openBracket, mathNodes);

            for (int i = 0; i < text.Length; i++)
                AddText(text.GetInternal(i), mathNodes);

            AddText(closeBracket, mathNodes);
        }

        /// <summary>
        /// Converts QE field token and its data to the corresponding <see cref="OfficeMath"/> node.
        /// </summary>
        private OfficeMath EqToOfficeMath(IList<string> tokens, IList<RichString> data)
        {
            string keyToken = tokens[0];
           
            // \a \b \d \f \i \l \o \r \s and \x.
            // WORDSNET-11226 Token can be uppercase.
            switch (keyToken.ToLower())
            {
                case "\\a":
                    return ArrayToOfficeMath(tokens, data);
                case "\\b":
                    return BracketToOfficeMath(tokens, data);
                case "\\d":
                    return DisplaceToOfficeMath(tokens, data);
                case "\\f":
                    return FractionToOfficeMath(data);
                case "\\i":
                    return IntegralToOfficeMath(tokens, data);
                case "\\l":
                    return ListToOfficeMath(data);
                case "\\o":
                    return OverstrikeToOfficeMath(tokens, data);
                case "\\r":
                    return RadicalToOfficeMath(data);
                case "\\s":
                    return ScriptToOfficeMath(tokens, data);
                case "\\x":
                    return BoxToOfficeMath(tokens, data);
                default:
                    throw new ArgumentException("Unexpected token.");
            }
        }

        /// <summary>
        /// Converts \a EQ token (array) to the corresponding OfficeMath node.
        /// </summary>
        private OfficeMath ArrayToOfficeMath(IList<string> tokens, IList<RichString> data)
        {
            MathObjectMatrix matrixObject = new MathObjectMatrix();
            matrixObject.IsEqArray = true;
            OfficeMath matrix = new OfficeMath(Document, matrixObject, mRunPr);

            MathMatrixColumnPr columnPr = new MathMatrixColumnPr();
            int columnsCount = 1;

            foreach (string token in tokens)
            {
                Match match = gTokenRegex.Match(token);

                string tokenName = match.Groups[1].Value.Trim();
                int tokenValue = StringUtil.HasChars(match.Groups[2].Value) ? FormatterPal.ParseInt(match.Groups[2].Value) : 0;

                switch (tokenName)
                {
                    case "\\al":
                        columnPr.HorizontalAlignment = HorizontalAlignment.Left;
                        break;
                    case "\\ac":
                        columnPr.HorizontalAlignment = HorizontalAlignment.Center;
                        break;
                    case "\\ar":
                        columnPr.HorizontalAlignment = HorizontalAlignment.Right;
                        break;
                    case "\\co":
                        // WORDSNET-19803 The number of columns cannot be less than 1.
                        columnsCount = System.Math.Max(columnsCount, tokenValue);
                        break;
                    case "\\vs":
                        matrixObject.RowSpacingRule = MathSpacingRule.Exactly;
                        matrixObject.RowSpacing = ConvertUtilCore.PointToTwip(tokenValue);
                        break;
                    case "\\hs":
                        matrixObject.ColumnSpacingRule = MathSpacingRule.Exactly;
                        matrixObject.ColumnGap = ConvertUtilCore.PointToTwip(tokenValue);
                        break;
                    default:
                        break;
                }
            }

            // Add columns.
            for (int i = 0; i < columnsCount; i++)
                matrixObject.ColumnPrCollection.Add(columnPr.Clone());
            // Set the minimum width. This is an experimental value.
            matrixObject.MinimumColumnWidth = 25;

            // Add rows.
            int currentColumn = 0;
            int rowCount = 0;
            OfficeMath currentRow = null;

            foreach (RichString item in data)
            {
                // Create a new row if needed
                if (currentRow == null || currentColumn == columnsCount)
                {
                    currentRow = CreateMatrixRow(matrix, mRunPr, false);
                    currentColumn = 0;
                    rowCount++;
                }

                MathObjectArgumentBase argBase = new MathObjectArgumentBase(MathObjectType.Argument);
                OfficeMath argument = new OfficeMath(Document, argBase,mRunPr);
                AddChildren(argument, ReadMathNodes(item));
                currentRow.AppendChild(argument);
                currentColumn++;
            }

            // WORDSNET-28688 Use enlarged row spacing if rows count is greater than 2. The method is experimental.
            if (matrixObject.RowSpacingRule == MathSpacingRule.Default && rowCount > 2)
            {
                matrixObject.RowSpacingRule = MathSpacingRule.Exactly;
                matrixObject.RowSpacing = ConvertUtilCore.PointToTwip(2.5);
            }

            return matrix;
        }

        /// <summary>
        /// Converts \b EQ token (bracket) to the corresponding OfficeMath node.
        /// </summary>
        private OfficeMath BracketToOfficeMath(IList<string> tokens, IList<RichString> data)
        {
            MathObjectDelimiter bracketsObject = new MathObjectDelimiter();
            string prevToken = string.Empty;

            foreach (string token in tokens)
                switch (token)
                {
                    case "\\b":
                        prevToken = token;
                        break;
                    case "\\rc":
                    case "\\lc":
                    case "\\bc":
                        // WORDSNET-19803 If \\rc,\\lc,\\bc are specified, then the default character for brackets is empty.
                        if (prevToken == "\\b")
                        {
                            const char emptyChar = char.MinValue;
                            bracketsObject.BeginningCharacter = emptyChar;
                            bracketsObject.EndingCharacter = emptyChar;
                        }
                        prevToken = token;
                        break;
                    default:
                    {
                        char c = token.Length > 1 ? token[1] : ' ';
                        switch (prevToken)
                        {
                            case "\\rc":
                                bracketsObject.EndingCharacter = c;
                                break;
                            case "\\lc":
                                bracketsObject.BeginningCharacter = c;
                                break;
                            case "\\bc":
                                bracketsObject.BeginningCharacter = c;
                                bracketsObject.EndingCharacter = GetClosingBracket(c);
                                break;
                            default:
                                break;
                        }

                        break;
                    }
                }

            OfficeMath brackets = new OfficeMath(Document, bracketsObject, mRunPr);
            OfficeMath argument = new OfficeMath(Document, new MathObjectArgumentBase(MathObjectType.Argument), mRunPr);
            AddChildren(argument, ReadMathNodes(data[0]));
            brackets.AppendChild(argument);

            return brackets;
        }

        /// <summary>
        /// Converts \d EQ token (displace) to the corresponding OfficeMath node.
        /// </summary>
        private OfficeMath DisplaceToOfficeMath(IList<string> tokens, IList<RichString> data)
        {
            OfficeMath displacement = new OfficeMath(Document, new MathObjectPhantom(), mRunPr);
            OfficeMath argument = new OfficeMath(Document, new MathObjectArgumentBase(MathObjectType.Argument), mRunPr);
            displacement.AppendChild(argument);
            foreach (string token in tokens)
            {
                Match match = gTokenRegex.Match(token);
                string tokenName = match.Groups[1].Value.Trim().ToLowerInvariant();
                string valueString = match.Groups[2].Value;
                int tokenValue = StringUtil.HasChars(valueString) ? FormatterPal.ParseInt(valueString) : 0;

                switch (tokenName)
                {
                    case LeftDisplacement:
                        argument.Displacement = -tokenValue;
                        break;
                    case RightDisplacement:
                        argument.Displacement = tokenValue;
                        break;
                    case UnderlineSpaceUp:
                    default:
                        break;
                }
            }

            if (data.Count > 0)
                AddChildren(argument, ReadMathNodes(data[0]));

            return displacement;
        }

        /// <summary>
        /// Converts \f EQ token (fraction) to the corresponding OfficeMath node.
        /// </summary>
        private OfficeMath FractionToOfficeMath(IList<RichString> data)
        {
            OfficeMath fraction = new OfficeMath(Document, new MathObjectFraction(), mRunPr);

            OfficeMath numerator = new OfficeMath(Document, new MathObjectArgumentBase(MathObjectType.Numerator), mRunPr);
            OfficeMath denominator = new OfficeMath(Document, new MathObjectArgumentBase(MathObjectType.Denominator), mRunPr);

            fraction.AppendChild(numerator);
            fraction.AppendChild(denominator);

            AddChildren(numerator, ReadMathNodes(data[0]));
            AddChildren(denominator, ReadMathNodes(data[1]));

            return fraction;
        }

        /// <summary>
        /// Converts \i EQ token (integral) to the corresponding OfficeMath node.
        /// </summary>
        private OfficeMath IntegralToOfficeMath(IList<string> tokens, IList<RichString> data)
        {
            MathObjectNAry naryObject = new MathObjectNAry();
            naryObject.LimitLocation = MathLimitLocation.UnderOver;
            string prevToken = string.Empty;
            naryObject.GrowToMatchOperand = true;

            foreach (string token in tokens)
            {
                switch (token)
                {
                    case "\\su":
                        naryObject.Character = '\x2211';
                        break;
                    case "\\pr":
                        naryObject.Character = '\x220F';
                        break;
                    case "\\in":
                        naryObject.LimitLocation = MathLimitLocation.SubscriptSuperscript;
                        break;
                    case "\\fc":
                    case "\\vc":
                        prevToken = token;
                        break;
                    default:
                    {
                        switch (prevToken)
                        {
                            case "\\fc":
                                naryObject.Character = token[1];
                                naryObject.GrowToMatchOperand = false;
                                break;
                            case "\\vc":
                                naryObject.Character = token[1];
                                naryObject.GrowToMatchOperand = true;
                                break;
                            default:
                                break;
                        }

                        break;
                    }
                }
            }

            OfficeMath nary = new OfficeMath(Document, naryObject, mRunPr);

            OfficeMath subscriptPart = new OfficeMath(Document, new MathObjectArgumentBase(MathObjectType.SubscriptPart), mRunPr);
            OfficeMath superscriptPart = new OfficeMath(Document, new MathObjectArgumentBase(MathObjectType.SuperscriptPart), mRunPr);
            OfficeMath argument = new OfficeMath(Document, new MathObjectArgumentBase(MathObjectType.Argument), mRunPr);

            nary.AppendChild(subscriptPart);
            nary.AppendChild(superscriptPart);
            nary.AppendChild(argument);

            AddChildren(subscriptPart, ReadMathNodes(data[0]));
            AddChildren(superscriptPart, ReadMathNodes(data[1]));
            AddChildren(argument, ReadMathNodes(data[2]));

            return nary;
        }

        /// <summary>
        /// Converts \l EQ token (list) to the corresponding OfficeMath node.
        /// </summary>
        private OfficeMath ListToOfficeMath(IList<RichString> data)
        {
            // List must display list of arguments separated by data delimiter.
            MathObjectDelimiter delimiterObject = new MathObjectDelimiter();
            delimiterObject.BeginningCharacter = '\0';
            delimiterObject.EndingCharacter = '\0';
            delimiterObject.SeparatorCharacter = DataSeparator;

            OfficeMath delimiter = new OfficeMath(Document, delimiterObject, mRunPr);

            foreach (RichString val in data)
            {
                OfficeMath argument = new OfficeMath(Document, new MathObjectArgumentBase(MathObjectType.Argument), mRunPr);
                delimiter.AppendChild(argument);
                AddChildren(argument, ReadMathNodes(val));
            }

            return delimiter;
        }

        /// <summary>
        /// Converts \o EQ token (overstrike) to the corresponding OfficeMath node.
        /// </summary>
        private OfficeMath OverstrikeToOfficeMath(IList<string> tokens, IList<RichString> data)
        {
            MathObjectMatrix matrixObject = new MathObjectMatrix();
            matrixObject.BaseJustification = MathBaseJustification.Top;
            OfficeMath matrix = new OfficeMath(Document, matrixObject, mRunPr);
            OfficeMath row = CreateMatrixRow(matrix, mRunPr, true);

            MathMatrixColumnPr columnPr = new MathMatrixColumnPr();
            foreach (string token in tokens)
            {
                switch (token)
                {
                    case "\\al":
                        columnPr.HorizontalAlignment = HorizontalAlignment.Left;
                        break;
                    case "\\ac":
                        columnPr.HorizontalAlignment = HorizontalAlignment.Center;
                        break;
                    case "\\ar":
                        columnPr.HorizontalAlignment = HorizontalAlignment.Right;
                        break;
                    default:
                        break;
                }
            }

            foreach (RichString val in data)
            {
                OfficeMath argument = new OfficeMath(Document, new MathObjectArgumentBase(MathObjectType.Argument), mRunPr);
                row.AppendChild(argument);
                matrixObject.ColumnPrCollection.Add(columnPr.Clone());
                AddChildren(argument, ReadMathNodes(val));
            }

            return matrix;
        }

        /// <summary>
        /// Converts \r EQ token (radical) to the corresponding OfficeMath node.
        /// </summary>
        private OfficeMath RadicalToOfficeMath(IList<RichString> data)
        {
            MathObjectRadical radicalObject = new MathObjectRadical();
            OfficeMath radical = new OfficeMath(Document, radicalObject, mRunPr);

            if (data.Count == 1)
            {
                // Add empty degree as MS Word does.
                OfficeMath degree = new OfficeMath(Document, new MathObjectArgumentBase(MathObjectType.Degree), mRunPr);
                radical.AppendChild(degree);

                OfficeMath argument = new OfficeMath(Document, new MathObjectArgumentBase(MathObjectType.Argument), mRunPr);
                AddChildren(argument, ReadMathNodes(data[0]));
                radical.AppendChild(argument);

                radicalObject.IsHideDegree = true;
            }
            else
            {
                OfficeMath degree = new OfficeMath(Document, new MathObjectArgumentBase(MathObjectType.Degree), mRunPr);
                AddChildren(degree, ReadMathNodes(data[0]));
                radical.AppendChild(degree);

                OfficeMath argument = new OfficeMath(Document, new MathObjectArgumentBase(MathObjectType.Argument), mRunPr);
                AddChildren(argument, ReadMathNodes(data[1]));
                radical.AppendChild(argument);
            }

            return radical;
        }

        /// <summary>
        /// Converts \s EQ token (superscript or subscript) to the corresponding OfficeMath node.
        /// </summary>
        private OfficeMath ScriptToOfficeMath(IList<string> tokens, IList<RichString> data)
        {
            // Super and sub scripts in QE field behaves differently that in OfficeMath,
            // That is why we represent it as matrix with two rows and one column.
            MathObjectMatrix matrixObject = new MathObjectMatrix();
            matrixObject.RowSpacingRule = MathSpacingRule.Exactly;
            matrixObject.ColumnPrCollection.Add(new MathMatrixColumnPr());
            OfficeMath matrix = new OfficeMath(Document, matrixObject, mRunPr);

            OfficeMath row0 = CreateMatrixRow(matrix, mRunPr, false);
            OfficeMath argument0 = new OfficeMath(Document, new MathObjectArgumentBase(MathObjectType.Argument), mRunPr);
            row0.AppendChild(argument0);

            OfficeMath row1 = CreateMatrixRow(matrix, mRunPr, false);
            OfficeMath argument1 = new OfficeMath(Document, new MathObjectArgumentBase(MathObjectType.Argument), mRunPr);
            row1.AppendChild(argument1);

            foreach (string token in tokens)
            {
                // WORDSNET-27858 The position can be negative. Regex should be changed. 
                Match match = gTokenRegex.Match(token);

                string tokenName = match.Groups[1].Value.Trim();
                string valueString = match.Groups[2].Value;

                // Tokens \s, \ai, \di are not currently supported. Do nothing.
                if (tokenName != "\\up" && tokenName != "\\do")
                    continue;

                // If token is not specified directly, MS Word uses 2 as the default value.
                int tokenValue = StringUtil.HasChars(valueString) ? FormatterPal.ParseInt(valueString) : 2;
                List<Node> nodes = ReadMathNodes(data[0]);
                bool swapArguments = (tokenName == "\\up" && tokenValue < 0) || (tokenName == "\\do" && tokenValue >= 0);
                OfficeMath upArg = swapArguments ? argument1 : argument0;
                OfficeMath downArg = swapArguments ? argument0 : argument1;
                AddScriptElements(upArg, downArg, nodes);
                matrixObject.RowSpacing = ConvertUtilCore.PointToTwip(System.Math.Abs(tokenValue));
                matrixObject.BaseJustification = swapArguments ? MathBaseJustification.Top : MathBaseJustification.Bottom;
            }

            return matrix;
        }

        /// <summary>
        /// Adds script elements.
        /// </summary>
        private void AddScriptElements(OfficeMath arg0, OfficeMath arg1, List<Node> nodes)
        {
            AddChildren(arg0, nodes);
            // Add dummy row value. If we do not add it, height of row will be zero that might lead into incorrect result.
            RunPr run = GetArgumentRunPr(arg0);
            RichString dummyString = RichString.CreateFromString(ControlChar.Space, run);
            List<Node> dummyNodes = ReadMathNodes(dummyString);
            AddChildren(arg1, dummyNodes);
        }

        /// <summary>
        /// Returns RunPr of the first child run of the specified OfficeMath,
        /// if there are no runs, returns RunPr of the OfficeMath itself.
        /// </summary>
        private static RunPr GetArgumentRunPr(OfficeMath math)
        {
            Run run = (Run)math.GetChild(NodeType.Run, 0, true);
            return (run != null) ? run.RunPr : math.RunPr;
        }

        /// <summary>
        /// Converts \b EQ token (box) to the corresponding OfficeMath node.
        /// </summary>
        private OfficeMath BoxToOfficeMath(IList<string> tokens, IList<RichString> data)
        {
            MathObjectBorderBox borderBoxObject = new MathObjectBorderBox();

            borderBoxObject.HideBottomEdge = true;
            borderBoxObject.HideLeftEdge = true;
            borderBoxObject.HideRightEdge = true;
            borderBoxObject.HideTopEdge = true;

            foreach (string token in tokens)
            {
                switch (token)
                {
                    case "\\to":
                        borderBoxObject.HideTopEdge = false;
                        break;
                    case "\\bo":
                        borderBoxObject.HideBottomEdge = false;
                        break;
                    case "\\le":
                        borderBoxObject.HideLeftEdge = false;
                        break;
                    case "\\ri":
                        borderBoxObject.HideRightEdge = false;
                        break;
                    default:
                        break;
                }
            }

            OfficeMath borderBox = new OfficeMath(Document, borderBoxObject, mRunPr);
            OfficeMath argument = new OfficeMath(Document, new MathObjectArgumentBase(MathObjectType.Argument), mRunPr);
            AddChildren(argument, ReadMathNodes(data[0]));
            borderBox.AppendChild(argument);

            return borderBox;
        }

        /// <summary>
        /// Creates matrix row and adds it to the parent matrix.
        /// </summary>
        private static OfficeMath CreateMatrixRow(OfficeMath parentMatrix, RunPr runPr, bool isOverstrikeRow)
        {
            MathObjectMatrixRow matrixRow = new MathObjectMatrixRow();
            matrixRow.IsOverstrikeRow = isOverstrikeRow;
            OfficeMath row = new OfficeMath(parentMatrix.Document, matrixRow, runPr);
            parentMatrix.AppendChild(row);

            return row;
        }

        /// <summary>
        /// Adds all nodes from children collection, to the parent node.
        /// </summary>
        private static void AddChildren(CompositeNode parent, IList<Node> children)
        {
            foreach (Node child in children)
                parent.AppendChild(child);
        }

        /// <summary>
        /// Returns closing bracket that corresponds the specified open bracket.
        /// If there is no corresponding closing bracket returns the original open bracket.
        /// </summary>
        private static char GetClosingBracket(char openBracket)
        {
            switch (openBracket)
            {
                case '{':
                    return '}';
                case '[':
                    return ']';
                case '(':
                    return ')';
                case '<':
                    return '>';
                default:
                    return openBracket;
            }
        }

        /// <summary>
        /// Returns true if the specified char is token start.
        /// </summary>
        private static bool IsTokenStart(char c)
        {
            return (c == '\\');
        }

        /// <summary>
        /// Returns true if the specified char is data start.
        /// </summary>
        private static bool IsDataStart(char c)
        {
            return (c == '(');
        }

        /// <summary>
        /// Returns true if the specified char is data end.
        /// </summary>
        private static bool IsDataEnd(char c)
        {
            return (c == ')');
        }

        /// <summary>
        /// Returns true if the specified char is data separator.
        /// </summary>
        private static bool IsDataSeparator(char c)
        {
            return (c == DataSeparator);
        }

        /// <summary>
        /// Returns either semicolon or coma depending on culture.
        /// </summary>
        private static char DataSeparator
        {
            get { return FormatterPal.GetDecimalSeparatorCurrent() == ',' ? ';' : ','; }
        }

        private const string LeftDisplacement = "\\ba";
        private const string RightDisplacement = "\\fo";
        private const string UnderlineSpaceUp = "\\li";
        private RunPr mRunPr;
        private static readonly Regex gTokenRegex = new Regex(@"(\\[^\d-]+)(-?\d+)?", RegexOptions.Compiled);

        private sealed class FieldEQCodeParser : IDisposable
        {
            private readonly NodeEnumerator mNodeEnumerator;
            private readonly FieldCharCounter mFieldCharCounter;

            private FieldEQCodeParser(NodeRange range)
            {
                mNodeEnumerator = new NodeEnumerator(range);
                mFieldCharCounter = new FieldCharCounter();
            }

            internal static RichString Parse(Field field)
            {
                FieldArgument argument = field.FieldCodeCache.GetArgument(0);
                if (argument == null)
                    return RichString.Empty;

                using (FieldEQCodeParser parser = new FieldEQCodeParser(argument.Range))
                    return parser.Parse();
            }

            private RichString Parse()
            {
                RichStringBuilder builder = new RichStringBuilder();

                while (true)
                {
                    if (!mNodeEnumerator.MoveToNextNode(true, false))
                        break;

                    mFieldCharCounter.VisitNode(CurrentNode);

                    ProcessNode(builder);
                }

                return builder.ToRichString();
            }

            private void ProcessNode(RichStringBuilder builder)
            {
                switch (CurrentNode.NodeType)
                {
                    case NodeType.FieldStart:
                        ProcessField(builder);
                        break;
                    case NodeType.Run:
                        if (mFieldCharCounter.IsInFieldCode)
                            break;
                        ProcessRun(builder);
                        break;
                    default:
                        // Do nothing.
                        break;
                }
            }

            private void ProcessField(RichStringBuilder builder)
            {
                FieldStart fieldStart = (FieldStart)CurrentNode;

                if (mFieldCharCounter.FieldsDepth != 1)
                    return;

                switch (fieldStart.FieldType)
                {
                    case FieldType.FieldSymbol:
                    case FieldType.FieldAutoNum:
                    case FieldType.FieldAutoNumLegal:
                    case FieldType.FieldAutoNumOutline:
                    case FieldType.FieldGoToButton:
                    case FieldType.FieldMacroButton:
                        NodeRange fakeResult = fieldStart.GetField().GetFakeResult();
                        ProcessNodeRange(builder, fakeResult);
                        break;
                    case FieldType.FieldEquation:
                        // WORDSNET-22098 Append any nested EQ field codes recursively and then convert them to OfficeMath as single field code.
                        builder.AppendInternal(Parse(fieldStart.GetField()));
                        break;
                    default:
                        // Do nothing.
                        break;
                }
            }

            private static void ProcessNodeRange(RichStringBuilder builder, NodeRange range)
            {
                if (range == null)
                    return;

                foreach (Node node in range)
                {
                    Run run = node as Run;
                    if (run != null)
                        builder.Append(run.Text, run.RunPr);
                }
            }

            private void ProcessRun(RichStringBuilder builder)
            {
                if (CurrentNodePosition.IsEnd)
                    return;

                Run run = (Run)CurrentNode;

                builder.Append(run.Text[CurrentNodeOffset], run.RunPr);
            }

            private Node CurrentNode
            {
                get { return mNodeEnumerator.CurrentNode; }
            }

            private DocumentPosition CurrentNodePosition
            {
                get { return mNodeEnumerator.CurrentPosition; }
            }

            private int CurrentNodeOffset
            {
                get { return mNodeEnumerator.CurrentPosition.Offset; }
            }

            void IDisposable.Dispose()
            {
                mNodeEnumerator.Dispose();
            }
        }
    }
}
