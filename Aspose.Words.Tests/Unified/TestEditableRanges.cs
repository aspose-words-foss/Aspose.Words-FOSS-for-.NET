// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/06/2013 by Andrey Noskov

using System;
using NUnit.Framework;

namespace Aspose.Words.Tests.Unified
{
    /// <summary>
    /// Unified tests for editable ranges.
    /// </summary>
    public class TestEditableRanges : UnifiedTestsBase
    {
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestEditableRanges(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\EditableRange\EditableRanges", GetNoGoldScenario(lf, sf));

            NodeCollection starts = doc.GetChildNodes(NodeType.EditableRangeStart, true);
            NodeCollection ends = doc.GetChildNodes(NodeType.EditableRangeEnd, true);

            // Check editable ranges are present.
            Assert.That(starts.Count, Is.EqualTo(2));
            Assert.That(ends.Count, Is.EqualTo(2));

            // Check editable ranges have Ids.
            EditableRangeStart prStart = (EditableRangeStart)starts[0];
            Assert.That(prStart.Id, Is.EqualTo(0));
            Assert.That(prStart.NextSibling.GetText().IndexOf("starts in this") != -1, Is.True);

            // Check single user for editable range is specified but editor group is not.
            Assert.That(prStart.SingleUser, Is.EqualTo("WEB1\\Andrey"));
            Assert.That(prStart.EditorGroup, Is.EqualTo(EditorType.Unspecified));
            Assert.That(prStart.PreviousSibling is Run, Is.True);
            Assert.That(prStart.NextSibling is Run, Is.True);

            EditableRangeEnd prEnd = (EditableRangeEnd)ends[0];
            Assert.That(prEnd.Id, Is.EqualTo(1));
            Assert.That(prEnd.NextSibling.GetText().IndexOf("and another") != -1, Is.True);

            // This editable range is inside editable range 1.
            prStart = (EditableRangeStart)starts[1];
            Assert.That(prStart.Id, Is.EqualTo(1));
            Assert.That(prStart.SingleUser, Is.EqualTo(""));
            Assert.That(prStart.EditorGroup, Is.EqualTo(EditorType.Everyone));
            Assert.That(prStart.NextSibling.GetText(), Is.EqualTo(" paragraph "));
        }

        /// <summary>
        /// Test several editable ranges start and end in the same position.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestEditableRangesSamePosition(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\EditableRange\TestEditableRangesSamePosition", GetNoGoldScenario(lf, sf));

            NodeCollection starts = doc.GetChildNodes(NodeType.EditableRangeStart, true);
            NodeCollection ends = doc.GetChildNodes(NodeType.EditableRangeEnd, true);
            
            // Check editable range are present.
            Assert.That(starts.Count, Is.EqualTo(3));
            Assert.That(ends.Count, Is.EqualTo(3));

            EditableRangeStart prStart1 = (EditableRangeStart)starts[0];
            Assert.That(prStart1.Id, Is.EqualTo(0));
            EditableRangeStart prStart2 = (EditableRangeStart)starts[1];
            Assert.That(prStart2.Id, Is.EqualTo(1));

            // Start of editableRange1 is right after editableRange2.
            Assert.That(prStart1.NextSibling == prStart2, Is.True);
            Assert.That(prStart2.NextSibling.GetText().IndexOf("Section1") != -1, Is.True);

            EditableRangeEnd prEnd2 = (EditableRangeEnd)ends[1];
            Assert.That(prEnd2.Id, Is.EqualTo(1));
            EditableRangeEnd prEnd3 = (EditableRangeEnd)ends[2];
            Assert.That(prEnd3.Id, Is.EqualTo(2));

            // End of editableRange3 is right after end of editableRange1.
            Assert.That(prEnd2.NextSibling == prEnd3, Is.True);
            Assert.That(prEnd3.NextSibling == null, Is.True);
        }

        /// <summary>
        /// Removes a section so only editable range end is available, should work okay and remove the editable range.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestEditableRangeCutStart(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.Open(@"Model\EditableRange\TestEditableRangesSamePosition", lf);
            doc.Sections.RemoveAt(0);

            doc = TestUtil.SaveOpen(doc, @"Model\EditableRange\TestEditableRangeCutStart", GetNoGoldScenario(lf, sf));

