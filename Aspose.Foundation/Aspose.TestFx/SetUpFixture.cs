// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/05/2022 by Edward Voronov

using System;
using System.Diagnostics;
using NUnit.Framework;

namespace Aspose.TestFx
{
    public class SetUpFixture
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // Fix Resharper and Rider empty Test Session Output
            EnsureConsoleTraceListenerUsesConsoleOutWriter();

            // Fix failed tests in Resharper and Rider caused by difference in Trace.Fail behaviour.
            DoNotThrowOnTraceFail();
        }

        private static void EnsureConsoleTraceListenerUsesConsoleOutWriter()
        {
#if !NETSTANDARD && !JAVA && !CPLUSPLUS
            foreach (TraceListener listener in Trace.Listeners)
            {
                ConsoleTraceListener consoleTraceListener = listener as ConsoleTraceListener;
                if (consoleTraceListener == null)
                    continue;

                if (consoleTraceListener.Writer == Console.Out)
                    continue;

                consoleTraceListener.Writer = Console.Out;
            }
#endif
        }

        private static void DoNotThrowOnTraceFail()
        {
#if !NETSTANDARD && !JAVA && !CPLUSPLUS
            for (int index = 0; index < Trace.Listeners.Count; index++)
            {
                TraceListener listener = Trace.Listeners[index];

                if (listener.GetType().FullName != "JetBrains.ReSharper.TestRunner.Logging.TraceListener")
                    continue;

                Trace.Listeners[index] = new DefaultTraceListener();
            }
#endif
        }
    }
}
