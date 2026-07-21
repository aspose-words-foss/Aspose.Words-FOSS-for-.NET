// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// NBidi - a .Net implementation of the BIDI (Bi-Directional Text) algorithm.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Aspose.Bidi;
using Aspose.Common;
using Aspose.JavaAttributes;

namespace Aspose.Words.Tests.Generators.NBidi
{
    /// <summary>
    /// Generate source code files 
    /// </summary>
    [JavaDelete("Not porting to Java.")]
    public class UnicodeDataCodeGenerator
    {
        internal struct UnicodeCharData
        {
            public UnicodeCharData(int num,
                                   UnicodeCanonicalClass canonicalClass,
                                   string decompositionMapping)
            {
                mNum = num;
                mCanonicalClass = canonicalClass;
                mDecompositionMapping = decompositionMapping;
            }

            private int mNum;
            private UnicodeCanonicalClass mCanonicalClass;
            private string mDecompositionMapping;

            public int Num
            {
                get { return mNum; }
            }

            public UnicodeCanonicalClass CanonicalClass
            {
                get { return mCanonicalClass; }
            }

            public string DecompositionMapping
            {
                get { return mDecompositionMapping; }
            }
        }

        private static void GenerateRunLengthArray(StreamWriter sw, int enumValue, string enumName, 
            string listName, List<int>[] arr)
        {
            sw.Write("\t\tstatic int[] {0}_{1} = new int[] {{ ", listName, enumName);
            int i = 4;
            int last_ci = -2;
            int count = 1;
            bool writeCount = false;
            foreach (int ci in arr[enumValue])
            {
                if (ci == last_ci + 1)
                    ++count;
                else
                {
                    if (writeCount)
                    {
                        sw.Write("{0}, ", count);
                        count = 1;
                    }
                    sw.Write("0x{0:X4}, ", ci);
                    ++i;
                    if (i > 9)
                    {
                        sw.WriteLine();
                        sw.Write("\t\t\t");
                        i = 0;
                    }
                    writeCount = true;
                }
                last_ci = ci;
            }
            if (writeCount)
                sw.Write(count);
            sw.WriteLine("};");
        }

