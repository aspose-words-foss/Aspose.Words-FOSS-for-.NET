// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/12/2016 by Vyacheslav Durin

using Aspose.TestFx.GoldComparers.Format;

namespace Aspose.TestFx.GoldComparers.Factory
{
    internal class RemoteComparersFactory : AbstractComparerFactory
    {
        public override AbstractGoldComparer ZipComparer { get { return gZipComparer; } }
        public override AbstractGoldComparer XpsComparer { get { return gXpsComparer; } }
        public override AbstractGoldComparer TextComparer { get { return gTxtComparer; } }
        public override AbstractGoldComparer EmfComparer { get { return gEmfComparer; } }
        public override AbstractGoldComparer MhtmlComparer { get { return gMhtmlComparer; } }
        public override AbstractGoldComparer ImageComparer { get { return gImgComparer; } }
        public override AbstractGoldComparer WoffComparer { get { return gWoffComparer; } }
        public override AbstractGoldComparer EpsComparer { get { return gEpsComparer; } }

        // FOSS

        private static readonly AbstractGoldComparer gZipComparer = new RemoteComparer(RemoteComparer.DocumentType.Zip);
        private static readonly AbstractGoldComparer gXpsComparer = new RemoteComparer(RemoteComparer.DocumentType.Xps);
        private static readonly AbstractGoldComparer gTxtComparer = new RemoteComparer(RemoteComparer.DocumentType.Txt);
        private static readonly AbstractGoldComparer gEmfComparer = new RemoteComparer(RemoteComparer.DocumentType.Emf);
        private static readonly AbstractGoldComparer gMhtmlComparer = new RemoteComparer(RemoteComparer.DocumentType.Mhtml);
        private static readonly AbstractGoldComparer gImgComparer = new RemoteComparer(RemoteComparer.DocumentType.Img);
        private static readonly AbstractGoldComparer gWoffComparer = new RemoteComparer(RemoteComparer.DocumentType.Woff);
        private static readonly AbstractGoldComparer gEpsComparer = new RemoteComparer(RemoteComparer.DocumentType.Eps);
        // FOSS
    }
}
