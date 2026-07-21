// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/03/2016 by Anatoly Sidorenko

/*
 * ******************************************************************************
 * Copyright (C) 2005-2014, International Business Machines Corporation and    *
 * others. All Rights Reserved.                                                *
 * ******************************************************************************
 */
// 24/03/16 port to C# by Anatoly Sidorenko

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Aspose.IO;

namespace Aspose.Charset
{
    /// <summary>
    /// <para>
    /// <c>CharsetDetector</c> provides a facility for detecting the
    /// charset or encoding of character data in an unknown format.
    /// The input data can either be from an input stream or an array of bytes.
    /// The result of the detection operation is a list of possibly matching
    /// charsets, or, for simple use, you can just ask for a Java Reader that
    /// will work over the input data.
    /// </para>
    /// <para>
    /// Character set detection is at best an imprecise operation.  The detection
    /// process will attempt to identify the charset that best matches the characteristics
    /// of the byte data, but the process is partly statistical in nature, and
    /// the results can not be guaranteed to always be correct.
    /// </para>
    /// <para>
    /// For best accuracy in charset detection, the input data should be primarily
    /// in a single language, and a minimum of a few hundred bytes worth of plain text
    /// in the language are needed.  The detection process will attempt to
    /// ignore html or xml style markup that could otherwise obscure the content.
    /// </para>
    /// <para>@stable ICU 3.4</para>
    /// </summary>
    public class CharsetDetector
    {

        //   Question: Should we have getters corresponding to the setters for input text and declared encoding?

        //   A thought: If we were to create our own type of Java Reader, we could defer
        //   figuring out an actual charset for data that starts out with too much English
        //   only ASCII until the user actually read through to something that didn't look
        //   like 7 bit English.  If  nothing else ever appeared, we would never need to
        //   actually choose the "real" charset.  All assuming that the application just
        //   wants the data, and doesn't care about a char set name.


        // ------------- Aspose ------------------------------------
        public static Encoding Detect(Stream stream)
        {
            long pos = stream.Position;
            try
            {
                stream.Position = 0;
                CharsetDetector detector = new CharsetDetector(stream);
                CharsetMatch match = detector.Detect();
                if (match == null)
                    return Encoding.UTF8;

                return GetEncoding(match);
            }
            catch (Exception ex)
            {
                // The line will be removed from production both in .Net and Java.
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                stream.Position = pos;
            }

            return null;
        }

        private static Encoding GetEncoding(CharsetMatch match)
        {
            string charsetName = match.GetName();
            Encoding encoding = EncodingUtil.GetEncoding(charsetName);

            // Some windows-125X and iso8859-Y charsets are very similar, but ICU4J reports
            // ISO-8859-* by default. It reports windows-125* only if short input text
            // contains bytes in the range 0x80 - 0x9F.
            // Lets reverse defaults for Windows platform.
            //
            // See also CharsetSingleByteRecognizer and
            // http://www.i18nqa.com/debug/table-iso8859-1-vs-windows-1252.html,
            // http://konfiguracja.c0.pl/iso02vscp1250en.html and google "windows-125X vs iso8859-Y".
            if (PlatformUtilPal.IsWindows())
            {
                if ("ISO-8859-1".Equals(charsetName, StringComparison.Ordinal))
                    encoding = Encoding.GetEncoding("Windows-1252");
                else if ("ISO-8859-2".Equals(charsetName, StringComparison.Ordinal))
                    encoding = Encoding.GetEncoding("Windows-1250");
                else if ("ISO-8859-7".Equals(charsetName, StringComparison.Ordinal) &&
                    "el".Equals(match.GetLanguage(), StringComparison.Ordinal))
                    encoding = Encoding.GetEncoding("Windows-1253");
                else if ("ISO-8859-8".Equals(charsetName, StringComparison.Ordinal) &&
                    "he".Equals(match.GetLanguage(), StringComparison.Ordinal))
                    encoding = Encoding.GetEncoding("Windows-1255");
                else if ("ISO-8859-8-I".Equals(charsetName, StringComparison.Ordinal) &&
                    "he".Equals(match.GetLanguage(), StringComparison.Ordinal))
                    encoding = Encoding.GetEncoding("Windows-1255");
                else if ("ISO-8859-9".Equals(charsetName, StringComparison.Ordinal) &&
                    "tr".Equals(match.GetLanguage(), StringComparison.Ordinal))
                    encoding = Encoding.GetEncoding("Windows-1254");
            }

            return encoding;
        }

