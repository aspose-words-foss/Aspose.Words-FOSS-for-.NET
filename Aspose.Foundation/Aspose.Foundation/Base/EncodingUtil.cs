// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/09/2017 by Andrey Noskov

using System.Text;

namespace Aspose
{
    public static class EncodingUtil
    {
        public static void RegisterEncodings()
        {
#if NETSTANDARD
            try
            {
                Encoding.GetEncoding(1252);
            }
            catch (System.Exception)
            {
                // Register encodings supported in the desktop .NET Framework but not in .NET Standard.
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            }
#else
            // Nothing to do.
#endif
        }

        /// <summary>Returns the encoding associated with the specified code page identifier.</summary>
        public static Encoding GetEncoding(int codepage)
        {
#if NETSTANDARD
            if (codepage == gUtf7CodePage)
#pragma warning disable SYSLIB0001 // Type or member is obsolete
                return Encoding.UTF7;
#pragma warning restore SYSLIB0001 // Type or member is obsolete
#endif

            return Encoding.GetEncoding(codepage);
        }

        /// <summary>Returns the encoding associated with the specified code page name.</summary>
        public static Encoding GetEncoding(string name)
        {
#if NETSTANDARD
            if (name == gUtf7EncodingName)
#pragma warning disable SYSLIB0001 // Type or member is obsolete
                return Encoding.UTF7;
#pragma warning restore SYSLIB0001 // Type or member is obsolete
#endif

            return Encoding.GetEncoding(name);
        }

#if NETSTANDARD
        private static readonly int gUtf7CodePage = 65000;
        private static readonly string gUtf7EncodingName = "UTF-7";
#endif
    }
}
