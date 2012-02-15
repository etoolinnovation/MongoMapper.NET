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

        public ValidateDownRelationException(string relation)
        {
            _relation = relation;
        }

        public override string Message
        {
            get { return "Error Validatin Relation " + _relation; }
        }
    }
}