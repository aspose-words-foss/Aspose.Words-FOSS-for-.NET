// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/10/2011 by Dmitry Vorobyev

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements the ADVANCE field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// Moves the starting point at which the text that lexically follows the field is displayed to the right or left,
    /// up or down, or to a specific horizontal or vertical position.
    /// </remarks>
    public class FieldAdvance : Field, IFieldCodeTokenInfoProvider
    {
        /// <summary>
        /// Gets or sets the number of points by which the text that follows the field should be moved down.
        /// </summary>
        public string DownOffset //double
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(DownOffsetSwitch); }
            set { FieldCodeCache.SetSwitchAsDouble(DownOffsetSwitch, value); }
        }

        /// <summary>
        /// Gets or sets the number of points by which the text that follows the field should be moved left.
        /// </summary>
        public string LeftOffset //double
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(LeftOffsetSwitch); }
            set { FieldCodeCache.SetSwitchAsDouble(LeftOffsetSwitch, value); }
        }

        /// <summary>
        /// Gets or sets the number of points by which the text that follows the field should be moved right.
        /// </summary>
        public string RightOffset //double
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(RightOffsetSwitch); }
            set { FieldCodeCache.SetSwitchAsDouble(RightOffsetSwitch, value); }
        }

        /// <summary>
        /// Gets or sets the number of points by which the text that follows the field should be moved up.
        /// </summary>
        public string UpOffset //double
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(UpOffsetSwitch); }
            set { FieldCodeCache.SetSwitchAsDouble(UpOffsetSwitch, value); }
        }

        /// <summary>
        /// Gets or sets the number of points by which the text that follows the field should be moved horizontally
        /// from the left edge of the column, frame, or text box.
        /// </summary>
        public string HorizontalPosition //double
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(HorizontalPositionSwitch); }
            set { FieldCodeCache.SetSwitchAsDouble(HorizontalPositionSwitch, value); }
        }

        /// <summary>
        /// Gets or sets the number of points by which the text that follows the field should be moved vertically
        /// from the top edge of the page.
        /// </summary>
        public string VerticalPosition //double
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(VerticalPositionSwitch); }
            set { FieldCodeCache.SetSwitchAsDouble(VerticalPositionSwitch, value); }
        }

        FieldSwitchType IFieldCodeTokenInfoProvider.GetSwitchType(string switchName)
        {
            switch (switchName)
            {
                case DownOffsetSwitch:
                case LeftOffsetSwitch:
                case RightOffsetSwitch:
                case UpOffsetSwitch:
                case HorizontalPositionSwitch:
                case VerticalPositionSwitch:
                {
                    return FieldSwitchType.HasArgument;
                }
                default:
                {
                    return FieldSwitchType.Unknown;
                }
            }
        }

        private const string DownOffsetSwitch = "\\d";
        private const string LeftOffsetSwitch = "\\l";
        private const string RightOffsetSwitch = "\\r";
        private const string UpOffsetSwitch = "\\u";
        private const string HorizontalPositionSwitch = "\\x";
        private const string VerticalPositionSwitch = "\\y";
    }
}
