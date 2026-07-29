// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.

using System.Collections.Generic;
using Aspose.JavaAttributes;

namespace Aspose.TestFx
{
    /// <summary>
    /// Implements conversion of Xps format to Png files.
    /// </summary>
    [AndroidDelete]
    internal static class XpsToPngClient
    {
        /// <summary>
        /// Default path to the server executable.
        /// You can override this using <see cref="ENVAR"/> environment variable.
        /// </summary>
        internal const string SERVER = "x:/awnet/aspose.foundation/tools/imageservices/bin/aspose.xpstopng.exe";
        internal static readonly string PIPE = System.IO.Path.GetFileNameWithoutExtension(SERVER) + ".pipe";
        internal static readonly string MUTEX = System.IO.Path.GetFileNameWithoutExtension(SERVER) + ".mutex";
        internal static readonly string ENVAR = System.IO.Path.GetFileNameWithoutExtension(SERVER) + ".path";

        public
#if JAVA
            static
#endif
            struct ConvertSourceParam
        {
            public string InputFile;
            public string Pages;
        }

        /// <summary>
        /// Converts Xps documents to Png images.
        /// </summary>
        [JavaThrows(true)]
        public static void Convert(string outputFolder, ConvertSourceParam[] args)
        {
            if (args.Length <= 0)
                throw new System.ArgumentException("args");

            List<string> p = new List<string>();
            p.Add("-o" + outputFolder);
            foreach (ConvertSourceParam a in args)
            {
                p.Add(a.InputFile);
                if (!string.IsNullOrWhiteSpace(a.Pages))
                    p.Add("-p" + a.Pages);
            }

            ClientUtil.PostServerJobGetResults(MUTEX, PIPE, SERVER, ENVAR, p.ToArray());
        }
    }
}
