using System;

namespace EtoolTech.MongoDB.Mapper.Exceptions
{
    [Serializable]
    public class TypeNotSupportedException : Exception
    {
        private readonly string _typeName;

        public TypeNotSupportedException(string typeName)
        {
            _typeName = typeName;
        }

        public override string Message
        {
            get { return String.Format("{0} not Supported", _typeName); }
        }
    }
}