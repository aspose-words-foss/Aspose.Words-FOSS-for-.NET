// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/04/2016 by Anatoly Sidorenko

/**
 * ******************************************************************************
 * Copyright (C) 2005-2014, International Business Machines Corporation and    *
 * others. All Rights Reserved.                                                *
 * ******************************************************************************
 */
// 31/03/16 port to C# by Anatoly Sidorenko

namespace Aspose.Charset
{
    /// <summary>
    /// class CharsetISO2022Recognizer  part of the ICU charset detection implementation.
    /// This is a superclass for the individual detectors for each of the detectable members of the ISO 2022 family
    /// of encodings.
    /// <p/>
    /// The separate classes are nested within this class.
    /// </summary>
    public abstract class CharsetISO2022Recognizer : CharsetRecognizer
    {
        /// <summary>
        /// Matching function shared among the 2022 detectors JP, CN and KR
        /// Counts up the number of legal an unrecognized escape sequences in
        /// the sample of text, and computes a score based on the total number &
        /// the proportion that fit the encoding.
        ///
        /// <param name="text">the byte buffer containing text to analyse</param> 
        /// <param name="textLen">the size of the text in the byte.</param>          
        /// <param name="escapeSequences">the byte escape sequences to test for.</param>  
        /// <returns>match quality, in the range of 0-100.</returns> 
        /// </summary>
        internal static int Match(byte[] text, int textLen, byte[][] escapeSequences)
        {
            int i = 0;
            int hits = 0;
            int misses = 0;
            int shifts = 0;
            int quality;

            while (i < textLen)
            {
                if (text[i] == 0x1b)
                {
                    int seqLenght = EqualLengthSequences(text, textLen, i, escapeSequences);
                    if (seqLenght > 0)
                    {
                        hits++;
                        i += seqLenght - 1;
                    }
                    else
                    {
                        misses++;
                    }
                }

                if (text[i] == 0x0e || text[i] == 0x0f)
                {
                    // Shift in/out
                    shifts++;
                }
                i++;
            }

            if (hits == 0)
                return 0;

            // Initial quality is based on relative proportion of recongized vs.
            //   unrecognized escape sequences.
            //   All good:  quality = 100;
            //   half or less good: quality = 0;
            //   linear inbetween.
            quality = (100 * hits - 100 * misses) / (hits + misses);

            // Back off quality if there were too few escape sequences seen.
            //   Include shifts in this computation, so that KR does not get penalized
            //   for having only a single Escape sequence, but many shifts.
            if (hits + shifts < 5)
                quality -= (5 - (hits + shifts)) * 10;

            if (quality < 0)
                quality = 0;

            return quality;
        }

        private static int EqualLengthSequences(byte[] text, int textLen, int textPos, byte[][] escapeSequences)
        {
            int seqLenght = 0;
            for (int escN = 0; escN < escapeSequences.Length; escN++)
            {

                byte[] seq = escapeSequences[escN];

                if ((textLen - textPos) >= seq.Length)
                {
                    for (int j = 1; j < seq.Length; j++)
                    {
                        if (seq[j] == text[textPos + j])
                            seqLenght++;
                        else
                            seqLenght = 0;
                        if (seqLenght == seq.Length)
                            return seqLenght;
                    }
                }

                seqLenght = 0;
            }

            return seqLenght;
        }
    }

    internal class CharsetISO_2022RecognizerJP : CharsetISO2022Recognizer
    {
        public override string GetName()
        {
            return "ISO-2022-JP";
        }

        public override CharsetMatch Match(CharsetDetector det)
        {
            int confidence = Match(det.GetInputBytes(), det.GetInputLen(), escapeSequences);
            return confidence == 0 ? null : new CharsetMatch(this, confidence);
        }

        private readonly byte[][] escapeSequences =
        {
                new byte[] {0x1b, 0x24, 0x28, 0x43}, // KS X 1001:1992
                new byte[] {0x1b, 0x24, 0x28, 0x44}, // JIS X 212-1990
                new byte[] {0x1b, 0x24, 0x40}, // JIS C 6226-1978
                new byte[] {0x1b, 0x24, 0x41}, // GB 2312-80
                new byte[] {0x1b, 0x24, 0x42}, // JIS X 208-1983
                new byte[] {0x1b, 0x26, 0x40}, // JIS X 208 1990, 1997
                new byte[] {0x1b, 0x28, 0x42}, // ASCII
                new byte[] {0x1b, 0x28, 0x48}, // JIS-Roman
                new byte[] {0x1b, 0x28, 0x49}, // Half-width katakana
                new byte[] {0x1b, 0x28, 0x4a}, // JIS-Roman
                new byte[] {0x1b, 0x2e, 0x41}, // ISO 8859-1
                new byte[] {0x1b, 0x2e, 0x46} // ISO 8859-7
            };
    }

    internal class CharsetISO_2022RecognizerKR : CharsetISO2022Recognizer
    {
        public override string GetName()
        {
            return "ISO-2022-KR";
        }

        public override CharsetMatch Match(CharsetDetector det)
        {
            int confidence = Match(det.GetInputBytes(), det.GetInputLen(), escapeSequences);
            return confidence == 0 ? null : new CharsetMatch(this, confidence);
        }

        private readonly byte[][] escapeSequences =
        {
                new byte[] {0x1b, 0x24, 0x29, 0x43}
            };
    }

    internal class CharsetISO_2022RecognizerCN : CharsetISO2022Recognizer
    {
        public override string GetName()
        {
#if JAVA
            // Absent in .NET, "x-cp50227" used instead.
            return "ISO-2022-CN";
#else
            return "x-cp50227";
#endif
        }

        public override CharsetMatch Match(CharsetDetector det)
        {
            int confidence = Match(det.GetInputBytes(), det.GetInputLen(), escapeSequences);
            return confidence == 0 ? null : new CharsetMatch(this, confidence);
        }

        private readonly byte[][] escapeSequences =
        {
                new byte[] {0x1b, 0x24, 0x29, 0x41}, // GB 2312-80
                new byte[] {0x1b, 0x24, 0x29, 0x47}, // CNS 11643-1992 Plane 1
                new byte[] {0x1b, 0x24, 0x2A, 0x48}, // CNS 11643-1992 Plane 2
                new byte[] {0x1b, 0x24, 0x29, 0x45}, // ISO-IR-165
                new byte[] {0x1b, 0x24, 0x2B, 0x49}, // CNS 11643-1992 Plane 3
                new byte[] {0x1b, 0x24, 0x2B, 0x4A}, // CNS 11643-1992 Plane 4
                new byte[] {0x1b, 0x24, 0x2B, 0x4B}, // CNS 11643-1992 Plane 5
                new byte[] {0x1b, 0x24, 0x2B, 0x4C}, // CNS 11643-1992 Plane 6
                new byte[] {0x1b, 0x24, 0x2B, 0x4D}, // CNS 11643-1992 Plane 7
                new byte[] {0x1b, 0x4e}, // SS2
                new byte[] {0x1b, 0x4f}, // SS3
            };
    }
}
