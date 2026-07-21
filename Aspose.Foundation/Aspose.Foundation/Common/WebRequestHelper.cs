// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/11/2021 by Bogdan Novosad

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using Aspose.IO;
using Aspose.JavaAttributes;

namespace Aspose.Common
{
    /// <summary>
    /// Contains helper methods for creating web requests.
    /// </summary>
    /// <remarks>
    /// Limits rate of unsuccessful requests.
    /// </remarks>
    [JavaManual("Platform abstraction for system utilities. Manual porting by design.")]
    [CodePorting.Translator.Cs2Cpp.CppSkipEntity("Platform abstraction for system utilities. Manual porting by design.")]
    internal static class WebRequestHelper
    {
        internal static Stream OpenStreamFromUri(string uri, int timeout)
        {
            // WORDSNET-12737 Newline characters inside URLs prevent HTML import from loading resources.
            uri = uri.Replace("\n", "");

            // WORDSNET-22710 Limit rate of unsuccessful requests.
            CheckExceptionCache(uri);

            WebRequest request = CreateRequestFromUri(uri);

            request.Timeout = timeout;
            MemoryStream result = new MemoryStream();
            // WORDSNET-11514 LoadOptions.WebRequestTimeout ignores the web request time out.
            // We cannot use BeginGetResponse method because this requires some synchronous setup tasks to complete
            // (DNS resolution, proxy detection, and TCP socket connection, for example) before this method becomes asynchronous.
            Thread thread = new Thread(GetResponse);
            WebRequestThreadState state = new WebRequestThreadState(request);
            thread.Start(state);

            if (!thread.Join(timeout))
                throw StoreException(uri, new WebException("The operation has timed out.", WebExceptionStatus.Timeout));

            if (state.Exception != null)
                throw StoreException(uri, state.Exception);

            if (state.Response != null)
            {
                using (Stream responseStream = state.Response.GetResponseStream())
                    StreamUtil.CopyStream(responseStream, result);
                result.Position = 0;
            }

            RemoveException(uri);
            return result;
        }

        [JavaThrows(true)]
        public static WebRequest CreateRequestFromUri(string uri)
        {
            // This handles http://, file:// and probably ftp://.
            WebRequest request = WebRequest.Create(new Uri(uri));

            HttpWebRequest requestHttpWebRequest = request as HttpWebRequest;
            if (requestHttpWebRequest != null)
            {
                requestHttpWebRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 5.01; Windows NT 5.0)";

                // WORDSNET-23888 We've seen a HTTP server that refused to respond unless the following HTTP headers
                // were specified.
                requestHttpWebRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                requestHttpWebRequest.Accept = "*/*";
                requestHttpWebRequest.Headers.Add("Accept-Language", "*");
                return requestHttpWebRequest;
            }

            return request;
        }

        public static string GetRequestAsString(HttpWebRequest request)
        {
            string str = request.Method + " " + request.RequestUri + Environment.NewLine;

            string[] headerKeys = request.Headers.AllKeys;
            foreach (string key in headerKeys)
            {
                str += key + ":" + request.Headers[key];
            }

            if (request.AutomaticDecompression != DecompressionMethods.None)
                str += "Accept-Encoding:" + request.AutomaticDecompression.ToString();

            return str;
        }

        /// <summary>
        /// Checks if <see cref="gRequestExceptionCache"/> has entry for <paramref name="uri"/>, and throws exceptions if
        /// exception timeout is not expired.
        /// </summary>
        /// <param name="uri"></param>
        /// <exception cref="Exception"></exception>
        private static void CheckExceptionCache(string uri)
        {
            Exception previousException;
            lock (gRequestExceptionCache)
                gRequestExceptionCache.TryGetValue(uri, out previousException);
            if (previousException != null && !IsExceptionTimeoutExpired(previousException))
                throw previousException;
        }

        /// <summary>
        /// Removes exception for <paramref name="uri"/> from <see cref="gRequestExceptionCache"/>.
        /// </summary>
        /// <param name="uri"></param>
        private static void RemoveException(string uri)
        {
            lock (gRequestExceptionCache)
                gRequestExceptionCache.Remove(uri);
        }

        /// <summary>
        /// Sets exceptions timeout and stores it in <see cref="gRequestExceptionCache"/>.
        /// </summary>
        /// <returns>Same exception that was passed as parameter.</returns>
        private static Exception StoreException(string uri, Exception ex)
        {
            SetExceptionTimeout(ex);
            lock (gRequestExceptionCache)
                gRequestExceptionCache[uri] = ex;

            return ex;
        }

        /// <summary>
        /// Returns true if <paramref name="ex"/> timeout is expired.
        /// </summary>
        private static bool IsExceptionTimeoutExpired(Exception ex)
        {
            DateTime nextTryAfter = (DateTime)ex.Data[TryAfterKey];
            return nextTryAfter < DateTime.UtcNow;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="ex"></param>
        private static void SetExceptionTimeout(Exception ex)
        {
            ex.Data[TryAfterKey] = DateTime.UtcNow.AddSeconds(ExceptionTimeout);
        }

        /// <summary>
        /// Used for asynchronous receipt of <see cref="WebResponse"/>.
        /// </summary>
        private static void GetResponse(object state)
        {
            WebRequestThreadState requestState = (WebRequestThreadState)state;
            try
            {
                requestState.Response = requestState.Request.GetResponse();
            }
            catch (Exception ex)
            {
                requestState.Exception = ex;
            }
        }

        /// <summary>
        /// Request timeout to the same uri in case of previous unsuccessful attempt in seconds.
        /// </summary>
        private const int ExceptionTimeout = 10;

        /// <summary>
        /// Name of the key for <see cref="Exception.Data"/> dictionary, that holds <see cref="DateTime"/>
        /// after which another WebRequest is possible.
        /// </summary>
        private const string TryAfterKey = "NextTryAfter";

        /// <summary>
        /// Holds uri and associated with it Exception that occured during web request.
        /// </summary>
        private static readonly Dictionary<string, Exception> gRequestExceptionCache
            = new Dictionary<string, Exception>();
    }
}
