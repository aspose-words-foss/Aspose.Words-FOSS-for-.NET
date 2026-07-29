// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/11/2018 by Roman Pankin

using System;
using System.Net;
using Aspose.Common;
using CodePorting.Translator.Cs2Cpp;
using NUnit.Framework;

namespace Aspose.Tests.Pal
{
    [TestFixture]
    public class TestSystemPal
    {
        /// <summary>
        /// WORDSJAVA-1936 - Image not visible in exported HTML
        /// The document in the jira task contains an image and broken URL (unavailable now) from which this image was loaded.
        /// Aspose.Words is trying to download an image from the Internet before and only if it's not available,
        /// it uses the image from the document's resources. To complete all steps properly, an exception should be raised
        /// if a resource can't be read from the Internet. The URL from the document is "http://img2.7wenta.com/upload/wti/20140528/14012099014997642.png"
        /// 
        /// The link below is unavailable. Both c# and Java have to raise an exception in this case.
        /// </summary>
        [Test]
        public void TestHttpRequest()
        {
            try
            {
                SystemPal.OpenStreamFromHref("http://aaa.bbb/ccc.png");
            }
            catch
            {
                return;
            }

            throw new Exception("Any exception is expected");
        }

        /// <summary>
        /// WORDSJAVA-2811 Images is lost after rendering the document.
        /// In Java Server returned HTTP response code: 403 for URL:
        /// https://www.sec.gov/Archives/edgar/data/1630970/000114420416130880/pg2img1_ex10-13.jpg
        /// It means that server discard request because of forbidden UserAgent string or problem with request.
        ///
        /// Both c# and Java have to use right request and get images.
        /// </summary>
        [Test]
        [CppSkipEntity]
        public void TestJavaRequest()
        {
            string url = "https://www.example.com/example.jpg";
            HttpWebRequest request = (HttpWebRequest)WebRequestHelper.CreateRequestFromUri(url);
            String requestString = WebRequestHelper.GetRequestAsString(request);

            string expectedRequest = "GET https://www.example.com/example.jpg\r\n" +
                "User-Agent:Mozilla/4.0 (compatible; MSIE 5.01; Windows NT 5.0)Accept:*/*Accept-Language:*Accept-Encoding:GZip, Deflate";
            
            Console.WriteLine(requestString);
            Assert.That(requestString, Is.EqualTo(expectedRequest));
        }
    }
}
