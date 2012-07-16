namespace EtoolTech.MongoDB.Mapper.Interfaces
{
    using System.Xml.Serialization;

    public interface IMongoMapperChildIdeable
    {
        #region Public Properties

        [XmlIgnore]
        long _id { get; set; }

        #endregion
    }
}