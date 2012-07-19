namespace EtoolTech.MongoDB.Mapper.Interfaces
{
    using System;

    public interface IMongoMapperTransaction : IDisposable
    {
        int QueueLenght { get; }

        void Commit();

        void RollBack();

        new void Dispose();
    }
}