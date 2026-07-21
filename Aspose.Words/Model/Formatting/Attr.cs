// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/11/2010 by Roman Korchagin

namespace Aspose.Words
{
    /// <summary>
    /// Base class for attribute values.
    /// 
    /// This does not do anything yet, but I want to keep it just to remember about the direction I want for the attributes.
    /// At the moment AttrCollection simple stores integers, strings, enums etc. I want this to improve - I want AttrCollection
    /// to store Attr objects. I think this will help to reduce the amount of boxing/unboxing (improve performance) and also
    /// to write better object oriented code for handling attributes of different types.
    /// </summary>
    internal abstract class Attr
    {
    }
}
