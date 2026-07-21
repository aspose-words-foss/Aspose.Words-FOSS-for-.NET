// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/03/2005 by Roman Korchagin
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Aspose.Ss
{
    /// <summary>
    /// When I read all structured storage into memory, I read it into MemoryStream and MemoryStorage objects.
    /// This represents a directory that can contain other files (streams and storages).
    /// The key is the name of the element.
    /// </summary>
    [ComVisible(false)]
    public class MemoryStorage : SortedList<string, object>
    {
        /// <summary>
        /// Creates a storage with an empty guid.
        /// </summary>
        public MemoryStorage() : this(Guid.Empty)
        {
        }

        /// <summary>
        /// Creates a storage with a specified guid.
        /// </summary>
        public MemoryStorage(Guid clsid) : base(new MemoryStorageNameComparer())
        {
            mClsid = clsid;
        }

        public void Add(string name, MemoryStream stream)
        {
            base.Add(name, stream);
        }

        public void Add(string name, MemoryStorage storage)
        {
            base.Add(name, storage);
        }

        public string GetKey(int index)
        {
            return Keys[index];
        }

        public object GetByIndex(int index)
        {
            return Values[index];
        }

        /// <summary>
        /// Gets a stream by the specified name or null. Sets the stream position to 0.
        /// </summary>
        public MemoryStream GetStreamZeroPositioned(string name)
        {
            object result = this.GetValueOrNull(name);
            MemoryStream memoryStream = result as MemoryStream;
            if (memoryStream != null)
                memoryStream.Position = 0;
            return memoryStream;
        }

        /// <summary>
        /// Gets a stream by the specified name or null.
        /// </summary>
        public MemoryStream GetStreamSafe(string name)
        {
            object result = this.GetValueOrNull(name);
            return result as MemoryStream;
        }

        /// <summary>
        /// Gets a stream by the specified name or throws. Sets the stream position to 0.
        /// </summary>
        public MemoryStream FetchStream(string name)
        {
            MemoryStream result = GetStreamZeroPositioned(name);
            if (result == null)
                throw new InvalidOperationException(string.Format("Cannot find stream '{0}' in the storage.", name));
            return result;
        }

        public MemoryStorage GetStorageSafe(string name)
        {
            object result = this.GetValueOrNull(name);
            return result as MemoryStorage;
        }

        public MemoryStorage FetchStorage(string name)
        {
            MemoryStorage result = GetStorageSafe(name);
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
