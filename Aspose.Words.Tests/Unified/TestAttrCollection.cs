// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using System;
using System.Collections.Generic;
using System.IO;
using Aspose.Drawing;
using Aspose.Words.Revisions;
using Aspose.Words.Tables;
using NUnit.Framework;

namespace Aspose.Words.Tests.Unified
{
    [TestFixture]
    public class TestAttrCollection
    {
        /// <summary>
        /// Test how attributes collection and revision attributes collection are cloned.
        /// </summary>
        [Test]
        public void TestCloneWithRevision()
        {
            //Create the main attribute collection and set the original attribute value.
            RunPr attrs = new RunPr();
            attrs.SetAttr(100, "xxx");

            //Create the revision attributes collection and add the revised attribute value.
            attrs.FormatRevision = new FormatRevision(new RunPr(), "me", new DateTime(2005, 5, 23, 0, 0, 0));
            attrs.FormatRevision.RevPr.SetAttr(100, "yyy");

            //Clone the main attribute collection with all its attributes including revision attributes inside it.
            RunPr clonedAttrs = attrs.Clone();

            //Check the main attribute collection after cloning.
            Assert.That(clonedAttrs != attrs, Is.True);
            Assert.That((string)clonedAttrs.FetchAttr(100), Is.EqualTo("xxx"));

            //Change cloned attributes and check the original is not affected.
            clonedAttrs.SetAttr(101, "ooo");
            Assert.That(attrs.ContainsKey(101), Is.False);

            //Check the revision attribute collection after cloning.
            Assert.That(clonedAttrs.FormatRevision != attrs.FormatRevision, Is.True);
            Assert.That(clonedAttrs.FormatRevision.RevPr != attrs.FormatRevision.RevPr, Is.True);
            Assert.That((string)clonedAttrs.FormatRevision.RevPr.FetchAttr(100), Is.EqualTo("yyy"));

            //Changed cloned revision attributes and check the original is not affected.
            clonedAttrs.FormatRevision.RevPr.SetAttr(101, "ooo");
            Assert.That(attrs.FormatRevision.RevPr.ContainsKey(101), Is.False);
        }

        /// <summary>
        /// Test how attribute collection expands simple attributes to full formatting.
        /// </summary>
        [Test]
        public void TestExpandSimpleAttr()
        {
            //Let's say:
            //srcAttrs are direct attributes specified on the run.
            //dstAttrs is a copy of the attributes specified in the style of the run.
            //
            //By merging srcAttrs into dstAttrs we achieve expansion into full formatting attributes.

            RunPr srcAttrs = new RunPr();
            srcAttrs.SetAttr(101, "srcOnly");        //This only exists in the source, copied to dst.
            srcAttrs.SetAttr(202, "srcOverrides");    //This overrides the attribute in dst.

            RunPr dstAttrs = new RunPr();
            dstAttrs.SetAttr(201, "dstOnly");        //This exists only in dst collection and stays there.
            dstAttrs.SetAttr(202, "dstOverridden");    //This will be overridden by an attribute from the source.

            srcAttrs.ExpandTo(dstAttrs);

            Assert.That(dstAttrs.Count, Is.EqualTo(3));
            Assert.That((string)dstAttrs.GetDirectAttr(101), Is.EqualTo("srcOnly"));
            Assert.That((string)dstAttrs.GetDirectAttr(201), Is.EqualTo("dstOnly"));
            Assert.That((string)dstAttrs.GetDirectAttr(202), Is.EqualTo("srcOverrides"));
        }

