// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/01/2008 by Roman Korchagin
using Aspose.Words.Fields;
using Aspose.Words.Nrx;

namespace Aspose.Words.RW.Docx.Reader
{
    internal static class DocxFfDataReader
    {
        /// <summary>
        /// If 'fldChar' has 'ffData' defined then we should memorize this data and use it later,
        /// when type of this field will become known to us.
        /// Make sure reader is positioned to element start.
        /// </summary>
        internal static void Read(NrxXmlReader xmlReader, OoxmlComplianceInfo complianceInfo)
        {
            FormFieldPr formFieldPr = new FormFieldPr();
            while (xmlReader.ReadChild("ffData"))
            {
                switch (xmlReader.LocalName)
                {
                    case "name":
                        formFieldPr.Name = xmlReader.ReadVal();
                        break;
                    case "enabled":
                        formFieldPr.Enabled = xmlReader.ReadBoolVal();
                        break;
                    case "calcOnExit":
                        formFieldPr.CalcOnExit = xmlReader.ReadBoolVal();
                        break;
                    case "entryMacro":
                        formFieldPr.EntryMacro = xmlReader.ReadVal();
                        break;
                    case "exitMacro":
                        formFieldPr.ExitMacro = xmlReader.ReadVal();
                        break;
                    case "helpText":
                        formFieldPr.OwnHelpText = true; // This is a default if type is missing.
                        while (xmlReader.MoveToNextAttribute())
                        {
                            switch (xmlReader.LocalName)
                            {
                                case "type":
                                    formFieldPr.OwnHelpText = (xmlReader.Value == "text");
                                    break;
                                case "val":
                                    formFieldPr.HelpText = xmlReader.Value;
                                    break;
                                default:
                                    Debug.Fail(xmlReader.LocalName);
                                    break;
                            }
                        }
                        break;
                    case "statusText":
                        formFieldPr.OwnStatusText = true; // This is a default if type is missing.
                        while (xmlReader.MoveToNextAttribute())
                        {
                            switch (xmlReader.LocalName)
                            {
                                case "type":
                                    formFieldPr.OwnStatusText = (xmlReader.Value == "text");
                                    break;
                                case "val":
                                    formFieldPr.StatusText = xmlReader.Value;
                                    break;
                                default:
                                    Debug.Fail(xmlReader.LocalName);
                                    break;
                            }
                        }
                        break;
                    case "textInput":
                        ReadTextInput(xmlReader, formFieldPr);
                        break;
                    case "checkBox":
                        ReadCheckBox(xmlReader, formFieldPr, complianceInfo);
                        break;
                    case "ddList":
                        ReadDropDownList(xmlReader, formFieldPr);
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }

            // Store formFieldPr on the stack for further processing.
            xmlReader.PushFormFieldPr(formFieldPr);
        }

        private static void ReadTextInput(NrxXmlReader partReader, FormFieldPr formFieldPr)
        {
            while (partReader.ReadChild("textInput"))
            {
                switch (partReader.LocalName)
                {
                    case "maxLength":
                        formFieldPr.TextInputMaxLength = partReader.ReadIntVal();
                        break;
                    case "type":
                        formFieldPr.TextInputType = DocxEnum.DocxToTextFormFieldType(partReader.ReadVal());
                        break;
                    case "default":
                        formFieldPr.TextInputDefault = partReader.ReadVal();
                        break;
                    case "format":
                        formFieldPr.TextInputFormat = partReader.ReadVal();
                        break;
                    default:
                        partReader.IgnoreElement();
                        break;
                }
            }
        }

        private static void ReadCheckBox(NrxXmlReader partReader, FormFieldPr formFieldPr, 
            OoxmlComplianceInfo complianceInfo)
        {
            while (partReader.ReadChild("checkBox"))
            {
                switch (partReader.LocalName)
                {
                    case "size":
                        formFieldPr.CheckBoxSizeAuto = false;
                        formFieldPr.CheckBoxSizeHalfPoints = partReader.ReadValAsHalfPoints(complianceInfo);
                        break;
                    case "sizeAuto":
                        formFieldPr.CheckBoxSizeAuto = partReader.ReadBoolVal();
                        break;
                    case "default":
                        formFieldPr.CheckBoxDefault = partReader.ReadBoolVal();
                        break;
                    case "checked":
                        formFieldPr.CheckBoxChecked = partReader.ReadBoolVal();
                        break;
                    default:
                        partReader.IgnoreElement();
                        break;
                }
            }
        }

        private static void ReadDropDownList(NrxXmlReader partReader, FormFieldPr formFieldPr)
        {
            while (partReader.ReadChild("ddList"))
            {
                switch (partReader.LocalName)
                {
                    case "default":
                        formFieldPr.DropDownDefault = partReader.ReadIntVal();
                        break;
                    case "result":
                        formFieldPr.DropDownResult = partReader.ReadIntVal();
                        break;
                    case "listEntry":
                        formFieldPr.DropDownItems.Add(partReader.ReadVal());
                        break;
                    default:
                        partReader.IgnoreElement();
                        break;
                }
            }
        }
    }
}
