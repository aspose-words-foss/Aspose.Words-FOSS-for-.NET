// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/05/2016 by Alexander Zhiltsov

namespace Aspose.Words.Themes
{
    /// <summary>
    /// The class is intended to store theme object defaults.
    /// </summary>
    /// <remarks>
    /// 20.1.6.7 objectDefaults (Object Defaults)
    /// This element allows for the definition of default shape, line, and textbox formatting properties. 
    /// An application can use this information to format a shape (or text) initially on insertion into a document.
    /// </remarks>
    internal class ThemeObjectDefaults
    {
        /// <summary>
        /// Gets or sets object that defines default shape definition. 
        /// The property may be <c>null</c> that is its default value.
        /// </summary>
        /// <remarks>
        /// 20.1.4.1.27 spDef (Shape Default)
        /// This element defines the formatting that is associated with the default shape. The default formatting
        /// can be applied to a shape when it is initially inserted into a document.
        /// </remarks>
        internal DefaultShapeDefinition ShapeDefaults 
        {
            get { return mShapeDefaults; }
            set { mShapeDefaults = value; }
        }

        /// <summary>
        /// Gets or sets object that defines default line definition. 
        /// The property may be <c>null</c> that is its default value.
        /// </summary>
        /// <remarks>
        /// 20.1.4.1.20 lnDef (Line Default)
        /// This element defines a default line that is used within a document.
        /// </remarks>
        internal DefaultShapeDefinition LineDefaults
        {
            get { return mLineDefaults; }
            set { mLineDefaults = value; }
        }

        /// <summary>
        /// Gets or sets object that defines default text box definition. 
        /// The property may be <c>null</c> that is its default value.
        /// </summary>
        /// <remarks>
        /// 20.1.4.1.28 txDef (Text Default)
        /// This element defines the default formatting which is applied to text in a document by default. The default
        /// formatting can and should be applied to the shape when it is initially inserted into a document.
        /// </remarks>
        internal DefaultShapeDefinition TextDefaults
        {
            get { return mTextDefaults; }
            set { mTextDefaults = value; }
        }

        private DefaultShapeDefinition mShapeDefaults;
        private DefaultShapeDefinition mLineDefaults;
        private DefaultShapeDefinition mTextDefaults;
    }
}
