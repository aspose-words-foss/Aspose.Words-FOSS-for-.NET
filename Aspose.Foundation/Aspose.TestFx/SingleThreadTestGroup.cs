// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/02/2015 by Konstantin Sidorenko

using System;

namespace Aspose.TestFx
{
    /// <summary>
    /// Each group contains test classes that fail when run in parallel.
    /// Test framework should run the classes in the single thread.
    /// Developer should fix test or production classes that cause multithread conflicts.
    /// </summary>
    public static class SingleThreadTestGroup
    {
        public const String SingleThread = "SingleThreadGroup";
    }
}
