// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2011 by Roman Korchagin

namespace Aspose.Ss.Property
{
    /// <summary>
    /// Represents property value with its optional name.
    /// </summary>
    public class Property
    {
        public Property(int id, string name, object value)
        {
            mId = id;
            mName = name;
            mValue = value;
        }

        public bool HasName
        {
            get { return StringUtil.HasChars(Name); }
        }

        /// <summary>
        /// Property mId.
        /// </summary>
        public int Id
        {
            get { return mId; }
            set { mId = value; }
        }

        /// <summary>
        /// Optional mName of the property. Normally only user defined properties have names
        /// in the structured storage.
        /// </summary>
        public string Name
        {
            get { return mName; }
            set { mName = value; }
        }

        /// <summary>
        /// The mValue of the property.
        /// </summary>
        public object Value {
            get { return mValue; }
            set { mValue = value; }
        }

        private int mId;
        private string mName;
        private object mValue;
    }
}
