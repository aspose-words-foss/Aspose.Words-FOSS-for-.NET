// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/08/2012 by Dmitry Vorobyev

using Aspose.Words.Drawing;
using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements the INCLUDEPICTURE field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// Retrieves a picture and displays it as the field result.
    /// </remarks>
    public class FieldIncludePicture : Field, IFieldCodeTokenInfoProvider, IFieldIncludePictureCode
    {
        internal override FieldUpdateAction UpdateCore()
        {
            return FieldIncludePictureUpdater.Update(this);
        }

        internal override void BeforeUnlink()
        {
            NodeRange range = GetFieldResultRange();
            foreach (Node node in range)
            {
                Shape shape = node as Shape;
                if (shape != null)
                    shape.ImageData.SourceFullName = string.Empty;
            }
        }

        FieldSwitchType IFieldCodeTokenInfoProvider.GetSwitchType(string switchName)
        {
            switch (switchName)
            {
                case GraphicFilterSwitch:
                    return FieldSwitchType.HasArgument;
                case IsLinkedSwitch:
                case ResizeHorizontallySwitch:
                case ResizeVerticallySwitch:
                    return FieldSwitchType.Flag;
                default:
                    return FieldSwitchType.Unknown;
            }
        }

        /// <summary>
        /// Gets or sets the location of the picture using an IRI.
        /// </summary>
        public string SourceFullName
        {
            get { return FieldCodeCache.GetArgumentAsString(SourceFullNameArgumentIndex); }
            set { FieldCodeCache.SetArgument(SourceFullNameArgumentIndex, value); }
        }

        /// <summary>
        /// Gets or sets the name of the filter for the format of the graphic that is to be inserted.
        /// </summary>
        public string GraphicFilter
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(GraphicFilterSwitch); }
            set { FieldCodeCache.SetSwitch(GraphicFilterSwitch, value); }
        }

        /// <summary>
        /// Gets or sets whether to reduce the file size by not storing graphics data with the document.
        /// </summary>
        public bool IsLinked
        {
            get { return FieldCodeCache.HasSwitch(IsLinkedSwitch); }
            set { FieldCodeCache.SetSwitch(IsLinkedSwitch, value); }
        }

        /// <summary>
        /// Gets or sets whether to resize the picture horizontally from the source.
        /// </summary>
        public bool ResizeHorizontally
        {
            get { return FieldCodeCache.HasSwitch(ResizeHorizontallySwitch); }
            set { FieldCodeCache.SetSwitch(ResizeHorizontallySwitch, value); }
        }

        /// <summary>
        /// Gets or sets whether to resize the picture vertically from the source.
        /// </summary>
        public bool ResizeVertically
        {
            get { return FieldCodeCache.HasSwitch(ResizeVerticallySwitch); }
            set { FieldCodeCache.SetSwitch(ResizeVerticallySwitch, value); }
        }

        [CppSkipEntity("C++ doesn't support interface properties and properties with the same names")]
        string IFieldIncludePictureCode.SourceFullName
        {
            get { return SourceFullName; }
        }

        [CppSkipEntity("C++ doesn't support interface properties and properties with the same names")]
        bool IFieldIncludePictureCode.IsLinked
        {
            get { return IsLinked; }
        }

        private const int SourceFullNameArgumentIndex = 0;

        private const string GraphicFilterSwitch = "\\c";
        private const string IsLinkedSwitch = "\\d";
        private const string ResizeHorizontallySwitch = "\\x";
        private const string ResizeVerticallySwitch = "\\y";
    }
}
