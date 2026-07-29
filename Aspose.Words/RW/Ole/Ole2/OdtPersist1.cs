// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/04/2012 by Alexey Morozov

using System;

namespace Aspose.Words.RW.Ole.Ole2
{
    /// <summary>
    /// Implements 2.9.166 ODTPersist1. See http://msdn.microsoft.com/en-us/library/gg132458(v=office.12).aspx
    /// </summary>
    [Flags]
    internal enum OdtPersist1
    {
        None = 0x0000,
        Reserved1 = 0x0001,
        /// <summary>
        ///  If set application MUST assume that this OLE object’s class identifier (CLSID) is {00020907-0000-0000-C000-000000000046}.
        /// </summary>
        DefHandler = 0x0002,
        Reserved2 = 0x0004,
        Reserved3 = 0x0008,
        /// <summary>
        /// Specifies whether this OLE object is a link.
        /// </summary>
        Link = 0x0010,
        Reserved4 = 0x0020,
        /// <summary>
        /// Specifies whether this OLE object is being represented by an icon.
        /// </summary>
        Icon = 0x0040,
        /// <summary>
        /// Specifies whether this OLE object is only compatible with OLE 1.
        /// </summary>
        Ole1 = 0x0080,
        /// <summary>
        /// Specifies whether the user has requested that this OLE object only be updated in response to a user action.
        ///  If Link is zero, then Manual is undefined and MUST be ignored.
        /// </summary>
        Manual = 0x0100,
        /// <summary>
        /// Specifies whether this OLE object has requested to be notified when it is resized by its container.
        /// </summary>
        RecomposeOnResize = 0x0200,
        Reserved5 = 0x0400,
        Reserved6 = 0x0800,
        /// <summary>
        /// Specifies whether this object is an OLE control.
        /// </summary>
        Ocx = 0x1000,
        /// <summary>
        /// If OCX is zero, then this bit MUST be zero. 
        /// If OCX is set, then Stream specifies whether this OLE control stores its data in a single stream instead of a storage. 
        /// If Stream is set, then the data for the OLE control is in a stream called "\003OCXDATA".
        /// </summary>
        Stream = 0x2000,
        Reserved7 = 0x4000,
        /// <summary>
        /// Specifies whether this OLE object supports the IViewObject interface.
        /// </summary>
        SupportIViewObject = 0x8000
    }
}
