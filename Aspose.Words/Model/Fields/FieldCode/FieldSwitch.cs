// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/08/2009 by Dmitry Vorobyev

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Represents a switch that may appear in field code.
    /// </summary>
    internal class FieldSwitch
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="range"></param>
        /// <param name="name"></param>
        /// <param name="argument"></param>
        internal FieldSwitch(NodeRange range, string name, FieldArgument argument)
        {
            Range = range;
            Name = name;
            Argument = argument;
        }

        /// <summary>
        /// Gets the name of the switch.
        /// </summary>
        internal string Name { get; }

        /// <summary>
        /// Gets the invariant (lower case) name of the switch.
        /// </summary>
        internal string InvariantName
        {
            get { return Name.ToLower(); }
        }

        /// <summary>
        /// Gets a node range of the field switch (without switch argument).
        /// </summary>
        internal NodeRange Range { get; }

        /// <summary>
        /// Gets the argument of the switch.
        /// </summary>
        internal FieldArgument Argument { get; }

        // WORDSNET-5091 Word works differently for the cases when no switch argument is specified
        // and when an empty switch argument is specified.
        /// <summary>
        /// Gets a boolean value indicating whether switch argument was specified.
        /// </summary>
        internal bool HasArgument
        {
            get { return (Argument != null); }
        }

        internal bool HasName(string name)
        {
            return StringUtil.EqualsIgnoreCase(Name, name);
        }

        internal string GetArgumentNormalizedText()
        {
            return HasArgument
                ? Argument.GetNormalizedText()
                : null;
        }

        internal bool IsPicture
        {
            get
            {
                return IsPictureSwitch(Name);
            }
        }

        internal bool IsFormatting
        {
            get { return IsGeneralFormatSwitch(Name); }
        }

        internal static bool IsPictureSwitch(string switchName)
        {
            switch (switchName)
            {
                case FieldFormat.NumericFormatSwitch:
                case FieldFormat.DateTimeFormatSwitch:
                    return true;
                default:
                    return false;
            }
        }

        internal static bool IsGeneralFormatSwitch(string switchName)
        {
            return switchName == FieldFormat.GeneralFormatSwitch;
        }

        internal static bool IsFormattingSwitch(string switchName)
        {
            return IsPictureSwitch(switchName) || IsGeneralFormatSwitch(switchName);
        }

        internal FieldSwitch Clone(NodeRange range)
        {
            return new FieldSwitch(range, Name, Argument);
        }

        internal FieldSwitch Clone(FieldArgument argument)
        {
            return new FieldSwitch(Range, Name, argument);
        }
    }
}
