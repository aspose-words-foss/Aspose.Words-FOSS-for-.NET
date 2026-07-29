// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/02/2010 by Denis Darkin

namespace Aspose.Words.Settings
{
    /// <summary>
    /// Custom compatibility settings. 
    /// The semantics for this element are implementation defined by the creator.
    /// </summary>
    /// <remarks>Defined in ISO 29500 spec, Part 1, §17.15.3.4</remarks>
    internal class CustomCompatibilitySetting
    {
        internal CustomCompatibilitySetting(string name, string uri, string value)
        {
            mName = name;
            mUri = uri;
            mValue = value;
        }
        
        /// <summary>
        /// Specifies the name of a custom compatibility setting.
        /// </summary>
        internal string Name
        {
            get { return mName; }
        }
        
        /// <summary>
        /// Specifies the namespace under which the compatibility setting is defined.
        /// </summary>
        internal string Uri
        {
            get { return mUri; }
        }


        /// <summary>
        /// Specifies the value of a custom compatibility setting.
        /// This value is interpreted using the implementation-defined behavior 
        /// published by the creator of this property.
        /// </summary>
        internal string Value
        {
            get { return mValue; }
            set { mValue = value; }
        }

        internal CustomCompatibilitySetting Clone()
        {
            return (CustomCompatibilitySetting) this.MemberwiseClone();
        }

        private readonly string mName;
        private readonly string mUri;
        private string mValue;
    }
}
