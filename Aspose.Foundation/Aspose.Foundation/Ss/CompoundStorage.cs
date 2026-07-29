// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/06/2016 by Alexey Morozov

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Aspose.Ss
{
    /// <summary>
    /// Implements structured storage.
    /// </summary>
    /// <remarks>
    /// This almost duplicates <see cref="FileSystem"/> class but deals with <see cref="CompoundStream"/> class 
    /// instead of MemoryStream.
    /// </remarks>
    [ComVisible(false)]
    public class CompoundStorage : SortedList<string, object>
    {
        /// <summary>
        /// Creates a storage with an empty guid.
        /// </summary>
        public CompoundStorage()
            : this(Guid.Empty)
        {
        }

        /// <summary>
        /// Creates a storage with a specified guid.
        /// </summary>
        public CompoundStorage(Guid clsid) 
            : base(new MemoryStorageNameComparer())
        {
            mClsid = clsid;
        }

        public void Add(string name, CompoundStream stream)
        {
            base.Add(name, stream);
        }

        public void Add(string name, CompoundStorage storage)
        {
            base.Add(name, storage);
        }

        /// <summary>
        /// Gets a stream by the specified name or null. Sets the stream position to 0.
        /// </summary>
        public CompoundStream GetStream(string name)
        {
            object result = this.GetValueOrNull(name);
            CompoundStream compoundStream = result as CompoundStream;
            if (compoundStream != null)
                compoundStream.Position = 0;
            return compoundStream;
        }

        /// <summary>
        /// Gets a stream by the specified name or throws. Sets the stream position to 0.
        /// </summary>
        public CompoundStream FetchStream(string name)
        {
            CompoundStream result = GetStream(name);
            if (result == null)
                throw new InvalidOperationException(string.Format("Cannot find stream '{0}' in the storage.", name));
            return result;
        }

        public CompoundStorage GetStorage(string name)
        {
            object result = this.GetValueOrNull(name);
            return result as CompoundStorage;
        }

        public CompoundStorage FetchStorage(string name)
        {
            CompoundStorage result = GetStorage(name);
            if (result == null)
                throw new InvalidOperationException(string.Format("Cannot find storage '{0}'.", name));
            return result;
        }

        /// <summary>
        /// A storage can optionally have a clsid.
        /// </summary>
        public Guid Clsid
        {
            get { return mClsid; }
            set { mClsid = value; }
        }

        private Guid mClsid = Guid.Empty;    // Must be initialized for Java to work.
    }
}
