// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.

//
// NOTE This source file is included by link from
// X:/awnet/Aspose.Foundation/Tools/XpsToPng
// X:/awnet/Aspose.Foundation/Tools/WebToPng
//
// DO NOT add using directives for Aspose namespaces
//

using System;
using System.Collections.Concurrent;
using System.Threading;
using Aspose.JavaAttributes;

namespace Aspose.TestFx
{
    /// <summary>
    /// Thread pool for running clients in parallel.
    ///
    /// NOTE We do not use TPL because it's banned in AW sources.
    ///
    /// NOTE We do not use ThreadPal because this source is included by link
    /// to server implementations which do not reference Aspose libraries.
    /// 
    /// NOTE I should have used TPL on the server and then ThreadPal here!
    /// </summary>
    [JavaDelete]
    internal class ThreadPool : IDisposable
    {
        public ThreadPool(int numThreads = 3, bool useSTA = false, Action<string> logger = null)
        {
            Logger = logger;

            _threads = new Thread[numThreads];
            for (int i = 0; i < _threads.Length; i++)
            {
                var thread = new Thread(ThreadProc);
                if (useSTA) thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                _threads[i] = thread;
            }
            Log0("started");

            // When constructor exits the threads may not have started yet, but this
            // is not a problem since work items will wait for them in the queue
        }

        public void Enqueue(Action workItem)
        {
            if (workItem == null) throw new ArgumentNullException(nameof(workItem));
            if (_cancellationTokenSource.IsCancellationRequested) throw new OperationCanceledException();

            Interlocked.Increment(ref _numUnfinished);
            _workQueue.Enqueue(workItem);

            Log0($"[{workItem.GetHashCode()}] enqueued");

            // release threads
            _enqueuedEvent.Set();
        }

        public void Dispose()
        {
            if (_threads[0] == null)
                return;
            _cancellationTokenSource.Cancel();
            Action unused;
            while (!_workQueue.IsEmpty)
                _workQueue.TryDequeue(out unused);
            foreach (var thread in _threads)
                thread.Join();
            for (var i = 0; i < _threads.Length; i++)
                _threads[i] = null;
            Log0("stopped");
        }

        public Action<string> Logger { get; set; }

        /// <summary>
        /// Blocks current thread until no work items remain in the queue
        /// </summary>
        public void WaitForIdle()
        {
            while (Interlocked.CompareExchange(ref _numUnfinished, 0, 0) > 0)
                _completedEvent.WaitOne();
        }

        private void Log0(string message) => Logger?.Invoke(message);

        private void Log(string message) => Logger?.Invoke($"[{System.Threading.Thread.CurrentThread.ManagedThreadId}] {message}");

        private void ThreadProc()
        {
            Log("running");
            while (true)
            {
                if (0 == WaitHandle.WaitAny(new WaitHandle[] { _cancellationTokenSource.Token.WaitHandle, _enqueuedEvent }))
                    break;

                Action work;
                while (_workQueue.TryDequeue(out work))
                {
                    try
                    {
                        Log($"[{work.GetHashCode()}] dequeued");
                        work.Invoke();
                        Log($"[{work.GetHashCode()}] completed");
                    }
                    catch (Exception e)
                    {
                        Log($"[{work.GetHashCode()}] failed: {e.Message}");
                    }
                    Interlocked.Decrement(ref _numUnfinished);
                    _completedEvent.Set();
                }
            }
            Log("finished");
        }

        private volatile int _numUnfinished;       // number of work items not yet finished
        private readonly Thread[] _threads;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly AutoResetEvent _enqueuedEvent = new AutoResetEvent(false);
        private readonly AutoResetEvent _completedEvent = new AutoResetEvent(false);
        private readonly ConcurrentQueue<Action> _workQueue = new ConcurrentQueue<Action>();
    }
}
