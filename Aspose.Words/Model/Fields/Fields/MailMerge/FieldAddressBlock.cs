// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/11/2010 by Dmitry Vorobyev

using Aspose.Collections;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements the ADDRESSBLOCK field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// Represents an address block. An <i>address block</i> is a block of text specifying information
    /// appropriate for a postal mailing address, in the order required by the destination country.
    /// </remarks>
    public class FieldAddressBlock : Field, IFieldCodeTokenInfoProvider
    {
        internal override FieldUpdateAction UpdateCore()
        {
            return MergeFieldUtil.ProcessUnusedField(this, "«AddressBlock»");
        }

        /// <summary>
        /// Gets or sets whether to format the address according to the country/region of the recipient
        /// as defined by POST*CODE (Universal Postal Union 2006).
        /// </summary>
        public bool FormatAddressOnCountryOrRegion
        {
            get { return FieldCodeCache.HasSwitch(FormatAddressOnCountryOrRegionSwitch); }
            set { FieldCodeCache.SetSwitch(FormatAddressOnCountryOrRegionSwitch, value); }
        }

        /// <summary>
        /// Gets or sets whether to include the name of the country/region.
        /// </summary>
        public string IncludeCountryOrRegionName //int
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(IncludeCountryOrRegionNameSwitch); }
            set { FieldCodeCache.SetSwitchAsInt32(IncludeCountryOrRegionNameSwitch, value); }
        }

        /// <summary>
        /// Gets or sets the excluded country/region name.
        /// </summary>
        /// <dev>TODO DV May be more than one.</dev>
        public string ExcludedCountryOrRegionName
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(ExcludedCountryOrRegionNameSwitch); }
            set { FieldCodeCache.SetSwitch(ExcludedCountryOrRegionNameSwitch, value); }
        }

        /// <summary>
        /// Gets or sets the name and address format.
        /// </summary>
        public string NameAndAddressFormat
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(NameAndAddressFormatSwitch); }
            set { FieldCodeCache.SetSwitch(NameAndAddressFormatSwitch, value); }
        }

        /// <summary>
        /// Gets or sets the language ID used to format the address.
        /// </summary>
        public string LanguageId
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(LanguageIdSwitch); }
            set { FieldCodeCache.SetSwitch(LanguageIdSwitch, value); }
        }

        FieldSwitchType IFieldCodeTokenInfoProvider.GetSwitchType(string switchName)
        {
            switch (switchName)
            {
                case FormatAddressOnCountryOrRegionSwitch:
                {
                    return FieldSwitchType.Flag;
                }
                case IncludeCountryOrRegionNameSwitch:
                case ExcludedCountryOrRegionNameSwitch:
                case NameAndAddressFormatSwitch:
                case LanguageIdSwitch:
                {
                    return FieldSwitchType.HasArgument;
                }
                default:
                {
                    return FieldSwitchType.Unknown;
                }
            }
        }

        static FieldAddressBlock()
        {
            // Found at http://msdn.microsoft.com/en-us/library/ff531301(office.12).aspx.

            gPlaceholdersToFieldsMap.Add("TITLE0", "Courtesy Title");
            gPlaceholdersToFieldsMap.Add("NICK0", "Nickname");
            gPlaceholdersToFieldsMap.Add("FIRST0", "First Name");
            gPlaceholdersToFieldsMap.Add("MIDDLE0", "Middle Name");
            gPlaceholdersToFieldsMap.Add("LAST0", "Last Name");
            gPlaceholdersToFieldsMap.Add("SUFFIX0", "Suffix");
            gPlaceholdersToFieldsMap.Add("TITLE1", "Spouse Courtesy Title");
            gPlaceholdersToFieldsMap.Add("NICK1", "Spouse Nickname");
            gPlaceholdersToFieldsMap.Add("FIRST1", "Spouse First Name");
            gPlaceholdersToFieldsMap.Add("MIDDLE1", "Spouse Middle Name");
            gPlaceholdersToFieldsMap.Add("LAST1", "Spouse Last Name");
            gPlaceholdersToFieldsMap.Add("SUFFIX1", "Spouse Suffix");
            gPlaceholdersToFieldsMap.Add("COMPANY", "Company");
            gPlaceholdersToFieldsMap.Add("STREET1", "Address 1");
            gPlaceholdersToFieldsMap.Add("STREET2", "Address 2");
            gPlaceholdersToFieldsMap.Add("CITY", "City");
            gPlaceholdersToFieldsMap.Add("STATE", "State");
            gPlaceholdersToFieldsMap.Add("POSTAL", "Postal Code");
            gPlaceholdersToFieldsMap.Add("COUNTRY", "Country or Region");
        }

        private const string FormatAddressOnCountryOrRegionSwitch = "\\d";
        private const string IncludeCountryOrRegionNameSwitch = "\\c";
        private const string ExcludedCountryOrRegionNameSwitch = "\\e";
        private const string NameAndAddressFormatSwitch = "\\f";
        private const string LanguageIdSwitch = "\\l";

        private static readonly StringToObjDictionary<string> gPlaceholdersToFieldsMap = new
            StringToObjDictionary<string>(false);
    }
}
