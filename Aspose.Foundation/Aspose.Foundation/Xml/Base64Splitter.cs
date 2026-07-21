// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/10/2009 by Roman Korchagin

using System.Text;

namespace Aspose.Xml
{
    /// <summary>
    /// We need this class because we sometimes need to write different line breaks
    /// to match what MS Word is writing. Yet .NET only allows one type of line breaks in the Convert class.
    /// </summary>
    public class Base64Splitter
    {
        public Base64Splitter(byte[] data, int index, int count)
        {
            mBase64Data = System.Convert.ToBase64String(data, index, count);
        }

        public string GetNext()
        {
            int length = System.Math.Min(LineLength, mBase64Data.Length - mCurPos);
            string result = mBase64Data.Substring(mCurPos, length);
            mCurPos += length;
            return result;
        }

        public bool IsEof
        {
            get { return mCurPos >= mBase64Data.Length; }
        }

        public static string Convert(byte[] data, string separator)
        {
            Base64Splitter splitter = new Base64Splitter(data, 0, data.Length);
            StringBuilder builder = new StringBuilder();
            while (true)
            {
                builder.Append(splitter.GetNext());
                if (splitter.IsEof)
                    break;

                builder.Append(separator);
            }
            // AM. I found that separator must be written at end otherwise Word fails to read Ink data.
            builder.Append(separator);
            
            return builder.ToString();
        }
            
        private readonly string mBase64Data;
        private int mCurPos;

        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int LineLength = 76;
    }

}
