using System;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace EtoolTech.MongoDB.Mapper
{
    public interface IWriter
    {
        bool Insert<T>(string Name, Type Type, T Document);
        bool Update<T>(string Name, Type Type, T Document);
        bool Delete<T>(string Name, Type Type, T Document);

        Task InsertAsync<T>(string Name, Type Type, T Document);
        Task<ReplaceOneResult> UpdateAsync<T>(string Name, Type Type, T Document);
        Task<DeleteResult> DeleteAsync<T>(string Name, Type Type, T Document);

    }
}