namespace EtoolTech.MongoDB.Mapper.Exceptions
{
    using System;

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
            this._message = message;
            this._propertyName = propertyName;
        }

        #endregion

        #region Public Properties

        public override string Message
        {
            get
            {
                return String.Format("Error Validating Property {0}: {1}", this._propertyName, this._message);
            }
        }

        #endregion
    }
}