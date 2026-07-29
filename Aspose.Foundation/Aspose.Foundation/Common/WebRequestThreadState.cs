// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/06/2010 by Roman Korchagin

using System;
using System.Net;
using Aspose.JavaAttributes;

namespace Aspose.Common
{
    [JavaManual("Platform abstraction for system utilities. Manual porting by design.")]
    [CodePorting.Translator.Cs2Cpp.CppSkipEntity("Platform abstraction for system utilities. Manual porting by design.")]
    internal class WebRequestThreadState
    {
        internal WebRequestThreadState(WebRequest request)
        {
            Debug.Assert(request != null);
            mRequest = request;
        }

        internal WebResponse Response
        {
            get { return mResponse; }
            set { mResponse = value; }
        }

        internal WebRequest Request
        {
            get { return mRequest; }
        }

        internal Exception Exception { get; set; }

        private readonly WebRequest mRequest;
        private WebResponse mResponse;
    }
}
