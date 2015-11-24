using System;
using System.Linq;

namespace EtoolTech.MongoDB.Mapper.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class MongoRelation : Attribute
    {

        public MongoRelation(string CurrentFieldNames, string RelationObjectName, string RelationFieldNames, bool UpRelation = false)
        {
            this.CurrentFieldNames = CurrentFieldNames.Split(',');
            this.RelationObjectName = RelationObjectName;
            this.RelationFieldNames = RelationFieldNames.Split(',');
            this.UpRelation = UpRelation;
        }
        
        #region Constants and Fields

        internal string[] CurrentFieldNames;

        internal string RelationObjectName;

        internal string[] RelationFieldNames;

        internal bool UpRelation { get; set; }

        #endregion
    }
}