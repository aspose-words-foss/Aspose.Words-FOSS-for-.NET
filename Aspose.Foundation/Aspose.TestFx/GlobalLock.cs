// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/09/2018 by Alexey Butalov

using System;
using System.Threading;

namespace Aspose.TestFx
{
    /// <summary>
    /// Helps to lock a resource globally.
    /// Usually used to prevent test file locking while tests execution in parallel.
    /// </summary>
    [JavaAttributes.JavaManual]
    public sealed class GlobalLock : IDisposable
    {
        public GlobalLock(string mutexName)
        {
            mMutex = new Mutex(false, mutexName);
            mMutex.WaitOne();
        }

        public void Dispose()
        {
            if (mMutex == null)
                return;

            mMutex.ReleaseMutex();
#if !CPLUSPLUS
            mMutex.Close();
#endif
            mMutex = null;
        }

        private Mutex mMutex;
    }
}
