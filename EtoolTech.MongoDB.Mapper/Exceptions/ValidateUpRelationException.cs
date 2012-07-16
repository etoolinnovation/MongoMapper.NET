namespace EtoolTech.MongoDB.Mapper.Exceptions
{
    using System;

    [Serializable]
    public class ValidateUpRelationException : Exception
    {
        #region Constants and Fields

        private readonly string _relation;

        #endregion

        #region Constructors and Destructors

        public ValidateUpRelationException()
        {
        }

        public ValidateUpRelationException(string Relation)
        {
            this._relation = Relation;
        }

        #endregion

        #region Public Properties

        public override string Message
        {
            get
            {
                return "Error Validatin Relation " + this._relation;
            }
        }

        #endregion
    }
}