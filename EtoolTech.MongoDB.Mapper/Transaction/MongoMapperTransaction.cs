using System;
using System.Collections.Generic;
using System.Linq;
using EtoolTech.MongoDB.Mapper.Exceptions;
using EtoolTech.MongoDB.Mapper.Interfaces;
using MongoDB.Driver;

namespace EtoolTech.MongoDB.Mapper
{
    public class MongoMapperTransaction : IMongoMapperTransaction
    {
        internal static bool InTransaction;
        internal static bool Commiting;
        private static readonly List<Queue> TransactionQueue = new List<Queue>();

        public MongoMapperTransaction()
        {
            if (InTransaction) throw new DuplicateTransaction();
            InTransaction = true;
        }

        #region IMongoMapperTransaction Members

        public int QueueLenght
        {
            get { return TransactionQueue.Count; }
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
                        WriteConcernResult result = Writer.Instance.Insert(queue.Type.Name, queue.Type, queue.Document);
                        queue.Procesed = true;
                    }

                    if (queue.OperationType == OperationType.Update)
                    {
                        WriteConcernResult result = Writer.Instance.Update(queue.Type.Name, queue.Type, queue.Document);
                        queue.Procesed = true;
                    }

                    if (queue.OperationType == OperationType.Delete)
                    {
                        WriteConcernResult result = Writer.Instance.Delete(queue.Type.Name, queue.Type, queue.Document);
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

        #endregion

        internal static void AddToQueue(OperationType OperationType, Type T, object Document)
        {
            TransactionQueue.Add(new Queue
                {
                    Order = !TransactionQueue.Any() ? 1 : TransactionQueue.Last().Order + 1,
                    OperationType = OperationType,
                    Document = Document,
                    Type = T
                });
        }
    }
}