        /// <summary>
        /// Test how boolex attribute is expanded.
        /// At the moment it is a simple attribute, but I think I should make it complex.
        /// </summary>
        [Test]
        public void TestExpandBoolEx()
        {
            RunPr srcAttrs = new RunPr();
            srcAttrs.SetAttr(FontAttr.Emboss, AttrBoolEx.True);
            srcAttrs.SetAttr(FontAttr.Italic, AttrBoolEx.Toggle);
            srcAttrs.SetAttr(FontAttr.Bidi, AttrBoolEx.Toggle);
            srcAttrs.SetAttr(FontAttr.Bold, AttrBoolEx.Same);
            srcAttrs.SetAttr(FontAttr.Hidden, AttrBoolEx.Same);
            srcAttrs.SetAttr(FontAttr.AllCaps, AttrBoolEx.False);

            RunPr dstAttrs = new RunPr();
            dstAttrs.SetAttr(FontAttr.Emboss, AttrBoolEx.False);
            dstAttrs.SetAttr(FontAttr.Bidi, AttrBoolEx.True);
            dstAttrs.SetAttr(FontAttr.Hidden, AttrBoolEx.True);

            srcAttrs.ExpandTo(dstAttrs);

            Assert.That(dstAttrs.Count, Is.EqualTo(6));
            Assert.That(dstAttrs.GetDirectAttr(FontAttr.Emboss), Is.EqualTo(AttrBoolEx.True));    //A value was overridden.
            Assert.That(dstAttrs.GetDirectAttr(FontAttr.Italic), Is.EqualTo(AttrBoolEx.True));    //Was toggled from default.
            Assert.That(dstAttrs.GetDirectAttr(FontAttr.Bidi), Is.EqualTo(AttrBoolEx.False));    //Was toggled.
            Assert.That(dstAttrs.GetDirectAttr(FontAttr.Bold), Is.EqualTo(AttrBoolEx.False));    //Default was kept
            Assert.That(dstAttrs.GetDirectAttr(FontAttr.Hidden), Is.EqualTo(AttrBoolEx.True));    //Same value was kept.
            Assert.That(dstAttrs.GetDirectAttr(FontAttr.AllCaps), Is.EqualTo(AttrBoolEx.False));//A value was set.
        }

        private class DummyBorderAttrSource : IBorderAttrSource
        {
            public object GetDirectBorderAttr(int key)
            {
                throw new NotImplementedException();
            }

            public object FetchInheritedBorderAttr(int key)
            {
                throw new NotImplementedException();
            }

            public void SetBorderAttr(int key, object value)
            {
                throw new NotImplementedException();
            }

            public SortedList<BorderType, int> PossibleBorderKeys
            {
                get
                {
                    throw new NotImplementedException();
                }
            }
        }

        /// <summary>
        /// Test expanding border complex attribute.
        /// </summary>
        [Test]
        public void TestExpandBorder()
        {
            RunPr srcAttrs = new RunPr();

            DummyBorderAttrSource parent = new DummyBorderAttrSource(); // expand object lifetime for C++
            Border src101 = new Border(parent, 101);
            src101 = CodePorting.Translator.Cs2Cpp.MemoryManagement.ExtendLifetime(src101, parent);
            srcAttrs.SetAttr(101, src101);    //This is an inherited border.

            Border src102 = new Border(LineStyle.Single, 456, DrColor.Black);
            srcAttrs.SetAttr(102, src102);    //Sets a new border.

            Border src103 = new Border(LineStyle.Single, 789, DrColor.Black);
            srcAttrs.SetAttr(103, src103);

            RunPr dstAttrs = new RunPr();

            Border dst101 = new Border(LineStyle.Single, 123, DrColor.Black);
            dstAttrs.SetAttr(101, dst101);

            Border dst103 = new Border(LineStyle.Single, 999, DrColor.Black);
            dstAttrs.SetAttr(103, dst103);


            srcAttrs.ExpandTo(dstAttrs);

            //Test when source border is in inherited mode it does not override destination.
            Border b = (Border)dstAttrs.GetDirectAttr(101);
            src101 = CodePorting.Translator.Cs2Cpp.MemoryManagement.ExtendLifetime(src101, dstAttrs.GetDirectAttr(101));
            Assert.That(b.RawLineWidth, Is.EqualTo(123));
            Assert.That(dst101, Is.EqualTo(b));        //The destination object stays as is.

            //Test when destination border is missing, it is set to a clone of the source border.
            b = (Border)dstAttrs.GetDirectAttr(102);
            src101 = CodePorting.Translator.Cs2Cpp.MemoryManagement.ExtendLifetime(src101, dstAttrs.GetDirectAttr(102));
            Assert.That(b.RawLineWidth, Is.EqualTo(456));
            Assert.That(b != src102, Is.True);        //The destination object is a clone from the source.

            //Test when source border overrides an existing destination border.
            b = (Border)dstAttrs.GetDirectAttr(103);
            src101 = CodePorting.Translator.Cs2Cpp.MemoryManagement.ExtendLifetime(src101, dstAttrs.GetDirectAttr(103));
            Assert.That(b.RawLineWidth, Is.EqualTo(789));
            Assert.That(b != src103, Is.True);        //This is actually a new object.
            Assert.That(b != dst103, Is.True);
        }

