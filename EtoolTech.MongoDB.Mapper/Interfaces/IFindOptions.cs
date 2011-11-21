using EtoolTech.MongoDB.Mapper.Core;

namespace EtoolTech.MongoDB.Mapper.Interfaces
{
    public interface IFindOptions
    {
        string FieldName { get; set; }
        object Value { get; set; }
        FindCondition FindCondition { get; set; }
    }
}