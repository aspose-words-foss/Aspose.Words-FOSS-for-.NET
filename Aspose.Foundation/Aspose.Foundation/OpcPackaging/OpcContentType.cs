// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/08/2007 by Vladimir Averkin

namespace Aspose.OpcPackaging
{
    /// <summary>
    /// Helper class to map names of common OPC content types to enum-like references.
    /// </summary>
    public static class OpcContentType
    {
        public const string Xml = "application/xml";
        public const string Relationships = "application/vnd.openxmlformats-package.relationships+xml";

        public const string ImageTiff = "image/tiff";
        public const string ImageBmp = "image/bmp";
        public const string ImagePng = "image/png";
        public const string ImageGif = "image/gif";
        public const string ImageJpeg = "image/jpeg";
        public const string ImageEmf = "image/x-emf";
        public const string ImageWmf = "image/x-wmf";
        public const string ImagePictCompressed = "image/x-pcz";
        public const string ImageEps = "image/x-eps";
        public const string ImageWebP = "image/webp";

        public const string Xps = "application/vnd.ms-xpsdocument";
        public const string Pdf = "application/pdf";
        public const string Svg = "image/svg+xml";
        public const string Html = "text/html";
        public const string PostScript = "application/postscript";
        public const string Pcl = "application/vnd.hp-pcl";

        public const string Odttf = "application/vnd.openxmlformats-officedocument.obfuscatedFont";

        public const string OctetStream = "application/octet-stream";
        public const string DigitalSignatureOrigin = "application/vnd.openxmlformats-package.digital-signature-origin";
        public const string DigitalSignature = "application/vnd.openxmlformats-package.digital-signature-xmlsignature+xml";
        public const string DigitalSignatureCertificate = "application/vnd.openxmlformats-package.digital-signature-certificate";
    }
}
