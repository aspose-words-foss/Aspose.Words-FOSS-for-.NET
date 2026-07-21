// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/12/2010 by Denis Darkin

namespace Aspose.Words.Math
{
    /// <summary>
    /// This element specifies an instance of mathematical text.
    /// </summary>
    internal class MathObjectOMath : MathObject
    {
        internal override MathObjectType MathObjectType
        {
            get { return MathObjectType.OMath; }
        }

        internal override bool CanBeArgument
        {
            get
            {
                // MS-OI29500, 22.1.2.77.(b) Word fails to open a file with an oMath element inside 
                // a math object argument or inside another oMath element. 
                return false;
            }
        }

        internal override bool NeedsMathObjectArgumentWrapper
        {
            get { return false; }
        }

    }
}
