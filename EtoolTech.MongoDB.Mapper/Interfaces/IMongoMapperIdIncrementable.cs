namespace EtoolTech.MongoDB.Mapper.Interfaces
{
    interface IMongoMapperIdIncrementable
    {
        bool IncrementalId { get; }
        bool IncrementalChildId { get; }
    }
}
