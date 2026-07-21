// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/08/2011 by Roman Korchagin

using System;
using Aspose.Warnings;

namespace Aspose.Words
{
    /// <summary>
    /// Contains information about a warning that Aspose.Words issued during document loading or saving.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/programming-with-documents/">Programming with Documents</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <para>You do not create instances of this class. Objects of this class are created
    /// and passed by Aspose.Words to the <see cref="IWarningCallback.Warning"/> method.</para>
    ///
    /// <seealso cref="IWarningCallback"/>
    /// </remarks>
    public class WarningInfo
    {
        /// <summary>
        /// No public ctor.
        /// </summary>
        internal WarningInfo(WarningType warningType, WarningSource source, string description)
        {
            mWarningType = warningType;
            mSource = source;
            mDescription = description;
        }

        internal static WarningType ConvertWarningTypeCoreToWarningType(WarningTypeCore warningTypeCore)
        {
            switch (warningTypeCore)
            {
                case WarningTypeCore.DataLoss:
                    return WarningType.DataLoss;
                case WarningTypeCore.MajorFormattingLoss:
                    return WarningType.MajorFormattingLoss;
                case WarningTypeCore.MinorFormattingLoss:
                    return WarningType.MinorFormattingLoss;
                case WarningTypeCore.FontSubstitution:
                    return WarningType.FontSubstitution;
                case WarningTypeCore.UnexpectedContent:
                    return WarningType.UnexpectedContent;
                default:
                    return WarningType.MinorFormattingLoss;
            }
        }

        internal static WarningSource ConvertWarningSourceCoreToWarningSource(WarningSourceCore warningSourceCore)
        {
            switch (warningSourceCore)
            {
                case WarningSourceCore.Shapes:
                    return WarningSource.Shapes;
                case WarningSourceCore.DrawingML:
                    return WarningSource.DrawingML;
                case WarningSourceCore.Metafile:
                    return WarningSource.Metafile;
                case WarningSourceCore.Xps:
                    return WarningSource.Xps;
                case WarningSourceCore.Pdf:
                    return WarningSource.Pdf;
                case WarningSourceCore.Image:
                    return WarningSource.Image;
                case WarningSourceCore.Svm:
                    return WarningSource.Svm;
                case WarningSourceCore.Font:
                    return WarningSource.Font;
                default:
                    return WarningSource.Unknown;
            }
        }

        internal static string WarningSourceToString(WarningSource warningSource)
        {
            switch (warningSource)
            {
                case WarningSource.Unknown:
                    return "Unknown";
                case WarningSource.Layout:
                    return "Layout";
                case WarningSource.DrawingML:
                    return "DrawingML";
                case WarningSource.OfficeMath:
                    return "OfficeMath";
                case WarningSource.Shapes:
                    return "Shapes";
                case WarningSource.Metafile:
                    return "Metafile";
                case WarningSource.Xps:
                    return "Xps";
                case WarningSource.Pdf:
                    return "Pdf";
                case WarningSource.Image:
                    return "Image";
                case WarningSource.Docx:
                    return "Docx";
                case WarningSource.Doc:
                    return "Doc";
                case WarningSource.Text:
                    return "Text";
                case WarningSource.Rtf:
                    return "Rtf";
                case WarningSource.WordML:
                    return "WordML";
                case WarningSource.Nrx:
                    return "Nrx";
                case WarningSource.Odt:
                    return "Odt";
                case WarningSource.Html:
                    return "Html";
                case WarningSource.Svm:
                    return "Svm";
                case WarningSource.Font:
                    return "Font";
                case WarningSource.Svg:
                    return "Svg";
                default:
                    throw new ArgumentOutOfRangeException("warningSource");
            }
        }

        /// <summary>
        /// Returns the type of the warning.
        /// </summary>
        public WarningType WarningType
        {
            get { return mWarningType; }
        }

        /// <summary>
        /// Returns the description of the warning.
        /// </summary>
        public string Description
        {
            get { return mDescription; }
        }

        /// <summary>
        /// Returns the source of the warning.
        /// </summary>
        public WarningSource Source
        {
            get { return mSource; }
        }

#if DEBUG
        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}", mWarningType, mSource, mDescription);
        }
#endif

        private readonly WarningType mWarningType;
        private readonly WarningSource mSource;
        private readonly string mDescription;
    }
}
