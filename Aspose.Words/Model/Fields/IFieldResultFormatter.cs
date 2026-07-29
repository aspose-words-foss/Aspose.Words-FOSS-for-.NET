// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/03/2017 by Edward Voronov

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;


namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implement this interface if you want to control how the field result is formatted.
    /// </summary>
    [ComVisible(false)]
    public interface IFieldResultFormatter
    {
        /// <summary>
        /// Called when Aspose.Words applies a numeric format switch, i.e. \# "#.##".
        /// </summary>
        /// <remarks>
        /// The implementation should return <c>null</c> to indicate that the default formatting should be applied.
        /// </remarks>
        string FormatNumeric(double value, string format);

        /// <summary>
        /// Called when Aspose.Words applies a date/time format switch, i.e. \@ "dd.MM.yyyy".
        /// </summary>
        /// <remarks>
        /// The implementation should return <c>null</c> to indicate that the default formatting should be applied.
        /// </remarks>
#if JAVA
        string formatDateTime(java.util.Date value, String format, /*CalendarType*/int calendarType);
#else
        string FormatDateTime(DateTime value, string format, CalendarType calendarType);
#endif

        /// <summary>
        /// Called when Aspose.Words applies a capitalization format switch, i.e. \* Upper.
        /// </summary>
        /// <remarks>
        /// The implementation should return <c>null</c> to indicate that the default formatting should be applied.
        /// </remarks>
        [SuppressMessage("Microsoft.Naming", "CA1719:ParameterNamesShouldNotMatchMemberNames", MessageId = "1#",
            Justification = "Public API, as designed.")]
        string Format(string value, GeneralFormat format);

        /// <summary>
        /// Called when Aspose.Words applies a number format switch, i.e. \* Ordinal.
        /// </summary>
        /// <remarks>
        /// The implementation should return <c>null</c> to indicate that the default formatting should be applied.
        /// </remarks>
        [SuppressMessage("Microsoft.Naming", "CA1719:ParameterNamesShouldNotMatchMemberNames", MessageId = "1#",
            Justification = "Public API, as designed.")]
        string Format(double value, GeneralFormat format);
    }
}
