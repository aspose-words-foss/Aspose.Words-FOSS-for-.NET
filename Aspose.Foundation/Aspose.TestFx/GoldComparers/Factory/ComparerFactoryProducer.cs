// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/12/2016 by Vyacheslav Durin

namespace Aspose.TestFx.GoldComparers.Factory
{
    internal static class ComparerFactoryProducer
    {
        private static readonly AbstractComparerFactory gWinFactory = new WindowsComparersFactory();
        private static readonly AbstractComparerFactory gRemoteFactory = new RemoteComparersFactory();

        public static AbstractComparerFactory Factory
        {
            get
            {
                // Use remote comparer for Unix like environments (Mac, Android, iOS, Ubuntu etc).
                return PlatformUtilPal.IsUnixLike() ? gRemoteFactory : gWinFactory;
            }
        }
    }
}
