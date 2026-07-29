// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/10/2011 by Dmitry Vorobyev

using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements the IMPORT field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// Retrieves the picture contained in the document.
    /// </remarks>
    public class FieldImport : Field, IFieldCodeTokenInfoProvider, IFieldIncludePictureCode
    {
        internal override FieldUpdateAction UpdateCore()
        {
            return FieldIncludePictureUpdater.Update(this);
        }

        /// <summary>
        /// Gets or sets the location of the picture.
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

        FieldSwitchType IFieldCodeTokenInfoProvider.GetSwitchType(string switchName)
        {
            switch (switchName)
            {
                case GraphicFilterSwitch:
                    return FieldSwitchType.HasArgument;
                case IsLinkedSwitch:
                    return FieldSwitchType.Flag;
                default:
                    return FieldSwitchType.Unknown;
            }
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
    }
}
