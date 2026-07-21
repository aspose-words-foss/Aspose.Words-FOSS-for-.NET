// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/03/2014 by Alexey Morozov

using System;
using System.IO;
using Aspose.JavaAttributes;

namespace Aspose.Words.RW.Ole.Moniker
{
    /// <summary>
    /// Base class for all monikers objects.
    /// </summary>
    internal abstract class MonikerBase
    {
        internal static MonikerBase Create(Guid clsId)
        {
            MonikerBase moniker = null;

            if (clsId.Equals(ItemMonikerClsId))
                moniker = new ItemMoniker();
            else if (clsId.Equals(FileMonikerClsId))
                moniker = new FileMoniker();
            else if (clsId.Equals(UrlMonikerClsId))
                moniker = new UrlMoniker();

            return moniker;
        }

        [JavaThrows(true)]
        internal abstract void Read(BinaryReader reader);

        [JavaThrows(true)]
        internal abstract void Write(BinaryWriter write);

        internal abstract Guid ClsId { get; }

        protected static readonly Guid FileMonikerClsId = new Guid("00000303-0000-0000-C000-000000000046");
        protected static readonly Guid ItemMonikerClsId = new Guid("00000304-0000-0000-C000-000000000046");
        protected static readonly Guid UrlMonikerClsId = new Guid("79EAC9E0-BAF9-11CE-8C82-00AA004BA90B");
    }
}
