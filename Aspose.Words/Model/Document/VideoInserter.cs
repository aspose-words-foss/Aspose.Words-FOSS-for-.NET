// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/09/2016 by Andrey Noskov

using System;
using System.IO;
using Aspose.Collections;
using Aspose.Common;
using Aspose.Drawing;
using Aspose.Images.Pal;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.NonVisualProperties;
using Aspose.Words.Drawing.Core.Dml.Readers;
using Aspose.Words.Nrx;
using Aspose.Words.Settings;

namespace Aspose.Words
{
    /// <summary>
    /// Adds a new web video to the document.
    /// </summary>
    internal class VideoInserter
    {
        internal VideoInserter(DocumentBuilder documentBuilder)
        {
            mDocumentBuilder = documentBuilder;
        }

        /// <summary>
        /// Inserts online video from the well-known (supported) video resources to the document.
        /// </summary>
        internal Shape InsertFromUrl(
            string videoUrl,
            RelativeHorizontalPosition horzPos,
            double left,
            RelativeVerticalPosition vertPos,
            double top,
            double width,
            double height,
            WrapType wrapType)
        {
            // Vimeo and YouTube support discovery of the oEmbed URL (http://oembed.com/). Each page that has a video will have
            // two link tags with types application/json+oembed and application/xml+oembed containing the URLs for
            // JSON and XML oEmbed responses. This response will be used to get embedHtml and thumbnailUrl.
            // Please follow this link to get more information https://developer.vimeo.com/apis/oembed

            string oEmbedApiEndpoint = string.Empty;
            string thumbnailUrl = string.Empty;
            string embedHtml = string.Empty;
            byte[] imgBytes = null;

            // Get the domain name.
            Uri url = new Uri(videoUrl);
            string domainName = url.Host;

            // Get oEmbed Api endpoint by the domain name.
            if (gSupportedVideoHosts.ContainsKey(domainName))
                oEmbedApiEndpoint = gSupportedVideoHosts[domainName];

            if (StringUtil.HasChars(oEmbedApiEndpoint))
            {
                Stream videoStream = null;
                try
                {
                    // Load oEmbed response stream.
                    string href = string.Format(oEmbedApiEndpoint + FormatRequest, videoUrl, PlayerWidth, PlayerHeight);
                    videoStream = SystemPal.OpenStreamFromHref(href);
                }
                catch (Exception ex)
                {
                    WarningUtil.Warn(Document.WarningCallback, WarningType.UnexpectedContent, WarningSource.Unknown, ex.Message);
                }

                if (videoStream != null)
                {
                    NrxXmlReader xmlReader = new NrxXmlReader(videoStream);

                    while (xmlReader.ReadChild("oembed"))
                    {
                        switch (xmlReader.LocalName)
                        {
                            case "html":
                                embedHtml = xmlReader.ReadString();
                                break;
                            case "thumbnail_url":
                                thumbnailUrl = xmlReader.ReadString();
                                break;
                            default:
                                // Other tags are not required, ignore.
                                xmlReader.IgnoreElement();
                                break;
                        }
                    }
                    videoStream.Close();
                }
            }
            else
            {
                WarningUtil.Warn(Document.WarningCallback, WarningType.UnexpectedContent, WarningSource.Unknown,
                    string.Format(WarningMessage, domainName));
            }

            if (StringUtil.HasChars(thumbnailUrl))
                imgBytes = ImageDataUtil.LoadImageBytes(thumbnailUrl, Document);

            return InsertFromVideoEmbedCode(videoUrl, embedHtml, imgBytes, horzPos, left, vertPos, top, width, height, wrapType);
        }

        internal Shape InsertFromVideoEmbedCode(
           string videoUrl,
           string videoEmbedCode,
           byte[] thumbnailImageBytes,
           RelativeHorizontalPosition horzPos,
           double left,
           RelativeVerticalPosition vertPos,
           double top,
           double width,
           double height,
           WrapType wrapType)
        {
            // Document have to be optimized for MS Word 2013 to show video.
            Document.CompatibilityOptions.OptimizeFor(MsWordVersion.Word2013);

            // Mark the document as has drawing extensions because there is wp15:webVideoPr.
            OoxmlComplianceInfo.MarkAsHasDrawingExtensions(Document);

            // Create Dml shape.
            Shape shape = new Shape(Document, ShapeMarkupLanguage.Dml);

            shape.RunPr = mDocumentBuilder.GetRunPrCopy();
            shape.RelativeHorizontalPosition = horzPos;
            shape.RelativeVerticalPosition = vertPos;
            shape.Left = left;
            shape.Top = top;
            shape.WrapType = wrapType;

            shape.SetShapeType(ShapeType.Image);

            // Set clickable shape hyperlink address.
            shape.SetShapeAttrInternal(ShapeAttr.HyperlinkAddress, videoUrl);

            shape.DmlNode = CreateDmlPicture(videoEmbedCode, thumbnailImageBytes, Document.GetNextShapeId());

            // We have to set size after DmlNode was set.
            shape.SetSizeSafe(width, height);

            // We presume we are inserting a top level shape, it has to be inserted into the tree
            // for the shape size validation below to work properly.
            mDocumentBuilder.InsertNode(shape);

            return shape;
        }

