// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/11/2011 by Dmitry Vorobyev

using System.IO;
using Aspose.JavaAttributes;
using Aspose.Words.Fields.Expressions;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements the FILESIZE field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <p>Retrieves the size of the current document's file or 0 if the size cannot be determined.</p>
    /// <p>In the current implementation, uses the <see cref="Document.OriginalFileName"/> property to retrieve
    /// the file name used to determine the file size.</p>
    /// </remarks>
    public class FieldFileSize : Field, IFieldCodeTokenInfoProvider
    {
        internal override FieldUpdateAction UpdateCore()
        {
            Int32Constant fileSize = GetResultInternal(FetchDocument(), FieldCodeCache);

            return new FieldUpdateActionApplyResult(this, fileSize);
        }

        [JavaThrows(false)]
        private static Int32Constant GetResultInternal(Document document, IFieldCode fieldCode)
        {
            int fileSize = 0;

            string fileName = document.OriginalFileName;
            if (fileName != null)
            {
                try
                {
                    FileInfo fileInfo = new FileInfo(fileName);
                    fileSize = (int)fileInfo.Length;    // I don't think there are that huge Word documents.

                    // It seems like Word performs rounding this way.
                    if (fieldCode.HasSwitch(IsInMegabytesSwitch))
                        fileSize = MathUtil.DoubleToInt((double)fileSize / (1000 * 1000));
                    else if (fieldCode.HasSwitch(IsInKilobytesSwitch))
                        fileSize = MathUtil.DoubleToInt((double)fileSize / 1000);
                }
                catch
                {
                    // Swallow the exception.
                }
            }

            return new Int32Constant(fileSize);
        }

        /// <summary>
        /// Gets or sets whether to display the file size in kilobytes.
        /// </summary>
        public bool IsInKilobytes
        {
            get { return FieldCodeCache.HasSwitch(IsInKilobytesSwitch); }
            set { FieldCodeCache.SetSwitch(IsInKilobytesSwitch, value); }
        }

        /// <summary>
        /// Gets or sets whether to display the file size in megabytes.
        /// </summary>
        public bool IsInMegabytes
        {
            get { return FieldCodeCache.HasSwitch(IsInMegabytesSwitch); }
            set { FieldCodeCache.SetSwitch(IsInMegabytesSwitch, value); }
        }

        FieldSwitchType IFieldCodeTokenInfoProvider.GetSwitchType(string switchName)
        {
            return GetSwitchTypeInternal(switchName);
        }

        internal static FieldSwitchType GetSwitchTypeInternal(string switchName)
        {
            switch (switchName)
            {
                case IsInKilobytesSwitch:
                case IsInMegabytesSwitch:
                    return FieldSwitchType.Flag;
                default:
                    return FieldSwitchType.Unknown;
            }
        }

        private const string IsInKilobytesSwitch = "\\k";
        private const string IsInMegabytesSwitch = "\\m";

        internal static IFieldInfoResultProvider FieldInfoResultProvider = new FileSizeInfoResultProvider();

        private class FileSizeInfoResultProvider : IFieldInfoResultProvider
        {
            Constant IFieldInfoResultProvider.GetResult(Document document, IFieldCode fieldCode)
            {
                return GetResultInternal(document, fieldCode);
            }
        }
    }
}
