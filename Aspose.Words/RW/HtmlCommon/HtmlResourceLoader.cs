// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/08/2011 by Kamimura Andrey

using System.Collections.Generic;
using System.IO;
using System.Text;
using Aspose.Common;
using Aspose.Images;
using Aspose.IO;
using Aspose.Words.Drawing;
using Aspose.Words.Loading;
using Aspose.Words.RW.Vml;

namespace Aspose.Words.RW.HtmlCommon
{
    /// <summary>
    /// <para>Loads external and local resources and caches them. Provides universal access to a resource by its Uri
    /// in spite of whether it's already loaded or not yet.</para>
    /// <para>If specified, uses a callback implementing <see cref="IResourceLoadingCallback"/> interface to resolve
    /// Uri of a resource to byte array or string with its data.</para>
    /// </summary>
    internal class HtmlResourceLoader
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal HtmlResourceLoader()
        {
            // Empty constructor. Nothing to set up.
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="resourceLoadingCallback">Callback used to resolve Uri of a resource
        /// to byte array or string with its data. Can be <c>null</c>.</param>
        /// <param name="warningCallback">Warning callback processor.</param>
        internal HtmlResourceLoader(IResourceLoadingCallback resourceLoadingCallback, IWarningCallback warningCallback)
        {
            mResourceLoadingCallback = resourceLoadingCallback;
            mWarningCallback = warningCallback;
        }

        /// <summary>
        /// <para>Returns byte array with image data.</para>
        /// <para>If downloading callback was not specified or was specified and has returned <see cref=
        /// "ResourceLoadingAction.Default"/> for an image but the image cannot be downloaded,
        /// returns standard 'no image' picture.</para>
        /// <para>If downloading callback was specified and it has returned <see cref=
        /// "ResourceLoadingAction.Skip"/> for an image, returns <c>null</c>.</para>
        /// </summary>
        internal byte[] LoadImage(string baseUri, string originalUri)
        {
            return LoadImage(baseUri, originalUri, true);
        }

        /// <summary>
        /// <para>Returns byte array with image data.</para>
        /// <para>If downloading callback was not specified or was specified and has returned <see cref=
        /// "ResourceLoadingAction.Default"/> for an image but the image cannot be downloaded,
        /// returns either the standard 'no image' picture or <c>null</c> depending on
        /// the <paramref name="returnNoImageIfMissing"/> parameter value.</para>
        /// <para>If downloading callback was specified and it has returned <see cref=
        /// "ResourceLoadingAction.Skip"/> for an image, returns <c>null</c>.</para>
        /// </summary>
        internal byte[] LoadImage(string baseUri, string originalUri, bool returnNoImageIfMissing)
        {
            ResourceLoadResult result = Load(baseUri, originalUri, ResourceType.Image);

            // Skipped images will be interpreted as external links.
            if (result.LoadingAction == ResourceLoadingAction.Skip)
                return null;

            if (HtmlImageUtil.IsInsertableImage(result.ResourceBytes))
                return result.ResourceBytes;

            // WORDSNET-2579 Try to load using the direct link for corrupted images.
            if (result.LoadingAction == ResourceLoadingAction.Default)
            {
                byte[] reloadedImageBytes = ReloadEmbeddedImage(baseUri, originalUri);
                if (HtmlImageUtil.IsInsertableImage(reloadedImageBytes))
                {
                    return reloadedImageBytes;
                }
            }

            return returnNoImageIfMissing
                ? ImageUtil.GetNoImageBytes()
                : null;
        }

        /// <summary>
        /// <para>Returns byte array with font binary data.</para>
        /// <para>If downloading callback was not specified or was specified and has returned <see cref=
        /// "ResourceLoadingAction.Default"/> or <see cref="ResourceLoadingAction.Skip"/> for a font
        /// but the font cannot be downloaded, returns <c>null</c>.</para>
        /// </summary>
        internal byte[] LoadFont(string baseUri, string originalUri)
        {
            ResourceLoadResult result = Load(baseUri, originalUri, ResourceType.Font);
            return result.ResourceBytes;
        }

        /// <summary>
        /// <para>Returns byte array with HTML document data.</para>
        /// <para>If downloading callback was not specified or was specified and has returned <see cref=
        /// "ResourceLoadingAction.Default"/> or <see cref="ResourceLoadingAction.Skip"/> for an HTML document
        /// but the HTML document cannot be downloaded, returns <c>null</c>.</para>
        /// </summary>
        internal byte[] LoadHtmlDocument(string baseUri, string originalUri)
        {
            ResourceLoadResult result = Load(baseUri, originalUri, ResourceType.Document);
            return result.ResourceBytes;
        }

        /// <summary>
        /// <para>Returns string with CSS style sheet. Returns <c>null</c> if CSS cannot be downloaded.</para>
        /// <para>Also returns <c>null</c> if downloading callback was specified and it has returned
        /// <see cref="ResourceLoadingAction.Skip"/> for the CSS resource.</para>
        /// </summary>
        internal string LoadCss(string baseUri, string originalUri)
        {
            ResourceLoadResult result = Load(baseUri, originalUri, ResourceType.CssStyleSheet);
            return (result.ResourceBytes != null)
                ? Encoding.UTF8.GetString(result.ResourceBytes)
                : null;
        }

        /// <summary>
        /// Adds bytes of an embedded resource to the cache. Used by MHTML import.
        /// </summary>
        internal void CacheEmbeddedResource(string originalUri, byte[] data)
        {
            mEmbeddedResourceCache[originalUri] = data;
        }

        /// <summary>
        /// Updates a specified shape depending on provided data in corresponding ResourceLoadingCallback of the document.
        /// </summary>
        internal void UpdateShape(Shape shape)
        {
            if (!shape.ImageData.IsLinkOnly)
                return;

            string baseUri = string.Empty;
            string originalUri = shape.ImageData.SourceFullName;

            ResourceLoadResult result = Load(baseUri, originalUri, ResourceType.Image);
            if (!ArrayUtil.HasData(result.ResourceBytes))
                return;

            switch (result.LoadingAction)
            {
                case ResourceLoadingAction.UserProvided:
                {
                    // The customer has provided an image or the image is embedded as a data URL,
                    // so lets insert it into the shape.
                    shape.ImageData.ImageBytes = result.ResourceBytes;
                    break;
                }
                case ResourceLoadingAction.Default:
                {
                    // By default we leave shape image as external link and just update its size from the actual image data.
                    break;
                }
                case ResourceLoadingAction.Skip:
                {
                    // The customer wants to skip load image, so do nothing.
                    return;
                }
                default:
                    Debug.Fail("Never invoked!");
                    return;
            }

            ImageSizeCore size = ImageUtil.GetImageSize(result.ResourceBytes);
            // WORDSNET-22219 The ImageSizeCore.Width/Height are in pixels, but in shape they should be in points.
            double widthInPoints = ConvertUtil.PixelToPoint(size.Width, size.HorizontalResolution);
            double heightInPoints = ConvertUtil.PixelToPoint(size.Height, size.VerticalResolution);
            shape.SetSizeSafe(widthInPoints, heightInPoints);
        }

        /// <summary>
        /// The number of milliseconds to wait before the web request times out. The default value is 100000 milliseconds (100 seconds).
        /// </summary>
        internal int WebRequestTimeout
        {
            get { return mWebRequestTimeout; }
            set { mWebRequestTimeout = value; }
        }

        /// <summary>
        /// <para>Removes embedded image (if any) from the cache and loads image by Uri provided.
        /// Embedded images are those which are embedded into imported MHTML document and
        /// are added to the cache by <see cref="CacheEmbeddedResource"/>.</para>
        /// <para>Used if embedded image is broken to try download it from external source.</para>
        /// <para>If an image is not embedded, it isn't removed from the cache and the method returns <c>null</c>.</para>
        /// </summary>
        /// <returns>Bytes of the image in case it has been reloaded by the URI. Otherwise, <c>null</c>.</returns>
        private byte[] ReloadEmbeddedImage(string baseUri, string originalUri)
        {
            if (mEmbeddedResourceCache.Remove(originalUri))
            {
                ResourceLoadResult loadResult = Load(baseUri, originalUri, ResourceType.Image);
                return loadResult.ResourceBytes;
            }
            return null;
        }

        /// <summary>
        /// Loads resource with using user defined callback interface.
        /// </summary>
        private ResourceLoadResult Load(string baseUri, string originalUri, ResourceType resourceType)
        {
            Debug.Assert(originalUri != null);

            // WORDSNET-23677 Empty URIs cannot reference anything and we shouldn't process them.
            if (!StringUtil.HasChars(originalUri))
            {
                return new ResourceLoadResult(null, ResourceLoadingAction.Default);
            }

            // First, check if the URI is a data URL. Resources embedded as data URLs are not considered external
            // and they should be processed internally, without invoking the resource loading callback.
            ResourceLoadResult dataUrlLoadResult = ParseDataUrl(originalUri, resourceType);
            if (dataUrlLoadResult != null)
            {
                return dataUrlLoadResult;
            }

            string absoluteUri = UriUtil.ConstructUnescapedAbsoluteUri(baseUri, originalUri);

            byte[] resourceBytes = null;
            ResourceLoadingAction action = ResourceLoadingAction.Default;

            // Invoke the resource loading callback, if provided by the user.
            if (mResourceLoadingCallback != null)
            {
                ResourceLoadingArgs args = new ResourceLoadingArgs(absoluteUri, originalUri, resourceType);
                action = mResourceLoadingCallback.ResourceLoading(args);

                // The user could change args.Uri in his callback.
                if (args.Uri != absoluteUri)
                {
                    absoluteUri = args.Uri;

                    // And the new URI privided by the user may be a data URL.
                    // We support this scenario and load data from such URLs.
                    if (action == ResourceLoadingAction.Default)
                    {
                        dataUrlLoadResult = ParseDataUrl(args.Uri, resourceType);
                        if (dataUrlLoadResult != null)
                        {
                            return dataUrlLoadResult;
                        }
                    }
                }

                if (action == ResourceLoadingAction.UserProvided)
                {
                    resourceBytes = args.GetData();
                }
            }

            if (action == ResourceLoadingAction.Default)
            {
                resourceBytes = LoadFromCacheOrUri(originalUri, absoluteUri, resourceType);
            }

            // If loading of the resource has been skipped, the resource data array must be null.
            Debug.Assert(!((action == ResourceLoadingAction.Skip) && (resourceBytes != null)));

            // WORDSNET-15038 Try to unzip an image blob, as it can come in a zip-archived format(.wmz or .emz).
            if (resourceType == ResourceType.Image)
                resourceBytes = VmlUtil.UnpackImage(resourceBytes);

            return new ResourceLoadResult(resourceBytes, action);
        }

        private ResourceLoadResult ParseDataUrl(
            string uri,
            ResourceType resourceType)
        {
            // From the calling code's point of view, data embedded as data URLs are similar to data provided by user code
            // in the way that they are not loaded from external resources.
            const ResourceLoadingAction dataUrlAction = ResourceLoadingAction.UserProvided;

            DataUrl dataUrl = DataUrl.Parse(uri);

            // An instance with no data is returned as an indication that the parser has recognized the URI
            // as a data URL but couldn't parse it further. This is different from the case where a URI is
            // not recognized as a data URL and should be processed somewhere else.
            if (dataUrl != null)
            {
                if (dataUrl.Data.Length == 0)
                {
                    WarnAboutResourceLoadingError(uri);
                    return new ResourceLoadResult(null, dataUrlAction);
                }

                byte[] dataBytes;

                // Image resources are normally binary data that should ignore charset information, except for SVG images,
                // that are textual data. All other resource types are textual data as well.
                // WORDSNET-19654 Added support for SVG embedded in data URLs.
                byte[] dataInUtf8 = dataUrl.GetDataInUtf8();
                dataBytes = ((resourceType == ResourceType.Image) && !ImageUtil.IsSvg(dataInUtf8))
                    ? dataUrl.Data
                    : dataInUtf8;

                return new ResourceLoadResult(dataBytes, dataUrlAction);
            }

            return null;
        }

        /// <summary>
        /// <para>Takes a resource from the cache. If not found, downloads it by Uri provided.</para>
        /// <para>If the resource cannot be downloaded never tries to download it again on subsequent requests.</para>
        /// </summary>
        /// <param name="originalUri">First name to search the resource in the cache.
        /// Used for storing resource embedded to MHTML.</param>
        /// <param name="absoluteUri">Second name to search the resource in the cache.</param>
        /// <param name="resourceType">Loading rules are different for resources of different types.</param>
        private byte[] LoadFromCacheOrUri(string originalUri, string absoluteUri, ResourceType resourceType)
        {
            byte[] data;
            if (mEmbeddedResourceCache.TryGetValue(originalUri, out data))
            {
                return data;
            }

            if (mExternalResourceCache.TryGetValue(absoluteUri, out data))
            {
                return data;
            }

            data = LoadResourceByUri(absoluteUri, resourceType);
            mExternalResourceCache[absoluteUri] = data;
            return data;
        }

        /// <summary>
        /// Loads array of bytes from specified uri. Make specific handling of various resource types.
        /// </summary>
        private byte[] LoadResourceByUri(string uri, ResourceType resourceType)
        {
            bool canLoad;
            switch (resourceType)
            {
                case ResourceType.CssStyleSheet:
                case ResourceType.Document:
                case ResourceType.Font:
                {
                    canLoad = true;
                    break;
                }
                case ResourceType.Image:
                {
                    canLoad = !UriUtil.IsCid(uri);
                    break;
                }
                default:
                {
                    Debug.Fail("Never invoked!");
                    canLoad = false;
                    break;
                }
            }
            return canLoad
                ? LoadBytesByUri(uri)
                : null;
        }

        /// <summary>
        /// Loads array of bytes from specified uri.
        /// </summary>
        private byte[] LoadBytesByUri(string uri)
        {
            byte[] loadedData = null;
            try
            {
                using (Stream stream = SystemPal.OpenStreamFromHref(uri, mWebRequestTimeout))
                {
                    if (stream.Length > 0)
                    {
                        loadedData = StreamUtil.CopyStreamToByteArray(stream);
                    }
                }
            }
            catch
            {
                // Suppress data loading exceptions.
            }

            // WORDSNET-14515 Issue a WarningType.DataLoss warning in case a resource is not loaded.
            if (loadedData == null)
            {
                WarnAboutResourceLoadingError(uri);
            }

            return loadedData;
        }

        private void WarnAboutResourceLoadingError(string uri)
        {
            if (mWarningCallback != null)
            {
                // URIs may be very long (especially if they are data URLs) so we need to make sure warning messages remain
                // relatively short. The limit has been chosen arbitrarily.
                const int uriLengthLimit = 256;
                if (uri.Length > uriLengthLimit)
                {
                    uri = uri.Substring(0, uriLengthLimit) + "...";
                }

                mWarningCallback.Warning(new WarningInfo(
                    WarningType.DataLoss,
                    WarningSource.Html,
                    "Couldn't load a resource from '" + uri + "'."));
            }
        }

        private readonly IWarningCallback mWarningCallback;

        private readonly IResourceLoadingCallback mResourceLoadingCallback;

        /// <summary>
        /// <para>Contains data of all resources previously requested. Key is the absolute URI of the resource (a string).
        /// Value is a byte array.</para>
        /// <para>Value is <c>null</c> or standard 'no image' picture for resources which cannot be downloaded.</para>
        /// </summary>
        private readonly Dictionary<string, byte[]> mExternalResourceCache = new Dictionary<string, byte[]>();

        /// <summary>
        /// <para>Contains data for all resources embedded to MHTML. Key is the original URI of the resource (a string,
        /// as specified in the source document). Value is a byte array.</para>
        /// </summary>
        private readonly Dictionary<string, byte[]> mEmbeddedResourceCache = new Dictionary<string, byte[]>();

        private int mWebRequestTimeout = 100000;
    }
}
