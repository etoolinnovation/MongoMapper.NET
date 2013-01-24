using System;

namespace EtoolTech.MongoDB.Mapper.Exceptions
{
    [Serializable]
    public class ValidateDownRelationException : Exception
    {
        #region Constants and Fields

        private readonly string _relation;

        #endregion

        #region Constructors and Destructors

        public ValidateDownRelationException()
        {
        }

        public ValidateDownRelationException(string relation)
        {
            _relation = relation;
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