// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/12/2018 by Alexey Butalov

using System;
using System.Threading;
using Aspose.JavaAttributes;

namespace Aspose.Words.Tests
{
    /// <summary>
    /// Class for determining hanging situations.
    /// This class is to be ported manually to Java.
    /// </summary>
    [JavaManual("Manual porting by design.")]
    internal class HangingDetector
    {
        private HangingDetector()
        {
            mWaitHandles = new WaitHandle[]
            {
                new AutoResetEvent(false),
                new AutoResetEvent(false)
            };
        }

        /// <summary>
        /// Returns TRUE if loading process hangs.
        /// </summary>
        /// <param name="fileName">Can be relative to the TestData folder or an explicit path.</param>
        /// <param name="timeOut">Timeout in milliseconds </param>
        internal static Document LoadWithTimeoutCore(string fileName, int timeOut)
        {
            HangingDetector detector = new HangingDetector();
            detector.mTimeout = timeOut;
            detector.mFilename = fileName;
            LoadFileContext loadContext = new LoadFileContext();
            loadContext.SyncObj = (AutoResetEvent)detector.mWaitHandles[1];

            ThreadPool.QueueUserWorkItem(new WaitCallback(detector.DoWaitTimeOut),detector.mWaitHandles[0]);
            // Creation of the new thread allows to abort it, when documents loading hangs.
            Thread docLoadThread = new Thread(detector.LoadFile);
            docLoadThread.Start(loadContext);

            int index = WaitHandle.WaitAny(detector.mWaitHandles);
            Document ladedDocument = null;

            if (index == 0)
            {
                // Just inject a "ThreadAbortException" on the thread and do not wait while it will be interrupted.
#if !NET50
                docLoadThread.Abort();
#endif
            }
            else
            {
                ladedDocument = loadContext.LoadedDocument;
            }

            return ladedDocument;
        }

        private void DoWaitTimeOut(Object state)
        {
            AutoResetEvent are = (AutoResetEvent)state;
            Thread.Sleep(mTimeout);
            are.Set();
        }

        private void LoadFile(Object state)
        {
            try
            {
                LoadFileContext context = (LoadFileContext)state;
                context.LoadedDocument = TestUtil.Open(mFilename);
                context.SyncObj.Set();
            }
            catch (ThreadAbortException)
            {
#if !NET50
                Thread.ResetAbort();
#endif
            }
        }

        private int mTimeout;
        private string mFilename;
        private readonly WaitHandle[] mWaitHandles;

        private class LoadFileContext
        {
            public AutoResetEvent SyncObj { get { return mSyncObj; } set { mSyncObj = value; } }

            public Document LoadedDocument { get { return mloadedDocument; } set { mloadedDocument = value; } }

            private AutoResetEvent mSyncObj;
            private Document mloadedDocument;
        }
    }
}