        /// <summary>
        /// Test how tab stops attribute is expanded.
        /// </summary>
        [Test]
        public void TestExpandTabStops()
        {
            RunPr srcAttrs = new RunPr();
            TabStopCollection srcTabStops = new TabStopCollection();
            srcAttrs.SetAttr(101, srcTabStops);

            srcTabStops.Add(new TabStop(1, TabAlignment.Right, TabLeader.Dots));    //This is an added tabstop
            srcTabStops.Add(new TabStop(2, TabAlignment.Center, TabLeader.Dots));//This overrides existing tab stop
            srcTabStops.Add(new TabStop(3, TabAlignment.Clear, TabLeader.None));    //This is a delete tabstop command.

            RunPr dstAttrs = new RunPr();
            TabStopCollection dstTabStops = new TabStopCollection();
            dstAttrs.SetAttr(101, dstTabStops);

            dstTabStops.Add(new TabStop(2, TabAlignment.Left, TabLeader.None));    //Tabstop to be overridden.
            dstTabStops.Add(new TabStop(3, TabAlignment.Left, TabLeader.None));    //Tabstop to be deleted.


            srcAttrs.ExpandTo(dstAttrs);

            TabStopCollection tabStops = (TabStopCollection)dstAttrs.GetDirectAttr(101);

            //Check it is actually a new object.
            Assert.That(tabStops != srcTabStops, Is.True);
            Assert.That(tabStops != dstTabStops, Is.True);

            //Check a tab stop was added.
            Assert.That(tabStops.GetPositionTwipsByIndex(0), Is.EqualTo(1));
            Assert.That(tabStops.GetByPositionTwips(1).Alignment, Is.EqualTo(TabAlignment.Right));

            //Check a tab stop was overridden.
            Assert.That(tabStops.GetByPositionTwips(2).Alignment, Is.EqualTo(TabAlignment.Center));

            // Clear tab stops are left in the model during attribute expansion.
            Assert.That(tabStops.GetByPositionTwips(3).Alignment, Is.EqualTo(TabAlignment.Clear));
        }

