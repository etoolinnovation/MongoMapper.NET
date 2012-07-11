using System;

namespace EtoolTech.MongoDB.Mapper.Exceptions
{
    public class ServerUpdateException : Exception
    {
		private readonly string _message;        

        public ServerUpdateException(string message)
        {
            _message = message;            
        }

        public override string Message
        {
            get { return String.Format("Error Updatind Data: {0}", _message); }
        }
    }
}