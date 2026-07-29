// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/08/2009 by Dmitry Vorobyev

using System;
using System.Collections.Generic;
using Aspose.Common;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Represents a field code parsed to field parts like field arguments or switches.
    /// Parsing field code may be required during field update or in other cases (e.g. to read field format
    /// when setting field result). That's why we have different ctors, and FieldUpdateContext may be null.
    /// </summary>
    internal class FieldCode : IFieldCodeTokenInfoProvider, IFieldCode
    {
        internal FieldCode(string fieldCode, IFieldCodeTokenInfoProvider tokenInfoProvider)
        {
            mTokenInfoProvider = tokenInfoProvider;
            mVerbatimArgumentIndex = FieldUtil.GetVerbatimArgumentIndex(fieldCode);
            mIgnoreSwitchesInFieldType = FieldUtil.IgnoreSwitchesInFieldType(fieldCode);

            using (FieldCodeParser parser = new FieldCodeParser(fieldCode, this))
                Parse(parser);
            mLanguageId = RunPr.ProcessOrUserDefaultLanguageId;
        }

        internal FieldCode(Field field)
        {
            mField = field;
            mTokenInfoProvider = field as IFieldCodeTokenInfoProvider;
            mVerbatimArgumentIndex = FieldUtil.GetVerbatimArgumentIndex(field.Type);
            mIgnoreSwitchesInFieldType = FieldUtil.IgnoreSwitchesInFieldType(field.Type);

            ParseField();

            mUpdateSupported = true;
        }

        private void ParseField()
        {
            mArguments.Clear();
            mSwitches.Clear();
            mElements.Clear();

            using (FieldCodeParser parser = new FieldCodeParser(mField, this))
            {
                Parse(parser);

                // Remember the field language.
                mLanguageId = parser.LanguageId;
                LanguageIdFarEast = parser.LanguageIdFarEast;
                mLanguageIdBi = parser.LanguageIdBi;
                mBidi = parser.Bidi;
                mLanguageNode = parser.LanguageNode;
            }
        }

        /// <summary>
        /// Parses field code into arguments and switches. I have noticed that arguments and switches (at least formatting
        /// switches) may be intermixed. For example, this construction works: IF 2 > 1  \* MERGEFORMAT True False
        /// Therefore we should collect arguments and switches independently.
        /// </summary>
        private void Parse(FieldCodeParser parser)
        {
            parser.MoveToFirstToken(mIgnoreSwitchesInFieldType);
            FieldType = parser.CurrentToken;

            string switchName = null;
            NodeRange switchRange = null;

            while (parser.MoveToNextToken(IsVerbatimArgument && switchName == null, IgnoreSymbolicQuotesArgument(switchName)))
            {
                if (parser.IsSwitch)
                {
                    if (switchName != null)
                    {
                        if (FieldSwitch.IsFormattingSwitch(switchName))
                            ParseErrorMessage = "Error! Switch argument not specified.";

                        AddSwitch(switchRange, switchName);
                        switchName = null;
                        switchRange = null;
                    }

                    if (GetSwitchType(parser.CurrentToken) != FieldSwitchType.Unknown)
                    {
                        switchName = parser.CurrentToken;
                        switchRange = parser.GetCurrentNodeRange();

                        if (FieldSwitch.IsPictureSwitch(switchName))
                        {
                            if (HasPictureSwitch())
                                ParseErrorMessage = "Error! Too many picture switches defined.";
                            else if (HasFormattingSwitch())
                                ParseErrorMessage = "Error! Picture switch must be first formatting switch.";
                        }
                    }
                }
                else
                {
                    bool isFieldArgument = (switchName == null) || (GetSwitchType(switchName) != FieldSwitchType.HasArgument);

                    FieldArgument argument = new FieldArgument(
                        mField,
                        parser.CurrentToken,
                        parser.CurrentRichToken,
                        parser.GetCurrentNodeRange(),
                        parser.GetCompleteFieldNodeRange(),
                        parser.IsCurrentTokenInDoubleQuotes);

                    if (isFieldArgument)
                    {
                        if (switchName != null)
                        {
                            AddSwitch(switchRange, switchName);
                            switchName = null;
                            switchRange = null;
                        }

                        // This is a field argument.
                        AddArgument(argument);
                    }
                    else
                    {
                        // This is a switch argument.
                        AddSwitch(switchRange, switchName, argument);
                        switchName = null;
                        switchRange = null;
                    }

                    // Notify field update context (if any) about that the argument is added.
                    if ((mField != null) && mField.IsUpdating)
                        mField.UpdateContext.NotifyArgumentAdded(argument);
                }
            }

            if (switchName != null)
                AddSwitch(switchRange, switchName);
        }

        private bool HasFormattingSwitch()
        {
            foreach (FieldSwitch fieldSwitch in mSwitches)
            {
                if (fieldSwitch.IsFormatting)
                    return true;
            }

            return false;
        }

        private bool HasPictureSwitch()
        {
            foreach (FieldSwitch fieldSwitch in mSwitches)
            {
                if (fieldSwitch.IsPicture)
                    return true;
            }

            return false;
        }

        private void AddSwitch(NodeRange range, string switchName)
        {
            AddSwitch(range, switchName, null);
        }

        private void AddSwitch(NodeRange range, string switchName, FieldArgument switchArgument)
        {
            FieldSwitch fieldSwitch = new FieldSwitch(range, switchName, switchArgument);
            mSwitches.Add(fieldSwitch);
            mElements.Add(fieldSwitch);
        }

        private void AddArgument(FieldArgument argument)
        {
            mArguments.Add(argument);
            mElements.Add(argument);
        }

        private void UpdateSwitch(int elementIndex, FieldSwitch fieldSwitch)
        {
            int index = mSwitches.IndexOf((FieldSwitch)mElements[elementIndex]);
            mSwitches[index] = fieldSwitch;
            mElements[elementIndex] = fieldSwitch;
        }

        private void UpdateArgument(int elementIndex, FieldArgument fieldArgument)
        {
            int index = mArguments.IndexOf((FieldArgument)mElements[elementIndex]);
            mArguments[index] = fieldArgument;
            mElements[elementIndex] = fieldArgument;
        }

        /// <summary>
        /// Returns <c>true</c> if the switch was found in the field code.
        /// </summary>
        public bool HasSwitch(string switchName)
        {
            return GetSwitch(switchName) != null;
        }

        /// <summary>
        /// Returns the argument of the specified switch or null if the switch was not found in field code
        /// or is missing the argument.
        /// </summary>
        internal FieldArgument GetSwitchArgument(string switchName)
        {
            FieldSwitch fieldSwitch = GetSwitch(switchName);
            return fieldSwitch != null
                ? fieldSwitch.Argument
                : null;
        }

        /// <summary>
        /// Returns all arguments of the specified switch that were found in field code.
        /// </summary>
        internal IEnumerable<FieldArgument> GetSwitchArguments(string switchName)
        {
            List<FieldArgument> result = new List<FieldArgument>();

            foreach (FieldSwitch fieldSwitch in mSwitches)
            {
                if (fieldSwitch.HasName(switchName) && fieldSwitch.HasArgument)
                    result.Add(fieldSwitch.Argument);
            }

            return result;
        }

        /// <summary>
        /// Gets the range for the argument of a switch with the specified name.
        /// </summary>
        /// <remarks>
        /// Creates a fake empty range using parentDocument if field argument is missing.
        /// A field with an empty field argument shall be saved when converting to RTF to mimic Word.
        /// </remarks>
        internal NodeRange GetSwitchArgumentRange(string switchName)
        {
            return GetArgumentRange(GetSwitchArgument(switchName));
        }

        /// <summary>
        /// Returns the argument of the specified switch as a text string or null
        /// if the switch was not found in field code or is missing the argument.
        /// </summary>
        internal string GetSwitchArgumentAsString(string switchName)
        {
            return GetSwitchArgumentAsString(switchName, false);
        }

        /// <summary>
        /// Returns the argument of the specified switch as a text string or null or empty string
        /// if the switch was not found in field code or is missing the argument.
        /// </summary>
        internal string GetSwitchArgumentAsString(string switchName, bool replaceNullWithEmptyString)
        {
            FieldArgument argument = GetSwitchArgument(switchName);
            string result = argument != null
                ? argument.GetNormalizedText()
                : null;

            if (result != null)
                return result;

            return replaceNullWithEmptyString
                ? string.Empty
                : null;
        }

        internal RichString GetSwitchArgumentAsRichString(string switchName)
        {
            return GetSwitchArgumentAsRichString(switchName, false);
        }

        private RichString GetSwitchArgumentAsRichString(string switchName, bool replaceNullWithEmptyString)
        {
            FieldArgument argument = GetSwitchArgument(switchName);
            RichString result = argument != null
                ? argument.GetNormalizedRichText()
                : null;

            if (result != null)
                return result;

            return replaceNullWithEmptyString
                ? RichString.Empty
                : null;
        }

        /// <summary>
        /// Returns all arguments of the specified switch that were found in field code as a text string.
        /// </summary>
        internal IList<string> GetSwitchArgumentsAsStrings(string switchName)
        {
            List<string> result = new List<string>();
            foreach (FieldArgument argument in GetSwitchArguments(switchName))
            {
                string argumentText = argument.GetNormalizedText();
                if (argumentText != null)
                    result.Add(argumentText);
            }

            return result;
        }

        /// <summary>
        /// Returns the argument of the specified switch as an integer number.
        /// Throws an exception if the switch was not found in field code.
        /// Returns null if the switch argument is missed or non-numeric.
        /// </summary>
        internal NullableInt32 GetSwitchArgumentAsInt32(string switchName)
        {
            if (!HasSwitch(switchName))
                throw new InvalidOperationException();

            return FormatterPal.ParseNullableInt(GetSwitchArgumentAsString(switchName));
        }

        /// <summary>
        /// Returns the argument of the specified switch as a floating point number.
        /// Returns -1 if the switch was not found in field code, is missing the argument or contains a non-numeric argument.
        /// </summary>
        internal double GetSwitchArgumentAsDouble(string switchName)
        {
            if (!HasSwitch(switchName))
                return -1d;

            double value = FormatterPal.TryParseDoubleCurrent(GetSwitchArgumentAsString(switchName));
            return !double.IsNaN(value) ? value : -1d;
        }

        /// <summary>
        /// Returns a value indicating whether a field argument with the specified index exists.
        /// </summary>
        internal bool HasArgument(int index)
        {
            return (index >= 0) && (index < mArguments.Count);
        }

        /// <summary>
        /// Returns a field argument at the specified index or null if the argument is omitted.
        /// </summary>
        internal FieldArgument GetArgument(int index)
        {
            return HasArgument(index) ? mArguments[index] : null;
        }

        internal FieldSwitch GetSwitch(int index)
        {
            return (index >= 0) && (index < mSwitches.Count)
                       ? mSwitches[index]
                       : null;
        }

        /// <summary>
        /// Gets the range for the field argument.
        /// </summary>
        /// <remarks>
        /// Creates a fake empty range using parentDocument if field argument is missing.
        /// A field with an empty field argument shall be saved when converting to RTF to mimic Word.
        /// </remarks>
        internal NodeRange GetArgumentRange(int argumentIndex)
        {
            return GetArgumentRange(GetArgument(argumentIndex));
        }

        private NodeRange GetArgumentRange(FieldArgument argument)
        {
            // WORDSNET-22419 NullReferenceException occurs during UpdateFields.
            // Field argument may be missing, though such field is incorrect.
            if (argument != null)
                return argument.Range;

            Run fakeRun = new Run(mField.Document);
            return new NodeRange(fakeRun, true, fakeRun, false);
        }

        /// <summary>
        /// Adds (or replaces if present) the specified argument at the specified position in the field code.
        /// If the position specified is too big, adds a number of empty arguments ("") to fill the gap.
        /// </summary>
        internal void SetArgument(int argumentIndex, string argumentText)
        {
            InitializeUpdate();

            Debug.Assert(argumentIndex >= 0);

            bool isVerbatimArgument = FieldUtil.GetVerbatimArgumentIndex(mField.Type) == argumentIndex;

            FieldArgument argument = GetArgument(argumentIndex);
            if (argument != null)
            {
                if (argumentText == null && argumentIndex == mArguments.Count - 1)
                    FieldCodeUpdater.RemoveArgument(argument);
                else
                    FieldCodeUpdater.UpdateArgument(this, argument, argumentText, isVerbatimArgument);

                FinalizeUpdate();
            }
            else if (argumentText != null)
            {
                string[] arguments = new string[argumentIndex - mArguments.Count + 1];
                arguments[arguments.Length - 1] = argumentText;
                FieldCodeUpdater.AppendArguments(this, arguments, isVerbatimArgument);

                FinalizeUpdate();
            }
        }

        /// <summary>
        /// Adds (or replaces) or removes a switch with the specified name to/from the field code.
        /// </summary>
        internal void SetSwitch(string switchName, bool set)
        {
            InitializeUpdate();

            Debug.Assert(switchName != null);

            bool switchRemoved = false;

            foreach (FieldSwitch fieldSwitch in mSwitches)
            {
                if (!fieldSwitch.HasName(switchName))
                    continue;

                if (set)
                    return;

                FieldCodeUpdater.RemoveSwitch(fieldSwitch);
                switchRemoved = true;

                // continue loop to remove all switches
            }

            if (switchRemoved)
            {
                FinalizeUpdate();
            }
            else if (set)
            {
                FieldCodeUpdater.AppendSwitch(this, switchName, null);
                FinalizeUpdate();
            }

        }

        /// <summary>
        /// Adds (or replaces) or removes a switch with the specified name and argument to/from the field code.
        /// </summary>
        internal void SetSwitch(string switchName, int switchArgument)
        {
            SetSwitch(switchName, switchArgument.ToString());
        }

        /// <summary>
        /// Adds (or replaces) or removes a switch with the specified name and argument to/from the field code.
        /// </summary>
        internal void SetSwitch(string switchName, double switchArgument)
        {
            SetSwitch(switchName, FormatterPal.DoubleToStr(switchArgument));
        }

        /// <summary>
        /// Adds (or replaces) or removes a switch with the specified name and argument to/from the field code.
        /// </summary>
        internal void SetSwitch(string switchName, string switchArgument)
        {
            SetSwitchInternal(switchName, switchArgument, -1);
        }

        internal void SetSwitchAsInt32(string switchName, string switchArgument)
        {
            int val = FormatterPal.TryParseInt(switchArgument);
            SetSwitch(switchName, val);
        }

        internal void SetSwitchAsDouble(string switchName, string switchArgument)
        {
            double val = FormatterPal.TryParseDoubleInvariant(switchArgument);
            SetSwitch(switchName, val);
        }

        /// <summary>
        /// Adds (or replaces) or removes a switch with the specified name and argument located at the specified position
        /// to/from the field code.
        /// </summary>
        internal void SetSwitch(string switchName, string switchArgument, int switchIndex)
        {
            Debug.Assert(switchName != null);
            Debug.Assert(switchIndex >= 0);

            SetSwitchInternal(switchName, switchArgument, switchIndex);
        }

        private void SetSwitchInternal(string switchName, string switchArgument, int switchIndex)
        {
            Debug.Assert(switchName != null);
            Debug.Assert(switchIndex >= -1);

            InitializeUpdate();

            bool setSingleSwitch = switchIndex != -1;
            bool switchFound = false;

            int index = 0;
            foreach (FieldSwitch fieldSwitch in mSwitches)
            {
                if (!fieldSwitch.HasName(switchName))
                    continue;

                if (setSingleSwitch && switchIndex != index)
                {
                    index++;
                    continue;
                }

                if (switchArgument != null)
                    FieldCodeUpdater.UpdateArgument(this, fieldSwitch.Argument, switchArgument, false);
                else
                    FieldCodeUpdater.RemoveSwitch(fieldSwitch);

                switchFound = true;

                if (setSingleSwitch)
                    break;
            }

            if (switchFound)
            {
                FinalizeUpdate();
            }
            else if (switchArgument != null)
            {
                FieldCodeUpdater.AppendSwitch(this, switchName, switchArgument);
                FinalizeUpdate();
            }
        }

        /// <summary>
        /// Gets an argument as a plain text at the specified index.
        /// </summary>
        public string GetArgumentAsString(int index)
        {
            return GetArgumentAsString(index, true);
        }

        /// <summary>
        /// Gets an argument as a plain text at the specified index.
        /// </summary>
        internal string GetArgumentAsString(int index, bool trimDoubleQuotes)
        {
            return GetArgumentAsString(index, trimDoubleQuotes, false);
        }

        /// <summary>
        /// Gets an argument as a plain text at the specified index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="trimDoubleQuotes">Double quotes should be trimmed in most cases except of in comparison
        /// expression operands because they are used there to determine whether to compare the operands as strings.</param>
        /// <param name="replaceNullWithEmptyString">Null arguments equal to empty strings in most cases except of
        /// in comparison fields where there is a need to know what arguments are omitted.</param>
        /// <returns></returns>
        internal string GetArgumentAsString(int index, bool trimDoubleQuotes, bool replaceNullWithEmptyString)
        {
            FieldArgument argument = GetArgument(index);

            if (argument == null)
                return replaceNullWithEmptyString ? string.Empty : null;

            return argument.GetNormalizedText(trimDoubleQuotes);
        }

        private FieldSwitch GetSwitch(string switchName)
        {
            // WORDSNET-3786 Page numbers lost in TOC when multiple \n switches are present.
            // Get the last switch with the given name.
            FieldSwitch result = null;

            foreach (FieldSwitch fieldSwitch in mSwitches)
            {
                if (fieldSwitch.HasName(switchName))
                    result = fieldSwitch;
            }

            return result;
        }

        public FieldSwitchType GetSwitchType(string switchName)
        {
            if (FieldSwitch.IsFormattingSwitch(switchName))
            {
                // The three formatting switches are common for all fields and may appear in any of them.
                return FieldSwitchType.HasArgument;
            }

            return (mTokenInfoProvider != null)
                ? mTokenInfoProvider.GetSwitchType(switchName.ToLower())
                : FieldSwitchType.Unknown;
        }

        /// <summary>
        /// Gets all collected field arguments, preserving the order.
        /// </summary>
        internal IList<FieldArgument> Arguments
        {
            get { return mArguments; }
        }

        /// <summary>
        /// Gets all collected switches.
        /// </summary>
        internal IList<FieldSwitch> Switches
        {
            get { return mSwitches; }
        }

        /// <summary>
        /// Gets all collected arguments and switches.
        /// </summary>
        internal IList<object> Elements
        {
            get { return mElements; }
        }

        internal string FieldType { get; private set; }

        internal Field Field
        {
            get { return mField; }
        }

        /// <summary>
        /// Gets the field language Id specified via field code formatting properties.
        /// </summary>
        internal int LanguageId
        {
            get { return mLanguageId; }
            set
            {
                if (mLanguageNode == null)
                    return;

                mLanguageId = value;
                mLanguageNode.Font.LocaleId = value;
            }
        }

        /// <summary>
        /// Gets the field East Asian language Id specified via field code formatting properties.
        /// </summary>
        internal int LanguageIdFarEast { get; private set; }

        /// <summary>
        /// Gets the field RTL language Id specified via field code formatting properties.
        /// </summary>
        internal int LanguageIdBi
        {
            get { return mLanguageIdBi; }
            set
            {
                if (mLanguageNode == null)
                    return;

                mLanguageIdBi = value;
                mLanguageNode.Font.LocaleIdBi = value;
            }
        }

        /// <summary>
        /// A boolean value indicating if RTL is specified via field code formatting properties.
        /// </summary>
        internal bool Bidi
        {
            get { return mBidi; }
            set
            {
                if (mLanguageNode == null)
                    return;

                mBidi = value;
                mLanguageNode.Font.Bidi = value;
            }
        }

        private bool IsVerbatimArgument
        {
            get { return mArguments.Count == mVerbatimArgumentIndex; }
        }

        private bool IgnoreSymbolicQuotesArgument(string switchName)
        {
            FieldType fieldType = FieldUtil.GetFieldType(FieldType);
            return switchName != null
                ? FieldUtil.IgnoreSymbolicQuotes(fieldType, switchName)
                : FieldUtil.IgnoreSymbolicQuotes(fieldType, mArguments.Count);
        }

        internal string ParseErrorMessage { get; private set; }

        internal bool HasParseError
        {
            get { return ParseErrorMessage != null; }
        }

        private void InitializeUpdate()
        {
            Debug.Assert(mUpdateSupported);

            IsolateElements();
        }

        private void FinalizeUpdate()
        {
            ParseField();

            EnsureTrailingSpaceIfNeeded();
        }

        /// <summary>
        /// Ad-hoc solution to workaround MS-Word bug: it updates hyperlink-like fields (REF and PAGEREF with \h switch)
        /// with error if field has general format switch (i.e. \* lower) in last position
        /// and there is no trailing whitespace after it.
        /// </summary>
        private void EnsureTrailingSpaceIfNeeded()
        {
            if (!IsHyperlinkLikeField())
                return;

            FieldSwitch lastSwitch = mSwitches[mSwitches.Count - 1];
            if (!lastSwitch.HasName(FieldFormat.GeneralFormatSwitch))
                return;

            if (!lastSwitch.HasArgument)
                return;

            FieldChar fieldCodeEnd = mField.FieldCodeEnd;
            if (lastSwitch.Argument.Range.End.Node != fieldCodeEnd)
                return;

            FieldCodeUpdater.AppendTrailingSpace(this);
        }

        private bool IsHyperlinkLikeField()
        {
            if (mField == null)
                return false;

            if (mSwitches.Count == 0)
                return false;

            switch (mField.Type)
            {
                case Fields.FieldType.FieldRef:
                    return HasSwitch(FieldRef.InsertHyperlinkSwitch);
                case Fields.FieldType.FieldPageRef:
                    return HasSwitch(FieldPageRef.InsertHyperlinkSwitch);
                default:
                    return false;
            }
        }

        /// <summary>
        /// Isolates elements into complete nodes.
        /// </summary>
        internal void IsolateElements()
        {
            FieldCodeIsolator isolator = new FieldCodeIsolator(this);
            isolator.IsolateElements();
        }

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly Field mField;
        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly IFieldCodeTokenInfoProvider mTokenInfoProvider;
        private readonly int mVerbatimArgumentIndex;
        private readonly bool mIgnoreSwitchesInFieldType;
        private readonly List<FieldArgument> mArguments = new List<FieldArgument>();
        private readonly List<FieldSwitch> mSwitches = new List<FieldSwitch>();
        private readonly List<object> mElements = new List<object>();

        private int mLanguageId;
        private int mLanguageIdBi;
        private bool mBidi;
        private Inline mLanguageNode;
        private readonly bool mUpdateSupported;

        private class FieldCodeIsolator
        {
            internal FieldCodeIsolator(FieldCode fieldCode)
            {
                mFieldCode = fieldCode;
            }

            internal void IsolateElements()
            {
                if (mFieldCode.Field.IsUpdating)
                {
                    foreach (FieldArgument fieldArgument in mFieldCode.Arguments)
                        fieldArgument.EnsureText();

                    foreach (FieldSwitch fieldSwitch in mFieldCode.Switches)
                        if (fieldSwitch.HasArgument)
                            fieldSwitch.Argument.EnsureText();
                }

                // Isolate elements in backward direction, so we need care about only range boundaries nodes changed, but offset remains unchanged.
                for (int elementIndex = mFieldCode.mElements.Count - 1; elementIndex >= 0; elementIndex--)
                {
                    TryIsolateSwitchArgument(elementIndex);
                    TryIsolateSwitch(elementIndex);
                    TryIsolateArgument(elementIndex);
                }
            }

            private void TryIsolateSwitchArgument(int elementIndex)
            {
                FieldSwitch fieldSwitch = mFieldCode.mElements[elementIndex] as FieldSwitch;
                if (fieldSwitch == null || !fieldSwitch.HasArgument)
                    return;

                IsolateRange(fieldSwitch.Argument.Range, elementIndex, true);
            }

            private void TryIsolateSwitch(int elementIndex)
            {
                FieldSwitch fieldSwitch = mFieldCode.mElements[elementIndex] as FieldSwitch;
                if (fieldSwitch == null)
                    return;

                IsolateRange(fieldSwitch.Range, elementIndex, false);
            }

            private void TryIsolateArgument(int elementIndex)
            {
                FieldArgument fieldArgument = mFieldCode.mElements[elementIndex] as FieldArgument;
                if (fieldArgument == null)
                    return;

                IsolateRange(fieldArgument.Range, elementIndex, false);
            }

            private void IsolateRange(NodeRange isolatedRange, int isolatedElementIndex, bool isSwitchArgument)
            {
                Affects affects = FindAffects(isolatedElementIndex, isolatedRange, isSwitchArgument);

                Node node = isolatedRange.Isolate();
                if (node == null)
                    return;

                RecoverAffects(affects, isolatedElementIndex, node);
            }

            private Affects FindAffects(int isolatedElementIndex, NodeRange isolatedRange, bool isSwitchArgument)
            {
                Affects result = new Affects(isolatedElementIndex);

                // If switch argument range is isolated, switch range may affected to.
                if (isSwitchArgument)
                {
                    FieldSwitch fieldSwitch = (FieldSwitch)mFieldCode.mElements[isolatedElementIndex];
                    result.CurrentSwitchAffect = GetRangeAffect(fieldSwitch.Range, isolatedRange);
                }

                for (int i = 0; i < isolatedElementIndex; i++)
                {
                    result.ElementsAffect[i] = new ElementAffect();

                    FieldSwitch leadingSwitch = mFieldCode.mElements[i] as FieldSwitch;
                    if (leadingSwitch != null)
                    {
                        result.ElementsAffect[i].SwitchAffect = GetRangeAffect(leadingSwitch.Range, isolatedRange);

                        if (leadingSwitch.HasArgument)
                        {
                            result.ElementsAffect[i].SwitchArgumentAffect = GetRangeAffect(leadingSwitch.Argument.Range, isolatedRange);
                        }
                    }

                    FieldArgument leadingArgument = mFieldCode.mElements[i] as FieldArgument;
                    if (leadingArgument != null)
                    {
                        result.ElementsAffect[i].ArgumentAffect = GetRangeAffect(leadingArgument.Range, isolatedRange);
                    }
                }

                return result;
            }

            private static RangeAffect GetRangeAffect(NodeRange leadingRange, NodeRange isolatedRange)
            {
                bool isStartAffected = leadingRange.Start.Node == isolatedRange.Start.Node;
                bool isEndAffected = leadingRange.End.Node == isolatedRange.Start.Node;
                return new RangeAffect(isStartAffected, isEndAffected);
            }

            private void RecoverAffects(Affects affects, int isolatedElementIndex, Node node)
            {
                if (affects.CurrentSwitchAffect.IsAffected)
                {
                    FieldSwitch fieldSwitch = (FieldSwitch)mFieldCode.mElements[isolatedElementIndex];
                    NodeRange recoveredRange = RecoverRange(fieldSwitch.Range, affects.CurrentSwitchAffect, node);
                    mFieldCode.UpdateSwitch(isolatedElementIndex, fieldSwitch.Clone(recoveredRange));
                }

                for (int j = 0; j < isolatedElementIndex; j++)
                {
                    ElementAffect elementAffect = affects.ElementsAffect[j];

                    if (elementAffect.SwitchArgumentAffect.IsAffected)
                    {
                        FieldSwitch fieldSwitch = (FieldSwitch)mFieldCode.mElements[j];
                        NodeRange recoveredRange = RecoverRange(fieldSwitch.Argument.Range, elementAffect.SwitchArgumentAffect, node);
                        FieldArgument recoveredSwitchArgument = fieldSwitch.Argument.Clone(recoveredRange);
                        mFieldCode.UpdateSwitch(j, fieldSwitch.Clone(recoveredSwitchArgument));
                    }

                    if (elementAffect.SwitchAffect.IsAffected)
                    {
                        FieldSwitch fieldSwitch = (FieldSwitch)mFieldCode.mElements[j];
                        NodeRange recoveredRange = RecoverRange(fieldSwitch.Range, elementAffect.SwitchAffect, node);
                        mFieldCode.UpdateSwitch(j, fieldSwitch.Clone(recoveredRange));
                    }

                    if (elementAffect.ArgumentAffect.IsAffected)
                    {
                        FieldArgument fieldArgument = (FieldArgument)mFieldCode.mElements[j];
                        NodeRange recoveredRange = RecoverRange(fieldArgument.Range, elementAffect.ArgumentAffect, node);
                        mFieldCode.UpdateArgument(j, fieldArgument.Clone(recoveredRange));
                    }
                }
            }

            private static NodeRange RecoverRange(NodeRange range, RangeAffect rangeAffect, Node node)
            {
                return new NodeRange(
                    rangeAffect.IsStartAffected ? new DocumentPosition(node, range.Start.Offset) : range.Start,
                    rangeAffect.IsEndAffected ? new DocumentPosition(node, range.End.Offset) : range.End);
            }

            private readonly FieldCode mFieldCode;

            private class Affects
            {
                internal Affects(int elementsCount)
                {
                    ElementsAffect = new ElementAffect[elementsCount];
                    CurrentSwitchAffect = RangeAffect.UnAffected;
                }

                internal RangeAffect CurrentSwitchAffect { get; set; }

                internal ElementAffect[] ElementsAffect { get; }
            }

            private class ElementAffect
            {
                internal ElementAffect()
                {
                    ArgumentAffect = RangeAffect.UnAffected;
                    SwitchArgumentAffect = RangeAffect.UnAffected;
                    SwitchAffect = RangeAffect.UnAffected;
                }

                internal RangeAffect SwitchAffect { get; set; }

                internal RangeAffect SwitchArgumentAffect { get; set; }

                internal RangeAffect ArgumentAffect { get; set; }
            }

            private class RangeAffect
            {
                internal RangeAffect(bool isStartAffected, bool isEndAffected)
                {
                    IsStartAffected = isStartAffected;
                    IsEndAffected = isEndAffected;
                }

                internal bool IsStartAffected { get; }

                internal bool IsEndAffected { get; }

                internal bool IsAffected
                {
                    get { return IsStartAffected || IsEndAffected; }
                }

                internal static readonly RangeAffect UnAffected = new RangeAffect(false, false);
            }
        }
    }
}
