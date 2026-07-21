// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/10/2006 by Roman Korchagin

using System.Text;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Parses and composes the HYPERLINK field code.
    /// </summary>
    internal class FieldCodeHyperlink
    {
        internal FieldCodeHyperlink()
        {
        }

        /// <summary>
        /// Creates a HYPERLINK field code from parts.
        /// </summary>
        internal FieldCodeHyperlink(
            string address,
            string subAddress,
            string screenTip,
            string target,
            string docLocation,
            bool noHistory)
        {
            Debug.Assert(address != null);
            Debug.Assert(subAddress != null);
            Debug.Assert(screenTip != null);
            Debug.Assert(target != null);
            Debug.Assert(docLocation != null);

            Address = address.Replace("\"", "%22");
            SubAddress = subAddress;
            ScreenTip = screenTip;
            Target = target;
            DocLocation = docLocation;
            NoHistory = noHistory;
        }

        /// <summary>
        /// Parses a HYPERLINK field code string into its parts.
        /// </summary>
        internal static FieldCodeHyperlink Parse(string fieldCode)
        {
            FieldCodeHyperlink result = new FieldCodeHyperlink();

            FieldCode parser = new FieldCode(fieldCode, new FieldHyperlink());

            result.Address = parser.GetArgumentAsString(FieldHyperlink.AddressArgumentIndex, true, true);
            result.SubAddress = parser.GetSwitchArgumentAsString(FieldHyperlink.SubAddressSwitch, true);

            if (parser.HasSwitch(FieldHyperlink.OpenInNewWindowSwitch))
                result.Target = BlankTarget;

            result.ScreenTip = parser.GetSwitchArgumentAsString(FieldHyperlink.ScreenTipSwitch, true);
            result.Target = parser.GetSwitchArgumentAsString(FieldHyperlink.TargetSwitch, true);
            result.DocLocation = parser.GetSwitchArgumentAsString(FieldHyperlink.DocLocationSwitch, true);
            result.NoHistory = parser.HasSwitch(FieldHyperlink.NoHistorySwitch);

            return result;
        }

        /// <summary>
        /// Parses a HYPERLINK field code into its parts.
        /// </summary>
        internal static FieldCodeHyperlink Parse(FieldBundle fieldBundle)
        {
            Debug.Assert(fieldBundle.FieldType == FieldType.FieldHyperlink);

            FieldHyperlink field = (FieldHyperlink)fieldBundle.GetField();

            FieldCodeHyperlink result = new FieldCodeHyperlink();

            result.Address = field.Address;
            result.SubAddress = field.SubAddress;

            if (field.OpenInNewWindow)
                result.Target = BlankTarget;

            result.ScreenTip = field.ScreenTip;
            result.Target = field.Target;
            result.DocLocation = field.DocLocation;
            result.NoHistory = field.NoHistory;

            return result;
        }

        /// <summary>
        /// Builds a complete HYPERLINK field code.
        /// </summary>
        internal string ToFieldCodeString()
        {
            StringBuilder fieldCode = new StringBuilder();
            fieldCode.Append(" HYPERLINK ");

            if (!string.IsNullOrEmpty(Address))
                fieldCode.AppendFormat("{0} ", WordUtil.EncodeMSWordUri(Address));

            AppendSwitch(fieldCode, FieldHyperlink.SubAddressSwitch, SubAddress);
            AppendSwitch(fieldCode, FieldHyperlink.ScreenTipSwitch, ScreenTip);
            AppendSwitch(fieldCode, FieldHyperlink.TargetSwitch, Target);
            AppendSwitch(fieldCode, FieldHyperlink.DocLocationSwitch, DocLocation);

            if (NoHistory)
                fieldCode.AppendFormat(" {0} ", FieldHyperlink.NoHistorySwitch);

            return fieldCode.ToString();
        }

        private static void AppendSwitch(StringBuilder fieldCode, string switchName, string switchValue)
        {
            if (string.IsNullOrEmpty(switchValue))
                return;

            fieldCode.AppendFormat("{0} {1} ", switchName, FieldCodeUpdater.EncodeArgument(switchValue, true));
        }

        internal void NormalizeAddressAndSubAddress()
        {
            if (string.IsNullOrEmpty(Address))
                return;

            if (Address.IndexOf('#') == -1)
                return;

            SubAddress = UriUtil.GetSubAddress(Address);
            Address = UriUtil.GetAddress(Address);
        }

        /// <summary>
        /// Optional hyperlink address. Never null.
        /// </summary>
        internal string Address
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get { return mAddress; }
            set { mAddress = value == null ? string.Empty : value; }
        }

        /// <summary>
        /// Optional hyperlink sub address, specified in the \l switch. Never null.
        /// </summary>
        internal string SubAddress
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get { return mSubAddress; }
            set { mSubAddress = value == null ? string.Empty : value; }
        }

        /// <summary>
        /// Optional screen type, specified in the \o switch. Never null.
        /// </summary>
        internal string ScreenTip
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get { return mScreenTip; }
            set { mScreenTip = value == null ? string.Empty : value; }
        }

        /// <summary>
        /// Optional target frame, specified in the \t switch. Never null.
        /// </summary>
        internal string Target
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get { return mTarget; }
            set { mTarget = value == null ? string.Empty : value; }
        }

        /// <summary>
        /// Address + "#" + SubAddress. Never null.
        /// </summary>
        internal URI HRef
        {
            get { return new URI(Address, SubAddress); }
        }

        /// <summary>
        /// Optional target location, specified in the \s switch. Never null.
        /// </summary>
        internal string DocLocation
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get { return mDocLocation; }
            set { mDocLocation = (value == null) ? string.Empty : value; }
        }

        /// <summary>
        /// Optional flag, specified by the \h switch.
        /// </summary>
        internal bool NoHistory { get; set; }

        private string mAddress = string.Empty;
        private string mSubAddress = string.Empty;
        private string mScreenTip = string.Empty;
        private string mTarget = string.Empty;
        private string mDocLocation = string.Empty;

        private const string BlankTarget = "_blank";
    }
}
