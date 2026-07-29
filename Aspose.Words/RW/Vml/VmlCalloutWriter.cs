// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/08/2007 by Vladimir Averkin

using Aspose.Words.Drawing.Core;
using Aspose.Words.RW.Nrx.Writer;


namespace Aspose.Words.RW.Vml
{
    /// <summary>
    /// Writes 'o:callout' sub-element of v:shape to WordML.
    /// </summary>
    internal class VmlCalloutWriter
    {
        internal VmlCalloutWriter(NrxXmlBuilder builder)
        {
            mBuilder = builder;
        }

        /// <summary>
        /// Add 'o:callout' related attribute.
        /// </summary>
        /// <param name="key">Attribute key.</param>
        /// <param name="value">Attribute value.</param>
        internal void AddAttribute(int key, object value)
        {
            // Determine if the added attribute is written inside 'o:callout' element.
            // This is done to avoid writing of the empty 'o:callout' declaration.
            if (value is bool)
                switch (key)
                {
                    // these values are true by default
                    case ShapeAttr.CalloutOn:
                    case ShapeAttr.CalloutTextBorder:
                        if (!(bool)value)
                            mAttrCount++;
                        break;
                    // other possible values are false by default
                    default:
                        if ((bool)value)
                            mAttrCount++;
                        break;
                }
            else
                mAttrCount++;

            switch (key)
            {
                case ShapeAttr.CalloutAccentBar:
                    {
                        mCalloutAccentBar = VmlUtil.BoolToVml(value, false);
                        break;
                    }
                case ShapeAttr.CalloutAngle:
                    {
                        mCalloutAngle = VmlEnum.CalloutAngleToVml((CalloutAngle)value);
                        break;
                    }
                case ShapeAttr.CalloutDropAuto:
                    {
                        mCalloutDropAuto = VmlUtil.BoolToVml(value, false);
                        break;
                    }
                case ShapeAttr.CalloutDropDistance:
                    {
                        mCalloutDropDistance = VmlUtil.EmuToVmlPoints((int)value);
                        break;
                    }
                case ShapeAttr.CalloutDropType:
                    {
                        mCalloutDropType = VmlEnum.CalloutDropTypeToVml((CalloutDropType)value);
                        break;
                    }
                case ShapeAttr.CalloutGap:
                    {
                        mCalloutGap = VmlUtil.EmuToVmlPoints((int)value);
                        break;
                    }
                case ShapeAttr.CalloutLength:
                    {
                        mCalloutLength = VmlUtil.EmuToVmlMillimeters((int)value);
                        break;
                    }
                case ShapeAttr.CalloutLengthSpecified:
                    {
                        mCalloutLengthSpecified = VmlUtil.BoolToVml(value, false);
                        break;
                    }
                case ShapeAttr.CalloutMinusX:
                    {
                        mCalloutMinusX = VmlUtil.BoolToVml(value, false);
                        break;
                    }
                case ShapeAttr.CalloutMinusY:
                    {
                        mCalloutMinusY = VmlUtil.BoolToVml(value, false);
                        break;
                    }
                case ShapeAttr.CalloutOn:
                    {
                        mCalloutOn = VmlUtil.BoolToVml(value, true);
                        break;
                    }
                case ShapeAttr.CalloutTextBorder:
                    {
                        mCalloutTextBorder = VmlUtil.BoolToVml(value, true);
                        break;
                    }
                case ShapeAttr.CalloutType:
                    {
                        mCalloutType = VmlEnum.CalloutTypeToVml((CalloutType)value);
                        break;
                    }
                default:
                    return;
            }
        }

        /// <summary>
        /// Write 'o:callout' element.
        /// </summary>
        internal void Write()
        {
            if (mAttrCount <= 0)
                return;

            mBuilder.StartElement("o:callout");

            mBuilder.WriteAttribute("v:ext", "edit");
            mBuilder.WriteAttribute("type", mCalloutType);
            mBuilder.WriteAttribute("gap", mCalloutGap);
            mBuilder.WriteAttribute("angle", mCalloutAngle);
            mBuilder.WriteAttribute("drop", mCalloutDropType);
            mBuilder.WriteAttribute("distance", mCalloutDropDistance);
            mBuilder.WriteAttribute("length", mCalloutLength);
            mBuilder.WriteAttribute("on", mCalloutOn);
            mBuilder.WriteAttribute("accentbar", mCalloutAccentBar);
            mBuilder.WriteAttribute("textborder", mCalloutTextBorder);
            mBuilder.WriteAttribute("minusx", mCalloutMinusX);
            mBuilder.WriteAttribute("minusy", mCalloutMinusY);
            mBuilder.WriteAttribute("dropauto", mCalloutDropAuto);
            mBuilder.WriteAttribute("lengthspecified", mCalloutLengthSpecified);

            mBuilder.EndElement(); //o:callout
        }

        private readonly NrxXmlBuilder mBuilder;

        private int mAttrCount = 0;

        private string mCalloutType = null;
        private string mCalloutGap = null;
        private string mCalloutAngle = null;
        private string mCalloutDropType = null;
        private string mCalloutDropDistance = null;
        private string mCalloutLength = null;
        private string mCalloutOn = null;
        private string mCalloutAccentBar = null;
        private string mCalloutTextBorder = null;
        private string mCalloutMinusX = null;
        private string mCalloutMinusY = null;
        private string mCalloutDropAuto = null;
        private string mCalloutLengthSpecified = null;


        //ext        Yes
        //type        Yes
        //gap        Yes
        //angle        Yes
        //drop        Yes
        //distance    Yes
        //length    Yes
        //on        Yes
        //accentbar    Yes
        //textborder    Yes
        //minusx    Yes
        //minusy    Yes
        //dropauto    Yes
        //lengthspecified    Yes
    }
}
