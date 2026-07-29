// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/11/2010 by Alexey Morozov
using System;
using System.Collections.Generic;
using System.IO;

namespace Aspose.IO
{
    public class TempFiler
    {
        /// <summary>
        /// Holds information about temporary files were created.
        /// It is internal class I think it's better to place it here.
        /// </summary>
        private class TempFile
        {
            /// <summary>
            /// Ctor.
            /// </summary>
            /// <param name="stream"></param>
            /// <param name="filename"></param>
            internal TempFile(FileStream stream, string filename)
            {
                mStream = stream;
                mFilename = filename;
            }

            public FileStream Stream
            {
               get { return mStream;}
            }

            public string Filename
            {
                get { return mFilename; }
            }

            private readonly FileStream mStream;
            private readonly string mFilename;
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="saveInfo"></param>
        public TempFiler(string tempFolder)
        {
            mTempFolder = tempFolder;
        }

        /// <summary>
        /// Creates either MemoryStream or FileStream depending on <see cref="SaveOptions.TempFolder"/> or
        /// <see cref="LoadOptions.TempFolder"/> setting. 
        /// Returned stream will be used as temporary stream when reading or writing doc.
        /// </summary>
        public Stream GetNewStream()
        {
            Stream stream;

            if (IsTempFilesEnabled)
            {
                // Temporary file is required for stream.
                string fileName = Path.Combine(mTempFolder, String.Format("{0}.tmp", Guid.NewGuid()));
                stream = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite);
                mTempFiles.Add(new TempFile((FileStream)stream, fileName));
            }
            else
            {
                // Temporary files are not required so create in memory stream.
                stream = new MemoryStream();
            }

            return stream;
        }

        /// <summary>
        /// Delete temporary files if they were created.
        /// </summary>
        /// <remarks>
        /// I thought about Finalize but decided that better if we can control 
        /// deletetion of temporary files itself.
        /// </remarks>
        public void Cleanup()
        {
            foreach(TempFile tempFile in mTempFiles)
            {
                tempFile.Stream.Close();
                File.Delete(tempFile.Filename);
            }
        }

        /// <summary>
        /// Indicates that TempFiler will return FileStream object instead of MemoryStream.
        /// </summary>
        public bool IsTempFilesEnabled
        {
            get { return StringUtil.HasChars(mTempFolder); }
        }

        /// <summary>
        /// Holds temporary files created to able to delete it.
        /// </summary>
        private readonly List<TempFile> mTempFiles = new List<TempFile>();
        private readonly string mTempFolder;
    }
}
