using System;

namespace EtoolTech.MongoDB.Mapper.Interfaces
{
    public interface IMongoMapperTransaction : IDisposable
    {
        int QueueLenght { get; }

        void Commit();

        void RollBack();

        new void Dispose();
    }
}