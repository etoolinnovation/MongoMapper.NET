using System;

namespace EtoolTech.MongoDB.Mapper.Exceptions
{
    [Serializable]
    public class ValidateUpRelationException : Exception
    {
        private readonly string _relation;

        public ValidateUpRelationException()
        {
        }

        public ValidateUpRelationException(string Relation)
        {
            _relation = Relation;
        }

        public override string Message
        {
            get
            {
                return "Error Validatin Relation " + _relation;
            }
        }
    }
}