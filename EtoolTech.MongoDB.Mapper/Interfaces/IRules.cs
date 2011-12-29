namespace EtoolTech.MongoDB.Mapper.Interfaces
{
    public interface IRules
    {
        void OnBeforeInsert(object sender, string className);

        void OnAfterInsert(object sender, string className);

        void OnBeforeModify(object sender, string className);

        void OnAfterModify(object sender, string className);

        void OnBeforeDelete(object sender, string className);

        void OnAfterDelete(object sender, string className);

        void OnAfterComplete(object sender, string className);
    }
}