        /// <summary>
        /// Return the charset that best matches the supplied input data.
        /// <p/>
        /// Note though, that because the detection
        /// only looks at the start of the input data,
        /// there is a possibility that the returned charset will fail to handle
        /// the full set of input data.
        /// <p/>
        /// Raise an exception if
        /// <ul>
        /// <li>no charset appears to match the data.</li>
        /// <li>no input text has been provided</li>
        /// </ul>
        /// <returns>a CharsetMatch object representing the best matching charset, or
        /// <code>null</code> if there are no matches.</returns>
        /// </summary>
        private CharsetMatch Detect()
        {
            // Iterate over all possible charsets, remember all that give a match quality > 0.
            CharsetMatch bestMatch = null;
            foreach (CharsetRecognizer charsetRecognizer in gCharsetRecognizers)
            {
                CharsetMatch curMatch = charsetRecognizer.Match(this);
                if (curMatch == null)
                    continue;

                if (curMatch.CompareTo(bestMatch) >= 0)
                    bestMatch = curMatch;
            }

            return bestMatch;
        }

        public byte[] GetRawInput()
        {
            return mRawInput;
        }

        public int GetRawLength()
        {
            return mRawInput.Length;
        }

        public byte[] GetInputBytes()
        {
            return mInputBytes;
        }

        public int GetInputLen()
        {
            return mInputBytes.Length;
        }

