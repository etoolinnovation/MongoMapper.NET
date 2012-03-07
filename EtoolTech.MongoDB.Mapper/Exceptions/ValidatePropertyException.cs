using System;

namespace EtoolTech.MongoDB.Mapper.Exceptions
{
    [Serializable]
    public class ValidatePropertyException : Exception
    {
        private readonly string _message;
        private readonly string _propertyName;

        public ValidatePropertyException(string propertyName, string message)
        {
            _message = message;
            _propertyName = propertyName;
        }

        public override string Message
        {
            get { return String.Format("Error Validating Property {0}: {1}", _propertyName, _message); }
        }
    }
}