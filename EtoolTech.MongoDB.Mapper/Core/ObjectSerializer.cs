using ServiceStack.Text;

namespace EtoolTech.MongoDB.Mapper.Core
{
    public static class ObjectSerializer
    {
        public static string SerializeToString(object o)
        {
            return TypeSerializer.SerializeToString(o);            
        }

        public static T DeserializeFromString<T>(string data)
        {
            return TypeSerializer.DeserializeFromString<T>(data);            
        }
    }
}