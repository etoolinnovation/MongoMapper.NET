namespace EtoolTech.MongoDB.Mapper.Interfaces
{
    using System.Collections.Generic;

    public interface IChildsManager
    {
        void ManageChilds(object sender);

        void GenerateChilsIds(string objName, IEnumerable<object> list);
    }
}