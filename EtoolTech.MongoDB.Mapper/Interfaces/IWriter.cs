using System;
using MongoDB.Driver;

namespace EtoolTech.MongoDB.Mapper
{
    public interface IWriter
    {
        WriteConcernResult Insert(string name, Type type, object document);
        WriteConcernResult Update(string name, Type type, object document);       
        WriteConcernResult Delete(string name, Type type, object document);
    }
}