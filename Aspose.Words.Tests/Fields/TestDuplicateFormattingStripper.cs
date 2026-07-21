// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/06/2011 by Dmitry Matveenko

using Aspose.Drawing;
using Aspose.TestFx.Pal;
using Aspose.Words.Fields;

using NUnit.Framework;

namespace Aspose.Words.Tests.Fields
{
    /// <summary>
    /// Test DuplicateFontFormattingStripper. Tests complex attribute equality as stripper relies on it.
    /// </summary>
    [TestFixture]
    public class TestDuplicateFormattingStripper : TestFieldsBase
    {
        [Test]
        public void TestDrColorEquality()
        {
            DrColor color = new DrColor(DrColor.Red.ToArgb());
            CheckEquality(DrColor.Red, color);
        }

        private static void CheckEquality(object source, object clone)
        {
            Assert.That(source.Equals(clone), Is.True);
            Assert.That(source.Equals(null), Is.False);
        }

        [Test]
        public void TestBorderEquality()
        {
            Border border = new Border(LineStyle.DashDotStroker, 2, DrColor.YellowGreen);
            Border clone = border.Clone();
            CheckEquality(border, clone);
        }

        [Test]
        public void TestShadingEquality()
        {
            Shading shading = MakeTestShading();
            Shading clone = MakeTestShading();
            CheckEquality(shading, clone);
        }

        private static Shading MakeTestShading()
        {
            Shading shading = new Shading();
            shading.Texture = TextureIndex.TextureSolid;
            shading.ThemeColor = "black";
            shading.ThemeTint = "tint";
            shading.ForegroundPatternColorInternal = new DrColor(DrColor.Brown.ToArgb());
            shading.BackgroundPatternColorInternal = new DrColor(DrColor.Blue.ToArgb());

            return shading;
        }

        [Test]
        public void TestDuplicateFormattingStripping()
        {
            const string fileName = @"Fields\Format\TestDuplicateFormattingStripping.docx";
            Document doc = TestUtil.Open(fileName);

            TestFontAttribute[] fontAttributes = GetAllFontAttributes();

            // Check that direct formatting was not present in the original document.
            CheckDirectFormattingStripped(doc, "Direct formatting is present in the original document.");

            DuplicateAttrsToDirectFormatting(doc, fontAttributes);

            // Strip duplicated formatting.
            TocNormalEntryAttributeModifier.StripInlineFontAttrsForToc(doc);

            // Check that direct formatting was stripped from testDoc
            CheckDirectFormattingStripped(doc, "Direct formatting remained after stripping.");

            // Check that all attributes are tested.
            CheckAllAttributesTested(fontAttributes);
        }

        private static void DuplicateAttrsToDirectFormatting(Document doc, TestFontAttribute[] fontAttributes)
        {
            NodeCollection nodes = doc.GetChildNodes(NodeType.Any, true);
            foreach (Node node in nodes)
            {
                Inline inlineNode = node as Inline;
                if (inlineNode != null)
                {
                    DuplicateAttrsToDirectFormatting(inlineNode, fontAttributes);
                }
            }
        }

        private static void DuplicateAttrsToDirectFormatting(Inline inlineNode, TestFontAttribute[] fontAttributes)
        {
            foreach (TestFontAttribute testAttr in fontAttributes)
            {
                if (!IgnoreAttrForStripTest(testAttr.Key))
                {
                    object attr = inlineNode.Font.FetchAttr(testAttr.Key);

                    if (attr != null)
                    {
                        // Clone the attribute to check equality better.
                        object clonedAttribute = CloneAttribute(attr);

                        // Set the same attribute directly.
                        inlineNode.RunPr.SetAttr(testAttr.Key, clonedAttribute);

                        // Remember the attribute is checked:
                        testAttr.CheckState = AttrCheckState.Checked;
                    }
                }
            }
        }

        private static object CloneAttribute(object attr)
        {
            IComplexAttr complexAttr = attr as IComplexAttr;
            if (complexAttr != null)
                return complexAttr.DeepCloneComplexAttr();

            DrColor color = attr as DrColor;
            if (color != null)
                return new DrColor(color.ToArgb());

            return attr;
        }

