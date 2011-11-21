using EtoolTech.MongoDB.Mapper.Interfaces;

namespace EtoolTech.MongoDB.Mapper.Core
{
    public class FindOptions : IFindOptions
    {
        public string FieldName { get; set; }
        public object Value { get; set; }
        public FindCondition FindCondition { get; set; }
    }
}