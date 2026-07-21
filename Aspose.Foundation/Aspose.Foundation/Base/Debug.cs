// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2011 by Michael Morozoff

using System;
using System.Diagnostics;
using Aspose.JavaAttributes;
#if NETSTANDARD
using TraceDebug = System.Diagnostics.Trace; // System.Diagnostics.Debug.Listeners is not avaliable in NetStandard.
#else
using TraceDebug = System.Diagnostics.Debug;
#endif

namespace Aspose
{
    /// <summary>
    /// Replaces standard <see cref="TraceDebug"/> class.
    /// By default routes all calls to the standard implementation.
    /// Provides additional configuration options.
    /// </summary>
    /// <remarks>
    /// Great number of Debug methods calls adds some noise in performance profiling result.
    /// Excluding of DEBUG_ASSERT from the test project constants can help to avoid this.
    /// We use DEBUG_ASSERT instead of DEBUG for this reason only.
    /// </remarks>
    [JavaManual("This is only really need in C# since it just adds some debugging facilities (like break on assert?).")]
    public static class Debug
    {
        /// <summary>
        /// If set will show assert dialog if assert fails.
        /// </summary>
        [Conditional("DEBUG_ASSERT")]
        [DebuggerStepThrough]
        public static void ShowDialogOnAssert(bool value)
        {
            DefaultTraceListener listener = Listeners["Default"] as DefaultTraceListener;
            if (null != listener)
                listener.AssertUiEnabled = value;
        }

        /// <summary>
        /// If set will call debugger if assert fails.
        /// </summary>
        [Conditional("DEBUG_ASSERT")]
        [DebuggerStepThrough]
        public static void BreakOnAssert(bool value)
        {
            gIsBreakOnAssert = value;
        }
        private static bool gIsBreakOnAssert;

        /// <summary>
        /// If set will throw exception if assert fails.
        /// </summary>
        [Conditional("DEBUG_ASSERT")]
        [DebuggerStepThrough]
        public static void ThrowOnAssert(bool value)
        {
            gIsThrowOnAssert = value;
        }
        private static bool gIsThrowOnAssert;

        /// <summary>
        /// Sets ThrowOnAssert(true) and ShowDialogOnAssert(false).
        /// </summary>
        [Conditional("DEBUG_ASSERT")]
        [DebuggerStepThrough]
        public static void SetTestDefaults()
        {
            ShowDialogOnAssert(false);
            //  With attached debugger we would like to see the assert
            if (Debugger.IsAttached)
                BreakOnAssert(true);
            else
                ThrowOnAssert(true);
        }

        [Conditional("DEBUG_ASSERT")]
        [DebuggerStepThrough]
        public static void Assert(bool condition)
        {
            Assert(condition, "", "");
        }

        [Conditional("DEBUG_ASSERT")]
        [DebuggerStepThrough]
        public static void Assert(bool condition, string message)
        {
            Assert(condition, message, "");
        }

        [Conditional("DEBUG_ASSERT")]
        [DebuggerStepThrough]
        public static void Assert(bool condition,
            string message,
            string detailMessage)
        {
            if (!condition)
            {
                if (gIsBreakOnAssert)
                    Debugger.Break();
                else if (gIsThrowOnAssert)
                    throw new InvalidOperationException(new StackTrace(0, true).ToString());
                else
                    TraceDebug.Assert(false, message, detailMessage);
            }
        }

        [Conditional("DEBUG_ASSERT")]
        [DebuggerStepThrough]
        public static void Fail(string message)
        {
            Assert(false, message);
        }

        [Conditional("DEBUG_ASSERT")]
        [DebuggerStepThrough]
        public static void Fail(string message, string detailMessage)
        {
            Assert(false, message, detailMessage);
        }

        [Conditional("DEBUG_ASSERT")]
        [DebuggerStepThrough]
        public static void Write(string message)
        {
            TraceDebug.Write(message);
        }

        [Conditional("DEBUG_ASSERT")]
        [DebuggerStepThrough]
        public static void Write(object value)
        {
            TraceDebug.Write(value);
        }

        [Conditional("DEBUG_ASSERT")]
        [DebuggerStepThrough]
        public static void Write(string message, string category)
        {
            TraceDebug.Write(message, category);
        }

        [Conditional("DEBUG_ASSERT")]
        [DebuggerStepThrough]
        public static void Write(object value, string category)
        {
            TraceDebug.Write(value, category);
        }

        [Conditional("DEBUG_ASSERT")]
        [DebuggerStepThrough]
        public static void WriteIf(bool condition, string message)
        {
            TraceDebug.WriteIf(condition, message);
        }

        [Conditional("DEBUG_ASSERT")]
        [DebuggerStepThrough]
        public static void WriteIf(bool condition, object value)
        {
            TraceDebug.WriteIf(condition, value);
        }

        [Conditional("DEBUG_ASSERT")]
        [DebuggerStepThrough]
        public static void WriteIf(bool condition, string message, string category)
        {
            TraceDebug.WriteIf(condition, message, category);
        }

        [Conditional("DEBUG_ASSERT")]
        [DebuggerStepThrough]
        public static void WriteIf(bool condition, object value, string category)
        {
            TraceDebug.WriteIf(condition, value, category);
        }

        [Conditional("DEBUG_ASSERT")]
        [DebuggerStepThrough]
        public static void WriteLine(string message)
        {
            TraceDebug.WriteLine(message);
        }

        [Conditional("DEBUG_ASSERT")]
        [DebuggerStepThrough]
        public static void WriteLine(object value)
        {
            TraceDebug.WriteLine(value);
        }

        [Conditional("DEBUG_ASSERT")]
        [DebuggerStepThrough]
        public static void WriteLine(string message, string category)
        {
            TraceDebug.WriteLine(message, category);
        }

        [Conditional("DEBUG_ASSERT")]
        [DebuggerStepThrough]
        public static void WriteLineIf(bool condition, string message)
        {
            TraceDebug.WriteLineIf(condition, message);
        }

        /// <summary>
        /// </summary>
        [Conditional("DEBUG_ASSERT")]
        [DebuggerStepThrough]
        public static void Indent()
        {
            TraceDebug.Indent();
        }

        /// <summary>
        /// </summary>
        [Conditional("DEBUG_ASSERT")]
        [DebuggerStepThrough]
        public static void Unindent()
        {
            TraceDebug.Unindent();
        }

         ///<summary>
        ///</summary>
        public static TraceListenerCollection Listeners
        {
            [DebuggerStepThrough]
            get { return TraceDebug.Listeners; }
        }

        /// <summary>
        /// Checks whether current method was called from a method of class <paramref name="type"/>.
        /// </summary>
        [Conditional("DEBUG_ASSERT")]
        [DebuggerStepThrough]
        public static void AssertCallingClass(Type type)
        {
#if !OPTIMIZED
            // Can be wrong if code optimization is ON because of inlining.
            Assert(new StackFrame(2, true).GetMethod().DeclaringType == type);
#else
            Assert(false, "Not supported if code optimization is ON because of inlining.");
#endif
        }

        /// <summary>
        /// Checks whether current method was called from a method of class named <paramref name="typeName"/>.
        /// </summary>
        [Conditional("DEBUG_ASSERT")]
        [DebuggerStepThrough]
        public static void AssertCallingClass(string typeName)
        {
#if !OPTIMIZED
            // Can be wrong if code optimization is ON because of inlining.
            Type type = new StackFrame(2, true).GetMethod().DeclaringType;
            Assert((type == null) || (type.Name == typeName));
#else
            Assert(false, "Not supported if code optimization is ON because of inlining.");
#endif
        }
    }
}
