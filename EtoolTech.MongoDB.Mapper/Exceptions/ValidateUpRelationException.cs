using System;

namespace EtoolTech.MongoDB.Mapper.Exceptions
{
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
            _relation = Relation;
        }

        #endregion

        #region Public Properties

        public override string Message
        {
            get { return "Error Validatin Relation " + _relation; }
        }

        #endregion
    }
}