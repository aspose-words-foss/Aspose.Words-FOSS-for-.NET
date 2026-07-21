// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/01/2020 by Konstantin Kornilov

using System;

namespace Aspose
{
    public class ProcessExecutionResult
    {
        public ProcessExecutionResult(string output, bool successful, string errors, Exception exception)
        {
            mOutput = output;
            mSuccessful = successful;
            mErrors = errors;
            mException = exception;
        }

        public string Output
        {
            get { return mOutput; }
        }

        public string Errors
        {
            get { return mErrors; }
        }

        public Exception Exception
        {
            get { return mException; }
        }

        public bool Successful
        {
            get { return mSuccessful; }
        }

        private readonly string mOutput;
        private readonly bool mSuccessful;
        private readonly string mErrors;
        private readonly Exception mException;
    }
}
