namespace EtoolTech.MongoDB.Mapper.Exceptions
{
    using System;

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
            this._relation = relation;
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