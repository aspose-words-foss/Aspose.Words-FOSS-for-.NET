// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/09/2019 by Alexander Sevidov

namespace Aspose.Words.Vba
{
    /// <summary>
    /// Allows to specify the type of a <see cref="VbaReference"/> object.
    /// </summary>
    public enum VbaReferenceType
    {
        /// <summary>
        /// Specifies an Automation type library reference type.
        /// </summary>
        /// <remarks>
        /// This type corresponds to 2.3.4.2.2.5 REFERENCEREGISTERED Record of [MS-OVBA]:
        /// https://docs.microsoft.com/en-us/openspecs/office_file_formats/ms-ovba/6c39388e-96f5-4b93-b90a-ae625a063fcf 
        /// </remarks>
        Registered = 0x0d,

        /// <summary>
        /// Specified an external VBA project reference type.
        /// </summary>
        /// <remarks>
        /// This type corresponds to 2.3.4.2.2.6 REFERENCEPROJECT Record of [MS-OVBA]:
        /// https://docs.microsoft.com/en-us/openspecs/office_file_formats/ms-ovba/08280eb0-d628-495c-867f-5985ed020142 
        /// </remarks>
        Project = 0x0e,

        /// <summary>
        /// Specifies an original Automation type library reference type.
        ///  </summary>
        /// <remarks>
        /// This type corresponds to 2.3.4.2.2.4 REFERENCEORIGINAL Record of [MS-OVBA]:
        /// https://docs.microsoft.com/en-us/openspecs/office_file_formats/ms-ovba/3ba66994-8c7a-4634-b2da-f9331ace6686 
        /// </remarks>
        Original = 0x33,

        /// <summary>
        /// Specifies a twiddled type library reference type.
        ///  </summary>
        /// <remarks>
        /// This type corresponds to 2.3.4.2.2.3 REFERENCECONTROL Record of [MS-OVBA]:
        /// https://docs.microsoft.com/en-us/openspecs/office_file_formats/ms-ovba/d64485fa-8562-4726-9c5e-11e8f01a81c0 
        /// </remarks>
        Control = 0x2f
    }
}
