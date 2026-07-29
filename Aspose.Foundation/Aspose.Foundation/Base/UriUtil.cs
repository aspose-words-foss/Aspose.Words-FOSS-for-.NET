// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/10/2006 by Roman Korchagin

using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Aspose.Bidi;

namespace Aspose
{
    /// <summary>
    /// RK I would have preferred using only System.Uri to deal with URIs, but unfortunately
    /// System.Uri does not support relative URIs and we can encounter relative URIs in Word documents.
    /// This is a set of utility functions that we use to deal with URIs in Word documents.
    ///
    /// There is a problem with naming. Strictly speaking, a URIs are like these:
    /// http://example/resource.txt#frag01
    /// ../../../resource.txt
    ///
    /// But MS Word can store either a URI, an absolute DOS file path or UNC file path in a single field.
    /// Therefore it requires some logic when handling. For example, we should escape/unescape URI only,
    /// never DOS or UNC path.
    ///
    /// So how should I call a value that can be URI, DOS or UNC path? In this code, I call them Href.
    /// </summary>
    public static class UriUtil
    {
        /// <summary>
        /// Returns extension of a resource file name from a Href.
        /// </summary>
        /// <param name="baseHref">Base Href of the resource. </param>
        /// <param name="resourceHref">Resource Href.</param>
        /// <returns>
        /// Extension of the resource file name, without the leading dot character.
        /// If the file name has no extension, an empty string is returned.
        /// </returns>
        /// <remarks>
        /// This method correctly works with URIs that have a query part, like 'http://example.com/file.ext?query=value'.
        /// Unlike <see cref="Path.GetExtension"/>, this method doesn't thow an exception in case the resource Href is an URI
        /// that contains characters not allowed in a Windows file path.
        /// </remarks>
        public static string GetExtension(string baseHref, string resourceHref)
        {
            string absoluteHref = ConstructAbsoluteUri(baseHref, resourceHref);
            if (!StringUtil.HasChars(absoluteHref))
            {
                return string.Empty;
            }

            // Hrefs that have a scheme are URIs, which can contain a query part. Let's skip it.
            int fileNameEndPosition = absoluteHref.Length;
            if (IsHrefWithScheme(absoluteHref))
            {
                int schemeEndPosition = absoluteHref.IndexOf(':');
                Debug.Assert(schemeEndPosition > 0);
                int queryStartPosition = absoluteHref.IndexOf('?');
                // Also protect against invalid schemes with '?' inside, like this: ht?tp://example.com
                if (queryStartPosition > schemeEndPosition)
                {
                    fileNameEndPosition = queryStartPosition;
                }
            }

            if (fileNameEndPosition <= 0)
            {
                return string.Empty;
            }

            // We know where the file name ends. Let's move backward and read its extension - the trailing part.
            for (int extensionStartPosition = fileNameEndPosition - 1; extensionStartPosition >= 0; extensionStartPosition--)
            {
                char currentChar = absoluteHref[extensionStartPosition];
                if (currentChar == '.')
                {
                    // Don't include the dot character into the result.
                    return absoluteHref.Substring(extensionStartPosition + 1, fileNameEndPosition - extensionStartPosition - 1);
                }
                if ((currentChar == '\\') || (currentChar == '/'))
                {
                    break;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Makes full uri path from base uri and given uri. Leaves escaped characters 'as is'.
        /// <seealso cref="ConstructUnescapedAbsoluteUri"/>.
        /// </summary>
        /// <remarks>
        /// For historical reasons, this method always treats the last segment of the path in the base URI as a folder rather
        /// than as a file. For example, "http://example.org/image" + "foo/bar" = "http://example.org/image/foo/bar", instead
        /// of "http://example.org/foo/bar" (the "image" segment is not replaced by the "foo" segment).
        /// </remarks>
        public static string ConstructAbsoluteUri(string baseUri, string originalUri)
        {
            // WORDSNET-22235 The previous condition did not check Linux absolute path.
            // Simple check for Linux absolute path does not work properly on Linux,
            // so added an additional check for 'fake' Linux absolute path.
            return IsAbsoluteHref(originalUri) && !IsFakeAbsoluteLocalFilePathUnix(baseUri, originalUri)
                ? originalUri
                : CombineHref(baseUri, originalUri);
        }

        /// <summary>
        /// Makes full uri path from base uri and given uri. Unescapes the result if required by the uri scheme.
        /// <seealso cref="ConstructAbsoluteUri"/>.
        /// </summary>
        public static string ConstructUnescapedAbsoluteUri(string baseUri, string originalUri)
        {
            string absoluteUri = ConstructAbsoluteUri(baseUri, originalUri);

            // WORDSNET-8682 A URI without scheme are processed as local file paths, and file system routines
            // do not accept percent-encoded characters in file paths, so characters in a URI without scheme are unescaped.
            // A URI with scheme (http:, file:, etc.) is processed as a remote resource reference (that is, as a normal URI),
            // which require percent-encoding of certain characters, so characters in a URI with scheme are left escaped.
            if (!IsHrefWithScheme(absoluteUri))
            {
                absoluteUri = UnescapeHref(absoluteUri);
            }

            return absoluteUri;
        }

        /// <summary>
        /// Gets the hyperlink part that is before the "#".
        /// </summary>
        public static string GetAddress(string href)
        {
            Debug.Assert(href != null);

            int hashPos = href.IndexOf('#');
            if (hashPos < 0)
                return href;
            else
                return href.Substring(0, hashPos);
        }

        /// <summary>
        /// Gets the hyperlink part that is after the "#" excluding the "#".
        /// </summary>
        public static string GetSubAddress(string href)
        {
            Debug.Assert(href != null);

            int hashPos = href.IndexOf('#');
            if (hashPos < 0)
                return string.Empty;
            else
                return href.Substring(hashPos + 1).TrimEnd('\'', '"');
        }

        /// <summary>
        /// If subaddress is not empty or null, returns address + "#" + subAddress, otherwise returns just address.
        /// </summary>
        public static string AppendSubAddress(string address, string subAddress)
        {
            if (address == null)
                address = string.Empty;

            if (StringUtil.HasChars(subAddress))
                address += "#" + subAddress;

            return address;
        }

        /// <summary>
        /// Returns true if the string seems to be a URI with a scheme.
        /// </summary>
        public static bool IsHrefWithScheme(string href)
        {
            // WORDSNET-17141 Path starting with "\\?\C:\" is a DOS device path.
            if (href.StartsWith(@"\\?\", StringComparison.Ordinal))
                return false;

            // Example: http://myserver/myfolder/myfile.doc returns true,
            // but X:\test\test.txt returns false
            return (href.IndexOf(':') > 1);
        }

        /// <summary>
        /// Returns true if the string seems to be a an absolute local file path or UNC path.
        /// </summary>
        public static bool IsFilePath(string href)
        {
            return IsAbsoluteLocalFilePath(href) || IsUncPath(href);
        }

        /// <summary>
        /// Returns true if the string seems to be an absolute local file path.
        /// </summary>
        public static bool IsAbsoluteLocalFilePath(string href)
        {
            return IsAbsoluteLocalFilePathWindows(href) || IsAbsoluteLocalFilePathUnix(href);
        }

        public static bool IsAbsoluteLocalFilePathWindows(string href)
        {
            // DOS/Windows: X:\MyFolder\MyFile.doc
            return href.Length > 2 && IsDriveLetter(href[0]) && href[1] == ':' && (href[2] == '\\' || href[2] == '/') ;
        }

        [JavaAttributes.JavaThrows(false)]
        public static bool IsAbsoluteLocalFilePathUnix(string href)
        {
            // Linux/MacOS/Android absolute path starts with root directory '/'. But do not count here two slashes '//'.
            return href.Length > 1 && href[0] == '/' && href[1] != '/' && PlatformUtilPal.IsUnixLike();
        }

        /// <summary>
        /// Returns true if the string seems to be a UNC path.
        /// </summary>
        public static bool IsUncPath(string href)
        {
            // Example: \\myserver\myfolder\myfile.doc
            return href.StartsWith("\\\\", StringComparison.Ordinal);
        }

        /// <summary>
        /// Returns true if original Uri starts with slash, like absolute path in Unix,
        /// but baseUri has a scheme. In this case originalUri should not be considered as absolute.
        /// </summary>
        private static bool IsFakeAbsoluteLocalFilePathUnix(string baseUri, string originalUri)
        {
            return IsAbsoluteLocalFilePathUnix(originalUri) && IsHrefWithScheme(baseUri);
        }

        /// <summary>
        /// Returns true if the string starts with "cid:".
        /// </summary>
        public static bool IsCid(string href)
        {
            return href.StartsWith("cid:", StringComparison.Ordinal);
        }

        private static bool IsDriveLetter(char c)
        {
            return ((c >= 'a') && (c <= 'z')) || ((c >= 'A') && (c <= 'Z'));
        }

        /// <summary>
        /// The base uri can be a local or remote uri.
        /// I'm not using the Uri class because if baseUri does not end with a slash,
        /// its ctor will treat the last subfolder as the file name.
        /// </summary>
        public static string CombineHref(string basePart, string relativePart)
        {
            if (!StringUtil.HasChars(basePart))
            {
                return relativePart;
            }

            // This is our best guess at using the correct path separator.
            // If there are forward slashes already, use one of them, otherwise use backward slash,
            char separator = (basePart.IndexOf('\\') >= 0) ? '\\' : '/';

            // In the resulting href, all separators should be uniform. Make sure slashes used in the relative part
            // are same as in the base part.
            char mirroredSeparator = (separator == '\\') ? '/' : '\\';

            // Canonicalize separators.
            basePart = basePart.Replace(mirroredSeparator, separator);
            relativePart = relativePart.Replace(mirroredSeparator, separator);

            // WORDSNET-17894 Resolving a relative URI is more than just appending the relative part to the base part.
            // We must also take into account cases where the relative part is a network path (starts with two slashes).
            // For details, see https://tools.ietf.org/html/rfc3986#section-4.2
            if (IsHrefWithScheme(basePart) && relativePart.StartsWith("//", StringComparison.Ordinal))
            {
                // Canonicalize separators.
                basePart = basePart.Replace('\\', '/');
                relativePart = relativePart.Replace('\\', '/');

                // Note that if we get here, the base URI must start with a scheme part.
                return GetScheme(basePart) + ":" + relativePart;
            }

            // WORDSNET-17894 Resolving a relative URI is more than just appending the relative part to the base part.
            // We must also take into account cases where the relative part is an absolute path (starts with a slash).
            // For details, see https://tools.ietf.org/html/rfc3986#section-4.2
            if (IsHrefWithScheme(basePart) && relativePart.StartsWith("/", StringComparison.Ordinal))
            {
                // Canonicalize separators.
                basePart = basePart.Replace('\\', '/');
                relativePart = relativePart.Replace('\\', '/');

                // The relative URI is an absolute path (for example, "/foo/bar/image.png").
                // Replace the path part of the base URI with the path specified in the relative URI.
                string schemeAndAuthority = GetSchemeAndAuthority(basePart);
                if (StringUtil.HasChars(schemeAndAuthority))
                {
                    return schemeAndAuthority + relativePart;
                }

                // If the base URI is malformed or doesn't have an authority part, process it as is.
                return basePart + relativePart;
            }

            bool basePartEndsWithSeparator = (basePart[basePart.Length - 1] == separator);
            bool relativePartStartsWithSeparator = relativePart.StartsWith(separator.ToString(), StringComparison.Ordinal);

            // WORDSNET-26575 Prevent extra slash char in a href.
            if (basePartEndsWithSeparator && relativePartStartsWithSeparator)
            {
                return basePart.TrimEnd(separator) + relativePart;
            }

            if (basePartEndsWithSeparator || relativePartStartsWithSeparator)
            {
                return basePart + relativePart;
            }

            return basePart + separator + relativePart;
        }

        public static string GetScheme(string uri)
        {
            Debug.Assert(IsHrefWithScheme(uri));

            int schemeEndIndex = uri.IndexOf(':');

            return uri.Substring(0, schemeEndIndex);
        }

        public static string GetSchemeAndAuthority(string uri)
        {
            Debug.Assert(IsHrefWithScheme(uri));

            // This regex matches the scheme and the authority parts of an URI.
            Match match = Regex.Match(uri, "^[^:]*:[/]*[^/]+");

            return match.Success ? match.Value : string.Empty;
        }

        /// <summary>
        /// Gets directory path from given HREF.
        /// </summary>
        public static string GetDirectoryHref(string href)
        {
            int index = href.LastIndexOfAny(gSlashes);
            return (index < 0) ? "." : href.Substring(0, Math.Max(index, 1));
        }

        /// <summary>
        /// Gets directory path from given HREF.
        /// </summary>
        /// <remarks>There were problems with TestImportMhtmlOther.TestDecodeUriWindows1251 test when
        /// <see cref="GetDirectoryHref"/> method was modified, so it's better to introduce new method and leave
        /// <see cref="GetDirectoryHref"/> method as is for now.</remarks>
        public static string GetDirectoryHrefWithScheme(string href)
        {
            Debug.Assert(IsHrefWithScheme(href) && !HasFileScheme(href));

            string schemeAndAuthority = GetSchemeAndAuthority(href);

            string hrefWithoutSchemeAndAuthority = href.Substring(schemeAndAuthority.Length);

            int indexOfLastSlash = hrefWithoutSchemeAndAuthority.LastIndexOfAny(gSlashes);

            return (indexOfLastSlash != -1)
                ? schemeAndAuthority + hrefWithoutSchemeAndAuthority.Substring(0, indexOfLastSlash + 1)
                : schemeAndAuthority;
        }

        /// <summary>
        /// Checks whether HREF represents a path with extension.
        ///
        /// RK This looks pretty nasty to me. What "path", what "href" you are talking about?
        /// </summary>
        public static bool IsPathWithExtension(string href)
        {
            int indexDot = href.LastIndexOf('.');
            int indexSlash = href.LastIndexOfAny(gSlashes);
            int indexSchemaEnd = href.IndexOf("://", StringComparison.Ordinal);

            // If all -1 then condition is also okay.
            return (indexDot > indexSlash) && ((indexSchemaEnd < 0) || (indexSchemaEnd + 2 != indexSlash));
        }

        /// <summary>
        /// Returns true if the string starts with the "#" sign.
        /// </summary>
        public static bool IsSubAddressOnly(string href)
        {
            return href.StartsWith("#", StringComparison.Ordinal);
        }

        /// <summary>
        /// Escapes the specified string if it needs escaping and is not already escaped.
        /// </summary>
        public static string EscapeHref(string href)
        {
            return IsHrefThatNeedsEscaping(href) ? EscapeHrefAnyway(href) : href;
        }

        /// <summary>
        /// Escapes the specified string.
        /// </summary>
        public static string EscapeHrefAnyway(string href)
        {
            href = href.Trim();

            StringBuilder builder = new StringBuilder();
            int index = 0;

            while (index < href.Length)
            {
                char ch = href[index];

                if (IsCharThatNeedsEscaping(ch))
                {
                    AppendEscapeChar(builder, ch);
                }
                else
                {
                    builder.Append(ch);
                }

                index++;
            }

            return builder.ToString();
        }

        /// <summary>
        /// Unescapes provided string if it needs unescaping.
        /// </summary>
        public static string UnescapeHref(string href)
        {
            if (!StringUtil.HasChars(href))
                return href;

            StringBuilder builder = new StringBuilder();

            int index = 0;

            while (index < href.Length)
            {
                char ch = href[index];

                if ((ch == '%') && (index + 2 < href.Length))
                {
                    string escapeString = href.Substring(index, 3);

                    if (IsEscapedCharString(escapeString))
                    {
                        builder.Append(UnescapeCharString(escapeString));
                        index += 3;
                        continue;
                    }
                }

                builder.Append(ch);

                index++;
            }

            return builder.ToString();
        }

        private static bool IsCharThatNeedsEscaping(char ch)
        {
            return CharsThatAreEscapedInUri.IndexOf(ch) >= 0;
        }

        /// <summary>
        /// Checks if the provided path string is escaped or does not need escaping.
        /// </summary>
        public static bool IsHrefThatNeedsEscaping(string href)
        {
            // Check if path string is empty or null.
            if (!StringUtil.HasChars(href))
                return false;

            // Local file and UNC paths do not need escaping.
            if (IsAbsoluteLocalFilePathWindows(href) || IsUncPath(href))
                return false;

            // Check if path string contains characters invalid for escaped URI.
            if (href.IndexOfAny(gCharsNotValidInEscapedUri) >= 0)
                return true;

            // Check if all '%' characters correspond to valid URI escapes.
            for (int i = 0; i < href.Length; i++)
            {
                if ((href[i] == '%') && (i + 2 < href.Length) && !IsEscapedCharString(href.Substring(i, 3)))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns true if the URI, UNC path or file path is absolute (rooted).
        ///
        /// This method is useful to call before trying to construct a .NET Uri object because
        /// the .NET Uri object will throw if the uri is relative.
        /// </summary>
        public static bool IsAbsoluteHref(string href)
        {
            return
                IsHrefWithScheme(href) ||
                IsAbsoluteLocalFilePath(href) ||
                IsUncPath(href);
        }

        /// <summary>
        /// Checks if the provided string is an escaped char.
        /// </summary>
        private static bool IsEscapedCharString(string s)
        {
            return (s.Length == 3) && (s[0] == '%') && StringUtil.IsHexDigit(s[1]) && StringUtil.IsHexDigit(s[2]);
        }

        private static char UnescapeCharString(string s)
        {
            return (char)(StringUtil.HexCharToDigit(s[1]) * 0x10 + StringUtil.HexCharToDigit(s[2]));
        }

        private static void AppendEscapeChar(StringBuilder stringBuilder, char ch)
        {
            AppendEscapeChar(stringBuilder, (byte)ch);
        }

        private static void AppendEscapeChar(StringBuilder stringBuilder, byte ch)
        {
            stringBuilder.Append('%');
            stringBuilder.Append(StringUtil.ByteToHex(ch));
        }

        /// <summary>
        /// Converts relative filename to absolute filename.
        /// Returns unchanged if given name is already absolute filename.
        /// </summary>
        public static string GetAbsoluteFileName(string masterFileName, string fileName)
        {
            // WORDSNET-10529 Loading document from the stream results to passing here null value, when reading subDoc.
            if ((masterFileName != null) && (Path.GetFileName(fileName) == fileName))
                return Path.Combine(Path.GetDirectoryName(masterFileName), Path.GetFileName(fileName));

            return fileName;
        }

        /// <summary>
        /// Creates instance of Uri class safely.
        /// We can't use Uri.IsWellFormedUriString Method for checking because of compatibility with .Net 1.1.
        /// </summary>
        /// <param name="uriString">Uri string.</param>
        /// <returns>Uri object or null if was passed invalid uri string.</returns>
        public static Uri CreateUriSafely(string uriString)
        {
            if (!StringUtil.HasChars(uriString))
                return null;

            try
            {
#if NETSTANDARD
                // For some reason Uri constructor does not like back slashes in uriString when run under Xamarin.Android.
                // Replace them with forward slashes.
                uriString = uriString.Replace('\\', '/');
#endif
                // alexnosk: Added UriKind to accept relative paths as a valid Uris,
                // this is needed to properly handle hyperlinks in XAML Flow.
                return new Uri(uriString, UriKind.RelativeOrAbsolute);
            }
            catch (UriFormatException)
            {
                return null;
            }
        }

        /// <summary>
        /// Returns true if the Uri has the file scheme.
        /// </summary>
        public static bool HasFileScheme(string uri)
        {
            return uri.StartsWith(FileSchemePrefix, StringComparison.Ordinal);
        }

        /// <summary>
        /// Removes file scheme prefix from the Uri string.
        /// </summary>
        public static string RemoveFileSchemePrefix(string uri)
        {
            Debug.Assert(HasFileScheme(uri));

            string localUri = uri.Substring(FileSchemePrefix.Length);
            // As we remove protocol name, we should check that path does not start from the single '\' character
            // and precede it with the additional '\' to make it valid in this case.
            if ((localUri.Length > 1) && (localUri[0] == '\\') && (localUri[1] != '\\'))
                localUri = '\\' + localUri;

            return localUri;
        }

        /// <summary>
        /// Adds the file scheme prefix "file:///".
        /// </summary>
        public static string AddFileSchemePrefix(string uri)
        {
            return FileSchemePrefix + uri;
        }

        /// <summary>
        /// Replaces the file scheme prefix "file:///".
        /// </summary>
        public static string ReplaceFileProtocolPrefix(string uri)
        {
            return uri.Replace(FileSchemePrefix, string.Empty);
        }

        /// <summary>
        /// Converts any href to proper URI according to RFC 3986/2396 and also escapes Unicode according to RFC 3987.
        /// </summary>
        public static string HrefToUri(string href)
        {
            if (!StringUtil.HasChars(href))
                return href;

            if (IsAbsoluteLocalFilePathWindows(href) || IsUncPath(href))
            {
                // Experiments show that for file paths MW does not resolve % escaped sequences.
                // I.e. for "C:\%41bc.docx" href MW opens "C:\%41bc.docx" file and not "C:\Abc.docx".
                // Uri class always resolves escaped sequences so explicitly escape % char before passing href into Uri.
                string path = href.Replace("%", "%25");

                // Try to handle path with Uri class. Seems that it handles paths the same way as MW.
                string pathEscaped = TryNativeEscape(path);
                if (pathEscaped != null)
                    return pathEscaped;

                // Sometimes native class cant handle paths. In this case we should manually add file prefix and escape the
                // string.
                return EscapeGeneralHrefToUri(AddFileSchemePrefix(path));
            }

            // Try to handle other hrefs with Uri class. Seems that it handles Uri the same way as MW.
            string uriEscaped = TryNativeEscape(href);
            if (uriEscaped != null)
                return uriEscaped;

            // Consider all other hrefs as relative. Uri class can't handle them so process them by own method.
            // Also here goes the Unix-like paths. Seems that Uri class can't handle them. And it seems that MW also
            // handles them the same way as other relative paths.
            return EscapeGeneralHrefToUri(href);
        }

        /// <summary>
        /// Returns index of the first occurrence of a valid URI scheme name within a specified string.
        /// The out param <paramref name="schemeName"/> is returned without trailing comma ':'.
        /// </summary>
        public static int FindUriScheme(string s, out string schemeName)
        {
            return FindUriScheme(s, 0, out schemeName);
        }

        /// <summary>
        /// Returns index of the first occurrence of a valid URI scheme name within a specified string.
        /// The out param <paramref name="schemeName"/> is returned without trailing comma ':'.
        /// </summary>
        /// <remarks>
        /// See remark at https://learn.microsoft.com/en-us/dotnet/api/system.uri.checkschemename for rules.
        /// </remarks>
        public static int FindUriScheme(string s, int index, out string schemeName)
        {
            schemeName = "";
            int i = index;
            // The scheme name must begin with a letter.
            while ((i < s.Length) && !char.IsLetter(s[i]))
                i++;

            int schemeIndex = i;

            i++;
            char c = '\0';
            while (i < s.Length)
            {
                c = s[i];
                // The scheme name must contain only letters, digits, and the characters ".", "+", or "-".
                if (!char.IsLetterOrDigit(c) && (c != '.') && (c != '+') && (c != '-'))
                    break;
                i++;
            }

            // The scheme name must end with a colon.
            if (c != ':')
            {
                // Try rest of input string.
                if (++i < s.Length)
                    return FindUriScheme(s, i, out schemeName);

                return -1;
            }

            schemeName = s.Substring(schemeIndex, i - schemeIndex);
            return schemeIndex;
        }

        /// <summary>
        /// Returns index of the first occurrence of a valid URI within a specified string.
        /// </summary>
        public static int FindUri(string s, out string uri)
        {
            return FindUri(s, 0, out uri);
        }

        /// <summary>
        /// Returns index of the first occurrence of a valid URI within a specified string.
        /// </summary>
        public static int FindUri(string s, int index, out string uri)
        {
            uri = "";

            string schemeName;
            int schemeIndex = FindUriScheme(s, index, out schemeName);
            if (schemeIndex == -1)
                return -1;

            int i = schemeIndex + schemeName.Length;
            while (i < s.Length)
            {
                char c = s[i];
                // The uri must contain only letters, digits and some other characters
                // in accordance with https://www.ietf.org/rfc/rfc3986.txt
                if (!char.IsLetterOrDigit(c) && !ArrayUtil.FindCharInArray(gValidUriChars, c))
                    break;
                i++;
            }

            uri = s.Substring(schemeIndex, i - schemeIndex);
            return schemeIndex;
        }

        /// <summary>
        /// Replaces JavaScript URIs with "javascript:void(0)".
        /// </summary>
        public static string RemoveJavaScript(string href)
        {
            return StringUtil.StartsWithOrdinalIgnoreCase(href, "javascript:")
                ? "javascript:void(0)"
                : href;
        }

        private static string TryNativeEscape(string href)
        {
            Uri uri = CreateUriSafely(href);
            if (uri == null || !uri.IsAbsoluteUri)
                return null;

            string escaped;
#if JAVA
            // JAVA-changed from uri.GetComponents(UriComponents.AbsoluteUri, UriFormat.UriEscaped):
            escaped = uri.AbsoluteUri;
#else
            try
            {
                escaped = uri.GetComponents(UriComponents.AbsoluteUri, UriFormat.UriEscaped);
            }
            catch (UriFormatException)
            {
                // WORDSNET-21631 Uri constructor throws on .NET 4.6 and does not throw on .NET 4.0 with specific
                // malformed URI string. GetComponents method throws instead on .NET 4.0.
                return null;
            }
#endif
            // For some reason Uri class does not escape Latin-1 Supplement chars (0x80-0xFF). So additionally escape
            // all remaining Unicode.
            return EscapeUnicode(escaped);
        }

        /// <summary>
        /// Own implementation of href escaping to URI (RFC 3986/2396).
        /// Native escaping works better but can't handle some cases. Also absolute file paths are escaped a bit
        /// differently than other hrefs by MW.
        /// </summary>
        private static string EscapeGeneralHrefToUri(string href)
        {
            StringBuilder builder = new StringBuilder();
            foreach (char ch in href)
            {
                // Note: For general hrefs (not absolute file path) MW seems to keep escape sequences intact even if
                // they points to files. I.e. MW opens a path ".\%41bc.docx" as "Abc.docx" and not as "%41bc.docx".
                // So do not process escape sequences to fit MW behavior.

                if (ch == '\\')
                {
                    // Backslash delimiters from Windows path should be replaced with forward slash in URI.
                    builder.Append('/');
                }
                else if (IsRegularAsciiChar(ch) && ch != ' ')
                {
                    // It seems that MW does not escape all regular ASCII chars excepting space
                    // (i.e. both reserved and unreserved URL chars).
                    builder.Append(ch);
                }
                else if (IsAsciiChar(ch))
                {
                    // Escape all remaining ASCII chars. Here goes space char and control chars.
                    // Control chars probably are not valid for href but escape them just in case.
                    AppendEscapeChar(builder, ch);
                }
                else
                {
                    // Just append all non-ASCII chars. They will be escaped later.
                    builder.Append(ch);
                }
            }

            return EscapeUnicode(builder.ToString());
        }

        private static bool IsRegularAsciiChar(char ch)
        {
            return ch >= ' ' && ch <= '~';
        }

        private static bool IsAsciiChar(char ch)
        {
            return ch < 0x80;
        }

        private static string EscapeUnicode(string href)
        {
            if (!IsUnicodeEscapeRequired(href))
                return href;

            // RFC 3986/2396 defines only ASCII chars in the URI.
            // RFC 3987 defines that remaining Unicode chars should be % escaped by they UTF8 codes.
            StringBuilder builder = new StringBuilder();
            int i = 0;
            while (i < href.Length)
            {
                char ch = href[i++];

                if (!IsAsciiChar(ch))
                {
                    string s = new string(ch, 1);
                    if (UnicodeUtil.IsSurrogatePair(href, i - 1))
                    {
                        s = href.Substring(i - 1, 2);
                        i++;
                    }

                    AppendEscapedUtf8(builder, s);
                }
                else
                {
                    builder.Append(ch);
                }
            }

            return builder.ToString();
        }

        private static bool IsUnicodeEscapeRequired(string href)
        {
            foreach (char c in href)
            {
                if (!IsAsciiChar(c))
                    return true;
            }

            return false;
        }

        private static void AppendEscapedUtf8(StringBuilder builder, string s)
        {
            byte[] utf8Data = new UTF8Encoding().GetBytes(s);
            foreach (byte b in utf8Data)
                AppendEscapeChar(builder, b);
        }

        /// <summary>
        /// The characters valid in URI in addition to Letters and Digits.
        /// </summary>
        /// <remarks>https://www.ietf.org/rfc/rfc3986.txt</remarks>
        private static readonly char[] gValidUriChars =
        {
            '-', '.', '_', '~', ':', '/', '?', '#', '[', ']', '@', '!', '$', '&', '\'', '(', ')', '*', '+', ',', ';', '%', '='
        };

        private const string CharsThatAreEscapedInUri = " <>{}|^[]`\"%#";

        private static readonly char[] gCharsNotValidInEscapedUri = CharsThatAreEscapedInUri
            .Replace("%", string.Empty)
            .ToCharArray();

        private static readonly char[] gSlashes = { '/', '\\' };

        /// <summary>
        /// This string is the prefix of file scheme URIs.
        /// </summary>
        private const string FileSchemePrefix = "file:///";
    }
}
