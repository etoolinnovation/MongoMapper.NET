using System;
using MongoDB.Driver;

namespace EtoolTech.MongoDB.Mapper
{
    public interface IWriter
    {
        WriteConcernResult Insert(string Name, Type Type, object Document);
        WriteConcernResult Update(string Name, Type Type, object Document);
        WriteConcernResult Delete(string Name, Type Type, object Document);
    }
}