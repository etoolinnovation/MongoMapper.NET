using ServiceStack.Text;

namespace EtoolTech.MongoDB.Mapper.Core
{
    public static class Serializer
    {
        public static string Serialize(object o)
        {
            return TypeSerializer.SerializeToString(o);            
        }

        public static T Deserialize<T>(string data)
        {
            return TypeSerializer.DeserializeFromString<T>(data);
        }
    }
}