        /// <summary>
        /// Test how an expanded attribute collection can be collapsed into a "normal" collection.
        /// This is needed during RTF import.
        /// </summary>
        [Test]
        public void TestCollapseSimple()
        {
            // Imagine we read this style definition from RTF.
            RunPr styleAttrs = new RunPr();
            styleAttrs.SetAttr(FontAttr.Istd, 15);
            styleAttrs.SetAttr(FontAttr.Bold, AttrBoolEx.True);
            styleAttrs.SetAttr(FontAttr.Italic, AttrBoolEx.True);
            styleAttrs.SetAttr(FontAttr.Emboss, AttrBoolEx.True);

            // We also read this run definition from RTF.
            RunPr runAttrs = new RunPr();
            runAttrs.SetAttr(FontAttr.Istd, 15);
            runAttrs.SetAttr(FontAttr.Italic, AttrBoolEx.True);
            runAttrs.SetAttr(FontAttr.Emboss, AttrBoolEx.False);
            runAttrs.SetAttr(FontAttr.Outline, AttrBoolEx.True);

            // This calculates properties that we can store for the run in the model.
            runAttrs.Collapse(styleAttrs, FontAttr.Istd);

            Assert.That(styleAttrs.Count, Is.EqualTo(4));
            Assert.That(runAttrs.Count, Is.EqualTo(4));
            Assert.That(runAttrs.GetDirectAttr(FontAttr.Istd), Is.EqualTo(15));

            // When Bold=true in the style and Bold=missing in the run in RTF, it means Bold=false
            // in the model because there is inheritance in the model, but no inheritance in RTF.
            Assert.That(runAttrs.GetDirectAttr(FontAttr.Bold), Is.EqualTo(AttrBoolEx.False));

            // When Italic=true in the style and Italic=true in the run in RTF, it means Italic=missing
            // in the model because it means that was simply an expanded attribute.

            // When Emboss=true in the style and Emboss=false in the run in RTF, it means Emboss=false
            // in the model because it is an attribute that was specified on the run directly.
            Assert.That(runAttrs.GetDirectAttr(FontAttr.Emboss), Is.EqualTo(AttrBoolEx.False));

            // When Outline=missing in the style Outline=true in the run in RTF, it means Outline=true
            // in the model because it is an attribute that was specified on the run directly.
            Assert.That(runAttrs.GetDirectAttr(FontAttr.Outline), Is.EqualTo(AttrBoolEx.True));
        }

        /// <summary>
        /// For RTF import.
        /// When a style does not have a border, but a run has,
        /// collapsing leaves the border on the run without changes.
        /// </summary>
        [Test]
        public void TestCollapseBorderAdded()
        {
            RunPr styleAttrs = new RunPr();

            RunPr runAttrs = new RunPr();
            runAttrs.SetAttr(FontAttr.Border, new Border(LineStyle.Dot, 8, DrColor.Blue));

            runAttrs.Collapse(styleAttrs);

            Assert.That(styleAttrs.Count, Is.EqualTo(0));
            Assert.That(runAttrs.Count, Is.EqualTo(1));
            Assert.That(runAttrs.Border.Equals(new Border(LineStyle.Dot, 8, DrColor.Blue)), Is.True);
        }

        /// <summary>
        /// For RTF import.
        /// When a style has a border and a run has the same border,
        /// collapsing removes the border from the run.
        /// </summary>
        [Test]
        public void TestCollapseBorderSame()
        {
            RunPr styleAttrs = new RunPr();
            styleAttrs.SetAttr(FontAttr.Border, new Border(LineStyle.Dot, 8, DrColor.Blue));

            RunPr runAttrs = new RunPr();
            runAttrs.SetAttr(FontAttr.Border, new Border(LineStyle.Dot, 8, DrColor.Blue));

            runAttrs.Collapse(styleAttrs);

            Assert.That(styleAttrs.Count, Is.EqualTo(1));
            Assert.That(runAttrs.Count, Is.EqualTo(0));
        }

        /// <summary>
        /// For RTF import.
        /// When a style has a border and a run has a border, but different,
        /// collapsing leaves the border on the run.
        /// </summary>
        [Test]
        public void TestCollapseBorderChanged()
        {
            RunPr styleAttrs = new RunPr();
            styleAttrs.SetAttr(FontAttr.Border, new Border(LineStyle.Dot, 8, DrColor.Blue));

            RunPr runAttrs = new RunPr();
            runAttrs.SetAttr(FontAttr.Border, new Border(LineStyle.Dot, 8, DrColor.Red));

            runAttrs.Collapse(styleAttrs);

            Assert.That(styleAttrs.Count, Is.EqualTo(1));
            Assert.That(styleAttrs.Border.Equals(new Border(LineStyle.Dot, 8, DrColor.Blue)), Is.True);

            Assert.That(runAttrs.Count, Is.EqualTo(1));
            Assert.That(runAttrs.Border.Equals(new Border(LineStyle.Dot, 8, DrColor.Red)), Is.True);
        }