        public static void GenerateCode(StreamWriter sw)
        {
            List<UnicodeCharData> charData = new List<UnicodeCharData>();

            List<int>[] charTypes = new List<int>[19];
            for (int i = 0; i < charTypes.Length; ++i)
                charTypes[i] = new List<int>();

            List<int>[] charCategories = new List<int>[30];
            for (int i = 0; i < charCategories.Length; ++i)
                charCategories[i] = new List<int>();

            List<int>[] charDecompTypes = new List<int>[17];
            for (int i = 0; i < charDecompTypes.Length; ++i)
                charDecompTypes[i] = new List<int>();

            using (StreamReader sr = File.OpenText(CodeGenerator.inputPath + "UnicodeData.txt"))
            {
                while (sr.Peek() >= 0)
                {
                    string line = sr.ReadLine();
                    int comment = line.IndexOf('#');
                    if (comment >= 0)
                        line = line.Substring(0, comment - 1);
                    if (!StringUtil.HasChars(line))
                        continue;
                    string[] fields = line.Split(';');
                    int charNum = int.Parse(fields[0], NumberStyles.HexNumber);
                    if (charNum <= 0xffff)
                    {
                        BidiCharacterType bct = (BidiCharacterType)(Enum.Parse(typeof(BidiCharacterType), fields[4]));
                        UnicodeGeneralCategory ugc = (UnicodeGeneralCategory)(Enum.Parse(typeof(UnicodeGeneralCategory), fields[2]));
                        UnicodeCanonicalClass ucc = (UnicodeCanonicalClass)(Enum.Parse(typeof(UnicodeCanonicalClass), fields[3]));
                        UnicodeDecompositionType udt = UnicodeDecompositionType.None;
                        string udm = null;
                        if (StringUtil.HasChars(fields[5]))
                        {
                            int startsWith = 0;
                            string[] decomposition = fields[5].Split(' ');
                            if (decomposition[0].StartsWith("<") && decomposition[0].EndsWith(">"))
                            {
                                udt = (UnicodeDecompositionType)(Enum.Parse(typeof(UnicodeDecompositionType), decomposition[0].Trim('<', '>'), true));
                                startsWith = 1;
                            }
                            if (decomposition.Length > startsWith)
                            {
                                udm = string.Empty;
                                for (int i = startsWith; i < decomposition.Length; ++i)
                                    udm += "\\u" + FormatterPal.IntToStrX4(int.Parse(decomposition[i], NumberStyles.HexNumber));
                            }
                        }
                        UnicodeCharData ucd = new UnicodeCharData(charNum, ucc, udm);
                        charData.Add(ucd);
                        charTypes[(int)bct].Add(charNum);
                        charCategories[(int)ugc].Add(charNum);
                        charDecompTypes[(int)udt].Add(charNum);
                    }
                }
            }

            CodeGenerator.WriteLicenseTerms(sw);
            sw.WriteLine("using System;");
            sw.WriteLine("using System.Collections;");
            sw.WriteLine("namespace Aspose.NBidi");
            sw.WriteLine("{");
            sw.WriteLine();

            sw.WriteLine("\tpublic abstract class UnicodeCharacterDataResolver");
            sw.WriteLine("\t{");
            sw.WriteLine("\t\tstatic BidiCharacterType[] bidiCharType = new BidiCharacterType[0xffff];");
            sw.WriteLine("\t\tstatic Hashtable categories = new Hashtable();");
            sw.WriteLine("\t\tstatic Hashtable decomType = new Hashtable();");
            sw.WriteLine("\t\tstatic Hashtable canonClass = new Hashtable();");
            sw.WriteLine("\t\tstatic Hashtable decomMapping = new Hashtable();");
            sw.WriteLine("\t\tstatic Hashtable composeMapping = new Hashtable();");
            sw.WriteLine();

            foreach (BidiCharacterType bct in Enum.GetValues(typeof(BidiCharacterType)))
            {
                if (bct != BidiCharacterType.L)
                    GenerateRunLengthArray(sw, (int)bct, bct.ToString(), "BctList", charTypes);
            }

            foreach (UnicodeGeneralCategory ugc in Enum.GetValues(typeof(UnicodeGeneralCategory)))
            {
                if (ugc != UnicodeGeneralCategory.Cn)
                    GenerateRunLengthArray(sw, (int)ugc, ugc.ToString(), "UgcList", charCategories);
            }

            foreach (UnicodeDecompositionType udt in Enum.GetValues(typeof(UnicodeDecompositionType)))
            {
                if (udt != UnicodeDecompositionType.None)
                    GenerateRunLengthArray(sw, (int)udt, udt.ToString(), "UdtList", charDecompTypes);
            }

            sw.WriteLine();
            sw.WriteLine("\t\t/// <summary>");
            sw.WriteLine("\t\t/// Returns the BiDi type for a given character.");
            sw.WriteLine("\t\t/// </summary>");
            sw.WriteLine("\t\t/// <param name=\"c\">A Unicode character for which to get the BiDi type.</param>");
            sw.WriteLine("\t\t/// <returns>The character BiDi type.</returns>");
            sw.WriteLine("\t\tpublic static BidiCharacterType GetBidiCharacterType(char c)");
            sw.WriteLine("\t\t{");
            sw.WriteLine("\t\t\treturn bidiCharType[c];");
            sw.WriteLine("\t\t}");
            sw.WriteLine();
            sw.WriteLine("\t\t/// <summary>");
            sw.WriteLine("\t\t/// Returns the Unicode category for a given character.");
            sw.WriteLine("\t\t/// </summary>");
            sw.WriteLine("\t\t/// <param name=\"c\">A Unicode character for which to get the general Unicode category.</param>");
            sw.WriteLine("\t\t/// <returns>The character general Unicode category.</returns>");
            sw.WriteLine("\t\tpublic static UnicodeGeneralCategory GetUnicodeGeneralCategory(char c)");
            sw.WriteLine("\t\t{");
            sw.WriteLine("\t\t\tif (categories.ContainsKey(c))");
            sw.WriteLine("\t\t\t\treturn (UnicodeGeneralCategory) categories[c];");
            sw.WriteLine("\t\t\treturn UnicodeGeneralCategory.Cn;");
            sw.WriteLine("\t\t}");
            sw.WriteLine();
            sw.WriteLine("\t\t/// <summary>");
            sw.WriteLine("\t\t/// Returns the Unicode canonical class for a given character.");
            sw.WriteLine("\t\t/// </summary>");
            sw.WriteLine("\t\t/// <param name=\"c\">A Unicode character for which to get the Unicode canonical class.</param>");
            sw.WriteLine("\t\t/// <returns>The character Unicode canonical class.</returns>");
            sw.WriteLine("\t\tpublic static UnicodeCanonicalClass GetUnicodeCanonicalClass(char c)");
            sw.WriteLine("\t\t{");
            sw.WriteLine("\t\t\tif (canonClass.ContainsKey(c))");
            sw.WriteLine("\t\t\t\treturn (UnicodeCanonicalClass) canonClass[c];");
            sw.WriteLine("\t\t\treturn UnicodeCanonicalClass.NR;");
            sw.WriteLine("\t\t}");
            sw.WriteLine();
            sw.WriteLine("\t\tpublic static UnicodeDecompositionType GetUnicodeDecompositionType(char c)");
            sw.WriteLine("\t\t{");
            sw.WriteLine("\t\t\tif (decomType.ContainsKey(c))");
            sw.WriteLine("\t\t\t\treturn (UnicodeDecompositionType)decomType[c];");
            sw.WriteLine("\t\t\treturn UnicodeDecompositionType.None;");
            sw.WriteLine("\t\t}");
            sw.WriteLine();
            sw.WriteLine("\t\tpublic static string GetUnicodeDecompositionMapping(char c)");
            sw.WriteLine("\t\t{");
            sw.WriteLine("\t\t\tif (decomMapping.ContainsKey(c))");
            sw.WriteLine("\t\t\t\treturn (string)decomMapping[c];");
            sw.WriteLine("\t\t\treturn null;");
            sw.WriteLine("\t\t}");
            sw.WriteLine();
            sw.WriteLine("\t\tpublic static char Compose(string sequence)");
            sw.WriteLine("\t\t{");
            sw.WriteLine("\t\t\tif (composeMapping.ContainsKey(sequence))");
            sw.WriteLine("\t\t\t\treturn (char)composeMapping[sequence];");
            sw.WriteLine("\t\t\treturn '\\uFFFF';");
            sw.WriteLine("\t\t}");
            sw.WriteLine();
            sw.WriteLine("\t\tstatic UnicodeCharacterDataResolver()");
            sw.WriteLine("\t\t{");

            sw.WriteLine("\t\t\tfor (int i = 0; i < 0xffff; ++i)");
            sw.WriteLine("\t\t\t{");
            sw.WriteLine("\t\t\t\tbidiCharType[i] = BidiCharacterType.L;");
            sw.WriteLine("\t\t\t}");

            foreach (BidiCharacterType bct in Enum.GetValues(typeof(BidiCharacterType)))
            {
                if (bct != BidiCharacterType.L)
                {
                    sw.WriteLine("\t\t\tfor (int i = 0; i < BctList_{0}.Length; i+=2)", bct.ToString());
                    sw.WriteLine("\t\t\t\tfor (int j = BctList_{0}[i]; j < BctList_{0}[i] + BctList_{0}[i+1]; ++j)", bct.ToString());
                    sw.WriteLine("\t\t\t\t\tbidiCharType[j] = BidiCharacterType.{0};", bct.ToString());
                    sw.WriteLine();
                }
            }

            foreach (UnicodeGeneralCategory ugc in Enum.GetValues(typeof(UnicodeGeneralCategory)))
            {
                if (ugc != UnicodeGeneralCategory.Cn)
                {
                    sw.WriteLine("\t\t\tfor (int i = 0; i < UgcList_{0}.Length; i+=2)", ugc.ToString());
                    sw.WriteLine("\t\t\t\tfor (int j = UgcList_{0}[i]; j < UgcList_{0}[i] + UgcList_{0}[i+1]; ++j)", ugc.ToString());
                    sw.WriteLine("\t\t\t\t\tcategories[(char)j] = UnicodeGeneralCategory.{0};", ugc.ToString());
                    sw.WriteLine();
                }
            }

            foreach (UnicodeDecompositionType udt in Enum.GetValues(typeof(UnicodeDecompositionType)))
            {
                if (udt != UnicodeDecompositionType.None)
                {
                    sw.WriteLine("\t\t\tfor (int i = 0; i < UdtList_{0}.Length; i+=2)", udt.ToString());
                    sw.WriteLine("\t\t\t\tfor (int j = UdtList_{0}[i]; j < UdtList_{0}[i] + UdtList_{0}[i+1]; ++j)", udt.ToString());
                    sw.WriteLine("\t\t\t\t\tdecomType[(char)j] = UnicodeDecompositionType.{0};", udt.ToString());
                    sw.WriteLine();
                }
            }

            foreach (UnicodeCharData ucd in charData)
            {
                if (ucd.CanonicalClass != UnicodeCanonicalClass.NR)
                    sw.WriteLine("\t\t\tcanonClass['\\u{0:X4}'] = UnicodeCanonicalClass.{1};", ucd.Num, ucd.CanonicalClass.ToString());
            }

            // These maps are two huge to compile in one method/constructor in Java. We are splitting them into several methods.
            sw.WriteLine();
            sw.WriteLine("\t\t\tInitDecomMapping1();");
            sw.WriteLine("\t\t\tInitDecomMapping2();");
            sw.WriteLine("\t\t\tInitComposeMapping1();");
            sw.WriteLine("\t\t\tInitComposeMapping2();");

            sw.WriteLine("\t\t}"); // cctor
            sw.WriteLine();

            // This is to divide huge maps
            int charDataCount = charData.Count;
            int halfCharDataCount = charDataCount / 2;

            // Decomposition mapping. Part1
            sw.WriteLine("\t\tprivate static void InitDecomMapping1()");
            sw.WriteLine("\t\t{");
            for (int i = 0; i < halfCharDataCount; ++i)
            {
                UnicodeCharData ucd = (UnicodeCharData)charData[i];
                if (ucd.DecompositionMapping != null)
                    sw.WriteLine("\t\t\tdecomMapping['\\u{0:X4}'] = \"{1}\";", ucd.Num, ucd.DecompositionMapping);
            }
            sw.WriteLine("\t\t}"); // InitDecomMapping1
            sw.WriteLine();

            // Decomposition mapping. Part2
            sw.WriteLine("\t\tprivate static void InitDecomMapping2()");
            sw.WriteLine("\t\t{");
            for (int i = halfCharDataCount; i < charDataCount; ++i)
            {
                UnicodeCharData ucd = (UnicodeCharData)charData[i];
                if (ucd.DecompositionMapping != null)
                    sw.WriteLine("\t\t\tdecomMapping['\\u{0:X4}'] = \"{1}\";", ucd.Num, ucd.DecompositionMapping);
            }
            sw.WriteLine("\t\t}"); // InitDecomMapping2
            sw.WriteLine();

            // Composition mapping. Part1
            sw.WriteLine("\t\tprivate static void InitComposeMapping1()");
            sw.WriteLine("\t\t{");
            for (int i = 0; i < halfCharDataCount; ++i)
            {
                UnicodeCharData ucd = (UnicodeCharData)charData[i];
                if (ucd.DecompositionMapping != null)
                    sw.WriteLine("\t\t\tcomposeMapping[\"{0}\"] = '\\u{1:X4}\';", ucd.DecompositionMapping, ucd.Num);
            }
            sw.WriteLine("\t\t}"); // InitComposeMapping1
            sw.WriteLine();

            // Composition mapping. Part2
            sw.WriteLine("\t\tprivate static void InitComposeMapping2()");
            sw.WriteLine("\t\t{");
            for (int i = halfCharDataCount; i < charDataCount; ++i)
            {
                UnicodeCharData ucd = (UnicodeCharData)charData[i];
                if (ucd.DecompositionMapping != null)
                    sw.WriteLine("\t\t\tcomposeMapping[\"{0}\"] = '\\u{1:X4}\';", ucd.DecompositionMapping, ucd.Num);
            }
            sw.WriteLine("\t\t}"); // InitComposeMapping2
            
            sw.WriteLine("\t}"); // class
            sw.WriteLine("}"); // namespace
        }
    }
}
