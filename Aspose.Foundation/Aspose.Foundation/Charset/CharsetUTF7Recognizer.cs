// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/03/2016 by Anatoly Sidorenko

/**
 * ******************************************************************************
 * Copyright (C) 2005-2014, International Business Machines Corporation and    *
 * others. All Rights Reserved.                                                *
 * ******************************************************************************
 */
// 30/03/16 port to C# by Anatoly Sidorenko

namespace Aspose.Charset
{
    /// <summary>
    /// It is very basic class which tries to detects UTF-7 either by BOM or by content.
    /// Not all of the RFC rules were implemented yet.
    /// UTF-7 is rarely used encoding. ICU4J doesn't even recognize it.
    /// It is been recommended to use UTF-8 instead.
    /// Anyway we have some tests which use UTF-7.
    /// <p/>
    /// <see cref="https://tools.ietf.org/html/rfc2152"/>
    /// </summary>
    internal class CharsetUTF7Recognizer : CharsetRecognizer
    {
        public override string GetName()
        {
            return "UTF-7";
        }

        public override CharsetMatch Match(CharsetDetector det)
        {
            bool hasBOM = false;
            byte[] input = det.GetRawInput();

            if (det.GetRawLength() >= 3 && (input[0] == 0x2b) && (input[1] == 0x2f) && (input[2] == 0x76))
                hasBOM = true;

            int numValidBase64EncodedStrings = 0;
            int numInvalidBase64Strings = 0;
            int prohibitedChars = 0;
            int numInvalid = 0;
            int numValid = 0;
            int numPotentialIssues = 0;
            int b;

            DataIterator it = new DataIterator(det);
            while ((b = it.nextByte()) >= 0)
            {
                if (IsProhibited(b))
                {
                    prohibitedChars++;
                }
                // RULE2, Set B. +Base64String-|CR|LF, also +-
                else if (b == '+')
                {
                    if (isBase64Encoded(it))
                        numValidBase64EncodedStrings++;
                    else
                        numInvalidBase64Strings++;
                }
                else
                {
                    if (isUTF7Alphabet(b)) // RULE1, Set D
                        numValid++;
                    else if (isSet0(b)) // RULE1, Set 0
                        numPotentialIssues++;
                    else if (isWhiteSpace(b)) // RULE 3, CR, LF etc.
                        numValid++;
                    else
                        numInvalid++;
                }
            }

            int totalValid = numValidBase64EncodedStrings + numValid;
            int totalInvalid = numInvalid + numInvalidBase64Strings;
            int invalidPercentage = (int)(((float)totalInvalid / it.getTotalBytes()) * 100);
            int confidence = 0;

            if (hasBOM && totalInvalid == 0)
                confidence = 100;
            else if (hasBOM && totalValid > (totalInvalid * 10))
                confidence = 80;
            // no bom and no encoded sequences most likely it is not UTF-7
            else if (!hasBOM && numValidBase64EncodedStrings == 0)
                confidence = 0; // use UTF-16
            else if (totalValid > 3 && numValidBase64EncodedStrings > 3 && totalInvalid == 0)
                confidence = 100;
            else if (invalidPercentage > 30) // too many invalid characters most likely it is not UTF-7
                confidence = 10; //
            else if (numValid > 3 && numValidBase64EncodedStrings > 3)
                // WORDSNET-24468 Decrease base confidence from 80 to 50 to give more chances for other encodings.
                confidence = 50 - invalidPercentage;

            // if there are prohibited chars then it could be mistake but also could be another encoding.
            confidence -= prohibitedChars * 10;
            // if there are lots of <>/ etc then probably it is markup language. Use another encoding then
            confidence -= numPotentialIssues * 10;

            return confidence <= 0 ? null : new CharsetMatch(this, confidence);
        }

        private static bool isBase64Encoded(DataIterator dataIterator)
        {
            int b;
            while ((b = dataIterator.nextByte()) >= 0)
            {
                if (b == '-' || b == '\n' || b == '\r')
                    break;

                if (!isBase64(b))
                    return false;
            }
            return true;
        }

        private static bool IsProhibited(int b)
        {
            return b == '=' || b == '\\' || b == '~';
        }

        private static bool isUTF7Alphabet(int b)
        {
            // set D
            return
                (b >= 'A' && b <= 'Z')
                || (b >= 'a' && b <= 'z')
                || (b >= '0' && b <= '9')
                || b == '\'' || b == '(' || b == ')' || b == ',' || b == '-' || b == '.'
                || b == '/' || b == ':' || b == '?'; // + and = omitted
        }

        private static bool isSet0(int b)
        {
            return b == '!' || b == '"' || b == '#' || b == '$' || b == '%' || b == '&' || b == '*'
                   || b == ';' || b == '<' || b == '=' || b == '>' || b == '@' || b == '[' || b == ']'
                   || b == '^' || b == '_' || b == '\'' || b == '{' || b == '|' || b == '}'
                ; // "\" and "~" are omitted
        }

        private static bool isWhiteSpace(int b)
        {
            return b == '\r' || b == '\t' || b == '\n' || b == ' ';
        }

        private static bool isBase64(int b)
        {
            return
                (b >= 'A' && b <= 'Z')
                || (b >= 'a' && b <= 'z')
                || (b >= '0' && b <= '9')
                || (b == '+' || b == '\\')
                || (b == '~')

                // plus non-printing
                || (b >= 0 && b <= 8)
                || (b == 11 || b == 12 || b == 127)
                || (b >= 14 && b <= 31)
                ;
        }

        private class DataIterator
        {
            internal DataIterator(CharsetDetector detector)
            {
                this.mDetector = detector;
            }

            public int nextByte()
            {
                if (mByteIndex >= mDetector.GetInputLen())
                    return -1;

                mTotalBytes++;
                return mDetector.GetInputBytes()[mByteIndex++] & 0xFF;
            }

            public int getTotalBytes()
            {
                return mTotalBytes;
            }

            private readonly CharsetDetector mDetector;
            private int mByteIndex;
            private int mTotalBytes;
        }
    }
}
