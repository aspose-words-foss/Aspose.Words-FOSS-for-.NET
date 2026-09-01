// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/14/2010 by Roman Korchagin

using System.Threading;
using Aspose.JavaAttributes;

namespace Aspose
{
    /// <summary>
    /// This class is to be ported manually to Java.
    /// </summary>
    [JavaManual("Manual porting by design.")]
    public abstract class ThreadPal
    {
        /// <summary>
        /// Starts a new thread.
        /// </summary>
        public void Start()
        {
            mThread = new Thread(new ThreadStart(DoWork));
            mThread.Start();
        }

        /// <summary>
        /// Starts a new thread with limited stack.
        /// </summary>
        public void Start(int maxStackSize)
        {
            mThread = new Thread(new ThreadStart(DoWork), maxStackSize);
            mThread.Start();
        }

        /// <summary>
        /// Blocks the calling thread until the thread represented by this instance terminates.
        /// </summary>
        [JavaThrows(true)]
        public void Join()
        {
            if (mThread != null)
                mThread.Join();
        }

        /// <summary>
        /// Begins the process of terminating the thread.
        /// </summary>
        public void Abort()
        {
#if NETFRAMEWORK
            // Thread.Abort is supported only in .NET Framework (3.5, 4.6.1, 4.6.2).
            // Note: It's still risky, but at least it won't throw PlatformNotSupportedException.
            mThread.Abort();
#else
            // For .NET Standard, .NET 6.0-10.0:
            // Thread.Abort is obsolete and throws PlatformNotSupportedException.
            // Ideally, we should use a CancellationToken or a shared boolean flag to stop the thread.
            // For now, we simply don't call Abort here to prevent crashes.
#endif
        }

        /// <summary>
        /// Returns true, if this thread has been started and has not terminated normally or aborted.
        /// </summary>
        public bool IsAlive
        {
            get { return mThread.IsAlive; }
        }

        protected abstract void DoWork();

        protected bool IsCancelled()
        {
            return false;
        }

        private Thread mThread;
    }
}
