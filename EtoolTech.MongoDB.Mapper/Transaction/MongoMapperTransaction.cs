using EtoolTech.MongoDB.Mapper.Configuration;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace EtoolTech.MongoDB.Mapper
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    using EtoolTech.MongoDB.Mapper.Exceptions;
    using EtoolTech.MongoDB.Mapper.Interfaces;

    public class MongoMapperTransaction: IMongoMapperTransaction
    {
        internal static bool InTransaction;
        internal static bool Commiting;
        private static readonly List<Queue> TransactionQueue = new List<Queue>();

        public int QueueLenght
        {
            get
            {
                return TransactionQueue.Count;
            }
        }

        internal static void AddToQueue(OperationType operationType, Type t, object document)
        {
            TransactionQueue.Add(new Queue
            {
                Order = !TransactionQueue.Any() ? 1 : TransactionQueue.Last().Order + 1,
                OperationType = operationType,
                Document = document,
                Type = t
            });
        }

        public MongoMapperTransaction()
        {
            if (InTransaction) throw new DuplicateTransaction();
            InTransaction = true;
        }

        public void Commit()
        {
            try
            {
                Commiting = true;

                foreach (Queue queue in TransactionQueue.OrderBy(q => q.Order))
                {
                    if (queue.OperationType == OperationType.Insert)
                    {
                        var result = Writer.Instance.Insert(queue.Type.Name, queue.Type, queue.Document);
                        queue.Procesed = true;
                    }

                    if (queue.OperationType == OperationType.Update)
                    {
                        var result = Writer.Instance.Update(queue.Type.Name, queue.Type, queue.Document);
                        queue.Procesed = true;
                    }

                    if (queue.OperationType == OperationType.Delete)
                    {
                        var result = Writer.Instance.Delete(queue.Type.Name, queue.Type, queue.Document);
                        queue.Procesed = true;                        
                        queue.Result = 2;
                    }
                }

            }
            catch (Exception)
            {
                //TODO: Dejarlo todo como estaba.
            }
            finally
            {
                TransactionQueue.Clear();
                Commiting = false;
            }
        }

 
        public void RollBack()
        {
            //TODO: si ya ha empezado dejarlo como al principio. 1 Update, 0 Insert, 2 delete
            TransactionQueue.Clear();
        }
   
        public void Dispose()
        {
            Commiting = false;
            InTransaction = false;
            TransactionQueue.Clear();
        }
    }
}