        /// <summary>
        /// For RTF import.
        /// When a style has a border, but the run does not have a border,
        /// collapsing adds an emtpy border to the run.
        /// </summary>
        [Test]
        public void TestCollapseBorderRemoved()
        {
            RunPr styleAttrs = new RunPr();
            styleAttrs.SetAttr(FontAttr.Border, new Border(LineStyle.Dot, 8, DrColor.Blue));

            RunPr runAttrs = new RunPr();

            runAttrs.Collapse(styleAttrs);

            Assert.That(styleAttrs.Count, Is.EqualTo(1));
            Assert.That(styleAttrs.Border.Equals(new Border(LineStyle.Dot, 8, DrColor.Blue)), Is.True);

            Assert.That(runAttrs.Count, Is.EqualTo(1));
            Assert.That(runAttrs.Border.Equals(new Border()), Is.True);

            // It is important that the empty border object on the run is not the global default instance. It must be a copy of it.
            Assert.That(runAttrs.Border != (Border)RunPr.FetchDefaultAttr(FontAttr.Border), Is.True);
        }

        /// <summary>
        /// In RTF, the tyle has no tab stops, but the paragraph has one.
        /// Collapsing leaves paragraph tab stop without changes in the model.
        /// </summary>
        [Test]
        public void TestCollapseTabStopsAdded()
        {
            ParaPr styleAttrs = new ParaPr();

            ParaPr paraAttrs = new ParaPr();
            paraAttrs.SetAttr(ParaAttr.TabStops, new TabStopCollection());
            paraAttrs.TabStops.Add(100, TabAlignment.Left, TabLeader.None);

            paraAttrs.Collapse(styleAttrs);

            Assert.That(styleAttrs.Count, Is.EqualTo(0));

            Assert.That(paraAttrs.Count, Is.EqualTo(1));
            Assert.That(paraAttrs.TabStops.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// In RTF, the style has one tab stop, paragraph has two.
        /// Collapsing leaves only the added tab stop in the paragraph in the model.
        /// </summary>
        [Test]
        public void TestCollapseTabStopAdded()
        {
            ParaPr styleAttrs = new ParaPr();
            styleAttrs.SetAttr(ParaAttr.TabStops, new TabStopCollection());
            styleAttrs.TabStops.Add(100, TabAlignment.Left, TabLeader.None);

            ParaPr paraAttrs = new ParaPr();
            paraAttrs.SetAttr(ParaAttr.TabStops, new TabStopCollection());
            paraAttrs.TabStops.Add(100, TabAlignment.Left, TabLeader.None);
            paraAttrs.TabStops.Add(200, TabAlignment.Left, TabLeader.None);

            paraAttrs.Collapse(styleAttrs);

            Assert.That(styleAttrs.Count, Is.EqualTo(1));
            Assert.That(styleAttrs.TabStops.Count, Is.EqualTo(1));

            Assert.That(paraAttrs.Count, Is.EqualTo(1));
            Assert.That(paraAttrs.TabStops.Count, Is.EqualTo(1));
            Assert.That(paraAttrs.TabStops.GetPositionByIndex(0), Is.EqualTo(200.0));
        }

        /// <summary>
        /// In RTF, the style has two tab stops, the paragraph has only one.
        /// Collapsing leaves only the deleted tab stop in the paragraph in the model.
        /// </summary>
        [Test]
        public void TestCollapseTabStopRemoved()
        {
            ParaPr styleAttrs = new ParaPr();
            styleAttrs.SetAttr(ParaAttr.TabStops, new TabStopCollection());
            styleAttrs.TabStops.Add(100, TabAlignment.Left, TabLeader.None);
            styleAttrs.TabStops.Add(200, TabAlignment.Left, TabLeader.None);

            ParaPr paraAttrs = new ParaPr();
            paraAttrs.SetAttr(ParaAttr.TabStops, new TabStopCollection());
            paraAttrs.TabStops.Add(200, TabAlignment.Left, TabLeader.None);

            paraAttrs.Collapse(styleAttrs);

            Assert.That(styleAttrs.Count, Is.EqualTo(1));
            Assert.That(styleAttrs.TabStops.Count, Is.EqualTo(2));

            Assert.That(paraAttrs.Count, Is.EqualTo(1));
            Assert.That(paraAttrs.TabStops.Count, Is.EqualTo(1));
            Assert.That(paraAttrs.TabStops[0].Equals(new TabStop(100f, TabAlignment.Clear, TabLeader.None)), Is.True);
        }

        /// <summary>
        /// In RTF, the style has two tab stops, the paragraph has no tab stops at all.
        /// Collapsing leaves paragraph with two "clear" tab stops in the model.
        /// </summary>
        [Test]
        public void TestCollapseTabStopsRemoved()
        {
            ParaPr styleAttrs = new ParaPr();
            styleAttrs.SetAttr(ParaAttr.TabStops, new TabStopCollection());
            styleAttrs.TabStops.Add(100, TabAlignment.Left, TabLeader.None);
            styleAttrs.TabStops.Add(200, TabAlignment.Left, TabLeader.None);

            ParaPr paraAttrs = new ParaPr();

            paraAttrs.Collapse(styleAttrs);

            Assert.That(styleAttrs.Count, Is.EqualTo(1));
            Assert.That(styleAttrs.TabStops.Count, Is.EqualTo(2));

            Assert.That(paraAttrs.Count, Is.EqualTo(1));
            Assert.That(paraAttrs.TabStops.Count, Is.EqualTo(2));
            Assert.That(paraAttrs.TabStops[0].Equals(new TabStop(100f, TabAlignment.Clear, TabLeader.None)), Is.True);
            Assert.That(paraAttrs.TabStops[1].Equals(new TabStop(200f, TabAlignment.Clear, TabLeader.None)), Is.True);
        }

        /// <summary>
        /// In RTF, the style has one tab stop, the paragraph has the same.
        /// Collapsing leaves paragraph with no tab stops attribute in the model.
        /// </summary>
        [Test]
        public void TestCollapseTabStopSame()
        {
            ParaPr styleAttrs = new ParaPr();
            styleAttrs.SetAttr(ParaAttr.TabStops, new TabStopCollection());
            styleAttrs.TabStops.Add(100, TabAlignment.Left, TabLeader.None);

            ParaPr paraAttrs = new ParaPr();
            paraAttrs.SetAttr(ParaAttr.TabStops, new TabStopCollection());
            paraAttrs.TabStops.Add(100, TabAlignment.Left, TabLeader.None);

            paraAttrs.Collapse(styleAttrs);

            Assert.That(styleAttrs.Count, Is.EqualTo(1));
            Assert.That(styleAttrs.TabStops.Count, Is.EqualTo(1));

            Assert.That(paraAttrs.Count, Is.EqualTo(0));
        }

        /// <summary>
        /// In RTF, the style has one tab stop, the paragraph has a different tab stop in the same position.
        /// Collapsing leaves the paragraph with it tab stop without changes in the model.
        /// </summary>
        [Test]
        public void TestCollapseTabStopDifferent()
        {
            ParaPr styleAttrs = new ParaPr();
            styleAttrs.SetAttr(ParaAttr.TabStops, new TabStopCollection());
            styleAttrs.TabStops.Add(100, TabAlignment.Left, TabLeader.None);

            ParaPr paraAttrs = new ParaPr();
            paraAttrs.SetAttr(ParaAttr.TabStops, new TabStopCollection());
            paraAttrs.TabStops.Add(100, TabAlignment.Left, TabLeader.Dots);    // DOTS different

            paraAttrs.Collapse(styleAttrs);

            Assert.That(styleAttrs.Count, Is.EqualTo(1));
            Assert.That(styleAttrs.TabStops.Count, Is.EqualTo(1));

            Assert.That(paraAttrs.Count, Is.EqualTo(1));
            Assert.That(paraAttrs.TabStops.Count, Is.EqualTo(1));
            Assert.That(paraAttrs.TabStops[0].Equals(new TabStop(100f, TabAlignment.Left, TabLeader.Dots)), Is.True);
        }

        /// <summary>
        /// Tests comparing collections by the <see cref="AttrCollection.Equals(AttrCollection,AttrCollection,int[])"/>
        /// method.
        /// </summary>
        [Test]
        public void TestEqual()
        {
            ParaPr paraPr1 = new ParaPr();
            ParaPr paraPr2 = new ParaPr();

            // Check empty collections.
            Assert.That(AttrCollection.Equals(paraPr1, paraPr2, null), Is.True);

            // Check simple attributes.
            paraPr1.Bidi = true;
            Assert.That(AttrCollection.Equals(paraPr1, paraPr2, null), Is.False);
            Assert.That(AttrCollection.Equals(paraPr2, paraPr1, null), Is.False);
            Assert.That(AttrCollection.Equals(paraPr1, paraPr2, new int[] { ParaAttr.Bidi }), Is.True);
            paraPr2.Bidi = false;
            Assert.That(AttrCollection.Equals(paraPr1, paraPr2, null), Is.False);
            Assert.That(AttrCollection.Equals(paraPr1, paraPr2, new int[] { ParaAttr.Bidi }), Is.True);
            paraPr2.Bidi = true;
            Assert.That(AttrCollection.Equals(paraPr1, paraPr2, null), Is.True);

            // Check complex attributes.
            Document doc = new Document();
            Style style = doc.Styles[StyleIdentifier.Normal];
            style.ParagraphFormat.Shading.Texture = TextureIndex.Texture20Percent;
            Paragraph para = new Paragraph(doc);

            Shading shading1 = new Shading(para.ParagraphFormat, ParaAttr.Shading);
            shading1 = CodePorting.Translator.Cs2Cpp.MemoryManagement.ExtendLifetime(shading1, para.ParagraphFormat);
            Assert.That(((IComplexAttr)shading1).IsInheritedComplexAttr, Is.True);
            paraPr1.Add(ParaAttr.Shading, shading1);
            Assert.That(AttrCollection.Equals(paraPr1, paraPr2, null), Is.True);
            Assert.That(AttrCollection.Equals(paraPr2, paraPr1, null), Is.True);

            shading1.Texture = TextureIndex.Texture25Percent;
            Assert.That(AttrCollection.Equals(paraPr1, paraPr2, null), Is.False);
            Assert.That(AttrCollection.Equals(paraPr2, paraPr1, null), Is.False);
        }

        /// <summary>
        /// WORDSNET-21593 Investigate SortedShortListIntegerFallback initial capacity.
        /// Initial capacity for font and paragraph properties were reduced.
        /// </summary>
        [Test]
        [CodePorting.Translator.Cs2Cpp.CppSkipEntity("Platform specific implementation")]
        public void Test21593()
        {
            ParaPr paraPr = new ParaPr();
            paraPr[ParaAttr.Alignment] = ParagraphAlignment.Left;

            Assert.That(paraPr.Capacity, Is.EqualTo(2));

            RunPr runPr = new RunPr();
            runPr[FontAttr.Bold] = true;

            Assert.That(runPr.Capacity, Is.EqualTo(4));
        }


        /// <summary>
        /// Gets list of legacy TabStops.
        /// </summary>
        private static List<TabStop> GetLegacyTabStops(ParaPr paraPr)
        {
            TabStopCollection tabStops = (TabStopCollection)paraPr[ParaAttr.TabStops];

            List<TabStop> legacyTabStops = new List<TabStop>();

            if (tabStops != null)
            {
                for (int i = 0; i < tabStops.Count; i++)
                {
                    if (tabStops[i].IsLegacyTab)
                        legacyTabStops.Add(tabStops[i]);
                }
            }

            return legacyTabStops;
        }
    }
}
