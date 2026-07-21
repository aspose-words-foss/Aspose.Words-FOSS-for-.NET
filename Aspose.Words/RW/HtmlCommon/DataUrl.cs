// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/06/2020 by Nikolay Sezganov

using System;
using System.Text;
using Aspose.Words.RW.Markdown;

namespace Aspose.Words.RW.HtmlCommon
{
    /// <summary>
    /// Represents a data URL.
    /// </summary>
    /// <remarks>
    /// The syntax of data URLs is as follows:
    ///   dataurl    := "data:" [ mediatype ] [ ";base64" ] "," data
    ///   mediatype  := [ type "/" subtype ] *( ";" parameter )
    ///   data       := *urlchar
    ///   parameter  := attribute "=" value
    /// For reference, see RFC 2397 (https://tools.ietf.org/html/rfc2397).
    /// </remarks>
    internal class DataUrl
    {
        private DataUrl(byte[] data, string charset, string mediaType)
        {
            Data = data;
            Charset = charset;
            MediaType = mediaType;
        }

        /// <summary>
        /// Data contained in the data URL. Never null but can be empty.
        /// </summary>
        internal byte[] Data { get; }

        /// <summary>
        /// Charset specified in the data URL. Never null but can be empty.
        /// </summary>
        internal string Charset { get; }

        /// <summary>
        /// Media type (MIME type) of the data URL. Never null but can be empty.
        /// </summary>
        internal string MediaType { get; }

        /// <summary>
        /// Gets data of the URL converted from the source charset to UTF-8.
        /// </summary>
        /// <returns>
        /// Converted data. The result is never null but can be empty.
        /// </returns>
        internal byte[] GetDataInUtf8()
        {
            byte[] result = Data;
            if (StringUtil.HasChars(Charset))
            {
                Encoding encoding = GetEncoding(Charset);
                if ((encoding != null) && !encoding.Equals(Encoding.UTF8))
                {
                    result = Encoding.Convert(encoding, Encoding.UTF8, result);
                }
            }
            return result;
        }

        /// <summary>
        /// Parses a string representing a data URL.
        /// </summary>
        /// <returns>
        /// A <see cref="DataUrl"/> instance with parsed data.
        /// If the source string is a malformed data URL, this method will return an instance with no data.
        /// If the source string is not a data URL, this method will return <c>null</c>.
        /// </returns>
        internal static DataUrl Parse(string uri)
        {
            if (uri == null)
            {
                return null;
            }

            uri = uri.Trim();

            // The scheme name is case-insensitive.
            const string dataScheme = "data:";
            if (!uri.StartsWith(dataScheme, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            int commaPosition = uri.IndexOf(',', dataScheme.Length);
            if (commaPosition < 0)
            {
                // No comma delimiter.
                return gEmptyDataUrl;
            }

            string data = uri.Substring(commaPosition + 1);
            if (data.Length == 0)
            {
                // A data URL with no data.
                return gEmptyDataUrl;
            }
            // The data part might be percent-encoded. Let's decode it.
            data = UriUtil.UnescapeHref(data);

            // Extract the metadata part. Note that it may be empty.
            string metadata = uri.Substring(dataScheme.Length, commaPosition - dataScheme.Length);

            string mediaType = "";
            string charset = "";
            bool isBase64 = false;

            string[] parameters = metadata.Split(';');

            // Parse parameters to determine the charset and whether the data is base64-encoded.
            // The first parameter is the MIME type.
            if ((parameters.Length > 0) && parameters[0].Contains("/"))
            {
                mediaType = parameters[0];
            }
            for (int i = 1; i < parameters.Length; i++)
            {

                string parameter = parameters[i];

                // Parameter names are case-insensitive. Notice that percent-encoded variants of "base64" are not recognized
                // by browsers.
                // WORDSNET-21500 The "base64" parameter may be surrounded by optional whitespace.
                if (string.Equals(parameter.Trim(), "base64", StringComparison.OrdinalIgnoreCase))
                {
                    isBase64 = true;
                    continue;
                }

                int equalsSignPosition = parameter.IndexOf('=');
                if (equalsSignPosition < 0)
                {
                    // Ignore parameters without values. We don't support any of them besides "base64", which is processed
                    // individually.
                    continue;
                }

                if ((equalsSignPosition > 0) && equalsSignPosition == (parameter.Length - 1))
                {
                    // The value of a parameter must be non-empty. Otherwise, it's a parsing error.
                    // Please notice that a parameter can consist of just a single equals character, which becomes its name.
                    return gEmptyDataUrl;
                }

                // Search for the "charset" parameter. Please notice that only the first such parameter is taken into account.
                if (charset == "")
                {
                    // Extract the parameter name. Parameter names are case-insensitive and can be percent-encoded.
                    string parameterName = parameter.Substring(0, equalsSignPosition);
                    parameterName = UriUtil.UnescapeHref(parameterName);
                    parameterName = parameterName.ToLowerInvariant();

                    if (parameterName == "charset")
                    {
                        // Extract the charset name. Charset names are case-insensitive and can be percent-encoded.
                        charset = parameter.Substring(equalsSignPosition + 1);
                        charset = UriUtil.UnescapeHref(charset);
                        charset = charset.ToLowerInvariant();
                    }
                }
            }

            return isBase64
                ? DecodeBase64Data(data, charset, mediaType)
                : DecodePlainTextData(data, charset, mediaType);
        }

        private static DataUrl DecodeBase64Data(
            string dataString,
            string charset,
            string mediaType)
        {
            // Decode Base64.
            // Please notice that we mimic behavior of modern browsers and correct some errors here. For example, we restore
            // trailing padding characters and ignore whitespace.
            byte[] dataBytes = StringUtil.ConvertFromBase64Safe(dataString);
            if (dataBytes.Length == 0)
            {
                // Base64 decode error.
                return gEmptyDataUrl;
            }

            return new DataUrl(dataBytes, charset, mediaType);
        }

        private static DataUrl DecodePlainTextData(
            string dataString,
            string charset,
            string mediaType)
        {
            // Get binary data by treating items of the source string as octets and not as characters (or code points).
            byte[] dataBytes = new byte[dataString.Length];
            for (int i = 0; i < dataString.Length; i++)
            {
                char c = dataString[i];
                if (c > byte.MaxValue)
                {
                    // Parsing error. Plain text data must consist of single-byte characters (octets) only.
                    // WORDSNET-18341 Excluded MarkdownUtil.SoftLineBreakChar to detect SVG properly in Markdown.
                    // I've added it here for speed reasons. If it will be necessary to exclude this character
                    // from here, then it should be replaced in LinkDestinationBlock.IsValidSvgDataUrl().
                    if (c != MarkdownUtil.SoftLineBreakChar)
                        return gEmptyDataUrl;

                    c = ControlChar.LineBreakChar;
                }
                dataBytes[i] = (byte)c;
            }

            return new DataUrl(dataBytes, charset, mediaType);
        }

        private static Encoding GetEncoding(string charset)
        {
            try
            {
                return Encoding.GetEncoding(charset);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        /// <summary>
        /// An instance with empty data that is returned to indicate a parsing error.
        /// </summary>
        private static readonly DataUrl gEmptyDataUrl = new DataUrl(new byte[0], string.Empty, string.Empty);
    }
}
