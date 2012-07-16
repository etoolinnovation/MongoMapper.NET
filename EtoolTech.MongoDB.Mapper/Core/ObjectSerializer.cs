namespace EtoolTech.MongoDB.Mapper.Core
{
    using ServiceStack.Text;

    public static class ObjectSerializer
    {
        #region Public Methods

        public static T DeserializeFromString<T>(string data)
        {
            return TypeSerializer.DeserializeFromString<T>(data);
        }

        public static string SerializeToString(object o)
        {
            return TypeSerializer.SerializeToString(o);
        }

        #endregion
    }
}