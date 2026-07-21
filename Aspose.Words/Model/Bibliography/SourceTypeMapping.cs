// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/07/2024 by Edward Voronov

using Aspose.Collections;

namespace Aspose.Words.Bibliography
{
    /// <summary>
    /// Serializes and deserializes <see cref="SourceType"/> values.
    /// </summary>
    internal static class SourceTypeMapping
    {
        static SourceTypeMapping()
        {
            gMap.AddEntry("ArticleInAPeriodical", (int)SourceType.ArticleInAPeriodical);
            gMap.AddEntry("Book", (int)SourceType.Book);
            gMap.AddEntry("BookSection", (int)SourceType.BookSection);
            gMap.AddEntry("JournalArticle", (int)SourceType.JournalArticle);
            gMap.AddEntry("ConferenceProceedings", (int)SourceType.ConferenceProceedings);
            gMap.AddEntry("Report", (int)SourceType.Report);
            gMap.AddEntry("SoundRecording", (int)SourceType.SoundRecording);
            gMap.AddEntry("Performance", (int)SourceType.Performance);
            gMap.AddEntry("Art", (int)SourceType.Art);
            gMap.AddEntry("DocumentFromInternetSite", (int)SourceType.DocumentFromInternetSite);
            gMap.AddEntry("InternetSite", (int)SourceType.InternetSite);
            gMap.AddEntry("Film", (int)SourceType.Film);
            gMap.AddEntry("Interview", (int)SourceType.Interview);
            gMap.AddEntry("Patent", (int)SourceType.Patent);
            gMap.AddEntry("ElectronicSource", (int)SourceType.Electronic);
            gMap.AddEntry("Case", (int)SourceType.Case);
            gMap.AddEntry("Misc", (int)SourceType.Misc);
        }

        internal static string EnumToString(SourceType sourceType)
        {
            return gMap.GetValue((int)sourceType);
        }

        internal static SourceType StringToEnum(string sourceType)
        {
            return (SourceType)gMap.GetValue(sourceType, (int)SourceType.Misc);
        }

        private static readonly StringToIntBidirectionalMap gMap = new StringToIntBidirectionalMap();
    }
}
