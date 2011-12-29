using System;

namespace EtoolTech.MongoDB.Mapper.enums
{
    public enum CommitOperation
    {
        Insert = 0,

        Update = 1,

        Detele = 2
    };

    public class CommitOperationMetaData
    {
        public static string[] Descriptions = new[] {"Insert", "Update", "Delete"};

        public static CommitOperation[] Values = new[]
                                                     {
                                                         CommitOperation.Insert, CommitOperation.Update,
                                                         CommitOperation.Detele
                                                     };

        public static string GetDescription(CommitOperation enumValue)
        {
            return Descriptions[Array.IndexOf(Values, enumValue)];
        }
    }
}