            NodeCollection starts = doc.GetChildNodes(NodeType.EditableRangeStart, true);
            NodeCollection ends = doc.GetChildNodes(NodeType.EditableRangeEnd, true);

            //Only one editable range now is left.
            Assert.That(starts.Count, Is.EqualTo(1));
            Assert.That(ends.Count, Is.EqualTo(1));

            EditableRangeStart prStart = (EditableRangeStart)starts[0];
            Assert.That(prStart.Id, Is.EqualTo(0));
        }

        /// <summary>
        /// Removes a section so only editable range start is available, should work okay and remove the editable range.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestEditableRangeCutEnd(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.Open(@"Model\EditableRange\TestEditableRangesSamePosition", lf);
            doc.Sections.RemoveAt(1);

            doc = TestUtil.SaveOpen(doc, @"Model\EditableRange\TestEditableRangeCutEnd", GetNoGoldScenario(lf, sf));

            NodeCollection starts = doc.GetChildNodes(NodeType.EditableRangeStart, true);
            NodeCollection ends = doc.GetChildNodes(NodeType.EditableRangeEnd, true);

            // Only one editable range now is left.
            Assert.That(starts.Count, Is.EqualTo(1));
            Assert.That(ends.Count, Is.EqualTo(1));

            EditableRangeStart prStart = (EditableRangeStart)starts[0];
            Assert.That(prStart.Id, Is.EqualTo(0));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestEditableRangeDuplicate(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.Open(@"Model\EditableRange\TestEditableRangesSamePosition", lf);

            // Adds a copy of the last section to the document.
            doc.FirstSection.AppendContent(doc.Sections[1].Clone());

            doc = TestUtil.SaveOpen(doc, @"Model\EditableRange\TestEditableRangeDuplicate", GetNoGoldScenario(lf, sf));

            NodeCollection starts = doc.GetChildNodes(NodeType.EditableRangeStart, true);
            NodeCollection ends = doc.GetChildNodes(NodeType.EditableRangeEnd, true);

            Assert.That(starts.Count, Is.EqualTo(4));
            Assert.That(ends.Count, Is.EqualTo(4));

            EditableRangeStart prStart1 = (EditableRangeStart)starts[0];
            Assert.That(prStart1.Id, Is.EqualTo(0));
            Assert.That(prStart1.SingleUser, Is.EqualTo("andrey@office.net.com"));

            EditableRangeStart prStart2 = (EditableRangeStart)starts[1];
            Assert.That(prStart2.Id, Is.EqualTo(1));
            Assert.That(prStart2.SingleUser, Is.EqualTo("DnsAdmins@office.net.com"));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestEditableRangeSetEditor(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\EditableRange\TestEditableRangesSamePosition", GetNoGoldScenario(lf, sf));

            EditableRangeStart start = (EditableRangeStart)doc.GetChild(NodeType.EditableRangeStart, 0, true);
            EditableRange editableRange = start.EditableRange;
            Assert.That(editableRange.SingleUser, Is.EqualTo("andrey@office.net.com"));
            Assert.That(editableRange.EditorGroup, Is.EqualTo(EditorType.Unspecified));
            start.EditableRange.SingleUser = "Andrey@aspose.com";

            doc = TestUtil.SaveOpen(doc, @"Model\EditableRange\TestEditableRangeSetEditor", GetNoGoldScenario(lf, sf));

            start = (EditableRangeStart)doc.GetChild(NodeType.EditableRangeStart, 0, true);
            editableRange = start.EditableRange;
            Assert.That(editableRange.SingleUser, Is.EqualTo("Andrey@aspose.com"));

            // Single user and editor group cannot be set simultaneously for the specific editable range,
            // if the one is set, the other will be clear.
            editableRange.EditorGroup = EditorType.Administrators;
            Assert.That(editableRange.SingleUser, Is.Empty);
            Assert.That(editableRange.EditorGroup, Is.EqualTo(EditorType.Administrators));
        }

        /// <summary>
        /// Test editable ranges start and end inside/outside table.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestTableColumnEditableRange(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\EditableRange\TestTableColumnEditableRange", GetNoGoldScenario(lf, sf));

            NodeCollection starts = doc.GetChildNodes(NodeType.EditableRangeStart, true);
            NodeCollection ends = doc.GetChildNodes(NodeType.EditableRangeEnd, true);

            // Check editable range are present.
            Assert.That(starts.Count, Is.EqualTo(17));
            Assert.That(ends.Count, Is.EqualTo(17));

            EditableRangeStart prStart1 = (EditableRangeStart)starts[0];
            Assert.That(prStart1.Id, Is.EqualTo(0));
            Assert.That(prStart1.EditorGroup, Is.EqualTo(EditorType.Everyone));

            // Check FirstColumn/LastColumn are correct.
            Assert.That(prStart1.FirstColumn, Is.EqualTo(1));
            Assert.That(prStart1.LastColumn, Is.EqualTo(1));

            EditableRangeStart prStart2 = (EditableRangeStart)starts[1];
            Assert.That(prStart2.Id, Is.EqualTo(1));
            Assert.That(prStart2.EditorGroup, Is.EqualTo(EditorType.Everyone));
            Assert.That(prStart2.FirstColumn, Is.EqualTo(2));
            Assert.That(prStart2.LastColumn, Is.EqualTo(2));
        }

        /// <summary>
        /// Test removing editable range.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestEditableRangeRemove(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\EditableRange\TestTableColumnEditableRange", GetNoGoldScenario(lf, sf));

            NodeCollection starts = doc.GetChildNodes(NodeType.EditableRangeStart, true);
            NodeCollection ends = doc.GetChildNodes(NodeType.EditableRangeEnd, true);

            // Check editable range are present.
            Assert.That(starts.Count, Is.EqualTo(17));
            Assert.That(ends.Count, Is.EqualTo(17));

            EditableRange editableRange1 = ((EditableRangeStart)starts[0]).EditableRange;
            Assert.That(editableRange1.Id, Is.EqualTo(0));
            editableRange1.Remove();

            starts = doc.GetChildNodes(NodeType.EditableRangeStart, true);
            ends = doc.GetChildNodes(NodeType.EditableRangeEnd, true);

            Assert.That(starts.Count, Is.EqualTo(16));
            Assert.That(ends.Count, Is.EqualTo(16));
        }

        /// <summary>
        /// Test editable range inside OfficeMath.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestEditableRangeOfficeMath(LoadFormat lf, SaveFormat sf)
        {
            switch (TestUtil.GetUnifiedScenario(lf, sf))
            {
                case UnifiedScenario.Doc2Doc:
                case UnifiedScenario.Doc2Rtf:
                case UnifiedScenario.Doc2Wml:
                case UnifiedScenario.Wml2WmlNoGold:
                case UnifiedScenario.Doc2Docx:
                    break;
                case UnifiedScenario.Rtf2RtfNoGold:
                case UnifiedScenario.Rtf2Rtf:
                case UnifiedScenario.Docx2DocxNoGold:
                case UnifiedScenario.Docx2Docx:
                {
                    Document doc = TestUtil.OpenSaveOpen(@"Model\EditableRange\UnifiedTestEditableRangeOfficeMath", GetNoGoldScenario(lf, sf));

                    NodeCollection starts = doc.GetChildNodes(NodeType.EditableRangeStart, true);
                    NodeCollection ends = doc.GetChildNodes(NodeType.EditableRangeEnd, true);

                    // Check editable range are present.
                    Assert.That(starts.Count, Is.EqualTo(3));
                    Assert.That(ends.Count, Is.EqualTo(3));

                    EditableRange editableRange1 = ((EditableRangeStart)starts[0]).EditableRange;

                    // Check parent node is OfficeMath.
                    Assert.That(editableRange1.EditableRangeStart.ParentNode.NodeType, Is.EqualTo(NodeType.OfficeMath));
                    Assert.That(editableRange1.EditableRangeEnd.ParentNode.NodeType, Is.EqualTo(NodeType.OfficeMath));
                    Assert.That(editableRange1.Id, Is.EqualTo(0));
                    Assert.That(editableRange1.EditorGroup, Is.EqualTo(EditorType.Everyone));
                    Assert.That(editableRange1.SingleUser, Is.Empty);
                    break;
                }
                default:
                    throw new InvalidOperationException("Unknown file format.");
            }
        }

        private static UnifiedScenario GetNoGoldScenario(LoadFormat lf, SaveFormat sf)
        {
            return TestUtil.GetUnifiedScenario(lf, sf) | UnifiedScenario.NoGold;
        }
    }
}
