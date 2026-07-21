// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// NBidi - a .Net implementation of the BIDI (Bi-Directional Text) algorithm.

using System.IO;
using Aspose.JavaAttributes;
using NUnit.Framework;

namespace Aspose.Words.Tests.Generators.NBidi
{
    /// <summary>
    /// Class for generation source code files by Unicode Character Database files:
    /// - UnicodeCharacterDataResolver.cs (by UnicodeData.txt)
    /// - BidiCharacterMirrorResolver.cs (by BidiMirroring.txt)
    /// - UnicodeArabicShapingResolver.cs (by ArabicShaping.txt)
    /// The latest version of files of the Unicode Character Database:
    /// http://unicode.org/Public/UNIDATA/
    /// </summary>
    [JavaDelete("Not porting to Java.")]
    [TestFixture]
    public class CodeGenerator
    {
        private static readonly string outputPath = TestUtil.GetInRootPath(@"Aspose.Foundation\Aspose.Foundation\NBidi\");
        internal static readonly string inputPath = TestUtil.GetInRootPath(@"Aspose.Words\Tests\Generators\NBidi\");

        [Test, Ignore("CodeGenerator")]
        public void TestGenerate()
        {
            using (StreamWriter sw = new StreamWriter(outputPath + "UnicodeCharacterDataResolver.cs", false, System.Text.Encoding.UTF8))
                UnicodeDataCodeGenerator.GenerateCode(sw);

            using (StreamWriter sw = new StreamWriter(outputPath + "BidiCharacterMirrorResolver.cs", false, System.Text.Encoding.UTF8))
                UnicodeMirroringCodeGenerator.GenerateCode(sw);

            using (StreamWriter sw = new StreamWriter(outputPath + "UnicodeArabicShapingResolver.cs", false, System.Text.Encoding.UTF8))
                UnicodeArabicShapingCodeGenerator.GenerateCode(sw);
        }

        internal static void WriteLicenseTerms(StreamWriter sw)
        {
            sw.WriteLine("// Copyright (c) 2001-2014 Aspose Pty Ltd. All Rights Reserved.");
            sw.WriteLine("// NBidi - a .Net implementation of the BIDI (Bi-Directional Text) algorithm.");
            sw.WriteLine("//");
            sw.WriteLine("// NOTE: The file is generated! See Aspose.Words.Tests.Generators.NBidi.CodeGenerator");
            sw.WriteLine("// and its implementations for details.");
            sw.WriteLine();
        }
    }
}
