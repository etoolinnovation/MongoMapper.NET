using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EtoolTech.MongoDB.Mapper.Interfaces
{
    public interface IMongoMapperVersionable
    {
        long MongoMapperDocumentVersion { get; set; }
        bool IsLastVersion();
        bool IsLastVersion(bool force);
        void FillFromLastVersion();
        void FillFromLastVersion(bool force);
    }
}
