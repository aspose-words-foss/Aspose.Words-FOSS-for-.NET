// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Aspose.JavaAttributes;

namespace Aspose.TestFx
{
    /// <summary>
    /// Implements conversion of web formats (SVG, HTML, etc. anything that Chrome can open) to Png files.
    /// </summary>
    [AndroidDelete]
    internal static class WebToPngClient
    {
        /// <summary>
        /// Default path to the server executable.
        /// You can override this using <see cref="ENVAR"/> environment variable.
        /// </summary>
        internal const string SERVER = "x:/awnet/aspose.foundation/tools/imageservices/bin/aspose.webtopng.exe";
        internal static readonly string PIPE = System.IO.Path.GetFileNameWithoutExtension(SERVER) + ".pipe";
        internal static readonly string MUTEX = System.IO.Path.GetFileNameWithoutExtension(SERVER) + ".mutex";
        internal static readonly string ENVAR = System.IO.Path.GetFileNameWithoutExtension(SERVER) + ".path";

        /// <summary>
        /// Converts <paramref name="sourceFiles"/> into Png images in <paramref name="outputFolder"/>
        /// </summary>
        [JavaThrows(true)]
        internal static void Convert(string outputFolder, string[] sourceFiles)
        {
            if (sourceFiles.Length <= 0)
                throw new System.ArgumentException("sourceFiles");

            List<string> p = new List<string>(sourceFiles);
            p.Insert(0, "-o"+ outputFolder);
            ClientUtil.PostServerJobGetResults(MUTEX, PIPE, SERVER, ENVAR, p.ToArray());
        }

        /// <summary>
        /// Returns file name which would be used for saving image of the uri.
        ///
        /// NOTE This method can generate same file name for different uri where only punctuation is different.
        /// NOTE This method does not verify that Uri is valid and absolute, call <see cref="IsValidAbsoluteUri(string)"/> beforehand.
        /// </summary>
        /// <param name="uri">A valid absolute uri of the resource.</param>
        /// <returns>A file name</returns>
        internal static string GetOutputFileName(string uri)
        {
            // source was validated when parameters were parsed
            Match m = _UriScheme.Match(uri);
            if (m.Success)
                uri = uri.Substring(m.Length);

            uri = UriUtil.UnescapeHref(uri);

            const string VALID_CHARS = "!\"#$%'()+,-.;=@_`{}~";
            StringBuilder sb = new StringBuilder(uri);
            foreach (char c in uri)
            {
                if (char.IsLetterOrDigit(c) || VALID_CHARS.IndexOf(c) >= 0)
                    sb.Append(c);
                else
                    sb.Append('_');
            }
            sb.Append(".PNG");
            return sb.ToString();
        }

        private static readonly Regex _UriScheme = new Regex("^[a-z]+://", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
    }
}
