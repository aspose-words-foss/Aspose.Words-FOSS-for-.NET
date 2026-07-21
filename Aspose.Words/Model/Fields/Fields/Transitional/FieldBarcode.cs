// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/10/2011 by Dmitry Vorobyev

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements the BARCODE field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// Inserts a postal barcode in a machine-readable form of address used by the U.S. Postal Service.
    /// </remarks>
    public class FieldBarcode : Field, IFieldCodeTokenInfoProvider
    {
        private BarcodeParameters GetBarcodeParameters()
        {
            BarcodeParameters parameters = new BarcodeParameters();
            parameters.PostalAddress = PostalAddress;
            parameters.IsBookmark = IsBookmark;
            parameters.FacingIdentificationMark = FacingIdentificationMark;
            parameters.IsUSPostalAddress = IsUSPostalAddress;
            return parameters;
        }

        internal override NodeRange GetFakeResult()
        {
            return FieldBarcodeUtil.GetFakeResult(Type, FetchDocument(), GetBarcodeParameters());
        }

        /// <summary>
        /// Gets or sets the postal address used for generating a barcode or the name of the bookmark that refers to it.
        /// </summary>
        public string PostalAddress
        {
            get { return FieldCodeCache.GetArgumentAsString(PostalAddressArgumentIndex); }
            set { FieldCodeCache.SetArgument(PostalAddressArgumentIndex, value); }
        }

        /// <summary>
        /// Gets or sets whether <see cref="PostalAddress"/> is the name of a bookmark.
        /// </summary>
        public bool IsBookmark
        {
            get { return FieldCodeCache.HasSwitch(IsBookmarkSwitch); }
            set { FieldCodeCache.SetSwitch(IsBookmarkSwitch, value); }
        }

        /// <summary>
        /// Gets or sets the type of a Facing Identification Mark (FIM) to insert.
        /// </summary>
        public string FacingIdentificationMark
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(FacingIdentificationMarkSwitch); }
            set { FieldCodeCache.SetSwitch(FacingIdentificationMarkSwitch, value); }
        }

        /// <summary>
        /// Gets or sets whether <see cref="PostalAddress"/> is a U.S. postal address.
        /// </summary>
        public bool IsUSPostalAddress
        {
            get { return FieldCodeCache.HasSwitch(IsUSPostalAddressSwitch); }
            set { FieldCodeCache.SetSwitch(IsUSPostalAddressSwitch, value); }
        }

        FieldSwitchType IFieldCodeTokenInfoProvider.GetSwitchType(string switchName)
        {
            switch (switchName)
            {
                case IsBookmarkSwitch:
                case IsUSPostalAddressSwitch:
                    return FieldSwitchType.Flag;
                case FacingIdentificationMarkSwitch:
                    return FieldSwitchType.HasArgument;
                default:
                    return FieldSwitchType.Unknown;
            }
        }

        private const int PostalAddressArgumentIndex = 0;

        private const string IsBookmarkSwitch = "\\b";
        private const string FacingIdentificationMarkSwitch = "\\f";
        private const string IsUSPostalAddressSwitch = "\\u";
    }
}
