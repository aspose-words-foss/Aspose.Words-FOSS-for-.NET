// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// NBidi - a .Net implementation of the BIDI (Bi-Directional Text) algorithm.

using System.IO;
using Aspose.JavaAttributes;

namespace Aspose.Words.Tests.Generators.NBidi
{
    [JavaDelete("Not porting to Java.")]
    public class UnicodeMirroringCodeGenerator
    {
        public static void GenerateCode(StreamWriter sw)
        {
            CodeGenerator.WriteLicenseTerms(sw);
            sw.WriteLine("namespace Aspose.NBidi");
            sw.WriteLine("{");
            sw.WriteLine("\t/// <summary>");
            sw.WriteLine("\t/// An helper class that provides the mirrored alternatives for characters.");
            sw.WriteLine("\t/// </summary>");
            sw.WriteLine("\tpublic abstract class BidiCharacterMirrorResolver");
            sw.WriteLine("\t{");
            sw.WriteLine("\t\t/// <summary>");
            sw.WriteLine("\t\t/// Returns the corresponding mirrored character for the given character, if any. If no mirroring available, returns the given character.");
            sw.WriteLine("\t\t/// </summary>");
            sw.WriteLine("\t\t/// <param name=\"c\">A character to mirror.</param>");
            sw.WriteLine("\t\t/// <returns>The mirrored character, or the given character if no mirroring available.</returns>");
            sw.WriteLine("\t\tpublic static char GetBidiCharacterMirror(char c)");
            sw.WriteLine("\t\t{");
            sw.WriteLine("\t\t\tswitch (c)");
            sw.WriteLine("\t\t\t{");

            using (StreamReader sr = File.OpenText(CodeGenerator.inputPath + "BidiMirroring.txt"))
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
                        sw.WriteLine("\t\t\t\tcase '\\u{0}': return '\\u{1}';", fields[0].Trim(), fields[1].Trim());
                    }
                }
            }
            sw.WriteLine("\t\t\t\tdefault: return c;");
            sw.WriteLine("\t\t\t}"); // switch
            sw.WriteLine("\t\t}"); // method
            sw.WriteLine("\t}"); // class
            sw.WriteLine("}"); // namespace
        }
    }
}
