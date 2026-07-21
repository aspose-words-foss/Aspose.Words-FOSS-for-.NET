// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/11/2025 by Edward Voronov

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Represents a field that supports different calendars.
    /// </summary>
    internal interface IFieldWithCalendar
    {
        /// <summary>
        /// Gets whether to use the Hijri Lunar or Hebrew Lunar calendar.
        /// </summary>
        bool UseLunarCalendar { get; }

        /// <summary>
        /// Gets whether to use the Saka Era calendar.
        /// </summary>
        bool UseSakaEraCalendar { get; }

        /// <summary>
        /// Gets whether to use the Um-al-Qura calendar.
        /// </summary>
        bool UseUmAlQuraCalendar { get; }
    }
}
