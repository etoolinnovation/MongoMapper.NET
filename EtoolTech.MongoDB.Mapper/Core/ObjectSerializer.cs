namespace EtoolTech.MongoDB.Mapper
{
    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    internal static class ObjectSerializer
    {
        #region Methods

        internal static byte[] ToByteArray(Object obj)
        {
            if (obj == null)
            {
                return null;
            }

            byte[] data;
            using (var ms = new MemoryStream())
            {
                var b = new BinaryFormatter();
                b.Serialize(ms, obj);
                data = ms.ToArray();
                ms.Close();
            }

            return data;
        }

        internal static T ToObjectSerialize<T>(byte[] serializedObject)
        {
            if (serializedObject == null)
            {
                return default(T);
            }

            Object obj;
            using (var ms = new MemoryStream())
            {
                ms.Write(serializedObject, 0, serializedObject.Length);
                ms.Seek(0, 0);
                var b = new BinaryFormatter();
                obj = b.Deserialize(ms);
                ms.Close();
            }
            return (T)obj;
        }

        #endregion
    }
}

