// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/05/2017 by Dmitry Sokolov

using Aspose.Words.Lists;

namespace Aspose.Words.Tests.Unified
{
    /// <summary>
    /// This class is for tests purposes only. Current implementation meaningfully adds formatting difference while
    /// an instance cloning. It can be used to determine operations, which generally does not change results. However,
    /// it is necessary to check that these operations were performed or not.
    /// </summary>
    internal class ListDefStub : ListDef
    {
        public ListDefStub(ListDef origListDef) : base(origListDef.Document,
            origListDef.Document.Lists.MakeUniqueListDefId(), origListDef.ListType, origListDef.TemplateCode)
        {
            foreach (ListLevel level in origListDef.Levels.Clone(Document))
                Levels[level.LevelNumber] = level;

            IsRestartAtEachSection = origListDef.IsRestartAtEachSection;
            ListStyleIstd = origListDef.ListStyleIstd;
            Name = origListDef.Name;
        }

        /// <summary>
        /// Clones list definition and changes its left indent.
        /// </summary>
        internal override ListDef Clone(DocumentBase document, int listDefId)
        {
            // Some value that we add to the original ListDef indent
            // to make it different with its clone.
            const int indentDifferenceWithOriginal = 10;

            ListDef clonedListDef = base.Clone(document, listDefId);
            ListLevel level = clonedListDef.Levels.FetchListLevel(0);
            level.ParaPr.LeftIndent += indentDifferenceWithOriginal;

            return clonedListDef;
        }
    }
}
