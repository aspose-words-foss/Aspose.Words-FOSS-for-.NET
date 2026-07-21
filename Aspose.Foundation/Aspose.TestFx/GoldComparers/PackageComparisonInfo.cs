// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/08/2007 by Vladimir Averkin

using System;
using System.IO;
using Aspose.JavaAttributes;

namespace Aspose.TestFx.GoldComparers
{
    public abstract class PackageComparisonInfo : FileComparisonInfo
    {
        public PackageComparisonInfo(
            string filenameSrc,
            string filenameOut,
            string filenameGold,
            string filenameMS)
            : base(
                  filenameSrc,
                  filenameOut,
                  filenameGold,
                  filenameMS)
        {
            mStreamProvider = new FileComparisonStreamProvider(filenameOut, filenameGold, null, filenameMS);
        }

        public PackageComparisonInfo(
            string filenameSrc,
            string filenameOut,
            string filenameGold,
            string filenameOriginalGold,
            string filenameMS)
            : base(
                  filenameSrc,
                  filenameOut,
                  filenameGold,
                  filenameOriginalGold,
                  filenameMS)
        {
            mStreamProvider = new FileComparisonStreamProvider(filenameOut, filenameGold, filenameOriginalGold, filenameMS);
        }

        public PackageComparisonInfo(IComparisonStreamProvider streamProvider)
            : base(
                  null,
                  null,
                  null,
                  null,
                  null)
        {
            mStreamProvider = streamProvider;
        }

        public override void UpdateStatus()
        {
            UpdateStatus(null);
        }

        /// <summary>
        /// Call this to update the comparison info status if using a custom package part comparer.
        /// </summary>
        public void UpdateStatus(IPackagePartComparer partComparer)
        {
            mPartComparisonInfos = new PackagePartComparisonInfoCollection();

            ResetStatus();

            // We need to compare all parts that occur in all three packages: Out, Gold and MS. 
            // For this we load and create a list of all parts found in all three packages.
            // But Gold and/or MS file can be missing and this is allowed.
            Stream streamOut = null;
            Stream streamGold = null;
            Stream streamOriginalGold = null;
            Stream streamMS = null;

            ComparisonPackage packageOut = null;
            ComparisonPackage packageGold = null;
            ComparisonPackage packageOriginalGold = null;
            ComparisonPackage packageMS = null;

            try
            {
                streamOut = mStreamProvider.GetStreamOut();
                if (streamOut != null)
                    packageOut = CreatePackage(streamOut);

                streamGold = mStreamProvider.GetStreamGold();
                if (streamGold != null)
                    packageGold = CreatePackage(streamGold);

                streamOriginalGold = mStreamProvider.GetStreamOriginalGold();
                if (streamOriginalGold != null)
                    packageOriginalGold = CreatePackage(streamOriginalGold);

                streamMS = mStreamProvider.GetStreamMS();
                if (streamMS != null)
                    packageMS = CreatePackage(streamMS);

                // Collect all part comparison infos.
                CollectParts(packageOut, PackageWhatFile.Out);
                CollectParts(packageGold, PackageWhatFile.Gold);
                CollectParts(packageOriginalGold, PackageWhatFile.OriginalGold);
                CollectParts(packageMS, PackageWhatFile.MS);

                // Update comparison status for all parts.
                foreach (PackagePartComparisonInfo partInfo in mPartComparisonInfos)
                    partInfo.UpdateStatus(partComparer);

                if (packageOut != null)
                {
                    mStatusOut = ComparisonStatus.Ok;

                    // Make the overall status Different for the Gold package if there is a problem with at least one part.
                    if (packageGold != null)
                    {
                        mStatusGold = ComparisonStatus.Ok;
                        foreach (PackagePartComparisonInfo partInfo in mPartComparisonInfos)
                        {
                            if (ShouldInvalidatePackage(partInfo.GoldStatus, partInfo.OutStatus))
                            {
                                mStatusGold = ComparisonStatus.Different;
                                break;
                            }
                        }
                    }
                    else
                    {
                        mStatusGold = ComparisonStatus.Missing;
                    }

                    // Make the overall status Different for the 'Original .Net Gold' package if there is a problem with at least one part.
                    if (packageOriginalGold != null)
                    {
                        mStatusOriginalGold = ComparisonStatus.Ok;
                        foreach (PackagePartComparisonInfo partInfo in mPartComparisonInfos)
                        {
                            if (ShouldInvalidatePackage(partInfo.OriginalGoldStatus, partInfo.OutStatus))
                            {
                                mStatusOriginalGold = ComparisonStatus.Different;
                                break;
                            }
                        }
                    }
                    else
                    {
                        mStatusOriginalGold = ComparisonStatus.Missing;
                    }


                    // Make the overall status Different for the MS package if there is a problem with at least one part.
                    if (packageMS != null)
                    {
                        mStatusMS = ComparisonStatus.Ok;
                        foreach (PackagePartComparisonInfo partInfo in mPartComparisonInfos)
                        {
                            if (ShouldInvalidatePackage(partInfo.MSStatus, partInfo.OutStatus))
                            {
                                mStatusMS = ComparisonStatus.Different;
                                break;
                            }
                        }
                    }
                    else
                    {
                        mStatusMS = ComparisonStatus.Missing;
                    }
                }
                else
                {
                    mStatusOut = ComparisonStatus.Missing;
                    mStatusGold = (packageGold != null) ? ComparisonStatus.Different : ComparisonStatus.Missing;
                    mStatusOriginalGold = (packageOriginalGold != null) ? ComparisonStatus.Different : ComparisonStatus.Missing;
                    mStatusMS = (packageMS != null) ? ComparisonStatus.Different : ComparisonStatus.Missing;
                }
            }
            finally
            {
                if (streamOut != null)
                    streamOut.Close();
                if (streamGold != null)
                    streamGold.Close();
                if (streamOriginalGold != null)
                    streamOriginalGold.Close();
                if (streamMS != null)
                    streamMS.Close();
            }
        }

