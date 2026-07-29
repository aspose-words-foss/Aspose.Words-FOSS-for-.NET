// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/01/2007 by Konstantin Sidorenko
// 14/01/2016 by Anatoliy Sidorenko

using System.IO;
using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System.IO
{
    [TestFixture]
    public class TestPath
    {
        [Test]
        public void TestPathGetFileName()
        {
            Assert.That("", Is.EqualTo(Path.GetFileName("C:\\tmp\\")));
            Assert.That("", Is.EqualTo(Path.GetFileName("C:\\")));
            Assert.That("", Is.EqualTo(Path.GetFileName("C:")));
            Assert.That("", Is.EqualTo(Path.GetFileName("")));
            Assert.That(null, Is.EqualTo(Path.GetFileName(null)));
            Assert.That("myfile", Is.EqualTo(Path.GetFileName("myfile")));
            Assert.That("simple.txt", Is.EqualTo(Path.GetFileName("C:\\tmp\\simple.txt")));
            Assert.That("very_simple.txt", Is.EqualTo(Path.GetFileName("very_simple.txt")));
            Assert.That("simple_forvard_slash.txt", Is.EqualTo(Path.GetFileName("C:/tmp/simple_forvard_slash.txt")));
            Assert.That("simple.with.dot.txt", Is.EqualTo(Path.GetFileName("C:\\tmp\\simple.with.dot.txt")));
            Assert.That("simple_without_dot", Is.EqualTo(Path.GetFileName("C:\\tmp\\simple_without_dot")));
            Assert.That("simple_with_dot_in_path.txt", Is.EqualTo(Path.GetFileName("C:\\tmp.dot\\simple_with_dot_in_path.txt")));
            Assert.That("simple.with.dot.evryvere.txt", Is.EqualTo(Path.GetFileName("C:\\tmp.dot\\simple.with.dot.evryvere.txt")));
        }

        [Test]
        public void TestPathGetFileNameWithoutExtension()
        {
            Assert.That("", Is.EqualTo(Path.GetFileNameWithoutExtension("")));
            Assert.That("", Is.EqualTo(Path.GetFileNameWithoutExtension("C:\\tmp\\")));
            Assert.That("", Is.EqualTo(Path.GetFileNameWithoutExtension("C:")));
            Assert.That("", Is.EqualTo(Path.GetFileNameWithoutExtension("C:\\tmp\\")));
            Assert.That("", Is.EqualTo(Path.GetFileNameWithoutExtension("")));
            Assert.That(null, Is.EqualTo(Path.GetFileNameWithoutExtension(null)));
            Assert.That("simple", Is.EqualTo(Path.GetFileNameWithoutExtension("C:\\tmp\\simple.txt")));
            Assert.That("very_simple", Is.EqualTo(Path.GetFileNameWithoutExtension("very_simple.txt")));
            Assert.That("simple_forvard_slash", Is.EqualTo(Path.GetFileNameWithoutExtension("C:/tmp/simple_forvard_slash.txt")));
            Assert.That("simple.with.dot", Is.EqualTo(Path.GetFileNameWithoutExtension("C:\\tmp\\simple.with.dot.txt")));
            Assert.That("simple_without_dot", Is.EqualTo(Path.GetFileNameWithoutExtension("C:\\tmp\\simple_without_dot")));
            Assert.That("simple_with_dot_in_path", Is.EqualTo(Path.GetFileNameWithoutExtension("C:\\tmp.dot\\simple_with_dot_in_path.txt")));
            Assert.That("simple.with.dot.evryvere", Is.EqualTo(Path.GetFileNameWithoutExtension("C:\\tmp.dot\\simple.with.dot.evryvere.txt")));
        }

        [Test]
        public void TestPathGetExtension()
        {
            Assert.That("", Is.EqualTo(Path.GetExtension("C:\\tmp\\simple")));
            Assert.That("", Is.EqualTo(Path.GetExtension("C:\\tmp\\")));
            Assert.That("", Is.EqualTo(Path.GetExtension("C:\\")));
            Assert.That("", Is.EqualTo(Path.GetExtension("C:")));
            Assert.That("", Is.EqualTo(Path.GetExtension("")));
            Assert.That(null, Is.EqualTo(Path.GetExtension(null)));
            Assert.That(".txt", Is.EqualTo(Path.GetExtension("C:\\tmp\\simple.txt")));
            Assert.That(".txt", Is.EqualTo(Path.GetExtension("very_simple.txt")));
            Assert.That(".txt", Is.EqualTo(Path.GetExtension("C:/tmp/simple_forvard_slash.txt")));
            Assert.That(".txt", Is.EqualTo(Path.GetExtension("C:\\tmp\\simple.with.dot.txt")));
            Assert.That("", Is.EqualTo(Path.GetExtension("C:\\tmp\\simple_without_dot")));
            Assert.That(".txt", Is.EqualTo(Path.GetExtension("C:\\tmp.dot\\simple_with_dot_in_path.txt")));
            Assert.That(".txt", Is.EqualTo(Path.GetExtension("C:\\tmp.dot\\simple.with.dot.evryvere.txt")));
        }

        [Test]
        public void TestPathGetDirectoryName()
        {
            Assert.That("C:\\tmp", Is.EqualTo(Path.GetDirectoryName("C:\\tmp\\simple.txt")));
            Assert.That("C:\\tmp", Is.EqualTo(Path.GetDirectoryName("C:\\tmp\\")));
            Assert.That(null, Is.EqualTo(Path.GetDirectoryName("C:")));
            Assert.That(null, Is.EqualTo(Path.GetDirectoryName("C:\\")));
            Assert.That("", Is.EqualTo(Path.GetDirectoryName("very_simple.txt")));
            Assert.That("tmp", Is.EqualTo(Path.GetDirectoryName("tmp\\simple.with.dot.txt")));
            // .Net throws on Path.GetDirectoryName(""), Java - doesn't.
            Assert.That(null, Is.EqualTo(Path.GetDirectoryName(null)));
            Assert.That("C:\\tmp", Is.EqualTo(Path.GetDirectoryName("C:/tmp/simple_forvard_slash.txt")));
            Assert.That("C:\\tmp", Is.EqualTo(Path.GetDirectoryName("C:/tmp\\simple_forvard_slash.txt")));
            Assert.That("C:\\tmp", Is.EqualTo(Path.GetDirectoryName("C:\\tmp\\simple.with.dot.txt")));
            Assert.That("C:\\tmp", Is.EqualTo(Path.GetDirectoryName("C:\\tmp\\simple_without_dot")));
            Assert.That("C:\\tmp.dot", Is.EqualTo(Path.GetDirectoryName("C:\\tmp.dot\\simple_with_dot_in_path.txt")));
            Assert.That("C:\\tmp.dot", Is.EqualTo(Path.GetDirectoryName("C:\\tmp.dot\\simple.with.dot.evryvere.txt")));
        }

        [Test]
        public void TestPathCombine()
        {
            Assert.That("C:\\tmp\\simple.txt", Is.EqualTo(Path.Combine("C:\\ignored.parent", "C:\\tmp\\simple.txt")));
            Assert.That("very_simple.txt", Is.EqualTo(Path.Combine("", "very_simple.txt")));
            Assert.That("C:\\tmp", Is.EqualTo(Path.Combine("C:\\tmp", "")));
            Assert.That("C:\\tmp\\subdir\\file.txt", Is.EqualTo(Path.Combine("C:\\tmp", "subdir\\file.txt")));
            Assert.That("C:\\tmp.txt\\subdir\\file.txt", Is.EqualTo(Path.Combine("C:\\tmp.txt", "subdir\\file.txt")));
#if JAVA
            //.Net doesn't normalize slash '/' to back slash '\\', but Java does. It is acceptable difference, imho.
            Assert.That("C:\\tmp\\simple_forvard_slash.txt", Is.EqualTo(Path.Combine("C:/tmp", "simple_forvard_slash.txt")));
            Assert.That("C:\\tmp\\simple_forvard_slash.txt", Is.EqualTo(Path.Combine("C:/tmp/", "simple_forvard_slash.txt")));
            Assert.That("C:\\tmp\\other\\different_slashes.txt", Is.EqualTo(Path.Combine("C:\\tmp/other", "different_slashes.txt")));
            //
#else
            Assert.That("C:/tmp\\simple_forvard_slash.txt", Is.EqualTo(Path.Combine("C:/tmp", "simple_forvard_slash.txt")));
            Assert.That("C:/tmp/simple_forvard_slash.txt", Is.EqualTo(Path.Combine("C:/tmp/", "simple_forvard_slash.txt")));
            Assert.That("C:\\tmp/other\\different_slashes.txt", Is.EqualTo(Path.Combine("C:\\tmp/other", "different_slashes.txt")));
#endif
            Assert.That("C:\\tmp\\simple.with.dot.txt", Is.EqualTo(Path.Combine("C:\\tmp", "simple.with.dot.txt")));
            Assert.That("C:\\tmp\\simple.with.dot.txt", Is.EqualTo(Path.Combine("C:\\tmp\\", "simple.with.dot.txt")));

            Assert.That("\\simple.with.dot.txt", Is.EqualTo(Path.Combine("C:\\tmp\\", "\\simple.with.dot.txt")));

            Assert.That("C:\\tmp\\simple_without_dot", Is.EqualTo(Path.Combine("C:\\tmp\\", "simple_without_dot")));
            Assert.That("C:\\tmp.dot\\simple_with_dot_in_path.txt", Is.EqualTo(Path.Combine("C:\\tmp.dot", "simple_with_dot_in_path.txt")));
            Assert.That("C:\\tmp.dot\\simple.with.dot.evryvere.txt", Is.EqualTo(Path.Combine("C:\\tmp.dot", "simple.with.dot.evryvere.txt")));

            Assert.That("C:\\parent\\..\\child\\file.txt", Is.EqualTo(Path.Combine("C:\\parent", "..\\child\\file.txt")));
        }

        [Test]
        public void TestPathIsPathRooted()
        {
            Assert.That(false, Is.EqualTo(Path.IsPathRooted("")));

            Assert.That(true, Is.EqualTo(Path.IsPathRooted("C:")));

            Assert.That(true, Is.EqualTo(Path.IsPathRooted("C:\\")));
            Assert.That(true, Is.EqualTo(Path.IsPathRooted("C:\\tmp\\simple.txt")));
            Assert.That(false, Is.EqualTo(Path.IsPathRooted("very_simple.txt")));
            Assert.That(true, Is.EqualTo(Path.IsPathRooted("C:/tmp/simple_forvard_slash.txt")));
            Assert.That(true, Is.EqualTo(Path.IsPathRooted("C:\\tmp/simple.with.different.slashes.txt")));

            Assert.That(true, Is.EqualTo(Path.IsPathRooted("\\\\tmp\\simple_without_drive")));
            Assert.That(true, Is.EqualTo(Path.IsPathRooted("\\tmp\\simple_without_drive")));

            Assert.That(true, Is.EqualTo(Path.IsPathRooted("//tmp/simple_without_drive")));
            Assert.That(true, Is.EqualTo(Path.IsPathRooted("/tmp/simple_without_drive")));

            Assert.That(false, Is.EqualTo(Path.IsPathRooted("tmp\\simple_without_root.txt")));
            Assert.That(false, Is.EqualTo(Path.IsPathRooted("tmp/simple_without_root.txt")));
        }
    }
}
