// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/07/2024 by Edward Voronov

using Aspose.Collections;

namespace Aspose.Words.Bibliography
{
    /// <summary>
    /// Serializes and deserializes <see cref="ContributorType"/> values.
    /// </summary>
    internal static class ContributorTypeMapping
    {
        static ContributorTypeMapping()
        {
            gMap.AddEntry("Artist", (int)ContributorType.Artist);
            gMap.AddEntry("Author", (int)ContributorType.Author);
            gMap.AddEntry("BookAuthor", (int)ContributorType.BookAuthor);
            gMap.AddEntry("Compiler", (int)ContributorType.Compiler);
            gMap.AddEntry("Composer", (int)ContributorType.Composer);
            gMap.AddEntry("Conductor", (int)ContributorType.Conductor);
            gMap.AddEntry("Counsel", (int)ContributorType.Counsel);
            gMap.AddEntry("Director", (int)ContributorType.Director);
            gMap.AddEntry("Editor", (int)ContributorType.Editor);
            gMap.AddEntry("Interviewee", (int)ContributorType.Interviewee);
            gMap.AddEntry("Interviewer", (int)ContributorType.Interviewer);
            gMap.AddEntry("Inventor", (int)ContributorType.Inventor);
            gMap.AddEntry("Performer", (int)ContributorType.Performer);
            gMap.AddEntry("ProducerName", (int)ContributorType.Producer);
            gMap.AddEntry("Translator", (int)ContributorType.Translator);
            gMap.AddEntry("Writer", (int)ContributorType.Writer);
        }

        internal static string EnumToString(ContributorType contributorType)
        {
            return gMap.GetValue((int)contributorType);
        }

        internal static bool TryStringToEnum(string contributorType, out ContributorType result)
        {
            int intResult = gMap.TryGetValue(contributorType);
            result = (ContributorType)intResult;
            return !StringToIntBidirectionalMap.IsNullSubstitute(intResult);
        }

        private static readonly StringToIntBidirectionalMap gMap = new StringToIntBidirectionalMap();
    }
}