        [JavaThrows(true)]
        protected abstract ComparisonPackage CreatePackage(Stream stream);

        private static bool ShouldInvalidatePackage(ComparisonStatus otherStatus, ComparisonStatus outStatus)
        {
            if (otherStatus != ComparisonStatus.Ok)
            {
                bool bothMissing = ((otherStatus == ComparisonStatus.Missing) && (outStatus == ComparisonStatus.Missing));
                // When both Out and Other parts are missing, it is okay and should not invalidate the whole package.
                // This happens when for example both Out and Gold do not have some part because we don't support it, but MS has it.
                return !bothMissing;
            }
            return false;
        }

        private void CollectParts(ComparisonPackage package, PackageWhatFile whatFile)
        {
            if (package == null)
                return;

            while (package.MoveToNextEntry())
            {
                string partName = package.EntryFileName;

                PackagePartComparisonInfo partComparisonInfo = mPartComparisonInfos[partName];
                if (partComparisonInfo == null)
                {
                    partComparisonInfo = new PackagePartComparisonInfo(partName);
                    mPartComparisonInfos.Add(partComparisonInfo);
                }

                switch (whatFile)
                {
                    case PackageWhatFile.Out:
                        partComparisonInfo.BufferOut = package.LoadEntryToByteArray();
                        break;
                    case PackageWhatFile.Gold:
                        partComparisonInfo.BufferGold = package.LoadEntryToByteArray();
                        break;
                    case PackageWhatFile.OriginalGold:
                        partComparisonInfo.BufferOriginalGold = package.LoadEntryToByteArray();
                        break;
                    case PackageWhatFile.MS:
                        partComparisonInfo.BufferMS = package.LoadEntryToByteArray();
                        break;
                    default:
                        throw new InvalidOperationException("Unexpected enum value.");
                }
            }
        }

        public PackagePartComparisonInfoCollection PartComparisonInfos
        {
            get { return mPartComparisonInfos; }
        }

        private PackagePartComparisonInfoCollection mPartComparisonInfos;
        private readonly IComparisonStreamProvider mStreamProvider;
    }
}
