// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/10/2011 by Dmitry Vorobyev

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements the RD field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// Identifies a file to include when you create a table of contents, a table of authorities, or an index
    /// with the TOC, TOA, or INDEX field
    /// </remarks>
    public class FieldRD : Field, IFieldCodeTokenInfoProvider
    {
        /// <summary>
        /// Gets or sets the name of the file to include when generating a table of contents, table of authorities, or index.
        /// </summary>
        public string FileName
        {
            get { return FieldCodeCache.GetArgumentAsString(FileNameArgumentIndex); }
            set { FieldCodeCache.SetArgument(FileNameArgumentIndex, value); }
        }

        /// <summary>
        /// Gets or sets whether the path is relative to the current document.
        /// </summary>
        public bool IsPathRelative
        {
            get { return FieldCodeCache.HasSwitch(IsPathRelativeSwitch); }
            set { FieldCodeCache.SetSwitch(IsPathRelativeSwitch, value); }
        }

        FieldSwitchType IFieldCodeTokenInfoProvider.GetSwitchType(string switchName)
        {
            switch (switchName)
            {
                case IsPathRelativeSwitch:
                    return FieldSwitchType.Flag;
                default:
                    return FieldSwitchType.Unknown;
            }
        }

        private const int FileNameArgumentIndex = 0;

        private const string IsPathRelativeSwitch = "\\f";
    }
}
