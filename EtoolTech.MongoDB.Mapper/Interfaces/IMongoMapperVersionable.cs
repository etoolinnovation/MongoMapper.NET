namespace EtoolTech.MongoDB.Mapper.Interfaces
{
    public interface IMongoMapperVersionable
    {
        long m_dv { get; set; }
        bool IsLastVersion();
        bool IsLastVersion(bool Force);
        void FillFromLastVersion();
        void FillFromLastVersion(bool Force);
    }
}