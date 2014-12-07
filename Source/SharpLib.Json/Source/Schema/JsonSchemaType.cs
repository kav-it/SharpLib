using System;

namespace SharpLib.Json.Schema
{
    [Flags]
    public enum JsonSchemaType
    {
        None = 0,

        String = 1,

        Float = 2,

        Integer = 4,

        Boolean = 8,

        Object = 16,

        Array = 32,

        Null = 64,

        Any = String | Float | Integer | Boolean | Object | Array | Null
    }
}