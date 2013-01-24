using System;

namespace EtoolTech.MongoDB.Mapper.Exceptions
{
    [Serializable]
    public class TypeNotSupportedException : Exception
    {
        #region Constants and Fields

        private readonly string _typeName;

        #endregion

        #region Constructors and Destructors

        public TypeNotSupportedException(string typeName)
        {
            _typeName = typeName;
        }

        #endregion

        #region Public Properties

        public override string Message
        {
            get { return String.Format("{0} not Supported", _typeName); }
        }

        #endregion
    }
}