        public bool HasC1Bytes()
        {
            return mHasC1Bytes;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="CharsetDetector"/> class.
        /// </summary>
        private CharsetDetector(Stream inputStream)
        {
            mRawInput = StreamUtil.ReadStream(inputStream, MaxRawBufSize);

            inputStream.Position = 0;
            // Set the input text (byte) data whose charset is to be detected.
            mInputBytes = ReadStreamFiltered(inputStream, MaxInputBufSize, GetIgnorableTextRanges());

            mHasC1Bytes = HasC1Bytes(mInputBytes);
        }

        /// <summary>
        /// Returns true, if a specified byte array has C1 codes.
        /// </summary>
        /// <remarks>
        /// C1 codes are the range 80h–9Fh and the default C1 set was originally
        /// defined in ECMA-48 (harmonized later with ISO 6429).
        /// </remarks>
        private static bool HasC1Bytes(byte[] array)
        {
            foreach (byte b in array)
            {
                if (b >= 0x80)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Reads stream and filter out specified text.
        /// </summary>
        /// <param name="stream">The input stream.</param>
        /// <param name="length">The length of the returning byte array. Note, as some text may be removed
        /// from the input stream, the number of bytes read from the stream can be greater than the
        /// specified length. This also can be less than the specified length, if there is insufficient
        /// number of bytes in the input stream. </param>
        /// <param name="textRangesToRemove">Specifies text ranges that will be filtered out while reading the stream.</param>
        private static byte[] ReadStreamFiltered(Stream stream, int length, List<TextRange> textRangesToRemove)
        {
            // Initial read.
            byte[] srcBuf = StreamUtil.ReadStream(stream, length);
            if (srcBuf.Length == 0)
                return srcBuf;

            // Result array.
            byte[] dstBuf = new byte[srcBuf.Length];

            int srcIndex = 0;
            int dstIndex = 0;
            while (dstIndex < dstBuf.Length)
            {
                // Read next portion of bytes to buffer being analyzed.
                if (srcIndex >= srcBuf.Length)
                {
                    srcBuf = StreamUtil.ReadStream(stream, length);
                    if (srcBuf.Length == 0)
                        return ArrayUtil.ResizeArray(dstBuf, dstIndex);

                    srcIndex = 0;
                }

                // Find text range with minimal start position to filter out.
                TextRange rangeToRemove = null;
                foreach (TextRange range in textRangesToRemove)
                {
                    if (range.Find(srcBuf, srcIndex) && range.IsBefore(rangeToRemove))
                        rangeToRemove = range;
                }

                // Copy either whole source buffer, or only bytes before text being removed.
                int count = 0;
                int lengthToCopy = (rangeToRemove == null) ? length: rangeToRemove.StartIndex - srcIndex;
                if (lengthToCopy > 0)
                {
                    count = ArrayUtil.Copy(srcBuf, srcIndex, lengthToCopy, dstBuf, dstIndex);
                    if (count == 0)
                        return ArrayUtil.ResizeArray(dstBuf, dstIndex);
                }

                srcIndex += count;
                dstIndex += count;

                if (rangeToRemove == null)
                    continue;

                // There is a range of text to remove, but its end has not been found yet.
                while (rangeToRemove.EndIndex == -1)
                {
                    // Read next portion of bytes to find end of range to remove.
                    srcBuf = StreamUtil.ReadStream(stream, length);
                    if (srcBuf.Length == 0)
                        return ArrayUtil.ResizeArray(dstBuf, dstIndex);

                    rangeToRemove.FindEnd(srcBuf, 0);
                }

                // There is no end of range, exit.
                if (rangeToRemove.EndIndex == -1)
                    return ArrayUtil.ResizeArray(dstBuf, dstIndex);

                // Skip filtered out range of text.
                srcIndex = rangeToRemove.NextIndex;
            }

            return dstBuf;
        }

        /// <summary>
        /// Returns a collection of <see cref="TextRange"/> objects to ignore in detection buffer.
        /// </summary>
        private static List<TextRange> GetIgnorableTextRanges()
        {
            List<TextRange> ignorableTextRanges = new List<TextRange>();

            // WORDSNET-24148 Strip base64 image data from the array of detecting bytes.
            ignorableTextRanges.Add(new TextRange("\"data:image", "\"", new string[] { "base64" }));

            // WORDSNET-27980 Strip javascript and styles html tags from the array.
            ignorableTextRanges.Add(new TextRange("<script", "script>", new string[] { "type=\"text/javascript\"" }));
            ignorableTextRanges.Add(new TextRange("<style", "style>", new string[] { "type=\"text/css\"" }));

            // WORDSNET-28178 Ignore more HTML tags in detection buffer.
            // Note, that only tag names are ignored, but not the text inside them.
            // For example, in the following snippet: <td>text</td>,
            // the tags <td> and </td> will be ignored, but the 'text'
            // will be considered.
            string[] htmlTagNames = new string[]
            {
                "title", "body", "table", "tr", "td", "th", "font", "center", "b", "i", "div"
            };
            foreach (string htmlTagName in htmlTagNames)
            {
                ignorableTextRanges.Add(new TextRange(string.Format("<{0}", htmlTagName), ">"));
                ignorableTextRanges.Add(new TextRange(string.Format("</{0}", htmlTagName), ">"));
            }

            return ignorableTextRanges;
        }

        //List of recognizers for all charsets known to the implementation.
        private static readonly List<CharsetRecognizer> gCharsetRecognizers;

        static CharsetDetector()
        {
            gCharsetRecognizers = new List<CharsetRecognizer>();

            gCharsetRecognizers.Add(new CharsetUTF7Recognizer());
            gCharsetRecognizers.Add(new CharsetUTF8Recognizer());
            gCharsetRecognizers.Add(new Charset_UTF_16_BERecognizer());
            gCharsetRecognizers.Add(new Charset_UTF_16_LERecognizer());
            gCharsetRecognizers.Add(new Charset_UTF_32_BERecognizer());
            gCharsetRecognizers.Add(new Charset_UTF_32_LERecognizer());

            gCharsetRecognizers.Add(new CharsetISO_2022RecognizerJP());
            gCharsetRecognizers.Add(new CharsetISO_2022RecognizerCN());
            gCharsetRecognizers.Add(new CharsetISO_2022RecognizerKR());
            gCharsetRecognizers.Add(new CharsetRecog_gb_18030());
            gCharsetRecognizers.Add(new CharsetRecog_euc_jp());
            gCharsetRecognizers.Add(new CharsetRecog_euc_kr());
            gCharsetRecognizers.Add(new CharsetRecog_sjis());
            gCharsetRecognizers.Add(new CharsetRecog_big5());

            gCharsetRecognizers.Add(new CharsetRecog_8859_1());
            gCharsetRecognizers.Add(new CharsetRecog_8859_2());
            gCharsetRecognizers.Add(new CharsetRecog_8859_5_ru());
            gCharsetRecognizers.Add(new CharsetRecog_8859_6_ar());
            gCharsetRecognizers.Add(new CharsetRecog_8859_7_el());
            gCharsetRecognizers.Add(new CharsetRecog_8859_8_I_he());
            gCharsetRecognizers.Add(new CharsetRecog_8859_8_he());
            gCharsetRecognizers.Add(new CharsetRecog_windows_1251());
            gCharsetRecognizers.Add(new CharsetRecog_windows_1256());
            gCharsetRecognizers.Add(new CharsetRecog_KOI8_R());
            gCharsetRecognizers.Add(new CharsetRecog_8859_9_tr());
        }

        /// <summary>
        /// The max size of array with specially prepared input bytes.
        /// </summary>
        /// <remarks>
        /// WORDSNET-18929 The read limit is increased from 512 bytes to 3kb as there is ‘ at the offset 2891,
        /// which should be taken into account to properly detect the encoding.
        /// </remarks>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int MaxInputBufSize = 1024 * 3;

        /// <summary>
        /// The text to be checked. Markup will be removed if appropriate.
        /// </summary>
        private readonly byte[] mInputBytes;

        /// <summary>
        /// The max size of array with raw input bytes.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int MaxRawBufSize = 1024 * 3;

        /// <summary>
        /// Original, untouched input bytes.
        /// </summary>
        private readonly byte[] mRawInput;

        /// <summary>
        /// The max size of array with raw input bytes.
        /// </summary>
        /// <remarks>
        /// True if any bytes in the range 0x80 - 0x9F are in the input.
        /// </remarks>
        private readonly bool mHasC1Bytes;
    }
}
