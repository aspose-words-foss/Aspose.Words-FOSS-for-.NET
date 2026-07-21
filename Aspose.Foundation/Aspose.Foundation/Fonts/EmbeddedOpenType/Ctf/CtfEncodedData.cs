// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/04/2016 by Ilya Navrotskiy

namespace Aspose.Fonts.EmbeddedOpenType.Ctf
{
    /// <summary>
    /// Contains CTF encoded data.
    /// </summary>
    /// <remarks>
    /// In CTF format all font tables are separated into the three parts: FontTable, PushData and Instructions.
    /// This class represents these three data arrays.
    /// Note, that FontTable part can be used not only for a 'glyph' table, but also for a union of all font tables.
    /// This depends on a place where it is used.
    /// </remarks>
    internal class CtfEncodedData
    {
        /// <summary>
        /// Font table(s) data.
        /// </summary>
        internal byte[] FontTable
        {
            get { return mFontTable; }
            set { mFontTable = value; }
        }

        /// <summary>
        /// Push data.
        /// </summary>
        internal byte[] PushData
        {
            get { return mPushData; }
            set { mPushData = value; }
        }

        /// <summary>
        /// Instructions data.
        /// </summary>
        internal byte[] Instructions
        {
            get { return mInstructions; }
            set { mInstructions = value; }
        }

        private byte[] mFontTable;
        private byte[] mPushData;
        private byte[] mInstructions;
    }
}
