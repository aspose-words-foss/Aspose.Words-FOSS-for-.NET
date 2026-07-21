// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/12/2016 by Vyacheslav Durin

namespace Aspose.TestFx.GoldComparers
{
    internal abstract class AbstractComparerFactory
    {
        public abstract AbstractGoldComparer ZipComparer { get; }
        public abstract AbstractGoldComparer XpsComparer { get; }
        public abstract AbstractGoldComparer TextComparer { get; }
        public abstract AbstractGoldComparer EmfComparer { get; }
        public abstract AbstractGoldComparer MhtmlComparer { get; }
        public abstract AbstractGoldComparer ImageComparer { get; }
        public abstract AbstractGoldComparer WoffComparer { get; }
        public abstract AbstractGoldComparer EpsComparer { get; }
        // FOSS
    }
}