        private static void CheckDirectFormattingStripped(Document doc, string context)
        {
            NodeCollection nodes = doc.GetChildNodes(NodeType.Any, true);
            foreach (Node node in nodes)
            {
                if (!(node is Inline))
                    continue;

                Inline inlineNode = (Inline)node;

                // List not stripped attributes.
                int unstrippedAttrCount = 0;
                for (int attrIdx = 0; attrIdx < inlineNode.RunPr.Count; ++attrIdx)
                {
                    int attrKey = inlineNode.RunPr.GetKey(attrIdx);
                    object attrValue = inlineNode.RunPr[attrKey];

                    if (!IgnoreAttrForStripTest(attrKey))
                    {
                        Debug.WriteLine(string.Format("Direct Formatting Attribute is present: {0} : {1}", attrKey, attrValue));
                        unstrippedAttrCount++;
                    }
                }

                Assert.That(unstrippedAttrCount, Is.EqualTo(0), context);
            }
        }

        private static bool IgnoreAttrForStripTest(int attr)
        {
            return (ArrayUtil.BinarySearch(gIgnoredAttributes, 0, gIgnoredAttributes.Length, attr) >= 0);
        }

        private static void CheckAllAttributesTested(TestFontAttribute[] fontAttributes)
        {
            Debug.WriteLine("Not tested attributes:");

            int attrCount = 0;
            // List not tested attributes.
            foreach (TestFontAttribute testAttr in fontAttributes)
            {
                if (testAttr.CheckState == AttrCheckState.NotChecked &&
                    !IgnoreAttrForStripTest(testAttr.Key))
                {
                    Debug.WriteLine(testAttr.Name);
                    ++attrCount;
                }
            }

            Debug.WriteLine(string.Format("{0} attributes not tested.", attrCount));
            Assert.That(attrCount, Is.EqualTo(0));
        }

        [Test]
        public void TestIgonredAttributesOrder()
        {
            int previousAttr = gIgnoredAttributes[0];
            for (int attrIdx = 1; attrIdx < gIgnoredAttributes.Length; ++attrIdx)
            {
                int currentAttr = gIgnoredAttributes[attrIdx];
                Assert.That(currentAttr > previousAttr, Is.True, string.Format("Attributes are not ordered, at idx {0}.", attrIdx));
                previousAttr = currentAttr;
            }
        }

        private static TestFontAttribute[] GetAllFontAttributes()
        {
#if !CPLUSPLUS
            int[] attrValues = TestUtilPal.GetIntConstantValues(typeof(FontAttr));
#else
            int[] attrValues = gFontAttributes;
#endif
            TestFontAttribute[] fontAttributes = new TestFontAttribute[attrValues.Length];

            for (int attrIdx = 0; attrIdx < attrValues.Length; attrIdx++)
            {
                fontAttributes[attrIdx] = new TestFontAttribute(null, attrValues[attrIdx]);
            }

            return fontAttributes;
        }

        private enum AttrCheckState
        {
            NotChecked,
            Checked
        }

        private class TestFontAttribute
        {
            public TestFontAttribute(string name, int attrKey)
            {
                Name = name;
                Key = attrKey;
                CheckState = AttrCheckState.NotChecked;
            }

            public readonly string Name;
            public readonly int Key;
            public AttrCheckState CheckState;
        }

