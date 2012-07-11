using System;

namespace EtoolTech.MongoDB.Mapper.Exceptions
{
    public class DeleteDocumentException : Exception
    {
		private readonly string _message;        

        public DeleteDocumentException(string message)
        {
            _message = message;            
        }

        public override string Message
        {
            get { return String.Format("Error Deleting Document: {0}", _message); }
        }
    }
}