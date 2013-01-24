using System;

namespace EtoolTech.MongoDB.Mapper.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class MongoDownRelation : Attribute
    {
        #region Constants and Fields

        public string FieldName;

        public string ObjectName;

        private string _relation;

        #endregion

        #region Public Properties

        public string Relation
        {
            get { return _relation; }
            set
            {
                //TODO: Validar formato
                _relation = value;
                string[] values = _relation.Split(',');
                ObjectName = values[0];
                FieldName = values[1];
            }
        }

        #endregion
    }
}