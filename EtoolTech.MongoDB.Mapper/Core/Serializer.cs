using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using ServiceStack.Text;

namespace EtoolTech.MongoDB.Mapper.Core
{
    public static class Serializer
    {
        public static string Serialize(object o)
        {
            string data = TypeSerializer.SerializeToString(o);
            return data;
        }

        public static T Deserialize<T>(string data)
        {
            return TypeSerializer.DeserializeFromString<T>(data);
        }        
    }
}
