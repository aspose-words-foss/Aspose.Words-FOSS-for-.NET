// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/08/2007 by Vladimir Averkin

using Aspose.Words.Drawing.Core;
using Aspose.Words.RW.Nrx.Writer;

namespace Aspose.Words.RW.Vml
{
    /// <summary>
    /// Writes 'o:lock' sub-element of v:shape to WordML.
    /// </summary>
    internal class VmlLockWriter
    {
        internal VmlLockWriter(NrxXmlBuilder builder)
        {
            mBuilder = builder;
        }

        /// <summary>
        /// Add 'o:lock' related attribute.
        /// </summary>
        /// <param name="key">Attribute key.</param>
        /// <param name="value">Attribute value.</param>
        internal void AddAttribute(int key, object value)
        {
            switch (key)
            {
                case ShapeAttr.LockAdjustHandles:
                {
                    // Ignored.
                    break;
                }
                case ShapeAttr.LockAgainstGrouping:
                {
                    // Ignored.
                    break;
                }
                case ShapeAttr.LockAgainstSelect:
                {
                    // Ignored.
                    break;
                }
                case ShapeAttr.LockAspectRatio:
                {
                    mLockAspectRatio = VmlUtil.BoolToVml(value);
                    mAttrCount++;
                    break;
                }
                case ShapeAttr.LockCropping:
                {
                    // Ignored.
                    break;
                }
                case ShapeAttr.LockPosition:
                {
                    // Ignored.
                    break;
                }
                case ShapeAttr.LockRotation:
                {
                    // Ignored.
                    break;
                }
                case ShapeAttr.LockShapeType:
                {
                    if ((bool)value)
                    {
                        mLockShapeType = VmlUtil.BoolToVml(value, false);
                        mAttrCount++;
                    }
                    break;
                }
                case ShapeAttr.LockText:
                {
                    if ((bool)value)
                    {
                        mLockText = VmlUtil.BoolToVml(value, false);
                        mAttrCount++;
                    }
                    break;
                }
                case ShapeAttr.LockVertices:
                {
                    if ((bool)value)
                    {
                        mLockVertices = VmlUtil.BoolToVml(value, false);
                        mAttrCount++;
                    }
                    break;
                }
                default:
                    return;
            }
        }

        /// <summary>
        /// Write 'o:lock' element.
        /// </summary>
        internal void Write()
        {
            if (mAttrCount <= 0)
                return;

            mBuilder.StartElement("o:lock");

            mBuilder.WriteAttribute("v:ext", "edit");
            mBuilder.WriteAttribute("aspectratio", mLockAspectRatio);
            mBuilder.WriteAttribute("verticies", mLockVertices);
            mBuilder.WriteAttribute("text", mLockText);
            mBuilder.WriteAttribute("shapetype", mLockShapeType);

            mBuilder.EndElement(); //o:lock
        }

        private readonly NrxXmlBuilder mBuilder;

        private int mAttrCount;

        private string mLockAspectRatio;
        private string mLockVertices;
        private string mLockText;
        private string mLockShapeType;

        //v:ext            Yes    
        //aspectratio    Yes    
        //vertices        Yes    
        //text            Yes    
        //shapetype        Yes    
        //adjusthandles    No need    Ignored in WordML.
        //cropping        No need    Ignored in WordML.
        //grouping        No need    Ignored in WordML.
        //position        No need    Ignored in WordML.
        //rotation        No need    Ignored in WordML.
        //selection        No need    Ignored in WordML.
    }
}
