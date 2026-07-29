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
    /// Charset recognizer for UTF-8
    /// </summary>
    internal class CharsetUTF8Recognizer : CharsetRecognizer
    {
        public override string GetName()
        {
            return "UTF-8";
        }

        public override CharsetMatch Match(CharsetDetector det)
        {
            int numValid = 0;
            int numInvalid = 0;
            int confidence = 0;

            // WORDSNET-27863 Use specially prepared InputBytes array insted of RawInput bytes.
            byte[] input = det.GetInputBytes();
            // Scan for multibyte sequences.
            for (int i = 0; i < det.GetInputLen(); i++)
            {
                byte b = input[i];
                if ((b & 0x80) == 0)
                    continue; // ASCII

                // High bit on char found.  Figure out how long the sequence should be.
                int trailBytes;
                if ((b & 0xe0) == 0xc0)
                    trailBytes = 1;
                else if ((b & 0xf0) == 0xe0)
                    trailBytes = 2;
                else if ((b & 0xf8) == 0xf0)
                    trailBytes = 3;
                else
                {
                    numInvalid++;
                    continue;
                }

                // Verify that we've got the right number of trail bytes in the sequence.
                while (true)
                {
                    i++;
                    if (i >= input.Length)
                        break;

                    b = input[i];
                    if ((b & 0xc0) != 0x80)
                    {
                        numInvalid++;
                        break;
                    }

                    if (--trailBytes == 0)
                    {
                        numValid++;
                        break;
                    }
                }
            }

            byte[] rawInput = det.GetRawInput();
            bool hasBom = (det.GetRawLength() >= 3) && (rawInput[0] == 0xef) && (rawInput[1] == 0xbb) && (rawInput[2] == 0xbf);

            // Cook up some sort of confidence score, based on presence of a BOM and
            // the existence of valid and/or invalid multibyte sequences.
            if (hasBom && numInvalid == 0)
                confidence = 100;

            else if (hasBom && numValid > numInvalid * 10)
                confidence = 80;

            else if (numValid > 3 && numInvalid == 0)
                confidence = 100;

            else if (numValid > 0 && numInvalid == 0)
                confidence = 80;

            else if (numValid == 0 && numInvalid == 0)
            {
                // Plain ASCII. Confidence must be > 10, it's more likely than UTF-16, which accepts ASCII with confidence = 10.
                // TODO: add plain ASCII as an explicitly detected type.
                confidence = 15;
            }
            else if (numValid > numInvalid * 10)
            {
                // Probably corrupt utf-8 data.  Valid sequences aren't likely by chance.
                confidence = 25;
            }

            return confidence == 0 ? null : new CharsetMatch(this, confidence);
        }
    }
}
