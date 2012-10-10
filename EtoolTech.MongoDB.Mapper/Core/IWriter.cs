using System;
using MongoDB.Driver;

namespace EtoolTech.MongoDB.Mapper
{
    public interface IWriter
    {
        SafeModeResult Insert(string name, Type type, object document);
        SafeModeResult Update(string name, Type type, object document);       
        SafeModeResult Delete(string name, Type type, object document);
    }
}