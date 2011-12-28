using System;

namespace EtoolTech.MongoDB.Mapper.Exceptions
{
    [Serializable]
    public class ValidateDownRelationException : Exception
    {
        private readonly string _relation;

        public ValidateDownRelationException()
        {
        }

        public ValidateDownRelationException(string Relation)
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