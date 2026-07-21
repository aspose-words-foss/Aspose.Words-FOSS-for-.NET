// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/10/2015 by Alexey Morozov

using System;
using System.IO;
using Aspose.JavaAttributes;
using Aspose.Ss;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Drawing.Ole.Core;
using Aspose.Words.Forms2;
using Aspose.Words.RW.Ole.Ole2;

namespace Aspose.Words.Drawing.Ole
{
    /// <summary>
    /// Represents OLE ActiveX control.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-ole-objects/">Working with Ole Objects</a> documentation article.</para>
    /// </summary>
    public class OleControl : IEmbeddedObject
    {
        /// <summary>
        /// Creates instance of the <see cref="OleControl"/> object with a specified name.
        /// </summary>
        internal OleControl(string name)
        {
            mName = name;
            mId = UniqueIdManager.GenerateInteger();
        }

        /// <summary>
        /// Returns instance of the <see cref="OleControl"/> object created from a specified storage,
        /// or <c>null</c> if the storage is not a <see cref="OleControl"/> object.
        /// </summary>
        internal static OleControl Create(MemoryStorage storage)
        {
            if (!IsOleControl(storage))
                return null;

            string name = GetName(storage);

            OleControl oleControl;
            switch (storage.Clsid.ToString())
            {
                case HtmlOptionClsid:
                    oleControl = new HtmlOptionOleControl(name);
                    break;
                case HtmlSelectClsid:
                case HtmlSubmitButtonClsid:
                case HtmlHiddenClsid:
                case HtmlTextClsid:
                    oleControl = new HtmlOleControl(name);
                    break;

                case OptionButtonControlClsid:
                    oleControl = new OptionButtonControl(name);
                    break;
                case LabelControlClsid:
                    oleControl = new LabelControl(name);
                    break;
                case TextBoxControlClsid:
                    oleControl = new TextBoxControl(name);
                    break;
                case CheckBoxControlClsid:
                    oleControl = new CheckBoxControl(name);
                    break;
                case ToggleButtonControlClsid:
                    oleControl = new ToggleButtonControl(name);
                    break;
                case SpinButtonControlClsid:
                    oleControl = new SpinButtonControl(name);
                    break;
                case ComboBoxControlClsid:
                    oleControl = new ComboBoxControl(name);
                    break;
                case CommandButtonControlClsid:
                    oleControl = new CommandButtonControl(name);
                    break;
                case ImageControlClsid:
                    oleControl = new ImageControl(name);
                    break;
                case ScrollBarControlClsid:
                    oleControl = new ScrollBarControl(name);
                    break;
                case TabStripControlClsid:
                    oleControl = new TabStripControl(name);
                    break;
                case ListBoxControlClsid:
                    oleControl = new ListBoxControl(name);
                    break;
                case FormControlClsid:
                    oleControl = new FormControl(name);
                    break;
                case FrameControlClsid:
                    oleControl = new FrameControl(name);
                    break;
                case MultiPageControlClsid:
                    oleControl = new MultiPageControl(name);
                    break;
                default:
                    oleControl = new OleControl(name);
                    break;
            }

            oleControl.Read(storage);
            return oleControl;
        }

