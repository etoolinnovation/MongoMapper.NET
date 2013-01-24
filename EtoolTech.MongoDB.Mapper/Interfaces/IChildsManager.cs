using System.Collections.Generic;

namespace EtoolTech.MongoDB.Mapper.Interfaces
{
    public interface IChildsManager
    {
        #region Public Methods

        void GenerateChilsIds(string ObjName, IEnumerable<object> List);

        void ManageChilds(object Sender);

        #endregion
    }
}