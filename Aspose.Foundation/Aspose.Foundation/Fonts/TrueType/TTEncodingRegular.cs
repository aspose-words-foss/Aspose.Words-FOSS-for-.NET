// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/02/2020 by Konstantin Kornilov

using System.Text;

namespace Aspose.Fonts.TrueType
{
    /// <summary>
    /// Regular encodings which can be handled by system class. 
    /// </summary>
    internal class TTEncodingRegular : TTEncoding
    {
        public TTEncodingRegular(Encoding encoding)
        {
            mEncoding = encoding;
        }
        
        public override string GetString(byte[] bytes)
        {
            return mEncoding.GetString(bytes);
        }
        
        private readonly Encoding mEncoding;
    }
}
