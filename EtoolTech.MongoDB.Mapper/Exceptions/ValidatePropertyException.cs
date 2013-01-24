using System;

namespace EtoolTech.MongoDB.Mapper.Exceptions
{
    [Serializable]
    public class ValidatePropertyException : Exception
    {
        #region Constants and Fields

        private readonly string _message;

        private readonly string _propertyName;

        #endregion

        #region Constructors and Destructors

        public ValidatePropertyException(string propertyName, string message)
        {
            _message = message;
            _propertyName = propertyName;
        }

        #endregion

        #region Public Properties

        public override string Message
        {
            get { return String.Format("Error Validating Property {0}: {1}", _propertyName, _message); }
        }

        #endregion
    }
}