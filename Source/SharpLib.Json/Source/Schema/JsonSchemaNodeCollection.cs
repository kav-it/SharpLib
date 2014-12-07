using System.Collections.ObjectModel;

namespace SharpLib.Json.Schema
{
    internal class JsonSchemaNodeCollection : KeyedCollection<string, JsonSchemaNode>
    {
        #region ������

        protected override string GetKeyForItem(JsonSchemaNode item)
        {
            return item.Id;
        }

        #endregion
    }
}