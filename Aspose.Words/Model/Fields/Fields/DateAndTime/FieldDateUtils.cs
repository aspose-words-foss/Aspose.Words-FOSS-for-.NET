// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/11/2025 by Edward Voronov

using System;
using System.Threading;

namespace Aspose.Words.Fields
{
    internal static class FieldDateUtils
    {
        /// <summary>
        /// Resolves calendar type for specified field.
        /// </summary>
        internal static CalendarType ResolveCalendarType<T>(T field)
            where T : Field, IFieldWithCalendar
        {
            if (field.UseLunarCalendar)
            {
                switch (field.FetchDocument().FieldOptions.FieldUpdateCultureSource)
                {
                    case FieldUpdateCultureSource.CurrentThread:
                        return GetLunarCalendarViaCurrentThread(field);
                    case FieldUpdateCultureSource.FieldCode:
                        return GetLunarCalendarViaFieldCode(field);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (field.UseSakaEraCalendar)
                return CalendarType.SakaEra;

            if (field.UseUmAlQuraCalendar)
                return CalendarType.UmAlQura;

            return CalendarType.Gregorian;
        }

        private static CalendarType GetLunarCalendarViaFieldCode(Field field)
        {
            int languageId = field.FieldCodeCache.LanguageId;
            int languageIdBi = field.FieldCodeCache.LanguageIdBi;

            if (!field.FieldCodeCache.Bidi && !LanguageOnly.Compare(languageId, LanguageOnly.English))
                return CalendarType.Gregorian;

            if (LanguageOnly.Compare(languageIdBi, LanguageOnly.Arabic))
                return CalendarType.Hijri;

            if (LanguageOnly.Compare(languageIdBi, LanguageOnly.Hebrew))
                return CalendarType.Hebrew;

            return CalendarType.Gregorian;
        }

        private static CalendarType GetLunarCalendarViaCurrentThread(Field field)
        {
            int languageId = Thread.CurrentThread.CurrentCulture.LCID;
            int languageIdBi = field.FieldCodeCache.LanguageIdBi;

            if (LanguageOnly.Compare(languageId, LanguageOnly.Arabic))
                return CalendarType.Hijri;

            if (LanguageOnly.Compare(languageId, LanguageOnly.Hebrew))
                return CalendarType.Hebrew;

            if (!LanguageOnly.Compare(languageId, LanguageOnly.English))
                return CalendarType.Gregorian;

            if (LanguageOnly.Compare(languageIdBi, LanguageOnly.Arabic))
                return CalendarType.Hijri;

            if (LanguageOnly.Compare(languageIdBi, LanguageOnly.Hebrew))
                return CalendarType.Hebrew;

            return CalendarType.Gregorian;
        }
    }
}
