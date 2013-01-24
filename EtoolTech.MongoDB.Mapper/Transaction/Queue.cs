using System;

namespace EtoolTech.MongoDB.Mapper
{
    public class Queue
    {
        public int Order { get; set; }
        public OperationType OperationType { get; set; }
        public Type Type { get; set; }
        public object Document { get; set; }
        public bool Procesed { get; set; }
        public int Result { get; set; }
    }
}