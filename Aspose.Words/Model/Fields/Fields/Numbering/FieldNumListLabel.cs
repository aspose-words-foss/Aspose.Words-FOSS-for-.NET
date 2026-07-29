// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/01/2013 by Ivan Lyagin

using Aspose.Words.Lists;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Contains data used to build fake results for LISTNUM, AUTONUM, AUTONUMOUT and AUTONUMLGL fields.
    /// </summary>
    internal class FieldNumListLabel
    {
        internal FieldNumListLabel(string text, RunPr runPrOverrides)
        {
            Text = text;
            RunPrOverrides = runPrOverrides;
        }

        internal FieldNumListLabel(string text, RunPr runPrOverrides, ListNumberState state)
            : this (text, runPrOverrides)
        {
            State = state.Snapshot();
        }

        /// <summary>
        /// A fake result text.
        /// </summary>
        internal string Text { get; }

        /// <summary>
        /// Gets a value indicating whether the instance contains a fake result text.
        /// </summary>
        internal bool HasText
        {
            get { return StringUtil.HasChars(Text); }
        }

        /// <summary>
        /// Font properties' overrides to be applied to a fake result.
        /// </summary>
        internal RunPr RunPrOverrides { get; }

        /// <summary>
        /// Gets a value indicating whether the instance contains font properties' overrides.
        /// </summary>
        internal bool HasRunPrOverrides
        {
            get { return (RunPrOverrides != null); }
        }

        /// <summary>
        /// Gets an <see cref="ListNumberState"/> object associated with this instance.
        /// </summary>
        internal ListNumberState State { get; }
    }
}
