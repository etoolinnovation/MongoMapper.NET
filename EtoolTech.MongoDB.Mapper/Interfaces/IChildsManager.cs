namespace EtoolTech.MongoDB.Mapper.Interfaces
{
    using System.Collections.Generic;

    public interface IChildsManager
    {
        #region Public Methods

        void GenerateChilsIds(string objName, IEnumerable<object> list);

        void ManageChilds(object sender);

        #endregion
    }
}