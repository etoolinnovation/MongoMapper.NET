using System;

namespace EtoolTech.MongoDB.Mapper.Exceptions
{
    [Serializable]
    public class ValidatePropertyException : Exception
    {
        private readonly string _message;
        private readonly string _propertyName;

        public ValidatePropertyException(string PropertyName, string Message)
        {
            _message = Message;
            _propertyName = PropertyName;
        }

        public override string Message
        {
            get { return String.Format("Error Validating Property {0}: {1}", _propertyName, _message); }
        }
    }
}