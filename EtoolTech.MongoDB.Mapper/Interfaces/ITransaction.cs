namespace EtoolTech.MongoDB.Mapper.Interfaces
{
    using System.Collections.Generic;

    public interface ITransaction
    {
        #region Public Properties

        List<string> Collections { get; set; }

        #endregion
    }
}