        /// <summary>
        /// Prepares Dml picture with video extension.
        /// </summary>
        private static DmlPicture CreateDmlPicture(string videoEmbedCode, byte[] thumbnailImageBytes, int shapeId)
        {
            // MSW inserts video as Dml picture with special extension.
            DmlPicture dmlPicture = new DmlPicture();
            dmlPicture.Geometry = DmlGeometryReader.GetPresetGeometry("rect");

            DmlBlipFill blipFill = new DmlBlipFill();
            blipFill.Blip.Extensions = new StringToObjDictionary<DmlExtension>();

                // Create video extension.
            DmlExtension extension = new DmlExtension(DmlExtensionUri.VideoPr, null);
            extension.WebVideoPr = new DmlWebVideoProperties(videoEmbedCode, PlayerWidth, PlayerHeight);
            blipFill.Blip.Extensions.Add(DmlExtensionUri.VideoPr, extension);

            if (thumbnailImageBytes == null)
                thumbnailImageBytes = CreateEmptyThumbnailImage();

            // Set thumbnail image.
            blipFill.Blip.EmbedImage = thumbnailImageBytes;

            dmlPicture.BlipFill = blipFill;

            // Dml shape requires NvDrawingProperties.Id.
            DmlNvPrPicture nvPrPicture = new DmlNvPrPicture();
            nvPrPicture.NvDrawingProperties = new DmlNvDrawingProperties(shapeId, "");
            nvPrPicture.CNvProperties = new DmlCnvPrPicture();
            dmlPicture.NonVisualPr = nvPrPicture;

            return dmlPicture;
        }

        /// <summary>
        /// Creates an empty thumbnail image if the original thumbnail is not set or not available.
        /// </summary>
        private static byte[] CreateEmptyThumbnailImage()
        {
            byte[] thumbnailImageBytes;

            // Mimic MSW behavior and create black bitmap 10x10.
            using (BitmapPal noThumbnailImg = new BitmapPal(10, 10))
            {
                // F0SS

                using (MemoryStream ms = new MemoryStream())
                {
                    noThumbnailImg.Save(ms, FileFormat.Png);
                    thumbnailImageBytes = ms.ToArray();
                }
            }

            return thumbnailImageBytes;
        }

        private Document Document
        {
            get { return mDocumentBuilder.Document; }
        }

        // The width of the video player. The default value for YouTube is 640.
        private const double PlayerWidth = 640;

        // The height of the video player. The default value for YouTube is 390.
        private const double PlayerHeight = 390;

        private const string FormatRequest = "?url={0}&format=xml&maxwidth={1}&maxheight={2}";

        private const string WarningMessage = "Video from {0} can not be embedded. " +
              "Please contact us through the Aspose.Words support forum: https://www.aspose.com/community/forums/default.aspx.";

        private readonly DocumentBuilder mDocumentBuilder;

        /// <summary>
        /// Supported video hosts collection.
        /// </summary>
        private static StringToObjDictionary<string> gSupportedVideoHosts  = InitSupportedVideoHosts();

        /// <summary>
        /// Init supported video hosts collection.
        /// </summary>
        private static StringToObjDictionary<string> InitSupportedVideoHosts()
        {
            // Use "https" to avoid exception "The remote server returned an error: (403) Forbidden." from Youtube
            gSupportedVideoHosts = new StringToObjDictionary<string>();
            gSupportedVideoHosts.Add("youtube.com", "https://www.youtube.com/oembed");
            gSupportedVideoHosts.Add("www.youtube.com", "https://www.youtube.com/oembed");
            gSupportedVideoHosts.Add("youtu.be", "https://www.youtube.com/oembed");
            gSupportedVideoHosts.Add("vimeo.com", "https://vimeo.com/api/oembed.xml");

            return gSupportedVideoHosts;
        }
    }
}
