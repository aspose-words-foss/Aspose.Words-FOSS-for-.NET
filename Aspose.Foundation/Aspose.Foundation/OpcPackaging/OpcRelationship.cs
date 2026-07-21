// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/07/2007 by Vladimir Averkin

namespace Aspose.OpcPackaging
{
    /// <summary>
    /// Represents an OPC relationship.
    /// </summary>
    public class OpcRelationship
    {
        public OpcRelationship(string id, string type, string target, bool isExternal)
        {
            // Check that arguments are not empty.
            ArgumentUtil.CheckHasChars(id, "id");
            ArgumentUtil.CheckHasChars(type, "type");
            ArgumentUtil.CheckHasChars(target, "target");

            mId = id;
            mTarget = target;
            mType = type;
            mIsExternal = isExternal;
        }

        /// <summary>
        /// Clones this instance of relationship.
        /// </summary>
        internal OpcRelationship Clone()
        {
            return (OpcRelationship)MemberwiseClone();
        }

        public string Id
        {
            get { return mId; }
        }

        public string Type
        {
            get { return mType; }
        }

        /// <summary>
        /// This value is as read from the package (escaped URI).
        /// 
        /// When the target is internal, can be absolute or relative.
        /// If absolute, then it basically equals the target part name.
        /// If relative, it needs to be converted to absolute to form a complete target part name.
        /// 
        /// When the target is external, this is probably always absolute. 
        /// </summary>
        public string Target
        {
            get { return mTarget; }
        }

        public bool IsExternal
        {
            get { return mIsExternal; }
        }

        private readonly string mId;
        private readonly string mType;
        private readonly string mTarget;
        private readonly bool mIsExternal;
    }
}
