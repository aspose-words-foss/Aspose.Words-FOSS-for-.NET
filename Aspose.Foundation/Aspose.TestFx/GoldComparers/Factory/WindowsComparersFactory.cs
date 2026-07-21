// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/12/2016 by Vyacheslav Durin

namespace Aspose.TestFx.GoldComparers.Factory
{
    internal class WindowsComparersFactory : AbstractComparerFactory
    {
        public override AbstractGoldComparer ZipComparer { get { return gZipComparer; } }
        public override AbstractGoldComparer XpsComparer { get { return gXpsComparer; } }
        public override AbstractGoldComparer TextComparer { get { return gTextComparer; } }
        public override AbstractGoldComparer EmfComparer { get { return gEmfComparer; } }
        public override AbstractGoldComparer MhtmlComparer { get { return gMhtmlComparer; } }
        public override AbstractGoldComparer ImageComparer { get { return gImageComparer; } }
        public override AbstractGoldComparer WoffComparer { get { return gWoffComparer; } }
        public override AbstractGoldComparer EpsComparer { get { return gEpsComparer; } }

        // FOSS

        private static readonly AbstractGoldComparer gZipComparer = new ZipFileComparer();
        private static readonly AbstractGoldComparer gXpsComparer = new XpsComparer();
        private static readonly AbstractGoldComparer gTextComparer = new TextFileComparer();
        private static readonly AbstractGoldComparer gEmfComparer = new EmfComparer();
        private static readonly AbstractGoldComparer gMhtmlComparer = new MhtmlFileComparer();
        private static readonly AbstractGoldComparer gImageComparer = new ImageFileComparer();
        private static readonly AbstractGoldComparer gWoffComparer = new WoffComparer();
        private static readonly AbstractGoldComparer gEpsComparer = new EpsComparer();
        // FOSS
    }
}