        /// <summary>
        ///Returns true, if a specified Clsid corresponds to <see cref="Forms2OleControl"/>.
        /// </summary>
        internal static bool IsForms2OleClsid(string clsid)
        {
            return clsid.Equals(OptionButtonControlClsid, StringComparison.OrdinalIgnoreCase) ||
                   clsid.Equals(LabelControlClsid, StringComparison.OrdinalIgnoreCase) ||
                   clsid.Equals(TextBoxControlClsid, StringComparison.OrdinalIgnoreCase) ||
                   clsid.Equals(CheckBoxControlClsid, StringComparison.OrdinalIgnoreCase) ||
                   clsid.Equals(ToggleButtonControlClsid, StringComparison.OrdinalIgnoreCase) ||
                   clsid.Equals(SpinButtonControlClsid, StringComparison.OrdinalIgnoreCase) ||
                   clsid.Equals(ComboBoxControlClsid, StringComparison.OrdinalIgnoreCase) ||
                   clsid.Equals(CommandButtonControlClsid, StringComparison.OrdinalIgnoreCase) ||
                   clsid.Equals(ImageControlClsid, StringComparison.OrdinalIgnoreCase) ||
                   clsid.Equals(ScrollBarControlClsid, StringComparison.OrdinalIgnoreCase) ||
                   clsid.Equals(TabStripControlClsid, StringComparison.OrdinalIgnoreCase) ||
                   clsid.Equals(ListBoxControlClsid, StringComparison.OrdinalIgnoreCase) ||
                   clsid.Equals(FormControlClsid, StringComparison.OrdinalIgnoreCase) ||
                   clsid.Equals(FrameControlClsid, StringComparison.OrdinalIgnoreCase) ||
                   clsid.Equals(MultiPageControlClsid, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Reads the control from a storage.
        /// </summary>
        [JavaThrows(true)]
        internal virtual void Read(MemoryStorage storage)
        {
            mParentOleObjectData = storage;
        }

        /// <summary>
        /// Reads the control from a binary reader.
        /// </summary>
        [JavaThrows(true)]
        internal virtual void Read(BinaryReader reader)
        {
        }

        /// <summary>
        /// Writes the control to a binary writer.
        /// </summary>
        [JavaThrows(true)]
        internal virtual uint Write(BinaryWriter writer)
        {
            return 0;
        }

        /// <summary>
        /// Writes the control to a storage.
        /// </summary>
        [JavaThrows(true)]
        internal virtual void Write(MemoryStorage storage)
        {
        }

        /// <summary>
        /// Adds CompObj stream to OLE data storage.
        /// </summary>
        private void AddCompObjStream(MemoryStorage oleData)
        {
            CompObjStream compObjStream = new CompObjStream();
            compObjStream.ProgId = ProgId;
            compObjStream.Clsid = new Guid(ClsidVirtual);
            compObjStream.UserType = UserType;
            compObjStream.AnsiClipboardFormat = ClipboardFormat;
            compObjStream.Write(oleData);
        }

        /// <summary>
        /// Returns name of the control from <see cref="OcxNameStream"/> of a specified storage.
        /// </summary>
        private static string GetName(MemoryStorage storage)
        {
            OcxNameStream ocxName = OcxNameStream.Read(storage);
            return (ocxName != null) ? ocxName.Value : string.Empty;
        }

        /// <summary>
        /// Returns true, if a specified storage is OLE control.
        /// </summary>
        private static bool IsOleControl(MemoryStorage storage)
        {
            ObjInfoStream objInfo = ObjInfoStream.Read(storage);

            return (objInfo != null) && ((objInfo.Flags1 & OdtPersist1.Ocx) != 0);
        }

        /// <summary>
        /// Gets or sets name of the ActiveX control.
        /// </summary>
        public string Name
        {
            get { return mName; }
            set
            {
                ArgumentUtil.CheckHasChars(value, "Name");
                mName = value;
            }
        }

        /// <summary>
        /// Returns <c>true</c> if the control is a <see cref="Forms2OleControl"/>.
        /// </summary>
        public bool IsForms2OleControl
        {
            get { return IsForms2OleClsid(ClsidVirtual); }
        }

        /// <summary>
        /// Gets a ClipboardFormat of the control.
        /// </summary>
        internal virtual ClipboardFormat ClipboardFormat
        {
            get { return null; }
        }

        /// <summary>
        /// Returns a name of the embedded object.
        /// </summary>
        [JavaDelete]
        string IEmbeddedObject.GetName()
        {
            return mName;
        }

        /// <summary>
        /// Returns instance of <see cref="OleObject"/> object.
        /// </summary>
        OleObject IEmbeddedObject.GetOleObject()
        {
            OcxNameStream ocxName;
            if (mParentOleObjectData != null)
            {
                // As there exists memory storage of the parent OleObject, then this is unparsed OleControl.
                // For the moment, only Name can be changed in such controls.
                ocxName = new OcxNameStream(Name);
                ocxName.Write(mParentOleObjectData);
                return new OleObject(mParentOleObjectData);
            }

            MemoryStorage storage = new MemoryStorage(new Guid(ClsidVirtual));
            AddCompObjStream(storage);

            // IN. Actually, ObjInfo and OcxName streams are present only in DOC and RTF. But Word is happy with these streams
            // in other formats as well. See additional comments made by RK in WmlWriter.WriteOleControl().
            ObjInfoStream objInfo = ObjInfoStream.DefaultControl();
            objInfo.Write(storage);

            ocxName = new OcxNameStream(Name);
            ocxName.Write(storage);

            Write(storage);

            return new OleObject(storage);
        }

        /// <summary>
        /// Returns the extension (with the leading dot).
        /// The extension must be for the inner (e.g. unwrapped) data that can be saved directly to a file.
        /// </summary>
        string IEmbeddedObject.GetExtensionForUser(string progId)
        {
            return string.Empty;
        }

        string IEmbeddedObject.GetFileNameForUser()
        {
            return string.Empty;
        }

        /// <summary>
        /// Saves the embedded object data in a way that makes it a valid standalone file.
        /// </summary>
        [JavaInternal]
        void IEmbeddedObject.SaveForUser(Stream stream, IShapeAttrSource attrSource)
        {
            // Nothing to do.
        }

        /// <summary>
        /// Gets a boolean value, indicating either the embedded object is Forms2Ole control.
        /// </summary>
        bool IEmbeddedObject.IsForms2OleControlInternal
        {
            get { return IsForms2OleControl; }
        }

        /// <summary>
        /// Gets a Clsid of the embedded object.
        /// </summary>
        string IEmbeddedObject.ClsidInternal
        {
            get { return ClsidVirtual; }
        }

        /// <summary>
        /// Gets or sets Id of the embedded object.
        /// </summary>
        int IEmbeddedObject.Id
        {
            get { return mId; }
            set { mId = value; }
        }

        bool IEmbeddedObject.IsEmpty
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a Clsid of the embedded object.
        /// </summary>
        protected virtual string ClsidVirtual
        {
            get { return Guid.Empty.ToString(); }
        }

        /// <summary>
        /// Gets a UserType of the control.
        /// </summary>
        protected virtual string UserType
        {
            get { return string.Empty; }
        }

        /// <summary>
        /// Gets a ProgId of the control.
        /// </summary>
        protected virtual string ProgId
        {
            get { return string.Empty; }
        }

        /// <summary>
        /// Represents Id of the embedded object.
        /// </summary>
        private int mId;

        internal const string HtmlOptionClsid = "5512d118-5cc6-11cf-8d67-00aa00bdce1d";

        protected const string HtmlSelectClsid = "5512d122-5cc6-11cf-8d67-00aa00bdce1d";
        protected const string HtmlSubmitButtonClsid = "5512d110-5cc6-11cf-8d67-00aa00bdce1d";
        protected const string HtmlHiddenClsid = "5512d11c-5cc6-11cf-8d67-00aa00bdce1d";
        protected const string HtmlTextClsid = "5512d11a-5cc6-11cf-8d67-00aa00bdce1d";

        protected const string OptionButtonControlClsid = "8bd21d50-ec42-11ce-9e0d-00aa006002f3";
        protected const string LabelControlClsid = "978c9e23-d4b0-11ce-bf2d-00aa003f40d0";
        protected const string TextBoxControlClsid = "8bd21d10-ec42-11ce-9e0d-00aa006002f3";
        protected const string CheckBoxControlClsid = "8bd21d40-ec42-11ce-9e0d-00aa006002f3";
        protected const string ToggleButtonControlClsid = "8bd21d60-ec42-11ce-9e0d-00aa006002f3";
        protected const string SpinButtonControlClsid = "79176fb0-b7f2-11ce-97ef-00aa006d2776";
        protected const string ComboBoxControlClsid = "8bd21d30-ec42-11ce-9e0d-00aa006002f3";
        protected const string CommandButtonControlClsid = "d7053240-ce69-11cd-a777-00dd01143c57";
        protected const string ImageControlClsid = "4c599241-6926-101b-9992-00000b65c6f9";
        protected const string ScrollBarControlClsid = "dfd181e0-5e2f-11ce-a449-00aa004a803d";
        protected const string TabStripControlClsid = "eae50eb0-4a62-11ce-bed6-00aa00611080";
        protected const string ListBoxControlClsid = "8bd21d20-ec42-11ce-9e0d-00aa006002f3";
        protected const string FormControlClsid = "c62a69f0-16dc-11ce-9e98-00aa00574a4f";
        protected const string FrameControlClsid = "6e182020-f460-11ce-9bcd-00aa00608e01";
        protected const string MultiPageControlClsid = "46e31370-3f7a-11ce-bed6-00aa00611080";

        private string mName;

        /// <summary>
        /// Keeps <see cref="OleObject.Data"/> storage of the OleObject from which this control was created.
        /// This is meaningful only for controls that are not parsed (<see cref="Create"/> for details).
        /// </summary>
        private MemoryStorage mParentOleObjectData;
    }
}
