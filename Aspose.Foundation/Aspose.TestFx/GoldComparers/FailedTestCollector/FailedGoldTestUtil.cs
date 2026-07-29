// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/05/2013 by Alexey Butalov

using System;
using System.Diagnostics;
using System.Reflection;
using Aspose.JavaAttributes;
using NUnit.Framework;


namespace Aspose.TestFx.GoldComparers.FailedTestCollector
{
    /// <summary>
    /// Utility methods for GOLD unit tests.
    /// </summary>
    [JavaDelete("RK This should be never ported to Java because it is part of GoldDiffViewer.")]
    public class FailedGoldTestUtil
    {
        private FailedGoldTestUtil()
        {
        }

        /// <summary>
        /// Gets current test method. Detects it by NUnit.Framework.TestAttribute.
        /// Note: cannot find test method when the function called from an other thread, returns top method in this case. 
        /// </summary>
        public static MethodBase GetTestMethod()
        {
            MethodBase topMethod = null;
            MethodBase testMethod = null;
            StackTrace stackTrace = new StackTrace();
            int topStackIndex = 1;

            while ((topStackIndex < stackTrace.FrameCount) && (testMethod == null))
            {
                StackFrame stackFrame = stackTrace.GetFrame(topStackIndex);
                MethodBase method = stackFrame.GetMethod();
                if (topStackIndex == 1)
                    topMethod = method;

                if (method.IsPublic) // NUnit test functions are always public.
                {
                    object[] testAttribs = method.GetCustomAttributes(typeof(TestAttribute), true);
                    if (testAttribs.Length != 0)
                        testMethod = method;
                }
                topStackIndex++;
            }

            return (testMethod != null) ? testMethod : topMethod;
        }

        /// <summary>
        /// Gets a base directory to output problem GOLD tests and config file.
        /// </summary>
        internal static string GetProblemTestBaseDir()
        {
            // PROBLEMGOLDTESTSDIR environment variable should be defined on a TeamCity build agent.
            const string problemTestDirVarName = "PROBLEMGOLDTESTSDIR";
            return Environment.GetEnvironmentVariable(problemTestDirVarName);
        }
    }
}
