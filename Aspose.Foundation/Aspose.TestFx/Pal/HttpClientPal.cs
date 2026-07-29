// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/01/2017 by Alexey Noskov

using System;
using System.IO;
using System.Net;
using System.Text;
using Aspose.Collections;
using Aspose.JavaAttributes;

namespace Aspose.TestFx.Pal
{
    [JavaManual("By design")]
    public static class HttpClientPal
    {
        public static void DoPost(string serverUrl, StringToStringDictionary httpParameters)
        {
            if (httpParameters == null || httpParameters.Count == 0)
                throw new ArgumentException("HttpParameters cannot be empty.");

#pragma warning disable SYSLIB0014 // Type or member is obsolete. Code is used only in tests so it is not critical.
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(serverUrl);
#pragma warning restore SYSLIB0014 // Type or member is obsolete
            request.Method = "POST";

            StringBuilder postPayload = new StringBuilder();
            bool first = true;

            StringToStringDictionary.Enumerator enumerator = httpParameters.GetEnumerator();
            while (enumerator.MoveNext())
            {
                string key = enumerator.CurrentKey;
                string value = enumerator.CurrentValue;

                if (string.IsNullOrEmpty(value))
                    continue;

                if (!first)
                    postPayload.Append("&");

                postPayload.Append(MakePair(key, value));
                first = false;
            }

            //https://msdn.microsoft.com/en-us/library/debx8sh9(v=vs.110).aspx

            //Console.WriteLine("REQUEST=" + postPayload);

            byte[] requestData = Encoding.UTF8.GetBytes(postPayload.ToString());

            // request
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = requestData.Length;
            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(requestData, 0, requestData.Length);
            }

            try { 
            // response
#if !CPLUSPLUS && !NETSTANDARD
                if (!request.HaveResponse) return;
#endif
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response != null)
                    {
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            reader.ReadToEnd();
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                if (ex.Response == null)
                {
                    throw;
                }

                HttpWebResponse errorResponse = (HttpWebResponse)ex.Response;
                StreamReader reader = new StreamReader(errorResponse.GetResponseStream());
                string error = reader.ReadToEnd();
                Debug.WriteLine("Error: " + error);
                throw new Exception(error);
            }

        }

        private static string MakePair(string key, string value)
        {
            return string.Format("{0}={1}", Uri.EscapeDataString(key), Uri.EscapeDataString(value));
        }
    }
}
