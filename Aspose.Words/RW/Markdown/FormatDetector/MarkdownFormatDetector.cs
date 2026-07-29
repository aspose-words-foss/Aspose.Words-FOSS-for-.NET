// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/06/2019 by Ilya Navrotskiy

using System.IO;
using System.Text;
using Aspose.Common;
using Aspose.Words.RW.Factories;

namespace Aspose.Words.RW.Markdown.FormatDetector
{
    /// <summary>
    /// The class to detect markdown file format.
    /// </summary>
    /// <remarks>
    /// It helps <see cref="FileFormatDetector"/> to confirm or reject an assumption that a provided stream is Markdown.
    /// </remarks>
    internal class MarkdownFormatDetector
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        private MarkdownFormatDetector(CustomTextReader textReader)
        {
            Debug.Assert(textReader != null);
            mContext = new MarkdownDetectorContext(textReader);
        }

        /// <summary>
        /// Returns <see cref="FileFormatInfo"/> if a specified reader represents Markdown file, or <c>null</c> otherwise.
        /// </summary>
        internal static FileFormatInfo Detect(CustomTextReader textReader)
        {
            FileStream fileStream = textReader.Stream as FileStream;
            if ((fileStream != null) && StringUtil.HasChars(fileStream.Name))
            {
                string extension = Path.GetExtension(fileStream.Name);
                if (FileFormatCore.FromExt(extension) == FileFormat.Markdown)
                    return GetMarkdownFileFormat(textReader.Encoding);
            }

            return new MarkdownFormatDetector(textReader).Detect();
        }

        /// <summary>
        /// Returns <see cref="FileFormatInfo"/> object if Markdown format is detected, or <c>null</c> otherwise.
        /// </summary>
        /// <remarks>
        /// Should be invoked only if a file extension was not found in <see cref="FileFormatCore"/>.
        /// </remarks>
        private FileFormatInfo Detect()
        {
            int totalWeight = 0;
            int totalCount = 0;

            while (mContext.ReadLine())
            {
                // The Markdown format is a text file, so let's limit a number of allowed non-printable characters in it.
                if (((double)mContext.NonPrintableCharsCount / mContext.Length) > NonPrintableCharsRatio)
                    return null;

                 // WORDSNET-28614 Consider lengths of sentences.
                if (((double)mContext.LongSentencesTotalLength / mContext.Length) > LongSentencesRatio)
                    return null;

                foreach (MarkdownDetectorBase detector in mMarkdownDetectors)
                {
                    if (detector.DetectAndUpdateContext(mContext))
                    {
                        totalCount += detector.Count;
                        totalWeight += detector.Weight;

                        if ((totalWeight >= TotalFeaturesWeight) && (totalCount >= TotalFeaturesCount))
                            return GetMarkdownFileFormat(mContext.Encoding);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Returns <see cref="FileFormatInfo"/> object valid for markdown file format.
        /// </summary>
        private static FileFormatInfo GetMarkdownFileFormat(Encoding encoding)
        {
            FileFormatInfo fileFormatInfo = new FileFormatInfo();
            fileFormatInfo.SetLoadFormat(LoadFormat.Markdown);
            fileFormatInfo.SetEncoding(encoding);

            return fileFormatInfo;
        }

        /// <summary>
        /// A context that contains a various data to detect markdown features.
        /// </summary>
        private readonly MarkdownDetectorContext mContext;

        /// <summary>
        /// An array of detectors to try recognize markdown features.
        /// </summary>
        private readonly MarkdownDetectorBase[] mMarkdownDetectors =
        {
            // The order of the detectors is important.
            new QuoteDetector(),
            new HeadingDetector(),
            new HorizontalRuleDetector(),
            new ListDetector(),
            new FencedCodeDetector(),
            new LinkDetector(),
            new EmphasisDetector()
        };

        /// <summary>
        /// A total allowed ratio of non-printable characters in a file.
        /// </summary>
        /// <remarks>It is chosen on the basis of common sense. Feel free to adjust it if needed.</remarks>
        private const double NonPrintableCharsRatio = 0.01;

        /// <summary>
        /// A total allowed ratio of too long sentences in a file.
        /// </summary>
        /// <remarks>It is chosen on the basis of common sense. Feel free to adjust it if needed.</remarks>
        private const double LongSentencesRatio = 0.9;

        /// <summary>
        /// A total weight of the detected features that allows to treat them as the markdown format.
        /// </summary>
        /// <remarks>It is chosen on the basis of common sense. Feel free to adjust it if needed.</remarks>
        private const int TotalFeaturesWeight = 12;

        /// <summary>
        /// A total number of the detected features that allows to treat them as the markdown format.
        /// </summary>
        /// <remarks>It is chosen on the basis of common sense. Feel free to adjust it if needed.</remarks>
        private const int TotalFeaturesCount = 2;
    }
}
