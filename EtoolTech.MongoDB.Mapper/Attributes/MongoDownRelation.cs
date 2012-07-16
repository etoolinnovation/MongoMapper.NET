namespace EtoolTech.MongoDB.Mapper.Attributes
{
    using System;

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
            get
            {
                return this._relation;
            }
            set
            {
                //TODO: Validar formato
                this._relation = value;
                string[] values = this._relation.Split(',');
                this.ObjectName = values[0];
                this.FieldName = values[1];
            }
        }

        #endregion
    }
}