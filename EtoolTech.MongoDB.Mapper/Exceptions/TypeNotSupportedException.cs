using System;

namespace EtoolTech.MongoDB.Mapper.Exceptions
{
    [Serializable]
    public class TypeNotSupportedException : Exception
    {
        private string typeName;
        public TypeNotSupportedException(string TypeName)
        {
            typeName = TypeName;
        }

        public override string Message
        {
            get { return String.Format("{0} not Supported", typeName); }
        }
    }
}