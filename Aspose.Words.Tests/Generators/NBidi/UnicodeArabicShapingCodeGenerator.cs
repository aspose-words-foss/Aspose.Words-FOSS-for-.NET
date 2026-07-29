// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// NBidi - a .Net implementation of the BIDI (Bi-Directional Text) algorithm.

using System.Globalization;
using System.IO;
using Aspose.Bidi;
using Aspose.JavaAttributes;

namespace Aspose.Words.Tests.Generators.NBidi
{
    [JavaDelete("Not porting to Java.")]
    public class UnicodeArabicShapingCodeGenerator
    {
        public static void GenerateCode(StreamWriter sw)
        {
            CodeGenerator.WriteLicenseTerms(sw);
            sw.WriteLine("using System.Collections;");
            sw.WriteLine("using Aspose.Collections;");
            sw.WriteLine();
            sw.WriteLine("namespace Aspose.NBidi");
            sw.WriteLine("{");
            sw.WriteLine("\tpublic class UnicodeArabicShapingResolver");
            sw.WriteLine("\t{");
            sw.WriteLine("\t\tprivate UnicodeArabicShapingResolver() {}");
            sw.WriteLine();
            sw.WriteLine("\t\tpublic static ArabicShapeJoiningType GetArabicShapeJoiningType(char c)");
            sw.WriteLine("\t\t{");
            int start_val = -1;
            int last_val = -1;
            string last_result = string.Empty;
            using (StreamReader sr = File.OpenText(CodeGenerator.inputPath + "ArabicShaping.txt"))
            {
                while (sr.Peek() >= 0)
                {
                    string line = sr.ReadLine();
                    int comment = line.IndexOf('#');
                    if (comment == 0) continue;
                    if (comment > 0)
                        line = line.Substring(0, comment - 1);
                    line = line.Trim();
                    if (!StringUtil.HasChars(line))
                        continue;
                    
                    string[] fields = line.Split(';');
                    int char_val = int.Parse(fields[0], NumberStyles.HexNumber);
                    if (char_val != last_val + 1 || last_result != fields[2])
                    {
                        if (start_val != -1)
                        {
                            if (last_val > start_val)
                                sw.WriteLine("\t\t\tif (c >= '\\u{0:X4}' && c <= '\\u{1:X4}') return ArabicShapeJoiningType.{2};",
                                             start_val, last_val, last_result.Trim());
                            else if (last_val == start_val)
                                sw.WriteLine("\t\t\tif (c == '\\u{0:X4}') return ArabicShapeJoiningType.{1};",
                                             last_val, last_result.Trim());
                        }
                        start_val = char_val;
                    }
                    
                    last_result = fields[2];
                    last_val = char_val;
                }
            }
            sw.WriteLine("\t\t\tUnicodeGeneralCategory ugc = UnicodeCharacterDataResolver.GetUnicodeGeneralCategory(c);");
            sw.WriteLine("\t\t\tif (ugc == UnicodeGeneralCategory.Mn ||");
            sw.WriteLine("\t\t\t    ugc == UnicodeGeneralCategory.Me ||");
            sw.WriteLine("\t\t\t    ugc == UnicodeGeneralCategory.Cf)");
            sw.WriteLine("\t\t\t\treturn ArabicShapeJoiningType.T;");
            sw.WriteLine("\t\t\treturn ArabicShapeJoiningType.U;");
            sw.WriteLine("\t\t}"); // method
            sw.WriteLine();
            sw.WriteLine("\t\tpublic static char GetArabicCharacterByLetterForm(char ch, LetterForm form)");
            sw.WriteLine("\t\t{");
            sw.WriteLine("\t\t\tint key = ((int)ch) | ((int)form) << 16;");
            sw.WriteLine("\t\t\tchar value = gCharForms[key];");
            sw.WriteLine("\t\t\treturn IntToCharDictionary.IsNullSubstitute(value) ? ch : value;");
            sw.WriteLine("\t\t}"); // method
            sw.WriteLine();
            sw.WriteLine("\t\tstatic UnicodeArabicShapingResolver()");
            sw.WriteLine("\t\t{");
            using (StreamReader sr = File.OpenText(CodeGenerator.inputPath + "UnicodeData.txt"))
            {
                while (sr.Peek() >= 0)
                {
                    string line = sr.ReadLine();
                    int comment = line.IndexOf('#');
                    if (comment > 0)
                        line = line.Substring(0, comment - 1);
                    line = line.Trim();
                    if (!StringUtil.HasChars(line))
                        continue;
                    
                    if (comment != 0)
                    {
                        string[] fields = line.Split(';');
                        string charStr = fields[0];
                        string[] decomposition = fields[5].Split(' ');
                        if (decomposition.Length == 2)
                        {
                            int charNum = int.Parse(decomposition[1], NumberStyles.HexNumber);
                            int key = charNum;
                            switch (decomposition[0])
                            {
                                case "<isolated>": key |= (int)(LetterForm.Isolated) << 16; break;
                                case "<final>": key |= (int)(LetterForm.Final) << 16; break;
                                case "<initial>": key |= (int)(LetterForm.Initial) << 16; break;
                                case "<medial>": key |= (int)(LetterForm.Medial) << 16; break;
                                default: key = -1; break;
                            }
                            if (key != -1)
                                sw.WriteLine("\t\t\tgCharForms[0x{0:X5}] = '\\u{1}';", key, charStr);
                        }
                    }
                }
            }        
            sw.WriteLine("\t\t}"); // cctor
            sw.WriteLine();
            sw.WriteLine("\t\tprivate static readonly IntToCharDictionary gCharForms = new IntToCharDictionary();");
            sw.WriteLine("\t}"); // class
            sw.WriteLine("}"); // namespace
        }
    }
}