        // Must be ordered or binary search will fail.
        private static readonly int[] gIgnoredAttributes =
        {
            // These presumably cannot be inherited from a style or default.
            RevisionAttr.DeleteRevision, // 12
            RevisionAttr.InsertRevision, // 14
            FontAttr.SysAttrs1Trigger, // 16
            FontAttr.RsidRPr, // 30
            FontAttr.RsidR,  // 40
            FontAttr.Istd, // 50
            FontAttr.SysAttrs2Trigger, // 202
            FontAttr.WordXPAttrsTrigger, // 402
            FontAttr.PictureBulletId, // 480
            FontAttr.PictureBulletFlags, // 490

            // This one I don't know what for, but it probably cannot be inherited either.
            FontAttr.CFMathPr, // 600

            // Adding one of these caused different run type. It seems they don't make sense for a regular run.
            FontAttr.MathIsLiteral,
            FontAttr.MathIsNormalText, // 710
            FontAttr.MathScript, // 720
            FontAttr.MathStyle, // 730
            FontAttr.MathLineBreak, // 740
            FontAttr.MathIsAlignmentPoint, // 750
            FontAttr.MathIsOMath, // 760
            FontAttr.FarEastLayout, // 780

            FontAttr.AlternateContent, // 790

            FontAttr.EffectGlow,                    // 810
            FontAttr.EffectShadow,                  // 815
            FontAttr.EffectReflection,              // 820
            FontAttr.EffectOutline,                 // 825
            FontAttr.EffectFill,                    // 830
            FontAttr.EffectScene3D,                 // 835
            FontAttr.EffectProps3D,                 // 840
            FontAttr.OpenTypeLigature,              // 850
            FontAttr.OpenTypeNumForm,               // 855
            FontAttr.OpenTypeNumSpacing,            // 860
            FontAttr.OpenTypeStylisticSets,         // 865
            FontAttr.OpenTypeContextualAlternates,  // 870
            FontAttr.FitText,                       // 880
            FontAttr.Ruby,                          // 885

            FontAttr.Sys_Symbol                    // 890
        };

#if CPLUSPLUS
        private static readonly int[] gFontAttributes =
            {
            FontAttr.SpecialHidden,
            FontAttr.SysAttrs1Trigger,
            FontAttr.HighlightColor,
            FontAttr.RsidRPr,
            FontAttr.RsidR,
            FontAttr.LineBreakClear,
            FontAttr.Istd,
            FontAttr.Bold,
            FontAttr.Italic,
            FontAttr.StrikeThrough,
            FontAttr.Outline,
            FontAttr.Shadow,
            FontAttr.SmallCaps,
            FontAttr.AllCaps,
            FontAttr.Hidden,
            FontAttr.WebHidden,
            FontAttr.Underline,
            FontAttr.Spacing,
            FontAttr.Color,
            FontAttr.Emboss,
            FontAttr.Engrave,
            FontAttr.Size,
            FontAttr.Position,
            FontAttr.SysAttrs2Trigger,
            FontAttr.VerticalAlignment,
            FontAttr.Kerning,
            FontAttr.NameAscii,
            FontAttr.NameFarEast,
            FontAttr.NameOther,
            FontAttr.BoldBi,
            FontAttr.ItalicBi,
            FontAttr.Bidi,
            FontAttr.ComplexScript,
            FontAttr.NameBi,
            FontAttr.Scaling,
            FontAttr.DoubleStrikeThrough,
            FontAttr.TextEffect,
            FontAttr.SnapToGrid,
            FontAttr.LocaleIdBi,
            FontAttr.SizeBi,
            FontAttr.Border,
            FontAttr.Shading,
            FontAttr.LocaleId,
            FontAttr.LocaleIdFarEast,
            FontAttr.CharacterCategoryHint,
            FontAttr.WordXPAttrsTrigger,
            FontAttr.NoProofing,
            FontAttr.UnderlineColor,
            FontAttr.HyphenRule,
            FontAttr.HyphenChar,
            FontAttr.PictureBulletId,
            FontAttr.PictureBulletFlags,
            FontAttr.ThemeColor,
            FontAttr.ThemeShade,
            FontAttr.ThemeTint,
            FontAttr.UnderlineThemeColor,
            FontAttr.UnderlineThemeShade,
            FontAttr.UnderlineThemeTint,
            FontAttr.CFMathPr,
            FontAttr.MathIsLiteral,
            FontAttr.MathIsNormalText,
            FontAttr.MathScript,
            FontAttr.MathStyle,
            FontAttr.MathLineBreak,
            FontAttr.MathIsAlignmentPoint,
            FontAttr.MathIsOMath,
            FontAttr.EmphasisMark,
            FontAttr.FarEastLayout,
            FontAttr.AlternateContent,
            FontAttr.EffectGlow,
            FontAttr.EffectShadow,
            FontAttr.EffectReflection,
            FontAttr.EffectOutline,
            FontAttr.EffectFill,
            FontAttr.EffectScene3D,
            FontAttr.EffectProps3D,
            FontAttr.OpenTypeLigature,
            FontAttr.OpenTypeNumForm,
            FontAttr.OpenTypeNumSpacing,
            FontAttr.OpenTypeStylisticSets,
            FontAttr.OpenTypeContextualAlternates,
            FontAttr.FitText,
            FontAttr.Ruby,
            FontAttr.Sys_Symbol
        };
#endif
    }
}
