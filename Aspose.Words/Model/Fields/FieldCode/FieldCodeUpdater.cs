// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/12/2011 by Dmitry Vorobyev

using System.Text;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Provides a set of methods that build a field code string based on an existing field code, adding, replacing,
    /// or removing an argument or a switch.
    /// </summary>
    internal static class FieldCodeUpdater
    {
        internal static string EncodeArgument(string argumentText)
        {
            return EncodeArgument(argumentText, false, false);
        }

        internal static string EncodeArgument(string argumentText, bool forceDoubleQuotes)
        {
            return EncodeArgument(argumentText, false, forceDoubleQuotes);
        }

        private static string EncodeArgument(string argumentText, bool isVerbatim, bool forceDoubleQuotes)
        {
            StringBuilder builder = new StringBuilder(argumentText.Length);
            bool containsWhitespaces = false;
            foreach (char symbol in argumentText)
            {
                if (FieldCodeParser.IsDoubleQuote(symbol) || (symbol == '\\'))
                    builder.Append('\\');

                builder.Append(symbol);

                containsWhitespaces = containsWhitespaces || char.IsWhiteSpace(symbol);
            }

            // If the argument is an empty string or contains any whitespaces, enclose it in double quotes.
            if (!isVerbatim && (forceDoubleQuotes || (builder.Length == 0) || containsWhitespaces))
                builder.Insert(0, '"').Append('"');

            return builder.ToString();
        }

        internal static void AppendSwitch(FieldCode fieldCode, string switchName, string switchArgument)
        {
            // Picture switch should always be first.
            if ((FieldSwitch.IsPictureSwitch(switchName)))
                AppendSwitchBeforeFirstSwitch(fieldCode, switchName, switchArgument);
            else
                AppendSwitchToEnd(fieldCode, switchName, switchArgument);
        }

        private static void AppendSwitchBeforeFirstSwitch(FieldCode fieldCode, string switchName, string switchArgument)
        {
            if (fieldCode.Switches.Count == 0)
            {
                AppendSwitchToEnd(fieldCode, switchName, switchArgument);
                return;
            }

            DocumentBuilder builder = new DocumentBuilder(fieldCode.Field.FetchDocument());
            MoveBeforeFirstSwitch(fieldCode, builder);

            builder.Write(switchName);
            if (switchArgument != null)
                WriteArgumentText(builder, true, switchArgument, false);

            builder.Write(" ");
        }

        private static void AppendSwitchToEnd(FieldCode fieldCode, string switchName, string switchArgument)
        {
            DocumentBuilder builder = new DocumentBuilder(fieldCode.Field.FetchDocument());
            MoveBeforeFieldCodeEnd(fieldCode, builder);

            builder.Write(string.Format(@" {0}", switchName));

            if (switchArgument != null)
                WriteArgumentText(builder, true, switchArgument, false);
        }

        internal static void RemoveSwitch(FieldSwitch fieldSwitch)
        {
            NodeRange range = fieldSwitch.Argument == null ?
                fieldSwitch.Range :
                new NodeRange(fieldSwitch.Range.Start, fieldSwitch.Argument.Range.End);

            range.Remove();
        }

        internal static void AppendArguments(FieldCode fieldCode, string[] argumentsTexts, bool lastArgumentIsVerbatim)
        {
            DocumentBuilder builder = new DocumentBuilder(fieldCode.Field.FetchDocument());
            bool separateFromPrevious = false;
            bool separateFromNext = false;

            bool moved = false;
            if (fieldCode.Switches.Count != 0)
            {
                moved = MoveAfterLastArgument(fieldCode, builder);
                separateFromNext = moved;
            }

            if (!moved)
            {
                MoveBeforeFieldCodeEnd(fieldCode, builder);
                separateFromPrevious = true;
            }

            for (int i = 0; i < argumentsTexts.Length; i++)
            {
                string argumentText = argumentsTexts[i];
                bool isVerbatimArgument = lastArgumentIsVerbatim && (i == argumentsTexts.Length - 1);

                WriteArgumentText(builder, separateFromPrevious, argumentText, isVerbatimArgument);

                if (separateFromNext && !isVerbatimArgument)
                    builder.Write(" ");
            }
        }

        internal static void UpdateArgument(FieldCode fieldCode, FieldArgument argument, string argumentText, bool isVerbatimArgument)
        {
            DocumentBuilder builder = new DocumentBuilder(fieldCode.Field.FetchDocument());
            NodeRange argumentActualRange = GetFieldArgumentActualRange(argument);
            builder.MoveTo(argumentActualRange.Start.Node);

            WriteArgumentText(builder, false, argumentText, isVerbatimArgument);

            argumentActualRange.Remove();
        }

        internal static void RemoveArgument(FieldArgument argument)
        {
            GetFieldArgumentActualRange(argument).Remove();
        }

        internal static void AppendTrailingSpace(FieldCode fieldCode)
        {
            DocumentBuilder builder = new DocumentBuilder(fieldCode.Field.FetchDocument());
            MoveBeforeFieldCodeEnd(fieldCode, builder);

            builder.Write(" ");
        }

        private static NodeRange GetFieldArgumentActualRange(FieldArgument argument)
        {
            return !argument.CompleteFieldRange.IsVoid
                       ? argument.CompleteFieldRange
                       : argument.Range;
        }

        private static void MoveBeforeFieldCodeEnd(FieldCode fieldCode, DocumentBuilder builder)
        {
            Node refNode = fieldCode.Field.FieldCodeEnd;
            builder.MoveTo(refNode);
        }

        private static bool MoveAfterLastArgument(FieldCode fieldCode, DocumentBuilder builder)
        {
            FieldArgument lastArgument = fieldCode.GetArgument(fieldCode.Arguments.Count - 1);
            if (lastArgument == null)
            {
                MoveBeforeFirstSwitch(fieldCode, builder);
                return true;
            }

            FieldSwitch switchAfterLastArgument = null;
            bool lastArgumentReached = false;
            foreach (object element in fieldCode.Elements)
            {
                if (lastArgumentReached)
                {
                    switchAfterLastArgument = (FieldSwitch)element;
                    break;
                }

                if (element == lastArgument)
                    lastArgumentReached = true;
            }

            if (switchAfterLastArgument == null)
                return false;

            Node refNode = switchAfterLastArgument.Range.Start.Node;
            builder.MoveTo(refNode);
            return true;
        }

        private static void MoveBeforeFirstSwitch(FieldCode fieldCode, DocumentBuilder builder)
        {
            FieldSwitch fieldSwitch = fieldCode.GetSwitch(0);
            Node refNode = fieldSwitch.Range.Start.Node;
            builder.MoveTo(refNode);
        }

        private static void WriteArgumentText(DocumentBuilder builder, bool separate, string argumentText, bool isVerbatimArgument)
        {
            if (separate)
                builder.Write(" ");

            builder.Write(argumentText == null ? "\"\"" : EncodeArgument(argumentText, isVerbatimArgument, false));
        }
    }
}
