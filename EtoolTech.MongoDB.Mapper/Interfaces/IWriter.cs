using System;
using MongoDB.Driver;

namespace EtoolTech.MongoDB.Mapper
{
    public interface IWriter
    {
        bool Insert<T>(string Name, Type Type, T Document);
        bool Update<T>(string Name, Type Type, T Document);
        bool Delete<T>(string Name, Type Type, T Document);
    }
}