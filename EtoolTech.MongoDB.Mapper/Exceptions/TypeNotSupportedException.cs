namespace EtoolTech.MongoDB.Mapper.Exceptions
{
    using System;

    [Serializable]
    public class TypeNotSupportedException : Exception
    {
        #region Constants and Fields

        private readonly string _typeName;

        #endregion

        #region Constructors and Destructors

        public TypeNotSupportedException(string typeName)
        {
            this._typeName = typeName;
        }

        #endregion

        #region Public Properties

        public override string Message
        {
            get
            {
                return String.Format("{0} not Supported", this._typeName);
            }
        }

        #endregion
    }
}