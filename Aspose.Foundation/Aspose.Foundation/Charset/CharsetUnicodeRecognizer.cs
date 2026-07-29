// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/03/2016 by Anatoly Sidorenko

/**
 * ******************************************************************************
 * Copyright (C) 2005-2014, International Business Machines Corporation and    *
 * others. All Rights Reserved.                                                *
 * ******************************************************************************
 */
// 30/03/16 port to C# by Anatoly Sidorenko

using System;

namespace Aspose.Charset
{
    /// <summary>
    /// This class matches UTF-16 and UTF-32, both big- and little-endian. The
    /// BOM will be used if it is present.
    /// </summary>
    public abstract class CharsetUnicodeRecognizer : CharsetRecognizer
    {
        internal static int CodeUnit16FromBytes(byte hi, byte lo)
        {
            return ((hi & 0xff) << 8) | (lo & 0xff);
        }

        // UTF-16 confidence calculation. Very simple minded, but better than nothing.
        // Any 8 bit non-control characters bump the confidence up. These have a zero high byte,
        // and are very likely to be UTF-16, although they could also be part of a UTF-32 code.
        // NULs are a contra-indication, they will appear commonly if the actual encoding is UTF-32.
        // NULs should be rare in actual text.
        internal static int AdjustConfidence(int codeUnit, int confidence)
        {
            if (codeUnit == 0)
                confidence -= 10;
            else if ((codeUnit >= 0x20 && codeUnit <= 0xff) || codeUnit == 0x0a)
                confidence += 10;
            if (confidence < 0)
                confidence = 0;
            else if (confidence > 100)
                confidence = 100;

            return confidence;
        }
    }


    internal class Charset_UTF_16_BERecognizer : CharsetUnicodeRecognizer
    {
        public override string GetName()
        {
            return "UTF-16BE";
        }

        public override CharsetMatch Match(CharsetDetector det)
        {
            byte[] input = det.GetRawInput();
            int confidence = 10;

            int bytesToCheck = Math.Min(input.Length, 30);
            for (int charIndex = 0; charIndex < bytesToCheck - 1; charIndex += 2)
            {
                int codeUnit = CodeUnit16FromBytes(input[charIndex], input[charIndex + 1]);
                if (charIndex == 0 && codeUnit == 0xFEFF)
                {
                    confidence = 100;
                    break;
                }
                confidence = AdjustConfidence(codeUnit, confidence);
                if (confidence == 0 || confidence == 100)
                    break;

            }
            if (bytesToCheck < 4 && confidence < 100)
                confidence = 0;

            if (confidence > 0)
                return new CharsetMatch(this, confidence);

            return null;
        }
    }

    internal class Charset_UTF_16_LERecognizer : CharsetUnicodeRecognizer
    {
        public override string GetName()
        {
            return "UTF-16LE";
        }

        public override CharsetMatch Match(CharsetDetector det)
        {
            byte[] input = det.GetRawInput();
            int confidence = 10;

            int bytesToCheck = Math.Min(input.Length, 30);
            for (int charIndex = 0; charIndex < bytesToCheck - 1; charIndex += 2)
            {
                int codeUnit = CodeUnit16FromBytes(input[charIndex + 1], input[charIndex]);
                if (charIndex == 0 && codeUnit == 0xFEFF)
                {
                    confidence = 100;
                    break;
                }

                confidence = AdjustConfidence(codeUnit, confidence);
                if (confidence == 0 || confidence == 100)
                    break;
            }

            if (bytesToCheck < 4 && confidence < 100)
                confidence = 0;

            if (confidence > 0)
                return new CharsetMatch(this, confidence);

            return null;
        }
    }

    internal abstract class Charset_UTF_32Recognizer : CharsetUnicodeRecognizer
    {
        public abstract int GetChar(byte[] input, int index);

        public override CharsetMatch Match(CharsetDetector det)
        {
            byte[] input = det.GetRawInput();
            int limit = (det.GetRawLength() / 4) * 4;
            int numValid = 0;
            int numInvalid = 0;
            bool hasBOM = false;
            int confidence = 0;

            if (limit == 0)
                return null;

            if (GetChar(input, 0) == 0x0000FEFF)
                hasBOM = true;

            for (int i = 0; i < limit; i += 4)
            {
                int ch = GetChar(input, i);

                if (ch < 0 || ch >= 0x10FFFF || (ch >= 0xD800 && ch <= 0xDFFF))
                    numInvalid += 1;
                else
                    numValid += 1;
            }

            // Cook up some sort of confidence score, based on presence of a BOM and the existence of valid and/or invalid multi-byte sequences.
            if (hasBOM && numInvalid == 0)
                confidence = 100;
            else if (hasBOM && numValid > numInvalid * 10)
                confidence = 80;
            else if (numValid > 3 && numInvalid == 0)
                confidence = 100;
            else if (numValid > 0 && numInvalid == 0)
                confidence = 80;
            else if (numValid > numInvalid * 10)
                confidence = 25;// Probably corrupt UTF-32BE data.  Valid sequences aren't likely by chance.

            return confidence == 0 ? null : new CharsetMatch(this, confidence);
        }
    }

    internal class Charset_UTF_32_BERecognizer : Charset_UTF_32Recognizer
    {
        public override int GetChar(byte[] input, int index)
        {
            return (input[index + 0] & 0xFF) << 24 | (input[index + 1] & 0xFF) << 16 |
                   (input[index + 2] & 0xFF) << 8 | (input[index + 3] & 0xFF);
        }

        public override string GetName()
        {
            return "UTF-32BE";
        }
    }

    internal class Charset_UTF_32_LERecognizer : Charset_UTF_32Recognizer
    {
        public override int GetChar(byte[] input, int index)
        {
            return (input[index + 3] & 0xFF) << 24 | (input[index + 2] & 0xFF) << 16 |
                   (input[index + 1] & 0xFF) << 8 | (input[index + 0] & 0xFF);
        }

        public override string GetName()
        {
            return "UTF-32LE";
        }
    }
}
