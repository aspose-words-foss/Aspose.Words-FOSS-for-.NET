// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/06/2023 by Victor Chebotok

using System.IO;
using System.Text;
using Aspose.Common;

namespace Aspose.Words.RW.Html.Reader
{
    /// <summary>
    /// Detects encoding of HTML documents.
    /// </summary>
    internal static class HtmlEncodingDetector
    {
        internal static Encoding Detect(
            Encoding encoding,
            Stream htmlStream)
        {
            if (encoding == null)
            {
                // WORDSNET-19885 FileFormatDetector has been replaced with HtmlFormatDetector to prevent
                // a wrong encoding result. We're now sure that the document is in the HTML format but FileFormatDetector
                // can detect it as something else and return a wrong encoding.
                FileFormatInfo fileFormatInfo;
                CustomTextReader textReader = new CustomTextReader(htmlStream, null);
                try
                {
                    fileFormatInfo = HtmlFormatDetector.Detect(textReader);
                }
                finally
                {
                    textReader.RewindStream();
                }
                // WORDSNET-22633 Here we're not interested whether the format detector is sure about the format
                // of the document. We only want it to detect encoding. That's why we don't verify the returned load format.
                // For some documents the format detector may be unsure if the format of a document is HTML but still be able
                // to correctly detect encoding of that document.
                encoding = fileFormatInfo.Encoding;
            }

            // Perform encoding replacements to work around a common error where an ISO encoding is declared,
            // but a windows encoding is used in an HTML document. This replacement is required by the HTML 5 specification.
            encoding = ReplaceWithWindowsEncoding(encoding);

            return encoding;
        }

        /// <summary>
        /// Performs document encoding replacement as required by the HTML 5 specification.
        /// </summary>
        /// <remarks>
        /// The replacement helps to work around a common error where an ISO encoding is declared for an HTML document
        /// that in fact uses a windows encoding. ISO and windows encodings are almost equal, except for characters in range
        /// 0x80 - 0x9F, which are control characters in ISO encodings, but map to visible characters in windows encodings.
        /// The HTML 5 specification also requires some other encoding replacements, but at the moment we perform only those
        /// requested by customers.
        /// For details, see http://www.w3.org/TR/html5/syntax.html#character-encodings-0
        /// </remarks>
        private static Encoding ReplaceWithWindowsEncoding(Encoding encoding)
        {
            switch (encoding.WebName)
            {
                case "us-ascii":
                case "iso-8859-1":
                    return Encoding.GetEncoding("windows-1252");
                case "iso-8859-9":
                    return Encoding.GetEncoding("windows-1254");
#if JAVA
                // WORDSJAVA-2606 - Java does not support circled characters (①, ②, etc.) in iso-2022-jp encoding.
                case "iso-2022-jp":
                    return tryToReplaceEncoding(encoding, "x-windows-iso2022jp");
#endif
                default:
                    return encoding;
            }
        }

        /// <summary>
        /// Currently, this method is only used in Java.
        /// </summary>
        private static Encoding TryToReplaceEncoding(Encoding inputEncoding, string desiredEncodingName)
        {
            Encoding desiredEncoding;
            try
            {
                desiredEncoding = Encoding.GetEncoding(desiredEncodingName);
            }
            catch (System.Exception)
            {
                return inputEncoding;
            }
            return desiredEncoding == Encoding.UTF8 ? inputEncoding : desiredEncoding;
        }
    }
}
