using System.Collections.Generic;

namespace EtoolTech.MongoDB.Mapper.Interfaces
{
    public interface ICalcField
    {
        decimal Sum(string ClassName, string FieldName, object Value);
        int Count(string ClassName, string FieldName, object Value);
        List<T> LookUp<T>(string ClassName, string FieldName, object Value);
    }
}