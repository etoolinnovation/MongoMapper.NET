using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;

namespace EtoolTech.MongoDB.Mapper.Core
{
    public class IntIdGenerator : IIdGenerator 
    {
        public object GenerateId(object container, object document)
        {           
            ObjectId id = (ObjectId) ObjectIdGenerator.Instance.GenerateId(container, document);
            return id.GetHashCode();

        }

        public bool IsEmpty(object id)
        {
            return (int) id == default(int);
        }
    }
}
