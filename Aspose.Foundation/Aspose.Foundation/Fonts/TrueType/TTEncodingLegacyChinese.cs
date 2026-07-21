// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/02/2020 by Konstantin Kornilov

using System.Text;

namespace Aspose.Fonts.TrueType
{
    /// <summary>
    /// Legacy Chinese encodings specific for TrueType fonts.
    /// </summary>
    internal class TTEncodingLegacyChinese : TTEncoding
    {
        public TTEncodingLegacyChinese(Encoding encoding)
        {
            mEncoding = encoding;
        }

        public override string GetString(byte[] bytes)
        {
            // Generally PRC encoding seems to be handled well by system GB2312 encoding.
            // But in case of WORDSNET-20026 there are additional zero byte for each encoded byte for some reason.
            byte[] actualBytes = bytes;
            if (CheckExtraBytes(bytes))
                actualBytes = RemoveExtraBytes(bytes);

            // fix Java problem with empty string
            if (mEncoding.GetString(actualBytes).Length == 0)
                return Encoding.GetEncoding("gb2312").GetString(actualBytes);
            else
                return mEncoding.GetString(actualBytes);
        }

        private static bool CheckExtraBytes(byte[] bytes)
        {
            if (bytes.Length % 2 == 1)
                return false;

            for (int i = 0; i < bytes.Length; i++)
                if (i % 2 == 0 && bytes[i] != 0)
                    return false;

            return true;
        }

        private static byte[] RemoveExtraBytes(byte[] bytes)
        {
            byte[] result = new byte[bytes.Length / 2];
            for (int i = 0; i < result.Length; i++)
                result[i] = bytes[i * 2 + 1];

            return result;
        }

        private readonly Encoding mEncoding;
    }
}
