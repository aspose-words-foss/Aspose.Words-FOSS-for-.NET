// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/08/2004 by Roman Korchagin

using System;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements the HYPERLINK field
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// When selected, causes control to jump to the location such as a bookmark or a URL.
    /// </remarks>
    public class FieldHyperlink : Field, IFieldCodeTokenInfoProvider, IFieldResultFormatProvider
    {
        internal override FieldUpdateAction UpdateCore()
        {
            if (HasResult && !IsErrorResult)
                return new FieldUpdateActionDoNothing(this);

            string title = GetHyperlinkText();
            if (title != null)
                return new FieldUpdateActionApplyResult(this, title);

            return new FieldUpdateActionInsertErrorMessage(this, "Error! Hyperlink reference not valid.");
        }

        internal override void BeforeUnlink()
        {
            NodeRange range = GetFieldResultRange();
            foreach (Node node in range)
            {
                Inline inline = node as Inline;
                if ((inline != null) && (inline.Font.StyleIdentifier == StyleIdentifier.Hyperlink))
                    inline.RunPr.Remove(FontAttr.Istd);
            }
        }

        private bool IsErrorResult
        {
            get { return Result.StartsWith("Error!", StringComparison.Ordinal); }
        }

        private string GetHyperlinkText()
        {
            string address = GetAddressSchemeAware();
            string subAddress = SubAddress;

            if (StringUtil.HasChars(address) && StringUtil.HasChars(subAddress))
                return string.Format("{0} - {1}", address, subAddress);

            if (StringUtil.HasChars(address))
                return address;

            if (StringUtil.HasChars(subAddress))
                return subAddress;

            return null;
        }

        protected override bool NeedStoreOldResultNodes()
        {
            return IsDirectResultUpdate || base.NeedStoreOldResultNodes();
        }

        /// <summary>
        /// Address + "#" + SubAddress. Never null.
        /// </summary>
        internal URI HRef
        {
            get { return new URI(GetAddressSchemeAware(), SubAddress); }
        }

        private string GetAddressSchemeAware()
        {
            return TrimAddressScheme(Address, "url");
        }

        private static string TrimAddressScheme(string address, string scheme)
        {
            if (string.IsNullOrEmpty(address))
                return address;

            if (!address.StartsWith(scheme + ":", StringComparison.Ordinal))
                return address;

            return address.Remove(0, scheme.Length + 1);
        }

        /// <summary>
        /// Gets or sets the target to which the link should be redirected.
        /// </summary>
        public string Target
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(TargetSwitch); }
            set { FieldCodeCache.SetSwitch(TargetSwitch, value); }
        }

        /// <summary>
        /// Gets or sets a location where this hyperlink jumps.
        /// </summary>
        public string Address
        {
            get { return FieldCodeCache.GetArgumentAsString(AddressArgumentIndex); }
            set { FieldCodeCache.SetArgument(AddressArgumentIndex, value); }
        }

        /// <summary>
        /// Gets or sets a location in the file, such as a bookmark, where this hyperlink jumps.
        /// </summary>
        public string SubAddress
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(SubAddressSwitch); }
            set { FieldCodeCache.SetSwitch(SubAddressSwitch, value); }
        }

        /// <summary>
        /// Gets or sets whether to append coordinates to the hyperlink for a server-side image map.
        /// </summary>
        public bool IsImageMap
        {
            get { return FieldCodeCache.HasSwitch(IsImageMapSwitch); }
            set { FieldCodeCache.SetSwitch(IsImageMapSwitch, value); }
        }

        /// <summary>
        /// Gets or sets whether to open the destination site in a new web browser window.
        /// </summary>
        public bool OpenInNewWindow
        {
            get { return FieldCodeCache.HasSwitch(OpenInNewWindowSwitch); }
            set { FieldCodeCache.SetSwitch(OpenInNewWindowSwitch, value); }
        }

        /// <summary>
        /// Gets or sets the ScreenTip text for the hyperlink.
        /// </summary>
        public string ScreenTip
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(ScreenTipSwitch); }
            set { FieldCodeCache.SetSwitch(ScreenTipSwitch, value); }
        }

        /// <summary>
        /// Gets or sets whether the target of the hyperlink should not be added to a list of viewed hyperlinks when the hyperlink is invoked.
        /// </summary>
        /// <remarks>
        /// [MS-OI29500] 2.1.477 Part 1 Section 17.16.5.25, HYPERLINK. -> a.
        /// </remarks>
        internal bool NoHistory
        {
            get { return FieldCodeCache.HasSwitch(NoHistorySwitch); }
            set { FieldCodeCache.SetSwitch(NoHistorySwitch, value); }
        }

        /// <summary>
        /// Gets or sets whether a location in the target of the hyperlink that has no bookmarks.
        /// </summary>
        /// <remarks>
        /// [MS-OI29500] 2.1.477 Part 1 Section 17.16.5.25, HYPERLINK. -> b.
        /// </remarks>
        internal string DocLocation
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(DocLocationSwitch); }
            set { FieldCodeCache.SetSwitch(DocLocationSwitch, value); }
        }

        FieldSwitchType IFieldCodeTokenInfoProvider.GetSwitchType(string switchName)
        {
            switch (switchName)
            {
                case IsImageMapSwitch:
                case OpenInNewWindowSwitch:
                case NoHistorySwitch:
                    return FieldSwitchType.Flag;
                case SubAddressSwitch:
                case ScreenTipSwitch:
                case TargetSwitch:
                case DocLocationSwitch:
                    return FieldSwitchType.HasArgument;
                default:
                    return FieldSwitchType.Unknown;
            }
        }

        Inline IFieldResultFormatProvider.GetSourceNode()
        {
            return null;
        }

        IFieldResultFormatApplier IFieldResultFormatProvider.GetFormatApplier()
        {
            // Retain original formatting from last inline node, when the field result is changed through the Field.Result property.
            // Mimic MS Word "Edit Hyperlink" dialog behaviour.
            if (IsDirectResultUpdate)
                return RetainingFormatApplier.GetFormatApplier(this);

            // Otherwise, apply Hyperlink style.
            return HyperlinkStyleFormatApplier.Instance;
        }

        /// <summary>
        /// Specifies whether the field result is changed through the Field.Result property or not.
        /// </summary>
        private bool IsDirectResultUpdate
        {
            get { return UpdateContext == null; }
        }

        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int AddressArgumentIndex = 0;

        internal const string SubAddressSwitch = "\\l";
        internal const string IsImageMapSwitch = "\\m";
        internal const string OpenInNewWindowSwitch = "\\n";
        internal const string ScreenTipSwitch = "\\o";
        internal const string TargetSwitch = "\\t";
        internal const string NoHistorySwitch = "\\h";
        internal const string DocLocationSwitch = "\\s";

        private class HyperlinkStyleFormatApplier : IFieldResultFormatApplier
        {
            private HyperlinkStyleFormatApplier()
            {
            }

            void IFieldResultFormatApplier.ApplyFormat(NodeRange result)
            {
                foreach (Node node in result)
                {
                    Inline inline = node as Inline;
                    if (inline == null)
                        continue;

                    inline.Font.ClearFormatting();
                    inline.Font.StyleIdentifier = StyleIdentifier.Hyperlink;
                }
            }

            internal static readonly HyperlinkStyleFormatApplier Instance = new HyperlinkStyleFormatApplier();
        }

        private class RetainingFormatApplier : IFieldResultFormatApplier
        {
            internal static RetainingFormatApplier GetFormatApplier(Field field)
            {
                NodeRange range = field.GetFieldResultRange();
                RunPr runPr = null;
                foreach (Node node in range)
                {
                    Inline inline = node as Inline;
                    if (inline == null)
                        continue;

                    runPr = inline.RunPr;
                }

                return new RetainingFormatApplier(runPr);
            }

            private RetainingFormatApplier(RunPr runPr)
            {
                mRunPr = runPr;
            }

            public void ApplyFormat(NodeRange result)
            {
                if (mRunPr == null)
                    return;

                foreach (Node node in result)
                {
                    Inline inline = node as Inline;
                    if (inline == null)
                        continue;

                    inline.RunPr = mRunPr.Clone();
                }
            }

            private readonly RunPr mRunPr;
        }
    }
}
