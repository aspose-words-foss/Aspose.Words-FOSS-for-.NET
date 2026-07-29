// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/02/2010 by Viktor Sazhaev

using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Aspose.Words.RW.MarkupLanguage.Writer
{
    /// <summary>
    /// Helps to write subsidiary resources referenced in the document model into non-native formats (primarily HTML and related).
    /// </summary>
    internal class ResourceWriterBase
    {
        /// <summary>
        /// Call this before building the document.
        /// </summary>
        /// <param name="fileName">Pass the name of the file you want to save to.
        /// Pass null if saving into a stream. Used to extract path and base name for resources.</param>
        /// <param name="subsidiaryContentPartCollector">Can be null. If not null, then we collect all subsidiary
        /// parts to be written separately or externally.</param>
        /// <param name="exportResourceFolder">ExportResourceFolder</param>
        /// <param name="exportResourceFolderAlias">ExportResourceFolderAlias</param>
        /// <param name="messageWhenNoOutputFolder">Message that is put into exception when we cannot write
        /// files because we have no clearly specified output directory.</param>
        /// <param name="doNotUseResourceFolderForResourceUri">
        /// Specifies that <paramref name="exportResourceFolderAlias"/> shouldn't be used to construct resource URI.
        /// This is needed when linked resources (CSS and fonts, for example) are in the same folder.</param>
        /// <param name="isMakeResourceFolderAbsolute">
        /// If exportResourcesFolder is relative, it will be expanded using the document folder as a base.</param>
        internal ResourceWriterBase(
            string fileName,
            SubsidiaryContentPartCollector subsidiaryContentPartCollector,
            string exportResourceFolder,
            string exportResourceFolderAlias,
            string messageWhenNoOutputFolder,
            bool doNotUseResourceFolderForResourceUri,
            bool isMakeResourceFolderAbsolute)
        {
            mSubsidiaryContentPartCollector = subsidiaryContentPartCollector;
            mMessageWhenNoOutputFolder = messageWhenNoOutputFolder;

            if (mSubsidiaryContentPartCollector != null)
            {
                // In this mode we just collect subsidiary parts not writing them anywhere. File paths are empty.
                mResourceFolder = string.Empty;
                mResourceFolderAlias = string.Empty;
                mResourceBaseName = string.Empty;
            }
            else
            {
                // Whether to write document to stream (fileNameSpecified==false) or to file
                bool fileNameSpecified = StringUtil.HasChars(fileName);
                bool exportFolderSpecified = StringUtil.HasChars(exportResourceFolder);
                bool exportAliasSpecified = StringUtil.HasChars(exportResourceFolderAlias);

                // If exportResourceFolder is specified then always use it, it has priority over current document file name.
                // Next, when file name is available, resources will be saved in the same folder as the document.
                // If nothing is given don't choose temp directory: in this case either streams via resource saving
                // event (maybe in the future) must be provided or exception will be thrown.
                if (exportFolderSpecified)
                {
                    mResourceFolder = exportResourceFolder;
                    if (!Path.IsPathRooted(mResourceFolder) && isMakeResourceFolderAbsolute)
                    {
                        string saveFileDirectoryName = (fileNameSpecified)
                            ? Path.GetDirectoryName(Path.GetFullPath(fileName))
                            : string.Empty;
                        if (StringUtil.HasChars(saveFileDirectoryName))
                        {
                            mResourceFolder = Path.Combine(saveFileDirectoryName, mResourceFolder);
                        }
                    }
                }
                else if (fileNameSpecified)
                {
                    mResourceFolder = Path.GetDirectoryName(Path.GetFullPath(fileName));
                }
                else
                {
                    mResourceFolder = string.Empty;
                }

                // If alias is specified then use it. Alias can consist of one dot ('.') or start with dot+slash. Remove this.
                // Next, when exportResourceFolder is specified explicitly write the same.
                // Defaults to nothing if directory comes from output file name or not available: if noting but output file
                // name is specified then client code expects resources in the same directory and short names in output.
                if (!doNotUseResourceFolderForResourceUri && exportAliasSpecified)
                {
                    mResourceFolderAlias = gMatchDotSlashes.Replace(exportResourceFolderAlias, string.Empty);
                }
                else if (!doNotUseResourceFolderForResourceUri && exportFolderSpecified)
                {
                    mResourceFolderAlias = exportResourceFolder;
                }
                else
                {
                    mResourceFolderAlias = string.Empty;
                }

                // The resources will be named after the document file name if available.
                // Otherwise we will save them into files called like "Aspose.Words.xxxxxxxxxx.nnn.ext".
                // So in the most cases you can save resources from different documents to the same directory.
                // Now using guid to generate file name instead of ticks to make file names unique.
                mResourceBaseName = (fileNameSpecified)
                                        ? Path.GetFileNameWithoutExtension(fileName)
                                        : string.Format("Aspose.Words.{0}", Guid.NewGuid());
            }
        }

        /// <summary>
        /// Returns full filename including the folder where the resources are saved.
        /// </summary>
        internal string GetFullFileName(string fileName)
        {
            return Path.Combine(mResourceFolder, fileName);
        }

        /// <summary>
        /// Maps a physical file name into a URI that can be written into HTML file.
        /// </summary>
        internal string MapResourceFileName(string fileName)
        {
            if (UriUtil.IsHrefWithScheme(fileName))
                return fileName;

            // RK Do not prepend file://. I don't think it is necessary, besides,
            // it screws up Aspose.Network so images in email appear as attachments.
            return UriUtil.CombineHref(mResourceFolderAlias, Path.GetFileName(fileName));
        }

        /// <summary>
        /// Checks whether we can write subsidiaries to disk.
        /// </summary>
        internal void CreateFolderIfNeeded()
        {
            if (!StringUtil.HasChars(mResourceFolder))
                throw new InvalidOperationException(mMessageWhenNoOutputFolder);

            // WORDSNET-14590 Create folder if it doesn't exist.
            if (!Directory.Exists(mResourceFolder))
                Directory.CreateDirectory(mResourceFolder);
        }

        /// <summary>
        /// Handler responsible for storing subsidiary parts when writing container formats.
        /// </summary>
        protected SubsidiaryContentPartCollector SubsidiaryContentPartCollector
        {
            get { return mSubsidiaryContentPartCollector; }
        }

        /// <summary>
        /// The base (common) part of all resource names exported from this file.
        /// </summary>
        protected string ResourceBaseName
        {
            get { return mResourceBaseName; }
        }

        /// <summary>
        /// Handler responsible for storing subsidiary parts when writing container formats.
        /// </summary>
        private readonly SubsidiaryContentPartCollector mSubsidiaryContentPartCollector;
        /// <summary>
        /// Message to throw in an exception when unable to write to disk.
        /// Should be dependent on whether we write HTML or PDF and kind of resource.
        /// </summary>
        private readonly string mMessageWhenNoOutputFolder;
        /// <summary>
        /// Folder where the resources are saved.
        /// </summary>
        private readonly string mResourceFolder;
        /// <summary>
        /// Name of the folder used when constructing the resources URI that is written into the file.
        /// </summary>
        private readonly string mResourceFolderAlias;
        /// <summary>
        /// The base (common) part of all resource names exported from this file.
        /// Maybe used differently for some resources. Basic idea is providing distinguishability.
        /// </summary>
        private readonly string mResourceBaseName;

        /// <summary>
        /// Matches the lonely '.', either './' or '.\' at the beginning. Removing this from the alias.
        /// </summary>
        private static readonly Regex gMatchDotSlashes = new Regex(@"^\.($|[/\\])", RegexOptions.Compiled);
    }
}
